using eduQuizApis.Application.DTOs;
using eduQuizApis.Application.Interfaces;
using eduQuizApis.Domain.Interfaces;
using eduQuizApis.Infrastructure.Data;
using eduQuizApis.Domain.Entities;
using eduQuizApis.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace eduQuizApis.Application.Services
{
    public class TecnicoFutebolService : ITecnicoFutebolService
    {
        private readonly IUserRepository _userRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TecnicoFutebolService> _logger;

        public TecnicoFutebolService(IUserRepository userRepository, IServiceProvider serviceProvider, ILogger<TecnicoFutebolService> logger)
        {
            _userRepository = userRepository;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        private EduQuizContext GetContext()
        {
            return _serviceProvider.GetRequiredService<EduQuizContext>();
        }

        // Dashboard do Técnico
        public async Task<DashboardTecnicoDTO> ObterDashboardAsync(int tecnicoId)
        {
            try
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Role != UserRole.TecnicoFutebol)
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                // Obter estatísticas dos alunos
                var context = GetContext();
                var totalAlunos = await context.Usuarios
                    .Where(u => u.Role == UserRole.Aluno && u.IsActive)
                    .CountAsync();

                var tentativasConcluidas = await context.TentativasQuiz
                    .Where(t => t.Concluida)
                    .ToListAsync();

                var mediaGeral = 0.0m;
                if (tentativasConcluidas.Any())
                {
                    try
                    {
                        mediaGeral = tentativasConcluidas.Average(t => 
                        {
                            if (t.Pontuacao.HasValue && t.PontuacaoMaxima.HasValue && t.PontuacaoMaxima.Value > 0)
                                return (decimal)t.Pontuacao.Value / t.PontuacaoMaxima.Value * 100;
                            return 0;
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Erro ao calcular média geral, usando 0");
                        mediaGeral = 0;
                    }
                }

                // Obter melhores alunos do mês
                var melhoresAlunos = await ObterMelhoresAlunosDoMesAsync();

                return new DashboardTecnicoDTO
                {
                    NomeTecnico = $"{tecnico.FirstName} {tecnico.LastName}",
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
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Role != UserRole.TecnicoFutebol)
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var context = GetContext();
                var query = context.Usuarios
                    .Where(u => u.Role == UserRole.Aluno && u.IsActive);

                if (!string.IsNullOrEmpty(busca))
                {
                    query = query.Where(u => 
                        u.FirstName.Contains(busca) || 
                        u.LastName.Contains(busca) || 
                        u.Email.Contains(busca));
                }

                // Simplificar consulta para evitar problemas com subconsultas
                var usuarios = await query.ToListAsync();
                var alunos = new List<AlunoRankingDTO>();

                foreach (var usuario in usuarios)
                {
                    var tentativas = await context.TentativasQuiz
                        .Where(t => t.UsuarioId == usuario.Id && t.Concluida)
                        .ToListAsync();

                    var totalQuizzes = tentativas.Count;
                    var scoreGeral = tentativas.Any() ? 
                        tentativas.Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100) : 0;
                    
                    var ultimoQuiz = tentativas
                        .OrderByDescending(t => t.DataConclusao)
                        .FirstOrDefault()?.DataConclusao;

                    alunos.Add(new AlunoRankingDTO
                    {
                        Id = usuario.Id,
                        Nome = $"{usuario.FirstName} {usuario.LastName}",
                        Email = usuario.Email,
                        Idade = CalcularIdade(usuario.DataNascimento),
                        TotalQuizzes = totalQuizzes,
                        ScoreGeral = Math.Round(scoreGeral, 1),
                        UltimoQuiz = ultimoQuiz
                    });
                }

                // Ordenar por score geral
                alunos = alunos.OrderByDescending(a => a.ScoreGeral).ToList();

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
                if (tecnico == null || tecnico.Role != UserRole.TecnicoFutebol)
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var context = GetContext();
                var usuarios = await context.Usuarios
                    .Where(u => u.Role == UserRole.Aluno && u.IsActive)
                    .ToListAsync();

                var alunos = new List<DesempenhoAlunoDTO>();

                foreach (var usuario in usuarios)
                {
                    var tentativas = await context.TentativasQuiz
                        .Where(t => t.UsuarioId == usuario.Id && t.Concluida)
                        .ToListAsync();

                    var totalQuizzes = tentativas.Count;
                    var scoreGeral = tentativas.Any() ? 
                        tentativas.Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100) : 0;
                    
                    var ultimoQuiz = tentativas
                        .OrderByDescending(t => t.DataConclusao)
                        .FirstOrDefault()?.DataConclusao;

                    alunos.Add(new DesempenhoAlunoDTO
                    {
                        Id = usuario.Id,
                        Nome = $"{usuario.FirstName} {usuario.LastName}",
                        TotalQuizzes = totalQuizzes,
                        UltimoQuiz = ultimoQuiz,
                        ScoreGeral = Math.Round(scoreGeral, 1)
                    });
                }

                // Ordenar por score geral
                alunos = alunos.OrderByDescending(a => a.ScoreGeral).ToList();

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
                if (tecnico == null || tecnico.Role != UserRole.TecnicoFutebol)
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var context = GetContext();
                var totalAlunos = await context.Usuarios
                    .Where(u => u.Role == UserRole.Aluno && u.IsActive)
                    .CountAsync();

                var mediaTurma = await CalcularMediaTurmaAsync();

                return new PerfilTecnicoDTO
                {
                    Id = tecnico.Id,
                    Nome = tecnico.FirstName,
                    Sobrenome = tecnico.LastName,
                    Email = tecnico.Email,
                    Funcao = tecnico.Role.ToString(),
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
                if (tecnico == null || tecnico.Role != UserRole.TecnicoFutebol)
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var context = GetContext();
                tecnico.FirstName = request.Nome;
                tecnico.LastName = request.Sobrenome;
                tecnico.Email = request.Email;
                tecnico.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync();

                return new PerfilTecnicoDTO
                {
                    Id = tecnico.Id,
                    Nome = tecnico.FirstName,
                    Sobrenome = tecnico.LastName,
                    Email = tecnico.Email,
                    Funcao = tecnico.Role.ToString(),
                    Instituicao = "Instituição de Ensino",
                    TotalAlunos = await context.Usuarios
                        .Where(u => u.Role == UserRole.Aluno && u.IsActive)
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
            try
            {
                var context = GetContext();
                var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var fimMes = inicioMes.AddMonths(1);

                // Simplificar a consulta para evitar problemas de relacionamentos
                var tentativasDoMes = await context.TentativasQuiz
                    .Where(t => t.Concluida && t.DataConclusao >= inicioMes && t.DataConclusao < fimMes)
                    .ToListAsync();

                if (!tentativasDoMes.Any())
                {
                    return new List<MelhorAlunoDTO>();
                }

                var alunosAgrupados = tentativasDoMes
                    .GroupBy(t => t.UsuarioId)
                    .Select(g => new
                    {
                        UsuarioId = g.Key,
                        TotalQuizzes = g.Count(),
                        MediaPontuacao = g.Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100)
                    })
                    .OrderByDescending(x => x.MediaPontuacao)
                    .ThenByDescending(x => x.TotalQuizzes)
                    .Take(3)
                    .ToList();

                var melhoresAlunos = new List<MelhorAlunoDTO>();
                for (int i = 0; i < alunosAgrupados.Count; i++)
                {
                    var aluno = alunosAgrupados[i];
                    var usuario = await context.Usuarios
                        .Where(u => u.Id == aluno.UsuarioId)
                        .FirstOrDefaultAsync();

                    if (usuario != null)
                    {
                        melhoresAlunos.Add(new MelhorAlunoDTO
                        {
                            Posicao = i + 1,
                            Nome = $"{usuario.FirstName} {usuario.LastName}",
                            Sequencia = CalcularSequencia(aluno.UsuarioId),
                            Performance = Math.Round(aluno.MediaPontuacao, 1),
                            TotalQuizzes = aluno.TotalQuizzes
                        });
                    }
                }

                return melhoresAlunos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter melhores alunos do mês");
                return new List<MelhorAlunoDTO>();
            }
        }

        private async Task<decimal> CalcularMediaTurmaAsync()
        {
            var context = GetContext();
            var tentativasConcluidas = await context.TentativasQuiz
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
