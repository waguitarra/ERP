# 🚚 PLANO COMPLETO - MÓDULO ORDERS WMS PROFISSIONAL

**Data**: 2025-11-25  
**Objetivo**: Transformar Orders em módulo WMS completo com rastreabilidade total

---

## 📋 ANÁLISE DA SITUAÇÃO ATUAL

### ❌ Problemas Identificados

1. **Status e Priority hardcoded no frontend**
   - Enums definidos apenas no código
   - Sem suporte a i18n (PT/EN/ES)
   - Não vem da base de dados

2. **Falta de relacionamentos WMS críticos**
   - Sem Vehicle (caminhão)
   - Sem Driver (motorista)
   - Sem Warehouse origem/destino
   - Sem rastreamento de localização

3. **Textos estáticos sem i18n**
   - Todos labels em português fixo
   - Não usa JSON de traduções
   - Não suporta PT/EN/ES

4. **Sem integração com mapas**
   - ShippingAddress é texto simples
   - Sem CEP/coordenadas
   - Sem visualização em mapa
   - Sem iframe Google Maps

5. **Falta de contexto logístico**
   - Não mostra de onde sai o pedido
   - Não mostra para onde vai
   - Não mostra quem entrega
   - Não mostra qual veículo

---

## 🎯 OBJETIVOS DO PLANO

### 1. Backend - Base de Dados
- ✅ Criar tabela `OrderStatuses` com i18n (PT/EN/ES)
- ✅ Criar tabela `OrderPriorities` com i18n (PT/EN/ES)
- ✅ Adicionar campos WMS em `Orders`:
  - `VehicleId` (Guid?)
  - `DriverId` (Guid?)
  - `OriginWarehouseId` (Guid?)
  - `DestinationWarehouseId` (Guid?)
  - `ShippingZipCode` (string?)
  - `ShippingLatitude` (decimal?)
  - `ShippingLongitude` (decimal?)
  - `EstimatedDeliveryDate` (DateTime?)
  - `ActualDeliveryDate` (DateTime?)
  - `TrackingNumber` (string?)

### 2. Backend - Endpoints
- ✅ GET `/api/orderstatus` - Lista todos status com i18n
- ✅ GET `/api/orderpriority` - Lista todas prioridades com i18n
- ✅ PUT `/api/orders/{id}/assign-vehicle` - Atribui veículo
- ✅ PUT `/api/orders/{id}/assign-driver` - Atribui motorista
- ✅ GET `/api/orders/{id}/tracking` - Rastreamento completo

### 3. Frontend - i18n Completo
- ✅ Criar `pt.json`, `en.json`, `es.json`
- ✅ Remover TODOS textos estáticos
- ✅ Implementar seletor de idioma
- ✅ Status e Priority vindo da API

### 4. Frontend - Google Maps
- ✅ Integração Google Maps API
- ✅ Autocomplete de endereço
- ✅ Visualização em mapa (iframe)
- ✅ Geocoding (endereço → lat/lng)
- ✅ Busca por CEP (ViaCEP API)

### 5. Frontend - Relacionamentos WMS
- ✅ Select de Vehicle (busca da API)
- ✅ Select de Driver (busca da API)
- ✅ Select de Warehouse origem
- ✅ Select de Warehouse destino
- ✅ Mostrar informações completas

---

## 🗄️ PARTE 1: BACKEND - MIGRAÇÕES

### 1.1 Criar Tabela OrderStatuses

```csharp
// Logistics.Domain/Entities/OrderStatus.cs
public class OrderStatus
{
    public int Id { get; private set; }
    public string Code { get; private set; }          // DRAFT, PENDING, CONFIRMED, etc.
    public string NamePT { get; private set; }        // Rascunho
    public string NameEN { get; private set; }        // Draft
    public string NameES { get; private set; }        // Borrador
    public string? DescriptionPT { get; private set; }
    public string? DescriptionEN { get; private set; }
    public string? DescriptionES { get; private set; }
    public string ColorHex { get; private set; }      // #6B7280 para UI
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    
    private OrderStatus() { }
    
    public OrderStatus(int id, string code, string namePT, string nameEN, string nameES, string colorHex, int sortOrder)
    {
        Id = id;
        Code = code;
        NamePT = namePT;
        NameEN = nameEN;
        NameES = nameES;
        ColorHex = colorHex;
        SortOrder = sortOrder;
        IsActive = true;
    }
}
```

