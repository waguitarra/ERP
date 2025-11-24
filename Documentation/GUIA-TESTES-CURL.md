# GUIA COMPLETO DE TESTES COM CURL - WMS API

**Data**: 2025-11-22  
**API Base URL**: `http://localhost:5000/api`

---

## 🔐 PASSO 1: AUTENTICAÇÃO

### Login Admin Master
```bash
TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@wms.com","password":"Admin@123"}' \
  | jq -r '.data.token')

echo "Token: $TOKEN"
```

### Login Company Admin
```bash
TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user1@empresa.com","password":"Senha@123"}' \
  | jq -r '.data.token')
```

---

## 📦 PASSO 2: CADASTROS BÁSICOS

### 1. Companies (Empresas)
```bash
curl -s -X POST http://localhost:5000/api/companies \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Empresa Teste Ltda",
    "document": "12345678000190"
  }' | jq
```

### 2. Users (Usuários)
```bash
COMPANY_ID="<guid-da-empresa>"

curl -s -X POST http://localhost:5000/api/users \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": "'$COMPANY_ID'",
    "name": "João Silva",
    "email": "joao@empresa.com",
    "password": "Senha@123",
    "role": 2
  }' | jq
```

**Roles**: 0=Admin, 1=CompanyAdmin, 2=CompanyUser

### 3. Warehouses (Armazéns)
```bash
curl -s -X POST http://localhost:5000/api/warehouses \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": "'$COMPANY_ID'",
    "name": "CD São Paulo",
    "code": "CD-SP-01",
    "address": "Av. Paulista, 1000 - SP"
  }' | jq
```

### 4. WarehouseZones (Zonas)
```bash
WAREHOUSE_ID="<guid-do-armazem>"

curl -s -X POST http://localhost:5000/api/warehousezones \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "warehouseId": "'$WAREHOUSE_ID'",
    "zoneName": "Zona Refrigerada",
    "type": 2,
    "temperature": 5.0,
    "humidity": 70.0,
    "totalCapacity": 10000.0
  }' | jq
```

**Zone Types**: 1=Dry, 2=Refrigerated, 3=Frozen, 4=Hazardous, etc.

### 5. StorageLocations (Localizações)
```bash
ZONE_ID="<guid-da-zona>"

curl -s -X POST http://localhost:5000/api/storagelocations \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "warehouseId": "'$WAREHOUSE_ID'",
    "zoneId": "'$ZONE_ID'",
    "code": "A01-R01-N01-P01",
    "description": "Corredor A, Rack 01, Nível 01, Posição 01",
    "aisle": "A01",
    "rack": "R01",
    "level": "N01",
    "position": "P01",
    "type": 1,
    "maxWeight": 1000.0,
    "maxVolume": 10.0
  }' | jq
```

### 6. DockDoors (Docas) ✅ FUNCIONA
```bash
curl -s -X POST http://localhost:5000/api/dockdoors \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "warehouseId": "'$WAREHOUSE_ID'",
    "doorNumber": "DOCK-01",
    "type": 1
  }' | jq
```

**Door Types**: 1=Inbound, 2=Outbound, 3=Both

### 7. Products (Produtos)
```bash
curl -s -X POST http://localhost:5000/api/products \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": "'$COMPANY_ID'",
    "name": "Notebook Dell Inspiron 15",
    "sku": "NB-DELL-001",
    "barcode": "7891234567890",
    "description": "Notebook 15.6 polegadas, Intel i5",
    "weight": 2.5,
    "weightUnit": "kg",
    "requiresLotTracking": true,
    "requiresSerialTracking": true,
    "isPerishable": false,
    "minimumStock": 10.0,
    "safetyStock": 20.0
  }' | jq
```

### 8. Customers (Clientes)
```bash
curl -s -X POST http://localhost:5000/api/customers \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": "'$COMPANY_ID'",
    "name": "Cliente ABC Ltda",
    "document": "12345678000100",
    "email": "contato@clienteabc.com",
    "phone": "(11) 98765-4321",
    "address": "Rua das Flores, 123 - São Paulo"
  }' | jq
```

