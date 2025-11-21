# 🧪 DOCUMENTO 10 - PLANO COMPLETO DE TESTES UNITÁRIOS

**Data**: 2025-11-21  
**Objetivo**: Cobertura completa de testes unitários para todos os módulos implementados

---

## 📊 ESTADO ATUAL DOS TESTES

### Testes Existentes: 79 testes (100% passando)

**Domain Tests (29)**:
- CompanyTests: 7 ✅
- UserTests: 8 ✅
- ProductTests: 3 ✅
- CustomerTests: 3 ✅
- SupplierTests: 3 ✅
- WarehouseTests: 2 ✅
- InventoryTests: 6 ✅

**Integration Tests (50)**:
- Repositories: 9 ✅
- Services: 41 ✅

---

## 🎯 TESTES UNITÁRIOS FALTANTES

### 1. Domain Entities - COMPLETAR

#### VehicleTests (NOVO - 0 testes)
```csharp
[Fact] public void CreateVehicle_WithValidData_ShouldCreateSuccessfully()
[Fact] public void CreateVehicle_WithEmptyCompanyId_ShouldThrowException()
[Fact] public void CreateVehicle_WithEmptyPlate_ShouldThrowException()
[Fact] public void CreateVehicle_WithInvalidYear_ShouldThrowException()
[Fact] public void UpdateVehicle_ShouldUpdateProperties()
[Fact] public void UpdateStatus_ToInTransit_ShouldChangeStatus()
[Fact] public void UpdateStatus_ToMaintenance_ShouldChangeStatus()
```

#### DriverTests (NOVO - 0 testes)
```csharp
[Fact] public void CreateDriver_WithValidData_ShouldCreateSuccessfully()
[Fact] public void CreateDriver_WithEmptyName_ShouldThrowException()
[Fact] public void CreateDriver_WithEmptyLicenseNumber_ShouldThrowException()
[Fact] public void UpdateDriver_ShouldUpdateProperties()
[Fact] public void ActivateDriver_ShouldSetIsActiveTrue()
[Fact] public void DeactivateDriver_ShouldSetIsActiveFalse()
```

#### StorageLocationTests (NOVO - 0 testes)
```csharp
[Fact] public void CreateStorageLocation_WithValidData_ShouldCreateSuccessfully()
[Fact] public void CreateStorageLocation_WithEmptyCode_ShouldThrowException()
[Fact] public void UpdateStorageLocation_ShouldUpdateProperties()
[Fact] public void ActivateStorageLocation_ShouldSetIsActiveTrue()
```

#### StockMovementTests (NOVO - 0 testes)
```csharp
[Fact] public void CreateStockMovement_Inbound_ShouldCreateSuccessfully()
[Fact] public void CreateStockMovement_Outbound_ShouldCreateSuccessfully()
[Fact] public void CreateStockMovement_WithZeroQuantity_ShouldThrowException()
[Fact] public void CreateStockMovement_WithNegativeQuantity_ShouldThrowException()
```

**Total Domain Tests a Criar**: ~25 novos testes

---

### 2. Application Services - TESTES UNITÁRIOS (Não Integração)

#### ProductService Unit Tests (NOVO - 0 testes)
```csharp
[Fact] public async Task CreateAsync_WithValidData_ShouldCallRepositoryAndCommit()
{
    // ARRANGE
    var mockProductRepo = new Mock<IProductRepository>();
    var mockCompanyRepo = new Mock<ICompanyRepository>();
    var mockUnitOfWork = new Mock<IUnitOfWork>();
    
    mockCompanyRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
        .ReturnsAsync(new Company("Test", "12345678000190"));
    
    mockProductRepo.Setup(r => r.SKUExistsAsync(It.IsAny<string>(), null))
        .ReturnsAsync(false);
    
    var service = new ProductService(
        mockProductRepo.Object, 
        mockCompanyRepo.Object, 
        mockUnitOfWork.Object
    );
    
    var request = new ProductRequest 
    { 
        CompanyId = Guid.NewGuid(), 
        Name = "Test", 
        SKU = "SKU123" 
    };
    
    // ACT
    var response = await service.CreateAsync(request);
    
    // ASSERT
    response.Should().NotBeNull();
    response.Name.Should().Be("Test");
    mockProductRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
    mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
}

[Fact] public async Task CreateAsync_WithNonExistentCompany_ShouldThrowException()
[Fact] public async Task CreateAsync_WithDuplicateSKU_ShouldThrowException()
[Fact] public async Task UpdateAsync_WithValidData_ShouldUpdateAndCommit()
[Fact] public async Task UpdateAsync_WhenNotExists_ShouldThrowException()
[Fact] public async Task DeleteAsync_WhenExists_ShouldDeleteAndCommit()
[Fact] public async Task GetByIdAsync_WhenNotExists_ShouldThrowException()
```

