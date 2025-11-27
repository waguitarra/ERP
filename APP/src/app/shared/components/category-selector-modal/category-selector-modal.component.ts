import { Component, signal, output, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductCategoriesService, ProductCategory } from '@core/services/product-categories.service';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-category-selector-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './category-selector-modal.component.html',
  styleUrls: ['./category-selector-modal.component.scss']
})
export class CategorySelectorModalComponent implements OnInit {
  private readonly categoriesService = inject(ProductCategoriesService);
  protected readonly i18n = inject(I18nService);

  categorySelected = output<ProductCategory>();

  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  searchTerm = signal<string>('');
  selectedCategory = signal<ProductCategory | null>(null);
  categories = signal<ProductCategory[]>([]);

  get filteredCategories(): ProductCategory[] {
    const term = this.searchTerm().toLowerCase();
    if (!term) return this.categories();

    return this.categories().filter(category =>
      category.name.toLowerCase().includes(term) ||
      category.code.toLowerCase().includes(term) ||
      (category.barcode && category.barcode.toLowerCase().includes(term))
    );
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  async loadCategories(): Promise<void> {
    this.loading.set(true);
    try {
      const categories = await this.categoriesService.getActive();
      this.categories.set(categories);
    } catch (error) {
      console.error('Erro ao carregar categorias:', error);
    } finally {
      this.loading.set(false);
    }
  }

  open(): void {
    this.isOpen.set(true);
    this.searchTerm.set('');
    this.selectedCategory.set(null);
    this.loadCategories();
  }

  close(): void {
    this.isOpen.set(false);
  }

  selectCategory(category: ProductCategory): void {
    this.selectedCategory.set(category);
  }

  confirm(): void {
    const selected = this.selectedCategory();
    if (selected) {
      this.categorySelected.emit(selected);
      this.close();
    }
  }
}