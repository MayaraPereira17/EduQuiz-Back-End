using System.ComponentModel.DataAnnotations;
using eduQuizApis.Domain.Enums;

namespace eduQuizApis.Application.DTOs
{
    /// <summary>
    /// DTO para requisição de login
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Email do usuário
        /// </summary>
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário
        /// </summary>
        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para requisição de registro de usuário
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Nome de usuário único
        /// </summary>
        [Required(ErrorMessage = "Username é obrigatório")]
        [MaxLength(50, ErrorMessage = "Username deve ter no máximo 50 caracteres")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email único do usuário
        /// </summary>
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        [MaxLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário
        /// </summary>
        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Confirmação da senha
        /// </summary>
        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [Compare("Password", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Nome do usuário
        /// </summary>
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(50, ErrorMessage = "Nome deve ter no máximo 50 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Sobrenome do usuário
        /// </summary>
        [Required(ErrorMessage = "Sobrenome é obrigatório")]
        [MaxLength(50, ErrorMessage = "Sobrenome deve ter no máximo 50 caracteres")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// CPF do usuário (opcional)
        /// </summary>
        [MaxLength(14, ErrorMessage = "CPF deve ter no máximo 14 caracteres")]
        public string? CPF { get; set; }

        /// <summary>
        /// Data de nascimento do usuário (opcional)
        /// </summary>
        public DateTime? DataNascimento { get; set; }

        /// <summary>
        /// Função do usuário no sistema
        /// </summary>
        public string Role { get; set; } = "Aluno";

        /// <summary>
        /// URL do avatar/foto do usuário (opcional)
        /// </summary>
        [MaxLength(500, ErrorMessage = "URL do avatar deve ter no máximo 500 caracteres")]
        public string? AvatarUrl { get; set; }
    }

    /// <summary>
    /// DTO para resposta de autenticação
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// Token JWT de autenticação
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Token de refresh para renovação
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Data de expiração do token
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Dados do usuário autenticado
        /// </summary>
        public UserDto User { get; set; } = null!;
    }

    /// <summary>
    /// DTO para dados do usuário (sem informações sensíveis)
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// ID único do usuário
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome de usuário
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Nome do usuário
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Sobrenome do usuário
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// CPF do usuário
        /// </summary>
        public string? CPF { get; set; }

        /// <summary>
        /// Data de nascimento do usuário
        /// </summary>
        public DateTime? DataNascimento { get; set; }

        /// <summary>
        /// URL do avatar/foto do usuário
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Função do usuário no sistema
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Indica se o usuário está ativo
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Data de criação do usuário
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Indica se é administrador
        /// </summary>
        public bool IsAdmin => Role == UserRole.TecnicoFutebol;

        /// <summary>
        /// Indica se é professor
        /// </summary>
        public bool IsTeacher => Role == UserRole.Professor;

        /// <summary>
        /// Indica se é aluno
        /// </summary>
        public bool IsStudent => Role == UserRole.Aluno;
    }

    /// <summary>
    /// DTO para requisição de alteração de senha
    /// </summary>
    public class ChangePasswordRequest
    {
        /// <summary>
        /// Senha atual do usuário
        /// </summary>
        [Required(ErrorMessage = "Senha atual é obrigatória")]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// Nova senha do usuário
        /// </summary>
        [Required(ErrorMessage = "Nova senha é obrigatória")]
        [MinLength(6, ErrorMessage = "Nova senha deve ter pelo menos 6 caracteres")]
        public string NewPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para requisição de atualização de perfil
    /// </summary>
    public class UpdateProfileRequest
    {
        /// <summary>
        /// Nome do usuário
        /// </summary>
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(50, ErrorMessage = "Nome deve ter no máximo 50 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Sobrenome do usuário
        /// </summary>
        [Required(ErrorMessage = "Sobrenome é obrigatório")]
        [MaxLength(50, ErrorMessage = "Sobrenome deve ter no máximo 50 caracteres")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário
        /// </summary>
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        [MaxLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// CPF do usuário (opcional)
        /// </summary>
        [MaxLength(14, ErrorMessage = "CPF deve ter no máximo 14 caracteres")]
        public string? CPF { get; set; }

        /// <summary>
        /// Data de nascimento do usuário (opcional)
        /// </summary>
        public DateTime? DataNascimento { get; set; }

        /// <summary>
        /// URL do avatar/foto do usuário (opcional)
        /// </summary>
        [MaxLength(500, ErrorMessage = "URL do avatar deve ter no máximo 500 caracteres")]
        public string? AvatarUrl { get; set; }
    }
}
