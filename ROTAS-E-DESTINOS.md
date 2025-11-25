# 🗺️ SISTEMA DE ROTAS E DESTINOS - IMPLEMENTAÇÃO COMPLETA

**Data**: 2025-11-25
**Status**: ✅ IMPLEMENTADO

---

## 🎯 REGRAS DE NEGÓCIO

### Fluxos Permitidos

1. **Armazém → Cliente** (Entrega)
   - Origem: SEMPRE um armazém
   - Destino: Cliente final
   - Exemplo: Armazém Madrid → Cliente em Barcelona

2. **Armazém → Armazém** (Transferência)
   - Origem: Armazém de origem
   - Destino: Armazém de destino
   - Exemplo: Armazém Madrid → Armazém Barcelona

### Fluxos NÃO Permitidos

❌ **Cliente → Armazém** (não faz sentido no modelo)
❌ **Cliente → Cliente** (não aplicável)

---

## 📊 ENTIDADES MODIFICADAS

### 1. Warehouse (Armazém)

**Novos Campos**:
```csharp
public string? City { get; private set; }
public string? State { get; private set; }
public string? ZipCode { get; private set; }
public string? Country { get; private set; }
public double? Latitude { get; private set; }
public double? Longitude { get; private set; }
```

**Novos Métodos**:
```csharp
public void Update(string name, string code, string? address, 
    string? city = null, string? state = null, string? zipCode = null, string? country = null)

public void SetGeolocation(double latitude, double longitude)
```

### 2. Customer (Cliente)

**Novos Campos**:
```csharp
public string? City { get; private set; }
public string? State { get; private set; }
public string? ZipCode { get; private set; }
public string? Country { get; private set; }
public double? Latitude { get; private set; }
public double? Longitude { get; private set; }
```

**Novos Métodos**:
```csharp
public void Update(string name, string document, string? phone, string? email, string? address,
    string? city = null, string? state = null, string? zipCode = null, string? country = null)

public void SetGeolocation(double latitude, double longitude)
```

---

## 🔧 MIGRATION EF CORE

**Nome**: `AddAddressFieldsToWarehouseAndCustomer`

**Arquivo**: `API/src/Logistics.Infrastructure/Migrations/[timestamp]_AddAddressFieldsToWarehouseAndCustomer.cs`

**Colunas Adicionadas**:
- `City` (string nullable)
- `State` (string nullable)
- `ZipCode` (string nullable)
- `Country` (string nullable)
- `Latitude` (double nullable)
- `Longitude` (double nullable)

**Status**: ✅ Aplicada com sucesso

**Comando usado**:
```bash
dotnet ef migrations add AddAddressFieldsToWarehouseAndCustomer
dotnet ef database update
```

---

## 📝 SCRIPT SQL DE DADOS FICTÍCIOS

**Arquivo**: `API/scripts/populate-spain-addresses.sql`

### Endereços na Espanha

#### Armazéns
- **Armazém WH-001** (Madrid):
  - Endereço: Calle de Alcalá, 123
  - CEP: 28009
  - Coordenadas: 40.4168, -3.7038

- **Armazém WH-002** (Barcelona):
  - Endereço: Carrer de Provença, 456
  - CEP: 08025
  - Coordenadas: 41.3851, 2.1734

- Demais armazéns: Valencia, CEP 46001, Coordenadas 39.4699, -0.3763

#### Clientes
- Endereços aleatórios em:
  - Madrid (CEP 280xx)
  - Barcelona (CEP 080xx)
  - Valencia (CEP 460xx)
  - Sevilla (CEP 410xx)

- Ruas fictícias:
  - Gran Vía
  - Paseo de Gracia
  - Calle Mayor

**Como executar**:
```bash
psql -U postgres -d logistics -f API/scripts/populate-spain-addresses.sql
```

---

## 🎨 FRONTEND - ORDER EDIT MODAL

### Modificações no Componente

**Arquivo**: `APP/src/app/features/orders/order-edit-modal/`

#### 1. Novo Signal: Tipo de Destino
```typescript
destinationType = signal<'warehouse' | 'customer'>('customer');
```

#### 2. Novos Signals de Seleção
```typescript
selectedCustomer = signal<Customer | null>(null);
selectedDestinationWarehouse = signal<Warehouse | null>(null);
```

#### 3. Novo Campo no Formulário
```typescript
customerId: ['']
```

#### 4. Novo Método: Alternar Tipo de Destino
```typescript
setDestinationType(type: 'warehouse' | 'customer'): void {
  this.destinationType.set(type);
  // Limpar seleção anterior
  if (type === 'customer') {
    this.selectedDestinationWarehouse.set(null);
    this.form.patchValue({ destinationWarehouseId: null });
  } else {
    this.selectedCustomer.set(null);
    this.form.patchValue({ customerId: null });
  }
}
```

