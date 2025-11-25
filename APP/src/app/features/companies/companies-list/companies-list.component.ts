import { Component, signal, computed, inject, OnInit, viewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CompaniesService } from '../companies.service';
import { Company } from '@core/models/company.model';
import { I18nService } from '@core/services/i18n.service';
import { CompanyCreateModalComponent } from '../company-create-modal/company-create-modal.component';
import { CompanyEditModalComponent } from '../company-edit-modal/company-edit-modal.component';

@Component({
  selector: 'app-companies-list',
  standalone: true,
  imports: [CommonModule, CompanyCreateModalComponent, CompanyEditModalComponent],
  templateUrl: './companies-list.component.html',
  styleUrls: ['./companies-list.component.scss']
})
export class CompaniesListComponent implements OnInit {
  private readonly companiesService = inject(CompaniesService);
  protected readonly i18n = inject(I18nService);

  loading = signal<boolean>(true);
  companies = signal<Company[]>([]);
  selectedCompany = signal<Company | null>(null);
  hasData = computed(() => this.companies().length > 0);

  createModal = viewChild<CompanyCreateModalComponent>('createModal');
  editModal = viewChild<CompanyEditModalComponent>('editModal');

  ngOnInit(): void {
    this.loadCompanies();
  }

  async loadCompanies(): Promise<void> {
    this.loading.set(true);
    try {
      const response = await this.companiesService.getAll();
      this.companies.set(response.data || []);
    } catch (err) {
      console.error('Erro ao carregar empresas:', err);
    } finally {
      this.loading.set(false);
    }
  }

  openCreateModal(): void {
    this.createModal()?.open();
  }

  openEditModal(company: Company): void {
    this.selectedCompany.set(company);
    setTimeout(() => this.editModal()?.open(), 0);
  }

  async deleteCompany(company: Company): Promise<void> {
    if (!confirm(`Deseja realmente excluir a empresa "${company.name}"?`)) return;
    try {
      await this.companiesService.delete(company.id);
      await this.loadCompanies();
    } catch (error) {
      console.error('Erro ao excluir empresa:', error);
      alert('Erro ao excluir empresa');
    }
  }
}
