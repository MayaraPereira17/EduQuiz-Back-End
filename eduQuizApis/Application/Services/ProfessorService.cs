using eduQuizApis.Application.DTOs;
using eduQuizApis.Application.Interfaces;
using eduQuizApis.Domain.Interfaces;
using eduQuizApis.Infrastructure.Data;
using eduQuizApis.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eduQuizApis.Application.Services
{
    public class ProfessorService : IProfessorService
    {
        private readonly IUserRepository _userRepository;
        private readonly EduQuizContext _context;

        public ProfessorService(IUserRepository userRepository, EduQuizContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        // Dashboard
        public async Task<DashboardProfessorDTO> ObterDashboardAsync(int professorId)
        {
            try
            {
                var professor = await _userRepository.GetByIdAsync(professorId);
                if (professor == null || professor.Role.ToString() != "Professor")
                    throw new ArgumentException("Professor não encontrado");

                // Obter quizzes criados pelo professor
                var quizzesCriados = await _context.Quizzes
                    .Where(q => q.CriadoPor == professorId)
                    .CountAsync();

                // Simplificar consultas para evitar problemas de relacionamentos
                var mediaDosAlunos = 0.0m;
                var totalAlunos = 0;
                var totalTentativas = 0;

                try
                {
                    // Obter IDs dos quizzes do professor
                    var quizIds = await _context.Quizzes
                        .Where(q => q.CriadoPor == professorId)
                        .Select(q => q.Id)
                        .ToListAsync();

                    if (quizIds.Any())
                    {
                        // Calcular estatísticas usando IDs dos quizzes
                        var tentativas = await _context.TentativasQuiz
                            .Where(t => quizIds.Contains(t.QuizId) && t.Concluida)
                            .ToListAsync();

                        totalTentativas = tentativas.Count;
                        totalAlunos = tentativas.Select(t => t.UsuarioId).Distinct().Count();

                        if (tentativas.Any(t => t.Pontuacao.HasValue && t.PontuacaoMaxima.HasValue))
                        {
                            mediaDosAlunos = tentativas
                                .Where(t => t.Pontuacao.HasValue && t.PontuacaoMaxima.HasValue)
                                .Average(t => (t.Pontuacao.Value / t.PontuacaoMaxima.Value) * 100);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log do erro mas não falha o dashboard
                    Console.WriteLine($"Erro ao calcular estatísticas: {ex.Message}");
                }

                // Obter quizzes recentes
                var quizzesRecentes = await ObterQuizzesRecentesAsync(professorId);

                return new DashboardProfessorDTO
                {
                    QuizzesCriados = quizzesCriados,
                    MediaDosAlunos = Math.Round(mediaDosAlunos, 1),
                    TotalAlunos = totalAlunos,
                    TotalTentativas = totalTentativas,
                    QuizzesRecentes = quizzesRecentes
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no ObterDashboardAsync: {ex.Message}");
                throw;
            }
        }

        // Gerenciamento de Quizzes
        public async Task<List<QuizListagemDTO>> ObterMeusQuizzesAsync(int professorId, string? busca = null)
        {
            var query = _context.Quizzes
                .Include(q => q.Categoria)
                .Where(q => q.CriadoPor == professorId);

            if (!string.IsNullOrEmpty(busca))
            {
                query = query.Where(q => 
                    q.Titulo.Contains(busca) || 
                    q.Descricao.Contains(busca));
            }

            var quizzes = await query
                .OrderByDescending(q => q.DataCriacao)
                .Select(q => new QuizListagemDTO
                {
                    Id = q.Id,
                    Titulo = q.Titulo,
                    Descricao = q.Descricao ?? "",
                    Categoria = q.Categoria.Nome,
                    Dificuldade = "Médio", // Pode ser calculado baseado nas questões
                    TempoLimite = q.TempoLimite,
                    TotalQuestoes = q.Questoes.Where(quest => quest.Ativo).Count(),
                    TotalTentativas = _context.TentativasQuiz.Count(t => t.QuizId == q.Id && t.Concluida),
                    Publicado = q.Publico,
                    DataCriacao = q.DataCriacao,
                    MediaPontuacao = 0 // Será calculado separadamente se necessário
                })
                .ToListAsync();

            return quizzes;
        }

        public async Task<QuizCompletoDTO> ObterQuizPorIdAsync(int professorId, int quizId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Categoria)
                .Include(q => q.Questoes)
                .ThenInclude(quest => quest.Opcoes)
                .FirstOrDefaultAsync(q => q.Id == quizId && q.CriadoPor == professorId);

            if (quiz == null)
                throw new ArgumentException("Quiz não encontrado ou você não tem permissão para acessá-lo");

            var questoesAtivas = quiz.Questoes
                .Where(q => q.Ativo)
                .OrderBy(q => q.OrdemIndice)
                .ToList();

            return new QuizCompletoDTO
            {
                Id = quiz.Id,
                Titulo = quiz.Titulo,
                Descricao = quiz.Descricao ?? "",
                CategoriaId = quiz.CategoriaId,
                Categoria = quiz.Categoria.Nome,
                Dificuldade = "Médio", // Pode ser calculado
                TempoLimite = quiz.TempoLimite,
                MaxTentativas = quiz.MaxTentativas,
                Ativo = quiz.Ativo,
                Publico = quiz.Publico,
                DataCriacao = quiz.DataCriacao,
                DataAtualizacao = quiz.DataAtualizacao,
                TotalQuestoes = questoesAtivas.Count,
                Questoes = questoesAtivas.Select(q => new QuestaoCompletaDTO
                {
                    Id = q.Id,
                    TextoQuestao = q.TextoQuestao,
                    TipoQuestao = q.TipoQuestao,
                    Pontos = q.Pontos,
                    OrdemIndice = q.OrdemIndice,
                    Ativo = q.Ativo,
                    Opcoes = q.Opcoes
                        .OrderBy(op => op.OrdemIndice)
                        .Select(op => new OpcaoCompletaDTO
                        {
                            Id = op.Id,
                            TextoOpcao = op.TextoOpcao,
                            Correta = op.Correta,
                            OrdemIndice = op.OrdemIndice
                        }).ToList()
                }).ToList()
            };
        }

        public async Task<CriarQuizResponseDTO> CriarQuizAsync(int professorId, CriarQuizRequestDTO request)
        {
            var professor = await _userRepository.GetByIdAsync(professorId);
            if (professor == null || professor.Role.ToString() != "Professor")
                throw new ArgumentException("Professor não encontrado");

            // Validar categoria
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == request.CategoriaId && c.Ativo);
            if (categoria == null)
                throw new ArgumentException("Categoria não encontrada");

            // Criar quiz
            var quiz = new Quizzes
            {
                Titulo = request.Titulo,
                Descricao = request.Descricao,
                CategoriaId = request.CategoriaId,
                CriadoPor = professorId,
                TempoLimite = request.TempoLimite,
                MaxTentativas = request.MaxTentativas,
                Ativo = true,
                Publico = request.Publico,
                DataCriacao = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            // Criar questões
            foreach (var questaoRequest in request.Questoes)
            {
                var questao = new Questoes
                {
                    QuizId = quiz.Id,
                    TextoQuestao = questaoRequest.TextoQuestao,
                    TipoQuestao = questaoRequest.TipoQuestao,
                    Pontos = questaoRequest.Pontos,
                    OrdemIndice = questaoRequest.OrdemIndice,
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow,
                    DataAtualizacao = DateTime.UtcNow
                };

                _context.Questoes.Add(questao);
                await _context.SaveChangesAsync();

                // Criar opções
                foreach (var opcaoRequest in questaoRequest.Opcoes)
                {
                    var opcao = new OpcoesQuestao
                    {
                        QuestaoId = questao.Id,
                        TextoOpcao = opcaoRequest.TextoOpcao,
                        Correta = opcaoRequest.Correta,
                        OrdemIndice = opcaoRequest.OrdemIndice,
                        DataCriacao = DateTime.UtcNow
                    };

                    _context.OpcoesQuestao.Add(opcao);
                }
            }

            await _context.SaveChangesAsync();

            return new CriarQuizResponseDTO
            {
                Id = quiz.Id,
                Titulo = quiz.Titulo,
                Mensagem = "Quiz criado com sucesso!"
            };
        }

        public async Task<AtualizarQuizResponseDTO> AtualizarQuizAsync(int professorId, int quizId, AtualizarQuizRequestDTO request)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questoes)
                .ThenInclude(q => q.Opcoes)
                .FirstOrDefaultAsync(q => q.Id == quizId && q.CriadoPor == professorId);

            if (quiz == null)
                throw new ArgumentException("Quiz não encontrado ou você não tem permissão para editá-lo");

            // Validar categoria
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == request.CategoriaId && c.Ativo);
            if (categoria == null)
                throw new ArgumentException("Categoria não encontrada");

            // Atualizar dados do quiz
            quiz.Titulo = request.Titulo;
            quiz.Descricao = request.Descricao;
            quiz.CategoriaId = request.CategoriaId;
            quiz.TempoLimite = request.TempoLimite;
            quiz.MaxTentativas = request.MaxTentativas;
            quiz.Ativo = request.Ativo;
            quiz.Publico = request.Publico;
            quiz.DataAtualizacao = DateTime.UtcNow;

            // Gerenciar questões
            await GerenciarQuestoesAsync(quiz.Id, request.Questoes);

            _context.Quizzes.Update(quiz);
            await _context.SaveChangesAsync();

            return new AtualizarQuizResponseDTO
            {
                Id = quiz.Id,
                Titulo = quiz.Titulo,
                Mensagem = "Quiz atualizado com sucesso!"
            };
        }

        public async Task<DeletarQuizResponseDTO> DeletarQuizAsync(int professorId, int quizId)
        {
            var quiz = await _context.Quizzes
                .FirstOrDefaultAsync(q => q.Id == quizId && q.CriadoPor == professorId);

            if (quiz == null)
                throw new ArgumentException("Quiz não encontrado ou você não tem permissão para deletá-lo");

            // Verificar se há tentativas
            var temTentativas = await _context.TentativasQuiz
                .AnyAsync(t => t.QuizId == quizId);

            if (temTentativas)
            {
                // Marcar como inativo em vez de deletar
                quiz.Ativo = false;
                quiz.DataAtualizacao = DateTime.UtcNow;
                _context.Quizzes.Update(quiz);
                await _context.SaveChangesAsync();

                return new DeletarQuizResponseDTO
                {
                    Id = quiz.Id,
                    Titulo = quiz.Titulo,
                    Mensagem = "Quiz desativado com sucesso! (Não pode ser deletado pois possui tentativas)"
                };
            }
            else
            {
                // Deletar completamente
                _context.Quizzes.Remove(quiz);
                await _context.SaveChangesAsync();

                return new DeletarQuizResponseDTO
                {
                    Id = quiz.Id,
                    Titulo = quiz.Titulo,
                    Mensagem = "Quiz deletado com sucesso!"
                };
            }
        }

        public async Task<AtualizarQuizResponseDTO> PublicarQuizAsync(int professorId, int quizId)
        {
            var quiz = await _context.Quizzes
                .FirstOrDefaultAsync(q => q.Id == quizId && q.CriadoPor == professorId);

            if (quiz == null)
                throw new ArgumentException("Quiz não encontrado ou você não tem permissão para editá-lo");

            quiz.Publico = true;
            quiz.DataAtualizacao = DateTime.UtcNow;

            _context.Quizzes.Update(quiz);
            await _context.SaveChangesAsync();

            return new AtualizarQuizResponseDTO
            {
                Id = quiz.Id,
                Titulo = quiz.Titulo,
                Mensagem = "Quiz publicado com sucesso!"
            };
        }

        public async Task<AtualizarQuizResponseDTO> DespublicarQuizAsync(int professorId, int quizId)
        {
            var quiz = await _context.Quizzes
                .FirstOrDefaultAsync(q => q.Id == quizId && q.CriadoPor == professorId);

            if (quiz == null)
                throw new ArgumentException("Quiz não encontrado ou você não tem permissão para editá-lo");

            quiz.Publico = false;
            quiz.DataAtualizacao = DateTime.UtcNow;

            _context.Quizzes.Update(quiz);
            await _context.SaveChangesAsync();

            return new AtualizarQuizResponseDTO
            {
                Id = quiz.Id,
                Titulo = quiz.Titulo,
                Mensagem = "Quiz despublicado com sucesso!"
            };
        }

        // Estatísticas
        public async Task<EstatisticasQuizDTO> ObterEstatisticasQuizAsync(int professorId, int quizId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questoes)
                .FirstOrDefaultAsync(q => q.Id == quizId && q.CriadoPor == professorId);

            if (quiz == null)
                throw new ArgumentException("Quiz não encontrado ou você não tem permissão para acessá-lo");

            var tentativas = await _context.TentativasQuiz
                .Include(t => t.Usuario)
                .Where(t => t.QuizId == quizId && t.Concluida)
                .ToListAsync();

            var totalTentativas = tentativas.Count;
            var totalAlunos = tentativas.Select(t => t.UsuarioId).Distinct().Count();
            var mediaPontuacao = tentativas.Any() ? 
                tentativas.Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100) : 0;
            var mediaTempo = tentativas.Any() && tentativas.Any(t => t.TempoGasto.HasValue) ? 
                tentativas.Where(t => t.TempoGasto.HasValue).Average(t => t.TempoGasto.Value) : 0;

            var estatisticasQuestoes = await ObterEstatisticasQuestoesAsync(professorId, quizId);
            var tentativasRecentes = await ObterTentativasQuizAsync(professorId, quizId);

            return new EstatisticasQuizDTO
            {
                QuizId = quizId,
                TituloQuiz = quiz.Titulo,
                TotalTentativas = totalTentativas,
                TotalAlunos = totalAlunos,
                MediaPontuacao = Math.Round(mediaPontuacao, 1),
                MediaTempo = Math.Round((decimal)mediaTempo, 1),
                TotalQuestoes = quiz.Questoes.Count,
                EstatisticasQuestoes = estatisticasQuestoes,
                TentativasRecentes = tentativasRecentes
            };
        }

        public async Task<List<EstatisticaQuestaoDTO>> ObterEstatisticasQuestoesAsync(int professorId, int quizId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questoes)
                .FirstOrDefaultAsync(q => q.Id == quizId && q.CriadoPor == professorId);

            if (quiz == null)
                throw new ArgumentException("Quiz não encontrado ou você não tem permissão para acessá-lo");

            var estatisticas = new List<EstatisticaQuestaoDTO>();

            foreach (var questao in quiz.Questoes.Where(q => q.Ativo))
            {
                var respostas = await _context.Respostas
                    .Where(r => r.QuestaoId == questao.Id)
                    .ToListAsync();

                var totalRespostas = respostas.Count;
                var respostasCorretas = respostas.Count(r => r.Correta == true);
                var respostasErradas = respostas.Count(r => r.Correta == false);
                var percentualAcerto = totalRespostas > 0 ? (decimal)respostasCorretas / totalRespostas * 100 : 0;

                estatisticas.Add(new EstatisticaQuestaoDTO
                {
                    QuestaoId = questao.Id,
                    TextoQuestao = questao.TextoQuestao,
                    TotalRespostas = totalRespostas,
                    RespostasCorretas = respostasCorretas,
                    RespostasErradas = respostasErradas,
                    PercentualAcerto = Math.Round(percentualAcerto, 1)
                });
            }

            return estatisticas;
        }

        public async Task<List<TentativaResumoDTO>> ObterTentativasQuizAsync(int professorId, int quizId)
        {
            var quiz = await _context.Quizzes
                .FirstOrDefaultAsync(q => q.Id == quizId && q.CriadoPor == professorId);

            if (quiz == null)
                throw new ArgumentException("Quiz não encontrado ou você não tem permissão para acessá-lo");

            var tentativas = await _context.TentativasQuiz
                .Include(t => t.Usuario)
                .Where(t => t.QuizId == quizId && t.Concluida)
                .OrderByDescending(t => t.DataConclusao)
                .Take(10)
                .Select(t => new TentativaResumoDTO
                {
                    TentativaId = t.Id,
                    NomeAluno = t.Usuario.FirstName + " " + t.Usuario.LastName,
                    Pontuacao = (int)(t.Pontuacao ?? 0),
                    PontuacaoMaxima = (int)(t.PontuacaoMaxima ?? 0),
                    Percentual = t.Pontuacao.HasValue && t.PontuacaoMaxima.HasValue ? 
                        Math.Round((t.Pontuacao.Value / t.PontuacaoMaxima.Value) * 100, 1) : 0,
                    TempoGasto = t.TempoGasto ?? 0,
                    DataConclusao = t.DataConclusao ?? t.DataInicio
                })
                .ToListAsync();

            return tentativas;
        }

        // Perfil
        public async Task<PerfilProfessorDTO> ObterPerfilAsync(int professorId)
        {
            var professor = await _userRepository.GetByIdAsync(professorId);
            if (professor == null)
                throw new ArgumentException("Professor não encontrado");

            var estatisticas = await ObterEstatisticasProfessorAsync(professorId);

            return new PerfilProfessorDTO
            {
                Id = professor.Id,
                Nome = professor.FirstName,
                Sobrenome = professor.LastName,
                NomeCompleto = $"{professor.FirstName} {professor.LastName}",
                Email = professor.Email,
                Funcao = professor.Role.ToString(),
                CPF = professor.CPF ?? "",
                DataNascimento = professor.DataNascimento,
                Escola = "", // Pode ser adicionado ao modelo se necessário
                Disciplina = "", // Pode ser adicionado ao modelo se necessário
                DataCriacao = professor.CreatedAt,
                Estatisticas = estatisticas
            };
        }

        public async Task<PerfilProfessorDTO> AtualizarPerfilAsync(int professorId, AtualizarPerfilProfessorRequestDTO request)
        {
            var professor = await _userRepository.GetByIdAsync(professorId);
            if (professor == null)
                throw new ArgumentException("Professor não encontrado");

            professor.FirstName = request.Nome;
            professor.LastName = request.Sobrenome;
            professor.CPF = request.CPF;
            professor.DataNascimento = request.DataNascimento;
            professor.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(professor);

            return await ObterPerfilAsync(professorId);
        }

        // Categorias
        public async Task<List<CategoriaDTO>> ObterCategoriasAsync()
        {
            return await _context.Categorias
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .Select(c => new CategoriaDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Descricao = c.Descricao ?? "",
                    Ativo = c.Ativo
                })
                .ToListAsync();
        }

        public async Task<CategoriaDTO> ObterCategoriaPorIdAsync(int categoriaId)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == categoriaId && c.Ativo);

            if (categoria == null)
                throw new ArgumentException("Categoria não encontrada");

            return new CategoriaDTO
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Descricao = categoria.Descricao ?? "",
                Ativo = categoria.Ativo
            };
        }

        // Métodos auxiliares privados
        private async Task<decimal> CalcularMediaDosAlunosAsync(int professorId)
        {
            var tentativas = await _context.TentativasQuiz
                .Include(t => t.Quiz)
                .Where(t => t.Quiz.CriadoPor == professorId && t.Concluida && 
                           t.Pontuacao.HasValue && t.PontuacaoMaxima.HasValue)
                .ToListAsync();

            if (!tentativas.Any()) return 0;

            return tentativas.Average(t => (t.Pontuacao.Value / t.PontuacaoMaxima.Value) * 100);
        }

        private async Task<List<QuizResumoDTO>> ObterQuizzesRecentesAsync(int professorId)
        {
            try
            {
                var quizzes = await _context.Quizzes
                    .Include(q => q.Categoria)
                    .Where(q => q.CriadoPor == professorId)
                    .OrderByDescending(q => q.DataCriacao)
                    .Take(5)
                    .ToListAsync();

                var resultado = new List<QuizResumoDTO>();

                foreach (var q in quizzes)
                {
                    var totalTentativas = 0;
                    try
                    {
                        totalTentativas = await _context.TentativasQuiz
                            .CountAsync(t => t.QuizId == q.Id && t.Concluida);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao contar tentativas para quiz {q.Id}: {ex.Message}");
                    }

                    resultado.Add(new QuizResumoDTO
                    {
                        Id = q.Id,
                        Titulo = q.Titulo,
                        Categoria = q.Categoria?.Nome ?? "Sem categoria",
                        TotalTentativas = totalTentativas,
                        MediaPontuacao = 0, // Será calculado separadamente se necessário
                        DataCriacao = q.DataCriacao,
                        Publicado = q.Publico
                    });
                }

                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no ObterQuizzesRecentesAsync: {ex.Message}");
                return new List<QuizResumoDTO>();
            }
        }

        private async Task GerenciarQuestoesAsync(int quizId, List<AtualizarQuestaoRequestDTO> questoesRequest)
        {
            var questoesExistentes = await _context.Questoes
                .Include(q => q.Opcoes)
                .Where(q => q.QuizId == quizId)
                .ToListAsync();

            // IDs das questões que devem permanecer
            var idsQuestoesManter = questoesRequest
                .Where(q => q.Id.HasValue)
                .Select(q => q.Id.Value)
                .ToList();

            // Remover questões que não estão na lista
            var questoesParaRemover = questoesExistentes
                .Where(q => !idsQuestoesManter.Contains(q.Id))
                .ToList();

            foreach (var questao in questoesParaRemover)
            {
                questao.Ativo = false;
                questao.DataAtualizacao = DateTime.UtcNow;
            }

            // Atualizar ou criar questões
            foreach (var questaoRequest in questoesRequest)
            {
                if (questaoRequest.Id.HasValue)
                {
                    // Atualizar questão existente
                    var questao = questoesExistentes.FirstOrDefault(q => q.Id == questaoRequest.Id.Value);
                    if (questao != null)
                    {
                        questao.TextoQuestao = questaoRequest.TextoQuestao;
                        questao.TipoQuestao = questaoRequest.TipoQuestao;
                        questao.Pontos = questaoRequest.Pontos;
                        questao.OrdemIndice = questaoRequest.OrdemIndice;
                        questao.Ativo = true;
                        questao.DataAtualizacao = DateTime.UtcNow;

                        // Gerenciar opções da questão
                        await GerenciarOpcoesAsync(questao.Id, questaoRequest.Opcoes);
                    }
                }
                else
                {
                    // Criar nova questão
                    var novaQuestao = new Questoes
                    {
                        QuizId = quizId,
                        TextoQuestao = questaoRequest.TextoQuestao,
                        TipoQuestao = questaoRequest.TipoQuestao,
                        Pontos = questaoRequest.Pontos,
                        OrdemIndice = questaoRequest.OrdemIndice,
                        Ativo = true,
                        DataCriacao = DateTime.UtcNow,
                        DataAtualizacao = DateTime.UtcNow
                    };

                    _context.Questoes.Add(novaQuestao);
                    await _context.SaveChangesAsync();

                    // Criar opções da nova questão
                    foreach (var opcaoRequest in questaoRequest.Opcoes)
                    {
                        var novaOpcao = new OpcoesQuestao
                        {
                            QuestaoId = novaQuestao.Id,
                            TextoOpcao = opcaoRequest.TextoOpcao,
                            Correta = opcaoRequest.Correta,
                            OrdemIndice = opcaoRequest.OrdemIndice,
                            DataCriacao = DateTime.UtcNow
                        };

                        _context.OpcoesQuestao.Add(novaOpcao);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task GerenciarOpcoesAsync(int questaoId, List<AtualizarOpcaoRequestDTO> opcoesRequest)
        {
            var opcoesExistentes = await _context.OpcoesQuestao
                .Where(o => o.QuestaoId == questaoId)
                .ToListAsync();

            // IDs das opções que devem permanecer
            var idsOpcoesManter = opcoesRequest
                .Where(o => o.Id.HasValue)
                .Select(o => o.Id.Value)
                .ToList();

            // Remover opções que não estão na lista
            var opcoesParaRemover = opcoesExistentes
                .Where(o => !idsOpcoesManter.Contains(o.Id))
                .ToList();

            _context.OpcoesQuestao.RemoveRange(opcoesParaRemover);

            // Atualizar ou criar opções
            foreach (var opcaoRequest in opcoesRequest)
            {
                if (opcaoRequest.Id.HasValue)
                {
                    // Atualizar opção existente
                    var opcao = opcoesExistentes.FirstOrDefault(o => o.Id == opcaoRequest.Id.Value);
                    if (opcao != null)
                    {
                        opcao.TextoOpcao = opcaoRequest.TextoOpcao;
                        opcao.Correta = opcaoRequest.Correta;
                        opcao.OrdemIndice = opcaoRequest.OrdemIndice;
                    }
                }
                else
                {
                    // Criar nova opção
                    var novaOpcao = new OpcoesQuestao
                    {
                        QuestaoId = questaoId,
                        TextoOpcao = opcaoRequest.TextoOpcao,
                        Correta = opcaoRequest.Correta,
                        OrdemIndice = opcaoRequest.OrdemIndice,
                        DataCriacao = DateTime.UtcNow
                    };

                    _context.OpcoesQuestao.Add(novaOpcao);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task<EstatisticasProfessorDTO> ObterEstatisticasProfessorAsync(int professorId)
        {
            var quizzesCriados = await _context.Quizzes
                .CountAsync(q => q.CriadoPor == professorId);

            var quizzesPublicados = await _context.Quizzes
                .CountAsync(q => q.CriadoPor == professorId && q.Publico);

            var mediaDosAlunos = await CalcularMediaDosAlunosAsync(professorId);

            var totalAlunos = await _context.TentativasQuiz
                .Include(t => t.Quiz)
                .Where(t => t.Quiz.CriadoPor == professorId && t.Concluida)
                .Select(t => t.UsuarioId)
                .Distinct()
                .CountAsync();

            var totalTentativas = await _context.TentativasQuiz
                .Include(t => t.Quiz)
                .Where(t => t.Quiz.CriadoPor == professorId && t.Concluida)
                .CountAsync();

            return new EstatisticasProfessorDTO
            {
                QuizzesCriados = quizzesCriados,
                MediaDosAlunos = Math.Round(mediaDosAlunos, 1),
                TotalAlunos = totalAlunos,
                TotalTentativas = totalTentativas,
                QuizzesPublicados = quizzesPublicados
            };
        }
    }
}
