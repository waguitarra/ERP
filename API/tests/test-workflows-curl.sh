#!/bin/bash
# Script de teste COMPLETO para popular workflows via CURL
set -e

API="http://localhost:5000/api"

echo "=========================================="
echo "  TESTE COMPLETO DE WORKFLOWS VIA CURL"
echo "=========================================="
echo ""

# Login
echo "→ Fazendo login..."
TOKEN=$(curl -s -X POST "$API/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@wms.com","password":"Admin@123"}' \
  | jq -r '.data.token')

if [ -z "$TOKEN" ] || [ "$TOKEN" = "null" ]; then
  echo "✗ Erro ao obter token!"
  exit 1
fi

echo "✓ Token obtido: ${TOKEN:0:50}..."
echo ""

# Obter IDs do banco
echo "→ Buscando IDs do banco..."
get_id() {
  mysql -u logistics_user -ppassword -D logistics_db -N -e "
    SELECT LOWER(CONCAT(
      SUBSTR(HEX(Id), 1, 8), '-',
      SUBSTR(HEX(Id), 9, 4), '-',
      SUBSTR(HEX(Id), 13, 4), '-',
      SUBSTR(HEX(Id), 17, 4), '-',
      SUBSTR(HEX(Id), 21)
    )) FROM $1 $2 LIMIT 1;" 2>/dev/null
}

COMPANY_ID=$(get_id "Companies")
WAREHOUSE_ID=$(get_id "Warehouses")
SUPPLIER_ID=$(get_id "Suppliers")
CUSTOMER_ID=$(get_id "Customers")
VEHICLE_ID=$(get_id "Vehicles")
DRIVER_ID=$(get_id "Drivers")
USER_ID=$(get_id "Users" "WHERE Role != 0")
PRODUCT_ID=$(get_id "Products")
ZONE_ID=$(get_id "WarehouseZones")
LOC1=$(get_id "StorageLocations" "")
LOC2=$(get_id "StorageLocations" "LIMIT 1 OFFSET 1")

echo "✓ IDs obtidos"
echo ""

# =======================
# CRIAR ORDER INBOUND
# =======================
echo "=== PASSO 1: Criar Order Inbound ==="
ORDER_IN_RESP=$(curl -s -X POST "$API/orders" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"companyId\": \"$COMPANY_ID\",
    \"orderNumber\": \"PO-TEST-$(date +%s)\",
    \"type\": 1,
    \"source\": 1,
    \"supplierId\": \"$SUPPLIER_ID\",
    \"expectedDate\": \"2025-12-25T10:00:00Z\",
    \"priority\": 1,
    \"isBOPIS\": false,
    \"items\": [{
      \"productId\": \"$PRODUCT_ID\",
      \"sku\": \"SKU-TEST\",
      \"quantityOrdered\": 100.0,
      \"unitPrice\": 50.00
    }]
  }")

ORDER_IN_ID=$(echo "$ORDER_IN_RESP" | jq -r '.data.id // empty')

if [ -n "$ORDER_IN_ID" ]; then
  echo "✓ Order Inbound criado: $ORDER_IN_ID"
else
  echo "✗ Falha ao criar Order Inbound"
  echo "$ORDER_IN_RESP" | jq
  exit 1
fi
echo ""

# =======================
# INBOUND SHIPMENT
# =======================
echo "=== PASSO 2: Criar InboundShipment (TESTE CORRIGIDO) ==="
INBOUND_RESP=$(curl -s -X POST "$API/inboundshipments" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"companyId\": \"$COMPANY_ID\",
    \"shipmentNumber\": \"ISH-TEST-001\",
    \"orderId\": \"$ORDER_IN_ID\",
    \"supplierId\": \"$SUPPLIER_ID\",
    \"vehicleId\": \"$VEHICLE_ID\",
    \"driverId\": \"$DRIVER_ID\",
    \"expectedArrivalDate\": \"2025-12-20T10:00:00Z\",
    \"dockDoorNumber\": \"DOCK-01\"
  }")

INBOUND_ID=$(echo "$INBOUND_RESP" | jq -r '.id // empty')

if [ -n "$INBOUND_ID" ]; then
  echo "✅ InboundShipment criado: $INBOUND_ID"
