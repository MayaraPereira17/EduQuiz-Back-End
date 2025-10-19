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

            // Configurações para Categorias
            modelBuilder.Entity<Categorias>(entity =>
            {
                entity.ToTable("Categorias");
                entity.Property(e => e.Nome).HasColumnName("Nome").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descricao).HasColumnName("Descricao");
                entity.Property(e => e.Ativo).HasColumnName("Ativo").HasDefaultValue(true);
                entity.Property(e => e.DataCriacao).HasColumnName("DataCriacao").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.DataAtualizacao).HasColumnName("DataAtualizacao").HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
            });

            // Configurações para Quizzes
            modelBuilder.Entity<Quizzes>(entity =>
            {
                entity.ToTable("Quizzes");
                entity.Property(e => e.Titulo).HasColumnName("Titulo").IsRequired().HasMaxLength(200);
                entity.Property(e => e.Descricao).HasColumnName("Descricao");
                entity.Property(e => e.CategoriaId).HasColumnName("CategoriaId").IsRequired();
                entity.Property(e => e.CriadoPor).HasColumnName("CriadoPor").IsRequired();
                entity.Property(e => e.TempoLimite).HasColumnName("TempoLimite");
                entity.Property(e => e.MaxTentativas).HasColumnName("MaxTentativas").HasDefaultValue(1);
                entity.Property(e => e.Ativo).HasColumnName("Ativo").HasDefaultValue(true);
                entity.Property(e => e.Publico).HasColumnName("Publico").HasDefaultValue(false);
                entity.Property(e => e.DataCriacao).HasColumnName("DataCriacao").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.DataAtualizacao).HasColumnName("DataAtualizacao").HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                // Relacionamentos
                entity.HasOne(q => q.Categoria)
                    .WithMany()
                    .HasForeignKey(q => q.CategoriaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(q => q.CriadoPorUser)
                    .WithMany()
                    .HasForeignKey(q => q.CriadoPor)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurações para Questoes
            modelBuilder.Entity<Questoes>(entity =>
            {
                entity.ToTable("Questoes");
                entity.Property(e => e.QuizId).HasColumnName("QuizId").IsRequired();
                entity.Property(e => e.TextoQuestao).HasColumnName("TextoQuestao").IsRequired();
                entity.Property(e => e.TipoQuestao).HasColumnName("TipoQuestao").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Pontos).HasColumnName("Pontos").HasDefaultValue(1);
                entity.Property(e => e.OrdemIndice).HasColumnName("OrdemIndice").IsRequired();
                entity.Property(e => e.Ativo).HasColumnName("Ativo").HasDefaultValue(true);
                entity.Property(e => e.DataCriacao).HasColumnName("DataCriacao").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.DataAtualizacao).HasColumnName("DataAtualizacao").HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                // Relacionamentos
                entity.HasOne(q => q.Quiz)
                    .WithMany(q => q.Questoes)
                    .HasForeignKey(q => q.QuizId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurações para OpcoesQuestao
            modelBuilder.Entity<OpcoesQuestao>(entity =>
            {
                entity.ToTable("OpcoesQuestao");
                entity.Property(e => e.QuestaoId).HasColumnName("QuestaoId").IsRequired();
                entity.Property(e => e.TextoOpcao).HasColumnName("TextoOpcao").IsRequired();
                entity.Property(e => e.Correta).HasColumnName("Correta").HasDefaultValue(false);
                entity.Property(e => e.OrdemIndice).HasColumnName("OrdemIndice").IsRequired();
                entity.Property(e => e.DataCriacao).HasColumnName("DataCriacao").HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relacionamentos
                entity.HasOne(o => o.Questao)
                    .WithMany(q => q.Opcoes)
                    .HasForeignKey(o => o.QuestaoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurações para TentativasQuiz
            modelBuilder.Entity<TentativasQuiz>(entity =>
            {
                entity.ToTable("TentativasQuiz");
                entity.Property(e => e.QuizId).HasColumnName("QuizId").IsRequired();
                entity.Property(e => e.UsuarioId).HasColumnName("UsuarioId").IsRequired();
                entity.Property(e => e.DataInicio).HasColumnName("DataInicio").IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.DataConclusao).HasColumnName("DataConclusao");
                entity.Property(e => e.Pontuacao).HasColumnName("Pontuacao").HasColumnType("DECIMAL(5,2)");
                entity.Property(e => e.PontuacaoMaxima).HasColumnName("PontuacaoMaxima").HasColumnType("DECIMAL(5,2)");
                entity.Property(e => e.TempoGasto).HasColumnName("TempoGasto");
                entity.Property(e => e.Concluida).HasColumnName("Concluida").HasDefaultValue(false);
                entity.Property(e => e.DataCriacao).HasColumnName("DataCriacao").HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relacionamentos
                entity.HasOne(t => t.Quiz)
                    .WithMany()
                    .HasForeignKey(t => t.QuizId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.Usuario)
                    .WithMany()
                    .HasForeignKey(t => t.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurações para Respostas
            modelBuilder.Entity<Respostas>(entity =>
            {
                entity.ToTable("Respostas");
                entity.Property(e => e.TentativaId).HasColumnName("TentativaId").IsRequired();
                entity.Property(e => e.QuestaoId).HasColumnName("QuestaoId").IsRequired();
                entity.Property(e => e.OpcaoSelecionadaId).HasColumnName("OpcaoSelecionadaId");
                entity.Property(e => e.TextoResposta).HasColumnName("TextoResposta");
                entity.Property(e => e.Correta).HasColumnName("Correta");
                entity.Property(e => e.PontosGanhos).HasColumnName("PontosGanhos").HasColumnType("DECIMAL(5,2)").HasDefaultValue(0);
                entity.Property(e => e.DataResposta).HasColumnName("DataResposta").HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relacionamentos
                entity.HasOne(r => r.Tentativa)
                    .WithMany(t => t.Respostas)
                    .HasForeignKey(r => r.TentativaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Questao)
                    .WithMany()
                    .HasForeignKey(r => r.QuestaoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.OpcaoSelecionada)
                    .WithMany()
                    .HasForeignKey(r => r.OpcaoSelecionadaId)
                    .OnDelete(DeleteBehavior.SetNull);
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
