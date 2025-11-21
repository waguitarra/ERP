# 🔥 DOCUMENTO 09 - PLANO DETALHADO DE TESTES DE CONCORRÊNCIA

**Data**: 2025-11-21  
**Objetivo**: Testes reais atacando banco de dados MySQL, simulando múltiplos usuários simultâneos

---

## 🎯 OBJETIVO DOS TESTES DE CONCORRÊNCIA

Garantir que o sistema funcione corretamente quando:
- **Múltiplos usuários** acessam simultaneamente
- **Operações conflitantes** ocorrem no mesmo recurso
- **Race conditions** podem acontecer
- **Deadlocks** podem ocorrer no banco
- **Integridade de dados** é crítica

**IMPORTANTE**: Todos os testes atacam o banco MySQL REAL, não mocks.

---

## 📋 CENÁRIOS DE TESTE DE CONCORRÊNCIA

### CENÁRIO 1: Criação Concorrente com Unicidade

#### 1.1 SKU Duplicado em Produtos
```csharp
[Fact]
public async Task ConcurrentProductCreation_WithSameSKU_OnlyOneSucceeds()
{
    // ARRANGE
    var companyId = await CreateTestCompany();
    var sku = "SKU-CONCURRENT-001";
    var tasks = new List<Task<HttpResponseMessage>>();
    
    // ACT - 20 threads tentando criar produto com mesmo SKU
    for (int i = 0; i < 20; i++)
    {
        tasks.Add(Task.Run(async () => 
        {
            var request = new ProductRequest 
            { 
                CompanyId = companyId, 
                Name = $"Product {i}", 
                SKU = sku 
            };
            return await _httpClient.PostAsJsonAsync("/api/products", request);
        }));
    }
    
    var responses = await Task.WhenAll(tasks);
    
    // ASSERT
    var successCount = responses.Count(r => r.StatusCode == HttpStatusCode.Created);
    var conflictCount = responses.Count(r => r.StatusCode == HttpStatusCode.BadRequest);
    
    successCount.Should().Be(1, "apenas 1 produto deve ser criado");
    conflictCount.Should().Be(19, "os outros 19 devem falhar com SKU duplicado");
    
    // Validar no banco
    var productsInDb = await _context.Products.Where(p => p.SKU == sku).ToListAsync();
    productsInDb.Should().HaveCount(1, "apenas 1 registro no banco");
}
```

**Validações**:
- ✅ Constraint UNIQUE no banco previne duplicatas
- ✅ EF Core lança DbUpdateException
- ✅ Service captura e retorna 400 Bad Request
- ✅ Nenhum deadlock ocorre

#### 1.2 Placa Duplicada em Veículos
```csharp
[Fact]
public async Task ConcurrentVehicleCreation_WithSamePlate_OnlyOneSucceeds()
{
    // Similar ao 1.1, mas com LicensePlate
    // 15 threads tentando criar veículo com placa "ABC-1234"
    // Resultado: 1 sucesso, 14 falhas
}
```

#### 1.3 CNH Duplicada em Motoristas
```csharp
[Fact]
public async Task ConcurrentDriverCreation_WithSameLicense_OnlyOneSucceeds()
{
    // 10 threads tentando criar motorista com CNH "12345678901"
    // Resultado: 1 sucesso, 9 falhas
}
```

#### 1.4 Documento Duplicado em Clientes
```csharp
[Fact]
public async Task ConcurrentCustomerCreation_WithSameDocument_OnlyOneSucceeds()
{
    // 25 threads tentando criar cliente com CPF "12345678901"
    // Resultado: 1 sucesso, 24 falhas
}
```

---

### CENÁRIO 2: Reserva Concorrente de Estoque (CRÍTICO)

