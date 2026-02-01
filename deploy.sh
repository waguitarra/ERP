#!/bin/bash
# ============================================
# WMS Admin - Script de Deploy para Produção
# ============================================

SERVER="root@178.18.252.13"
REMOTE_PATH="/opt/wms-admin"

echo "=========================================="
echo "🚀 WMS Admin - Deploy para Produção"
echo "=========================================="

# Criar arquivo de exclusão temporário
cat > /tmp/rsync_exclude.txt << EOF
node_modules
.angular
dist
.git
*.log
backups
.env
EOF

echo ""
echo "📦 Sincronizando arquivos com o servidor..."
rsync -avz --progress --delete \
    --exclude-from=/tmp/rsync_exclude.txt \
    ./ ${SERVER}:${REMOTE_PATH}/

echo ""
echo "🔧 Configurando ambiente no servidor..."
ssh ${SERVER} << 'REMOTE_SCRIPT'
cd /opt/wms-admin

# Copiar .env de produção
cp .env.prod .env

# Parar containers existentes (se houver)
docker compose -f docker-compose.prod.yml down 2>/dev/null

# Build e start dos containers
echo "🐳 Construindo e iniciando containers..."
docker compose -f docker-compose.prod.yml up -d --build

# Aguardar serviços
echo "⏳ Aguardando serviços..."
sleep 15

# Status
echo ""
echo "📊 Status dos containers:"
docker compose -f docker-compose.prod.yml ps

echo ""
echo "✅ Deploy concluído!"
echo "🌐 Frontend: http://178.18.252.13:4202"
REMOTE_SCRIPT

# Limpar arquivo temporário
rm -f /tmp/rsync_exclude.txt

echo ""
echo "=========================================="
echo "✅ Deploy finalizado!"
echo "=========================================="
