using eduQuizApis.Application.DTOs;

namespace eduQuizApis.Application.Interfaces
{
    public interface ITecnicoFutebolService
    {
        Task<DashboardTecnicoDTO> ObterDashboardAsync(int tecnicoId);
        Task<GerenciarAlunosDTO> ObterGerenciarAlunosAsync(int tecnicoId, string? busca = null);
        Task<RelatorioDesempenhoDTO> ObterRelatorioDesempenhoAsync(int tecnicoId);
        Task<PerfilTecnicoDTO> ObterPerfilAsync(int tecnicoId);
        Task<PerfilTecnicoDTO> AtualizarPerfilAsync(int tecnicoId, AtualizarPerfilTecnicoRequestDTO request);
        Task<AlunoRankingDTO> AtualizarAlunoAsync(int tecnicoId, int alunoId, AtualizarAlunoRequestDTO request);
        Task<ExcluirAlunoResponseDTO> ExcluirAlunoAsync(int tecnicoId, int alunoId);
        Task<GerenciarProfessoresDTO> ObterProfessoresAsync(int tecnicoId, string? busca = null);
        Task<ProfessorDTO> AtualizarProfessorAsync(int tecnicoId, int professorId, AtualizarProfessorRequestDTO request);
        Task<ExcluirProfessorResponseDTO> ExcluirProfessorAsync(int tecnicoId, int professorId);
        
        // Gerenciar Times
        Task<GerenciarTimesDTO> ObterTimesAsync(int tecnicoId);
        Task<TimeDTO> CriarTimeAsync(int tecnicoId, CriarTimeRequestDTO request);
        Task<TimeDTO> AdicionarJogadorAoTimeAsync(int tecnicoId, int timeId, AdicionarJogadorRequestDTO request);
        Task<RemoverJogadorResponseDTO> RemoverJogadorDoTimeAsync(int tecnicoId, int timeId, int jogadorId);
        Task<DeletarTimeResponseDTO> DeletarTimeAsync(int tecnicoId, int timeId);
        
        // Exportar Relatório
        Task<byte[]> ExportarRelatorioAsync(int tecnicoId, string formato, int? quantidade = null);
    }
}