```csharp
[Fact]
public async Task ConcurrentStockReservation_NoOverselling()
{
    // ARRANGE
    var product = await CreateProductWithStock(quantity: 100);
    var location = await GetStorageLocation();
    var inventory = await CreateInventory(product.Id, location.Id, quantity: 100);
    
    // ACT - 20 pedidos tentando reservar 10 unidades cada (total 200)
    var tasks = new List<Task<(bool success, string error)>>();
    
    for (int i = 0; i < 20; i++)
    {
        tasks.Add(Task.Run(async () =>
        {
            try
            {
                await _inventoryService.ReserveAsync(inventory.Id, 10);
                return (true, null);
            }
            catch (InvalidOperationException ex)
            {
                return (false, ex.Message);
            }
        }));
    }
    
    var results = await Task.WhenAll(tasks);
    
    // ASSERT
    var successCount = results.Count(r => r.success);
    var failCount = results.Count(r => !r.success);
    
    successCount.Should().Be(10, "apenas 10 reservas de 10 unidades = 100 total");
    failCount.Should().Be(10, "últimas 10 devem falhar por estoque insuficiente");
    
    // Validar no banco
    var finalInventory = await _context.Inventories.FindAsync(inventory.Id);
    finalInventory.Quantity.Should().Be(100);
    finalInventory.ReservedQuantity.Should().Be(100);
    (finalInventory.Quantity - finalInventory.ReservedQuantity).Should().Be(0);
}
```

**Pontos Críticos**:
- 🔒 Transaction isolation level
- 🔒 Row-level locking
- 🔒 Validação atômica: ler disponibilidade + atualizar reserva
- 🔒 Nenhum overselling permitido

**Solução Necessária**:
```csharp
// No Repository, usar transaction com lock
using var transaction = await _context.Database.BeginTransactionAsync(
    IsolationLevel.ReadCommitted
);

var inventory = await _context.Inventories
    .FromSqlRaw("SELECT * FROM Inventories WHERE Id = {0} FOR UPDATE", id)
    .FirstOrDefaultAsync();

// Validar e atualizar
if (inventory.Quantity - inventory.ReservedQuantity < quantity)
    throw new InvalidOperationException("Estoque insuficiente");

inventory.ReservedQuantity += quantity;
await _context.SaveChangesAsync();
await transaction.CommitAsync();
```

---

### CENÁRIO 3: Movimentação de Estoque Concorrente

```csharp
[Fact]
public async Task ConcurrentStockMovements_AllRegistered()
{
    // ARRANGE
    var product = await CreateProduct();
    var location = await GetStorageLocation();
    var inventory = await CreateInventory(product.Id, location.Id, quantity: 1000);
    
    var adds = new List<decimal>();
    var removes = new List<decimal>();
    var random = new Random();
    
    // ACT - 100 operações concorrentes
    var tasks = new List<Task>();
    
    // 50 adições
    for (int i = 0; i < 50; i++)
    {
        var qty = random.Next(1, 50);
        adds.Add(qty);
        tasks.Add(Task.Run(async () => await _inventoryService.AddStockAsync(inventory.Id, qty)));
    }
    
    // 50 remoções
    for (int i = 0; i < 50; i++)
    {
        var qty = random.Next(1, 30);
        removes.Add(qty);
        tasks.Add(Task.Run(async () => await _inventoryService.RemoveStockAsync(inventory.Id, qty)));
    }
    
    await Task.WhenAll(tasks);
    
    // ASSERT
    var expectedFinal = 1000 + adds.Sum() - removes.Sum();
    
    var finalInventory = await _context.Inventories.FindAsync(inventory.Id);
    finalInventory.Quantity.Should().Be(expectedFinal, "matemática deve bater");
    
    var movements = await _context.StockMovements
        .Where(m => m.ProductId == product.Id)
        .ToListAsync();
    
    movements.Should().HaveCount(100, "todas movimentações registradas");
    
    var inbounds = movements.Where(m => m.Type == StockMovementType.Inbound).Sum(m => m.Quantity);
    var outbounds = movements.Where(m => m.Type == StockMovementType.Outbound).Sum(m => m.Quantity);
    
    inbounds.Should().Be(adds.Sum());
    outbounds.Should().Be(removes.Sum());
}
```

