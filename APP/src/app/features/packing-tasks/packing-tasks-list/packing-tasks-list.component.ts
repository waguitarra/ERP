import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PackingTasksService } from '../packing-tasks.service';
import { PackingTask, PackingTaskStatus } from '@core/models/packing-task.model';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-packing-tasks-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './packing-tasks-list.component.html',
  styleUrls: ['./packing-tasks-list.component.scss']
})
export class PackingTasksListComponent implements OnInit {
  private readonly packingTasksService = inject(PackingTasksService);
  protected readonly i18n = inject(I18nService);

  loading = signal<boolean>(false);
  error = signal<string | null>(null);
  tasks = signal<PackingTask[]>([]);
  selectedFilter = signal<string>('all');
  searchTerm = signal<string>('');
  
  hasData = computed(() => this.filteredTasks().length > 0);
  
  filteredTasks = computed(() => {
    let result = this.tasks();
    const search = this.searchTerm().toLowerCase();
    
    if (search) {
      result = result.filter(t => 
        t.taskNumber.toLowerCase().includes(search) ||
        t.orderNumber?.toLowerCase().includes(search) ||
        t.assignedToName?.toLowerCase().includes(search)
      );
    }
    
    return result;
  });

  // Statistics
  stats = computed(() => {
    const all = this.tasks();
    return {
      total: all.length,
      pending: all.filter(t => t.status === PackingTaskStatus.Pending || t.status === PackingTaskStatus.Assigned).length,
      inProgress: all.filter(t => t.status === PackingTaskStatus.InProgress).length,
      completed: all.filter(t => t.status === PackingTaskStatus.Completed).length,
      cancelled: all.filter(t => t.status === PackingTaskStatus.Cancelled).length
    };
  });

  ngOnInit(): void {
    this.loadTasks();
  }

  async loadTasks(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);

    try {
      let response;
      switch (this.selectedFilter()) {
        case 'pending':
          response = await this.packingTasksService.getPending();
          break;
        case 'in-progress':
          response = await this.packingTasksService.getInProgress();
          break;
        case 'completed':
          response = await this.packingTasksService.getByStatus(PackingTaskStatus.Completed);
          break;
        default:
          response = await this.packingTasksService.getAll();
      }
      this.tasks.set(response.data || []);
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao carregar tarefas de empacotamento');
      console.error('Error loading packing tasks:', err);
    } finally {
      this.loading.set(false);
    }
  }

  onFilterChange(filter: string): void {
    this.selectedFilter.set(filter);
    this.loadTasks();
  }

  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchTerm.set(value);
  }

  async startTask(task: PackingTask): Promise<void> {
    try {
      await this.packingTasksService.start(task.id);
      await this.loadTasks();
    } catch (error) {
      console.error('Erro ao iniciar task:', error);
    }
  }

  async completeTask(task: PackingTask): Promise<void> {
    try {
      await this.packingTasksService.complete(task.id);
      await this.loadTasks();
    } catch (error) {
      console.error('Erro ao completar task:', error);
    }
  }

  async cancelTask(task: PackingTask): Promise<void> {
    if (!confirm('Deseja realmente cancelar esta tarefa?')) return;
    
    try {
      await this.packingTasksService.cancel(task.id);
      await this.loadTasks();
    } catch (error) {
      console.error('Erro ao cancelar task:', error);
    }
  }

  async deleteTask(task: PackingTask): Promise<void> {
    if (!confirm('Deseja realmente excluir esta tarefa? Esta ação não pode ser desfeita.')) return;
    
    try {
      await this.packingTasksService.delete(task.id);
      await this.loadTasks();
    } catch (error) {
      console.error('Erro ao excluir task:', error);
    }
  }

  getStatusClass(status: number): string {
    switch (status) {
      case PackingTaskStatus.Pending:
        return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400';
      case PackingTaskStatus.Assigned:
        return 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400';
      case PackingTaskStatus.InProgress:
        return 'bg-indigo-100 text-indigo-800 dark:bg-indigo-900/30 dark:text-indigo-400';
      case PackingTaskStatus.Completed:
        return 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400';
      case PackingTaskStatus.Cancelled:
        return 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400';
      default:
        return 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400';
    }
  }

  canStart(task: PackingTask): boolean {
    return task.status === PackingTaskStatus.Pending || task.status === PackingTaskStatus.Assigned;
  }

  canComplete(task: PackingTask): boolean {
    return task.status === PackingTaskStatus.InProgress;
  }

  canCancel(task: PackingTask): boolean {
    return task.status !== PackingTaskStatus.Completed && task.status !== PackingTaskStatus.Cancelled;
  }

  formatDate(dateStr: string | undefined): string {
    if (!dateStr) return '-';
    return new Date(dateStr).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
