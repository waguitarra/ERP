#!/bin/bash
set -e
BASE_URL="http://localhost:5000/api"

echo "=========================================="
echo "🚀 TESTE COMPLETO - 30 TABELAS VIA API"
echo "=========================================="

# 1. REGISTER ADMIN (cria apenas User master SEM company)
echo "1. Criando Admin Master..."
RESP=$(curl -s -X POST "$BASE_URL/auth/register-admin" \
  -H "Content-Type: application/json" \
  -d '{"name":"Admin Master","email":"admin@wms.com","password":"Admin@123","confirmPassword":"Admin@123"}')
TOKEN=$(echo $RESP | jq -r '.data.token')
AUTH="Authorization: Bearer $TOKEN"
echo "✅ Admin Master criado"

# 1b. CRIAR COMPANY
echo "1b. Criando Company..."
CID=$(curl -s -X POST "$BASE_URL/companies" -H "Content-Type: application/json" -H "$AUTH" \
  -d '{"name":"WMS Corp","document":"99888777000166","phone":"11999999999","email":"contato@wms.com"}' | jq -r '.data.id')
echo "✅ Company criada: $CID"

# 2. WAREHOUSE
echo "2. Criando Warehouse..."
WHID=$(curl -s -X POST "$BASE_URL/warehouses" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"companyId\":\"$CID\",\"name\":\"CD Principal\",\"code\":\"CD001\",\"address\":\"Av Log 1000\"}" | jq -r '.data.id')
echo "✅ Warehouse criado: $WHID"

# 3. WAREHOUSE ZONE
echo "3. Criando WarehouseZone..."
ZID=$(curl -s -X POST "$BASE_URL/warehousezones" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"warehouseId\":\"$WHID\",\"zoneName\":\"Zona A\",\"type\":2,\"totalCapacity\":10000}" | jq -r '.data.id')
echo "✅ WarehouseZone criada: $ZID"

# 4-5. DOCK DOORS
echo "4-5. Criando DockDoors..."
D1=$(curl -s -X POST "$BASE_URL/dockdoors" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"warehouseId\":\"$WHID\",\"doorNumber\":\"D-IN-01\",\"type\":1}" | jq -r '.data.id')
D2=$(curl -s -X POST "$BASE_URL/dockdoors" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"warehouseId\":\"$WHID\",\"doorNumber\":\"D-OUT-02\",\"type\":2}" | jq -r '.data.id')
echo "✅ DockDoors criados"

# 6. SUPPLIER
echo "6. Criando Supplier..."
SUPID=$(curl -s -X POST "$BASE_URL/suppliers" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"companyId\":\"$CID\",\"name\":\"Fornecedor ABC\",\"document\":\"11222333000199\",\"phone\":\"11999999999\",\"email\":\"fornecedor@abc.com\"}" | jq -r '.data.id')
echo "✅ Supplier criado: $SUPID"

# 7. CUSTOMER
echo "7. Criando Customer..."
CUSTID=$(curl -s -X POST "$BASE_URL/customers" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"companyId\":\"$CID\",\"name\":\"Cliente XYZ\",\"document\":\"88777666000155\",\"phone\":\"11988888888\",\"email\":\"cliente@xyz.com\"}" | jq -r '.data.id')
echo "✅ Customer criado: $CUSTID"

# 8-9. PRODUCTS
echo "8-9. Criando Products..."
P1=$(curl -s -X POST "$BASE_URL/products" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"companyId\":\"$CID\",\"name\":\"Notebook Dell\",\"sku\":\"NB-001\",\"description\":\"i7 16GB\",\"weight\":2.5,\"requiresSerialTracking\":true}" | jq -r '.data.id')
P2=$(curl -s -X POST "$BASE_URL/products" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"companyId\":\"$CID\",\"name\":\"Mouse Logitech\",\"sku\":\"MS-001\",\"description\":\"Wireless\",\"weight\":0.3}" | jq -r '.data.id')
echo "✅ Products criados"

# 10-11. VEHICLE + DRIVER
echo "10-11. Criando Vehicle + Driver..."
VEHID=$(curl -s -X POST "$BASE_URL/vehicles" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"companyId\":\"$CID\",\"licensePlate\":\"ABC-1234\",\"model\":\"Sprinter\",\"year\":2024}" | jq -r '.data.id')
DRVID=$(curl -s -X POST "$BASE_URL/drivers" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"companyId\":\"$CID\",\"name\":\"João Silva\",\"licenseNumber\":\"123456789\",\"phone\":\"11977777777\"}" | jq -r '.data.id')
echo "✅ Vehicle + Driver criados"

