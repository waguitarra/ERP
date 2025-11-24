import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-inbound-shipments-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './inbound-shipments-list.component.html',
  styleUrls: ['./inbound-shipments-list.component.scss']
})
export class InboundShipmentsListComponent {
  protected readonly i18n = inject(I18nService);
}
