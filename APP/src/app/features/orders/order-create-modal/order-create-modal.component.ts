import { Component, signal, inject, output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { OrdersService } from '../orders.service';
import { ProductCategoriesService, ProductCategory } from '@core/services/product-categories.service';
import { ProductsService } from '../../products/products.service';
import { AuthService } from '@core/services/auth.service';
import { OrderType, OrderSource, OrderPriority } from '@core/models/enums';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-order-create-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './order-create-modal.component.html',
  styleUrls: ['./order-create-modal.component.scss']
})
export class OrderCreateModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly ordersService = inject(OrdersService);
  private readonly categoriesService = inject(ProductCategoriesService);
  private readonly productsService = inject(ProductsService);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  
  orderCreated = output<void>();
  
  categories = signal<ProductCategory[]>([]);
  products = signal<any[]>([]);
  selectedCategoryId = signal<string>('');
  
  // Enums para template
  OrderType = OrderType;
  OrderSource = OrderSource;
  OrderPriority = OrderPriority;

  form: FormGroup = this.fb.group({
    orderNumber: ['', [Validators.required]],
    type: [OrderType.Outbound, [Validators.required]],
    source: [OrderSource.Manual, [Validators.required]],
    customerId: [''],
    supplierId: [''],
    expectedDate: [''],
    priority: [OrderPriority.Normal, [Validators.required]],
    shippingAddress: [''],
    specialInstructions: [''],
    isBOPIS: [false],
    items: this.fb.array([])
  });

  get items(): FormArray {
    return this.form.get('items') as FormArray;
  }

  async ngOnInit(): Promise<void> {
    await this.loadCategories();
  }

  async loadCategories(): Promise<void> {
    try {
      const data = await this.categoriesService.getActive();
      this.categories.set(data);
    } catch (error) {
      console.error('Erro ao carregar categorias:', error);
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
      type: OrderType.Outbound,
      source: OrderSource.Manual,
      priority: OrderPriority.Normal,
      isBOPIS: false
    });
    this.items.clear();
    this.addItem();
  }

  close(): void {
    this.isOpen.set(false);
    this.form.reset();
    this.items.clear();
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
      alert(this.i18n.t('orders.validation.requiredFieldsAndItems'));
      return;
    }

    this.loading.set(true);
    try {
      const user = this.authService.currentUser();
      const companyId = user?.companyId;
      
      if (!companyId) {
        alert(this.i18n.t('common.errors.companyIdNotFound'));
        return;
      }

      const formValue = this.form.value;
      const payload = {
        companyId,
        orderNumber: formValue.orderNumber,
        type: parseInt(formValue.type),
        source: parseInt(formValue.source),
        customerId: formValue.customerId || undefined,
        supplierId: formValue.supplierId || undefined,
        orderDate: new Date().toISOString(),
        expectedDate: formValue.expectedDate ? new Date(formValue.expectedDate).toISOString() : undefined,
        priority: parseInt(formValue.priority),
        shippingAddress: formValue.shippingAddress || undefined,
        specialInstructions: formValue.specialInstructions || undefined,
        isBOPIS: formValue.isBOPIS || false,
        items: formValue.items.map((item: any) => ({
          productId: item.productId,
          sku: item.sku,
          quantityOrdered: parseInt(item.quantityOrdered),
          unitPrice: parseFloat(item.unitPrice)
        }))
      };

      await this.ordersService.create(payload);
      this.orderCreated.emit();
      this.close();
    } catch (error) {
      console.error('Erro ao criar pedido:', error);
      alert(this.i18n.t('common.errors.createOrder'));
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
