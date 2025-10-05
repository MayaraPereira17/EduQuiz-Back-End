using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eduQuizApis.Infrastructure.Data;
using eduQuizApis.Domain.Entities;
using eduQuizApis.Domain.Enums;
using BCrypt.Net;

namespace eduQuizApis.Presentation.Web.Controllers
{
    /// <summary>
    /// Controller para testes e operações de desenvolvimento
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TestController : ControllerBase
    {
        private readonly EduQuizContext _context;
        private readonly ILogger<TestController> _logger;

        public TestController(EduQuizContext context, ILogger<TestController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Testa a conexão com o banco de dados
        /// </summary>
        /// <returns>Status da conexão e informações do banco</returns>
        [HttpGet("database")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<object>> TestDatabaseConnection()
        {
            try
            {
                _logger.LogInformation("Testando conexão com banco de dados");

                // Testar conexão
                await _context.Database.OpenConnectionAsync();
                await _context.Database.CloseConnectionAsync();

                // Contar usuários
                var userCount = await _context.Usuarios.CountAsync();

                // Buscar usuário técnico
                var tecnico = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == "tecnico@eduquiz.com");

                var result = new
                {
                    success = true,
                    message = "Conexão com banco de dados OK",
                    timestamp = DateTime.UtcNow,
                    statistics = new
                    {
                        totalUsers = userCount,
                        activeUsers = await _context.Usuarios.CountAsync(u => u.IsActive),
                        students = await _context.Usuarios.CountAsync(u => u.Role == UserRole.Aluno),
                        teachers = await _context.Usuarios.CountAsync(u => u.Role == UserRole.Professor),
                        admins = await _context.Usuarios.CountAsync(u => u.Role == UserRole.TecnicoFutebol)
                    },
                    tecnicoUser = tecnico != null ? new
                    {
                        id = tecnico.Id,
                        username = tecnico.Username,
                        email = tecnico.Email,
                        role = tecnico.Role.ToString(),
                        isActive = tecnico.IsActive,
                        createdAt = tecnico.CreatedAt
                    } : null
                };

                _logger.LogInformation("Teste de conexão com banco realizado com sucesso");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao testar conexão com banco de dados");

                var result = new
                {
                    success = false,
                    message = "Não foi possível conectar ao banco de dados",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow,
                    connectionString = _context.Database.GetConnectionString()?.Substring(0, 50) + "..."
                };

                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Cria um usuário técnico padrão se ele não existir
        /// </summary>
        /// <returns>Resultado da operação</returns>
        [HttpPost("create-tecnico")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<object>> CreateTecnico()
        {
            try
            {
                _logger.LogInformation("Criando usuário técnico padrão");

                // Verificar se já existe
                var existingTecnico = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == "tecnico@eduquiz.com");

                if (existingTecnico != null)
                {
                    _logger.LogInformation("Usuário técnico já existe: {UserId}", existingTecnico.Id);

                    var result = new
                    {
                        success = true,
                        message = "Usuário técnico já existe",
                        timestamp = DateTime.UtcNow,
                        user = new
                        {
                            id = existingTecnico.Id,
                            username = existingTecnico.Username,
                            email = existingTecnico.Email,
                            role = existingTecnico.Role.ToString(),
                            isActive = existingTecnico.IsActive,
                            createdAt = existingTecnico.CreatedAt
                        }
                    };

                    return Ok(result);
                }

                // Criar usuário técnico
                var tecnico = new User
                {
                    Username = "tecnico",
                    Email = "tecnico@eduquiz.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), // Senha: admin123
                    FirstName = "Técnico",
                    LastName = "Matemática",
                    Role = UserRole.TecnicoFutebol,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Usuarios.Add(tecnico);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuário técnico criado com sucesso: {UserId}", tecnico.Id);

                var successResult = new
                {
                    success = true,
                    message = "Usuário técnico 'tecnico@eduquiz.com' criado com sucesso!",
                    timestamp = DateTime.UtcNow,
                    credentials = new
                    {
                        email = "tecnico@eduquiz.com",
                        password = "admin123",
                        role = "TecnicoFutebol"
                    },
                    user = new
                    {
                        id = tecnico.Id,
                        username = tecnico.Username,
                        email = tecnico.Email,
                        role = tecnico.Role.ToString(),
                        isActive = tecnico.IsActive,
                        createdAt = tecnico.CreatedAt
                    }
                };

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário técnico");

                var result = new
                {
                    success = false,
                    message = $"Erro ao criar usuário técnico: {ex.Message}",
                    timestamp = DateTime.UtcNow,
                    error = ex.ToString()
                };

                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Obtém estatísticas do sistema
        /// </summary>
        /// <returns>Estatísticas do sistema</returns>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<object>> GetStatistics()
        {
            try
            {
                _logger.LogInformation("Obtendo estatísticas do sistema");

                var totalUsers = await _context.Usuarios.CountAsync();
                var activeUsers = await _context.Usuarios.CountAsync(u => u.IsActive);
                var students = await _context.Usuarios.CountAsync(u => u.Role == UserRole.Aluno);
                var teachers = await _context.Usuarios.CountAsync(u => u.Role == UserRole.Professor);
                var admins = await _context.Usuarios.CountAsync(u => u.Role == UserRole.TecnicoFutebol);

                var recentUsers = await _context.Usuarios
                    .Where(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                    .CountAsync();

                var result = new
                {
                    success = true,
                    message = "Estatísticas obtidas com sucesso",
                    timestamp = DateTime.UtcNow,
                    statistics = new
                    {
                        users = new
                        {
                            total = totalUsers,
                            active = activeUsers,
                            inactive = totalUsers - activeUsers,
                            recent = recentUsers
                        },
                        roles = new
                        {
                            students = students,
                            teachers = teachers,
                            admins = admins
                        },
                        percentages = new
                        {
                            studentsPercent = totalUsers > 0 ? Math.Round((double)students / totalUsers * 100, 2) : 0,
                            teachersPercent = totalUsers > 0 ? Math.Round((double)teachers / totalUsers * 100, 2) : 0,
                            adminsPercent = totalUsers > 0 ? Math.Round((double)admins / totalUsers * 100, 2) : 0
                        }
                    }
                };

                _logger.LogInformation("Estatísticas obtidas com sucesso");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas do sistema");

                var result = new
                {
                    success = false,
                    message = $"Erro ao obter estatísticas: {ex.Message}",
                    timestamp = DateTime.UtcNow,
                    error = ex.ToString()
                };

                return StatusCode(500, result);
            }
        }
    }
}
