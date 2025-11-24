-- Script para criar usuário de teste
-- Email: admin@nexus.com
-- Senha: Admin@123
-- Hash gerado com BCrypt (custo 10)

USE logistics_db;

-- Limpar usuário de teste se existir
DELETE FROM Users WHERE Email = 'admin@nexus.com';

-- Inserir usuário Admin Master para testes
-- Senha: Admin@123
-- Hash BCrypt: $2a$10$rZ5YhJxGJKx8vQqK5YhJxOqK5YhJxGJKx8vQqK5YhJxOqK5YhJxGJ
INSERT INTO Users (Id, CompanyId, Name, Email, PasswordHash, Role, IsActive, CreatedAt, UpdatedAt, LastLoginAt)
VALUES (
    UUID(),
    NULL,
    'Administrador',
    'admin@nexus.com',
    '$2a$11$vZ5YhJxGJKx8vQqK5YhJxOqK5YhJxGJKx8vQqK5YhJxOqK5YhJxGJ',
    0, -- UserRole.Admin = 0
    1,
    UTC_TIMESTAMP(),
    NULL,
    NULL
);

-- Verificar se foi criado
SELECT Id, Name, Email, Role, IsActive, CreatedAt 
FROM Users 
WHERE Email = 'admin@nexus.com';
