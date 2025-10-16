using eduQuizApis.Application.DTOs;
using eduQuizApis.Application.Interfaces;
using eduQuizApis.Application.UseCases.Auth;
using eduQuizApis.Domain.Interfaces;
using eduQuizApis.Domain.Entities;
using eduQuizApis.Domain.Enums;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace eduQuizApis.Application.Services
{
    /// <summary>
    /// Implementação do serviço de autenticação
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly LoginUseCase _loginUseCase;
        private readonly RegisterUseCase _registerUseCase;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService,
            LoginUseCase loginUseCase,
            RegisterUseCase registerUseCase,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _loginUseCase = loginUseCase;
            _registerUseCase = registerUseCase;
            _logger = logger;
        }

        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            _logger.LogInformation("Executando login para email: {Email}", request.Email);
            return await _loginUseCase.ExecuteAsync(request);
        }

        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            _logger.LogInformation("Executando registro para email: {Email}", request.Email);
            return await _registerUseCase.ExecuteAsync(request);
        }

        /// <summary>
        /// Busca perfil do usuário
        /// </summary>
        public async Task<UserDto?> GetUserProfileAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Buscando perfil do usuário: {UserId}", userId);

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Usuário não encontrado: {UserId}", userId);
                    return null;
                }

                return MapToUserDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar perfil do usuário: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Atualiza perfil do usuário
        /// </summary>
        public async Task<UserDto?> UpdateUserProfileAsync(int userId, UpdateProfileRequest request)
        {
            try
            {
                _logger.LogInformation("Atualizando perfil do usuário: {UserId}", userId);

                // 1. Buscar usuário
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Usuário não encontrado para atualização: {UserId}", userId);
                    return null;
                }

                // 2. Verificar se email já existe (se mudou)
                if (user.Email != request.Email && await _userRepository.EmailExistsAsync(request.Email, userId))
                {
                    _logger.LogWarning("Email já existe para outro usuário: {Email}", request.Email);
                    return null;
                }

                // 3. Atualizar dados
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.CPF = request.CPF;
                user.DataNascimento = request.DataNascimento;
                user.AvatarUrl = request.AvatarUrl;
                user.UpdatedAt = DateTime.UtcNow;

                // 4. Salvar alterações
                var updatedUser = await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Perfil atualizado com sucesso para usuário: {UserId}", userId);
                return MapToUserDto(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar perfil do usuário: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Altera senha do usuário
        /// </summary>
        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            try
            {
                _logger.LogInformation("Alterando senha do usuário: {UserId}", userId);

                // 1. Buscar usuário
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Usuário não encontrado para alteração de senha: {UserId}", userId);
                    return false;
                }

                // 2. Verificar senha atual
                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                {
                    _logger.LogWarning("Senha atual incorreta para usuário: {UserId}", userId);
                    return false;
                }

                // 3. Atualizar senha
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                // 4. Salvar alterações
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Senha alterada com sucesso para usuário: {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar senha do usuário: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Busca todos os usuários
        /// </summary>
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                _logger.LogInformation("Buscando todos os usuários");

                var users = await _userRepository.GetAllAsync();
                return users.Select(MapToUserDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os usuários");
                throw;
            }
        }

        /// <summary>
        /// Valida senha do usuário
        /// </summary>
        public async Task<bool> ValidatePasswordAsync(string email, string password)
        {
            try
            {
                _logger.LogInformation("Validando senha para email: {Email}", email);

                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null || !user.IsActive)
                {
                    _logger.LogWarning("Usuário não encontrado ou inativo: {Email}", email);
                    return false;
                }

                var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                _logger.LogInformation("Validação de senha para {Email}: {IsValid}", email, isValid);
                
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar senha para email: {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Mapeia User para UserDto
        /// </summary>
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
