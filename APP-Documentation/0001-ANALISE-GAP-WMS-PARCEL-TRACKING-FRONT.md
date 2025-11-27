# 📱 ANÁLISE FRONTEND - PURCHASE ORDER & PARCEL TRACKING

**Data**: 2025-11-27  
**Versão**: 1.0  
**Tipo**: Implementação Frontend Angular  
**Referência Backend**: `API-Documentation/0001-ANALISE-GAP-WMS-PARCEL-TRACKING.md`

---

## 📋 SUMÁRIO EXECUTIVO

### Objetivo
Implementar interface completa para **Purchase Orders (Pedidos de Compra)** com funcionalidades de:
- ✅ Criar/Editar Purchase Orders (PO)
- ✅ Definir hierarquia de embalagem (Pallets → Caixas → Unidades)
- ✅ Upload de documentos (Invoice, DI, BL, etc.) em WebP
- ✅ Impressão de Purchase Order
- ✅ Dashboard de recebimento (com progresso)
- ✅ Rastreabilidade completa (LPN, Barcodes, Serials)

### Tecnologias
- **Framework**: Angular 17+ (Standalone Components)
- **Estado**: Signals
- **UI**: TailwindCSS + Dark Mode
- **i18n**: 3 idiomas (pt-BR, en-US, es-ES)
- **Padrão**: Componentes reutilizáveis

---

## 1. ESTRUTURA DE ARQUIVOS IMPLEMENTADA ✅

### 1.1 Product Categories (COMPLETO)

```
APP/src/app/features/product-categories/
├── product-categories.component.ts          # Lista + CRUD
├── product-categories.component.html        # UI com dark mode
└── product-categories.component.scss        # Tailwind CSS

APP/src/app/core/services/
└── product-categories.service.ts            # HTTP Service
```

**Funcionalidades**:
- ✅ Listar todas as categorias
- ✅ Criar nova categoria
- ✅ Editar categoria existente
- ✅ Ativar/Desativar categoria
- ✅ Excluir categoria (se sem produtos)
- ✅ Busca por nome/código
- ✅ Dark mode completo
- ✅ i18n (pt-BR, en-US, es-ES)

### 1.2 Purchase Orders (COMPLETO)

```
APP/src/app/features/purchase-orders/
├── purchase-orders.component.ts             # Lista + CRUD
├── purchase-orders.component.html           # UI com dark mode
└── purchase-orders.component.scss           # Tailwind CSS

APP/src/app/core/services/
└── purchase-orders.service.ts               # HTTP Service
```

**Funcionalidades**:
- ✅ Listar Purchase Orders por empresa
- ✅ Criar PO com seleção de categoria → produto
- ✅ Adicionar múltiplos itens
- ✅ Cálculo automático de totais
- ✅ Filtro por categoria antes de selecionar produto
- ✅ Integração com fornecedores, armazéns, veículos, motoristas
- ✅ Dark mode completo
- ✅ i18n (pt-BR, en-US, es-ES)

### 1.3 Estrutura Futura (PLANEJADA)

```
APP/src/app/features/purchase-orders/ (EXPANDIR)
├── models/
│   ├── purchase-order.model.ts                  # Interfaces
│   ├── order-document.model.ts
│   ├── inbound-parcel.model.ts
│   └── inbound-carton.model.ts
├── purchase-order-details/
│   ├── purchase-order-details.component.ts      # Detalhes + Tabs
│   ├── purchase-order-details.component.html
│   └── purchase-order-details.component.scss
├── components/
│   ├── purchase-details-form/                   # Preços e margens
│   ├── packaging-hierarchy-form/                # Hierarquia (pallets/caixas)
│   ├── international-form/                      # Dados importação
│   ├── documents-upload/                        # Upload múltiplo WebP
│   ├── receiving-dashboard/                     # Dashboard recebimento
│   └── print-preview/                           # Preview de impressão
└── shared/
    ├── supplier-selector/                       # Selector de fornecedor (REUTILIZÁVEL)
    ├── product-selector/                        # Selector de produto (REUTILIZÁVEL)
    └── document-viewer/                         # Visualizador WebP (REUTILIZÁVEL)
```

