using System.ComponentModel.DataAnnotations;
using eduQuizApis.Domain.Common;

namespace eduQuizApis.Domain.Entities
{
    public class RelatoriosPerformance : IEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int UsuarioId { get; set; }
        
        [Required]
        public int QuizId { get; set; }
        
        [Required]
        public int TentativaId { get; set; }
        
        [Required]
        public int TotalQuestoes { get; set; }
        
        [Required]
        public int RespostasCorretas { get; set; }
        
        [Required]
        public int RespostasErradas { get; set; }
        
        [Required]
        public decimal Percentual { get; set; }
        
        [Required]
        public int TempoGasto { get; set; }
        
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
        public virtual User Usuario { get; set; } = null!;
        public virtual Quizzes Quiz { get; set; } = null!;
        public virtual TentativasQuiz Tentativa { get; set; } = null!;
    }
}
