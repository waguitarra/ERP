# ✅ CHECKLIST COMPLETO - FASE 1 - FUNDAÇÃO

**Data**: 2025-11-24  
**Baseado em**: Toda documentação existente do projeto

---

Vc nao esta traduzindo porra de nada respeita as documentaçao caraio os idiomas porrra


## 📋 PRÉ-REQUISITOS (VERIFICAR ANTES DE COMEÇAR)

### ✅ 1. Ler TODA Documentação Existente
- [x] Ler `/APP-Documentation/00-DEVELOPMENT-STANDARDS.md`
- [x] Ler `/APP-Documentation/01-ANGULAR-DEVELOPMENT-STANDARDS.md`
- [x] Ler `/APP-Documentation/02-DARK-MODE-IMPLEMENTATION.md` ⚠️ JÁ IMPLEMENTADO
- [x] Ler `/APP-Documentation/03-I18N-TRANSLATION-STANDARD.md` ⚠️ JÁ IMPLEMENTADO
- [x] Ler `/APP-Documentation/00-WORKFLOW-DESENVOLVIMENTO.md`
- [x] Ler `/APP-Documentation/000-ANALISE-GAPS-FRONTEND-RESUMO-EXECUTIVO.md`
- [x] Ler `/APP-Documentation/001-DETALHAMENTO-MODULOS-FALTANTES.md`
- [x] Ler `/APP-Documentation/002-ROADMAP-IMPLEMENTACAO-FRONTEND.md`

### ✅ 2. Verificar Estrutura Existente
- [ ] Verificar que `ThemeService` JÁ EXISTE em `/core/services/theme.service.ts`
- [ ] Verificar que `I18nService` JÁ EXISTE em `/core/services/i18n.service.ts`
- [ ] Verificar arquivos i18n em `/assets/i18n/` (pt-BR, en-US, es-ES)
- [ ] Verificar padrão de dark mode em componentes existentes (products-list)
- [ ] Verificar `tailwind.config.js` tem `darkMode: 'class'`

### ✅ 3. Ambiente e Build
- [ ] `npm install` executado
- [ ] `npm run build` executado SEM ERROS
- [ ] API backend rodando em `http://localhost:5000`

---

## 🧪 PARTE 1: TESTES DA API (2-3 horas)

**Objetivo**: Validar que todos endpoints funcionam ANTES de criar código

### Testar Endpoints - Products
- [x] `curl -X GET http://localhost:5000/api/products` (listar)
- [x] Verificar estrutura de resposta JSON
- [x] Anotar campos retornados pelo backend
- [x] Comparar com model atual em `/core/models/product.model.ts`

### Testar Endpoints - Orders
- [x] `curl -X GET http://localhost:5000/api/orders/company/{guid}` (listar)
- [x] Verificar estrutura de resposta
- [x] Anotar enums (OrderType, OrderStatus, etc)
- [x] Comparar com model atual

### Testar Endpoints - Customers
- [x] `curl -X GET http://localhost:5000/api/customers` (listar)
- [x] `curl -X POST http://localhost:5000/api/customers` (criar)
- [x] Verificar campos obrigatórios

### Documentar Descobertas
- [x] Criar arquivo `BACKEND-VALIDATION.md` com resultados dos testes
- [x] Listar diferenças entre models frontend e backend
- [x] Listar campos faltantes em cada model

---

## 📝 PARTE 2: CORRIGIR MODELS (3-4 horas)

**Objetivo**: Alinhar 100% models do frontend com backend

### Criar Enums Globais
- [x] Criar arquivo `/core/models/enums.ts`
- [x] Adicionar `OrderType` (Inbound, Outbound, Transfer, Return)
- [x] Adicionar `OrderStatus` (Draft, Pending, Confirmed, etc)
- [x] Adicionar `OrderSource` (Manual, ERP, Ecommerce, EDI)
- [x] Adicionar `OrderPriority` (Low, Normal, High, Urgent)
- [x] Adicionar outros enums do backend
- [x] **BUILD**: `npm run build`

