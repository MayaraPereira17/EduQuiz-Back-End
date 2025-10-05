using eduQuizApis.Domain.Entities;
using eduQuizApis.Domain.Enums;
using eduQuizApis.Domain.Interfaces;
using eduQuizApis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eduQuizApis.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de usuários
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(EduQuizContext context) : base(context)
        {
        }

        /// <summary>
        /// Busca usuário por email
        /// </summary>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Busca usuário por username
        /// </summary>
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <summary>
        /// Verifica se email já existe
        /// </summary>
        public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
        {
            var query = _dbSet.Where(u => u.Email == email);
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync();
        }

        /// <summary>
        /// Verifica se username já existe
        /// </summary>
        public async Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null)
        {
            var query = _dbSet.Where(u => u.Username == username);
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync();
        }

        /// <summary>
        /// Busca usuários por função
        /// </summary>
        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
        {
            return await _dbSet
                .Where(u => u.Role == role && u.IsActive)
                .ToListAsync();
        }

        /// <summary>
        /// Busca usuários ativos
        /// </summary>
        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbSet
                .Where(u => u.IsActive)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        /// <summary>
        /// Busca usuários por nome (busca parcial)
        /// </summary>
        public async Task<IEnumerable<User>> SearchByNameAsync(string name)
        {
            var searchTerm = name.ToLower();
            
            return await _dbSet
                .Where(u => u.IsActive && 
                           (u.FirstName.ToLower().Contains(searchTerm) ||
                            u.LastName.ToLower().Contains(searchTerm) ||
                            u.Username.ToLower().Contains(searchTerm)))
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }
    }
}
