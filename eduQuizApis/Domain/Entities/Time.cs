using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eduQuizApis.Domain.Common;

namespace eduQuizApis.Domain.Entities
{
    /// <summary>
    /// Entidade que representa um time de futebol criado pelo técnico
    /// </summary>
    public class Time : IEntity
    {
        /// <summary>
        /// Identificador único do time
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome do time
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// ID do técnico que criou o time
        /// </summary>
        [Required]
        public int TecnicoId { get; set; }

        /// <summary>
        /// Data de criação do time
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Indica se o time está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Implementação da interface IEntity
        public DateTime CreatedAt
        {
            get => DataCriacao;
            set => DataCriacao = value;
        }

        public DateTime UpdatedAt
        {
            get => DataCriacao;
            set => DataCriacao = value;
        }

        // Navigation properties
        [ForeignKey("TecnicoId")]
        public virtual User Tecnico { get; set; } = null!;

        public virtual ICollection<JogadorTime> Jogadores { get; set; } = new List<JogadorTime>();
    }
}