---

## 2. ANÁLISE DOS COMPONENTES EXISTENTES (PADRÃO)

### 2.1 OrdersListComponent (REFERÊNCIA)

**Padrões identificados**:

```typescript
@Component({
  selector: 'app-orders-list',
  standalone: true,
  imports: [CommonModule, FormsModule, /* ... */],
  templateUrl: './orders-list.component.html'
})
export class OrdersListComponent implements OnInit {
  // ✅ Injeção via inject()
  private readonly ordersService = inject(OrdersService);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);
  
  // ✅ Signals (Angular 17+)
  loading = signal<boolean>(false);
  orders = signal<Order[]>([]);
  selectedOrder = signal<Order | null>(null);
  
  // ✅ Computed
  totalOrders = computed(() => this.orders().length);
  
  // ✅ Métodos assíncronos
  async loadOrders(): Promise<void> {
    this.loading.set(true);
    try {
      const response = await this.ordersService.getAll();
      this.orders.set(response.data || []);
    } catch (error) {
      console.error('Error loading orders:', error);
    } finally {
      this.loading.set(false);
    }
  }
  
  // ✅ Classes CSS dark mode
  getStatusClass(status: string): string {
    const classes = {
      'Pending': 'bg-amber-50 dark:bg-amber-900/30 text-amber-600 dark:text-amber-400',
      'Processing': 'bg-blue-50 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400',
      'Completed': 'bg-green-50 dark:bg-green-900/30 text-green-600 dark:text-green-400'
    };
    return classes[status] || 'bg-slate-50 dark:bg-slate-700';
  }
}
```

**Template HTML**:
```html
<div class="p-6 bg-white dark:bg-slate-900 min-h-screen">
  <!-- Header -->
  <div class="flex items-center justify-between mb-6">
    <h1 class="text-2xl font-bold text-slate-800 dark:text-slate-100">
      {{ i18n.t('orders.title') }}
    </h1>
    <button (click)="openCreateModal()"
            class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">
      {{ i18n.t('common.buttons.create') }}
    </button>
  </div>
  
  <!-- Loading -->
  @if (loading()) {
    <div class="flex justify-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
    </div>
  }
  
  <!-- Tabela -->
  @if (!loading() && orders().length > 0) {
    <div class="bg-white dark:bg-slate-800 rounded-lg shadow overflow-hidden">
      <table class="min-w-full divide-y divide-slate-200 dark:divide-slate-700">
        <thead class="bg-slate-50 dark:bg-slate-900">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase">
              {{ i18n.t('orders.columns.orderNumber') }}
            </th>
            <!-- Mais colunas -->
          </tr>
        </thead>
        <tbody class="divide-y divide-slate-200 dark:divide-slate-700">
          @for (order of orders(); track order.id) {
            <tr class="hover:bg-slate-50 dark:hover:bg-slate-700/50 cursor-pointer"
                (click)="selectOrder(order)">
              <td class="px-6 py-4 text-sm text-slate-800 dark:text-slate-200">
                {{ order.orderNumber }}
              </td>
              <!-- Mais células -->
            </tr>
          }
        </tbody>
      </table>
    </div>
  }
  
  <!-- Empty state -->
  @if (!loading() && orders().length === 0) {
    <div class="text-center py-12">
      <p class="text-slate-500 dark:text-slate-400">
        {{ i18n.t('orders.emptyState') }}
      </p>
    </div>
  }
</div>
```

### 2.2 Padrões CSS Obrigatórios

**1. Cores (Dark Mode)**:
```css
/* Background */
bg-white dark:bg-slate-900           /* Página principal */
bg-slate-50 dark:bg-slate-800        /* Cards/tabelas */
bg-slate-100 dark:bg-slate-700       /* Hover */

/* Text */
text-slate-800 dark:text-slate-100   /* Títulos */
text-slate-600 dark:text-slate-300   /* Texto normal */
text-slate-500 dark:text-slate-400   /* Labels/placeholders */

/* Borders */
border-slate-200 dark:border-slate-700

/* Status badges */
bg-green-50 dark:bg-green-900/30 text-green-600 dark:text-green-400  /* Success */
bg-amber-50 dark:bg-amber-900/30 text-amber-600 dark:text-amber-400  /* Warning */
bg-red-50 dark:bg-red-900/30 text-red-600 dark:text-red-400         /* Error */
bg-blue-50 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400     /* Info */
```

