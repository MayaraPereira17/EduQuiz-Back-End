using eduQuizApis.Domain.Entities;

namespace eduQuizApis.Application.Interfaces
{
    /// <summary>
    /// Interface para o serviço de JWT
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Gera token JWT para o usuário
        /// </summary>
        /// <param name="user">Usuário para gerar o token</param>
        /// <returns>Token JWT</returns>
        string GenerateToken(User user);

        /// <summary>
        /// Gera token de refresh
        /// </summary>
        /// <returns>Token de refresh</returns>
        string GenerateRefreshToken();

        /// <summary>
        /// Obtém tempo de expiração do token em horas
        /// </summary>
        /// <returns>Horas de expiração</returns>
        int GetTokenExpiryInHours();
    }
}
