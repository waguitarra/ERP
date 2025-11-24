# 📋 PADRÃO DE DESENVOLVIMENTO - NEXUS ADMIN ANGULAR 18

## 🎯 Visão Geral
Este documento estabelece os padrões de desenvolvimento para o projeto NEXUS ADMIN, um sistema ERP completo construído com Angular 18 e Tailwind CSS.

---

## 📁 Estrutura de Pastas

```
APP/
├── src/
│   ├── app/
│   │   ├── core/                          # Módulo principal (singleton)
│   │   │   ├── guards/                    # Route guards
│   │   │   ├── interceptors/              # HTTP interceptors
│   │   │   ├── services/                  # Serviços globais
│   │   │   │   ├── auth.service.ts
│   │   │   │   ├── api.service.ts
│   │   │   │   ├── storage.service.ts
│   │   │   │   └── theme.service.ts       # Gerenciamento de tema
│   │   │   └── models/                    # Interfaces e tipos globais
│   │   │
│   │   ├── shared/                        # Componentes e recursos compartilhados
│   │   │   ├── components/
│   │   │   │   ├── table/                 # Componente de tabela reutilizável
│   │   │   │   ├── modal/                 # Modal genérico
│   │   │   │   ├── form-input/            # Inputs padronizados
│   │   │   │   ├── pagination/            # Paginação
│   │   │   │   ├── loading-spinner/       # Loading state
│   │   │   │   └── confirm-dialog/        # Diálogo de confirmação
│   │   │   ├── directives/
│   │   │   ├── pipes/
│   │   │   └── utils/
│   │   │
│   │   ├── layout/                        # Componentes de layout
│   │   │   ├── main-layout/
│   │   │   │   └── main-layout.component.ts
│   │   │   ├── sidebar/
│   │   │   │   └── sidebar.component.ts
│   │   │   ├── header/
│   │   │   │   └── header.component.ts
│   │   │   └── footer/
│   │   │       └── footer.component.ts
│   │   │
│   │   ├── features/                      # Módulos de features (um por endpoint)
│   │   │   ├── dashboard/
│   │   │   ├── products/
│   │   │   ├── customers/
│   │   │   └── ... (um para cada controller)
│   │   │
│   │   ├── app.component.ts               # Root component
│   │   ├── app.routes.ts                  # Rotas principais
│   │   └── app.config.ts                  # Configuração da aplicação
│   │
│   ├── assets/
│   ├── styles/
│   │   └── tailwind.css
│   ├── environments/
│   ├── index.html
│   └── main.ts
│
├── angular.json
├── package.json
├── tsconfig.json
├── tailwind.config.js
└── README.md
```

---

## 🌓 MODO ESCURO (DARK MODE)

### Configuração Tailwind

O projeto usa a estratégia `class` do Tailwind para modo escuro:

```javascript
// tailwind.config.js
module.exports = {
  darkMode: 'class', // ⚠️ OBRIGATÓRIO
  // ...
}
```

### Paleta de Cores para Dark Mode

```typescript
const CORES_DARK_MODE = {
  // LIGHT MODE
  background: 'bg-slate-50',           // Fundo claro
  backgroundSecondary: 'bg-white',     // Cards e containers
  text: 'text-slate-800',              // Texto principal
  textSecondary: 'text-slate-500',     // Texto secundário
  border: 'border-slate-200',          // Bordas
  
  // DARK MODE (usar com dark:)
  darkBackground: 'dark:bg-slate-900',           // Fundo escuro
  darkBackgroundSecondary: 'dark:bg-slate-800',  // Cards escuros
  darkText: 'dark:text-slate-100',               // Texto claro
  darkTextSecondary: 'dark:text-slate-400',      // Texto secundário claro
  darkBorder: 'dark:border-slate-700',           // Bordas escuras
};
```

### Padrão de Classes Dark

**REGRA:** Toda classe de cor/fundo DEVE ter sua versão `dark:`

```html
<!-- ❌ ERRADO - Sem dark mode -->
<div class="bg-white text-slate-800">

<!-- ✅ CORRETO - Com dark mode -->
<div class="bg-white dark:bg-slate-800 text-slate-800 dark:text-slate-100">
```

---

## 🎨 Padrões de CSS/Estilo

### 1. Tailwind CSS - Paleta de Cores Oficial

```typescript
const CORES_PADRAO = {
  primaria: 'blue-500, blue-600',              // Ações principais
  secundaria: 'slate-800, slate-900',           // Textos e fundos escuros
  sucesso: 'green-500, emerald-500',            // Confirmações e positivo
  erro: 'red-500',                              // Erros e deletar
  aviso: 'amber-500, orange-500',               // Avisos
  info: 'purple-500',                           // Informações
  
  // Light Mode
  fundoClaro: 'slate-50, slate-100',            // Backgrounds
  textoClaro: 'slate-800, slate-600, slate-400',// Hierarquia de texto
  
  // Dark Mode
  fundoEscuro: 'slate-900, slate-800, slate-700',
  textoEscuro: 'slate-100, slate-300, slate-400'
};
```