**2. Botões**:
```html
<!-- Primary -->
<button class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">

<!-- Secondary -->
<button class="px-4 py-2 text-slate-600 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-700 rounded-lg">

<!-- Danger -->
<button class="px-4 py-2 bg-red-600 hover:bg-red-700 text-white rounded-lg">
```

**3. Inputs**:
```html
<input class="w-full px-3 py-2 
               bg-white dark:bg-slate-800 
               text-slate-800 dark:text-slate-100
               border border-slate-300 dark:border-slate-600
               rounded-lg
               focus:outline-none focus:ring-2 focus:ring-blue-500"
       [placeholder]="i18n.t('placeholder')">
```

---

## 3. MODELOS TYPESCRIPT (CRIAR)

### 3.1 purchase-order.model.ts

```typescript
export interface PurchaseOrder {
  id: string;
  companyId: string;
  orderNumber: string;
  type: OrderType;
  source: OrderSource;
  supplierId: string;
  supplierName?: string;
  orderDate: Date;
  expectedDate: Date;
  priority: OrderPriority;
  status: OrderStatus;
  totalQuantity: number;
  totalValue: number;
  
  // Purchase Order específicos
  unitCost?: number;
  totalCost?: number;
  taxAmount?: number;
  taxPercentage?: number;
  desiredMarginPercentage?: number;
  suggestedSalePrice?: number;
  estimatedProfit?: number;
  
  // Hierarquia de embalagem
  expectedParcels?: number;
  receivedParcels?: number;
  expectedCartons?: number;
  unitsPerCarton?: number;
  cartonsPerParcel?: number;
  
  // Logística
  shippingDistance?: string;
  shippingCost?: number;
  dockDoorNumber?: string;
  
  // Internacional
  isInternational?: boolean;
  originCountry?: string;
  portOfEntry?: string;
  customsBroker?: string;
  isOwnCarrier?: boolean;
  thirdPartyCarrier?: string;
  containerNumber?: string;
  billOfLading?: string;
  importLicenseNumber?: string;
  estimatedArrivalPort?: Date;
  actualArrivalPort?: Date;
  incoterm?: string;
  
  items: PurchaseOrderItem[];
  documents?: OrderDocument[];
  createdAt: Date;
  updatedAt?: Date;
}

export interface PurchaseOrderItem {
  id: string;
  productId: string;
  sku: string;
  productName?: string;
  quantityOrdered: number;
  quantityReceived?: number;
  unitPrice: number;
}

export enum OrderType {
  Inbound = 'Inbound',
  Outbound = 'Outbound',
  Transfer = 'Transfer',
  Return = 'Return'
}

export enum OrderStatus {
  Draft = 'Draft',
  Pending = 'Pending',
  Processing = 'Processing',
  Completed = 'Completed',
  Cancelled = 'Cancelled'
}

export enum OrderPriority {
  Low = 'Low',
  Normal = 'Normal',
  High = 'High',
  Critical = 'Critical'
}
```

### 3.2 order-document.model.ts

```typescript
export interface OrderDocument {
  id: string;
  orderId: string;
  fileName: string;
  type: DocumentType;
  filePath: string;
  fileUrl: string;
  fileSizeBytes: number;
  mimeType: string;
  uploadedBy: string;
  uploadedAt: Date;
  deletedAt?: Date;
  deletedBy?: string;
}

export enum DocumentType {
  Invoice = 'Invoice',
  DI = 'DI',                  // Declaração de Importação
  BL = 'BL',                  // Bill of Lading
  PackingList = 'PackingList',
  Certificate = 'Certificate',
  Other = 'Other'
}
```

---

## 4. SERVICE (CRIAR)

### 4.1 purchase-orders.service.ts

