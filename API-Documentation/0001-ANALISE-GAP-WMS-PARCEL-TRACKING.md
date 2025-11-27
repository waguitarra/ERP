# ANÁLISE DE GAP - SISTEMA WMS: RASTREAMENTO DE PARCELS/VOLUMES

**Data**: 2025-11-27  
**Versão**: 1.0  
**Autor**: Análise Técnica WMS

---

## 📋 SUMÁRIO EXECUTIVO

### Problema Identificado
O sistema WMS atual **NÃO possui controle de volumes/parcels** (partes) que chegam em remessas de entrada (Inbound). Quando um caminhão chega com mercadorias, não há como rastrear:
- **Quantos volumes/parcels** chegaram
- **Quais produtos** estão em cada volume
- **Onde cada volume está** no processo de recebimento
- **Hierarquia de embalagem** (Pallet → Caixa → Produto)

### Impacto no Negócio
❌ **Impossível rastrear**: Se compro 1.000 produtos de um fornecedor, eles podem vir em 50 volumes, mas não há como controlar isso  
❌ **Sem visibilidade**: Não sei qual volume está sendo processado ou onde está  
❌ **Conferência limitada**: Recebimento é feito item a item, sem agrupar por volume  
❌ **Ineficiência**: Operador não sabe quantos volumes faltam descarregar  

---

## 1. MAPEAMENTO COMPLETO DO SISTEMA ATUAL

### 1.1 Backend - Orders (API)

#### **1.1.1 Endpoints Existentes**

**Controller**: `OrdersController.cs`  
**Rota Base**: `/api/orders`  
**Autenticação**: `[Authorize]` - **OBRIGATÓRIO** em todos os endpoints

| Método | Endpoint | Ação | Request | Response |
|--------|----------|------|---------|----------|
| POST | `/api/orders` | Criar pedido | CreateOrderRequest | OrderResponse (201) |
| GET | `/api/orders/{id}` | Buscar por ID | - | OrderResponse (200) |
| GET | `/api/orders/company/{companyId}` | Listar por empresa | - | List<OrderResponse> (200) |
| PUT | `/api/orders/{id}` | Atualizar pedido | UpdateOrderRequest | OrderResponse (200) |

**Exemplo de Request (POST)**:
```json
{
  "companyId": "guid-empresa",
  "orderNumber": "ORD-2025-001",
  "type": 2,              // 1=Inbound, 2=Outbound, 3=Transfer, 4=Return
  "source": 1,            // 1=Manual, 2=ERP, 3=Ecommerce, 4=EDI
  "customerId": "guid-cliente",
  "supplierId": null,
  "expectedDate": "2025-11-30T00:00:00Z",
  "priority": 1,          // 0=Low, 1=Normal, 2=High, 3=Urgent
  "shippingAddress": "Rua A, 123",
  "specialInstructions": "Frágil",
  "isBOPIS": false,
  "items": [
    {
      "productId": "guid-produto",
      "sku": "PROD-001",
      "quantityOrdered": 10,
      "unitPrice": 50.00
    }
  ]
}
```

#### **1.1.2 Serviço e Regras de Negócio**

**Arquivo**: `OrderService.cs`  
**Padrão**: Domain-Driven Design (DDD)

**Regras Implementadas**:

```csharp
public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, Guid createdBy)
{
    // ✅ VALIDAÇÃO 1: Empresa deve existir
    if (await _companyRepository.GetByIdAsync(request.CompanyId) == null)
        throw new KeyNotFoundException("Empresa não encontrada");
    
    // ✅ VALIDAÇÃO 2: OrderNumber único por empresa
    if (await _orderRepository.GetByOrderNumberAsync(request.OrderNumber, request.CompanyId) != null)
        throw new InvalidOperationException("Número de pedido já existe");
    
    // ✅ CRIAÇÃO: Usando construtor da entidade (DDD)
    var order = new Order(request.CompanyId, request.OrderNumber, request.Type, request.Source);
    
    // ✅ MÉTODOS DE DOMÍNIO: Entidade controla suas próprias regras
    order.SetCustomer(request.CustomerId.Value);
    order.SetDates(request.ExpectedDate);
    order.SetPriority(request.Priority);
    
    // ✅ ITEMS: Validar cada produto existe
    foreach (var itemRequest in request.Items)
    {
        var product = await _productRepository.GetByIdAsync(itemRequest.ProductId);
        if (product == null)
            throw new KeyNotFoundException($"Produto {itemRequest.ProductId} não encontrado");
        
        var item = new OrderItem(/* ... */);
        order.AddItem(item);  // Método do domínio
    }
    
    // ✅ TOTAIS: Calculados automaticamente
    order.UpdateTotals(totalQty, totalValue);
    
    // ✅ PERSISTÊNCIA: Repository + Unit of Work
    await _orderRepository.AddAsync(order);
    await _unitOfWork.CommitAsync();
    
    return await GetByIdAsync(order.Id);
}
```

**Padrões Arquiteturais Usados**:
- ✅ **Repository Pattern**: `IOrderRepository`
- ✅ **Unit of Work**: `IUnitOfWork` para transações
- ✅ **Domain Entities**: Entidades com lógica de negócio
- ✅ **DTOs**: Separação Request/Response
- ✅ **Dependency Injection**: Injeção via construtor

#### **1.1.3 Entidade Order (Domain)**

**Arquivo**: `Order.cs`  
**Namespace**: `Logistics.Domain.Entities`

```csharp
public class Order
{
    // Construtor privado para EF Core
    private Order() { }
    
    // Construtor público com validações
    public Order(Guid companyId, string orderNumber, OrderType type, OrderSource source)
    {
        if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido");
        if (string.IsNullOrWhiteSpace(orderNumber)) throw new ArgumentException("OrderNumber inválido");
        
        Id = Guid.NewGuid();
        CompanyId = companyId;
        OrderNumber = orderNumber;
        Type = type;
        Source = source;
        Status = OrderStatus.Draft;
        OrderDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
    
    // Propriedades com private set (encapsulamento)
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string OrderNumber { get; private set; }
    public OrderType Type { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalQuantity { get; private set; }
    public decimal TotalValue { get; private set; }
    
    // Navigation Properties
    public Company Company { get; private set; } = null!;
    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();
    
    // Métodos de negócio
    public void SetStatus(OrderStatus status) { Status = status; UpdatedAt = DateTime.UtcNow; }
    public void SetPriority(OrderPriority priority) { Priority = priority; UpdatedAt = DateTime.UtcNow; }
    public void AddItem(OrderItem item) { Items.Add(item); }
    public void UpdateTotals(decimal qty, decimal value) { TotalQuantity = qty; TotalValue = value; }
}
```

#### **1.1.4 Configuração EF Core (Entity Framework)**

**Arquivo**: `OrderConfiguration.cs`  
**Pattern**: Fluent API

```csharp
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);
        
        // Campos obrigatórios
        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
        
        // Enums como string
        builder.Property(o => o.Type).IsRequired().HasConversion<string>();
        builder.Property(o => o.Status).IsRequired().HasConversion<string>();
        
        // Precisão decimal
        builder.Property(o => o.TotalQuantity).HasPrecision(18, 2);
        builder.Property(o => o.TotalValue).HasPrecision(18, 2);
        
        // Relacionamentos
        builder.HasOne(o => o.Company)
            .WithMany()
            .HasForeignKey(o => o.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Índices para performance
        builder.HasIndex(o => new { o.CompanyId, o.OrderNumber }).IsUnique();
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.OrderDate);
    }
}
```

#### **1.1.5 Migrations (EF Core)**

**⚠️ REGRA CRÍTICA**: **NUNCA** alterar banco de dados diretamente!

**Processo Correto**:
```bash
# 1. Criar migration após mudar entidades
cd API/src/Logistics.Infrastructure
dotnet ef migrations add AddParcelTracking --startup-project ../Logistics.API

# 2. Aplicar migration no banco
dotnet ef database update --startup-project ../Logistics.API

# 3. Verificar migration criada
ls Migrations/
```

**Exemplo de Migration Existente**:
- `20251122174613_InitialCreateComplete.cs` - Criação inicial
- `20251125212515_AddOrderStatusPriorityAndWMSFields.cs` - Campos WMS

### 1.2 Frontend - Orders (Angular)

#### **1.2.1 Estrutura de Arquivos**

```
APP/src/app/features/orders/
├── orders.service.ts                    # Serviço HTTP
├── orders-list/
│   ├── orders-list.component.ts         # Lista principal
│   ├── orders-list.component.html
│   └── orders-list.component.scss
├── order-create-modal/
│   ├── order-create-modal.component.ts  # Modal de criação
│   ├── order-create-modal.component.html
│   └── order-create-modal.component.scss
└── order-edit-modal/
    ├── order-edit-modal.component.ts    # Modal de edição
    ├── order-edit-modal.component.html
    └── order-edit-modal.component.scss
```

#### **1.2.2 Serviço Orders**

**Arquivo**: `orders.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class OrdersService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/orders';

  getAll(companyId?: string): Promise<any> {
    if (!companyId) return Promise.resolve({ success: true, data: [] });
    return this.api.get<any>(`${this.endpoint}/company/${companyId}`);
  }

  getById(id: string): Promise<any> {
    return this.api.get<any>(`${this.endpoint}/${id}`);
  }

  create(data: CreateOrderRequest): Promise<any> {
    return this.api.post<any>(this.endpoint, data);
  }

  update(id: string, data: UpdateOrderDto): Promise<any> {
    return this.api.put<any>(`${this.endpoint}/${id}`, data);
  }
}
```

**Padrões**:
- ✅ `inject()` moderno do Angular 17+
- ✅ `ApiService` centralizado (auth headers automático)
- ✅ Promises (não Observables) para simplicidade

#### **1.2.3 Componente Lista**

**Arquivo**: `orders-list.component.ts`

```typescript
export class OrdersListComponent implements OnInit {
  private readonly ordersService = inject(OrdersService);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);  // ✅ i18n obrigatório

  // ✅ Signals (Angular 17+)
  loading = signal<boolean>(true);
  orders = signal<Order[]>([]);
  
  // ✅ Computed values
  totalOrders = computed(() => this.orders().length);
  totalValue = computed(() => this.orders().reduce((sum, o) => sum + o.totalValue, 0));

  ngOnInit(): void {
    this.loadOrders();
  }

  async loadOrders(): Promise<void> {
    this.loading.set(true);
    try {
      const user = this.authService.currentUser();  // ✅ Auth obrigatório
      const companyId = user?.companyId;
      
      const response = await this.ordersService.getAll(companyId);
      this.orders.set(response.data || []);
    } catch (err) {
      console.error('Erro:', err);
    } finally {
      this.loading.set(false);
    }
  }
  
  // ✅ Classes para dark mode
  getStatusClass(status: OrderStatus): string {
    const classes: Record<number, string> = {
      [OrderStatus.Pending]: 'bg-amber-50 dark:bg-amber-900/30 text-amber-600 dark:text-amber-400',
      [OrderStatus.Delivered]: 'bg-green-50 dark:bg-green-900/30 text-green-600 dark:text-green-400'
    };
    return classes[status] || 'bg-slate-50 dark:bg-slate-700';
  }
}
```

#### **1.2.4 Padrões de UI Obrigatórios**

**1. Internacionalização (i18n)**:
```typescript
// ✅ NO COMPONENTE
protected readonly i18n = inject(I18nService);

// ✅ NO TEMPLATE
<h2>{{ i18n.t('orders.title') }}</h2>
<button>{{ i18n.t('common.buttons.create') }}</button>
```

**2. Dark Mode**:
```html
<!-- ✅ Classes com dark: prefix -->
<div class="bg-white dark:bg-slate-900">
  <h2 class="text-slate-800 dark:text-slate-100">Título</h2>
  <p class="text-slate-600 dark:text-slate-400">Texto</p>
</div>
```

**3. Autenticação**:
```typescript
// ✅ Route guard
const routes: Routes = [
  {
    path: 'orders',
    component: OrdersListComponent,
    canActivate: [AuthGuard]  // ✅ Obrigatório
  }
];
```

**4. Layout Padrão**:
```html
<!-- ✅ Modal structure -->
<div class="fixed inset-0 bg-black/50 dark:bg-black/70 flex items-center justify-center z-50">
  <div class="bg-white dark:bg-slate-900 rounded-xl shadow-2xl w-full max-w-4xl">
    <!-- Header -->
    <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
      <h2 class="text-xl font-bold text-slate-800 dark:text-slate-100">
        {{ i18n.t('modals.createOrder') }}
      </h2>
    </div>
    
    <!-- Body -->
    <form class="p-6">
      <!-- Campos -->
    </form>
    
    <!-- Footer -->
    <div class="px-6 py-4 border-t border-slate-200 dark:border-slate-700 flex justify-end gap-3">
      <button class="px-4 py-2 text-slate-600 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-800">
        {{ i18n.t('common.buttons.cancel') }}
      </button>
      <button class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white">
        {{ i18n.t('common.buttons.save') }}
      </button>
    </div>
  </div>
</div>
```

### 1.3 GAP Identificado: Ausência de Controle de Parcels

**Sistema Atual**:
```
InboundShipment (Remessa de Entrada)
    └── Order (Pedido de Compra)
        └── OrderItem[] (Itens do Pedido)
    └── Receipt (Recebimento)
        └── ReceiptLine[] (Linhas de Recebimento por Produto)
```

❌ **PROBLEMA**: Não há entidades para volumes/parcels  
❌ **IMPACTO**: Impossível rastrear quantos volumes chegaram  
❌ **CONSEQUÊNCIA**: Conferência ineficiente, sem visibilidade  

---

## 2. IMPLEMENTAÇÃO BACKEND - PARCEL TRACKING

### 2.1 Arquitetura DDD Completa

#### **2.1.1 Camadas da Aplicação**

```
Logistics.Domain/        ← Entidades, Enums, Interfaces de domínio
Logistics.Application/   ← DTOs, Services, Interfaces de aplicação
Logistics.Infrastructure/← Repositories, EF Config, Migrations
Logistics.API/           ← Controllers, Program.cs, DI
```

### 2.2 Entidades de Domínio (Domain Layer)

#### **ENTIDADE 1: InboundParcel**

**Arquivo**: `API/src/Logistics.Domain/Entities/InboundParcel.cs`

