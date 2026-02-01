import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { I18nService } from '@core/services/i18n.service';
import { InventoryService } from '../inventory.service';

interface InventoryItem {
  id: string;
  productId: string;
  productName: string;
  warehouseId: string;
  warehouseName: string;
  storageLocationId: string;
  storageLocationCode: string;
  quantity: number;
  minimumStock: number;
  maximumStock: number;
  lastUpdated: string;
}

@Component({
  selector: 'app-inventory-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './inventory-list.component.html',
  styleUrls: ['./inventory-list.component.scss']
})
export class InventoryListComponent implements OnInit {
  protected readonly i18n = inject(I18nService);
  private readonly inventoryService = inject(InventoryService);

  loading = signal<boolean>(false);
  error = signal<string | null>(null);
  inventoryItems = signal<InventoryItem[]>([]);
  searchTerm = signal<string>('');
  warehouseFilter = signal<string>('all');
  stockFilter = signal<string>('all');

  filteredItems = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    const warehouse = this.warehouseFilter();
    const stock = this.stockFilter();
    let result = this.inventoryItems();

    if (term) {
      result = result.filter(item =>
        item.productName?.toLowerCase().includes(term) ||
        item.storageLocationCode?.toLowerCase().includes(term) ||
        item.warehouseName?.toLowerCase().includes(term)
      );
    }

    if (warehouse !== 'all') {
      result = result.filter(item => item.warehouseId === warehouse);
    }

    if (stock === 'low') {
      result = result.filter(item => item.quantity <= item.minimumStock && item.quantity > 0);
    } else if (stock === 'out') {
      result = result.filter(item => item.quantity === 0);
    } else if (stock === 'normal') {
      result = result.filter(item => item.quantity > item.minimumStock);
    }

    return result;
  });

  totalStock = computed(() => this.inventoryItems().reduce((sum, item) => sum + item.quantity, 0));
  lowStockCount = computed(() => this.inventoryItems().filter(item => item.quantity <= item.minimumStock && item.quantity > 0).length);
  outOfStockCount = computed(() => this.inventoryItems().filter(item => item.quantity === 0).length);
  totalItems = computed(() => this.inventoryItems().length);

  warehouses = computed(() => {
    const map = new Map<string, string>();
    this.inventoryItems().forEach(item => {
      if (item.warehouseId && item.warehouseName) {
        map.set(item.warehouseId, item.warehouseName);
      }
    });
    return Array.from(map.entries()).map(([id, name]) => ({ id, name }));
  });

  ngOnInit(): void {
    this.loadInventory();
  }

  async loadInventory(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      const response = await this.inventoryService.getAll();
      this.inventoryItems.set(response.data || response || []);
    } catch (err: any) {
      this.error.set(err.message || this.i18n.t('inventory.errors.loadFailed'));
    } finally {
      this.loading.set(false);
    }
  }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm.set(input.value);
  }

  onWarehouseChange(event: Event): void {
    this.warehouseFilter.set((event.target as HTMLSelectElement).value);
  }

  onStockChange(event: Event): void {
    this.stockFilter.set((event.target as HTMLSelectElement).value);
  }

  clearFilters(): void {
    this.searchTerm.set('');
    this.warehouseFilter.set('all');
    this.stockFilter.set('all');
  }

  getStockStatus(item: InventoryItem): 'normal' | 'low' | 'out' {
    if (item.quantity === 0) return 'out';
    if (item.minimumStock > 0 && item.quantity <= item.minimumStock) return 'low';
    return 'normal';
  }

  getStockStatusClass(status: 'normal' | 'low' | 'out'): string {
    switch (status) {
      case 'out': return 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400';
      case 'low': return 'bg-amber-100 text-amber-800 dark:bg-amber-900/30 dark:text-amber-400';
      default: return 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400';
    }
  }

  getStockStatusLabel(status: 'normal' | 'low' | 'out'): string {
    switch (status) {
      case 'out': return this.i18n.t('inventory.status.outOfStock');
      case 'low': return this.i18n.t('inventory.status.lowStock');
      default: return this.i18n.t('inventory.status.inStock');
    }
  }
}
