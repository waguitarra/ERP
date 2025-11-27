# ✅ CHECKLIST OBRIGATÓRIO DE DESENVOLVIMENTO

## ⚠️ REGRAS FUNDAMENTAIS - NUNCA VIOLAR

### 🚫 SE NÃO TESTOU, NÃO ESTÁ FEITO
- Endpoint criado mas não testado = TRABALHO INCOMPLETO
- Componente criado mas não renderizou no navegador = TRABALHO INCOMPLETO
- Migration criada mas não aplicada = TRABALHO INCOMPLETO
- Build passou mas não verificou logs = TRABALHO INCOMPLETO

### ✅ DEFINIÇÃO DE "FEITO":
1. Backend: Código escrito + Build OK + Swagger OK + Testado com curl autenticado + Dados no MySQL
2. Frontend: Código escrito + Build OK + Renderiza no navegador + Sem erros no console + Traduções funcionam
3. Integração: Frontend chama backend + Response correto + Dados persistem + Logs sem erro

**SE FALTAR QUALQUER ITEM ACIMA, NÃO DIGA QUE ESTÁ PRONTO**

---

## 📋 ANTES DE DIZER QUE ALGO ESTÁ FUNCIONANDO

### 1. ✅ BACKEND - VERIFICAÇÕES OBRIGATÓRIAS

#### 1.1 Endpoints da API
- [ ] Verificar se TODOS os endpoints necessários existem no Controller
- [ ] Verificar métodos: GET, POST, PUT, DELETE conforme necessário
- [ ] Confirmar assinatura dos endpoints (parâmetros, tipos de retorno)
- [ ] Verificar se os DTOs/Requests estão corretos

#### 1.2 Entity Framework Core
- [ ] Se alterou entidades do Domain, criar migration: `dotnet ef migrations add NomeDaMigration`
- [ ] Revisar migration gerada antes de aplicar
- [ ] Aplicar migration: `dotnet ef database update` ou usar script `aplicar-migration.sh`
- [ ] NUNCA alterar banco direto via SQL - SEMPRE usar migrations

#### 1.3 Build e Compilação
- [ ] `cd API && dotnet build` - DEVE compilar sem erros
- [ ] Verificar warnings importantes
- [ ] Verificar se não quebrou outros endpoints

#### 1.4 Swagger
- [ ] Backend DEVE estar rodando
- [ ] Acessar `http://localhost:5295/swagger`
- [ ] Confirmar que TODOS os endpoints aparecem
- [ ] Verificar schemas dos requests/responses
- [ ] Testar endpoints no Swagger UI
- [ ] Se precisar autenticação, FAZER LOGIN ANTES

#### 1.5 Autenticação e Login
- [ ] Buscar documentação de login (onde está o endpoint, como funciona)
- [ ] Fazer login: `curl -X POST http://localhost:5295/api/auth/login -H "Content-Type: application/json" -d '{"email":"...","password":"..."}' `
- [ ] Salvar token JWT retornado
- [ ] Usar token em TODOS os requests: `-H "Authorization: Bearer SEU_TOKEN"`

#### 1.6 Testes com curl AUTENTICADO
- [ ] Testar CREATE (POST) com Authorization header
- [ ] Testar READ (GET por ID, GET lista) com Authorization header
- [ ] Testar UPDATE (PUT) com Authorization header
- [ ] Testar DELETE com Authorization header
- [ ] Verificar se response tem estrutura esperada `{ success, data, message }`
- [ ] Status code correto (200, 201, 204, 400, 401, 404, etc)
- [ ] Se der 401 Unauthorized, refazer login

#### 1.7 LOGS DA API - SEMPRE VERIFICAR
- [ ] Ver logs completos: `docker logs -f logistics-api` ou `tail -f logs/api.log`
- [ ] Procurar por `[ERR]`, `[ERROR]`, `Exception`, `failed`
- [ ] Se tiver erro, CORRIGIR ANTES de continuar
- [ ] Logs devem mostrar requests sendo processados
- [ ] Logs devem mostrar conexão com banco OK

---

### 2. ✅ FRONTEND - VERIFICAÇÕES OBRIGATÓRIAS

#### 2.1 Build Angular
- [ ] `cd APP && npm run build` ou `ng build` - DEVE compilar sem erros
- [ ] Verificar se não tem erros de tipo TypeScript
- [ ] Verificar imports faltando
- [ ] Verificar componentes não declarados

#### 2.2 Traduções i18n
- [ ] Verificar se TODAS as chaves usadas existem em pt-BR.json
- [ ] Verificar se TODAS as chaves existem em en-US.json
- [ ] Verificar se TODAS as chaves existem em es-ES.json
- [ ] Rodar script de validação: `python3 validate-i18n-keys.py` (se existir)
- [ ] Sem chaves duplicadas nos JSONs

#### 2.3 Componentes e Serviços
- [ ] Componente importa TODOS os módulos necessários
- [ ] Service tem TODOS os métodos (getAll, getById, create, update, delete)
- [ ] Interfaces/Models refletem exatamente a estrutura da API
- [ ] ViewChild/signals estão declarados corretamente

#### 2.4 Layout e Responsividade
- [ ] Testar em desktop (1920x1080)
- [ ] Testar em tablet (768px)
- [ ] Testar em mobile (375px)
- [ ] Classes Tailwind corretas (md:, lg:, etc)
- [ ] Dark mode funciona

---

### 3. ✅ INTEGRAÇÃO COMPLETA