### 2. Componentes com Classes Padrão + Dark Mode

#### **Card Container**
```html
<div class="bg-white dark:bg-slate-800 p-6 rounded-xl border border-slate-200 dark:border-slate-700 shadow-sm hover:shadow-md transition-shadow duration-300">
  <!-- Conteúdo -->
</div>
```

#### **Botão Primário**
```html
<button class="px-4 py-2 bg-blue-600 hover:bg-blue-700 dark:bg-blue-500 dark:hover:bg-blue-600 text-white rounded-lg transition-colors duration-200 font-medium">
  Ação
</button>
```

#### **Botão Secundário**
```html
<button class="px-4 py-2 border border-slate-300 dark:border-slate-600 text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-700 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-600 transition-colors duration-200">
  Cancelar
</button>
```

#### **Input de Formulário**
```html
<input 
  type="text" 
  class="block w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-slate-100 placeholder-slate-400 dark:placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition duration-150"
  placeholder="Digite aqui..."
>
```

#### **Badge de Status**
```html
<!-- Sucesso -->
<span class="px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-50 dark:bg-green-900/30 text-green-600 dark:text-green-400">
  Ativo
</span>

<!-- Pendente -->
<span class="px-2.5 py-0.5 rounded-full text-xs font-medium bg-amber-50 dark:bg-amber-900/30 text-amber-600 dark:text-amber-400">
  Pendente
</span>

<!-- Erro -->
<span class="px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-50 dark:bg-red-900/30 text-red-600 dark:text-red-400">
  Cancelado
</span>
```

#### **Texto e Títulos**
```html
<!-- Título Principal -->
<h1 class="text-2xl font-bold text-slate-800 dark:text-slate-100">Título</h1>

<!-- Subtítulo -->
<p class="text-slate-500 dark:text-slate-400">Descrição</p>

<!-- Texto Corpo -->
<p class="text-slate-700 dark:text-slate-300">Conteúdo</p>
```

### 3. Responsividade Obrigatória

**Breakpoints Tailwind:**
- `sm:` 640px - Tablets pequenos
- `md:` 768px - Tablets
- `lg:` 1024px - Desktop
- `xl:` 1280px - Desktop grande

**Padrão de Grid:**
```html
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
  <!-- Cards -->
</div>
```

---

## 🧩 Padrões de Componentes Angular

### 1. Estrutura de Componente Standalone

```typescript
import { Component, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-nome-componente',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './nome-componente.component.html',
  styleUrls: ['./nome-componente.component.scss']
})
export class NomeComponenteComponent {
  // Injeção de dependências com inject()
  private readonly service = inject(NomeService);
  
  // Estados com Signals (Angular 17+)
  loading = signal<boolean>(false);
  data = signal<TipoData[]>([]);
  error = signal<string | null>(null);
  
  // Computed signals
  hasData = computed(() => this.data().length > 0);
  
  // Métodos
  async loadData(): Promise<void> {
    this.loading.set(true);
    try {
      const result = await this.service.getData();
      this.data.set(result);
    } catch (err) {
      this.error.set('Erro ao carregar dados');
    } finally {
      this.loading.set(false);
    }
  }
}
```

### 2. ThemeService Pattern

```typescript
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
      this.isDarkMode.set(
        window.matchMedia('(prefers-color-scheme: dark)').matches
      );
    }

    // Aplica tema ao carregar
    this.applyTheme();

    // Effect para aplicar tema quando mudar
    effect(() => {
      this.applyTheme();
    });
  }

  toggleTheme(): void {
    this.isDarkMode.update(dark => !dark);
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
```

### 3. Service Pattern

```typescript
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NomeService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api`;

  // Métodos CRUD padrão
  getAll(): Promise<TipoData[]> {
    return firstValueFrom(
      this.http.get<TipoData[]>(`${this.baseUrl}/endpoint`)
    );
  }

  getById(id: number): Promise<TipoData> {
    return firstValueFrom(
      this.http.get<TipoData>(`${this.baseUrl}/endpoint/${id}`)
    );
  }

  create(data: CreateDto): Promise<TipoData> {
    return firstValueFrom(
      this.http.post<TipoData>(`${this.baseUrl}/endpoint`, data)
    );
  }

  update(id: number, data: UpdateDto): Promise<void> {
    return firstValueFrom(
      this.http.put<void>(`${this.baseUrl}/endpoint/${id}`, data)
    );
  }

  delete(id: number): Promise<void> {
    return firstValueFrom(
      this.http.delete<void>(`${this.baseUrl}/endpoint/${id}`)
    );
  }
}
```

### 4. Model/Interface Pattern

```typescript
export interface NomeEntity {
  id: number;
  nome: string;
  descricao: string;
  status: 'Ativo' | 'Inativo';
  dataCriacao: Date;
  dataAtualizacao: Date;
}

