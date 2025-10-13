using eduQuizApis.Infrastructure.Data;
using eduQuizApis.Domain.Entities;

namespace eduQuizApis.Domain.Interfaces
{
    public interface IAlunoRepository
    {
        Task<bool> UsuarioPodeAcessarQuizAsync(int usuarioId, int quizId);
        Task<int> ObterTentativasRestantesAsync(int usuarioId, int quizId);
        Task<bool> ExisteTentativaEmAndamentoAsync(int usuarioId, int quizId);
        Task<List<TentativasQuiz>> ObterTentativasConcluidasAsync(int usuarioId);
        Task<RankingAlunos?> ObterRankingUsuarioAsync(int usuarioId, int categoriaId);
        Task<List<RankingAlunos>> ObterRankingCompletoAsync(int categoriaId, int? limit = null);
        Task<List<RelatoriosPerformance>> ObterRelatoriosPerformanceAsync(int usuarioId);
        Task<List<ConquistasAlunos>> ObterConquistasUsuarioAsync(int usuarioId);
        Task<int> ObterTotalUsuariosAtivosAsync();
        Task<decimal> CalcularMediaGeralUsuarioAsync(int usuarioId);
        Task<int> CalcularSequenciaConsecutivaAsync(int usuarioId);
        Task<List<TentativasQuiz>> ObterTentativasRecentesAsync(int usuarioId, int limit = 5);
        Task<bool> VerificarConquistasDisponiveisAsync(int usuarioId);
    }
}
