using System.ComponentModel.DataAnnotations;
using eduQuizApis.Domain.Common;

namespace eduQuizApis.Domain.Entities
{
    public class Respostas : IEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int TentativaId { get; set; }
        
        [Required]
        public int QuestaoId { get; set; }
        
        public int? OpcaoSelecionadaId { get; set; }
        
        public string? TextoResposta { get; set; }
        
        public bool? Correta { get; set; }
        
        public decimal PontosGanhos { get; set; } = 0;
        
        public DateTime DataResposta { get; set; } = DateTime.UtcNow;

        // Implementação da interface IEntity
        public DateTime CreatedAt 
        { 
            get => DataResposta; 
            set => DataResposta = value; 
        }
        
        public DateTime UpdatedAt 
        { 
            get => DataResposta; 
            set => DataResposta = value; 
        }
        
        // Navigation properties
        public virtual TentativasQuiz Tentativa { get; set; } = null!;
        public virtual Questoes Questao { get; set; } = null!;
        public virtual OpcoesQuestao? OpcaoSelecionada { get; set; }
    }
}
