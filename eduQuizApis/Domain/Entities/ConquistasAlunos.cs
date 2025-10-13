using System.ComponentModel.DataAnnotations;
using eduQuizApis.Domain.Common;

namespace eduQuizApis.Domain.Entities
{
    public class ConquistasAlunos : IEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int UsuarioId { get; set; }
        
        [Required]
        public int ConquistaId { get; set; }
        
        public DateTime DataConquista { get; set; } = DateTime.UtcNow;

        // Implementação da interface IEntity
        public DateTime CreatedAt 
        { 
            get => DataConquista; 
            set => DataConquista = value; 
        }
        
        public DateTime UpdatedAt 
        { 
            get => DataConquista; 
            set => DataConquista = value; 
        }
        
        // Navigation properties
        public virtual User Usuario { get; set; } = null!;
        public virtual Conquistas Conquista { get; set; } = null!;
    }
}
