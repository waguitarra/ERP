import { Component, signal, computed, inject, output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { PurchaseOrdersService } from '@core/services/purchase-orders.service';
import { ProductCategoriesService, ProductCategory } from '@core/services/product-categories.service';
import { ProductsService } from '../../products/products.service';
import { SuppliersService } from '../../suppliers/suppliers.service';
import { WarehousesService } from '@core/services/warehouses.service';
import { VehiclesService } from '@core/services/vehicles.service';
import { DriversService } from '@core/services/drivers.service';
import { AuthService } from '@core/services/auth.service';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-purchase-order-create-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './purchase-order-create-modal.component.html'
})
export class PurchaseOrderCreateModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly purchaseOrdersService = inject(PurchaseOrdersService);
  private readonly categoriesService = inject(ProductCategoriesService);
  private readonly productsService = inject(ProductsService);
  private readonly suppliersService = inject(SuppliersService);
  private readonly warehousesService = inject(WarehousesService);
  private readonly vehiclesService = inject(VehiclesService);
  private readonly driversService = inject(DriversService);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  
  orderCreated = output<void>();
  
  categories = signal<ProductCategory[]>([]);
  suppliers = signal<any[]>([]);
  products = signal<any[]>([]);
  warehouses = signal<any[]>([]);
  vehicles = signal<any[]>([]);
  drivers = signal<any[]>([]);
  selectedCategoryId = signal<string>('');
  
  // Cálculos automáticos
  suggestedPrice = computed(() => {
    const unitCost = this.form.get('unitCost')?.value || 0;
    const taxRate = this.form.get('taxPercentage')?.value || 0;
    const margin = this.form.get('desiredMarginPercentage')?.value || 0;
    const costWithTax = unitCost + (unitCost * (taxRate / 100));
    return costWithTax + (costWithTax * (margin / 100));
  });
  
  estimatedProfit = computed(() => {
    const unitCost = this.form.get('unitCost')?.value || 0;
    const taxRate = this.form.get('taxPercentage')?.value || 0;
    const margin = this.form.get('desiredMarginPercentage')?.value || 0;
    const totalQty = this.getTotalQuantity();
    const costWithTax = unitCost + (unitCost * (taxRate / 100));
    const salePrice = costWithTax + (costWithTax * (margin / 100));
    return (salePrice - costWithTax) * totalQty;
  });
  
  calculatedUnits = computed(() => {
    const parcels = this.form.get('expectedParcels')?.value || 0;
    const cartonsPerParcel = this.form.get('cartonsPerParcel')?.value || 0;
    const unitsPerCarton = this.form.get('unitsPerCarton')?.value || 0;
    return parcels * cartonsPerParcel * unitsPerCarton;
  });
  
  hierarchyValid = computed(() => {
    return this.calculatedUnits() === this.getTotalQuantity();
  });

  form: FormGroup = this.fb.group({
    purchaseOrderNumber: ['', [Validators.required]],
    supplierId: ['', [Validators.required]],
    expectedDate: [''],
    priority: [1, [Validators.required]],
    
    // Preços e Margens (Passo 3 do documento)
    unitCost: [0, [Validators.required, Validators.min(0)]],
    taxPercentage: [18, [Validators.required, Validators.min(0), Validators.max(100)]],
    desiredMarginPercentage: [30, [Validators.required, Validators.min(0)]],
    
    // Hierarquia de Embalagem (Passo 4 do documento)
    expectedParcels: [0, [Validators.required, Validators.min(1)]],
    cartonsPerParcel: [0, [Validators.required, Validators.min(1)]],
    unitsPerCarton: [0, [Validators.required, Validators.min(1)]],
    
    // Logística (Passo 6 do documento)
    destinationWarehouseId: [''],
    vehicleId: [''],
    driverId: [''],
    dockDoorNumber: [''],
    shippingDistance: [''],
    shippingCost: [0, [Validators.min(0)]],
    
    // Importação (Passo 5 do documento)
    isInternational: [false],
    originCountry: [''],
    portOfEntry: [''],
    containerNumber: [''],
    incoterm: [''],
    billOfLading: [''],
    importLicenseNumber: [''],
    
    items: this.fb.array([])
  });

  get items(): FormArray {
    return this.form.get('items') as FormArray;
  }

  async ngOnInit(): Promise<void> {
    await Promise.all([
      this.loadCategories(),
      this.loadSuppliers(),
      this.loadWarehouses(),
      this.loadVehicles(),
      this.loadDrivers()
    ]);
  }

  async loadCategories(): Promise<void> {
    try {
      const data = await this.categoriesService.getActive();
      this.categories.set(data);
    } catch (error) {
      console.error('Erro ao carregar categorias:', error);
    }
  }

  async loadSuppliers(): Promise<void> {
    try {
      const data = await this.suppliersService.getAll();
      this.suppliers.set(data.suppliers || data);
    } catch (error) {
      console.error('Erro ao carregar fornecedores:', error);
    }
  }
  
  async loadWarehouses(): Promise<void> {
    try {
      const data = await this.warehousesService.getAll();
      this.warehouses.set(data);
    } catch (error) {
      console.error('Erro ao carregar warehouses:', error);
    }
  }
  
  async loadVehicles(): Promise<void> {
    try {
      const data = await this.vehiclesService.getAll();
      this.vehicles.set(data);
    } catch (error) {
      console.error('Erro ao carregar vehicles:', error);
    }
  }
  
  async loadDrivers(): Promise<void> {
    try {
      const data = await this.driversService.getAll();
      this.drivers.set(data);
    } catch (error) {
      console.error('Erro ao carregar drivers:', error);
    }
  }

  async onCategoryChange(categoryId: string, itemIndex: number): Promise<void> {
    this.selectedCategoryId.set(categoryId);
    if (categoryId) {
      try {
        const companyId = this.authService.currentUser()?.companyId || '';
        const response: any = await this.productsService.getAll(companyId);
        const allProducts = response.products || response || [];
        const filtered = allProducts.filter((p: any) => p.categoryId === categoryId);
        this.products.set(filtered);
        
        // Limpar seleção de produto quando categoria muda
        const item = this.items.at(itemIndex);
        item.patchValue({ productId: '', sku: '' });
      } catch (error) {
        console.error('Erro ao carregar produtos:', error);
      }
    } else {
      this.products.set([]);
    }
  }

  onProductChange(productId: string, itemIndex: number): void {
    const product = this.products().find(p => p.id === productId);
    if (product) {
      const item = this.items.at(itemIndex);
      item.patchValue({ 
        sku: product.sku,
        unitPrice: product.unitCost || 0
      });
    }
  }

  open(): void {
    this.isOpen.set(true);
    this.form.reset({
      priority: 1
    });
    this.items.clear();
    this.addItem();
  }

  close(): void {
    this.isOpen.set(false);
    this.form.reset();
    this.items.clear();
    this.products.set([]);
    this.selectedCategoryId.set('');
  }

  addItem(): void {
    const itemGroup = this.fb.group({
      categoryId: ['', [Validators.required]],
      productId: ['', [Validators.required]],
      sku: ['', [Validators.required]],
      quantityOrdered: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]]
    });
    this.items.push(itemGroup);
  }

  removeItem(index: number): void {
    this.items.removeAt(index);
    if (this.items.length === 0) {
      this.addItem();
    }
  }

  getTotalQuantity(): number {
    return this.items.controls.reduce((sum, control) => {
      return sum + (control.get('quantityOrdered')?.value || 0);
    }, 0);
  }

  getTotalValue(): number {
    return this.items.controls.reduce((sum, control) => {
      const qty = control.get('quantityOrdered')?.value || 0;
      const price = control.get('unitPrice')?.value || 0;
      return sum + (qty * price);
    }, 0);
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid || this.items.length === 0) {
      this.form.markAllAsTouched();
      this.items.controls.forEach(c => c.markAllAsTouched());
      alert('Preencha todos os campos obrigatórios e adicione pelo menos 1 item');
      return;
    }

    this.loading.set(true);
    try {
      const user = this.authService.currentUser();
      const companyId = user?.companyId;
      
      if (!companyId) {
        alert('Usuário sem empresa vinculada');
        return;
      }

      const formValue = this.form.value;
      const payload = {
        companyId,
        purchaseOrderNumber: formValue.purchaseOrderNumber,
        supplierId: formValue.supplierId,
        expectedDate: formValue.expectedDate ? new Date(formValue.expectedDate).toISOString() : undefined,
        priority: parseInt(formValue.priority),
        
        // Preços e Margens
        unitCost: parseFloat(formValue.unitCost),
        taxPercentage: parseFloat(formValue.taxPercentage),
        desiredMarginPercentage: parseFloat(formValue.desiredMarginPercentage),
        
        // Hierarquia de Embalagem
        expectedParcels: parseInt(formValue.expectedParcels),
        cartonsPerParcel: parseInt(formValue.cartonsPerParcel),
        unitsPerCarton: parseInt(formValue.unitsPerCarton),
        
        // Logística
        destinationWarehouseId: formValue.destinationWarehouseId || undefined,
        vehicleId: formValue.vehicleId || undefined,
        driverId: formValue.driverId || undefined,
        dockDoorNumber: formValue.dockDoorNumber || undefined,
        shippingDistance: formValue.shippingDistance || undefined,
        shippingCost: parseFloat(formValue.shippingCost || 0),
        
        // Importação
        isInternational: formValue.isInternational || false,
        originCountry: formValue.originCountry || undefined,
        portOfEntry: formValue.portOfEntry || undefined,
        containerNumber: formValue.containerNumber || undefined,
        incoterm: formValue.incoterm || undefined,
        billOfLading: formValue.billOfLading || undefined,
        importLicenseNumber: formValue.importLicenseNumber || undefined,
        
        items: formValue.items.map((item: any) => ({
          productId: item.productId,
          sku: item.sku,
          quantityOrdered: parseInt(item.quantityOrdered),
          unitPrice: parseFloat(item.unitPrice)
        }))
      };

      await this.purchaseOrdersService.create(payload);
      this.orderCreated.emit();
      this.close();
    } catch (error) {
      console.error('Erro ao criar pedido de compra:', error);
      alert('Erro ao criar pedido de compra');
    } finally {
      this.loading.set(false);
    }
  }

  getError(fieldName: string): string {
    const field = this.form.get(fieldName);
    if (field?.hasError('required')) return 'Campo obrigatório';
    if (field?.hasError('min')) return 'Valor mínimo não atingido';
    return '';
  }
}
