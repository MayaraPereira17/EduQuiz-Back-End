-- Script completo de criação do banco de dados EduQuiz
-- Baseado nas entidades atuais do código
-- Execute este script no MySQL do Railway

USE railway;

-- Tabela de Usuários (entidade User)
CREATE TABLE IF NOT EXISTS Usuarios (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    CPF VARCHAR(14) NULL,
    DataNascimento DATE NULL,
    AvatarUrl VARCHAR(500) NULL,
    Role ENUM('Aluno', 'Professor', 'TecnicoFutebol') NOT NULL DEFAULT 'Aluno',
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Tabela de Categorias
CREATE TABLE IF NOT EXISTS Categorias (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Descricao TEXT,
    Ativo BOOLEAN NOT NULL DEFAULT TRUE,
    DataCriacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DataAtualizacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Tabela de Quizzes (entidade Quizzes)
CREATE TABLE IF NOT EXISTS Quizzes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Titulo VARCHAR(200) NOT NULL,
    Descricao TEXT,
    CategoriaId INT NOT NULL,
    CriadoPor INT NOT NULL,
    Dificuldade VARCHAR(20) NOT NULL DEFAULT 'Media',
    TempoLimite INT NULL, -- em minutos, NULL = sem limite
    MaxTentativas INT DEFAULT 1,
    Ativo BOOLEAN NOT NULL DEFAULT TRUE,
    Publico BOOLEAN NOT NULL DEFAULT FALSE,
    DataCriacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DataAtualizacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id) ON DELETE CASCADE,
    FOREIGN KEY (CriadoPor) REFERENCES Usuarios(Id) ON DELETE CASCADE
);

-- Tabela de Questões (entidade Questoes)
CREATE TABLE IF NOT EXISTS Questoes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    QuizId INT NOT NULL,
    TextoQuestao TEXT NOT NULL,
    TipoQuestao VARCHAR(50) NOT NULL,
    Pontos INT NOT NULL DEFAULT 1,
    OrdemIndice INT NOT NULL,
    Ativo BOOLEAN NOT NULL DEFAULT TRUE,
    DataCriacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DataAtualizacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (QuizId) REFERENCES Quizzes(Id) ON DELETE CASCADE
);

-- Tabela de Opções de Resposta (entidade OpcoesQuestao)
CREATE TABLE IF NOT EXISTS OpcoesQuestao (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    QuestaoId INT NOT NULL,
    TextoOpcao TEXT NOT NULL,
    Correta BOOLEAN NOT NULL DEFAULT FALSE,
    OrdemIndice INT NOT NULL,
    DataCriacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (QuestaoId) REFERENCES Questoes(Id) ON DELETE CASCADE
);

-- Tabela de Tentativas de Quiz (entidade TentativasQuiz)
CREATE TABLE IF NOT EXISTS TentativasQuiz (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    QuizId INT NOT NULL,
    UsuarioId INT NOT NULL,
    DataInicio DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DataConclusao DATETIME NULL,
    Pontuacao DECIMAL(5,2) NULL,
    PontuacaoMaxima DECIMAL(5,2) NULL,
    TempoGasto INT NULL, -- em segundos
    Concluida BOOLEAN NOT NULL DEFAULT FALSE,
    DataCriacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (QuizId) REFERENCES Quizzes(Id) ON DELETE CASCADE,
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id) ON DELETE CASCADE
);

-- Tabela de Respostas (entidade Respostas)
CREATE TABLE IF NOT EXISTS Respostas (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    TentativaId INT NOT NULL,
    QuestaoId INT NOT NULL,
    OpcaoSelecionadaId INT NULL,
    TextoResposta TEXT NULL,
    Correta BOOLEAN NULL,
    PontosGanhos DECIMAL(5,2) DEFAULT 0,
    DataResposta DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (TentativaId) REFERENCES TentativasQuiz(Id) ON DELETE CASCADE,
    FOREIGN KEY (QuestaoId) REFERENCES Questoes(Id) ON DELETE CASCADE,
    FOREIGN KEY (OpcaoSelecionadaId) REFERENCES OpcoesQuestao(Id) ON DELETE SET NULL
);