```typescript
import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { PurchaseOrder, OrderDocument } from '../models';

@Injectable({ providedIn: 'root' })
export class PurchaseOrdersService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/orders`;

  async getAll(companyId: string): Promise<ApiResponse<PurchaseOrder[]>> {
    return this.http.get<ApiResponse<PurchaseOrder[]>>(
      `${this.apiUrl}/company/${companyId}`
    ).toPromise();
  }

  async getById(id: string): Promise<ApiResponse<PurchaseOrder>> {
    return this.http.get<ApiResponse<PurchaseOrder>>(
      `${this.apiUrl}/${id}`
    ).toPromise();
  }

  async create(data: CreatePurchaseOrderRequest): Promise<ApiResponse<PurchaseOrder>> {
    return this.http.post<ApiResponse<PurchaseOrder>>(
      this.apiUrl, data
    ).toPromise();
  }

  async update(id: string, data: Partial<PurchaseOrder>): Promise<ApiResponse<PurchaseOrder>> {
    return this.http.put<ApiResponse<PurchaseOrder>>(
      `${this.apiUrl}/${id}`, data
    ).toPromise();
  }

  // Endpoints específicos Purchase Order
  async setPurchaseDetails(id: string, data: {
    unitCost: number;
    taxPercentage: number;
    desiredMarginPercentage: number;
  }): Promise<ApiResponse<PurchaseOrder>> {
    return this.http.post<ApiResponse<PurchaseOrder>>(
      `${this.apiUrl}/${id}/purchase-details`, data
    ).toPromise();
  }

  async setPackagingHierarchy(id: string, data: {
    expectedParcels: number;
    cartonsPerParcel: number;
    unitsPerCarton: number;
  }): Promise<ApiResponse<PurchaseOrder>> {
    return this.http.post<ApiResponse<PurchaseOrder>>(
      `${this.apiUrl}/${id}/packaging-hierarchy`, data
    ).toPromise();
  }

  async setAsInternational(id: string, data: {
    originCountry: string;
    portOfEntry: string;
    containerNumber: string;
    incoterm: string;
  }): Promise<ApiResponse<PurchaseOrder>> {
    return this.http.post<ApiResponse<PurchaseOrder>>(
      `${this.apiUrl}/${id}/set-international`, data
    ).toPromise();
  }

  // Documentos
  async getDocuments(orderId: string): Promise<ApiResponse<OrderDocument[]>> {
    return this.http.get<ApiResponse<OrderDocument[]>>(
      `${this.apiUrl}/${orderId}/documents`
    ).toPromise();
  }

  async uploadDocument(orderId: string, file: File, type: DocumentType): Promise<ApiResponse<OrderDocument>> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('type', type);
    
    return this.http.post<ApiResponse<OrderDocument>>(
      `${this.apiUrl}/${orderId}/documents`, formData
    ).toPromise();
  }

  async softDeleteDocument(orderId: string, documentId: string, deletedBy: string): Promise<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(
      `${this.apiUrl}/${orderId}/documents/${documentId}/soft-delete`,
      { deletedBy }
    ).toPromise();
  }

  // Impressão
  async printPurchaseOrder(id: string): Promise<Blob> {
    return this.http.get(
      `${this.apiUrl}/${id}/print`,
      { responseType: 'blob' }
    ).toPromise();
  }
}

interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: string[];
}
```

---

## 5. COMPONENTES PRINCIPAIS (CRIAR)

### 5.1 PurchaseOrdersListComponent

**Funcionalidades**:
- ✅ Listar Purchase Orders (filtrar por Inbound)
- ✅ Buscar por número/fornecedor
- ✅ Filtrar por status/prioridade
- ✅ Abrir detalhes
- ✅ Criar novo PO
- ✅ Badges de status com progress

**Template destaque**:
```html
<!-- Card com progresso de recebimento -->
<div class="bg-white dark:bg-slate-800 rounded-lg p-4">
  <div class="flex items-center justify-between mb-2">
    <span class="font-semibold">{{ po.orderNumber }}</span>
    <span [class]="getStatusClass(po.status)">{{ po.status }}</span>
  </div>
  
  <!-- Progress Bar -->
  @if (po.expectedParcels && po.expectedParcels > 0) {
    <div class="mt-3">
      <div class="flex justify-between text-xs text-slate-500 dark:text-slate-400 mb-1">
        <span>{{ i18n.t('purchaseOrders.receiving') }}</span>
        <span>{{ po.receivedParcels || 0 }} / {{ po.expectedParcels }}</span>
      </div>
      <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2">
        <div class="bg-blue-600 h-2 rounded-full transition-all"
             [style.width.%]="(po.receivedParcels || 0) / po.expectedParcels * 100">
        </div>
      </div>
    </div>
  }
