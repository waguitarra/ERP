#!/bin/bash

export PATH=$PATH:$HOME/.dotnet

# Script para aplicar migration no banco de dados
# Uso: bash aplicar-migration.sh

cd /home/wagnerfb/Projetos/ERP/API/src/Logistics.API

echo "🔄 Aplicando migrations no banco de dados..."
dotnet ef database update

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Migration aplicada com sucesso!"
    echo ""
    echo "📋 Verificações:"
    echo "1. Tabela OrderStatuses criada (10 registros)"
    echo "2. Tabela OrderPriorities criada (4 registros)"
    echo "3. Orders atualizado com 15 campos WMS"
    echo ""
    echo "🚀 Próximo passo: Reiniciar API e testar no Swagger"
    echo "Execute: cd /home/wagnerfb/Projetos/ERP && bash restart-app.sh"
else
    echo ""
    echo "❌ Erro ao aplicar migration"
    echo "Verifique os erros acima"
fi
