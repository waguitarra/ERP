# ANÁLISE COMPARATIVA: API vs FRONTEND
## Contagem Completa de Endpoints e Componentes

**Data**: 2025-11-25  
**Status**: ⚠️ CRÍTICO - 67% dos endpoints não consumidos

---

## 📊 RESUMO EXECUTIVO

### API (Backend)
- **Controllers**: 26
- **Endpoints totais**: ~156
- **Entities**: 29
- **Cobertura**: 100% funcional

### Frontend
- **Services**: 10 (38% dos controllers)
- **Componentes List**: 14
- **Modais Create**: 6
- **Modais Edit**: 6
- **Endpoints consumidos**: ~52 (33%)
- **Endpoints NÃO consumidos**: ~104 (67%)

---

## 🔴 ANÁLISE DETALHADA POR CONTROLLER

### 1. ✅ AuthController (2 endpoints)
**Backend**: `/api/auth`
- POST `/api/auth/login` ✅
- POST `/api/auth/register-admin` ✅

**Frontend**: AuthService ✅
- ✅ Login implementado
- ✅ Register implementado
- **Cobertura**: 100%

---

### 2. ❌ UsersController (7 endpoints)
**Backend**: `/api/users`
- POST `/api/users`
- GET `/api/users/{id}`
- GET `/api/users`
- GET `/api/users/company/{companyId}`
- PUT `/api/users/{id}`
- PATCH `/api/users/{id}/role`
- DELETE `/api/users/{id}`

**Frontend**: ❌ NÃO EXISTE
- **Cobertura**: 0% (0/7)
- **Impacto**: CRÍTICO - Gestão de usuários impossível

---

### 3. ✅ CompaniesController (5 endpoints)
**Backend**: `/api/companies`
- POST `/api/companies` ❌
- GET `/api/companies/{id}` ✅
- GET `/api/companies` ✅
- PUT `/api/companies/{id}` ❌
- DELETE `/api/companies/{id}` ❌

**Frontend**: CompaniesService ✅ + CompaniesListComponent ✅
- ✅ Service criado
- ✅ Lista implementada
- ❌ **FALTANDO**: Modal create
- ❌ **FALTANDO**: Modal edit
- **Cobertura**: 40% (2/5)

---

### 4. ✅ WarehousesController (5 endpoints)
**Backend**: `/api/warehouses`
- POST `/api/warehouses` ✅
- GET `/api/warehouses/{id}` ✅
- GET `/api/warehouses/company/{companyId}` ✅
- PUT `/api/warehouses/{id}` ✅
- DELETE `/api/warehouses/{id}` ✅

**Frontend**: WarehousesService ✅ + WarehousesListComponent ✅ + Modais ✅
- ✅ Service completo
- ✅ Lista implementada
- ✅ Modal create
- ✅ Modal edit
- **Cobertura**: 100% (5/5) ✅

---

### 5. ❌ WarehouseZonesController (5 endpoints)
**Backend**: `/api/warehousezones`
- POST `/api/warehousezones`
- GET `/api/warehousezones/{id}`
- GET `/api/warehousezones/warehouse/{warehouseId}`
- PUT `/api/warehousezones/{id}`
- DELETE `/api/warehousezones/{id}`

**Frontend**: ❌ NÃO EXISTE
- **Cobertura**: 0% (0/5)
- **Impacto**: ALTO - Organização de armazéns

---

### 6. ✅ StorageLocationsController (6 endpoints)
**Backend**: `/api/storagelocations`
- POST `/api/storagelocations` ❌
- GET `/api/storagelocations/{id}` ✅
- GET `/api/storagelocations` ✅
- PUT `/api/storagelocations/{id}` ❌
- DELETE `/api/storagelocations/{id}` ❌
- POST `/api/storagelocations/{id}/block` ✅
- POST `/api/storagelocations/{id}/unblock` ✅

**Frontend**: StorageLocationsService ✅ + StorageLocationsListComponent ✅
- ✅ Service criado
- ✅ Lista implementada
- ✅ Block/Unblock implementado
- ❌ **FALTANDO**: Modal create
- ❌ **FALTANDO**: Modal edit
- **Cobertura**: 67% (4/6)

---

### 7. ✅ ProductsController (7 endpoints)
**Backend**: `/api/products`
- POST `/api/products` ✅
- GET `/api/products/{id}` ✅
- GET `/api/products/company/{companyId}` ✅
- GET `/api/products/sku/{sku}/company/{companyId}` ❌
- PUT `/api/products/{id}` ✅
- DELETE `/api/products/{id}` ✅