</div>
```

### 5.2 PurchaseOrderFormComponent

**Wizard Multi-Step**:
```
Step 1: Informações Básicas (Fornecedor, Data, Produtos)
Step 2: Preços e Margens (unitCost, tax, margin) → Calcula automaticamente
Step 3: Hierarquia (Pallets, Caixas, Unidades) → Validação: 10×10×10 = 1.000
Step 4: Dados Internacionais (se isInternational = true)
Step 5: Revisão e Confirmação
```

**Form Reactivo**:
```typescript
purchaseOrderForm = this.fb.group({
  // Step 1
  orderNumber: ['', [Validators.required, Validators.maxLength(50)]],
  supplierId: ['', Validators.required],
  expectedDate: [null, Validators.required],
  priority: ['Normal', Validators.required],
  items: this.fb.array([]),
  
  // Step 2
  unitCost: [null, [Validators.required, Validators.min(0)]],
  taxPercentage: [null, [Validators.required, Validators.min(0), Validators.max(100)]],
  desiredMarginPercentage: [null, [Validators.required, Validators.min(0)]],
  
  // Step 3
  expectedParcels: [null, [Validators.required, Validators.min(1)]],
  cartonsPerParcel: [null, [Validators.required, Validators.min(1)]],
  unitsPerCarton: [null, [Validators.required, Validators.min(1)]],
  
  // Step 4
  isInternational: [false],
  originCountry: [''],
  portOfEntry: [''],
  containerNumber: [''],
  incoterm: ['FOB']
});

// Calculated field
get suggestedSalePrice(): number {
  const unitCost = this.purchaseOrderForm.get('unitCost')?.value || 0;
  const taxPercent = this.purchaseOrderForm.get('taxPercentage')?.value || 0;
  const marginPercent = this.purchaseOrderForm.get('desiredMarginPercentage')?.value || 0;
  
  const costWithTax = unitCost * (1 + taxPercent / 100);
  return costWithTax * (1 + marginPercent / 100);
}

// Validation
validateHierarchy(): boolean {
  const parcels = this.purchaseOrderForm.get('expectedParcels')?.value || 0;
  const cartons = this.purchaseOrderForm.get('cartonsPerParcel')?.value || 0;
  const units = this.purchaseOrderForm.get('unitsPerCarton')?.value || 0;
  const totalOrdered = this.getTotalQuantity();
  
  return (parcels * cartons * units) === totalOrdered;
}
```

### 5.3 PurchaseOrderDetailsComponent

**Tabs**:
1. **Resumo**: Informações gerais + cálculos
2. **Produtos**: Lista de items
3. **Documentos**: Upload/visualização
4. **Recebimento**: Dashboard (se status != Draft)
5. **Rastreabilidade**: Timeline completa

```html
<div class="bg-white dark:bg-slate-900 min-h-screen p-6">
  <!-- Header com ações -->
  <div class="flex items-center justify-between mb-6">
    <div>
      <h1 class="text-2xl font-bold text-slate-800 dark:text-slate-100">
        {{ purchaseOrder()?.orderNumber }}
      </h1>
      <p class="text-slate-500 dark:text-slate-400">
        {{ purchaseOrder()?.supplierName }}
      </p>
    </div>
    
    <div class="flex gap-3">
      <button (click)="print()" 
              class="px-4 py-2 bg-slate-600 hover:bg-slate-700 text-white rounded-lg">
        {{ i18n.t('common.buttons.print') }}
      </button>
      <button (click)="edit()"
              class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">
        {{ i18n.t('common.buttons.edit') }}
      </button>
    </div>
  </div>
  
  <!-- Tabs -->
  <div class="border-b border-slate-200 dark:border-slate-700 mb-6">
    <nav class="flex gap-6">
      @for (tab of tabs; track tab.id) {
        <button (click)="activeTab.set(tab.id)"
                [class]="getTabClass(tab.id)"
                class="pb-3 px-1 font-medium text-sm">
          {{ i18n.t(tab.label) }}
        </button>
      }
    </nav>
  </div>
  
  <!-- Tab Content -->
  @switch (activeTab()) {
    @case ('summary') {
      <app-purchase-order-summary [purchaseOrder]="purchaseOrder()" />
    }
    @case ('products') {
      <app-purchase-order-products [items]="purchaseOrder()?.items" />
    }
    @case ('documents') {
      <app-documents-upload [orderId]="purchaseOrder()?.id" />
    }
    @case ('receiving') {
      <app-receiving-dashboard [orderId]="purchaseOrder()?.id" />
    }
    @case ('traceability') {
      <app-traceability-timeline [orderId]="purchaseOrder()?.id" />
    }
  }
