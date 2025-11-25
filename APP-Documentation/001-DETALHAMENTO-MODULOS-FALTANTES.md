# DETALHAMENTO TÉCNICO - MÓDULOS FALTANTES NO FRONTEND

**Data**: 2025-11-24  
**Versão**: 1.0

---

## 📋 ÍNDICE

1. [Componentes Compartilhados Necessários](#componentes-compartilhados)
2. [Módulos Básicos - CRUD Completo](#modulos-basicos)
3. [Módulos Core WMS](#modulos-core-wms)
4. [Módulos de Fluxo Inbound](#modulos-inbound)
5. [Módulos de Fluxo Outbound](#modulos-outbound)
6. [Módulos de Rastreabilidade](#modulos-rastreabilidade)
7. [Módulos de Gestão de Pátio](#modulos-patio)

---

## 1. COMPONENTES COMPARTILHADOS NECESSÁRIOS

### 1.1 Modal Genérico Reutilizável

**Prioridade**: 🔴 CRÍTICA

**Arquivo**: `src/app/shared/components/modal/modal.component.ts`

**Funcionalidades**:
- Abertura/fechamento suave
- Tamanhos configuráveis (sm, md, lg, xl)
- Header customizável
- Footer com botões de ação
- Click outside para fechar
- ESC para fechar
- Acessibilidade (ARIA)

**Uso**:
```typescript
// Exemplo de uso
openCreateProductModal() {
  this.modalService.open(ProductFormComponent, {
    size: 'lg',
    title: 'Criar Novo Produto',
    data: { mode: 'create' }
  });
}
```

### 1.2 Formulários Reativos com Validação

**Prioridade**: 🔴 CRÍTICA

**Padrão a seguir**:
```typescript
// product-form.component.ts
export class ProductFormComponent {
  form = this.fb.group({
    companyId: ['', Validators.required],
    name: ['', [Validators.required, Validators.minLength(3)]],
    sku: ['', [Validators.required, Validators.pattern(/^[A-Z0-9-]+$/)]],
    barcode: [''],
    description: [''],
    weight: [0, [Validators.required, Validators.min(0)]],
    weightUnit: ['kg', Validators.required],
    volume: [0],
    volumeUnit: ['m3'],
    length: [0],
    width: [0],
    height: [0],
    dimensionUnit: ['cm'],
    requiresLotTracking: [false],
    requiresSerialTracking: [false],
    isPerishable: [false],
    shelfLifeDays: [null],
    minimumStock: [0],
    safetyStock: [0],
    abcClassification: ['']
  });

  async onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    
    const data = this.form.value;
    await this.productsService.create(data);
    this.modalService.close();
  }
}
```

### 1.3 Componente de Seleção (Autocomplete)

**Prioridade**: 🔴 CRÍTICA

**Arquivo**: `src/app/shared/components/autocomplete/autocomplete.component.ts`

**Casos de uso**:
- Selecionar produto
- Selecionar cliente
- Selecionar fornecedor
- Selecionar armazém
- Selecionar localização

**Exemplo**:
```html
<app-autocomplete
  [items]="products"
  [displayField]="'name'"
  [valueField]="'id'"
  placeholder="Selecione um produto"
  (selectionChange)="onProductSelected($event)"
></app-autocomplete>
```

### 1.4 Tabela com Paginação e Filtros

**Prioridade**: 🟡 ALTA

**Funcionalidades**:
- Ordenação por coluna
- Paginação
- Filtros por coluna
- Seleção múltipla
- Ações em massa
- Exportação (CSV, Excel)

---

## 2. MÓDULOS BÁSICOS - CRUD COMPLETO

### 2.1 Products (Produtos)

#### ❌ Faltando: Modal de Criação

**Arquivo**: `src/app/features/products/product-create-modal/product-create-modal.component.ts`

**Model Correto**:
```typescript
export interface Product {
  id: string;                          // Guid
  companyId: string;                   // Guid - OBRIGATÓRIO
  name: string;
  sku: string;
  barcode?: string;
  description?: string;
  
  // Dimensões e Peso
  weight: number;
  weightUnit?: string;                 // kg, g, lb
  volume: number;
  volumeUnit?: string;                 // m3, cm3
  length: number;
  width: number;
  height: number;
  dimensionUnit?: string;              // cm, m, in
  
  // WMS Tracking
  requiresLotTracking: boolean;
  requiresSerialTracking: boolean;
  isPerishable: boolean;
  shelfLifeDays?: number;
  
  // Estoque
  minimumStock?: number;
  safetyStock?: number;
  abcClassification?: string;          // A, B, C
  
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateProductDto {
  companyId: string;                   // SEMPRE obrigatório
  name: string;
  sku: string;
  barcode?: string;
  description?: string;
  weight: number;
  weightUnit?: string;
  volume?: number;
  volumeUnit?: string;
  length?: number;
  width?: number;
  height?: number;
  dimensionUnit?: string;
  requiresLotTracking?: boolean;
  requiresSerialTracking?: boolean;
  isPerishable?: boolean;
  shelfLifeDays?: number;
  minimumStock?: number;
  safetyStock?: number;
  abcClassification?: string;
}
```

**Validações**:
- SKU único por empresa
- Peso > 0
- Se isPerishable = true, shelfLifeDays obrigatório
- Se requiresLotTracking = true, mostrar avisos

#### ❌ Faltando: Modal de Edição

**Arquivo**: `src/app/features/products/product-edit-modal/product-edit-modal.component.ts`

**Diferenças da criação**:
- Pré-preencher formulário com dados existentes
- Não permitir editar SKU (ou validar se não está em uso)
- Mostrar histórico de mudanças (se disponível)

#### ❌ Faltando: Detalhes do Produto

**Arquivo**: `src/app/features/products/product-detail/product-detail.component.ts`

**Mostrar**:
- Todos os dados do produto
- Estoque atual em cada armazém
- Movimentações recentes
- Lotes ativos (se aplicável)
- Números de série (se aplicável)

---

### 2.2 Orders (Pedidos)

#### ❌ Problema Crítico: Model Incompatível

**Backend**:
```csharp
public enum OrderType {
    Inbound = 1,      // Entrada
    Outbound = 2,     // Saída
    Transfer = 3,     // Transferência
    Return = 4        // Devolução
}

public enum OrderStatus {
    Draft = 0,
    Pending = 1,
    Confirmed = 2,
    InProgress = 3,
    PartiallyFulfilled = 4,
    Fulfilled = 5,
    Shipped = 6,
    Delivered = 7,
    Cancelled = 8,
    OnHold = 9
}

public enum OrderSource {
    Manual = 1,
    ERP = 2,
    Ecommerce = 3,
    EDI = 4
}

public enum OrderPriority {
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}
```

**Frontend CORRETO**:
```typescript
export enum OrderType {
  Inbound = 1,
  Outbound = 2,
  Transfer = 3,
  Return = 4
}

export enum OrderStatus {
  Draft = 0,
  Pending = 1,
  Confirmed = 2,
  InProgress = 3,
  PartiallyFulfilled = 4,
  Fulfilled = 5,
  Shipped = 6,
  Delivered = 7,
  Cancelled = 8,
  OnHold = 9
}

export enum OrderSource {
  Manual = 1,
  ERP = 2,
  Ecommerce = 3,
  EDI = 4
}

export enum OrderPriority {
  Low = 0,
  Normal = 1,
  High = 2,
  Urgent = 3
}

export interface Order {
  id: string;
  companyId: string;
  orderNumber: string;
  type: OrderType;                    // NÃO é status!
  source: OrderSource;
  customerId?: string;
  customerName?: string;
  supplierId?: string;
  supplierName?: string;
  orderDate: Date;
  expectedDate?: Date;
  priority: OrderPriority;
  status: OrderStatus;
  totalQuantity: number;
  totalValue: number;
  shippingAddress?: string;
  specialInstructions?: string;
  isBOPIS: boolean;                  // Buy Online Pickup In Store
  createdBy: string;
  items: OrderItem[];
  createdAt: Date;
  updatedAt?: Date;
}

export interface OrderItem {
  id: string;
  orderId: string;
  productId: string;
  productName?: string;
  sku: string;
  quantityOrdered: number;
  quantityAllocated: number;
  quantityPicked: number;
  quantityShipped: number;
  unitPrice: number;
  requiredLotNumber?: string;
  requiredShipDate?: Date;
}

export interface CreateOrderRequest {
  companyId: string;
  orderNumber: string;
  type: OrderType;
  source: OrderSource;
  customerId?: string;
  supplierId?: string;
  orderDate: Date;
  expectedDate?: Date;
  priority: OrderPriority;
  shippingAddress?: string;
  specialInstructions?: string;
  isBOPIS?: boolean;
  items: CreateOrderItemRequest[];
}

export interface CreateOrderItemRequest {
  productId: string;
  sku: string;
  quantityOrdered: number;
  unitPrice: number;
  requiredLotNumber?: string;
  requiredShipDate?: Date;
}
```

#### ❌ Faltando: Modal de Criação de Pedido

**Arquivo**: `src/app/features/orders/order-create-modal/order-create-modal.component.ts`

**Fluxo**:
1. Selecionar tipo (Inbound/Outbound/Transfer/Return)
2. Se Inbound: selecionar Supplier
3. Se Outbound: selecionar Customer
4. Adicionar items:
   - Autocomplete de produtos
   - Quantidade
   - Preço unitário
   - Data esperada (opcional)
5. Calcular total automaticamente
6. Definir prioridade
7. Endereço de entrega (se Outbound)
8. Instruções especiais

**Cálculos necessários**:
```typescript
calculateItemTotal(item: OrderItem): number {
  return item.quantityOrdered * item.unitPrice;
}

calculateOrderTotal(): number {
  return this.items.reduce((sum, item) => 
    sum + this.calculateItemTotal(item), 0
  );
}

calculateTotalQuantity(): number {
  return this.items.reduce((sum, item) => 
    sum + item.quantityOrdered, 0
  );
}
```

---

### 2.3 Customers (Clientes)

#### ❌ Faltando: Modal de Criação

**Model**:
```typescript
export interface Customer {
  id: string;
  companyId: string;
  name: string;
  document?: string;        // CPF ou CNPJ
  email?: string;
  phone?: string;
  address?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}
```

**Validações**:
- Nome obrigatório (mín 3 caracteres)
- Email válido (se preenchido)
- CPF/CNPJ válido (se preenchido)
- Telefone formato brasileiro

---

## 3. MÓDULOS CORE WMS

### 3.1 ❌ Users (Usuários) - NÃO EXISTE

**Prioridade**: 🔴 CRÍTICA

**Endpoints Backend**:
- POST /api/users - Criar
- GET /api/users/{id} - Buscar
- GET /api/users - Listar todos
- GET /api/users/company/{companyId} - Por empresa
- PUT /api/users/{id} - Atualizar
- PATCH /api/users/{id}/role - Atualizar role
- DELETE /api/users/{id} - Deletar

**Arquivos a criar**:
```
src/app/features/users/
  ├── users.service.ts
  ├── users-list/
  │   ├── users-list.component.ts
  │   ├── users-list.component.html
  │   └── users-list.component.scss
  ├── user-create-modal/
  │   ├── user-create-modal.component.ts
  │   └── user-create-modal.component.html
  └── user-edit-modal/
      ├── user-edit-modal.component.ts
      └── user-edit-modal.component.html
```

**Model**:
```typescript
export enum UserRole {
  Admin = 0,
  CompanyAdmin = 1,
  CompanyUser = 2
}

export interface User {
  id: string;
  companyId?: string;        // NULL para Admin Master
  companyName?: string;
  name: string;
  email: string;
  role: UserRole;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
  lastLoginAt?: Date;
}

export interface CreateUserRequest {
  companyId?: string;
  name: string;
  email: string;
  password: string;
  role: UserRole;
}
```

**Validações**:
- Email único
- Email válido
- Senha mínimo 8 caracteres
- Admin Master não pode ter companyId
- CompanyAdmin e CompanyUser devem ter companyId

---

### 3.2 ❌ Storage Locations (Localizações) - NÃO EXISTE

**Prioridade**: 🔴 CRÍTICA - CORE DO WMS

**Endpoints Backend**:
- POST /api/storagelocations
- GET /api/storagelocations/{id}
- GET /api/storagelocations/warehouse/{warehouseId}
- PUT /api/storagelocations/{id}
- POST /api/storagelocations/{id}/block
- POST /api/storagelocations/{id}/unblock
- DELETE /api/storagelocations/{id}

**Model**:
```typescript
export enum LocationType {
  Pallet = 0,
  Shelf = 1,
  Floor = 2,
  Bulk = 3,
  Rack = 4,
  Bin = 5,
  DriveIn = 6,
  Cantilever = 7
}

export interface StorageLocation {
  id: string;
  warehouseId: string;
  warehouseName?: string;
  zoneId?: string;
  zoneName?: string;
  code: string;                    // Ex: A-01-2-B
  description?: string;
  
  // Estrutura WMS
  aisle?: string;                  // Corredor: A, B, C
  rack?: string;                   // Rack: 01, 02, 03
  level?: string;                  // Nível: 1, 2, 3
  position?: string;               // Posição: A, B, C
  type: LocationType;
  
  // Capacidade
  maxWeight: number;
  maxVolume: number;
  currentWeight: number;
  currentVolume: number;
  
  // Bloqueio
  isBlocked: boolean;
  blockReason?: string;
  
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}
```

**Funcionalidades essenciais**:
1. **Criação em massa**: gerar múltiplas localizações
   - Ex: A-01 a A-10, níveis 1-5, posições A-D
   - Gera 10 × 5 × 4 = 200 localizações

2. **Visualização de mapa**: grid visual do armazém

3. **Status em tempo real**: 
   - Vazia
   - Parcialmente ocupada
   - Cheia
   - Bloqueada

4. **Bloqueio/Desbloqueio** com motivo

---

### 3.3 ❌ Warehouse Zones (Zonas) - NÃO EXISTE

**Prioridade**: 🔴 CRÍTICA

**Endpoints Backend**:
- POST /api/warehousezones
- GET /api/warehousezones/{id}
- GET /api/warehousezones/warehouse/{warehouseId}
- PUT /api/warehousezones/{id}
- DELETE /api/warehousezones/{id}

**Model**:
```typescript
export enum ZoneType {
  Receiving = 0,
  Storage = 1,
  Picking = 2,
  Packing = 3,
  Shipping = 4,
  Staging = 5,
  Returns = 6,
  Quarantine = 7,
  Refrigerated = 8,
  Hazmat = 9
}

export interface WarehouseZone {
  id: string;
  warehouseId: string;
  warehouseName?: string;
  zoneName: string;
  type: ZoneType;
  temperature?: number;
  humidity?: number;
  totalCapacity: number;
  usedCapacity: number;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}
```

---

## 4. MÓDULOS DE FLUXO INBOUND

### 4.1 ❌ Receipts (Recebimentos) - NÃO EXISTE

**Prioridade**: 🔴 CRÍTICA

**Endpoints Backend**:
- POST /api/receipts
- GET /api/receipts/{id}
- GET /api/receipts/shipment/{shipmentId}

**Model**:
```typescript
export enum ReceiptStatus {
  Draft = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3
}

export interface Receipt {
  id: string;
  receiptNumber: string;
  inboundShipmentId: string;
  warehouseId: string;
  status: ReceiptStatus;
  receivedBy: string;
  receivedByName?: string;
  receivedDate?: Date;
  lines: ReceiptLine[];
  createdAt: Date;
  updatedAt?: Date;
}

export interface ReceiptLine {
  id: string;
  receiptId: string;
  productId: string;
  productName?: string;
  sku: string;
  quantityExpected: number;
  quantityReceived: number;
  lotNumber?: string;
  expiryDate?: Date;
  notes?: string;
}
```

**Fluxo de tela**:
1. Selecionar InboundShipment
2. Para cada item esperado:
   - Mostrar quantidade esperada
   - Input quantidade recebida
   - Se produto requer lote: input lote e validade
   - Campo de observações
3. Comparar esperado vs recebido
4. Se diferença > 10%: alertar
5. Completar recebimento

---

### 4.2 ❌ Putaway Tasks (Endereçamento) - NÃO EXISTE

**Prioridade**: 🔴 CRÍTICA

**Endpoints Backend**:
- POST /api/putawaytasks
- GET /api/putawaytasks/{id}
- GET /api/putawaytasks/receipt/{receiptId}
- POST /api/putawaytasks/{id}/assign
- POST /api/putawaytasks/{id}/complete
- DELETE /api/putawaytasks/{id}

**Model**:
```typescript
export enum PutawayStatus {
  Pending = 0,
  Assigned = 1,
  InProgress = 2,
  Completed = 3,
  Cancelled = 4
}

export interface PutawayTask {
  id: string;
  taskNumber: string;
  receiptId: string;
  productId: string;
  productName?: string;
  sku: string;
  quantity: number;
  fromLocationId: string;        // Staging area
  fromLocationCode?: string;
  toLocationId: string;          // Storage location
  toLocationCode?: string;
  suggestedLocationId?: string;  // Sugestão do sistema
  lotId?: string;
  status: PutawayStatus;
  assignedTo?: string;
  assignedToName?: string;
  startedAt?: Date;
  completedAt?: Date;
  createdAt: Date;
  updatedAt?: Date;
}
```

**Tela para operador**:
1. Lista de tarefas pendentes/atribuídas
2. Escanear produto (barcode)
3. Confirmar quantidade
4. Mostrar localização sugerida
5. Permitir escolher outra localização
6. Escanear localização de destino
7. Completar tarefa

---

## 5. MÓDULOS DE FLUXO OUTBOUND

### 5.1 ❌ Picking Waves (Ondas de Separação) - NÃO EXISTE

**Prioridade**: 🔴 CRÍTICA

**Endpoints Backend**:
- POST /api/pickingwaves
- GET /api/pickingwaves/{id}
- GET /api/pickingwaves/warehouse/{warehouseId}
- POST /api/pickingwaves/{id}/release
- POST /api/pickingwaves/{id}/complete

**Model**:
```typescript
export enum PickingWaveStatus {
  Pending = 0,
  Released = 1,
  InProgress = 2,
  Completed = 3,
  Cancelled = 4
}

export enum PickingStrategy {
  Discrete = 0,     // Um pedido por vez
  Batch = 1,        // Vários pedidos juntos
  Wave = 2,         // Por zona/horário
  Zone = 3          // Por zona com consolidação
}

export interface PickingWave {
  id: string;
  waveNumber: string;
  warehouseId: string;
  strategy: PickingStrategy;
  status: PickingWaveStatus;
  totalOrders: number;
  totalLines: number;
  totalQuantity: number;
  releasedAt?: Date;
  completedAt?: Date;
  orders: string[];              // Order IDs
  createdAt: Date;
  updatedAt?: Date;
}
```

**Funcionalidades**:
1. Criar onda manualmente ou automática
2. Adicionar pedidos à onda
3. Liberar onda (aloca estoque)
4. Gerar picking tasks
5. Monitorar progresso
6. Completar onda

---

## 6. MÓDULOS DE RASTREABILIDADE

### 6.1 ❌ Lots (Lotes) - NÃO EXISTE

**Prioridade**: 🔴 CRÍTICA para produtos perecíveis

**Endpoints Backend**:
- POST /api/lots
- GET /api/lots/{id}
- GET /api/lots/product/{productId}
- PUT /api/lots/{id}
- DELETE /api/lots/{id}

**Model**:
```typescript
export enum LotStatus {
  Available = 0,
  Reserved = 1,
  Expired = 2,
  Blocked = 3
}

export interface Lot {
  id: string;
  companyId: string;
  lotNumber: string;
  productId: string;
  productName?: string;
  manufactureDate: Date;
  expiryDate?: Date;
  quantityReceived: number;
  quantityAvailable: number;
  quantityReserved: number;
  status: LotStatus;
  supplierId?: string;
  supplierName?: string;
  notes?: string;
  createdAt: Date;
  updatedAt?: Date;
}
```

**Funcionalidades**:
1. Criar lote no recebimento
2. Rastrear quantidade disponível
3. FEFO (First Expired, First Out)
4. Alertas de vencimento
5. Bloquear lotes vencidos
6. Rastreabilidade completa

---

### 6.2 ❌ Stock Movements (Movimentações) - NÃO EXISTE

**Prioridade**: 🟡 ALTA - Rastreabilidade

**Endpoints Backend**:
- POST /api/stockmovements
- GET /api/stockmovements/{id}
- GET /api/stockmovements/product/{productId}
- GET /api/stockmovements

**Model**:
```typescript
export enum MovementType {
  Inbound = 0,           // Recebimento
  Outbound = 1,          // Expedição
  Transfer = 2,          // Transferência
  Adjustment = 3,        // Ajuste
  Return = 4,            // Devolução
  Putaway = 5,           // Endereçamento
  Picking = 6,           // Separação
  CycleCount = 7         // Contagem
}

export interface StockMovement {
  id: string;
  productId: string;
  productName?: string;
  fromLocationId?: string;
  fromLocationCode?: string;
  toLocationId?: string;
  toLocationCode?: string;
  quantity: number;
  type: MovementType;
  referenceId?: string;         // ID do documento origem
  referenceType?: string;       // Receipt, PickingTask, etc
  lotId?: string;
  serialNumber?: string;
  performedBy: string;
  performedByName?: string;
  notes?: string;
  createdAt: Date;
}
```

**Tela de consulta**:
- Filtros: produto, data, tipo, usuário
- Timeline de movimentações
- Detalhes de cada movimento
- Exportação de relatório

---

## 7. MÓDULOS DE GESTÃO DE PÁTIO

### 7.1 ❌ Vehicle Appointments (Agendamentos) - NÃO EXISTE

**Prioridade**: 🟡 ALTA

**Endpoints Backend**:
- POST /api/vehicleappointments
- GET /api/vehicleappointments/{id}
- GET /api/vehicleappointments/warehouse/{warehouseId}
- POST /api/vehicleappointments/{id}/checkin
- POST /api/vehicleappointments/{id}/checkout
- DELETE /api/vehicleappointments/{id}

**Model**:
```typescript
export enum AppointmentType {
  Inbound = 1,
  Outbound = 2
}

export enum AppointmentStatus {
  Scheduled = 0,
  CheckedIn = 1,
  InProgress = 2,
  Completed = 3,
  Cancelled = 4,
  NoShow = 5
}

export interface VehicleAppointment {
  id: string;
  appointmentNumber: string;
  warehouseId: string;
  type: AppointmentType;
  scheduledDate: Date;
  vehicleId?: string;
  vehiclePlate?: string;
  driverId?: string;
  driverName?: string;
  dockDoorId?: string;
  dockDoorNumber?: string;
  status: AppointmentStatus;
  checkInTime?: Date;
  checkOutTime?: Date;
  notes?: string;
  createdAt: Date;
  updatedAt?: Date;
}
```

**Tela de portaria**:
1. Calendário de agendamentos
2. Check-in de veículos chegando
3. Atribuir porta de docagem
4. Monitorar tempo de permanência
5. Check-out ao finalizar
6. KPIs: tempo médio, fila, etc

---

## 📊 RESUMO DE ARQUIVOS A CRIAR

### Componentes Compartilhados (4 arquivos)
- Modal genérico
- Formulário genérico
- Autocomplete
- Tabela com filtros

### Módulos Novos (13 módulos × ~5 arquivos cada = 65 arquivos)
- Users
- Companies  
- Storage Locations
- Warehouse Zones
- Receipts
- Putaway Tasks
- Picking Waves
- Picking Tasks (completar)
- Packages
- Stock Movements
- Lots
- Serial Numbers
- Vehicle Appointments

### Modais para Módulos Existentes (8 módulos × 2 modais = 16 arquivos)
- Products (create + edit)
- Orders (create + edit)
- Customers (create + edit)
- Suppliers (create + edit)
- Warehouses (create + edit)
- Vehicles (create + edit)
- Drivers (create + edit)
- Inventory (adjust)

### Models/DTOs (30+ arquivos)
- Corrigir e expandir models existentes
- Criar models faltantes
- Criar enums

**TOTAL ESTIMADO**: ~120 novos arquivos + refatoração de 30 existentes

---

**PRÓXIMO DOCUMENTO**: Exemplos de código para cada módulo