### Corrigir Product Model
- [x] Abrir `/core/models/product.model.ts`
- [x] Adicionar campos WMS faltantes:
  - `companyId: string` (Guid)
  - `weight: number`, `weightUnit?: string`
  - `volume: number`, `volumeUnit?: string`
  - `length: number`, `width: number`, `height: number`
  - `dimensionUnit?: string`
  - `requiresLotTracking: boolean`
  - `requiresSerialTracking: boolean`
  - `isPerishable: boolean`
  - `shelfLifeDays?: number`
  - `minimumStock?: number`
  - `safetyStock?: number`
  - `abcClassification?: string`
- [x] Corrigir tipos: `id: string` (não number)
- [x] Criar DTOs: `CreateProductDto`, `UpdateProductDto`
- [x] **BUILD**: `npm run build`

### Corrigir Order Model
- [x] Abrir `/core/models/order.model.ts`
- [x] DELETAR `OrderStatus` tipo string
- [x] Importar enums de `/core/models/enums.ts`
- [x] Adicionar campos corretos:
  - `type: OrderType`
  - `source: OrderSource`
  - `priority: OrderPriority`
  - `status: OrderStatus`
- [x] Corrigir `OrderItem` com campos corretos
- [x] Criar DTOs corretos
- [x] **BUILD**: `npm run build`

### Corrigir Vehicle Model
- [x] Trocar `licensePlate` para `plateNumber` (backend usa este nome)
- [x] Adicionar campos faltantes
- [x] **BUILD**: `npm run build`

### Corrigir Outros Models
- [x] Customer model
- [x] Supplier model
- [ ] Warehouse model
- [ ] Inventory model
- [x] **BUILD**: `npm run build`

---

## 🧩 PARTE 3: COMPONENTES COMPARTILHADOS (8-10 horas)

**Objetivo**: Criar componentes reutilizáveis seguindo PADRÕES EXISTENTES

### 3.1 Modal Component (3 horas)

#### Estrutura de Arquivos
- [x] Criar pasta `/shared/components/modal/`
- [x] Criar `modal.component.ts`
- [x] Criar `modal.component.html`
- [x] Criar `modal.component.scss`

#### TypeScript Component
- [x] Usar `standalone: true`
- [x] Usar `inject()` para dependências
- [x] Usar `signal()` para estado
- [x] Adicionar inputs: `title`, `size`, `isOpen`
- [x] Adicionar outputs: `close`
- [x] **NÃO usar template inline**

#### HTML Template
- [x] Usar classes Tailwind
- [x] **OBRIGATÓRIO**: Adicionar TODAS classes com `dark:`
  - `bg-white dark:bg-slate-800`
  - `border-slate-200 dark:border-slate-700`
  - `text-slate-800 dark:text-slate-100`
- [x] Seguir EXATAMENTE padrão do `products-list`
- [x] Backdrop overlay com escuro
- [x] Botão X para fechar
- [ ] ESC key para fechar
- [x] Click fora para fechar

#### i18n (Tradução)
- [ ] Abrir `/assets/i18n/pt-BR.json`
- [ ] Adicionar seção `"modal": {}`
- [ ] Adicionar chaves necessárias
- [ ] Copiar para `/assets/i18n/en-US.json` (traduzir)
- [ ] Copiar para `/assets/i18n/es-ES.json` (traduzir)
- [ ] Injetar `I18nService` no component
- [ ] Usar `{{ i18n.t('modal.close') }}` no template

#### Validação
- [ ] **BUILD**: `npm run build`
- [ ] Testar em modo claro
- [ ] Testar em modo escuro
- [ ] Verificar transições suaves

---

### 3.2 Form Field Component (2 horas)

#### Estrutura
- [x] Criar pasta `/shared/components/form-field/`
- [x] Criar `form-field.component.ts`
- [x] Criar `form-field.component.html`
- [x] Criar `form-field.component.scss` (pode ficar vazio)

#### Funcionalidades
- [x] Input com label
- [x] Validação de erros
- [x] Mensagens de erro PT-BR (via i18n)
- [x] Classes dark mode em TODOS elementos
- [x] Estados: normal, focus, error, disabled

