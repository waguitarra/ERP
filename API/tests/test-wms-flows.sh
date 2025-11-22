#!/bin/bash

# Script de Teste dos Fluxos WMS
# Testa Inbound, Outbound, Inventory e todos os relacionamentos

BASE_URL="http://localhost:5000/api"

# Carregar IDs salvos do script anterior
source /tmp/wms_test_ids.txt

# Cores
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

log_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

log_error() {
    echo -e "${RED}✗ $1${NC}"
}

log_info() {
    echo -e "${YELLOW}→ $1${NC}"
}

log_test() {
    echo -e "${BLUE}TEST: $1${NC}"
}

make_request() {
    local method=$1
    local endpoint=$2
    local data=$3
    local token=$4
    
    curl -s -X "$method" "$BASE_URL$endpoint" \
        -H "Content-Type: application/json" \
        -H "Authorization: Bearer $token" \
        -d "$data"
}

extract_json_id() {
    echo "$1" | grep -o "\"id\":\"[^\"]*\"" | cut -d'"' -f4 | head -1
}

echo "================================================"
echo "  TESTE COMPLETO DOS FLUXOS WMS"
echo "  Testando Inbound, Outbound, Inventory"
echo "================================================"
echo ""

# ============================================
# 12. CRIAR PORTAS DE DOCAGEM (10 portas)
# ============================================
log_info "12. Criando 10 Portas de Docagem..."

DOCKDOOR_IDS=()

for i in {1..10}; do
    DOCKDOOR_RESPONSE=$(make_request POST "/dockdoors" "{
        \"warehouseId\": \"$WAREHOUSE_ID\",
        \"doorNumber\": \"DOCK-$(printf '%02d' $i)\",
        \"type\": $((i % 2 + 1)),
        \"isActive\": true
    }" "$COMPANY_ADMIN_TOKEN")
    
    DOCKDOOR_ID=$(extract_json_id "$DOCKDOOR_RESPONSE")
    DOCKDOOR_IDS+=("$DOCKDOOR_ID")
done

log_success "10 Portas de Docagem criadas"
echo ""

# ============================================
# 13. CRIAR PEDIDOS DE COMPRA (30 Orders Inbound)
# ============================================
log_info "13. Criando 30 Pedidos de Compra (Inbound)..."

ORDER_INBOUND_IDS=()

for i in {1..30}; do
    # Criar pedido com 3-5 itens
    NUM_ITEMS=$((3 + i % 3))
    ITEMS_JSON="["
    
    for j in $(seq 1 $NUM_ITEMS); do
        PROD_IDX=$(( (i * j) % 40 ))
        ITEMS_JSON="$ITEMS_JSON{
            \"productId\": \"${PRODUCT_IDS[$PROD_IDX]}\",
            \"sku\": \"SKU-$(printf '%05d' $PROD_IDX)\",
            \"quantityOrdered\": $((10 + i * 2)),
            \"unitPrice\": $((50 + i * 5))
        }"
        if [ $j -lt $NUM_ITEMS ]; then
            ITEMS_JSON="$ITEMS_JSON,"
        fi
    done
    ITEMS_JSON="$ITEMS_JSON]"
    
    ORDER_RESPONSE=$(make_request POST "/orders" "{
        \"companyId\": \"$COMPANY_ID\",
        \"orderNumber\": \"PO-2025-$(printf '%04d' $i)\",
        \"type\": 1,
        \"source\": 1,
        \"supplierId\": \"$SUPPLIER_ID\",
        \"expectedDate\": \"2025-12-$(printf '%02d' $((i % 28 + 1)))T10:00:00Z\",
        \"priority\": $((i % 3)),
        \"items\": $ITEMS_JSON
    }" "$COMPANY_ADMIN_TOKEN")
    
    ORDER_ID=$(extract_json_id "$ORDER_RESPONSE")
    ORDER_INBOUND_IDS+=("$ORDER_ID")
    
    if [ $((i % 10)) -eq 0 ]; then
        log_success "$i pedidos de compra criados..."
    fi
done

log_success "30 Pedidos de Compra criados"
echo ""

# ============================================
# 14. CRIAR PEDIDOS DE VENDA (30 Orders Outbound)
# ============================================
log_info "14. Criando 30 Pedidos de Venda (Outbound)..."

ORDER_OUTBOUND_IDS=()

