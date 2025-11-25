# EXEMPLOS DE CÓDIGO - IMPLEMENTAÇÃO FRONTEND

**Data**: 2025-11-24

---

## 1. MODAL GENÉRICO REUTILIZÁVEL

### modal.service.ts
```typescript
import { Injectable, inject, ComponentRef } from '@angular/core';
import { Subject } from 'rxjs';

export interface ModalConfig {
  title?: string;
  size?: 'sm' | 'md' | 'lg' | 'xl';
  data?: any;
}

@Injectable({ providedIn: 'root' })
export class ModalService {
  private modals: Map<string, ComponentRef<any>> = new Map();
  
  open<T>(component: any, config: ModalConfig = {}): ComponentRef<T> {
    // Implementação do modal
    return componentRef;
  }
  
  close(id?: string): void {
    // Fecha modal
  }
}
```

---

## 2. PRODUCT CREATE MODAL - EXEMPLO COMPLETO

### product-create-modal.component.ts
```typescript
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { ProductsService } from '../products.service';
import { ModalService } from '@shared/services/modal.service';
import { AuthService } from '@core/services/auth.service';

@Component({
  selector: 'app-product-create-modal',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  template: `
    <form [formGroup]="form" (ngSubmit)="onSubmit()">
      <h2>Criar Novo Produto</h2>
      
      <div class="form-group">
        <label>Nome *</label>
        <input formControlName="name" />
        <span *ngIf="form.get('name')?.errors?.['required']">
          Nome é obrigatório
        </span>
      </div>
      
      <div class="form-group">
        <label>SKU *</label>
        <input formControlName="sku" />
      </div>
      
      <div class="form-group">
        <label>Código de Barras</label>
        <input formControlName="barcode" />
      </div>
      
      <div class="form-row">
        <div class="form-group">
          <label>Peso *</label>
          <input type="number" formControlName="weight" />
        </div>
        <div class="form-group">
          <label>Unidade</label>
          <select formControlName="weightUnit">
            <option value="kg">kg</option>
            <option value="g">g</option>
            <option value="lb">lb</option>
          </select>
        </div>
      </div>
      
      <div class="form-group">
        <label>
          <input type="checkbox" formControlName="requiresLotTracking" />
          Requer Rastreamento por Lote
        </label>
      </div>
      
      <div class="form-group">
        <label>
          <input type="checkbox" formControlName="isPerishable" />
          Produto Perecível
        </label>
      </div>
      
      <div *ngIf="form.get('isPerishable')?.value" class="form-group">
        <label>Validade (dias) *</label>
        <input type="number" formControlName="shelfLifeDays" />
      </div>
      
      <div class="actions">
        <button type="button" (click)="onCancel()">Cancelar</button>
        <button type="submit" [disabled]="form.invalid || saving()">
          {{ saving() ? 'Salvando...' : 'Criar Produto' }}
        </button>
      </div>
    </form>
  `
})
export class ProductCreateModalComponent {
  private fb = inject(FormBuilder);
  private productsService = inject(ProductsService);
  private modalService = inject(ModalService);
  private authService = inject(AuthService);
  
  saving = signal(false);
  
  form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    sku: ['', [Validators.required]],
    barcode: [''],
    description: [''],
    weight: [0, [Validators.required, Validators.min(0)]],
    weightUnit: ['kg'],
    volume: [0],
    length: [0],
    width: [0],
    height: [0],
    requiresLotTracking: [false],
    requiresSerialTracking: [false],
    isPerishable: [false],
    shelfLifeDays: [null],
    minimumStock: [0],
    safetyStock: [0]
  });
  
  async onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    
    this.saving.set(true);
    
    try {
      const user = this.authService.currentUser();
      const data = {
        ...this.form.value,
        companyId: user?.companyId
      };
      
      await this.productsService.create(data);
      this.modalService.close();
    } catch (error: any) {
      alert(error.message || 'Erro ao criar produto');
    } finally {
      this.saving.set(false);
    }
  }
  
  onCancel() {
    this.modalService.close();
  }
}
```

---

## 3. ORDER CREATE COM ITEMS

