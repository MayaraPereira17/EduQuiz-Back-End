using eduQuizApis.Domain.Interfaces;
using eduQuizApis.Infrastructure.Data;
using eduQuizApis.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eduQuizApis.Infrastructure.Repositories
{
    public class ProfessorRepository : IProfessorRepository
    {
        private readonly EduQuizContext _context;

        public ProfessorRepository(EduQuizContext context)
        {
            _context = context;
        }

        // Quizzes
        public async Task<List<Quizzes>> ObterQuizzesPorProfessorAsync(int professorId)
        {
            return await _context.Quizzes
                .Include(q => q.Categoria)
                .Include(q => q.Questoes)
                .Where(q => q.CriadoPor == professorId)
                .OrderByDescending(q => q.DataCriacao)
                .ToListAsync();
        }

        public async Task<Quizzes?> ObterQuizPorIdEProfessorAsync(int quizId, int professorId)
        {
            return await _context.Quizzes
                .Include(q => q.Categoria)
                .Include(q => q.Questoes.Where(quest => quest.Ativo).OrderBy(quest => quest.OrdemIndice))
                .ThenInclude(quest => quest.Opcoes.OrderBy(op => op.OrdemIndice))
                .FirstOrDefaultAsync(q => q.Id == quizId && q.CriadoPor == professorId);
        }

        public async Task<bool> ProfessorPodeEditarQuizAsync(int professorId, int quizId)
        {
            return await _context.Quizzes
                .AnyAsync(q => q.Id == quizId && q.CriadoPor == professorId);
        }

        public async Task<bool> QuizTemTentativasAsync(int quizId)
        {
            return await _context.TentativasQuiz
                .AnyAsync(t => t.QuizId == quizId);
        }

        // Estatísticas
        public async Task<int> ContarQuizzesCriadosAsync(int professorId)
        {
            return await _context.Quizzes
                .CountAsync(q => q.CriadoPor == professorId);
        }

        public async Task<int> ContarQuizzesPublicadosAsync(int professorId)
        {
            return await _context.Quizzes
                .CountAsync(q => q.CriadoPor == professorId && q.Publico);
        }

        public async Task<decimal> CalcularMediaAlunosQuizAsync(int quizId)
        {
            var tentativas = await _context.TentativasQuiz
                .Where(t => t.QuizId == quizId && t.Concluida && 
                           t.Pontuacao.HasValue && t.PontuacaoMaxima.HasValue)
                .ToListAsync();

            if (!tentativas.Any()) return 0;

            return tentativas.Average(t => (t.Pontuacao.Value / t.PontuacaoMaxima.Value) * 100);
        }

        public async Task<List<TentativasQuiz>> ObterTentativasPorQuizAsync(int quizId)
        {
            return await _context.TentativasQuiz
                .Include(t => t.Usuario)
                .Where(t => t.QuizId == quizId && t.Concluida)
                .OrderByDescending(t => t.DataConclusao)
                .ToListAsync();
        }

        public async Task<List<TentativasQuiz>> ObterTentativasPorProfessorAsync(int professorId)
        {
            return await _context.TentativasQuiz
                .Include(t => t.Usuario)
                .Include(t => t.Quiz)
                .Where(t => t.Quiz.CriadoPor == professorId && t.Concluida)
                .OrderByDescending(t => t.DataConclusao)
                .ToListAsync();
        }

        // Categorias
        public async Task<List<Categorias>> ObterCategoriasAtivasAsync()
        {
            return await _context.Categorias
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<Categorias?> ObterCategoriaPorIdAsync(int categoriaId)
        {
            return await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == categoriaId && c.Ativo);
        }

        // Questões e Opções
        public async Task<List<Questoes>> ObterQuestoesPorQuizAsync(int quizId)
        {
            return await _context.Questoes
                .Include(q => q.Opcoes)
                .Where(q => q.QuizId == quizId && q.Ativo)
                .OrderBy(q => q.OrdemIndice)
                .ToListAsync();
        }

        public async Task<List<OpcoesQuestao>> ObterOpcoesPorQuestaoAsync(int questaoId)
        {
            return await _context.OpcoesQuestao
                .Where(o => o.QuestaoId == questaoId)
                .OrderBy(o => o.OrdemIndice)
                .ToListAsync();
        }

        public async Task<bool> QuestaoPertenceAoQuizAsync(int questaoId, int quizId)
        {
            return await _context.Questoes
                .AnyAsync(q => q.Id == questaoId && q.QuizId == quizId);
        }

        // Relatórios
        public async Task<List<RelatoriosPerformance>> ObterRelatoriosPorQuizAsync(int quizId)
        {
            return await _context.RelatoriosPerformance
                .Include(r => r.Usuario)
                .Where(r => r.QuizId == quizId)
                .OrderByDescending(r => r.DataCriacao)
                .ToListAsync();
        }

        public async Task<List<RelatoriosPerformance>> ObterRelatoriosPorProfessorAsync(int professorId)
        {
            return await _context.RelatoriosPerformance
                .Include(r => r.Usuario)
                .Include(r => r.Quiz)
                .Where(r => r.Quiz.CriadoPor == professorId)
                .OrderByDescending(r => r.DataCriacao)
                .ToListAsync();
        }

        // Validações
        public async Task<bool> UsuarioEPossuiQuizAsync(int professorId, int quizId)
        {
            return await _context.Quizzes
                .AnyAsync(q => q.Id == quizId && q.CriadoPor == professorId);
        }

        public async Task<bool> CategoriaExisteEAtivaAsync(int categoriaId)
        {
            return await _context.Categorias
                .AnyAsync(c => c.Id == categoriaId && c.Ativo);
        }
    }
}
