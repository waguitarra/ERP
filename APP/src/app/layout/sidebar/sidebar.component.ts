import { Component, signal, output, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '@core/services/auth.service';

interface MenuItem {
  label: string;
  icon: string;
  route: string;
  category?: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  template: `
    <aside 
      class="fixed inset-y-0 left-0 z-30 w-64 bg-slate-900 dark:bg-slate-950 text-white transition-transform duration-300 ease-in-out lg:translate-x-0 lg:static lg:inset-0 shadow-xl flex flex-col"
      [class.-translate-x-full]="!isOpen()"
      [class.translate-x-0]="isOpen()"
    >
      <!-- Logo -->
      <div class="flex items-center justify-center h-16 bg-slate-950 dark:bg-black shadow-md">
        <div class="flex items-center space-x-2 font-bold text-xl tracking-wider">
          <div class="w-8 h-8 bg-blue-500 rounded-lg flex items-center justify-center">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
            </svg>
          </div>
          <span>NEXUS<span class="text-blue-500">ADMIN</span></span>
        </div>
      </div>

      <!-- Menu de Navegação -->
      <nav class="flex-1 px-4 py-6 space-y-2 overflow-y-auto">
        <p class="px-4 text-xs font-semibold text-slate-400 uppercase tracking-wider mb-2">Principal</p>
        
        @for (item of mainMenuItems; track item.route) {
          <a
            [routerLink]="item.route"
            routerLinkActive="bg-blue-600 text-white"
            [routerLinkActiveOptions]="{exact: item.route === '/dashboard'}"
            class="w-full flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors duration-200 text-slate-400 hover:bg-slate-800 hover:text-white"
            (click)="onMenuClick()"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" [attr.d]="item.icon" />
            </svg>
            <span>{{item.label}}</span>
          </a>
        }

        <p class="px-4 text-xs font-semibold text-slate-400 uppercase tracking-wider mb-2 mt-6">Gestão de Estoque</p>
        
        @for (item of inventoryMenuItems; track item.route) {
          <a
            [routerLink]="item.route"
            routerLinkActive="bg-blue-600 text-white"
            class="w-full flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors duration-200 text-slate-400 hover:bg-slate-800 hover:text-white"
            (click)="onMenuClick()"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" [attr.d]="item.icon" />
            </svg>
            <span>{{item.label}}</span>
          </a>
        }

        <p class="px-4 text-xs font-semibold text-slate-400 uppercase tracking-wider mb-2 mt-6">Logística</p>
        
        @for (item of logisticsMenuItems; track item.route) {
          <a
            [routerLink]="item.route"
            routerLinkActive="bg-blue-600 text-white"
            class="w-full flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors duration-200 text-slate-400 hover:bg-slate-800 hover:text-white"
            (click)="onMenuClick()"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" [attr.d]="item.icon" />
            </svg>
            <span>{{item.label}}</span>
          </a>
        }

        <p class="px-4 text-xs font-semibold text-slate-400 uppercase tracking-wider mb-2 mt-6">Configurações</p>
        
        <button 
          class="w-full flex items-center space-x-3 px-4 py-3 rounded-lg text-slate-400 hover:bg-slate-800 hover:text-white transition-colors duration-200"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
          </svg>
          <span>Ajustes</span>
        </button>
      </nav>

      <!-- Perfil Footer -->
      <div class="p-4 border-t border-slate-800 dark:border-slate-900">
        <div class="flex items-center space-x-3">
          @if (currentUser()) {
            <img src="https://i.pravatar.cc/150?img=11" alt="User" class="w-10 h-10 rounded-full border-2 border-slate-600">
            <div>
              <p class="text-sm font-semibold text-white">{{currentUser()?.name}}</p>
              <p class="text-xs text-slate-400">{{currentUser()?.role}}</p>
            </div>
          } @else {
            <div class="w-10 h-10 rounded-full bg-slate-700 animate-pulse"></div>
            <div class="flex-1">
              <div class="h-4 bg-slate-700 rounded animate-pulse mb-1"></div>
              <div class="h-3 bg-slate-700 rounded animate-pulse w-3/4"></div>
            </div>
          }
        </div>
      </div>
    </aside>
  `,
  styles: []
})
export class SidebarComponent {
  isOpen = input<boolean>(false);
  menuClick = output<void>();

  currentUser = signal(this.authService.currentUser());

  constructor(private authService: AuthService) {}

  mainMenuItems: MenuItem[] = [
    {
      label: 'Dashboard',
      icon: 'M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z',
      route: '/dashboard'
    },
    {
      label: 'Clientes',
      icon: 'M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z',
      route: '/customers'
    },
    {
      label: 'Fornecedores',
      icon: 'M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4',
      route: '/suppliers'
    },
    {
      label: 'Produtos',
      icon: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4',
      route: '/products'
    },
    {
      label: 'Pedidos',
      icon: 'M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z',
      route: '/orders'
    }
  ];

  inventoryMenuItems: MenuItem[] = [
    {
      label: 'Armazéns',
      icon: 'M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6',
      route: '/warehouses'
    },
    {
      label: 'Inventário',
      icon: 'M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01',
      route: '/inventory'
    },
    {
      label: 'Tarefas de Separação',
      icon: 'M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2',
      route: '/picking-tasks'
    },
    {
      label: 'Tarefas de Embalagem',
      icon: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4',
      route: '/packing-tasks'
    }
  ];

  logisticsMenuItems: MenuItem[] = [
    {
      label: 'Recebimentos',
      icon: 'M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12',
      route: '/inbound-shipments'
    },
    {
      label: 'Expedições',
      icon: 'M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M9 19l3 3m0 0l3-3m-3 3V10',
      route: '/outbound-shipments'
    },
    {
      label: 'Veículos',
      icon: 'M13 16V6a1 1 0 00-1-1H4a1 1 0 00-1 1v10a1 1 0 001 1h1m8-1a1 1 0 01-1 1H9m4-1V8a1 1 0 011-1h2.586a1 1 0 01.707.293l3.414 3.414a1 1 0 01.293.707V16a1 1 0 01-1 1h-1m-6-1a1 1 0 001 1h1M5 17a2 2 0 104 0m-4 0a2 2 0 114 0m6 0a2 2 0 104 0m-4 0a2 2 0 114 0',
      route: '/vehicles'
    },
    {
      label: 'Motoristas',
      icon: 'M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z',
      route: '/drivers'
    }
  ];

  onMenuClick(): void {
    this.menuClick.emit();
  }
}
