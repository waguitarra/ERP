# MAPEAMENTO COMPLETO - 30 ENTIDADES

## ✅ ENTIDADES COM CONTROLLER PRÓPRIO (18)
1. **Company** → CompaniesController
2. **User** → AuthController (register/login)
3. **Warehouse** → WarehousesController
4. **WarehouseZone** → WarehouseZonesController
5. **DockDoor** → DockDoorsController
6. **Supplier** → SuppliersController
7. **Customer** → CustomersController
8. **Product** → ProductsController
9. **Vehicle** → VehiclesController
10. **Driver** → DriversController
11. **StorageLocation** → StorageLocationsController
12. **Order** → OrdersController
13. **VehicleAppointment** → VehicleAppointmentsController
14. **InboundShipment** → InboundShipmentsController
15. **Receipt** → ReceiptsController
16. **Inventory** → InventoriesController
17. **StockMovement** → StockMovementsController
18. **PickingWave** → PickingWavesController

## 🔗 ENTIDADES CRIADAS VIA RELACIONAMENTO (6)
19. **OrderItem** → Criado via CreateOrderRequest.Items (no OrdersController)
20. **ReceiptLine** → Criado via Receipt (no ReceiptsController) 
21. **PickingTask** → Criado via PickingWave
22. **PickingLine** → Criado via PickingTask

## ❌ ENTIDADES SEM ENDPOINT (6 faltando)
23. **Lot** - Lotes de produtos (precisa controller)
24. **PutawayTask** - Tarefas de armazenamento (precisa controller)
25. **PackingTask** - Tarefas de embalagem (precisa controller)
26. **Package** - Pacotes (precisa controller)
27. **OutboundShipment** - Expedições saída (precisa controller)
28. **SerialNumber** - Números de série (precisa controller)
29. **CycleCount** - Contagens cíclicas (precisa controller)

## AÇÕES NECESSÁRIAS
1. Criar 7 controllers faltantes (Lot, PutawayTask, PackingTask, Package, OutboundShipment, SerialNumber, CycleCount)
2. Criar Services correspondentes
3. Criar DTOs (Request/Response)
4. Registrar no Program.cs
5. Testar via curl
