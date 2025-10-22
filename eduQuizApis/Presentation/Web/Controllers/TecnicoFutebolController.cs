using Microsoft.AspNetCore.Mvc;
using eduQuizApis.Application.Interfaces;
using eduQuizApis.Application.DTOs;
using System.Security.Claims;

namespace eduQuizApis.Presentation.Web.Controllers
{
    [ApiController]
    [Route("api/tecnico")]
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
    }
}
