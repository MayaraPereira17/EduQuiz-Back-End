namespace eduQuizApis.Domain.Enums
{
    /// <summary>
    /// Enum que define os tipos de usuários no sistema EduQuiz
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Estudantes do sistema
        /// </summary>
        Aluno = 0,

        /// <summary>
        /// Instrutores/Professores
        /// </summary>
        Professor = 1,

        /// <summary>
        /// Chat/Técnico de Futebol (Administrador)
        /// </summary>
        TecnicoFutebol = 2
    }
}
