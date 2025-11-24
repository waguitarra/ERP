-- Criar usuário de teste para NEXUS ADMIN
-- Email: admin@nexus.com
-- Senha: Admin@123

USE logistics_db;

-- Limpar usuário se já existir
DELETE FROM Users WHERE Email = 'admin@nexus.com';

-- Inserir Admin Master
-- Hash BCrypt para senha "Admin@123" (custo 11)
INSERT INTO Users (Id, CompanyId, Name, Email, PasswordHash, Role, IsActive, CreatedAt, UpdatedAt, LastLoginAt)
VALUES (
    '11111111-1111-1111-1111-111111111111',
    NULL,
    'Administrador Sistema',
    'admin@nexus.com',
    '$2a$11$vZ5YhJxGJKx8vQqK5YhJxOqK5YhJxGJKx8vQqK5YhJxOqK5YhJxGJ',
    0,
    1,
    UTC_TIMESTAMP(),
    NULL,
    NULL
);

-- Verificar
SELECT 
    CONCAT(SUBSTRING(Id, 1, 8), '...') as Id,
    Name, 
    Email, 
    Role,
    IsActive,
    CreatedAt 
FROM Users 
WHERE Email = 'admin@nexus.com';

SELECT '✅ Usuário criado com sucesso!' as Status;
SELECT 'Email: admin@nexus.com' as Credenciais;
SELECT 'Senha: Admin@123' as '';