</div>
```

---

## 6. COMPONENTES REUTILIZÁVEIS (CRIAR)

### 6.1 SupplierSelectorComponent

```typescript
@Component({
  selector: 'app-supplier-selector',
  standalone: true,
  template: `
    <div class="relative">
      <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
        {{ i18n.t('purchaseOrders.fields.supplier') }}
      </label>
      <input [(ngModel)]="searchTerm"
             (input)="filterSuppliers()"
             [placeholder]="i18n.t('common.placeholders.search')"
             class="w-full px-3 py-2 bg-white dark:bg-slate-800 border rounded-lg">
      
      @if (showDropdown()) {
        <div class="absolute z-10 mt-1 w-full bg-white dark:bg-slate-800 border rounded-lg shadow-lg max-h-60 overflow-auto">
          @for (supplier of filteredSuppliers(); track supplier.id) {
            <div (click)="selectSupplier(supplier)"
                 class="px-3 py-2 hover:bg-slate-100 dark:hover:bg-slate-700 cursor-pointer">
              <p class="font-medium">{{ supplier.name }}</p>
              <p class="text-xs text-slate-500">{{ supplier.email }}</p>
            </div>
          }
        </div>
      }
    </div>
  `
})
export class SupplierSelectorComponent {
  @Input() selectedId: string | null = null;
  @Output() supplierSelected = new EventEmitter<Supplier>();
  
  protected readonly i18n = inject(I18nService);
  private readonly suppliersService = inject(SuppliersService);
  