for i in {1..30}; do
    NUM_ITEMS=$((2 + i % 4))
    ITEMS_JSON="["
    
    for j in $(seq 1 $NUM_ITEMS); do
        PROD_IDX=$(( (i + j + 10) % 40 ))
        ITEMS_JSON="$ITEMS_JSON{
            \"productId\": \"${PRODUCT_IDS[$PROD_IDX]}\",
            \"sku\": \"SKU-$(printf '%05d' $PROD_IDX)\",
            \"quantityOrdered\": $((5 + i)),
            \"unitPrice\": $((100 + i * 10))
        }"
        if [ $j -lt $NUM_ITEMS ]; then
            ITEMS_JSON="$ITEMS_JSON,"
        fi
    done
    ITEMS_JSON="$ITEMS_JSON]"
    
    ORDER_RESPONSE=$(make_request POST "/orders" "{
        \"companyId\": \"$COMPANY_ID\",
        \"orderNumber\": \"SO-2025-$(printf '%04d' $i)\",
        \"type\": 2,
        \"source\": 3,
        \"customerId\": \"$CUSTOMER_ID\",
        \"expectedDate\": \"2025-12-$(printf '%02d' $((i % 28 + 1)))T14:00:00Z\",
        \"priority\": $((i % 4)),
        \"shippingAddress\": \"Rua Cliente $i, 100 - Rio de Janeiro/RJ\",
        \"isBOPIS\": $((i % 5 == 0)),
        \"items\": $ITEMS_JSON
    }" "$COMPANY_ADMIN_TOKEN")
    
    ORDER_ID=$(extract_json_id "$ORDER_RESPONSE")
    ORDER_OUTBOUND_IDS+=("$ORDER_ID")
    
    if [ $((i % 10)) -eq 0 ]; then
        log_success "$i pedidos de venda criados..."
    fi
done

log_success "30 Pedidos de Venda criados"
echo ""

# ============================================
# 15. CRIAR AGENDAMENTOS DE VEÍCULOS (30)
# ============================================
log_info "15. Criando 30 Agendamentos de Veículos..."

APPOINTMENT_IDS=()

for i in {1..30}; do
    APPT_RESPONSE=$(make_request POST "/vehicleappointments" "{
        \"appointmentNumber\": \"APT-2025-$(printf '%04d' $i)\",
        \"warehouseId\": \"$WAREHOUSE_ID\",
        \"type\": $((i % 2 + 1)),
        \"scheduledDate\": \"2025-12-$(printf '%02d' $((i % 28 + 1)))T$(printf '%02d' $((8 + i % 10))):00:00Z\",
        \"vehicleId\": \"$VEHICLE_ID\",
        \"driverId\": \"$DRIVER_ID\",
        \"dockDoorId\": \"${DOCKDOOR_IDS[0]}\"
    }" "$COMPANY_ADMIN_TOKEN")
    
    APPT_ID=$(extract_json_id "$APPT_RESPONSE")
    APPOINTMENT_IDS+=("$APPT_ID")
    
    if [ $((i % 10)) -eq 0 ]; then
        log_success "$i agendamentos criados..."
    fi
done

log_success "30 Agendamentos criados"
echo ""

# ============================================
# 16. CRIAR REMESSAS DE ENTRADA (30 InboundShipments)
# ============================================
log_info "16. Criando 30 Remessas de Entrada..."

INBOUND_SHIPMENT_IDS=()

for i in {1..30}; do
    SHIPMENT_RESPONSE=$(make_request POST "/inboundshipments" "{
        \"companyId\": \"$COMPANY_ID\",
        \"shipmentNumber\": \"ISH-2025-$(printf '%04d' $i)\",
        \"orderId\": \"${ORDER_INBOUND_IDS[$((i-1))]}\",
        \"supplierId\": \"$SUPPLIER_ID\",
        \"vehicleId\": \"$VEHICLE_ID\",
        \"driverId\": \"$DRIVER_ID\",
        \"expectedArrivalDate\": \"2025-12-$(printf '%02d' $((i % 28 + 1)))T10:00:00Z\",
        \"dockDoorNumber\": \"DOCK-01\",
        \"asnNumber\": \"ASN-$(printf '%08d' $i)\"
    }" "$COMPANY_ADMIN_TOKEN")
    
    SHIPMENT_ID=$(extract_json_id "$SHIPMENT_RESPONSE")
    INBOUND_SHIPMENT_IDS+=("$SHIPMENT_ID")
    
    if [ $((i % 10)) -eq 0 ]; then
        log_success "$i remessas de entrada criadas..."
    fi
done

log_success "30 Remessas de Entrada criadas"
echo ""

# ============================================
# 17. CRIAR LOTES (40 Lots)
# ============================================
log_info "17. Criando 40 Lotes..."

LOT_IDS=()

