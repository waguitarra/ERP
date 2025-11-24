import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-toast-notifications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './toast-notifications.component.html',
  styleUrls: ['./toast-notifications.component.scss']
})
export class ToastNotificationsComponent {
  private notificationService = inject(NotificationService);
  
  notifications = this.notificationService.notifications;

  close(id: string): void {
    this.notificationService.remove(id);
  }
}
