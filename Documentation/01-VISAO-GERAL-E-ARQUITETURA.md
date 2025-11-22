# DOCUMENTAÇÃO TÉCNICA COMPLETA - SISTEMA WMS
## Volume 1: Visão Geral e Arquitetura

**Versão**: 3.0  
**Data**: 2025-11-22  
**Autores**: Equipe de Desenvolvimento  
**Status**: ✅ Documentação Técnica Completa

---

## 📋 ÍNDICE GERAL DA DOCUMENTAÇÃO

Esta documentação está dividida em volumes para facilitar a navegação:

1. **Volume 1**: Visão Geral e Arquitetura (ESTE DOCUMENTO)
2. **Volume 2**: Modelo de Dados e Entidades
3. **Volume 3**: API Endpoints e Controllers
4. **Volume 4**: Serviços e Lógica de Negócio
5. **Volume 5**: Fluxos de Processo WMS
6. **Volume 6**: Autenticação, Segurança e Deployment
7. **Volume 7**: Guia de Implementação para Programadores

---

## 🎯 1. VISÃO GERAL DO SISTEMA

### 1.1 O que é este Sistema WMS?

O **Sistema WMS (Warehouse Management System)** é uma aplicação completa para gestão de armazéns e logística desenvolvida em **.NET 6** com arquitetura **DDD (Domain-Driven Design)** e **Multi-Tenancy**.

**Características Principais**:
- ✅ Arquitetura em camadas (API, Application, Domain, Infrastructure)
- ✅ Multi-tenancy por empresa (CompanyId)
- ✅ Autenticação JWT com roles (Admin, CompanyAdmin, CompanyUser)
- ✅ Banco de dados MySQL/MariaDB com Entity Framework Core
- ✅ RESTful API com Swagger/OpenAPI
- ✅ Logging estruturado com Serilog
- ✅ Repository Pattern + Unit of Work
- ✅ DTOs para comunicação
- ✅ Validação de negócio nas entidades

### 1.2 Funcionalidades Principais

#### **Módulo de Gestão**
- Empresas (Multi-tenant)
- Usuários e Permissões (3 níveis de acesso)
- Armazéns (Warehouses)
- Zonas de Armazém (Receiving, Storage, Shipping, etc.)
- Localizações de Armazenamento (Endereços)

#### **Módulo de Cadastros**
- Produtos (com SKU, código de barras, dimensões)
- Clientes
- Fornecedores
- Veículos
- Motoristas
- Portas de Docagem (Dock Doors)

#### **Módulo WMS - Recebimento (Inbound)**
- Pedidos de Compra (Orders - Inbound)
- Remessas de Entrada (Inbound Shipments)
- Recebimentos (Receipts)
- Tarefas de Endereçamento (Putaway Tasks)
- Agendamento de Veículos (Vehicle Appointments)

#### **Módulo WMS - Expedição (Outbound)**
- Pedidos de Venda (Orders - Outbound)
- Ondas de Separação (Picking Waves)
- Tarefas de Separação (Picking Tasks)
- Tarefas de Embalagem (Packing Tasks)
- Pacotes (Packages)
- Remessas de Saída (Outbound Shipments)

#### **Módulo de Inventário**
- Estoque em Tempo Real (Inventory)
- Movimentações de Estoque (Stock Movements)
- Lotes (Lot Tracking)
- Números de Série (Serial Number Tracking)
- Contagem Cíclica (Cycle Counts)

---

## 🏗️ 2. ARQUITETURA DO SISTEMA

### 2.1 Diagrama de Arquitetura em Camadas

