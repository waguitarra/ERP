#!/usr/bin/env python3
import os
import re

# Lista de componentes que precisam do I18nService
components = [
    "src/app/features/customers/customer-create-modal/customer-create-modal.component.ts",
    "src/app/features/customers/customer-edit-modal/customer-edit-modal.component.ts",
    "src/app/features/suppliers/supplier-create-modal/supplier-create-modal.component.ts",
    "src/app/features/suppliers/supplier-edit-modal/supplier-edit-modal.component.ts",
    "src/app/features/companies/company-create-modal/company-create-modal.component.ts",
    "src/app/features/companies/company-edit-modal/company-edit-modal.component.ts",
    "src/app/features/products/product-create-modal/product-create-modal.component.ts",
    "src/app/features/products/product-edit-modal/product-edit-modal.component.ts",
    "src/app/features/warehouses/warehouse-create-modal/warehouse-create-modal.component.ts",
    "src/app/features/warehouses/warehouse-edit-modal/warehouse-edit-modal.component.ts",
    "src/app/features/vehicles/vehicle-create-modal/vehicle-create-modal.component.ts",
    "src/app/features/vehicles/vehicle-edit-modal/vehicle-edit-modal.component.ts",
    "src/app/features/drivers/driver-create-modal/driver-create-modal.component.ts",
    "src/app/features/drivers/driver-edit-modal/driver-edit-modal.component.ts",
    "src/app/features/storage-locations/storage-location-create-modal/storage-location-create-modal.component.ts",
    "src/app/features/storage-locations/storage-location-edit-modal/storage-location-edit-modal.component.ts",
    "src/app/features/orders/order-create-modal/order-create-modal.component.ts",
    "src/app/features/orders/order-edit-modal/order-edit-modal.component.ts",
    "src/app/shared/components/customer-selector-modal/customer-selector-modal.component.ts",
]

for comp_path in components:
    filepath = os.path.join("/home/wagnerfb/Projetos/ERP/APP", comp_path)
    
    if not os.path.exists(filepath):
        print(f"Arquivo não encontrado: {filepath}")
        continue
    
    with open(filepath, 'r') as f:
        content = f.read()
    
    # Verificar se já tem i18n
    if 'i18n' in content and 'I18nService' in content:
        print(f"Já possui i18n: {comp_path}")
        continue
    
    print(f"Adicionando i18n: {comp_path}")
    
    # Adicionar import se não existir
    if "I18nService" not in content:
        # Procurar a última linha de import
        import_lines = []
        for i, line in enumerate(content.split('\n')):
            if line.startswith('import '):
                import_lines.append(i)
        
        if import_lines:
            lines = content.split('\n')
            last_import_idx = import_lines[-1]
            lines.insert(last_import_idx + 1, "import { I18nService } from '@core/services/i18n.service';")
            content = '\n'.join(lines)
    
    # Adicionar injeção
    if 'protected readonly i18n' not in content and 'protected i18n' not in content:
        # Procurar a primeira injeção com inject() para adicionar após ela
        lines = content.split('\n')
        inject_line_idx = None
        
        for i, line in enumerate(lines):
            if 'inject(' in line and ('private' in line or 'protected' in line or 'readonly' in line):
                inject_line_idx = i
        
        if inject_line_idx is not None:
            # Adicionar após a última injeção
            lines.insert(inject_line_idx + 1, "  protected readonly i18n = inject(I18nService);")
            content = '\n'.join(lines)
    
    # Salvar arquivo
    with open(filepath, 'w') as f:
        f.write(content)

print("Processamento concluído!")
