# 📐 DIAGRAMAS VISUAIS - SISTEMA DE LOGÍSTICA

**Data**: 2025-11-21

---

## 1. DIAGRAMA ENTIDADE-RELACIONAMENTO ATUAL

```
                    ┌─────────────────┐
                    │    COMPANY      │
                    │─────────────────│
                    │ PK: Id (GUID)   │
                    │ Name            │
                    │ Document (CNPJ) │
                    │ IsActive        │
                    └────────┬────────┘
                             │ 1
          ┌──────────────────┼──────────────────┐
          │                  │                  │
          │ N                │ N                │ N
  ┌───────▼────────┐  ┌─────▼──────┐  ┌────────▼───────┐
  │     USER       │  │  VEHICLE    │  │    DRIVER      │
  │────────────────│  │─────────────│  │────────────────│
  │ PK: Id         │  │ PK: Id      │  │ PK: Id         │
  │ FK: CompanyId? │  │ FK: CompanyId│ │ FK: CompanyId  │
  │ Name           │  │ LicensePlate│  │ Name           │
  │ Email          │  │ Model       │  │ LicenseNumber  │
  │ PasswordHash   │  │ Year        │  │ Phone          │
  │ Role           │  │ Status      │  │ IsActive       │
  └────────────────┘  └─────────────┘  └────────────────┘

LEGENDA:
✅ Company e User = API completa (Controller + Service)
⚠️ Vehicle e Driver = Sem API (apenas Repository)
```

## 2. ARQUITETURA DDD EM CAMADAS

```
┌─────────────────────────────────────────────────┐
│            API LAYER (Apresentação)             │
│  Controllers, Middlewares, Swagger              │
│  ✅ AuthController, CompaniesController         │
│  ❌ VehiclesController, DriversController       │
└────────────────────┬────────────────────────────┘
                     │ DTOs
┌────────────────────▼────────────────────────────┐
│       APPLICATION LAYER (Casos de Uso)          │
│  Services, DTOs, Validators                     │
│  ✅ AuthService, CompanyService                 │
│  ❌ VehicleService, DriverService               │
└────────────────────┬────────────────────────────┘
                     │ Entities
┌────────────────────▼────────────────────────────┐
│         DOMAIN LAYER (Regras de Negócio)        │
│  Entities, Value Objects, Interfaces            │
│  ✅ Company, User, Vehicle, Driver              │
└────────────────────┬────────────────────────────┘
                     │ Interfaces
┌────────────────────▼────────────────────────────┐
│      INFRASTRUCTURE LAYER (Persistência)        │
│  Repositories, DbContext, MySQL                 │
│  ✅ Todos os Repositories implementados         │
└────────────────────┬────────────────────────────┘
                     │ SQL
┌────────────────────▼────────────────────────────┐
│              DATABASE (MySQL)                   │
│  ✅ Companies, Users, Vehicles, Drivers         │
└─────────────────────────────────────────────────┘
```

## 3. FLUXO DE AUTENTICAÇÃO JWT

```
Cliente                  API                 Database
  │                       │                      │
  │ 1. POST /api/auth/    │                      │
  │    register-admin     │                      │
  ├──────────────────────>│                      │
  │                       │ 2. Hash password     │
  │                       │    (BCrypt)          │
  │                       │                      │
  │                       │ 3. INSERT User       │
  │                       ├─────────────────────>│
  │                       │                      │
  │                       │ 4. Generate JWT      │
  │                       │    (8h expiration)   │
  │                       │                      │
  │ 5. Return token       │                      │
  │<──────────────────────┤                      │
  │                       │                      │
  │ 6. POST /api/companies│                      │
  │    Header: Bearer JWT │                      │
  ├──────────────────────>│                      │
  │                       │ 7. Validate JWT      │
  │                       │    Check signature   │
  │                       │    Check expiration  │
  │                       │                      │
  │                       │ 8. Check Policy      │
  │                       │    (AdminOnly)       │
  │                       │                      │
  │                       │ 9. Execute action    │
  │                       ├─────────────────────>│
  │                       │                      │
  │ 10. Return response   │                      │
  │<──────────────────────┤                      │
```

## 4. FLUXO MULTI-TENANT