```csharp
using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class InboundParcel
{
    // ✅ Construtor privado para EF Core
    private InboundParcel() { }

    // ✅ Construtor público com validações (DDD)
    public InboundParcel(Guid inboundShipmentId, string parcelNumber, ParcelType type)
    {
        if (inboundShipmentId == Guid.Empty)
            throw new ArgumentException("InboundShipmentId inválido");
        if (string.IsNullOrWhiteSpace(parcelNumber))
            throw new ArgumentException("ParcelNumber inválido");

        Id = Guid.NewGuid();
        InboundShipmentId = inboundShipmentId;
        ParcelNumber = parcelNumber;
        Type = type;
        Status = ParcelStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    // ✅ Propriedades com private set (encapsulamento)
    public Guid Id { get; private set; }
    public Guid InboundShipmentId { get; private set; }
    public string ParcelNumber { get; private set; } = string.Empty;
    public string? LPN { get; private set; }
    public ParcelType Type { get; private set; }
    public int SequenceNumber { get; private set; }
    public int TotalParcels { get; private set; }
    
    // Hierarquia
    public Guid? ParentParcelId { get; private set; }
    
    // Dimensões
    public decimal Weight { get; private set; }
    public decimal Length { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }
    public string? DimensionUnit { get; private set; }
    
    // Rastreamento
    public ParcelStatus Status { get; private set; }
    public string? CurrentLocation { get; private set; }
    public DateTime? ReceivedAt { get; private set; }
    public Guid? ReceivedBy { get; private set; }
    
    // Qualidade
    public bool HasDamage { get; private set; }
    public string? DamageNotes { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // ✅ Navigation Properties
    public InboundShipment InboundShipment { get; private set; } = null!;
    public InboundParcel? ParentParcel { get; private set; }
    public ICollection<InboundParcel> ChildParcels { get; private set; } = new List<InboundParcel>();
    public ICollection<InboundParcelItem> Items { get; private set; } = new List<InboundParcelItem>();

    // ✅ MÉTODOS DE NEGÓCIO (DDD)
    public void SetLPN(string lpn)
    {
        if (string.IsNullOrWhiteSpace(lpn))
            throw new ArgumentException("LPN inválido");
        LPN = lpn;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetSequence(int sequenceNumber, int totalParcels)
    {
        if (sequenceNumber <= 0 || totalParcels <= 0)
            throw new ArgumentException("Sequência inválida");
        if (sequenceNumber > totalParcels)
            throw new ArgumentException("Sequência não pode ser maior que total");
        
        SequenceNumber## 📦 Product (Produto)

**Descrição**: Produtos cadastrados no sistema

**Campos principais**:
- `Id` (guid, PK)
- `CompanyId` (guid, FK → Companies)
- `CategoryId` (guid, FK → ProductCategories) 
- `SKU` (string, único)
- `Name` (string)
- `Description` (string)
- `Barcode` (string)
- `UnitOfMeasure` (string)
- `UnitCost` (decimal)
- `SalePrice` (decimal)
- `Weight` (decimal)
- `Dimensions` (string)
- `IsActive` (bool)
- `IsTrackedBySerial` (bool)

**Relacionamentos**:
- `N:1` com ProductCategory (categoria do produto)

---

## 🏷️ ProductCategory (Categoria de Produto) 

**Descrição**: Categorias para organizar produtos (Computadores, Ferramentas, Manutenção, etc.)

**Campos principais**:
- `Id` (guid, PK)
- `Name` (string, obrigatório) - Ex: "Computadores e Periféricos"
- `Code` (string, único, obrigatório) - Ex: "COMP"
- `Description` (string, opcional)
- `Barcode` (string, opcional) - Código de barras da categoria
- `Reference` (string, opcional) - Referência interna
- `IsMaintenance` (bool) - Se é categoria de manutenção
- `IsActive` (bool) - Se está ativa
- `Attributes` (json, opcional) - Atributos extras customizáveis
- `CreatedAt` (datetime)
- `UpdatedAt` (datetime)

**Relacionamentos**:
- `1:N` com Product (uma categoria tem vários produtos)

**Endpoints**:
- `GET /api/product-categories` - Listar todas
- `GET /api/product-categories/active` - Listar apenas ativas
- `GET /api/product-categories/{id}` - Detalhes por ID
- `GET /api/product-categories/by-code/{code}` - Buscar por código
- `POST /api/product-categories` - Criar nova categoria
- `PUT /api/product-categories/{id}` - Atualizar categoria
- `POST /api/product-categories/{id}/activate` - Ativar categoria
- `POST /api/product-categories/{id}/deactivate` - Desativar categoria
- `DELETE /api/product-categories/{id}` - Excluir (se sem produtos)

**Request para criar categoria**:
```json
{
  "name": "Computadores e Periféricos",
  "code": "COMP",
  "description": "Notebooks, desktops, mouses, teclados",
  "barcode": "CAT-COMP-001",
  "reference": "REF-COMP-2025",
  "isMaintenance": false,
  "attributes": "{\"color\":\"blue\",\"icon\":\"computer\"}"
}
```

**Regras de negócio**:
1. Code deve ser único
2. Não pode excluir categoria com produtos vinculados
3. Ao desativar, produtos permanecem vinculados
4. Attributes aceita JSON livre para metadados extras
5. Barcode pode ser usado para scan rápido de categoria

**Uso em Purchase Orders**:
- Ao criar Purchase Order, produtos são filtrados por categoria
- Na listagem, pode agrupar por categoria
- Relatórios podem ser gerados por categoria
- Estoque pode ser consultado por categoria

        Weight = weight;
        Length = length;
        Width = width;
{{ ... }}
        Height = height;
        DimensionUnit = unit;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(ParcelStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsReceived(Guid receivedBy, string? location = null)
    {
        Status = ParcelStatus.Received;
        ReceivedAt = DateTime.UtcNow;
        ReceivedBy = receivedBy;
        if (!string.IsNullOrEmpty(location))
            CurrentLocation = location;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReportDamage(string notes)
    {
        HasDamage = true;
        DamageNotes = notes;
        Status = ParcelStatus.Damaged;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetLocation(string location)
    {
        CurrentLocation = location;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

#### **ENTIDADE 2: InboundParcelItem**

**Arquivo**: `API/src/Logistics.Domain/Entities/InboundParcelItem.cs`

```csharp
namespace Logistics.Domain.Entities;

public class InboundParcelItem
{
    private InboundParcelItem() { }

