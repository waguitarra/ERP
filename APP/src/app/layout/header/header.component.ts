import { Component, output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '@core/services/auth.service';
import { ThemeService } from '@core/services/theme.service';
import { I18nService, Language } from '@core/services/i18n.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  menuToggle = output<void>();
  
  private router = inject(Router);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);
  readonly themeService = inject(ThemeService);

  availableLanguages: Language[] = ['pt-BR', 'en-US', 'es-ES'];
  
  changeLanguage(lang: Language): void {
    this.i18n.setLanguage(lang);
  }

  toggleMenu(): void {
    this.menuToggle.emit();
  }

  logout(): void {
    this.authService.logout();
  }

  toggleDarkMode(): void {
    this.themeService.toggleTheme();
  }
}
