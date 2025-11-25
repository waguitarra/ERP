# 📦 COMPONENTES MODAIS SELETORES REUTILIZÁVEIS

**Data**: 2025-11-25
**Status**: ✅ IMPLEMENTADO E FUNCIONANDO

---

## 🎯 OBJETIVO

Criar componentes **separados e específicos** para seleção de entidades WMS, que podem ser **reutilizados em qualquer lugar** do sistema.

Cada modal é independente, bonito, com busca em tempo real e exibe **TODAS as informações** da entidade para o usuário fazer uma seleção informada.

---

## 📦 COMPONENTES CRIADOS

### 1. 🚗 VehicleSelectorModalComponent

**Localização**: `APP/src/app/shared/components/vehicle-selector-modal/`

**Responsabilidade**: Seleção de veículos

**Informações Exibidas**:
- ✅ Placa (grande, destaque)
- ✅ Modelo
- ✅ Ano
- ✅ Cor
- ✅ Capacidade
- ✅ Status (Ativo/Inativo) - badge verde/vermelho

**Funcionalidades**:
- 🔍 Busca em tempo real por: placa, modelo, cor, ano
- 📋 Grid 2 colunas responsivo
- ✨ Highlight azul no item selecionado
- 🎨 Header azul com ícone
- 📱 Totalmente responsivo

**Como Usar**:
```typescript
// No component.ts
import { VehicleSelectorModalComponent } from '@shared/components/vehicle-selector-modal/vehicle-selector-modal.component';

@Component({
  imports: [VehicleSelectorModalComponent]
})
export class MyComponent {
  vehicleModal = viewChild<VehicleSelectorModalComponent>('vehicleModal');
  selectedVehicle = signal<Vehicle | null>(null);

  openVehicleSelector(): void {
    this.vehicleModal()?.open();
  }

  onVehicleSelected(vehicle: Vehicle): void {
    console.log('Veículo selecionado:', vehicle);
    this.selectedVehicle.set(vehicle);
    // Fazer algo com o veículo...
  }
}
```

```html
<!-- No template.html -->
<button (click)="openVehicleSelector()">Selecionar Veículo</button>

<app-vehicle-selector-modal 
  #vehicleModal 
  (vehicleSelected)="onVehicleSelected($event)">
</app-vehicle-selector-modal>
```

---

### 2. 👨‍✈️ DriverSelectorModalComponent

**Localização**: `APP/src/app/shared/components/driver-selector-modal/`

**Responsabilidade**: Seleção de motoristas

**Informações Exibidas**:
- ✅ Nome (grande, destaque)
- ✅ CNH (monospace)
- ✅ Telefone (com ícone)
- ✅ Email (com ícone)
- ✅ Status (Ativo/Inativo) - badge verde/vermelho

**Funcionalidades**:
- 🔍 Busca em tempo real por: nome, CNH, telefone, email
- 📋 Grid 2 colunas responsivo
- ✨ Highlight verde no item selecionado
- 🎨 Header verde com ícone de pessoa
- 📱 Totalmente responsivo

**Como Usar**:
```typescript
// No component.ts
import { DriverSelectorModalComponent } from '@shared/components/driver-selector-modal/driver-selector-modal.component';

@Component({
  imports: [DriverSelectorModalComponent]
})
export class MyComponent {
  driverModal = viewChild<DriverSelectorModalComponent>('driverModal');
  selectedDriver = signal<Driver | null>(null);

  openDriverSelector(): void {
    this.driverModal()?.open();
  }

  onDriverSelected(driver: Driver): void {
    console.log('Motorista selecionado:', driver);
    this.selectedDriver.set(driver);
  }
}
```

```html
<button (click)="openDriverSelector()">Selecionar Motorista</button>

<app-driver-selector-modal 
  #driverModal 
  (driverSelected)="onDriverSelected($event)">
</app-driver-selector-modal>
```

---

### 3. 🏭 WarehouseSelectorModalComponent

**Localização**: `APP/src/app/shared/components/warehouse-selector-modal/`

