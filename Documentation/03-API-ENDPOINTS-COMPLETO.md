# DOCUMENTAÇÃO TÉCNICA COMPLETA - SISTEMA WMS
## Volume 3: API Endpoints e Controllers - REFERÊNCIA COMPLETA

**Versão**: 3.0  
**Data**: 2025-11-22

---

## 📋 ÍNDICE

1. [Visão Geral da API](#1-visão-geral-da-api)
2. [AuthController - Autenticação](#2-authcontroller)
3. [UsersController - Usuários](#3-userscontroller)
4. [CompaniesController - Empresas](#4-companiescontroller)
5. [WarehousesController - Armazéns](#5-warehousescontroller)
6. [ProductsController - Produtos](#6-productscontroller)
7. [CustomersController - Clientes](#7-customerscontroller)
8. [SuppliersController - Fornecedores](#8-supplierscontroller)
9. [VehiclesController - Veículos](#9-vehiclescontroller)
10. [DriversController - Motoristas](#10-driverscontroller)
11. [OrdersController - Pedidos](#11-orderscontroller)
12. [InboundShipmentsController - Remessas Entrada](#12-inboundshipmentscontroller)
13. [ReceiptsController - Recebimentos](#13-receiptscontroller)
14. [PutawayTasksController - Endereçamento](#14-putawaytaskscontroller)
15. [InventoriesController - Estoque](#15-inventoriescontroller)
16. [PickingWavesController - Ondas Separação](#16-pickingwavescontroller)
17. [PackingTasksController - Embalagem](#17-packingtaskscontroller)
18. [PackagesController - Pacotes](#18-packagescontroller)
19. [OutboundShipmentsController - Remessas Saída](#19-outboundshipmentscontroller)
20. [StockMovementsController - Movimentações](#20-stockmovementscontroller)
21. [LotsController - Lotes](#21-lotscontroller)
22. [SerialNumbersController - Números Série](#22-serialnumberscontroller)
23. [Demais Controllers](#23-demais-controllers)

---

## 1. VISÃO GERAL DA API

### 1.1 Informações Gerais

**Base URL**: `http://localhost:5000/api` (desenvolvimento)  
**Documentação Swagger**: `http://localhost:5000` (raiz)  
**Formato**: JSON  
**Autenticação**: JWT Bearer Token  
**Versionamento**: v1

### 1.2 Headers Padrão

```http
Content-Type: application/json
Authorization: Bearer {seu-token-jwt}
```

### 1.3 Estrutura de Response Padrão

**Sucesso**:
```json
{
  "success": true,
  "message": "Operação realizada com sucesso",
  "data": { ... }
}
```

**Erro**:
```json
{
  "success": false,
  "message": "Mensagem de erro",
  "errors": ["Detalhe do erro 1", "Detalhe do erro 2"]
}
```

### 1.4 Códigos de Status HTTP

| Código | Significado | Uso |
|--------|-------------|-----|
| 200 | OK | Operação bem-sucedida |
| 201 | Created | Recurso criado com sucesso |
| 400 | Bad Request | Dados inválidos/validação falhou |
| 401 | Unauthorized | Não autenticado (token inválido/ausente) |
| 403 | Forbidden | Sem permissão para acessar recurso |
| 404 | Not Found | Recurso não encontrado |
| 500 | Internal Server Error | Erro interno do servidor |

---

## 2. AuthController

**Rota Base**: `/api/auth`  
**Arquivo**: `Logistics.API/Controllers/AuthController.cs`

### 2.1 POST /api/auth/login

**Descrição**: Autentica um usuário e retorna token JWT

**Autorização**: Nenhuma (AllowAnonymous)

**Request Body**:
```json
{
  "email": "admin@empresa.com",
  "password": "senha123"
}
```

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Login realizado com sucesso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "admin@empresa.com",
    "name": "Administrador",
    "role": "CompanyAdmin",
    "companyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }
}
```

**Response Error (401)**:
```json
{
  "success": false,
  "message": "Credenciais inválidas"
}
```

**Exemplo cURL**:
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@empresa.com",
    "password": "senha123"
  }'
```

---

### 2.2 POST /api/auth/register-admin

**Descrição**: Registra o primeiro administrador master do sistema (apenas uma vez)

**Autorização**: Nenhuma (AllowAnonymous)

**Request Body**:
```json
{
  "name": "Super Admin",
  "email": "admin@sistema.com",
  "password": "SenhaForte@123",
  "confirmPassword": "SenhaForte@123"
}
```

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Administrador criado com sucesso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "admin@sistema.com",
    "name": "Super Admin",
    "role": "Admin",
    "companyId": null
  }
}
```

**Response Error (400)**:
```json
{
  "success": false,
  "message": "Já existe um administrador master cadastrado"
}
```

**Regras**:
- ✅ Apenas pode ser executado uma vez
- ✅ Se já existe Admin, retorna erro
- ✅ Senha e confirmação devem ser iguais
- ✅ Email não pode estar em uso

---

## 3. UsersController

**Rota Base**: `/api/users`  
**Arquivo**: `Logistics.API/Controllers/UsersController.cs`  
**Autorização**: `[Authorize]` (requer autenticação)

### 3.1 POST /api/users

**Descrição**: Cria novo usuário

**Autorização**: CompanyAdmin ou Admin

**Request Body**:
```json
{
  "companyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "João Silva",
  "email": "joao@empresa.com",
  "password": "senha123",
  "role": 2
}
```

**Roles**:
- `0` = Admin (Master)
- `1` = CompanyAdmin
- `2` = CompanyUser

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Usuário criado com sucesso",
  "data": {
    "id": "9b1deb4d-3b7d-4bad-9bdd-2b0d7b3dcb6d",
    "companyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "companyName": "Empresa XYZ",
    "name": "João Silva",
    "email": "joao@empresa.com",
    "role": "CompanyUser",
    "isActive": true,
    "createdAt": "2025-11-22T14:30:00Z",
    "lastLoginAt": null
  }
}
```

**Response Error (400)**:
```json
{
  "success": false,
  "message": "Email já cadastrado"
}
```

**Exemplo cURL**:
```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {seu-token}" \
  -d '{
    "companyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "João Silva",
    "email": "joao@empresa.com",
    "password": "senha123",
    "role": 2
  }'
```

---

### 3.2 GET /api/users/{id}

**Descrição**: Busca usuário por ID

**Parâmetros**:
- `id` (Guid) - ID do usuário

**Response Success (200)**:
```json
{
  "success": true,
  "data": {
    "id": "9b1deb4d-3b7d-4bad-9bdd-2b0d7b3dcb6d",
    "companyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "companyName": "Empresa XYZ",
    "name": "João Silva",
    "email": "joao@empresa.com",
    "role": "CompanyUser",
    "isActive": true,
    "createdAt": "2025-11-22T14:30:00Z",
    "lastLoginAt": "2025-11-22T15:00:00Z"
  }
}
```

**Response Error (404)**:
```json
{
  "success": false,
  "message": "Usuário não encontrado"
}
```

---

### 3.3 GET /api/users

**Descrição**: Lista todos os usuários

**Response Success (200)**:
```json
{
  "success": true,
  "data": [
    {
      "id": "...",
      "name": "João Silva",
      "email": "joao@empresa.com",
      "role": "CompanyUser",
      ...
    },
    {
      "id": "...",
      "name": "Maria Santos",
      "email": "maria@empresa.com",
      "role": "CompanyAdmin",
      ...
    }
  ]
}
```

---

### 3.4 GET /api/users/company/{companyId}

**Descrição**: Lista usuários de uma empresa específica

**Parâmetros**:
- `companyId` (Guid) - ID da empresa

**Response Success (200)**:
```json
{
  "success": true,
  "data": [
    {
      "id": "...",
      "companyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "João Silva",
      ...
    }
  ]
}
```

---

### 3.5 PUT /api/users/{id}

**Descrição**: Atualiza dados do usuário

**Parâmetros**:
- `id` (Guid) - ID do usuário

**Request Body**:
```json
{
  "name": "João Silva Santos",
  "email": "joao.santos@empresa.com"
}
```

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Usuário atualizado com sucesso",
  "data": {
    "id": "9b1deb4d-3b7d-4bad-9bdd-2b0d7b3dcb6d",
    "name": "João Silva Santos",
    "email": "joao.santos@empresa.com",
    ...
  }
}
```

---

### 3.6 PATCH /api/users/{id}/role

**Descrição**: Atualiza role do usuário

**Parâmetros**:
- `id` (Guid) - ID do usuário

**Request Body**:
```json
1
```
(Número inteiro representando a role: 0=Admin, 1=CompanyAdmin, 2=CompanyUser)

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Role atualizado com sucesso",
  "data": {
    "id": "...",
    "role": "CompanyAdmin",
    ...
  }
}
```

---

### 3.7 DELETE /api/users/{id}

**Descrição**: Deleta usuário

**Parâmetros**:
- `id` (Guid) - ID do usuário

**Response Success (200)**:
```json
{
  "success": true,
  "message": "Usuário deletado com sucesso"
}
```

---

## 4. CompaniesController

**Rota Base**: `/api/companies`  
**Arquivo**: `Logistics.API/Controllers/CompaniesController.cs`

### 4.1 POST /api/companies

**Descrição**: Cria nova empresa

**Request Body**:
```json
{
  "name": "Empresa ABC Ltda",
  "document": "12.345.678/0001-90"
}
```

**Response Success (201)**:
```json
{
  "success": true,
  "message": "Empresa criada com sucesso",
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Empresa ABC Ltda",
    "document": "12.345.678/0001-90",
    "isActive": true,
    "createdAt": "2025-11-22T14:30:00Z"
  }
}
```

---

### 4.2 GET /api/companies/{id}

**Descrição**: Busca empresa por ID

**Response Success (200)**:
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Empresa ABC Ltda",
    "document": "12345678000190",
    "isActive": true,
    "createdAt": "2025-11-22T14:30:00Z"
  }
}
```

---

### 4.3 GET /api/companies

**Descrição**: Lista todas as empresas

**Response Success (200)**:
```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Empresa ABC Ltda",
      "document": "12345678000190",
      "isActive": true
    },
    {
      "id": "...",
      "name": "Empresa XYZ SA",
      "document": "98765432000100",
      "isActive": true
    }
  ]
}
```

---

### 4.4 PUT /api/companies/{id}

**Descrição**: Atualiza empresa

**Request Body**:
```json
{
  "name": "Empresa ABC Ltda - Matriz",
  "document": "12.345.678/0001-90"
}
```

---

### 4.5 DELETE /api/companies/{id}

**Descrição**: Deleta empresa (soft delete)

---

## 5. WarehousesController

**Rota Base**: `/api/warehouses`

### 5.1 POST /api/warehouses

**Request Body**:
```json
{
  "companyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "CD Central",
  "code": "CD-001",
  "address": "Rua A, 123 - São Paulo/SP"
}
```

---

### 5.2 GET /api/warehouses/{id}

**Response Success (200)**:
```json
{
  "success": true,
  "data": {
    "id": "...",
    "companyId": "...",
    "name": "CD Central",
    "code": "CD-001",
    "address": "Rua A, 123 - São Paulo/SP",
    "isActive": true,
    "createdAt": "..."
  }
}
```

---

### 5.3 GET /api/warehouses/company/{companyId}

**Descrição**: Lista armazéns de uma empresa

---

## 6. ProductsController

**Rota Base**: `/api/products`

### 6.1 POST /api/products

**Request Body**:
```json
{
  "companyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Notebook Dell Inspiron",
  "sku": "DELL-INS-15-001",
  "barcode": "7891234567890",
  "description": "Notebook 15 polegadas",
  "weight": 2.5,
  "weightUnit": "kg"
}
```

**Response Success (201)**:
```json
{
  "success": true,
  "message": "Produto criado com sucesso",
  "data": {
    "id": "...",
    "companyId": "...",
    "name": "Notebook Dell Inspiron",
    "sku": "DELL-INS-15-001",
    "barcode": "7891234567890",
    "description": "Notebook 15 polegadas",
    "weight": 2.5,
    "weightUnit": "kg",
    "requiresLotTracking": false,
    "requiresSerialTracking": false,
    "isActive": true,
    "createdAt": "2025-11-22T14:30:00Z"
  }
}
```

---

### 6.2 GET /api/products/{id}

**Descrição**: Busca produto por ID

---

### 6.3 GET /api/products/company/{companyId}

**Descrição**: Lista produtos de uma empresa

---

### 6.4 GET /api/products/sku/{sku}/company/{companyId}

**Descrição**: Busca produto por SKU

---

### 6.5 PUT /api/products/{id}

**Descrição**: Atualiza produto

---

### 6.6 DELETE /api/products/{id}

**Descrição**: Deleta produto

---

## 7. OrdersController

**Rota Base**: `/api/orders`

### 7.1 POST /api/orders

**Request Body**:
```json
{
  "companyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "orderNumber": "ORD-2025-001",
  "type": 2,
  "source": 1,
  "customerId": "...",
  "expectedDate": "2025-11-25T00:00:00Z",
  "priority": 1,
  "shippingAddress": "Rua B, 456 - Rio de Janeiro/RJ",
  "isBOPIS": false,
  "items": [
    {
      "productId": "...",
      "sku": "DELL-INS-15-001",
      "quantityOrdered": 5,
      "unitPrice": 3500.00
    },
    {
      "productId": "...",
      "sku": "MOUSE-LOG-001",
      "quantityOrdered": 10,
      "unitPrice": 150.00
    }
  ]
}
```

**Tipos (type)**:
- `1` = Inbound (Entrada)
- `2` = Outbound (Saída)
- `3` = Transfer (Transferência)
- `4` = Return (Devolução)

**Fontes (source)**:
- `1` = Manual
- `2` = ERP
- `3` = Ecommerce
- `4` = EDI

**Prioridades (priority)**:
- `0` = Low
- `1` = Normal
- `2` = High
- `3` = Urgent

**Response Success (201)**:
```json
{
  "success": true,
  "data": {
    "id": "...",
    "orderNumber": "ORD-2025-001",
    "type": "Outbound",
    "status": "Draft",
    "totalQuantity": 15,
    "totalValue": 19000.00,
    "items": [
      {
        "id": "...",
        "productId": "...",
        "sku": "DELL-INS-15-001",
        "quantityOrdered": 5,
        "unitPrice": 3500.00
      }
    ],
    "createdAt": "..."
  }
}
```

---

### 7.2 GET /api/orders/{id}

**Descrição**: Busca pedido por ID

---

### 7.3 GET /api/orders/company/{companyId}

**Descrição**: Lista pedidos de uma empresa

---

## 8. InboundShipmentsController

**Rota Base**: `/api/inboundshipments`

### 8.1 POST /api/inboundshipments

**Request Body**:
```json
{
  "companyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "shipmentNumber": "ISH-2025-001",
  "orderId": "...",
  "supplierId": "...",
  "vehicleId": "...",
  "driverId": "...",
  "expectedArrivalDate": "2025-11-23T10:00:00Z",
  "dockDoorNumber": "DOCK-01",
  "asnNumber": "ASN-123456"
}
```

**Response Success (201)**:
```json
{
  "success": true,
  "data": {
    "id": "...",
    "shipmentNumber": "ISH-2025-001",
    "orderId": "...",
    "supplierId": "...",
    "supplierName": "Fornecedor ABC",
    "status": "Scheduled",
    "expectedArrivalDate": "2025-11-23T10:00:00Z",
    "dockDoorNumber": "DOCK-01",
    "totalQuantityExpected": 0,
    "totalQuantityReceived": 0,
    "hasQualityIssues": false,
    "createdAt": "..."
  }
}
```

---

### 8.2 POST /api/inboundshipments/{id}/receive

**Descrição**: Marca remessa como recebida

**Request Body**:
```json
"9b1deb4d-3b7d-4bad-9bdd-2b0d7b3dcb6d"
```
(Guid do usuário que está recebendo)

---

### 8.3 POST /api/inboundshipments/{id}/complete

**Descrição**: Completa o recebimento da remessa

---

## 9. InventoriesController

**Rota Base**: `/api/inventories`

### 9.1 GET /api/inventories/warehouse/{warehouseId}

**Descrição**: Lista estoque de um armazém

**Response Success (200)**:
```json
{
  "success": true,
  "data": [
    {
      "id": "...",
      "productId": "...",
      "productName": "Notebook Dell Inspiron",
      "warehouseId": "...",
      "warehouseName": "CD Central",
      "storageLocationId": "...",
      "storageLocationCode": "A-01-2-B",
      "quantity": 50,
      "minimumStock": 10,
      "maximumStock": 100,
      "lastUpdated": "2025-11-22T14:30:00Z"
    }
  ]
}
```

---

### 9.2 GET /api/inventories/product/{productId}

**Descrição**: Lista estoque de um produto (todas as localizações)

---

## 10. RESUMO DOS ENDPOINTS

### Total de Controllers: 26

| Controller | Endpoints | Principais Operações |
|-----------|-----------|---------------------|
| AuthController | 2 | Login, Register Admin |
| UsersController | 7 | CRUD + Role + ListByCompany |
| CompaniesController | 5 | CRUD + List |
| WarehousesController | 5 | CRUD + ListByCompany |
| WarehouseZonesController | 5 | CRUD + ListByWarehouse |
| StorageLocationsController | 6 | CRUD + ListByWarehouse + Block/Unblock |
| ProductsController | 7 | CRUD + GetBySKU + ListByCompany |
| CustomersController | 5 | CRUD + ListByCompany |
| SuppliersController | 5 | CRUD + ListByCompany |
| VehiclesController | 6 | CRUD + ListByCompany + UpdateStatus |
| DriversController | 6 | CRUD + ListByCompany + UpdateLicense |
| OrdersController | 3 | Create, GetById, ListByCompany |
| InboundShipmentsController | 6 | CRUD + Receive + Complete |
| ReceiptsController | 3 | Create, GetById, ListByShipment |
| PutawayTasksController | 5 | CRUD + Assign + Complete |
| InventoriesController | 6 | CRUD + ListByWarehouse + ListByProduct |
| StockMovementsController | 4 | Create, GetById, List, ListByProduct |
| LotsController | 5 | CRUD + ListByProduct |
| SerialNumbersController | 6 | CRUD + GetBySerial + ListByProduct |
| PickingWavesController | 4 | Create, GetById, Release, Complete |
| PackingTasksController | 4 | Create, GetById, Start, Complete |
| PackagesController | 4 | Create, GetById, SetDimensions, UpdateStatus |
| OutboundShipmentsController | 4 | Create, GetById, List, Ship |
| VehicleAppointmentsController | 5 | Create, GetById, CheckIn, CheckOut |
| DockDoorsController | 5 | CRUD + ListByWarehouse |
| CycleCountsController | 4 | Create, GetById, Complete |

### Padrões de Endpoint

**CRUD Completo** (maioria dos controllers):
- `POST /api/{resource}` - Criar
- `GET /api/{resource}/{id}` - Buscar por ID
- `GET /api/{resource}` - Listar todos
- `PUT /api/{resource}/{id}` - Atualizar
- `DELETE /api/{resource}/{id}` - Deletar

**Filtros Comuns**:
- `GET /api/{resource}/company/{companyId}` - Por empresa
- `GET /api/{resource}/warehouse/{warehouseId}` - Por armazém
- `GET /api/{resource}/product/{productId}` - Por produto

**Ações Específicas** (padrão POST):
- `POST /api/inboundshipments/{id}/receive` - Receber
- `POST /api/inboundshipments/{id}/complete` - Completar
- `POST /api/pickingwaves/{id}/release` - Liberar onda
- `POST /api/vehicleappointments/{id}/checkin` - Check-in
- `POST /api/vehicleappointments/{id}/checkout` - Check-out

---

**Próximo**: [Volume 4 - Serviços e Lógica de Negócio](04-SERVICOS-LOGICA-NEGOCIO.md)
