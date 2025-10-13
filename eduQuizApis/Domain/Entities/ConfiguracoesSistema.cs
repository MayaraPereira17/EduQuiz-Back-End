using System.ComponentModel.DataAnnotations;
using eduQuizApis.Domain.Common;

namespace eduQuizApis.Domain.Entities
{
    public class ConfiguracoesSistema : IEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string ChaveConfiguracao { get; set; } = string.Empty;
        
        public string? ValorConfiguracao { get; set; }
        
        public string? Descricao { get; set; }
        
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

        // Implementação da interface IEntity
        public DateTime CreatedAt 
        { 
            get => DataAtualizacao; 
            set => DataAtualizacao = value; 
        }
        
        public DateTime UpdatedAt 
        { 
            get => DataAtualizacao; 
            set => DataAtualizacao = value; 
        }
    }
}
