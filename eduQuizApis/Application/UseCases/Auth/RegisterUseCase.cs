using eduQuizApis.Application.DTOs;
using eduQuizApis.Application.Interfaces;
using eduQuizApis.Domain.Interfaces;
using eduQuizApis.Domain.Entities;
using eduQuizApis.Domain.Enums;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace eduQuizApis.Application.UseCases.Auth
{
    /// <summary>
    /// Caso de uso para registro de usuário
    /// </summary>
    public class RegisterUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<RegisterUseCase> _logger;

        public RegisterUseCase(
            IUserRepository userRepository,
            IJwtService jwtService,
            ILogger<RegisterUseCase> logger)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _logger = logger;
        }

        /// <summary>
        /// Executa o caso de uso de registro
        /// </summary>
        /// <param name="request">Dados de registro</param>
        /// <returns>Resposta de autenticação ou null se falhar</returns>
        public async Task<AuthResponse?> ExecuteAsync(RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando processo de registro para email: {Email}", request.Email);

                // 1. Verificar se email já existe
                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    _logger.LogWarning("Email já existe: {Email}", request.Email);
                    return null;
                }

                // 2. Verificar se username já existe
                if (await _userRepository.UsernameExistsAsync(request.Username))
                {
                    _logger.LogWarning("Username já existe: {Username}", request.Username);
                    return null;
                }

                // 3. Converter string para enum
                var userRole = ConvertStringToUserRole(request.Role);

                // 4. Criar novo usuário
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    CPF = request.CPF,
                    DataNascimento = request.DataNascimento,
                    Role = userRole,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // 5. Salvar no banco
                var savedUser = await _userRepository.AddAsync(user);

                // 6. Gerar tokens
                var token = _jwtService.GenerateToken(savedUser);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // 7. Criar resposta
                var response = new AuthResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(_jwtService.GetTokenExpiryInHours()),
                    User = MapToUserDto(savedUser)
                };

                _logger.LogInformation("Registro realizado com sucesso para usuário: {UserId}", savedUser.Id);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar registro para email: {Email}", request.Email);
                throw;
            }
        }

        /// <summary>
        /// Converte string para enum UserRole
        /// </summary>
        /// <param name="role">Role em string</param>
        /// <returns>UserRole enum</returns>
        private static UserRole ConvertStringToUserRole(string role)
        {
            return role switch
            {
                "0" or "Aluno" => UserRole.Aluno,
                "1" or "Professor" => UserRole.Professor,
                "2" or "TecnicoFutebol" => UserRole.TecnicoFutebol,
                _ => UserRole.Aluno // Default para Aluno
            };
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
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
