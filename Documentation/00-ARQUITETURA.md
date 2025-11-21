# Arquitetura do Sistema de Logística - API .NET Core

## 1. Visão Geral do Projeto

Sistema de gerenciamento de logística desenvolvido em **.NET Core** utilizando arquitetura **DDD (Domain-Driven Design) simplificada**, com foco em manutenibilidade e separação de responsabilidades.

### Características Principais
- **Multi-tenant**: Segmentação por empresa
- **Autenticação e Autorização**: JWT com sistema de Roles
- **Banco de Dados**: MySQL com Entity Framework Core
- **Arquitetura**: DDD simplificada (4 camadas)

---

## 2. Estrutura de Usuários e Permissões

### 2.1 Hierarquia de Usuários

#### Usuário Master (Admin)
- **Role**: `Admin`
- **Responsabilidades**:
  - Criar e gerenciar empresas
  - Criar usuários admin para empresas
  - Acesso total ao sistema
  - Não está vinculado a nenhuma empresa específica

#### Usuário de Empresa
- **Role**: `CompanyAdmin` | `CompanyUser`
- **Características**:
  - Vinculado a uma empresa específica (CompanyId)
  - Acesso limitado aos dados da sua empresa
  - `CompanyAdmin`: Pode gerenciar usuários da empresa
  - `CompanyUser`: Acesso operacional

### 2.2 Isolamento de Dados (Multi-tenancy)
- Todas as entidades de negócio possuem `CompanyId`
- Filtros automáticos garantem que usuários só acessem dados da sua empresa
- Admin master tem acesso cross-company

---

## 3. Arquitetura DDD Simplificada

### 3.1 Camadas do Sistema

```
API/
├── src/
│   ├── Logistics.Domain/          # Camada de Domínio
│   ├── Logistics.Application/     # Camada de Aplicação
│   ├── Logistics.Infrastructure/  # Camada de Infraestrutura
│   └── Logistics.API/            # Camada de Apresentação
└── tests/
    └── Logistics.Tests/
```

### 3.2 Responsabilidades das Camadas

#### **Domain** (Núcleo do Negócio)
- **Entidades**: Classes que representam objetos de negócio
- **Value Objects**: Objetos imutáveis sem identidade
- **Interfaces de Repositório**: Contratos para acesso a dados
- **Exceptions de Domínio**: Regras de negócio violadas
- **Enums**: Tipos enumerados do domínio

**Princípio**: Zero dependências externas, apenas lógica de negócio pura.

#### **Application** (Casos de Uso)
- **Services**: Orquestração de casos de uso
- **DTOs**: Objetos de transferência de dados (Request/Response)
- **Interfaces**: Contratos de serviços
- **Validators**: Validação de entrada (FluentValidation)
- **Mappings**: AutoMapper profiles

**Princípio**: Coordena o fluxo de dados entre Domain e Infrastructure.

#### **Infrastructure** (Implementação Técnica)
- **Data Context**: DbContext do Entity Framework
- **Repositories**: Implementação dos repositórios
- **Migrations**: Migrações do banco de dados
- **External Services**: Integrações externas
- **Identity**: Implementação de autenticação

**Princípio**: Detalhes técnicos e frameworks. Depende de Domain.

#### **API** (Camada de Apresentação)
- **Controllers**: Endpoints REST
- **Middlewares**: Autenticação, logging, exceções
- **Filters**: Filtros de multi-tenancy
- **Configuration**: Startup, DI, appsettings

**Princípio**: Interface HTTP. Depende de Application e Infrastructure.

---

## 4. Modelo de Dados

### 4.1 Entidades Principais

#### **Company** (Empresa)
```
- Id (Guid)
- Name (string)
- Document (CNPJ - string)
- IsActive (bool)
- CreatedAt (DateTime)
- UpdatedAt (DateTime?)
```

#### **User** (Usuário)
```
- Id (Guid)
- CompanyId (Guid?) - null para Admin Master
- Name (string)
- Email (string) - único
- PasswordHash (string)
- Role (UserRole enum)
- IsActive (bool)
- CreatedAt (DateTime)
- UpdatedAt (DateTime?)
- LastLoginAt (DateTime?)
```

#### **UserRole** (Enum)
```
- Admin = 0        # Master admin
- CompanyAdmin = 1 # Admin da empresa
- CompanyUser = 2  # Usuário operacional
```

### 4.2 Entidades de Logística (Fase Inicial)

#### **Vehicle** (Veículo)
```
- Id (Guid)
- CompanyId (Guid)
- LicensePlate (string)
- Model (string)
- Year (int)
- Status (VehicleStatus enum)
- CreatedAt (DateTime)
```

#### **Driver** (Motorista)
```
- Id (Guid)
- CompanyId (Guid)
- Name (string)
- LicenseNumber (string)
- Phone (string)
- IsActive (bool)
- CreatedAt (DateTime)
```

---

## 5. Segurança

### 5.1 Autenticação
- **JWT (JSON Web Token)**
- Token contém: UserId, Email, Role, CompanyId
- Tempo de expiração: 8 horas
- Refresh token: não implementado na fase 1

### 5.2 Autorização
- **Policy-based authorization**
- Policies:
  - `AdminOnly`: Apenas Admin master
  - `CompanyAccess`: Usuários de empresa
  - `CompanyAdminOnly`: Admin de empresa

### 5.3 Multi-tenancy Filter
- Action filter automático que:
  - Injeta CompanyId nas queries
  - Valida acesso aos recursos
  - Permite bypass para Admin master

