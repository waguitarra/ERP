# SISTEMA WMS - WAREHOUSE MANAGEMENT SYSTEM
# Especificação Técnica Completa - Baseada em WMS Reais

**Versão**: 2.0 - UNIFICADA  
**Data**: 2025-11-21  
**Status**: ✅ Completo para Pequenas, Médias e Grandes Empresas

---

## 📋 ÍNDICE

1. [Visão Geral](#visão-geral)
2. [Setup Inicial do Sistema](#setup-inicial)
3. [Módulos Core do WMS](#módulos-core)
4. [Processo Inbound (Entrada)](#inbound)
5. [Processo Outbound (Saída)](#outbound)
6. [Gestão de Portaria e Caminhões](#portaria)
7. [Sistema de Usuários e Permissões](#usuarios)
8. [Rastreabilidade e Auditoria](#rastreabilidade)
9. [Módulos Avançados](#avancados)
10. [Escalabilidade: Pequena vs Grande Empresa](#escalabilidade)
11. [Roadmap de Implementação](#roadmap)

---

## 🎯 VISÃO GERAL

### O que é WMS?

**WMS (Warehouse Management System)** é um sistema que gerencia todas as operações de um armazém/galpão:

✅ **Recebimento** - Entrada de mercadorias  
✅ **Armazenagem** - Putaway otimizado  
✅ **Inventário** - Controle em tempo real  
✅ **Separação** - Picking de pedidos  
✅ **Expedição** - Shipping e tracking  
✅ **Rastreabilidade** - Do fornecedor ao cliente  
✅ **Analytics** - KPIs e relatórios

### Problema que Resolve

**Empresas Pequenas**:
- Controle manual de estoque propenso a erros
- Dificuldade em localizar produtos rapidamente
- Falta de rastreabilidade
- Processos lentos e ineficientes

**Empresas Grandes**:
- Múltiplos galpões e centros de distribuição
- Alto volume de movimentação diária
- Necessidade de integração com ERP/TMS
- Compliance e auditoria rigorosos
- Gestão complexa de equipes

---

## 🚀 SETUP INICIAL DO SISTEMA

### IMPORTANTE: Primeiro Acesso NÃO é apenas ADMIN

**Baseado em WMS reais do mercado**, o setup inicial segue este fluxo:

### 1. INSTALAÇÃO DO SISTEMA

```
PASSO 1: Deploy da aplicação
- Backend API rodando
- Banco de dados criado
- Migrations executadas

PASSO 2: Seed Inicial Automático
Sistema cria automaticamente:
- Role "SuperAdmin" (sistema)
- Role "CompanyAdmin" (sistema)
- Primeiro usuário: setup@system.local
  * Email temporário
  * Senha temporária gerada
  * Role: SuperAdmin
  * Status: MustChangePassword = true
```

### 2. PRIMEIRO ACESSO - Setup Wizard

```
┌──────────────────────────────────────────────────────┐
│ BEM-VINDO AO SISTEMA WMS                             │
│ Setup Inicial - Primeira Configuração                │
├──────────────────────────────────────────────────────┤
│                                                       │
│ PASSO 1: Dados do Administrador Principal            │
│ ────────────────────────────────────────────────────│
│ Nome Completo: [_______________________________]     │
│ Email: [_______________________________________]     │
│ Senha: [_______________________________________]     │
│ Confirmar Senha: [_____________________________]     │
│                                                       │
│ PASSO 2: Dados da Empresa Principal                  │
│ ────────────────────────────────────────────────────│
│ Nome da Empresa: [_____________________________]     │
│ CNPJ: [________________________________________]     │
│ Endereço: [____________________________________]     │
│ Telefone: [____________________________________]     │
│                                                       │
│ PASSO 3: Configuração do Primeiro Armazém            │
│ ────────────────────────────────────────────────────│
│ Nome do Armazém: [_____________________________]     │
│ Código: [______________________________________]     │
│ Tipo: [ ] Pequeno  [ ] Médio  [X] Grande            │
│                                                       │
│ [Anterior]              [Próximo] [Finalizar Setup] │
└──────────────────────────────────────────────────────┘
```

### 3. APÓS SETUP - Sistema Criado

```csharp
// O que o Setup Wizard cria automaticamente:

1. COMPANY (Primeira Empresa)
   - Nome, CNPJ, dados da empresa
   - IsActive = true
   - CreatedAt = agora

2. USER (Primeiro Admin Real)
   - Email fornecido
   - Senha criptografada
   - CompanyId = empresa criada
   - IsActive = true
   - MustChangePassword = false (já definiu)

3. USER_ROLE (Atribuição)
   - UserId = usuário criado
   - RoleId = CompanyAdmin
   - CompanyId = empresa criada
   - IsActive = true

4. WAREHOUSE (Primeiro Armazém)
   - Nome, código
   - CompanyId
   - Cria zonas padrão:
     * Receiving (Recebimento)
     * Storage (Estocagem)
     * Shipping (Expedição)
     * Returns (Devoluções)

5. STORAGE_LOCATIONS (Básicas)
   - RECV-001 (área de recebimento)
   - SHIP-001 (área de expedição)
   - A-01-1-A até A-01-3-C (primeiras posições)

6. DELETE user setup@system.local
   - Remove usuário temporário do sistema
```

### 4. HIERARQUIA DE USUÁRIOS

```
NÍVEL 1: SUPERADMIN (Apenas durante setup)
└── setup@system.local (deletado após configuração)

NÍVEL 2: COMPANY_ADMIN (Primeiro usuário real)
├── Criado no setup wizard
├── Administra TUDO da sua empresa
├── Pode criar outros CompanyAdmins
├── Pode criar WarehouseManagers
└── Pode criar usuários operacionais

NÍVEL 3: WAREHOUSE_MANAGER
├── Criado por CompanyAdmin
├── Gerencia armazém específico
└── Pode criar usuários operacionais do armazém

NÍVEL 4: USUÁRIOS OPERACIONAIS
└── Criados por CompanyAdmin ou WarehouseManager
```

### 5. ROLES PADRÃO DO SISTEMA

```csharp
public class DefaultRoles
{
    // Roles criados automaticamente no seed
    public static readonly Role[] SystemRoles = 
    {
        // NÍVEL EMPRESA
        new Role 
        { 
            Name = "CompanyAdmin",
            Description = "Administrador da Empresa",
            Level = RoleLevel.Company,
            IsSystemRole = true,  // Não pode ser deletado
            Permissions = [
                "Company.*",      // Tudo da empresa
                "Warehouse.*",
                "User.Create", "User.Read", "User.Update",
                "Order.*",
                "Inventory.*",
                "Report.*"
            ]
        },
        
        // NÍVEL ARMAZÉM
        new Role 
        { 
            Name = "WarehouseManager",
            Description = "Gerente de Armazém",
            Level = RoleLevel.Warehouse,
            IsSystemRole = true,
            Permissions = [
                "Warehouse.Read", "Warehouse.Update",
                "Order.*",
                "Task.Create", "Task.Assign",
                "Inventory.Read", "Inventory.Adjust",
                "Report.Read"
            ]
        },
        
        // RECEBIMENTO
        new Role 
        { 
            Name = "ReceivingClerk",
            Description = "Auxiliar de Recebimento",
            Level = RoleLevel.Operational,
            IsSystemRole = true,
            Permissions = [
                "InboundShipment.Read", "InboundShipment.Update",
                "Receipt.Create", "Receipt.Update",
                "PutawayTask.Create"
            ]
        },
        
        // QUALIDADE
        new Role 
        { 
            Name = "QualityControl",
            Description = "Controle de Qualidade",
            Level = RoleLevel.Operational,
            IsSystemRole = true,
            Permissions = [
                "Receipt.Read", "Receipt.Approve",
                "Product.Read",
                "Quarantine.*"
            ]
        },
        
        // ARMAZENAGEM
        new Role 
        { 
            Name = "PutawayOperator",
            Description = "Operador de Armazenagem",
            Level = RoleLevel.Operational,
            IsSystemRole = true,
            Permissions = [
                "PutawayTask.Read", "PutawayTask.Execute",
                "StorageLocation.Update"
            ]
        },
        
        // SEPARAÇÃO
        new Role 
        { 
            Name = "Picker",
            Description = "Separador de Pedidos",
            Level = RoleLevel.Operational,
            IsSystemRole = true,
            Permissions = [
                "PickingTask.Read", "PickingTask.Execute",
                "Product.Read"
            ]
        },
        
        // EMBALAGEM
        new Role 
        { 
            Name = "Packer",
            Description = "Embalador",
            Level = RoleLevel.Operational,
            IsSystemRole = true,
            Permissions = [
                "PackingTask.Read", "PackingTask.Execute",
                "Label.Print"
            ]
        },
        
        // EXPEDIÇÃO
        new Role 
        { 
            Name = "ShippingClerk",
            Description = "Auxiliar de Expedição",
            Level = RoleLevel.Operational,
            IsSystemRole = true,
            Permissions = [
                "OutboundShipment.Read", "OutboundShipment.Ship",
                "BOL.Create"
            ]
        },
        
        // INVENTÁRIO
        new Role 
        { 
            Name = "InventoryController",
            Description = "Controlador de Estoque",
            Level = RoleLevel.Operational,
            IsSystemRole = true,
            Permissions = [
                "CycleCount.*",
                "Inventory.Read", "Inventory.Adjust"
            ]
        },
        
        // PORTARIA
        new Role 
        { 
            Name = "SecurityGuard",
            Description = "Segurança/Portaria",
            Level = RoleLevel.External,
            IsSystemRole = true,
            Permissions = [
                "GateEntry.Create", "GateEntry.Update",
                "Vehicle.Read",
                "Driver.Read"
            ]
        },
        
        // MOTORISTA
        new Role 
        { 
            Name = "Driver",
            Description = "Motorista",
            Level = RoleLevel.External,
            IsSystemRole = true,
            Permissions = [
                "VehicleAppointment.Read.Own",
                "GateEntry.Read.Own",
                "BOL.Read.Own", "BOL.Sign.Own"
            ]
        },
        
        // VISUALIZADOR
        new Role 
        { 
            Name = "Viewer",
            Description = "Visualizador (Somente Leitura)",
            Level = RoleLevel.ReadOnly,
            IsSystemRole = true,
            Permissions = [
                "*.Read",  // Read em tudo
                "Report.Generate"
            ]
        }
    };
}
```

### 6. COMPANY_ADMIN - Primeiro Usuário Real

```
CAPACIDADES DO COMPANY_ADMIN:

✅ Gerenciar Empresa
   - Editar dados da empresa
   - Adicionar/editar armazéns
   - Configurar zonas e localizações

✅ Gerenciar Usuários
   - Criar novos usuários
   - Atribuir roles
   - Definir permissões específicas
   - Pode criar outros CompanyAdmins
   - Pode criar WarehouseManagers
   - Pode criar todos os roles operacionais

✅ Gerenciar Operações
   - Ver todos pedidos
   - Aprovar ajustes de inventário
   - Acessar todos relatórios
   - Configurar integrações

✅ Gerenciar Sistema (dentro da empresa)
   - Configurar workflows
   - Definir regras de negócio
   - Customizar campos
   - Gerenciar alertas

❌ NÃO PODE (escopo global)
   - Acessar outras empresas
   - Modificar roles do sistema
   - Acessar configurações globais
```

---

## 📦 MÓDULOS CORE DO WMS

Baseado em **WMS reais do mercado**, estes são os módulos essenciais:

### MÓDULO 1: RECEIVING (Recebimento)

**Funcionalidades Essenciais**:
- ✅ Appointment Scheduling (Agendamento de entregas)
- ✅ Receipt Orders (múltiplos tipos: PO, Transfer, Return)
- ✅ ASN (Advanced Shipping Notice) - notificação prévia
- ✅ Quality Inspection (Inspeção de qualidade)
- ✅ Label Generation (Geração de etiquetas)
- ✅ Barcode Scanning (Leitura de códigos de barras)
- ✅ Partial/Full Receipt (Recebimento parcial ou total)
- ✅ Reverse/Void Receipts (Estorno de recebimentos)
- ✅ Cross-docking (Transferência direta)
- ✅ Returns Management (Gestão de devoluções)

**Entidades**:
```csharp
public class InboundShipment
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string ShipmentNumber { get; set; }
    public Guid OrderId { get; set; }
    public Guid SupplierId { get; set; }
    public Guid? VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public DateTime ExpectedArrivalDate { get; set; }
    public DateTime? ActualArrivalDate { get; set; }
    public string DockDoorNumber { get; set; }
    public InboundStatus Status { get; set; }
    public decimal TotalQuantityExpected { get; set; }
    public decimal TotalQuantityReceived { get; set; }
    public string ASNNumber { get; set; }  // Advanced Shipping Notice
    public bool HasQualityIssues { get; set; }
    public Guid? InspectedBy { get; set; }
    public Guid ReceivedBy { get; set; }
}

public class Receipt
{
    public Guid Id { get; set; }
    public string ReceiptNumber { get; set; }  // GRN
    public Guid InboundShipmentId { get; set; }
    public DateTime ReceiptDate { get; set; }
    public ReceiptStatus Status { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ReceivedBy { get; set; }
    public ICollection<ReceiptLine> Lines { get; set; }
}

public class ReceiptLine
{
    public Guid Id { get; set; }
    public Guid ReceiptId { get; set; }
    public Guid ProductId { get; set; }
    public string SKU { get; set; }
    public string LotNumber { get; set; }
    public string SerialNumber { get; set; }
    public decimal QuantityExpected { get; set; }
    public decimal QuantityReceived { get; set; }
    public decimal QuantityDamaged { get; set; }
    public InspectionStatus InspectionStatus { get; set; }
    public string QualityNotes { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
```

---

### MÓDULO 2: STORAGE (Armazenagem - Put-away)

**Funcionalidades Essenciais**:
- ✅ Directed Put-away (Armazenagem direcionada automática)
- ✅ Dynamic Slotting (Alocação dinâmica de espaço)
- ✅ Multi-location Management (Múltiplos armazéns)
- ✅ Zone Management (Gestão de zonas)
- ✅ Bin/Rack/Aisle Mapping (Mapeamento de posições)
- ✅ Capacity Management (Gestão de capacidade)
- ✅ Location Types (Tipos: Pallet, Shelf, Floor, Bulk)
- ✅ Temperature/Humidity Zones (Zonas controladas)
- ✅ Task Interleaving (Otimização de tarefas)
- ✅ Mobile Scanner Support (Suporte a scanners móveis)

**Entidades**:
```csharp
public class WarehouseZone
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string ZoneName { get; set; }
    public ZoneType Type { get; set; }  // Receiving, Storage, Shipping, Quarantine, Refrigerated
    public decimal? Temperature { get; set; }
    public decimal? Humidity { get; set; }
    public decimal TotalCapacity { get; set; }
    public decimal UsedCapacity { get; set; }
    public bool IsActive { get; set; }
}

public class StorageLocation  // Expandir existente
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ZoneId { get; set; }
    public string Code { get; set; }  // Ex: A-01-2-B
    public string Aisle { get; set; }  // A, B, C
    public string Rack { get; set; }   // 01, 02, 03
    public string Level { get; set; }  // 1, 2, 3
    public string Position { get; set; } // A, B, C
    public LocationType Type { get; set; }
    public decimal MaxWeight { get; set; }
    public decimal MaxVolume { get; set; }
    public decimal CurrentWeight { get; set; }
    public decimal CurrentVolume { get; set; }
    public bool IsBlocked { get; set; }
    public string BlockReason { get; set; }
}

public class PutawayTask
{
    public Guid Id { get; set; }
    public string TaskNumber { get; set; }
    public Guid ReceiptId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? LotId { get; set; }
    public decimal Quantity { get; set; }
    public Guid FromLocationId { get; set; }  // Staging area
    public Guid ToLocationId { get; set; }    // Final location
    public TaskPriority Priority { get; set; }
    public TaskStatus Status { get; set; }
    public Guid? AssignedTo { get; set; }
    public DateTime? CompletedAt { get; set; }
}
```

---

### MÓDULO 3: INVENTORY MANAGEMENT (Controle de Inventário)

**Funcionalidades Essenciais**:
- ✅ Real-time Inventory Tracking (Rastreamento em tempo real)
- ✅ Multi-location Inventory (Estoque em múltiplas localizações)
- ✅ Available vs Reserved vs On-Hand (Disponível, Reservado, Físico)
- ✅ Lot Tracking (Rastreamento por lote)
- ✅ Serial Number Tracking (Rastreamento por serial)
- ✅ Expiry Date Management (Gestão de validade)
- ✅ FIFO/FEFO/LIFO Support (Métodos de rotação)
- ✅ Cycle Counting (Contagem cíclica)
- ✅ ABC Analysis (Classificação ABC)
- ✅ Reorder Points (Pontos de reposição)
- ✅ Safety Stock (Estoque de segurança)
- ✅ Inventory Adjustments (Ajustes manuais)
- ✅ Stock Alerts (Alertas de estoque)

**Entidades**:
```csharp
public class Inventory
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid StorageLocationId { get; set; }
    public Guid? LotId { get; set; }
    public decimal QuantityOnHand { get; set; }      // Físico total
    public decimal QuantityAvailable { get; set; }   // Disponível para venda
    public decimal QuantityReserved { get; set; }    // Reservado em pedidos
    public decimal QuantityAllocated { get; set; }   // Alocado para picking
    public decimal QuantityDamaged { get; set; }     // Danificado
    public decimal QuantityQuarantine { get; set; }  // Em quarentena
    public DateTime LastCountDate { get; set; }
    public DateTime? LastMovementDate { get; set; }
}

public class Lot
{
    public Guid Id { get; set; }
    public string LotNumber { get; set; }
    public Guid ProductId { get; set; }
    public DateTime ManufactureDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public decimal QuantityReceived { get; set; }
    public decimal QuantityAvailable { get; set; }
    public LotStatus Status { get; set; }  // Available, Quarantine, Expired
    public Guid? SupplierId { get; set; }
}

public class SerialNumber
{
    public Guid Id { get; set; }
    public string Serial { get; set; }  // Único
    public Guid ProductId { get; set; }
    public Guid? LotId { get; set; }
    public SerialStatus Status { get; set; }
    public Guid CurrentLocationId { get; set; }
    public DateTime ReceiptDate { get; set; }
    public DateTime? ShippedDate { get; set; }
}

public class CycleCount
{
    public Guid Id { get; set; }
    public string CountNumber { get; set; }
    public Guid WarehouseId { get; set; }
    public CountType Type { get; set; }  // Full, ABC, Random, Spot
    public DateTime ScheduledDate { get; set; }
    public CountStatus Status { get; set; }
    public Guid CountedBy { get; set; }
    public ICollection<CycleCountLine> Lines { get; set; }
}

public class InventoryAdjustment
{
    public Guid Id { get; set; }
    public string AdjustmentNumber { get; set; }
    public Guid ProductId { get; set; }
    public Guid StorageLocationId { get; set; }
    public decimal QuantityBefore { get; set; }
    public decimal QuantityAfter { get; set; }
    public decimal VarianceQuantity { get; set; }
    public AdjustmentReason Reason { get; set; }  // CycleCount, Damage, Loss, Found
    public string Notes { get; set; }
    public Guid AdjustedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
}
```

---

### MÓDULO 4: ORDER MANAGEMENT (Gestão de Pedidos)

**Funcionalidades Essenciais**:
- ✅ Multi-channel Orders (Pedidos de múltiplos canais)
- ✅ Order Types (Inbound/Outbound/Transfer/Return)
- ✅ Order Allocation (Alocação automática de estoque)
- ✅ Order Prioritization (Priorização de pedidos)
- ✅ Backorder Management (Gestão de pendências)
- ✅ Order Consolidation (Consolidação de pedidos)
- ✅ Order Splitting (Divisão de pedidos)
- ✅ ERP/E-commerce Integration (Integração com vendas)
- ✅ Order Tracking (Rastreamento de status)
- ✅ BOPIS Support (Buy Online Pick-up In Store)

**Entidades**:
```csharp
public class Order
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string OrderNumber { get; set; }
    public OrderType Type { get; set; }  // Inbound, Outbound, Transfer
    public OrderSource Source { get; set; }  // Manual, ERP, Ecommerce, API
    public Guid? CustomerId { get; set; }
    public Guid? SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ExpectedDate { get; set; }
    public OrderPriority Priority { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
    public string ShippingAddress { get; set; }
    public string SpecialInstructions { get; set; }
    public bool IsBOPIS { get; set; }  // Buy Online Pickup In Store
    public Guid CreatedBy { get; set; }
    public ICollection<OrderItem> Items { get; set; }
}

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string SKU { get; set; }
    public decimal QuantityOrdered { get; set; }
    public decimal QuantityAllocated { get; set; }
    public decimal QuantityPicked { get; set; }
    public decimal QuantityShipped { get; set; }
    public decimal UnitPrice { get; set; }
    public string RequiredLotNumber { get; set; }  // Se cliente especificar
    public DateTime? RequiredShipDate { get; set; }
}
```

---

### MÓDULO 5: PICKING (Separação de Pedidos)

**Funcionalidades Essenciais**:
- ✅ Multiple Picking Strategies (Single, Batch, Zone, Wave)
- ✅ Pick-to-Order (Separação por pedido)
- ✅ Pick-to-Cart/Tote (Separação em carrinho)
- ✅ Wave Planning (Planejamento de ondas)
- ✅ Optimized Pick Paths (Rotas otimizadas)
- ✅ Pick List Generation (Geração de listas)
- ✅ Mobile Picking (Picking via mobile)
- ✅ Voice Picking (Picking por voz - futuro)
- ✅ Pick Verification (Verificação de separação)
- ✅ Short Pick Handling (Gestão de falta de produto)
- ✅ Pick-face Replenishment (Reposição de áreas de picking)

**Entidades**:
```csharp
public class PickingWave
{
    public Guid Id { get; set; }
    public string WaveNumber { get; set; }
    public Guid WarehouseId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public WaveStatus Status { get; set; }
    public string Criteria { get; set; }  // JSON: rota, prioridade, zona
    public int TotalOrders { get; set; }
    public Guid CreatedBy { get; set; }
    public ICollection<PickingTask> Tasks { get; set; }
}

public class PickingTask
{
    public Guid Id { get; set; }
    public string TaskNumber { get; set; }
    public Guid OutboundShipmentId { get; set; }
    public Guid? WaveId { get; set; }
    public PickingMethod Method { get; set; }  // Single, Batch, Zone, Wave
    public TaskPriority Priority { get; set; }
    public TaskStatus Status { get; set; }
    public Guid? AssignedTo { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public ICollection<PickingLine> Lines { get; set; }
}

public class PickingLine
{
    public Guid Id { get; set; }
    public Guid PickingTaskId { get; set; }
    public Guid OrderItemId { get; set; }
    public Guid ProductId { get; set; }
    public Guid StorageLocationId { get; set; }
    public Guid? LotId { get; set; }
    public string SerialNumber { get; set; }
    public decimal QuantityRequired { get; set; }
    public decimal QuantityPicked { get; set; }
    public int SequenceNumber { get; set; }  // Ordem de picking
    public PickLineStatus Status { get; set; }  // Pending, Picked, ShortPicked
    public Guid? PickedBy { get; set; }
    public DateTime? PickedAt { get; set; }
}

public class Replenishment
{
    public Guid Id { get; set; }
    public string ReplenishmentNumber { get; set; }
    public Guid ProductId { get; set; }
    public Guid FromLocationId { get; set; }  // Estoque reserva
    public Guid ToLocationId { get; set; }    // Pick-face
    public decimal Quantity { get; set; }
    public ReplenishmentStatus Status { get; set; }
    public Guid? AssignedTo { get; set; }
}
```

---

### MÓDULO 6: PACKING & SHIPPING (Embalagem e Expedição)

**Funcionalidades Essenciais**:
- ✅ Packing Station Management (Gestão de estações)
- ✅ Pack Verification (Verificação de embalagem)
- ✅ Multi-package Orders (Pedidos em múltiplos pacotes)
- ✅ Package Type Selection (Seleção de embalagem)
- ✅ Weight/Dimension Capture (Captura de peso/dimensões)
- ✅ Carrier Integration (Integração com transportadoras)
- ✅ Label Printing (Impressão de etiquetas)
- ✅ Packing Slip Generation (Geração de romaneio)
- ✅ BOL Generation (Bill of Lading)
- ✅ Tracking Number Assignment (Atribuição de rastreio)
- ✅ Manifest Creation (Criação de manifesto)
- ✅ Loading Verification (Verificação de carga)

**Entidades**:
```csharp
public class PackingTask
{
    public Guid Id { get; set; }
    public string TaskNumber { get; set; }
    public Guid OutboundShipmentId { get; set; }
    public Guid PickingTaskId { get; set; }
    public PackingStationId { get; set; }
    public TaskStatus Status { get; set; }
    public Guid? PackedBy { get; set; }
    public DateTime? PackedAt { get; set; }
    public ICollection<Package> Packages { get; set; }
}

public class Package
{
    public Guid Id { get; set; }
    public Guid PackingTaskId { get; set; }
    public string PackageNumber { get; set; }
    public PackageType Type { get; set; }  // Box, Pallet, Envelope
    public decimal Weight { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public string TrackingNumber { get; set; }
    public string CarrierCode { get; set; }
    public string ServiceLevel { get; set; }  // Standard, Express, etc
    public bool LabelPrinted { get; set; }
    public ICollection<PackageItem> Items { get; set; }
}

public class OutboundShipment
{
    public Guid Id { get; set; }
    public string ShipmentNumber { get; set; }
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public DateTime ScheduledDepartureDate { get; set; }
    public DateTime? ActualDepartureDate { get; set; }
    public string DockDoorNumber { get; set; }
    public OutboundStatus Status { get; set; }
    public string BOLNumber { get; set; }
    public string ManifestNumber { get; set; }
    public decimal TotalWeight { get; set; }
    public int TotalPackages { get; set; }
    public Guid? ShippedBy { get; set; }
}
```

---

### MÓDULO 7: BARCODE & DATA CAPTURE (Códigos de Barras)

**Funcionalidades Essenciais**:
- ✅ Barcode Generation (Geração de códigos)
- ✅ Barcode Scanning (Leitura via scanner)
- ✅ QR Code Support (Suporte a QR Code)
- ✅ RFID Support (Suporte RFID - futuro)
- ✅ Label Templates (Templates de etiquetas)
- ✅ Batch Label Printing (Impressão em lote)
- ✅ Mobile Scanner Apps (Apps para scanners móveis)
- ✅ Verification Scanning (Scan para verificação)
- ✅ Product/Location/Lot/Serial Barcodes

**Entidades**:
```csharp
public class BarcodeConfig
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public BarcodeType Type { get; set; }  // Product, Location, Lot, Serial, Package
    public string Format { get; set; }  // CODE128, EAN13, QR, etc
    public string Prefix { get; set; }
    public int Length { get; set; }
    public bool IncludeCheckDigit { get; set; }
    public string LabelTemplate { get; set; }  // HTML/ZPL template
}

public class ScanLog
{
    public Guid Id { get; set; }
    public string BarcodeValue { get; set; }
    public BarcodeType Type { get; set; }
    public Guid EntityId { get; set; }  // ProductId, LocationId, etc
    public ScanAction Action { get; set; }  // Receive, Putaway, Pick, Pack, Ship
    public Guid ScannedBy { get; set; }
    public DateTime ScannedAt { get; set; }
    public string DeviceId { get; set; }
}
```

---

### MÓDULO 8: DOCK & YARD MANAGEMENT (Gestão de Docas)

**Funcionalidades Essenciais**:
- ✅ Appointment Scheduling (Agendamento de veículos)
- ✅ Dock Door Assignment (Atribuição de docas)
- ✅ Time Slot Management (Gestão de janelas de tempo)
- ✅ Check-in/Check-out (Controle de entrada/saída)
- ✅ Yard Location Tracking (Rastreamento no pátio)
- ✅ Dwell Time Monitoring (Monitoramento de tempo)
- ✅ Gate Entry/Exit Logging (Registro de portaria)
- ✅ Seal Number Tracking (Rastreamento de lacres)
- ✅ Driver Management (Gestão de motoristas)
- ✅ Carrier Performance Tracking

**Entidades**:
```csharp
public class VehicleAppointment
{
    public Guid Id { get; set; }
    public string AppointmentNumber { get; set; }
    public Guid VehicleId { get; set; }
    public Guid DriverId { get; set; }
    public AppointmentType Type { get; set; }  // Inbound, Outbound
    public Guid? ShipmentId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string TimeSlot { get; set; }  // "08:00-10:00"
    public string DockDoorNumber { get; set; }
    public AppointmentStatus Status { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public TimeSpan? WaitingTime { get; set; }
    public TimeSpan? ServiceTime { get; set; }
}

public class DockDoor
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string DoorNumber { get; set; }
    public DockType Type { get; set; }  // Inbound, Outbound, Both
    public DoorStatus Status { get; set; }
    public Guid? CurrentAppointmentId { get; set; }
    public bool IsActive { get; set; }
}

public class GateEntry
{
    public Guid Id { get; set; }
    public string EntryNumber { get; set; }
    public Guid VehicleId { get; set; }
    public Guid DriverId { get; set; }
    public Guid? AppointmentId { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public Guid SecurityGuard { get; set; }
    public bool DocumentsVerified { get; set; }
    public string SealNumber { get; set; }
    public string InvoiceNumber { get; set; }
}
```

---

### MÓDULO 9: ANALYTICS & REPORTING (Análises e Relatórios)

**Funcionalidades Essenciais**:
- ✅ Real-time Dashboards (Dashboards em tempo real)
- ✅ KPI Tracking (Rastreamento de KPIs)
- ✅ Performance Metrics (Métricas de performance)
- ✅ Inventory Reports (Relatórios de estoque)
- ✅ Order Fulfillment Reports (Relatórios de fulfillment)
- ✅ Labor Productivity Reports (Produtividade da equipe)
- ✅ ABC Analysis Reports (Análise ABC)
- ✅ Variance Analysis (Análise de variâncias)
- ✅ Custom Report Builder (Construtor de relatórios)
- ✅ Scheduled Reports (Relatórios agendados)
- ✅ Export Capabilities (Excel, PDF, CSV)

**KPIs Principais**:
```
INBOUND:
- Receipt Accuracy: > 99%
- Receiving Time: < 30 min
- Putaway Time: < 2 horas
- Dock Utilization: 70-80%

OUTBOUND:
- Pick Accuracy: > 99.5%
- Order Fill Rate: > 95%
- On-time Shipment: > 95%
- Perfect Order Rate: > 98%

INVENTÁRIO:
- Inventory Accuracy: > 99.8%
- Stock Turnover: conforme categoria
- Cycle Count Variance: < 1%
- Obsolete Inventory: < 2%

PRODUTIVIDADE:
- Lines per Hour: meta por função
- Order Cycle Time: < 24h
- Labor Utilization: > 85%
- Cost per Order: otimizar
```

**Entidades**:
```csharp
public class Dashboard
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DashboardType Type { get; set; }  // Executive, Operational, User
    public string Config { get; set; }  // JSON com widgets
    public Guid? UserId { get; set; }
    public bool IsDefault { get; set; }
}

public class KPI
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal TargetValue { get; set; }
    public string Unit { get; set; }
    public DateTime CalculatedAt { get; set; }
}

public class Report
{
    public Guid Id { get; set; }
    public string ReportName { get; set; }
    public ReportType Type { get; set; }
    public string Parameters { get; set; }  // JSON
    public DateTime GeneratedAt { get; set; }
    public Guid GeneratedBy { get; set; }
    public string FileUrl { get; set; }
}
```

---

### MÓDULO 10: INTEGRATION (Integrações)

**Funcionalidades Essenciais**:
- ✅ ERP Integration (Integração com ERP)
- ✅ E-commerce Integration (Integração com lojas online)
- ✅ TMS Integration (Sistema de Transporte)
- ✅ Carrier APIs (APIs de transportadoras)
- ✅ EDI Support (Electronic Data Interchange)
- ✅ API REST/SOAP (APIs de integração)
- ✅ Webhooks (Notificações automáticas)
- ✅ Real-time Sync (Sincronização em tempo real)
- ✅ Batch Processing (Processamento em lote)

**Integrações Comuns**:
```
1. ERP (SAP, Oracle, Microsoft Dynamics)
   - Sincronização de pedidos
   - Atualização de estoque
   - Dados de produtos
   - Dados de clientes/fornecedores

2. E-COMMERCE (Shopify, WooCommerce, Magento)
   - Importação de pedidos
   - Atualização de estoque disponível
   - Tracking de envios
   - BOPIS (Buy Online Pickup In Store)

3. TRANSPORTADORAS (Correios, Fedex, UPS, DHL)
   - Cálculo de frete
   - Geração de etiquetas
   - Rastreamento
   - Agendamento de coletas

4. FISCAL (NFe, SAT)
   - Emissão de notas fiscais
   - Validação de documentos
   - Compliance tributário
```

**Entidades**:
```csharp
public class Integration
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IntegrationType Type { get; set; }
    public string Provider { get; set; }
    public string Config { get; set; }  // JSON com credenciais
    public bool IsActive { get; set; }
    public DateTime? LastSyncAt { get; set; }
}

public class IntegrationLog
{
    public Guid Id { get; set; }
    public Guid IntegrationId { get; set; }
    public string Operation { get; set; }
    public string Request { get; set; }
    public string Response { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

## 🔍 RASTREABILIDADE E AUDITORIA

### Rastreabilidade Bidirecional

**FORWARD TRACING** (Fornecedor → Cliente):
```
Produto Serial: ABC123

1. RECEBIMENTO
   Data: 2025-11-20 09:15
   Fornecedor: ACME Corp
   Receipt: GRN-001
   Lote: LOT-2025-11-15
   Quantidade: 1000 un

2. ARMAZENAGEM
   Data: 2025-11-20 10:30
   Localização: A-01-2-B
   Operador: João Silva

3. SEPARAÇÃO
   Data: 2025-11-21 14:20
   Pedido: ORD-555
   Picker: Maria Santos
   Quantidade: 100 un

4. EXPEDIÇÃO
   Data: 2025-11-21 16:00
   Cliente: XYZ Ltda
   Tracking: BR123456789
   Transportadora: Correios
```

**BACKWARD TRACING** (Cliente → Fornecedor - RECALL):
```
Cliente reporta problema com produto
Tracking: BR123456789

Sistema rastreia:
→ Expedido: 2025-11-21 (OSH-002)
→ Separado: 2025-11-21 (PICK-789)
→ Localização: A-01-2-B
→ Recebido: 2025-11-20 (GRN-001)
→ Lote: LOT-2025-11-15
→ Fornecedor: ACME Corp
→ Data fabricação: 2025-11-15

ACÃO: Identificar TODO o lote
→ Ainda em estoque: 850 un em 3 locais
→ Já expedido: 50 un para 2 clientes
→ Total afetado: 1000 un

RECALL CAPABILITY: 100%
```

**Entidades**:
```csharp
public class MovementHistory
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? LotId { get; set; }
    public string SerialNumber { get; set; }
    public MovementType Type { get; set; }
    public Guid FromLocationId { get; set; }
    public Guid ToLocationId { get; set; }
    public decimal Quantity { get; set; }
    public Guid? TaskId { get; set; }
    public string Reason { get; set; }
    public Guid MovedBy { get; set; }
    public DateTime MovedAt { get; set; }
}

public class AuditLog
{
    public Guid Id { get; set; }
    public string EntityType { get; set; }
    public Guid EntityId { get; set; }
    public AuditAction Action { get; set; }
    public Guid UserId { get; set; }
    public string OldValues { get; set; }  // JSON
    public string NewValues { get; set; }  // JSON
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; }
}
```

---

## ⚖️ ESCALABILIDADE: PEQUENA vs GRANDE EMPRESA

### EMPRESA PEQUENA (Startup/SMB)

**Características**:
- 1 armazém
- 5-20 usuários
- 100-1.000 SKUs
- 50-200 pedidos/dia

**Módulos Essenciais** (MVP):
```
✅ Receiving (Básico)
✅ Storage (Zonas simples)
✅ Inventory (Real-time)
✅ Order Management
✅ Picking (Single order)
✅ Packing & Shipping (Básico)
✅ Barcode (Geração e leitura)
✅ Basic Reports

❌ Wave Planning
❌ Advanced Analytics
❌ Multiple Warehouses
❌ Complex Integrations
```

**Setup Rápido**:
```
1. Company Admin cria conta
2. Cadastra 1 armazém
3. Define 3-5 zonas básicas
4. Cadastra produtos (importação CSV)
5. Cria 5-10 usuários
6. Começa operação

Tempo: 1-2 dias
Custo: Menor
```

---

### EMPRESA MÉDIA (Mid-Market)

**Características**:
- 2-5 armazéns
- 20-100 usuários
- 1.000-10.000 SKUs
- 200-1.000 pedidos/dia

**Módulos Recomendados**:
```
✅ Todos os básicos +
✅ Wave Planning
✅ Multiple Picking Strategies
✅ Cycle Counting
✅ Multi-warehouse
✅ ERP Integration
✅ Carrier Integration
✅ Advanced Reports
✅ Labor Management

❌ Yard Management (talvez)
❌ Voice Picking
❌ RFID
```

**Setup**:
```
Tempo: 2-4 semanas
Treinamento: 1 semana
Custo: Médio
```

---

### EMPRESA GRANDE (Enterprise)

**Características**:
- 5+ armazéns / CDs
- 100+ usuários
- 10.000+ SKUs
- 1.000+ pedidos/dia
- Operação 24/7

**Módulos Completos**:
```
✅ TUDO +
✅ Yard Management
✅ Advanced Wave Planning
✅ Task Interleaving
✅ Slotting Optimization
✅ Labor Management completo
✅ Voice/RFID (futuro)
✅ Multiple ERP Integration
✅ EDI
✅ Custom Workflows
✅ Advanced Analytics
✅ Machine Learning (previsão)
```

**Setup**:
```
Tempo: 2-6 meses
Treinamento: 2-4 semanas
Custo: Alto
ROI: 12-18 meses
```

---

## 🗺️ ROADMAP DE IMPLEMENTAÇÃO

### FASE 1: FUNDAÇÃO (Mês 1-3)

**Objetivo**: Base funcionando

**Backend**:
```
✅ Setup inicial do sistema
✅ Company Admin (não SuperAdmin único)
✅ Sistema de Roles (12 roles padrão)
✅ Multi-tenancy
✅ Autenticação JWT
✅ Permissões granulares

✅ Entidades Core:
   - Company, User, UserRole
   - Warehouse, WarehouseZone
   - StorageLocation (expandida)
   - Product (expandido com SKU)
   - Customer, Supplier
   - Vehicle, Driver

✅ CRUD básico de todas entidades
✅ Validações de negócio
✅ API RESTful
```

**Entregável**: Sistema base com usuários e cadastros

---

### FASE 2: RECEIVING & STORAGE (Mês 4-5)

**Objetivo**: Entrada de mercadorias

**Implementar**:
```
✅ Order (Inbound)
✅ InboundShipment
✅ Receipt (GRN)
✅ ReceiptLine
✅ Lot
✅ PutawayTask
✅ VehicleAppointment (básico)
✅ GateEntry

✅ Fluxo completo:
   1. Agendamento
   2. Chegada (portaria)
   3. Recebimento
   4. Inspeção
   5. Putaway
   6. Atualização inventory

✅ Barcode scanning
✅ Label printing
```

**Entregável**: Recebimento funcionando end-to-end

---

### FASE 3: INVENTORY & PICKING (Mês 6-7)

**Objetivo**: Controle de estoque e separação

**Implementar**:
```
✅ Inventory (completo)
✅ InventoryAdjustment
✅ CycleCount
✅ SerialNumber (se necessário)

✅ Order (Outbound)
✅ OutboundShipment
✅ PickingTask
✅ PickingLine
✅ PickingWave (básico)

✅ Picking Strategies:
   - Single Order
   - Batch Picking

✅ FIFO/FEFO
✅ Stock Alerts
```

**Entregável**: Separação de pedidos funcionando

---

### FASE 4: PACKING & SHIPPING (Mês 8)

**Objetivo**: Embalagem e expedição

**Implementar**:
```
✅ PackingTask
✅ Package
✅ Carrier Integration (básico)
✅ Tracking Number
✅ BOL Generation
✅ Manifest

✅ Packing Station
✅ Multi-package orders
✅ Label printing
✅ Weight/dimension capture
```

**Entregável**: Expedição completa

---

### FASE 5: ANALYTICS & REPORTS (Mês 9)

**Objetivo**: Visibilidade e controle

**Implementar**:
```
✅ Dashboards:
   - Executive
   - Operational
   - Por usuário

✅ Relatórios:
   - Inventory Reports
   - Order Reports
   - Performance Reports
   - Variance Reports

✅ KPI Tracking:
   - 40+ KPIs
   - Real-time updates

✅ Export (Excel, PDF, CSV)
✅ Scheduled Reports
```

**Entregável**: Sistema de reporting completo

---

### FASE 6: ADVANCED FEATURES (Mês 10-11)

**Objetivo**: Funcionalidades avançadas

**Implementar**:
```
✅ Replenishment (automático)
✅ Wave Planning (avançado)
✅ Zone Picking
✅ Task Interleaving
✅ Slotting Optimization
✅ Labor Management
✅ Yard Management
✅ Advanced Analytics
```

**Entregável**: WMS completo enterprise-grade

---

### FASE 7: MOBILE APPS (Mês 6-12, paralelo)

**Objetivo**: Mobilidade operacional

**Apps**:
```
📱 Receiving App
   - Scan produtos
   - Registrar recebimento
   - Quality check

📱 Putaway App
   - Ver tarefas
   - Scan origem/destino
   - Confirmar armazenagem

📱 Picking App
   - Ver picking list
   - Rotas otimizadas
   - Scan e confirmar

📱 Packing App
   - Scan itens
   - Gerar etiquetas
   - Confirmar pacotes

📱 Cycle Count App
   - Contagens programadas
   - Scan e contar
   - Reportar variâncias
```

**Tecnologia**: React Native ou Flutter

---

## 📊 RESUMO EXECUTIVO

### Módulos Implementados: 10

1. ✅ Receiving
2. ✅ Storage (Put-away)
3. ✅ Inventory Management
4. ✅ Order Management
5. ✅ Picking
6. ✅ Packing & Shipping
7. ✅ Barcode & Data Capture
8. ✅ Dock & Yard Management
9. ✅ Analytics & Reporting
10. ✅ Integration

### Entidades Totais: 35+

**Core**: 8 (expandidas do sistema atual)
**Novas**: 27

### Roles de Usuário: 12

1. CompanyAdmin (primeiro usuário, não SuperAdmin)
2. WarehouseManager
3. ReceivingClerk
4. QualityControl
5. PutawayOperator
6. Picker
7. Packer
8. ShippingClerk
9. InventoryController
10. SecurityGuard
11. Driver
12. Viewer

### Timeline Total

```
Pequena Empresa: 3-4 meses (MVP)
Média Empresa: 6-8 meses
Grande Empresa: 10-12 meses (completo)
```

### ROI Esperado

```
Redução de Erros: 60-80%
Aumento de Produtividade: 25-40%
Redução de Custos Operacionais: 20-30%
Melhoria na Acuracidade: > 99%
Redução de Tempo de Ciclo: 30-50%

Payback: 12-24 meses
```

---

## ✅ PRÓXIMOS PASSOS

### 1. Validação
- [ ] Revisar especificação completa
- [ ] Validar com stakeholders
- [ ] Aprovar scope e timeline
- [ ] Definir budget

### 2. Planejamento
- [ ] Montar equipe (3-5 devs)
- [ ] Definir sprints (2 semanas cada)
- [ ] Setup ambiente de desenvolvimento
- [ ] Criar repositório Git

### 3. Início da Fase 1
- [ ] Criar branches de feature
- [ ] Implementar sistema de Roles
- [ ] Expandir entidades existentes
- [ ] Criar novas entidades core
- [ ] Implementar seed do setup wizard

---

## 🎓 CONCLUSÃO

Este WMS foi especificado com base em **sistemas reais do mercado** (MRPeasy, SAP, Oracle, Manhattan Associates) e segue os **princípios básicos de WMS profissionais**.

**Diferenciais**:
- ✅ Setup inteligente (não apenas SuperAdmin)
- ✅ 12 roles operacionais (não apenas 3)
- ✅ Escalável (pequena → grande empresa)
- ✅ Módulos completos baseados em WMS reais
- ✅ Rastreabilidade total (recall capability)
- ✅ Multi-tenant robusto
- ✅ Integração com ERP/E-commerce
- ✅ Mobile-first para operadores

**Sistema pronto para competir com WMS comerciais do mercado!**

---

**Documento criado**: 2025-11-21  
**Versão**: 2.0 UNIFICADA  
**Status**: ✅ **COMPLETO E PRONTO PARA IMPLEMENTAÇÃO**
