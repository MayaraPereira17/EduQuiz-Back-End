using System.ComponentModel.DataAnnotations;
using eduQuizApis.Domain.Common;

namespace eduQuizApis.Domain.Entities
{
    public class TentativasQuiz : IEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int QuizId { get; set; }
        
        [Required]
        public int UsuarioId { get; set; }
        
        public DateTime DataInicio { get; set; } = DateTime.UtcNow;
        
        public DateTime? DataConclusao { get; set; }
        
        public decimal? Pontuacao { get; set; }
        
        public decimal? PontuacaoMaxima { get; set; }
        
        public int? TempoGasto { get; set; }
        
        public bool Concluida { get; set; } = false;
        
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
        public virtual Quizzes Quiz { get; set; } = null!;
        public virtual User Usuario { get; set; } = null!;
        public virtual ICollection<Respostas> Respostas { get; set; } = new List<Respostas>();
    }
}
