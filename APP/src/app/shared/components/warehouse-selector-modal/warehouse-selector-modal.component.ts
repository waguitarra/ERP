import { Component, signal, output, input, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WarehousesService, Warehouse } from '@core/services/warehouses.service';

@Component({
  selector: 'app-warehouse-selector-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './warehouse-selector-modal.component.html',
  styles: [`
    .animate-spin { animation: spin 1s linear infinite; }
    @keyframes spin { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }
  `]
})
export class WarehouseSelectorModalComponent implements OnInit {
  private readonly warehousesService = inject(WarehousesService);
  
  title = input<string>('Selecionar Armazém');
  warehouseSelected = output<Warehouse>();
  
  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  searchTerm = signal<string>('');
  selectedWarehouse = signal<Warehouse | null>(null);
  warehouses = signal<Warehouse[]>([]);
  
  get filteredWarehouses(): Warehouse[] {
    const term = this.searchTerm().toLowerCase();
    if (!term) return this.warehouses();
    
    return this.warehouses().filter(wh => 
      wh.code.toLowerCase().includes(term) ||
      wh.name.toLowerCase().includes(term) ||
      (wh.city && wh.city.toLowerCase().includes(term)) ||
      (wh.state && wh.state.toLowerCase().includes(term))
    );
  }

  ngOnInit(): void {
    this.loadWarehouses();
  }

  async loadWarehouses(): Promise<void> {
    this.loading.set(true);
    try {
      const warehouses = await this.warehousesService.getAll();
      this.warehouses.set(warehouses);
    } catch (error) {
      console.error('Erro ao carregar armazéns:', error);
    } finally {
      this.loading.set(false);
    }
  }

  open(): void {
    this.isOpen.set(true);
    this.searchTerm.set('');
    this.selectedWarehouse.set(null);
    this.loadWarehouses();
  }

  close(): void {
    this.isOpen.set(false);
  }

  selectWarehouse(warehouse: Warehouse): void {
    this.selectedWarehouse.set(warehouse);
  }

  confirm(): void {
    const selected = this.selectedWarehouse();
    if (selected) {
      this.warehouseSelected.emit(selected);
      this.close();
    }
  }
}
