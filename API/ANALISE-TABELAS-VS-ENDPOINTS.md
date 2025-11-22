# ANÁLISE: TABELAS vs ENDPOINTS

## 30 TABELAS MYSQL
1. Companies
2. Users
3. Warehouses
4. WarehouseZones
5. DockDoors
6. Suppliers
7. Customers
8. Products
9. Vehicles
10. Drivers
11. StorageLocations
12. Orders
13. OrderItems
14. VehicleAppointments
15. InboundShipments
16. Receipts
17. ReceiptLines
18. Lots
19. Inventories
20. StockMovements
21. PutawayTasks
22. PickingWaves
23. PickingTasks
24. PickingLines
25. PackingTasks
26. Packages
27. OutboundShipments
28. SerialNumbers
29. CycleCounts

## 18 CONTROLLERS EXISTENTES
1. ✅ AuthController
2. ✅ CompaniesController
3. ✅ CustomersController
4. ✅ DockDoorsController
5. ✅ DriversController
6. ✅ InboundShipmentsController
7. ✅ InventoriesController
8. ✅ OrdersController
9. ✅ PickingWavesController
10. ✅ ProductsController
11. ✅ ReceiptsController
12. ✅ StockMovementsController
13. ✅ StorageLocationsController
14. ✅ SuppliersController
15. ✅ VehicleAppointmentsController
16. ✅ VehiclesController
17. ✅ WarehouseZonesController
18. ✅ WarehousesController

## ❌ TABELAS SEM ENDPOINTS (12 faltando)
1. **OrderItems** - Items de pedidos (FK: Orders)
2. **ReceiptLines** - Linhas de recebimento (FK: Receipts)
3. **Lots** - Lotes de produtos
4. **PutawayTasks** - Tarefas de armazenamento
5. **PickingTasks** - Tarefas de separação (FK: PickingWaves)
6. **PickingLines** - Linhas de separação (FK: PickingTasks)
7. **PackingTasks** - Tarefas de embalagem
8. **Packages** - Pacotes
9. **OutboundShipments** - Expedições saída
10. **SerialNumbers** - Números de série
11. **CycleCounts** - Contagens cíclicas
12. **Users** - (tem no banco mas não no Controllers)

## PRÓXIMOS PASSOS
1. Criar Services para as 12 tabelas faltantes
2. Criar DTOs (Request/Response)
3. Criar Controllers
4. Validar no Swagger
5. Criar script curl testando TUDO
