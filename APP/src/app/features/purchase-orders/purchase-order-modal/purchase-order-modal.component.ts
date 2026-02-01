import { Component, signal, computed, inject, output, input, viewChild, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray } from '@angular/forms';
import { PurchaseOrdersService, PurchaseOrder } from '@core/services/purchase-orders.service';
import { I18nService } from '@core/services/i18n.service';
import { NotificationService } from '@core/services/notification.service';
import { SupplierSelectorModalComponent } from '@shared/components/supplier-selector-modal/supplier-selector-modal.component';
import { VehicleSelectorModalComponent } from '@shared/components/vehicle-selector-modal/vehicle-selector-modal.component';
import { DriverSelectorModalComponent } from '@shared/components/driver-selector-modal/driver-selector-modal.component';
import { WarehouseSelectorModalComponent } from '@shared/components/warehouse-selector-modal/warehouse-selector-modal.component';
import { PurchaseOrderPriority } from '@core/models/enums';
import { AuthService } from '@core/services/auth.service';
import { ProductCategoriesService, ProductCategory } from '@core/services/product-categories.service';
import { ProductsService } from '../../products/products.service';
import { SuppliersService } from '../../suppliers/suppliers.service';
import { WarehousesService } from '@core/services/warehouses.service';
import { VehiclesService } from '@core/services/vehicles.service';
import { DriversService } from '@core/services/drivers.service';
import { CategorySelectorModalComponent } from '@shared/components/category-selector-modal/category-selector-modal.component';

