using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using eduQuizApis.Domain.Interfaces;
using eduQuizApis.Infrastructure.Data;

namespace eduQuizApis.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação genérica do repositório
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly EduQuizContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(EduQuizContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Busca uma entidade por ID
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Busca todas as entidades
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Busca entidades por expressão lambda
        /// </summary>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Busca a primeira entidade que atende ao critério
        /// </summary>
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Adiciona uma nova entidade
        /// </summary>
        public virtual async Task<T> AddAsync(T entity)
        {
            var entry = await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Atualiza uma entidade existente
        /// </summary>
        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Remove uma entidade
        /// </summary>
        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove uma entidade por ID
        /// </summary>
        public virtual async Task DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }

        /// <summary>
        /// Verifica se existe uma entidade que atende ao critério
        /// </summary>
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// Conta quantas entidades atendem ao critério
        /// </summary>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _dbSet.CountAsync();
            
            return await _dbSet.CountAsync(predicate);
        }
    }
}