    public InboundParcelItem(Guid inboundParcelId, Guid productId, string sku, decimal quantity)
    {
        if (inboundParcelId == Guid.Empty)
            throw new ArgumentException("InboundParcelId inválido");
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId inválido");
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU inválido");
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero");

        Id = Guid.NewGuid();
        InboundParcelId = inboundParcelId;
        ProductId = productId;
        SKU = sku;
        Quantity = quantity;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid InboundParcelId { get; private set; }
    public Guid ProductId { get; private set; }
    public string SKU { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public Guid? LotId { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public InboundParcel InboundParcel { get; private set; } = null!;
    public Product Product { get; private set; } = null!;
    public Lot? Lot { get; private set; }

    public void SetLot(Guid lotId, DateTime? expiryDate)
    {
        LotId = lotId;
        ExpiryDate = expiryDate;
    }
}
```

#### **ENUMS: ParcelType e ParcelStatus**

**Arquivo**: `API/src/Logistics.Domain/Enums/ParcelType.cs`

```csharp
namespace Logistics.Domain.Enums;

public enum ParcelType
{
    Pallet = 1,
    Carton = 2,
    Box = 3,
    Bag = 4,
    Bulk = 5,
    Container = 6,
    Tote = 7,
    Drum = 8
}
```

**Arquivo**: `API/src/Logistics.Domain/Enums/ParcelStatus.cs`

```csharp
namespace Logistics.Domain.Enums;

public enum ParcelStatus
{
    Pending = 0,
    InTransit = 1,
    AtDock = 2,
    Receiving = 3,
    Received = 4,
    Damaged = 5,
    Quarantine = 6,
    ReadyForPutaway = 7,
    PutawayInProgress = 8,
    Stored = 9,
    CrossDock = 10
}
```

### 2.3 Configuração EF Core (Infrastructure Layer)

**Arquivo**: `API/src/Logistics.Infrastructure/Data/Configurations/InboundParcelConfiguration.cs`

```csharp
using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Infrastructure.Data.Configurations;

public class InboundParcelConfiguration : IEntityTypeConfiguration<InboundParcel>
{
    public void Configure(EntityTypeBuilder<InboundParcel> builder)
    {
        builder.ToTable("InboundParcels");
        builder.HasKey(p => p.Id);

        // ✅ Campos obrigatórios
        builder.Property(p => p.ParcelNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.LPN)
            .HasMaxLength(100);

        // ✅ Enums como string
        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();

        // ✅ Precisão decimal
        builder.Property(p => p.Weight).HasPrecision(18, 2);
        builder.Property(p => p.Length).HasPrecision(18, 2);
        builder.Property(p => p.Width).HasPrecision(18, 2);
        builder.Property(p => p.Height).HasPrecision(18, 2);

        builder.Property(p => p.DimensionUnit).HasMaxLength(10);
        builder.Property(p => p.CurrentLocation).HasMaxLength(100);
        builder.Property(p => p.DamageNotes).HasMaxLength(1000);

        // ✅ Relacionamentos
        builder.HasOne(p => p.InboundShipment)
            .WithMany()
            .HasForeignKey(p => p.InboundShipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ParentParcel)
            .WithMany(p => p.ChildParcels)
            .HasForeignKey(p => p.ParentParcelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Items)
            .WithOne(i => i.InboundParcel)
            .HasForeignKey(i => i.InboundParcelId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ Índices para performance
        builder.HasIndex(p => new { p.InboundShipmentId, p.ParcelNumber }).IsUnique();
        builder.HasIndex(p => p.LPN);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.CreatedAt);
    }
}
```

### 2.4 Migration

**⚠️ PROCESSO OBRIGATÓRIO**:

```bash
# 1. Navegar para Infrastructure
cd /home/wagnerfb/Projetos/ERP/API/src/Logistics.Infrastructure

# 2. Criar migration
dotnet ef migrations add AddInboundParcelTracking --startup-project ../Logistics.API

# 3. Verificar migration gerada
cat Migrations/*AddInboundParcelTracking.cs

# 4. Aplicar no banco
dotnet ef database update --startup-project ../Logistics.API
```

**Migration será criada automaticamente com**:
- Tabela `InboundParcels`
- Tabela `InboundParcelItems`
- Foreign keys
- Índices

### 2.5 Repository (Infrastructure Layer)

**Interface**: `API/src/Logistics.Domain/Interfaces/IInboundParcelRepository.cs`

```csharp
using Logistics.Domain.Entities;

namespace Logistics.Domain.Interfaces;

public interface IInboundParcelRepository
{
    Task<InboundParcel?> GetByIdAsync(Guid id);
    Task<IEnumerable<InboundParcel>> GetByShipmentIdAsync(Guid shipmentId);
    Task<InboundParcel?> GetByLPNAsync(string lpn);
    Task AddAsync(InboundParcel parcel);
    Task UpdateAsync(InboundParcel parcel);
    Task<int> CountByShipmentIdAsync(Guid shipmentId);
}
```

**Implementação**: `API/src/Logistics.Infrastructure/Repositories/InboundParcelRepository.cs`

```csharp
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Infrastructure.Repositories;

public class InboundParcelRepository : IInboundParcelRepository
{
    private readonly LogisticsDbContext _context;

    public InboundParcelRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<InboundParcel?> GetByIdAsync(Guid id)
    {
        return await _context.InboundParcels
            .Include(p => p.Items)
                .ThenInclude(i => i.Product)
            .Include(p => p.ChildParcels)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<InboundParcel>> GetByShipmentIdAsync(Guid shipmentId)
    {
        return await _context.InboundParcels
            .Include(p => p.Items)
            .Where(p => p.InboundShipmentId == shipmentId)
            .OrderBy(p => p.SequenceNumber)
            .ToListAsync();
    }

    public async Task<InboundParcel?> GetByLPNAsync(string lpn)
    {
        return await _context.InboundParcels
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.LPN == lpn);
    }

    public async Task AddAsync(InboundParcel parcel)
    {
        await _context.InboundParcels.AddAsync(parcel);
    }

    public async Task UpdateAsync(InboundParcel parcel)
    {
        _context.InboundParcels.Update(parcel);
    }

    public async Task<int> CountByShipmentIdAsync(Guid shipmentId)
    {
        return await _context.InboundParcels
            .CountAsync(p => p.InboundShipmentId == shipmentId);
    }
}
```

### 2.6 Application Layer - DTOs

**Arquivo**: `API/src/Logistics.Application/DTOs/InboundParcel/CreateInboundParcelRequest.cs`

```csharp
using Logistics.Domain.Enums;

namespace Logistics.Application.DTOs.InboundParcel;

public record CreateInboundParcelRequest(
    Guid InboundShipmentId,
    string ParcelNumber,
    string? LPN,
    ParcelType Type,
    int SequenceNumber,
    int TotalParcels,
    decimal Weight,
    List<CreateInboundParcelItemRequest> Items
);

public record CreateInboundParcelItemRequest(
    Guid ProductId,
    string SKU,
    decimal Quantity
);
```

### 2.7 Service (Application Layer)

**Arquivo**: `API/src/Logistics.Application/Services/InboundParcelService.cs`

```csharp
using Logistics.Application.DTOs.InboundParcel;
using Logistics.Domain.Entities;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class InboundParcelService
{
    private readonly IInboundParcelRepository _parcelRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InboundParcelService(
        IInboundParcelRepository parcelRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _parcelRepository = parcelRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InboundParcel> CreateAsync(CreateInboundParcelRequest request)
    {
        // ✅ Validações
        var parcel = new InboundParcel(
            request.InboundShipmentId,
            request.ParcelNumber,
            request.Type
        );

        if (!string.IsNullOrEmpty(request.LPN))
            parcel.SetLPN(request.LPN);

        parcel.SetSequence(request.SequenceNumber, request.TotalParcels);

        // ✅ Adicionar items
        foreach (var itemRequest in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId);
            if (product == null)
                throw new KeyNotFoundException($"Produto {itemRequest.ProductId} não encontrado");

            var item = new InboundParcelItem(
                parcel.Id,
                itemRequest.ProductId,
                itemRequest.SKU,
                itemRequest.Quantity
            );
        }

        await _parcelRepository.AddAsync(parcel);
        await _unitOfWork.CommitAsync();

        return parcel;
    }
}
```

### 2.8 Controller (API Layer)

**Arquivo**: `API/src/Logistics.API/Controllers/InboundParcelsController.cs`

```csharp
using Logistics.Application.DTOs.InboundParcel;
using Logistics.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/inboundparcels")]
[Authorize]  // ✅ AUTH OBRIGATÓRIO
public class InboundParcelsController : ControllerBase
{
    private readonly InboundParcelService _service;

    public InboundParcelsController(InboundParcelService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateInboundParcelRequest request)
    {
        var parcel = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = parcel.Id }, parcel);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        // Implementation
        return Ok();
    }

    [HttpGet("shipment/{shipmentId}")]
    public async Task<ActionResult> GetByShipment(Guid shipmentId)
    {
        // Implementation
        return Ok();
    }
}
```

### 2.9 Dependency Injection (Program.cs)

**Arquivo**: `API/src/Logistics.API/Program.cs`

```csharp
// ✅ Adicionar no Program.cs
builder.Services.AddScoped<IInboundParcelRepository, InboundParcelRepository>();
builder.Services.AddScoped<InboundParcelService>();
```

---

## 3. IMPLEMENTAÇÃO FRONTEND - PARCEL TRACKING

### 3.1 Estrutura de Arquivos

```
APP/src/app/features/inbound-parcels/
├── inbound-parcels.service.ts
├── parcel-list/
│   ├── parcel-list.component.ts
│   ├── parcel-list.component.html
│   └── parcel-list.component.scss
├── parcel-create-modal/
│   ├── parcel-create-modal.component.ts
│   ├── parcel-create-modal.component.html
│   └── parcel-create-modal.component.scss
└── parcel-receive-modal/
    ├── parcel-receive-modal.component.ts
    ├── parcel-receive-modal.component.html
    └── parcel-receive-modal.component.scss
```

### 3.2 Model

**Arquivo**: `APP/src/app/core/models/inbound-parcel.model.ts`

```typescript
import { ParcelType, ParcelStatus } from './enums';

export interface InboundParcel {
  id: string;
  inboundShipmentId: string;
  parcelNumber: string;
  lpn?: string;
  type: ParcelType;
  sequenceNumber: number;
  totalParcels: number;
  weight: number;
  status: ParcelStatus;
  currentLocation?: string;
  hasDamage: boolean;
  receivedAt?: Date;
  items: InboundParcelItem[];
}

export interface InboundParcelItem {
  id: string;
  productId: string;
  sku: string;
  quantity: number;
}

export interface CreateInboundParcelRequest {
  inboundShipmentId: string;
  parcelNumber: string;
  lpn?: string;
  type: ParcelType;
  sequenceNumber: number;
  totalParcels: number;
  weight: number;
  items: CreateInboundParcelItemRequest[];
}

export interface CreateInboundParcelItemRequest {
  productId: string;
  sku: string;
  quantity: number;
}
```

### 3.3 Enums

**Arquivo**: `APP/src/app/core/models/enums.ts` (adicionar)

```typescript
export enum ParcelType {
  Pallet = 1,
  Carton = 2,
  Box = 3,
  Bag = 4,
  Bulk = 5,
  Container = 6,
  Tote = 7,
  Drum = 8
}

export enum ParcelStatus {
  Pending = 0,
  InTransit = 1,
  AtDock = 2,
  Receiving = 3,
  Received = 4,
  Damaged = 5,
  Quarantine = 6,
  ReadyForPutaway = 7,
  PutawayInProgress = 8,
  Stored = 9,
  CrossDock = 10
}
```

### 3.4 Service

**Arquivo**: `APP/src/app/features/inbound-parcels/inbound-parcels.service.ts`

```typescript
import { Injectable, inject } from '@angular/core';
import { ApiService } from '@core/services/api.service';
import { InboundParcel, CreateInboundParcelRequest } from '@core/models/inbound-parcel.model';

@Injectable({
  providedIn: 'root'
})
export class InboundParcelsService {
  private readonly api = inject(ApiService);
  private readonly endpoint = '/inboundparcels';

  getByShipment(shipmentId: string): Promise<any> {
    return this.api.get<any>(`${this.endpoint}/shipment/${shipmentId}`);
  }

  getById(id: string): Promise<any> {
    return this.api.get<any>(`${this.endpoint}/${id}`);
  }

  create(data: CreateInboundParcelRequest): Promise<any> {
    return this.api.post<any>(this.endpoint, data);
  }

  markAsReceived(id: string, receivedBy: string): Promise<any> {
    return this.api.post<any>(`${this.endpoint}/${id}/receive`, { receivedBy });
  }
}
```

### 3.5 Componente Lista de Parcels

**Arquivo**: `APP/src/app/features/inbound-parcels/parcel-list/parcel-list.component.ts`

```typescript
import { Component, signal, computed, inject, input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InboundParcelsService } from '../inbound-parcels.service';
import { InboundParcel } from '@core/models/inbound-parcel.model';
import { ParcelStatus } from '@core/models/enums';
import { I18nService } from '@core/services/i18n.service';  // ✅ i18n
import { AuthService } from '@core/services/auth.service';  // ✅ auth

@Component({
  selector: 'app-parcel-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './parcel-list.component.html',
  styleUrls: ['./parcel-list.component.scss']
})
export class ParcelListComponent implements OnInit {
  private readonly parcelsService = inject(InboundParcelsService);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);  // ✅ i18n obrigatório

  shipmentId = input.required<string>();
  
  // ✅ Signals
  loading = signal<boolean>(true);
  parcels = signal<InboundParcel[]>([]);
  
  // ✅ Computed
  totalParcels = computed(() => this.parcels().length);
  receivedParcels = computed(() => 
    this.parcels().filter(p => p.status === ParcelStatus.Received).length
  );
  progress = computed(() => {
    const total = this.totalParcels();
    return total > 0 ? Math.round((this.receivedParcels() / total) * 100) : 0;
  });

  ngOnInit(): void {
    this.loadParcels();
  }

  async loadParcels(): Promise<void> {
    this.loading.set(true);
    try {
      const response = await this.parcelsService.getByShipment(this.shipmentId());
      this.parcels.set(response.data || []);
    } catch (err) {
      console.error('Erro ao carregar parcels:', err);
    } finally {
      this.loading.set(false);
    }
  }

  // ✅ Classes com dark mode
  getStatusClass(status: ParcelStatus): string {
    const classes: Record<number, string> = {
      [ParcelStatus.Pending]: 'bg-slate-100 dark:bg-slate-800 text-slate-600 dark:text-slate-300',
      [ParcelStatus.Receiving]: 'bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400',
      [ParcelStatus.Received]: 'bg-green-100 dark:bg-green-900/30 text-green-600 dark:text-green-400',
      [ParcelStatus.Damaged]: 'bg-red-100 dark:bg-red-900/30 text-red-600 dark:text-red-400'
    };
    return classes[status] || 'bg-slate-100 dark:bg-slate-800';
  }

  getStatusLabel(status: ParcelStatus): string {
    return this.i18n.t(`parcels.status.${ParcelStatus[status].toLowerCase()}`);
  }
}
```

### 3.6 Template com i18n e Dark Mode

**Arquivo**: `APP/src/app/features/inbound-parcels/parcel-list/parcel-list.component.html`

```html
<!-- ✅ Container com dark mode -->
<div class="bg-white dark:bg-slate-900 rounded-lg shadow p-6">
  <!-- Header -->
  <div class="flex items-center justify-between mb-6">
    <h2 class="text-2xl font-bold text-slate-800 dark:text-slate-100">
      {{ i18n.t('parcels.title') }}
    </h2>
    <button 
      class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition"
      (click)="openCreateModal()">
      {{ i18n.t('parcels.buttons.addParcel') }}
    </button>
  </div>

  <!-- Progress Bar -->
  <div class="mb-6">
    <div class="flex justify-between mb-2">
      <span class="text-sm text-slate-600 dark:text-slate-400">
        {{ i18n.t('parcels.progress') }}
      </span>
      <span class="text-sm font-semibold text-slate-800 dark:text-slate-200">
        {{ receivedParcels() }}/{{ totalParcels() }} ({{ progress() }}%)
      </span>
    </div>
    <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-3">
      <div 
        class="bg-green-600 dark:bg-green-500 h-3 rounded-full transition-all duration-300"
        [style.width.%]="progress()">
      </div>
    </div>
  </div>

  <!-- Loading State -->
  @if (loading()) {
    <div class="text-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
      <p class="mt-4 text-slate-600 dark:text-slate-400">{{ i18n.t('common.loading') }}</p>
    </div>
  }

  <!-- Parcels List -->
  @else if (parcels().length > 0) {
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      @for (parcel of parcels(); track parcel.id) {
        <div class="border border-slate-200 dark:border-slate-700 rounded-lg p-4 hover:shadow-md transition">
          <!-- Parcel Header -->
          <div class="flex items-start justify-between mb-3">
            <div>
              <h3 class="font-semibold text-slate-800 dark:text-slate-100">
                {{ parcel.parcelNumber }}
              </h3>
              <p class="text-sm text-slate-500 dark:text-slate-400">
                {{ parcel.sequenceNumber }}/{{ parcel.totalParcels }}
              </p>
            </div>
            <span 
              class="px-2 py-1 rounded text-xs font-medium"
              [ngClass]="getStatusClass(parcel.status)">
              {{ getStatusLabel(parcel.status) }}
            </span>
          </div>

          <!-- Parcel Info -->
          <div class="space-y-2 text-sm">
            @if (parcel.lpn) {
              <div class="flex justify-between">
                <span class="text-slate-600 dark:text-slate-400">LPN:</span>
                <span class="font-mono text-slate-800 dark:text-slate-200">{{ parcel.lpn }}</span>
              </div>
            }
            <div class="flex justify-between">
              <span class="text-slate-600 dark:text-slate-400">{{ i18n.t('parcels.weight') }}:</span>
              <span class="text-slate-800 dark:text-slate-200">{{ parcel.weight }} kg</span>
            </div>
            <div class="flex justify-between">
              <span class="text-slate-600 dark:text-slate-400">{{ i18n.t('parcels.items') }}:</span>
              <span class="text-slate-800 dark:text-slate-200">{{ parcel.items.length }}</span>
            </div>
          </div>

          <!-- Actions -->
          <div class="mt-4 flex gap-2">
            @if (parcel.status === ParcelStatus.Pending || parcel.status === ParcelStatus.AtDock) {
              <button 
                class="flex-1 px-3 py-1.5 bg-green-600 hover:bg-green-700 text-white text-sm rounded transition"
                (click)="receiveParcel(parcel)">
                {{ i18n.t('parcels.buttons.receive') }}
              </button>
            }
            <button 
              class="px-3 py-1.5 border border-slate-300 dark:border-slate-600 hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-700 dark:text-slate-300 text-sm rounded transition"
              (click)="viewDetails(parcel)">
              {{ i18n.t('common.buttons.details') }}
            </button>
          </div>
        </div>
      }
    </div>
  }

  <!-- Empty State -->
  @else {
    <div class="text-center py-12">
      <p class="text-slate-600 dark:text-slate-400">{{ i18n.t('parcels.empty') }}</p>
    </div>
  }
</div>
```

### 3.7 Chaves i18n Necessárias

**Adicionar em**: `APP/src/assets/i18n/pt-BR.json`

```json
{
  "parcels": {
    "title": "Volumes/Parcels",
    "progress": "Progresso do Recebimento",
    "weight": "Peso",
    "items": "Itens",
    "empty": "Nenhum volume encontrado",
    "buttons": {
      "addParcel": "Adicionar Volume",
      "receive": "Receber"
    },
    "status": {
      "pending": "Pendente",
      "intransit": "Em Trânsito",
      "atdock": "No Dock",
      "receiving": "Recebendo",
      "received": "Recebido",
      "damaged": "Avariado",
      "quarantine": "Quarentena"
    }
  }
}
```

---

## 4. FLUXO WMS REAL - EMPRESAS GRANDES

### 4.1 Padrão SAP WM/EWM

**Como funciona em empresas que usam SAP**:

```
1. FORNECEDOR ENVIA ASN (Advanced Shipping Notice)
   ├─ Documento: Inbound Delivery
   ├─ Contém: Lista de volumes (HU - Handling Units)
   └─ Cada HU tem: Número único, peso, produtos

2. SISTEMA CRIA AUTOMATICAMENTE
   ├─ Inbound Delivery no SAP
   ├─ Handling Units (HU) vinculados
   └─ Expected GR (Goods Receipt)

3. CAMINHÃO CHEGA
   ├─ Scanner lê placa do veículo
   ├─ Check-in automático no dock door
   └─ Status: "At Dock"

4. OPERADOR ESCANEIA PALLET
   ├─ Lê barcode/RFID do HU
   ├─ Sistema mostra: "HU 1 de 50"
   ├─ Lista produtos esperados
   └─ Operador confirma ou ajusta

5. CONFERÊNCIA POR VOLUME
   ├─ Scan: Produto 1 → Quantidade OK ✓
   ├─ Scan: Produto 2 → Quantidade OK ✓
   ├─ HU completo → Status: "Received"
   └─ Automático: Cria Transfer Order (TO)

6. PUTAWAY (ENDEREÇAMENTO)
   ├─ Sistema sugere melhor localização
   ├─ Operador move HU para bin
   ├─ Confirma com scanner
   └─ Estoque atualizado

7. CROSS-DOCKING (se aplicável)
   ├─ Sistema detecta: HU tem pedido de saída
   ├─ Rota direta: Dock → Staging → Expedição
   └─ Pula armazenamento
```

### 4.2 Padrão Oracle WMS Cloud

**Fluxo Oracle**:

```
1. ASN INTEGRATION
   ├─ EDI 856 ou XML
   ├─ Cria Receipt com LPNs
   └─ LPN = License Plate Number (nosso Parcel)

2. LPN RECEIVING
   ├─ Mobile app escaneia LPN
   ├─ Valida contra ASN
   ├─ Operador confirma
   └─ Status: "Blind Receive" ou "ASN Receive"

3. QUALITY CHECK
   ├─ Se produto sensível → QC obrigatório
   ├─ LPN vai para zona de inspeção
   └─ Aprovado → Available

4. PUTAWAY RULES
   ├─ Sistema aplica estratégias:
   │   ├─ FIFO (First In First Out)
   │   ├─ ABC Classification
   │   └─ Cube utilization
   └─ Gera task de putaway

5. RF GUN CONFIRMATION
   ├─ Operador usa RF gun
   ├─ Scan LPN → Scan Location
   └─ Instant inventory update
```

### 4.3 Melhores Práticas Indústria

**1. LPN/SSCC Tracking**:
- Cada volume tem código único (barcode ou RFID)
- Formato: SSCC-18 (Serial Shipping Container Code)
- Permite rastreamento end-to-end

**2. ASN Detalhado**:
- Fornecedor envia antes da chegada
- Lista completa de volumes
- Packing list: volume → produtos

**3. Blind vs Expected Receive**:
- **Expected**: Sistema já sabe o que esperar (ASN)
- **Blind**: Operador registra tudo do zero
- Empresas grandes usam Expected (mais eficiente)

**4. Exception Handling**:
- Volume faltando → Status "Short"
- Volume extra → Status "Over"
- Avaria → Foto + relatório + quarentena

**5. Real-time Updates**:
- Cada scan atualiza dashboard
- Supervisor vê progresso em tempo real
- Alertas automáticos se atraso

---

## 5. INTEGRAÇÃO ORDERS + PARCELS

### 5.1 Cenário Real

**Compra de 1.000 notebooks**:

```
1. CRIAR ORDER (INBOUND)
   POST /api/orders
   {
     "type": 1,  // Inbound
     "orderNumber": "PO-2025-001",
     "supplierId": "guid-dell",
     "items": [
       { "productId": "guid-notebook", "sku": "DELL-001", "quantityOrdered": 1000 }
     ]
   }

2. CRIAR INBOUND SHIPMENT
   POST /api/inboundshipments
   {
     "orderId": "guid-order",
     "shipmentNumber": "ISH-2025-001",
     "supplierId": "guid-dell",
     "expectedArrivalDate": "2025-12-01"
   }

3. FORNECEDOR ENVIA ASN: 50 PALLETS
   POST /api/inboundshipments/{id}/asn
   {
     "asnNumber": "ASN-123456",
     "totalParcels": 50,
     "parcels": [
       {
         "parcelNumber": "PL-001",
         "lpn": "LPN000001",
         "type": 1,  // Pallet
         "sequenceNumber": 1,
         "totalParcels": 50,
         "weight": 250.5,
         "items": [
           { "productId": "guid-notebook", "sku": "DELL-001", "quantity": 20 }
         ]
       },
       // ... mais 49 pallets
     ]
   }

4. CAMINHÃO CHEGA
   ├─ Operador escaneia LPN000001
   ├─ Sistema mostra: "Pallet 1/50 - 20 notebooks"
   ├─ Confere e marca como recebido
   └─ Repete para todos os 50

5. DASHBOARD MOSTRA
   ╔═══════════════════════════════╗
   ║ PEDIDO PO-2025-001            ║
   ║ Remessa ISH-2025-001          ║
   ║ ───────────────────────────── ║
   ║ PROGRESSO: ██████░░ 35/50     ║
   ║ ✓ Recebidos:    35 pallets    ║
   ║ ⧗ Processando:   5 pallets    ║
   ║ ⊗ Pendentes:    10 pallets    ║
   ║ ⚠ Avariados:     2 pallets    ║
   ╚═══════════════════════════════╝
```

### 5.2 Vínculo Order → Parcel

**No Frontend**:

```typescript
// orders-detail.component.ts
export class OrderDetailComponent {
  order = signal<Order | null>(null);
  shipment = signal<InboundShipment | null>(null);
  parcels = signal<InboundParcel[]>([]);

  async loadOrderDetails(orderId: string): Promise<void> {
    // 1. Carregar order
    const orderResp = await this.ordersService.getById(orderId);
    this.order.set(orderResp.data);

    // 2. Buscar shipment vinculado
    const shipmentResp = await this.shipmentsService.getByOrderId(orderId);
    this.shipment.set(shipmentResp.data);

    // 3. Carregar parcels do shipment
    if (this.shipment()) {
      const parcelsResp = await this.parcelsService.getByShipment(this.shipment()!.id);
      this.parcels.set(parcelsResp.data);
    }
  }
}
```

**Template**:

```html
<div class="space-y-6">
  <!-- Order Info -->
  <div class="bg-white dark:bg-slate-900 p-6 rounded-lg">
    <h2>{{ order()?.orderNumber }}</h2>
    <p>Total: {{ order()?.totalQuantity }} unidades</p>
  </div>

  <!-- Shipment Info -->
  @if (shipment()) {
    <div class="bg-blue-50 dark:bg-blue-900/20 p-6 rounded-lg">
      <h3>Remessa: {{ shipment()?.shipmentNumber }}</h3>
      <p>{{ parcels().length }} volumes</p>
    </div>
  }

  <!-- Parcels List -->
  <app-parcel-list [shipmentId]="shipment()?.id" />
</div>
```

### 5.3 Fluxo Completo

```
USER FLOW:

1. Acessa "Pedidos" → Lista todos os pedidos
2. Clica em pedido PO-2025-001
3. Vê detalhes do pedido
4. Clica em "Ver Remessa" (se existe)
5. Dashboard mostra:
   ├─ Informações do pedido
   ├─ Status da remessa
   └─ Lista de volumes/parcels
6. Clica em "Receber Volume"
7. Modal abre com scanner
8. Escaneia LPN → Confirma
9. Volume marcado como recebido
10. Progress bar atualiza em tempo real
```

---

## 6. RESUMO E PRÓXIMOS PASSOS

### 6.1 Checklist de Implementação

**BACKEND** (API):
- [x] Criar entidades InboundParcel e InboundParcelItem
- [x] Criar enums ParcelType e ParcelStatus
- [x] Configurar EF Core (Fluent API)
- [ ] Criar migration `AddInboundParcelTracking`
- [ ] Aplicar migration no banco
- [ ] Criar repository
- [ ] Criar service
- [ ] Criar controller com [Authorize]
- [ ] Registrar DI no Program.cs
- [ ] Testar endpoints com Postman/curl

**FRONTEND** (Angular):
- [ ] Criar models (interfaces)
- [ ] Criar service
- [ ] Criar componente lista
- [ ] Criar modal de criação
- [ ] Criar modal de recebimento
- [ ] Adicionar chaves i18n (pt-BR, en-US, es-ES)
- [ ] Aplicar classes dark mode
- [ ] Adicionar AuthGuard nas rotas
- [ ] Integrar com Orders
- [ ] Testar fluxo completo

**VALIDAÇÃO**:
- [ ] Build backend sem erros
- [ ] Build frontend sem erros
- [ ] Migrations aplicadas
- [ ] Auth funciona em todos endpoints
- [ ] i18n completo nos 3 idiomas
- [ ] Dark mode funciona
- [ ] Fluxo Orders → Shipment → Parcels

### 6.2 Estimativa Final

- **Backend**: ~20h
- **Frontend**: ~18h
- **Integração + Testes**: ~6h
- **TOTAL**: ~44 horas (1 semana)

### 6.3 Prioridade

🔴 **CRÍTICO**: Backend entities + migrations  
🟡 **IMPORTANTE**: Frontend básico  
🟢 **DESEJÁVEL**: Features avançadas

---

---

## 7. PURCHASE ORDERS (PEDIDOS DE COMPRA) - FLUXO COMPLETO

### 7.1 Visão Geral do Processo

**Cenário Real**: Compra de 1.000 computadores do fornecedor Dell

```
📦 HIERARQUIA COMPLETA:

Purchase Order PO-2025-001 (1.000 notebooks)
  ├─ Fornecedor: Dell Inc.
  ├─ Valor Total: R$ 2.500.000,00
  ├─ Preço Unitário: R$ 2.500,00
  └─ Logística:
      ├─ Veículo: Caminhão ABC-1234
      ├─ Motorista: João Silva
      ├─ Dock Door: DOCK-01
      └─ Frete: 850 km

  └── InboundShipment ISH-2025-001
      ├─ ASN: ASN-DELL-123456
      ├─ Total Parcels: 10
      └─ Expected: 1.000 notebooks

      └── Parcel #1 (Pallet PL-001) - LPN: SSCC0001
          ├─ Sequência: 1 de 10
          ├─ Peso: 250 kg
          ├─ Quantidade: 100 notebooks
          └─ Caixas: 10 caixas (Cartons)

          └── Carton #1 (CTN-001-01) - Código de Barras: EAN128001
              ├─ Sequência: 1 de 10
              ├─ Peso: 25 kg
              ├─ Quantidade: 10 notebooks
              └─ Produtos:

              └── Produto (Notebook Dell Inspiron 15)
                  ├─ SKU: DELL-INSP-15-001
                  ├─ Serial Number: SN123456789
                  ├─ Preço Compra: R$ 2.500,00
                  └─ Rastreabilidade:
                      ├─ Fornecedor: Dell Inc.
                      ├─ Pallet: PL-001
                      ├─ Caixa: CTN-001-01
                      └─ Data Recebimento: 2025-12-01
```

### 7.2 Purchase Order - Entidade Atualizada

**Arquivo**: `API/src/Logistics.Domain/Entities/Order.cs` (já existe, adicionar campos)

```csharp
public class Order
{
    // ... propriedades existentes ...
    
    // 🆕 CAMPOS PARA PURCHASE ORDER (COMPRAS)
    
    // Preços e Custos
    public decimal UnitCost { get; private set; }              // Preço de compra unitário
    public decimal TotalCost { get; private set; }             // Custo total da compra
    public decimal TaxAmount { get; private set; }             // Valor de impostos
    public decimal TaxPercentage { get; private set; }         // % de impostos
    
    // Cálculo de Margem (automático)
    public decimal DesiredMarginPercentage { get; private set; }  // Margem desejada %
    public decimal SuggestedSalePrice { get; private set; }       // Preço de venda sugerido
    public decimal EstimatedProfit { get; private set; }          // Lucro estimado
    
    // Parcels/Volumes Esperados
    public int ExpectedParcels { get; private set; }           // Qtd de parcels/pallets esperados
    public int ReceivedParcels { get; private set; }           // Qtd de parcels recebidos
    
    // Hierarquia de Embalagem Esperada
    public int ExpectedCartons { get; private set; }           // Qtd total de caixas esperadas
    public int UnitsPerCarton { get; private set; }            // Unidades por caixa
    public int CartonsPerParcel { get; private set; }          // Caixas por pallet
    
    // Logística de Entrega
    public string? ShippingDistance { get; private set; }     // Distância em km
    public decimal ShippingCost { get; private set; }         // Custo de frete
    
    // 🆕 MÉTODOS DE NEGÓCIO
    
    public void SetPurchaseDetails(
        decimal unitCost,
        decimal taxPercentage,
        decimal desiredMarginPercentage)
    {
        if (unitCost <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero");
        if (taxPercentage < 0 || taxPercentage > 100)
            throw new ArgumentException("Percentual de imposto inválido");
        if (desiredMarginPercentage < 0)
            throw new ArgumentException("Margem não pode ser negativa");

        UnitCost = unitCost;
        TaxPercentage = taxPercentage;
        DesiredMarginPercentage = desiredMarginPercentage;
        
        // Cálculos automáticos
        TotalCost = UnitCost * TotalQuantity;
        TaxAmount = TotalCost * (TaxPercentage / 100);
        
        // Preço de venda sugerido: Custo + Impostos + Margem
        var costWithTax = UnitCost + (UnitCost * (TaxPercentage / 100));
        SuggestedSalePrice = costWithTax + (costWithTax * (DesiredMarginPercentage / 100));
        EstimatedProfit = (SuggestedSalePrice - costWithTax) * TotalQuantity;
        
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetPackagingHierarchy(
        int expectedParcels,
        int cartonsPerParcel,
        int unitsPerCarton)
    {
        if (expectedParcels <= 0 || cartonsPerParcel <= 0 || unitsPerCarton <= 0)
            throw new ArgumentException("Valores de hierarquia devem ser maiores que zero");

        ExpectedParcels = expectedParcels;
        CartonsPerParcel = cartonsPerParcel;
        UnitsPerCarton = unitsPerCarton;
        ExpectedCartons = expectedParcels * cartonsPerParcel;
        
        // Validação: total deve bater com quantidade do pedido
        var calculatedTotal = expectedParcels * cartonsPerParcel * unitsPerCarton;
        if (calculatedTotal != TotalQuantity)
            throw new InvalidOperationException(
                $"Hierarquia inconsistente: {expectedParcels} parcels × {cartonsPerParcel} caixas × {unitsPerCarton} unidades = {calculatedTotal}, esperado {TotalQuantity}");
        
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetShippingLogistics(string distance, decimal shippingCost)
    {
        ShippingDistance = distance;
        ShippingCost = shippingCost;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void IncrementReceivedParcels()
    {
        ReceivedParcels++;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### 7.3 Nova Entidade: Carton (Caixa)

**Arquivo**: `API/src/Logistics.Domain/Entities/InboundCarton.cs` (NOVO)

```csharp
using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class InboundCarton
{
    private InboundCarton() { }

    public InboundCarton(Guid inboundParcelId, string cartonNumber, int sequenceNumber, int totalCartons)
    {
        if (inboundParcelId == Guid.Empty)
            throw new ArgumentException("InboundParcelId inválido");
        if (string.IsNullOrWhiteSpace(cartonNumber))
            throw new ArgumentException("CartonNumber inválido");
        if (sequenceNumber <= 0 || totalCartons <= 0)
            throw new ArgumentException("Sequência inválida");
        if (sequenceNumber > totalCartons)
            throw new ArgumentException("Sequência não pode ser maior que total");

        Id = Guid.NewGuid();
        InboundParcelId = inboundParcelId;
        CartonNumber = cartonNumber;
        SequenceNumber = sequenceNumber;
        TotalCartons = totalCartons;
        Status = CartonStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid InboundParcelId { get; private set; }
    public string CartonNumber { get; private set; } = string.Empty;
    public string? Barcode { get; private set; }              // EAN-128, UPC, etc.
    public int SequenceNumber { get; private set; }            // 1 de 10
    public int TotalCartons { get; private set; }              // 10 total
    
    public decimal Weight { get; private set; }
    public decimal Length { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }
    
    public CartonStatus Status { get; private set; }
    public DateTime? ReceivedAt { get; private set; }
    public Guid? ReceivedBy { get; private set; }
    
    public bool HasDamage { get; private set; }
    public string? DamageNotes { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    public InboundParcel InboundParcel { get; private set; } = null!;
    public ICollection<InboundCartonItem> Items { get; private set; } = new List<InboundCartonItem>();

    public void SetBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            throw new ArgumentException("Barcode inválido");
        Barcode = barcode;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDimensions(decimal weight, decimal length, decimal width, decimal height)
    {
        if (weight < 0 || length < 0 || width < 0 || height < 0)
            throw new ArgumentException("Dimensões não podem ser negativas");
        
        Weight = weight;
        Length = length;
        Width = width;
        Height = height;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsReceived(Guid receivedBy)
    {
        Status = CartonStatus.Received;
        ReceivedAt = DateTime.UtcNow;
        ReceivedBy = receivedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReportDamage(string notes)
    {
        HasDamage = true;
        DamageNotes = notes;
        Status = CartonStatus.Damaged;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

**Arquivo**: `API/src/Logistics.Domain/Entities/InboundCartonItem.cs` (NOVO)

```csharp
namespace Logistics.Domain.Entities;

public class InboundCartonItem
{
    private InboundCartonItem() { }

    public InboundCartonItem(Guid inboundCartonId, Guid productId, string sku, string? serialNumber = null)
    {
        if (inboundCartonId == Guid.Empty)
            throw new ArgumentException("InboundCartonId inválido");
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId inválido");
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU inválido");

        Id = Guid.NewGuid();
        InboundCartonId = inboundCartonId;
        ProductId = productId;
        SKU = sku;
        SerialNumber = serialNumber;
        IsReceived = false;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid InboundCartonId { get; private set; }
    public Guid ProductId { get; private set; }
    public string SKU { get; private set; } = string.Empty;
    public string? SerialNumber { get; private set; }         // Serial individual do produto
    public bool IsReceived { get; private set; }              // Se foi escaneado/recebido
    public DateTime? ReceivedAt { get; private set; }
    public Guid? ReceivedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public InboundCarton InboundCarton { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    public void MarkAsReceived(Guid receivedBy)
    {
        IsReceived = true;
        ReceivedAt = DateTime.UtcNow;
        ReceivedBy = receivedBy;
    }
}
```

**Enums**: `API/src/Logistics.Domain/Enums/CartonStatus.cs` (NOVO)

```csharp
namespace Logistics.Domain.Enums;

public enum CartonStatus
{
    Pending = 0,
    Receiving = 1,
    Received = 2,
    Damaged = 3,
    Quarantine = 4,
    Stored = 5
}
```

### 7.4 Fluxo Completo - Purchase Order

#### **PASSO 1: Criar Purchase Order**

**Endpoint**: `POST /api/orders`

```json
{
  "companyId": "guid-empresa",
  "orderNumber": "PO-2025-001",
  "type": 1,                    // Inbound (compra)
  "source": 1,                  // Manual
  "supplierId": "guid-dell",
  "expectedDate": "2025-12-01",
  
  // 🆕 LOGÍSTICA
  "vehicleId": "guid-caminhao-abc1234",
  "driverId": "guid-joao-silva",
  "destinationWarehouseId": "guid-warehouse-sp",
  "dockDoorNumber": "DOCK-01",
  "shippingDistance": "850 km",
  "shippingCost": 5000.00,
  
  // 🆕 PREÇOS E CÁLCULOS
  "unitCost": 2500.00,           // Preço de compra unitário
  "taxPercentage": 18.0,         // ICMS + IPI
  "desiredMarginPercentage": 30.0,  // Margem desejada 30%
  
  // 🆕 HIERARQUIA DE EMBALAGEM
  "expectedParcels": 10,         // 10 pallets
  "cartonsPerParcel": 10,        // 10 caixas por pallet
  "unitsPerCarton": 10,          // 10 notebooks por caixa
  
  "items": [
    {
      "productId": "guid-notebook-dell",
      "sku": "DELL-INSP-15-001",
      "quantityOrdered": 1000,   // Total: 1.000 notebooks
      "unitPrice": 2500.00
    }
  ]
}
```

**Response Automático**:
```json
{
  "id": "guid-order",
  "orderNumber": "PO-2025-001",
  "totalQuantity": 1000,
  "totalCost": 2500000.00,
  "taxAmount": 450000.00,         // 18% de 2.500.000
  "suggestedSalePrice": 3835.00,  // Calculado automaticamente
  "estimatedProfit": 885000.00,   // Lucro total estimado
  "expectedParcels": 10,
  "expectedCartons": 100           // 10 pallets × 10 caixas
}
```

#### **PASSO 2: Sistema Cria InboundShipment + Parcels + Cartons**

**Quando Order é confirmado, sistema cria automaticamente**:

```csharp
// Service: OrderService.CreateAsync
public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, Guid createdBy)
{
    // ... código existente de criação do Order ...
    
    // 🆕 Se for Purchase Order (Inbound), criar estrutura de recebimento
    if (order.Type == OrderType.Inbound)
    {
        // 1. Criar InboundShipment
        var shipment = new InboundShipment(
            order.CompanyId,
            $"ISH-{DateTime.UtcNow:yyyyMMdd}-{order.OrderNumber}",
            order.Id,
            order.SupplierId.Value
        );
        shipment.SetExpectedArrival(order.ExpectedDate.Value, request.DockDoorNumber);
        await _shipmentRepository.AddAsync(shipment);
        
        // 2. Criar Parcels conforme hierarquia
        for (int p = 1; p <= order.ExpectedParcels; p++)
        {
            var parcel = new InboundParcel(
                shipment.Id,
                $"PL-{p:D3}",
                ParcelType.Pallet
            );
            parcel.SetSequence(p, order.ExpectedParcels);
            await _parcelRepository.AddAsync(parcel);
            
            // 3. Criar Cartons dentro de cada Parcel
            for (int c = 1; c <= order.CartonsPerParcel; c++)
            {
                var carton = new InboundCarton(
                    parcel.Id,
                    $"CTN-{p:D3}-{c:D2}",
                    c,
                    order.CartonsPerParcel
                );
                await _cartonRepository.AddAsync(carton);
                
                // 4. Criar Items esperados em cada Carton
                for (int u = 1; u <= order.UnitsPerCarton; u++)
                {
                    var item = new InboundCartonItem(
                        carton.Id,
                        order.Items.First().ProductId,  // Simplificado
                        order.Items.First().SKU
                    );
                    // Nota: Serial numbers serão preenchidos no recebimento
                }
            }
        }
    }
    
    await _unitOfWork.CommitAsync();
    return MapToResponse(order);
}
```

### 7.5 Fluxo de Recebimento com PDA/Scanner

#### **CENÁRIO: Caminhão Dell chega com 10 pallets**

```
📱 PDA/SCANNER WORKFLOW:

1️⃣ OPERADOR ESCANEIA PALLET
   Scan: LPN SSCC0001 (código de barras do pallet)
   
   GET /api/inboundparcels/lpn/SSCC0001
   
   Response:
   {
     "id": "guid-parcel-1",
     "parcelNumber": "PL-001",
     "sequenceNumber": 1,
     "totalParcels": 10,
     "status": "Pending",
     "order": {
       "orderNumber": "PO-2025-001",
       "supplier": "Dell Inc."
     },
     "expectedItems": [
       {
         "sku": "DELL-INSP-15-001",
         "description": "Notebook Dell Inspiron 15",
         "expectedQuantity": 100,
         "expectedCartons": 10
       }
     ]
   }
   
   🖥️ PDA MOSTRA:
   ╔═══════════════════════════════════╗
   ║ ✓ PALLET IDENTIFICADO            ║
   ║ PO-2025-001 | Dell Inc.          ║
   ║ Pallet 1 de 10                    ║
   ║ ───────────────────────────────── ║
   ║ Esperado neste pallet:            ║
   ║ • 100x Notebook Dell Inspiron 15  ║
   ║ • 10 caixas                       ║
   ║ ───────────────────────────────── ║
   ║ [Iniciar Conferência]             ║
   ╚═══════════════════════════════════╝

2️⃣ OPERADOR ESCANEIA CAIXAS
   Scan: EAN128001 (código da caixa 1)
   
   POST /api/inboundcartons/scan
   {
     "barcode": "EAN128001",
     "parcelId": "guid-parcel-1"
   }
   
   Response:
   {
     "cartonNumber": "CTN-001-01",
     "sequenceNumber": 1,
     "totalCartons": 10,
     "expectedUnits": 10,
     "product": {
       "sku": "DELL-INSP-15-001",
       "description": "Notebook Dell Inspiron 15"
     }
   }
   
   🖥️ PDA MOSTRA:
   ╔═══════════════════════════════════╗
   ║ ✓ CAIXA 1 de 10                  ║
   ║ CTN-001-01                        ║
   ║ ───────────────────────────────── ║
   ║ Esperado: 10 notebooks            ║
   ║ Escaneie cada produto:            ║
   ║                                   ║
   ║ Recebidos: 0/10 ░░░░░░░░░░        ║
   ╚═══════════════════════════════════╝

3️⃣ OPERADOR ESCANEIA PRODUTOS INDIVIDUAIS
   Scan: SN123456789 (serial do notebook 1)
   
   POST /api/inboundcartons/{cartonId}/items/scan
   {
     "serialNumber": "SN123456789",
     "sku": "DELL-INSP-15-001"
   }
   
   🖥️ PDA ATUALIZA:
   ╔═══════════════════════════════════╗
   ║ ✓ CAIXA 1 de 10                  ║
   ║ Recebidos: 1/10 ██░░░░░░░░        ║
   ║                                   ║
   ║ • SN123456789 ✓                   ║
   ╚═══════════════════════════════════╝
   
   Repete scan para os 10 notebooks da caixa...
   
   Quando 10/10:
   ╔═══════════════════════════════════╗
   ║ ✅ CAIXA COMPLETA!                ║
   ║ CTN-001-01: 10/10 notebooks       ║
   ║ ───────────────────────────────── ║
   ║ [Próxima Caixa] [Reportar Avaria] ║
   ╚═══════════════════════════════════╝

4️⃣ REPETE PARA 10 CAIXAS
   Caixa 1: ✅ 10/10
   Caixa 2: ✅ 10/10
   ...
   Caixa 10: ✅ 10/10
   
   🖥️ PDA MOSTRA:
   ╔═══════════════════════════════════╗
   ║ ✅ PALLET COMPLETO!               ║
   ║ PL-001: 100/100 notebooks         ║
   ║ 10/10 caixas conferidas           ║
   ║ ───────────────────────────────── ║
   ║ [Finalizar Pallet] [Ver Resumo]   ║
   ╚═══════════════════════════════════╝

5️⃣ SISTEMA ATUALIZA AUTOMATICAMENTE
   - Parcel PL-001: Status → Received
   - Order PO-2025-001: ReceivedParcels = 1/10
   - Estoque: +100 notebooks Dell (com rastreabilidade)
   - Cada produto sabe:
     • Fornecedor: Dell Inc.
     • Purchase Order: PO-2025-001
     • Pallet: PL-001
     • Caixa: CTN-001-01
     • Serial Number: SN123456789
     • Data Recebimento: 2025-12-01 14:32:15
```

### 7.6 Dashboard de Recebimento (Frontend)

**Componente**: `purchase-order-receiving-dashboard.component.html`

```html
<div class="bg-white dark:bg-slate-900 p-6 rounded-lg">
  <!-- Header do Pedido -->
  <div class="mb-6">
    <h1 class="text-3xl font-bold text-slate-800 dark:text-slate-100">
      {{ i18n.t('purchaseOrders.receiving.title') }}
    </h1>
    <p class="text-slate-600 dark:text-slate-400 mt-2">
      {{ order()?.orderNumber }} | {{ order()?.supplier?.name }}
    </p>
  </div>

  <!-- Progress Geral -->
  <div class="grid grid-cols-4 gap-4 mb-6">
    <div class="bg-blue-50 dark:bg-blue-900/20 p-4 rounded-lg">
      <div class="text-sm text-slate-600 dark:text-slate-400">
        {{ i18n.t('purchaseOrders.totalOrdered') }}
      </div>
      <div class="text-2xl font-bold text-blue-600 dark:text-blue-400">
        {{ order()?.totalQuantity }}
      </div>
    </div>
    
    <div class="bg-green-50 dark:bg-green-900/20 p-4 rounded-lg">
      <div class="text-sm text-slate-600 dark:text-slate-400">
        {{ i18n.t('purchaseOrders.received') }}
      </div>
      <div class="text-2xl font-bold text-green-600 dark:text-green-400">
        {{ receivedQuantity() }}
      </div>
    </div>
    
    <div class="bg-amber-50 dark:bg-amber-900/20 p-4 rounded-lg">
      <div class="text-sm text-slate-600 dark:text-slate-400">
        {{ i18n.t('purchaseOrders.parcels') }}
      </div>
      <div class="text-2xl font-bold text-amber-600 dark:text-amber-400">
        {{ order()?.receivedParcels }}/{{ order()?.expectedParcels }}
      </div>
    </div>
    
    <div class="bg-purple-50 dark:bg-purple-900/20 p-4 rounded-lg">
      <div class="text-sm text-slate-600 dark:text-slate-400">
        {{ i18n.t('purchaseOrders.cartons') }}
      </div>
      <div class="text-2xl font-bold text-purple-600 dark:text-purple-400">
        {{ receivedCartons() }}/{{ order()?.expectedCartons }}
      </div>
    </div>
  </div>

  <!-- Progress Bar Detalhado -->
  <div class="mb-6">
    <div class="flex justify-between mb-2">
      <span class="text-sm font-medium text-slate-700 dark:text-slate-300">
        {{ i18n.t('purchaseOrders.receiving.progress') }}
      </span>
      <span class="text-sm font-semibold text-slate-800 dark:text-slate-200">
        {{ receivedQuantity() }}/{{ order()?.totalQuantity }} ({{ progress() }}%)
      </span>
    </div>
    <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-4">
      <div 
        class="bg-gradient-to-r from-green-500 to-green-600 h-4 rounded-full transition-all duration-500 flex items-center justify-end pr-2"
        [style.width.%]="progress()">
        <span class="text-xs text-white font-bold">{{ progress() }}%</span>
      </div>
    </div>
  </div>

  <!-- Lista de Pallets -->
  <div class="space-y-4">
    @for (parcel of parcels(); track parcel.id) {
      <div class="border border-slate-200 dark:border-slate-700 rounded-lg p-4">
        <!-- Header do Pallet -->
        <div class="flex items-center justify-between mb-4">
          <div>
            <h3 class="text-lg font-semibold text-slate-800 dark:text-slate-100">
              {{ parcel.parcelNumber }} ({{ parcel.sequenceNumber }}/{{ parcel.totalParcels }})
            </h3>
            @if (parcel.lpn) {
              <p class="text-sm text-slate-500 dark:text-slate-400">
                LPN: <span class="font-mono">{{ parcel.lpn }}</span>
              </p>
            }
          </div>
          <span 
            class="px-3 py-1 rounded-full text-sm font-medium"
            [ngClass]="getParcelStatusClass(parcel.status)">
            {{ getParcelStatusLabel(parcel.status) }}
          </span>
        </div>

        <!-- Progress do Pallet -->
        <div class="mb-4">
          <div class="flex justify-between text-sm mb-1">
            <span class="text-slate-600 dark:text-slate-400">
              {{ i18n.t('purchaseOrders.cartons') }}
            </span>
            <span class="font-semibold text-slate-800 dark:text-slate-200">
              {{ getParcelReceivedCartons(parcel) }}/{{ order()?.cartonsPerParcel }}
            </span>
          </div>
          <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2">
            <div 
              class="bg-blue-600 dark:bg-blue-500 h-2 rounded-full transition-all"
              [style.width.%]="getParcelProgress(parcel)">
            </div>
          </div>
        </div>

        <!-- Caixas do Pallet -->
        <div class="grid grid-cols-5 gap-2">
          @for (carton of getParcelCartons(parcel); track carton.id) {
            <div 
              class="p-2 rounded border text-center text-sm"
              [ngClass]="getCartonClass(carton)">
              <div class="font-mono text-xs">{{ carton.cartonNumber }}</div>
              <div class="text-xs mt-1">
                {{ getCartonReceivedCount(carton) }}/{{ order()?.unitsPerCarton }}
              </div>
            </div>
          }
        </div>

        <!-- Actions -->
        <div class="mt-4 flex gap-2">
          @if (parcel.status !== ParcelStatus.Received) {
            <button 
              class="px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg transition"
              (click)="openScanModal(parcel)">
              {{ i18n.t('purchaseOrders.buttons.scanParcel') }}
            </button>
          }
          <button 
            class="px-4 py-2 border border-slate-300 dark:border-slate-600 hover:bg-slate-100 dark:hover:bg-slate-800 text-slate-700 dark:text-slate-300 rounded-lg transition"
            (click)="viewParcelDetails(parcel)">
            {{ i18n.t('common.buttons.details') }}
          </button>
        </div>
      </div>
    }
  </div>
</div>
```

### 7.7 Rastreabilidade Completa

**Quando produto é vendido, sistema mostra origem**:

```typescript
// product-detail.component.ts
export class ProductDetailComponent {
  async loadProductHistory(productId: string): Promise<void> {
    const history = await this.productsService.getTraceability(productId);
    
    /*
    Response:
    {
      "product": {
        "sku": "DELL-INSP-15-001",
        "serialNumber": "SN123456789",
        "currentStock": "SHELF-A-12-03"
      },
      "origin": {
        "supplier": "Dell Inc.",
        "purchaseOrder": "PO-2025-001",
        "purchaseDate": "2025-11-27",
        "unitCost": 2500.00,
        "receivedDate": "2025-12-01 14:32:15"
      },
      "packaging": {
        "parcel": "PL-001",
        "parcelLPN": "SSCC0001",
        "carton": "CTN-001-01",
        "cartonBarcode": "EAN128001"
      },
      "logistics": {
        "vehicle": "ABC-1234",
        "driver": "João Silva",
        "dockDoor": "DOCK-01",
        "distance": "850 km",
        "shippingCost": 5000.00
      },
      "receivedBy": {
        "user": "Maria Santos",
        "timestamp": "2025-12-01 14:32:15"
      }
    }
    */
  }
}
```

**Template de Rastreabilidade**:

```html
<div class="bg-white dark:bg-slate-900 rounded-lg p-6">
  <h2 class="text-xl font-bold mb-4">{{ i18n.t('products.traceability.title') }}</h2>
  
