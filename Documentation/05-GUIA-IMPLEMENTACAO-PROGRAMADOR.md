# DOCUMENTAÇÃO TÉCNICA COMPLETA - SISTEMA WMS
## Volume 5: Guia de Implementação para Programadores

**Versão**: 3.0  
**Data**: 2025-11-22

---

## 📋 ÍNDICE

1. [Setup do Ambiente de Desenvolvimento](#1-setup-ambiente)
2. [Estrutura do Código](#2-estrutura-codigo)
3. [Como Criar um Novo Módulo](#3-criar-modulo)
4. [Padrões de Código](#4-padroes-codigo)
5. [Testes](#5-testes)
6. [Deployment](#6-deployment)
7. [Troubleshooting](#7-troubleshooting)

---

## 1. SETUP DO AMBIENTE DE DESENVOLVIMENTO

### 1.1 Pré-requisitos

**Software Necessário**:
- .NET 6.0 SDK ou superior
- MySQL 8.0+ ou MariaDB 10.6+
- Visual Studio 2022 / VS Code / Rider
- Git
- Postman ou Insomnia (para testes de API)

**Verificar Instalações**:
```bash
dotnet --version          # Deve mostrar 6.0 ou superior
mysql --version           # MySQL instalado
git --version             # Git instalado
```

### 1.2 Clonar e Configurar Projeto

```bash
# 1. Clonar repositório
git clone https://github.com/seu-repo/logistics-wms.git
cd logistics-wms/API

# 2. Restaurar pacotes NuGet
dotnet restore

# 3. Configurar banco de dados
# Editar: src/Logistics.API/appsettings.json
```

**appsettings.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=logistics_wms;User=root;Password=sua_senha;"
  },
  "JwtSettings": {
    "Secret": "sua-chave-secreta-minimo-32-caracteres",
    "Issuer": "LogisticsAPI",
    "Audience": "LogisticsClient",
    "ExpirationHours": "8"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 1.3 Criar Banco de Dados

```bash
# Opção 1: Criar banco manualmente
mysql -u root -p
CREATE DATABASE logistics_wms CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
EXIT;

# Opção 2: Deixar o EF criar via migrations
cd src/Logistics.API
dotnet ef database update
```

### 1.4 Executar Migrations

```bash
# Aplicar todas as migrations
cd src/Logistics.API
dotnet ef database update

# Ver migrations aplicadas
dotnet ef migrations list

# Criar nova migration (quando alterar entidades)
dotnet ef migrations add NomeDaMigration

# Reverter última migration
dotnet ef database update PreviousMigrationName
```

### 1.5 Executar Aplicação

```bash
# Executar em modo desenvolvimento
cd src/Logistics.API
dotnet run

# Aplicação estará em:
# http://localhost:5000
# https://localhost:5001

# Swagger UI em:
# http://localhost:5000/swagger
```

---

## 2. ESTRUTURA DO CÓDIGO

### 2.1 Organização dos Projetos

```
API/src/
├── Logistics.API/              # Camada de Apresentação
│   ├── Controllers/            # Controllers REST
│   ├── Program.cs              # Entry point e configuração
│   └── appsettings.json        # Configurações
│
├── Logistics.Application/      # Camada de Aplicação
│   ├── DTOs/                   # Data Transfer Objects
│   │   ├── Auth/
│   │   ├── User/
│   │   ├── Product/
│   │   └── ...
│   ├── Services/               # Serviços de aplicação
│   └── Interfaces/             # Interfaces dos serviços
│
├── Logistics.Domain/           # Camada de Domínio
│   ├── Entities/               # Entidades de domínio
│   ├── Enums/                  # Enumerações
│   └── Interfaces/             # Interfaces de repositórios
│
└── Logistics.Infrastructure/   # Camada de Infraestrutura
    ├── Data/                   # DbContext e configurações
    ├── Repositories/           # Implementação de repositórios
    └── Migrations/             # Migrations do EF Core
```

### 2.2 Fluxo de Dependências

```
API (Controllers)
  ↓ depende de
Application (Services)
  ↓ depende de
Domain (Entities, Interfaces)
  ↑ implementado por
Infrastructure (Repositories, DbContext)
```

**Regras**:
- ✅ API pode referenciar Application
- ✅ Application pode referenciar Domain
- ✅ Infrastructure implementa interfaces do Domain
- ❌ Domain NÃO pode referenciar Infrastructure
- ❌ Domain NÃO pode referenciar Application

### 2.3 Injeção de Dependências

**Configuração no Program.cs**:
```csharp
// Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
// ... demais repositórios

// Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
// ... demais serviços

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// DbContext
builder.Services.AddDbContext<LogisticsDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
```

---

## 3. COMO CRIAR UM NOVO MÓDULO

Vamos criar um módulo completo para **Transportadoras (Carriers)** como exemplo.

### 3.1 PASSO 1: Criar Entidade (Domain)

**Arquivo**: `Logistics.Domain/Entities/Carrier.cs`

```csharp
namespace Logistics.Domain.Entities;

public class Carrier
{
    private Carrier() { } // EF Core
    
    public Carrier(Guid companyId, string name, string code)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("CompanyId não pode ser vazio");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome não pode ser vazio");
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Código não pode ser vazio");
            
        Id = Guid.NewGuid();
        CompanyId = companyId;
        Name = name;
        Code = code;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }
    
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? ContactPhone { get; private set; }
    public string? ContactEmail { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation Property
    public Company Company { get; private set; } = null!;
    
    public void Update(string name, string code, string? contactPhone, string? contactEmail)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome não pode ser vazio");
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Código não pode ser vazio");
            
        Name = name;
        Code = code;
        ContactPhone = contactPhone;
        ContactEmail = contactEmail;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
```

### 3.2 PASSO 2: Criar Interface do Repositório (Domain)

**Arquivo**: `Logistics.Domain/Interfaces/ICarrierRepository.cs`

```csharp
using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface ICarrierRepository : IRepository<Carrier>
{
    Task<IEnumerable<Carrier>> GetByCompanyIdAsync(Guid companyId);
    Task<Carrier?> GetByCodeAsync(string code, Guid companyId);
}
```

### 3.3 PASSO 3: Criar DTOs (Application)

**Arquivo**: `Logistics.Application/DTOs/Carrier/CreateCarrierRequest.cs`

```csharp
namespace Logistics.Application.DTOs.Carrier;

public record CreateCarrierRequest(
    Guid CompanyId,
    string Name,
    string Code,
    string? ContactPhone,
    string? ContactEmail
);
```

**Arquivo**: `Logistics.Application/DTOs/Carrier/UpdateCarrierRequest.cs`

```csharp
namespace Logistics.Application.DTOs.Carrier;

public record UpdateCarrierRequest(
    string Name,
    string Code,
    string? ContactPhone,
    string? ContactEmail
);
```

**Arquivo**: `Logistics.Application/DTOs/Carrier/CarrierResponse.cs`

```csharp
namespace Logistics.Application.DTOs.Carrier;

public record CarrierResponse(
    Guid Id,
    Guid CompanyId,
    string CompanyName,
    string Name,
    string Code,
    string? ContactPhone,
    string? ContactEmail,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
```

### 3.4 PASSO 4: Criar Interface do Service (Application)

**Arquivo**: `Logistics.Application/Interfaces/ICarrierService.cs`

```csharp
using Logistics.Application.DTOs.Carrier;

namespace Logistics.Application.Interfaces;

public interface ICarrierService
{
    Task<CarrierResponse> CreateAsync(CreateCarrierRequest request);
    Task<CarrierResponse> GetByIdAsync(Guid id);
    Task<IEnumerable<CarrierResponse>> GetAllAsync();
    Task<IEnumerable<CarrierResponse>> GetByCompanyIdAsync(Guid companyId);
    Task<CarrierResponse> UpdateAsync(Guid id, UpdateCarrierRequest request);
    Task DeleteAsync(Guid id);
}
```

### 3.5 PASSO 5: Implementar Service (Application)

**Arquivo**: `Logistics.Application/Services/CarrierService.cs`

```csharp
using Logistics.Application.DTOs.Carrier;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class CarrierService : ICarrierService
{
    private readonly ICarrierRepository _repository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CarrierService(
        ICarrierRepository repository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<CarrierResponse> CreateAsync(CreateCarrierRequest request)
    {
        // Validar se empresa existe
        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company == null)
            throw new KeyNotFoundException("Empresa não encontrada");
        
        // Validar se código já existe
        var existing = await _repository.GetByCodeAsync(request.Code, request.CompanyId);
        if (existing != null)
            throw new InvalidOperationException("Código já cadastrado");
        
        // Criar entidade
        var carrier = new Carrier(
            request.CompanyId,
            request.Name,
            request.Code
        );
        
        if (!string.IsNullOrWhiteSpace(request.ContactPhone) || 
            !string.IsNullOrWhiteSpace(request.ContactEmail))
        {
            carrier.Update(
                request.Name,
                request.Code,
                request.ContactPhone,
                request.ContactEmail
            );
        }
        
        // Salvar
        await _repository.AddAsync(carrier);
        await _unitOfWork.CommitAsync();
        
        return await MapToResponseAsync(carrier);
    }
    
    public async Task<CarrierResponse> GetByIdAsync(Guid id)
    {
        var carrier = await _repository.GetByIdAsync(id);
        if (carrier == null)
            throw new KeyNotFoundException("Transportadora não encontrada");
            
        return await MapToResponseAsync(carrier);
    }
    
    public async Task<IEnumerable<CarrierResponse>> GetAllAsync()
    {
        var carriers = await _repository.GetAllAsync();
        var responses = new List<CarrierResponse>();
        
        foreach (var carrier in carriers)
        {
            responses.Add(await MapToResponseAsync(carrier));
        }
        
        return responses;
    }
    
    public async Task<IEnumerable<CarrierResponse>> GetByCompanyIdAsync(Guid companyId)
    {
        var carriers = await _repository.GetByCompanyIdAsync(companyId);
        var responses = new List<CarrierResponse>();
        
        foreach (var carrier in carriers)
        {
            responses.Add(await MapToResponseAsync(carrier));
        }
        
        return responses;
    }
    
    public async Task<CarrierResponse> UpdateAsync(Guid id, UpdateCarrierRequest request)
    {
        var carrier = await _repository.GetByIdAsync(id);
        if (carrier == null)
            throw new KeyNotFoundException("Transportadora não encontrada");
        
        carrier.Update(
            request.Name,
            request.Code,
            request.ContactPhone,
            request.ContactEmail
        );
        
        await _repository.UpdateAsync(carrier);
        await _unitOfWork.CommitAsync();
        
        return await MapToResponseAsync(carrier);
    }
    
    public async Task DeleteAsync(Guid id)
    {
        var carrier = await _repository.GetByIdAsync(id);
        if (carrier == null)
            throw new KeyNotFoundException("Transportadora não encontrada");
        
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
    
    private async Task<CarrierResponse> MapToResponseAsync(Carrier carrier)
    {
        var company = await _companyRepository.GetByIdAsync(carrier.CompanyId);
        
        return new CarrierResponse(
            carrier.Id,
            carrier.CompanyId,
            company?.Name ?? "",
            carrier.Name,
            carrier.Code,
            carrier.ContactPhone,
            carrier.ContactEmail,
            carrier.IsActive,
            carrier.CreatedAt,
            carrier.UpdatedAt
        );
    }
}
```

### 3.6 PASSO 6: Implementar Repository (Infrastructure)

**Arquivo**: `Logistics.Infrastructure/Repositories/CarrierRepository.cs`

```csharp
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Logistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class CarrierRepository : BaseRepository<Carrier>, ICarrierRepository
{
    public CarrierRepository(LogisticsDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Carrier>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _dbSet
            .Where(c => c.CompanyId == companyId)
            .ToListAsync();
    }
    
    public async Task<Carrier?> GetByCodeAsync(string code, Guid companyId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Code == code && c.CompanyId == companyId);
    }
}
```

### 3.7 PASSO 7: Adicionar DbSet no DbContext

**Arquivo**: `Logistics.Infrastructure/Data/LogisticsDbContext.cs`

```csharp
public DbSet<Carrier> Carriers { get; set; }
```

### 3.8 PASSO 8: Criar Controller (API)

**Arquivo**: `Logistics.API/Controllers/CarriersController.cs`

```csharp
using Logistics.Application.DTOs.Carrier;
using Logistics.Application.DTOs.Common;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarriersController : ControllerBase
{
    private readonly ICarrierService _service;
    private readonly ILogger<CarriersController> _logger;
    
    public CarriersController(ICarrierService service, ILogger<CarriersController> logger)
    {
        _service = service;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CarrierResponse>>> Create([FromBody] CreateCarrierRequest request)
    {
        try
        {
            var carrier = await _service.CreateAsync(request);
            return Ok(ApiResponse<CarrierResponse>.SuccessResponse(carrier, "Transportadora criada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CarrierResponse>.ErrorResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CarrierResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar transportadora");
            return StatusCode(500, ApiResponse<CarrierResponse>.ErrorResponse("Erro interno no servidor"));
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CarrierResponse>>> GetById(Guid id)
    {
        try
        {
            var carrier = await _service.GetByIdAsync(id);
            return Ok(ApiResponse<CarrierResponse>.SuccessResponse(carrier));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CarrierResponse>.ErrorResponse(ex.Message));
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CarrierResponse>>>> GetAll()
    {
        var carriers = await _service.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<CarrierResponse>>.SuccessResponse(carriers));
    }
    
    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CarrierResponse>>>> GetByCompanyId(Guid companyId)
    {
        var carriers = await _service.GetByCompanyIdAsync(companyId);
        return Ok(ApiResponse<IEnumerable<CarrierResponse>>.SuccessResponse(carriers));
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CarrierResponse>>> Update(Guid id, [FromBody] UpdateCarrierRequest request)
    {
        try
        {
            var carrier = await _service.UpdateAsync(id, request);
            return Ok(ApiResponse<CarrierResponse>.SuccessResponse(carrier, "Transportadora atualizada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CarrierResponse>.ErrorResponse(ex.Message));
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Transportadora deletada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }
}
```

### 3.9 PASSO 9: Registrar no DI Container

**Arquivo**: `Logistics.API/Program.cs`

```csharp
// Repositories
builder.Services.AddScoped<ICarrierRepository, CarrierRepository>();

// Services
builder.Services.AddScoped<ICarrierService, CarrierService>();
```

### 3.10 PASSO 10: Criar Migration

```bash
cd src/Logistics.API
dotnet ef migrations add AddCarrierEntity
dotnet ef database update
```

### 3.11 PASSO 11: Testar no Swagger

1. Executar aplicação: `dotnet run`
2. Abrir: `http://localhost:5000/swagger`
3. Testar endpoints de Carriers

---

## 4. PADRÕES DE CÓDIGO

### 4.1 Nomenclatura

**Classes**:
- PascalCase
- Singular para entidades: `Product`, `Order`
- Plural para collections: `Products`, `Orders`

**Métodos**:
- PascalCase
- Verbos no início: `CreateAsync`, `GetByIdAsync`, `UpdateAsync`

**Propriedades**:
- PascalCase
- Substantivos: `Name`, `Email`, `CreatedAt`

**Variáveis Locais**:
- camelCase
- Descritivas: `userId`, `productName`, `orderTotal`

**Constantes**:
- PascalCase ou UPPER_CASE
- Exemplo: `MaxRetries`, `DEFAULT_TIMEOUT`

### 4.2 Async/Await

**SEMPRE use async/await para I/O**:

```csharp
// ✅ CORRETO
public async Task<Product> GetProductAsync(Guid id)
{
    return await _repository.GetByIdAsync(id);
}

// ❌ ERRADO
public Product GetProduct(Guid id)
{
    return _repository.GetByIdAsync(id).Result; // Bloqueia thread
}
```

### 4.3 Exception Handling

**No Controller**:
```csharp
try
{
    var result = await _service.CreateAsync(request);
    return Ok(ApiResponse<T>.SuccessResponse(result));
}
catch (KeyNotFoundException ex)
{
    return NotFound(ApiResponse<T>.ErrorResponse(ex.Message));
}
catch (InvalidOperationException ex)
{
    return BadRequest(ApiResponse<T>.ErrorResponse(ex.Message));
}
catch (Exception ex)
{
    _logger.LogError(ex, "Erro ao criar recurso");
    return StatusCode(500, ApiResponse<T>.ErrorResponse("Erro interno"));
}
```

**No Service**:
```csharp
// Lançar exceções específicas
if (entity == null)
    throw new KeyNotFoundException("Entidade não encontrada");

if (duplicado)
    throw new InvalidOperationException("Já existe registro");
```

### 4.4 Logging

```csharp
// Information
_logger.LogInformation("Criando produto {ProductName}", product.Name);

// Warning
_logger.LogWarning("Tentativa de acesso negada para usuário {UserId}", userId);

// Error
_logger.LogError(ex, "Erro ao salvar no banco de dados");

// Debug
_logger.LogDebug("Validando dados de entrada");
```

---

## 5. TESTES

### 5.1 Estrutura de Testes

```
tests/
└── Logistics.Tests/
    ├── Unit/               # Testes unitários
    │   ├── Services/
    │   ├── Entities/
    │   └── Validators/
    ├── Integration/        # Testes de integração
    │   ├── Controllers/
    │   └── Repositories/
    └── E2E/                # Testes end-to-end
```

### 5.2 Exemplo de Teste Unitário

```csharp
using Xunit;
using Moq;
using FluentAssertions;

namespace Logistics.Tests.Unit.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly ProductService _service;
    
    public ProductServiceTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _mockUow = new Mock<IUnitOfWork>();
        _service = new ProductService(_mockRepo.Object, _mockUow.Object);
    }
    
    [Fact]
    public async Task CreateAsync_ValidProduct_ReturnsProductResponse()
    {
        // Arrange
        var request = new CreateProductRequest(
            Guid.NewGuid(),
            "Test Product",
            "SKU-001",
            null
        );
        
        // Act
        var result = await _service.CreateAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Product");
        _mockUow.Verify(u => u.CommitAsync(), Times.Once);
    }
}
```

---

## 6. DEPLOYMENT

### 6.1 Publicar Aplicação

```bash
# Build para produção
dotnet publish -c Release -o ./publish

# A pasta ./publish contém todos os arquivos necessários
```

### 6.2 Docker (Opcional)

**Dockerfile**:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Logistics.API/Logistics.API.csproj", "Logistics.API/"]
RUN dotnet restore "Logistics.API/Logistics.API.csproj"
COPY . .
WORKDIR "/src/Logistics.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Logistics.API.dll"]
```

---

## 7. TROUBLESHOOTING

### 7.1 Problemas Comuns

**Migration não funciona**:
```bash
# Limpar bin e obj
dotnet clean
rm -rf bin/ obj/

# Restaurar e tentar novamente
dotnet restore
dotnet ef migrations add NomeMigration
```

**Erro de conexão com banco**:
- Verificar se MySQL está rodando
- Verificar credenciais no appsettings.json
- Verificar se banco existe

**JWT não autentica**:
- Verificar se Secret tem mínimo 32 caracteres
- Verificar se está enviando header correto
- Verificar se token não expirou

---

**FIM DO VOLUME 5**