### order-create-modal.component.ts
```typescript
import { Component, inject, signal, computed } from '@angular/core';
import { FormBuilder, FormArray, Validators } from '@angular/forms';

export class OrderCreateModalComponent {
  private fb = inject(FormBuilder);
  
  form = this.fb.group({
    orderNumber: ['', Validators.required],
    type: [OrderType.Outbound, Validators.required],
    customerId: [''],
    supplierId: [''],
    expectedDate: [null],
    priority: [OrderPriority.Normal],
    shippingAddress: [''],
    items: this.fb.array([])
  });
  
  items = computed(() => this.form.get('items') as FormArray);
  totalQuantity = computed(() => 
    this.items().controls.reduce((sum, item) => 
      sum + (item.get('quantityOrdered')?.value || 0), 0
    )
  );
  
  totalValue = computed(() => 
    this.items().controls.reduce((sum, item) => {
      const qty = item.get('quantityOrdered')?.value || 0;
      const price = item.get('unitPrice')?.value || 0;
      return sum + (qty * price);
    }, 0)
  );
  
  addItem() {
    const itemForm = this.fb.group({
      productId: ['', Validators.required],
      sku: [''],
      quantityOrdered: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]]
    });
    
    this.items().push(itemForm);
  }
  
  removeItem(index: number) {
    this.items().removeAt(index);
  }
  
  async onProductSelected(index: number, product: any) {
    const item = this.items().at(index);
    item.patchValue({
      productId: product.id,
      sku: product.sku,
      unitPrice: product.unitPrice || 0
    });
  }
  
  async onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    
    const data = {
      ...this.form.value,
      companyId: this.authService.currentUser()?.companyId,
      totalQuantity: this.totalQuantity(),
      totalValue: this.totalValue()
    };
    
    await this.ordersService.create(data);
    this.modalService.close();
  }
}
```

---

## 4. AUTOCOMPLETE GENÉRICO

### autocomplete.component.ts
```typescript
import { Component, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-autocomplete',
  template: `
    <div class="autocomplete">
      <input 
        [value]="searchTerm()"
        (input)="onSearch($event)"
        [placeholder]="placeholder()"
      />
      
      <div *ngIf="showResults()" class="results">
        <div 
          *ngFor="let item of filteredItems()"
          (click)="onSelect(item)"
          class="result-item"
        >
          {{ getDisplayValue(item) }}
        </div>
      </div>
    </div>
  `
})
export class AutocompleteComponent {
  items = input<any[]>([]);
  displayField = input<string>('name');
  valueField = input<string>('id');
  placeholder = input<string>('Buscar...');
  
  selectionChange = output<any>();
  
  searchTerm = signal('');
  showResults = signal(false);
  
  filteredItems = computed(() => {
    const term = this.searchTerm().toLowerCase();
    return this.items().filter(item => 
      this.getDisplayValue(item).toLowerCase().includes(term)
    );
  });
  
  getDisplayValue(item: any): string {
    return item[this.displayField()];
  }
  
  onSearch(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.searchTerm.set(value);
    this.showResults.set(true);
  }
  
  onSelect(item: any) {
    this.selectionChange.emit(item);
    this.searchTerm.set(this.getDisplayValue(item));
    this.showResults.set(false);
  }
}
```

---

## 5. STORAGE LOCATION BULK CREATE

```typescript
export class LocationBulkCreateComponent {
  form = this.fb.group({
    aisleStart: ['A', Validators.required],
    aisleEnd: ['C', Validators.required],
    rackStart: [1, Validators.required],
    rackEnd: [10, Validators.required],
    levelStart: [1, Validators.required],
    levelEnd: [5, Validators.required],
    positionStart: ['A'],
    positionEnd: ['D'],
    type: [LocationType.Pallet],
    maxWeight: [1000],
    maxVolume: [10]
  });
  
  preview = computed(() => {
    const aisles = this.generateRange(
      this.form.get('aisleStart')?.value,
      this.form.get('aisleEnd')?.value
    );
    const racks = this.generateNumericRange(
      this.form.get('rackStart')?.value,
      this.form.get('rackEnd')?.value
    );
    // ... calcular total de localizações
    return { aisles, racks, total };
  });
  
  async createLocations() {
    const locations = this.generateLocations();
    for (const location of locations) {
      await this.service.create(location);
    }
  }
}
```

---

## 6. MODELS CORRETOS

### product.model.ts
```typescript
export interface Product {
  id: string;
  companyId: string;
  name: string;
  sku: string;
  barcode?: string;
  description?: string;
  weight: number;
  weightUnit?: string;
  volume: number;
  volumeUnit?: string;
  length: number;
  width: number;
  height: number;
  dimensionUnit?: string;
  requiresLotTracking: boolean;
  requiresSerialTracking: boolean;
  isPerishable: boolean;
  shelfLifeDays?: number;
  minimumStock?: number;
  safetyStock?: number;
  abcClassification?: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}
```

### order.model.ts
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

export interface Order {
  id: string;
  companyId: string;
  orderNumber: string;
  type: OrderType;
  source: OrderSource;
  customerId?: string;
  supplierId?: string;
  orderDate: Date;
  expectedDate?: Date;
  priority: OrderPriority;
  status: OrderStatus;
  totalQuantity: number;
  totalValue: number;
  items: OrderItem[];
  createdAt: Date;
}
```

---

**Total de documentos criados**: 3  
**Próximo passo**: Iniciar Fase 1 do Roadmap
