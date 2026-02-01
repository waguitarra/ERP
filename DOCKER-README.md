# 🚀 ERP WMS ADMIN - Docker Setup

Sistema ERP completo containerizado com Docker.

## 📦 Arquitetura

```
┌─────────────────────────────────────────────────────────────┐
│                     Docker Compose                           │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐   │
│  │   Frontend   │    │     API      │    │    MySQL     │   │
│  │   (Angular)  │───▶│   (.NET 8)   │───▶│     8.0      │   │
│  │   :4200      │    │    :5000     │    │    :3307     │   │
│  └──────────────┘    └──────────────┘    └──────────────┘   │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

## 🚀 Quick Start

### 1. Iniciar todos os containers

```bash
./docker.sh start
```

### 2. Acessar a aplicação

- **Frontend:** http://localhost:4200
- **API Swagger:** http://localhost:5000/swagger
- **MySQL:** localhost:3307

## 🔑 Credenciais de Acesso

| Serviço | Usuário | Senha |
|---------|---------|-------|
| **Sistema** | admin@WMS.com | admin@123456 |
| **MySQL** | logistics_user | password |
| **MySQL Root** | root | root123 |

## 📋 Comandos Disponíveis

```bash
# Iniciar containers
./docker.sh start

# Parar containers
./docker.sh stop

# Reiniciar containers
./docker.sh restart

# Ver logs
./docker.sh logs           # Todos
./docker.sh logs-api       # Apenas API
./docker.sh logs-frontend  # Apenas Frontend
./docker.sh logs-mysql     # Apenas MySQL

# Status dos containers
./docker.sh status

# Acessar shell dos containers
./docker.sh shell-api      # Shell da API
./docker.sh shell-mysql    # MySQL CLI

# Operações do banco
./docker.sh migrate        # Executar migrations
./docker.sh seed           # Popular com dados de teste

# Limpeza completa
./docker.sh clean

# Ver credenciais
./docker.sh credentials
```

## 🔧 Configuração Manual

### Build e Start

```bash
docker-compose up -d --build
```

### Parar

```bash
docker-compose down
```

### Limpar tudo (incluindo volumes)

```bash
docker-compose down -v --rmi local
```

## 🌐 Variáveis de Ambiente

### API (.NET)

| Variável | Descrição | Padrão |
|----------|-----------|--------|
| ConnectionStrings__DefaultConnection | String de conexão MySQL | Server=mysql;Database=logistics_db;... |
| JwtSettings__Secret | Chave secreta JWT | logistics-super-secret-key... |
| ASPNETCORE_ENVIRONMENT | Ambiente | Production |

### MySQL

| Variável | Descrição | Padrão |
|----------|-----------|--------|
| MYSQL_ROOT_PASSWORD | Senha root | root123 |
| MYSQL_DATABASE | Nome do banco | logistics_db |
| MYSQL_USER | Usuário | logistics_user |
| MYSQL_PASSWORD | Senha do usuário | password |

## 📁 Estrutura de Arquivos Docker

```
ERP/
├── docker-compose.yml      # Orquestração dos containers
├── docker.sh               # Script de gerenciamento
├── docker/
│   └── mysql/
│       └── init/
│           └── 01-init.sql # Script inicial do MySQL
├── API/
│   └── Dockerfile          # Build da API .NET
└── APP/
    ├── Dockerfile          # Build do Frontend Angular
    └── docker/
        └── nginx.conf      # Configuração do Nginx
```

## 🐛 Troubleshooting

### Porta já em uso

```bash
# Verificar o que está usando a porta
lsof -i :5000
lsof -i :4200
lsof -i :3307

# Matar processo
kill -9 <PID>
```

### Container não inicia

```bash
# Ver logs detalhados
docker-compose logs api
docker-compose logs frontend
docker-compose logs mysql
```

### Resetar banco de dados

```bash
# Remove volume do MySQL e recria
docker-compose down -v
docker-compose up -d --build
```

### Rebuild forçado

```bash
docker-compose build --no-cache
docker-compose up -d
```

## 📊 Health Checks

- **API:** http://localhost:5000/api/health
- **MySQL:** `mysqladmin ping`

## 🔐 Segurança

⚠️ **ATENÇÃO:** As configurações padrão são para desenvolvimento/demonstração.

Para produção, altere:
1. Senhas do MySQL
2. JWT Secret
3. Remova credenciais de demo da tela de login
4. Configure HTTPS
5. Use secrets do Docker/Kubernetes

---

**WMS ADMIN** © 2024 - Sistema ERP Containerizado