  <!-- Timeline de Rastreabilidade -->
  <div class="space-y-4">
    <div class="flex gap-4">
      <div class="flex flex-col items-center">
        <div class="w-10 h-10 bg-blue-600 rounded-full flex items-center justify-center text-white">
          🏭
        </div>
        <div class="w-0.5 h-full bg-slate-200 dark:bg-slate-700"></div>
      </div>
      <div class="flex-1 pb-8">
        <h3 class="font-semibold">{{ i18n.t('products.traceability.supplier') }}</h3>
        <p>{{ history()?.origin.supplier }}</p>
        <p class="text-sm text-slate-500">PO: {{ history()?.origin.purchaseOrder }}</p>
        <p class="text-sm text-slate-500">{{ history()?.origin.purchaseDate }}</p>
      </div>
    </div>

    <div class="flex gap-4">
      <div class="flex flex-col items-center">
        <div class="w-10 h-10 bg-green-600 rounded-full flex items-center justify-center text-white">
          📦
        </div>
        <div class="w-0.5 h-full bg-slate-200 dark:bg-slate-700"></div>
      </div>
      <div class="flex-1 pb-8">
        <h3 class="font-semibold">{{ i18n.t('products.traceability.packaging') }}</h3>
        <p>Pallet: {{ history()?.packaging.parcel }} ({{ history()?.packaging.parcelLPN }})</p>
        <p>Caixa: {{ history()?.packaging.carton }} ({{ history()?.packaging.cartonBarcode }})</p>
      </div>
    </div>

