using eduQuizApis.Application.DTOs;
using eduQuizApis.Application.Interfaces;
using eduQuizApis.Domain.Interfaces;
using eduQuizApis.Domain.Entities;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace eduQuizApis.Application.UseCases.Auth
{
    /// <summary>
    /// Caso de uso para login de usuário
    /// </summary>
    public class LoginUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<LoginUseCase> _logger;

        public LoginUseCase(
            IUserRepository userRepository,
            IJwtService jwtService,
            ILogger<LoginUseCase> logger)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _logger = logger;
        }

        /// <summary>
        /// Executa o caso de uso de login
        /// </summary>
        /// <param name="request">Dados de login</param>
        /// <returns>Resposta de autenticação ou null se falhar</returns>
        public async Task<AuthResponse?> ExecuteAsync(LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando processo de login para email: {Email}", request.Email);

                // 1. Buscar usuário por email
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.LogWarning("Usuário não encontrado para email: {Email}", request.Email);
                    return null;
                }

                // 2. Verificar se usuário está ativo
                if (!user.IsActive)
                {
                    _logger.LogWarning("Usuário inativo tentando fazer login: {Email}", request.Email);
                    return null;
                }

                // 3. Verificar senha
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Senha incorreta para usuário: {Email}", request.Email);
                    return null;
                }

                // 4. Gerar tokens
                var token = _jwtService.GenerateToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // 5. Criar resposta
                var response = new AuthResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(_jwtService.GetTokenExpiryInHours()),
                    User = MapToUserDto(user)
                };

                _logger.LogInformation("Login realizado com sucesso para usuário: {UserId}", user.Id);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar login para email: {Email}", request.Email);
                throw;
            }
        }

        /// <summary>
        /// Mapeia User para UserDto
        /// </summary>
        /// <param name="user">Entidade User</param>
        /// <returns>UserDto</returns>
        private static UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CPF = user.CPF,
                DataNascimento = user.DataNascimento,
                AvatarUrl = user.AvatarUrl,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
