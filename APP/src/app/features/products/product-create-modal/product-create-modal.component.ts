import { Component, output, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ModalComponent } from '@shared/components/modal/modal.component';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { ProductsService } from '../products.service';
import { AuthService } from '@core/services/auth.service';
import { CreateProductDto } from '@core/models/product.model';
import { I18nService } from '@core/services/i18n.service';
import { ProductCategoriesService, ProductCategory } from '@core/services/product-categories.service';

@Component({
  selector: 'app-product-create-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ModalComponent, FormFieldComponent],
  templateUrl: './product-create-modal.component.html',
  styleUrls: ['./product-create-modal.component.scss']
})
export class ProductCreateModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productsService = inject(ProductsService);
  private readonly authService = inject(AuthService);
  private readonly categoriesService = inject(ProductCategoriesService);
  protected readonly i18n = inject(I18nService);
  
  closeModal = output<void>();
  productCreated = output<void>();
  
  isOpen = true;
  isSaving = false;
  categories: ProductCategory[] = [];
  
  form: FormGroup = this.fb.group({
    categoryId: ['', Validators.required],
    name: ['', [Validators.required, Validators.minLength(3)]],
    sku: ['', [Validators.required]],
    barcode: [''],
    description: [''],
    weight: [0, [Validators.required, Validators.min(0)]],
    weightUnit: ['kg'],
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
    abcClassification: ['B']
  });
  
  async ngOnInit(): Promise<void> {
    await this.loadCategories();
  }
  
  async loadCategories(): Promise<void> {
    try {
      this.categories = await this.categoriesService.getActive();
    } catch (error) {
      console.error('Erro ao carregar categorias:', error);
    }
  }
  
  onClose(): void {
    this.closeModal.emit();
  }
  
  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      Object.keys(this.form.controls).forEach(key => {
        this.form.get(key)?.markAsTouched();
      });
      return;
    }
    
    if (!confirm(this.i18n.t('common.messages.confirmSave'))) return;
    this.isSaving = true;
    try {
      const user = this.authService.currentUser();
      const companyId = user?.companyId;
      
      if (!companyId) {
        alert(this.i18n.t('common.errors.companyIdNotFound'));
        return;
      }
      
      const data: CreateProductDto = {
        companyId,
        ...this.form.value
      };
      
      await this.productsService.create(data);
      this.productCreated.emit();
      this.onClose();
    } catch (error) {
      console.error('Erro ao criar produto:', error);
      alert(this.i18n.t('common.errors.createProduct'));
    } finally {
      this.isSaving = false;
    }
  }
}
