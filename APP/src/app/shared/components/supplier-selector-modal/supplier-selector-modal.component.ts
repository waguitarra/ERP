import { Component, signal, output, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SuppliersService, Supplier } from '@core/services/suppliers.service';

@Component({
  selector: 'app-supplier-selector-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './supplier-selector-modal.component.html',
  styles: [`
    .animate-spin { animation: spin 1s linear infinite; }
    @keyframes spin { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }
  `]
})
export class SupplierSelectorModalComponent implements OnInit {
  private readonly suppliersService = inject(SuppliersService);
  
  supplierSelected = output<Supplier>();
  
  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  searchTerm = signal<string>('');
  selectedSupplier = signal<Supplier | null>(null);
  suppliers = signal<Supplier[]>([]);
  
  get filteredSuppliers(): Supplier[] {
    const term = this.searchTerm().toLowerCase();
    if (!term) return this.suppliers();
    
    return this.suppliers().filter(supplier => 
      supplier.name.toLowerCase().includes(term) ||
      (supplier.email && supplier.email.toLowerCase().includes(term)) ||
      (supplier.taxId && supplier.taxId.toLowerCase().includes(term)) ||
      (supplier.phone && supplier.phone.toLowerCase().includes(term))
    );
  }

  ngOnInit(): void {
    this.loadSuppliers();
  }

  async loadSuppliers(): Promise<void> {
    this.loading.set(true);
    try {
      const suppliers = await this.suppliersService.getAll();
      this.suppliers.set(suppliers);
    } catch (error) {
      console.error('Erro ao carregar fornecedores:', error);
    } finally {
      this.loading.set(false);
    }
  }

  open(): void {
    this.isOpen.set(true);
    this.searchTerm.set('');
    this.selectedSupplier.set(null);
    this.loadSuppliers();
  }

  close(): void {
    this.isOpen.set(false);
  }

  selectSupplier(supplier: Supplier): void {
    this.selectedSupplier.set(supplier);
  }

  confirm(): void {
    const selected = this.selectedSupplier();
    if (selected) {
      this.supplierSelected.emit(selected);
      this.close();
    }
  }
}
