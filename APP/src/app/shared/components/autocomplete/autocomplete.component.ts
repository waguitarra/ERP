import { Component, input, output, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-autocomplete',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './autocomplete.component.html',
  styleUrls: ['./autocomplete.component.scss']
})
export class AutocompleteComponent {
  label = input<string>('');
  control = input.required<FormControl>();
  items = input<any[]>([]);
  displayField = input<string>('name');
  placeholder = input<string>('Buscar...');
  required = input<boolean>(false);
  
  selectionChange = output<any>();
  
  searchTerm = signal('');
  showResults = signal(false);
  
  filteredItems = computed(() => {
    const term = this.searchTerm().toLowerCase();
    if (!term) return this.items();
    return this.items().filter(item => 
      this.getDisplayValue(item).toLowerCase().includes(term)
    );
  });
  
  getDisplayValue(item: any): string {
    return item[this.displayField()] || '';
  }
  
  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchTerm.set(value);
    this.showResults.set(true);
  }
  
  onSelect(item: any): void {
    this.selectionChange.emit(item);
    this.searchTerm.set(this.getDisplayValue(item));
    this.showResults.set(false);
    this.control().setValue(item);
  }
  
  onBlur(): void {
    setTimeout(() => this.showResults.set(false), 200);
  }
}
