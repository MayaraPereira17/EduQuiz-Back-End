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
        /// Exporta relatório de desempenho em PDF ou Excel
        /// </summary>
        /// <param name="formato">Formato do arquivo: "pdf" ou "excel"</param>
        /// <param name="quantidade">Quantidade de alunos a incluir (opcional, top N do ranking)</param>
        /// <returns>Arquivo PDF ou Excel</returns>
        [HttpGet("relatorio-desempenho/exportar")]
        public async Task<IActionResult> ExportarRelatorio([FromQuery] string formato, [FromQuery] int? quantidade = null)
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var arquivo = await _tecnicoService.ExportarRelatorioAsync(tecnicoId, formato, quantidade);

                if (formato?.ToLower() == "pdf")
                {
                    return File(arquivo, "application/pdf", $"relatorio-desempenho-{DateTime.Now:yyyyMMddHHmmss}.pdf");
                }
                else if (formato?.ToLower() == "excel")
                {
                    return File(arquivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                        $"relatorio-desempenho-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
                }
                else
                {
                    return BadRequest("Formato inválido. Use 'pdf' ou 'excel'");
                }
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

        /// <summary>
        /// Lista todos os times criados pelo técnico
        /// </summary>
        /// <returns>Lista de times com seus jogadores</returns>
        [HttpGet("times")]
        public async Task<ActionResult<GerenciarTimesDTO>> ObterTimes()
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var times = await _tecnicoService.ObterTimesAsync(tecnicoId);
                return Ok(times);
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
                return StatusCode(500, new { message = "Erro ao obter lista de times. Tente novamente mais tarde." });
            }
        }

        /// <summary>
        /// Cria um novo time com os jogadores especificados
        /// </summary>
        /// <param name="request">Dados do time (nome e IDs dos jogadores)</param>
        /// <returns>Time criado com seus jogadores</returns>
        [HttpPost("times")]
        public async Task<ActionResult<TimeDTO>> CriarTime([FromBody] CriarTimeRequestDTO request)
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var time = await _tecnicoService.CriarTimeAsync(tecnicoId, request);
                return Ok(time);
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
                return StatusCode(500, new { message = "Erro ao criar time. Tente novamente mais tarde." });
            }
        }

        /// <summary>
        /// Adiciona um jogador a um time existente
        /// </summary>
        /// <param name="timeId">ID do time</param>
        /// <param name="request">Dados do jogador (alunoId)</param>
        /// <returns>Time atualizado com o novo jogador</returns>
        [HttpPost("times/{timeId}/jogadores")]
        public async Task<ActionResult<TimeDTO>> AdicionarJogadorAoTime(int timeId, [FromBody] AdicionarJogadorRequestDTO request)
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var time = await _tecnicoService.AdicionarJogadorAoTimeAsync(tecnicoId, timeId, request);
                return Ok(time);
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
                return StatusCode(500, new { message = "Erro ao adicionar jogador ao time. Tente novamente mais tarde." });
            }
        }

        /// <summary>
        /// Remove um jogador de um time
        /// </summary>
        /// <param name="timeId">ID do time</param>
        /// <param name="jogadorId">ID do jogador no time</param>
        /// <returns>Mensagem de sucesso</returns>
        [HttpDelete("times/{timeId}/jogadores/{jogadorId}")]
        public async Task<ActionResult<RemoverJogadorResponseDTO>> RemoverJogadorDoTime(int timeId, int jogadorId)
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var resultado = await _tecnicoService.RemoverJogadorDoTimeAsync(tecnicoId, timeId, jogadorId);
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
                return StatusCode(500, new { message = "Erro ao remover jogador do time. Tente novamente mais tarde." });
            }
        }

        /// <summary>
        /// Deleta um time (soft delete)
        /// </summary>
        /// <param name="timeId">ID do time</param>
        /// <returns>Mensagem de sucesso</returns>
        [HttpDelete("times/{timeId}")]
        public async Task<ActionResult<DeletarTimeResponseDTO>> DeletarTime(int timeId)
        {
            try
            {
                var tecnicoId = ObterUsuarioId();
                var resultado = await _tecnicoService.DeletarTimeAsync(tecnicoId, timeId);
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
                return StatusCode(500, new { message = "Erro ao deletar time. Tente novamente mais tarde." });
            }
        }
    }
}
