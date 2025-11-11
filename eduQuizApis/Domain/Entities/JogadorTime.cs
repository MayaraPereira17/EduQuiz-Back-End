using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eduQuizApis.Domain.Common;

namespace eduQuizApis.Domain.Entities
{
    /// <summary>
    /// Entidade que representa a relação entre um time e um jogador (aluno)
    /// </summary>
    public class JogadorTime : IEntity
    {
        /// <summary>
        /// Identificador único do registro
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ID do time
        /// </summary>
        [Required]
        public int TimeId { get; set; }

        /// <summary>
        /// ID do aluno/jogador
        /// </summary>
        [Required]
        public int AlunoId { get; set; }

        /// <summary>
        /// Data de escalação do jogador
        /// </summary>
        public DateTime DataEscalacao { get; set; } = DateTime.UtcNow;

        // Implementação da interface IEntity
        public DateTime CreatedAt
        {
            get => DataEscalacao;
            set => DataEscalacao = value;
        }

        public DateTime UpdatedAt
        {
            get => DataEscalacao;
            set => DataEscalacao = value;
        }

        // Navigation properties
        [ForeignKey("TimeId")]
        public virtual Time Time { get; set; } = null!;

        [ForeignKey("AlunoId")]
        public virtual User Aluno { get; set; } = null!;
    }
}