```
┌─────────────────────────────────────────────────────────────────┐
│                      CAMADA DE APRESENTAÇÃO                      │
│                      (Logistics.API)                             │
├─────────────────────────────────────────────────────────────────┤
│  • Controllers (26 controllers)                                  │
│  • Autenticação JWT                                              │
│  • Autorização baseada em Roles                                  │
│  • Validação de Request                                          │
│  • Documentação Swagger                                          │
│  • Middleware de Logging                                         │
└──────────────────────┬──────────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────────┐
│                    CAMADA DE APLICAÇÃO                           │
│                  (Logistics.Application)                         │
├─────────────────────────────────────────────────────────────────┤
│  • Services (26 serviços)                                        │
│  • DTOs (Data Transfer Objects)                                  │
│  • Interfaces de Serviços                                        │
│  • Mapeamento de Entidades → DTOs                                │
│  • Orquestração de Regras de Negócio                             │
│  • Validações de Aplicação                                       │
└──────────────────────┬──────────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────────┐
│                      CAMADA DE DOMÍNIO                           │
│                    (Logistics.Domain)                            │
├─────────────────────────────────────────────────────────────────┤
│  • Entidades (29 entidades)                                      │
│  • Enums (27 enumerações)                                        │
│  • Interfaces de Repositórios                                    │
│  • Regras de Negócio                                             │
│  • Validações de Domínio                                         │
│  • Eventos de Domínio (se aplicável)                             │
└──────────────────────┬──────────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────────┐
│                  CAMADA DE INFRAESTRUTURA                        │
│                 (Logistics.Infrastructure)                       │
├─────────────────────────────────────────────────────────────────┤
│  • DbContext (Entity Framework Core)                             │
│  • Repositórios (26 repositórios)                                │
│  • Unit of Work                                                  │
│  • Configurações de Mapeamento                                   │
│  • Migrações de Banco de Dados                                   │
│  • Logging                                                       │
└──────────────────────┬──────────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────────┐
│                      BANCO DE DADOS                              │
│                    MySQL / MariaDB                               │
├─────────────────────────────────────────────────────────────────┤
│  • Tabelas Normalizadas                                          │
│  • Índices Otimizados                                            │
│  • Constraints de Integridade                                    │
│  • Triggers (se necessário)                                      │
└─────────────────────────────────────────────────────────────────┘
```

### 2.2 Stack Tecnológica

#### **Backend**
- **.NET 6.0** - Framework principal
- **ASP.NET Core Web API** - API RESTful
- **Entity Framework Core 7.x** - ORM
- **MySQL/MariaDB** - Banco de dados
- **BCrypt.Net** - Hash de senhas
- **JWT (JSON Web Tokens)** - Autenticação
- **Serilog** - Logging estruturado
- **Swagger/OpenAPI** - Documentação da API

#### **Padrões e Práticas**
- **DDD (Domain-Driven Design)** - Arquitetura
- **Repository Pattern** - Acesso a dados
- **Unit of Work** - Gerenciamento de transações
- **DTO Pattern** - Transferência de dados
- **Dependency Injection** - Inversão de controle
- **SOLID Principles** - Princípios de design

### 2.3 Estrutura de Projetos

```
API/
├── src/
│   ├── Logistics.API/                    # Camada de Apresentação
│   │   ├── Controllers/                  # 26 Controllers
│   │   ├── Program.cs                    # Configuração da aplicação
│   │   ├── appsettings.json              # Configurações
│   │   └── GlobalUsings.cs               # Usings globais
│   │
│   ├── Logistics.Application/            # Camada de Aplicação
│   │   ├── DTOs/                         # Data Transfer Objects
│   │   │   ├── Auth/
│   │   │   ├── User/
│   │   │   ├── Company/
│   │   │   ├── Product/
│   │   │   ├── Order/
│   │   │   ├── Inventory/
│   │   │   └── ... (27 pastas de DTOs)
│   │   ├── Services/                     # 26 Serviços
│   │   └── Interfaces/                   # Interfaces dos serviços
│   │
│   ├── Logistics.Domain/                 # Camada de Domínio
│   │   ├── Entities/                     # 29 Entidades
│   │   ├── Enums/                        # 27 Enumerações
│   │   └── Interfaces/                   # Interfaces de repositórios
│   │
│   └── Logistics.Infrastructure/         # Camada de Infraestrutura
│       ├── Data/
│       │   └── LogisticsDbContext.cs     # DbContext
│       ├── Repositories/                 # 26 Repositórios
│       └── Migrations/                   # Migrações EF Core
│
└── tests/
    └── Logistics.Tests/                  # Testes unitários
```

