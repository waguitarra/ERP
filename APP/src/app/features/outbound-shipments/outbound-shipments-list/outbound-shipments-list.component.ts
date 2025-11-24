import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-outbound-shipments-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './outbound-shipments-list.component.html',
  styleUrls: ['./outbound-shipments-list.component.scss']
})
export class OutboundShipmentsListComponent {
  protected readonly i18n = inject(I18nService);
}
