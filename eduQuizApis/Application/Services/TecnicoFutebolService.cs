using eduQuizApis.Application.DTOs;
using eduQuizApis.Domain.Interfaces;
using eduQuizApis.Infrastructure.Data;
using eduQuizApis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eduQuizApis.Application.Services
{
    public class TecnicoFutebolService : ITecnicoFutebolService
    {
        private readonly IUserRepository _userRepository;
        private readonly EduQuizContext _context;
        private readonly ILogger<TecnicoFutebolService> _logger;

        public TecnicoFutebolService(IUserRepository userRepository, EduQuizContext context, ILogger<TecnicoFutebolService> logger)
        {
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }

        // Dashboard do Técnico
        public async Task<DashboardTecnicoDTO> ObterDashboardAsync(int tecnicoId)
        {
            try
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Funcao != "TecnicoFutebol")
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                // Obter estatísticas dos alunos
                var totalAlunos = await _context.Usuarios
                    .Where(u => u.Funcao == "Aluno" && u.Ativo)
                    .CountAsync();

                var tentativasConcluidas = await _context.TentativasQuiz
                    .Where(t => t.Concluida)
                    .ToListAsync();

                var mediaGeral = tentativasConcluidas.Any() ? 
                    tentativasConcluidas.Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100) : 0;

                // Obter melhores alunos do mês
                var melhoresAlunos = await ObterMelhoresAlunosDoMesAsync();

                return new DashboardTecnicoDTO
                {
                    NomeTecnico = $"{tecnico.Nome} {tecnico.Sobrenome}",
                    EmailTecnico = tecnico.Email,
                    TotalAlunos = totalAlunos,
                    PerformanceGeral = Math.Round(mediaGeral, 1),
                    MelhoresAlunos = melhoresAlunos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dashboard do técnico");
                throw;
            }
        }

        // Gerenciar Alunos e Ranking
        public async Task<GerenciarAlunosDTO> ObterGerenciarAlunosAsync(int tecnicoId, string? busca = null)
        {
            try
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Funcao != "TecnicoFutebol")
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var query = _context.Usuarios
                    .Where(u => u.Funcao == "Aluno" && u.Ativo);

                if (!string.IsNullOrEmpty(busca))
                {
                    query = query.Where(u => 
                        u.Nome.Contains(busca) || 
                        u.Sobrenome.Contains(busca) || 
                        u.Email.Contains(busca));
                }

                var alunos = await query
                    .Select(u => new AlunoRankingDTO
                    {
                        Id = u.Id,
                        Nome = $"{u.Nome} {u.Sobrenome}",
                        Email = u.Email,
                        Idade = CalcularIdade(u.DataNascimento),
                        TotalQuizzes = _context.TentativasQuiz
                            .Where(t => t.UsuarioId == u.Id && t.Concluida)
                            .Count(),
                        ScoreGeral = _context.TentativasQuiz
                            .Where(t => t.UsuarioId == u.Id && t.Concluida)
                            .Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100),
                        UltimoQuiz = _context.TentativasQuiz
                            .Where(t => t.UsuarioId == u.Id && t.Concluida)
                            .OrderByDescending(t => t.DataConclusao)
                            .Select(t => t.DataConclusao)
                            .FirstOrDefault()
                    })
                    .OrderByDescending(a => a.ScoreGeral)
                    .ToListAsync();

                // Adicionar posição no ranking
                for (int i = 0; i < alunos.Count; i++)
                {
                    alunos[i].Posicao = i + 1;
                }

                return new GerenciarAlunosDTO
                {
                    Alunos = alunos,
                    TotalAlunos = alunos.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter gerenciamento de alunos");
                throw;
            }
        }

        // Relatório de Desempenho
        public async Task<RelatorioDesempenhoDTO> ObterRelatorioDesempenhoAsync(int tecnicoId)
        {
            try
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Funcao != "TecnicoFutebol")
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var alunos = await _context.Usuarios
                    .Where(u => u.Funcao == "Aluno" && u.Ativo)
                    .Select(u => new DesempenhoAlunoDTO
                    {
                        Id = u.Id,
                        Nome = $"{u.Nome} {u.Sobrenome}",
                        TotalQuizzes = _context.TentativasQuiz
                            .Where(t => t.UsuarioId == u.Id && t.Concluida)
                            .Count(),
                        UltimoQuiz = _context.TentativasQuiz
                            .Where(t => t.UsuarioId == u.Id && t.Concluida)
                            .OrderByDescending(t => t.DataConclusao)
                            .Select(t => t.DataConclusao)
                            .FirstOrDefault(),
                        ScoreGeral = _context.TentativasQuiz
                            .Where(t => t.UsuarioId == u.Id && t.Concluida)
                            .Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100)
                    })
                    .OrderByDescending(a => a.ScoreGeral)
                    .ToListAsync();

                return new RelatorioDesempenhoDTO
                {
                    Alunos = alunos,
                    TotalAlunos = alunos.Count,
                    MediaGeral = alunos.Any() ? alunos.Average(a => a.ScoreGeral) : 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter relatório de desempenho");
                throw;
            }
        }

        // Perfil do Técnico
        public async Task<PerfilTecnicoDTO> ObterPerfilAsync(int tecnicoId)
        {
            try
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Funcao != "TecnicoFutebol")
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var totalAlunos = await _context.Usuarios
                    .Where(u => u.Funcao == "Aluno" && u.Ativo)
                    .CountAsync();

                var mediaTurma = await CalcularMediaTurmaAsync();

                return new PerfilTecnicoDTO
                {
                    Id = tecnico.Id,
                    Nome = tecnico.Nome,
                    Sobrenome = tecnico.Sobrenome,
                    Email = tecnico.Email,
                    Funcao = tecnico.Funcao,
                    Instituicao = "Instituição de Ensino", // Pode ser expandido futuramente
                    TotalAlunos = totalAlunos,
                    MediaTurma = Math.Round(mediaTurma, 1)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter perfil do técnico");
                throw;
            }
        }

        public async Task<PerfilTecnicoDTO> AtualizarPerfilAsync(int tecnicoId, AtualizarPerfilTecnicoRequestDTO request)
        {
            try
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Funcao != "TecnicoFutebol")
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                tecnico.Nome = request.Nome;
                tecnico.Sobrenome = request.Sobrenome;
                tecnico.Email = request.Email;
                tecnico.DataAtualizacao = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new PerfilTecnicoDTO
                {
                    Id = tecnico.Id,
                    Nome = tecnico.Nome,
                    Sobrenome = tecnico.Sobrenome,
                    Email = tecnico.Email,
                    Funcao = tecnico.Funcao,
                    Instituicao = "Instituição de Ensino",
                    TotalAlunos = await _context.Usuarios
                        .Where(u => u.Funcao == "Aluno" && u.Ativo)
                        .CountAsync(),
                    MediaTurma = await CalcularMediaTurmaAsync()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar perfil do técnico");
                throw;
            }
        }

        // Métodos auxiliares
        private async Task<List<MelhorAlunoDTO>> ObterMelhoresAlunosDoMesAsync()
        {
            var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fimMes = inicioMes.AddMonths(1);

            var melhoresAlunos = await _context.TentativasQuiz
                .Where(t => t.Concluida && t.DataConclusao >= inicioMes && t.DataConclusao < fimMes)
                .GroupBy(t => t.UsuarioId)
                .Select(g => new
                {
                    UsuarioId = g.Key,
                    Usuario = g.First().Usuario,
                    TotalQuizzes = g.Count(),
                    MediaPontuacao = g.Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100),
                    Sequencia = CalcularSequencia(g.Key)
                })
                .OrderByDescending(x => x.MediaPontuacao)
                .ThenByDescending(x => x.TotalQuizzes)
                .Take(3)
                .ToListAsync();

            return melhoresAlunos.Select((aluno, index) => new MelhorAlunoDTO
            {
                Posicao = index + 1,
                Nome = $"{aluno.Usuario.Nome} {aluno.Usuario.Sobrenome}",
                Sequencia = aluno.Sequencia,
                Performance = Math.Round(aluno.MediaPontuacao, 1),
                TotalQuizzes = aluno.TotalQuizzes
            }).ToList();
        }

        private async Task<decimal> CalcularMediaTurmaAsync()
        {
            var tentativasConcluidas = await _context.TentativasQuiz
                .Where(t => t.Concluida)
                .ToListAsync();

            return tentativasConcluidas.Any() ? 
                tentativasConcluidas.Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100) : 0;
        }

        private int CalcularIdade(DateTime? dataNascimento)
        {
            if (!dataNascimento.HasValue)
                return 0;

            var hoje = DateTime.Today;
            var idade = hoje.Year - dataNascimento.Value.Year;
            if (dataNascimento.Value.Date > hoje.AddYears(-idade))
                idade--;

            return idade;
        }

        private int CalcularSequencia(int usuarioId)
        {
            // Implementar lógica de sequência de dias consecutivos
            // Por enquanto, retornar valor aleatório para demonstração
            return new Random().Next(5, 20);
        }
    }
}