**Frontend**: ProductsService ✅ + ProductsListComponent ✅ + Modais ✅
- ✅ Service completo
- ✅ Lista implementada
- ✅ Modal create
- ✅ Modal edit
- ❌ **FALTANDO**: Busca por SKU
- **Cobertura**: 71% (5/7)

---

### 8. ✅ CustomersController (5 endpoints)
**Backend**: `/api/customers`
- POST `/api/customers` ✅
- GET `/api/customers/{id}` ✅
- GET `/api/customers/company/{companyId}` ✅
- PUT `/api/customers/{id}` ✅
- DELETE `/api/customers/{id}` ✅

**Frontend**: CustomersService ✅ + CustomersListComponent ✅ + Modais ✅
- ✅ Service completo
- ✅ Lista implementada
- ✅ Modal create
- ✅ Modal edit
- **Cobertura**: 100% (5/5) ✅

---

### 9. ✅ SuppliersController (5 endpoints)
**Backend**: `/api/suppliers`
- POST `/api/suppliers` ✅
- GET `/api/suppliers/{id}` ✅
- GET `/api/suppliers/company/{companyId}` ✅
- PUT `/api/suppliers/{id}` ✅
- DELETE `/api/suppliers/{id}` ✅

**Frontend**: SuppliersService ✅ + SuppliersListComponent ✅ + Modais ✅
- ✅ Service completo
- ✅ Lista implementada
- ✅ Modal create
- ✅ Modal edit
- **Cobertura**: 100% (5/5) ✅

---

### 10. ✅ VehiclesController (6 endpoints)
**Backend**: `/api/vehicles`
- POST `/api/vehicles` ✅
- GET `/api/vehicles/{id}` ✅
- GET `/api/vehicles/company/{companyId}` ✅
- PUT `/api/vehicles/{id}` ✅
- DELETE `/api/vehicles/{id}` ✅
- PATCH `/api/vehicles/{id}/status` ❌

**Frontend**: VehiclesService ✅ + VehiclesListComponent ✅ + Modais ✅
- ✅ Service completo
- ✅ Lista implementada
- ✅ Modal create
- ✅ Modal edit
- ❌ **FALTANDO**: Update status
- **Cobertura**: 83% (5/6)

---

### 11. ✅ DriversController (6 endpoints)
**Backend**: `/api/drivers`
- POST `/api/drivers` ✅
- GET `/api/drivers/{id}` ✅
- GET `/api/drivers/company/{companyId}` ✅
- PUT `/api/drivers/{id}` ✅
- DELETE `/api/drivers/{id}` ✅
- PATCH `/api/drivers/{id}/license` ❌

**Frontend**: DriversService ✅ + DriversListComponent ✅ + Modais ✅
- ✅ Service completo
- ✅ Lista implementada
- ✅ Modal create
- ✅ Modal edit
- ❌ **FALTANDO**: Update license
- **Cobertura**: 83% (5/6)

---

### 12. ⚠️ OrdersController (3 endpoints)
**Backend**: `/api/orders`
- POST `/api/orders` ❌
- GET `/api/orders/{id}` ✅
- GET `/api/orders/company/{companyId}` ✅

**Frontend**: OrdersService ✅ + OrdersListComponent ✅
- ✅ Service básico
- ✅ Lista implementada
- ❌ **FALTANDO**: Criação de pedidos
- ❌ **FALTANDO**: Modal create com items
- **Cobertura**: 67% (2/3)

---

### 13. ❌ InboundShipmentsController (6 endpoints)
**Backend**: `/api/inboundshipments`
- POST `/api/inboundshipments`
- GET `/api/inboundshipments/{id}`
- GET `/api/inboundshipments/company/{companyId}`
- POST `/api/inboundshipments/{id}/receive`
- POST `/api/inboundshipments/{id}/complete`
- DELETE `/api/inboundshipments/{id}`

**Frontend**: InboundShipmentsListComponent ⚠️
- ❌ Service NÃO EXISTE
- ⚠️ Componente vazio
- **Cobertura**: 0% (0/6)
- **Impacto**: CRÍTICO - Fluxo Inbound

---

### 14. ❌ ReceiptsController (3 endpoints)
**Backend**: `/api/receipts`
- POST `/api/receipts`
- GET `/api/receipts/{id}`
- GET `/api/receipts/shipment/{shipmentId}`

**Frontend**: ❌ NÃO EXISTE
- **Cobertura**: 0% (0/3)
- **Impacto**: CRÍTICO - Recebimento GRN

---

