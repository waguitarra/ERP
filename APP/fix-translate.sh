#!/bin/bash

# Script para substituir pipe translate por i18n.t()

FILES=$(find /home/wagnerfb/Projetos/ERP/APP/src/app -name "*.html" -type f)

for file in $FILES; do
  if grep -q "| translate" "$file"; then
    echo "Corrigindo: $file"
    
    # Substituir {{ 'key' | translate }} por {{ i18n.t('key') }}
    sed -i "s/{{\s*'\([^']*\)'\s*|\s*translate\s*}}/{{ i18n.t('\1') }}/g" "$file"
    
    # Substituir {{ "key" | translate }} por {{ i18n.t("key") }}
    sed -i 's/{{\s*"\([^"]*\)"\s*|\s*translate\s*}}/{{ i18n.t("\1") }}/g' "$file"
    
    # Substituir [placeholder]="'key' | translate" por [placeholder]="i18n.t('key')"
    sed -i "s/\[placeholder\]=\"'\([^']*\)'\s*|\s*translate\"/[placeholder]=\"i18n.t('\1')\"/g" "$file"
    
    # Substituir [title]="'key' | translate" por [title]="i18n.t('key')"
    sed -i "s/\[title\]=\"'\([^']*\)'\s*|\s*translate\"/[title]=\"i18n.t('\1')\"/g" "$file"
    
    # Substituir [label]="'key' | translate" por [label]="i18n.t('key')"
    sed -i "s/\[label\]=\"'\([^']*\)'\s*|\s*translate\"/[label]=\"i18n.t('\1')\"/g" "$file"
    
    # Substituir qualquer outro atributo binding
    sed -i "s/=\"'\([^']*\)'\s*|\s*translate\"/=\"i18n.t('\1')\"/g" "$file"
    
  fi
done

echo "Correção concluída!"