---

## 🔐 3. SEGURANÇA E AUTENTICAÇÃO

### 3.1 Sistema de Autenticação JWT

O sistema usa **JWT (JSON Web Tokens)** para autenticação stateless.

**Fluxo de Autenticação**:
```
1. Cliente faz POST /api/auth/login com email e senha
2. Sistema valida credenciais
3. Sistema gera token JWT com claims
4. Cliente recebe token
5. Cliente envia token no header Authorization: Bearer {token}
6. Sistema valida token em cada requisição
7. Sistema autoriza baseado em roles
```

**Claims no Token JWT**:
```json
{
  "sub": "guid-do-usuario",
  "email": "usuario@empresa.com",
  "role": "CompanyAdmin",
  "CompanyId": "guid-da-empresa",
  "jti": "guid-do-token",
  "exp": "timestamp-expiracao"
}
```

### 3.2 Níveis de Acesso (Roles)

O sistema possui **3 níveis hierárquicos** de acesso:

#### **1. Admin (Master Admin)**
- **Descrição**: Administrador global do sistema
- **CompanyId**: NULL (não pertence a nenhuma empresa)
- **Permissões**: Acesso total a TUDO no sistema
- **Uso**: Apenas para setup inicial e manutenção do sistema
- **Quantidade**: 1 usuário apenas (criado no primeiro acesso)

#### **2. CompanyAdmin (Administrador da Empresa)**
- **Descrição**: Administrador de uma empresa específica
- **CompanyId**: Guid da empresa
- **Permissões**:
  - Criar/editar/deletar usuários da sua empresa
  - Criar/editar armazéns
  - Criar/editar produtos, clientes, fornecedores
  - Acessar todos os dados da empresa
  - Configurar sistema
- **Uso**: Gerentes e administradores da empresa
- **Quantidade**: Vários por empresa

#### **3. CompanyUser (Usuário Operacional)**
- **Descrição**: Usuário operacional da empresa
- **CompanyId**: Guid da empresa
- **Permissões**:
  - Executar operações do dia a dia
  - Recebimento, separação, embalagem
  - Consultar produtos e estoque
  - Não pode criar/deletar usuários
  - Não pode alterar configurações
- **Uso**: Operadores de armazém, conferentes
- **Quantidade**: Muitos por empresa

### 3.3 Políticas de Autorização

**Políticas definidas no Program.cs**:

```csharp
// Apenas Admin Master
options.AddPolicy("AdminOnly", policy => 
    policy.RequireRole("Admin"));

// Admin Master OU CompanyAdmin
options.AddPolicy("CompanyAdminOnly", policy => 
    policy.RequireRole("Admin", "CompanyAdmin"));

// Qualquer usuário autenticado da empresa
options.AddPolicy("CompanyAccess", policy => 
    policy.RequireRole("Admin", "CompanyAdmin", "CompanyUser"));
```

**Uso nos Controllers**:

```csharp
[Authorize(Policy = "CompanyAdminOnly")]
public async Task<ActionResult> CreateUser(...)
{
    // Apenas Admin ou CompanyAdmin podem criar usuários
}
```

### 3.4 Multi-Tenancy

O sistema é **multi-tenant por empresa** (CompanyId).

**Isolamento de Dados**:
- Cada empresa tem seus próprios dados
- Usuários só acessam dados da sua empresa
- Admin Master vê tudo
- CompanyId é filtro em TODAS as consultas

**Exemplo de Isolamento**:
```csharp
// Service sempre filtra por CompanyId
public async Task<IEnumerable<Product>> GetByCompanyId(Guid companyId)
{
    return await _repository.FindAsync(p => p.CompanyId == companyId);
}
```

