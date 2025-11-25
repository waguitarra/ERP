#!/bin/bash

export PATH=$PATH:$HOME/.dotnet

# Matar processos nas portas 4200 e 5000
kill -9 $(lsof -t -i:4200) 2>/dev/null
kill -9 $(lsof -t -i:5000) 2>/dev/null

# Iniciar backend
cd /home/wagnerfb/Projetos/ERP/API/src/Logistics.API
dotnet run &

# Iniciar frontend
cd /home/wagnerfb/Projetos/ERP/APP
npm start &

echo "✅ Aplicação reiniciada!"
echo "Frontend: http://localhost:4200"
echo "Backend: http://localhost:5000"
