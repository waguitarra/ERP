# DOCUMENTAÇÃO TÉCNICA COMPLETA - SISTEMA WMS
## Volume 2: Modelo de Dados e Entidades

**Versão**: 3.0  
**Data**: 2025-11-22

---

## 📋 ÍNDICE

1. [Visão Geral do Modelo de Dados](#1-visão-geral)
2. [Entidades Core](#2-entidades-core)
3. [Entidades de Cadastro](#3-entidades-de-cadastro)
4. [Entidades WMS - Inbound](#4-entidades-wms-inbound)
5. [Entidades WMS - Outbound](#5-entidades-wms-outbound)
6. [Entidades de Inventário](#6-entidades-de-inventário)
7. [Enumerações](#7-enumerações)
8. [Relacionamentos](#8-relacionamentos)

---

## 1. VISÃO GERAL DO MODELO DE DADOS

### 1.1 Total de Entidades

O sistema possui **29 entidades** organizadas em 6 grupos:

| Grupo | Quantidade | Entidades |
|-------|-----------|-----------|
| **Core** | 5 | Company, User, Warehouse, WarehouseZone, StorageLocation |
| **Cadastros** | 7 | Product, Customer, Supplier, Vehicle, Driver, DockDoor, Lot |
| **Inbound** | 4 | InboundShipment, Receipt, ReceiptLine, PutawayTask |
| **Outbound** | 7 | Order, OrderItem, PickingWave, PickingTask, PickingLine, PackingTask, Package, OutboundShipment |
| **Inventário** | 4 | Inventory, StockMovement, SerialNumber, CycleCount |
| **Agendamento** | 2 | VehicleAppointment, DockDoor |

### 1.2 Padrões das Entidades

Todas as entidades seguem os mesmos padrões:

```csharp
public class EntidadeExemplo
{
    // 1. Construtor privado para EF Core
    private EntidadeExemplo() { }
    
    // 2. Construtor público com validações
    public EntidadeExemplo(parametros...)
    {
        // Validações
        // Inicializações
        // CreatedAt = DateTime.UtcNow
    }
    
    // 3. Propriedades com private set
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // 4. Navigation Properties
    public RelatedEntity Related { get; private set; } = null!;
    
    // 5. Métodos de negócio
    public void Update(...)
    {
        // Validações
        // Atualização
        UpdatedAt = DateTime.UtcNow;
    }
}
```

---

## 2. ENTIDADES CORE

### 2.1 Company (Empresa)

**Arquivo**: `Logistics.Domain/Entities/Company.cs`

```csharp
public class Company
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }        // Nome da empresa
    public string Document { get; private set; }    // CNPJ (14 dígitos)
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation Properties
    public ICollection<User> Users { get; private set; }
    public ICollection<Vehicle> Vehicles { get; private set; }
    public ICollection<Driver> Drivers { get; private set; }
    public ICollection<Warehouse> Warehouses { get; private set; }
    public ICollection<Product> Products { get; private set; }
}
```

**Regras de Negócio**:
- ✅ Nome obrigatório
- ✅ Document (CNPJ) obrigatório e deve ter 14 dígitos
- ✅ Document é validado removendo pontos, barras e hífens
- ✅ IsActive padrão = true
- ✅ Multi-tenancy: cada empresa é isolada

**Validações no Construtor**:
```csharp
// Nome não pode ser vazio
if (string.IsNullOrWhiteSpace(name))
    throw new ArgumentException("Nome da empresa é obrigatório");

// Document não pode ser vazio
if (string.IsNullOrWhiteSpace(document))
    throw new ArgumentException("Documento da empresa é obrigatório");

// Validação CNPJ (14 dígitos)
var cleanDocument = Document.Replace(".", "").Replace("/", "").Replace("-", "");
if (cleanDocument.Length != 14)
    throw new ArgumentException("Documento deve ser um CNPJ válido (14 dígitos)");
```

**Métodos Públicos**:
- `Update(name, document)` - Atualiza dados
- `Activate()` - Ativa empresa
- `Deactivate()` - Desativa empresa

**Tabela no Banco**: `Companies`

---

### 2.2 User (Usuário)

**Arquivo**: `Logistics.Domain/Entities/User.cs`

```csharp
public class User
{
    public Guid Id { get; private set; }
    public Guid? CompanyId { get; private set; }    // NULL para Admin Master
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }       // Enum: Admin, CompanyAdmin, CompanyUser
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    
    // Navigation Property
    public Company? Company { get; private set; }
}
```

**Regras de Negócio**:
- ✅ Admin Master NÃO pode ter CompanyId (deve ser NULL)
- ✅ CompanyAdmin e CompanyUser DEVEM ter CompanyId
- ✅ Email deve ser único no sistema
- ✅ Email deve conter '@'
- ✅ Senha é armazenada com BCrypt hash
- ✅ IsActive padrão = true

**Validações no Construtor**:
```csharp
// Nome obrigatório
if (string.IsNullOrWhiteSpace(Name))
    throw new ArgumentException("Nome é obrigatório");

// Email obrigatório e válido
if (string.IsNullOrWhiteSpace(Email))
    throw new ArgumentException("Email é obrigatório");
if (!Email.Contains("@"))
    throw new ArgumentException("Email inválido");

// Admin Master não pode ter CompanyId
if (Role == UserRole.Admin && CompanyId.HasValue)
    throw new InvalidOperationException("Admin Master não pode estar vinculado a uma empresa");

// Usuários de empresa devem ter CompanyId
if (Role != UserRole.Admin && !CompanyId.HasValue)
    throw new InvalidOperationException("Usuários de empresa devem estar vinculados a uma empresa");
```

**Métodos Públicos**:
- `Update(name, email)` - Atualiza dados
- `UpdatePassword(passwordHash)` - Atualiza senha
- `UpdateRole(role)` - Muda role
- `Activate()` / `Deactivate()` - Ativa/desativa
- `UpdateLastLogin()` - Registra último login

**Tabela no Banco**: `Users`

---

### 2.3 Warehouse (Armazém)

**Arquivo**: `Logistics.Domain/Entities/Warehouse.cs`

```csharp
public class Warehouse
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }         // Código único do armazém
    public string? Address { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation Properties
    public Company Company { get; private set; } = null!;
    public ICollection<StorageLocation> StorageLocations { get; private set; }
    public ICollection<WarehouseZone> Zones { get; private set; }
}
```

**Regras de Negócio**:
- ✅ CompanyId obrigatório
- ✅ Nome obrigatório
- ✅ Code obrigatório e único por empresa
- ✅ Cada empresa pode ter múltiplos armazéns

**Validações no Construtor**:
```csharp
if (companyId == Guid.Empty)
    throw new ArgumentException("CompanyId não pode ser vazio");
if (string.IsNullOrWhiteSpace(name))
    throw new ArgumentException("Nome não pode ser vazio");
if (string.IsNullOrWhiteSpace(code))
    throw new ArgumentException("Código não pode ser vazio");
```

**Métodos Públicos**:
- `Update(name, code, address)` - Atualiza dados
- `Activate()` / `Deactivate()` - Ativa/desativa

**Tabela no Banco**: `Warehouses`

---

### 2.4 WarehouseZone (Zona de Armazém)

**Arquivo**: `Logistics.Domain/Entities/WarehouseZone.cs`

```csharp
public class WarehouseZone
{
    public Guid Id { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string ZoneName { get; private set; }
    public ZoneType Type { get; private set; }       // Enum: Receiving, Storage, Picking, etc.
    public decimal? Temperature { get; private set; }
    public decimal? Humidity { get; private set; }
    public decimal TotalCapacity { get; private set; }
    public decimal UsedCapacity { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation Properties
    public Warehouse Warehouse { get; private set; } = null!;
    public ICollection<StorageLocation> StorageLocations { get; private set; }
}
```

**Tipos de Zona (ZoneType)**:
- `Receiving` - Área de recebimento
- `Storage` - Estocagem geral
- `Picking` - Área de separação
- `Packing` - Área de embalagem
- `Shipping` - Expedição
- `Staging` - Staging/Buffer
- `Returns` - Devoluções
- `Quarantine` - Quarentena
- `Refrigerated` - Refrigerado
- `Hazmat` - Materiais perigosos

**Regras de Negócio**:
- ✅ Um armazém pode ter várias zonas
- ✅ Cada zona tem tipo específico
- ✅ Controle de capacidade (total vs usada)
- ✅ Zonas refrigeradas têm temperatura e umidade

**Métodos Públicos**:
- `Update(zoneName, type, temperature, humidity, totalCapacity)` - Atualiza
- `UpdateUsedCapacity(usedCapacity)` - Atualiza capacidade usada
- `Activate()` / `Deactivate()` - Ativa/desativa

**Tabela no Banco**: `WarehouseZones`

---

### 2.5 StorageLocation (Localização de Armazenamento)

**Arquivo**: `Logistics.Domain/Entities/StorageLocation.cs`

```csharp
public class StorageLocation
{
    public Guid Id { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid? ZoneId { get; private set; }
    public string Code { get; private set; }         // Ex: A-01-2-B
    public string? Description { get; private set; }
    
    // Estrutura WMS
    public string? Aisle { get; private set; }       // Corredor: A, B, C
    public string? Rack { get; private set; }        // Rack: 01, 02, 03
    public string? Level { get; private set; }       // Nível: 1, 2, 3
    public string? Position { get; private set; }    // Posição: A, B, C
    public LocationType Type { get; private set; }   // Enum: Pallet, Shelf, etc.
    
    // Capacidade
    public decimal MaxWeight { get; private set; }
    public decimal MaxVolume { get; private set; }
    public decimal CurrentWeight { get; private set; }
    public decimal CurrentVolume { get; private set; }
    public bool IsBlocked { get; private set; }
    public string? BlockReason { get; private set; }
    
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation Properties
    public Warehouse Warehouse { get; private set; } = null!;
    public WarehouseZone? Zone { get; private set; }
    public ICollection<Inventory> Inventories { get; private set; }
}
```

**Tipos de Localização (LocationType)**:
- `Pallet` - Posição de pallet
- `Shelf` - Prateleira
- `Floor` - Chão
- `Bulk` - A granel
- `Rack` - Rack vertical
- `Bin` - Gaveta/Bin
- `Drive_In` - Drive-in
- `Cantilever` - Cantilever

**Estrutura de Código**:
```
Formato: A-01-2-B
         │ │  │ │
         │ │  │ └─ Position (Posição)
         │ │  └─── Level (Nível)
         │ └────── Rack (Estante)
         └──────── Aisle (Corredor)
```

**Regras de Negócio**:
- ✅ Code único por armazém
- ✅ Controle de peso e volume máximo
- ✅ Controle de ocupação atual
- ✅ Pode ser bloqueado com motivo
- ✅ Pertence a uma zona (opcional)

**Métodos Públicos**:
- `Update(code, description)` - Atualiza dados
- `SetStructure(aisle, rack, level, position)` - Define estrutura
- `SetCapacity(maxWeight, maxVolume, type)` - Define capacidade
- `UpdateCurrentUsage(currentWeight, currentVolume)` - Atualiza uso
- `Block(reason)` - Bloqueia localização
- `Unblock()` - Desbloqueia
- `Activate()` / `Deactivate()` - Ativa/desativa

**Tabela no Banco**: `StorageLocations`

---

## 3. ENTIDADES DE CADASTRO

### 3.1 Product (Produto)

**Arquivo**: `Logistics.Domain/Entities/Product.cs`

```csharp
public class Product
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; }
    public string SKU { get; private set; }          // Código SKU único
    public string? Barcode { get; private set; }     // Código de barras
    public string? Description { get; private set; }
    public decimal Weight { get; private set; }
    public string? WeightUnit { get; private set; }  // kg, g, lb
    
    // Campos WMS
    public decimal Volume { get; private set; }
    public string? VolumeUnit { get; private set; }
    public decimal Length { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }
    public string? DimensionUnit { get; private set; } // cm, m, in
    public bool RequiresLotTracking { get; private set; }
    public bool RequiresSerialTracking { get; private set; }
    public bool IsPerishable { get; private set; }
    public int? ShelfLifeDays { get; private set; }
    public decimal? MinimumStock { get; private set; }
    public decimal? SafetyStock { get; private set; }
    public string? ABCClassification { get; private set; } // A, B, C
    
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation Property
    public Company Company { get; private set; } = null!;
    public ICollection<Inventory> Inventories { get; private set; }
    public ICollection<Lot> Lots { get; private set; }
}
```

**Regras de Negócio**:
- ✅ SKU único por empresa
- ✅ Nome obrigatório
- ✅ Pode ter rastreamento por lote
- ✅ Pode ter rastreamento por número de série
- ✅ Produtos perecíveis têm shelf life
- ✅ Classificação ABC para gestão

**Métodos Públicos**:
- `Update(name, sku, barcode, description, weight, weightUnit)` - Atualiza básicos
- `UpdateWMSProperties(...)` - Atualiza propriedades WMS
- `Activate()` / `Deactivate()` - Ativa/desativa

**Tabela no Banco**: `Products`

---

### 3.2 Customer (Cliente)

**Arquivo**: `Logistics.Domain/Entities/Customer.cs`

```csharp
public class Customer
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; }
    public string? Document { get; private set; }    // CPF/CNPJ
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation Property
    public Company Company { get; private set; } = null!;
    public ICollection<Order> Orders { get; private set; }
}
```

**Tabela no Banco**: `Customers`

---

### 3.3 Supplier (Fornecedor)

**Arquivo**: `Logistics.Domain/Entities/Supplier.cs`

```csharp
public class Supplier
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; }
    public string? Document { get; private set; }    // CNPJ
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation Property
    public Company Company { get; private set; } = null!;
    public ICollection<InboundShipment> InboundShipments { get; private set; }
}
```

**Tabela no Banco**: `Suppliers`

---

### 3.4 Vehicle (Veículo)

**Arquivo**: `Logistics.Domain/Entities/Vehicle.cs`

```csharp
public class Vehicle
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string PlateNumber { get; private set; }  // Placa
    public string? Model { get; private set; }
    public string? Brand { get; private set; }
    public int? Year { get; private set; }
    public decimal? Capacity { get; private set; }   // Capacidade em kg
    public VehicleStatus Status { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation Property
    public Company Company { get; private set; } = null!;
}
```

**Status do Veículo**:
- `Available` - Disponível
- `InTransit` - Em trânsito
- `UnderMaintenance` - Em manutenção
- `OutOfService` - Fora de serviço

**Tabela no Banco**: `Vehicles`

---

### 3.5 Driver (Motorista)

**Arquivo**: `Logistics.Domain/Entities/Driver.cs`

```csharp
public class Driver
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; }
    public string? Document { get; private set; }    // CPF/CNH
    public string? Phone { get; private set; }
    public string? LicenseNumber { get; private set; }
    public DateTime? LicenseExpiry { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation Property
    public Company Company { get; private set; } = null!;
}
```

**Tabela no Banco**: `Drivers`

---

## 4. ENTIDADES WMS - INBOUND

### 4.1 Order (Pedido)

**Arquivo**: `Logistics.Domain/Entities/Order.cs`

```csharp
public class Order
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string OrderNumber { get; private set; }
    public OrderType Type { get; private set; }       // Inbound, Outbound, Transfer, Return
    public OrderSource Source { get; private set; }   // Manual, ERP, Ecommerce, EDI
    public Guid? CustomerId { get; private set; }
    public Guid? SupplierId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? ExpectedDate { get; private set; }
    public OrderPriority Priority { get; private set; } // Low, Normal, High, Urgent
    public OrderStatus Status { get; private set; }    // Draft, Pending, Confirmed, etc.
    public decimal TotalQuantity { get; private set; }
    public decimal TotalValue { get; private set; }
    public string? ShippingAddress { get; private set; }
    public string? SpecialInstructions { get; private set; }
    public bool IsBOPIS { get; private set; }         // Buy Online Pickup In Store
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation Properties
    public Company Company { get; private set; } = null!;
    public Customer? Customer { get; private set; }
    public Supplier? Supplier { get; private set; }
    public ICollection<OrderItem> Items { get; private set; }
}
```

**Tipos de Pedido**:
- `Inbound` - Entrada (recebimento de fornecedor)
- `Outbound` - Saída (venda para cliente)
- `Transfer` - Transferência entre armazéns
- `Return` - Devolução

**Status do Pedido**:
- `Draft` - Rascunho
- `Pending` - Pendente
- `Confirmed` - Confirmado
- `InProgress` - Em progresso
- `PartiallyFulfilled` - Parcialmente atendido
- `Fulfilled` - Atendido
- `Shipped` - Enviado
- `Delivered` - Entregue
- `Cancelled` - Cancelado
- `OnHold` - Em espera

**Tabela no Banco**: `Orders`

---

### 4.2 OrderItem (Item de Pedido)

**Arquivo**: `Logistics.Domain/Entities/OrderItem.cs`

```csharp
public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string SKU { get; private set; }
    public decimal QuantityOrdered { get; private set; }
    public decimal QuantityAllocated { get; private set; }
    public decimal QuantityPicked { get; private set; }
    public decimal QuantityShipped { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string? RequiredLotNumber { get; private set; }
    public DateTime? RequiredShipDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation Properties
    public Order Order { get; private set; } = null!;
    public Product Product { get; private set; } = null!;
}
```

**Regras de Negócio**:
- ✅ QuantityOrdered é a quantidade solicitada
- ✅ QuantityAllocated é a quantidade reservada no estoque
- ✅ QuantityPicked é a quantidade separada
- ✅ QuantityShipped é a quantidade enviada
- ✅ Pode ter lote específico requerido
- ✅ Pode ter data de envio específica

**Tabela no Banco**: `OrderItems`

---

### 4.3 InboundShipment (Remessa de Entrada)

**Arquivo**: `Logistics.Domain/Entities/InboundShipment.cs`

```csharp
public class InboundShipment
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string ShipmentNumber { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid? VehicleId { get; private set; }
    public Guid? DriverId { get; private set; }
    public DateTime? ExpectedArrivalDate { get; private set; }
    public DateTime? ActualArrivalDate { get; private set; }
    public string? DockDoorNumber { get; private set; }
    public InboundStatus Status { get; private set; }  // Scheduled, InProgress, Completed, Cancelled
    public decimal TotalQuantityExpected { get; private set; }
    public decimal TotalQuantityReceived { get; private set; }
    public string? ASNNumber { get; private set; }     // Advanced Shipping Notice
    public bool HasQualityIssues { get; private set; }
    public Guid? InspectedBy { get; private set; }
    public Guid? ReceivedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation Properties
    public Company Company { get; private set; } = null!;
    public Order Order { get; private set; } = null!;
    public Supplier Supplier { get; private set; } = null!;
    public Vehicle? Vehicle { get; private set; }
    public Driver? Driver { get; private set; }
    public ICollection<Receipt> Receipts { get; private set; }
}
```

**Fluxo do Inbound**:
```
1. Scheduled (Agendado)
   ↓
2. InProgress (Chegou e está sendo recebido)
   ↓
3. Completed (Recebimento completo)
```

**Tabela no Banco**: `InboundShipments`

---

Continua no próximo arquivo...