#### 3.1 Teste End-to-End Manual
- [ ] Abrir aplicação no navegador
- [ ] Criar novo registro - verificar no banco
- [ ] Editar registro - verificar alteração no banco
- [ ] Deletar registro - verificar remoção no banco
- [ ] Verificar console do navegador (sem erros)
- [ ] Verificar Network tab (requests corretos, status 200/201)

#### 3.2 Banco de Dados
- [ ] Conectar no MySQL e verificar tabelas
- [ ] Confirmar que registros foram criados/editados/deletados
- [ ] Verificar relacionamentos (FKs)
- [ ] Verificar campos nullable/required

---

## 🚫 O QUE NUNCA FAZER - PECADOS MORTAIS

### Backend
- ❌ **PECADO MORTAL**: Criar endpoint e não testar com curl
- ❌ **PECADO MORTAL**: Dizer que endpoint funciona sem fazer login e usar token
- ❌ **PECADO MORTAL**: Não ver os logs da API
- ❌ **PECADO MORTAL**: Não verificar dados no MySQL
- ❌ Alterar schema do banco com SQL direto (ALTER TABLE, etc)
- ❌ Pular criação de migrations
- ❌ Dizer que funciona sem testar no Swagger
- ❌ Dizer que funciona sem fazer dotnet build
- ❌ Ignorar warnings do compilador

### Frontend
- ❌ **PECADO MORTAL**: Dizer que componente funciona sem abrir no navegador
- ❌ **PECADO MORTAL**: Não verificar console do navegador (F12)
- ❌ **PECADO MORTAL**: Não verificar Network tab (requests/responses)
- ❌ **PECADO MORTAL**: Traduções não aparecem e ignorar
- ❌ Usar chaves i18n que não existem nos JSONs
- ❌ Dizer que funciona sem fazer ng build
- ❌ Criar componentes sem imports necessários
- ❌ Esquecer de adicionar componentes nos imports do módulo pai

### Geral - OS PIORES
- ❌ **PECADO MORTAL**: Assumir que funciona sem testar
- ❌ **PECADO MORTAL**: Dizer "está pronto" sem validar no banco de dados
- ❌ **PECADO MORTAL**: Criar código e parar aí, sem testar
- ❌ **PECADO MORTAL**: Não investigar até o fim (serviços, repositories, banco, migrations)
- ❌ Ignorar erros de compilação
- ❌ Não verificar console do navegador
- ❌ Não buscar documentação existente (login, autenticação, etc)

---

## 📝 TEMPLATE DE RESPOSTA QUANDO COMPLETAR TAREFA

Ao finalizar uma implementação, SEMPRE incluir:

```
✅ VALIDAÇÕES REALIZADAS:

Backend:
- [ ] dotnet build - compilou sem erros
- [ ] Endpoints verificados no Swagger
- [ ] Testado com curl: [comandos usados]
- [ ] Migration criada/aplicada (se necessário)

Frontend:
- [ ] ng build - compilou sem erros
- [ ] Chaves i18n validadas (pt-BR, en-US, es-ES)
- [ ] Componente renderiza sem erros
- [ ] Console do navegador sem erros

Banco de Dados:
- [ ] Testado CREATE - registro inserido
- [ ] Testado UPDATE - registro alterado
- [ ] Testado DELETE - registro removido
- [ ] Tabelas/campos verificados no MySQL

EVIDÊNCIAS:
[Logs de build, screenshots, outputs de curl, etc]
```

---

## 🎯 FLUXO CORRETO DE DESENVOLVIMENTO

1. **Planejar** - Entender requisito completamente
2. **Backend First**:
   - Criar/modificar entidade
   - Criar migration se necessário
   - Criar/modificar controller
   - Build backend
   - Testar no Swagger
   - Testar com curl
3. **Frontend**:
   - Criar/modificar service
   - Criar/modificar componente
   - Adicionar traduções i18n
   - Build frontend
   - Testar no navegador
4. **Validar no Banco** - Confirmar dados persistidos
5. **Só então** dizer que está funcionando

---

---

## 🔥 REGRA DE OURO - GRAVAR NA MEMÓRIA

### ENDPOINT NÃO ESTÁ FEITO SE:
1. Não fez login na API
2. Não testou com curl usando token de autenticação
3. Não verificou response do endpoint
4. Não verificou dados no MySQL
5. Não viu logs da API
6. Não verificou no Swagger

### COMPONENTE NÃO ESTÁ FEITO SE:
1. Não abriu no navegador
2. Não verificou console (F12)
3. Não verificou Network tab
4. Traduções não aparecem
5. Tem erros no console
6. Não testou interação (clique, submit, etc)

### MIGRATION NÃO ESTÁ FEITA SE:
1. Não aplicou no banco
2. Não verificou tabelas no MySQL
3. Não verificou se colunas/campos foram criados
4. Não testou inserir dados

**LEMBRE-SE**: 
- Build não é opcional. 
- Testes não são opcionais. 
- Validação não é opcional.
- Ver logs não é opcional.
- Fazer login não é opcional.
- Verificar banco não é opcional.

## ⚡ ANTES DE DIZER "ESTÁ PRONTO":

**PARE. RESPIRE. PERGUNTE-SE:**

1. ✅ Eu REALMENTE testei isso?
2. ✅ Eu vi funcionar com meus próprios olhos (Swagger/navegador)?
3. ✅ Eu vi os dados no banco de dados?
4. ✅ Eu vi os logs sem erros?
5. ✅ Eu fiz login e usei o token?
6. ✅ Eu verifiquei TODAS as traduções?
7. ✅ Eu verifiquei console do navegador?

**SE QUALQUER RESPOSTA FOR "NÃO", TRABALHO NÃO ESTÁ PRONTO.**
