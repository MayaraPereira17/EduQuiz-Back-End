using eduQuizApis.Domain.Entities;
using eduQuizApis.Domain.Enums;

namespace eduQuizApis.Domain.Interfaces
{
    /// <summary>
    /// Interface específica para repositório de usuários
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Busca usuário por email
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <returns>Usuário encontrado ou null</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Busca usuário por username
        /// </summary>
        /// <param name="username">Username do usuário</param>
        /// <returns>Usuário encontrado ou null</returns>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// Verifica se email já existe
        /// </summary>
        /// <param name="email">Email a ser verificado</param>
        /// <param name="excludeUserId">ID do usuário a ser excluído da verificação (para updates)</param>
        /// <returns>True se email existe, False caso contrário</returns>
        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);

        /// <summary>
        /// Verifica se username já existe
        /// </summary>
        /// <param name="username">Username a ser verificado</param>
        /// <param name="excludeUserId">ID do usuário a ser excluído da verificação (para updates)</param>
        /// <returns>True se username existe, False caso contrário</returns>
        Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null);

        /// <summary>
        /// Busca usuários por função
        /// </summary>
        /// <param name="role">Função dos usuários</param>
        /// <returns>Lista de usuários com a função especificada</returns>
        Task<IEnumerable<User>> GetByRoleAsync(UserRole role);

        /// <summary>
        /// Busca usuários ativos
        /// </summary>
        /// <returns>Lista de usuários ativos</returns>
        Task<IEnumerable<User>> GetActiveUsersAsync();

        /// <summary>
        /// Busca usuários por nome (busca parcial)
        /// </summary>
        /// <param name="name">Nome ou parte do nome</param>
        /// <returns>Lista de usuários que contêm o nome</returns>
        Task<IEnumerable<User>> SearchByNameAsync(string name);
    }
}
