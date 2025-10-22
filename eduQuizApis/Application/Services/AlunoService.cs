using eduQuizApis.Application.DTOs;
using eduQuizApis.Application.Interfaces;
using eduQuizApis.Domain.Interfaces;
using eduQuizApis.Infrastructure.Data;
using eduQuizApis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eduQuizApis.Application.Services
{
    public class AlunoService : IAlunoService
    {
        private readonly IUserRepository _userRepository;
        private readonly EduQuizContext _context;
        private readonly ILogger<AlunoService> _logger;

        public AlunoService(IUserRepository userRepository, EduQuizContext context, ILogger<AlunoService> logger)
        {
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }

        // Dashboard
        public async Task<DashboardAlunoDTO> ObterDashboardAsync(int usuarioId)
        {
            var usuario = await _userRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
                throw new ArgumentException("Usuário não encontrado");

            // Obter estatísticas das tentativas concluídas
            var tentativasConcluidas = await _context.TentativasQuiz
                .Where(t => t.UsuarioId == usuarioId && t.Concluida)
                .ToListAsync();

            var quizzesCompletos = tentativasConcluidas.Count;
            var mediaGeral = tentativasConcluidas.Any() ? 
                tentativasConcluidas.Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100) : 0;

            // Obter posição no ranking e somar todos os pontos
            var rankings = await _context.RankingAlunos
                .Where(r => r.UsuarioId == usuarioId)
                .ToListAsync();

            var posicaoRanking = rankings.FirstOrDefault()?.PosicaoRanking ?? 0;
            var pontos = rankings.Sum(r => r.PontosExperiencia);

            // Calcular sequência (dias consecutivos com quiz concluído)
            var sequencia = await CalcularSequenciaAsync(usuarioId);

            // Obter total de usuários
            var totalUsuarios = await _context.Usuarios
                .Where(u => u.Role == Domain.Enums.UserRole.Aluno && u.IsActive)
                .CountAsync();

            // Obter quizzes recentes
            var quizzesRecentes = await ObterQuizzesRecentesAsync(usuarioId);

            return new DashboardAlunoDTO
            {
                QuizzesCompletos = quizzesCompletos,
                MediaGeral = Math.Round(mediaGeral, 1),
                PosicaoRanking = posicaoRanking,
                Sequencia = sequencia,
                Pontos = pontos,
                TotalUsuarios = totalUsuarios,
                QuizzesRecentes = quizzesRecentes
            };
        }

        // Quiz
        public async Task<List<QuizDisponivelDTO>> ObterQuizzesDisponiveisAsync(int usuarioId)
        {
            var quizzes = await _context.Quizzes
                .Include(q => q.Categoria)
                .Include(q => q.Questoes)
                .Where(q => q.Ativo && q.Publico)
                .Select(q => new QuizDisponivelDTO
                {
                    Id = q.Id,
                    Titulo = q.Titulo,
                    Descricao = q.Descricao ?? "",
                    Categoria = q.Categoria.Nome,
                    Dificuldade = q.Dificuldade,
                    TempoLimite = q.TempoLimite,
                    TotalQuestoes = q.Questoes.Count(quest => quest.Ativo),
                    PontuacaoTotal = q.Questoes.Count(quest => quest.Ativo), // Todas as questões valem 1 ponto
                    Disponivel = true,
                    QuizConcluido = false, // Será atualizado abaixo
                    TentativasRestantes = 1 // Sempre 1 para novos quizzes
                })
                .ToListAsync();

            // Verificar quais quizzes já foram concluídos pelo usuário
            var tentativasConcluidas = await _context.TentativasQuiz
                .Where(t => t.UsuarioId == usuarioId && t.Concluida)
                .Select(t => t.QuizId)
                .ToListAsync();

            // Atualizar status dos quizzes
            foreach (var quiz in quizzes)
            {
                if (tentativasConcluidas.Contains(quiz.Id))
                {
                    quiz.QuizConcluido = true;
                    quiz.TentativasRestantes = 0;
                    quiz.Disponivel = false; // Não pode fazer novamente
                }
            }

            return quizzes;
        }

        public async Task<QuizDetalhesDTO> ObterQuizPorIdAsync(int usuarioId, int quizId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Categoria)
                .Include(q => q.Questoes.Where(quest => quest.Ativo).OrderBy(quest => quest.OrdemIndice))
                .ThenInclude(quest => quest.Opcoes.OrderBy(op => op.OrdemIndice))
                .FirstOrDefaultAsync(q => q.Id == quizId && q.Ativo && q.Publico);

            if (quiz == null)
                throw new ArgumentException("Quiz não encontrado ou não disponível.");

            // Verificar se o usuário já fez este quiz (limitação de 1 tentativa)
            var tentativaExistente = await _context.TentativasQuiz
                .FirstOrDefaultAsync(t => t.UsuarioId == usuarioId && t.QuizId == quizId);
            
            if (tentativaExistente != null)
                throw new ArgumentException("Você já realizou este quiz. Cada quiz pode ser feito apenas uma vez.");
            
            var tentativasRestantes = 1; // Sempre 1 para novos quizzes

            return new QuizDetalhesDTO
            {
                Id = quiz.Id,
                Titulo = quiz.Titulo,
                Descricao = quiz.Descricao ?? "",
                Categoria = new CategoriaDTO
                {
                    Id = quiz.Categoria.Id,
                    Nome = quiz.Categoria.Nome
                },
                Dificuldade = quiz.Dificuldade,
                TempoLimite = quiz.TempoLimite,
                MaxTentativas = quiz.MaxTentativas,
                TentativasRestantes = Math.Max(0, tentativasRestantes),
                TotalQuestoes = quiz.Questoes.Count,
                CriadoPor = "Professor", // Pode ser obtido do relacionamento com Usuarios
                DataCriacao = quiz.DataCriacao,
                Questoes = quiz.Questoes.Select(questao => new QuestaoDetalhesDTO
                {
                    Id = questao.Id,
                    TextoQuestao = questao.TextoQuestao,
                    TipoQuestao = questao.TipoQuestao,
                    Pontos = questao.Pontos,
                    OrdemIndice = questao.OrdemIndice,
                    Opcoes = questao.Opcoes.Select(opcao => new OpcaoDetalhesDTO
                    {
                        Id = opcao.Id,
                        TextoOpcao = opcao.TextoOpcao,
                        OrdemIndice = opcao.OrdemIndice
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<ResponderQuizResponseDTO> ResponderQuizAsync(int usuarioId, int quizId, ResponderQuizRequestDTO request)
        {
            _logger.LogInformation($"🔍 DEBUG: ResponderQuizAsync - UsuarioId: {usuarioId}, QuizId: {quizId}");
            
            var quiz = await _context.Quizzes
                .Include(q => q.Questoes.Where(quest => quest.Ativo).OrderBy(quest => quest.OrdemIndice))
                .ThenInclude(quest => quest.Opcoes.OrderBy(op => op.OrdemIndice))
                .FirstOrDefaultAsync(q => q.Id == quizId && q.Ativo && q.Publico);

            if (quiz == null)
                throw new ArgumentException("Quiz não encontrado ou não disponível.");

            // Buscar tentativa existente (deve ter sido criada em IniciarQuizAsync)
            var tentativa = await _context.TentativasQuiz
                .FirstOrDefaultAsync(t => t.UsuarioId == usuarioId && t.QuizId == quizId);
            
            if (tentativa == null)
                throw new InvalidOperationException("Tentativa não encontrada. Inicie o quiz primeiro.");

            if (tentativa.Concluida)
                throw new InvalidOperationException("Este quiz já foi concluído.");

            // Atualizar tentativa existente
            tentativa.Concluida = true;
            tentativa.DataConclusao = DateTime.UtcNow;
            tentativa.Pontuacao = 0;
            tentativa.PontuacaoMaxima = quiz.Questoes.Count; // Cada questão vale 1 ponto

            // Processar respostas
            var respostas = new List<RespostaResultadoDTO>();
            var pontuacaoTotal = 0;
            var respostasCorretas = 0;
            var respostasIncorretas = 0;

            foreach (var resposta in request.Respostas)
            {
                var questao = quiz.Questoes.FirstOrDefault(q => q.Id == resposta.QuestaoId);
                if (questao == null) continue;

                var opcaoCorreta = questao.Opcoes.FirstOrDefault(o => o.Correta);
                var opcaoSelecionada = questao.Opcoes.FirstOrDefault(o => o.Id == resposta.OpcaoSelecionadaId);
                
                var correta = opcaoSelecionada?.Correta ?? false;
                var pontosObtidos = correta ? 1 : 0; // Cada questão vale 1 ponto
                
                if (correta)
                {
                    pontuacaoTotal += pontosObtidos;
                    respostasCorretas++;
                }
                else
                {
                    respostasIncorretas++;
                }

                // Salvar resposta no banco
                var respostaEntity = new Respostas
                {
                    TentativaId = tentativa.Id,
                    QuestaoId = questao.Id,
                    OpcaoSelecionadaId = resposta.OpcaoSelecionadaId,
                    TextoResposta = resposta.TextoResposta,
                    Correta = correta,
                    PontosGanhos = pontosObtidos,
                    DataResposta = DateTime.UtcNow
                };

                _context.Respostas.Add(respostaEntity);

                respostas.Add(new RespostaResultadoDTO
                {
                    QuestaoId = questao.Id,
                    OpcaoSelecionadaId = resposta.OpcaoSelecionadaId,
                    TextoRespostaSelecionada = opcaoSelecionada?.TextoOpcao,
                    OpcaoCorretaId = opcaoCorreta?.Id,
                    TextoRespostaCorreta = opcaoCorreta?.TextoOpcao,
                    Correta = correta,
                    PontosObtidos = pontosObtidos
                });
            }

            // Atualizar tentativa com pontuação final
            tentativa.Pontuacao = pontuacaoTotal;
            _logger.LogInformation($"🔍 DEBUG: Pontuação calculada - Total: {pontuacaoTotal}, Corretas: {respostasCorretas}, Incorretas: {respostasIncorretas}");
            _context.TentativasQuiz.Update(tentativa);
            await _context.SaveChangesAsync();

            // Atualizar ranking do aluno
            await AtualizarRankingAsync(usuarioId, quiz.CategoriaId);

            var percentualAcerto = tentativa.PontuacaoMaxima > 0 ? 
                (decimal)tentativa.Pontuacao / tentativa.PontuacaoMaxima * 100 : 0;

            return new ResponderQuizResponseDTO
            {
                TentativaId = tentativa.Id,
                QuizId = quizId,
                AlunoId = usuarioId,
                PontuacaoTotal = pontuacaoTotal,
                PontuacaoMaxima = (int)(tentativa.PontuacaoMaxima ?? 0),
                PercentualAcerto = (decimal)percentualAcerto,
                DataTentativa = tentativa.DataConclusao ?? DateTime.UtcNow,
                TempoGasto = (int)((tentativa.DataConclusao - tentativa.DataInicio)?.TotalSeconds ?? 0),
                RespostasCorretas = respostasCorretas,
                RespostasIncorretas = respostasIncorretas,
                Respostas = respostas,
                Message = "Quiz respondido com sucesso!",
                NovoRecorde = false // Pode ser implementado para verificar se é o melhor resultado
            };
        }

        public async Task<IniciarQuizResponseDTO> IniciarQuizAsync(int usuarioId, IniciarQuizRequestDTO request)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questoes.Where(quest => quest.Ativo).OrderBy(quest => quest.OrdemIndice))
                .ThenInclude(quest => quest.Opcoes.OrderBy(op => op.OrdemIndice))
                .FirstOrDefaultAsync(q => q.Id == request.QuizId && q.Ativo && q.Publico);

            if (quiz == null)
                throw new ArgumentException("Quiz não encontrado ou não disponível");

            // Verificar se o usuário já fez este quiz (limitação de 1 tentativa)
            var tentativaExistente = await _context.TentativasQuiz
                .FirstOrDefaultAsync(t => t.UsuarioId == usuarioId && t.QuizId == request.QuizId);
            
            if (tentativaExistente != null)
                throw new InvalidOperationException("Você já realizou este quiz. Cada quiz pode ser feito apenas uma vez.");

            // Criar nova tentativa
            var tentativa = new TentativasQuiz
            {
                QuizId = request.QuizId,
                UsuarioId = usuarioId,
                DataInicio = DateTime.UtcNow,
                PontuacaoMaxima = quiz.Questoes.Count,
                Concluida = false
            };

            _context.TentativasQuiz.Add(tentativa);
            await _context.SaveChangesAsync();

            var primeiraQuestao = quiz.Questoes.First();
            var questaoAtual = new QuestaoAtualDTO
            {
                Id = primeiraQuestao.Id,
                TextoQuestao = primeiraQuestao.TextoQuestao,
                TipoQuestao = primeiraQuestao.TipoQuestao.ToString(),
                Pontos = primeiraQuestao.Pontos,
                OrdemIndice = primeiraQuestao.OrdemIndice,
                Opcoes = primeiraQuestao.Opcoes.Select(op => new OpcaoRespostaDTO
                {
                    Id = op.Id,
                    TextoOpcao = op.TextoOpcao,
                    OrdemIndice = op.OrdemIndice
                }).ToList()
            };

            var progresso = new ProgressoQuizDTO
            {
                QuestaoAtual = 1,
                TotalQuestoes = quiz.Questoes.Count,
                PercentualCompleto = 0,
                PontuacaoAtual = 0,
                TempoGasto = 0
            };

            return new IniciarQuizResponseDTO
            {
                TentativaId = tentativa.Id,
                QuizId = quiz.Id,
                TituloQuiz = quiz.Titulo,
                QuestaoAtual = questaoAtual,
                Progresso = progresso
            };
        }

        public async Task<ResponderQuestaoResponseDTO> ResponderQuestaoAsync(int usuarioId, int tentativaId, ResponderQuestaoRequestDTO request)
        {
            var tentativa = await _context.TentativasQuiz
                .Include(t => t.Quiz)
                .ThenInclude(q => q.Questoes.Where(quest => quest.Ativo).OrderBy(quest => quest.OrdemIndice))
                .ThenInclude(quest => quest.Opcoes)
                .FirstOrDefaultAsync(t => t.Id == tentativaId && t.UsuarioId == usuarioId && !t.Concluida);

            if (tentativa == null)
                throw new ArgumentException("Tentativa não encontrada ou já concluída");

            var questao = tentativa.Quiz.Questoes.FirstOrDefault(q => q.Id == request.QuestaoId);
            if (questao == null)
                throw new ArgumentException("Questão não encontrada");

            // Verificar se já existe resposta para esta questão
            var respostaExistente = await _context.Respostas
                .FirstOrDefaultAsync(r => r.TentativaId == tentativaId && r.QuestaoId == request.QuestaoId);

            if (respostaExistente != null)
                throw new InvalidOperationException("Questão já foi respondida");

            // Validar resposta
            bool respostaCorreta = false;
            string respostaCorretaTexto = "";
            int pontosGanhos = 0;

            if (questao.TipoQuestao == "MultiplaEscolha")
            {
                var opcaoCorreta = questao.Opcoes.FirstOrDefault(op => op.Correta);
                if (opcaoCorreta != null)
                {
                    respostaCorreta = request.OpcaoSelecionadaId == opcaoCorreta.Id;
                    respostaCorretaTexto = opcaoCorreta.TextoOpcao;
                }
            }
            else if (questao.TipoQuestao == "VerdadeiroFalso")
            {
                // Implementar lógica para verdadeiro/falso se necessário
                respostaCorreta = true; // Placeholder
                respostaCorretaTexto = "Resposta correta";
            }

            if (respostaCorreta)
                pontosGanhos = 1; // Sempre 1 ponto conforme regra

            // Criar resposta
            var resposta = new Respostas
            {
                TentativaId = tentativaId,
                QuestaoId = request.QuestaoId,
                OpcaoSelecionadaId = request.OpcaoSelecionadaId,
                TextoResposta = request.TextoResposta,
                Correta = respostaCorreta,
                PontosGanhos = pontosGanhos,
                DataResposta = DateTime.UtcNow
            };

            _context.Respostas.Add(resposta);

            // Atualizar pontuação da tentativa
            tentativa.Pontuacao = (tentativa.Pontuacao ?? 0) + pontosGanhos;
            _context.TentativasQuiz.Update(tentativa);

            await _context.SaveChangesAsync();

            // Verificar se é a última questão
            var questaoAtualIndex = tentativa.Quiz.Questoes.ToList().FindIndex(q => q.Id == request.QuestaoId);
            var proximaQuestao = tentativa.Quiz.Questoes.Skip(questaoAtualIndex + 1).FirstOrDefault();

            var response = new ResponderQuestaoResponseDTO
            {
                RespostaCorreta = respostaCorreta,
                PontosGanhos = pontosGanhos,
                RespostaCorretaTexto = respostaCorretaTexto,
                Feedback = respostaCorreta ? "Correta!" : "Incorreto!",
                QuizConcluido = proximaQuestao == null
            };

            // Se não é a última questão, retornar a próxima
            if (proximaQuestao != null)
            {
                response.ProximaQuestao = new QuestaoAtualDTO
                {
                    Id = proximaQuestao.Id,
                    TextoQuestao = proximaQuestao.TextoQuestao,
                    TipoQuestao = proximaQuestao.TipoQuestao.ToString(),
                    Pontos = proximaQuestao.Pontos,
                    OrdemIndice = proximaQuestao.OrdemIndice,
                    Opcoes = proximaQuestao.Opcoes.Select(op => new OpcaoRespostaDTO
                    {
                        Id = op.Id,
                        TextoOpcao = op.TextoOpcao,
                        OrdemIndice = op.OrdemIndice
                    }).ToList()
                };
            }
            else
            {
                // Quiz concluído - calcular resultado final
                response.ResultadoFinal = await FinalizarQuizInternoAsync(tentativa);
            }

            return response;
        }

        public async Task<ProgressoQuizDTO> ObterProgressoQuizAsync(int usuarioId, int tentativaId)
        {
            var tentativa = await _context.TentativasQuiz
                .Include(t => t.Quiz)
                .ThenInclude(q => q.Questoes.Where(quest => quest.Ativo).OrderBy(quest => quest.OrdemIndice))
                .Include(t => t.Respostas)
                .FirstOrDefaultAsync(t => t.Id == tentativaId && t.UsuarioId == usuarioId);

            if (tentativa == null)
                throw new ArgumentException("Tentativa não encontrada");

            var questaoAtual = tentativa.Respostas.Count + 1;
            var totalQuestoes = tentativa.Quiz.Questoes.Count;
            var percentualCompleto = (decimal)questaoAtual / totalQuestoes * 100;
            var tempoGasto = (int)(DateTime.UtcNow - tentativa.DataInicio).TotalSeconds;

            return new ProgressoQuizDTO
            {
                QuestaoAtual = questaoAtual,
                TotalQuestoes = totalQuestoes,
                PercentualCompleto = Math.Round(percentualCompleto, 1),
                PontuacaoAtual = (int)(tentativa.Pontuacao ?? 0),
                TempoGasto = tempoGasto
            };
        }

        public async Task<ResultadoQuizDTO> FinalizarQuizAsync(int usuarioId, int tentativaId)
        {
            var tentativa = await _context.TentativasQuiz
                .Include(t => t.Quiz)
                .ThenInclude(q => q.Questoes.Where(quest => quest.Ativo))
                .Include(t => t.Respostas)
                .FirstOrDefaultAsync(t => t.Id == tentativaId && t.UsuarioId == usuarioId && !t.Concluida);

            if (tentativa == null)
                throw new ArgumentException("Tentativa não encontrada ou já concluída");

            return await FinalizarQuizInternoAsync(tentativa);
        }

        // Ranking
        public async Task<RankingCompletoDTO> ObterRankingCompletoAsync(int usuarioId, string? busca = null)
        {
            try
            {
                // Obter dados dos rankings agrupados por usuário
                var rankingsData = await _context.RankingAlunos
                    .Include(r => r.Usuario)
                    .Where(r => r.Usuario.IsActive && r.Usuario.Role == Domain.Enums.UserRole.Aluno)
                    .ToListAsync();

                // Aplicar filtro de busca se fornecido
                if (!string.IsNullOrEmpty(busca))
                {
                    rankingsData = rankingsData.Where(r => 
                        r.Usuario.FirstName.Contains(busca) || 
                        r.Usuario.LastName.Contains(busca)).ToList();
                }

                // Agrupar por usuário e somar os pontos
                var rankingsAgrupados = rankingsData
                    .GroupBy(r => r.UsuarioId)
                    .Select(g => new
                    {
                        UsuarioId = g.Key,
                        Usuario = g.First().Usuario,
                        PontosTotal = g.Sum(r => r.PontosExperiencia),
                        QuizzesTotal = g.Sum(r => r.TotalQuizzes),
                        MediaGeral = g.Average(r => r.MediaPontuacao),
                        Sequencia = 0 // Pode ser calculado se necessário
                    })
                    .OrderByDescending(r => r.PontosTotal)
                    .ThenByDescending(r => r.MediaGeral)
                    .ToList();

                var rankings = rankingsAgrupados
                    .Select((r, index) => new RankingAlunoDTO
                    {
                        Posicao = index + 1,
                        UsuarioId = r.UsuarioId,
                        NomeCompleto = $"{r.Usuario.FirstName} {r.Usuario.LastName}",
                        Avatar = "", // Pode ser implementado posteriormente
                        Pontos = r.PontosTotal,
                        Quizzes = r.QuizzesTotal,
                        Media = Math.Round(r.MediaGeral, 2),
                        Sequencia = r.Sequencia
                    })
                    .ToList();

                var posicaoUsuarioLogado = rankings
                    .FirstOrDefault(r => r.UsuarioId == usuarioId)?.Posicao ?? 0;

                return new RankingCompletoDTO
                {
                    Alunos = rankings,
                    TotalAlunos = rankings.Count,
                    PosicaoUsuarioLogado = posicaoUsuarioLogado
                };
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Erro no ranking: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        // Perfil
        public async Task<PerfilAlunoDTO> ObterPerfilAsync(int usuarioId)
        {
            var usuario = await _userRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
                throw new ArgumentException("Usuário não encontrado");

            var estatisticas = await ObterEstatisticasPerfilAsync(usuarioId);

            return new PerfilAlunoDTO
            {
                Id = usuario.Id,
                Nome = usuario.FirstName,
                Sobrenome = usuario.LastName,
                NomeCompleto = $"{usuario.FirstName} {usuario.LastName}",
                Email = usuario.Email,
                Funcao = usuario.Role.ToString(),
                CPF = usuario.CPF ?? "",
                DataNascimento = usuario.DataNascimento,
                DataCriacao = usuario.CreatedAt,
                Estatisticas = estatisticas
            };
        }

        public async Task<PerfilAlunoDTO> AtualizarPerfilAsync(int usuarioId, AtualizarPerfilRequestDTO request)
        {
            var usuario = await _userRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
                throw new ArgumentException("Usuário não encontrado");

            usuario.FirstName = request.Nome;
            usuario.LastName = request.Sobrenome;
            usuario.CPF = request.CPF;
            usuario.DataNascimento = request.DataNascimento;
            usuario.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(usuario);

            return await ObterPerfilAsync(usuarioId);
        }

        public async Task<List<DesempenhoQuizDTO>> ObterDesempenhoAsync(int usuarioId)
        {
            var desempenhos = await _context.RelatoriosPerformance
                .Include(r => r.Quiz)
                .ThenInclude(q => q.Categoria)
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(r => r.DataCriacao)
                .Select(r => new DesempenhoQuizDTO
                {
                    QuizId = r.QuizId,
                    TituloQuiz = r.Quiz.Titulo,
                    Categoria = r.Quiz.Categoria.Nome,
                    PercentualAcerto = r.Percentual,
                    Pontuacao = r.RespostasCorretas, // Cada questão vale 1 ponto
                    PontuacaoMaxima = r.TotalQuestoes,
                    DataConclusao = r.CreatedAt,
                    TempoGasto = r.TempoGasto
                })
                .ToListAsync();

            return desempenhos;
        }

        public async Task<List<AtividadeRecenteDTO>> ObterAtividadesRecentesAsync(int usuarioId)
        {
            var atividades = new List<AtividadeRecenteDTO>();

            // Quizzes concluídos
            var quizzesConcluidos = await _context.TentativasQuiz
                .Include(t => t.Quiz)
                .Where(t => t.UsuarioId == usuarioId && t.Concluida)
                .OrderByDescending(t => t.DataConclusao)
                .Take(5)
                .ToListAsync();

            foreach (var tentativa in quizzesConcluidos)
            {
                atividades.Add(new AtividadeRecenteDTO
                {
                    Id = tentativa.Id,
                    Tipo = "QuizConcluido",
                    Descricao = $"Você concluiu o '{tentativa.Quiz.Titulo}' com {tentativa.Pontuacao}/{tentativa.PontuacaoMaxima} pontos",
                    Data = tentativa.DataConclusao ?? tentativa.DataInicio,
                    Icone = "check-circle",
                    Cor = "green"
                });
            }

            // Quizzes iniciados
            var quizzesIniciados = await _context.TentativasQuiz
                .Include(t => t.Quiz)
                .Where(t => t.UsuarioId == usuarioId && !t.Concluida)
                .OrderByDescending(t => t.DataInicio)
                .Take(3)
                .ToListAsync();

            foreach (var tentativa in quizzesIniciados)
            {
                atividades.Add(new AtividadeRecenteDTO
                {
                    Id = tentativa.Id,
                    Tipo = "QuizIniciado",
                    Descricao = $"Você iniciou o '{tentativa.Quiz.Titulo}'",
                    Data = tentativa.DataInicio,
                    Icone = "play-circle",
                    Cor = "blue"
                });
            }

            return atividades
                .OrderByDescending(a => a.Data)
                .Take(10)
                .ToList();
        }

        // Métodos auxiliares privados
        private async Task<int> CalcularSequenciaAsync(int usuarioId)
        {
            var tentativasPorDia = await _context.TentativasQuiz
                .Where(t => t.UsuarioId == usuarioId && t.Concluida)
                .GroupBy(t => t.DataConclusao.Value.Date)
                .Select(g => g.Key)
                .OrderByDescending(d => d)
                .ToListAsync();

            if (!tentativasPorDia.Any())
                return 0;

            int sequencia = 0;
            var dataAtual = DateTime.UtcNow.Date;

            foreach (var data in tentativasPorDia)
            {
                if (data == dataAtual.AddDays(-sequencia))
                    sequencia++;
                else
                    break;
            }

            return sequencia;
        }

        private async Task<List<QuizRecenteDTO>> ObterQuizzesRecentesAsync(int usuarioId)
        {
            return await _context.TentativasQuiz
                .Include(t => t.Quiz)
                .ThenInclude(q => q.Categoria)
                .Where(t => t.UsuarioId == usuarioId && t.Concluida)
                .OrderByDescending(t => t.DataConclusao)
                .Take(5)
                .Select(t => new QuizRecenteDTO
                {
                    QuizId = t.QuizId,
                    Titulo = t.Quiz.Titulo,
                    Categoria = t.Quiz.Categoria.Nome,
                    PercentualAcerto = (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100,
                    DataConclusao = t.DataConclusao ?? t.DataInicio
                })
                .ToListAsync();
        }

        private async Task<EstatisticasPerfilDTO> ObterEstatisticasPerfilAsync(int usuarioId)
        {
            var tentativasConcluidas = await _context.TentativasQuiz
                .Where(t => t.UsuarioId == usuarioId && t.Concluida)
                .ToListAsync();

            var quizzesCompletos = tentativasConcluidas.Count;
            var mediaGeral = tentativasConcluidas.Any() ? 
                tentativasConcluidas.Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100) : 0;

            var sequencia = await CalcularSequenciaAsync(usuarioId);

            var ranking = await _context.RankingAlunos
                .Where(r => r.UsuarioId == usuarioId)
                .FirstOrDefaultAsync();

            var pontos = ranking?.PontosExperiencia ?? 0;

            return new EstatisticasPerfilDTO
            {
                QuizzesCompletos = quizzesCompletos,
                MediaGeral = Math.Round(mediaGeral, 1),
                Sequencia = sequencia,
                Pontos = pontos
            };
        }

        private async Task<ResultadoQuizDTO> FinalizarQuizInternoAsync(TentativasQuiz tentativa)
        {
            tentativa.Concluida = true;
            tentativa.DataConclusao = DateTime.UtcNow;
            tentativa.TempoGasto = (int)(DateTime.UtcNow - tentativa.DataInicio).TotalSeconds;

            _context.TentativasQuiz.Update(tentativa);

            // Criar relatório de performance
            var respostas = await _context.Respostas
                .Where(r => r.TentativaId == tentativa.Id)
                .ToListAsync();

            var relatorio = new RelatoriosPerformance
            {
                UsuarioId = tentativa.UsuarioId,
                QuizId = tentativa.QuizId,
                TentativaId = tentativa.Id,
                TotalQuestoes = tentativa.Quiz.Questoes.Count,
                RespostasCorretas = respostas.Count(r => r.Correta == true),
                RespostasErradas = respostas.Count(r => r.Correta == false),
                Percentual = (decimal)respostas.Count(r => r.Correta == true) / tentativa.Quiz.Questoes.Count * 100,
                TempoGasto = tentativa.TempoGasto ?? 0,
                DataCriacao = DateTime.UtcNow
            };

            _context.RelatoriosPerformance.Add(relatorio);

            // Atualizar ranking
            await AtualizarRankingAsync(tentativa.UsuarioId, tentativa.Quiz.CategoriaId);

            await _context.SaveChangesAsync();

            return new ResultadoQuizDTO
            {
                TentativaId = tentativa.Id,
                PontuacaoFinal = (int)(tentativa.Pontuacao ?? 0),
                PontuacaoMaxima = (int)(tentativa.PontuacaoMaxima ?? 0),
                PercentualAcerto = relatorio.Percentual,
                TempoGasto = tentativa.TempoGasto ?? 0,
                TotalQuestoes = relatorio.TotalQuestoes,
                RespostasCorretas = relatorio.RespostasCorretas,
                RespostasErradas = relatorio.RespostasErradas,
                DataConclusao = tentativa.DataConclusao ?? DateTime.UtcNow
            };
        }

        private async Task AtualizarRankingAsync(int usuarioId, int categoriaId)
        {
            var ranking = await _context.RankingAlunos
                .FirstOrDefaultAsync(r => r.UsuarioId == usuarioId && r.CategoriaId == categoriaId);

            if (ranking == null)
            {
                ranking = new RankingAlunos
                {
                    UsuarioId = usuarioId,
                    CategoriaId = categoriaId,
                    DataAtualizacao = DateTime.UtcNow
                };
                _context.RankingAlunos.Add(ranking);
                await _context.SaveChangesAsync(); // Salvar primeiro para obter ID
            }

            // Calcular estatísticas atualizadas
            var tentativasConcluidas = await _context.TentativasQuiz
                .Where(t => t.UsuarioId == usuarioId && t.Concluida && t.Quiz.CategoriaId == categoriaId)
                .ToListAsync();

            ranking.TotalQuizzes = tentativasConcluidas.Count;
            ranking.PontuacaoTotal = tentativasConcluidas.Sum(t => t.Pontuacao ?? 0);
            ranking.MediaPontuacao = tentativasConcluidas.Any() ? 
                tentativasConcluidas.Average(t => (t.Pontuacao ?? 0) / (t.PontuacaoMaxima ?? 1) * 100) : 0;
            ranking.PontosExperiencia = (int)ranking.PontuacaoTotal;
            ranking.DataAtualizacao = DateTime.UtcNow;

            _context.RankingAlunos.Update(ranking);

            // Recalcular posições de todos os usuários nesta categoria
            await RecalcularPosicoesRankingAsync(categoriaId);
        }

        private async Task RecalcularPosicoesRankingAsync(int categoriaId)
        {
            var rankings = await _context.RankingAlunos
                .Where(r => r.CategoriaId == categoriaId)
                .OrderByDescending(r => r.PontuacaoTotal)
                .ThenByDescending(r => r.MediaPontuacao)
                .ToListAsync();

            for (int i = 0; i < rankings.Count; i++)
            {
                rankings[i].PosicaoRanking = i + 1;
            }

            _context.RankingAlunos.UpdateRange(rankings);
        }
    }
}