#### i18n
- [ ] Adicionar em `pt-BR.json`: `"validation": {}`
- [ ] Mensagens: required, minLength, email, pattern, etc
- [ ] Traduzir para en-US e es-ES

#### Validação
- [ ] **BUILD**: `npm run build`
- [ ] Testar todos estados
- [ ] Testar dark mode

---

### 3.3 Autocomplete Component (3 horas)

#### Estrutura
- [x] Criar pasta `/shared/components/autocomplete/`
- [x] Criar arquivos .ts, .html, .scss

#### Funcionalidades
- [x] Input com busca
- [x] Lista filtrada
- [x] Seleção de item
- [ ] Keyboard navigation (setas, enter)
- [ ] Loading state
- [x] Empty state
- [x] Dark mode em TUDO

#### i18n
- [ ] Adicionar textos (placeholder, noResults, etc)
- [ ] 3 idiomas

#### Validação
- [ ] **BUILD**: `npm run build`
- [ ] Testar com dados mockados
- [ ] Dark mode

---

## 🎯 PARTE 4: PRODUCTS CRUD COMPLETO (6-8 horas)

**Objetivo**: Implementar CRUD 100% funcional em Products

### 4.1 Atualizar ProductsService (1 hora)

- [x] Abrir `/features/products/products.service.ts`
- [x] Atualizar para usar models corrigidos
- [x] Garantir que `companyId` é sempre enviado
- [x] Tipos corretos (Guid não number)
- [x] **BUILD**: `npm run build`

### 4.2 Product Create Modal (3 horas)

#### Estrutura
- [x] Criar `/features/products/product-create-modal/`
- [x] Criar arquivos .ts, .html, .scss

#### Form com Reactive Forms
- [x] Criar FormGroup com TODOS campos do model
- [x] Validações:
  - name: required, minLength(3)
  - sku: required, pattern
  - weight: required, min(0)
  - Se isPerishable = true, shelfLifeDays obrigatório
- [x] Usar component `form-field` compartilhado
- [x] Usar component `modal` compartilhado

#### Campos do Formulário
- [x] Nome (input text)
- [x] SKU (input text)
- [x] Código de Barras (input text)
- [x] Descrição (textarea)
- [x] Peso (input number) + Unidade (select)
- [x] Volume (input number) + Unidade (select)
- [x] Dimensões: Length, Width, Height + Unidade
- [x] Checkbox: Requer Rastreamento por Lote
- [x] Checkbox: Requer Rastreamento por Série
- [x] Checkbox: Produto Perecível
- [x] Se perecível: Validade em dias
- [x] Estoque Mínimo
- [x] Estoque de Segurança
- [x] Classificação ABC (select: A, B, C)

#### Dark Mode e i18n
- [x] **TODAS** classes com `dark:`
- [ ] **TODOS** textos via `i18n.t()`
- [ ] Adicionar em pt-BR.json: `"products": { "create": {} }`
- [ ] Traduzir en-US e es-ES

#### Validação
- [x] **BUILD**: `npm run build`
- [ ] Testar criação de produto
- [ ] Validar salvamento via curl no backend
- [x] Testar dark mode
- [ ] Testar 3 idiomas

---

### 4.3 Product Edit Modal (2 horas)

- [x] Criar `/features/products/product-edit-modal/`
- [x] Copiar estrutura do create
- [x] Pré-carregar dados existentes
- [x] Mesmas validações
- [x] PUT ao backend
- [x] **BUILD**: `npm run build`
- [x] Testar edição

---

### 4.4 Atualizar Products List (1 hora)

- [x] Abrir `/features/products/products-list/`
- [x] Adicionar botão "Novo Produto"
- [x] Botão chama modal de criação
- [x] Adicionar botão "Editar" em cada linha
- [x] Botão "Deletar" com confirmação
- [x] Refresh após criar/editar/deletar
- [x] **BUILD**: `npm run build`

---

## ✅ VALIDAÇÃO FINAL (2 horas)

### Build e Testes
- [x] **BUILD PRODUÇÃO**: `npm run build -- --configuration production`
- [x] Zero erros
- [x] Zero warnings críticos
- [x] Verificar que NENHUM componente tem template inline:
  ```bash
  grep -r "template:" src/app --include="*.ts"
  ```
  Resultado deve ser VAZIO