**Responsabilidade**: Seleção de armazéns (origem/destino)

**Informações Exibidas**:
- ✅ Código (grande, destaque)
- ✅ Nome
- ✅ Cidade
- ✅ Estado
- ✅ CEP
- ✅ Status (Ativo/Inativo) - badge verde/vermelho

**Funcionalidades**:
- 🔍 Busca em tempo real por: código, nome, cidade, estado
- 📋 Grid 2 colunas responsivo
- ✨ Highlight roxo no item selecionado
- 🎨 Header roxo com ícone de prédio
- 📱 Totalmente responsivo
- 🎯 **Título customizável** (ex: "Armazém de Origem" ou "Armazém de Destino")

**Como Usar**:
```typescript
// No component.ts
import { WarehouseSelectorModalComponent } from '@shared/components/warehouse-selector-modal/warehouse-selector-modal.component';

@Component({
  imports: [WarehouseSelectorModalComponent]
})
export class MyComponent {
  warehouseModal = viewChild<WarehouseSelectorModalComponent>('warehouseModal');
  selectedWarehouse = signal<Warehouse | null>(null);

  openWarehouseSelector(): void {
    this.warehouseModal()?.open();
  }

  onWarehouseSelected(warehouse: Warehouse): void {
    console.log('Armazém selecionado:', warehouse);
    this.selectedWarehouse.set(warehouse);
  }
}
```

```html
<button (click)="openWarehouseSelector()">Selecionar Armazém</button>

<!-- Título customizável via input -->
<app-warehouse-selector-modal 
  #warehouseModal 
  [title]="'Selecionar Armazém de Origem'"
  (warehouseSelected)="onWarehouseSelected($event)">
</app-warehouse-selector-modal>
```

---

## 🎨 DESIGN PATTERN

### Cores e Temas

Cada modal tem sua própria cor para diferenciar visualmente:

| Componente | Cor Header | Cor Highlight | Razão |
|------------|-----------|---------------|-------|
| Vehicle | Azul (`blue-600`) | Azul | Veículos = transporte |
| Driver | Verde (`green-600`) | Verde | Pessoas = natureza |
| Warehouse | Roxo (`purple-600`) | Roxo | Lugares = realeza |

### Estrutura Visual

```
┌─────────────────────────────────────────┐
│  [ÍCONE] TÍTULO             [X]         │  ← Header colorido
├─────────────────────────────────────────┤
│  🔍 [Campo de Busca]                    │  ← Busca
├─────────────────────────────────────────┤
│  ┌──────────────┐  ┌──────────────┐    │
│  │  ITEM 1      │  │  ITEM 2      │    │  ← Grid 2 cols
│  │  Info 1      │  │  Info 1      │    │
│  │  Info 2      │  │  Info 2      │    │
│  └──────────────┘  └──────────────┘    │
│                                         │
│  ┌──────────────┐  ┌──────────────┐    │
│  │  ITEM 3 ✓    │  │  ITEM 4      │    │  ← Selecionado
│  └──────────────┘  └──────────────┘    │
├─────────────────────────────────────────┤
│              [Cancelar] [✓ Confirmar]   │  ← Footer
└─────────────────────────────────────────┘
```

### Estados Visuais

1. **Loading**: Spinner animado + texto "Carregando..."
2. **Empty**: Ícone grande + "Nenhum resultado encontrado"
3. **Hover**: Border muda de cor + shadow
4. **Selecionado**: Border colorida + background suave + highlight

---

## 🔌 INTEGRAÇÃO TÉCNICA

### Arquitetura

```typescript
Component
├── VehiclesService.getAll() ─────> Backend API
├── Signal<Vehicle[]>
├── Computed filteredVehicles
├── Signal<Vehicle | null> (selected)
└── Output vehicleSelected
```

### Fluxo de Dados

```mermaid
Usuario clica botão
    ↓
Component chama modal.open()
    ↓
Modal carrega dados da API
    ↓
Usuario pesquisa/filtra
    ↓
Usuario clica em item
    ↓
Signal selectedItem atualizado
    ↓
Usuario clica "Confirmar"
    ↓
Output emit(selectedItem)
    ↓
Component pai recebe item
    ↓
Atualiza formulário/estado
```

