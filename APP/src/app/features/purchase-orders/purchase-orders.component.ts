import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { PurchaseOrdersService, PurchaseOrder, CreatePurchaseOrderRequest, PurchaseOrderItem } from '../../core/services/purchase-orders.service';
import { ProductCategoriesService, ProductCategory } from '../../core/services/product-categories.service';
import { ProductsService } from '../products/products.service';
import { SuppliersService } from '../suppliers/suppliers.service';
import { WarehousesService } from '../../core/services/warehouses.service';
import { VehiclesService } from '../../core/services/vehicles.service';
import { DriversService } from '../../core/services/drivers.service';
import { AuthService } from '../../core/services/auth.service';
import { I18nService } from '../../core/services/i18n.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-purchase-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './purchase-orders.component.html',
  styleUrl: './purchase-orders.component.scss'
})
export class PurchaseOrdersComponent implements OnInit {
  private readonly purchaseOrdersService = inject(PurchaseOrdersService);
  private readonly categoriesService = inject(ProductCategoriesService);
  private readonly productsService = inject(ProductsService);
  private readonly suppliersService = inject(SuppliersService);
  private readonly warehousesService = inject(WarehousesService);
  private readonly vehiclesService = inject(VehiclesService);
  private readonly driversService = inject(DriversService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  protected readonly i18n = inject(I18nService);
  private readonly notification = inject(NotificationService);

  loading = signal<boolean>(false);
  purchaseOrders = signal<PurchaseOrder[]>([]);
  categories = signal<ProductCategory[]>([]);
  suppliers = signal<any[]>([]);
  warehouses = signal<any[]>([]);
  vehicles = signal<any[]>([]);
  drivers = signal<any[]>([]);
  products = signal<any[]>([]);

  filteredOrders = computed(() => {
    const search = this.searchTerm().toLowerCase();
    if (!search) return this.purchaseOrders();
    return this.purchaseOrders().filter(po => 
      po.purchaseOrderNumber.toLowerCase().includes(search) ||
      po.status.toLowerCase().includes(search)
    );
  });

  searchTerm = signal<string>('');
  showModal = signal<boolean>(false);
  editingOrder = signal<PurchaseOrder | null>(null);
  selectedCategoryId = signal<string>('');
  
  formData = signal<CreatePurchaseOrderRequest>({
    companyId: '',
    purchaseOrderNumber: '',
    supplierId: '',
    expectedDate: '',
    priority: 1,
    items: []
  });

  currentItem = signal<PurchaseOrderItem>({
    productId: '',
    sku: '',
    quantityOrdered: 0,
    unitPrice: 0
  });

  async ngOnInit(): Promise<void> {
    const companyId = this.authService.currentUser()?.companyId;
    if (companyId) {
      this.formData.update(data => ({ ...data, companyId }));
      await Promise.all([
        this.loadPurchaseOrders(companyId),
        this.loadCategories(),
        this.loadSuppliers(),
        this.loadWarehouses(),
        this.loadVehicles(),
        this.loadDrivers()
      ]);
    }
  }

  async loadPurchaseOrders(companyId: string): Promise<void> {
    this.loading.set(true);
    try {
      const data = await this.purchaseOrdersService.getAll(companyId);
      this.purchaseOrders.set(data);
    } catch (error: any) {
      this.notification.error(error.message || this.i18n.t('errors.load_failed'));
    } finally {
      this.loading.set(false);
    }
  }

  async loadCategories(): Promise<void> {
    try {
      const data = await this.categoriesService.getActive();
      this.categories.set(data);
    } catch (error: any) {
      this.notification.error(error.message || this.i18n.t('errors.load_failed'));
    }
  }

  async loadSuppliers(): Promise<void> {
    try {
      const data = await this.suppliersService.getAll();
      this.suppliers.set(data.suppliers || data);
    } catch (error) {}
  }

  async loadWarehouses(): Promise<void> {
    try {
      const data = await this.warehousesService.getAll();
      this.warehouses.set(data);
    } catch (error) {}
  }

  async loadVehicles(): Promise<void> {
    try {
      const data = await this.vehiclesService.getAll();
      this.vehicles.set(data);
    } catch (error) {}
  }

  async loadDrivers(): Promise<void> {
    try {
      const data = await this.driversService.getAll();
      this.drivers.set(data);
    } catch (error) {}
  }

  async onCategoryChange(categoryId: string): Promise<void> {
    this.selectedCategoryId.set(categoryId);
    if (categoryId) {
      try {
        const companyId = this.authService.currentUser()?.companyId || '';
        const response: any = await this.productsService.getAll(companyId);
        const allProducts = response.products || response || [];
        const filtered = allProducts.filter((p: any) => p.categoryId === categoryId);
        this.products.set(filtered);
      } catch (error: any) {
        this.notification.error(error.message || this.i18n.t('errors.load_failed'));
      }
    } else {
      this.products.set([]);
    }
  }

  openCreateModal(): void {
    const companyId = this.authService.currentUser()?.companyId;
    this.editingOrder.set(null);
    this.formData.set({
      companyId: companyId || '',
      purchaseOrderNumber: '',
      supplierId: '',
      expectedDate: '',
      priority: 1,
      items: []
    });
    this.selectedCategoryId.set('');
    this.products.set([]);
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingOrder.set(null);
  }

  addItem(): void {
    const item = this.currentItem();
    if (!item.productId || item.quantityOrdered <= 0 || item.unitPrice <= 0) {
      this.notification.error(this.i18n.t('purchase_orders.validation.required_fields'));
      return;
    }

    const product = this.products().find(p => p.id === item.productId);
    if (product) {
      this.formData.update(data => ({
        ...data,
        items: [...data.items, { ...item, sku: product.sku }]
      }));
      this.currentItem.set({ productId: '', sku: '', quantityOrdered: 0, unitPrice: 0 });
      this.selectedCategoryId.set('');
      this.products.set([]);
    }
  }

  removeItem(index: number): void {
    this.formData.update(data => ({
      ...data,
      items: data.items.filter((_, i) => i !== index)
    }));
  }

  async savePurchaseOrder(): Promise<void> {
    const data = this.formData();
    if (!data.supplierId || data.items.length === 0) {
      this.notification.error(this.i18n.t('purchase_orders.validation.required_fields'));
      return;
    }

    this.loading.set(true);
    try {
      await this.purchaseOrdersService.create(data);
      this.notification.success(this.i18n.t('purchase_orders.created_success'));
      const companyId = this.authService.currentUser()?.companyId;
      if (companyId) {
        await this.loadPurchaseOrders(companyId);
      }
      this.closeModal();
    } catch (error: any) {
      this.notification.error(error.message || this.i18n.t('errors.save_failed'));
    } finally {
      this.loading.set(false);
    }
  }

  async deletePurchaseOrder(order: PurchaseOrder): Promise<void> {
    if (!confirm(this.i18n.t('purchase_orders.confirm_delete'))) return;

    this.loading.set(true);
    try {
      await this.purchaseOrdersService.delete(order.id);
      this.notification.success(this.i18n.t('purchase_orders.deleted_success'));
      const companyId = this.authService.currentUser()?.companyId;
      if (companyId) {
        await this.loadPurchaseOrders(companyId);
      }
    } catch (error: any) {
      this.notification.error(error.message || this.i18n.t('errors.delete_failed'));
    } finally {
      this.loading.set(false);
    }
  }

  updateFormField(field: string, value: any): void {
    this.formData.update(data => ({ ...data, [field]: value }));
  }

  updateItemField(field: keyof PurchaseOrderItem, value: any): void {
    this.currentItem.update(item => ({ ...item, [field]: value }));
  }

  getTotalQuantity(): number {
    return this.formData().items.reduce((sum, item) => sum + item.quantityOrdered, 0);
  }

  getTotalValue(): number {
    return this.formData().items.reduce((sum, item) => sum + (item.quantityOrdered * item.unitPrice), 0);
  }
}
