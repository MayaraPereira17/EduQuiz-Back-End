using System.ComponentModel.DataAnnotations;

namespace eduQuizApis.Application.DTOs
{
    // DTOs para Dashboard do Professor
    public class DashboardProfessorDTO
    {
        public int QuizzesCriados { get; set; }
        public decimal MediaDosAlunos { get; set; }
        public int TotalAlunos { get; set; }
        public int TotalTentativas { get; set; }
        public List<QuizResumoDTO> QuizzesRecentes { get; set; } = new List<QuizResumoDTO>();
    }

    public class QuizResumoDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int TotalTentativas { get; set; }
        public decimal MediaPontuacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Publicado { get; set; }
    }

    // DTOs para Gerenciamento de Quizzes
    public class QuizCompletoDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string Dificuldade { get; set; } = string.Empty;
        public int? TempoLimite { get; set; }
        public int MaxTentativas { get; set; } = 1;
        public bool Ativo { get; set; }
        public bool Publico { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public int TotalQuestoes { get; set; }
        public List<QuestaoCompletaDTO> Questoes { get; set; } = new List<QuestaoCompletaDTO>();
    }

    public class QuestaoCompletaDTO
    {
        public int Id { get; set; }
        public string TextoQuestao { get; set; } = string.Empty;
        public string TipoQuestao { get; set; } = string.Empty;
        public int Pontos { get; set; } = 1;
        public int OrdemIndice { get; set; }
        public bool Ativo { get; set; }
        public List<OpcaoCompletaDTO> Opcoes { get; set; } = new List<OpcaoCompletaDTO>();
    }

    public class OpcaoCompletaDTO
    {
        public int Id { get; set; }
        public string TextoOpcao { get; set; } = string.Empty;
        public bool Correta { get; set; }
        public int OrdemIndice { get; set; }
    }

    // DTOs para Criação/Edição de Quiz
    public class CriarQuizRequestDTO
    {
        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; } = string.Empty;
        
        public string? Descricao { get; set; }
        
        [Required]
        public int CategoriaId { get; set; }
        
        [Required]
        public string Dificuldade { get; set; } = "Media"; // Fácil, Média, Difícil
        
        public int? TempoLimite { get; set; }
        
        public int MaxTentativas { get; set; } = 1;
        
        public bool Publico { get; set; } = false;
        
        public List<CriarQuestaoRequestDTO> Questoes { get; set; } = new List<CriarQuestaoRequestDTO>();
    }

    public class CriarQuestaoRequestDTO
    {
        [Required]
        public string TextoQuestao { get; set; } = string.Empty;
        
        [Required]
        public string TipoQuestao { get; set; } = string.Empty;
        
        public int Pontos { get; set; } = 1;
        
        [Required]
        public int OrdemIndice { get; set; }
        
        public List<CriarOpcaoRequestDTO> Opcoes { get; set; } = new List<CriarOpcaoRequestDTO>();
    }

    public class CriarOpcaoRequestDTO
    {
        [Required]
        public string TextoOpcao { get; set; } = string.Empty;
        
        public bool Correta { get; set; } = false;
        
        [Required]
        public int OrdemIndice { get; set; }
    }

    public class AtualizarQuizRequestDTO
    {
        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; } = string.Empty;
        
        public string? Descricao { get; set; }
        
        [Required]
        public int CategoriaId { get; set; }
        
        [Required]
        public string Dificuldade { get; set; } = string.Empty;
        
        public int? TempoLimite { get; set; }
        
        public int MaxTentativas { get; set; } = 1;
        
        public bool Ativo { get; set; }
        
        public bool Publico { get; set; }
        
        public List<AtualizarQuestaoRequestDTO> Questoes { get; set; } = new List<AtualizarQuestaoRequestDTO>();
    }

    public class AtualizarQuestaoRequestDTO
    {
        public int? Id { get; set; } // null para novas questões
        
        [Required]
        public string TextoQuestao { get; set; } = string.Empty;
        
        [Required]
        public string TipoQuestao { get; set; } = string.Empty;
        
        public int Pontos { get; set; } = 1;
        
        [Required]
        public int OrdemIndice { get; set; }
        
        public List<AtualizarOpcaoRequestDTO> Opcoes { get; set; } = new List<AtualizarOpcaoRequestDTO>();
    }

    public class AtualizarOpcaoRequestDTO
    {
        public int? Id { get; set; } // null para novas opções
        
        [Required]
        public string TextoOpcao { get; set; } = string.Empty;
        
        public bool Correta { get; set; } = false;
        
        [Required]
        public int OrdemIndice { get; set; }
    }

    // DTOs para Listagem de Quizzes
    public class QuizListagemDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Dificuldade { get; set; } = string.Empty;
        public int? TempoLimite { get; set; }
        public int TotalQuestoes { get; set; }
        public int TotalTentativas { get; set; }
        public bool Publicado { get; set; }
        public DateTime DataCriacao { get; set; }
        public decimal MediaPontuacao { get; set; }
    }

    // DTOs para Estatísticas de Quiz
    public class EstatisticasQuizDTO
    {
        public int QuizId { get; set; }
        public string TituloQuiz { get; set; } = string.Empty;
        public int TotalTentativas { get; set; }
        public int TotalAlunos { get; set; }
        public decimal MediaPontuacao { get; set; }
        public decimal MediaTempo { get; set; }
        public int TotalQuestoes { get; set; }
        public List<EstatisticaQuestaoDTO> EstatisticasQuestoes { get; set; } = new List<EstatisticaQuestaoDTO>();
        public List<TentativaResumoDTO> TentativasRecentes { get; set; } = new List<TentativaResumoDTO>();
    }

    public class EstatisticaQuestaoDTO
    {
        public int QuestaoId { get; set; }
        public string TextoQuestao { get; set; } = string.Empty;
        public int TotalRespostas { get; set; }
        public int RespostasCorretas { get; set; }
        public int RespostasErradas { get; set; }
        public decimal PercentualAcerto { get; set; }
    }

    public class TentativaResumoDTO
    {
        public int TentativaId { get; set; }
        public string NomeAluno { get; set; } = string.Empty;
        public int Pontuacao { get; set; }
        public int PontuacaoMaxima { get; set; }
        public decimal Percentual { get; set; }
        public int TempoGasto { get; set; }
        public DateTime DataConclusao { get; set; }
    }

    // DTOs para Perfil do Professor
    public class PerfilProfessorDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Funcao { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public DateTime? DataNascimento { get; set; }
        public string? Escola { get; set; }
        public string? Disciplina { get; set; }
        public DateTime DataCriacao { get; set; }
        public EstatisticasProfessorDTO Estatisticas { get; set; } = new EstatisticasProfessorDTO();
    }

    public class EstatisticasProfessorDTO
    {
        public int QuizzesCriados { get; set; }
        public decimal MediaDosAlunos { get; set; }
        public int TotalAlunos { get; set; }
        public int TotalTentativas { get; set; }
        public int QuizzesPublicados { get; set; }
    }

    public class AtualizarPerfilProfessorRequestDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public DateTime? DataNascimento { get; set; }
        public string? Escola { get; set; }
        public string? Disciplina { get; set; }
    }

    // DTOs para Categorias
    public class CategoriaDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public bool Ativo { get; set; }
    }

    // DTOs para Resposta de Operações
    public class CriarQuizResponseDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = "Quiz criado com sucesso!";
        public bool Sucesso { get; set; } = true;
    }

    public class AtualizarQuizResponseDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = "Quiz atualizado com sucesso!";
        public bool Sucesso { get; set; } = true;
    }

    public class DeletarQuizResponseDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = "Quiz deletado com sucesso!";
        public bool Sucesso { get; set; } = true;
    }
}
