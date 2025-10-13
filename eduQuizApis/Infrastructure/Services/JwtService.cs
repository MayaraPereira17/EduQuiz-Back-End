using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using eduQuizApis.Domain.Entities;
using eduQuizApis.Application.Interfaces;

namespace eduQuizApis.Infrastructure.Services
{
    /// <summary>
    /// Implementação do serviço JWT
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Gera token JWT para o usuário
        /// </summary>
        public string GenerateToken(User user)
        {
            try
            {
                _logger.LogInformation("Gerando token JWT para usuário: {UserId}", user.Id);

                // Obter chave secreta e configurações
                var secretKey = _configuration["JwtSettings:SecretKey"];
                var issuer = _configuration["JwtSettings:Issuer"];
                var audience = _configuration["JwtSettings:Audience"];

                if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                {
                    throw new InvalidOperationException("Configurações JWT não encontradas no appsettings.json");
                }

                // Criar chave simétrica
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Criar claims (informações do usuário)
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("Funcao", user.Role.ToString()),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("IsActive", user.IsActive.ToString()),
                    new Claim("FullName", user.FullName)
                };

                // Criar token
                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(GetTokenExpiryInHours()),
                    signingCredentials: credentials
                );

                // Converter para string
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                _logger.LogInformation("Token JWT gerado com sucesso para usuário: {UserId}", user.Id);
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar token JWT para usuário: {UserId}", user.Id);
                throw;
            }
        }

        /// <summary>
        /// Gera token de refresh
        /// </summary>
        public string GenerateRefreshToken()
        {
            try
            {
                _logger.LogInformation("Gerando token de refresh");
                
                // Gerar GUID único como refresh token
                var refreshToken = Guid.NewGuid().ToString();
                
                _logger.LogInformation("Token de refresh gerado com sucesso");
                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar token de refresh");
                throw;
            }
        }

        /// <summary>
        /// Obtém tempo de expiração do token em horas
        /// </summary>
        public int GetTokenExpiryInHours()
        {
            var expiryHours = _configuration["JwtSettings:ExpiryInHours"];
            
            if (int.TryParse(expiryHours, out int hours))
            {
                return hours;
            }

            // Valor padrão se não conseguir fazer parse
            _logger.LogWarning("Não foi possível obter ExpiryInHours das configurações, usando valor padrão: 24");
            return 24;
        }

        /// <summary>
        /// Valida token JWT
        /// </summary>
        /// <param name="token">Token a ser validado</param>
        /// <returns>Claims do token ou null se inválido</returns>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                _logger.LogInformation("Validando token JWT");

                var secretKey = _configuration["JwtSettings:SecretKey"];
                var issuer = _configuration["JwtSettings:Issuer"];
                var audience = _configuration["JwtSettings:Audience"];

                if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                {
                    _logger.LogError("Configurações JWT não encontradas");
                    return null;
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero // Remove tolerância de tempo
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                _logger.LogInformation("Token JWT validado com sucesso");
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token JWT inválido");
                return null;
            }
        }

        /// <summary>
        /// Extrai ID do usuário do token
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>ID do usuário ou null se não encontrado</returns>
        public int? GetUserIdFromToken(string token)
        {
            try
            {
                var principal = ValidateToken(token);
                if (principal == null) return null;

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair ID do usuário do token");
                return null;
            }
        }
    }
}
