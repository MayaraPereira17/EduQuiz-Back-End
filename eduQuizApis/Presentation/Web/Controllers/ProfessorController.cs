using eduQuizApis.Application.DTOs;
using eduQuizApis.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eduQuizApis.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/professor")]
    [Authorize(Policy = "ProfessorOnly")] // Todas as rotas exigem autenticação e função de Professor
    public class ProfessorController : ControllerBase
    {
        private readonly IProfessorService _professorService;

        public ProfessorController(IProfessorService professorService)
        {
            _professorService = professorService;
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
        /// Obtém os dados do dashboard do professor (estatísticas gerais, quizzes criados, etc.)
        /// </summary>
        /// <returns>Dados do dashboard incluindo quizzes criados, média dos alunos, total de alunos e tentativas</returns>
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardProfessorDTO>> ObterDashboard()
        {
            try
            {
                var professorId = ObterUsuarioId();
                var dashboard = await _professorService.ObterDashboardAsync(professorId);
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

        #region Gerenciamento de Quizzes

        /// <summary>
        /// Lista todos os quizzes criados pelo professor
        /// </summary>
        /// <param name="busca">Termo opcional para buscar por título ou descrição</param>
        /// <returns>Lista de quizzes com informações básicas e estatísticas</returns>
        [HttpGet("quizzes")]
        public async Task<ActionResult<List<QuizListagemDTO>>> ObterMeusQuizzes([FromQuery] string? busca = null)
        {
            try
            {
                var professorId = ObterUsuarioId();
                var quizzes = await _professorService.ObterMeusQuizzesAsync(professorId, busca);
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obtém um quiz específico com todas as suas questões e opções
        /// </summary>
        /// <param name="quizId">ID do quiz</param>
        /// <returns>Dados completos do quiz incluindo questões e opções</returns>
        [HttpGet("quizzes/{quizId}")]
        public async Task<ActionResult<QuizCompletoDTO>> ObterQuizPorId(int quizId)
        {
            try
            {
                var professorId = ObterUsuarioId();
                var quiz = await _professorService.ObterQuizPorIdAsync(professorId, quizId);
                return Ok(quiz);
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
        /// Cria um novo quiz com suas questões e opções
        /// </summary>
        /// <param name="request">Dados do quiz a ser criado</param>
        /// <returns>Dados do quiz criado</returns>
        [HttpPost("quizzes")]
        public async Task<ActionResult<CriarQuizResponseDTO>> CriarQuiz([FromBody] CriarQuizRequestDTO request)
        {
            try
            {
                Console.WriteLine("=== INÍCIO DA CRIAÇÃO DE QUIZ ===");
                Console.WriteLine($"Request recebido: {System.Text.Json.JsonSerializer.Serialize(request)}");
                
                var professorId = ObterUsuarioId();
                Console.WriteLine($"Professor ID extraído: {professorId}");
                
                var resultado = await _professorService.CriarQuizAsync(professorId, request);
                Console.WriteLine("=== QUIZ CRIADO COM SUCESSO ===");
                return CreatedAtAction(nameof(ObterQuizPorId), new { quizId = resultado.Id }, resultado);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ERRO de argumento: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO GERAL na criação de quiz: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza um quiz existente
        /// </summary>
        /// <param name="quizId">ID do quiz</param>
        /// <param name="request">Dados atualizados do quiz</param>
        /// <returns>Dados do quiz atualizado</returns>
        [HttpPut("quizzes/{quizId}")]
        public async Task<ActionResult<AtualizarQuizResponseDTO>> AtualizarQuiz(int quizId, [FromBody] AtualizarQuizRequestDTO request)
        {
            try
            {
                var professorId = ObterUsuarioId();
                var resultado = await _professorService.AtualizarQuizAsync(professorId, quizId, request);
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

        /// <summary>
        /// Deleta um quiz (ou desativa se houver tentativas)
        /// </summary>
        /// <param name="quizId">ID do quiz</param>
        /// <returns>Confirmação da operação</returns>
        [HttpDelete("quizzes/{quizId}")]
        public async Task<ActionResult<DeletarQuizResponseDTO>> DeletarQuiz(int quizId)
        {
            try
            {
                var professorId = ObterUsuarioId();
                var resultado = await _professorService.DeletarQuizAsync(professorId, quizId);
                return Ok(resultado);
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
        /// Publica um quiz (torna público para os alunos)
        /// </summary>
        /// <param name="quizId">ID do quiz</param>
        /// <returns>Confirmação da publicação</returns>
        [HttpPost("quizzes/{quizId}/publicar")]
        public async Task<ActionResult<AtualizarQuizResponseDTO>> PublicarQuiz(int quizId)
        {
            try
            {
                var professorId = ObterUsuarioId();
                var resultado = await _professorService.PublicarQuizAsync(professorId, quizId);
                return Ok(resultado);
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
        /// Despublica um quiz (remove da lista pública)
        /// </summary>
        /// <param name="quizId">ID do quiz</param>
        /// <returns>Confirmação da despublicação</returns>
        [HttpPost("quizzes/{quizId}/despublicar")]
        public async Task<ActionResult<AtualizarQuizResponseDTO>> DespublicarQuiz(int quizId)
        {
            try
            {
                var professorId = ObterUsuarioId();
                var resultado = await _professorService.DespublicarQuizAsync(professorId, quizId);
                return Ok(resultado);
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

        #region Estatísticas

        /// <summary>
        /// Obtém estatísticas detalhadas de um quiz específico
        /// </summary>
        /// <param name="quizId">ID do quiz</param>
        /// <returns>Estatísticas completas do quiz</returns>
        [HttpGet("quizzes/{quizId}/estatisticas")]
        public async Task<ActionResult<EstatisticasQuizDTO>> ObterEstatisticasQuiz(int quizId)
        {
            try
            {
                var professorId = ObterUsuarioId();
                var estatisticas = await _professorService.ObterEstatisticasQuizAsync(professorId, quizId);
                return Ok(estatisticas);
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
        /// Obtém estatísticas detalhadas das questões de um quiz
        /// </summary>
        /// <param name="quizId">ID do quiz</param>
        /// <returns>Estatísticas de cada questão</returns>
        [HttpGet("quizzes/{quizId}/estatisticas/questoes")]
        public async Task<ActionResult<List<EstatisticaQuestaoDTO>>> ObterEstatisticasQuestoes(int quizId)
        {
            try
            {
                var professorId = ObterUsuarioId();
                var estatisticas = await _professorService.ObterEstatisticasQuestoesAsync(professorId, quizId);
                return Ok(estatisticas);
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
        /// Obtém as tentativas recentes de um quiz
        /// </summary>
        /// <param name="quizId">ID do quiz</param>
        /// <returns>Lista das tentativas recentes</returns>
        [HttpGet("quizzes/{quizId}/tentativas")]
        public async Task<ActionResult<List<TentativaResumoDTO>>> ObterTentativasQuiz(int quizId)
        {
            try
            {
                var professorId = ObterUsuarioId();
                var tentativas = await _professorService.ObterTentativasQuizAsync(professorId, quizId);
                return Ok(tentativas);
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

        #region Perfil

        /// <summary>
        /// Obtém os dados do perfil do professor
        /// </summary>
        /// <returns>Informações pessoais e estatísticas do professor</returns>
        [HttpGet("perfil")]
        public async Task<ActionResult<PerfilProfessorDTO>> ObterPerfil()
        {
            try
            {
                var professorId = ObterUsuarioId();
                var perfil = await _professorService.ObterPerfilAsync(professorId);
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
        /// Atualiza os dados do perfil do professor
        /// </summary>
        /// <param name="request">Dados atualizados do perfil</param>
        /// <returns>Perfil atualizado</returns>
        [HttpPut("perfil")]
        public async Task<ActionResult<PerfilProfessorDTO>> AtualizarPerfil([FromBody] AtualizarPerfilProfessorRequestDTO request)
        {
            try
            {
                var professorId = ObterUsuarioId();
                var perfil = await _professorService.AtualizarPerfilAsync(professorId, request);
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

        #endregion

        #region Categorias

        /// <summary>
        /// Lista todas as categorias disponíveis
        /// </summary>
        /// <returns>Lista de categorias ativas</returns>
        [HttpGet("categorias")]
        public async Task<ActionResult<List<CategoriaDTO>>> ObterCategorias()
        {
            try
            {
                var categorias = await _professorService.ObterCategoriasAsync();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obtém uma categoria específica
        /// </summary>
        /// <param name="categoriaId">ID da categoria</param>
        /// <returns>Dados da categoria</returns>
        [HttpGet("categorias/{categoriaId}")]
        public async Task<ActionResult<CategoriaDTO>> ObterCategoriaPorId(int categoriaId)
        {
            try
            {
                var categoria = await _professorService.ObterCategoriaPorIdAsync(categoriaId);
                return Ok(categoria);
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
    }
}