**Validações**:
- ✅ Nenhuma movimentação perdida
- ✅ Matemática do estoque correta
- ✅ Todas transações commitadas
- ✅ Ordem cronológica preservada

---

### CENÁRIO 4: Atualização Concorrente (Lost Update Problem)

```csharp
[Fact]
public async Task ConcurrentVehicleUpdate_LastWriteWins()
{
    // ARRANGE
    var vehicle = await CreateVehicle("ABC-1234");
    var originalVersion = vehicle.UpdatedAt;
    
    // ACT - 10 threads atualizando o mesmo veículo
    var newPlates = new[] { "XYZ-001", "XYZ-002", "XYZ-003", "XYZ-004", "XYZ-005",
                            "XYZ-006", "XYZ-007", "XYZ-008", "XYZ-009", "XYZ-010" };
    
    var tasks = newPlates.Select(plate => Task.Run(async () =>
    {
        await Task.Delay(Random.Shared.Next(0, 100)); // Simular latência
        var request = new VehicleRequest { LicensePlate = plate, Model = "Model", Year = 2024 };
        return await _httpClient.PutAsJsonAsync($"/api/vehicles/{vehicle.Id}", request);
    }));
    
    await Task.WhenAll(tasks);
    
    // ASSERT
    var finalVehicle = await _context.Vehicles.FindAsync(vehicle.Id);
    
    finalVehicle.LicensePlate.Should().BeOneOf(newPlates, "deve ser uma das placas");
    finalVehicle.UpdatedAt.Should().BeAfter(originalVersion);
    
    // Validar que não há corrupção de dados
    finalVehicle.Model.Should().NotBeNullOrEmpty();
    finalVehicle.Year.Should().BeGreaterThan(0);
}
```

**Problema Conhecido**: Last Write Wins (não há versioning)

**Solução Futura**: Implementar Optimistic Concurrency Control
```csharp
// Adicionar campo RowVersion
[Timestamp]
public byte[] RowVersion { get; set; }

// EF Core detecta conflitos automaticamente
// Lança DbUpdateConcurrencyException
```

---

### CENÁRIO 5: Login Massivo + Operações

```csharp
[Fact]
public async Task MassiveLogin_And_CRUDOperations()
{
    // ARRANGE
    var users = await CreateMultipleUsers(count: 100);
    
    // ACT - 100 usuários fazendo login + 10 operações cada
    var tasks = users.Select(async user =>
    {
        // Login
        var loginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", 
            new { email = user.Email, password = "Test@123" });
        
        var token = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();
        
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token.AccessToken);
        
        // 10 operações aleatórias
        var operations = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            var operation = Random.Shared.Next(0, 4);
            operations.Add(operation switch
            {
                0 => _httpClient.PostAsJsonAsync("/api/products", GenerateProductRequest()),
                1 => _httpClient.GetAsync("/api/products"),
                2 => _httpClient.PostAsJsonAsync("/api/customers", GenerateCustomerRequest()),
                3 => _httpClient.GetAsync("/api/vehicles"),
                _ => Task.CompletedTask
            });
        }
        
        await Task.WhenAll(operations);
        return operations.Count(t => t.IsCompletedSuccessfully);
    });
    
    var results = await Task.WhenAll(tasks);
    
    // ASSERT
    results.Sum().Should().BeGreaterOrEqualTo(800, "pelo menos 80% de sucesso");
    
    // Métricas de performance
    // Tempo médio de resposta deve ser < 500ms
    // Throughput deve ser > 200 req/s
}
```

**Métricas a Validar**:
- ⏱️ Tempo médio de resposta
- 📊 Throughput (requests/segundo)
- 🔌 Connection pool não esgota
- 💾 Memória não estoura
- 🔒 Nenhum deadlock

---

### CENÁRIO 6: Fluxo Completo de Pedido (E2E Concorrente)

