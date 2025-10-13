using System.ComponentModel.DataAnnotations;
using eduQuizApis.Domain.Common;

namespace eduQuizApis.Domain.Entities
{
    public class Categorias : IEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        public string? Descricao { get; set; }
        
        public bool Ativo { get; set; } = true;
        
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
    }
}