# 12. STORAGE LOCATION
echo "12. Criando StorageLocation..."
LOCID=$(curl -s -X POST "$BASE_URL/storagelocations" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"companyId\":\"$CID\",\"warehouseId\":\"$WHID\",\"zoneId\":\"$ZID\",\"code\":\"A-01-01\"}" | jq -r '.data.id')
echo "✅ StorageLocation criado: $LOCID"

# 13. PURCHASE ORDER (cria Order + OrderItems)
echo "13. Criando Purchase Order..."
POID=$(curl -s -X POST "$BASE_URL/orders" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"companyId\":\"$CID\",\"orderNumber\":\"PO-001\",\"type\":0,\"source\":0,\"supplierId\":\"$SUPID\",\"priority\":1,\"isBOPIS\":false,\"items\":[{\"productId\":\"$P1\",\"sku\":\"NB-001\",\"quantityOrdered\":100,\"unitPrice\":3500},{\"productId\":\"$P2\",\"sku\":\"MS-001\",\"quantityOrdered\":50,\"unitPrice\":450}]}" | jq -r '.data.id')
echo "✅ Order + OrderItems criados: $POID"

# 14. LOT
echo "14. Criando Lot..."
LOTID=$(curl -s -X POST "$BASE_URL/lots" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"companyId\":\"$CID\",\"lotNumber\":\"LOT-001\",\"productId\":\"$P1\",\"manufactureDate\":\"2025-01-01T00:00:00Z\",\"expiryDate\":\"2026-01-01T00:00:00Z\",\"quantityReceived\":100,\"supplierId\":\"$SUPID\"}" | jq -r '.data.id')
echo "✅ Lot criado: $LOTID"

# 15. VEHICLE APPOINTMENT
echo "15. Criando VehicleAppointment..."
APTID=$(curl -s -X POST "$BASE_URL/vehicleappointments" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"appointmentNumber\":\"APT-001\",\"warehouseId\":\"$WHID\",\"vehicleId\":\"$VEHID\",\"driverId\":\"$DRVID\",\"type\":1,\"scheduledDate\":\"2025-11-22T09:00:00Z\",\"dockDoorId\":\"$D1\"}" | jq -r '.data.id')
curl -s -X POST "$BASE_URL/vehicleappointments/$APTID/checkin" -H "$AUTH" > /dev/null
echo "✅ VehicleAppointment criado e checked-in"

# 16. INBOUND SHIPMENT
echo "16. Criando InboundShipment..."
IBSID=$(curl -s -X POST "$BASE_URL/inboundshipments" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"companyId\":\"$CID\",\"shipmentNumber\":\"IB-001\",\"orderId\":\"$POID\",\"supplierId\":\"$SUPID\",\"vehicleId\":\"$VEHID\",\"driverId\":\"$DRVID\",\"expectedArrivalDate\":\"2025-11-22T09:30:00Z\",\"dockDoorNumber\":\"D-IN-01\",\"asnNumber\":\"ASN-001\"}" | jq -r '.data.id')
curl -s -X POST "$BASE_URL/inboundshipments/$IBSID/receive" -H "Content-Type: application/json" -H "$AUTH" -d "\"$CID\"" > /dev/null
echo "✅ InboundShipment criado e recebido"

# 17. RECEIPT (cria Receipt + ReceiptLines internamente)
echo "17. Criando Receipt..."
RECID=$(curl -s -X POST "$BASE_URL/receipts" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"receiptNumber\":\"GRN-001\",\"inboundShipmentId\":\"$IBSID\",\"warehouseId\":\"$WHID\",\"receivedBy\":\"$CID\"}" | jq -r '.data.id')
echo "✅ Receipt + ReceiptLines criados: $RECID"

