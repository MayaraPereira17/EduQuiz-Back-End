using Microsoft.EntityFrameworkCore;
using eduQuizApis.Domain.Entities;

namespace eduQuizApis.Infrastructure.Data
{
    /// <summary>
    /// Contexto do Entity Framework para o EduQuiz
    /// </summary>
    public class EduQuizContext : DbContext
    {
        public EduQuizContext(DbContextOptions<EduQuizContext> options) : base(options)
        {
        }

        /// <summary>
        /// DbSets para as entidades do sistema
        /// </summary>
        public DbSet<User> Usuarios { get; set; }
        public DbSet<Categorias> Categorias { get; set; }
        public DbSet<Quizzes> Quizzes { get; set; }
        public DbSet<Questoes> Questoes { get; set; }
        public DbSet<OpcoesQuestao> OpcoesQuestao { get; set; }
        public DbSet<TentativasQuiz> TentativasQuiz { get; set; }
        public DbSet<Respostas> Respostas { get; set; }
        public DbSet<RelatoriosPerformance> RelatoriosPerformance { get; set; }
        public DbSet<ConfiguracoesSistema> ConfiguracoesSistema { get; set; }
        public DbSet<RankingAlunos> RankingAlunos { get; set; }
        public DbSet<Conquistas> Conquistas { get; set; }
        public DbSet<ConquistasAlunos> ConquistasAlunos { get; set; }

        /// <summary>
        /// Configuração do modelo de dados
        /// </summary>
        /// <param name="modelBuilder">Builder do modelo</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações para User
            modelBuilder.Entity<User>(entity =>
            {
                // Mapear para tabela em português
                entity.ToTable("Usuarios");

                // Mapear propriedades para nomes das colunas em português
                entity.Property(e => e.Username).HasColumnName("Username");
                entity.Property(e => e.Email).HasColumnName("Email");
                entity.Property(e => e.PasswordHash).HasColumnName("SenhaHash");
                entity.Property(e => e.FirstName).HasColumnName("Nome");
                entity.Property(e => e.LastName).HasColumnName("Sobrenome");
                entity.Property(e => e.CPF).HasColumnName("CPF");
                entity.Property(e => e.DataNascimento).HasColumnName("DataNascimento");
                entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
                
                // Conversão de enum para string e vice-versa
                entity.Property(e => e.Role)
                    .HasColumnName("Funcao")
                    .HasConversion(
                        v => v.ToString(),  // Enum → String (para salvar)
                        v => (Domain.Enums.UserRole)Enum.Parse(typeof(Domain.Enums.UserRole), v)); // String → Enum (para ler)

                entity.Property(e => e.IsActive).HasColumnName("Ativo");
                entity.Property(e => e.CreatedAt).HasColumnName("DataCriacao");
                entity.Property(e => e.UpdatedAt).HasColumnName("DataAtualizacao");

                // Índices únicos
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                // Valores padrão para timestamps
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
            });

            // Seed Data - Usuário técnico padrão
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "tecnico",
                    Email = "tecnico@eduquiz.com",
                    PasswordHash = "$2a$11$rQZ8K9vL2mN3pO4qR5sT6uV7wX8yZ9aB0cD1eF2gH3iJ4kL5mN6oP7qR8sT9uV", // Senha: admin123
                    FirstName = "Técnico",
                    LastName = "Matemática",
                    Role = Domain.Enums.UserRole.TecnicoFutebol,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
