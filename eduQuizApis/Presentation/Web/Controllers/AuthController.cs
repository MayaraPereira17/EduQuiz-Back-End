using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using eduQuizApis.Application.DTOs;
using eduQuizApis.Application.Interfaces;

namespace eduQuizApis.Presentation.Web.Controllers
{
    /// <summary>
    /// Controller para operações de autenticação
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        /// <param name="request">Dados para registro</param>
        /// <returns>Token de autenticação e dados do usuário</returns>
        /// <response code="200">Registro realizado com sucesso</response>
        /// <response code="400">Dados inválidos ou usuário já existe</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("Requisição de registro recebida para email: {Email}", request.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dados inválidos na requisição de registro");
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(ApiResponse<AuthResponse>.ErrorResponse("Dados inválidos fornecidos", errors));
                }

                var result = await _authService.RegisterAsync(request);

                if (result == null)
                {
                    _logger.LogWarning("Falha no registro - usuário já existe: {Email}", request.Email);
                    return BadRequest(ApiResponse<AuthResponse>.ErrorResponse("Email ou nome de usuário já existem no sistema"));
                }

                _logger.LogInformation("Registro realizado com sucesso para usuário: {UserId}", result.User.Id);
                return Ok(ApiResponse<AuthResponse>.SuccessResponse("Usuário registrado com sucesso", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno durante registro para email: {Email}", request.Email);
                return StatusCode(500, ApiResponse<AuthResponse>.ErrorResponse("Erro interno do servidor. Tente novamente mais tarde."));
            }
        }

        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        /// <param name="request">Credenciais de login</param>
        /// <returns>Token de autenticação e dados do usuário</returns>
        /// <response code="200">Login realizado com sucesso</response>
        /// <response code="401">Credenciais inválidas</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Requisição de login recebida para email: {Email}", request.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dados inválidos na requisição de login");
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(ApiResponse<AuthResponse>.ErrorResponse("Dados inválidos fornecidos", errors));
                }

                var result = await _authService.LoginAsync(request);

                if (result == null)
                {
                    _logger.LogWarning("Falha no login - credenciais inválidas: {Email}", request.Email);
                    return Unauthorized(ApiResponse<AuthResponse>.ErrorResponse("Email ou senha incorretos"));
                }

                _logger.LogInformation("Login realizado com sucesso para usuário: {UserId}", result.User.Id);
                return Ok(ApiResponse<AuthResponse>.SuccessResponse("Login realizado com sucesso", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno durante login para email: {Email}", request.Email);
                return StatusCode(500, ApiResponse<AuthResponse>.ErrorResponse("Erro interno do servidor. Tente novamente mais tarde."));
            }
        }

        /// <summary>
        /// Obtém o perfil do usuário autenticado
        /// </summary>
        /// <returns>Dados do usuário</returns>
        /// <response code="200">Perfil obtido com sucesso</response>
        /// <response code="401">Usuário não autenticado</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    _logger.LogWarning("Usuário não autenticado tentando acessar perfil");
                    return Unauthorized(ApiResponse<UserDto>.ErrorResponse("Usuário não autenticado"));
                }

                _logger.LogInformation("Requisição de perfil para usuário: {UserId}", userId);

                var user = await _authService.GetUserProfileAsync(userId.Value);

                if (user == null)
                {
                    _logger.LogWarning("Perfil não encontrado para usuário: {UserId}", userId);
                    return NotFound(ApiResponse<UserDto>.ErrorResponse("Usuário não encontrado"));
                }

                _logger.LogInformation("Perfil obtido com sucesso para usuário: {UserId}", userId);
                return Ok(ApiResponse<UserDto>.SuccessResponse("Perfil obtido com sucesso", user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao obter perfil do usuário");
                return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("Erro interno do servidor. Tente novamente mais tarde."));
            }
        }

        /// <summary>
        /// Atualiza o perfil do usuário autenticado
        /// </summary>
        /// <param name="request">Dados para atualização</param>
        /// <returns>Dados atualizados do usuário</returns>
        /// <response code="200">Perfil atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Usuário não autenticado</response>
        [HttpPut("profile")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    _logger.LogWarning("Usuário não autenticado tentando atualizar perfil");
                    return Unauthorized(ApiResponse<UserDto>.ErrorResponse("Usuário não autenticado"));
                }

                _logger.LogInformation("Requisição de atualização de perfil para usuário: {UserId}", userId);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dados inválidos na requisição de atualização de perfil");
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(ApiResponse<UserDto>.ErrorResponse("Dados inválidos fornecidos", errors));
                }

                var result = await _authService.UpdateUserProfileAsync(userId.Value, request);

                if (result == null)
                {
                    _logger.LogWarning("Falha na atualização de perfil para usuário: {UserId}", userId);
                    return BadRequest(ApiResponse<UserDto>.ErrorResponse("Erro ao atualizar perfil. Verifique se o email não está sendo usado por outro usuário."));
                }

                _logger.LogInformation("Perfil atualizado com sucesso para usuário: {UserId}", userId);
                return Ok(ApiResponse<UserDto>.SuccessResponse("Perfil atualizado com sucesso", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao atualizar perfil do usuário");
                return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("Erro interno do servidor. Tente novamente mais tarde."));
            }
        }

        /// <summary>
        /// Altera a senha do usuário autenticado
        /// </summary>
        /// <param name="request">Dados para alteração de senha</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="200">Senha alterada com sucesso</response>
        /// <response code="400">Dados inválidos ou senha atual incorreta</response>
        /// <response code="401">Usuário não autenticado</response>
        [HttpPut("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    _logger.LogWarning("Usuário não autenticado tentando alterar senha");
                    return Unauthorized(ApiResponse.ErrorResponse("Usuário não autenticado"));
                }

                _logger.LogInformation("Requisição de alteração de senha para usuário: {UserId}", userId);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dados inválidos na requisição de alteração de senha");
                    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(ApiResponse.ErrorResponse("Dados inválidos fornecidos", errors));
                }

                var result = await _authService.ChangePasswordAsync(userId.Value, request);

                if (!result)
                {
                    _logger.LogWarning("Falha na alteração de senha para usuário: {UserId}", userId);
                    return BadRequest(ApiResponse.ErrorResponse("Senha atual incorreta"));
                }

                _logger.LogInformation("Senha alterada com sucesso para usuário: {UserId}", userId);
                return Ok(ApiResponse.SuccessResponse("Senha alterada com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao alterar senha do usuário");
                return StatusCode(500, ApiResponse.ErrorResponse("Erro interno do servidor. Tente novamente mais tarde."));
            }
        }

        /// <summary>
        /// Obtém todos os usuários (apenas para administradores)
        /// </summary>
        /// <returns>Lista de usuários</returns>
        /// <response code="200">Lista obtida com sucesso</response>
        /// <response code="401">Usuário não autenticado</response>
        /// <response code="403">Usuário não tem permissão</response>
        [HttpGet("users")]
        [Authorize(Roles = "TecnicoFutebol")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        [ProducesResponseType(typeof(ApiResponse), 403)]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("Requisição de lista de usuários");

                var users = await _authService.GetAllUsersAsync();

                _logger.LogInformation("Lista de usuários obtida com sucesso. Total: {Count}", users.Count());
                return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse("Lista de usuários obtida com sucesso", users));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao obter lista de usuários");
                return StatusCode(500, ApiResponse<IEnumerable<UserDto>>.ErrorResponse("Erro interno do servidor. Tente novamente mais tarde."));
            }
        }

        /// <summary>
        /// Obtém o ID do usuário atual a partir do token JWT
        /// </summary>
        /// <returns>ID do usuário ou null se não encontrado</returns>
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
