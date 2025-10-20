using eduQuizApis.Application.DTOs;
using eduQuizApis.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eduQuizApis.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/aluno")]
    [Authorize(Policy = "AlunoOnly")] // Todas as rotas exigem autenticação e função de Aluno
    public class AlunoController : ControllerBase
    {
        private readonly IAlunoService _alunoService;

        public AlunoController(IAlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        /// <summary>
        /// Obtém o ID do usuário logado a partir do token JWT
        /// </summary>
        private int ObterUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Usuário não autenticado");
            }
            return userId;
        }

        #region Dashboard

        /// <summary>
        /// Obtém os dados do dashboard do aluno (estatísticas gerais, quizzes recentes, etc.)
        /// </summary>
        /// <returns>Dados do dashboard incluindo quizzes completos, média geral, posição no ranking, sequência e pontos</returns>
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardAlunoDTO>> ObterDashboard()
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var dashboard = await _alunoService.ObterDashboardAsync(usuarioId);
                return Ok(dashboard);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        #endregion

        #region Quiz

        /// <summary>
        /// Lista todos os quizzes disponíveis para o aluno (endpoint principal)
        /// </summary>
        /// <returns>Lista de quizzes com informações básicas (título, descrição, categoria, dificuldade, tempo limite, etc.)</returns>
        [HttpGet("quizzes")]
        public async Task<ActionResult<List<QuizDisponivelDTO>>> ObterQuizzes()
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var quizzes = await _alunoService.ObterQuizzesDisponiveisAsync(usuarioId);
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Lista todos os quizzes disponíveis para o aluno (endpoint alternativo)
        /// </summary>
        /// <returns>Lista de quizzes com informações básicas (título, descrição, categoria, dificuldade, tempo limite, etc.)</returns>
        [HttpGet("quizzes/disponiveis")]
        public async Task<ActionResult<List<QuizDisponivelDTO>>> ObterQuizzesDisponiveis()
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var quizzes = await _alunoService.ObterQuizzesDisponiveisAsync(usuarioId);
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obtém detalhes de um quiz específico para o aluno
        /// </summary>
        /// <param name="quizId">ID do quiz</param>
        /// <returns>Detalhes completos do quiz incluindo questões e opções</returns>
        [HttpGet("quizzes/{quizId}")]
        public async Task<ActionResult<QuizDetalhesDTO>> ObterQuizPorId(int quizId)
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var quiz = await _alunoService.ObterQuizPorIdAsync(usuarioId, quizId);
                return Ok(quiz);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Inicia uma nova tentativa de quiz para o aluno
        /// </summary>
        /// <param name="request">Dados do quiz a ser iniciado</param>
        /// <returns>Detalhes da primeira questão e informações da tentativa criada</returns>
        [HttpPost("quizzes/iniciar")]
        public async Task<ActionResult<IniciarQuizResponseDTO>> IniciarQuiz([FromBody] IniciarQuizRequestDTO request)
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var resultado = await _alunoService.IniciarQuizAsync(usuarioId, request);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Responde a um quiz completo (envia todas as respostas de uma vez)
        /// </summary>
        /// <param name="quizId">ID do quiz</param>
        /// <param name="request">Todas as respostas do quiz</param>
        /// <returns>Resultado completo do quiz</returns>
        [HttpPost("quizzes/{quizId}/responder")]
        public async Task<ActionResult<ResponderQuizResponseDTO>> ResponderQuiz(int quizId, [FromBody] ResponderQuizRequestDTO request)
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var resultado = await _alunoService.ResponderQuizAsync(usuarioId, quizId, request);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Responde a uma questão específica de um quiz em andamento
        /// </summary>
        /// <param name="tentativaId">ID da tentativa de quiz</param>
        /// <param name="request">Resposta da questão (opção selecionada ou texto)</param>
        /// <returns>Feedback da resposta e próxima questão (se houver)</returns>
        [HttpPost("tentativas/{tentativaId}/responder")]
        public async Task<ActionResult<ResponderQuestaoResponseDTO>> ResponderQuestao(int tentativaId, [FromBody] ResponderQuestaoRequestDTO request)
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var resultado = await _alunoService.ResponderQuestaoAsync(usuarioId, tentativaId, request);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obtém o progresso atual de uma tentativa de quiz
        /// </summary>
        /// <param name="tentativaId">ID da tentativa de quiz</param>
        /// <returns>Informações do progresso (questão atual, total, percentual, pontuação, tempo gasto)</returns>
        [HttpGet("tentativas/{tentativaId}/progresso")]
        public async Task<ActionResult<ProgressoQuizDTO>> ObterProgressoQuiz(int tentativaId)
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var progresso = await _alunoService.ObterProgressoQuizAsync(usuarioId, tentativaId);
                return Ok(progresso);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Finaliza uma tentativa de quiz e calcula o resultado final
        /// </summary>
        /// <param name="tentativaId">ID da tentativa de quiz</param>
        /// <returns>Resultado final do quiz (pontuação, percentual de acerto, tempo gasto, etc.)</returns>
        [HttpPost("tentativas/{tentativaId}/finalizar")]
        public async Task<ActionResult<ResultadoQuizDTO>> FinalizarQuiz(int tentativaId)
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var resultado = await _alunoService.FinalizarQuizAsync(usuarioId, tentativaId);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        #endregion

        #region Ranking

        /// <summary>
        /// Obtém o ranking completo de alunos com opção de busca por nome
        /// </summary>
        /// <param name="busca">Termo de busca opcional para filtrar por nome</param>
        /// <returns>Lista de alunos ranqueados com suas estatísticas e posição do usuário logado</returns>
        [HttpGet("ranking")]
        public async Task<ActionResult<RankingCompletoDTO>> ObterRankingCompleto([FromQuery] string? busca = null)
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var ranking = await _alunoService.ObterRankingCompletoAsync(usuarioId, busca);
                return Ok(ranking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        #endregion

        #region Perfil

        /// <summary>
        /// Obtém os dados completos do perfil do aluno
        /// </summary>
        /// <returns>Informações pessoais e estatísticas do aluno</returns>
        [HttpGet("perfil")]
        public async Task<ActionResult<PerfilAlunoDTO>> ObterPerfil()
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var perfil = await _alunoService.ObterPerfilAsync(usuarioId);
                return Ok(perfil);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Atualiza os dados do perfil do aluno
        /// </summary>
        /// <param name="request">Dados atualizados do perfil</param>
        /// <returns>Perfil atualizado</returns>
        [HttpPut("perfil")]
        public async Task<ActionResult<PerfilAlunoDTO>> AtualizarPerfil([FromBody] AtualizarPerfilRequestDTO request)
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var perfil = await _alunoService.AtualizarPerfilAsync(usuarioId, request);
                return Ok(perfil);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obtém o desempenho do aluno em todos os quizzes realizados
        /// </summary>
        /// <returns>Lista de quizzes com percentual de acerto, pontuação e tempo gasto</returns>
        [HttpGet("perfil/desempenho")]
        public async Task<ActionResult<List<DesempenhoQuizDTO>>> ObterDesempenho()
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var desempenho = await _alunoService.ObterDesempenhoAsync(usuarioId);
                return Ok(desempenho);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obtém as atividades recentes do aluno (quizzes concluídos, iniciados, conquistas, etc.)
        /// </summary>
        /// <returns>Lista de atividades recentes com descrição e data</returns>
        [HttpGet("perfil/atividades-recentes")]
        public async Task<ActionResult<List<AtividadeRecenteDTO>>> ObterAtividadesRecentes()
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var atividades = await _alunoService.ObterAtividadesRecentesAsync(usuarioId);
                return Ok(atividades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        #endregion
    }
}
