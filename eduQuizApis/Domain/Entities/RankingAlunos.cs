using System.ComponentModel.DataAnnotations;
using eduQuizApis.Domain.Common;

namespace eduQuizApis.Domain.Entities
{
    public class RankingAlunos : IEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int UsuarioId { get; set; }
        
        [Required]
        public int CategoriaId { get; set; }
        
        [Required]
        public decimal PontuacaoTotal { get; set; } = 0;
        
        [Required]
        public int TotalQuizzes { get; set; } = 0;
        
        [Required]
        public decimal MediaPontuacao { get; set; } = 0;
        
        [Required]
        public int PosicaoRanking { get; set; } = 0;
        
        [Required]
        [MaxLength(50)]
        public string NivelAtual { get; set; } = "Iniciante";
        
        [Required]
        public int PontosExperiencia { get; set; } = 0;
        
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
        
        // Navigation properties
        public virtual User Usuario { get; set; } = null!;
        public virtual Categorias Categoria { get; set; } = null!;
    }
}
