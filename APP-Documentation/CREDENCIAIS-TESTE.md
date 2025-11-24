# 🔐 CREDENCIAIS DE TESTE - NEXUS ADMIN

## Usuário Administrador

**Email:** `admin@nexus.com`  
**Senha:** `Admin@123456`  
**Role:** Admin Master (sem vínculo com empresa)

---

## Informações do Sistema

### Backend (API)
- **URL:** http://localhost:5000
- **Status:** ✅ Rodando (PID: 280210)
- **Banco:** MySQL - logistics_db

### Frontend (Angular)
- **URL:** http://localhost:4200 (quando iniciar)
- **Build:** ✅ Compilado com sucesso
- **Tamanho:** 1.43 MB (initial) + lazy chunks

---

## Como Testar

### 1. Iniciar Frontend
```bash
cd /home/wagnerfb/Projetos/ERP/APP
npm start
```

### 2. Acessar
- Abrir navegador em: http://localhost:4200
- Fazer login com as credenciais acima
- Testar navegação entre módulos

### 3. Testar API Diretamente
```bash
# Login (testado e funcionando ✅)
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@nexus.com",
    "password": "Admin@123456"
  }'

# Listar Produtos (com token)
curl -X GET http://localhost:5000/api/products \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

---

## Status Atual

✅ Backend rodando (http://localhost:5000)  
✅ Banco de dados configurado  
✅ Usuário criado via API (testado com CURL)  
✅ Login funcionando (validado com CURL)  
✅ Dependências Angular instaladas  
✅ Build Angular compilado (331.58 kB)  
✅ Environment URLs corrigidas (localhost:5000)  
✅ Autocomplete adicionado nos inputs  
✅ Modo escuro implementado  
✅ Todas as rotas configuradas  

---

## Próximos Passos

1. Iniciar o frontend: `npm start`
2. Testar login
3. Navegar pelos módulos
4. Testar modo escuro (botão no header)
5. Verificar integração com API

---

**Tudo pronto para desenvolvimento! 🚀**
