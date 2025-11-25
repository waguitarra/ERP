#!/bin/bash

export PATH=$PATH:$HOME/.dotnet

# Script para criar migration EF Core
# Uso: bash criar-migration.sh

cd /home/wagnerfb/Projetos/ERP/API/src/Logistics.API

echo "🔄 Criando migration AddOrderStatusPriorityAndWMSFields..."
dotnet ef migrations add AddOrderStatusPriorityAndWMSFields -p ../Logistics.Infrastructure -s .

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Migration criada com sucesso!"
    echo ""
    echo "📋 Próximo passo: Aplicar migration no banco"
    echo "Execute: dotnet ef database update"
    echo ""
    echo "Ou use: bash aplicar-migration.sh"
else
    echo ""
    echo "❌ Erro ao criar migration"
    echo "Verifique os erros acima"
fi
