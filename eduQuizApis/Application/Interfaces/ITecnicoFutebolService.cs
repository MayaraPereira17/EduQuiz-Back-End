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
    }
}
