-- SCRIPT PARA INSERIR DADOS DE TESTE PURCHASE ORDER
-- Executar após migrations aplicadas

USE logistics_db;

-- 1. Inserir Company de teste (se não existir)
INSERT IGNORE INTO Companies (Id, Name, Email, Phone, CreatedAt)
VALUES ('3fa85f64-5717-4562-b3fc-2c963f66afa6', 'Empresa Teste', 'teste@empresa.com', '11999999999', UTC_TIMESTAMP());

-- 2. Inserir Supplier de teste
INSERT IGNORE INTO Suppliers (Id, CompanyId, Name, Email, Phone, Address, CreatedAt)
VALUES ('3fa85f64-5717-4562-b3fc-2c963f66afa7', '3fa85f64-5717-4562-b3fc-2c963f66afa6', 'Dell Inc.', 'supplier@dell.com', '11888888888', 'USA', UTC_TIMESTAMP());

-- 3. Inserir Product de teste
INSERT IGNORE INTO Products (Id, CompanyId, SKU, Name, Description, UnitPrice, StockQuantity, CreatedAt)
VALUES ('3fa85f64-5717-4562-b3fc-2c963f66afa8', '3fa85f64-5717-4562-b3fc-2c963f66afa6', 'NOTEBOOK-001', 'Dell Latitude 5430', 'Notebook Dell i7 16GB RAM', 2500.00, 0, UTC_TIMESTAMP());

-- 4. Inserir User de teste (admin)
INSERT IGNORE INTO Users (Id, CompanyId, Name, Email, PasswordHash, Role, IsActive, CreatedAt)
VALUES ('3fa85f64-5717-4562-b3fc-2c963f66afa9', '3fa85f64-5717-4562-b3fc-2c963f66afa6', 'Admin Teste', 'admin@test.com', 
'$2a$11$1234567890123456789012345678901234567890123456', -- senha: Admin@123
'Admin', 1, UTC_TIMESTAMP());

-- 5. Inserir Warehouse de teste
INSERT IGNORE INTO Warehouses (Id, CompanyId, Name, Code, Address, City, State, ZipCode, Country, IsActive, CreatedAt)
VALUES ('3fa85f64-5717-4562-b3fc-2c963f66afaa', '3fa85f64-5717-4562-b3fc-2c963f66afa6', 'Armazém Principal', 'WH-01', 'Rua Teste 123', 'São Paulo', 'SP', '01000-000', 'Brasil', 1, UTC_TIMESTAMP());

SELECT '✅ Dados de teste inseridos com sucesso!' as Status;
SELECT 'Company ID: 3fa85f64-5717-4562-b3fc-2c963f66afa6' as Info;
SELECT 'Supplier ID: 3fa85f64-5717-4562-b3fc-2c963f66afa7' as Info;
SELECT 'Product ID: 3fa85f64-5717-4562-b3fc-2c963f66afa8' as Info;
SELECT 'User ID: 3fa85f64-5717-4562-b3fc-2c963f66afa9' as Info;
SELECT 'Warehouse ID: 3fa85f64-5717-4562-b3fc-2c963f66afaa' as Info;
