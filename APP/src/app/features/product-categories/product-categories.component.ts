import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductCategoriesService, ProductCategory, CreateProductCategoryRequest } from '../../core/services/product-categories.service';
import { I18nService } from '../../core/services/i18n.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-product-categories',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-categories.component.html',
  styleUrl: './product-categories.component.scss'
})
export class ProductCategoriesComponent implements OnInit {
  private readonly categoriesService = inject(ProductCategoriesService);
  private readonly router = inject(Router);
  protected readonly i18n = inject(I18nService);
  private readonly notification = inject(NotificationService);

  loading = signal<boolean>(false);
  categories = signal<ProductCategory[]>([]);
  filteredCategories = computed(() => {
    const search = this.searchTerm().toLowerCase();
    if (!search) return this.categories();
    return this.categories().filter(c => 
      c.name.toLowerCase().includes(search) ||
      c.code.toLowerCase().includes(search) ||
      (c.description && c.description.toLowerCase().includes(search))
    );
  });

  searchTerm = signal<string>('');
  showModal = signal<boolean>(false);
  editingCategory = signal<ProductCategory | null>(null);
  
  formData = signal<CreateProductCategoryRequest>({
    name: '',
    code: '',
    description: '',
    barcode: '',
    reference: '',
    isMaintenance: false,
    attributes: ''
  });

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
    this.editingCategory.set(null);
    this.formData.set({
      name: '',
      code: '',
      description: '',
      barcode: '',
      reference: '',
      isMaintenance: false,
      attributes: ''
    });
    this.showModal.set(true);
  }

  openEditModal(category: ProductCategory): void {
    this.editingCategory.set(category);
    this.formData.set({
      name: category.name,
      code: category.code,
      description: category.description,
      barcode: category.barcode,
      reference: category.reference,
      isMaintenance: category.isMaintenance,
      attributes: category.attributes
    });
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingCategory.set(null);
  }

  async saveCategory(): Promise<void> {
    if (!this.formData().name || !this.formData().code) {
      this.notification.error(this.i18n.t('product_categories.validation.required_fields'));
      return;
    }

    this.loading.set(true);
    try {
      const editing = this.editingCategory();
      if (editing) {
        await this.categoriesService.update(editing.id, this.formData());
        this.notification.success(this.i18n.t('product_categories.updated_success'));
      } else {
        await this.categoriesService.create(this.formData());
        this.notification.success(this.i18n.t('product_categories.created_success'));
      }
      await this.loadCategories();
      this.closeModal();
    } catch (error: any) {
      this.notification.error(error.message || this.i18n.t('errors.save_failed'));
    } finally {
      this.loading.set(false);
    }
  }

  async toggleActive(category: ProductCategory): Promise<void> {
    this.loading.set(true);
    try {
      if (category.isActive) {
        await this.categoriesService.deactivate(category.id);
        this.notification.success(this.i18n.t('product_categories.deactivated_success'));
      } else {
        await this.categoriesService.activate(category.id);
        this.notification.success(this.i18n.t('product_categories.activated_success'));
      }
      await this.loadCategories();
    } catch (error: any) {
      this.notification.error(error.message || this.i18n.t('errors.operation_failed'));
    } finally {
      this.loading.set(false);
    }
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

  updateFormField(field: keyof CreateProductCategoryRequest, value: any): void {
    this.formData.update(data => ({ ...data, [field]: value }));
  }
}
