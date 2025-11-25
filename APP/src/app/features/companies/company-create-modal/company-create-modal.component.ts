import { Component, signal, inject, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CompaniesService } from '../companies.service';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-company-create-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './company-create-modal.component.html',
  styleUrls: ['./company-create-modal.component.scss']
})
export class CompanyCreateModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly companiesService = inject(CompaniesService);
  protected readonly i18n = inject(I18nService);

  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  
  companyCreated = output<void>();
  
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
    country: ['Brasil']
  });

  open(): void {
    this.isOpen.set(true);
    this.form.reset({ country: 'Brasil' });
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
      await this.companiesService.create(this.form.value);
      this.companyCreated.emit();
      this.close();
    } catch (error) {
      console.error('Erro ao criar empresa:', error);
      alert('Erro ao criar empresa');
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
