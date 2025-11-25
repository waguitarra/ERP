import { Component, signal, output, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductsService } from '@features/products/products.service';
import { Product } from '@core/models/product.model';

@Component({
  selector: 'app-product-selector-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-selector-modal.component.html',
  styles: [`
    .animate-spin { animation: spin 1s linear infinite; }
    @keyframes spin { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }
  `]
})
export class ProductSelectorModalComponent implements OnInit {
  private readonly productsService = inject(ProductsService);
  
  productSelected = output<Product>();
  
  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  searchTerm = signal<string>('');
  selectedProduct = signal<Product | null>(null);
  products = signal<Product[]>([]);
  
  get filteredProducts(): Product[] {
    const term = this.searchTerm().toLowerCase();
    if (!term) return this.products();
    
    return this.products().filter(p => 
      p.name.toLowerCase().includes(term) ||
      p.sku.toLowerCase().includes(term) ||
      (p.description && p.description.toLowerCase().includes(term))
    );
  }

  ngOnInit(): void {
    this.loadProducts();
  }

  async loadProducts(): Promise<void> {
    this.loading.set(true);
    try {
      const products = await this.productsService.getAll();
      this.products.set(products);
    } catch (error) {
      console.error('Erro ao carregar produtos:', error);
    } finally {
      this.loading.set(false);
    }
  }

  open(): void {
    this.isOpen.set(true);
    this.searchTerm.set('');
    this.selectedProduct.set(null);
    this.loadProducts();
  }

  close(): void {
    this.isOpen.set(false);
  }

  selectProduct(product: Product): void {
    this.selectedProduct.set(product);
  }

  confirm(): void {
    const selected = this.selectedProduct();
    if (selected) {
      this.productSelected.emit(selected);
      this.close();
    }
  }
}