#### 5. Novo Método: Seleção de Cliente
```typescript
onCustomerSelected(customer: Customer): void {
  this.form.patchValue({ customerId: customer.id });
  this.selectedCustomer.set(customer);
}
```

### UI/UX

#### Radio Buttons: Tipo de Destino
```html
<div class="col-span-2">
  <label>Tipo de Destino</label>
  <div class="flex gap-4">
    <label class="flex items-center cursor-pointer">
      <input type="radio" name="destinationType" value="customer" 
             (change)="setDestinationType('customer')" 
             [checked]="destinationType() === 'customer'">
      <span>Cliente</span>
    </label>
    <label class="flex items-center cursor-pointer">
      <input type="radio" name="destinationType" value="warehouse"
             (change)="setDestinationType('warehouse')"
             [checked]="destinationType() === 'warehouse'">
      <span>Armazém (Transferência)</span>
    </label>
  </div>
</div>
```

#### Condicional: Mostrar Campo Cliente OU Armazém
```html
@if (destinationType() === 'customer') {
  <div class="col-span-2">
    <button (click)="openCustomerSelector()">
      Selecionar cliente...
    </button>
  </div>
}

@if (destinationType() === 'warehouse') {
  <div class="col-span-2">
    <button (click)="openDestinationWarehouseSelector()">
      Selecionar armazém...
    </button>
  </div>
}
```

#### Modal de Cliente Integrado
```html
<app-customer-selector-modal 
  #customerModal 
  (customerSelected)="onCustomerSelected($event)">
</app-customer-selector-modal>
```

---

## 🔄 FLUXO DE USO

### Cenário 1: Entrega para Cliente

1. Usuário abre "Editar Pedido"
2. Seleciona **Origem**: Armazém (ex: Madrid)
3. Marca radio button **"Cliente"**
4. Clica em "Selecionar cliente..."
5. Modal abre com lista de clientes (com endereços completos)
6. Seleciona cliente (ex: João Silva - Barcelona)
7. Sistema preenche `customerId` no formulário
8. Salva pedido com:
   - `originWarehouseId`: [ID Madrid]
   - `customerId`: [ID João]
   - `destinationWarehouseId`: null

### Cenário 2: Transferência entre Armazéns

1. Usuário abre "Editar Pedido"
2. Seleciona **Origem**: Armazém Madrid
3. Marca radio button **"Armazém (Transferência)"**
4. Clica em "Selecionar armazém..."
5. Modal abre com lista de armazéns (exceto origem)
6. Seleciona armazém destino (ex: Barcelona)
7. Sistema preenche `destinationWarehouseId`
8. Salva pedido com:
   - `originWarehouseId`: [ID Madrid]
   - `destinationWarehouseId`: [ID Barcelona]
   - `customerId`: null

---

## 🗺️ PRÓXIMOS PASSOS (Cálculo de Rotas)

### Integração com API de Mapas

Para calcular distância e traçar rota entre origem e destino:

1. **Obter coordenadas**:
   - Origem: `originWarehouse.Latitude`, `originWarehouse.Longitude`
   - Destino (Cliente): `customer.Latitude`, `customer.Longitude`
   - Destino (Armazém): `destinationWarehouse.Latitude`, `destinationWarehouse.Longitude`

2. **API sugerida**: Google Maps Distance Matrix API
   ```typescript
   async calculateRoute(
     originLat: number, originLng: number,
     destLat: number, destLng: number
   ): Promise<RouteInfo> {
     const response = await fetch(
       `https://maps.googleapis.com/maps/api/distancematrix/json?
        origins=${originLat},${originLng}&
        destinations=${destLat},${destLng}&
        key=${API_KEY}`
     );
     return response.json();
   }
   ```

3. **Dados retornados**:
   - Distância em km
   - Tempo estimado
   - Rota visual (polyline)

4. **Armazenar no Order**:
   - Adicionar campos: `DistanceKm`, `EstimatedDurationMinutes`, `RoutePolyline`

---

## ✅ STATUS FINAL

- ✅ Migration criada e aplicada
- ✅ Campos de endereço em Warehouse e Customer
- ✅ Script SQL para dados fictícios na Espanha
- ✅ Tipo de destino (Cliente/Armazém) no frontend
- ✅ Modal de seleção de cliente integrado
- ✅ Lógica condicional funcionando
- ✅ Build sem erros
- ✅ Frontend compilado e pronto

**Sistema pronto para calcular rotas com endereços reais!** 🚀
