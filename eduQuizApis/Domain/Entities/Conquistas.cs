using System.ComponentModel.DataAnnotations;
using eduQuizApis.Domain.Common;

namespace eduQuizApis.Domain.Entities
{
    public class Conquistas : IEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        public string? Descricao { get; set; }
        
        [Required]
        public string TipoConquista { get; set; } = string.Empty;
        
        [Required]
        public decimal CriterioMinimo { get; set; }
        
        [MaxLength(50)]
        public string? Icone { get; set; }
        
        [MaxLength(20)]
        public string? Cor { get; set; }
        
        public bool Ativo { get; set; } = true;
        
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

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
        public virtual ICollection<ConquistasAlunos> ConquistasAlunos { get; set; } = new List<ConquistasAlunos>();
    }
}
