using System.ComponentModel.DataAnnotations;

namespace eduQuizApis.Application.DTOs
{
    // DTOs para Dashboard do Aluno
    public class DashboardAlunoDTO
    {
        public int QuizzesCompletos { get; set; }
        public decimal MediaGeral { get; set; }
        public int PosicaoRanking { get; set; }
        public int Sequencia { get; set; }
        public int Pontos { get; set; }
        public int TotalUsuarios { get; set; }
        public List<QuizRecenteDTO> QuizzesRecentes { get; set; } = new List<QuizRecenteDTO>();
        public List<TimeEscalacaoDTO> TimesEscalados { get; set; } = new List<TimeEscalacaoDTO>();
    }

    public class TimeEscalacaoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public DateTime DataEscalacao { get; set; }
    }

    public class QuizRecenteDTO
    {
        public int QuizId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal PercentualAcerto { get; set; }
        public DateTime DataConclusao { get; set; }
    }

    // DTOs para Quiz
    public class QuizDisponivelDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Dificuldade { get; set; } = string.Empty;
        public int? TempoLimite { get; set; }
        public int TotalQuestoes { get; set; }
        public int PontuacaoTotal { get; set; }
        public bool Disponivel { get; set; }
        public bool QuizConcluido { get; set; } = false;
        public int TentativasRestantes { get; set; } = 1;
    }

    public class IniciarQuizRequestDTO
    {
        [Required]
        public int QuizId { get; set; }
    }

    public class IniciarQuizResponseDTO
    {
        public int TentativaId { get; set; }
        public int QuizId { get; set; }
        public string TituloQuiz { get; set; } = string.Empty;
        public QuestaoAtualDTO QuestaoAtual { get; set; } = new QuestaoAtualDTO();
        public ProgressoQuizDTO Progresso { get; set; } = new ProgressoQuizDTO();
    }

    public class QuestaoAtualDTO
    {
        public int Id { get; set; }
        public string TextoQuestao { get; set; } = string.Empty;
        public string TipoQuestao { get; set; } = string.Empty;
        public List<OpcaoRespostaDTO> Opcoes { get; set; } = new List<OpcaoRespostaDTO>();
        public int Pontos { get; set; }
        public int OrdemIndice { get; set; }
    }

    public class OpcaoRespostaDTO
    {
        public int Id { get; set; }
        public string TextoOpcao { get; set; } = string.Empty;
        public int OrdemIndice { get; set; }
    }

    public class ProgressoQuizDTO
    {
        public int QuestaoAtual { get; set; }
        public int TotalQuestoes { get; set; }
        public decimal PercentualCompleto { get; set; }
        public int PontuacaoAtual { get; set; }
        public int TempoGasto { get; set; } // em segundos
    }

    public class ResponderQuestaoRequestDTO
    {
        [Required]
        public int QuestaoId { get; set; }
        public int? OpcaoSelecionadaId { get; set; }
        public string? TextoResposta { get; set; }
    }

    public class ResponderQuestaoResponseDTO
    {
        public bool RespostaCorreta { get; set; }
        public int PontosGanhos { get; set; }
        public string RespostaCorretaTexto { get; set; } = string.Empty;
        public string Feedback { get; set; } = string.Empty;
        public QuestaoAtualDTO? ProximaQuestao { get; set; }
        public bool QuizConcluido { get; set; }
        public ResultadoQuizDTO? ResultadoFinal { get; set; }
    }

    public class ResultadoQuizDTO
    {
        public int TentativaId { get; set; }
        public int PontuacaoFinal { get; set; }
        public int PontuacaoMaxima { get; set; }
        public decimal PercentualAcerto { get; set; }
        public int TempoGasto { get; set; }
        public int TotalQuestoes { get; set; }
        public int RespostasCorretas { get; set; }
        public int RespostasErradas { get; set; }
        public DateTime DataConclusao { get; set; }
    }

    // DTOs para Ranking
    public class RankingAlunoDTO
    {
        public int Posicao { get; set; }
        public int UsuarioId { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public int Pontos { get; set; }
        public int Quizzes { get; set; }
        public decimal Media { get; set; }
        public int Sequencia { get; set; }
    }

    public class RankingCompletoDTO
    {
        public List<RankingAlunoDTO> Alunos { get; set; } = new List<RankingAlunoDTO>();
        public int TotalAlunos { get; set; }
        public int PosicaoUsuarioLogado { get; set; }
    }

    // DTOs para Tentativas
    public class TentativaQuizDTO
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string TituloQuiz { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime? DataConclusao { get; set; }
        public bool Concluida { get; set; }
        public decimal? Pontuacao { get; set; }
        public decimal? PontuacaoMaxima { get; set; }
        public decimal? PercentualAcerto { get; set; }
        public int? TempoGasto { get; set; }
        public string Status { get; set; } = string.Empty; // "Em Andamento", "Concluído", "Pendente"
    }

    public class DetalhesTentativaDTO
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string TituloQuiz { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime? DataConclusao { get; set; }
        public bool Concluida { get; set; }
        public decimal? Pontuacao { get; set; }
        public decimal? PontuacaoMaxima { get; set; }
        public decimal? PercentualAcerto { get; set; }
        public int? TempoGasto { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<RespostaTentativaDTO> Respostas { get; set; } = new List<RespostaTentativaDTO>();
    }

    public class RespostaTentativaDTO
    {
        public int Id { get; set; }
        public int QuestaoId { get; set; }
        public string TextoQuestao { get; set; } = string.Empty;
        public string TipoQuestao { get; set; } = string.Empty;
        public int? OpcaoSelecionadaId { get; set; }
        public string? TextoResposta { get; set; }
        public bool? Correta { get; set; }
        public decimal PontosGanhos { get; set; }
        public DateTime DataResposta { get; set; }
        public List<OpcaoRespostaDTO> Opcoes { get; set; } = new List<OpcaoRespostaDTO>();
    }

    // DTOs para Perfil
    public class PerfilAlunoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Funcao { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public DateTime? DataNascimento { get; set; }
        public DateTime DataCriacao { get; set; }
        public EstatisticasPerfilDTO Estatisticas { get; set; } = new EstatisticasPerfilDTO();
    }

    public class EstatisticasPerfilDTO
    {
        public int QuizzesCompletos { get; set; }
        public decimal MediaGeral { get; set; }
        public int Sequencia { get; set; }
        public int Pontos { get; set; }
    }

    public class AtualizarPerfilRequestDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public DateTime? DataNascimento { get; set; }
    }

    public class DesempenhoQuizDTO
    {
        public int QuizId { get; set; }
        public string TituloQuiz { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal PercentualAcerto { get; set; }
        public int Pontuacao { get; set; }
        public int PontuacaoMaxima { get; set; }
        public DateTime DataConclusao { get; set; }
        public int TempoGasto { get; set; }
    }

    public class AtividadeRecenteDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string Icone { get; set; } = string.Empty;
        public string Cor { get; set; } = string.Empty;
    }

    // DTOs para Quiz Detalhado
    public class QuizDetalhesDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public CategoriaDTO Categoria { get; set; } = new CategoriaDTO();
        public string Dificuldade { get; set; } = string.Empty;
        public int? TempoLimite { get; set; }
        public int MaxTentativas { get; set; }
        public int TentativasRestantes { get; set; }
        public int TotalQuestoes { get; set; }
        public string CriadoPor { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public List<QuestaoDetalhesDTO> Questoes { get; set; } = new List<QuestaoDetalhesDTO>();
    }


    public class QuestaoDetalhesDTO
    {
        public int Id { get; set; }
        public string TextoQuestao { get; set; } = string.Empty;
        public string TipoQuestao { get; set; } = string.Empty;
        public int Pontos { get; set; }
        public int OrdemIndice { get; set; }
        public List<OpcaoDetalhesDTO> Opcoes { get; set; } = new List<OpcaoDetalhesDTO>();
    }

    public class OpcaoDetalhesDTO
    {
        public int Id { get; set; }
        public string TextoOpcao { get; set; } = string.Empty;
        public int OrdemIndice { get; set; }
    }

    // DTOs para Responder Quiz Completo
    public class ResponderQuizRequestDTO
    {
        [Required]
        public List<RespostaQuizDTO> Respostas { get; set; } = new List<RespostaQuizDTO>();
    }

    public class RespostaQuizDTO
    {
        [Required]
        public int QuestaoId { get; set; }
        public int? OpcaoSelecionadaId { get; set; }
        public string? TextoResposta { get; set; }
    }

    public class ResponderQuizResponseDTO
    {
        public int TentativaId { get; set; }
        public int QuizId { get; set; }
        public int AlunoId { get; set; }
        public int PontuacaoTotal { get; set; }
        public int PontuacaoMaxima { get; set; }
        public decimal PercentualAcerto { get; set; }
        public DateTime DataTentativa { get; set; }
        public int TempoGasto { get; set; }
        public int RespostasCorretas { get; set; }
        public int RespostasIncorretas { get; set; }
        public List<RespostaResultadoDTO> Respostas { get; set; } = new List<RespostaResultadoDTO>();
        public string Message { get; set; } = string.Empty;
        public bool NovoRecorde { get; set; }
    }

    public class RespostaResultadoDTO
    {
        public int QuestaoId { get; set; }
        public int? OpcaoSelecionadaId { get; set; }
        public string? TextoRespostaSelecionada { get; set; }
        public int? OpcaoCorretaId { get; set; }
        public string? TextoRespostaCorreta { get; set; }
        public bool Correta { get; set; }
        public int PontosObtidos { get; set; }
    }
}
