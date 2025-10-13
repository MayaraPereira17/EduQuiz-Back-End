using System.ComponentModel.DataAnnotations;
using eduQuizApis.Domain.Common;

namespace eduQuizApis.Domain.Entities
{
    public class OpcoesQuestao : IEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int QuestaoId { get; set; }
        
        [Required]
        public string TextoOpcao { get; set; } = string.Empty;
        
        public bool Correta { get; set; } = false;
        
        [Required]
        public int OrdemIndice { get; set; }
        
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
        public virtual Questoes Questao { get; set; } = null!;
    }
}
