#!/usr/bin/env python3
"""
Script FINAL para popular TODAS as tabelas do WMS com 30+ registros
Com DEBUG completo dos erros
"""

import requests
import json
import sys

BASE_URL = "http://localhost:5000/api"
admin_token = None
company_admin_token = None
company_id = None
warehouse_id = None

# Arrays de IDs
product_ids = []
customer_ids = []
supplier_ids = []
vehicle_ids = []
driver_ids = []
location_ids = []
zone_ids = []
lot_ids = []
serial_ids = []

def log(msg, color=""):
    colors = {"green": "\033[0;32m", "red": "\033[0;31m", "yellow": "\033[1;33m", "blue": "\033[0;34m"}
    reset = "\033[0m"
    print(f"{colors.get(color, '')}{msg}{reset}")

def api_call(method, endpoint, data=None, token=None):
    headers = {"Content-Type": "application/json"}
    if token:
        headers["Authorization"] = f"Bearer {token}"
    
    url = f"{BASE_URL}{endpoint}"
    
    try:
        if method == "POST":
            resp = requests.post(url, json=data, headers=headers)
        elif method == "GET":
            resp = requests.get(url, headers=headers)
        else:
            resp = requests.put(url, json=data, headers=headers)
        
        result = resp.json()
        
        if not result.get("success"):
            log(f"✗ ERRO em {endpoint}: {result.get('message', result.get('errors', 'Unknown'))}", "red")
            if result.get('errors'):
                for err in result.get('errors', []):
                    log(f"  - {err}", "red")
            return None
        
        return result.get("data")
    except Exception as e:
        log(f"✗ EXCEPTION em {endpoint}: {e}", "red")
        return None

# 1. ADMIN
log("\n→ 1. Criando Admin Master...", "yellow")
data = api_call("POST", "/auth/register-admin", {
    "name": "Super Admin",
    "email": "admin@wms.com",
    "password": "Admin@123",
    "confirmPassword": "Admin@123"
})

if data:
    admin_token = data["token"]
    log("✓ Admin criado", "green")
else:
    login = api_call("POST", "/auth/login", {"email": "admin@wms.com", "password": "Admin@123"})
    if login:
        admin_token = login["token"]
        log("✓ Login Admin OK", "green")

# 2. EMPRESAS
log("\n→ 2. Criando 5 Empresas...", "yellow")
for i in range(1, 6):
    data = api_call("POST", "/companies", {"name": f"Empresa {i} Ltda", "document": f"{i:014d}"}, admin_token)
    if data:
        if i == 1:
            company_id = data["id"]
log(f"✓ Empresa principal: {company_id}", "green")

# 3. USUÁRIOS  
log("\n→ 3. Criando 40 Usuários...", "yellow")
for i in range(1, 41):
    role = 1 if i <= 5 else 2
    data = api_call("POST", "/users", {
        "companyId": company_id,
        "name": f"Usuario {i}",
        "email": f"user{i}@empresa.com",
        "password": "Senha@123",
        "role": role
    }, admin_token)
log("✓ 40 usuários", "green")

# LOGIN COMPANY ADMIN
login = api_call("POST", "/auth/login", {"email": "user1@empresa.com", "password": "Senha@123"})
if login:
    company_admin_token = login["token"]
    log("✓ Login CompanyAdmin OK", "green")

# 4. ARMAZÉNS
log("\n→ 4. Criando 3 Armazéns...", "yellow")
for i in range(1, 4):
    data = api_call("POST", "/warehouses", {
        "companyId": company_id,
        "name": f"CD {i}",
        "code": f"CD-{i:03d}",
        "address": f"Rua {i}, SP"
    }, company_admin_token)
    if data and i == 1:
        warehouse_id = data["id"]
log(f"✓ Armazém principal: {warehouse_id}", "green")

# 5. ZONAS - COM DEBUG
log("\n→ 5. Criando 30 Zonas...", "yellow")
for i in range(1, 31):
    data = api_call("POST", "/warehousezones", {
        "warehouseId": warehouse_id,
        "zoneName": f"Zona {i}",
        "type": (i % 10) + 1,
        "temperature": 20.0,
        "humidity": 60.0,
        "totalCapacity": 10000.0
    }, company_admin_token)
    if data:
        zone_ids.append(data["id"])
log(f"✓ {len(zone_ids)} zonas criadas", "green")

