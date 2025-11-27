# ✅ SEPARAÇÃO COMPLETA: PURCHASE ORDERS ≠ SALES ORDERS

**Data**: 2025-11-27  
**Status**: ✅ CONCLUÍDO

---

## 🎯 PROBLEMA RESOLVIDO

**ANTES (ERRADO)**:
```
❌ Tabela única "Orders" com campo Type='Inbound'/'Outbound'
❌ Compras e vendas misturadas
❌ Campos genéricos para ambos
❌ Relacionamentos confusos (SupplierId e CustomerId opcionais)
```

**DEPOIS (CORRETO)**:
```
✅ PurchaseOrders (Compras) - Tabela separada
✅ SalesOrders (Vendas) - Tabela separada
✅ Campos específicos para cada tipo
✅ Relacionamentos corretos (SupplierId obrigatório em PO, CustomerId obrigatório em SO)
```

---

## 📊 DADOS MIGRADOS

| Tabela | Total | Status |
|--------|-------|--------|
| **PurchaseOrders** | 153 | ✅ Migrados |
| **SalesOrders** | 81 | ✅ Migrados |
| **Orders (OLD)** | 234 | ⚠️ Manter por enquanto (compatibilidade) |

---

## 🏗️ ESTRUTURA CRIADA

### 1. Entidades (Domain)

**PurchaseOrder.cs**:
- ✅ SupplierId (obrigatório)
- ✅ Campos de compra: UnitCost, TaxPercentage, DesiredMarginPercentage
- ✅ Cálculo automático: SuggestedSalePrice, EstimatedProfit
- ✅ Hierarquia: ExpectedParcels, ReceivedParcels, CartonsPerParcel, UnitsPerCarton
- ✅ Internacional: IsInternational, OriginCountry, PortOfEntry, ContainerNumber, Incoterm
- ✅ Navigation: PurchaseOrderItems, PurchaseOrderDocuments

**SalesOrder.cs**:
- ✅ CustomerId (obrigatório)
- ✅ Campos de venda: ShippingAddress, TrackingNumber, IsBOPIS
- ✅ Hierarquia: ExpectedParcels, PackedParcels, CartonsPerParcel, UnitsPerCarton (NOVO)
- ✅ Rastreamento: ShippedAt, DeliveredAt, EstimatedDeliveryDate
- ✅ Navigation: SalesOrderItems

---

## 🔌 ENDPOINTS CRIADOS

### Purchase Orders (Compras)
```
GET    /api/purchase-orders/company/{companyId}     # Listar por empresa
GET    /api/purchase-orders/{id}                    # Detalhes
POST   /api/purchase-orders                         # Criar
POST   /api/purchase-orders/{id}/purchase-details   # Definir preços/margens
POST   /api/purchase-orders/{id}/packaging-hierarchy # Definir hierarquia
POST   /api/purchase-orders/{id}/set-international  # Definir como importação
```

### Sales Orders (Vendas)
```
GET    /api/sales-orders/company/{companyId}        # Listar por empresa
GET    /api/sales-orders/{id}                       # Detalhes
POST   /api/sales-orders                            # Criar
POST   /api/sales-orders/{id}/packaging-hierarchy   # Definir hierarquia (NOVO)
POST   /api/sales-orders/{id}/mark-shipped          # Marcar enviado
POST   /api/sales-orders/{id}/mark-delivered        # Marcar entregue
```

---

## 💾 BANCO DE DADOS

### Tabelas Criadas

**PurchaseOrders**:
- PurchaseOrderNumber (string, unique por CompanyId)
- SupplierId (obrigatório)
- Campos de compra (UnitCost, TaxPercentage, etc.)
- Campos internacionais (OriginCountry, ContainerNumber, etc.)

**PurchaseOrderItems**:
- QuantityOrdered
- QuantityReceived

**PurchaseOrderDocuments**:
- Invoice, DI, BL, PackingList, Certificate
- Soft delete (DeletedAt, DeletedBy)

