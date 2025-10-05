using eduQuizApis.Application.DTOs;

namespace eduQuizApis.Application.Interfaces
{
    /// <summary>
    /// Interface para o serviço de autenticação
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        /// <param name="request">Dados de login</param>
        /// <returns>Resposta de autenticação ou null se falhar</returns>
        Task<AuthResponse?> LoginAsync(LoginRequest request);

        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        /// <param name="request">Dados de registro</param>
        /// <returns>Resposta de autenticação ou null se falhar</returns>
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Busca perfil do usuário
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <returns>Dados do usuário ou null se não encontrado</returns>
        Task<UserDto?> GetUserProfileAsync(int userId);

        /// <summary>
        /// Atualiza perfil do usuário
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <param name="request">Dados para atualização</param>
        /// <returns>Dados atualizados do usuário ou null se falhar</returns>
        Task<UserDto?> UpdateUserProfileAsync(int userId, UpdateProfileRequest request);

        /// <summary>
        /// Altera senha do usuário
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <param name="request">Dados de alteração de senha</param>
        /// <returns>True se sucesso, False se falhar</returns>
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);

        /// <summary>
        /// Busca todos os usuários
        /// </summary>
        /// <returns>Lista de usuários</returns>
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        /// <summary>
        /// Valida senha do usuário
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <param name="password">Senha a ser validada</param>
        /// <returns>True se senha correta, False caso contrário</returns>
        Task<bool> ValidatePasswordAsync(string email, string password);
    }
}