@Component({
  selector: 'app-purchase-order-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, SupplierSelectorModalComponent, VehicleSelectorModalComponent, DriverSelectorModalComponent, WarehouseSelectorModalComponent, CategorySelectorModalComponent],
  templateUrl: './purchase-order-modal.component.html',
  styleUrls: ['./purchase-order-modal.component.scss']
})
export class PurchaseOrderModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly purchaseOrdersService = inject(PurchaseOrdersService);
  private readonly notification = inject(NotificationService);
  protected readonly i18n = inject(I18nService);
  private readonly authService = inject(AuthService);
  private readonly categoriesService = inject(ProductCategoriesService);
  private readonly productsService = inject(ProductsService);
  private readonly suppliersService = inject(SuppliersService);
  private readonly warehousesService = inject(WarehousesService);
  private readonly vehiclesService = inject(VehiclesService);
  private readonly driversService = inject(DriversService);
  
  supplierModal = viewChild<SupplierSelectorModalComponent>('supplierModal');
  vehicleModal = viewChild<VehicleSelectorModalComponent>('vehicleModal');
  driverModal = viewChild<DriverSelectorModalComponent>('driverModal');
  warehouseModal = viewChild<WarehouseSelectorModalComponent>('warehouseModal');
  categoryModal = viewChild<CategorySelectorModalComponent>('categoryModal');

  order = input<PurchaseOrder | null>();
  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  isCreateMode = signal<boolean>(false);
  
  selectedSupplier = signal<any | null>(null);
  selectedWarehouse = signal<any | null>(null);
  selectedVehicle = signal<any | null>(null);
  selectedDriver = signal<any | null>(null);
  selectedCategory = signal<ProductCategory | null>(null);
  
  categories = signal<ProductCategory[]>([]);
  suppliers = signal<any[]>([]);
  products = signal<any[]>([]);
  warehouses = signal<any[]>([]);
  vehicles = signal<any[]>([]);
  drivers = signal<any[]>([]);
  selectedCategoryId = signal<string>('');
  
  orderUpdated = output<void>();
  
  // Cálculos automáticos
  suggestedPrice = computed(() => {
    const unitCost = this.form.get('unitCost')?.value || 0;
    const taxRate = this.form.get('taxPercentage')?.value || 0;
    const margin = this.form.get('desiredMarginPercentage')?.value || 0;
    const costWithTax = unitCost + (unitCost * (taxRate / 100));
    return costWithTax + (costWithTax * (margin / 100));
  });
  
  calculatedUnits = computed(() => {
    const parcels = this.form.get('expectedParcels')?.value || 0;
    const cartonsPerParcel = this.form.get('cartonsPerParcel')?.value || 0;
    const unitsPerCarton = this.form.get('unitsPerCarton')?.value || 0;
    return parcels * cartonsPerParcel * unitsPerCarton;
  });

  form: FormGroup = this.fb.group({
    purchaseOrderNumber: ['', [Validators.required]],
    supplierId: ['', [Validators.required]],
    categoryId: ['', [Validators.required]],
    expectedDate: [''],
    priority: [1, [Validators.required]],
    
    // Preços e Margens
    unitCost: [0, [Validators.min(0)]],
    taxPercentage: [18, [Validators.min(0), Validators.max(100)]],
    desiredMarginPercentage: [30, [Validators.min(0)]],
    
    // Hierarquia de Embalagem
    expectedParcels: [0, [Validators.min(0)]],
    cartonsPerParcel: [0, [Validators.min(0)]],
    unitsPerCarton: [0, [Validators.min(0)]],
    
    // Logística
    destinationWarehouseId: [''],
    vehicleId: [''],
    driverId: [''],
    dockDoorNumber: [''],
    shippingDistance: [''],
    shippingCost: [0, [Validators.min(0)]],
    
    // Importação
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

  get itemControls() {
    return (this.form.get('items') as FormArray).controls;
  }
  
  get priorityOptions() {
    return [
      { value: PurchaseOrderPriority.Low, label: this.i18n.t('purchase_orders.priority.low') },
      { value: PurchaseOrderPriority.Normal, label: this.i18n.t('purchase_orders.priority.normal') },
      { value: PurchaseOrderPriority.High, label: this.i18n.t('purchase_orders.priority.high') },
      { value: PurchaseOrderPriority.Urgent, label: this.i18n.t('purchase_orders.priority.urgent') }
    ];
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

  async onCategoryChange(event: Event, itemIndex: number): Promise<void> {
    const categoryId = (event.target as HTMLSelectElement).value;
    this.selectedCategoryId.set(categoryId);
    if (categoryId) {
      try {
        const companyId = this.authService.currentUser()?.companyId || '';
        const response: any = await this.productsService.getAll(companyId);
        const allProducts = response.products || response || [];
        const filtered = allProducts.filter((p: any) => p.categoryId === categoryId);
        this.products.set(filtered);
        
        // Limpar seleção de produto quando categoria muda
        const item = (this.form.get('items') as FormArray).at(itemIndex);
        item.patchValue({ productId: '', sku: '' });
      } catch (error) {
        console.error('Erro ao carregar produtos:', error);
      }
    } else {
      this.products.set([]);
    }
  }

  onProductChange(event: Event, itemIndex: number): void {
    const productId = (event.target as HTMLSelectElement).value;
    const product = this.products().find(p => p.id === productId);
    if (product) {
      const item = (this.form.get('items') as FormArray).at(itemIndex);
      item.patchValue({ 
        sku: product.sku,
        unitPrice: product.unitCost || 0
      });
    }
  }

  open(): void {
    this.isCreateMode.set(!this.order());
    const ord = this.order();
    
    if (ord) {
      // Carregar dados pré-existentes nos signals
      if (ord.supplierId) {
        this.selectedSupplier.set({ id: ord.supplierId, name: ord.supplierName || '' });
      }
      
      this.form.patchValue({
        supplierId: ord.supplierId || '',
        expectedDate: ord.expectedDate ? new Date(ord.expectedDate).toISOString().split('T')[0] : '',
        priority: ord.priority || 1,
        unitCost: ord.unitCost || 0,
        taxPercentage: ord.taxPercentage || 18,
        desiredMarginPercentage: ord.desiredMarginPercentage || 30,
        expectedParcels: ord.expectedParcels || 0,
        cartonsPerParcel: ord.cartonsPerParcel || 0,
        unitsPerCarton: ord.unitsPerCarton || 0,
        destinationWarehouseId: ord.destinationWarehouseId || '',
        vehicleId: ord.vehicleId || '',
        driverId: ord.driverId || '',
        dockDoorNumber: ord.dockDoorNumber || '',
        shippingDistance: ord.shippingDistance || '',
        shippingCost: ord.shippingCost || 0,
        isInternational: ord.isInternational || false,
        originCountry: ord.originCountry || '',
        portOfEntry: ord.portOfEntry || '',
        containerNumber: ord.containerNumber || '',
        incoterm: ord.incoterm || ''
      });
    } else {
      this.form.reset();
      this.selectedSupplier.set(null);
      this.selectedCategory.set(null);
      this.selectedWarehouse.set(null);
      this.selectedVehicle.set(null);
      this.selectedDriver.set(null);
      (this.form.get('items') as FormArray).clear();
      this.addItem();
    }
    this.isOpen.set(true);
  }

  close(): void {
    this.isOpen.set(false);
    this.form.reset();
  }

  openSupplierSelector(): void {
    this.supplierModal()?.open();
  }

  openCategorySelector(): void {
    this.categoryModal()?.open();
  }

  openWarehouseSelector(): void {
    this.warehouseModal()?.open();
  }

  openVehicleSelector(): void {
    this.vehicleModal()?.open();
  }

  openDriverSelector(): void {
    this.driverModal()?.open();
  }

  onSupplierSelected(supplier: any): void {
    this.selectedSupplier.set(supplier);
    this.form.patchValue({ supplierId: supplier.id });
  }

  onCategorySelected(category: ProductCategory): void {
    this.selectedCategory.set(category);
    this.form.patchValue({ categoryId: category.id });
  }

  onWarehouseSelected(warehouse: any): void {
    this.selectedWarehouse.set(warehouse);
    this.form.patchValue({ destinationWarehouseId: warehouse.id });
  }

  onVehicleSelected(vehicle: any): void {
    this.selectedVehicle.set(vehicle);
    this.form.patchValue({ vehicleId: vehicle.id });
  }

  onDriverSelected(driver: any): void {
    this.selectedDriver.set(driver);
    this.form.patchValue({ driverId: driver.id });
  }

  addItem(): void {
    const itemGroup = this.fb.group({
      categoryId: ['', [Validators.required]],
      productId: ['', [Validators.required]],
      sku: ['', [Validators.required]],
      quantityOrdered: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]]
    });
    (this.form.get('items') as FormArray).push(itemGroup);
  }

  removeItem(index: number): void {
    (this.form.get('items') as FormArray).removeAt(index);
    if ((this.form.get('items') as FormArray).length === 0) {
      this.addItem();
    }
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid || (this.isCreateMode() && (this.form.get('items') as FormArray).length === 0)) {
      this.form.markAllAsTouched();
      if (this.isCreateMode()) (this.form.get('items') as FormArray).controls.forEach((c: any) => c.markAllAsTouched());
      return;
    }

    if (!confirm(this.i18n.t('common.messages.confirmSave'))) return;
    this.loading.set(true);
    try {
      const formValue = this.form.value;

      if (this.isCreateMode()) {
        // Create mode
        const user = this.authService.currentUser();
        const companyId = user?.companyId;
        
        if (!companyId) {
          this.notification.error(this.i18n.t('common.errors.companyIdNotFound'));
          return;
        }

        const payload = {
          companyId,
          purchaseOrderNumber: formValue.purchaseOrderNumber,
          supplierId: formValue.supplierId,
          categoryId: formValue.categoryId,
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
        this.notification.success(this.i18n.t('purchase_orders.created_success'));
      } else {
        // Edit mode
        const orderId = this.order()!.id;

        // Chamar endpoints separados do backend
        await this.purchaseOrdersService.setPurchaseDetails(orderId, {
          unitCost: parseFloat(formValue.unitCost) || 0,
          taxPercentage: parseFloat(formValue.taxPercentage) || 0,
          desiredMarginPercentage: parseFloat(formValue.desiredMarginPercentage) || 0
        });

        await this.purchaseOrdersService.setPackagingHierarchy(orderId, {
          expectedParcels: parseInt(formValue.expectedParcels) || 0,
          cartonsPerParcel: parseInt(formValue.cartonsPerParcel) || 0,
          unitsPerCarton: parseInt(formValue.unitsPerCarton) || 0
        });

        if (formValue.isInternational) {
          await this.purchaseOrdersService.setInternational(orderId, {
            originCountry: formValue.originCountry || '',
            portOfEntry: formValue.portOfEntry || '',
            containerNumber: formValue.containerNumber || '',
            incoterm: formValue.incoterm || ''
          });
        }

        await this.purchaseOrdersService.setLogistics(orderId, {
          destinationWarehouseId: formValue.destinationWarehouseId || undefined,
          vehicleId: formValue.vehicleId || undefined,
          driverId: formValue.driverId || undefined,
          dockDoorNumber: formValue.dockDoorNumber || undefined,
          shippingDistance: formValue.shippingDistance || undefined,
          shippingCost: parseFloat(formValue.shippingCost || 0)
        });

        this.notification.success(this.i18n.t('purchase_orders.updated_success'));
      }

      this.orderUpdated.emit();
      this.close();
    } catch (error: any) {
      console.error('Erro ao salvar purchase order:', error);
      this.notification.error(error.message || this.i18n.t('errors.save_failed'));
    } finally {
      this.loading.set(false);
    }
  }
}