export interface CreateNomeDto {
  nome: string;
  descricao: string;
}

export interface UpdateNomeDto {
  nome?: string;
  descricao?: string;
  status?: 'Ativo' | 'Inativo';
}

export interface NomeListResponse {
  data: NomeEntity[];
  total: number;
  page: number;
  pageSize: number;
}
```

---

## 🛣️ Padrões de Rotas

```typescript
// app.routes.ts
import { Routes } from '@angular/router';
import { authGuard } from '@core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component')
      .then(m => m.LoginComponent)
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./layout/main-layout/main-layout.component')
      .then(m => m.MainLayoutComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component')
          .then(m => m.DashboardComponent)
      },
      {
        path: 'products',
        loadChildren: () => import('./features/products/products.routes')
          .then(m => m.PRODUCTS_ROUTES)
      }
    ]
  }
];
```

---

## 📋 Template Padrão com Dark Mode

### Lista/Tabela de Dados

```html
<div class="space-y-6">
  <!-- Header -->
  <div class="flex items-center justify-between">
    <div>
      <h1 class="text-2xl font-bold text-slate-800 dark:text-slate-100">Título da Página</h1>
      <p class="text-slate-500 dark:text-slate-400 mt-1">Descrição</p>
    </div>
    <button class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors font-medium">
      Novo Item
    </button>
  </div>

  <!-- Tabela -->
  <div class="bg-white dark:bg-slate-800 rounded-xl border border-slate-200 dark:border-slate-700 shadow-sm overflow-hidden">
    <table class="w-full">
      <thead class="bg-slate-50 dark:bg-slate-700/50 border-b border-slate-200 dark:border-slate-600">
        <tr>
          <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase">
            Coluna
          </th>
        </tr>
      </thead>
      <tbody class="divide-y divide-slate-200 dark:divide-slate-700">
        <tr class="hover:bg-slate-50 dark:hover:bg-slate-700/50 transition-colors">
          <td class="px-6 py-4 text-sm text-slate-900 dark:text-slate-100">
            Conteúdo
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</div>
```

---

## ✅ Checklist de Qualidade

### Antes de Commit

- [ ] Componente usa Signals para estado reativo
- [ ] Componente é standalone
- [ ] Segue padrão de CSS (Tailwind)
- [ ] **Todas as classes de cor têm versão dark:**
- [ ] É responsivo (mobile, tablet, desktop)
- [ ] Tem tratamento de erro
- [ ] Tem loading state
- [ ] Tem empty state
- [ ] Service usa async/await com firstValueFrom
- [ ] Interfaces/Models estão tipados
- [ ] Não há console.log no código
- [ ] Não há CSS inline
- [ ] Usa inject() ao invés de constructor injection

---

## 🚫 O QUE NÃO FAZER

1. ❌ **NUNCA** usar `any` - sempre tipar corretamente
2. ❌ **NUNCA** criar CSS customizado fora do Tailwind
3. ❌ **NUNCA** usar módulos NgModule - sempre standalone
4. ❌ **NUNCA** misturar padrões de cores diferentes
5. ❌ **NUNCA** fazer componentes não responsivos
6. ❌ **NUNCA** usar BehaviorSubject quando Signal resolver
7. ❌ **NUNCA** fazer requests HTTP sem tratamento de erro
8. ❌ **NUNCA** hardcodar URLs da API - usar environment
9. ❌ **NUNCA** esquecer classes `dark:` para modo escuro

---

## 📦 Dependências Padrão

```json
{
  "@angular/core": "^18.0.0",
  "@angular/common": "^18.0.0",
  "@angular/forms": "^18.0.0",
  "@angular/router": "^18.0.0",
  "tailwindcss": "^3.4.0",
  "rxjs": "^7.8.0"
}
```

---

## 📝 Convenções de Nomenclatura

- **Componentes**: PascalCase + Component (ex: `ProductListComponent`)
- **Services**: PascalCase + Service (ex: `ProductService`)
- **Interfaces**: PascalCase (ex: `Product`, `CreateProductDto`)
- **Variáveis/Métodos**: camelCase (ex: `loadProducts`, `currentUser`)
- **Constantes**: UPPER_SNAKE_CASE (ex: `API_BASE_URL`)
- **Arquivos**: kebab-case (ex: `product-list.component.ts`)

---

**Este padrão deve ser seguido RIGOROSAMENTE em todo o projeto. Não há exceções.**