### 15. ❌ PutawayTasksController (5 endpoints)
**Backend**: `/api/putawaytasks`
- POST `/api/putawaytasks`
- GET `/api/putawaytasks/{id}`
- GET `/api/putawaytasks/warehouse/{warehouseId}`
- POST `/api/putawaytasks/{id}/assign`
- POST `/api/putawaytasks/{id}/complete`

**Frontend**: ❌ NÃO EXISTE
- **Cobertura**: 0% (0/5)
- **Impacto**: CRÍTICO - Endereçamento

---

### 16. ⚠️ InventoriesController (6 endpoints)
**Backend**: `/api/inventories`
- POST `/api/inventories` ❌
- GET `/api/inventories/{id}` ❌
- GET `/api/inventories/warehouse/{warehouseId}` ✅
- GET `/api/inventories/product/{productId}` ✅
- PUT `/api/inventories/{id}` ❌
- DELETE `/api/inventories/{id}` ❌

**Frontend**: InventoryService ✅ + InventoryListComponent ✅
- ✅ Service básico
- ✅ Lista implementada
- ❌ **FALTANDO**: Ajustes de estoque
- ❌ **FALTANDO**: CRUD completo
- **Cobertura**: 33% (2/6)

---

### 17. ❌ PickingWavesController (4 endpoints)
**Backend**: `/api/pickingwaves`
- POST `/api/pickingwaves`
- GET `/api/pickingwaves/{id}`
- POST `/api/pickingwaves/{id}/release`
- POST `/api/pickingwaves/{id}/complete`

**Frontend**: ❌ NÃO EXISTE
- **Cobertura**: 0% (0/4)
- **Impacto**: CRÍTICO - Fluxo Outbound

---

### 18. ❌ PackingTasksController (4 endpoints)
**Backend**: `/api/packingtasks`
- POST `/api/packingtasks`
- GET `/api/packingtasks/{id}`
- POST `/api/packingtasks/{id}/start`
- POST `/api/packingtasks/{id}/complete`

**Frontend**: PackingTasksListComponent ⚠️
- ❌ Service NÃO EXISTE
- ⚠️ Componente vazio
- **Cobertura**: 0% (0/4)

---

### 19. ❌ PackagesController (4 endpoints)
**Backend**: `/api/packages`
- POST `/api/packages`
- GET `/api/packages/{id}`
- PUT `/api/packages/{id}/dimensions`
- PATCH `/api/packages/{id}/status`

**Frontend**: ❌ NÃO EXISTE
- **Cobertura**: 0% (0/4)

---

### 20. ❌ OutboundShipmentsController (4 endpoints)
**Backend**: `/api/outboundshipments`
- POST `/api/outboundshipments`
- GET `/api/outboundshipments/{id}`
- GET `/api/outboundshipments/company/{companyId}`
- POST `/api/outboundshipments/{id}/ship`

**Frontend**: OutboundShipmentsListComponent ⚠️
- ❌ Service NÃO EXISTE
- ⚠️ Componente vazio
- **Cobertura**: 0% (0/4)

---

### 21. ❌ StockMovementsController (4 endpoints)
**Backend**: `/api/stockmovements`
- POST `/api/stockmovements`
- GET `/api/stockmovements/{id}`
- GET `/api/stockmovements`
- GET `/api/stockmovements/product/{productId}`

**Frontend**: ❌ NÃO EXISTE
- **Cobertura**: 0% (0/4)
- **Impacto**: ALTO - Rastreabilidade

---

### 22. ❌ LotsController (5 endpoints)
**Backend**: `/api/lots`
- POST `/api/lots`
- GET `/api/lots/{id}`
- GET `/api/lots/product/{productId}`
- PUT `/api/lots/{id}`
- DELETE `/api/lots/{id}`

**Frontend**: ❌ NÃO EXISTE
- **Cobertura**: 0% (0/5)
- **Impacto**: CRÍTICO - FEFO/Rastreabilidade

---

### 23. ❌ SerialNumbersController (6 endpoints)
**Backend**: `/api/serialnumbers`
- POST `/api/serialnumbers`
- GET `/api/serialnumbers/{id}`
- GET `/api/serialnumbers/serial/{serialNumber}`
- GET `/api/serialnumbers/product/{productId}`
- PUT `/api/serialnumbers/{id}`
- DELETE `/api/serialnumbers/{id}`

**Frontend**: ❌ NÃO EXISTE
- **Cobertura**: 0% (0/6)
- **Impacto**: CRÍTICO - Rastreabilidade

---

### 24. ❌ CycleCountsController (4 endpoints)
**Backend**: `/api/cyclecounts`
- POST `/api/cyclecounts`
- GET `/api/cyclecounts/{id}`
- GET `/api/cyclecounts/warehouse/{warehouseId}`
- POST `/api/cyclecounts/{id}/complete`

