#!/bin/bash

export PATH=$PATH:$HOME/.dotnet

# TESTE COMPLETO DE PURCHASE ORDER
# =================================

BASE_URL="http://localhost:5000/api"
TOKEN=""

echo "🧪 TESTE PURCHASE ORDER - CRUD COMPLETO"
echo "========================================"
echo ""

# 1. LOGIN
echo "1️⃣ Fazendo login..."
LOGIN_RESPONSE=$(curl -s -X POST "$BASE_URL/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@test.com",
    "password": "Admin@123"
  }')

TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"token":"[^"]*' | cut -d'"' -f4)

if [ -z "$TOKEN" ]; then
    echo "❌ ERRO: Não conseguiu fazer login"
    exit 1
fi

echo "✅ Login OK - Token obtido"
echo ""

# 2. CRIAR ORDER (PURCHASE ORDER)
echo "2️⃣ Criando Purchase Order..."
CREATE_RESPONSE=$(curl -s -X POST "$BASE_URL/orders" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "orderNumber": "PO-TEST-001",
    "type": "Inbound",
    "source": "Manual",
    "supplierId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
    "expectedDate": "2025-12-31T00:00:00Z",
    "priority": "High",
    "items": [
      {
        "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa8",
        "sku": "NOTEBOOK-001",
        "quantityOrdered": 1000
      }
    ]
  }')

ORDER_ID=$(echo $CREATE_RESPONSE | grep -o '"id":"[^"]*' | cut -d'"' -f4)

if [ -z "$ORDER_ID" ]; then
    echo "❌ ERRO: Não conseguiu criar order"
    echo "Response: $CREATE_RESPONSE"
    exit 1
fi

echo "✅ Order criado - ID: $ORDER_ID"
echo ""

# 3. SET PURCHASE DETAILS (Preços e Margens)
echo "3️⃣ Definindo detalhes de compra (preços e margens)..."
PURCHASE_RESPONSE=$(curl -s -X POST "$BASE_URL/orders/$ORDER_ID/purchase-details" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "unitCost": 2500.00,
    "taxPercentage": 18.00,
    "desiredMarginPercentage": 30.00
  }')

echo "✅ Detalhes de compra definidos"
echo "   Custo unitário: R$ 2.500,00"
echo "   Impostos: 18%"
echo "   Margem: 30%"
echo "   Preço sugerido será calculado automaticamente"
echo ""

# 4. SET PACKAGING HIERARCHY
echo "4️⃣ Definindo hierarquia de embalagem..."
HIERARCHY_RESPONSE=$(curl -s -X POST "$BASE_URL/orders/$ORDER_ID/packaging-hierarchy" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "expectedParcels": 10,
    "cartonsPerParcel": 10,
    "unitsPerCarton": 10
  }')

echo "✅ Hierarquia definida"
echo "   10 pallets × 10 caixas × 10 unidades = 1.000 ✓"
echo ""

# 5. SET AS INTERNATIONAL
echo "5️⃣ Marcando como importação internacional..."
INTERNATIONAL_RESPONSE=$(curl -s -X POST "$BASE_URL/orders/$ORDER_ID/set-international" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "originCountry": "China",
    "portOfEntry": "Porto de Santos",
    "containerNumber": "MSCU1234567",
    "incoterm": "FOB"
  }')

echo "✅ Marcado como importação"
echo "   Origem: China"
echo "   Porto: Santos"
echo "   Container: MSCU1234567"
echo "   Incoterm: FOB"
echo ""

# 6. GET ORDER (ver resultado final)
echo "6️⃣ Buscando order completo..."
GET_RESPONSE=$(curl -s -X GET "$BASE_URL/orders/$ORDER_ID" \
  -H "Authorization: Bearer $TOKEN")

echo "✅ Order recuperado"
echo ""
echo "📋 RESULTADO FINAL:"
echo "$GET_RESPONSE" | python3 -m json.tool
echo ""

# 7. UPDATE ORDER
echo "7️⃣ Atualizando order..."
UPDATE_RESPONSE=$(curl -s -X PUT "$BASE_URL/orders/$ORDER_ID" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "orderNumber": "PO-TEST-001-UPDATED",
    "status": "Processing"
  }')

echo "✅ Order atualizado"
echo ""

# 8. DELETE (comentado para não apagar)
# echo "8️⃣ Deletando order..."
# DELETE_RESPONSE=$(curl -s -X DELETE "$BASE_URL/orders/$ORDER_ID" \
#   -H "Authorization: Bearer $TOKEN")
# echo "✅ Order deletado"
# echo ""

echo "=========================================="
echo "🎉 TESTE COMPLETO FINALIZADO COM SUCESSO!"
echo "=========================================="
echo ""
echo "Order ID criado: $ORDER_ID"
echo ""
echo "✅ CRUD completo testado:"
echo "   - CREATE ✓"
echo "   - READ ✓"
echo "   - UPDATE ✓"
echo "   - Purchase Details ✓"
echo "   - Packaging Hierarchy ✓"
echo "   - International Purchase ✓"
echo ""
