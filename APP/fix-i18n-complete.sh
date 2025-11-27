#!/bin/bash

# Script completo para corrigir injeção do I18nService

echo "Iniciando correção completa do I18nService..."

# Lista de componentes que usam i18n no HTML
COMPONENTS=(
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/warehouses/warehouse-edit-modal/warehouse-edit-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/warehouses/warehouse-create-modal/warehouse-create-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/customers/customer-create-modal/customer-create-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/customers/customer-edit-modal/customer-edit-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/suppliers/supplier-create-modal/supplier-create-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/suppliers/supplier-edit-modal/supplier-edit-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/companies/company-create-modal/company-create-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/companies/company-edit-modal/company-edit-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/products/product-create-modal/product-create-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/products/product-edit-modal/product-edit-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/vehicles/vehicle-create-modal/vehicle-create-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/vehicles/vehicle-edit-modal/vehicle-edit-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/drivers/driver-create-modal/driver-create-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/drivers/driver-edit-modal/driver-edit-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/storage-locations/storage-location-create-modal/storage-location-create-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/storage-locations/storage-location-edit-modal/storage-location-edit-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/orders/order-create-modal/order-create-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/features/orders/order-edit-modal/order-edit-modal.component.ts"
  "/home/wagnerfb/Projetos/ERP/APP/src/app/shared/components/customer-selector-modal/customer-selector-modal.component.ts"
)

for ts_file in "${COMPONENTS[@]}"; do
  if [ -f "$ts_file" ]; then
    echo "Processando: $ts_file"
    
    # Remover duplicações de i18n se existirem
    awk '!seen[$0]++ || !/protected readonly i18n = inject/' "$ts_file" > "$ts_file.tmp" && mv "$ts_file.tmp" "$ts_file"
    
    # Adicionar import do I18nService se não existir
    if ! grep -q "import.*I18nService" "$ts_file"; then
      # Encontrar a última linha de import
      last_import=$(grep -n "^import" "$ts_file" | tail -1 | cut -d: -f1)
      if [ -n "$last_import" ]; then
        sed -i "${last_import}a import { I18nService } from '@core/services/i18n.service';" "$ts_file"
      fi
    fi
    
    # Adicionar injeção do i18n se não existir
    if ! grep -q "i18n.*=.*inject(I18nService)" "$ts_file"; then
      # Encontrar onde adicionar (após outras injeções ou após declaração da classe)
      if grep -q "readonly.*= inject(" "$ts_file"; then
        # Adicionar após a última injeção
        last_inject=$(grep -n "readonly.*= inject(.*);$" "$ts_file" | tail -1 | cut -d: -f1)
        if [ -n "$last_inject" ]; then
          sed -i "${last_inject}a \  protected readonly i18n = inject(I18nService);" "$ts_file"
        fi
      else
        # Adicionar após declaração da classe
        class_line=$(grep -n "export class.*Component" "$ts_file" | cut -d: -f1)
        if [ -n "$class_line" ]; then
          sed -i "$((class_line + 1))i \  protected readonly i18n = inject(I18nService);" "$ts_file"
        fi
      fi
    fi
  fi
done

echo "Correção concluída!"