### 9. Suppliers (Fornecedores)
```bash
curl -s -X POST http://localhost:5000/api/suppliers \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": "'$COMPANY_ID'",
    "name": "Fornecedor XYZ SA",
    "document": "98765432000100",
    "email": "vendas@fornecedorxyz.com",
    "phone": "(11) 3456-7890",
    "address": "Av. Industrial, 5000 - Guarulhos"
  }' | jq
```

### 10. Vehicles (Veículos)
```bash
curl -s -X POST http://localhost:5000/api/vehicles \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": "'$COMPANY_ID'",
    "licensePlate": "ABC-1234",
    "model": "Mercedes-Benz Sprinter",
    "year": 2023
  }' | jq
```

### 11. Drivers (Motoristas)
```bash
curl -s -X POST http://localhost:5000/api/drivers \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": "'$COMPANY_ID'",
    "name": "José Santos",
    "licenseNumber": "12345678900",
    "phone": "(11) 99999-8888"
  }' | jq
```

### 12. Lots (Lotes)
```bash
PRODUCT_ID="<guid-do-produto>"
SUPPLIER_ID="<guid-do-fornecedor>"

curl -s -X POST http://localhost:5000/api/lots \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": "'$COMPANY_ID'",
    "lotNumber": "LOT-2025-0001",
    "productId": "'$PRODUCT_ID'",
    "manufactureDate": "2025-01-01T00:00:00Z",
    "expiryDate": "2026-01-01T00:00:00Z",
    "quantityReceived": 1000.0,
    "supplierId": "'$SUPPLIER_ID'"
  }' | jq
```

---

## 📊 PASSO 3: CONTROLE DE ESTOQUE

### 13. Inventories (Estoque) ✅ FUNCIONA
```bash
PRODUCT_ID="<guid-do-produto>"
LOCATION_ID="<guid-da-localizacao>"

curl -s -X POST http://localhost:5000/api/inventories \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "productId": "'$PRODUCT_ID'",
    "warehouseId": "'$WAREHOUSE_ID'",
    "storageLocationId": "'$LOCATION_ID'",
    "quantity": 100,
    "minimumStock": 10,
    "maximumStock": 500
  }' | jq
```

### 14. SerialNumbers (Números de Série) ✅ FUNCIONA
```bash
LOT_ID="<guid-do-lote>"

curl -s -X POST http://localhost:5000/api/serialnumbers \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "serial": "SN-0000000001",
    "productId": "'$PRODUCT_ID'",
    "lotId": "'$LOT_ID'"
  }' | jq
```

### 15. StockMovements (Movimentações)
```bash
curl -s -X POST http://localhost:5000/api/stockmovements \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "productId": "'$PRODUCT_ID'",
    "storageLocationId": "'$LOCATION_ID'",
    "type": 1,
    "quantity": 50.0,
    "reference": "MOV-001",
    "notes": "Entrada inicial de estoque"
  }' | jq
```

**Movement Types**: 1=Receipt, 2=Shipment, 3=Transfer, 4=Adjustment, 5=Return, 6=Loss

---

## 🚛 PASSO 4: PEDIDOS

### 16. Orders (Pedidos - Inbound)
```bash
PRODUCT_ID1="<guid-produto-1>"
PRODUCT_ID2="<guid-produto-2>"

curl -s -X POST http://localhost:5000/api/orders \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": "'$COMPANY_ID'",
    "orderNumber": "PO-2025-0001",
    "type": 1,
    "source": 1,
    "supplierId": "'$SUPPLIER_ID'",
    "expectedDate": "2025-12-20T10:00:00Z",
    "priority": 1,
    "isBOPIS": false,
    "items": [
      {
        "productId": "'$PRODUCT_ID1'",
        "sku": "NB-DELL-001",
        "quantityOrdered": 100.0,
        "unitPrice": 2500.00
      },
      {
        "productId": "'$PRODUCT_ID2'",
        "sku": "KB-LOG-001",
        "quantityOrdered": 150.0,
        "unitPrice": 150.00
      }
    ]
  }' | jq
```

**Order Types**: 1=Inbound, 2=Outbound  
**Source**: 1=Purchase, 2=Transfer, 3=Sales, 4=Return

