import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { I18nService } from '@core/services/i18n.service';
import { NotificationService } from '@core/services/notification.service';
import { ProductCategoriesService, ProductCategory } from '@core/services/product-categories.service';
import { ProductCategoryCreateModalComponent } from './product-category-create-modal/product-category-create-modal.component';
import { ProductCategoryEditModalComponent } from './product-category-edit-modal/product-category-edit-modal.component';

@Component({
  selector: 'app-product-categories',
  standalone: true,
  imports: [CommonModule, ProductCategoryCreateModalComponent, ProductCategoryEditModalComponent],
  templateUrl: './product-categories.component.html',
  styleUrl: './product-categories.component.scss'
})
export class ProductCategoriesComponent implements OnInit {
  readonly i18n = inject(I18nService);
  private readonly categoriesService = inject(ProductCategoriesService);
  private readonly notification = inject(NotificationService);

  categories = signal<ProductCategory[]>([]);
  loading = signal(false);
  showCreateModal = signal(false);
  showEditModal = signal(false);
  selectedCategory = signal<ProductCategory | null>(null);

  hasData = () => this.categories().length > 0;

  async ngOnInit(): Promise<void> {
    await this.loadCategories();
  }

  async loadCategories(): Promise<void> {
    this.loading.set(true);
    try {
      const data = await this.categoriesService.getAll();
      this.categories.set(data);
    } catch (error: any) {
      this.notification.error(error.message || this.i18n.t('errors.load_failed'));
    } finally {
      this.loading.set(false);
    }
  }

  openCreateModal(): void {
    this.showCreateModal.set(true);
  }

  openEditModal(category: ProductCategory): void {
    this.selectedCategory.set(category);
    this.showEditModal.set(true);
  }

  closeCreateModal(): void {
    this.showCreateModal.set(false);
  }

  closeEditModal(): void {
    this.showEditModal.set(false);
    this.selectedCategory.set(null);
  }

  async onCreated(): Promise<void> {
    await this.loadCategories();
  }

  async onUpdated(): Promise<void> {
    await this.loadCategories();
  }

  async deleteCategory(category: ProductCategory): Promise<void> {
    if (!confirm(this.i18n.t('product_categories.confirm_delete'))) return;

    this.loading.set(true);
    try {
      await this.categoriesService.delete(category.id);
      this.notification.success(this.i18n.t('product_categories.deleted_success'));
      await this.loadCategories();
    } catch (error: any) {
      this.notification.error(error.message || this.i18n.t('errors.delete_failed'));
    } finally {
      this.loading.set(false);
    }
  }
}