**Dados Iniciais (Seed)**:
```csharp
new OrderStatus(0, "DRAFT", "Rascunho", "Draft", "Borrador", "#6B7280", 0),
new OrderStatus(1, "PENDING", "Pendente", "Pending", "Pendiente", "#F59E0B", 1),
new OrderStatus(2, "CONFIRMED", "Confirmado", "Confirmed", "Confirmado", "#3B82F6", 2),
new OrderStatus(3, "IN_PROGRESS", "Em Andamento", "In Progress", "En Progreso", "#8B5CF6", 3),
new OrderStatus(4, "PARTIALLY_FULFILLED", "Parcialmente Atendido", "Partially Fulfilled", "Parcialmente Cumplido", "#F59E0B", 4),
new OrderStatus(5, "FULFILLED", "Atendido", "Fulfilled", "Cumplido", "#10B981", 5),
new OrderStatus(6, "SHIPPED", "Enviado", "Shipped", "Enviado", "#06B6D4", 6),
new OrderStatus(7, "DELIVERED", "Entregue", "Delivered", "Entregado", "#22C55E", 7),
new OrderStatus(8, "CANCELLED", "Cancelado", "Cancelled", "Cancelado", "#EF4444", 8),
new OrderStatus(9, "ON_HOLD", "Em Espera", "On Hold", "En Espera", "#F97316", 9)
```

### 1.2 Criar Tabela OrderPriorities

```csharp
// Logistics.Domain/Entities/OrderPriority.cs
public class OrderPriority
{
    public int Id { get; private set; }
    public string Code { get; private set; }          // LOW, NORMAL, HIGH, URGENT
    public string NamePT { get; private set; }
    public string NameEN { get; private set; }
    public string NameES { get; private set; }
    public string ColorHex { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    
    private OrderPriority() { }
    
    public OrderPriority(int id, string code, string namePT, string nameEN, string nameES, string colorHex, int sortOrder)
    {
        Id = id;
        Code = code;
        NamePT = namePT;
        NameEN = nameEN;
        NameES = nameES;
        ColorHex = colorHex;
        SortOrder = sortOrder;
        IsActive = true;
    }
}
```

**Dados Iniciais (Seed)**:
```csharp
new OrderPriority(0, "LOW", "Baixa", "Low", "Baja", "#6B7280", 0),
new OrderPriority(1, "NORMAL", "Normal", "Normal", "Normal", "#3B82F6", 1),
new OrderPriority(2, "HIGH", "Alta", "High", "Alta", "#F59E0B", 2),
new OrderPriority(3, "URGENT", "Urgente", "Urgent", "Urgente", "#EF4444", 3)
```

### 1.3 Atualizar Entidade Order

```csharp
// Adicionar em Logistics.Domain/Entities/Order.cs

// Relacionamentos WMS
public Guid? VehicleId { get; private set; }
public Guid? DriverId { get; private set; }
public Guid? OriginWarehouseId { get; private set; }
public Guid? DestinationWarehouseId { get; private set; }

// Endereço Completo
public string? ShippingZipCode { get; private set; }
public decimal? ShippingLatitude { get; private set; }
public decimal? ShippingLongitude { get; private set; }
public string? ShippingCity { get; private set; }
public string? ShippingState { get; private set; }
public string? ShippingCountry { get; private set; }

// Rastreamento
public string? TrackingNumber { get; private set; }
public DateTime? EstimatedDeliveryDate { get; private set; }
public DateTime? ActualDeliveryDate { get; private set; }
public DateTime? ShippedAt { get; private set; }
public DateTime? DeliveredAt { get; private set; }

// Navigation Properties
public Vehicle? Vehicle { get; private set; }
public Driver? Driver { get; private set; }
public Warehouse? OriginWarehouse { get; private set; }
public Warehouse? DestinationWarehouse { get; private set; }

// Métodos
public void AssignVehicle(Guid vehicleId)
{
    VehicleId = vehicleId;
    UpdatedAt = DateTime.UtcNow;
}

public void AssignDriver(Guid driverId)
{
    DriverId = driverId;
    UpdatedAt = DateTime.UtcNow;
}

public void SetShippingLocation(string zipCode, decimal? latitude, decimal? longitude, string? city, string? state, string? country)
{
    ShippingZipCode = zipCode;
    ShippingLatitude = latitude;
    ShippingLongitude = longitude;
    ShippingCity = city;
    ShippingState = state;
    ShippingCountry = country;
    UpdatedAt = DateTime.UtcNow;
}

public void SetTrackingNumber(string trackingNumber)
{
    TrackingNumber = trackingNumber;
    UpdatedAt = DateTime.UtcNow;
}

public void MarkAsShipped()
{
    Status = OrderStatus.Shipped;
    ShippedAt = DateTime.UtcNow;
    UpdatedAt = DateTime.UtcNow;
}

public void MarkAsDelivered()
{
    Status = OrderStatus.Delivered;
    DeliveredAt = DateTime.UtcNow;
    ActualDeliveryDate = DateTime.UtcNow;
    UpdatedAt = DateTime.UtcNow;
}
```

