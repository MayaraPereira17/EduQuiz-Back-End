using System.ComponentModel.DataAnnotations;
using eduQuizApis.Domain.Enums;
using eduQuizApis.Domain.Common;

namespace eduQuizApis.Domain.Entities
{
    /// <summary>
    /// Entidade que representa um usuário do sistema EduQuiz
    /// </summary>
    public class User : IEntity
    {
        /// <summary>
        /// Identificador único do usuário
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nome de usuário único no sistema
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email único do usuário
        /// </summary>
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hash da senha do usuário (criptografada)
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Nome do usuário
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Sobrenome do usuário
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// CPF do usuário (opcional)
        /// </summary>
        [MaxLength(14)]
        public string? CPF { get; set; }

        /// <summary>
        /// Data de nascimento do usuário (opcional)
        /// </summary>
        public DateTime? DataNascimento { get; set; }

        /// <summary>
        /// URL do avatar/foto do usuário (opcional)
        /// </summary>
        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Função/Papel do usuário no sistema
        /// </summary>
        [Required]
        public UserRole Role { get; set; } = UserRole.Aluno;

        /// <summary>
        /// Indica se o usuário está ativo no sistema
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Data de criação do usuário
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data da última atualização do usuário
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Verifica se o usuário é um administrador
        /// </summary>
        public bool IsAdmin => Role == UserRole.TecnicoFutebol;

        /// <summary>
        /// Verifica se o usuário é um professor
        /// </summary>
        public bool IsTeacher => Role == UserRole.Professor;

        /// <summary>
        /// Verifica se o usuário é um aluno
        /// </summary>
        public bool IsStudent => Role == UserRole.Aluno;
    }
}