  suppliers = signal<Supplier[]>([]);
  filteredSuppliers = signal<Supplier[]>([]);
  showDropdown = signal<boolean>(false);
  searchTerm = '';
}
```

### 6.2 DocumentsUploadComponent

```typescript
@Component({
  selector: 'app-documents-upload',
  template: `
    <div class="space-y-4">
      <!-- Upload Area -->
      <div class="border-2 border-dashed border-slate-300 dark:border-slate-600 rounded-lg p-6 text-center">
        <input #fileInput type="file" accept="image/*" multiple hidden 
               (change)="onFilesSelected($event)">
        <button (click)="fileInput.click()"
                class="px-4 py-2 bg-blue-600 text-white rounded-lg">
          {{ i18n.t('documents.upload') }}
        </button>
        <p class="text-sm text-slate-500 mt-2">
          {{ i18n.t('documents.uploadHint') }}
        </p>
      </div>
      
      <!-- Lista de documentos -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        @for (doc of documents(); track doc.id) {
          <div class="bg-white dark:bg-slate-800 rounded-lg p-4 border">
            <div class="flex items-start justify-between mb-2">
              <span [class]="getDocTypeClass(doc.type)">
                {{ doc.type }}
              </span>
              <button (click)="deleteDocument(doc.id)"
                      class="text-red-600 hover:text-red-700">
                <svg><!-- trash icon --></svg>
              </button>
            </div>
            <p class="text-sm font-medium truncate">{{ doc.fileName }}</p>
            <p class="text-xs text-slate-500">{{ formatFileSize(doc.fileSizeBytes) }}</p>
            <button (click)="viewDocument(doc)"
                    class="mt-2 text-blue-600 hover:text-blue-700 text-sm">
              {{ i18n.t('common.buttons.view') }}
            </button>
          </div>
        }
      </div>
    </div>
  `
})
```

---

## 7. INTERNACIONALIZAÇÃO (i18n)

### 7.1 Adicionar chaves nos 3 idiomas

**Arquivo**: `APP/src/assets/i18n/pt-BR.json`

```json
{
  "purchaseOrders": {
    "title": "Purchase Orders",
    "create": "Nova Purchase Order",
    "edit": "Editar Purchase Order",
    "details": "Detalhes da Purchase Order",
    "list": "Lista de Purchase Orders",
    "emptyState": "Nenhuma purchase order encontrada",
    "fields": {
      "orderNumber": "Número da Ordem",
      "supplier": "Fornecedor",
      "expectedDate": "Data Esperada",
      "priority": "Prioridade",
      "status": "Status",
      "totalValue": "Valor Total",
      "unitCost": "Custo Unitário",
      "taxPercentage": "Impostos (%)",
      "desiredMargin": "Margem Desejada (%)",
      "suggestedPrice": "Preço Sugerido",
      "expectedParcels": "Pallets Esperados",
      "cartonsPerParcel": "Caixas por Pallet",
      "unitsPerCarton": "Unidades por Caixa",
      "isInternational": "Importação Internacional",
      "originCountry": "País de Origem",
      "portOfEntry": "Porto de Entrada",
      "containerNumber": "Número do Container",
      "incoterm": "Incoterm"
    },
    "tabs": {
      "summary": "Resumo",
      "products": "Produtos",
      "documents": "Documentos",
      "receiving": "Recebimento",
      "traceability": "Rastreabilidade"
    },
    "receiving": "Recebimento",
    "validation": {
      "hierarchyMismatch": "Hierarquia não corresponde à quantidade total"
    }
  },
  "documents": {
    "upload": "Upload de Documento",
    "uploadHint": "Arraste arquivos ou clique para selecionar",
    "types": {
      "Invoice": "Nota Fiscal",
      "DI": "Declaração de Importação",
      "BL": "Bill of Lading",
      "PackingList": "Packing List",
      "Certificate": "Certificado",
      "Other": "Outro"
    }
  }
}
```

**en-US.json** e **es-ES.json**: Mesma estrutura traduzida

---

## 8. ROTAS (CRIAR)

**Arquivo**: `APP/src/app/features/purchase-orders/purchase-orders.routes.ts`

```typescript
import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';