### 1.4 Migration

```bash
cd API/src/Logistics.API
dotnet ef migrations add AddOrderStatusPriorityTablesAndWMSFields -p ../Logistics.Infrastructure -s .
dotnet ef database update
```

---

## 🌐 PARTE 2: BACKEND - ENDPOINTS

### 2.1 OrderStatusController

```csharp
// Logistics.API/Controllers/OrderStatusController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderStatusController : ControllerBase
{
    private readonly IOrderStatusService _service;

    public OrderStatusController(IOrderStatusService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderStatusResponse>>> GetAll([FromQuery] string? language = "pt")
    {
        var statuses = await _service.GetAllAsync(language);
        return Ok(statuses);
    }
}
```

### 2.2 OrderPriorityController

```csharp
// Logistics.API/Controllers/OrderPriorityController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderPriorityController : ControllerBase
{
    private readonly IOrderPriorityService _service;

    public OrderPriorityController(IOrderPriorityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderPriorityResponse>>> GetAll([FromQuery] string? language = "pt")
    {
        var priorities = await _service.GetAllAsync(language);
        return Ok(priorities);
    }
}
```

### 2.3 Atualizar OrdersController

```csharp
// Adicionar em Logistics.API/Controllers/OrdersController.cs

[HttpPut("{id}/assign-vehicle")]
public async Task<ActionResult> AssignVehicle(Guid id, [FromBody] AssignVehicleRequest request)
{
    await _service.AssignVehicleAsync(id, request.VehicleId);
    return NoContent();
}

[HttpPut("{id}/assign-driver")]
public async Task<ActionResult> AssignDriver(Guid id, [FromBody] AssignDriverRequest request)
{
    await _service.AssignDriverAsync(id, request.DriverId);
    return NoContent();
}

[HttpGet("{id}/tracking")]
public async Task<ActionResult<OrderTrackingResponse>> GetTracking(Guid id)
{
    var tracking = await _service.GetTrackingAsync(id);
    return Ok(tracking);
}

[HttpPut("{id}/shipping-location")]
public async Task<ActionResult> SetShippingLocation(Guid id, [FromBody] SetShippingLocationRequest request)
{
    await _service.SetShippingLocationAsync(id, request);
    return NoContent();
}
```

---

## 🌍 PARTE 3: FRONTEND - i18n COMPLETO

### 3.1 Estrutura de Arquivos

```
APP/src/assets/i18n/
├── pt.json
├── en.json
└── es.json
```

### 3.2 pt.json (Português)

```json
{
  "common": {
    "buttons": {
      "save": "Salvar",
      "cancel": "Cancelar",
      "delete": "Excluir",
      "edit": "Editar",
      "create": "Criar",
      "close": "Fechar",
      "search": "Buscar",
      "filter": "Filtrar"
    },
    "loading": "Carregando...",
    "noData": "Nenhum dado disponível",
    "error": "Erro ao processar requisição"
  },
  "orders": {
    "title": "Pedidos",
    "subtitle": "Gestão de pedidos de entrada e saída",
    "newOrder": "Novo Pedido",
    "orderNumber": "Número do Pedido",
    "orderDate": "Data do Pedido",
    "expectedDate": "Data Esperada",
    "customer": "Cliente",
    "supplier": "Fornecedor",
    "vehicle": "Veículo",
    "driver": "Motorista",
    "originWarehouse": "Armazém Origem",
    "destinationWarehouse": "Armazém Destino",
    "shippingAddress": "Endereço de Entrega",
    "zipCode": "CEP",
    "viewOnMap": "Ver no Mapa",
    "searchAddress": "Buscar Endereço",
    "trackingNumber": "Número de Rastreamento",
    "estimatedDelivery": "Entrega Estimada",
    "actualDelivery": "Entrega Real",
    "specialInstructions": "Instruções Especiais",
    "items": "Items",
    "totalQuantity": "Quantidade Total",
    "totalValue": "Valor Total",
    "type": {
      "inbound": "Entrada",
      "outbound": "Saída",
      "transfer": "Transferência",
      "return": "Devolução"
    },
    "source": {
      "manual": "Manual",
      "erp": "ERP",
      "ecommerce": "E-commerce",
      "edi": "EDI"
    }
  }
}
```

