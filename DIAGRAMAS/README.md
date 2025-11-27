# 📊 DIAGRAMAS WMS - WAREHOUSE MANAGEMENT SYSTEM

**Versão Visual e Detalhada**

---

## 📁 ARQUIVOS DISPONÍVEIS

### 📦 COMPRAS (PURCHASE ORDERS)

1. **DIAGRAMA-PURCHASE-ORDERS.md** 
   - Diagrama técnico completo com Mermaid
   - Entidades e relacionamentos (ER Diagram)
   - Endpoints da API
   - Fluxo detalhado de recebimento
   - Para desenvolvedores e analistas técnicos

2. **VISUAL-PURCHASE-ORDERS.md** ✨
   - Diagrama visual colorido e simplificado
   - Exemplos práticos passo a passo
   - Mockups de telas do sistema
   - Explicações para leigos
   - **Recomendado para apresentações e treinamento**

### 🚚 VENDAS (SALES ORDERS)

3. **VISUAL-SALES-ORDERS.md** ✨
   - Diagrama visual colorido e simplificado
   - Fluxo completo de venda
   - Picking e Packing detalhados
   - Rastreamento e entrega
   - Mockups de telas
   - **Recomendado para apresentações e treinamento**

---

## 🎯 COMO USAR

### Para Desenvolvedores
👉 Use: `DIAGRAMA-PURCHASE-ORDERS.md`
- Código Mermaid para copiar
- Estrutura de banco de dados
- Endpoints da API
- Validações e regras

### Para Gerentes e Usuários Finais
👉 Use: `VISUAL-PURCHASE-ORDERS.md` e `VISUAL-SALES-ORDERS.md`
- Diagramas coloridos e didáticos
- Explicação passo a passo
- Exemplos do dia a dia
- Fácil de entender

### Para Apresentações
👉 Visualize os diagramas Mermaid no:
- GitHub (renderiza automaticamente)
- VS Code (com extensão Mermaid)
- Markdown Preview
- Mermaid Live Editor (https://mermaid.live)

---

## 📖 ÍNDICE RÁPIDO

### Purchase Orders (Compras)
```
1. Selecionar Fornecedor
2. Criar Purchase Order
3. Adicionar Produtos (com estoque verificado)
4. Definir Preços e Margens (cálculo automático)
5. Organizar Hierarquia (pallets → caixas → unidades)
6. [Opcional] Dados Internacionais (se importação)
7. Definir Logística (galpão, caminhão, motorista)
8. Upload Documentos (Invoice, DI, BL)
9. Imprimir PO
10. Recebimento (scan LPN → cartons → produtos)
11. Atualizar Estoque (+)
```

### Sales Orders (Vendas)
```
1. Selecionar Cliente
2. Criar Sales Order
3. Adicionar Produtos (verifica estoque)
4. Reservar Estoque
5. Definir Entrega (BOPIS ou Endereço)
6. Definir Logística (galpão origem, caminhão, motorista)
7. Picking (separar produtos do estoque)
8. Packing (embalar em caixas e pallets)
9. Gerar Nota Fiscal
10. Imprimir Etiquetas
11. Despachar (marcar como enviado)
12. Rastreamento
13. Entrega (cliente recebe)
14. Atualizar Estoque (-)
```

---

## 🔗 ENTIDADES PRINCIPAIS

### Comuns a Ambos
- Company (Empresa)
- ProductCategory (Categoria de Produto) ✅ **NOVO**
- Product (Produto)
- Warehouse (Galpão)
- Vehicle (Veículo)
- Driver (Motorista)
- Inventory (Estoque)

### Específicas de Compras
- Supplier (Fornecedor)
- PurchaseOrder
- PurchaseOrderItem
- PurchaseOrderDocument
- InboundShipment
- InboundParcel (pallet chegando)
- InboundCarton (caixa chegando)

### Específicas de Vendas
- Customer (Cliente)
- SalesOrder
- SalesOrderItem
- PickingWave (onda de separação)
- PickingTask (tarefa de picking)
- PackingTask (tarefa de embalagem)
- OutboundShipment
- OutboundParcel (pallet saindo)
- OutboundCarton (caixa saindo)

---

## 📊 ESTATÍSTICAS DO SISTEMA

**Implementado no banco**:
- ✅ 153 Purchase Orders migrados
- ✅ 81 Sales Orders migrados
- ✅ 200+ items de compra
- ✅ 100+ items de venda
- ✅ 50+ produtos diferentes
- ✅ Todas entidades vinculadas

**Endpoints disponíveis**:
- ✅ 10+ endpoints de Purchase Orders
- ✅ 8+ endpoints de Sales Orders
- ✅ Upload de documentos
- ✅ Soft delete
- ✅ Rastreabilidade completa

---

## 🎨 VISUALIZAÇÃO

Os diagramas Mermaid são renderizados automaticamente em:

- ✅ GitHub
- ✅ GitLab
- ✅ VS Code (com extensão)
- ✅ Notion
- ✅ Confluence
- ✅ Obsidian

Para editar ou visualizar:
👉 https://mermaid.live

---

## 📝 NOTAS IMPORTANTES

1. **Hierarquia de Embalagem**
   - Sempre validada: `parcels × cartons × units = total`
   - Obrigatório definir antes do recebimento/envio

2. **Estoque**
   - Purchase Orders: **ADICIONA** (+) ao estoque
   - Sales Orders: **REMOVE** (-) do estoque
   - Reserva automática em vendas

3. **Documentos**
   - Soft delete (nunca apaga fisicamente)
   - Conversão automática para WebP
   - Limite 10MB por arquivo

4. **Rastreabilidade**
   - LPN (License Plate Number) para pallets
   - Barcode para caixas
   - Serial Number (opcional) para produtos

---

## 🚀 PRÓXIMAS FUNCIONALIDADES

- [ ] Dashboard visual de recebimento
- [ ] Dashboard visual de expedição
- [ ] Impressão de etiquetas
- [ ] Geração automática de Nota Fiscal
- [ ] Integração com transportadoras
- [ ] App mobile para scanning

---

**Documentação criada por**: Cascade AI  
**Data**: 2025-11-27  
**Versão**: 1.0
