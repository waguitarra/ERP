# 🚀 INSTRUÇÕES PARA INICIAR API E EXECUTAR TESTES

## ⚠️ SITUAÇÃO ATUAL
A API **NÃO está rodando**. Nenhum processo detectado na porta 5000.

## ✅ O QUE FOI FEITO
1. ✅ Criados 7 novos controllers (Lot, PutawayTask, PackingTask, Package, OutboundShipment, SerialNumber, CycleCount)
2. ✅ Criados todos Services, Repositories, DTOs e Interfaces
3. ✅ Registrados no Program.cs
4. ✅ Script de teste completo criado: `tests/curl-tests/TESTE-30-TABELAS-API.sh`

## 📋 PASSOS PARA INICIAR

### 1. INICIAR A API
```bash
cd /home/wagnerfb/Projetos/ERP/API
dotnet run --project src/Logistics.API/Logistics.API.csproj
```

Aguarde até ver:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://0.0.0.0:5000
```

### 2. VERIFICAR SE API ESTÁ RODANDO
Em outro terminal:
```bash
curl -s http://localhost:5000/swagger/v1/swagger.json | jq -r '.paths | keys[]' | wc -l
```

Deve retornar **~100 endpoints** (25 controllers).

### 3. EXECUTAR TESTE COMPLETO
```bash
cd /home/wagnerfb/Projetos/ERP/API
chmod +x tests/curl-tests/TESTE-30-TABELAS-API.sh
./tests/curl-tests/TESTE-30-TABELAS-API.sh
```

## 📊 O QUE O SCRIPT FAZ
Popula **30 tabelas** via API na seguinte ordem:

1. Company + User (via register-admin)
2. Warehouse
3. WarehouseZone
4. DockDoors (2)
5. Supplier
6. Customer
7. Products (2)
8. Vehicle + Driver
9. StorageLocation
10. Order + OrderItems (Purchase Order)
11. Lot
12. VehicleAppointment
13. InboundShipment
14. Receipt + ReceiptLines
15. PutawayTask
16. Inventory + StockMovement
17. Order (Sales Order)
18. PickingWave + PickingTasks + PickingLines
19. PackingTask
20. Package
21. OutboundShipment
22. SerialNumber
23. CycleCount

## ✅ VALIDAÇÃO FINAL
O script exibe contagem de registros em todas as 30 tabelas.

## 🔧 SE DER ERRO
1. Verifique logs da API no terminal onde rodou `dotnet run`
2. Verifique se MySQL está rodando: `mysql -h localhost -u logistics_user -ppassword logistics_db -e "SELECT 1"`
3. Se erro de compilação, rode: `dotnet build src/Logistics.API/Logistics.API.csproj`
4. Se erro em endpoint específico, veja o erro no output do script

## 📝 CONTROLLERS DISPONÍVEIS (25)
1. AuthController
2. CompaniesController
3. CustomersController
4. CycleCountsController ⭐ NOVO
5. DockDoorsController
6. DriversController
7. InboundShipmentsController
8. InventoriesController
9. LotsController ⭐ NOVO
10. OrdersController
11. OutboundShipmentsController ⭐ NOVO
12. PackagesController ⭐ NOVO
13. PackingTasksController ⭐ NOVO
14. PickingWavesController
15. ProductsController
16. PutawayTasksController ⭐ NOVO
17. ReceiptsController
18. SerialNumbersController ⭐ NOVO
19. StockMovementsController
20. StorageLocationsController
21. SuppliersController
22. VehicleAppointmentsController
23. VehiclesController
24. WarehouseZonesController
25. WarehousesController

## 🎯 RESULTADO ESPERADO
```
✅ 30 TABELAS POPULADAS VIA API!
Companies: 1
Users: 1
Warehouses: 1
WarehouseZones: 1
DockDoors: 2
Suppliers: 1
Customers: 1
Products: 2
Vehicles: 1
Drivers: 1
StorageLocations: 1
Orders: 2 (1 PO + 1 SO)
OrderItems: 3
VehicleAppointments: 1
InboundShipments: 1
Receipts: 1
ReceiptLines: variável
Lots: 1
Inventories: 1
StockMovements: 1
PutawayTasks: 1
PickingWaves: 1
PickingTasks: variável
PickingLines: variável
PackingTasks: 1
Packages: 1
OutboundShipments: 1
SerialNumbers: 1
CycleCounts: 1
```