---

## ✅ VANTAGENS DESSA ARQUITETURA

### 1. **Reutilizável em Qualquer Lugar**
```typescript
// Pode usar em Orders
import { VehicleSelectorModalComponent } from '@shared/...';

// Pode usar em Shipments
import { VehicleSelectorModalComponent } from '@shared/...';

// Pode usar em Deliveries
import { VehicleSelectorModalComponent } from '@shared/...';
```

### 2. **Independente**
- Cada modal busca seus próprios dados
- Não depende de props/inputs complexos
- Gerencia seu próprio estado

### 3. **Consistente**
- Mesmo UX em todo o sistema
- Mesmas funcionalidades (busca, loading, empty state)
- Mesmo padrão visual

### 4. **Manutenível**
- Um bug? Corrige em 1 lugar, afeta todos os usos
- Nova feature? Adiciona em 1 lugar, todos ganham
- Refactor? Componente isolado, sem quebrar outros

### 5. **Escalável**
- Fácil criar novos modais seguindo o pattern
- Exemplo: `ProductSelectorModalComponent`, `SupplierSelectorModalComponent`, etc.

---

## 📊 ONDE ESTÁ SENDO USADO

### 1. Order Edit Modal
**Arquivo**: `APP/src/app/features/orders/order-edit-modal/`

**Uso**:
```typescript
// 4 modais diferentes
vehicleModal = viewChild<VehicleSelectorModalComponent>('vehicleModal');
driverModal = viewChild<DriverSelectorModalComponent>('driverModal');
originWarehouseModal = viewChild<WarehouseSelectorModalComponent>('originModal');
destinationWarehouseModal = viewChild<WarehouseSelectorModalComponent>('destModal');
```

**Resultado**: Usuario seleciona veículo, motorista, armazém origem e destino ao editar pedido.

---

## 🚀 FUTURAS EXPANSÕES

### Novos Modais Seguindo o Padrão

1. **ProductSelectorModalComponent**
   - Seleção de produtos
   - Info: SKU, Nome, Categoria, Preço, Estoque

2. **SupplierSelectorModalComponent**
   - Seleção de fornecedores
   - Info: CNPJ, Nome, Telefone, Email

3. **CustomerSelectorModalComponent**
   - Seleção de clientes
   - Info: CPF/CNPJ, Nome, Telefone, Cidade

4. **LocationSelectorModalComponent**
   - Seleção de localizações no armazém
   - Info: Corredor, Prateleira, Nível

---

## 📝 CHECKLIST PARA CRIAR NOVO MODAL

```markdown
- [ ] Criar pasta em `shared/components/[nome]-selector-modal/`
- [ ] Criar component.ts com:
  - [ ] Service inject
  - [ ] Signal para lista
  - [ ] Signal para searchTerm
  - [ ] Signal para selectedItem
  - [ ] Computed filteredItems
  - [ ] Output itemSelected
  - [ ] Métodos: open(), close(), selectItem(), confirm()
- [ ] Criar template inline ou .html com:
  - [ ] Header colorido com ícone
  - [ ] Campo de busca
  - [ ] Loading state
  - [ ] Empty state
  - [ ] Grid de items
  - [ ] Footer com botões
- [ ] Testar:
  - [ ] Open/Close
  - [ ] Busca funciona
  - [ ] Seleção funciona
  - [ ] Output emite corretamente
  - [ ] Dark mode
  - [ ] Responsividade
```

---

## 🎉 RESULTADO FINAL

✅ **3 componentes separados e independentes**
✅ **Cada um com sua cor e identidade visual**
✅ **Busca em tempo real funcionando**
✅ **Exibe TODAS informações da entidade**
✅ **Reutilizável em qualquer lugar do sistema**
✅ **Dark mode completo**
✅ **Loading e empty states**
✅ **Build sem erros**

---

**Sistema escalável e profissional!** 🚀
