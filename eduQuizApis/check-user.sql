-- -- Verificar se o usuário técnico existe
-- SELECT * FROM Users WHERE Email = 'tecnico@eduquiz.com';

-- -- Se não existir, criar o usuário técnico
-- INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedAt, UpdatedAt)
-- VALUES (
--     'tecnico', 
--     'tecnico@eduquiz.com', 
--     '$2a$11$rQZ8K9vL2mN3pO4qR5sT6uV7wX8yZ9aB0cD1eF2gH3iJ4kL5mN6oP7qR8sT9uV', 
--     'Técnico', 
--     'Matemática', 
--     2, -- TecnicoFutebol = 2
--     1, -- IsActive = true
--     NOW(), 
--     NOW()
-- );

-- -- Verificar todas as categorias
-- SELECT * FROM Categories;

-- -- Verificar configurações
-- SELECT * FROM SystemSettings;