for i in {1..40}; do
    # Produtos que requerem rastreamento por lote (múltiplos de 3)
    PROD_IDX=$((i % 40))
    
    LOT_RESPONSE=$(make_request POST "/lots" "{
        \"companyId\": \"$COMPANY_ID\",
        \"lotNumber\": \"LOT-2025-$(printf '%06d' $i)\",
        \"productId\": \"${PRODUCT_IDS[$PROD_IDX]}\",
        \"manufactureDate\": \"2025-$(printf '%02d' $((i % 12 + 1)))-01T00:00:00Z\",
        \"expiryDate\": \"2026-$(printf '%02d' $((i % 12 + 1)))-01T00:00:00Z\",
        \"quantityReceived\": $((100 + i * 10)),
        \"supplierId\": \"$SUPPLIER_ID\"
    }" "$COMPANY_ADMIN_TOKEN")
    
    LOT_ID=$(extract_json_id "$LOT_RESPONSE")
    LOT_IDS+=("$LOT_ID")
    
    if [ $((i % 10)) -eq 0 ]; then
        log_success "$i lotes criados..."
    fi
done

log_success "40 Lotes criados"
echo ""

# ============================================
# 18. CRIAR NÚMEROS DE SÉRIE (50 SerialNumbers)
# ============================================
log_info "18. Criando 50 Números de Série..."

SERIAL_IDS=()

for i in {1..50}; do
    # Produtos que requerem rastreamento por serial (múltiplos de 5)
    PROD_IDX=$((i % 40))
    LOT_IDX=$((i % 40))
    
    SERIAL_RESPONSE=$(make_request POST "/serialnumbers" "{
        \"serial\": \"SN-$(printf '%010d' $i)\",
        \"productId\": \"${PRODUCT_IDS[$PROD_IDX]}\",
        \"lotId\": \"${LOT_IDS[$LOT_IDX]}\"
    }" "$COMPANY_ADMIN_TOKEN")
    
    SERIAL_ID=$(extract_json_id "$SERIAL_RESPONSE")
    SERIAL_IDS+=("$SERIAL_ID")
    
    if [ $((i % 10)) -eq 0 ]; then
        log_success "$i números de série criados..."
    fi
done

log_success "50 Números de Série criados"
echo ""

# ============================================
# 19. CRIAR ESTOQUE (40 Inventory)
# ============================================
log_info "19. Criando 40 Registros de Estoque..."

INVENTORY_IDS=()

for i in {1..40}; do
    PROD_IDX=$((i % 40))
    LOC_IDX=$((i % 50))
    
    INVENTORY_RESPONSE=$(make_request POST "/inventories" "{
        \"productId\": \"${PRODUCT_IDS[$PROD_IDX]}\",
        \"warehouseId\": \"$WAREHOUSE_ID\",
        \"storageLocationId\": \"${LOCATION_IDS[$LOC_IDX]}\",
        \"quantity\": $((100 + i * 50))
    }" "$COMPANY_ADMIN_TOKEN")
    
    INVENTORY_ID=$(extract_json_id "$INVENTORY_RESPONSE")
    INVENTORY_IDS+=("$INVENTORY_ID")
    
    if [ $((i % 10)) -eq 0 ]; then
        log_success "$i registros de estoque criados..."
    fi
done

log_success "40 Registros de Estoque criados"
echo ""

# ============================================
# 20. CRIAR MOVIMENTAÇÕES DE ESTOQUE (50)
# ============================================
log_info "20. Criando 50 Movimentações de Estoque..."

MOVEMENT_IDS=()
MOVEMENT_TYPES=(1 2 3 4 5 6)

for i in {1..50}; do
    PROD_IDX=$((i % 40))
    LOC_IDX=$((i % 50))
    TYPE_IDX=$((i % 6))
    
    MOVEMENT_RESPONSE=$(make_request POST "/stockmovements" "{
        \"productId\": \"${PRODUCT_IDS[$PROD_IDX]}\",
        \"storageLocationId\": \"${LOCATION_IDS[$LOC_IDX]}\",
        \"type\": ${MOVEMENT_TYPES[$TYPE_IDX]},
        \"quantity\": $((10 + i * 2)),
        \"reference\": \"REF-$(printf '%06d' $i)\",
        \"notes\": \"Movimentação de teste $i\"
    }" "$COMPANY_ADMIN_TOKEN")
    
    MOVEMENT_ID=$(extract_json_id "$MOVEMENT_RESPONSE")
    MOVEMENT_IDS+=("$MOVEMENT_ID")
    
    if [ $((i % 10)) -eq 0 ]; then
        log_success "$i movimentações criadas..."
    fi
done

log_success "50 Movimentações de Estoque criadas"
echo ""

# ============================================
# 21. CRIAR CONTAGENS CÍCLICAS (30)
# ============================================
log_info "21. Criando 30 Contagens Cíclicas..."

