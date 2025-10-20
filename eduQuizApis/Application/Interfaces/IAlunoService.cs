using eduQuizApis.Application.DTOs;

namespace eduQuizApis.Application.Interfaces
{
    public interface IAlunoService
    {
        // Dashboard
        Task<DashboardAlunoDTO> ObterDashboardAsync(int usuarioId);
        
        // Quiz
        Task<List<QuizDisponivelDTO>> ObterQuizzesDisponiveisAsync(int usuarioId);
        Task<QuizDetalhesDTO> ObterQuizPorIdAsync(int usuarioId, int quizId);
        Task<ResponderQuizResponseDTO> ResponderQuizAsync(int usuarioId, int quizId, ResponderQuizRequestDTO request);
        Task<IniciarQuizResponseDTO> IniciarQuizAsync(int usuarioId, IniciarQuizRequestDTO request);
        Task<ResponderQuestaoResponseDTO> ResponderQuestaoAsync(int usuarioId, int tentativaId, ResponderQuestaoRequestDTO request);
        Task<ProgressoQuizDTO> ObterProgressoQuizAsync(int usuarioId, int tentativaId);
        Task<ResultadoQuizDTO> FinalizarQuizAsync(int usuarioId, int tentativaId);
        
        // Ranking
        Task<RankingCompletoDTO> ObterRankingCompletoAsync(int usuarioId, string? busca = null);
        
        // Perfil
        Task<PerfilAlunoDTO> ObterPerfilAsync(int usuarioId);
        Task<PerfilAlunoDTO> AtualizarPerfilAsync(int usuarioId, AtualizarPerfilRequestDTO request);
        Task<List<DesempenhoQuizDTO>> ObterDesempenhoAsync(int usuarioId);
        Task<List<AtividadeRecenteDTO>> ObterAtividadesRecentesAsync(int usuarioId);
    }
}