-- Tabela de Relatórios de Performance (entidade RelatoriosPerformance)
CREATE TABLE IF NOT EXISTS RelatoriosPerformance (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioId INT NOT NULL,
    QuizId INT NOT NULL,
    TentativaId INT NOT NULL,
    TotalQuestoes INT NOT NULL,
    RespostasCorretas INT NOT NULL,
    RespostasErradas INT NOT NULL,
    Percentual DECIMAL(5,2) NOT NULL,
    TempoGasto INT NOT NULL, -- em segundos
    DataCriacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id) ON DELETE CASCADE,
    FOREIGN KEY (QuizId) REFERENCES Quizzes(Id) ON DELETE CASCADE,
    FOREIGN KEY (TentativaId) REFERENCES TentativasQuiz(Id) ON DELETE CASCADE
);

-- Tabela de Configurações do Sistema (entidade ConfiguracoesSistema)
CREATE TABLE IF NOT EXISTS ConfiguracoesSistema (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ChaveConfiguracao VARCHAR(100) NOT NULL UNIQUE,
    ValorConfiguracao TEXT,
    Descricao TEXT,
    DataAtualizacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Tabela de Ranking de Alunos (entidade RankingAlunos)
CREATE TABLE IF NOT EXISTS RankingAlunos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioId INT NOT NULL,
    CategoriaId INT NOT NULL,
    PontuacaoTotal DECIMAL(10,2) NOT NULL DEFAULT 0,
    TotalQuizzes INT NOT NULL DEFAULT 0,
    MediaPontuacao DECIMAL(5,2) NOT NULL DEFAULT 0,
    PosicaoRanking INT NOT NULL DEFAULT 0,
    NivelAtual VARCHAR(50) NOT NULL DEFAULT 'Iniciante',
    PontosExperiencia INT NOT NULL DEFAULT 0,
    DataAtualizacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id) ON DELETE CASCADE,
    FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id) ON DELETE CASCADE,
    UNIQUE KEY unique_user_category (UsuarioId, CategoriaId)
);

-- Tabela de Conquistas/Badges (entidade Conquistas)
CREATE TABLE IF NOT EXISTS Conquistas (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Descricao TEXT,
    TipoConquista VARCHAR(50) NOT NULL,
    CriterioMinimo DECIMAL(10,2) NOT NULL,
    Icone VARCHAR(50),
    Cor VARCHAR(20),
    Ativo BOOLEAN NOT NULL DEFAULT TRUE,
    DataCriacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de Conquistas dos Alunos (entidade ConquistasAlunos)
CREATE TABLE IF NOT EXISTS ConquistasAlunos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioId INT NOT NULL,
    ConquistaId INT NOT NULL,
    DataConquista DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id) ON DELETE CASCADE,
    FOREIGN KEY (ConquistaId) REFERENCES Conquistas(Id) ON DELETE CASCADE,
    UNIQUE KEY unique_user_achievement (UsuarioId, ConquistaId)
);

-- Inserir dados iniciais - Categorias de Matemática
INSERT INTO Categorias (Nome, Descricao) VALUES 
('Matemática Básica', 'Operações fundamentais, frações, decimais'),
('Álgebra', 'Equações, funções, sistemas lineares'),
('Geometria', 'Formas geométricas, áreas, volumes'),
('Trigonometria', 'Seno, cosseno, tangente, identidades'),
('Cálculo', 'Derivadas, integrais, limites'),
('Estatística', 'Probabilidade, análise de dados, gráficos');

-- Inserir usuário técnico padrão
INSERT INTO Usuarios (Username, Email, PasswordHash, FirstName, LastName, Role) VALUES 
('tecnico', 'tecnico@eduquiz.com', '123', 'Técnico', 'Matemática', 'TecnicoFutebol');

