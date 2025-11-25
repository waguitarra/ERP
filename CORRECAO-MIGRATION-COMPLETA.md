# ✅ CORREÇÃO COMPLETA - MIGRATION EF CORE

**Data**: 2025-11-25 22:15  
**Status**: ✅ CONFIGURAÇÕES CRIADAS - PRONTO PARA MIGRATION

---

## 🔥 PROBLEMA ANTERIOR

❌ **ERRADO**: Criei script SQL direto no banco  
✅ **CORRETO**: Usar migrations do EF Core (como está na documentação)

---

## ✅ CORREÇÃO IMPLEMENTADA

### 1. Configurações EF Core Criadas

**OrderStatusConfiguration.cs** ✅
- Configuração completa da entidade
- Seed data com 10 status (PT/EN/ES)
- Índice único em Code
- Localização: `Logistics.Infrastructure/Data/Configurations/`

**OrderPriorityConfiguration.cs** ✅
- Configuração completa da entidade
- Seed data com 4 prioridades (PT/EN/ES)
- Índice único em Code
- Localização: `Logistics.Infrastructure/Data/Configurations/`

**OrderConfiguration.cs** ✅ (Atualizado)
- Adicionados 15 campos WMS
- Relacionamentos com Vehicle, Driver, Warehouse
- Índices para performance
- Localização: `Logistics.Infrastructure/Data/Configurations/`

### 2. Campos WMS Adicionados

```csharp
// Logística
VehicleId (Guid?)
DriverId (Guid?)
OriginWarehouseId (Guid?)
DestinationWarehouseId (Guid?)

// Geolocalização
ShippingZipCode (string, max 20)
ShippingLatitude (decimal 10,8)
ShippingLongitude (decimal 11,8)
ShippingCity (string, max 100)
ShippingState (string, max 50)
ShippingCountry (string, max 50)

// Rastreamento
TrackingNumber (string, max 100)
EstimatedDeliveryDate (DateTime?)
ActualDeliveryDate (DateTime?)
ShippedAt (DateTime?)
DeliveredAt (DateTime?)
```

### 3. Relacionamentos Configurados

```csharp
Order → Vehicle (OnDelete: SetNull)
Order → Driver (OnDelete: SetNull)
Order → OriginWarehouse (OnDelete: SetNull)
Order → DestinationWarehouse (OnDelete: SetNull)
```

### 4. Índices Criados

```csharp
VehicleId
DriverId
OriginWarehouseId
DestinationWarehouseId
TrackingNumber
```

---

## 🚀 COMO EXECUTAR (MÉTODO CORRETO)

### Opção 1: Scripts Bash (RECOMENDADO)

**Passo 1: Criar Migration**
```bash
cd /home/wagnerfb/Projetos/ERP/API
bash criar-migration.sh
```

**Passo 2: Aplicar no Banco**
```bash
bash aplicar-migration.sh
```

**Passo 3: Reiniciar App**
```bash
cd /home/wagnerfb/Projetos/ERP
bash restart-app.sh
```

### Opção 2: Comandos Manuais

**Passo 1: Criar Migration**
```bash
cd /home/wagnerfb/Projetos/ERP/API/src/Logistics.API
dotnet ef migrations add AddOrderStatusPriorityAndWMSFields -p ../Logistics.Infrastructure -s .
```

**Passo 2: Revisar Migration**
```bash
# Ver arquivos criados em:
ls ../Logistics.Infrastructure/Migrations/

# Deve criar 2 arquivos:
# - YYYYMMDDHHMMSS_AddOrderStatusPriorityAndWMSFields.cs
# - YYYYMMDDHHMMSS_AddOrderStatusPriorityAndWMSFields.Designer.cs
```

**Passo 3: Aplicar no Banco**
```bash
dotnet ef database update
```

**Passo 4: Verificar**
```sql
-- MySQL Workbench
USE logistics_wms;

-- Verificar tabelas
SHOW TABLES LIKE 'Order%';

-- Verificar dados
SELECT * FROM OrderStatuses;    -- 10 registros
SELECT * FROM OrderPriorities;  -- 4 registros

-- Verificar campos novos
DESCRIBE Orders;
```

---

## 📋 O QUE A MIGRATION VAI FAZER

### 1. Criar Tabela OrderStatuses
```sql
CREATE TABLE OrderStatuses (
    Id int PRIMARY KEY,
    Code varchar(50) UNIQUE,
    NamePT varchar(100),
    NameEN varchar(100),
    NameES varchar(100),
    DescriptionPT varchar(500),
    DescriptionEN varchar(500),
    DescriptionES varchar(500),
    ColorHex varchar(10),
    SortOrder int,
    IsActive bit,
    CreatedAt datetime
);

-- INSERT 10 registros (seed data)
```

