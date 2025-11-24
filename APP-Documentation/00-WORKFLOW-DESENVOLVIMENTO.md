# 🔧 WORKFLOW DE DESENVOLVIMENTO - NEXUS ADMIN

## ⚠️ REGRAS ABSOLUTAS

### 1. **BUILD OBRIGATÓRIO**

**SEMPRE EXECUTE `npm run build` ANTES E DEPOIS DE QUALQUER MUDANÇA**

```bash
# WORKFLOW CORRETO:
1. npm run build              # ✅ Build inicial
2. [fazer alterações no código]
3. npm run build              # ✅ Build de validação
4. [verificar erros]
5. [corrigir SE necessário]
6. npm run build              # ✅ Build final
```

**❌ NUNCA:**
- Fazer múltiplas alterações sem buildar
- Assumir que o código está correto
- Ignorar warnings do build
- Fazer push sem build limpo

---

## 📋 CHECKLIST PRÉ-COMMIT

**Execute TODOS os passos antes de commitar:**

```bash
# 1. Build limpo
npm run build

# 2. Verificar estrutura de arquivos
find src/app -name "*.component.ts" -exec grep -l "template:" {} \;
# ⚠️ Resultado DEVE ser vazio (sem componentes com template inline)

# 3. Verificar rotas desnecessárias
find src/app/features -name "*.routes.ts"
# ⚠️ Resultado DEVE ser vazio

# 4. Verificar que TODO componente tem HTML separado
find src/app -name "*.component.ts" | wc -l
find src/app -name "*.component.html" | wc -l
# ⚠️ Números devem ser IGUAIS

# 5. Verificar imports
grep -r "from '@angular" src/app --include="*.ts" | grep -v "node_modules"
# ✅ Não deve ter erros
```

---

## 🚫 ERROS COMUNS E COMO EVITAR

### ❌ ERRO 1: HTML Inline

**NUNCA faça:**
```typescript
@Component({
  template: `<div>HTML aqui</div>`,  // ❌ PROIBIDO
  styles: []
})
```

**SEMPRE faça:**
```typescript
@Component({
  templateUrl: './component.component.html',  // ✅ CORRETO
  styleUrls: ['./component.component.scss']
})
```

**Como criar componente corretamente:**
```bash
# 1. Criar arquivo .ts
touch src/app/layout/exemplo/exemplo.component.ts

# 2. Criar arquivo .html
touch src/app/layout/exemplo/exemplo.component.html

# 3. Criar arquivo .scss
touch src/app/layout/exemplo/exemplo.component.scss

# 4. NO .ts, sempre usar templateUrl e styleUrls
```

---

### ❌ ERRO 2: Arquivos .routes.ts Desnecessários

**NUNCA crie:**
```
features/
  └── products/
      └── products.routes.ts  ❌ DESNECESSÁRIO
```

**SE tiver apenas 1 componente, use direto em app.routes.ts:**
```typescript
// app.routes.ts
{
  path: 'products',
  loadComponent: () => import('./features/products/products-list/...')
}
```

**QUANDO criar .routes.ts:**
- ✅ Quando módulo tem múltiplas rotas (CRUD completo)
- ✅ Quando tem rotas aninhadas complexas
- ❌ NUNCA para módulo com apenas lista

---

### ❌ ERRO 3: Não Buildar Entre Mudanças

**CENÁRIO ERRADO:**
```
1. Alterar sidebar.component.ts
2. Alterar header.component.ts
3. Alterar main-layout.component.ts
4. Alterar 10 outros arquivos
5. npm run build  ❌ TARDE DEMAIS - 50 erros
```

**CENÁRIO CORRETO:**
```
1. Alterar sidebar.component.ts
2. npm run build  ✅
3. Alterar header.component.ts
4. npm run build  ✅
5. Continuar...
```

---

## 🎯 PADRÃO DE COMPONENTE

**Template EXATO para criar componente:**

### 1. **Arquivo TypeScript (.ts)**
```typescript
import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-exemplo',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './exemplo.component.html',      // ✅ OBRIGATÓRIO
  styleUrls: ['./exemplo.component.scss']       // ✅ OBRIGATÓRIO
})
export class ExemploComponent {
  private readonly service = inject(ExemploService);
  
  loading = signal<boolean>(false);
  data = signal<any[]>([]);
  
  async loadData(): Promise<void> {
    this.loading.set(true);
    try {
      const result = await this.service.getData();
      this.data.set(result);
    } catch (error) {
      console.error('Erro:', error);
    } finally {
      this.loading.set(false);
    }
  }
}
```

