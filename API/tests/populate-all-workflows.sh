#!/bin/bash
# Script para popular TODAS as tabelas de workflows do WMS
set -e

API="http://localhost:5000/api"

echo "=========================================="
echo "  POPULAR TODOS OS WORKFLOWS DO WMS"
echo "=========================================="
echo ""

# Login
TOKEN=$(curl -s -X POST "$API/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@wms.com","password":"Admin@123"}' \
  | jq -r '.data.token')

echo "✓ Token obtido"

# Obter IDs via API
COMPANY_ID=$(curl -s "$API/companies" -H "Authorization: Bearer $TOKEN" | jq -r '.data[0].id')
WAREHOUSE_ID=$(curl -s "$API/warehouses" -H "Authorization: Bearer $TOKEN" | jq -r '.data[0].id')
SUPPLIER_ID=$(curl -s "$API/suppliers" -H "Authorization: Bearer $TOKEN" | jq -r '.data[0].id')
CUSTOMER_ID=$(curl -s "$API/customers" -H "Authorization: Bearer $TOKEN" | jq -r '.data[0].id')
VEHICLE_ID=$(curl -s "$API/vehicles" -H "Authorization: Bearer $TOKEN" | jq -r '.data[0].id')
DRIVER_ID=$(curl -s "$API/drivers" -H "Authorization: Bearer $TOKEN" | jq -r '.data[0].id')
PRODUCT_ID=$(curl -s "$API/products" -H "Authorization: Bearer $TOKEN" | jq -r '.data[0].id')
ZONE_ID=$(curl -s "$API/warehousezones" -H "Authorization: Bearer $TOKEN" | jq -r '.data[0].id')
USER_ID=$(curl -s "$API/users" -H "Authorization: Bearer $TOKEN" | jq -r '.data[1].id')

LOCATIONS=($(curl -s "$API/storagelocations" -H "Authorization: Bearer $TOKEN" | jq -r '.data[].id' | head -5))
LOC1=${LOCATIONS[0]}
LOC2=${LOCATIONS[1]}

echo "✓ IDs obtidos"
echo ""

# Arrays para guardar IDs
ORDERS_IN=()
ORDERS_OUT=()
INBOUNDS=()
RECEIPTS=()
WAVES=()
PACKINGS=()

# ==========================================
# CRIAR 30 ORDERS INBOUND
# ==========================================
echo "=== CRIANDO 30 ORDERS INBOUND ==="
for i in {1..30}; do
  ORDER=$(curl -s -X POST "$API/orders" \
    -H "Authorization: Bearer $TOKEN" \
    -H "Content-Type: application/json" \
    -d "{\"companyId\":\"$COMPANY_ID\",\"orderNumber\":\"PO-2025-$(printf '%04d' $i)\",\"type\":1,\"source\":1,\"supplierId\":\"$SUPPLIER_ID\",\"expectedDate\":\"2025-12-25T10:00:00Z\",\"priority\":1,\"isBOPIS\":false,\"items\":[{\"productId\":\"$PRODUCT_ID\",\"sku\":\"SKU-$i\",\"quantityOrdered\":$((100+i)),\"unitPrice\":50}]}" \
    | jq -r '.id // empty')
  
  if [ -n "$ORDER" ]; then
    ORDERS_IN+=("$ORDER")
    echo "✓ Order Inbound $i"
  fi
done
echo ""

