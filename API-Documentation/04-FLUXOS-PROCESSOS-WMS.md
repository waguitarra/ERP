# DOCUMENTAÇÃO TÉCNICA COMPLETA - SISTEMA WMS
## Volume 4: Fluxos de Processos WMS

**Versão**: 3.0  
**Data**: 2025-11-22

---

## 📋 ÍNDICE

1. [Fluxo de Recebimento (Inbound)](#1-fluxo-inbound)
2. [Fluxo de Endereçamento (Putaway)](#2-fluxo-putaway)
3. [Fluxo de Separação (Picking)](#3-fluxo-picking)
4. [Fluxo de Expedição (Outbound)](#4-fluxo-outbound)
5. [Fluxo de Inventário](#5-fluxo-inventario)
6. [Fluxo de Gestão de Lotes](#6-fluxo-lotes)

---

## 1. FLUXO DE RECEBIMENTO (INBOUND)

### 1.1 Diagrama do Processo

```
┌─────────────────┐
│ 1. CRIAR PEDIDO │ (Order type=Inbound)
│   DE COMPRA     │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 2. AGENDAR      │ (VehicleAppointment)
│   CHEGADA       │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 3. CRIAR        │ (InboundShipment)
│   REMESSA       │ Status: Scheduled
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 4. CHEGADA DO   │ VehicleAppointment.CheckIn()
│   CAMINHÃO      │ InboundShipment → InProgress
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 5. CRIAR        │ (Receipt)
│   RECEBIMENTO   │ Status: Draft
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 6. CONFERIR     │ (ReceiptLine por produto)
│   ITENS         │ Qtd esperada vs recebida
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 7. INSPEÇÃO     │ HasQualityIssues?
│   QUALIDADE     │ Se SIM → Quarantine
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 8. GERAR LOTE   │ (Lot) se produto requer
│   (se aplicável)│ RequiresLotTracking
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 9. GERAR TAREFAS│ (PutawayTask)
│   ENDEREÇAMENTO │ Para cada item
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│10. COMPLETAR    │ Receipt → Completed
│   RECEBIMENTO   │ InboundShipment → Completed
└─────────────────┘
```

### 1.2 Passo a Passo Detalhado

#### **PASSO 1: Criar Pedido de Compra**

**Endpoint**: `POST /api/orders`

```json
{
  "companyId": "guid-empresa",
  "orderNumber": "PO-2025-001",
  "type": 1,
  "source": 1,
  "supplierId": "guid-fornecedor",
  "expectedDate": "2025-11-25T00:00:00Z",
  "items": [
    {
      "productId": "guid-produto",
      "sku": "PROD-001",
      "quantityOrdered": 100,
      "unitPrice": 50.00
    }
  ]
}
```

**Entidades Criadas**:
- Order (OrderType.Inbound)
- OrderItems

---

#### **PASSO 2: Agendar Chegada do Veículo**

**Endpoint**: `POST /api/vehicleappointments`

```json
{
  "appointmentNumber": "APT-2025-001",
  "warehouseId": "guid-armazem",
  "type": 1,
  "scheduledDate": "2025-11-23T10:00:00Z",
  "vehicleId": "guid-veiculo",
  "driverId": "guid-motorista",
  "dockDoorId": "guid-porta-docagem"
}
```

**Entidade Criada**:
- VehicleAppointment (Status: Scheduled)

---

#### **PASSO 3: Criar Remessa de Entrada**

**Endpoint**: `POST /api/inboundshipments`

```json
{
  "companyId": "guid-empresa",
  "shipmentNumber": "ISH-2025-001",
  "orderId": "guid-pedido",
  "supplierId": "guid-fornecedor",
  "vehicleId": "guid-veiculo",
  "driverId": "guid-motorista",
  "expectedArrivalDate": "2025-11-23T10:00:00Z",
  "dockDoorNumber": "DOCK-01",
  "asnNumber": "ASN-123456"
}
```

**Entidade Criada**:
- InboundShipment (Status: Scheduled)

---

#### **PASSO 4: Chegada do Caminhão**

**Endpoint**: `POST /api/vehicleappointments/{id}/checkin`

```json
{
  "arrivalDate": "2025-11-23T10:15:00Z"
}
```

**Ações no Sistema**:
- VehicleAppointment.Status → InProgress
- InboundShipment.ActualArrivalDate = now
- InboundShipment.Status → InProgress

---

#### **PASSO 5: Criar Recebimento (GRN)**

**Endpoint**: `POST /api/receipts`

```json
{
  "receiptNumber": "GRN-2025-001",
  "inboundShipmentId": "guid-remessa",
  "warehouseId": "guid-armazem",
  "receivedBy": "guid-usuario"
}
```

**Entidade Criada**:
- Receipt (Status: Draft)

---

#### **PASSO 6: Conferir Itens**

Para cada produto no pedido, criar ReceiptLine:

**Endpoint**: `POST /api/receipts/{id}/lines`

```json
{
  "productId": "guid-produto",
  "quantityExpected": 100,
  "quantityReceived": 98,
  "lotNumber": "LOT-2025-001",
  "notes": "2 itens danificados"
}
```

**Entidade Criada**:
- ReceiptLine

**Regras de Validação**:
- QuantityReceived pode ser diferente de QuantityExpected
- Se diferente, marcar para investigação
- Capturar lote se produto RequiresLotTracking

---

#### **PASSO 7: Inspeção de Qualidade**

Se houver problemas de qualidade:

**Endpoint**: `PUT /api/inboundshipments/{id}/quality`

```json
{
  "hasQualityIssues": true,
  "inspectedBy": "guid-usuario",
  "notes": "Embalagens amassadas"
}
```

**Ações**:
- InboundShipment.HasQualityIssues = true
- Produtos com problemas vão para Zona de Quarentena

---

#### **PASSO 8: Gerar Lote**

Se produto RequiresLotTracking:

**Endpoint**: `POST /api/lots`

```json
{
  "companyId": "guid-empresa",
  "lotNumber": "LOT-2025-001",
  "productId": "guid-produto",
  "manufactureDate": "2025-11-01",
  "expiryDate": "2026-11-01",
  "quantityReceived": 98,
  "supplierId": "guid-fornecedor"
}
```

**Entidade Criada**:
- Lot (Status: Available)

---

#### **PASSO 9: Gerar Tarefas de Endereçamento**

Para cada ReceiptLine, criar PutawayTask:

**Endpoint**: `POST /api/putawaytasks`

```json
{
  "taskNumber": "PUT-2025-001",
  "receiptId": "guid-receipt",
  "productId": "guid-produto",
  "quantity": 98,
  "fromLocationId": "guid-staging-area",
  "toLocationId": "guid-storage-location",
  "lotId": "guid-lote"
}
```

**Entidade Criada**:
- PutawayTask (Status: Pending)

---

#### **PASSO 10: Completar Recebimento**

**Endpoint**: `POST /api/receipts/{id}/complete`

**Ações**:
- Receipt.Status → Completed
- InboundShipment.Status → Completed
- VehicleAppointment.CheckOut()

---

### 1.3 Validações e Regras de Negócio

**Regras Críticas**:
- ✅ InboundShipment DEVE ter Order associado
- ✅ Receipt DEVE ter InboundShipment associado
- ✅ QuantityReceived pode ser < ou > QuantityExpected
- ✅ Se diferença > 10%, alertar supervisor
- ✅ Produtos perecíveis DEVEM ter lote
- ✅ Produtos com serial DEVEM ter números de série registrados
- ✅ Staging area é localização temporária obrigatória
- ✅ Não pode completar receipt se existem PutawayTasks pendentes

---

## 2. FLUXO DE ENDEREÇAMENTO (PUTAWAY)

### 2.1 Diagrama do Processo

```
┌─────────────────┐
│ 1. TAREFA       │ PutawayTask criada
│   CRIADA        │ Status: Pending
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 2. ATRIBUIR     │ AssignTo(userId)
│   OPERADOR      │ Status: Assigned
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 3. OPERADOR     │ Start()
│   INICIA        │ Status: InProgress
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 4. ESCANEAR     │ Validar produto
│   PRODUTO       │ Validar quantidade
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 5. ESCANEAR     │ Validar localização
│   DESTINO       │ Verificar capacidade
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 6. MOVER        │ Física movimentação
│   PRODUTO       │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 7. CONFIRMAR    │ Complete()
│   ENDEREÇAMENTO │ Status: Completed
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 8. ATUALIZAR    │ Inventory.AddStock()
│   ESTOQUE       │ StockMovement (IN)
└─────────────────┘
```

### 2.2 Lógica de Sugestão de Localização

**Algoritmo de Sugestão**:

```python
def SugerirLocalizacao(produto, quantidade):
    # 1. Verificar se já existe estoque do mesmo produto
    localizacoes_existentes = GetLocalizacoesComProduto(produto.Id)
    
    if localizacoes_existentes:
        # 2. Consolidar no mesmo local se tiver espaço
        for loc in localizacoes_existentes:
            if TemCapacidade(loc, produto, quantidade):
                return loc
    
    # 3. Buscar localização vazia na mesma zona
    zona = GetZonaPorTipoProduto(produto)
    localizacoes_vazias = GetLocalizacoesVazias(zona)
    
    for loc in localizacoes_vazias:
        if TemCapacidade(loc, produto, quantidade):
            return loc
    
    # 4. Se não encontrar, alocar em qualquer zona de storage
    return GetPrimeiraLocalizacaoDisponivel()
```

**Critérios de Seleção**:
- ✅ Mesma localização se já tem o produto
- ✅ Zona apropriada (refrigerado, perecível, etc.)
- ✅ Capacidade de peso e volume
- ✅ Proximidade da expedição (produtos A)
- ✅ Não bloqueada

---

## 3. FLUXO DE SEPARAÇÃO (PICKING)

### 3.1 Diagrama do Processo

```
┌─────────────────┐
│ 1. PEDIDOS      │ Orders (type=Outbound)
│   DE VENDA      │ Status: Confirmed
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 2. CRIAR ONDA   │ PickingWave
│   DE SEPARAÇÃO  │ Agrupa vários pedidos
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 3. ALOCAR       │ Reserve inventory
│   ESTOQUE       │ Inventory.Reserve()
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 4. GERAR        │ PickingTask por pedido
│   TAREFAS       │ PickingLine por item
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 5. LIBERAR      │ PickingWave.Release()
│   ONDA          │ Status: Released
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 6. OPERADOR     │ Escaneia itens
│   SEPARA        │ Confirma quantidades
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 7. COMPLETAR    │ PickingTask.Complete()
│   SEPARAÇÃO     │ PickingWave.Complete()
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 8. ATUALIZAR    │ Inventory.RemoveStock()
│   ESTOQUE       │ ReleaseReservation()
└─────────────────┘
```

### 3.2 Estratégias de Picking

#### **A. Discrete Picking (Pedido a Pedido)**
- Cada pedido é separado individualmente
- Ideal para: poucos pedidos, itens grandes
- Vantagem: simples, sem erros
- Desvantagem: lento

#### **B. Batch Picking (Lote)**
- Vários pedidos separados juntos
- Ideal para: muitos pedidos pequenos
- Vantagem: rápido
- Desvantagem: precisa consolidar depois

#### **C. Wave Picking (Onda)**
- Separa por zona ou horário
- Ideal para: alta rotatividade
- Vantagem: otimizado por zona
- Desvantagem: complexo

#### **D. Zone Picking (Zona)**
- Cada operador fica em uma zona
- Ideal para: armazém grande
- Vantagem: especialização
- Desvantagem: precisa consolidar

---

## 4. FLUXO DE EXPEDIÇÃO (OUTBOUND)

### 4.1 Diagrama do Processo

```
┌─────────────────┐
│ 1. SEPARAÇÃO    │ PickingTask completed
│   COMPLETA      │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 2. CRIAR TAREFA │ PackingTask
│   EMBALAGEM     │ Por pedido
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 3. EMBALAR      │ Operador embala
│   ITENS         │ Gera Package
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 4. CAPTURAR     │ Peso, dimensões
│   MEDIDAS       │ Package.SetDimensions()
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 5. GERAR        │ Etiqueta de envio
│   ETIQUETA      │ Tracking number
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 6. CRIAR        │ OutboundShipment
│   REMESSA SAÍDA │ Agrupa pacotes
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 7. CONFERÊNCIA  │ Validar itens
│   FINAL         │ Validar documentos
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 8. CARREGAR     │ Load no veículo
│   VEÍCULO       │ VehicleAppointment
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 9. DESPACHAR    │ OutboundShipment.Ship()
│   CAMINHÃO      │ Order → Shipped
└─────────────────┘
```

---

## 5. FLUXO DE INVENTÁRIO

### 5.1 Contagem Cíclica

```
┌─────────────────┐
│ 1. AGENDAR      │ CycleCount.Create()
│   CONTAGEM      │ Por produto ou zona
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 2. CONGELAR     │ Lock inventory
│   MOVIMENTAÇÃO  │ Durante contagem
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 3. CONTAR       │ Operador conta físico
│   FÍSICO        │ Registra quantidade
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 4. COMPARAR     │ Físico vs Sistema
│   COM SISTEMA   │ Identificar diferenças
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 5. INVESTIGAR   │ Se diferença > tolerância
│   DIFERENÇAS    │ Recontagem
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 6. AJUSTAR      │ StockMovement (Adjustment)
│   ESTOQUE       │ Inventory.Update()
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 7. COMPLETAR    │ CycleCount.Complete()
│   CONTAGEM      │ Unlock inventory
└─────────────────┘
```

---

## 6. FLUXO DE GESTÃO DE LOTES

### 6.1 Criação e Rastreamento

```
┌─────────────────┐
│ 1. RECEBIMENTO  │ Produto RequiresLotTracking
│   COM LOTE      │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 2. CRIAR LOTE   │ Lot.Create()
│                 │ ManufactureDate, ExpiryDate
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 3. VINCULAR     │ ReceiptLine → LotId
│   RECEBIMENTO   │ Inventory → LotId
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 4. REGRA FEFO   │ First Expired, First Out
│   NA SEPARAÇÃO  │ Pega lote mais próximo vencimento
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ 5. RASTREAMENTO │ Todo movimento tem LotId
│   COMPLETO      │ Recall capability
└─────────────────┘
```

### 6.2 Controle de Validade

```python
def VerificarLotesVencidos():
    lotes = GetLotesAtivos()
    
    for lote in lotes:
        dias_vencimento = (lote.ExpiryDate - DateTime.Now).Days
        
        if dias_vencimento < 0:
            # Vencido
            lote.MarkAsExpired()
            BloquearEstoque(lote)
            AlertarGestor(lote, "VENCIDO")
            
        elif dias_vencimento <= 7:
            # Vencendo em 7 dias
            AlertarGestor(lote, "VENCENDO")
            PriorizarSeparacao(lote)
```

---

**Próximo**: [Volume 5 - Guia de Implementação](05-GUIA-IMPLEMENTACAO.md)