# 6. LOCALIZAÇÕES
log("\n→ 6. Criando 50 Localizações...", "yellow")
for i in range(1, 51):
    data = api_call("POST", "/storagelocations", {
        "warehouseId": warehouse_id,
        "zoneId": zone_ids[0] if zone_ids else None,
        "code": f"LOC-{i:03d}",
        "description": f"Localização {i}",
        "aisle": f"A{i//10+1}",
        "rack": f"{i%10+1:02d}",
        "level": str(i%5+1),
        "position": chr(65+i%4),
        "type": (i%8)+1,
        "maxWeight": 1000.0,
        "maxVolume": 10.0
    }, company_admin_token)
    if data:
        location_ids.append(data["id"])
log(f"✓ {len(location_ids)} localizações", "green")

# 7. PRODUTOS
log("\n→ 7. Criando 50 Produtos...", "yellow")
for i in range(1, 51):
    data = api_call("POST", "/products", {
        "companyId": company_id,
        "name": f"Produto {i}",
        "sku": f"SKU-{i:05d}",
        "barcode": f"789{i:010d}",
        "description": f"Desc {i}",
        "weight": float(100+i*10),
        "weightUnit": "kg",
        "requiresLotTracking": (i%3==0),
        "requiresSerialTracking": (i%5==0),
        "isPerishable": (i%4==0),
        "minimumStock": 10.0,
        "safetyStock": 20.0
    }, company_admin_token)
    if data:
        product_ids.append(data["id"])
log(f"✓ {len(product_ids)} produtos", "green")

# 8. CLIENTES
log("\n→ 8. Criando 40 Clientes...", "yellow")
for i in range(1, 41):
    data = api_call("POST", "/customers", {
        "companyId": company_id,
        "name": f"Cliente {i}",
        "document": f"{i:011d}00",
        "email": f"cliente{i}@mail.com",
        "phone": f"(11) 9{i:04d}-{i:04d}",
        "address": f"Rua {i}, SP"
    }, company_admin_token)
    if data:
        customer_ids.append(data["id"])
log(f"✓ {len(customer_ids)} clientes", "green")

# 9. FORNECEDORES
log("\n→ 9. Criando 40 Fornecedores...", "yellow")
for i in range(1, 41):
    data = api_call("POST", "/suppliers", {
        "companyId": company_id,
        "name": f"Fornecedor {i}",
        "document": f"{(i+50000):014d}",
        "email": f"forn{i}@mail.com",
        "phone": f"(11) 3{i:04d}-{i:04d}",
        "address": f"Av {i}, SP"
    }, company_admin_token)
    if data:
        supplier_ids.append(data["id"])
log(f"✓ {len(supplier_ids)} fornecedores", "green")

# 10. VEÍCULOS - COM DEBUG
log("\n→ 10. Criando 35 Veículos...", "yellow")
for i in range(1, 36):
    data = api_call("POST", "/vehicles", {
        "companyId": company_id,
        "licensePlate": f"ABC{i:04d}",
        "model": f"Modelo {i%5+1}",
        "year": 2020+(i%5)
    }, company_admin_token)
    if data:
        vehicle_ids.append(data["id"])
log(f"✓ {len(vehicle_ids)} veículos", "green")

# 11. MOTORISTAS
log("\n→ 11. Criando 35 Motoristas...", "yellow")
for i in range(1, 36):
    data = api_call("POST", "/drivers", {
        "companyId": company_id,
        "name": f"Motorista {i}",
        "licenseNumber": f"CNH{i:08d}",
        "phone": f"(11) 98{i:03d}-{i:04d}"
    }, company_admin_token)
    if data:
        driver_ids.append(data["id"])
log(f"✓ {len(driver_ids)} motoristas", "green")

# 12. LOTES - COM DEBUG
log("\n→ 12. Criando 50 Lotes...", "yellow")
for i in range(1, 51):
    prod_idx = i % len(product_ids)
    supp_idx = i % len(supplier_ids)
    
    data = api_call("POST", "/lots", {
        "companyId": company_id,
        "lotNumber": f"LOT-{i:06d}",
        "productId": product_ids[prod_idx],
        "manufactureDate": f"2025-{(i%12)+1:02d}-01T00:00:00Z",
        "expiryDate": f"2026-{(i%12)+1:02d}-01T00:00:00Z",
        "quantityReceived": float(100+i*10),
        "supplierId": supplier_ids[supp_idx]
    }, company_admin_token)
    if data:
        lot_ids.append(data["id"])
log(f"✓ {len(lot_ids)} lotes", "green")

