# WMS Admin - Sistema de Gestão de Armazéns

Sistema ERP completo para gestão logística e de armazéns.

---

## 🌐 Acesso Produção

| Serviço | URL | Observação |
|---------|-----|------------|
| **Frontend** | http://178.18.252.13:4202 | Acesso público |
| **API** | Interno (via proxy) | Não exposto externamente |
| **MySQL** | Interno | Não exposto externamente |

### Credenciais Produção
- **Email:** `admin@nexus.com`
- **Senha:** `admin@123456`

---

## 🏗️ Arquitetura de Produção

```
┌─────────────────────────────────────────────────────────┐
│                    SERVIDOR (178.18.252.13)             │
├─────────────────────────────────────────────────────────┤
│  ┌─────────────────┐                                    │
│  │   FRONTEND      │ ◄── Porta 4202 (PÚBLICA)          │
│  │   (Nginx)       │                                    │
│  └────────┬────────┘                                    │
│           │ /api/* (proxy interno)                      │
│  ┌────────▼────────┐                                    │
│  │      API        │ ◄── Porta 5000 (INTERNA)          │
│  │   (.NET 8)      │     Não acessível externamente    │
│  └────────┬────────┘                                    │
│           │                                             │
│  ┌────────▼────────┐                                    │
│  │     MySQL       │ ◄── Porta 3306 (INTERNA)          │
│  │      8.0        │     Não acessível externamente    │
│  └─────────────────┘                                    │
│                                                         │
│  Rede interna Docker: wms-internal (isolada)           │
└─────────────────────────────────────────────────────────┘
```

### Portas Utilizadas no Servidor

| Porta | Serviço | Status |
|-------|---------|--------|
| 22 | SSH | Em uso |
| 80 | Nginx (outro projeto) | Em uso |
| 443 | Nginx SSL (outro projeto) | Em uso |
| 3000 | Outro projeto | Em uso |
| 3307 | Outro MySQL | Em uso |
| **4202** | **WMS Admin Frontend** | **NOVO** |
| 4201 | Outro projeto | Em uso |
| 5001 | Outro projeto | Em uso |

---

## 📁 Estrutura do Projeto

```
/opt/wms-admin/           # Diretório de produção no servidor
├── API/                  # Backend .NET 8
├── APP/                  # Frontend Angular 19
├── docker-compose.prod.yml  # Compose de produção
├── .env.prod             # Variáveis de ambiente
└── ...
```

---

## 🚀 Deploy Manual (Passo a Passo)

### 1. Copiar arquivos para o servidor
```bash
rsync -avz --progress \
    --exclude='node_modules' \
    --exclude='.angular' \
    --exclude='dist' \
    --exclude='.git' \
    --exclude='backups' \
    ./ root@178.18.252.13:/opt/wms-admin/
```

### 2. Conectar ao servidor
```bash
ssh root@178.18.252.13
cd /opt/wms-admin
```

### 3. Configurar ambiente
```bash
cp .env.prod .env
```

### 4. Subir MySQL
```bash
docker compose -f docker-compose.prod.yml up -d mysql
# Aguardar ficar healthy
docker compose -f docker-compose.prod.yml ps
```

### 5. Subir API
```bash
docker compose -f docker-compose.prod.yml up -d api
# Aguardar build e migrations
docker compose -f docker-compose.prod.yml logs -f api
```

### 6. Subir Frontend
```bash
docker compose -f docker-compose.prod.yml up -d frontend
```

### 7. Verificar status
```bash
docker compose -f docker-compose.prod.yml ps
```

---

## 🔧 Comandos Úteis (Produção)

### Ver status dos containers
```bash
docker compose -f docker-compose.prod.yml ps
```

### Ver logs em tempo real
```bash
docker compose -f docker-compose.prod.yml logs -f
docker compose -f docker-compose.prod.yml logs -f api
docker compose -f docker-compose.prod.yml logs -f frontend
```

### Reiniciar serviços
```bash
docker compose -f docker-compose.prod.yml restart
```

### Parar tudo
```bash
docker compose -f docker-compose.prod.yml down
```

### Rebuild completo
```bash
docker compose -f docker-compose.prod.yml down
docker compose -f docker-compose.prod.yml up -d --build
```

---

## 🖥️ Desenvolvimento Local

### Requisitos
- Docker & Docker Compose
- Node.js 20+
- .NET 8 SDK

### Iniciar ambiente local
```bash
./start.sh
```

### URLs locais
- Frontend: http://localhost:4200
- API: http://localhost:5000
- MySQL: localhost:3308

### Credenciais locais
- Email: `admin@nexus.com`
- Senha: `admin@123456`

---

## 🔒 Segurança

### Boas Práticas Implementadas
1. **API não exposta externamente** - Acesso apenas via proxy nginx
2. **MySQL não exposto externamente** - Apenas rede Docker interna
3. **Rede Docker isolada** - `wms-internal` com `internal: true`
4. **CORS configurado** - AllowAnyOrigin para desenvolvimento
5. **Headers de segurança** no nginx:
   - X-Frame-Options: SAMEORIGIN
   - X-Content-Type-Options: nosniff
   - X-XSS-Protection: 1; mode=block

---

## ⚠️ Proteção dos Dados

### REGRAS OBRIGATÓRIAS
```
❌ NUNCA use: docker compose down -v    (isso APAGA todos os dados!)
❌ NUNCA delete o volume: wms-admin-mysql-data
✅ Use apenas: docker compose down      (sem -v)
✅ SEMPRE faça backup antes de manutenção
```

### Fazer Backup
```bash
# No servidor
docker exec wms-admin-mysql mysqldump -uroot -p'WmsAdmin@Prod2024!' logistics_db > /opt/wms-admin/backups/backup_$(date +%Y%m%d_%H%M%S).sql
```

### Restaurar Backup
```bash
# No servidor
docker exec -i wms-admin-mysql mysql -uroot -p'WmsAdmin@Prod2024!' logistics_db < /opt/wms-admin/backups/NOME_DO_ARQUIVO.sql
```

### Volume Protegido
O volume `wms-admin-mysql-data` está configurado como **externo** no docker-compose.prod.yml, o que significa:
- Não é excluído com `docker compose down -v`
- Precisa ser criado manualmente se não existir: `docker volume create wms-admin-mysql-data`

---

## 📊 Stack Tecnológica

| Componente | Tecnologia |
|------------|------------|
| Frontend | Angular 19, TailwindCSS |
| Backend | .NET 8, Entity Framework Core |
| Banco de Dados | MySQL 8.0 |
| Web Server | Nginx Alpine |
| Containerização | Docker, Docker Compose |

---

## 🆘 Troubleshooting

### API unhealthy
```bash
# Verificar logs
docker compose -f docker-compose.prod.yml logs api --tail 50

# Reiniciar
docker compose -f docker-compose.prod.yml restart api
```

### Frontend não carrega
```bash
# Verificar nginx
docker compose -f docker-compose.prod.yml logs frontend

# Rebuild
docker compose -f docker-compose.prod.yml up -d --build frontend
```

### Erro de conexão com banco
```bash
# Verificar MySQL
docker compose -f docker-compose.prod.yml logs mysql

# Verificar se está healthy
docker compose -f docker-compose.prod.yml ps
```