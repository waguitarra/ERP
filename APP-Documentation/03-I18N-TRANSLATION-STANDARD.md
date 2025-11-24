# 🌍 PADRÃO OBRIGATÓRIO DE INTERNACIONALIZAÇÃO (i18n)

## 🎯 Regra Absoluta

**🔴 TODO componente DEVE ter suporte a 3 idiomas: Português (pt-BR), Inglês (en-US) e Espanhol (es-ES)**

---

## 📁 Estrutura de Arquivos

```
APP/
├── src/
│   ├── app/
│   │   └── core/
│   │       └── services/
│   │           └── i18n.service.ts      # Serviço de tradução
│   └── assets/
│       └── i18n/
│           ├── pt-BR.json               # Português Brasil
│           ├── en-US.json               # Inglês Americano
│           └── es-ES.json               # Espanhol
```

---

## 🛠️ Como Usar o Serviço i18n

### 1. Injetar o Serviço no Componente

```typescript
import { Component, inject } from '@angular/core';
import { I18nService } from '@core/services/i18n.service';

@Component({
  selector: 'app-products-list',
  standalone: true,
  // ...
})
export class ProductsListComponent {
  protected readonly i18n = inject(I18nService);
  
  // Usar no template ou no código
  title = this.i18n.t('products.title');
}
```

### 2. Usar no Template HTML

```html
<!-- Texto simples -->
<h1>{{ i18n.t('products.title') }}</h1>
<p>{{ i18n.t('products.subtitle') }}</p>

<!-- Com parâmetros -->
<span>{{ i18n.t('products.total', {count: products().length}) }}</span>

<!-- Botão -->
<button>{{ i18n.t('products.newProduct') }}</button>
```

---

## 📋 Estrutura dos Arquivos JSON

### Padrão de Chaves (Naming Convention)

```json
{
  "nomeDoModulo": {
    "title": "Título da Página",
    "subtitle": "Descrição da página",
    "newItem": "Novo Item",
    "searchPlaceholder": "Buscar...",
    "table": {
      "column1": "Coluna 1",
      "column2": "Coluna 2"
    },
    "empty": {
      "title": "Título vazio",
      "subtitle": "Descrição vazio",
      "button": "Ação"
    },
    "loading": "Carregando...",
    "stats": {
      "stat1": "Estatística 1"
    }
  }
}
```

### Chaves Comuns (Reutilizáveis)

```json
{
  "common": {
    "buttons": {
      "save": "...",
      "cancel": "...",
      "delete": "...",
      "edit": "..."
    },
    "status": {
      "active": "...",
      "inactive": "..."
    },
    "messages": {
      "loading": "...",
      "error": "...",
      "success": "..."
    }
  }
}
```

---

## ✅ Checklist Obrigatório para Novos Componentes

### Antes de criar um componente:

- [ ] Adicionar TODAS as strings em `pt-BR.json`
- [ ] Adicionar TODAS as strings em `en-US.json`
- [ ] Adicionar TODAS as strings em `es-ES.json`
- [ ] Injetar `I18nService` no componente
- [ ] Substituir TODOS os textos hardcoded por `i18n.t('chave')`
- [ ] Testar em todos os 3 idiomas

---

## 🔄 Mudança de Idioma

### No Header/Layout

```typescript
// header.component.ts
changeLanguage(lang: Language) {
  this.i18n.setLanguage(lang);
}
```

```html
<!-- header.component.html -->
<select (change)="changeLanguage($event.target.value)">
  <option value="pt-BR">🇧🇷 Português</option>
  <option value="en-US">🇺🇸 English</option>
  <option value="es-ES">🇪🇸 Español</option>
</select>
```

---

## 📝 Exemplos Práticos

### Componente Products-List

**TypeScript:**
```typescript
export class ProductsListComponent {
  protected readonly i18n = inject(I18nService);
}
```

**HTML:**
```html
<h1>{{ i18n.t('products.title') }}</h1>
<p>{{ i18n.t('products.subtitle') }}</p>
<button>{{ i18n.t('products.newProduct') }}</button>
<input [placeholder]="i18n.t('products.searchPlaceholder')">

<table>
  <thead>
    <th>{{ i18n.t('products.table.sku') }}</th>
    <th>{{ i18n.t('products.table.product') }}</th>
  </thead>
</table>

<!-- Empty State -->
<h3>{{ i18n.t('products.empty.title') }}</h3>
<p>{{ i18n.t('products.empty.subtitle') }}</p>
<button>{{ i18n.t('products.empty.button') }}</button>
```

---

## 🚫 O QUE NÃO FAZER

### ❌ NUNCA:

1. **Hardcodar texto no HTML:**
   ```html
   <!-- ERRADO -->
   <h1>Produtos</h1>
   
   <!-- CORRETO -->
   <h1>{{ i18n.t('products.title') }}</h1>
   ```

2. **Esquecer de adicionar em todos os 3 idiomas**

3. **Usar textos diferentes entre idiomas (manter estrutura)**

4. **Criar chaves sem seguir a convenção de nomenclatura**

---

## 🎨 Integração com Dark Mode

**Ambos são obrigatórios:**
- ✅ Dark Mode (seguir padrão do products-list)
- ✅ i18n (3 idiomas: pt-BR, en-US, es-ES)

---

## 📊 Status de Implementação

### Componentes com i18n:
- [ ] Login
- [ ] Dashboard
- [ ] Products-list
- [ ] Customers-list
- [ ] Orders-list
- [ ] Inventory-list
- [ ] Suppliers-list
- [ ] Warehouses-list
- [ ] Drivers-list
- [ ] Vehicles-list
- [ ] Picking-tasks-list
- [ ] Packing-tasks-list
- [ ] Inbound-shipments-list
- [ ] Outbound-shipments-list
- [ ] Main-layout (sidebar/header)

---

**🔴 ESTA REGRA É ABSOLUTA E DEVE SER SEGUIDA EM TODO NOVO COMPONENTE**