-- Inserir configurações padrão
INSERT INTO ConfiguracoesSistema (ChaveConfiguracao, ValorConfiguracao, Descricao) VALUES 
('max_tentativas_quiz', '3', 'Número máximo de tentativas por quiz'),
('tempo_limite_padrao', '30', 'Tempo limite padrão para quizzes em minutos'),
('percentual_aprovacao', '70', 'Percentual mínimo para aprovação'),
('permitir_anonimo', 'false', 'Permitir acesso anônimo aos quizzes'),
('categoria_foco', 'Matemática', 'Categoria principal do sistema');

-- Inserir conquistas padrão
INSERT INTO Conquistas (Nome, Descricao, TipoConquista, CriterioMinimo, Icone, Cor) VALUES 
('Primeiro Passo', 'Complete seu primeiro quiz de matemática', 'Frequencia', 1, '🎯', '#4CAF50'),
('Matemático Iniciante', 'Complete 5 quizzes de matemática', 'Frequencia', 5, '📚', '#2196F3'),
('Matemático Intermediário', 'Complete 15 quizzes de matemática', 'Frequencia', 15, '📖', '#FF9800'),
('Matemático Avançado', 'Complete 30 quizzes de matemática', 'Frequencia', 30, '🎓', '#9C27B0'),
('Precisão Perfeita', 'Obtenha 100% de acerto em um quiz', 'Precisao', 100, '💯', '#F44336'),
('Velocidade da Luz', 'Complete um quiz em menos de 5 minutos', 'Velocidade', 300, '⚡', '#FFC107'),
('Mestre da Álgebra', 'Obtenha média de 90% em 10 quizzes de álgebra', 'Pontuacao', 90, '🧮', '#E91E63'),
('Gênio da Geometria', 'Obtenha média de 90% em 10 quizzes de geometria', 'Pontuacao', 90, '📐', '#00BCD4'),
('Calculista Expert', 'Obtenha média de 90% em 10 quizzes de cálculo', 'Pontuacao', 90, '📊', '#795548'),
('Campeão do Ranking', 'Fique em 1º lugar no ranking geral', 'Especial', 1, '🏆', '#FFD700');

-- Criar índices para melhor performance
CREATE INDEX idx_usuarios_role ON Usuarios(Role);
CREATE INDEX idx_usuarios_isactive ON Usuarios(IsActive);
CREATE INDEX idx_quizzes_category ON Quizzes(CategoriaId);
CREATE INDEX idx_quizzes_created_by ON Quizzes(CriadoPor);
CREATE INDEX idx_quizzes_dificuldade ON Quizzes(Dificuldade);
CREATE INDEX idx_quizzes_ativo_publico ON Quizzes(Ativo, Publico);
CREATE INDEX idx_questions_quiz ON Questoes(QuizId);
CREATE INDEX idx_questions_tipo ON Questoes(TipoQuestao);
CREATE INDEX idx_questions_ativo ON Questoes(Ativo);
CREATE INDEX idx_question_options_question ON OpcoesQuestao(QuestaoId);
CREATE INDEX idx_quiz_attempts_quiz_user ON TentativasQuiz(QuizId, UsuarioId);
CREATE INDEX idx_quiz_attempts_concluida ON TentativasQuiz(Concluida);
CREATE INDEX idx_answers_attempt_question ON Respostas(TentativaId, QuestaoId);
CREATE INDEX idx_answers_correta ON Respostas(Correta);
CREATE INDEX idx_performance_reports_user ON RelatoriosPerformance(UsuarioId);
CREATE INDEX idx_performance_reports_quiz ON RelatoriosPerformance(QuizId);
CREATE INDEX idx_ranking_posicao ON RankingAlunos(PosicaoRanking);
CREATE INDEX idx_ranking_pontuacao ON RankingAlunos(PontuacaoTotal);
CREATE INDEX idx_conquistas_tipo ON Conquistas(TipoConquista);
CREATE INDEX idx_conquistas_ativo ON Conquistas(Ativo);