```
Cenário: 2 empresas diferentes usando o sistema

┌──────────────────────────────────────────────────┐
│              COMPANY A (ABC Transportes)         │
│  Id: aaa-111                                     │
│  ├─ Users:                                       │
│  │  ├─ admin@abc.com (CompanyAdmin)             │
│  │  └─ operador@abc.com (CompanyUser)           │
│  ├─ Vehicles:                                    │
│  │  ├─ ABC-1234 (Mercedes)                       │
│  │  └─ ABC-5678 (Volvo)                          │
│  └─ Drivers:                                     │
│     ├─ João Silva                                │
│     └─ Maria Santos                              │
└──────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────┐
│              COMPANY B (XYZ Logística)           │
│  Id: bbb-222                                     │
│  ├─ Users:                                       │
│  │  └─ gestor@xyz.com (CompanyAdmin)            │
│  ├─ Vehicles:                                    │
│  │  └─ XYZ-9999 (Scania)                         │
│  └─ Drivers:                                     │
│     └─ Carlos Souza                              │
└──────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────┐
│              ADMIN MASTER (Super Admin)          │
│  ├─ master@system.com                            │
│  └─ CompanyId: null (acesso a todas empresas)   │
└──────────────────────────────────────────────────┘

FILTROS AUTOMÁTICOS:
- User da Company A só vê dados da Company A
- User da Company B só vê dados da Company B
- Admin Master vê tudo

Exemplo Query:
GET /api/vehicles
→ Se user.CompanyId = aaa-111
  WHERE CompanyId = 'aaa-111'
  RETORNA: [ ABC-1234, ABC-5678 ]

→ Se user.Role = Admin (master)
  SEM WHERE (retorna tudo)
  RETORNA: [ ABC-1234, ABC-5678, XYZ-9999 ]
```

## 5. DIAGRAMA: POR QUE VEHICLE/DRIVER NÃO APARECEM NO SWAGGER

```
┌────────────────────────────────────────────────────┐
│              COMPANY (✅ FUNCIONA)                 │
├────────────────────────────────────────────────────┤
│ 1. Tabela MySQL: Companies          ✅            │
│ 2. Entity: Company.cs                ✅            │
│ 3. Repository: CompanyRepository.cs  ✅            │
│ 4. Service: CompanyService.cs        ✅            │
│ 5. Controller: CompaniesController   ✅            │
│ 6. Swagger: /api/companies           ✅ VISÍVEL   │
└────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────┐
│           VEHICLE (❌ NÃO FUNCIONA)                │
├────────────────────────────────────────────────────┤
│ 1. Tabela MySQL: Vehicles           ✅            │
│ 2. Entity: Vehicle.cs                ✅            │
│ 3. Repository: VehicleRepository.cs  ✅            │
│ 4. Service: VehicleService.cs        ❌ FALTA!    │
│ 5. Controller: VehiclesController    ❌ FALTA!    │
│ 6. Swagger: /api/vehicles            ❌ INVISÍVEL │
└────────────────────────────────────────────────────┘

CONCLUSÃO:
Swagger = Controller
Sem Controller = Sem Swagger
Vehicle/Driver param no Repository = Órfãos no sistema
```

## 6. ROADMAP DE EXPANSÃO PARA ERP COMPLETO

```
FASE 1: COMPLETAR BÁSICO (Atual + 1 semana)
├─ ✅ Company (completo)
├─ ✅ User (completo)
├─ ⚠️ Vehicle (50%) → Criar Service + Controller
└─ ⚠️ Driver (50%) → Criar Service + Controller

FASE 2: PRODUTOS E ESTOQUE (2-3 semanas)
├─ Product, ProductCategory
├─ Warehouse, StorageLocation
├─ Inventory, StockMovement
└─ Barcode scanning

FASE 3: PEDIDOS (2 semanas)
├─ Customer, Supplier
├─ SalesOrder, PurchaseOrder
└─ OrderItem

FASE 4: OPERAÇÕES LOGÍSTICAS (3-4 semanas)
├─ Receipt (Recebimento)
├─ Shipment (Expedição)
├─ PickingList, PackingList
└─ LoadingList

FASE 5: ENTREGAS E RASTREAMENTO (3 semanas)
├─ Route, Delivery, DeliveryStop
├─ GPS Tracking
├─ ProofOfDelivery
└─ Notifications

FASE 6: RELATÓRIOS E BI (2 semanas)
├─ Dashboards
├─ KPIs operacionais
├─ Analytics
└─ Exports (PDF, Excel)

TOTAL ESTIMADO: 13-16 semanas para ERP completo
```

## 7. COMPARAÇÃO: ATUAL vs IDEAL

```
SISTEMA ATUAL (~8%)              ERP IDEAL (100%)
==================               ==================
✅ Cadastro Empresas            ✅ Cadastro Empresas
✅ Cadastro Usuários            ✅ Cadastro Usuários
⚠️ Cadastro Veículos (50%)      ✅ Cadastro Veículos
⚠️ Cadastro Motoristas (50%)    ✅ Cadastro Motoristas
❌ Cadastro Produtos            ✅ Cadastro Produtos
❌ Cadastro Clientes            ✅ Cadastro Clientes
❌ Cadastro Fornecedores        ✅ Cadastro Fornecedores
❌ Gestão de Estoque            ✅ WMS Completo
❌ Pedidos                      ✅ OMS Completo
❌ Recebimento                  ✅ Inbound Process
❌ Expedição                    ✅ Outbound Process
❌ Entregas                     ✅ TMS + GPS Tracking
❌ Rastreamento                 ✅ Real-time Tracking
❌ Relatórios                   ✅ BI + Analytics
```

---

**Documento criado em**: 2025-11-21  
**Autor**: Cascade AI  
**Próximo passo**: Implementar correções da Fase 1