---

## 📊 4. BANCO DE DADOS

### 4.1 Tecnologia

- **SGBD**: MySQL 8.0+ ou MariaDB 10.6+
- **ORM**: Entity Framework Core 7.x
- **Migrações**: Code-First Migrations
- **Connection String**: Configurada em appsettings.json

**appsettings.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=logistics_wms;User=root;Password=senha;"
  }
}
```

### 4.2 Tabelas Principais

O sistema possui **29 tabelas principais** organizadas por módulo:

#### **Módulo Core (5 tabelas)**
- `Companies` - Empresas
- `Users` - Usuários
- `Warehouses` - Armazéns
- `WarehouseZones` - Zonas de armazém
- `StorageLocations` - Endereços de armazenamento

#### **Módulo Cadastros (7 tabelas)**
- `Products` - Produtos
- `Customers` - Clientes
- `Suppliers` - Fornecedores
- `Vehicles` - Veículos
- `Drivers` - Motoristas
- `DockDoors` - Portas de docagem
- `Lots` - Lotes

#### **Módulo Inbound (4 tabelas)**
- `InboundShipments` - Remessas de entrada
- `Receipts` - Recebimentos
- `ReceiptLines` - Linhas de recebimento
- `PutawayTasks` - Tarefas de endereçamento

#### **Módulo Outbound (7 tabelas)**
- `Orders` - Pedidos
- `OrderItems` - Itens de pedidos
- `PickingWaves` - Ondas de separação
- `PickingTasks` - Tarefas de separação
- `PickingLines` - Linhas de separação
- `PackingTasks` - Tarefas de embalagem
- `Packages` - Pacotes
- `OutboundShipments` - Remessas de saída

#### **Módulo Inventário (6 tabelas)**
- `Inventories` - Estoque
- `StockMovements` - Movimentações
- `SerialNumbers` - Números de série
- `CycleCounts` - Contagens cíclicas
- `VehicleAppointments` - Agendamentos

### 4.3 Relacionamentos Principais

```
Company (1) -----> (N) Users
Company (1) -----> (N) Warehouses
Company (1) -----> (N) Products
Company (1) -----> (N) Orders

Warehouse (1) -----> (N) WarehouseZones
Warehouse (1) -----> (N) StorageLocations

Product (1) -----> (N) Inventory
Product (1) -----> (N) OrderItems
Product (1) -----> (N) Lots

StorageLocation (1) -----> (N) Inventory

Order (1) -----> (N) OrderItems
Order (1) -----> (1) InboundShipment
Order (1) -----> (1) OutboundShipment

InboundShipment (1) -----> (N) Receipts
Receipt (1) -----> (N) ReceiptLines
Receipt (1) -----> (N) PutawayTasks

PickingWave (1) -----> (N) PickingTasks
PackingTask (1) -----> (N) Packages
```

---

## 🔄 5. FLUXO DE DADOS

### 5.1 Fluxo de uma Requisição Típica

```
1. Cliente HTTP
   ↓
2. Controller (API Layer)
   - Recebe Request
   - Valida JWT
   - Valida Autorização (Roles)
   - Valida ModelState
   ↓
3. Service (Application Layer)
   - Orquestra lógica de negócio
   - Chama Repositories
   - Mapeia Entidades ↔ DTOs
   ↓
4. Repository (Infrastructure Layer)
   - Acessa DbContext
   - Executa queries
   - Retorna Entidades
   ↓
5. DbContext (EF Core)
   - Traduz LINQ para SQL
   - Executa no banco
   ↓
6. MySQL Database
   - Retorna dados
   ↓
7. Resposta volta pela cadeia
   - Entity → Service
   - Service → DTO
   - DTO → Controller
   - Controller → HTTP Response JSON