---

## 6. Configuração do Banco de Dados

### 6.1 MySQL
- **Versão**: MySQL 8.0+
- **Connection String**: Configurada via appsettings.json
- **Charset**: utf8mb4
- **Collation**: utf8mb4_unicode_ci

### 6.2 Entity Framework Core
- **Code-First Approach**
- **Migrations**: Gerenciamento de schema
- **Lazy Loading**: Desabilitado (usar Include explícito)
- **Tracking**: Configurado por query

---

## 7. Padrões e Boas Práticas

### 7.1 Naming Conventions
- **Classes**: PascalCase
- **Métodos**: PascalCase
- **Propriedades**: PascalCase
- **Variáveis**: camelCase
- **Interfaces**: I + PascalCase

### 7.2 Response Patterns
```json
{
  "success": true,
  "data": { },
  "message": "Operation completed successfully",
  "errors": []
}
```

### 7.3 Exception Handling
- Middleware global para captura de exceções
- Logging estruturado
- Mensagens de erro amigáveis
- Códigos HTTP apropriados

### 7.4 Validação
- FluentValidation para DTOs
- Validações de domínio nas entidades
- Validação em camadas (API → Application → Domain)

---

## 8. Dependências do Projeto

### 8.1 Pacotes NuGet Principais

#### Domain
- Nenhuma dependência externa

#### Application
- `FluentValidation`
- `AutoMapper`

#### Infrastructure
- `Pomelo.EntityFrameworkCore.MySql`
- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.Design`
- `BCrypt.Net-Next` (hash de senha)

#### API
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Swashbuckle.AspNetCore` (Swagger)
- `Serilog` (logging)

---

## 9. Endpoints API (Fase 1)

### 9.1 Autenticação
- `POST /api/auth/login` - Login
- `POST /api/auth/register-admin` - Registrar admin master (público, primeira execução)

### 9.2 Companies (Admin Only)
- `POST /api/companies` - Criar empresa
- `GET /api/companies` - Listar empresas
- `GET /api/companies/{id}` - Obter empresa
- `PUT /api/companies/{id}` - Atualizar empresa
- `DELETE /api/companies/{id}` - Desativar empresa

### 9.3 Users
- `POST /api/users` - Criar usuário
- `GET /api/users` - Listar usuários (filtrado por empresa)
- `GET /api/users/{id}` - Obter usuário
- `PUT /api/users/{id}` - Atualizar usuário
- `DELETE /api/users/{id}` - Desativar usuário

### 9.4 Vehicles
- `POST /api/vehicles` - Criar veículo
- `GET /api/vehicles` - Listar veículos
- `GET /api/vehicles/{id}` - Obter veículo
- `PUT /api/vehicles/{id}` - Atualizar veículo
- `DELETE /api/vehicles/{id}` - Deletar veículo

### 9.5 Drivers
- `POST /api/drivers` - Criar motorista
- `GET /api/drivers` - Listar motoristas
- `GET /api/drivers/{id}` - Obter motorista
- `PUT /api/drivers/{id}` - Atualizar motorista
- `DELETE /api/drivers/{id}` - Desativar motorista

---

## 10. Roadmap de Implementação

### Fase 1: Fundação ✓ (Em Progresso)
- [ ] Criar estrutura de projetos
- [ ] Configurar Domain: Entidades base (Company, User)
- [ ] Configurar Infrastructure: DbContext, MySQL
- [ ] Implementar autenticação JWT
- [ ] Criar endpoints de Auth e Companies
- [ ] Implementar multi-tenancy filter

### Fase 2: Logística Base
- [ ] Entidades: Vehicle, Driver
- [ ] Endpoints CRUD para Vehicle e Driver
- [ ] Validações de negócio

### Fase 3: Operações de Logística
- [ ] Entidades: Route, Delivery, Order
- [ ] Gestão de entregas
- [ ] Status tracking

### Fase 4: Relatórios e Analytics
- [ ] Dashboard
- [ ] Relatórios de entregas
- [ ] Métricas de performance

---

## 11. Variáveis de Ambiente

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=logistics_db;User=root;Password=your_password;"
  },
  "JwtSettings": {
    "Secret": "your-super-secret-key-min-32-chars",
    "ExpirationHours": 8,
    "Issuer": "LogisticsAPI",
    "Audience": "LogisticsClient"
  }
}
```

---

## 12. Como Executar

### 12.1 Pré-requisitos
- .NET 8.0 SDK
- MySQL 8.0+
- IDE: Visual Studio 2022 ou VS Code

### 12.2 Setup Inicial
```bash
# 1. Restaurar pacotes
dotnet restore

# 2. Configurar connection string em appsettings.json

# 3. Criar banco de dados
dotnet ef database update --project src/Logistics.Infrastructure --startup-project src/Logistics.API

# 4. Executar API
dotnet run --project src/Logistics.API
```

### 12.3 Primeiro Acesso
1. Acessar Swagger: `https://localhost:5001/swagger`
2. Registrar admin master via endpoint `POST /api/auth/register-admin`
3. Fazer login e obter token JWT
4. Usar token no header: `Authorization: Bearer {token}`

---

## 13. Próximos Passos

- Implementar refresh token
- Adicionar logs estruturados
- Implementar cache (Redis)
- Adicionar testes unitários e integração
- Dockerização do projeto
- CI/CD pipeline

---

**Documento criado em**: 2025-11-21  
**Versão**: 1.0  
**Status**: Em desenvolvimento
