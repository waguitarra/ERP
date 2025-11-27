import { Component, inject, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { I18nService } from '@core/services/i18n.service';
import { NotificationService } from '@core/services/notification.service';
import { ProductCategoriesService, CreateProductCategoryRequest } from '@core/services/product-categories.service';

@Component({
  selector: 'app-product-category-create-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-category-create-modal.component.html'
})
export class ProductCategoryCreateModalComponent {
  readonly i18n = inject(I18nService);
  private readonly categoriesService = inject(ProductCategoriesService);
  private readonly notification = inject(NotificationService);

  close = output<void>();
  created = output<void>();

  loading = signal(false);
  formData = signal<CreateProductCategoryRequest>({
    name: '',
    code: '',
    description: '',
    barcode: '',
    reference: '',
    isMaintenance: false
  });

  updateField(field: keyof CreateProductCategoryRequest, value: any): void {
    this.formData.update(data => ({ ...data, [field]: value }));
  }

  async save(): Promise<void> {
    const data = this.formData();
    if (!data.name || !data.code) {
      this.notification.error(this.i18n.t('common.messages.fillRequired'));
      return;
    }

    this.loading.set(true);
    try {
      await this.categoriesService.create(data);
      this.notification.success(this.i18n.t('product_categories.created_success'));
      this.created.emit();
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