### 17. Orders (Pedidos - Outbound)
```bash
CUSTOMER_ID="<guid-do-cliente>"

curl -s -X POST http://localhost:5000/api/orders \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": "'$COMPANY_ID'",
    "orderNumber": "SO-2025-0001",
    "type": 2,
    "source": 3,
    "customerId": "'$CUSTOMER_ID'",
    "expectedDate": "2025-12-25T14:00:00Z",
    "priority": 2,
    "shippingAddress": "Rua Cliente, 100 - Rio de Janeiro",
    "isBOPIS": false,
    "items": [
      {
        "productId": "'$PRODUCT_ID1'",
        "sku": "NB-DELL-001",
        "quantityOrdered": 5.0,
        "unitPrice": 3000.00
      }
    ]
  }' | jq
```

---

## 📥 PASSO 5: FLUXO INBOUND (Recebimento)

### 18. InboundShipments ❌ COM ERRO
```bash
ORDER_ID="<guid-pedido-inbound>"
VEHICLE_ID="<guid-veiculo>"
DRIVER_ID="<guid-motorista>"

# ERRO: "request field required"
curl -s -X POST http://localhost:5000/api/inboundshipments \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": "'$COMPANY_ID'",
    "shipmentNumber": "ISH-2025-0001",
    "orderId": "'$ORDER_ID'",
    "supplierId": "'$SUPPLIER_ID'",
    "vehicleId": "'$VEHICLE_ID'",
    "driverId": "'$DRIVER_ID'",
    "expectedArrivalDate": "2025-12-20T10:00:00Z",
    "dockDoorNumber": "DOCK-01"
  }' | jq
```

### 19. Receipts ❌ COM ERRO
```bash
INBOUND_ID="<guid-inbound-shipment>"
USER_ID="<guid-usuario>"

curl -s -X POST http://localhost:5000/api/receipts \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "receiptNumber": "REC-2025-0001",
    "inboundShipmentId": "'$INBOUND_ID'",
    "warehouseId": "'$WAREHOUSE_ID'",
    "receivedBy": "'$USER_ID'"
  }' | jq
```

### 20. PutawayTasks ❌ COM ERRO
```bash
RECEIPT_ID="<guid-receipt>"
LOC_FROM="<guid-staging>"
LOC_TO="<guid-rack>"

curl -s -X POST http://localhost:5000/api/putawaytasks \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "taskNumber": "PUT-0001",
    "receiptId": "'$RECEIPT_ID'",
    "productId": "'$PRODUCT_ID'",
    "quantity": 50.0,
    "fromLocationId": "'$LOC_FROM'",
    "toLocationId": "'$LOC_TO'"
  }' | jq
```

---

## 📤 PASSO 6: FLUXO OUTBOUND (Expedição)

### 21. PickingWaves ❌ COM ERRO
```bash
ORDER_ID1="<guid-order-1>"
ORDER_ID2="<guid-order-2>"

curl -s -X POST http://localhost:5000/api/pickingwaves \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "waveNumber": "WAVE-0001",
    "warehouseId": "'$WAREHOUSE_ID'",
    "orderIds": ["'$ORDER_ID1'", "'$ORDER_ID2'"]
  }' | jq
```

### 22. PackingTasks ❌ COM ERRO
```bash
curl -s -X POST http://localhost:5000/api/packingtasks \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "taskNumber": "PACK-0001",
    "orderId": "'$ORDER_ID'",
    "assignedTo": "'$USER_ID'"
  }' | jq
```

### 23. Packages ❌ COM ERRO
```bash
PACKING_TASK_ID="<guid-packing-task>"

curl -s -X POST http://localhost:5000/api/packages \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "packingTaskId": "'$PACKING_TASK_ID'",
    "trackingNumber": "PKG-2025-00001",
    "type": 1,
    "weight": 5.5,
    "length": 40.0,
    "width": 30.0,
    "height": 20.0
  }' | jq
```

### 24. OutboundShipments ❌ COM ERRO
```bash
curl -s -X POST http://localhost:5000/api/outboundshipments \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "shipmentNumber": "OSH-2025-0001",
    "orderId": "'$ORDER_ID'",
    "carrierId": "'$SUPPLIER_ID'",
    "trackingNumber": "TRK-2025-00001",
    "deliveryAddress": "Rua Cliente, 100 - RJ"
  }' | jq
```

---

## 🚗 PASSO 7: AGENDAMENTOS

