using System;
using BCrypt.Net;

// Script para gerar hash BCrypt e criar usuário de teste
// Compilar e executar: dotnet script CreateTestUser.cs

var password = "Admin@123";
var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, 11);

Console.WriteLine("=== USUÁRIO DE TESTE ===");
Console.WriteLine($"Email: admin@nexus.com");
Console.WriteLine($"Senha: {password}");
Console.WriteLine($"Hash BCrypt: {passwordHash}");
Console.WriteLine();
Console.WriteLine("=== SQL PARA INSERIR NO BANCO ===");
Console.WriteLine($@"
USE logistics_db;

DELETE FROM Users WHERE Email = 'admin@nexus.com';

INSERT INTO Users (Id, CompanyId, Name, Email, PasswordHash, Role, IsActive, CreatedAt, UpdatedAt, LastLoginAt)
VALUES (
    '{Guid.NewGuid()}',
    NULL,
    'Administrador',
    'admin@nexus.com',
    '{passwordHash}',
    0,
    1,
    UTC_TIMESTAMP(),
    NULL,
    NULL
);

SELECT * FROM Users WHERE Email = 'admin@nexus.com';
");
