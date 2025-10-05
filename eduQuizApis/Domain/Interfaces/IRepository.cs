using System.Linq.Expressions;

namespace eduQuizApis.Domain.Interfaces
{
    /// <summary>
    /// Interface genérica para repositórios
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Busca uma entidade por ID
        /// </summary>
        /// <param name="id">ID da entidade</param>
        /// <returns>Entidade encontrada ou null</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Busca todas as entidades
        /// </summary>
        /// <returns>Lista de entidades</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Busca entidades por expressão lambda
        /// </summary>
        /// <param name="predicate">Expressão de busca</param>
        /// <returns>Lista de entidades que atendem ao critério</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Busca a primeira entidade que atende ao critério
        /// </summary>
        /// <param name="predicate">Expressão de busca</param>
        /// <returns>Entidade encontrada ou null</returns>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adiciona uma nova entidade
        /// </summary>
        /// <param name="entity">Entidade a ser adicionada</param>
        /// <returns>Entidade adicionada</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Atualiza uma entidade existente
        /// </summary>
        /// <param name="entity">Entidade a ser atualizada</param>
        /// <returns>Entidade atualizada</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Remove uma entidade
        /// </summary>
        /// <param name="entity">Entidade a ser removida</param>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Remove uma entidade por ID
        /// </summary>
        /// <param name="id">ID da entidade</param>
        Task DeleteByIdAsync(int id);

        /// <summary>
        /// Verifica se existe uma entidade que atende ao critério
        /// </summary>
        /// <param name="predicate">Expressão de busca</param>
        /// <returns>True se existe, False caso contrário</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Conta quantas entidades atendem ao critério
        /// </summary>
        /// <param name="predicate">Expressão de busca (opcional)</param>
        /// <returns>Quantidade de entidades</returns>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    }
}