### 3.3 en.json (English)

```json
{
  "common": {
    "buttons": {
      "save": "Save",
      "cancel": "Cancel",
      "delete": "Delete",
      "edit": "Edit",
      "create": "Create",
      "close": "Close",
      "search": "Search",
      "filter": "Filter"
    },
    "loading": "Loading...",
    "noData": "No data available",
    "error": "Error processing request"
  },
  "orders": {
    "title": "Orders",
    "subtitle": "Inbound and outbound order management",
    "newOrder": "New Order",
    "orderNumber": "Order Number",
    "orderDate": "Order Date",
    "expectedDate": "Expected Date",
    "customer": "Customer",
    "supplier": "Supplier",
    "vehicle": "Vehicle",
    "driver": "Driver",
    "originWarehouse": "Origin Warehouse",
    "destinationWarehouse": "Destination Warehouse",
    "shippingAddress": "Shipping Address",
    "zipCode": "ZIP Code",
    "viewOnMap": "View on Map",
    "searchAddress": "Search Address",
    "trackingNumber": "Tracking Number",
    "estimatedDelivery": "Estimated Delivery",
    "actualDelivery": "Actual Delivery",
    "specialInstructions": "Special Instructions",
    "items": "Items",
    "totalQuantity": "Total Quantity",
    "totalValue": "Total Value",
    "type": {
      "inbound": "Inbound",
      "outbound": "Outbound",
      "transfer": "Transfer",
      "return": "Return"
    },
    "source": {
      "manual": "Manual",
      "erp": "ERP",
      "ecommerce": "E-commerce",
      "edi": "EDI"
    }
  }
}
```

### 3.4 es.json (Español)

```json
{
  "common": {
    "buttons": {
      "save": "Guardar",
      "cancel": "Cancelar",
      "delete": "Eliminar",
      "edit": "Editar",
      "create": "Crear",
      "close": "Cerrar",
      "search": "Buscar",
      "filter": "Filtrar"
    },
    "loading": "Cargando...",
    "noData": "No hay datos disponibles",
    "error": "Error al procesar la solicitud"
  },
  "orders": {
    "title": "Pedidos",
    "subtitle": "Gestión de pedidos de entrada y salida",
    "newOrder": "Nuevo Pedido",
    "orderNumber": "Número de Pedido",
    "orderDate": "Fecha del Pedido",
    "expectedDate": "Fecha Esperada",
    "customer": "Cliente",
    "supplier": "Proveedor",
    "vehicle": "Vehículo",
    "driver": "Conductor",
    "originWarehouse": "Almacén Origen",
    "destinationWarehouse": "Almacén Destino",
    "shippingAddress": "Dirección de Envío",
    "zipCode": "Código Postal",
    "viewOnMap": "Ver en el Mapa",
    "searchAddress": "Buscar Dirección",
    "trackingNumber": "Número de Seguimiento",
    "estimatedDelivery": "Entrega Estimada",
    "actualDelivery": "Entrega Real",
    "specialInstructions": "Instrucciones Especiales",
    "items": "Artículos",
    "totalQuantity": "Cantidad Total",
    "totalValue": "Valor Total",
    "type": {
      "inbound": "Entrada",
      "outbound": "Salida",
      "transfer": "Transferencia",
      "return": "Devolución"
    },
    "source": {
      "manual": "Manual",
      "erp": "ERP",
      "ecommerce": "Comercio Electrónico",
      "edi": "EDI"
    }
  }
}
```

---

## 🗺️ PARTE 4: FRONTEND - GOOGLE MAPS

### 4.1 Configuração

```typescript
// APP/src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  googleMapsApiKey: 'SUA_CHAVE_AQUI'
};
```

### 4.2 Service de Geocoding

