import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PickingTasksService } from '../picking-tasks.service';
import { PickingTask, PickingTaskStatus, TaskPriority } from '@core/models/picking-task.model';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-picking-tasks-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './picking-tasks-list.component.html',
  styleUrls: ['./picking-tasks-list.component.scss']
})
export class PickingTasksListComponent implements OnInit {
  private readonly pickingTasksService = inject(PickingTasksService);
  protected readonly i18n = inject(I18nService);

  loading = signal<boolean>(false);
  error = signal<string | null>(null);
  tasks = signal<PickingTask[]>([]);
  selectedFilter = signal<string>('all');
  searchTerm = signal<string>('');
  
  hasData = computed(() => this.tasks().length > 0);
  
  filteredTasks = computed(() => {
    let result = this.tasks();
    const search = this.searchTerm().toLowerCase();
    
    if (search) {
      result = result.filter(t => 
        t.taskNumber.toLowerCase().includes(search) ||
        t.orderNumber?.toLowerCase().includes(search) ||
        t.waveNumber?.toLowerCase().includes(search)
      );
    }
    
    return result;
  });

  // Statistics
  stats = computed(() => {
    const all = this.tasks();
    return {
      total: all.length,
      pending: all.filter(t => t.status === PickingTaskStatus.Pending).length,
      inProgress: all.filter(t => t.status === PickingTaskStatus.InProgress).length,
      completed: all.filter(t => t.status === PickingTaskStatus.Completed).length,
      cancelled: all.filter(t => t.status === PickingTaskStatus.Cancelled).length
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
          response = await this.pickingTasksService.getPending();
          break;
        case 'in-progress':
          response = await this.pickingTasksService.getInProgress();
          break;
        case 'completed':
          response = await this.pickingTasksService.getByStatus(PickingTaskStatus.Completed);
          break;
        default:
          response = await this.pickingTasksService.getAll();
      }
      this.tasks.set(response.data || []);
    } catch (err: any) {
      this.error.set(err.message || 'Erro ao carregar tarefas de picking');
      console.error('Error loading picking tasks:', err);
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

  async startTask(task: PickingTask): Promise<void> {
    try {
      await this.pickingTasksService.start(task.id);
      await this.loadTasks();
    } catch (error) {
      console.error('Erro ao iniciar task:', error);
    }
  }

  async completeTask(task: PickingTask): Promise<void> {
    try {
      await this.pickingTasksService.complete(task.id);
      await this.loadTasks();
    } catch (error) {
      console.error('Erro ao completar task:', error);
    }
  }

  async cancelTask(task: PickingTask): Promise<void> {
    if (!confirm('Deseja realmente cancelar esta tarefa?')) return;
    
    try {
      await this.pickingTasksService.cancel(task.id);
      await this.loadTasks();
    } catch (error) {
      console.error('Erro ao cancelar task:', error);
    }
  }

  getStatusClass(status: number): string {
    switch (status) {
      case PickingTaskStatus.Pending:
        return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400';
      case PickingTaskStatus.InProgress:
        return 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400';
      case PickingTaskStatus.Completed:
        return 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400';
      case PickingTaskStatus.Cancelled:
        return 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400';
      default:
        return 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400';
    }
  }

  getPriorityClass(priority: number): string {
    switch (priority) {
      case TaskPriority.Urgent:
        return 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400';
      case TaskPriority.High:
        return 'bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-400';
      case TaskPriority.Normal:
        return 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400';
      case TaskPriority.Low:
        return 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400';
      default:
        return 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400';
    }
  }

  getProgress(task: PickingTask): number {
    if (task.totalQuantityToPick === 0) return 0;
    return Math.round((task.totalQuantityPicked / task.totalQuantityToPick) * 100);
  }
}
