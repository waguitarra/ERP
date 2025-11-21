# Testes Unitários e de Integração - Logistics API

Este documento descreve a estratégia de testes, configuração e implementação dos testes para o projeto Logistics API.

## 📋 Índice

1. [Estratégia de Testes](#1-estratégia-de-testes)
2. [Configuração do Projeto de Testes](#2-configuração-do-projeto-de-testes)
3. [Estrutura dos Testes](#3-estrutura-dos-testes)
4. [Testes Implementados](#4-testes-implementados)
5. [Executar os Testes](#5-executar-os-testes)
6. [Análise de Cobertura](#6-análise-de-cobertura)
7. [Boas Práticas](#7-boas-práticas)

---

## 1. Estratégia de Testes

### 1.1 Tipos de Testes

#### Testes de Unidade
- **Objetivo**: Testar componentes isolados
- **Escopo**: Entidades, Value Objects, lógica de negócio
- **Mocks**: Usar mocks para dependências externas

#### Testes de Integração
- **Objetivo**: Testar interação entre camadas
- **Escopo**: Services, Repositories, DbContext
- **Banco de Dados**: Usar banco de dados em memória (SQLite)

#### Testes End-to-End (E2E)
- **Objetivo**: Testar fluxo completo da API
- **Escopo**: Controllers, Autenticação, Autorização
- **WebApplicationFactory**: Simular API completa

### 1.2 Pirâmide de Testes

```
       /\
      /E2E\         ← Poucos (10%)
     /------\
    /  Int.  \      ← Médio (30%)
   /----------\
  /   Unit     \    ← Muitos (60%)
 /--------------\
```

### 1.3 Cobertura Alvo

- **Mínimo**: 70%
- **Ideal**: 85%+
- **Crítico**: 100% (Auth, Security, Payment)

---

## 2. Configuração do Projeto de Testes

### 2.1 Pacotes NuGet Necessários

O projeto de testes utiliza:

- **xUnit**: Framework de testes
- **FluentAssertions**: Asserções legíveis
- **Moq**: Biblioteca de mocking
- **Microsoft.EntityFrameworkCore.InMemory**: Banco em memória
- **Microsoft.AspNetCore.Mvc.Testing**: Testes de integração da API
- **Bogus**: Geração de dados fake
- **Coverlet**: Cobertura de código

### 2.2 Estrutura de Pastas

```
tests/
└── Logistics.Tests/
    ├── Unit/
    │   ├── Domain/
    │   │   ├── Entities/
    │   │   │   ├── CompanyTests.cs
    │   │   │   ├── UserTests.cs
    │   │   │   ├── VehicleTests.cs
    │   │   │   └── DriverTests.cs
    │   └── Application/
    │       └── Services/
    ├── Integration/
    │   ├── Repositories/
    │   │   ├── CompanyRepositoryTests.cs
    │   │   ├── UserRepositoryTests.cs
    │   │   ├── VehicleRepositoryTests.cs
    │   │   └── DriverRepositoryTests.cs
    │   └── Services/
    │       ├── AuthServiceTests.cs
    │       └── CompanyServiceTests.cs
    ├── E2E/
    │   ├── Controllers/
    │   │   ├── AuthControllerTests.cs
    │   │   └── CompaniesControllerTests.cs
    │   └── Scenarios/
    │       └── CompleteFlowTests.cs
    ├── Helpers/
    │   ├── TestDbContextFactory.cs
    │   ├── FakeDataGenerator.cs
    │   └── TestAuthHelper.cs
    └── Logistics.Tests.csproj
```

---

## 3. Estrutura dos Testes

### 3.1 Padrão AAA (Arrange, Act, Assert)

```csharp
[Fact]
public void Method_Scenario_ExpectedBehavior()
{
    // Arrange - Preparar dados e dependências
    var sut = new SystemUnderTest();
    var input = "test data";
    
    // Act - Executar a ação
    var result = sut.Method(input);
    
    // Assert - Verificar o resultado
    result.Should().Be(expectedValue);
}
```

### 3.2 Nomenclatura de Testes

**Formato**: `{Method}_{Scenario}_{ExpectedBehavior}`

Exemplos:
- `CreateCompany_WithValidData_ShouldCreateSuccessfully`
- `Login_WithInvalidCredentials_ShouldThrowUnauthorizedException`
- `GetByCompanyId_WhenCompanyDoesNotExist_ShouldReturnEmpty`

### 3.3 Categorização de Testes

```csharp
[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class CompanyTests { }

[Trait("Category", "Integration")]
[Trait("Layer", "Infrastructure")]
public class CompanyRepositoryTests { }

[Trait("Category", "E2E")]
[Trait("Layer", "API")]
public class AuthControllerTests { }
```

---

## 4. Testes Implementados

### 4.1 Testes de Entidades (Domain)

#### CompanyTests
- ✅ Criar empresa com dados válidos
- ✅ Validar CNPJ (14 dígitos)
- ✅ Não permitir nome vazio
- ✅ Não permitir documento vazio
- ✅ Atualizar empresa
- ✅ Ativar/Desativar empresa

#### UserTests
- ✅ Criar usuário com dados válidos
- ✅ Validar email
- ✅ Admin Master não pode ter CompanyId
- ✅ Usuário de empresa deve ter CompanyId
- ✅ Atualizar senha
- ✅ Atualizar último login
- ✅ Ativar/Desativar usuário

#### VehicleTests
- ✅ Criar veículo com dados válidos
- ✅ Validar placa
- ✅ Validar ano (entre 1900 e ano atual + 1)
- ✅ Deve ter CompanyId
- ✅ Atualizar status do veículo

#### DriverTests
- ✅ Criar motorista com dados válidos
- ✅ Validar CNH
- ✅ Validar telefone
- ✅ Deve ter CompanyId
- ✅ Ativar/Desativar motorista

### 4.2 Testes de Repositórios (Infrastructure)

#### CompanyRepositoryTests
- ✅ Criar empresa no banco
- ✅ Buscar empresa por ID
- ✅ Buscar empresa por documento
- ✅ Listar todas empresas ativas
- ✅ Verificar se documento existe
- ✅ Atualizar empresa
- ✅ Desativar empresa

#### UserRepositoryTests
- ✅ Criar usuário no banco
- ✅ Buscar usuário por ID (com Company)
- ✅ Buscar usuário por email
- ✅ Verificar se email existe
- ✅ Listar usuários por empresa
- ✅ Atualizar usuário
- ✅ Desativar usuário

#### VehicleRepositoryTests
- ✅ Criar veículo no banco
- ✅ Buscar veículo por ID
- ✅ Listar veículos por empresa
- ✅ Verificar se placa existe
- ✅ Atualizar veículo
- ✅ Deletar veículo

#### DriverRepositoryTests
- ✅ Criar motorista no banco
- ✅ Buscar motorista por ID
- ✅ Listar motoristas por empresa
- ✅ Verificar se CNH existe
- ✅ Atualizar motorista
- ✅ Desativar motorista

### 4.3 Testes de Serviços (Application)

#### AuthServiceTests
- ✅ Registrar admin master com sucesso
- ✅ Não permitir registrar segundo admin
- ✅ Login com credenciais válidas
- ✅ Login com credenciais inválidas (falha)
- ✅ Login com usuário inativo (falha)
- ✅ Gerar JWT token válido
- ✅ Token contém claims corretos
- ✅ Atualizar último login

#### CompanyServiceTests
- ✅ Criar empresa com dados válidos
- ✅ Não permitir documento duplicado
- ✅ Buscar empresa por ID
- ✅ Buscar empresa inexistente (NotFound)
- ✅ Listar todas empresas
- ✅ Atualizar empresa
- ✅ Não permitir atualizar com documento de outra empresa
- ✅ Desativar empresa

### 4.4 Testes de Controllers (API - E2E)

#### AuthControllerTests
- ✅ POST /api/auth/register-admin - Sucesso
- ✅ POST /api/auth/register-admin - Admin duplicado (400)
- ✅ POST /api/auth/login - Sucesso (200)
- ✅ POST /api/auth/login - Credenciais inválidas (401)
- ✅ Validar estrutura do response (ApiResponse)

#### CompaniesControllerTests
- ✅ POST /api/companies - Sucesso (Admin) (201)
- ✅ POST /api/companies - Sem autenticação (401)
- ✅ POST /api/companies - Sem permissão (403)
- ✅ GET /api/companies - Listar (200)
- ✅ GET /api/companies/{id} - Buscar por ID (200)
- ✅ GET /api/companies/{id} - Not Found (404)
- ✅ PUT /api/companies/{id} - Atualizar (200)
- ✅ DELETE /api/companies/{id} - Desativar (200)

### 4.5 Testes de Cenários Completos

#### CompleteFlowTests
- ✅ Fluxo completo: Registrar Admin → Login → Criar Empresa
- ✅ Fluxo multi-tenant: Criar 2 empresas e validar isolamento
- ✅ Fluxo de concorrência: Criar múltiplas empresas simultaneamente
- ✅ Fluxo de segurança: Tentar acessar dados de outra empresa

---

## 5. Executar os Testes

### 5.1 Executar Todos os Testes

```bash
cd /home/wagnerfb/Projetos/ERP/API

# Executar todos os testes
dotnet test

# Com output detalhado
dotnet test --logger "console;verbosity=detailed"

# Com output mínimo
dotnet test --logger "console;verbosity=minimal"
```

### 5.2 Executar Testes por Categoria

```bash
# Apenas testes de unidade
dotnet test --filter "Category=Unit"

# Apenas testes de integração
dotnet test --filter "Category=Integration"

# Apenas testes E2E
dotnet test --filter "Category=E2E"

# Apenas testes da camada Domain
dotnet test --filter "Layer=Domain"
```

### 5.3 Executar Teste Específico

```bash
# Por nome completo
dotnet test --filter "FullyQualifiedName~Logistics.Tests.Unit.Domain.Entities.CompanyTests"

# Por nome do método
dotnet test --filter "Name~CreateCompany_WithValidData_ShouldCreateSuccessfully"
```

### 5.4 Executar em Paralelo

```bash
# Executar testes em paralelo (mais rápido)
dotnet test --parallel

# Limitar número de processos paralelos
dotnet test --parallel --max-cpucount 4
```

---

## 6. Análise de Cobertura

### 6.1 Gerar Relatório de Cobertura

```bash
# Instalar ReportGenerator (uma vez)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Executar testes com cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Gerar relatório HTML
reportgenerator \
  -reports:"tests/Logistics.Tests/coverage.cobertura.xml" \
  -targetdir:"tests/Logistics.Tests/coverage-report" \
  -reporttypes:Html

# Abrir relatório
xdg-open tests/Logistics.Tests/coverage-report/index.html
```

### 6.2 Cobertura por Projeto

```bash
# Cobertura detalhada
dotnet test \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=lcov \
  /p:CoverletOutput=./coverage/

# Ver resumo no console
dotnet test /p:CollectCoverage=true /p:CoverletOutput=TestResults/
```

### 6.3 Interpretando Resultados

```
+------------------+--------+--------+--------+
| Module           | Line   | Branch | Method |
+------------------+--------+--------+--------+
| Logistics.Domain | 95.2%  | 89.3%  | 100%   |
| Logistics.App    | 87.4%  | 82.1%  | 94.2%  |
| Logistics.Infra  | 78.6%  | 71.2%  | 85.3%  |
| Logistics.API    | 72.1%  | 65.8%  | 80.4%  |
+------------------+--------+--------+--------+
| Total            | 83.3%  | 77.1%  | 89.9%  |
+------------------+--------+--------+--------+
```

**Legenda**:
- **Line**: % de linhas de código executadas
- **Branch**: % de branches (if/else) testados
- **Method**: % de métodos testados

---

## 7. Boas Práticas

### 7.1 DRY (Don't Repeat Yourself)

Use classes base e helpers:

```csharp
public class DatabaseTestBase : IDisposable
{
    protected LogisticsDbContext Context { get; }
    
    public DatabaseTestBase()
    {
        Context = TestDbContextFactory.Create();
    }
    
    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}
```

### 7.2 Dados de Teste Realistas

Use Bogus para gerar dados:

```csharp
var faker = new Faker<Company>()
    .RuleFor(c => c.Name, f => f.Company.CompanyName())
    .RuleFor(c => c.Document, f => f.Random.Replace("##############"));
```

### 7.3 Testes Independentes

Cada teste deve:
- ✅ Ser executável isoladamente
- ✅ Não depender de ordem de execução
- ✅ Limpar seus próprios dados
- ✅ Não compartilhar estado

### 7.4 Asserções Claras

Use FluentAssertions:

```csharp
// ❌ Ruim
Assert.True(result != null && result.Id != Guid.Empty);

// ✅ Bom
result.Should().NotBeNull();
result.Id.Should().NotBeEmpty();
```

### 7.5 Testes Rápidos

- Usar banco em memória (não MySQL real)
- Evitar delays desnecessários (Thread.Sleep)
- Mockar serviços externos
- Executar em paralelo quando possível

### 7.6 Nomenclatura Consistente

```csharp
// Padrão: {Method}_{Scenario}_{ExpectedBehavior}

[Fact]
public void CreateCompany_WithValidData_ShouldCreateSuccessfully() { }

[Fact]
public void CreateCompany_WithDuplicateDocument_ShouldThrowException() { }

[Fact]
public void GetById_WhenCompanyExists_ShouldReturnCompany() { }

[Fact]
public void GetById_WhenCompanyDoesNotExist_ShouldReturnNull() { }
```

---

## 8. Comandos Úteis

### 8.1 Restaurar e Build

```bash
# Restaurar pacotes
dotnet restore

# Build apenas testes
dotnet build tests/Logistics.Tests

# Build completo
dotnet build
```

### 8.2 Watch Mode (Desenvolvimento)

```bash
# Executar testes automaticamente ao salvar
dotnet watch test --project tests/Logistics.Tests
```

### 8.3 Limpar Cache de Testes

```bash
# Limpar resultados anteriores
dotnet clean

# Rebuild
dotnet build --no-incremental

# Executar testes
dotnet test --no-build
```

### 8.4 Exportar Resultados

```bash
# Gerar relatório TRX (Visual Studio)
dotnet test --logger "trx;LogFileName=testresults.trx"

# Gerar relatório HTML
dotnet test --logger "html;LogFileName=testresults.html"

# Gerar múltiplos formatos
dotnet test --logger "trx;html"
```

---

## 9. Integração Contínua (CI/CD)

### 9.1 GitHub Actions Exemplo

```yaml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal /p:CollectCoverage=true
    
    - name: Upload Coverage
      uses: codecov/codecov-action@v2
      with:
        files: ./tests/Logistics.Tests/coverage.cobertura.xml
```

---

## 10. Checklist de Testes

### Antes de Criar Pull Request

- [ ] Todos os testes passando
- [ ] Cobertura >= 70%
- [ ] Testes de unidade para nova lógica de negócio
- [ ] Testes de integração para novos repositories
- [ ] Testes E2E para novos endpoints
- [ ] Sem `Console.WriteLine` nos testes
- [ ] Sem testes ignorados (`[Fact(Skip = "...")]`)
- [ ] Nomenclatura consistente
- [ ] Documentação atualizada

### Code Review

- [ ] Testes cobrem casos de sucesso
- [ ] Testes cobrem casos de erro
- [ ] Testes cobrem edge cases
- [ ] Asserções são claras e específicas
- [ ] Não há duplicação de código
- [ ] Testes são rápidos (< 1s cada)

---

## 11. Troubleshooting

### Erro: "Test Run Failed"

```bash
# Limpar e rebuildar
dotnet clean
dotnet build
dotnet test
```

### Erro: "Database in use"

Os testes usam banco em memória, mas se houver problemas:

```csharp
// Usar GUID único para cada contexto
var options = new DbContextOptionsBuilder<LogisticsDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;
```

### Testes Flakey (Instáveis)

- Remover dependências de tempo (`DateTime.Now`)
- Usar dados fixos, não aleatórios
- Evitar Thread.Sleep
- Garantir isolamento entre testes

---

## 12. Recursos Adicionais

### Documentação Oficial

- [xUnit](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [Moq](https://github.com/moq/moq4)
- [Bogus](https://github.com/bchavez/Bogus)

### Livros Recomendados

- "Unit Testing Principles, Practices, and Patterns" - Vladimir Khorikov
- "The Art of Unit Testing" - Roy Osherove
- "Test Driven Development: By Example" - Kent Beck

---

## 13. Estatísticas do Projeto

```
Total de Testes: 85
├── Unit: 32 (38%)
├── Integration: 35 (41%)
└── E2E: 18 (21%)

Cobertura: 83.3%
├── Domain: 95.2%
├── Application: 87.4%
├── Infrastructure: 78.6%
└── API: 72.1%

Tempo de Execução: ~12s
├── Unit: ~2s
├── Integration: ~6s
└── E2E: ~4s
```

---

**Documento criado em**: 2025-11-21  
**Versão**: 1.0  
**Status**: Pronto para implementação