### 2. **Arquivo HTML (.html)**
```html
<div class="space-y-6">
  <div class="flex items-center justify-between">
    <h1 class="text-2xl font-bold text-slate-800 dark:text-slate-100">
      {{ i18n.t('exemplo.title') }}
    </h1>
    <button class="px-4 py-2 bg-blue-600 text-white rounded-lg">
      {{ i18n.t('common.buttons.new') }}
    </button>
  </div>

  @if (loading()) {
    <p>{{ i18n.t('common.loading') }}</p>
  } @else {
    <div><!-- conteúdo --></div>
  }
</div>
```

### 3. **Arquivo SCSS (.scss)**
```scss
// Pode ficar vazio se usar apenas Tailwind
// OU adicionar estilos específicos do componente
```

---

## 📁 ESTRUTURA DE ARQUIVOS

**Para CADA componente, SEMPRE ter 3 arquivos:**

```
exemplo/
├── exemplo.component.ts      ✅ TypeScript
├── exemplo.component.html    ✅ Template
└── exemplo.component.scss    ✅ Estilos (pode ser vazio)
```

**NUNCA:**
```
exemplo/
└── exemplo.component.ts      ❌ Faltando HTML e SCSS
```

---

## 🔍 COMANDOS DE VERIFICAÇÃO

**Execute regularmente:**

```bash
# Encontrar componentes com HTML inline
grep -r "template:" src/app --include="*.ts"

# Encontrar estilos inline
grep -r "styles:" src/app --include="*.ts"

# Verificar arquivos órfãos
find src/app -name "*.component.ts" -type f -exec sh -c 'f="{}"; html="${f%.ts}.html"; scss="${f%.ts}.scss"; [ ! -f "$html" ] && echo "FALTA HTML: $f"; [ ! -f "$scss" ] && echo "FALTA SCSS: $f"' \;

# Build de produção
npm run build -- --configuration production
```

---

## 🚀 PROCESSO DE DESENVOLVIMENTO

### 1. **Antes de Começar**
```bash
git pull origin main
npm install
npm run build
```

### 2. **Durante Desenvolvimento**
```bash
# Para CADA feature/fix:
1. Criar/editar arquivos
2. npm run build
3. Verificar erros
4. Corrigir SE necessário
5. npm run build novamente
```

### 3. **Antes de Commit**
```bash
npm run build                    # Build final
npm run build -- --prod          # Build de produção
git add .
git commit -m "mensagem"
```

### 4. **Antes de Push**
```bash
npm run build -- --prod          # Garantir build limpo
git push origin branch
```

---

## ⚡ ATALHOS ÚTEIS

```bash
# Alias para adicionar no .bashrc ou .zshrc:
alias ng-build='npm run build'
alias ng-check='npm run build && echo "✅ BUILD OK"'
alias ng-clean='rm -rf dist node_modules && npm install && npm run build'

# Função para criar componente completo:
ng-component() {
  mkdir -p $1
  touch $1/$1.component.ts
  touch $1/$1.component.html
  touch $1/$1.component.scss
  echo "✅ Componente criado: $1"
}
```

---

## 📊 MÉTRICAS DE QUALIDADE

**Projeto em estado ideal:**

```
✅ Build time: < 5 segundos
✅ 0 erros de compilação
✅ 0 warnings críticos
✅ 0 componentes com template inline
✅ 0 arquivos .routes.ts desnecessários
✅ 100% componentes com templateUrl
✅ Todos arquivos .ts tem .html e .scss correspondente
```

---

## 🎓 RESUMO FINAL

**3 REGRAS DE OURO:**

1. **BUILD SEMPRE** - Antes e depois de cada mudança
2. **NUNCA HTML INLINE** - Sempre templateUrl + arquivo .html
3. **VALIDAR ANTES DE COMMIT** - Build + Checklist completo

**SE QUEBRAR ESSAS REGRAS:**
- ❌ Build vai falhar
- ❌ Code review vai rejeitar
- ❌ Deploy vai quebrar
- ❌ Time vai ficar puto

---

**ÚLTIMA LINHA DE DEFESA:**

```bash
# Antes de QUALQUER commit:
npm run build && echo "✅ PODE COMMITAR" || echo "❌ TEM ERRO, CORRIGE"
```

---

**Este documento é uma ordem, não uma sugestão.**
