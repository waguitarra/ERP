# 🚀 Testes de Carga Via CURL

Scripts para testar API simulando front-end real.

## 📋 Pré-requisitos

1. **API rodando**:
```bash
cd /home/wagnerfb/Projetos/ERP/API/src/Logistics.API
dotnet run
```

2. **MySQL rodando** com banco `logistics_db`

## 🎯 Execução

### Teste Rápido (Recomendado)
```bash
cd /home/wagnerfb/Projetos/ERP/API/tests/curl-tests
./massive-load-test.sh
```

**Cria ~220 registros**:
- 10 Empresas
- 50 Produtos
- 50 Clientes
- 40 Veículos
- 30 Motoristas
- 30 Fornecedores
- 10 Armazéns

### Teste Completo (Separado por Fases)
```bash
./99-run-all.sh
```

## ✅ Validação

Após executar os testes:

```bash
./validate-database.sh
```

Mostra:
- Total de registros por tabela
- Validações de integridade (órfãos, duplicatas)
- Distribuição por empresa
- Últimos registros criados

## 📊 Validação Manual no MySQL

```sql
-- Ver totais
SELECT 
    (SELECT COUNT(*) FROM Companies) as Companies,
    (SELECT COUNT(*) FROM Users) as Users,
    (SELECT COUNT(*) FROM Products) as Products,
    (SELECT COUNT(*) FROM Customers) as Customers,
    (SELECT COUNT(*) FROM Suppliers) as Suppliers,
    (SELECT COUNT(*) FROM Vehicles) as Vehicles,
    (SELECT COUNT(*) FROM Drivers) as Drivers,
    (SELECT COUNT(*) FROM Warehouses) as Warehouses;

-- Ver produtos criados
SELECT p.Name, p.SKU, c.Name as Company, p.CreatedAt
FROM Products p
INNER JOIN Companies c ON p.CompanyId = c.Id
ORDER BY p.CreatedAt DESC
LIMIT 20;
```

## 🔧 Troubleshooting

**Problema**: `Falha ao obter token`
- Verifique se API está rodando em http://localhost:5000
- Teste: `curl http://localhost:5000/health`

**Problema**: `Credenciais inválidas`
- Admin já existe mas senha diferente
- Deletar user: `DELETE FROM Users WHERE Email = 'admin@logistics.com'`
- Rodar script novamente

**Problema**: Constraint unique violation
- Banco já tem dados
- Para limpar: `DELETE FROM [tabela]` (ordem: StockMovements, Inventories, etc)