```

### 5.2 Exemplo Prático: Criar Produto

**1. Request HTTP**:
```http
POST /api/products
Authorization: Bearer eyJhbGci0iJIUzI1...
Content-Type: application/json

{
  "companyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Notebook Dell",
  "sku": "DELL-NB-001",
  "barcode": "7891234567890"
}
```

**2. Controller**:
```csharp
[HttpPost]
[Authorize]
public async Task<ActionResult<ProductResponse>> Create([FromBody] CreateProductRequest request)
{
    var product = await _service.CreateAsync(request);
    return Ok(ApiResponse<ProductResponse>.SuccessResponse(product));
}
```

**3. Service**:
```csharp
public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
{
    // Valida se empresa existe
    var company = await _companyRepository.GetByIdAsync(request.CompanyId);
    if (company == null) throw new KeyNotFoundException("Empresa não encontrada");
    
    // Cria entidade
    var product = new Product(request.CompanyId, request.Name, request.SKU, request.Barcode);
    
    // Salva no banco
    await _productRepository.AddAsync(product);
    await _unitOfWork.CommitAsync();
    
    // Retorna DTO
    return MapToResponse(product);
}
```

**4. Repository**:
```csharp
public async Task AddAsync(Product product)
{
    await _context.Products.AddAsync(product);
}
```

**5. Response HTTP**:
```json
{
  "success": true,
  "message": "Produto criado com sucesso",
  "data": {
    "id": "9b1deb4d-3b7d-4bad-9bdd-2b0d7b3dcb6d",
    "companyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Notebook Dell",
    "sku": "DELL-NB-001",
    "barcode": "7891234567890",
    "isActive": true,
    "createdAt": "2025-11-22T14:30:00Z"
  }
}
```

---

## 📝 6. CONVENÇÕES E PADRÕES

### 6.1 Nomenclatura

**Entidades (Domain)**:
- PascalCase
- Singular
- Exemplo: `Product`, `User`, `Order`

**DTOs (Application)**:
- PascalCase
- Sufixo Request/Response
- Exemplo: `CreateProductRequest`, `ProductResponse`

**Services (Application)**:
- PascalCase
- Sufixo Service
- Exemplo: `ProductService`, `UserService`

**Repositories (Infrastructure)**:
- PascalCase
- Sufixo Repository
- Exemplo: `ProductRepository`, `UserRepository`

**Controllers (API)**:
- PascalCase
- Sufixo Controller
- Exemplo: `ProductsController`, `UsersController`

### 6.2 Estrutura de Response Padrão

Todas as respostas da API seguem o padrão `ApiResponse<T>`:

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}
```

**Sucesso**:
```json
{
  "success": true,
  "message": "Operação realizada com sucesso",
  "data": { ... }
}
```

**Erro**:
```json
{
  "success": false,
  "message": "Erro ao processar requisição",
  "errors": ["Erro 1", "Erro 2"]
}
```

### 6.3 Códigos HTTP

- `200 OK` - Sucesso
- `201 Created` - Recurso criado
- `400 Bad Request` - Validação falhou
- `401 Unauthorized` - Não autenticado
- `403 Forbidden` - Sem permissão
- `404 Not Found` - Recurso não encontrado
- `500 Internal Server Error` - Erro interno

---

## 🚀 7. PRÓXIMOS VOLUMES

Este é o **Volume 1** da documentação técnica completa. Consulte os próximos volumes para:

- **Volume 2**: Modelo de Dados e Entidades (detalhamento de cada entidade)
- **Volume 3**: API Endpoints e Controllers (todos os endpoints com exemplos)
- **Volume 4**: Serviços e Lógica de Negócio (cada serviço explicado)
- **Volume 5**: Fluxos de Processo WMS (fluxogramas detalhados)
- **Volume 6**: Autenticação, Segurança e Deployment
- **Volume 7**: Guia de Implementação para Programadores

---

**Próximo**: [Volume 2 - Modelo de Dados e Entidades](02-MODELO-DE-DADOS-ENTIDADES.md)