### 2. Criar Tabela OrderPriorities
```sql
CREATE TABLE OrderPriorities (
    Id int PRIMARY KEY,
    Code varchar(50) UNIQUE,
    NamePT varchar(100),
    NameEN varchar(100),
    NameES varchar(100),
    DescriptionPT varchar(500),
    DescriptionEN varchar(500),
    DescriptionES varchar(500),
    ColorHex varchar(10),
    SortOrder int,
    IsActive bit,
    CreatedAt datetime
);

-- INSERT 4 registros (seed data)
```

### 3. Atualizar Tabela Orders
```sql
ALTER TABLE Orders ADD COLUMN VehicleId char(36);
ALTER TABLE Orders ADD COLUMN DriverId char(36);
ALTER TABLE Orders ADD COLUMN OriginWarehouseId char(36);
ALTER TABLE Orders ADD COLUMN DestinationWarehouseId char(36);
ALTER TABLE Orders ADD COLUMN ShippingZipCode varchar(20);
ALTER TABLE Orders ADD COLUMN ShippingLatitude decimal(10,8);
ALTER TABLE Orders ADD COLUMN ShippingLongitude decimal(11,8);
ALTER TABLE Orders ADD COLUMN ShippingCity varchar(100);
ALTER TABLE Orders ADD COLUMN ShippingState varchar(50);
ALTER TABLE Orders ADD COLUMN ShippingCountry varchar(50);
ALTER TABLE Orders ADD COLUMN TrackingNumber varchar(100);
ALTER TABLE Orders ADD COLUMN EstimatedDeliveryDate datetime;
ALTER TABLE Orders ADD COLUMN ActualDeliveryDate datetime;
ALTER TABLE Orders ADD COLUMN ShippedAt datetime;
ALTER TABLE Orders ADD COLUMN DeliveredAt datetime;

-- Criar Foreign Keys
ALTER TABLE Orders ADD CONSTRAINT FK_Orders_Vehicles_VehicleId FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id) ON DELETE SET NULL;
ALTER TABLE Orders ADD CONSTRAINT FK_Orders_Drivers_DriverId FOREIGN KEY (DriverId) REFERENCES Drivers(Id) ON DELETE SET NULL;
-- ... etc

-- Criar Índices
CREATE INDEX IX_Orders_VehicleId ON Orders(VehicleId);
CREATE INDEX IX_Orders_DriverId ON Orders(DriverId);
-- ... etc
```

---

## ✅ VALIDAÇÃO

Após aplicar migration, verificar:

- [ ] Tabela `OrderStatuses` criada
- [ ] Tabela `OrderPriorities` criada
- [ ] 10 status inseridos (DRAFT, PENDING, CONFIRMED, etc.)
- [ ] 4 prioridades inseridas (LOW, NORMAL, HIGH, URGENT)
- [ ] Orders tem 15 campos novos
- [ ] Foreign keys criadas
- [ ] Índices criados
- [ ] API reinicia sem erros
- [ ] Swagger mostra endpoints `/api/orderstatus` e `/api/orderpriority`
- [ ] GET `/api/orderstatus?language=pt` retorna 10 items
- [ ] GET `/api/orderpriority?language=pt` retorna 4 items

---

## 🎯 ARQUITETURA CORRETA (SEGUINDO DOCUMENTAÇÃO)

```
1. Criar Entidades (Domain) ✅
   └─ OrderStatus.cs
   └─ OrderPriority.cs
   └─ Order.cs (atualizado)

2. Criar Repositories (Infrastructure) ✅
   └─ OrderStatusRepository.cs
   └─ OrderPriorityRepository.cs

3. Criar Services (Application) ✅
   └─ OrderStatusService.cs
   └─ OrderPriorityService.cs

4. Criar Controllers (API) ✅
   └─ OrderStatusController.cs
   └─ OrderPriorityController.cs

5. Criar Configurations (Infrastructure) ✅
   └─ OrderStatusConfiguration.cs (com seed)
   └─ OrderPriorityConfiguration.cs (com seed)
   └─ OrderConfiguration.cs (atualizado)

6. Registrar no DI (API/Program.cs) ✅

7. CRIAR MIGRATION ⚠️ PRÓXIMO PASSO
   └─ dotnet ef migrations add ...

8. APLICAR MIGRATION ⚠️ PRÓXIMO PASSO
   └─ dotnet ef database update
```

---

## 📝 SCRIPTS CRIADOS

1. **criar-migration.sh** - Cria migration EF Core
2. **aplicar-migration.sh** - Aplica migration no banco

**Localização**: `/home/wagnerfb/Projetos/ERP/API/`

---

## 🚨 IMPORTANTE

✅ **SEMPRE use migrations do EF Core**  
✅ **NUNCA execute SQL direto no banco** (exceto SELECTs)  
✅ **Seed data vai nas Configurations** (não em scripts SQL)  
✅ **Siga a documentação em API-Documentation/**

---

**Status**: ✅ Pronto para criar e aplicar migration  
**Próximo passo**: Execute `bash criar-migration.sh`
