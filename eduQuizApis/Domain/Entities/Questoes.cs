using System.ComponentModel.DataAnnotations;
using eduQuizApis.Domain.Common;

namespace eduQuizApis.Domain.Entities
{
    public class Questoes : IEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int QuizId { get; set; }
        
        [Required]
        public string TextoQuestao { get; set; } = string.Empty;
        
        [Required]
        public string TipoQuestao { get; set; } = string.Empty;
        
        public int Pontos { get; set; } = 1;
        
        [Required]
        public int OrdemIndice { get; set; }
        
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
        
        // Navigation properties
        public virtual Quizzes Quiz { get; set; } = null!;
        public virtual ICollection<OpcoesQuestao> Opcoes { get; set; } = new List<OpcoesQuestao>();
    }
}