# ==========================================
# CRIAR 30 INBOUND SHIPMENTS
# ==========================================
echo "=== CRIANDO 30 INBOUND SHIPMENTS ==="
for i in {1..30}; do
  if [ $i -le ${#ORDERS_IN[@]} ]; then
    ORDER_ID=${ORDERS_IN[$((i-1))]}
    
    INBOUND=$(curl -s -X POST "$API/inboundshipments" \
      -H "Authorization: Bearer $TOKEN" \
      -H "Content-Type: application/json" \
      -d "{\"companyId\":\"$COMPANY_ID\",\"shipmentNumber\":\"ISH-$(printf '%04d' $i)\",\"orderId\":\"$ORDER_ID\",\"supplierId\":\"$SUPPLIER_ID\",\"vehicleId\":\"$VEHICLE_ID\",\"driverId\":\"$DRIVER_ID\"}" \
      | jq -r '.id // empty')
    
    if [ -n "$INBOUND" ]; then
      INBOUNDS+=("$INBOUND")
      echo "✓ InboundShipment $i"
    fi
  fi
done
echo ""

# ==========================================
# CRIAR 30 RECEIPTS
# ==========================================
echo "=== CRIANDO 30 RECEIPTS ==="
for i in {1..30}; do
  if [ $i -le ${#INBOUNDS[@]} ]; then
    INBOUND_ID=${INBOUNDS[$((i-1))]}
    
    RECEIPT=$(curl -s -X POST "$API/receipts" \
      -H "Authorization: Bearer $TOKEN" \
      -H "Content-Type: application/json" \
      -d "{\"receiptNumber\":\"REC-$(printf '%04d' $i)\",\"inboundShipmentId\":\"$INBOUND_ID\",\"warehouseId\":\"$WAREHOUSE_ID\",\"receivedBy\":\"$USER_ID\"}" \
      | jq -r '.id // empty')
    
    if [ -n "$RECEIPT" ]; then
      RECEIPTS+=("$RECEIPT")
      echo "✓ Receipt $i"
    fi
  fi
done
echo ""

# ==========================================
# CRIAR 30 PUTAWAY TASKS
# ==========================================
echo "=== CRIANDO 30 PUTAWAY TASKS ==="
for i in {1..30}; do
  if [ $i -le ${#RECEIPTS[@]} ]; then
    RECEIPT_ID=${RECEIPTS[$((i-1))]}
    
    PUTAWAY=$(curl -s -X POST "$API/putawaytasks" \
      -H "Authorization: Bearer $TOKEN" \
      -H "Content-Type: application/json" \
      -d "{\"taskNumber\":\"PUT-$(printf '%04d' $i)\",\"receiptId\":\"$RECEIPT_ID\",\"productId\":\"$PRODUCT_ID\",\"quantity\":$((50+i)),\"fromLocationId\":\"$LOC1\",\"toLocationId\":\"$LOC2\"}" \
      | jq -r '.id // empty')
    
    if [ -n "$PUTAWAY" ]; then
      echo "✓ PutawayTask $i"
    fi
  fi
done
echo ""

# ==========================================
# CRIAR 30 ORDERS OUTBOUND
# ==========================================
echo "=== CRIANDO 30 ORDERS OUTBOUND ==="
for i in {1..30}; do
  ORDER=$(curl -s -X POST "$API/orders" \
    -H "Authorization: Bearer $TOKEN" \
    -H "Content-Type: application/json" \
    -d "{\"companyId\":\"$COMPANY_ID\",\"orderNumber\":\"SO-2025-$(printf '%04d' $i)\",\"type\":2,\"source\":3,\"customerId\":\"$CUSTOMER_ID\",\"expectedDate\":\"2025-12-26T14:00:00Z\",\"priority\":2,\"shippingAddress\":\"Rua Cliente $i\",\"isBOPIS\":false,\"items\":[{\"productId\":\"$PRODUCT_ID\",\"sku\":\"SKU-OUT-$i\",\"quantityOrdered\":$((10+i)),\"unitPrice\":75}]}" \
    | jq -r '.id // empty')
  
  if [ -n "$ORDER" ]; then
    ORDERS_OUT+=("$ORDER")
    echo "✓ Order Outbound $i"
  fi
done
echo ""

# ==========================================
# CRIAR 20 PICKING WAVES
# ==========================================
echo "=== CRIANDO 20 PICKING WAVES ==="
for i in {1..20}; do
  start=$((i*3-2))
  ORDER_IDS="[\"${ORDERS_OUT[$start]}\",\"${ORDERS_OUT[$((start+1))]}\",\"${ORDERS_OUT[$((start+2))]}\"]"
  
  WAVE=$(curl -s -X POST "$API/pickingwaves" \
    -H "Authorization: Bearer $TOKEN" \
    -H "Content-Type: application/json" \
    -d "{\"waveNumber\":\"WAVE-$(printf '%04d' $i)\",\"warehouseId\":\"$WAREHOUSE_ID\",\"orderIds\":$ORDER_IDS}" \
    | jq -r '.id // empty')
  
  if [ -n "$WAVE" ]; then
    WAVES+=("$WAVE")
    echo "✓ PickingWave $i"
  fi
done
echo ""

# ==========================================
# CRIAR 25 PACKING TASKS
# ==========================================
echo "=== CRIANDO 25 PACKING TASKS ==="
for i in {1..25}; do
  if [ $i -le ${#ORDERS_OUT[@]} ]; then
    ORDER_ID=${ORDERS_OUT[$((i-1))]}
    
    PACKING=$(curl -s -X POST "$API/packingtasks" \
      -H "Authorization: Bearer $TOKEN" \
      -H "Content-Type: application/json" \
      -d "{\"taskNumber\":\"PACK-$(printf '%04d' $i)\",\"orderId\":\"$ORDER_ID\",\"assignedTo\":\"$USER_ID\"}" \
      | jq -r '.id // empty')
    
    if [ -n "$PACKING" ]; then
      PACKINGS+=("$PACKING")
      echo "✓ PackingTask $i"
    fi
  fi
done
echo ""

# ==========================================
# CRIAR 25 PACKAGES
# ==========================================
echo "=== CRIANDO 25 PACKAGES ==="
for i in {1..25}; do
  if [ $i -le ${#PACKINGS[@]} ]; then
    PACKING_ID=${PACKINGS[$((i-1))]}
    
    PACKAGE=$(curl -s -X POST "$API/packages" \
      -H "Authorization: Bearer $TOKEN" \
      -H "Content-Type: application/json" \
      -d "{\"packingTaskId\":\"$PACKING_ID\",\"trackingNumber\":\"PKG-$(printf '%08d' $i)\",\"type\":1,\"weight\":$((5+i)),\"length\":40,\"width\":30,\"height\":20}" \
      | jq -r '.id // empty')
    
    if [ -n "$PACKAGE" ]; then
      echo "✓ Package $i"
    fi
  fi
done
echo ""

# ==========================================
# CRIAR 25 OUTBOUND SHIPMENTS
# ==========================================
echo "=== CRIANDO 25 OUTBOUND SHIPMENTS ==="
for i in {1..25}; do
  if [ $i -le ${#ORDERS_OUT[@]} ]; then
    ORDER_ID=${ORDERS_OUT[$((i-1))]}
    
    OUTBOUND=$(curl -s -X POST "$API/outboundshipments" \
      -H "Authorization: Bearer $TOKEN" \
      -H "Content-Type: application/json" \
      -d "{\"shipmentNumber\":\"OSH-$(printf '%04d' $i)\",\"orderId\":\"$ORDER_ID\",\"carrierId\":\"$SUPPLIER_ID\",\"trackingNumber\":\"TRK-$(printf '%08d' $i)\",\"deliveryAddress\":\"Endereço Cliente $i\"}" \
      | jq -r '.id // empty')
    
    if [ -n "$OUTBOUND" ]; then
      echo "✓ OutboundShipment $i"
    fi
  fi
done
echo ""

# ==========================================
# CRIAR 10 CYCLE COUNTS
# ==========================================
echo "=== CRIANDO 10 CYCLE COUNTS ==="
for i in {1..10}; do
  CYCLE=$(curl -s -X POST "$API/cyclecounts" \
    -H "Authorization: Bearer $TOKEN" \
    -H "Content-Type: application/json" \
    -d "{\"countNumber\":\"CC-$(printf '%04d' $i)\",\"warehouseId\":\"$WAREHOUSE_ID\",\"zoneId\":\"$ZONE_ID\",\"countedBy\":\"$USER_ID\"}" \
    | jq -r '.id // empty')
  
  if [ -n "$CYCLE" ]; then
    echo "✓ CycleCount $i"
  fi
done
echo ""

# ==========================================
# RELATÓRIO FINAL COMPLETO
# ==========================================
echo "=========================================="
echo "  RELATÓRIO FINAL - TODAS AS 29 TABELAS"
echo "=========================================="
echo ""

mysql -u logistics_user -ppassword -D logistics_db -e "
SELECT 
    'OrderItems' as Tabela, COUNT(*) as Total FROM OrderItems
UNION ALL SELECT 'Orders', COUNT(*) FROM Orders
UNION ALL SELECT 'StockMovements', COUNT(*) FROM StockMovements
UNION ALL SELECT 'Products', COUNT(*) FROM Products
UNION ALL SELECT 'SerialNumbers', COUNT(*) FROM SerialNumbers
UNION ALL SELECT 'Lots', COUNT(*) FROM Lots
UNION ALL SELECT 'StorageLocations', COUNT(*) FROM StorageLocations
UNION ALL SELECT 'Inventories', COUNT(*) FROM Inventories
UNION ALL SELECT 'Suppliers', COUNT(*) FROM Suppliers
UNION ALL SELECT 'Customers', COUNT(*) FROM Customers
UNION ALL SELECT 'Vehicles', COUNT(*) FROM Vehicles
UNION ALL SELECT 'Drivers', COUNT(*) FROM Drivers
UNION ALL SELECT 'Users', COUNT(*) FROM Users
UNION ALL SELECT 'WarehouseZones', COUNT(*) FROM WarehouseZones
UNION ALL SELECT 'DockDoors', COUNT(*) FROM DockDoors
UNION ALL SELECT 'Warehouses', COUNT(*) FROM Warehouses
UNION ALL SELECT 'Companies', COUNT(*) FROM Companies
UNION ALL SELECT 'InboundShipments', COUNT(*) FROM InboundShipments
UNION ALL SELECT 'Receipts', COUNT(*) FROM Receipts
UNION ALL SELECT 'ReceiptLines', COUNT(*) FROM ReceiptLines
UNION ALL SELECT 'PutawayTasks', COUNT(*) FROM PutawayTasks
UNION ALL SELECT 'PickingWaves', COUNT(*) FROM PickingWaves
UNION ALL SELECT 'PickingTasks', COUNT(*) FROM PickingTasks
UNION ALL SELECT 'PickingLines', COUNT(*) FROM PickingLines
UNION ALL SELECT 'PackingTasks', COUNT(*) FROM PackingTasks
UNION ALL SELECT 'Packages', COUNT(*) FROM Packages
UNION ALL SELECT 'OutboundShipments', COUNT(*) FROM OutboundShipments
UNION ALL SELECT 'VehicleAppointments', COUNT(*) FROM VehicleAppointments
UNION ALL SELECT 'CycleCounts', COUNT(*) FROM CycleCounts
ORDER BY Total DESC;
" 2>/dev/null

echo ""
echo "=========================================="
echo "✅ TODAS AS TABELAS POPULADAS!"
echo "=========================================="
