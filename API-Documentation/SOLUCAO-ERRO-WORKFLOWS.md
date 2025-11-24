# SOLUÇÃO DO ERRO: Workflows não funcionam

## 🔴 PROBLEMA IDENTIFICADO

Os endpoints de workflows retornam erro:
```json
{
  "errors": {
    "request": ["The request field is required."],
    "$.companyId": ["The JSON value could not be converted to CreateInboundShipmentRequest"]
  }
}
```

### Endpoints afetados:
- `/api/inboundshipments`
- `/api/receipts`
- `/api/putawaytasks`
- `/api/pickingwaves`
- `/api/packingtasks`
- `/api/packages`
- `/api/outboundshipments`
- `/api/vehicleappointments`
- `/api/cyclecounts`

---

## 🔍 CAUSA RAIZ

**TODOS os DTOs de workflows são `record types`**, mas há um problema:

1. **CreateInboundShipmentRequest** é `record` - ❌ NÃO FUNCIONA
2. **CreateDockDoorRequest** é `record` - ✅ FUNCIONA
3. **CreateSerialNumberRequest** é `record` - ✅ FUNCIONA
4. **InventoryRequest** é `class` - ✅ FUNCIONA

### Por que alguns records funcionam e outros não?

Investigando...

**Hipótese 1**: Problema com `Guid` nullable em records  
**Hipótese 2**: Falta de construtores explícitos  
**Hipótese 3**: Problema no Service (não no Controller)

---

## ✅ SOLUÇÃO IMEDIATA

### Opção 1: Converter records para classes (RÁPIDO)

Transformar todos `CreateXRequest` de `record` para `class`:

```csharp
// ANTES (não funciona):
public record CreateInboundShipmentRequest(
    Guid CompanyId,
    string ShipmentNumber,
    Guid OrderId,
    Guid SupplierId,
    Guid? VehicleId,
    Guid? DriverId,
    DateTime? ExpectedArrivalDate,
    string? DockDoorNumber,
    string? ASNNumber
);

// DEPOIS (vai funcionar):
public class CreateInboundShipmentRequest
{
    public Guid CompanyId { get; set; }
    public string ShipmentNumber { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public Guid SupplierId { get; set; }
    public Guid? VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public DateTime? ExpectedArrivalDate { get; set; }
    public string? DockDoorNumber { get; set; }
    public string? ASNNumber { get; set; }
}
```

### Opção 2: Adicionar atributos aos records (MANTÉM IMMUTABILITY)

```csharp
using System.Text.Json.Serialization;

public record CreateInboundShipmentRequest(
    [property: JsonPropertyName("companyId")] Guid CompanyId,
    [property: JsonPropertyName("shipmentNumber")] string ShipmentNumber,
    [property: JsonPropertyName("orderId")] Guid OrderId,
    [property: JsonPropertyName("supplierId")] Guid SupplierId,
    [property: JsonPropertyName("vehicleId")] Guid? VehicleId,
    [property: JsonPropertyName("driverId")] Guid? DriverId,
    [property: JsonPropertyName("expectedArrivalDate")] DateTime? ExpectedArrivalDate,
    [property: JsonPropertyName("dockDoorNumber")] string? DockDoorNumber,
    [property: JsonPropertyName("asnNumber")] string? ASNNumber
);
```

### Opção 3: Verificar Program.cs (configuração JSON)

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
```

---

## 📋 ARQUIVOS A CORRIGIR

Se escolher **Opção 1 (converter para class)**:

1. `/src/Logistics.Application/DTOs/InboundShipment/CreateInboundShipmentRequest.cs`
2. `/src/Logistics.Application/DTOs/Receipt/CreateReceiptRequest.cs`
3. `/src/Logistics.Application/DTOs/PutawayTask/CreatePutawayTaskRequest.cs`
4. `/src/Logistics.Application/DTOs/PickingWave/CreatePickingWaveRequest.cs`
5. `/src/Logistics.Application/DTOs/PackingTask/CreatePackingTaskRequest.cs`
6. `/src/Logistics.Application/DTOs/Package/CreatePackageRequest.cs`
7. `/src/Logistics.Application/DTOs/OutboundShipment/CreateOutboundShipmentRequest.cs`
8. `/src/Logistics.Application/DTOs/VehicleAppointment/CreateVehicleAppointmentRequest.cs`
9. `/src/Logistics.Application/DTOs/CycleCount/CreateCycleCountRequest.cs`

---

## 🎯 PLANO DE AÇÃO

1. ✅ Documento criado (`GUIA-TESTES-CURL.md`)
2. ⏳ **Converter 9 DTOs de record para class**
3. ⏳ Buildar aplicação (`dotnet build`)
4. ⏳ Reiniciar API
5. ⏳ Testar cada endpoint via CURL
6. ⏳ Popular todas tabelas de workflows
7. ⏳ Validar 29 tabelas populadas

---

## 🔧 COMANDOS

### Build
```bash
cd /home/wagnerfb/Projetos/ERP/API/src/Logistics.API
dotnet build
```

### Restart API
```bash
pkill -f "dotnet run"
dotnet run --urls "http://0.0.0.0:5000"
```

### Teste após correção
```bash
TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@wms.com","password":"Admin@123"}' \
  | jq -r '.data.token')

COMPANY_ID=$(mysql -u logistics_user -ppassword -D logistics_db -N -e "SELECT BIN_TO_UUID(Id) FROM Companies LIMIT 1;")
SUPPLIER_ID=$(mysql -u logistics_user -ppassword -D logistics_db -N -e "SELECT BIN_TO_UUID(Id) FROM Suppliers LIMIT 1;")
ORDER_ID=$(mysql -u logistics_user -ppassword -D logistics_db -N -e "SELECT BIN_TO_UUID(Id) FROM Orders WHERE Type=1 LIMIT 1;")

curl -s -X POST http://localhost:5000/api/inboundshipments \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"companyId\": \"$COMPANY_ID\",
    \"shipmentNumber\": \"ISH-TEST-001\",
    \"orderId\": \"$ORDER_ID\",
    \"supplierId\": \"$SUPPLIER_ID\"
  }" | jq
```

---

## ✅ RESULTADO ESPERADO

Após as correções, TODAS as tabelas devem ser populadas:

| Tabela | Registros Meta | Status Atual |
|--------|----------------|--------------|
| DockDoors | 20 | ✅ 20 |
| Inventories | 40 | ✅ 40 |
| SerialNumbers | 50 | ✅ 50 |
| InboundShipments | 30 | ❌ 0 → ✅ 30 |
| Receipts | 30 | ❌ 0 → ✅ 30 |
| PutawayTasks | 30 | ❌ 0 → ✅ 30 |
| PickingWaves | 20 | ❌ 0 → ✅ 20 |
| PackingTasks | 25 | ❌ 0 → ✅ 25 |
| Packages | 25 | ❌ 0 → ✅ 25 |
| OutboundShipments | 25 | ❌ 0 → ✅ 25 |
| VehicleAppointments | 15 | ❌ 0 → ✅ 15 |
| CycleCounts | 10 | ❌ 0 → ✅ 10 |

**TOTAL: 29 tabelas 100% populadas** ✅

---

**Status**: ⏳ AGUARDANDO CORREÇÃO DOS DTOs
