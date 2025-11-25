import { Component, output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ModalComponent } from '@shared/components/modal/modal.component';
import { FormFieldComponent } from '@shared/components/form-field/form-field.component';
import { ProductsService } from '../products.service';
import { AuthService } from '@core/services/auth.service';
import { CreateProductDto } from '@core/models/product.model';

@Component({
  selector: 'app-product-create-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ModalComponent, FormFieldComponent],
  templateUrl: './product-create-modal.component.html',
  styleUrls: ['./product-create-modal.component.scss']
})
export class ProductCreateModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly productsService = inject(ProductsService);
  private readonly authService = inject(AuthService);
  
  closeModal = output<void>();
  productCreated = output<void>();
  
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
    abcClassification: ['B']
  });
  
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
      const user = this.authService.currentUser();
      const companyId = user?.companyId;
      
      if (!companyId) {
        alert('Erro: CompanyId não encontrado');
        return;
      }
      
      const data: CreateProductDto = {
        companyId,
        ...this.form.value
      };
      
      await this.productsService.create(data);
      this.productCreated.emit();
      this.onClose();
    } catch (error) {
      console.error('Erro ao criar produto:', error);
      alert('Erro ao criar produto');
    } finally {
      this.isSaving = false;
    }
  }
}
