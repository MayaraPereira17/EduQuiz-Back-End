using eduQuizApis.Domain.Interfaces;
using eduQuizApis.Infrastructure.Data;
using eduQuizApis.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eduQuizApis.Infrastructure.Repositories
{
    public class AlunoRepository : IAlunoRepository
    {
        private readonly EduQuizContext _context;

        public AlunoRepository(EduQuizContext context)
        {
            _context = context;
        }

        public async Task<bool> UsuarioPodeAcessarQuizAsync(int usuarioId, int quizId)
        {
            var quiz = await _context.Quizzes
                .FirstOrDefaultAsync(q => q.Id == quizId && q.Ativo && q.Publico);

            return quiz != null;
        }

        public async Task<int> ObterTentativasRestantesAsync(int usuarioId, int quizId)
        {
            var quiz = await _context.Quizzes
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null) return 0;

            var tentativasRealizadas = await _context.TentativasQuiz
                .CountAsync(t => t.UsuarioId == usuarioId && t.QuizId == quizId && t.Concluida);

            return Math.Max(0, quiz.MaxTentativas - tentativasRealizadas);
        }

        public async Task<bool> ExisteTentativaEmAndamentoAsync(int usuarioId, int quizId)
        {
            return await _context.TentativasQuiz
                .AnyAsync(t => t.UsuarioId == usuarioId && t.QuizId == quizId && !t.Concluida);
        }

        public async Task<List<TentativasQuiz>> ObterTentativasConcluidasAsync(int usuarioId)
        {
            return await _context.TentativasQuiz
                .Include(t => t.Quiz)
                .ThenInclude(q => q.Categoria)
                .Where(t => t.UsuarioId == usuarioId && t.Concluida)
                .OrderByDescending(t => t.DataConclusao)
                .ToListAsync();
        }

        public async Task<RankingAlunos?> ObterRankingUsuarioAsync(int usuarioId, int categoriaId)
        {
            return await _context.RankingAlunos
                .Include(r => r.Usuario)
                .Include(r => r.Categoria)
                .FirstOrDefaultAsync(r => r.UsuarioId == usuarioId && r.CategoriaId == categoriaId);
        }

        public async Task<List<RankingAlunos>> ObterRankingCompletoAsync(int categoriaId, int? limit = null)
        {
            var query = _context.RankingAlunos
                .Include(r => r.Usuario)
                .Include(r => r.Categoria)
                .Where(r => r.CategoriaId == categoriaId && r.Usuario.IsActive)
                .OrderByDescending(r => r.PontuacaoTotal)
                .ThenByDescending(r => r.MediaPontuacao);

            if (limit.HasValue)
            {
                return await query.Take(limit.Value).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<List<RelatoriosPerformance>> ObterRelatoriosPerformanceAsync(int usuarioId)
        {
            return await _context.RelatoriosPerformance
                .Include(r => r.Quiz)
                .ThenInclude(q => q.Categoria)
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(r => r.DataCriacao)
                .ToListAsync();
        }

        public async Task<List<ConquistasAlunos>> ObterConquistasUsuarioAsync(int usuarioId)
        {
            return await _context.ConquistasAlunos
                .Include(c => c.Conquista)
                .Where(c => c.UsuarioId == usuarioId)
                .OrderByDescending(c => c.DataConquista)
                .ToListAsync();
        }

        public async Task<int> ObterTotalUsuariosAtivosAsync()
        {
            return await _context.Usuarios
                .Where(u => u.IsActive && u.Role.ToString() == "Aluno")
                .CountAsync();
        }

        public async Task<decimal> CalcularMediaGeralUsuarioAsync(int usuarioId)
        {
            var tentativas = await _context.TentativasQuiz
                .Where(t => t.UsuarioId == usuarioId && t.Concluida && t.Pontuacao.HasValue && t.PontuacaoMaxima.HasValue)
                .ToListAsync();

            if (!tentativas.Any()) return 0;

            return tentativas.Average(t => (t.Pontuacao.Value / t.PontuacaoMaxima.Value) * 100);
        }

        public async Task<int> CalcularSequenciaConsecutivaAsync(int usuarioId)
        {
            var datasComQuiz = await _context.TentativasQuiz
                .Where(t => t.UsuarioId == usuarioId && t.Concluida && t.DataConclusao.HasValue)
                .Select(t => t.DataConclusao.Value.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToListAsync();

            if (!datasComQuiz.Any()) return 0;

            int sequencia = 0;
            var dataAtual = DateTime.UtcNow.Date;

            foreach (var data in datasComQuiz)
            {
                if (data == dataAtual.AddDays(-sequencia))
                    sequencia++;
                else
                    break;
            }

            return sequencia;
        }

        public async Task<List<TentativasQuiz>> ObterTentativasRecentesAsync(int usuarioId, int limit = 5)
        {
            return await _context.TentativasQuiz
                .Include(t => t.Quiz)
                .ThenInclude(q => q.Categoria)
                .Where(t => t.UsuarioId == usuarioId && t.Concluida)
                .OrderByDescending(t => t.DataConclusao)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<bool> VerificarConquistasDisponiveisAsync(int usuarioId)
        {
            var conquistas = await _context.Conquistas
                .Where(c => c.Ativo)
                .ToListAsync();

            var conquistasUsuario = await _context.ConquistasAlunos
                .Where(c => c.UsuarioId == usuarioId)
                .Select(c => c.ConquistaId)
                .ToListAsync();

            var conquistasDisponiveis = conquistas
                .Where(c => !conquistasUsuario.Contains(c.Id))
                .ToList();

            foreach (var conquista in conquistasDisponiveis)
            {
                bool criterioAtendido = false;

                switch (conquista.TipoConquista)
                {
                    case "Pontuacao":
                        var pontuacaoTotal = await _context.TentativasQuiz
                            .Where(t => t.UsuarioId == usuarioId && t.Concluida)
                            .SumAsync(t => t.Pontuacao ?? 0);
                        criterioAtendido = pontuacaoTotal >= (decimal)conquista.CriterioMinimo;
                        break;

                    case "Frequencia":
                        var totalQuizzes = await _context.TentativasQuiz
                            .Where(t => t.UsuarioId == usuarioId && t.Concluida)
                            .CountAsync();
                        criterioAtendido = totalQuizzes >= (int)conquista.CriterioMinimo;
                        break;

                    case "Velocidade":
                        var tempoMedio = await _context.TentativasQuiz
                            .Where(t => t.UsuarioId == usuarioId && t.Concluida && t.TempoGasto.HasValue)
                            .AverageAsync(t => t.TempoGasto.Value);
                        criterioAtendido = (decimal)tempoMedio <= conquista.CriterioMinimo;
                        break;

                    case "Precisao":
                        var mediaPrecisao = await _context.RelatoriosPerformance
                            .Where(r => r.UsuarioId == usuarioId)
                            .AverageAsync(r => r.Percentual);
                        criterioAtendido = mediaPrecisao >= (decimal)conquista.CriterioMinimo;
                        break;
                }

                if (criterioAtendido)
                {
                    var novaConquista = new ConquistasAlunos
                    {
                        UsuarioId = usuarioId,
                        ConquistaId = conquista.Id,
                        DataConquista = DateTime.UtcNow
                    };

                    _context.ConquistasAlunos.Add(novaConquista);
                }
            }

            await _context.SaveChangesAsync();
            return conquistasDisponiveis.Any();
        }
    }
}