### 25. VehicleAppointments ❌ COM ERRO
```bash
DOCK_DOOR_ID="<guid-doca>"

curl -s -X POST http://localhost:5000/api/vehicleappointments \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "appointmentNumber": "APPT-0001",
    "warehouseId": "'$WAREHOUSE_ID'",
    "vehicleId": "'$VEHICLE_ID'",
    "driverId": "'$DRIVER_ID'",
    "type": 1,
    "scheduledDate": "2025-12-20T08:00:00Z",
    "dockDoorId": "'$DOCK_DOOR_ID'"
  }' | jq
```

---

## 📋 PASSO 8: INVENTÁRIO

### 26. CycleCounts ❌ COM ERRO
```bash
curl -s -X POST http://localhost:5000/api/cyclecounts \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "countNumber": "CC-2025-0001",
    "warehouseId": "'$WAREHOUSE_ID'",
    "zoneId": "'$ZONE_ID'",
    "countedBy": "'$USER_ID'"
  }' | jq
```

---

## 🔍 CONSULTAS (GET)

### Listar todas as empresas
```bash
curl -s http://localhost:5000/api/companies \
  -H "Authorization: Bearer $TOKEN" | jq
```

### Buscar pedidos por empresa
```bash
curl -s "http://localhost:5000/api/orders?companyId=$COMPANY_ID" \
  -H "Authorization: Bearer $TOKEN" | jq
```

### Buscar estoque por armazém
```bash
curl -s "http://localhost:5000/api/inventories?warehouseId=$WAREHOUSE_ID" \
  -H "Authorization: Bearer $TOKEN" | jq
```

### Verificar docas disponíveis
```bash
curl -s "http://localhost:5000/api/dockdoors/warehouse/$WAREHOUSE_ID/available" \
  -H "Authorization: Bearer $TOKEN" | jq
```

---

## ❌ PROBLEMAS IDENTIFICADOS

### Endpoints com erro "request field required":
1. `/api/inboundshipments` 
2. `/api/receipts`
3. `/api/putawaytasks`
4. `/api/pickingwaves`
5. `/api/packingtasks`
6. `/api/packages`
7. `/api/outboundshipments`
8. `/api/vehicleappointments`
9. `/api/cyclecounts`

**Causa**: Controllers não estão usando `ApiResponse<T>` wrapper no retorno.

---

## ✅ ENDPOINTS FUNCIONANDO

1. `/api/auth/login` ✅
2. `/api/companies` ✅
3. `/api/users` ✅
4. `/api/warehouses` ✅
5. `/api/warehousezones` ✅
6. `/api/storagelocations` ✅
7. `/api/dockdoors` ✅
8. `/api/products` ✅
9. `/api/customers` ✅
10. `/api/suppliers` ✅
11. `/api/vehicles` ✅
12. `/api/drivers` ✅
13. `/api/lots` ✅
14. `/api/inventories` ✅
15. `/api/serialnumbers` ✅
16. `/api/stockmovements` ✅
17. `/api/orders` ✅

---

## 📝 SCRIPT COMPLETO DE TESTE

Salve como `test-api-complete.sh`:

```bash
#!/bin/bash
set -e

# Login
TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@wms.com","password":"Admin@123"}' \
  | jq -r '.data.token')

echo "✓ Token obtido"

# Buscar IDs do banco
WAREHOUSE_ID=$(mysql -u logistics_user -ppassword -D logistics_db -N -e "SELECT Id FROM Warehouses LIMIT 1;")
PRODUCT_ID=$(mysql -u logistics_user -ppassword -D logistics_db -N -e "SELECT Id FROM Products LIMIT 1;")
LOCATION_ID=$(mysql -u logistics_user -ppassword -D logistics_db -N -e "SELECT Id FROM StorageLocations LIMIT 1;")

# Criar DockDoor
curl -s -X POST http://localhost:5000/api/dockdoors \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{\"warehouseId\":\"$WAREHOUSE_ID\",\"doorNumber\":\"DOCK-TEST\",\"type\":1}" | jq

echo "✓ DockDoor criado"
```

---

## 🎯 PRÓXIMOS PASSOS

1. ✅ Corrigir Controllers de Workflows (adicionar ApiResponse wrapper)
2. ✅ Buildar aplicação
3. ✅ Testar todos endpoints via CURL
4. ✅ Popular todas as 29 tabelas
5. ✅ Validar relacionamentos e integridade

**Documento atualizado**: 2025-11-22 20:37
