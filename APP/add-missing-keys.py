#!/usr/bin/env python3
import json
from pathlib import Path

# Traduções das chaves faltantes
missing_translations = {
    'auth.login.footer': {
        'pt-BR': '© 2024 NEXUS ADMIN. Todos os direitos reservados.',
        'en-US': '© 2024 NEXUS ADMIN. All rights reserved.',
        'es-ES': '© 2024 NEXUS ADMIN. Todos los derechos reservados.'
    },
    'auth.login.forgotPassword': {
        'pt-BR': 'Esqueceu sua senha?',
        'en-US': 'Forgot your password?',
        'es-ES': '¿Olvidó su contraseña?'
    },
    'common.buttons.toggleDarkMode': {
        'pt-BR': 'Alternar modo escuro',
        'en-US': 'Toggle dark mode',
        'es-ES': 'Alternar modo oscuro'
    },
    'common.buttons.tryAgain': {
        'pt-BR': 'Tentar Novamente',
        'en-US': 'Try Again',
        'es-ES': 'Intentar de Nuevo'
    },
    'common.fields.actions': {
        'pt-BR': 'Ações',
        'en-US': 'Actions',
        'es-ES': 'Acciones'
    },
    'common.fields.status': {
        'pt-BR': 'Status',
        'en-US': 'Status',
        'es-ES': 'Estado'
    },
    'common.fields.type': {
        'pt-BR': 'Tipo',
        'en-US': 'Type',
        'es-ES': 'Tipo'
    },
    'header.searchPlaceholder': {
        'pt-BR': 'Buscar dados, usuários...',
        'en-US': 'Search data, users...',
        'es-ES': 'Buscar datos, usuarios...'
    },
    'orders.empty.createButton': {
        'pt-BR': 'Criar Primeiro Pedido',
        'en-US': 'Create First Order',
        'es-ES': 'Crear Primer Pedido'
    },
    'orders.empty.message': {
        'pt-BR': 'Nenhum pedido cadastrado ainda',
        'en-US': 'No orders registered yet',
        'es-ES': 'No hay pedidos registrados aún'
    },
    'orders.table.totalValue': {
        'pt-BR': 'Valor Total',
        'en-US': 'Total Value',
        'es-ES': 'Valor Total'
    },
    'suppliers.empty.title': {
        'pt-BR': 'Nenhum fornecedor encontrado',
        'en-US': 'No suppliers found',
        'es-ES': 'No se encontraron proveedores'
    },
    'suppliers.empty.subtitle': {
        'pt-BR': 'Comece adicionando seu primeiro fornecedor',
        'en-US': 'Start by adding your first supplier',
        'es-ES': 'Comience agregando su primer proveedor'
    },
    'suppliers.loading': {
        'pt-BR': 'Carregando fornecedores...',
        'en-US': 'Loading suppliers...',
        'es-ES': 'Cargando proveedores...'
    },
    'products.table.actions': {
        'pt-BR': 'Ações',
        'en-US': 'Actions',
        'es-ES': 'Acciones'
    },
    'products.table.category': {
        'pt-BR': 'Categoria',
        'en-US': 'Category',
        'es-ES': 'Categoría'
    },
    'products.table.price': {
        'pt-BR': 'Preço',
        'en-US': 'Price',
        'es-ES': 'Precio'
    },
    'products.table.product': {
        'pt-BR': 'Produto',
        'en-US': 'Product',
        'es-ES': 'Producto'
    },
    'products.table.sku': {
        'pt-BR': 'SKU',
        'en-US': 'SKU',
        'es-ES': 'SKU'
    },
    'products.table.status': {
        'pt-BR': 'Status',
        'en-US': 'Status',
        'es-ES': 'Estado'
    }
}

# Função para adicionar chave aninhada
def add_nested_key(data, key_path, value):
    keys = key_path.split('.')
    current = data
    for i, key in enumerate(keys[:-1]):
        if key not in current:
            current[key] = {}
        current = current[key]
    current[keys[-1]] = value

# Processar cada arquivo JSON
json_files = {
    'pt-BR': '/home/wagnerfb/Projetos/ERP/APP/src/assets/i18n/pt-BR.json',
    'en-US': '/home/wagnerfb/Projetos/ERP/APP/src/assets/i18n/en-US.json',
    'es-ES': '/home/wagnerfb/Projetos/ERP/APP/src/assets/i18n/es-ES.json',
}

for lang, filepath in json_files.items():
    print(f"\n🔧 Processando {lang}...")
    
    with open(filepath, 'r', encoding='utf-8') as f:
        data = json.load(f)
    
    keys_added = 0
    for key_path, translations in missing_translations.items():
        if lang in translations:
            add_nested_key(data, key_path, translations[lang])
            keys_added += 1
            print(f"   ✅ {key_path}: {translations[lang]}")
    
    # Salvar com formatação
    with open(filepath, 'w', encoding='utf-8') as f:
        json.dump(data, f, ensure_ascii=False, indent=2)
    
    print(f"   📊 Total de chaves adicionadas: {keys_added}")

print("\n✅ Todas as chaves faltantes foram adicionadas nos 3 idiomas!")
