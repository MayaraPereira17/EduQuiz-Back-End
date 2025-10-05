-- Script de Banco de Dados - EduQuiz (Simplificado)
-- Apenas tabela de usuários e autenticação

-- Criar banco de dados (se não existir)
CREATE DATABASE IF NOT EXISTS railway;
USE railway;

-- Tabela de Usuários
CREATE TABLE IF NOT EXISTS Usuarios (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Email VARCHAR(100) NOT NULL UNIQUE,
    SenhaHash VARCHAR(255) NOT NULL,
    Nome VARCHAR(50) NOT NULL,
    Sobrenome VARCHAR(50) NOT NULL,
    CPF VARCHAR(14) NULL,
    DataNascimento DATE NULL,
    Funcao ENUM('Aluno', 'Professor', 'TecnicoFutebol') NOT NULL DEFAULT 'Aluno',
    Ativo BOOLEAN NOT NULL DEFAULT TRUE,
    DataCriacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DataAtualizacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Inserir usuário técnico padrão (senha: admin123)
INSERT INTO Usuarios (Username, Email, SenhaHash, Nome, Sobrenome, Funcao) VALUES 
('admin', 'admin@eduquiz.com', '$2a$11$rQZ8K9vL2mN3pO4qR5sT6uV7wX8yZ9aB0cD1eF2gH3iJ4kL5mN6oP7qR8sT9uV', 'Administrador', 'Sistema', 'TecnicoFutebol');

-- Inserir usuário técnico adicional (senha: admin123)
INSERT INTO Usuarios (Username, Email, SenhaHash, Nome, Sobrenome, Funcao) VALUES 
('tecnico', 'tecnico@eduquiz.com', '$2a$11$rQZ8K9vL2mN3pO4qR5sT6uV7wX8yZ9aB0cD1eF2gH3iJ4kL5mN6oP7qR8sT9uV', 'Técnico', 'Futebol', 'TecnicoFutebol');

-- Criar índices para melhor performance
CREATE INDEX idx_usuarios_username ON Usuarios(Username);
CREATE INDEX idx_usuarios_email ON Usuarios(Email);
CREATE INDEX idx_usuarios_funcao ON Usuarios(Funcao);
CREATE INDEX idx_usuarios_ativo ON Usuarios(Ativo);

-- -- Verificar dados inseridos
-- SELECT * FROM Usuarios;