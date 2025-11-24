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
│   │   │   │   └── storage.service.ts
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
│   │   │   │   ├── dashboard.component.ts
│   │   │   │   ├── dashboard.routes.ts
│   │   │   │   └── dashboard.service.ts
│   │   │   │
│   │   │   ├── products/
│   │   │   │   ├── products-list/
│   │   │   │   │   └── products-list.component.ts
│   │   │   │   ├── product-detail/
│   │   │   │   │   └── product-detail.component.ts
│   │   │   │   ├── product-form/
│   │   │   │   │   └── product-form.component.ts
│   │   │   │   ├── products.routes.ts
│   │   │   │   ├── products.service.ts
│   │   │   │   └── products.model.ts
│   │   │   │
│   │   │   ├── customers/
│   │   │   ├── orders/
│   │   │   ├── warehouses/
│   │   │   ├── inventory/
│   │   │   ├── suppliers/
│   │   │   ├── inbound-shipments/
│   │   │   ├── outbound-shipments/
│   │   │   ├── picking-tasks/
│   │   │   ├── packing-tasks/
│   │   │   ├── vehicles/
│   │   │   └── ... (um para cada controller)
│   │   │
│   │   ├── app.component.ts               # Root component
│   │   ├── app.routes.ts                  # Rotas principais
│   │   └── app.config.ts                  # Configuração da aplicação
│   │
│   ├── assets/
│   │   ├── images/
│   │   └── icons/
│   │
│   ├── styles/
│   │   ├── tailwind.css                   # Configuração Tailwind
│   │   ├── _variables.scss                # Variáveis CSS
│   │   └── _utilities.scss                # Utilities customizadas
│   │
│   ├── environments/
│   │   ├── environment.ts
│   │   └── environment.development.ts
│   │
│   ├── index.html
│   ├── main.ts
│   └── styles.scss
│
├── angular.json
├── package.json
├── tsconfig.json
├── tailwind.config.js
└── README.md
```

---

## 🎨 Padrões de CSS/Estilo

### 1. Tailwind CSS - Paleta de Cores Oficial

```typescript
// Usar APENAS estas classes de cores:
const CORES_PADRAO = {
  primaria: 'blue-500, blue-600',      // Ações principais
  secundaria: 'slate-800, slate-900',   // Textos e fundos escuros
  sucesso: 'green-500, emerald-500',    // Confirmações e positivo
  erro: 'red-500',                      // Erros e deletar
  aviso: 'amber-500, orange-500',       // Avisos
  info: 'purple-500',                   // Informações
  fundo: 'slate-50, slate-100',         // Backgrounds
  texto: 'slate-800, slate-600, slate-400' // Hierarquia de texto
};
```

### 2. ⚠️ PADRÃO OBRIGATÓRIO DE DARK MODE

**🔴 REGRA ABSOLUTA: TODOS os componentes DEVEM seguir este padrão EXATAMENTE como o componente `products-list`**

#### Classes Obrigatórias para Dark Mode:

**Títulos e Textos:**
```html
<!-- Título Principal -->
<h1 class="text-2xl font-bold text-slate-800 dark:text-slate-100">Título</h1>

<!-- Subtítulo/Descrição -->
<p class="text-slate-500 dark:text-slate-400">Descrição</p>

<!-- Texto Primário -->
<span class="text-slate-900 dark:text-slate-100">Texto</span>

<!-- Texto Secundário -->
<span class="text-slate-600 dark:text-slate-400">Info</span>
```

**Cards e Containers:**
```html
<!-- Card Principal -->
<div class="bg-white dark:bg-slate-800 p-6 rounded-xl border border-slate-200 dark:border-slate-700 shadow-sm">
  <!-- Conteúdo -->
</div>

<!-- Card Filtros/Busca -->
<div class="bg-white dark:bg-slate-800 p-4 rounded-xl border border-slate-200 dark:border-slate-700 shadow-sm">
  <!-- Filtros -->
</div>
```

**Inputs:**
```html
<input 
  class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 bg-white dark:bg-slate-700 text-slate-900 dark:text-slate-100 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition duration-150"
>
```

**Tabelas:**
```html
<!-- Thead -->
<thead class="bg-slate-50 dark:bg-slate-700/50 border-b border-slate-200 dark:border-slate-600">

<!-- Tbody -->
<tbody class="divide-y divide-slate-200 dark:divide-slate-700">

<!-- Row Hover -->
<tr class="hover:bg-slate-50 dark:hover:bg-slate-700/50 transition-colors">
```

**Badges de Status:**
```html
<!-- Sucesso -->
<span class="px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-50 dark:bg-green-900/30 text-green-600 dark:text-green-400">Ativo</span>

<!-- Erro -->
<span class="px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-50 dark:bg-red-900/30 text-red-600 dark:text-red-400">Inativo</span>

<!-- Aviso -->
<span class="px-2.5 py-0.5 rounded-full text-xs font-medium bg-amber-50 dark:bg-amber-900/30 text-amber-600 dark:text-amber-400">Pendente</span>
```

**Ícones/Avatares de Background:**
```html
<!-- Background neutro -->
<div class="bg-slate-100 dark:bg-slate-700">

<!-- Background colorido -->
<div class="bg-blue-100 dark:bg-blue-900/30">
<div class="bg-purple-100 dark:bg-purple-900/30">
<div class="bg-green-100 dark:bg-green-900/30">
```

**Estados Empty/Loading:**
```html
<!-- Loading -->
<p class="text-slate-500 dark:text-slate-400">Carregando...</p>