```typescript
// APP/src/app/core/services/geocoding.service.ts
@Injectable({ providedIn: 'root' })
export class GeocodingService {
  private readonly apiKey = environment.googleMapsApiKey;

  async geocodeAddress(address: string): Promise<{lat: number, lng: number} | null> {
    const url = `https://maps.googleapis.com/maps/api/geocode/json?address=${encodeURIComponent(address)}&key=${this.apiKey}`;
    const response = await fetch(url);
    const data = await response.json();
    
    if (data.results && data.results.length > 0) {
      const location = data.results[0].geometry.location;
      return { lat: location.lat, lng: location.lng };
    }
    return null;
  }

  async searchByCep(cep: string): Promise<any> {
    const cleanCep = cep.replace(/\D/g, '');
    const response = await fetch(`https://viacep.com.br/ws/${cleanCep}/json/`);
    return await response.json();
  }
}
```

### 4.3 Componente de Mapa

```typescript
// APP/src/app/shared/components/map-viewer/map-viewer.component.ts
@Component({
  selector: 'app-map-viewer',
  template: `
    <div class="map-container">
      @if (address()) {
        <iframe
          [src]="mapUrl()"
          width="100%"
          height="400"
          style="border:0;"
          loading="lazy"
          referrerpolicy="no-referrer-when-downgrade">
        </iframe>
      }
    </div>
  `
})
export class MapViewerComponent {
  address = input.required<string>();
  
  mapUrl = computed(() => {
    const addr = encodeURIComponent(this.address());
    return this.sanitizer.bypassSecurityTrustResourceUrl(
      `https://www.google.com/maps/embed/v1/place?key=${environment.googleMapsApiKey}&q=${addr}`
    );
  });
  
  constructor(private sanitizer: DomSanitizer) {}
}
```

---

## 📊 CRONOGRAMA DE IMPLEMENTAÇÃO

### Sprint 1: Backend Base (2-3 dias)
- [ ] Criar entidades OrderStatus e OrderPriority
- [ ] Criar migration com seed data
- [ ] Atualizar entidade Order com campos WMS
- [ ] Criar repositories
- [ ] Criar services

### Sprint 2: Backend Endpoints (1-2 dias)
- [ ] Criar OrderStatusController
- [ ] Criar OrderPriorityController
- [ ] Atualizar OrdersController com novos endpoints
- [ ] Testar todos endpoints no Swagger

### Sprint 3: Frontend i18n (2 dias)
- [ ] Criar arquivos pt.json, en.json, es.json
- [ ] Atualizar I18nService para carregar JSONs
- [ ] Remover TODOS textos estáticos
- [ ] Criar seletor de idioma no header

### Sprint 4: Frontend Google Maps (2 dias)
- [ ] Configurar Google Maps API
- [ ] Criar GeocodingService
- [ ] Criar MapViewerComponent
- [ ] Integrar busca por CEP (ViaCEP)
- [ ] Adicionar autocomplete de endereço

### Sprint 5: Frontend Relacionamentos (2 dias)
- [ ] Criar selects de Vehicle
- [ ] Criar selects de Driver
- [ ] Criar selects de Warehouse
- [ ] Atualizar modais create/edit
- [ ] Mostrar informações completas na lista

### Sprint 6: Testes e Ajustes (1 dia)
- [ ] Testar fluxo completo
- [ ] Ajustar UX
- [ ] Documentar

**TOTAL ESTIMADO**: 10-12 dias

---

## ✅ CHECKLIST FINAL

### Backend
- [ ] Tabela OrderStatuses criada com i18n
- [ ] Tabela OrderPriorities criada com i18n
- [ ] Order atualizado com campos WMS
- [ ] Endpoints de status/priority funcionando
- [ ] Endpoints de assign vehicle/driver funcionando
- [ ] Migration aplicada no banco

### Frontend
- [ ] Arquivos i18n (pt/en/es) criados
- [ ] Nenhum texto estático no código
- [ ] Status vindo da API
- [ ] Priority vindo da API
- [ ] Google Maps integrado
- [ ] Busca por CEP funcionando
- [ ] Selects de Vehicle/Driver/Warehouse
- [ ] Visualização completa de rastreamento

### Documentação
- [ ] README atualizado
- [ ] Endpoints documentados
- [ ] Guia de uso do Google Maps
- [ ] Guia de tradução

---

**Próximo Passo**: Começar Sprint 1 - Backend Base
