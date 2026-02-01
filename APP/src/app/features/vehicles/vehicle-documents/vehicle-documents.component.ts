import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { I18nService } from '@core/services/i18n.service';
import { VehicleManagementService } from '../vehicle-management.service';
import { VehicleDocument, VehicleDocumentType, CreateDocumentRequest } from '@core/models/vehicle-management.model';

@Component({
  selector: 'app-vehicle-documents',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vehicle-documents.component.html',
  styleUrls: ['./vehicle-documents.component.scss']
})
export class VehicleDocumentsComponent implements OnInit {
  @Input({ required: true }) vehicleId!: string;

  protected readonly i18n = inject(I18nService);
  private readonly service = inject(VehicleManagementService);

  documents = signal<VehicleDocument[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  showModal = signal(false);
  editingDocument = signal<VehicleDocument | null>(null);
  saving = signal(false);

  formData: CreateDocumentRequest = this.getEmptyForm();

  documentTypes = Object.entries(VehicleDocumentType)
    .filter(([key, value]) => typeof value === 'number')
    .map(([key, value]) => ({ value: value as number, label: key }));

  ngOnInit(): void {
    this.loadDocuments();
  }

  async loadDocuments(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      const response = await this.service.getDocuments(this.vehicleId);
      this.documents.set(response.data || []);
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao carregar documentos');
    } finally {
      this.loading.set(false);
    }
  }

  openCreateModal(): void {
    this.formData = this.getEmptyForm();
    this.editingDocument.set(null);
    this.showModal.set(true);
  }

  openEditModal(doc: VehicleDocument): void {
    this.formData = {
      type: doc.type,
      documentNumber: doc.documentNumber,
      description: doc.description,
      issueDate: doc.issueDate.split('T')[0],
      expiryDate: doc.expiryDate?.split('T')[0],
      issuingAuthority: doc.issuingAuthority,
      cost: doc.cost,
      alertOnExpiry: doc.alertOnExpiry,
      alertDaysBefore: doc.alertDaysBefore,
      notes: doc.notes
    };
    this.editingDocument.set(doc);
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingDocument.set(null);
  }

  async saveDocument(): Promise<void> {
    if (!confirm(this.i18n.t('common.messages.confirmSave'))) return;
    this.saving.set(true);

    try {
      const editing = this.editingDocument();
      if (editing) {
        await this.service.updateDocument(this.vehicleId, editing.id, this.formData);
      } else {
        await this.service.createDocument(this.vehicleId, this.formData);
      }
      this.closeModal();
      await this.loadDocuments();
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao salvar documento');
    } finally {
      this.saving.set(false);
    }
  }

  async deleteDocument(doc: VehicleDocument): Promise<void> {
    if (!confirm(this.i18n.t('vehicleDetail.documents.confirmDelete'))) return;

    try {
      await this.service.deleteDocument(this.vehicleId, doc.id);
      await this.loadDocuments();
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao excluir documento');
    }
  }

  getStatusClass(doc: VehicleDocument): string {
    if (doc.isExpired) return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300';
    if (doc.isExpiringSoon) return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300';
    return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300';
  }

  getStatusLabel(doc: VehicleDocument): string {
    if (doc.isExpired) return this.i18n.t('vehicleDetail.documents.expired');
    if (doc.isExpiringSoon) return this.i18n.t('vehicleDetail.documents.expiringSoon');
    return this.i18n.t('vehicleDetail.documents.valid');
  }

  formatDate(date: string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR');
  }

  formatCurrency(value: number | undefined): string {
    if (value === undefined || value === null) return '-';
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'EUR' }).format(value);
  }

  getDocumentTypeLabel(type: VehicleDocumentType): string {
    const typeKey = VehicleDocumentType[type]?.charAt(0).toLowerCase() + VehicleDocumentType[type]?.slice(1);
    return this.i18n.t(`vehicleDetail.documents.types.${typeKey}`) || VehicleDocumentType[type];
  }

  private getEmptyForm(): CreateDocumentRequest {
    return {
      type: VehicleDocumentType.RegistrationCertificate,
      documentNumber: '',
      description: '',
      issueDate: new Date().toISOString().split('T')[0],
      expiryDate: undefined,
      issuingAuthority: '',
      cost: undefined,
      alertOnExpiry: true,
      alertDaysBefore: 30,
      notes: ''
    };
  }
}
