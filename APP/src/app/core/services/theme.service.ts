import { Injectable, signal, effect } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly THEME_KEY = 'nexus_theme';
  
  isDarkMode = signal<boolean>(false);

  constructor() {
    // Carrega tema salvo ou usa preferência do sistema
    const savedTheme = localStorage.getItem(this.THEME_KEY);
    if (savedTheme) {
      this.isDarkMode.set(savedTheme === 'dark');
    } else {
      // Detecta preferência do sistema
      this.isDarkMode.set(
        window.matchMedia('(prefers-color-scheme: dark)').matches
      );
    }

    // Aplica tema ao carregar
    this.applyTheme();

    // Effect para aplicar tema automaticamente quando mudar
    effect(() => {
      this.applyTheme();
    });
  }

  toggleTheme(): void {
    this.isDarkMode.update(dark => !dark);
  }

  setDarkMode(isDark: boolean): void {
    this.isDarkMode.set(isDark);
  }

  private applyTheme(): void {
    const theme = this.isDarkMode() ? 'dark' : 'light';
    localStorage.setItem(this.THEME_KEY, theme);
    
    if (this.isDarkMode()) {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
  }
}
