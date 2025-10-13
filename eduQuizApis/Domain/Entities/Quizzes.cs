using System.ComponentModel.DataAnnotations;
using eduQuizApis.Domain.Common;

namespace eduQuizApis.Domain.Entities
{
    public class Quizzes : IEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; } = string.Empty;
        
        public string? Descricao { get; set; }
        
        [Required]
        public int CategoriaId { get; set; }
        
        [Required]
        public int CriadoPor { get; set; }
        
        public int? TempoLimite { get; set; }
        
        public int MaxTentativas { get; set; } = 1;
        
        public bool Ativo { get; set; } = true;
        
        public bool Publico { get; set; } = false;
        
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

        // Implementação da interface IEntity
        public DateTime CreatedAt 
        { 
            get => DataCriacao; 
            set => DataCriacao = value; 
        }
        
        public DateTime UpdatedAt 
        { 
            get => DataAtualizacao; 
            set => DataAtualizacao = value; 
        }
        
        // Navigation properties
        public virtual Categorias Categoria { get; set; } = null!;
        public virtual User CriadoPorUser { get; set; } = null!;
        public virtual ICollection<Questoes> Questoes { get; set; } = new List<Questoes>();
    }
}