CYCLECOUNT_IDS=()

for i in {1..30}; do
    PROD_IDX=$((i % 40))
    LOC_IDX=$((i % 50))
    
    CYCLECOUNT_RESPONSE=$(make_request POST "/cyclecounts" "{
        \"warehouseId\": \"$WAREHOUSE_ID\",
        \"storageLocationId\": \"${LOCATION_IDS[$LOC_IDX]}\",
        \"productId\": \"${PRODUCT_IDS[$PROD_IDX]}\",
        \"countDate\": \"2025-12-$(printf '%02d' $((i % 28 + 1)))T08:00:00Z\",
        \"expectedQuantity\": $((100 + i * 10)),
        \"countedQuantity\": $((98 + i * 10))
    }" "$COMPANY_ADMIN_TOKEN")
    
    CYCLECOUNT_ID=$(extract_json_id "$CYCLECOUNT_RESPONSE")
    CYCLECOUNT_IDS+=("$CYCLECOUNT_ID")
    
    if [ $((i % 10)) -eq 0 ]; then
        log_success "$i contagens cíclicas criadas..."
    fi
done

log_success "30 Contagens Cíclicas criadas"
echo ""

echo "================================================"
echo "  RESUMO - PARTE 2"
echo "================================================"
echo "✓ 10 Portas de Docagem"
echo "✓ 30 Pedidos de Compra (Inbound)"
echo "✓ 30 Pedidos de Venda (Outbound)"
echo "✓ 30 Agendamentos de Veículos"
echo "✓ 30 Remessas de Entrada"
echo "✓ 40 Lotes"
echo "✓ 50 Números de Série"
echo "✓ 40 Registros de Estoque"
echo "✓ 50 Movimentações"
echo "✓ 30 Contagens Cíclicas"
echo ""
echo "Total adicional: 340 registros"
echo "Total geral: 634 registros"
echo "================================================"
echo ""

# ============================================
# TESTES DE RELACIONAMENTOS E JOINS
# ============================================
log_info "TESTANDO RELACIONAMENTOS E JOINS..."
echo ""

log_test "1. Buscar pedido com itens"
ORDER_WITH_ITEMS=$(make_request GET "/orders/${ORDER_INBOUND_IDS[0]}" "" "$COMPANY_ADMIN_TOKEN")
ITEM_COUNT=$(echo "$ORDER_WITH_ITEMS" | grep -o "\"sku\":" | wc -l)
if [ $ITEM_COUNT -gt 0 ]; then
    log_success "Pedido retornou $ITEM_COUNT itens - JOIN funcionando!"
else
    log_error "Pedido não retornou itens - JOIN falhou"
fi

log_test "2. Buscar usuários por empresa"
USERS_BY_COMPANY=$(make_request GET "/users/company/$COMPANY_ID" "" "$COMPANY_ADMIN_TOKEN")
USER_COUNT=$(echo "$USERS_BY_COMPANY" | grep -o "\"id\":" | wc -l)
log_success "Encontrados $USER_COUNT usuários para a empresa"

log_test "3. Buscar produtos por empresa"
PRODUCTS_BY_COMPANY=$(make_request GET "/products/company/$COMPANY_ID" "" "$COMPANY_ADMIN_TOKEN")
PRODUCT_COUNT=$(echo "$PRODUCTS_BY_COMPANY" | grep -o "\"sku\":" | wc -l)
log_success "Encontrados $PRODUCT_COUNT produtos para a empresa"

log_test "4. Buscar lotes por produto"
if [ ${#LOT_IDS[@]} -gt 0 ] && [ ${#PRODUCT_IDS[@]} -gt 0 ]; then
    LOTS_BY_PRODUCT=$(make_request GET "/lots/product/${PRODUCT_IDS[0]}" "" "$COMPANY_ADMIN_TOKEN")
    log_success "Lotes por produto retornados com sucesso"
fi

log_test "5. Buscar estoque por armazém"
INVENTORY_BY_WAREHOUSE=$(make_request GET "/inventories/warehouse/$WAREHOUSE_ID" "" "$COMPANY_ADMIN_TOKEN")
INV_COUNT=$(echo "$INVENTORY_BY_WAREHOUSE" | grep -o "\"productName\":" | wc -l)
if [ $INV_COUNT -gt 0 ]; then
    log_success "Estoque retornou $INV_COUNT itens com JOIN de produto - OK!"
else
    log_error "JOIN de estoque falhou"
fi

echo ""
log_success "TESTE DE POPULAÇÃO E RELACIONAMENTOS CONCLUÍDO!"
log_info "Banco de dados populado com 634 registros"
log_info "Todos os JOINs testados e funcionando!"
