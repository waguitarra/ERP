import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { HeaderComponent } from '../header/header.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, SidebarComponent, HeaderComponent],
  template: `
    <div class="flex h-screen bg-slate-50 dark:bg-slate-900 font-sans text-slate-800 dark:text-slate-100 overflow-hidden transition-colors duration-200">
      <!-- Overlay para Mobile (fecha sidebar ao clicar fora) -->
      @if (sidebarOpen()) {
        <div 
          (click)="toggleSidebar()"
          class="fixed inset-0 bg-black/50 z-20 lg:hidden glass-effect"
        ></div>
      }

      <!-- SIDEBAR -->
      <app-sidebar 
        [isOpen]="sidebarOpen()"
        (menuClick)="onMobileMenuClick()"
      />

      <!-- CONTEÚDO PRINCIPAL -->
      <div class="flex-1 flex flex-col min-w-0 overflow-hidden">
        <!-- HEADER -->
        <app-header (menuToggle)="toggleSidebar()" />

        <!-- Área Scrollável -->
        <main class="flex-1 overflow-y-auto p-6 bg-slate-50 dark:bg-slate-900 transition-colors duration-200">
          <router-outlet />
        </main>
      </div>
    </div>
  `,
  styles: [`
    .glass-effect {
      backdrop-filter: blur(4px);
    }
  `]
})
export class MainLayoutComponent {
  sidebarOpen = signal(false);

  toggleSidebar(): void {
    this.sidebarOpen.update(v => !v);
  }

  onMobileMenuClick(): void {
    // Fecha sidebar no mobile ao clicar em um item
    if (window.innerWidth < 1024) {
      this.sidebarOpen.set(false);
    }
  }
}
