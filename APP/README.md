# 🚀 WMS ADMIN - Sistema ERP

Sistema de Gestão ERP completo construído com **Angular 18** e **Tailwind CSS**.

## 📋 Funcionalidades

- ✅ Dashboard com métricas e gráficos
- ✅ Gestão de Produtos
- ✅ Gestão de Clientes
- ✅ Gestão de Fornecedores
- ✅ Gestão de Pedidos
- ✅ Controle de Estoque e Inventário
- ✅ Gestão de Armazéns
- ✅ Recebimentos e Expedições
- ✅ Tarefas de Separação (Picking)
- ✅ Tarefas de Embalagem (Packing)
- ✅ Gestão de Veículos e Motoristas
- ✅ Sistema de Autenticação JWT
- ✅ Layout Responsivo (Mobile, Tablet, Desktop)

## 🛠️ Tecnologias

- **Angular 18** - Framework principal
- **TypeScript 5.4** - Linguagem
- **Tailwind CSS 3.4** - Estilização
- **Signals** - Gerenciamento de estado reativo
- **Standalone Components** - Arquitetura moderna
- **HTTP Client** - Comunicação com API

## 📦 Instalação

### Pré-requisitos

- Node.js 18+ 
- npm ou yarn

### Passos

1. **Instalar dependências:**
```bash
cd APP
npm install
```

2. **Configurar variáveis de ambiente:**

Edite o arquivo `src/environments/environment.development.ts` e configure a URL da API:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000',  // URL da sua API
  appName: 'WMS ADMIN [DEV]',
  version: '1.0.0-dev'
};
```

3. **Iniciar servidor de desenvolvimento:**
```bash
npm start
```

A aplicação estará disponível em `http://localhost:4200`

## 🏗️ Estrutura do Projeto

```
APP/
├── src/
│   ├── app/
│   │   ├── core/              # Serviços, guards, interceptors
│   │   ├── shared/            # Componentes compartilhados
│   │   ├── layout/            # Layout (sidebar, header)
│   │   ├── features/          # Módulos de funcionalidades
│   │   │   ├── dashboard/
│   │   │   ├── products/
│   │   │   ├── customers/
│   │   │   ├── orders/
│   │   │   └── ...
│   │   ├── app.component.ts
│   │   ├── app.config.ts
│   │   └── app.routes.ts
│   ├── environments/
│   ├── styles.scss
│   └── index.html
├── angular.json
├── package.json
├── tailwind.config.js
└── tsconfig.json
```

## 📐 Padrões de Desenvolvimento

**IMPORTANTE:** Leia o documento [DEVELOPMENT-STANDARDS.md](./DEVELOPMENT-STANDARDS.md) antes de fazer qualquer modificação no código.

### Principais Padrões:

1. **Componentes Standalone** - Todos os componentes devem ser standalone
2. **Signals** - Usar Signals para estado reativo ao invés de BehaviorSubject
3. **Tailwind CSS** - Usar apenas classes Tailwind, sem CSS customizado
4. **Responsividade** - Mobile-first, responsivo para todas as telas
5. **TypeScript Strict** - Tipagem forte em todo o código

### Exemplo de Componente:

```typescript
import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductsService } from './products.service';

@Component({
  selector: 'app-products-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './products-list.component.html'
})
export class ProductsListComponent implements OnInit {
  private readonly service = inject(ProductsService);
  
  loading = signal<boolean>(false);
  data = signal<Product[]>([]);
  
  ngOnInit(): void {
    this.loadData();
  }
  
  async loadData(): Promise<void> {
    this.loading.set(true);
    try {
      const result = await this.service.getAll();
      this.data.set(result.data);
    } catch (err) {
      console.error(err);
    } finally {
      this.loading.set(false);
    }
  }
}
```

## 🎨 Paleta de Cores

- **Primária:** Blue (500, 600, 700)
- **Secundária:** Slate (800, 900)
- **Sucesso:** Green/Emerald (500)
- **Erro:** Red (500, 600)
- **Aviso:** Amber/Orange (500)
- **Info:** Purple (500)

## 🔐 Autenticação

O sistema usa JWT para autenticação. O token é armazenado no localStorage com o prefixo `WMS_`.

### Login:
```typescript
await authService.login({
  email: 'user@example.com',
  password: 'senha123'
});
```

### Logout:
```typescript
authService.logout();
```

## 📱 Responsividade

Breakpoints Tailwind:
- `sm:` 640px - Tablets pequenos
- `md:` 768px - Tablets
- `lg:` 1024px - Desktop
- `xl:` 1280px - Desktop grande

## 🚀 Build para Produção

```bash
npm run build
```

Os arquivos otimizados serão gerados em `dist/WMS-admin/`

## 📝 Scripts Disponíveis

- `npm start` - Inicia servidor de desenvolvimento
- `npm run build` - Build de produção
- `npm run watch` - Build em modo watch
- `npm test` - Executa testes
- `npm run lint` - Executa linter

## 🔧 Integração com API

Todas as chamadas à API passam pelo `ApiService` que:
- Adiciona automaticamente o token JWT
- Trata erros de forma padronizada
- Usa async/await com Promises
- Converte QueryParams automaticamente

### Exemplo de Service:

```typescript
@Injectable({ providedIn: 'root' })
export class ProductsService {
  private readonly api = inject(ApiService);
  
  getAll(page: number = 1): Promise<ProductListResponse> {
    return this.api.get<ProductListResponse>('/products', { page });
  }
  
  create(data: CreateProductDto): Promise<Product> {
    return this.api.post<Product>('/products', data);
  }
}
```

## 🐛 Troubleshooting

### Erro: "Cannot find module..."
```bash
npm install
```

### Erro de compilação Tailwind
```bash
npm install -D tailwindcss postcss autoprefixer
```

### API não conecta
Verifique a configuração em `src/environments/environment.development.ts`

## 📄 Licença

© 2024 WMS ADMIN - Todos os direitos reservados

## 👥 Suporte

Para dúvidas e suporte, consulte a documentação interna ou entre em contato com a equipe de desenvolvimento.
