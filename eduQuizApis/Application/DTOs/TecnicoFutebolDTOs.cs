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
        public string Username { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? CPF { get; set; }
        public int Idade { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? AvatarUrl { get; set; }
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

    // Atualizar Aluno
    public class AtualizarAlunoRequestDTO
    {
        public string? Username { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? CPF { get; set; }
        public int? Idade { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? AvatarUrl { get; set; }
    }

    // Excluir Aluno Response
    public class ExcluirAlunoResponseDTO
    {
        public string Message { get; set; } = string.Empty;
        public int AlunoId { get; set; }
    }

    // Gerenciar Professores
    public class GerenciarProfessoresDTO
    {
        public List<ProfessorDTO> Professores { get; set; } = new List<ProfessorDTO>();
        public int TotalProfessores { get; set; }
    }

    public class ProfessorDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? CPF { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Instituicao { get; set; }
        public string? AreaEspecializacao { get; set; }
        public int TotalQuizzes { get; set; }
        public DateTime DataCadastro { get; set; }
    }

    // Atualizar Professor
    public class AtualizarProfessorRequestDTO
    {
        public string? Username { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? CPF { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Instituicao { get; set; }
        public string? AreaEspecializacao { get; set; }
    }

    // Excluir Professor Response
    public class ExcluirProfessorResponseDTO
    {
        public string Message { get; set; } = string.Empty;
        public int ProfessorId { get; set; }
    }

    // Gerenciar Times
    public class GerenciarTimesDTO
    {
        public List<TimeDTO> Times { get; set; } = new List<TimeDTO>();
        public int TotalTimes { get; set; }
    }

    public class TimeDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public List<JogadorTimeDTO> Jogadores { get; set; } = new List<JogadorTimeDTO>();
    }

    public class JogadorTimeDTO
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Posicao { get; set; }
        public decimal ScoreGeral { get; set; }
    }

    // Criar Time
    public class CriarTimeRequestDTO
    {
        public string Nome { get; set; } = string.Empty;
        public List<int> JogadoresIds { get; set; } = new List<int>();
    }

    // Adicionar Jogador ao Time
    public class AdicionarJogadorRequestDTO
    {
        public int AlunoId { get; set; }
    }

    // Remover Jogador do Time
    public class RemoverJogadorResponseDTO
    {
        public string Message { get; set; } = string.Empty;
    }

    // Deletar Time
    public class DeletarTimeResponseDTO
    {
        public string Message { get; set; } = string.Empty;
        public int TimeId { get; set; }
    }
}