    <div class="flex gap-4">
      <div class="flex flex-col items-center">
        <div class="w-10 h-10 bg-purple-600 rounded-full flex items-center justify-center text-white">
          🚚
        </div>
        <div class="w-0.5 h-full bg-slate-200 dark:bg-slate-700"></div>
      </div>
      <div class="flex-1 pb-8">
        <h3 class="font-semibold">{{ i18n.t('products.traceability.logistics') }}</h3>
        <p>Veículo: {{ history()?.logistics.vehicle }}</p>
        <p>Motorista: {{ history()?.logistics.driver }}</p>
        <p>Dock: {{ history()?.logistics.dockDoor }}</p>
        <p class="text-sm text-slate-500">{{ history()?.logistics.distance }}</p>
      </div>
    </div>

    <div class="flex gap-4">
      <div class="w-10 h-10 bg-amber-600 rounded-full flex items-center justify-center text-white">
        ✓
      </div>
      <div class="flex-1">
        <h3 class="font-semibold">{{ i18n.t('products.traceability.received') }}</h3>
        <p>Por: {{ history()?.receivedBy.user }}</p>
        <p class="text-sm text-slate-500">{{ history()?.receivedBy.timestamp }}</p>
      </div>
    </div>
  </div>
</div>
```

### 7.8 Endpoints Completos - Purchase Orders

```
POST   /api/orders                              # Criar Purchase Order
GET    /api/orders/{id}/receiving-dashboard     # Dashboard de recebimento
GET    /api/orders/{id}/traceability            # Rastreabilidade completa

POST   /api/inboundparcels/lpn/{lpn}/start      # Iniciar recebimento de pallet
POST   /api/inboundparcels/{id}/complete        # Finalizar pallet

POST   /api/inboundcartons/scan                 # Escanear caixa
POST   /api/inboundcartons/{id}/items/scan      # Escanear produto individual
GET    /api/inboundcartons/{id}/progress        # Progresso da caixa

GET    /api/products/{id}/traceability          # Rastreabilidade do produto
```

### 7.9 Compra Nacional vs Internacional + Documentos

#### **7.9.1 Campos Adicionais na Entidade Order**

**Arquivo**: `API/src/Logistics.Domain/Entities/Order.cs` (adicionar mais campos)

```csharp
public class Order
{
    // ... campos existentes ...
    