**Frontend**: ❌ NÃO EXISTE
- **Cobertura**: 0% (0/4)
- **Impacto**: ALTO - Inventário

---

### 25. ❌ VehicleAppointmentsController (5 endpoints)
**Backend**: `/api/vehicleappointments`
- POST `/api/vehicleappointments`
- GET `/api/vehicleappointments/{id}`
- GET `/api/vehicleappointments/warehouse/{warehouseId}`
- POST `/api/vehicleappointments/{id}/checkin`
- POST `/api/vehicleappointments/{id}/checkout`

**Frontend**: ❌ NÃO EXISTE
- **Cobertura**: 0% (0/5)
- **Impacto**: ALTO - Gestão de pátio

---

### 26. ❌ DockDoorsController (5 endpoints)
**Backend**: `/api/dockdoors`
- POST `/api/dockdoors`
- GET `/api/dockdoors/{id}`
- GET `/api/dockdoors/warehouse/{warehouseId}`
- PUT `/api/dockdoors/{id}`
- DELETE `/api/dockdoors/{id}`

**Frontend**: ❌ NÃO EXISTE
- **Cobertura**: 0% (0/5)
- **Impacto**: MÉDIO - Operação de docas

---

## 📈 ANÁLISE ESTATÍSTICA FINAL

### Cobertura por Controller
| Status | Controllers | % |
|--------|-------------|---|
| ✅ 100% implementado | 3 | 12% |
| ✅ 67-99% implementado | 7 | 27% |
| ❌ 0% implementado | 16 | 61% |

### Endpoints
| Categoria | Quantidade | % |
|-----------|------------|---|
| Total API | 156 | 100% |
| Consumidos | 52 | 33% |
| NÃO consumidos | 104 | 67% |

### Componentes Frontend
| Tipo | Existentes | Necessários | Gap |
|------|------------|-------------|-----|
| Services | 10 | 26 | 16 faltando |
| List Components | 14 | 26 | 12 faltando |
| Create Modals | 6 | 20 | 14 faltando |
| Edit Modals | 6 | 20 | 14 faltando |

---

## 🎯 PRIORIDADES DE IMPLEMENTAÇÃO

### CRÍTICO (Impede uso do sistema)
1. **Users** - 0% (7 endpoints)
2. **Receipts** - 0% (3 endpoints)
3. **PutawayTasks** - 0% (5 endpoints)
4. **PickingWaves** - 0% (4 endpoints)
5. **InboundShipments** - 0% (6 endpoints) - Componente existe mas vazio
6. **Lots** - 0% (5 endpoints)
7. **SerialNumbers** - 0% (6 endpoints)

### ALTO (Funcionalidades importantes)
8. **WarehouseZones** - 0% (5 endpoints)
9. **StockMovements** - 0% (4 endpoints)
10. **CycleCounts** - 0% (4 endpoints)
11. **VehicleAppointments** - 0% (5 endpoints)
12. **Orders** - 67% - Falta criação

### MÉDIO (Completar existentes)
13. **Companies** - 40% - Faltam modais
14. **StorageLocations** - 67% - Faltam modais
15. **Inventory** - 33% - Falta CRUD completo
16. **Products** - 71% - Falta busca por SKU
17. **Vehicles** - 83% - Falta update status
18. **Drivers** - 83% - Falta update license

---

## 📋 PRÓXIMOS PASSOS

### FASE 1: Completar CRUD Existentes (2 dias)
- Companies: modais create/edit
- StorageLocations: modais create/edit
- Orders: modal create com items
- Inventory: ajustes de estoque
- Products: busca por SKU
- Vehicles: update status
- Drivers: update license

### FASE 2: Módulos Críticos WMS (5 dias)
- Users: service + list + modais
- InboundShipments: service + funcionalidades
- OutboundShipments: service + funcionalidades
- Receipts: service + list + modais
- PutawayTasks: service + list + gestão

### FASE 3: Rastreabilidade (3 dias)
- Lots: service + list + modais
- SerialNumbers: service + list + modais
- StockMovements: service + list + visualização

### FASE 4: Operações Avançadas (3 dias)
- PickingWaves: service + list + release/complete
- PackingTasks: service + funcionalidades
- Packages: service + list + modais
- VehicleAppointments: service + checkin/checkout
- CycleCounts: service + list + complete

### FASE 5: Complementos (2 dias)
- WarehouseZones: service + list + modais
- DockDoors: service + list + modais

**TOTAL ESTIMADO: 15 dias de desenvolvimento**

---

✅ **ANÁLISE CONCLUÍDA**