# 13. SERIAL NUMBERS
log("\n→ 13. Criando 60 Números de Série...", "yellow")
for i in range(1, 61):
    if not lot_ids:
        break
    data = api_call("POST", "/serialnumbers", {
        "serial": f"SN-{i:010d}",
        "productId": product_ids[i%len(product_ids)],
        "lotId": lot_ids[i%len(lot_ids)]
    }, company_admin_token)
    if data:
        serial_ids.append(data["id"])
log(f"✓ {len(serial_ids)} seriais", "green")

# 14. ESTOQUE
log("\n→ 14. Criando 50 Estoques...", "yellow")
count = 0
for i in range(1, 51):
    data = api_call("POST", "/inventories", {
        "productId": product_ids[i%len(product_ids)],
        "warehouseId": warehouse_id,
        "storageLocationId": location_ids[i%len(location_ids)],
        "quantity": float(100+i*50)
    }, company_admin_token)
    if data:
        count += 1
log(f"✓ {count} estoques", "green")

# 15. MOVIMENTAÇÕES
log("\n→ 15. Criando 60 Movimentações...", "yellow")
count = 0
for i in range(1, 61):
    data = api_call("POST", "/stockmovements", {
        "productId": product_ids[i%len(product_ids)],
        "storageLocationId": location_ids[i%len(location_ids)],
        "type": (i%6)+1,
        "quantity": float(10+i*2),
        "reference": f"REF-{i:06d}",
        "notes": f"Mov {i}"
    }, company_admin_token)
    if data:
        count += 1
log(f"✓ {count} movimentações", "green")

# 16. PEDIDOS - COM DEBUG
log("\n→ 16. Criando 60 Pedidos...", "yellow")
count = 0
# 30 Inbound
for i in range(1, 31):
    items = []
    for j in range(3):
        idx = ((i*3)+j) % len(product_ids)
        items.append({
            "productId": product_ids[idx],
            "sku": f"SKU-{idx+1:05d}",
            "quantityOrdered": float(10+i*2),
            "unitPrice": float(50+i*5)
        })
    
    data = api_call("POST", "/orders", {
        "companyId": company_id,
        "orderNumber": f"PO-{i:04d}",
        "type": 1,
        "source": 1,
        "supplierId": supplier_ids[i%len(supplier_ids)],
        "expectedDate": f"2025-12-{(i%28)+1:02d}T10:00:00Z",
        "priority": i%3,
        "isBOPIS": False,
        "items": items
    }, company_admin_token)
    if data:
        count += 1

# 30 Outbound
for i in range(31, 61):
    items = []
    for j in range(2):
        idx = ((i*2)+j) % len(product_ids)
        items.append({
            "productId": product_ids[idx],
            "sku": f"SKU-{idx+1:05d}",
            "quantityOrdered": float(5+i),
            "unitPrice": float(100+i*10)
        })
    
    data = api_call("POST", "/orders", {
        "companyId": company_id,
        "orderNumber": f"SO-{i-30:04d}",
        "type": 2,
        "source": 3,
        "customerId": customer_ids[i%len(customer_ids)],
        "expectedDate": f"2025-12-{(i%28)+1:02d}T14:00:00Z",
        "priority": i%4,
        "shippingAddress": f"Rua {i}, RJ",
        "isBOPIS": (i%5==0),
        "items": items
    }, company_admin_token)
    if data:
        count += 1

log(f"✓ {count} pedidos", "green")

# RESUMO FINAL
log("\n" + "="*60, "blue")
log("  RESUMO FINAL DA POPULAÇÃO", "blue")
log("="*60, "blue")
log(f"✓ 5 Empresas", "green")
log(f"✓ 40 Usuários", "green")
log(f"✓ 3 Armazéns", "green")
log(f"✓ {len(zone_ids)} Zonas", "green")
log(f"✓ {len(location_ids)} Localizações", "green")
log(f"✓ {len(product_ids)} Produtos", "green")
log(f"✓ {len(customer_ids)} Clientes", "green")
log(f"✓ {len(supplier_ids)} Fornecedores", "green")
log(f"✓ {len(vehicle_ids)} Veículos", "green")
log(f"✓ {len(driver_ids)} Motoristas", "green")
log(f"✓ {len(lot_ids)} Lotes", "green")
log(f"✓ {len(serial_ids)} Seriais", "green")
log("="*60, "blue")
log("✓ POPULAÇÃO COMPLETA!", "green")
