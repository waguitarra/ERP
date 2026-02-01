#!/bin/bash

# ============================================
# Script para gerenciar o ambiente Docker
# ============================================

set -e

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_header() {
    echo -e "${BLUE}"
    echo "╔══════════════════════════════════════════════════════════════╗"
    echo "║                    ERP WMS ADMIN                           ║"
    echo "║                Docker Management Script                       ║"
    echo "╚══════════════════════════════════════════════════════════════╝"
    echo -e "${NC}"
}

print_credentials() {
    echo -e "${GREEN}"
    echo "╔══════════════════════════════════════════════════════════════╗"
    echo "║                   CREDENCIAIS DE ACESSO                       ║"
    echo "╠══════════════════════════════════════════════════════════════╣"
    echo "║  📧 Email:    admin@WMS.com                                 ║"
    echo "║  🔑 Senha:    admin@123456                                    ║"
    echo "╠══════════════════════════════════════════════════════════════╣"
    echo "║                        URLS                                   ║"
    echo "╠══════════════════════════════════════════════════════════════╣"
    echo "║  🌐 Frontend: http://localhost:4200                           ║"
    echo "║  🔧 API:      http://localhost:5000                           ║"
    echo "║  📊 Swagger:  http://localhost:5000/swagger                   ║"
    echo "║  🗄️  MySQL:    localhost:3307 (user: logistics_user)          ║"
    echo "╚══════════════════════════════════════════════════════════════╝"
    echo -e "${NC}"
}

case "$1" in
    start|up)
        print_header
        echo -e "${YELLOW}🚀 Iniciando containers...${NC}"
        docker compose up -d --build
        echo ""
        echo -e "${GREEN}✅ Containers iniciados com sucesso!${NC}"
        echo ""
        echo -e "${YELLOW}⏳ Aguarde alguns segundos para os serviços iniciarem...${NC}"
        sleep 10
        print_credentials
        ;;
    
    stop|down)
        print_header
        echo -e "${YELLOW}🛑 Parando containers...${NC}"
        docker compose down
        echo -e "${GREEN}✅ Containers parados!${NC}"
        ;;
    
    restart)
        print_header
        echo -e "${YELLOW}🔄 Reiniciando containers...${NC}"
        docker compose down
        docker compose up -d --build
        echo -e "${GREEN}✅ Containers reiniciados!${NC}"
        sleep 10
        print_credentials
        ;;
    
    logs)
        docker compose logs -f "${2:-}"
        ;;
    
    logs-api)
        docker compose logs -f api
        ;;
    
    logs-frontend)
        docker compose logs -f frontend
        ;;
    
    logs-mysql)
        docker compose logs -f mysql
        ;;
    
    status)
        print_header
        echo -e "${YELLOW}📊 Status dos containers:${NC}"
        docker compose ps
        ;;
    
    clean)
        print_header
        echo -e "${RED}🧹 Limpando containers e volumes...${NC}"
        docker compose down -v --rmi local
        echo -e "${GREEN}✅ Limpeza concluída!${NC}"
        ;;
    
    shell-api)
        docker compose exec api /bin/bash
        ;;
    
    shell-mysql)
        docker compose exec mysql mysql -u logistics_user -ppassword logistics_db
        ;;
    
    migrate)
        echo -e "${YELLOW}🔄 Executando migrations...${NC}"
        docker compose exec api dotnet ef database update
        echo -e "${GREEN}✅ Migrations executadas!${NC}"
        ;;
    
    seed)
        echo -e "${YELLOW}🌱 Populando banco de dados...${NC}"
        curl -X POST http://localhost:5000/api/DataSeeder/seed-all
        echo ""
        echo -e "${GREEN}✅ Dados populados!${NC}"
        ;;
    
    credentials|creds)
        print_credentials
        ;;
    
    *)
        print_header
        echo "Uso: $0 {comando}"
        echo ""
        echo "Comandos disponíveis:"
        echo "  start|up        - Inicia todos os containers"
        echo "  stop|down       - Para todos os containers"
        echo "  restart         - Reinicia todos os containers"
        echo "  logs [serviço]  - Mostra logs (api, frontend, mysql)"
        echo "  logs-api        - Mostra logs da API"
        echo "  logs-frontend   - Mostra logs do Frontend"
        echo "  logs-mysql      - Mostra logs do MySQL"
        echo "  status          - Mostra status dos containers"
        echo "  clean           - Remove containers e volumes"
        echo "  shell-api       - Acessa shell do container da API"
        echo "  shell-mysql     - Acessa MySQL CLI"
        echo "  migrate         - Executa migrations do EF Core"
        echo "  seed            - Popula banco com dados de teste"
        echo "  credentials     - Mostra credenciais de acesso"
        echo ""
        print_credentials
        ;;
esac
