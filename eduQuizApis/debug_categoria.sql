-- Script para verificar se a categoria ID 6 existe
USE railway;

-- Verificar se a categoria existe
SELECT * FROM Categorias WHERE Id = 6;

-- Ver todas as categorias
SELECT * FROM Categorias;

-- Verificar se há categorias ativas
SELECT * FROM Categorias WHERE Ativo = TRUE;
