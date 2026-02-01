import { Component, inject, input, output, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { I18nService } from '@core/services/i18n.service';
import { NotificationService } from '@core/services/notification.service';
import { ProductCategoriesService, ProductCategory, CreateProductCategoryRequest } from '@core/services/product-categories.service';

@Component({
  selector: 'app-product-category-edit-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-category-edit-modal.component.html'
})
export class ProductCategoryEditModalComponent {
  readonly i18n = inject(I18nService);
  private readonly categoriesService = inject(ProductCategoriesService);
  private readonly notification = inject(NotificationService);

  category = input.required<ProductCategory>();
  close = output<void>();
  updated = output<void>();

  loading = signal(false);
  formData = signal<CreateProductCategoryRequest>({
    name: '',
    code: '',
    description: '',
    barcode: '',
    reference: '',
    isMaintenance: false
  });

  constructor() {
    effect(
      () => {
        const cat = this.category();
        this.formData.set({
          name: cat.name,
          code: cat.code,
          description: cat.description || '',
          barcode: cat.barcode || '',
          reference: cat.reference || '',
          isMaintenance: cat.isMaintenance
        });
      },
      { allowSignalWrites: true }
    );
  }

  updateField(field: keyof CreateProductCategoryRequest, value: any): void {
    this.formData.update(data => ({ ...data, [field]: value }));
  }

  async save(): Promise<void> {
    const data = this.formData();
    if (!data.name || !data.code) {
      this.notification.error(this.i18n.t('common.messages.fillRequired'));
      return;
    }

    if (!confirm(this.i18n.t('common.messages.confirmSave'))) return;
    this.loading.set(true);
    try {
      await this.categoriesService.update(this.category().id, data);
      this.notification.success(this.i18n.t('product_categories.updated_success'));
      this.updated.emit();
      this.close.emit();
    } catch (error: any) {
      this.notification.error(error.message || this.i18n.t('errors.save_failed'));
    } finally {
      this.loading.set(false);
    }
  }

  closeModal(): void {
    this.close.emit();
  }
}
