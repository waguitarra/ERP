import { Component, signal, inject, output, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CompaniesService } from '../companies.service';
import { Company } from '@core/models/company.model';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-company-edit-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './company-edit-modal.component.html',
  styleUrls: ['./company-edit-modal.component.scss']
})
export class CompanyEditModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly companiesService = inject(CompaniesService);
  protected readonly i18n = inject(I18nService);

  company = input.required<Company>();
  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  
  companyUpdated = output<void>();
  
  form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    tradeName: [''],
    document: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required]],
    address: ['', [Validators.required]],
    city: ['', [Validators.required]],
    state: ['', [Validators.required]],
    zipCode: ['', [Validators.required]],
    country: [''],
    isActive: [true]
  });

  open(): void {
    const comp = this.company();
    this.form.patchValue({
      name: comp.name,
      tradeName: comp.tradeName,
      document: comp.document,
      email: comp.email,
      phone: comp.phone,
      address: comp.address,
      city: comp.city,
      state: comp.state,
      zipCode: comp.zipCode,
      country: comp.country,
      isActive: comp.isActive
    });
    this.isOpen.set(true);
  }

  close(): void {
    this.isOpen.set(false);
    this.form.reset();
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    try {
      await this.companiesService.update(this.company().id, this.form.value);
      this.companyUpdated.emit();
      this.close();
    } catch (error) {
      console.error('Erro ao atualizar empresa:', error);
      alert('Erro ao atualizar empresa');
    } finally {
      this.loading.set(false);
    }
  }

  getError(fieldName: string): string {
    const field = this.form.get(fieldName);
    if (field?.hasError('required')) return 'Campo obrigatório';
    if (field?.hasError('email')) return 'Email inválido';
    if (field?.hasError('minlength')) return 'Mínimo de 3 caracteres';
    return '';
  }
}
