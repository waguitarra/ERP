#!/bin/bash

echo "=========================================="
echo "🛑 Parando todos os containers Docker..."
echo "=========================================="

# Para e remove todos os containers do projeto (SEM deletar volumes/dados)
# ATENÇÃO: Removido o -v para preservar dados do MySQL
docker compose down 2>/dev/null

# Mata qualquer container órfão
docker ps -aq | xargs -r docker stop 2>/dev/null
docker ps -aq | xargs -r docker rm 2>/dev/null

echo ""
echo "=========================================="
echo "🔪 Liberando portas 3308, 5000, 4200..."
echo "=========================================="

# Mata processos nas portas usadas pelo projeto
kill_port() {
    local port=$1
    local pids=$(lsof -t -i:$port 2>/dev/null)
    if [ -n "$pids" ]; then
        echo "Matando processos na porta $port: $pids"
        echo $pids | xargs -r kill -9 2>/dev/null
    else
        echo "Porta $port já está livre"
    fi
}

kill_port 3308   # MySQL
kill_port 5000   # API Backend
kill_port 4200   # Frontend Angular

# Alternativa com fuser se lsof não funcionar
fuser -k 3308/tcp 2>/dev/null
fuser -k 5000/tcp 2>/dev/null
fuser -k 4200/tcp 2>/dev/null

echo ""
echo "=========================================="
echo "🧹 Limpando recursos Docker não usados..."
echo "=========================================="

# Remove networks órfãs do projeto
docker network prune -f 2>/dev/null

echo ""
echo "=========================================="
echo "⏳ Aguardando portas ficarem livres..."
echo "=========================================="
sleep 3

echo ""
echo "=========================================="
echo "🚀 Iniciando containers Docker..."
echo "=========================================="

# Rebuild e inicia todos os containers
docker compose up -d --build

echo ""
echo "=========================================="
echo "⏳ Aguardando serviços ficarem prontos..."
echo "=========================================="

# Aguarda MySQL ficar pronto
echo "Aguardando MySQL..."
until docker compose exec -T mysql mysqladmin ping -h localhost -u root -proot123 --silent 2>/dev/null; do
    sleep 2
    echo -n "."
done
echo " ✅ MySQL pronto!"

# Aguarda API ficar pronta
echo "Aguardando API..."
max_attempts=60
attempt=0
until curl -s http://localhost:5000/health > /dev/null 2>&1 || [ $attempt -ge $max_attempts ]; do
    sleep 2
    echo -n "."
    ((attempt++))
done

if [ $attempt -ge $max_attempts ]; then
    echo " ⚠️ API demorou para iniciar, mas continuando..."
else
    echo " ✅ API pronta!"
fi

# Aguarda Frontend ficar pronto
echo "Aguardando Frontend..."
attempt=0
until curl -s http://localhost:4200 > /dev/null 2>&1 || [ $attempt -ge $max_attempts ]; do
    sleep 2
    echo -n "."
    ((attempt++))
done

if [ $attempt -ge $max_attempts ]; then
    echo " ⚠️ Frontend demorou para iniciar, mas continuando..."
else
    echo " ✅ Frontend pronto!"
fi

echo ""
echo "=========================================="
echo "📊 Status dos containers:"
echo "=========================================="
docker compose ps

echo ""
echo "=========================================="
echo "✅ Sistema ERP iniciado com sucesso!"
echo "=========================================="
echo ""
echo "🌐 URLs disponíveis:"
echo "   Frontend: http://localhost:4200"
echo "   API:      http://localhost:5000"
echo "   MySQL:    localhost:3308"
echo ""
echo "🔐 Credenciais:"
echo "   Email: admin@WMS.com"
echo "   Senha: admin@123456"
echo ""
echo "📋 Logs em tempo real: docker compose logs -f"
echo "=========================================="
