#!/bin/bash

# Remove imports e injeções duplicadas

echo "Removendo duplicações..."

# Lista de arquivos a corrigir
FILES=(
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/warehouses/warehouse-edit-modal/warehouse-edit-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/shared/components/customer-selector-modal/customer-selector-modal.component.ts"
)

for file in "${FILES[@]}"; do
  if [ -f "$file" ]; then
    echo "Limpando: $file"
    
    # Usar awk para remover linhas duplicadas mantendo apenas a primeira ocorrência
    awk '
    !seen[$0]++ {
      print
    }
    ' "$file" > "$file.tmp" && mv "$file.tmp" "$file"
  fi
done

echo "Duplicações removidas!"
