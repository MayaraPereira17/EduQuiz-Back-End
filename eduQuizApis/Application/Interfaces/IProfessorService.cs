using eduQuizApis.Application.DTOs;

namespace eduQuizApis.Application.Interfaces
{
    public interface IProfessorService
    {
        // Dashboard
        Task<DashboardProfessorDTO> ObterDashboardAsync(int professorId);
        
        // Gerenciamento de Quizzes
        Task<List<QuizListagemDTO>> ObterMeusQuizzesAsync(int professorId, string? busca = null);
        Task<QuizCompletoDTO> ObterQuizPorIdAsync(int professorId, int quizId);
        Task<CriarQuizResponseDTO> CriarQuizAsync(int professorId, CriarQuizRequestDTO request);
        Task<AtualizarQuizResponseDTO> AtualizarQuizAsync(int professorId, int quizId, AtualizarQuizRequestDTO request);
        Task<DeletarQuizResponseDTO> DeletarQuizAsync(int professorId, int quizId);
        Task<AtualizarQuizResponseDTO> PublicarQuizAsync(int professorId, int quizId);
        Task<AtualizarQuizResponseDTO> DespublicarQuizAsync(int professorId, int quizId);
        
        // Estatísticas
        Task<EstatisticasQuizDTO> ObterEstatisticasQuizAsync(int professorId, int quizId);
        Task<List<EstatisticaQuestaoDTO>> ObterEstatisticasQuestoesAsync(int professorId, int quizId);
        Task<List<TentativaResumoDTO>> ObterTentativasQuizAsync(int professorId, int quizId);
        
        // Perfil
        Task<PerfilProfessorDTO> ObterPerfilAsync(int professorId);
        Task<PerfilProfessorDTO> AtualizarPerfilAsync(int professorId, AtualizarPerfilProfessorRequestDTO request);
        
        // Categorias
        Task<List<CategoriaDTO>> ObterCategoriasAsync();
        Task<CategoriaDTO> ObterCategoriaPorIdAsync(int categoriaId);
    }
}
