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
                        u.Email.Contains(busca) ||
                        u.Username.Contains(busca));
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
                        Username = usuario.Username,
                        Nome = $"{usuario.FirstName} {usuario.LastName}",
                        Email = usuario.Email,
                        CPF = usuario.CPF,
                        Idade = CalcularIdade(usuario.DataNascimento),
                        DataNascimento = usuario.DataNascimento,
                        AvatarUrl = usuario.AvatarUrl,
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

        // Atualizar Aluno
        public async Task<AlunoRankingDTO> AtualizarAlunoAsync(int tecnicoId, int alunoId, AtualizarAlunoRequestDTO request)
        {
            try
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Role != UserRole.TecnicoFutebol)
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var context = GetContext();
                var aluno = await context.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == alunoId && u.Role == UserRole.Aluno);

                if (aluno == null)
                    throw new ArgumentException("Aluno não encontrado");

                // Validar e atualizar username
                if (!string.IsNullOrWhiteSpace(request.Username))
                {
                    var username = request.Username.Trim();
                    if (username.Length > 50)
                    {
                        throw new ArgumentException("Username deve ter no máximo 50 caracteres");
                    }

                    // Verificar se username já está em uso por outro usuário
                    if (await _userRepository.UsernameExistsAsync(username, alunoId))
                    {
                        throw new ArgumentException("Username já está em uso por outro usuário");
                    }

                    aluno.Username = username;
                }

                // Validar e atualizar nome
                if (!string.IsNullOrWhiteSpace(request.Nome))
                {
                    var partesNome = request.Nome.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (partesNome.Length > 0)
                    {
                        aluno.FirstName = partesNome[0];
                        aluno.LastName = partesNome.Length > 1 
                            ? string.Join(" ", partesNome.Skip(1)) 
                            : string.Empty;
                    }
                    else
                    {
                        throw new ArgumentException("Nome não pode estar vazio");
                    }
                }

                // Validar e atualizar email
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    // Validar formato de email
                    if (!System.Text.RegularExpressions.Regex.IsMatch(request.Email, 
                        @"^[^@\s]+@[^@\s]+\.[^@\s]+$", 
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        throw new ArgumentException("Email inválido");
                    }

                    if (request.Email.Length > 100)
                    {
                        throw new ArgumentException("Email deve ter no máximo 100 caracteres");
                    }

                    // Verificar se email já está em uso por outro usuário
                    if (await _userRepository.EmailExistsAsync(request.Email, alunoId))
                    {
                        throw new ArgumentException("Email já está em uso por outro usuário");
                    }

                    aluno.Email = request.Email;
                }

                // Validar e atualizar CPF
                if (request.CPF != null)
                {
                    var cpf = request.CPF.Trim();
                    if (string.IsNullOrEmpty(cpf))
                    {
                        aluno.CPF = null; // Permite remover CPF
                    }
                    else
                    {
                        if (cpf.Length > 14)
                        {
                            throw new ArgumentException("CPF deve ter no máximo 14 caracteres");
                        }
                        aluno.CPF = cpf;
                    }
                }

                // Validar e atualizar data de nascimento
                if (request.DataNascimento.HasValue)
                {
                    aluno.DataNascimento = request.DataNascimento.Value;
                }
                else if (request.Idade.HasValue)
                {
                    // Compatibilidade: se idade for fornecida, converter para data de nascimento
                    if (request.Idade.Value <= 0)
                    {
                        throw new ArgumentException("Idade deve ser um número positivo");
                    }

                    // Calcular data de nascimento aproximada (assumindo aniversário já ocorreu este ano)
                    var dataNascimento = DateTime.UtcNow.AddYears(-request.Idade.Value);
                    aluno.DataNascimento = dataNascimento;
                }

                // Validar e atualizar avatar URL
                if (request.AvatarUrl != null)
                {
                    var avatarUrl = request.AvatarUrl.Trim();
                    if (string.IsNullOrEmpty(avatarUrl))
                    {
                        aluno.AvatarUrl = null; // Permite remover avatar
                    }
                    else
                    {
                        if (avatarUrl.Length > 500)
                        {
                            throw new ArgumentException("Avatar URL deve ter no máximo 500 caracteres");
                        }
                        aluno.AvatarUrl = avatarUrl;
                    }
                }

                aluno.UpdatedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();

                // Buscar estatísticas atualizadas do aluno
                var tentativas = await context.TentativasQuiz
                    .Where(t => t.UsuarioId == alunoId && t.Concluida)
                    .ToListAsync();

                var totalQuizzes = tentativas.Count;
                var scoreGeral = tentativas.Any() ? 
                    tentativas.Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100) : 0;
                
                var ultimoQuiz = tentativas
                    .OrderByDescending(t => t.DataConclusao)
                    .FirstOrDefault()?.DataConclusao;

                // Calcular posição no ranking
                var todosAlunos = await context.Usuarios
                    .Where(u => u.Role == UserRole.Aluno && u.IsActive)
                    .ToListAsync();

                var alunosComScore = new List<(int Id, decimal Score)>();
                foreach (var u in todosAlunos)
                {
                    var tentativasAluno = await context.TentativasQuiz
                        .Where(t => t.UsuarioId == u.Id && t.Concluida)
                        .ToListAsync();
                    
                    var score = tentativasAluno.Any() ? 
                        tentativasAluno.Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100) : 0;
                    
                    alunosComScore.Add((u.Id, score));
                }

                var posicao = alunosComScore
                    .OrderByDescending(a => a.Score)
                    .ToList()
                    .FindIndex(a => a.Id == alunoId) + 1;

                return new AlunoRankingDTO
                {
                    Id = aluno.Id,
                    Posicao = posicao,
                    Username = aluno.Username,
                    Nome = $"{aluno.FirstName} {aluno.LastName}".Trim(),
                    Email = aluno.Email,
                    CPF = aluno.CPF,
                    Idade = CalcularIdade(aluno.DataNascimento),
                    DataNascimento = aluno.DataNascimento,
                    AvatarUrl = aluno.AvatarUrl,
                    TotalQuizzes = totalQuizzes,
                    ScoreGeral = Math.Round(scoreGeral, 1),
                    UltimoQuiz = ultimoQuiz
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar aluno");
                throw new Exception("Erro ao atualizar aluno. Tente novamente mais tarde.");
            }
        }

        // Excluir Aluno (Soft Delete)
        public async Task<ExcluirAlunoResponseDTO> ExcluirAlunoAsync(int tecnicoId, int alunoId)
        {
            try
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Role != UserRole.TecnicoFutebol)
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var context = GetContext();
                var aluno = await context.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == alunoId && u.Role == UserRole.Aluno);

                if (aluno == null)
                    throw new ArgumentException("Aluno não encontrado");

                // Soft delete: marcar como inativo
                aluno.IsActive = false;
                aluno.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync();

                _logger.LogInformation("Aluno {AlunoId} excluído (soft delete) pelo técnico {TecnicoId}", alunoId, tecnicoId);

                return new ExcluirAlunoResponseDTO
                {
                    Message = "Aluno excluído com sucesso",
                    AlunoId = alunoId
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir aluno");
                throw new Exception("Erro ao excluir aluno. Tente novamente mais tarde.");
            }
        }

        // Listar Professores
        public async Task<GerenciarProfessoresDTO> ObterProfessoresAsync(int tecnicoId, string? busca = null)
        {
            try
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Role != UserRole.TecnicoFutebol)
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var context = GetContext();
                var query = context.Usuarios
                    .Where(u => u.Role == UserRole.Professor && u.IsActive);

                if (!string.IsNullOrEmpty(busca))
                {
                    query = query.Where(u => 
                        u.FirstName.Contains(busca) || 
                        u.LastName.Contains(busca) || 
                        u.Email.Contains(busca) ||
                        u.Username.Contains(busca));
                }

                var usuarios = await query.ToListAsync();
                var professores = new List<ProfessorDTO>();

                foreach (var usuario in usuarios)
                {
                    var totalQuizzes = await context.Quizzes
                        .Where(q => q.CriadoPor == usuario.Id)
                        .CountAsync();

                    professores.Add(new ProfessorDTO
                    {
                        Id = usuario.Id,
                        Username = usuario.Username,
                        Nome = $"{usuario.FirstName} {usuario.LastName}".Trim(),
                        Email = usuario.Email,
                        CPF = usuario.CPF,
                        DataNascimento = usuario.DataNascimento,
                        AvatarUrl = usuario.AvatarUrl,
                        Instituicao = null, // Campo não existe no banco
                        AreaEspecializacao = null, // Campo não existe no banco
                        TotalQuizzes = totalQuizzes,
                        DataCadastro = usuario.CreatedAt
                    });
                }

                return new GerenciarProfessoresDTO
                {
                    Professores = professores,
                    TotalProfessores = professores.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter lista de professores");
                throw;
            }
        }

        // Atualizar Professor
        public async Task<ProfessorDTO> AtualizarProfessorAsync(int tecnicoId, int professorId, AtualizarProfessorRequestDTO request)
        {
            try
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Role != UserRole.TecnicoFutebol)
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var context = GetContext();
                var professor = await context.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == professorId && u.Role == UserRole.Professor);

                if (professor == null)
                    throw new ArgumentException("Professor não encontrado");

                // Validar e atualizar username
                if (!string.IsNullOrWhiteSpace(request.Username))
                {
                    var username = request.Username.Trim();
                    if (username.Length > 50)
                    {
                        throw new ArgumentException("Username deve ter no máximo 50 caracteres");
                    }

                    // Verificar se username já está em uso por outro usuário
                    if (await _userRepository.UsernameExistsAsync(username, professorId))
                    {
                        throw new ArgumentException("Username já está em uso por outro usuário");
                    }

                    professor.Username = username;
                }

                // Validar e atualizar nome
                if (!string.IsNullOrWhiteSpace(request.Nome))
                {
                    var partesNome = request.Nome.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (partesNome.Length > 0)
                    {
                        professor.FirstName = partesNome[0];
                        professor.LastName = partesNome.Length > 1 
                            ? string.Join(" ", partesNome.Skip(1)) 
                            : string.Empty;
                    }
                    else
                    {
                        throw new ArgumentException("Nome não pode estar vazio");
                    }
                }

                // Validar e atualizar email
                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    // Validar formato de email
                    if (!System.Text.RegularExpressions.Regex.IsMatch(request.Email, 
                        @"^[^@\s]+@[^@\s]+\.[^@\s]+$", 
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        throw new ArgumentException("Email inválido");
                    }

                    if (request.Email.Length > 100)
                    {
                        throw new ArgumentException("Email deve ter no máximo 100 caracteres");
                    }

                    // Verificar se email já está em uso por outro usuário
                    if (await _userRepository.EmailExistsAsync(request.Email, professorId))
                    {
                        throw new ArgumentException("Email já está em uso por outro usuário");
                    }

                    professor.Email = request.Email;
                }

                // Validar e atualizar CPF
                if (request.CPF != null)
                {
                    var cpf = request.CPF.Trim();
                    if (string.IsNullOrEmpty(cpf))
                    {
                        professor.CPF = null; // Permite remover CPF
                    }
                    else
                    {
                        if (cpf.Length > 14)
                        {
                            throw new ArgumentException("CPF deve ter no máximo 14 caracteres");
                        }
                        professor.CPF = cpf;
                    }
                }

                // Validar e atualizar data de nascimento
                if (request.DataNascimento.HasValue)
                {
                    professor.DataNascimento = request.DataNascimento.Value;
                }

                // Validar e atualizar avatar URL
                if (request.AvatarUrl != null)
                {
                    var avatarUrl = request.AvatarUrl.Trim();
                    if (string.IsNullOrEmpty(avatarUrl))
                    {
                        professor.AvatarUrl = null; // Permite remover avatar
                    }
                    else
                    {
                        if (avatarUrl.Length > 500)
                        {
                            throw new ArgumentException("Avatar URL deve ter no máximo 500 caracteres");
                        }
                        professor.AvatarUrl = avatarUrl;
                    }
                }

                // Nota: Instituicao e AreaEspecializacao não existem no banco de dados
                // Esses campos são apenas retornados como null no response
                // Se no futuro forem adicionados ao banco, devem ser atualizados aqui

                professor.UpdatedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();

                // Buscar estatísticas atualizadas do professor
                var totalQuizzes = await context.Quizzes
                    .Where(q => q.CriadoPor == professorId)
                    .CountAsync();

                return new ProfessorDTO
                {
                    Id = professor.Id,
                    Username = professor.Username,
                    Nome = $"{professor.FirstName} {professor.LastName}".Trim(),
                    Email = professor.Email,
                    CPF = professor.CPF,
                    DataNascimento = professor.DataNascimento,
                    AvatarUrl = professor.AvatarUrl,
                    Instituicao = request.Instituicao, // Retorna o valor enviado, mas não salva no banco
                    AreaEspecializacao = request.AreaEspecializacao, // Retorna o valor enviado, mas não salva no banco
                    TotalQuizzes = totalQuizzes,
                    DataCadastro = professor.CreatedAt
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar professor");
                throw new Exception("Erro ao atualizar professor. Tente novamente mais tarde.");
            }
        }

        // Excluir Professor (Soft Delete)
        public async Task<ExcluirProfessorResponseDTO> ExcluirProfessorAsync(int tecnicoId, int professorId)
        {
            try
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Role != UserRole.TecnicoFutebol)
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var context = GetContext();
                var professor = await context.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == professorId && u.Role == UserRole.Professor);

                if (professor == null)
                    throw new ArgumentException("Professor não encontrado");

                // Verificar se professor tem quizzes criados (opcional - pode bloquear exclusão)
                var temQuizzes = await context.Quizzes
                    .AnyAsync(q => q.CriadoPor == professorId);

                // Soft delete: marcar como inativo
                professor.IsActive = false;
                professor.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync();

                _logger.LogInformation("Professor {ProfessorId} excluído (soft delete) pelo técnico {TecnicoId}. Tinha quizzes: {TemQuizzes}", 
                    professorId, tecnicoId, temQuizzes);

                return new ExcluirProfessorResponseDTO
                {
                    Message = "Professor excluído com sucesso",
                    ProfessorId = professorId
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir professor");
                throw new Exception("Erro ao excluir professor. Tente novamente mais tarde.");
            }
        }

        // Gerenciar Times

        // Listar Times
        public async Task<GerenciarTimesDTO> ObterTimesAsync(int tecnicoId)
        {
            try
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Role != UserRole.TecnicoFutebol)
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var context = GetContext();
                var times = await context.Times
                    .Include(t => t.Jogadores)
                    .ThenInclude(j => j.Aluno)
                    .Where(t => t.TecnicoId == tecnicoId && t.IsActive)
                    .OrderByDescending(t => t.DataCriacao)
                    .ToListAsync();

                // Otimização: calcular scores de todos os alunos de uma vez
                var todosAlunos = await context.Usuarios
                    .Where(u => u.Role == UserRole.Aluno && u.IsActive)
                    .ToListAsync();

                var todasTentativas = await context.TentativasQuiz
                    .Where(t => t.Concluida)
                    .ToListAsync();

                // Criar dicionário de scores por aluno
                var scoresPorAluno = todasTentativas
                    .GroupBy(t => t.UsuarioId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100)
                    );

                // Calcular posições no ranking
                var alunosComScore = todosAlunos
                    .Select(u => new
                    {
                        Id = u.Id,
                        Score = scoresPorAluno.ContainsKey(u.Id) ? scoresPorAluno[u.Id] : 0
                    })
                    .OrderByDescending(a => a.Score)
                    .ToList();

                var posicoesPorAluno = alunosComScore
                    .Select((a, index) => new { a.Id, Posicao = index + 1 })
                    .ToDictionary(a => a.Id, a => a.Posicao);

                var timesDTO = new List<TimeDTO>();

                foreach (var time in times)
                {
                    var jogadoresDTO = new List<JogadorTimeDTO>();
                    
                    foreach (var jogador in time.Jogadores.OrderBy(j => j.DataEscalacao))
                    {
                        var aluno = jogador.Aluno;
                        var scoreGeral = scoresPorAluno.ContainsKey(aluno.Id) ? scoresPorAluno[aluno.Id] : 0;
                        var posicao = posicoesPorAluno.ContainsKey(aluno.Id) ? posicoesPorAluno[aluno.Id] : 0;

                        jogadoresDTO.Add(new JogadorTimeDTO
                        {
                            Id = jogador.Id,
                            AlunoId = aluno.Id,
                            Nome = $"{aluno.FirstName} {aluno.LastName}".Trim(),
                            Email = aluno.Email,
                            Posicao = posicao,
                            ScoreGeral = Math.Round(scoreGeral, 1)
                        });
                    }

                    timesDTO.Add(new TimeDTO
                    {
                        Id = time.Id,
                        Nome = time.Nome,
                        DataCriacao = time.DataCriacao,
                        Jogadores = jogadoresDTO
                    });
                }

                return new GerenciarTimesDTO
                {
                    Times = timesDTO,
                    TotalTimes = timesDTO.Count
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter times");
                throw new Exception("Erro ao obter times. Tente novamente mais tarde.");
            }
        }

        // Criar Time
        public async Task<TimeDTO> CriarTimeAsync(int tecnicoId, CriarTimeRequestDTO request)
        {
            try
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Role != UserRole.TecnicoFutebol)
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                // Validações
                if (string.IsNullOrWhiteSpace(request.Nome))
                    throw new ArgumentException("Nome do time é obrigatório");

                if (request.JogadoresIds == null || request.JogadoresIds.Count == 0)
                    throw new ArgumentException("É necessário adicionar pelo menos um jogador");

                var context = GetContext();

                // Validar se os alunos existem e estão ativos
                var alunos = await context.Usuarios
                    .Where(u => request.JogadoresIds.Contains(u.Id) && 
                               u.Role == UserRole.Aluno && 
                               u.IsActive)
                    .ToListAsync();

                if (alunos.Count != request.JogadoresIds.Count)
                    throw new ArgumentException("Um ou mais alunos não foram encontrados ou estão inativos");

                // Criar time
                var time = new Time
                {
                    Nome = request.Nome.Trim(),
                    TecnicoId = tecnicoId,
                    DataCriacao = DateTime.UtcNow,
                    IsActive = true
                };

                context.Times.Add(time);
                await context.SaveChangesAsync();

                // Adicionar jogadores ao time
                foreach (var alunoId in request.JogadoresIds)
                {
                    // Verificar se jogador já está no time (evitar duplicatas)
                    var jaEstaNoTime = await context.JogadoresTime
                        .AnyAsync(j => j.TimeId == time.Id && j.AlunoId == alunoId);

                    if (!jaEstaNoTime)
                    {
                        var jogadorTime = new JogadorTime
                        {
                            TimeId = time.Id,
                            AlunoId = alunoId,
                            DataEscalacao = DateTime.UtcNow
                        };

                        context.JogadoresTime.Add(jogadorTime);
                    }
                }

                await context.SaveChangesAsync();

                _logger.LogInformation("Time {TimeId} criado pelo técnico {TecnicoId} com {QuantidadeJogadores} jogadores", 
                    time.Id, tecnicoId, request.JogadoresIds.Count);

                // Retornar time criado
                return await ObterTimePorIdAsync(context, time.Id);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar time");
                throw new Exception("Erro ao criar time. Tente novamente mais tarde.");
            }
        }

        // Adicionar Jogador ao Time
        public async Task<TimeDTO> AdicionarJogadorAoTimeAsync(int tecnicoId, int timeId, AdicionarJogadorRequestDTO request)
        {
            try
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Role != UserRole.TecnicoFutebol)
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var context = GetContext();

                // Validar se time existe e pertence ao técnico
                var time = await context.Times
                    .FirstOrDefaultAsync(t => t.Id == timeId && t.TecnicoId == tecnicoId && t.IsActive);

                if (time == null)
                    throw new ArgumentException("Time não encontrado");

                // Validar se aluno existe e está ativo
                var aluno = await context.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == request.AlunoId && 
                                            u.Role == UserRole.Aluno && 
                                            u.IsActive);

                if (aluno == null)
                    throw new ArgumentException("Aluno não encontrado ou está inativo");

                // Verificar se aluno já está no time
                var jaEstaNoTime = await context.JogadoresTime
                    .AnyAsync(j => j.TimeId == timeId && j.AlunoId == request.AlunoId);

                if (jaEstaNoTime)
                    throw new ArgumentException("Aluno já está no time");

                // Adicionar jogador ao time
                var jogadorTime = new JogadorTime
                {
                    TimeId = timeId,
                    AlunoId = request.AlunoId,
                    DataEscalacao = DateTime.UtcNow
                };

                context.JogadoresTime.Add(jogadorTime);
                await context.SaveChangesAsync();

                _logger.LogInformation("Jogador {AlunoId} adicionado ao time {TimeId} pelo técnico {TecnicoId}", 
                    request.AlunoId, timeId, tecnicoId);

                // Retornar time atualizado
                return await ObterTimePorIdAsync(context, timeId);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar jogador ao time");
                throw new Exception("Erro ao adicionar jogador ao time. Tente novamente mais tarde.");
            }
        }

        // Remover Jogador do Time
        public async Task<RemoverJogadorResponseDTO> RemoverJogadorDoTimeAsync(int tecnicoId, int timeId, int jogadorId)
        {
            try
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Role != UserRole.TecnicoFutebol)
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var context = GetContext();

                // Validar se time existe e pertence ao técnico
                var time = await context.Times
                    .FirstOrDefaultAsync(t => t.Id == timeId && t.TecnicoId == tecnicoId && t.IsActive);

                if (time == null)
                    throw new ArgumentException("Time não encontrado");

                // Validar se jogador existe no time
                var jogadorTime = await context.JogadoresTime
                    .FirstOrDefaultAsync(j => j.Id == jogadorId && j.TimeId == timeId);

                if (jogadorTime == null)
                    throw new ArgumentException("Jogador não encontrado no time");

                // Remover jogador do time
                context.JogadoresTime.Remove(jogadorTime);
                await context.SaveChangesAsync();

                _logger.LogInformation("Jogador {JogadorId} removido do time {TimeId} pelo técnico {TecnicoId}", 
                    jogadorId, timeId, tecnicoId);

                return new RemoverJogadorResponseDTO
                {
                    Message = "Jogador removido do time com sucesso"
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover jogador do time");
                throw new Exception("Erro ao remover jogador do time. Tente novamente mais tarde.");
            }
        }

        // Deletar Time
        public async Task<DeletarTimeResponseDTO> DeletarTimeAsync(int tecnicoId, int timeId)
        {
            try
            {
                var tecnico = await _userRepository.GetByIdAsync(tecnicoId);
                if (tecnico == null || tecnico.Role != UserRole.TecnicoFutebol)
                    throw new ArgumentException("Técnico não encontrado ou sem permissão");

                var context = GetContext();

                // Validar se time existe e pertence ao técnico
                var time = await context.Times
                    .Include(t => t.Jogadores)
                    .FirstOrDefaultAsync(t => t.Id == timeId && t.TecnicoId == tecnicoId && t.IsActive);

                if (time == null)
                    throw new ArgumentException("Time não encontrado");

                // Soft delete: marcar como inativo
                time.IsActive = false;

                // Opcional: remover jogadores do time (cascade delete já faz isso)
                // Mas vamos manter os registros para histórico

                await context.SaveChangesAsync();

                _logger.LogInformation("Time {TimeId} excluído (soft delete) pelo técnico {TecnicoId}. Tinha {QuantidadeJogadores} jogadores", 
                    timeId, tecnicoId, time.Jogadores.Count);

                return new DeletarTimeResponseDTO
                {
                    Message = "Time excluído com sucesso",
                    TimeId = timeId
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar time");
                throw new Exception("Erro ao deletar time. Tente novamente mais tarde.");
            }
        }

        // Método auxiliar para obter time por ID
        private async Task<TimeDTO> ObterTimePorIdAsync(EduQuizContext context, int timeId)
        {
            var time = await context.Times
                .Include(t => t.Jogadores)
                .ThenInclude(j => j.Aluno)
                .FirstOrDefaultAsync(t => t.Id == timeId && t.IsActive);

            if (time == null)
                throw new ArgumentException("Time não encontrado");

            // Otimização: calcular scores de todos os alunos de uma vez
            var todosAlunos = await context.Usuarios
                .Where(u => u.Role == UserRole.Aluno && u.IsActive)
                .ToListAsync();

            var todasTentativas = await context.TentativasQuiz
                .Where(t => t.Concluida)
                .ToListAsync();

            // Criar dicionário de scores por aluno
            var scoresPorAluno = todasTentativas
                .GroupBy(t => t.UsuarioId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100)
                );

            // Calcular posições no ranking
            var alunosComScore = todosAlunos
                .Select(u => new
                {
                    Id = u.Id,
                    Score = scoresPorAluno.ContainsKey(u.Id) ? scoresPorAluno[u.Id] : 0
                })
                .OrderByDescending(a => a.Score)
                .ToList();

            var posicoesPorAluno = alunosComScore
                .Select((a, index) => new { a.Id, Posicao = index + 1 })
                .ToDictionary(a => a.Id, a => a.Posicao);

            var jogadoresDTO = new List<JogadorTimeDTO>();

            foreach (var jogador in time.Jogadores.OrderBy(j => j.DataEscalacao))
            {
                var aluno = jogador.Aluno;
                var scoreGeral = scoresPorAluno.ContainsKey(aluno.Id) ? scoresPorAluno[aluno.Id] : 0;
                var posicao = posicoesPorAluno.ContainsKey(aluno.Id) ? posicoesPorAluno[aluno.Id] : 0;

                jogadoresDTO.Add(new JogadorTimeDTO
                {
                    Id = jogador.Id,
                    AlunoId = aluno.Id,
                    Nome = $"{aluno.FirstName} {aluno.LastName}".Trim(),
                    Email = aluno.Email,
                    Posicao = posicao,
                    ScoreGeral = Math.Round(scoreGeral, 1)
                });
            }

            return new TimeDTO
            {
                Id = time.Id,
                Nome = time.Nome,
                DataCriacao = time.DataCriacao,
                Jogadores = jogadoresDTO
            };
        }
    }
}