    // 🆕 COMPRA NACIONAL vs INTERNACIONAL
    public bool IsInternational { get; private set; }          // Nacional ou Internacional?
    public string? OriginCountry { get; private set; }         // País de origem (se internacional)
    
    // 🆕 INFORMAÇÕES DE IMPORTAÇÃO (obrigatório se IsInternational = true)
    public string? PortOfEntry { get; private set; }           // Porto de entrada (Santos, Paranaguá, etc.)
    public string? CustomsBroker { get; private set; }         // Despachante aduaneiro
    public bool IsOwnCarrier { get; private set; }             // Transportadora própria?
    public string? ThirdPartyCarrier { get; private set; }     // Nome da trade/transportadora terceira
    public string? ContainerNumber { get; private set; }       // Número do container (ex: MSCU1234567)
    public string? BillOfLading { get; private set; }          // Bill of Lading (BL)
    public string? ImportLicenseNumber { get; private set; }   // LI - Licença de Importação
    public DateTime? EstimatedArrivalPort { get; private set; } // ETA no porto
    public DateTime? ActualArrivalPort { get; private set; }   // Data real de chegada no porto
    public string? Incoterm { get; private set; }              // FOB, CIF, EXW, etc.
    
    // Navigation para documentos
    public ICollection<OrderDocument> Documents { get; private set; } = new List<OrderDocument>();
    
    // 🆕 MÉTODOS DE NEGÓCIO
    