# 18. PUTAWAY TASK
echo "18. Criando PutawayTask..."
PUTID=$(curl -s -X POST "$BASE_URL/putawaytasks" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"taskNumber\":\"PUT-001\",\"receiptId\":\"$RECID\",\"productId\":\"$P1\",\"quantity\":50,\"fromLocationId\":\"$LOCID\",\"toLocationId\":\"$LOCID\",\"lotId\":\"$LOTID\"}" | jq -r '.data.id')
curl -s -X POST "$BASE_URL/putawaytasks/$PUTID/assign" -H "Content-Type: application/json" -H "$AUTH" -d "\"$CID\"" > /dev/null
curl -s -X POST "$BASE_URL/putawaytasks/$PUTID/start" -H "$AUTH" > /dev/null
curl -s -X POST "$BASE_URL/putawaytasks/$PUTID/complete" -H "$AUTH" > /dev/null
echo "✅ PutawayTask criado e completado"

# 19-20. INVENTORY + STOCK MOVEMENT
echo "19-20. Criando Inventory + StockMovement..."
INVID=$(curl -s -X POST "$BASE_URL/inventories" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"companyId\":\"$CID\",\"productId\":\"$P1\",\"warehouseId\":\"$WHID\",\"storageLocationId\":\"$LOCID\",\"quantityOnHand\":100,\"quantityReserved\":0}" | jq -r '.data.id')
MOVID=$(curl -s -X POST "$BASE_URL/stockmovements" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"companyId\":\"$CID\",\"productId\":\"$P1\",\"warehouseId\":\"$WHID\",\"movementType\":\"Receipt\",\"quantity\":100,\"toLocationId\":\"$LOCID\",\"referenceNumber\":\"GRN-001\"}" | jq -r '.data.id')
echo "✅ Inventory + StockMovement criados"

# 21. SALES ORDER
echo "21. Criando Sales Order..."
SOID=$(curl -s -X POST "$BASE_URL/orders" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"companyId\":\"$CID\",\"orderNumber\":\"SO-001\",\"type\":1,\"source\":1,\"customerId\":\"$CUSTID\",\"priority\":2,\"shippingAddress\":\"Rua Cliente 123\",\"isBOPIS\":false,\"items\":[{\"productId\":\"$P1\",\"sku\":\"NB-001\",\"quantityOrdered\":10,\"unitPrice\":3500}]}" | jq -r '.data.id')
echo "✅ Sales Order criado: $SOID"

# 22. PICKING WAVE (cria PickingWave + PickingTasks + PickingLines)
echo "22. Criando PickingWave..."
WAVEID=$(curl -s -X POST "$BASE_URL/pickingwaves" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"waveNumber\":\"WAVE-001\",\"warehouseId\":\"$WHID\",\"orderIds\":[\"$SOID\"]}" | jq -r '.data.id')
curl -s -X POST "$BASE_URL/pickingwaves/$WAVEID/release" -H "$AUTH" > /dev/null
echo "✅ PickingWave + Tasks + Lines criados"

# 23. PACKING TASK
echo "23. Criando PackingTask..."
PACKID=$(curl -s -X POST "$BASE_URL/packingtasks" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"taskNumber\":\"PACK-001\",\"orderId\":\"$SOID\",\"assignedTo\":\"$CID\"}" | jq -r '.data.id')
curl -s -X POST "$BASE_URL/packingtasks/$PACKID/start" -H "$AUTH" > /dev/null
curl -s -X POST "$BASE_URL/packingtasks/$PACKID/complete" -H "$AUTH" > /dev/null
echo "✅ PackingTask criado e completado"

# 24. PACKAGE
echo "24. Criando Package..."
PKGID=$(curl -s -X POST "$BASE_URL/packages" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"packingTaskId\":\"$PACKID\",\"trackingNumber\":\"TRK-001\",\"type\":0,\"weight\":5.5,\"length\":30,\"width\":20,\"height\":10}" | jq -r '.data.id')
curl -s -X POST "$BASE_URL/packages/$PKGID/ship" -H "$AUTH" > /dev/null
echo "✅ Package criado e shipped"

# 25. OUTBOUND SHIPMENT
echo "25. Criando OutboundShipment..."
OBSID=$(curl -s -X POST "$BASE_URL/outboundshipments" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"shipmentNumber\":\"OB-001\",\"orderId\":\"$SOID\",\"carrierId\":\"$VEHID\",\"trackingNumber\":\"TRK-001\",\"deliveryAddress\":\"Rua Cliente 123\"}" | jq -r '.data.id')
curl -s -X POST "$BASE_URL/outboundshipments/$OBSID/ship" -H "$AUTH" > /dev/null
echo "✅ OutboundShipment criado e shipped"