#### CustomerService Unit Tests (NOVO - 0 testes)
```csharp
[Fact] public async Task CreateAsync_WithDuplicateDocument_ShouldThrowException()
[Fact] public async Task UpdateAsync_WithValidData_ShouldCallRepositoryAndCommit()
// ... similar ao ProductService
```

#### SupplierService Unit Tests (NOVO - 0 testes)
```csharp
// Similar ao CustomerService
```

#### WarehouseService Unit Tests (NOVO - 0 testes)
```csharp
[Fact] public async Task CreateAsync_WithDuplicateCode_ShouldThrowException()
// ... similar aos outros
```

**Total Service Unit Tests a Criar**: ~40 novos testes

---

### 3. Validators - TESTES UNITÁRIOS (NOVO)

#### ProductRequestValidator Tests
```csharp
[Fact] public void Validate_WithValidData_ShouldPass()
{
    var validator = new ProductRequestValidator();
    var request = new ProductRequest 
    { 
        CompanyId = Guid.NewGuid(), 
        Name = "Test", 
        SKU = "SKU123" 
    };
    
    var result = validator.Validate(request);
    result.IsValid.Should().BeTrue();
}

[Fact] public void Validate_WithEmptyName_ShouldFail()
[Fact] public void Validate_WithNameTooLong_ShouldFail()
[Fact] public void Validate_WithEmptySKU_ShouldFail()
[Fact] public void Validate_WithSKUTooLong_ShouldFail()
[Fact] public void Validate_WithNegativeWeight_ShouldFail()
```

#### VehicleRequestValidator Tests
```csharp
[Fact] public void Validate_WithInvalidYear_ShouldFail()
[Fact] public void Validate_WithPlateTooLong_ShouldFail()
// ...
```

**Total Validator Tests a Criar**: ~30 novos testes

---

### 4. DTOs - TESTES DE MAPEAMENTO

#### Mapping Tests
```csharp
[Fact] public void MapProductToResponse_ShouldMapAllProperties()
{
    var product = new Product(Guid.NewGuid(), "Test", "SKU123", "BAR123");
    product.Update("Test", "SKU123", "BAR123", "Desc", 10.5m, "kg");
    
    var response = ProductMapper.MapToResponse(product);
    
    response.Id.Should().Be(product.Id);
    response.Name.Should().Be(product.Name);
    response.SKU.Should().Be(product.SKU);
    response.Weight.Should().Be(10.5m);
}

[Fact] public void MapVehicleToResponse_ShouldMapAllProperties()
// ...
```

**Total Mapping Tests a Criar**: ~15 novos testes

---

## 📋 ORGANIZAÇÃO DOS TESTES

### Estrutura de Pastas
```
tests/
└── Logistics.Tests/
    ├── Unit/
    │   ├── Domain/
    │   │   └── Entities/
    │   │       ├── CompanyTests.cs ✅
    │   │       ├── UserTests.cs ✅
    │   │       ├── VehicleTests.cs ⚠️ CRIAR
    │   │       ├── DriverTests.cs ⚠️ CRIAR
    │   │       ├── ProductTests.cs ✅
    │   │       ├── CustomerTests.cs ✅
    │   │       ├── SupplierTests.cs ✅
    │   │       ├── WarehouseTests.cs ✅
    │   │       ├── StorageLocationTests.cs ⚠️ CRIAR
    │   │       ├── InventoryTests.cs ✅
    │   │       └── StockMovementTests.cs ⚠️ CRIAR
    │   ├── Application/
    │   │   ├── Services/
    │   │   │   ├── ProductServiceTests.cs ⚠️ CRIAR
    │   │   │   ├── CustomerServiceTests.cs ⚠️ CRIAR
    │   │   │   ├── SupplierServiceTests.cs ⚠️ CRIAR
    │   │   │   ├── WarehouseServiceTests.cs ⚠️ CRIAR
    │   │   │   ├── VehicleServiceTests.cs ⚠️ CRIAR (unit, não integration)
    │   │   │   └── DriverServiceTests.cs ⚠️ CRIAR (unit, não integration)
    │   │   ├── Validators/
    │   │   │   ├── ProductRequestValidatorTests.cs ⚠️ CRIAR
    │   │   │   ├── VehicleRequestValidatorTests.cs ⚠️ CRIAR
    │   │   │   └── ... ⚠️ CRIAR
    │   │   └── Mappers/
    │   │       └── MappingTests.cs ⚠️ CRIAR
    └── Integration/
        ├── Repositories/ ✅ Existente
        ├── Services/ ✅ Existente
        └── Concurrency/ ⚠️ CRIAR (Doc 09)
```

---

## 🛠️ CONVENÇÕES E BOAS PRÁTICAS

