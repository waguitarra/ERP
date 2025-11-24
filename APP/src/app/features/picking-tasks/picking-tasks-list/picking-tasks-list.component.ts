import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-picking-tasks-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './picking-tasks-list.component.html',
  styleUrls: ['./picking-tasks-list.component.scss']
})
export class PickingTasksListComponent {
  protected readonly i18n = inject(I18nService);
}