<!-- Empty State Icon Container -->
<div class="bg-slate-100 dark:bg-slate-700 rounded-full">

<!-- Empty State Texto -->
<h3 class="text-slate-900 dark:text-slate-100">Nenhum item</h3>
<p class="text-slate-500 dark:text-slate-400">Descrição</p>
```

**⚠️ NUNCA USAR:**
- ❌ `bg-white` sem `dark:bg-slate-800`
- ❌ `text-slate-800` sem `dark:text-slate-100`
- ❌ `border-slate-200` sem `dark:border-slate-700`
- ❌ `bg-green-50` sem `dark:bg-green-900/30`
- ❌ Qualquer cor de fundo claro sem o equivalente dark mode

### 3. Componentes com Classes Padrão

#### **Card Container**
```html
<div class="bg-white p-6 rounded-xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow duration-300">
  <!-- Conteúdo -->
</div>
```

#### **Botão Primário**
```html
<button class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors duration-200 font-medium">
  Ação
</button>
```

#### **Botão Secundário**
```html
<button class="px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 transition-colors duration-200">
  Cancelar
</button>
```

#### **Input de Formulário**
```html
<input 
  type="text" 
  class="block w-full px-4 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition duration-150"
  placeholder="Digite aqui..."
>
```

#### **Badge de Status**
```html
<!-- Sucesso -->
<span class="px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-50 text-green-600">
  Ativo
</span>

<!-- Pendente -->
<span class="px-2.5 py-0.5 rounded-full text-xs font-medium bg-amber-50 text-amber-600">
  Pendente
</span>

<!-- Erro -->
<span class="px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-50 text-red-600">
  Cancelado
</span>
```

### 4. Responsividade Obrigatória

**Breakpoints Tailwind:**
- `sm:` 640px - Tablets pequenos
- `md:` 768px - Tablets
- `lg:` 1024px - Desktop
- `xl:` 1280px - Desktop grande

**Padrão de Grid:**
```html
<!-- Mobile: 1 coluna | Tablet: 2 colunas | Desktop: 4 colunas -->
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

### 2. Service Pattern

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

### 3. Model/Interface Pattern

```typescript
// models/nome.model.ts
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
import { AuthGuard } from '@core/guards/auth.guard';

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
    canActivate: [AuthGuard],
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
      },
      // ... demais rotas
    ]
  }
];
```

---

## 📋 Padrões de Templates

### Lista/Tabela de Dados

```html
<div class="space-y-6">
  <!-- Header -->
  <div class="flex items-center justify-between">
    <div>
      <h1 class="text-2xl font-bold text-slate-800">Título da Página</h1>
      <p class="text-slate-500 mt-1">Descrição da funcionalidade</p>
    </div>
    <button 
      (click)="openCreateModal()"
      class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors duration-200 font-medium flex items-center space-x-2"
    >
      <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
      </svg>
      <span>Novo Item</span>
    </button>
  </div>

  <!-- Filtros e Busca -->
  <div class="bg-white p-4 rounded-xl border border-slate-200">
    <div class="flex flex-col md:flex-row gap-4">
      <div class="flex-1">
        <input 
          type="search"
          placeholder="Buscar..."
          class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
      </div>
      <button class="px-4 py-2 border border-slate-300 rounded-lg hover:bg-slate-50">
        Filtros
      </button>
    </div>
  </div>

  <!-- Tabela/Cards -->
  <div class="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
    @if (loading()) {
      <div class="p-12 text-center">
        <div class="inline-block w-8 h-8 border-4 border-blue-500 border-t-transparent rounded-full animate-spin"></div>
        <p class="mt-4 text-slate-500">Carregando...</p>
      </div>
    } @else if (error()) {
      <div class="p-12 text-center">
        <p class="text-red-600">{{ error() }}</p>
      </div>
    } @else if (hasData()) {
      <table class="w-full">
        <thead class="bg-slate-50 border-b border-slate-200">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">
              Coluna 1
            </th>
            <!-- ... -->
          </tr>
        </thead>
        <tbody class="divide-y divide-slate-200">
          @for (item of data(); track item.id) {
            <tr class="hover:bg-slate-50 transition-colors">
              <td class="px-6 py-4">{{ item.nome }}</td>
              <!-- ... -->
            </tr>
          }
        </tbody>
      </table>
    } @else {
      <div class="p-12 text-center">
        <p class="text-slate-500">Nenhum item encontrado</p>
      </div>
    }
  </div>
</div>
```

---

## ✅ Checklist de Qualidade

### Antes de Commit

- [ ] Componente usa Signals para estado reativo
- [ ] Componente é standalone
- [ ] Segue padrão de CSS (Tailwind)
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
2. ❌ **NUNCA** criar CSS customizado fora do Tailwind (exceto utilities específicas)
3. ❌ **NUNCA** usar módulos NgModule - sempre standalone
4. ❌ **NUNCA** misturar padrões de cores diferentes
5. ❌ **NUNCA** fazer componentes não responsivos
6. ❌ **NUNCA** usar BehaviorSubject quando Signal resolver
7. ❌ **NUNCA** fazer requests HTTP sem tratamento de erro
8. ❌ **NUNCA** hardcodar URLs da API - usar environment

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

## 🔐 Padrões de Segurança

1. Sempre usar AuthGuard nas rotas protegidas
2. Token JWT armazenado em localStorage com prefix `nexus_`
3. Interceptor para adicionar token automaticamente
4. Redirect para login em caso de 401
5. Sanitização de inputs do usuário

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
