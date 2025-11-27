#!/bin/bash

# Script para adicionar injeção do I18nService em componentes que usam i18n.t() no template

echo "Procurando componentes que precisam do I18nService..."

# Encontrar todos os componentes TypeScript
for ts_file in $(find /home/wagnerfb/Projetos/ERP/APP/src/app/features -name "*.component.ts"); do
    html_file="${ts_file%.ts}.html"
    
    # Verificar se o HTML usa i18n.t()
    if [ -f "$html_file" ] && grep -q "i18n.t(" "$html_file"; then
        # Verificar se o TS já tem i18n injetado
        if ! grep -q "protected.*i18n.*=.*inject(I18nService)" "$ts_file" && ! grep -q "i18n.*:.*I18nService" "$ts_file"; then
            echo "Corrigindo: $ts_file"
            
            # Adicionar import do I18nService se não existir
            if ! grep -q "import.*I18nService" "$ts_file"; then
                sed -i "/^import.*from '@core\/services\/auth.service';/a import { I18nService } from '@core/services/i18n.service';" "$ts_file"
            fi
            
            # Adicionar injeção do i18n após outras injeções
            if grep -q "private readonly.*= inject(" "$ts_file"; then
                # Adicionar após a última injeção existente
                sed -i "/private readonly.*= inject(.*);/a \  protected readonly i18n = inject(I18nService);" "$ts_file"
            elif grep -q "readonly.*= inject(" "$ts_file"; then
                sed -i "/readonly.*= inject(.*);/a \  protected readonly i18n = inject(I18nService);" "$ts_file"
            else
                # Se não há injeções, adicionar após a declaração da classe
                sed -i "/export class.*Component {/a \  protected readonly i18n = inject(I18nService);" "$ts_file"
            fi
        fi
    fi
done

echo "Correção de injeções concluída!"