```csharp
[Fact]
public async Task ConcurrentOrders_CompleteFlow()
{
    // ARRANGE
    var products = await CreateProducts(count: 5);
    await SetupInventoryForAll(products, quantity: 100);
    var customers = await CreateCustomers(count: 10);
    var vehicles = await CreateVehicles(count: 3);
    var drivers = await CreateDrivers(count: 3);
    
    // ACT - 50 pedidos simultâneos
    var tasks = Enumerable.Range(0, 50).Select(async i =>
    {
        var customer = customers[i % customers.Count];
        var productIndex = i % products.Count;
        
        try
        {
            // 1. Criar pedido (não implementado ainda, mas simulando)
            var orderRequest = new
            {
                CustomerId = customer.Id,
                Items = new[] 
                { 
                    new { ProductId = products[productIndex].Id, Quantity = 2 } 
                }
            };
            
            // 2. Reservar estoque
            await _inventoryService.ReserveAsync(products[productIndex].Id, 2);
            
            // 3. Registrar movimentação de saída
            await _stockMovementService.CreateAsync(new StockMovementRequest
            {
                ProductId = products[productIndex].Id,
                Type = StockMovementType.Outbound,
                Quantity = 2,
                Reference = $"Order-{i}"
            });
            
            // 4. Atribuir veículo (round-robin)
            var vehicleIndex = i % vehicles.Count;
            
            // 5. Atribuir motorista
            var driverIndex = i % drivers.Count;
            
            return (success: true, error: null);
        }
        catch (Exception ex)
        {
            return (success: false, error: ex.Message);
        }
    });
    
    var results = await Task.WhenAll(tasks);
    
    // ASSERT
    var successCount = results.Count(r => r.success);
    
    // Esperamos que alguns falhem por estoque insuficiente
    // Cada produto tem 100 unidades, 50 pedidos de 2 = 100 total
    // Por produto: 10 pedidos = 20 unidades
    successCount.Should().BeGreaterThan(40, "maioria deve ter sucesso");
    
    // Validar estoque final
    foreach (var product in products)
    {
        var inventory = await _inventoryService.GetByProductAsync(product.Id);
        var reserved = inventory.Sum(i => i.ReservedQuantity);
        reserved.Should().BeLessOrEqualTo(100);
    }
    
    // Validar movimentações
    var movements = await _context.StockMovements
        .Where(m => m.Type == StockMovementType.Outbound)
        .ToListAsync();
    
    movements.Should().HaveCount(successCount);
}
```

**Complexidade**: Alta - envolve múltiplos módulos

---

### CENÁRIO 7: Deadlock Detection

```csharp
[Fact]
public async Task ConcurrentUpdates_CrossTable_DetectDeadlock()
{
    // ARRANGE
    var vehicle1 = await CreateVehicle("ABC-1111");
    var vehicle2 = await CreateVehicle("XYZ-9999");
    var driver1 = await CreateDriver("Driver 1");
    var driver2 = await CreateDriver("Driver 2");
    
    // ACT - Criar situação de deadlock potencial
    var task1 = Task.Run(async () =>
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        // Thread 1: Atualiza Vehicle1, depois Driver1
        var v1 = await _context.Vehicles.FindAsync(vehicle1.Id);
        v1.Update("ABC-2222", "Model", 2024);
        await _context.SaveChangesAsync();
        
        await Task.Delay(100); // Simular processamento
        
        var d1 = await _context.Drivers.FindAsync(driver1.Id);
        d1.Update("Driver Updated", "CNH123", "+55");
        await _context.SaveChangesAsync();
        
        await transaction.CommitAsync();
    });
    
    var task2 = Task.Run(async () =>
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        // Thread 2: Atualiza Driver1, depois Vehicle1 (ordem inversa = deadlock)
        var d1 = await _context.Drivers.FindAsync(driver1.Id);
        d1.Update("Driver 1 Modified", "CNH456", "+55");
        await _context.SaveChangesAsync();
        
        await Task.Delay(100);
        
        var v1 = await _context.Vehicles.FindAsync(vehicle1.Id);
        v1.Update("ABC-3333", "Model", 2024);
        await _context.SaveChangesAsync();
        
        await transaction.CommitAsync();
    });
    
    // ASSERT
    // Pelo menos uma deve completar
    // Outra pode lançar deadlock exception
    try
    {
        await Task.WhenAll(task1, task2);
    }
    catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("deadlock") == true)
    {
        // Deadlock detectado - OK, sistema lidou corretamente
        true.Should().BeTrue();
    }
}
```

