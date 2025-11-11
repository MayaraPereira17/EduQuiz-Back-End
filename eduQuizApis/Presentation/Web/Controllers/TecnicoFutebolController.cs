using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using eduQuizApis.Application.Interfaces;
using eduQuizApis.Application.DTOs;
using System.Security.Claims;

namespace eduQuizApis.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/tecnico")]
    [Authorize(Policy = "TecnicoFutebolOnly")] // Todas as rotas exigem autenticação e função de Técnico
    public class TecnicoFutebolController : ControllerBase
    {
        private readonly ITecnicoFutebolService _tecnicoService;

        public TecnicoFutebolController(ITecnicoFutebolService tecnicoService)
        {
            _tecnicoService = tecnicoService;
        }

        private int ObterUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Usuário não autenticado");
            }
            return userId;
        }

        /// <summary>
        /// Obtém o dashboard do técnico de futebol
        /// </summary>
        /// <returns>Dashboard com estatísticas dos alunos</returns>
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardTecnicoDTO>> ObterDashboard()
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var dashboard = await _tecnicoService.ObterDashboardAsync(tecnicoId);
                return Ok(dashboard);
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
        /// Obtém a lista de alunos para gerenciamento e ranking
        /// </summary>
        /// <param name="busca">Termo de busca opcional</param>
        /// <returns>Lista de alunos com ranking</returns>
        [HttpGet("alunos")]
        public async Task<ActionResult<GerenciarAlunosDTO>> ObterGerenciarAlunos([FromQuery] string? busca = null)
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var gerenciarAlunos = await _tecnicoService.ObterGerenciarAlunosAsync(tecnicoId, busca);
                return Ok(gerenciarAlunos);
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
        /// Obtém o relatório de desempenho dos alunos
        /// </summary>
        /// <returns>Relatório detalhado de desempenho</returns>
        [HttpGet("relatorio-desempenho")]
        public async Task<ActionResult<RelatorioDesempenhoDTO>> ObterRelatorioDesempenho()
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var relatorio = await _tecnicoService.ObterRelatorioDesempenhoAsync(tecnicoId);
                return Ok(relatorio);
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
        /// Obtém o perfil do técnico
        /// </summary>
        /// <returns>Dados do perfil do técnico</returns>
        [HttpGet("perfil")]
        public async Task<ActionResult<PerfilTecnicoDTO>> ObterPerfil()
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var perfil = await _tecnicoService.ObterPerfilAsync(tecnicoId);
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
        /// Atualiza o perfil do técnico
        /// </summary>
        /// <param name="request">Dados para atualização</param>
        /// <returns>Perfil atualizado</returns>
        [HttpPut("perfil")]
        public async Task<ActionResult<PerfilTecnicoDTO>> AtualizarPerfil([FromBody] AtualizarPerfilTecnicoRequestDTO request)
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var perfil = await _tecnicoService.AtualizarPerfilAsync(tecnicoId, request);
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
        /// Atualiza os dados de um aluno
        /// </summary>
        /// <param name="alunoId">ID do aluno a ser atualizado</param>
        /// <param name="request">Dados para atualização (nome, email, idade)</param>
        /// <returns>Dados atualizados do aluno</returns>
        [HttpPut("alunos/{alunoId}")]
        public async Task<ActionResult<AlunoRankingDTO>> AtualizarAluno(int alunoId, [FromBody] AtualizarAlunoRequestDTO request)
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var aluno = await _tecnicoService.AtualizarAlunoAsync(tecnicoId, alunoId, request);
                return Ok(aluno);
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.Contains("não encontrado"))
                    return NotFound(new { message = ex.Message });
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Token inválido ou expirado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar aluno. Tente novamente mais tarde." });
            }
        }

        /// <summary>
        /// Exclui um aluno (soft delete)
        /// </summary>
        /// <param name="alunoId">ID do aluno a ser excluído</param>
        /// <returns>Mensagem de sucesso</returns>
        [HttpDelete("alunos/{alunoId}")]
        public async Task<ActionResult<ExcluirAlunoResponseDTO>> ExcluirAluno(int alunoId)
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var resultado = await _tecnicoService.ExcluirAlunoAsync(tecnicoId, alunoId);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.Contains("não encontrado"))
                    return NotFound(new { message = ex.Message });
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Token inválido ou expirado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao excluir aluno. Tente novamente mais tarde." });
            }
        }

        /// <summary>
        /// Lista todos os professores cadastrados no sistema
        /// </summary>
        /// <param name="busca">Termo de busca opcional para filtrar por nome ou email</param>
        /// <returns>Lista de professores com suas estatísticas</returns>
        [HttpGet("professores")]
        public async Task<ActionResult<GerenciarProfessoresDTO>> ObterProfessores([FromQuery] string? busca = null)
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var professores = await _tecnicoService.ObterProfessoresAsync(tecnicoId, busca);
                return Ok(professores);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Token inválido ou expirado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao obter lista de professores. Tente novamente mais tarde." });
            }
        }

        /// <summary>
        /// Atualiza os dados de um professor
        /// </summary>
        /// <param name="professorId">ID do professor a ser atualizado</param>
        /// <param name="request">Dados para atualização (nome, email, instituicao, areaEspecializacao)</param>
        /// <returns>Dados atualizados do professor</returns>
        [HttpPut("professores/{professorId}")]
        public async Task<ActionResult<ProfessorDTO>> AtualizarProfessor(int professorId, [FromBody] AtualizarProfessorRequestDTO request)
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var professor = await _tecnicoService.AtualizarProfessorAsync(tecnicoId, professorId, request);
                return Ok(professor);
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.Contains("não encontrado"))
                    return NotFound(new { message = ex.Message });
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Token inválido ou expirado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao atualizar professor. Tente novamente mais tarde." });
            }
        }

        /// <summary>
        /// Exclui um professor (soft delete)
        /// </summary>
        /// <param name="professorId">ID do professor a ser excluído</param>
        /// <returns>Mensagem de sucesso</returns>
        [HttpDelete("professores/{professorId}")]
        public async Task<ActionResult<ExcluirProfessorResponseDTO>> ExcluirProfessor(int professorId)
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var resultado = await _tecnicoService.ExcluirProfessorAsync(tecnicoId, professorId);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.Contains("não encontrado"))
                    return NotFound(new { message = ex.Message });
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Token inválido ou expirado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao excluir professor. Tente novamente mais tarde." });
            }
        }
    }
}
