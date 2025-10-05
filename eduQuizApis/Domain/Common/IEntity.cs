namespace eduQuizApis.Domain.Common
{
    /// <summary>
    /// Interface base para todas as entidades do domínio
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Identificador único da entidade
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Data de criação da entidade
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data da última atualização da entidade
        /// </summary>
        DateTime UpdatedAt { get; set; }
    }
}