**SalesOrders**:
- SalesOrderNumber (string, unique por CompanyId)
- CustomerId (obrigatório)
- Campos de venda (ShippingAddress, TrackingNumber, etc.)
- Hierarquia de embalagem (ExpectedParcels, PackedParcels, etc.)

**SalesOrderItems**:
- QuantityOrdered
- QuantityAllocated
- QuantityPicked
- QuantityShipped

---

## 🔧 CONFIGURAÇÕES EF CORE

**Indexes criados**:
- `(CompanyId, PurchaseOrderNumber)` UNIQUE
- `(CompanyId, SalesOrderNumber)` UNIQUE
- `Status`
- `OrderDate`
- `SupplierId` / `CustomerId`

**Relationships**:
- PurchaseOrder → Supplier (Restrict)
- SalesOrder → Customer (Restrict)
- Items → Product (Restrict)
- Documents → PurchaseOrder (Cascade)

---

## 📝 DIFERENÇAS PRINCIPAIS

| Aspecto | PurchaseOrder (Compra) | SalesOrder (Venda) |
|---------|------------------------|-------------------|
| **Quem** | Fornecedor (Supplier) | Cliente (Customer) |
| **Fluxo** | Entrada (Receiving) | Saída (Shipping) |
| **Preços** | UnitCost, TaxPercentage, Margin | UnitPrice |
| **Quantidade** | QuantityReceived | QuantityPicked, QuantityShipped |
| **Tracking** | ContainerNumber, BL | TrackingNumber |
| **Documentos** | Invoice, DI, BL, Packing List | Nota Fiscal |
| **Internacional** | Sim (OriginCountry, Incoterm) | Não |
| **Endereço** | Não | ShippingAddress |
| **Hierarquia** | ExpectedParcels / ReceivedParcels | ExpectedParcels / PackedParcels |

---

## ✅ NOVO RECURSO: HIERARQUIA EM VENDAS

Agora **SalesOrders** também tem hierarquia de embalagem:
- `ExpectedParcels`: Quantos pallets/caixas enviar
- `PackedParcels`: Quantos já foram embalados
- `CartonsPerParcel`: Caixas por pallet
- `UnitsPerCarton`: Unidades por caixa

Mesma lógica de validação:
```
ExpectedParcels × CartonsPerParcel × UnitsPerCarton = TotalQuantity
```

---

## 🧪 TESTES

**Validações implementadas**:
- ✅ Número único de PO/SO por empresa
- ✅ Fornecedor obrigatório em PurchaseOrder
- ✅ Cliente obrigatório em SalesOrder
- ✅ Hierarquia deve bater com quantidade total
- ✅ Cálculo automático de preços em PurchaseOrder
- ✅ Produto deve existir ao adicionar item

---

## 🚀 PRÓXIMOS PASSOS

### Backend
1. ✅ Manter Orders (antigo) por compatibilidade
2. ⏳ Migrar InboundShipments para referenciar PurchaseOrders
3. ⏳ Migrar OutboundShipments para referenciar SalesOrders
4. ⏳ Criar PurchaseOrderDocumentsController
5. ⏳ Implementar upload WebP em PurchaseOrders

### Frontend
1. ⏳ Criar módulo `purchase-orders/` (conforme documentação)
2. ⏳ Criar módulo `sales-orders/` (separado de orders)
3. ⏳ Adicionar i18n para ambos
4. ⏳ Componentes reutilizáveis (SupplierSelector, DocumentUpload)
5. ⏳ Dashboard de recebimento (PurchaseOrders)
6. ⏳ Dashboard de expedição (SalesOrders)

---

## 📚 DOCUMENTAÇÃO

**Backend**: `/API-Documentation/0001-ANALISE-GAP-WMS-PARCEL-TRACKING.md`
**Frontend**: `/APP-Documentation/0001-ANALISE-GAP-WMS-PARCEL-TRACKING-FRONT.md`

---

**Conclusão**: Problema resolvido. Compras e vendas agora são **100% separadas** com tabelas, endpoints e lógica de negócio específicas.
