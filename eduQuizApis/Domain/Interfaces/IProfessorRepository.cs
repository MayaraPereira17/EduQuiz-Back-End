using eduQuizApis.Infrastructure.Data;
using eduQuizApis.Domain.Entities;

namespace eduQuizApis.Domain.Interfaces
{
    public interface IProfessorRepository
    {
        // Quizzes
        Task<List<Quizzes>> ObterQuizzesPorProfessorAsync(int professorId);
        Task<Quizzes?> ObterQuizPorIdEProfessorAsync(int quizId, int professorId);
        Task<bool> ProfessorPodeEditarQuizAsync(int professorId, int quizId);
        Task<bool> QuizTemTentativasAsync(int quizId);
        
        // Estatísticas
        Task<int> ContarQuizzesCriadosAsync(int professorId);
        Task<int> ContarQuizzesPublicadosAsync(int professorId);
        Task<decimal> CalcularMediaAlunosQuizAsync(int quizId);
        Task<List<TentativasQuiz>> ObterTentativasPorQuizAsync(int quizId);
        Task<List<TentativasQuiz>> ObterTentativasPorProfessorAsync(int professorId);
        
        // Categorias
        Task<List<Categorias>> ObterCategoriasAtivasAsync();
        Task<Categorias?> ObterCategoriaPorIdAsync(int categoriaId);
        
        // Questões e Opções
        Task<List<Questoes>> ObterQuestoesPorQuizAsync(int quizId);
        Task<List<OpcoesQuestao>> ObterOpcoesPorQuestaoAsync(int questaoId);
        Task<bool> QuestaoPertenceAoQuizAsync(int questaoId, int quizId);
        
        // Relatórios
        Task<List<RelatoriosPerformance>> ObterRelatoriosPorQuizAsync(int quizId);
        Task<List<RelatoriosPerformance>> ObterRelatoriosPorProfessorAsync(int professorId);
        
        // Validações
        Task<bool> UsuarioEPossuiQuizAsync(int professorId, int quizId);
        Task<bool> CategoriaExisteEAtivaAsync(int categoriaId);
    }
}