else
  echo "✗ Falha ao criar InboundShipment"
  echo "$INBOUND_RESP" | jq
  exit 1
fi
echo ""

# =======================
# RECEIPT
# =======================
echo "=== PASSO 3: Criar Receipt (TESTE CORRIGIDO) ==="
RECEIPT_RESP=$(curl -s -X POST "$API/receipts" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"receiptNumber\": \"REC-TEST-001\",
    \"inboundShipmentId\": \"$INBOUND_ID\",
    \"warehouseId\": \"$WAREHOUSE_ID\",
    \"receivedBy\": \"$USER_ID\"
  }")

RECEIPT_ID=$(echo "$RECEIPT_RESP" | jq -r '.id // empty')

if [ -n "$RECEIPT_ID" ]; then
  echo "✅ Receipt criado: $RECEIPT_ID"
else
  echo "✗ Falha ao criar Receipt"
  echo "$RECEIPT_RESP" | jq
  exit 1
fi
echo ""

# =======================
# PUTAWAY TASK
# =======================
echo "=== PASSO 4: Criar PutawayTask (TESTE CORRIGIDO) ==="
PUTAWAY_RESP=$(curl -s -X POST "$API/putawaytasks" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"taskNumber\": \"PUT-TEST-001\",
    \"receiptId\": \"$RECEIPT_ID\",
    \"productId\": \"$PRODUCT_ID\",
    \"quantity\": 50.0,
    \"fromLocationId\": \"$LOC1\",
    \"toLocationId\": \"$LOC2\"
  }")

PUTAWAY_ID=$(echo "$PUTAWAY_RESP" | jq -r '.id // empty')

if [ -n "$PUTAWAY_ID" ]; then
  echo "✅ PutawayTask criado: $PUTAWAY_ID"
else
  echo "✗ Falha ao criar PutawayTask"
  echo "$PUTAWAY_RESP" | jq
  exit 1
fi
echo ""

# =======================
# ORDER OUTBOUND
# =======================
echo "=== PASSO 5: Criar Order Outbound ==="
ORDER_OUT_RESP=$(curl -s -X POST "$API/orders" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"companyId\": \"$COMPANY_ID\",
    \"orderNumber\": \"SO-TEST-$(date +%s)\",
    \"type\": 2,
    \"source\": 3,
    \"customerId\": \"$CUSTOMER_ID\",
    \"expectedDate\": \"2025-12-26T14:00:00Z\",
    \"priority\": 2,
    \"shippingAddress\": \"Rua Teste, 100 - RJ\",
    \"isBOPIS\": false,
    \"items\": [{
      \"productId\": \"$PRODUCT_ID\",
      \"sku\": \"SKU-OUT\",
      \"quantityOrdered\": 10.0,
      \"unitPrice\": 75.00
    }]
  }")

ORDER_OUT_ID=$(echo "$ORDER_OUT_RESP" | jq -r '.data.id // empty')

if [ -n "$ORDER_OUT_ID" ]; then
  echo "✓ Order Outbound criado: $ORDER_OUT_ID"
else
  echo "✗ Falha ao criar Order Outbound"
  exit 1
fi
echo ""

# =======================
# PICKING WAVE
# =======================
echo "=== PASSO 6: Criar PickingWave (TESTE CORRIGIDO) ==="
WAVE_RESP=$(curl -s -X POST "$API/pickingwaves" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"waveNumber\": \"WAVE-TEST-001\",
    \"warehouseId\": \"$WAREHOUSE_ID\",
    \"orderIds\": [\"$ORDER_OUT_ID\"]
  }")

WAVE_ID=$(echo "$WAVE_RESP" | jq -r '.id // empty')

if [ -n "$WAVE_ID" ]; then
  echo "✅ PickingWave criado: $WAVE_ID"
else
  echo "✗ Falha ao criar PickingWave"
  echo "$WAVE_RESP" | jq
  exit 1
fi
echo ""

# =======================
# PACKING TASK
# =======================
echo "=== PASSO 7: Criar PackingTask (TESTE CORRIGIDO) ==="
PACKING_RESP=$(curl -s -X POST "$API/packingtasks" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"taskNumber\": \"PACK-TEST-001\",
    \"orderId\": \"$ORDER_OUT_ID\",
    \"assignedTo\": \"$USER_ID\"
  }")