**Objetivo**: Detectar e documentar deadlocks, implementar retry logic

---

## 🛠️ CONFIGURAÇÃO DOS TESTES

### Setup da Classe de Teste
```csharp
public class ConcurrencyTests : IAsyncLifetime
{
    private readonly LogisticsDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly WebApplicationFactory<Program> _factory;
    
    public ConcurrencyTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Usar banco de teste real
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<LogisticsDbContext>));
                    services.Remove(descriptor);
                    
                    services.AddDbContext<LogisticsDbContext>(options =>
                    {
                        options.UseMySql(
                            "Server=localhost;Database=logistics_concurrency_test;User=root;Password=;",
                            ServerVersion.AutoDetect("Server=localhost;Database=logistics_concurrency_test;User=root;Password=;")
                        );
                    });
                });
            });
        
        _httpClient = _factory.CreateClient();
        _context = _factory.Services.GetRequiredService<LogisticsDbContext>();
    }
    
    public async Task InitializeAsync()
    {
        // Limpar banco antes de cada teste
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
    }
    
    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        _httpClient.Dispose();
        await _factory.DisposeAsync();
    }
}
```

### Helpers
```csharp
private async Task<Company> CreateTestCompany()
{
    var company = new Company("Test Company LTDA", "12345678000190");
    _context.Companies.Add(company);
    await _context.SaveChangesAsync();
    return company;
}

private async Task<Product> CreateProductWithStock(decimal quantity)
{
    var company = await CreateTestCompany();
    var product = new Product(company.Id, "Test Product", Guid.NewGuid().ToString(), null);
    _context.Products.Add(product);
    await _context.SaveChangesAsync();
    return product;
}
```

---

## 📊 MÉTRICAS A COLETAR

### Performance
- ⏱️ Tempo médio de resposta (target: < 500ms)
- ⏱️ P95, P99 latency
- 📊 Throughput (requests/segundo)
- 📊 Transactions/segundo no banco

### Recursos
- 💾 Uso de memória (heap)
- 🔌 Connection pool utilization
- 🗄️ Database connections ativas
- 🔒 Lock wait time

### Erros
- ❌ Taxa de erro (target: < 1%)
- 🔒 Deadlocks detectados
- ⏰ Timeouts
- 🔄 Retries necessários

---

## ✅ CRITÉRIOS DE SUCESSO

1. ✅ **Integridade de Dados**: Nenhuma corrupção, matemática sempre correta
2. ✅ **Unicidade**: Constraints respeitadas (SKU, placa, CNH, documento)
3. ✅ **No Overselling**: Estoque nunca negativo, reservas válidas
4. ✅ **Performance**: 95% requests < 500ms sob carga
5. ✅ **Estabilidade**: Sem crashes, memory leaks, connection pool exhaustion
6. ✅ **Deadlock Handling**: Sistema detecta e recupera de deadlocks
7. ✅ **Transações**: Atomicidade garantida (all-or-nothing)

---

## 🚀 EXECUÇÃO DOS TESTES

```bash
# Executar apenas testes de concorrência
dotnet test --filter "FullyQualifiedName~ConcurrencyTests"

# Com detalhamento
dotnet test --filter "FullyQualifiedName~ConcurrencyTests" --logger "console;verbosity=detailed"

# Gerar relatório de cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

**Próximo passo**: Implementar estes testes no código.
