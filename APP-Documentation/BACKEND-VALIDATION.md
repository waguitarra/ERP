# 🧪 VALIDAÇÃO DO BACKEND - API

**Data**: 2025-11-24

## ⚠️ Status da API

- API backend está em processo de inicialização (comando rodando)
- Endpoint não está respondendo ainda
- **PRÓXIMO PASSO**: Aguardar API ou iniciar manualmente quando necessário

## 📝 Campos Conhecidos (da Documentação Backend)

### Product (ProductResponse - Backend)
```csharp
- Id: Guid
- CompanyId: Guid
- Name: string
- Sku: string
- Barcode: string (nullable)
- Description: string (nullable)
- Weight: decimal
- WeightUnit: string (nullable)
- Volume: decimal
- VolumeUnit: string (nullable)
- Length: decimal
- Width: decimal
- Height: decimal
- DimensionUnit: string (nullable)
- RequiresLotTracking: bool
- RequiresSerialTracking: bool
- IsPerishable: bool
- ShelfLifeDays: int (nullable)
- MinimumStock: decimal (nullable)
- SafetyStock: decimal (nullable)
- AbcClassification: string (nullable)
- IsActive: bool
- CreatedAt: DateTime
- UpdatedAt: DateTime (nullable)
```

### Order (OrderResponse - Backend)
```csharp
- Id: Guid
- CompanyId: Guid
- OrderNumber: string
- Type: OrderType (enum: Inbound=1, Outbound=2, Transfer=3, Return=4)
- Source: OrderSource (enum: Manual=1, ERP=2, Ecommerce=3, EDI=4)
- CustomerId: Guid (nullable)
- CustomerName: string (nullable)
- SupplierId: Guid (nullable)
- SupplierName: string (nullable)
- OrderDate: DateTime
- ExpectedDate: DateTime (nullable)
- Priority: OrderPriority (enum: Low=0, Normal=1, High=2, Urgent=3)
- Status: OrderStatus (enum: Draft=0, Pending=1, Confirmed=2, InProgress=3, ...)
- TotalQuantity: decimal
- TotalValue: decimal
- ShippingAddress: string (nullable)
- SpecialInstructions: string (nullable)
- IsBOPIS: bool
- CreatedBy: Guid
- Items: List<OrderItemResponse>
- CreatedAt: DateTime
- UpdatedAt: DateTime (nullable)
```

### Vehicle (VehicleResponse - Backend)
```csharp
- Id: Guid
- CompanyId: Guid
- PlateNumber: string  ⚠️ NÃO é "licensePlate"
- Model: string
- Capacity: decimal (nullable)
- VehicleType: string (nullable)
- Status: VehicleStatus (enum)
- IsActive: bool
- CreatedAt: DateTime
- UpdatedAt: DateTime (nullable)
```

## 🔴 Diferenças Críticas Identificadas

### Product Model - Frontend vs Backend
- ❌ Frontend: `id: number` → Backend: `Id: Guid` 
- ❌ Faltam 15+ campos WMS no frontend
- ❌ Frontend não tem campos de tracking
- ❌ Frontend não tem campos de estoque

### Order Model - Frontend vs Backend  
- ❌ Frontend usa `OrderStatus` como string literal
- ❌ Backend usa enums numéricos
- ❌ Frontend não tem `OrderType`, `OrderSource`, `Priority`
- ❌ Conceitos completamente diferentes

### Vehicle Model - Frontend vs Backend
- ❌ Frontend: `licensePlate` → Backend: `PlateNumber`
- ❌ Nomes de campos incompatíveis

## ✅ Ações Necessárias (PARTE 2 do Checklist)

1. Criar `/core/models/enums.ts` com TODOS enums do backend
2. Corrigir `product.model.ts` - adicionar TODOS campos
3. Corrigir `order.model.ts` - usar enums corretos
4. Corrigir `vehicle.model.ts` - usar PlateNumber
5. Criar DTOs corretos para cada model

**Status**: Pronto para iniciar PARTE 2