    public void SetAsInternational(
        string originCountry,
        string portOfEntry,
        string containerNumber,
        string incoterm)
    {
        if (string.IsNullOrWhiteSpace(originCountry))
            throw new ArgumentException("País de origem é obrigatório para importação");
        if (string.IsNullOrWhiteSpace(portOfEntry))
            throw new ArgumentException("Porto de entrada é obrigatório para importação");
        if (string.IsNullOrWhiteSpace(containerNumber))
            throw new ArgumentException("Número do container é obrigatório para importação");
        if (string.IsNullOrWhiteSpace(incoterm))
            throw new ArgumentException("Incoterm é obrigatório para importação");

        IsInternational = true;
        OriginCountry = originCountry;
        PortOfEntry = portOfEntry;
        ContainerNumber = containerNumber;
        Incoterm = incoterm;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetImportDetails(
        string? customsBroker,
        bool isOwnCarrier,
        string? thirdPartyCarrier,
        string? billOfLading,
        string? importLicenseNumber,
        DateTime? estimatedArrivalPort)
    {
        if (!IsInternational)
            throw new InvalidOperationException("Order não é internacional");

        CustomsBroker = customsBroker;
        IsOwnCarrier = isOwnCarrier;
        ThirdPartyCarrier = thirdPartyCarrier;
        BillOfLading = billOfLading;
        ImportLicenseNumber = importLicenseNumber;
        EstimatedArrivalPort = estimatedArrivalPort;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetActualPortArrival(DateTime arrivalDate)
    {
        if (!IsInternational)
            throw new InvalidOperationException("Order não é internacional");
        
        ActualArrivalPort = arrivalDate;
}

#### **7.9.2- ✅ Soft Delete em OrderDocuments
- ✅ Upload de documentos com conversão WebP
- ✅ Endpoints de impressão (placeholder)
- ✅ **Tabelas separadas: PurchaseOrders ≠ SalesOrders [27/11/2025]**
- ✅ **Campos logísticos completos (WarehouseId, VehicleId, DriverId) [27/11/2025]**
- ✅ **ProductCategories com relacionamento [27/11/2025]**

#### **7.9.2 Entidade OrderDocument (Upload de Documentos)**

**Arquivo**: `API/src/Logistics.Domain/Entities/OrderDocument.cs` (NOVO)

```csharp
using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class OrderDocument
{
    private OrderDocument() { }

    public OrderDocument(Guid orderId, string fileName, DocumentType type, Guid uploadedBy)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("OrderId inválido");
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("FileName inválido");

        Id = Guid.NewGuid();
        OrderId = orderId;
        FileName = fileName;
        Type = type;
        UploadedBy = uploadedBy;
        UploadedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public DocumentType Type { get; private set; }              // Invoice, DI, PackingList, etc.
    public string FilePath { get; private set; } = string.Empty;  // Path no storage
    public string FileUrl { get; private set; } = string.Empty;   // URL pública (se aplicável)
    public long FileSizeBytes { get; private set; }              // Tamanho em bytes
    public string MimeType { get; private set; } = "image/webp"; // Sempre WebP
    public Guid UploadedBy { get; private set; }
    public DateTime UploadedAt { get; private set; }
    
    // Navigation
    public Order Order { get; private set; } = null!;

    public void SetFilePath(string filePath, string fileUrl, long sizeBytes)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("FilePath inválido");

        FilePath = filePath;
        FileUrl = fileUrl;
        FileSizeBytes = sizeBytes;
    }
}
```

**Enum**: `API/src/Logistics.Domain/Enums/DocumentType.cs` (NOVO)

```csharp
namespace Logistics.Domain.Enums;

public enum DocumentType
{
    Invoice = 1,                    // Fatura comercial
    PackingList = 2,                // Lista de embalagem
    ImportDeclaration = 3,          // DI - Declaração de Importação
    BillOfLading = 4,               // BL - Conhecimento de embarque
    ImportLicense = 5,              // LI - Licença de Importação
    CertificateOfOrigin = 6,        // Certificado de Origem
    CustomsDocumentation = 7,       // Documentos aduaneiros
    Other = 99                      // Outros
}
```

#### **7.9.3 Upload de Documentos em WebP**

**Service**: `API/src/Logistics.Application/Services/DocumentService.cs` (NOVO)

```csharp
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Logistics.Domain.Interfaces;

namespace Logistics.Application.Services;

public class DocumentService
{
    private readonly IOrderDocumentRepository _documentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _storagePath;

    public DocumentService(
        IOrderDocumentRepository documentRepository,
        IUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
        _storagePath = configuration["Storage:DocumentsPath"] ?? "./storage/documents";
    }

    public async Task<OrderDocument> UploadDocumentAsync(
        Guid orderId,
        IFormFile file,
        DocumentType type,
        Guid uploadedBy)
    {
        // Validação
        if (file == null || file.Length == 0)
            throw new ArgumentException("Arquivo inválido");

        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "application/pdf" };
        if (!allowedTypes.Contains(file.ContentType))
            throw new ArgumentException("Tipo de arquivo não suportado. Use JPG, PNG ou PDF");

        // Criar documento
        var document = new OrderDocument(orderId, file.FileName, type, uploadedBy);

        // Converter para WebP (se for imagem)
        string savedPath;
        long fileSize;

        if (file.ContentType.StartsWith("image/"))
        {
            // 🆕 CONVERTER PARA WEBP
            using var image = await Image.LoadAsync(file.OpenReadStream());
            
            // Criar diretório se não existir
            var orderFolder = Path.Combine(_storagePath, orderId.ToString());
            Directory.CreateDirectory(orderFolder);

            // Nome do arquivo WebP
            var webpFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.UtcNow:yyyyMMddHHmmss}.webp";
            savedPath = Path.Combine(orderFolder, webpFileName);

            // Salvar como WebP com alta qualidade, baixo tamanho
            var encoder = new WebpEncoder
            {
                Quality = 85,              // 85% de qualidade (bom balanço)
                Method = WebpEncodingMethod.BestQuality,
                FileFormat = WebpFileFormatType.Lossy
            };

            await image.SaveAsync(savedPath, encoder);
            
            // Tamanho do arquivo WebP
            var fileInfo = new FileInfo(savedPath);
            fileSize = fileInfo.Length;
        }
        else if (file.ContentType == "application/pdf")
        {
            // Para PDFs, salvar diretamente (não converter)
            var orderFolder = Path.Combine(_storagePath, orderId.ToString());
            Directory.CreateDirectory(orderFolder);

            var pdfFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            savedPath = Path.Combine(orderFolder, pdfFileName);

            using var stream = new FileStream(savedPath, FileMode.Create);
            await file.CopyToAsync(stream);
            fileSize = file.Length;
        }
        else
        {
            throw new ArgumentException("Tipo de arquivo não suportado");
        }

        // URL pública (ajustar conforme infraestrutura)
        var fileUrl = $"/api/documents/{document.Id}/download";

        document.SetFilePath(savedPath, fileUrl, fileSize);

        await _documentRepository.AddAsync(document);
        await _unitOfWork.CommitAsync();

        return document;
    }

    public async Task<(Stream fileStream, string contentType, string fileName)> DownloadDocumentAsync(Guid documentId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null)
            throw new KeyNotFoundException("Documento não encontrado");

        if (!File.Exists(document.FilePath))
            throw new FileNotFoundException("Arquivo físico não encontrado");

        var fileStream = new FileStream(document.FilePath, FileMode.Open, FileAccess.Read);
        var contentType = document.MimeType;
        var fileName = document.FileName;

        return (fileStream, contentType, fileName);
    }
}
```

**Controller**: `API/src/Logistics.API/Controllers/DocumentsController.cs` (NOVO)

```csharp
using Logistics.Application.Services;
using Logistics.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly DocumentService _documentService;

    public DocumentsController(DocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpPost("upload")]
    public async Task<ActionResult> Upload(
        [FromForm] Guid orderId,
        [FromForm] IFormFile file,
        [FromForm] DocumentType type)
    {
        var userId = Guid.Parse(User.FindFirst("userId")?.Value ?? throw new UnauthorizedAccessException());
        
        var document = await _documentService.UploadDocumentAsync(orderId, file, type, userId);
        
        return Ok(new
        {
            id = document.Id,
            fileName = document.FileName,
            fileUrl = document.FileUrl,
            fileSizeBytes = document.FileSizeBytes,
            type = document.Type.ToString()
        });
    }

    [HttpGet("{id}/download")]
    public async Task<ActionResult> Download(Guid id)
    {
        var (fileStream, contentType, fileName) = await _documentService.DownloadDocumentAsync(id);
        return File(fileStream, contentType, fileName);
    }
}
```

**Dependência**: Adicionar no `Logistics.API.csproj`

```xml
<ItemGroup>
  <PackageReference Include="SixLabors.ImageSharp" Version="3.1.0" />
  <PackageReference Include="SixLabors.ImageSharp.Web" Version="3.1.0" />
</ItemGroup>
```

### 7.10 Impressão de Purchase Order (PDF/A4)

#### **7.10.1 Template de Purchase Order/Invoice**

**Service**: `API/src/Logistics.Application/Services/PurchaseOrderPrintService.cs` (NOVO)

```csharp
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Logistics.Domain.Entities;

namespace Logistics.Application.Services;

public class PurchaseOrderPrintService
{
    public byte[] GeneratePurchaseOrderPDF(Order order)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                // Header
                page.Header().Element(ComposeHeader);

                // Content
                page.Content().Element(c => ComposeContent(c, order));

                // Footer
                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("PURCHASE ORDER").FontSize(20).Bold();
                column.Item().Text("Your Company Name").FontSize(12);
                column.Item().Text("Rua Exemplo, 123 - São Paulo, SP");
                column.Item().Text("CNPJ: 00.000.000/0001-00");
            });

            row.ConstantItem(100).Height(50).Placeholder();
        });
    }

    void ComposeContent(IContainer container, Order order)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Spacing(10);

            // Order Info
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text($"PO Number: {order.OrderNumber}").Bold();
                    col.Item().Text($"Date: {order.OrderDate:dd/MM/yyyy}");
                    col.Item().Text($"Expected: {order.ExpectedDate:dd/MM/yyyy}");
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().Text($"Supplier: {order.Supplier?.Name}").Bold();
                    col.Item().Text($"Status: {order.Status}");
                    if (order.IsInternational)
                    {
                        col.Item().Text($"🌍 INTERNATIONAL PURCHASE").FontColor(Colors.Blue.Medium);
                        col.Item().Text($"Origin: {order.OriginCountry}");
                        col.Item().Text($"Incoterm: {order.Incoterm}");
                    }
                });
            });

            // International Details
            if (order.IsInternational)
            {
                column.Item().Element(c => ComposeInternationalDetails(c, order));
            }

            // Items Table
            column.Item().Element(c => ComposeItemsTable(c, order));

            // Packaging Hierarchy
            column.Item().Element(c => ComposePackagingInfo(c, order));

            // Totals
            column.Item().Element(c => ComposeTotals(c, order));
        });
    }

    void ComposeInternationalDetails(IContainer container, Order order)
    {
        container.Border(1).Padding(10).Column(column =>
        {
            column.Item().Text("IMPORT INFORMATION").FontSize(12).Bold();
            column.Item().Text($"Container: {order.ContainerNumber}");
            column.Item().Text($"Port of Entry: {order.PortOfEntry}");
            column.Item().Text($"Bill of Lading: {order.BillOfLading}");
            if (!string.IsNullOrEmpty(order.ImportLicenseNumber))
                column.Item().Text($"Import License (LI): {order.ImportLicenseNumber}");
            column.Item().Text($"Customs Broker: {order.CustomsBroker}");
            column.Item().Text($"Carrier: {(order.IsOwnCarrier ? "Own Carrier" : order.ThirdPartyCarrier)}");
            if (order.EstimatedArrivalPort.HasValue)
                column.Item().Text($"ETA Port: {order.EstimatedArrivalPort:dd/MM/yyyy}");
        });
    }

    void ComposeItemsTable(IContainer container, Order order)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(30);
                columns.RelativeColumn(3);
                columns.RelativeColumn(2);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("#");
                header.Cell().Element(CellStyle).Text("Description");
                header.Cell().Element(CellStyle).Text("SKU");
                header.Cell().Element(CellStyle).Text("Qty");
                header.Cell().Element(CellStyle).Text("Unit Price");
                header.Cell().Element(CellStyle).Text("Total");

                static IContainer CellStyle(IContainer c) => c.Background(Colors.Grey.Lighten2).Padding(5);
            });

            int index = 1;
            foreach (var item in order.Items)
            {
                table.Cell().Element(CellStyle).Text(index.ToString());
                table.Cell().Element(CellStyle).Text(item.Product?.Description ?? "-");
                table.Cell().Element(CellStyle).Text(item.SKU);
                table.Cell().Element(CellStyle).AlignRight().Text(item.QuantityOrdered.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(order.UnitCost.ToString("C2"));
                table.Cell().Element(CellStyle).AlignRight().Text((item.QuantityOrdered * order.UnitCost).ToString("C2"));

                index++;
            }

            static IContainer CellStyle(IContainer c) => c.BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5);
        });
    }

    void ComposePackagingInfo(IContainer container, Order order)
    {
        container.Border(1).Padding(10).Column(column =>
        {
            column.Item().Text("PACKAGING INFORMATION").FontSize(12).Bold();
            column.Item().Text($"Total Parcels (Pallets): {order.ExpectedParcels}");
            column.Item().Text($"Cartons per Parcel: {order.CartonsPerParcel}");
            column.Item().Text($"Units per Carton: {order.UnitsPerCarton}");
            column.Item().Text($"Total Cartons: {order.ExpectedCartons}");
        });
    }

    void ComposeTotals(IContainer container, Order order)
    {
        container.AlignRight().Column(column =>
        {
            column.Item().Text($"Subtotal: {order.TotalCost:C2}");
            column.Item().Text($"Tax ({order.TaxPercentage}%): {order.TaxAmount:C2}");
            if (order.ShippingCost > 0)
                column.Item().Text($"Shipping: {order.ShippingCost:C2}");
            column.Item().Text($"TOTAL: {(order.TotalCost + order.TaxAmount + order.ShippingCost):C2}").FontSize(14).Bold();
        });
    }
}
```

**Controller Endpoint**:

```csharp
[HttpGet("{id}/print")]
public async Task<ActionResult> PrintPurchaseOrder(Guid id)
{
    var order = await _orderService.GetByIdAsync(id);
    var pdfBytes = _printService.GeneratePurchaseOrderPDF(order);
    
    return File(pdfBytes, "application/pdf", $"PO-{order.OrderNumber}.pdf");
}
```

**Dependência**: Adicionar no `Logistics.API.csproj`

```xml
<ItemGroup>
  <PackageReference Include="QuestPDF" Version="2023.12.0" />
</ItemGroup>
```

---

## 8. CHECKLIST DE IMPLEMENTAÇÃO - BACKEND (C#)

### 8.1 Fase 1: Entidades e Domínio (DDD)

- [ ] **1.1 Atualizar Order.cs**
  - [ ] Adicionar campos de compra (UnitCost, TaxPercentage, Margin)
  - [ ] Adicionar campos de hierarquia (ExpectedParcels, CartonsPerParcel, UnitsPerCarton)
  - [ ] Adicionar campos internacionais (IsInternational, OriginCountry, PortOfEntry, etc.)
  - [ ] Adicionar métodos: SetPurchaseDetails(), SetPackagingHierarchy(), SetAsInternational()

- [ ] **1.2 Criar InboundParcel.cs**
  - [ ] Propriedades: ParcelNumber, LPN, Type, SequenceNumber, Status
  - [ ] Navigation: InboundShipment, ChildParcels, Items
  - [ ] Métodos: SetLPN(), SetSequence(), MarkAsReceived()

- [ ] **1.3 Criar InboundParcelItem.cs**
  - [ ] Propriedades: ProductId, SKU, Quantity
  - [ ] Navigation: InboundParcel, Product

- [ ] **1.4 Criar InboundCarton.cs**
  - [ ] Propriedades: CartonNumber, Barcode, SequenceNumber, Status
  - [ ] Navigation: InboundParcel, Items
  - [ ] Métodos: SetBarcode(), MarkAsReceived()

- [ ] **1.5 Criar InboundCartonItem.cs**
  - [ ] Propriedades: ProductId, SKU, SerialNumber, IsReceived
  - [ ] Métodos: MarkAsReceived()

- [ ] **1.6 Criar OrderDocument.cs**
  - [ ] Propriedades: FileName, Type, FilePath, FileUrl, FileSizeBytes
  - [ ] Métodos: SetFilePath()

- [ ] **1.7 Criar Enums**
  - [ ] ParcelType.cs (Pallet, Carton, Box, etc.)
  - [ ] ParcelStatus.cs (Pending, Received, Damaged, etc.)
  - [ ] CartonStatus.cs
  - [ ] DocumentType.cs (Invoice, DI, PackingList, etc.)

### 8.2 Fase 2: Infrastructure (EF Core)

- [ ] **2.1 Atualizar OrderConfiguration.cs**
  - [ ] Configurar novos campos decimais com precisão
  - [ ] Configurar campos opcionais de importação
  - [ ] Configurar relacionamento com OrderDocument

- [ ] **2.2 Criar InboundParcelConfiguration.cs**
  - [ ] ToTable("InboundParcels")
  - [ ] Configurar enums como string
  - [ ] Configurar relacionamentos (Shipment, Items, ChildParcels)
  - [ ] Criar índices (LPN, Status, ShipmentId)

- [ ] **2.3 Criar InboundCartonConfiguration.cs**
  - [ ] ToTable("InboundCartons")
  - [ ] Configurar relacionamentos com Parcel e Items
  - [ ] Criar índice em Barcode

- [ ] **2.4 Criar OrderDocumentConfiguration.cs**
  - [ ] ToTable("OrderDocuments")
  - [ ] Configurar relacionamento com Order
  - [ ] Criar índice em OrderId

- [ ] **2.5 Criar Repositories**
  - [ ] IInboundParcelRepository + implementação
  - [ ] IInboundCartonRepository + implementação
  - [ ] IOrderDocumentRepository + implementação

### 8.3 Fase 3: Migrations

- [ ] **3.1 Criar Migration: AddPurchaseOrderFields**
  ```bash
  cd API/src/Logistics.Infrastructure
  dotnet ef migrations add AddPurchaseOrderFields --startup-project ../Logistics.API
  ```
  - [ ] Verificar Up(): ALTER TABLE Orders ADD UnitCost, TaxPercentage, etc.
  - [ ] Verificar Down(): ALTER TABLE Orders DROP COLUMN ...

- [ ] **3.2 Criar Migration: AddInboundParcelTracking**
  ```bash
  dotnet ef migrations add AddInboundParcelTracking --startup-project ../Logistics.API
  ```
  - [ ] Verificar criação de InboundParcels, InboundParcelItems
  - [ ] Verificar foreign keys

- [ ] **3.3 Criar Migration: AddInboundCartons**
  ```bash
  dotnet ef migrations add AddInboundCartons --startup-project ../Logistics.API
  ```
  - [ ] Verificar criação de InboundCartons, InboundCartonItems

- [ ] **3.4 Criar Migration: AddOrderDocuments**
  ```bash
  dotnet ef migrations add AddOrderDocuments --startup-project ../Logistics.API
  ```
  - [ ] Verificar criação de OrderDocuments

- [ ] **3.5 Aplicar Migrations no Banco**
  ```bash
  dotnet ef database update --startup-project ../Logistics.API
  ```

### 8.4 Fase 4: Application Layer (Services)

- [ ] **4.1 Atualizar OrderService.cs**
  - [ ] Método CreateAsync: adicionar lógica de hierarquia automática
  - [ ] Criar Shipment + Parcels + Cartons automaticamente se Inbound
  - [ ] Calcular preços e margens automaticamente

- [ ] **4.2 Criar InboundParcelService.cs**
  - [ ] CreateAsync()
  - [ ] GetByShipmentIdAsync()
  - [ ] GetByLPNAsync()
  - [ ] MarkAsReceivedAsync()

- [ ] **4.3 Criar InboundCartonService.cs**
  - [ ] ScanCartonAsync(barcode)
  - [ ] ScanItemAsync(serialNumber)
  - [ ] GetProgressAsync()

- [ ] **4.4 Criar DocumentService.cs**
  - [ ] UploadDocumentAsync() - converter para WebP
  - [ ] DownloadDocumentAsync()
  - [ ] DeleteDocumentAsync()

- [ ] **4.5 Criar PurchaseOrderPrintService.cs**
  - [ ] GeneratePurchaseOrderPDF()
  - [ ] GeneratePackingListPDF()

- [ ] **4.6 Criar DTOs**
  - [ ] CreateOrderRequest (adicionar novos campos)
  - [ ] CreateInboundParcelRequest
  - [ ] ScanCartonRequest
  - [ ] UploadDocumentRequest

### 8.5 Fase 5: API Layer (Controllers)

- [ ] **5.1 Atualizar OrdersController.cs**
  - [ ] POST /api/orders - criar PO com hierarquia
  - [ ] GET /api/orders/{id}/receiving-dashboard
  - [ ] GET /api/orders/{id}/print - gerar PDF

- [ ] **5.2 Criar InboundParcelsController.cs**
  - [ ] GET /api/inboundparcels/lpn/{lpn}
  - [ ] POST /api/inboundparcels/{id}/receive
  - [ ] GET /api/inboundparcels/shipment/{shipmentId}

- [ ] **5.3 Criar InboundCartonsController.cs**
  - [ ] POST /api/inboundcartons/scan
  - [ ] POST /api/inboundcartons/{id}/items/scan
  - [ ] GET /api/inboundcartons/{id}/progress

- [ ] **5.4 Criar DocumentsController.cs**
  - [ ] POST /api/documents/upload
  - [ ] GET /api/documents/{id}/download
  - [ ] DELETE /api/documents/{id}

- [ ] **5.5 Adicionar [Authorize] em TODOS os endpoints**

### 8.6 Fase 6: Dependency Injection (Program.cs)

- [ ] **6.1 Registrar Repositories**
  ```csharp
  builder.Services.AddScoped<IInboundParcelRepository, InboundParcelRepository>();
  builder.Services.AddScoped<IInboundCartonRepository, InboundCartonRepository>();
  builder.Services.AddScoped<IOrderDocumentRepository, OrderDocumentRepository>();
  ```

- [ ] **6.2 Registrar Services**
  ```csharp
  builder.Services.AddScoped<InboundParcelService>();
  builder.Services.AddScoped<InboundCartonService>();
  builder.Services.AddScoped<DocumentService>();
  builder.Services.AddScoped<PurchaseOrderPrintService>();
  ```

- [ ] **6.3 Adicionar pacotes NuGet**
  ```bash
  dotnet add package SixLabors.ImageSharp
  dotnet add package QuestPDF
  ```

### 8.7 Fase 7: Testes

- [ ] **7.1 Testar Migrations**
  - [ ] Verificar tabelas criadas no banco
  - [ ] Verificar foreign keys
  - [ ] Verificar índices

- [ ] **7.2 Testar Endpoints com Postman/curl**
  - [ ] POST /api/orders - criar PO nacional
  - [ ] POST /api/orders - criar PO internacional
  - [ ] Verificar criação automática de Parcels e Cartons
  - [ ] POST /api/documents/upload - upload de invoice
  - [ ] GET /api/orders/{id}/print - gerar PDF

- [ ] **7.3 Testar Fluxo de Recebimento**
  - [ ] Scan de LPN (pallet)
  - [ ] Scan de Barcode (caixa)
  - [ ] Scan de Serial Number (produto)
  - [ ] Verificar atualização de status
  - [ ] Verificar rastreabilidade

- [ ] **7.4 Testar Upload WebP**
  - [ ] Upload de JPG - verificar conversão
  - [ ] Upload de PNG - verificar conversão
  - [ ] Upload de PDF - verificar armazenamento
  - [ ] Verificar tamanho do arquivo

---

## 9. ORDEM DE EXECUÇÃO

**1ª Etapa - BACKEND (C#)**:
1. Criar/atualizar entidades de domínio
2. Criar configurações EF Core
3. Gerar e aplicar migrations
4. Testar banco de dados
5. Criar services
6. Criar controllers
7. Registrar DI
8. Testar endpoints

**2ª Etapa - FRONTEND** (depois do backend funcionando):
1. Criar models TypeScript
2. Criar services Angular
3. Criar componentes
4. Adicionar i18n (pt-BR, en-US, es-ES)
5. Aplicar dark mode
6. Testar fluxo completo

---

**FIM DO DOCUMENTO**

#### **InboundShipment** - O que existe:
```csharp
public class InboundShipment
{
    public Guid Id { get; private set; }
    public string ShipmentNumber { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid SupplierId { get; private set; }
    public decimal TotalQuantityExpected { get; private set; }  // ❌ Apenas total
    public decimal TotalQuantityReceived { get; private set; }   // ❌ Apenas total
    public string? ASNNumber { get; private set; }               // ✅ ASN existe
    // Navigation
    public Order Order { get; private set; }
}
```

**PROBLEMAS**:
- ❌ Não há campos para **número de volumes**
- ❌ Não há relacionamento com **volumes individuais**
- ❌ Apenas quantidade total, sem quebra por volume
- ❌ ASN existe, mas não tem detalhamento de volumes

#### **Receipt/ReceiptLine** - O que existe:
```csharp
public class Receipt
{
    public Guid InboundShipmentId { get; private set; }
    // ❌ Não referencia volumes
}

public class ReceiptLine
{
    public Guid ProductId { get; private set; }
    public decimal QuantityExpected { get; private set; }
    public decimal QuantityReceived { get; private set; }
    // ❌ Não indica de qual volume/parcel veio
}
```

**PROBLEMAS**:
- ❌ Recebimento é direto produto → quantidade
- ❌ Não há agrupamento por volume/parcel
- ❌ Impossível saber "este produto veio do volume #5"

### 1.2 Entidades de Embalagem Outbound (Existentes)

```csharp
public class Package  // ⚠️ Apenas para SAÍDA (Outbound)
{
    public Guid PackingTaskId { get; private set; }
    public string TrackingNumber { get; private set; }
    public PackageType Type { get; private set; }  // Box, Pallet, etc.
    public decimal Weight { get; private set; }
    // ❌ Não serve para Inbound
}
```

**CONCLUSÃO**: Existe controle de embalagem para **saída** (expedição), mas **NÃO existe para entrada** (recebimento).

---

## 2. PADRÕES WMS DA INDÚSTRIA

### 2.1 Hierarquia de Embalagem Padrão WMS

Sistemas WMS profissionais utilizam hierarquia de 3 níveis:

```
NÍVEL 1: PALLET (ou Shipment Unit)
    │
    ├── NÍVEL 2: CARTON/BOX (Caixa)
    │       │
    │       └── NÍVEL 3: UNIT (Produto Individual)
    │
    └── NÍVEL 2: CARTON/BOX
            │
            └── NÍVEL 3: UNIT
```

**Exemplo Real**:
```
Pedido de Compra: 1.000 notebooks
    ↓
Remessa chega em: 4 PALLETS
    ↓
Pallet #1 (250 unidades)
    ├── Caixa #1 → 50 notebooks
    ├── Caixa #2 → 50 notebooks
    ├── Caixa #3 → 50 notebooks
    ├── Caixa #4 → 50 notebooks
    └── Caixa #5 → 50 notebooks
    
Pallet #2 (250 unidades)
    └── [mesma estrutura]
...
```

### 2.2 Funcionalidades WMS Profissionais

✅ **ASN (Advanced Shipping Notice)**: Fornecedor informa o que está vindo  
✅ **Parcel/LPN (License Plate Number)**: Cada volume tem código único  
✅ **Hierarquia**: Produto → Caixa → Pallet  
✅ **Cross-docking**: Identificar volumes que não precisam armazenamento  
✅ **Rastreamento**: Saber onde está cada volume no fluxo  
✅ **Conferência por Volume**: Receber volume por volume  

### 2.3 Campos Essenciais de Parcel/Volume

```
- ParcelNumber/LPN (License Plate Number): Código único do volume
- ParcelType: Pallet, Carton, Box, Bag, etc.
- ContainerNumber: Número do container (se importação)
- Sequence: Ordem de descarregamento (1/50, 2/50, etc.)
- Status: Pending, Receiving, Received, Putaway, etc.
- Location: Onde está (Dock Door, Staging Area, etc.)
- Weight/Dimensions: Peso e dimensões físicas
- ParentParcelId: Hierarquia (caixa dentro de pallet)
```

---

## 3. GAPS IDENTIFICADOS NO SISTEMA ATUAL

### ❌ GAP #1: Ausência de Entidade "Parcel/Volume"
**Problema**: Não há tabela/entidade para representar volumes individuais que chegam  
**Impacto**: Impossível rastrear volumes, apenas quantidades totais  

### ❌ GAP #2: Sem Hierarquia de Embalagem (Inbound)
**Problema**: Não há relação Pallet → Caixa → Produto  
**Impacto**: Não sei quais produtos estão em qual caixa/pallet  

### ❌ GAP #3: Receipt não referencia Volumes
**Problema**: ReceiptLine registra apenas Produto + Quantidade  
**Impacto**: Não sei de qual volume veio cada produto recebido  

### ❌ GAP #4: ASN sem Detalhamento
**Problema**: ASNNumber existe, mas não tem estrutura de volumes  
**Impacto**: Não aproveito informação prévia do fornecedor  

### ❌ GAP #5: Sem Controle de Descarregamento
**Problema**: Não há status/sequência de volumes sendo descarregados  
**Impacto**: Operador não sabe "faltam 10 volumes de 50"  

### ❌ GAP #6: Sem Cross-Docking Eficiente
**Problema**: Não identifica volumes que vão direto para expedição  
**Impacto**: Perde oportunidade de otimização  

---

## 4. PROPOSTA DE SOLUÇÃO

### 4.1 Novas Entidades de Dados

#### **ENTIDADE 1: InboundParcel**

```csharp
public class InboundParcel
{
    public Guid Id { get; private set; }
    public Guid InboundShipmentId { get; private set; }
    public string ParcelNumber { get; private set; }        // Ex: "PL-001", "CTN-045"
    public string? LPN { get; private set; }                // License Plate Number
    public ParcelType Type { get; private set; }            // Pallet, Carton, Box, Bag
    public int SequenceNumber { get; private set; }         // 1 de 50
    public int TotalParcels { get; private set; }           // Total: 50
    
    // Hierarquia
    public Guid? ParentParcelId { get; private set; }       // Se caixa está em pallet
    
    // Dimensões
    public decimal Weight { get; private set; }
    public decimal Length { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }
    public string? DimensionUnit { get; private set; }      // cm, m, in
    
    // Rastreamento
    public ParcelStatus Status { get; private set; }        // Pending, Receiving, Received, etc.
    public string? CurrentLocation { get; private set; }    // DOCK-01, STAGING-A, etc.
    public DateTime? ReceivedAt { get; private set; }
    public Guid? ReceivedBy { get; private set; }
    
    // Container (importação)
    public string? ContainerNumber { get; private set; }
    public string? SealNumber { get; private set; }
    
    // Qualidade
    public bool HasDamage { get; private set; }
    public string? DamageNotes { get; private set; }
    
    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation
    public InboundShipment InboundShipment { get; private set; } = null!;
    public InboundParcel? ParentParcel { get; private set; }
    public ICollection<InboundParcel> ChildParcels { get; private set; }
    public ICollection<InboundParcelItem> Items { get; private set; }
}
```

#### **ENTIDADE 2: InboundParcelItem**

```csharp
public class InboundParcelItem
{
    public Guid Id { get; private set; }
    public Guid InboundParcelId { get; private set; }
    public Guid ProductId { get; private set; }
    public string SKU { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid? LotId { get; private set; }                // Se produto tem lote
    public string? SerialNumbers { get; private set; }       // JSON array de SNs
    public DateTime? ExpiryDate { get; private set; }       // Para perecíveis
    public DateTime CreatedAt { get; private set; }
    
    // Navigation
    public InboundParcel InboundParcel { get; private set; } = null!;
    public Product Product { get; private set; } = null!;
    public Lot? Lot { get; private set; }
}
```

#### **ENUM: ParcelType**

```csharp
public enum ParcelType
{
    Pallet = 1,      // Palete completo
    Carton = 2,      // Caixa de papelão
    Box = 3,         // Caixa rígida
    Bag = 4,         // Saco/Embalagem flexível
    Bulk = 5,        // A granel
    Container = 6,   // Container (importação)
    Tote = 7,        // Cesto/Bandeja
    Drum = 8,        // Tambor
    Roll = 9         // Rolo (tecidos, etc.)
}
```

#### **ENUM: ParcelStatus**

```csharp
public enum ParcelStatus
{
    Pending = 0,          // Aguardando chegada
    InTransit = 1,        // Em trânsito
    AtDock = 2,           // No dock door
    Receiving = 3,        // Sendo recebido/conferido
    Received = 4,         // Recebido e conferido
    Damaged = 5,          // Com avaria
    Quarantine = 6,       // Em quarentena
    ReadyForPutaway = 7,  // Pronto para endereçamento
    PutawayInProgress = 8,// Endereçamento em andamento
    Stored = 9,           // Armazenado
    CrossDock = 10        // Cross-docking direto
}
```

### 4.2 Atualização de Entidades Existentes

#### **InboundShipment** - Adicionar campos:

```csharp
public class InboundShipment
{
    // NOVOS CAMPOS:
    public int TotalParcels { get; private set; }           // Qtd total de volumes
    public int ParcelsReceived { get; private set; }        // Qtd volumes recebidos
    public bool HasASNDetail { get; private set; }          // Se ASN tem detalhes
    
    // NOVA NAVIGATION:
    public ICollection<InboundParcel> Parcels { get; private set; }
}
```

#### **ReceiptLine** - Adicionar referência:

```csharp
public class ReceiptLine
{
    // NOVO CAMPO:
    public Guid? InboundParcelId { get; private set; }      // De qual volume veio
    
    // NOVA NAVIGATION:
    public InboundParcel? InboundParcel { get; private set; }
}
```

---

## 5. NOVOS ENDPOINTS DA API

### 5.1 InboundParcelsController

#### **POST /api/inboundparcels**
Cria novo volume/parcel

**Request**:
```json
{
  "inboundShipmentId": "guid",
  "parcelNumber": "PL-001",
  "lpn": "LPN123456",
  "type": 1,
  "sequenceNumber": 1,
  "totalParcels": 50,
  "weight": 250.5,
  "items": [
    {
      "productId": "guid",
      "sku": "PROD-001",
      "quantity": 50
    }
  ]
}
```

#### **GET /api/inboundparcels/shipment/{shipmentId}**
Lista todos os parcels de uma remessa

**Response**:
```json
{
  "success": true,
  "data": [
    {
      "id": "guid",
      "parcelNumber": "PL-001",
      "type": "Pallet",
      "status": "Received",
      "sequenceNumber": 1,
      "totalParcels": 50,
      "weight": 250.5,
      "items": [...]
    }
  ]
}
```

#### **GET /api/inboundparcels/{id}**
Busca parcel específico com todos os detalhes

#### **PATCH /api/inboundparcels/{id}/status**
Atualiza status do parcel

**Request**:
```json
{
  "status": 4,
  "location": "STAGING-A",
  "receivedBy": "guid-usuario"
}
```

#### **POST /api/inboundparcels/{id}/receive**
Marca parcel como recebido e cria linhas de recebimento

#### **POST /api/inboundparcels/{id}/damage**
Reporta avaria em um parcel

**Request**:
```json
{
  "hasDamage": true,
  "damageNotes": "Caixa amassada, produtos aparentemente OK"
}
```

#### **GET /api/inboundparcels/lpn/{lpn}**
Busca parcel por LPN (License Plate Number)

### 5.2 Atualizações em Controllers Existentes

#### **InboundShipmentsController**

**POST /api/inboundshipments/asn** (NOVO)
Cria remessa a partir de ASN detalhado com parcels

```json
{
  "asnNumber": "ASN-123456",
  "shipmentNumber": "ISH-2025-001",
  "supplierId": "guid",
  "totalParcels": 50,
  "parcels": [
    {
      "parcelNumber": "PL-001",
      "type": 1,
      "weight": 250.5,
      "items": [
        {"productId": "guid", "sku": "PROD-001", "quantity": 50}
      ]
    }
  ]
}
```

**GET /api/inboundshipments/{id}/parcels/summary**
Resumo de parcels por status

```json
{
  "totalParcels": 50,
  "pending": 10,
  "receiving": 5,
  "received": 35,
  "damaged": 2
}
```

---

## 6. FLUXO DE PROCESSO ATUALIZADO

### 6.1 Fluxo Inbound com Parcels

```
1. FORNECEDOR ENVIA ASN
   ├─ ASN contém: 50 pallets
   └─ Cada pallet tem lista de produtos

2. SISTEMA CRIA InboundShipment
   └─ Cria 50 InboundParcel automaticamente do ASN

3. CAMINHÃO CHEGA
   ├─ Check-in no dock door
   └─ Status parcels: Pending → AtDock

4. INÍCIO DO RECEBIMENTO
   ├─ Operador escaneia LPN do pallet
   └─ Sistema mostra: "Pallet 1 de 50"

5. CONFERÊNCIA DO PARCEL
   ├─ Operador confere produtos do pallet
   ├─ Registra quantidades recebidas
   └─ Marca parcel como "Received"

6. REPETIR ATÉ 50/50 PARCELS

7. GERAR TAREFAS DE PUTAWAY
   └─ Uma tarefa por parcel ou agrupado

8. COMPLETAR INBOUND
   └─ Todos parcels received → Shipment complete
```

### 6.2 Tela de Recebimento (Frontend)

```
╔════════════════════════════════════════════════╗
║ RECEBIMENTO - Remessa ISH-2025-001            ║
║ Fornecedor: ABC Ltda | ASN: ASN-123456        ║
╠════════════════════════════════════════════════╣
║                                                ║
║ PROGRESSO: ████████████░░░░ 35/50 parcels     ║
║                                                ║
║ ┌──────────────────────────────────────────┐  ║
║ │ Escanear LPN: [____________] 🔍          │  ║
║ └──────────────────────────────────────────┘  ║
║                                                ║
║ PARCELS POR STATUS:                            ║
║   ✓ Recebidos:    35 parcels                  ║
║   ⧗ Em processo:   5 parcels                  ║
║   ⊗ Pendentes:    10 parcels                  ║
║   ⚠ Com avaria:    2 parcels                  ║
║                                                ║
║ ÚLTIMO PARCEL RECEBIDO:                        ║
║ ┌──────────────────────────────────────────┐  ║
║ │ LPN: PL-035 | Pallet 35/50               │  ║
║ │ Produtos: 50 unidades de PROD-001         │  ║
║ │ Recebido por: João Silva às 14:32         │  ║
║ └──────────────────────────────────────────┘  ║
╚════════════════════════════════════════════════╝
```

---

## 7. BENEFÍCIOS DA IMPLEMENTAÇÃO

### ✅ Rastreabilidade Total
- Saber exatamente quantos volumes chegaram
- Rastrear onde cada volume está no processo
- Histórico completo de cada parcel

### ✅ Conferência Eficiente
- Receber volume por volume (não produto por produto)
- Operador vê progresso: "35 de 50 parcels recebidos"
- Identificar rapidamente se falta algum volume

### ✅ Gestão de Avarias
- Marcar volumes com avaria
- Isolar volumes danificados
- Quarentena automática

### ✅ Cross-Docking
- Identificar volumes que vão direto para expedição
- Otimizar fluxo sem armazenamento intermediário

### ✅ Integração com Fornecedores
- Aproveitar ASN detalhado
- Pré-configurar recebimento
- Reduzir tempo de conferência

### ✅ Conformidade WMS
- Alinha sistema com padrões da indústria
- Facilita integração com ERPs externos
- Suporta operações complexas

---

## 8. ESTIMATIVA DE IMPLEMENTAÇÃO

### 8.1 Backend (API)

| Tarefa | Estimativa | Complexidade |
|--------|-----------|--------------|
| Criar entidade InboundParcel | 2h | Média |
| Criar entidade InboundParcelItem | 1h | Baixa |
| Atualizar InboundShipment | 1h | Baixa |
| Atualizar ReceiptLine | 1h | Baixa |
| Criar InboundParcelsController | 4h | Média |
| Endpoints CRUD parcels | 3h | Média |
| Lógica de ASN com parcels | 3h | Alta |
| Testes unitários | 4h | Média |
| **TOTAL BACKEND** | **19h** | - |

### 8.2 Frontend (Angular)

| Tarefa | Estimativa | Complexidade |
|--------|-----------|--------------|
| Modelo de dados Parcel | 1h | Baixa |
| Serviço InboundParcels | 2h | Média |
| Tela de lista de parcels | 3h | Média |
| Tela de recebimento por parcel | 4h | Alta |
| Dashboard de progresso | 2h | Média |
| Integração com scanner LPN | 3h | Alta |
| **TOTAL FRONTEND** | **15h** | - |

### 8.3 Database

| Tarefa | Estimativa |
|--------|-----------|
| Migration InboundParcels | 1h |
| Migration InboundParcelItems | 1h |
| Índices e constraints | 1h |
| **TOTAL DATABASE** | **3h** |

### **ESTIMATIVA TOTAL: 37 horas** (aproximadamente 1 semana de desenvolvimento)

---

## 9. PRIORIZAÇÃO

### 🔴 CRÍTICO (MVP)
1. Entidades InboundParcel e InboundParcelItem
2. CRUD básico de parcels
3. Recebimento por parcel
4. Status tracking

### 🟡 IMPORTANTE (Fase 2)
5. ASN detalhado com parcels
6. Hierarquia de embalagem (parent/child)
7. Reportar avarias
8. Dashboard de progresso

### 🟢 DESEJÁVEL (Fase 3)
9. Cross-docking automático
10. Integração com scanner LPN
11. Relatórios avançados
12. Mobile app para recebimento

---

## 10. CONCLUSÃO

O sistema WMS atual está **incompleto** para operações profissionais de recebimento. A ausência de controle de **parcels/volumes** impede:
- Rastreabilidade adequada
- Conferência eficiente
- Integração com fornecedores via ASN
- Gestão de avarias por volume
- Cross-docking otimizado

A implementação das entidades **InboundParcel** e **InboundParcelItem** é **ESSENCIAL** para transformar o sistema em um WMS completo e profissional.

**Recomendação**: Priorizar implementação das entidades de parcel tracking no próximo sprint.

---

**Documentos Relacionados**:
- [02-MODELO-DE-DADOS-ENTIDADES.md](02-MODELO-DE-DADOS-ENTIDADES.md)
- [04-FLUXOS-PROCESSOS-WMS.md](04-FLUXOS-PROCESSOS-WMS.md)
- [03-API-ENDPOINTS-COMPLETO.md](03-API-ENDPOINTS-COMPLETO.md)
