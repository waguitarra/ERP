import { Component, input, output, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ModalComponent } from '@shared/components/modal/modal.component';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { ProductsService } from '../products.service';
import { Product, UpdateProductDto } from '@core/models/product.model';

@Component({
  selector: 'app-product-edit-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ModalComponent, FormFieldComponent],
  templateUrl: './product-edit-modal.component.html',
  styleUrls: ['./product-edit-modal.component.scss']
})
export class ProductEditModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productsService = inject(ProductsService);
  
  product = input.required<Product>();
  
  closeModal = output<void>();
  productUpdated = output<void>();
  
  isOpen = true;
  isSaving = false;
  
  form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    sku: ['', [Validators.required]],
    barcode: [''],
    description: [''],
    weight: [0, [Validators.required, Validators.min(0)]],
    weightUnit: ['kg'],
    volume: [0],
    volumeUnit: ['m3'],
    length: [0],
    width: [0],
    height: [0],
    dimensionUnit: ['cm'],
    requiresLotTracking: [false],
    requiresSerialTracking: [false],
    isPerishable: [false],
    shelfLifeDays: [null],
    minimumStock: [0],
    safetyStock: [0],
    abcClassification: ['B'],
    isActive: [true]
  });
  
  ngOnInit(): void {
    const p = this.product();
    this.form.patchValue({
      name: p.name,
      sku: p.sku,
      barcode: p.barcode || '',
      description: p.description || '',
      weight: p.weight,
      weightUnit: p.weightUnit || 'kg',
      volume: p.volume,
      volumeUnit: p.volumeUnit || 'm3',
      length: p.length,
      width: p.width,
      height: p.height,
      dimensionUnit: p.dimensionUnit || 'cm',
      requiresLotTracking: p.requiresLotTracking,
      requiresSerialTracking: p.requiresSerialTracking,
      isPerishable: p.isPerishable,
      shelfLifeDays: p.shelfLifeDays,
      minimumStock: p.minimumStock || 0,
      safetyStock: p.safetyStock || 0,
      abcClassification: p.abcClassification || 'B',
      isActive: p.isActive
    });
  }
  
  onClose(): void {
    this.closeModal.emit();
  }
  
  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      Object.keys(this.form.controls).forEach(key => {
        this.form.get(key)?.markAsTouched();
      });
      return;
    }
    
    this.isSaving = true;
    try {
      const data: UpdateProductDto = this.form.value;
      await this.productsService.update(this.product().id, data);
      this.productUpdated.emit();
      this.onClose();
    } catch (error) {
      console.error('Erro ao atualizar produto:', error);
      alert('Erro ao atualizar produto');
    } finally {
      this.isSaving = false;
    }
  }
}
