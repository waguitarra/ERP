-- ============================================
-- Script de inicialização do banco de dados
-- ============================================

-- Garantir que o usuário tem permissões
GRANT ALL PRIVILEGES ON logistics_db.* TO 'logistics_user'@'%';
FLUSH PRIVILEGES;

-- O EF Core vai criar as tabelas automaticamente
-- Este arquivo é para configurações iniciais se necessário