### 1. Nomenclatura
```csharp
// Padrão: MethodName_StateUnderTest_ExpectedBehavior
[Fact]
public void CreateProduct_WithEmptyName_ShouldThrowException()

// Para testes async
[Fact]
public async Task CreateAsync_WithValidData_ShouldReturnResponse()
```

### 2. Arrange-Act-Assert (AAA)
```csharp
[Fact]
public void TestMethod()
{
    // ARRANGE - Setup
    var sut = new SystemUnderTest();
    var input = "test";
    
    // ACT - Execute
    var result = sut.Method(input);
    
    // ASSERT - Verify
    result.Should().Be("expected");
}
```

### 3. Uso de Mocks
```csharp
// Para testes unitários de Services, usar Moq
var mockRepo = new Mock<IProductRepository>();
mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
    .ReturnsAsync(new Product(...));

// Verificar chamadas
mockRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
```

### 4. FluentAssertions
```csharp
// Preferir FluentAssertions para legibilidade
result.Should().NotBeNull();
result.Should().Be(expected);
result.Should().BeOfType<Product>();
result.Should().HaveCount(5);
action.Should().Throw<ArgumentException>()
    .WithMessage("*Nome*");
```

### 5. Testes de Exceção
```csharp
[Fact]
public void Method_WhenInvalid_ShouldThrowException()
{
    // Arrange
    var sut = new SystemUnderTest();
    
    // Act
    Action act = () => sut.Method(null);
    
    // Assert
    act.Should().Throw<ArgumentNullException>()
        .WithMessage("*parameter*");
}
```

---

## 📊 COBERTURA DE CÓDIGO ALVO

### Metas por Camada
```
Domain Layer:        95% (crítico - regras de negócio)
Application Layer:   90% (services e validators)
Infrastructure:      70% (repositories testados via integration)
API Controllers:     80% (testados via integration e E2E)

TOTAL ALVO:         85%+ de cobertura
```

### Ferramentas
```bash
# Gerar cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Gerar relatório HTML
reportgenerator -reports:coverage.opencover.xml -targetdir:coveragereport

# Visualizar
open coveragereport/index.html
```

---

## ✅ CHECKLIST DE IMPLEMENTAÇÃO

### Fase 1: Domain Tests (25 testes)
- [ ] VehicleTests (7 testes)
- [ ] DriverTests (6 testes)
- [ ] StorageLocationTests (4 testes)
- [ ] StockMovementTests (4 testes)
- [ ] Melhorar ProductTests (adicionar 4 testes)

### Fase 2: Service Unit Tests (40 testes)
- [ ] ProductServiceTests (8 testes)
- [ ] CustomerServiceTests (7 testes)
- [ ] SupplierServiceTests (7 testes)
- [ ] WarehouseServiceTests (7 testes)
- [ ] VehicleServiceTests unit (6 testes)
- [ ] DriverServiceTests unit (5 testes)

### Fase 3: Validators (30 testes)
- [ ] ProductRequestValidatorTests (6 testes)
- [ ] VehicleRequestValidatorTests (6 testes)
- [ ] DriverRequestValidatorTests (5 testes)
- [ ] CustomerRequestValidatorTests (6 testes)
- [ ] SupplierRequestValidatorTests (5 testes)
- [ ] WarehouseRequestValidatorTests (5 testes)

### Fase 4: Mappers (15 testes)
- [ ] ProductMappingTests (3 testes)
- [ ] VehicleMappingTests (3 testes)
- [ ] DriverMappingTests (3 testes)
- [ ] CustomerMappingTests (2 testes)
- [ ] SupplierMappingTests (2 testes)
- [ ] WarehouseMappingTests (2 testes)

**TOTAL NOVOS TESTES**: ~110 testes unitários

---

## 🎯 RESULTADO ESPERADO

```
Testes Atuais:     79 testes
Novos Unit Tests: +110 testes
Concurrency Tests: +20 testes (Doc 09)
─────────────────────────────
TOTAL FINAL:       209 testes

Cobertura: 85%+ em todas as camadas críticas
```

---

## 🚀 PRÓXIMOS PASSOS

1. ✅ Documento 08 criado (Arquitetura Atual)
2. ✅ Documento 09 criado (Testes de Concorrência)
3. ✅ Documento 10 criado (Testes Unitários)
4. ⚠️ **IMPLEMENTAR** Domain Tests faltantes (VehicleTests, DriverTests, etc)
5. ⚠️ **IMPLEMENTAR** Service Unit Tests (com Mocks)
6. ⚠️ **IMPLEMENTAR** Testes de Concorrência (Doc 09)
7. ⚠️ **EXECUTAR** todos testes e validar 100% sucesso
8. ⚠️ **GERAR** relatório de cobertura
9. ⚠️ Implementar StorageLocation, Inventory, StockMovement Services/Controllers
10. ⚠️ Implementar módulos avançados (Orders, Deliveries, etc)

**Pronto para começar a implementação!**
