import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductsService } from '../products.service';
import { Product } from '@core/models/product.model';
import { AuthService } from '@core/services/auth.service';
import { I18nService } from '@core/services/i18n.service';
import { ProductCreateModalComponent } from '../product-create-modal/product-create-modal.component';
import { ProductEditModalComponent } from '../product-edit-modal/product-edit-modal.component';

@Component({
  selector: 'app-products-list',
  standalone: true,
  imports: [CommonModule, RouterLink, ProductCreateModalComponent, ProductEditModalComponent],
  templateUrl: './products-list.component.html',
  styleUrls: ['./products-list.component.scss']
})
export class ProductsListComponent implements OnInit {
  private readonly productsService = inject(ProductsService);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);

  loading = signal<boolean>(false);
  error = signal<string | null>(null);
  products = signal<Product[]>([]);
  searchTerm = signal<string>('');
  hasData = computed(() => this.products().length > 0);
  
  showCreateModal = signal<boolean>(false);
  showEditModal = signal<boolean>(false);
  selectedProduct = signal<Product | null>(null);

  ngOnInit(): void {
    this.loadProducts();
  }

  async loadProducts(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      const user = this.authService.currentUser();
      const companyId = user?.companyId ?? undefined;
      const response = await this.productsService.getAll(companyId);
      this.products.set(response.data || []);
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao carregar produtos');
    } finally {
      this.loading.set(false);
    }
  }

  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchTerm.set(value);
    this.loadProducts();
  }

  openCreateModal(): void {
    this.showCreateModal.set(true);
  }
  
  closeCreateModal(): void {
    this.showCreateModal.set(false);
  }
  
  onProductCreated(): void {
    this.loadProducts();
  }
  
  openEditModal(product: Product): void {
    this.selectedProduct.set(product);
    this.showEditModal.set(true);
  }
  
  closeEditModal(): void {
    this.showEditModal.set(false);
    this.selectedProduct.set(null);
  }
  
  onProductUpdated(): void {
    this.loadProducts();
  }
  
  async deleteProduct(product: Product): Promise<void> {
    if (!confirm(`Deseja realmente excluir o produto "${product.name}"?`)) {
      return;
    }
    
    try {
      await this.productsService.delete(product.id);
      await this.loadProducts();
    } catch (error) {
      console.error('Erro ao excluir produto:', error);
      alert('Erro ao excluir produto');
    }
  }
}
