# ⚡ INSTRUÇÕES URGENTES - EXECUTAR AGORA

## 🔴 AÇÃO OBRIGATÓRIA 1: EXECUTAR SCRIPT SQL

**Arquivo**: `API/scripts/add-orderstatus-priority.sql`

### Opção 1: MySQL Workbench (RECOMENDADO)
```
1. Abrir MySQL Workbench
2. Conectar no banco "logistics_wms"
3. File → Open SQL Script → Selecionar: API/scripts/add-orderstatus-priority.sql
4. Clicar no raio ⚡ ou Ctrl+Shift+Enter
5. Verificar mensagens: deve aparecer "XX rows affected"
```

### Opção 2: Linha de Comando
```bash
cd /home/wagnerfb/Projetos/ERP/API/scripts
mysql -u root -p logistics_wms < add-orderstatus-priority.sql
```

### Validar Execução
```sql
-- Abrir MySQL e executar:
SELECT COUNT(*) FROM OrderStatuses;  -- Deve retornar: 10
SELECT COUNT(*) FROM OrderPriorities; -- Deve retornar: 4
DESCRIBE Orders; -- Deve mostrar novos campos: VehicleId, DriverId, etc.
```

---

## 🔴 AÇÃO OBRIGATÓRIA 2: REINICIAR APLICAÇÃO

```bash
cd /home/wagnerfb/Projetos/ERP
bash restart-app.sh
```

Aguardar mensagens:
```
✅ API rodando em: http://localhost:5000
✅ Swagger em: http://localhost:5000/swagger
✅ Frontend rodando em: http://localhost:4200
```

---

## 🔴 AÇÃO OBRIGATÓRIA 3: TESTAR NO SWAGGER

### Passo 1: Fazer Login
```
1. Abrir: http://localhost:5000/swagger
2. Endpoint: POST /api/auth/login
3. Body:
   {
     "email": "admin@nexus.com",
     "password": "Admin@123456"
   }
4. Copiar o "token" da resposta
```

### Passo 2: Autorizar no Swagger
```
1. Clicar no botão "Authorize" (cadeado verde) no topo
2. Colar: Bearer SEU_TOKEN_AQUI
3. Clicar "Authorize"
4. Fechar modal
```

### Passo 3: Testar OrderStatus
```
1. Endpoint: GET /api/orderstatus
2. Parameter "language": pt
3. Clicar "Try it out" → "Execute"
4. DEVE RETORNAR: Array com 10 status em português
```

**Resposta Esperada**:
```json
[
  {
    "id": 0,
    "code": "DRAFT",
    "name": "Rascunho",
    "colorHex": "#6B7280",
    "sortOrder": 0
  },
  ...10 status total
]
```

### Passo 4: Testar OrderPriority
```
1. Endpoint: GET /api/orderpriority
2. Parameter "language": pt
3. Clicar "Try it out" → "Execute"
4. DEVE RETORNAR: Array com 4 prioridades em português
```

**Resposta Esperada**:
```json
[
  {
    "id": 0,
    "code": "LOW",
    "name": "Baixa",
    "colorHex": "#6B7280",
    "sortOrder": 0
  },
  ...4 prioridades total
]
```

### Passo 5: Testar Outros Idiomas
```
Testar com language=en (inglês):
- Status devem vir em inglês: "Draft", "Pending", etc.

Testar com language=es (espanhol):
- Status devem vir em espanhol: "Borrador", "Pendiente", etc.
```

---

## ✅ VALIDAÇÃO FINAL

Marcar se funcionou:

- [ ] Script SQL executado sem erros
- [ ] Tabelas OrderStatuses e OrderPriorities criadas
- [ ] 10 status + 4 prioridades inseridos no banco
- [ ] Orders tem novos campos (VehicleId, DriverId, etc.)
- [ ] API reiniciou sem erros
- [ ] Swagger carregou endpoints novos
- [ ] GET /api/orderstatus?language=pt retorna 10 items
- [ ] GET /api/orderpriority?language=pt retorna 4 items
- [ ] Testado com EN e ES funciona
- [ ] Frontend buildo sem erros (já feito ✅)

---

## 🚨 SE DER ERRO

### Erro: "Table 'OrderStatuses' already exists"
```sql
-- Dropar e recriar:
DROP TABLE IF EXISTS OrderStatuses;
DROP TABLE IF EXISTS OrderPriorities;
-- Depois executar script novamente
```

### Erro: "Cannot add foreign key constraint"
```sql
-- Remover foreign keys primeiro:
ALTER TABLE Orders 
  DROP FOREIGN KEY IF EXISTS FK_Orders_Vehicles_VehicleId,
  DROP FOREIGN KEY IF EXISTS FK_Orders_Drivers_DriverId;
-- Depois executar script novamente
```

### Erro: "Column 'VehicleId' already exists"
```sql
-- Verificar se já existe:
DESCRIBE Orders;
-- Se existir, pular essa parte do script
```

### API não inicia
```bash
# Ver logs de erro:
cd /home/wagnerfb/Projetos/ERP/API/src/Logistics.API
dotnet run
# Verificar mensagem de erro e me avisar
```

---

## 📍 LOCALIZAÇÃO DOS ARQUIVOS

```
/home/wagnerfb/Projetos/ERP/
├── API/
│   ├── scripts/
│   │   └── add-orderstatus-priority.sql  ← EXECUTAR ESTE
│   └── src/
│       └── Logistics.API/
│           └── Controllers/
│               ├── OrderStatusController.cs      ← NOVO
│               └── OrderPriorityController.cs    ← NOVO
├── APP/
│   ├── src/
│   │   ├── assets/
│   │   │   └── i18n/
│   │   │       ├── pt.json  ← NOVO
│   │   │       ├── en.json  ← NOVO
│   │   │       └── es.json  ← NOVO
│   │   └── app/
│   │       └── core/
│   │           └── services/
│   │               ├── order-status.service.ts      ← NOVO
│   │               ├── order-priority.service.ts    ← NOVO
│   │               └── geocoding.service.ts         ← NOVO
└── APP-Documentation/
    └── IMPLEMENTACAO-ORDERS-WMS-COMPLETA.md  ← DOCUMENTAÇÃO COMPLETA
```

---

## 🎯 RESULTADO ESPERADO

Após executar tudo:

✅ Banco de dados com OrderStatuses e OrderPriorities  
✅ API com 2 controllers novos funcionando  
✅ Swagger mostrando 6 endpoints novos  
✅ Tradução funcionando (PT/EN/ES)  
✅ Frontend pronto para consumir  
✅ Build sem erros (355.29 kB)

**Tempo estimado**: 10-15 minutos para executar tudo

---

**COMEÇAR AGORA** ⚡