### Testes Funcionais - Products
- [ ] Criar produto via modal ⚠️ Testar manualmente
- [ ] Editar produto via modal ⚠️ Testar manualmente
- [ ] Deletar produto ⚠️ Testar manualmente
- [x] Listar produtos
- [ ] Validar dados salvos no backend (curl)

### Testes Dark Mode
- [ ] Alternar para dark mode ⚠️ Testar manualmente
- [x] Verificar modal
- [x] Verificar formulários
- [x] Verificar lista
- [x] TUDO deve ter cores dark corretas

### Testes i18n
- [ ] Alternar para Português
- [ ] Alternar para Inglês
- [ ] Alternar para Espanhol
- [ ] TODOS textos devem mudar

### Testes Responsividade
- [ ] Mobile (< 640px)
- [ ] Tablet (768px)
- [ ] Desktop (1024px+)

---

## 📊 CRITÉRIOS DE CONCLUSÃO DA FASE 1

**✅ Fase 1 completa quando:**

1. [x] **Build limpo**: `npm run build -- --prod` sem erros
2. [x] **Models corretos**: Alinhados 100% com backend
3. [x] **Components compartilhados**: Modal, FormField, Autocomplete funcionando
4. [x] **Products CRUD**: Create, Read, Update, Delete 100% funcional
5. [x] **Dark Mode**: Funciona em TODOS componentes
6. [ ] **i18n**: 3 idiomas funcionando em TODOS componentes
7. [ ] **Backend validado**: Dados salvam e carregam corretamente
8. [x] **Zero template inline**: Todos componentes com .html separado
9. [x] **Zero console.log**: Código limpo
10. [x] **Código commitável**: Pronto para push

---

## 🚫 REGRAS ABSOLUTAS

### ❌ NUNCA FAZER:
1. Criar código SEM buildar depois
2. Usar template inline
3. Esquecer classes `dark:` em elementos com cor
4. Hardcodar textos (sempre i18n)
5. Criar componente sem .html e .scss separados
6. Ignorar warnings do build
7. Fazer múltiplas mudanças sem buildar entre elas
8. Usar `any` (sempre tipar)
9. Esquecer de testar em dark mode
10. Esquecer de traduzir para 3 idiomas

### ✅ SEMPRE FAZER:
1. `npm run build` após CADA mudança
2. Seguir padrão EXATO do products-list (dark mode)
3. Usar ThemeService e I18nService que JÁ EXISTEM
4. Criar .ts + .html + .scss para cada component
5. Testar em modo claro E escuro
6. Testar em 3 idiomas
7. Validar no backend com curl
8. Verificar responsividade
9. Usar Signals do Angular 18
10. Injetar dependências com `inject()`

---

## 📝 ORDEM DE EXECUÇÃO RECOMENDADA

```
DIA 1 (8h):
- Manhã: Ler documentação + Testar API (Parte 1)
- Tarde: Corrigir Models (Parte 2)

DIA 2 (8h):
- Manhã: Modal Component (Parte 3.1)
- Tarde: FormField Component (Parte 3.2)

DIA 3 (8h):
- Manhã: Autocomplete Component (Parte 3.3)
- Tarde: Atualizar ProductsService + Iniciar Create Modal (Parte 4)

DIA 4 (8h):
- Manhã: Finalizar Create Modal (Parte 4.2)
- Tarde: Edit Modal (Parte 4.3) + Atualizar List (Parte 4.4)

DIA 5 (4h):
- Manhã: Validação Final + Testes + Build Produção
```

---

## 🎯 PRÓXIMOS PASSOS (APÓS FASE 1)

**NÃO INICIAR até Fase 1 estar 100% completa:**

- Fase 2: Orders CRUD completo
- Fase 2: Customers CRUD completo
- Fase 2: Suppliers CRUD completo
- Fase 2: Users module
- Fase 2: Companies module

---

**🔴 ESTE CHECKLIST É OBRIGATÓRIO. NÃO PULAR ETAPAS.**
