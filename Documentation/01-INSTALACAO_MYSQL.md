# Instalação e Configuração do MySQL - Logistics API

Este documento descreve o processo completo de instalação e configuração segura do MySQL para o projeto Logistics API.

## 📋 Índice

1. [Instalação do MySQL](#1-instalação-do-mysql)
2. [Configuração de Segurança](#2-configuração-de-segurança)
3. [Criação do Banco de Dados](#3-criação-do-banco-de-dados)
4. [Configuração do Usuário da Aplicação](#4-configuração-do-usuário-da-aplicação)
5. [Configuração da Connection String](#5-configuração-da-connection-string)
6. [Migrations do Entity Framework](#6-migrations-do-entity-framework)
7. [Verificação da Instalação](#7-verificação-da-instalação)
8. [Troubleshooting](#8-troubleshooting)

---

## 1. Instalação do MySQL

### 1.1 Ubuntu/Debian

```bash
# Atualizar repositórios
sudo apt update

# Instalar MySQL Server
sudo apt install mysql-server -y

# Verificar status do serviço
sudo systemctl status mysql

# Iniciar MySQL (se não estiver rodando)
sudo systemctl start mysql

# Habilitar inicialização automática
sudo systemctl enable mysql
```

### 1.2 Verificar Versão

```bash
mysql --version
# Deve mostrar: mysql  Ver 8.0.x
```

---

## 2. Configuração de Segurança

### 2.1 Executar Script de Segurança

```bash
sudo mysql_secure_installation
```

Responda as perguntas:

```
1. VALIDATE PASSWORD COMPONENT? → Y (Sim)
2. Password validation policy → 1 (MEDIUM)
3. Set root password → Y (Sim)
   Digite: password
   Confirme: password
4. Remove anonymous users → Y (Sim)
5. Disallow root login remotely → Y (Sim)
6. Remove test database → Y (Sim)
7. Reload privilege tables → Y (Sim)
```

### 2.2 Testar Acesso Root

```bash
sudo mysql -u root -p
# Digite a senha: password
```

Se entrou com sucesso, você verá:
```
mysql>
```

---

## 3. Criação do Banco de Dados

### 3.1 Acessar MySQL como Root

```bash
sudo mysql -u root -p
# Senha: password
```

### 3.2 Criar o Banco de Dados

```sql
-- Criar banco com charset UTF-8
CREATE DATABASE logistics_db 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

-- Verificar se foi criado
SHOW DATABASES;

-- Deve aparecer 'logistics_db' na lista
```

---

## 4. Configuração do Usuário da Aplicação

### 4.1 Criar Usuário Dedicado (Recomendado)

Para maior segurança, crie um usuário específico para a aplicação:

```sql
-- Criar usuário 'logistics_user' com senha 'password'
CREATE USER 'logistics_user'@'localhost' IDENTIFIED BY 'password';

-- Conceder privilégios apenas no banco logistics_db
GRANT ALL PRIVILEGES ON logistics_db.* TO 'logistics_user'@'localhost';

-- Aplicar as mudanças
FLUSH PRIVILEGES;

-- Verificar usuário criado
SELECT User, Host FROM mysql.user WHERE User = 'logistics_user';
```

### 4.2 Testar Acesso do Novo Usuário

```bash
# Sair do MySQL
exit;

# Tentar conectar com o novo usuário
mysql -u logistics_user -p
# Senha: password

# Dentro do MySQL, testar acesso ao banco
USE logistics_db;
SHOW TABLES;

# Deve funcionar sem erros (tabelas vazias por enquanto)
exit;
```

---

## 5. Configuração da Connection String

### 5.1 Opção 1: Usar Usuário Root (Desenvolvimento Local)

Edite o arquivo: `/home/wagnerfb/Projetos/ERP/API/src/Logistics.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=logistics_db;User=root;Password=password;CharSet=utf8mb4;AllowUserVariables=true;"
  },
  "JwtSettings": {
    "Secret": "logistics-super-secret-key-with-at-least-32-characters-2025",
    "Issuer": "LogisticsAPI",
    "Audience": "LogisticsClient",
    "ExpirationHours": "8"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    }
  },
  "AllowedHosts": "*"
}
```

### 5.2 Opção 2: Usar Usuário Dedicado (Recomendado)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=logistics_db;User=logistics_user;Password=password;CharSet=utf8mb4;AllowUserVariables=true;"
  }
}
```

### 5.3 Segurança Adicional com User Secrets (Recomendado para Produção)

Para não expor a senha no código fonte:

```bash
cd /home/wagnerfb/Projetos/ERP/API/src/Logistics.API

# Inicializar User Secrets
dotnet user-secrets init

# Armazenar connection string de forma segura
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=logistics_db;User=logistics_user;Password=password;CharSet=utf8mb4;AllowUserVariables=true;"

# Armazenar JWT Secret
dotnet user-secrets set "JwtSettings:Secret" "logistics-super-secret-key-with-at-least-32-characters-2025"

# Listar secrets configurados
dotnet user-secrets list
```

Com User Secrets, você pode deixar o `appsettings.json` assim:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Configurado via User Secrets"
  },
  "JwtSettings": {
    "Secret": "Configurado via User Secrets",
    "Issuer": "LogisticsAPI",
    "Audience": "LogisticsClient",
    "ExpirationHours": "8"
  }
}
```

---

## 6. Migrations do Entity Framework

### 6.1 Verificar Instalação do EF Tools

```bash
# Verificar se dotnet-ef está instalado
dotnet ef --version

# Se não estiver instalado, instalar globalmente
dotnet tool install --global dotnet-ef

# Atualizar se já estiver instalado
dotnet tool update --global dotnet-ef
```

### 6.2 Criar a Migration Inicial

```bash
cd /home/wagnerfb/Projetos/ERP/API

# Criar migration inicial
dotnet ef migrations add InitialCreate \
  --project src/Logistics.Infrastructure \
  --startup-project src/Logistics.API \
  --output-dir Data/Migrations
```

Saída esperada:
```
Build started...
Build succeeded.
Done. To undo this action, use 'ef migrations remove'
```

### 6.3 Revisar Migration Criada

```bash
# Listar migrations
dotnet ef migrations list \
  --project src/Logistics.Infrastructure \
  --startup-project src/Logistics.API
```

Arquivos criados em `src/Logistics.Infrastructure/Data/Migrations/`:
- `xxxxxxxxxx_InitialCreate.cs` - Migration
- `xxxxxxxxxx_InitialCreate.Designer.cs` - Metadata
- `LogisticsDbContextModelSnapshot.cs` - Snapshot do modelo

### 6.4 Aplicar Migration ao Banco de Dados

```bash
# Aplicar migrations pendentes
dotnet ef database update \
  --project src/Logistics.Infrastructure \
  --startup-project src/Logistics.API
```

Saída esperada:
```
Build started...
Build succeeded.
Applying migration '20250121xxxxxx_InitialCreate'.
Done.
```

### 6.5 Verificar Tabelas Criadas

```bash
# Acessar MySQL
mysql -u logistics_user -p
# Senha: password

# Dentro do MySQL
USE logistics_db;

-- Listar tabelas criadas
SHOW TABLES;
```

Você deve ver:
```
+-------------------------+
| Tables_in_logistics_db  |
+-------------------------+
| Companies               |
| Drivers                 |
| Users                   |
| Vehicles                |
| __EFMigrationsHistory   |
+-------------------------+
```

```sql
-- Ver estrutura de uma tabela
DESCRIBE Users;

-- Ver histórico de migrations
SELECT * FROM __EFMigrationsHistory;

-- Sair
exit;
```

---

## 7. Verificação da Instalação

### 7.1 Script de Verificação Completa

Crie e execute este script bash:

```bash
#!/bin/bash
# Script de verificação da instalação MySQL

echo "=== Verificação da Instalação MySQL - Logistics API ==="
echo ""

# 1. Verificar serviço MySQL
echo "1. Status do serviço MySQL:"
sudo systemctl is-active mysql
echo ""

# 2. Verificar versão
echo "2. Versão do MySQL:"
mysql --version
echo ""

# 3. Verificar se banco existe
echo "3. Verificando banco de dados logistics_db:"
mysql -u logistics_user -ppassword -e "SHOW DATABASES LIKE 'logistics_db';" 2>/dev/null
echo ""

# 4. Verificar tabelas
echo "4. Tabelas criadas:"
mysql -u logistics_user -ppassword logistics_db -e "SHOW TABLES;" 2>/dev/null
echo ""

# 5. Verificar migrations
echo "5. Migrations aplicadas:"
mysql -u logistics_user -ppassword logistics_db -e "SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory;" 2>/dev/null
echo ""

echo "=== Verificação Concluída ==="
```

Salve como `verify_mysql.sh` e execute:

```bash
chmod +x verify_mysql.sh
./verify_mysql.sh
```

### 7.2 Teste de Conexão via .NET

```bash
cd /home/wagnerfb/Projetos/ERP/API

# Testar build
dotnet build

# Se build OK, testar conexão executando a API
dotnet run --project src/Logistics.API
```

Você deve ver logs indicando:
```
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (connection opened successfully)
```

---

## 8. Troubleshooting

### 8.1 Erro: "Access denied for user"

```bash
# Resetar senha do root
sudo mysql

ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY 'password';
FLUSH PRIVILEGES;
exit;
```

### 8.2 Erro: "Can't connect to MySQL server"

```bash
# Verificar se MySQL está rodando
sudo systemctl status mysql

# Se não estiver, iniciar
sudo systemctl start mysql

# Verificar porta
sudo netstat -tlnp | grep mysql
# Deve mostrar porta 3306
```

### 8.3 Erro: "The server requested authentication method unknown"

Isso acontece com drivers antigos. Solução:

```sql
-- Entrar no MySQL
sudo mysql -u root -p

-- Alterar método de autenticação
ALTER USER 'logistics_user'@'localhost' IDENTIFIED WITH mysql_native_password BY 'password';
FLUSH PRIVILEGES;
exit;
```

### 8.4 Erro nas Migrations: "Build failed"

```bash
# Limpar e rebuildar
cd /home/wagnerfb/Projetos/ERP/API
dotnet clean
dotnet restore
dotnet build

# Tentar novamente
dotnet ef database update --project src/Logistics.Infrastructure --startup-project src/Logistics.API
```

### 8.5 Erro: "Table already exists"

Se você já tinha tabelas e quer recomeçar:

```bash
# Remover banco de dados
mysql -u root -p
DROP DATABASE logistics_db;
CREATE DATABASE logistics_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
exit;

# Remover migrations antigas
cd /home/wagnerfb/Projetos/ERP/API
rm -rf src/Logistics.Infrastructure/Data/Migrations

# Criar nova migration
dotnet ef migrations add InitialCreate --project src/Logistics.Infrastructure --startup-project src/Logistics.API

# Aplicar
dotnet ef database update --project src/Logistics.Infrastructure --startup-project src/Logistics.API
```

### 8.6 Permissões Insuficientes

```sql
-- Dar todas as permissões ao usuário
GRANT ALL PRIVILEGES ON logistics_db.* TO 'logistics_user'@'localhost';
GRANT CREATE, ALTER, DROP, INSERT, UPDATE, DELETE, SELECT, REFERENCES ON logistics_db.* TO 'logistics_user'@'localhost';
FLUSH PRIVILEGES;
```

---

## 9. Comandos Úteis

### 9.1 MySQL

```bash
# Iniciar serviço
sudo systemctl start mysql

# Parar serviço
sudo systemctl stop mysql

# Reiniciar serviço
sudo systemctl restart mysql

# Ver logs
sudo tail -f /var/log/mysql/error.log

# Backup do banco
mysqldump -u logistics_user -p logistics_db > backup_$(date +%Y%m%d).sql

# Restaurar backup
mysql -u logistics_user -p logistics_db < backup_20250121.sql
```

### 9.2 Entity Framework

```bash
# Listar migrations
dotnet ef migrations list --project src/Logistics.Infrastructure --startup-project src/Logistics.API

# Remover última migration
dotnet ef migrations remove --project src/Logistics.Infrastructure --startup-project src/Logistics.API

# Gerar script SQL da migration
dotnet ef migrations script --project src/Logistics.Infrastructure --startup-project src/Logistics.API --output migration.sql

# Reverter para migration específica
dotnet ef database update NomeDaMigration --project src/Logistics.Infrastructure --startup-project src/Logistics.API

# Resetar banco (voltar ao início)
dotnet ef database update 0 --project src/Logistics.Infrastructure --startup-project src/Logistics.API
```

---

## 10. Segurança em Produção

### 10.1 Checklist de Segurança

- [ ] Usar usuário dedicado (não root)
- [ ] Senha forte (mínimo 16 caracteres)
- [ ] Usar User Secrets ou variáveis de ambiente
- [ ] Firewall configurado (apenas localhost ou IPs específicos)
- [ ] SSL/TLS habilitado para conexões remotas
- [ ] Backups automáticos configurados
- [ ] Logs de auditoria habilitados
- [ ] Desabilitar acesso remoto root
- [ ] Limitar privilégios do usuário da aplicação

### 10.2 Configuração de Firewall

```bash
# Permitir apenas localhost
sudo ufw allow from 127.0.0.1 to any port 3306

# Ou permitir IP específico
sudo ufw allow from 192.168.1.100 to any port 3306
```

### 10.3 Habilitar SSL

```bash
# Verificar se SSL está habilitado
mysql -u root -p -e "SHOW VARIABLES LIKE '%ssl%';"

# Configurar em /etc/mysql/mysql.conf.d/mysqld.cnf
[mysqld]
require_secure_transport=ON
ssl-ca=/etc/mysql/ssl/ca.pem
ssl-cert=/etc/mysql/ssl/server-cert.pem
ssl-key=/etc/mysql/ssl/server-key.pem
```

---

## 11. Resumo dos Passos

```bash
# 1. Instalar MySQL
sudo apt install mysql-server -y

# 2. Configurar segurança
sudo mysql_secure_installation

# 3. Criar banco e usuário
sudo mysql -u root -p
CREATE DATABASE logistics_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER 'logistics_user'@'localhost' IDENTIFIED BY 'password';
GRANT ALL PRIVILEGES ON logistics_db.* TO 'logistics_user'@'localhost';
FLUSH PRIVILEGES;
exit;

# 4. Configurar appsettings.json (connection string)

# 5. Criar e aplicar migrations
cd /home/wagnerfb/Projetos/ERP/API
dotnet ef migrations add InitialCreate --project src/Logistics.Infrastructure --startup-project src/Logistics.API
dotnet ef database update --project src/Logistics.Infrastructure --startup-project src/Logistics.API

# 6. Executar aplicação
dotnet run --project src/Logistics.API
```

---

**Documento criado em**: 2025-11-21  
**Versão**: 1.0  
**Status**: Pronto para uso
