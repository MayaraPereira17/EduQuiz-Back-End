-- Script para adicionar tabelas de Times ao banco de dados EduQuiz
-- Execute este script após as tabelas principais estarem criadas

-- Tabela de Times
CREATE TABLE IF NOT EXISTS Times (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    TecnicoId INT NOT NULL,
    DataCriacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    INDEX idx_times_tecnico (TecnicoId),
    INDEX idx_times_ativo (IsActive),
    FOREIGN KEY (TecnicoId) REFERENCES Usuarios(Id) ON DELETE RESTRICT
);

-- Tabela de JogadoresTime (relacionamento entre Times e Alunos)
CREATE TABLE IF NOT EXISTS JogadoresTime (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    TimeId INT NOT NULL,
    AlunoId INT NOT NULL,
    DataEscalacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_jogadores_time (TimeId),
    INDEX idx_jogadores_aluno (AlunoId),
    FOREIGN KEY (TimeId) REFERENCES Times(Id) ON DELETE CASCADE,
    FOREIGN KEY (AlunoId) REFERENCES Usuarios(Id) ON DELETE CASCADE
    -- Permite que um aluno esteja em múltiplos times
    -- Se quiser permitir apenas um time por aluno, descomentar a linha abaixo:
    -- , UNIQUE KEY unique_aluno_time (AlunoId)
);

-- Verificar se as tabelas foram criadas
-- SELECT * FROM Times;
-- SELECT * FROM JogadoresTime;