export const purchaseOrdersRoutes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        loadComponent: () => import('./purchase-orders-list/purchase-orders-list.component')
          .then(m => m.PurchaseOrdersListComponent),
        title: 'Purchase Orders'
      },
      {
        path: 'create',
        loadComponent: () => import('./purchase-order-form/purchase-order-form.component')
          .then(m => m.PurchaseOrderFormComponent),
        title: 'Nova Purchase Order'
      },
      {
        path: ':id',
        loadComponent: () => import('./purchase-order-details/purchase-order-details.component')
          .then(m => m.PurchaseOrderDetailsComponent),
        title: 'Detalhes Purchase Order'
      },
      {
        path: ':id/edit',
        loadComponent: () => import('./purchase-order-form/purchase-order-form.component')
          .then(m => m.PurchaseOrderFormComponent),
        title: 'Editar Purchase Order'
      }
    ]
  }
];
```

**Integrar no app.routes.ts**:
```typescript
{
  path: 'purchase-orders',
  loadChildren: () => import('./features/purchase-orders/purchase-orders.routes')
    .then(m => m.purchaseOrdersRoutes)
}
```

---

## 9. CHECKLIST DE IMPLEMENTAÇÃO

### Fase 1: Estrutura Base
- [ ] Criar pasta `features/purchase-orders/`
- [ ] Criar models (purchase-order, order-document, etc.)
- [ ] Criar service (purchase-orders.service.ts)
- [ ] Criar rotas (purchase-orders.routes.ts)

### Fase 2: Componentes Principais
- [ ] PurchaseOrdersListComponent (lista + filtros)
- [ ] PurchaseOrderFormComponent (wizard multi-step)
- [ ] PurchaseOrderDetailsComponent (detalhes + tabs)

### Fase 3: Componentes Reutilizáveis
- [ ] SupplierSelectorComponent (dropdown com busca)
- [ ] ProductSelectorComponent (adicionar produtos)
- [ ] DocumentsUploadComponent (upload WebP)
- [ ] DocumentViewerComponent (visualizar WebP)
- [ ] ReceivingDashboardComponent (progresso)

### Fase 4: Formulários Específicos
- [ ] PurchaseDetailsFormComponent (preços/margens)
- [ ] PackagingHierarchyFormComponent (pallets/caixas)
- [ ] InternationalFormComponent (dados importação)

### Fase 5: i18n
- [ ] Adicionar chaves pt-BR.json
- [ ] Adicionar chaves en-US.json
- [ ] Adicionar chaves es-ES.json
- [ ] Validar script validate-i18n-keys.py

### Fase 6: Impressão
- [ ] PrintPreviewComponent (preview A4)
- [ ] Integração com backend `/api/orders/{id}/print`

### Fase 7: Testes
- [ ] Criar Purchase Order completo
- [ ] Upload de documentos
- [ ] Validação de hierarquia
- [ ] Impressão
- [ ] Dark mode em todos componentes
- [ ] Navegação entre telas

---

## 10. REGRAS DE NEGÓCIO CRÍTICAS

### 10.1 Validação de Hierarquia
```typescript
// Deve validar: pallets × caixas × unidades = quantidade total
validateHierarchy(): boolean {
  const totalUnits = this.expectedParcels * this.cartonsPerParcel * this.unitsPerCarton;
  const totalOrdered = this.items.reduce((sum, item) => sum + item.quantityOrdered, 0);
  return totalUnits === totalOrdered;
}
```

### 10.2 Cálculo Automático de Preço
```typescript
// Backend calcula, frontend apenas exibe
// Fórmula: unitCost × (1 + tax%) × (1 + margin%)
```

### 10.3 Upload de Documentos
- ✅ Converter JPG/PNG para WebP no backend
- ✅ Máximo 10MB por arquivo
- ✅ Soft delete (DeletedAt, DeletedBy)

### 10.4 Status e Progresso
```
Draft → Pending → Processing → Completed
   ↓         ↓           ↓
  0%       30%        100%
```

---

## 11. DIFERENÇAS ENTRE ORDERS E PURCHASE ORDERS

| Aspecto | Orders (Outbound) | Purchase Orders (Inbound) |
|---------|-------------------|---------------------------|
| Cliente/Fornecedor | Cliente | Fornecedor |
| Fluxo | Saída (picking) | Entrada (receiving) |
| Hierarquia | Box → Produto | Pallet → Caixa → Produto |
| Documentos | Nota Fiscal saída | Invoice, DI, BL, Packing List |
| Progress | % picking | % receiving (pallets) |
| Impressão | Etiqueta | Purchase Order A4 |
| Custos | Não | Sim (unitCost, margins) |

---

## 12. PRÓXIMOS PASSOS

1. ✅ **Backend completo** (já feito)
2. 🚧 **Criar estrutura de pastas** (próximo)
3. 🚧 **Implementar models** (próximo)
4. 🚧 **Implementar service** (próximo)
5. 🚧 **Componente lista** (próximo)
6. 🚧 **Componente form wizard** (próximo)
7. 🚧 **Componentes reutilizáveis** (próximo)
8. 🚧 **i18n completo** (próximo)
9. 🚧 **Testes end-to-end** (próximo)

---

**Documentado por**: Cascade AI  
**Data**: 2025-11-27  
**Versão**: 1.0  
**Status**: 📝 Aguardando implementação
