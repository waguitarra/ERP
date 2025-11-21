# Logistics API - Sistema de Logística

API RESTful desenvolvida em .NET 8.0 com arquitetura DDD simplificada para gerenciamento de operações de logística com suporte multi-tenant.

## 🏗️ Arquitetura

O projeto segue os princípios de Domain-Driven Design (DDD) com 4 camadas:

- **Domain**: Entidades, Value Objects, Interfaces de Repositório
- **Application**: Services, DTOs, Validators, Use Cases
- **Infrastructure**: DbContext, Repositórios, Integrações externas
- **API**: Controllers, Middlewares, Configurações

Para mais detalhes, consulte: [Documentation/ARQUITETURA.md](../Documentation/ARQUITETURA.md)

## 🚀 Tecnologias

- .NET 8.0
- Entity Framework Core 8.0
- MySQL 8.0+
- JWT Authentication
- Swagger/OpenAPI
- Serilog
- BCrypt

## 📋 Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- MySQL 8.0+ instalado e rodando
- IDE: Visual Studio 2022, VS Code ou Rider

## ⚙️ Configuração

### 1. Clone o repositório

```bash
cd /home/wagnerfb/Projetos/ERP/API
```

### 2. Configure o banco de dados MySQL

Crie um banco de dados MySQL:

```sql
CREATE DATABASE logistics_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

### 3. Configure a Connection String

Edite o arquivo `src/Logistics.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=logistics_db;User=root;Password=SUA_SENHA_AQUI;CharSet=utf8mb4;"
  }
}
```

### 4. Configure o JWT Secret

No mesmo arquivo `appsettings.json`, configure uma chave secreta forte:

```json
{
  "JwtSettings": {
    "Secret": "sua-chave-secreta-com-no-minimo-32-caracteres-aqui"
  }
}
```

## 🔨 Instalação

### 1. Restaurar pacotes NuGet

```bash
dotnet restore
```

### 2. Criar as migrações do banco de dados

```bash
dotnet ef migrations add InitialCreate --project src/Logistics.Infrastructure --startup-project src/Logistics.API
```

### 3. Aplicar as migrações

```bash
dotnet ef database update --project src/Logistics.Infrastructure --startup-project src/Logistics.API
```

## ▶️ Executando a Aplicação

### Modo Development

```bash
dotnet run --project src/Logistics.API
```

A API estará disponível em:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger: https://localhost:5001 (raiz do projeto)

### Build para Produção

```bash
dotnet build --configuration Release
dotnet publish --configuration Release --output ./publish
```

## 📖 Uso da API

### 1. Registrar Administrador Master (Primeira vez)

```bash
POST /api/auth/register-admin
Content-Type: application/json

{
  "name": "Admin Master",
  "email": "admin@logistics.com",
  "password": "Admin@123",
  "confirmPassword": "Admin@123"
}
```

### 2. Login

```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@logistics.com",
  "password": "Admin@123"
}
```

Resposta:
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "email": "admin@logistics.com",
    "name": "Admin Master",
    "role": "Admin",
    "companyId": null
  }
}
```

### 3. Criar Empresa (Admin Only)

```bash
POST /api/companies
Authorization: Bearer {seu-token}
Content-Type: application/json

{
  "name": "Transportadora XYZ",
  "document": "12345678901234"
}
```

### 4. Listar Empresas

```bash
GET /api/companies
Authorization: Bearer {seu-token}
```

## 🔐 Autenticação e Autorização

### Roles disponíveis:

- **Admin**: Administrador master com acesso total ao sistema
- **CompanyAdmin**: Administrador de uma empresa específica
- **CompanyUser**: Usuário operacional de uma empresa

### Policies:

- **AdminOnly**: Apenas Admin master
- **CompanyAccess**: Todos os usuários autenticados
- **CompanyAdminOnly**: Admin master e admins de empresa

## 🗂️ Estrutura do Projeto

```
API/
├── src/
│   ├── Logistics.Domain/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── Interfaces/
│   ├── Logistics.Application/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   └── Services/
│   ├── Logistics.Infrastructure/
│   │   ├── Data/
│   │   │   ├── Configurations/
│   │   │   └── LogisticsDbContext.cs
│   │   └── Repositories/
│   └── Logistics.API/
│       ├── Controllers/
│       ├── Properties/
│       ├── Program.cs
│       └── appsettings.json
├── tests/
│   └── Logistics.Tests/
├── Logistics.sln
└── README.md
```

## 📊 Banco de Dados

### Entidades principais:

- **Companies**: Empresas cadastradas
- **Users**: Usuários do sistema (Master Admin e usuários de empresas)
- **Vehicles**: Veículos de cada empresa
- **Drivers**: Motoristas de cada empresa

### Diagrama de relacionamentos:

```
Company (1) -----> (*) Users
Company (1) -----> (*) Vehicles
Company (1) -----> (*) Drivers
```

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Com cobertura
dotnet test /p:CollectCoverage=true
```

## 📝 Logs

Os logs são salvos em:
- Console (desenvolvimento)
- Arquivo: `logs/log-YYYYMMDD.txt`

## 🔧 Comandos Úteis

### Entity Framework

```bash
# Adicionar nova migration
dotnet ef migrations add NomeDaMigration --project src/Logistics.Infrastructure --startup-project src/Logistics.API

# Remover última migration
dotnet ef migrations remove --project src/Logistics.Infrastructure --startup-project src/Logistics.API

# Listar migrations
dotnet ef migrations list --project src/Logistics.Infrastructure --startup-project src/Logistics.API

# Gerar script SQL
dotnet ef migrations script --project src/Logistics.Infrastructure --startup-project src/Logistics.API
```

### Build e Clean

```bash
# Limpar build
dotnet clean

# Rebuild
dotnet build --no-incremental

# Restaurar + Build
dotnet restore && dotnet build
```

## 🐛 Troubleshooting

### Erro de conexão com MySQL

Verifique se:
1. MySQL está rodando: `sudo systemctl status mysql`
2. As credenciais estão corretas no `appsettings.json`
3. O banco de dados existe: `SHOW DATABASES;`

### Erro de JWT

Verifique se:
1. O `Secret` tem pelo menos 32 caracteres
2. O token está sendo enviado no header: `Authorization: Bearer {token}`

### Erro nas Migrations

```bash
# Resetar banco de dados
dotnet ef database drop --project src/Logistics.Infrastructure --startup-project src/Logistics.API

# Recriar
dotnet ef database update --project src/Logistics.Infrastructure --startup-project src/Logistics.API
```

## 📚 Documentação da API

Acesse a documentação interativa via Swagger:
- **URL**: https://localhost:5001
- Todos os endpoints estão documentados com exemplos

## 🛣️ Roadmap

- [x] Autenticação JWT
- [x] Multi-tenancy
- [x] CRUD de Empresas
- [ ] CRUD de Usuários
- [ ] CRUD de Veículos
- [ ] CRUD de Motoristas
- [ ] Gestão de Rotas
- [ ] Gestão de Entregas
- [ ] Dashboard e Relatórios

## 📄 Licença

Este projeto é privado e proprietário.

## 👥 Contato

Para dúvidas ou suporte, entre em contato com a equipe de desenvolvimento.

---

**Versão**: 1.0.0  
**Última atualização**: 2025-11-21
