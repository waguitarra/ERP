import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-packing-tasks-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './packing-tasks-list.component.html',
  styleUrls: ['./packing-tasks-list.component.scss']
})
export class PackingTasksListComponent {
  protected readonly i18n = inject(I18nService);
}
