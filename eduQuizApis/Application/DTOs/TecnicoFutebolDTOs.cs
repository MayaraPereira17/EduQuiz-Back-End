namespace eduQuizApis.Application.DTOs
{
    // Dashboard do Técnico
    public class DashboardTecnicoDTO
    {
        public string NomeTecnico { get; set; } = string.Empty;
        public string EmailTecnico { get; set; } = string.Empty;
        public int TotalAlunos { get; set; }
        public decimal PerformanceGeral { get; set; }
        public List<MelhorAlunoDTO> MelhoresAlunos { get; set; } = new List<MelhorAlunoDTO>();
    }

    public class MelhorAlunoDTO
    {
        public int Posicao { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Sequencia { get; set; }
        public decimal Performance { get; set; }
        public int TotalQuizzes { get; set; }
    }

    // Gerenciar Alunos
    public class GerenciarAlunosDTO
    {
        public List<AlunoRankingDTO> Alunos { get; set; } = new List<AlunoRankingDTO>();
        public int TotalAlunos { get; set; }
    }

    public class AlunoRankingDTO
    {
        public int Id { get; set; }
        public int Posicao { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Idade { get; set; }
        public int TotalQuizzes { get; set; }
        public decimal ScoreGeral { get; set; }
        public DateTime? UltimoQuiz { get; set; }
    }

    // Relatório de Desempenho
    public class RelatorioDesempenhoDTO
    {
        public List<DesempenhoAlunoDTO> Alunos { get; set; } = new List<DesempenhoAlunoDTO>();
        public int TotalAlunos { get; set; }
        public decimal MediaGeral { get; set; }
    }

    public class DesempenhoAlunoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int TotalQuizzes { get; set; }
        public DateTime? UltimoQuiz { get; set; }
        public decimal ScoreGeral { get; set; }
    }

    // Perfil do Técnico
    public class PerfilTecnicoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Funcao { get; set; } = string.Empty;
        public string Instituicao { get; set; } = string.Empty;
        public int TotalAlunos { get; set; }
        public decimal MediaTurma { get; set; }
    }

    public class AtualizarPerfilTecnicoRequestDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