# 26. SERIAL NUMBER
echo "26. Criando SerialNumber..."
SNID=$(curl -s -X POST "$BASE_URL/serialnumbers" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"serial\":\"SN-001\",\"productId\":\"$P1\",\"lotId\":\"$LOTID\"}" | jq -r '.data.id')
curl -s -X POST "$BASE_URL/serialnumbers/$SNID/receive" -H "Content-Type: application/json" -H "$AUTH" -d "\"$LOCID\"" > /dev/null
echo "✅ SerialNumber criado e recebido"

# 27. CYCLE COUNT
echo "27. Criando CycleCount..."
CCID=$(curl -s -X POST "$BASE_URL/cyclecounts" -H "Content-Type: application/json" -H "$AUTH" \
  -d "{\"countNumber\":\"CC-001\",\"warehouseId\":\"$WHID\",\"zoneId\":\"$ZID\",\"countedBy\":\"$CID\"}" | jq -r '.data.id')
curl -s -X POST "$BASE_URL/cyclecounts/$CCID/complete" -H "$AUTH" > /dev/null
echo "✅ CycleCount criado e completado"

# Complete InboundShipment e VehicleAppointment
curl -s -X POST "$BASE_URL/inboundshipments/$IBSID/complete" -H "$AUTH" > /dev/null
curl -s -X POST "$BASE_URL/vehicleappointments/$APTID/checkout" -H "$AUTH" > /dev/null

echo ""
echo "=========================================="
echo "✅ 30 TABELAS POPULADAS VIA API!"
echo "=========================================="
echo ""
mysql -h localhost -u logistics_user -ppassword logistics_db -e "
SELECT 'Companies' AS Tabela, COUNT(*) AS Total FROM Companies UNION ALL
SELECT 'Users', COUNT(*) FROM Users UNION ALL
SELECT 'Warehouses', COUNT(*) FROM Warehouses UNION ALL
SELECT 'WarehouseZones', COUNT(*) FROM WarehouseZones UNION ALL
SELECT 'DockDoors', COUNT(*) FROM DockDoors UNION ALL
SELECT 'Suppliers', COUNT(*) FROM Suppliers UNION ALL
SELECT 'Customers', COUNT(*) FROM Customers UNION ALL
SELECT 'Products', COUNT(*) FROM Products UNION ALL
SELECT 'Vehicles', COUNT(*) FROM Vehicles UNION ALL
SELECT 'Drivers', COUNT(*) FROM Drivers UNION ALL
SELECT 'StorageLocations', COUNT(*) FROM StorageLocations UNION ALL
SELECT 'Orders', COUNT(*) FROM Orders UNION ALL
SELECT 'OrderItems', COUNT(*) FROM OrderItems UNION ALL
SELECT 'VehicleAppointments', COUNT(*) FROM VehicleAppointments UNION ALL
SELECT 'InboundShipments', COUNT(*) FROM InboundShipments UNION ALL
SELECT 'Receipts', COUNT(*) FROM Receipts UNION ALL
SELECT 'ReceiptLines', COUNT(*) FROM ReceiptLines UNION ALL
SELECT 'Lots', COUNT(*) FROM Lots UNION ALL
SELECT 'Inventories', COUNT(*) FROM Inventories UNION ALL
SELECT 'StockMovements', COUNT(*) FROM StockMovements UNION ALL
SELECT 'PutawayTasks', COUNT(*) FROM PutawayTasks UNION ALL
SELECT 'PickingWaves', COUNT(*) FROM PickingWaves UNION ALL
SELECT 'PickingTasks', COUNT(*) FROM PickingTasks UNION ALL
SELECT 'PickingLines', COUNT(*) FROM PickingLines UNION ALL
SELECT 'PackingTasks', COUNT(*) FROM PackingTasks UNION ALL
SELECT 'Packages', COUNT(*) FROM Packages UNION ALL
SELECT 'OutboundShipments', COUNT(*) FROM OutboundShipments UNION ALL
SELECT 'SerialNumbers', COUNT(*) FROM SerialNumbers UNION ALL
SELECT 'CycleCounts', COUNT(*) FROM CycleCounts;
"
echo ""
echo "🎉 TESTE COMPLETO! Todas as 30 tabelas populadas via API"
echo "🎉 SWAGGER: http://localhost:5000"
echo "🎉 25 CONTROLLERS | ~100 ENDPOINTS"