PACKING_ID=$(echo "$PACKING_RESP" | jq -r '.id // empty')

if [ -n "$PACKING_ID" ]; then
  echo "✅ PackingTask criado: $PACKING_ID"
else
  echo "✗ Falha ao criar PackingTask"
  echo "$PACKING_RESP" | jq
  exit 1
fi
echo ""

# =======================
# PACKAGE
# =======================
echo "=== PASSO 8: Criar Package (TESTE CORRIGIDO) ==="
PACKAGE_RESP=$(curl -s -X POST "$API/packages" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"packingTaskId\": \"$PACKING_ID\",
    \"trackingNumber\": \"PKG-TEST-001\",
    \"type\": 1,
    \"weight\": 5.5,
    \"length\": 40.0,
    \"width\": 30.0,
    \"height\": 20.0
  }")

PACKAGE_ID=$(echo "$PACKAGE_RESP" | jq -r '.id // empty')

if [ -n "$PACKAGE_ID" ]; then
  echo "✅ Package criado: $PACKAGE_ID"
else
  echo "✗ Falha ao criar Package"
  echo "$PACKAGE_RESP" | jq
  exit 1
fi
echo ""

# =======================
# OUTBOUND SHIPMENT
# =======================
echo "=== PASSO 9: Criar OutboundShipment (TESTE CORRIGIDO) ==="
OUTBOUND_RESP=$(curl -s -X POST "$API/outboundshipments" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"shipmentNumber\": \"OSH-TEST-001\",
    \"orderId\": \"$ORDER_OUT_ID\",
    \"carrierId\": \"$SUPPLIER_ID\",
    \"trackingNumber\": \"TRK-TEST-001\",
    \"deliveryAddress\": \"Rua Cliente, 100 - RJ\"
  }")

OUTBOUND_ID=$(echo "$OUTBOUND_RESP" | jq -r '.id // empty')

if [ -n "$OUTBOUND_ID" ]; then
  echo "✅ OutboundShipment criado: $OUTBOUND_ID"
else
  echo "✗ Falha ao criar OutboundShipment"
  echo "$OUTBOUND_RESP" | jq
  exit 1
fi
echo ""

# =======================
# CYCLE COUNT
# =======================
echo "=== PASSO 10: Criar CycleCount (TESTE CORRIGIDO) ==="
CYCLE_RESP=$(curl -s -X POST "$API/cyclecounts" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"countNumber\": \"CC-TEST-001\",
    \"warehouseId\": \"$WAREHOUSE_ID\",
    \"zoneId\": \"$ZONE_ID\",
    \"countedBy\": \"$USER_ID\"
  }")

CYCLE_ID=$(echo "$CYCLE_RESP" | jq -r '.id // empty')

if [ -n "$CYCLE_ID" ]; then
  echo "✅ CycleCount criado: $CYCLE_ID"
else
  echo "✗ Falha ao criar CycleCount"
  echo "$CYCLE_RESP" | jq
fi
echo ""

# =======================
# RELATÓRIO FINAL
# =======================
echo "=========================================="
echo "  RELATÓRIO FINAL - WORKFLOWS"
echo "=========================================="
echo ""

mysql -u logistics_user -ppassword -D logistics_db -e "
SELECT 
    'InboundShipments' as Tabela, COUNT(*) as Total FROM InboundShipments
UNION ALL SELECT 'Receipts', COUNT(*) FROM Receipts
UNION ALL SELECT 'PutawayTasks', COUNT(*) FROM PutawayTasks
UNION ALL SELECT 'PickingWaves', COUNT(*) FROM PickingWaves
UNION ALL SELECT 'PackingTasks', COUNT(*) FROM PackingTasks
UNION ALL SELECT 'Packages', COUNT(*) FROM Packages
UNION ALL SELECT 'OutboundShipments', COUNT(*) FROM OutboundShipments
UNION ALL SELECT 'CycleCounts', COUNT(*) FROM CycleCounts
ORDER BY Total DESC;
" 2>/dev/null

echo ""
echo "=========================================="
echo "✅ TESTE DE WORKFLOWS FINALIZADO!"
echo "=========================================="
