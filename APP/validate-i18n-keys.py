#!/usr/bin/env python3
import os
import re
import json
from pathlib import Path

# Caminhos dos arquivos JSON
json_files = {
    'pt-BR': '/home/wagnerfb/Projetos/ERP/APP/src/assets/i18n/pt-BR.json',
    'en-US': '/home/wagnerfb/Projetos/ERP/APP/src/assets/i18n/en-US.json',
    'es-ES': '/home/wagnerfb/Projetos/ERP/APP/src/assets/i18n/es-ES.json',
}

# Carregar JSONs
translations = {}
for lang, filepath in json_files.items():
    with open(filepath, 'r', encoding='utf-8') as f:
        translations[lang] = json.load(f)

# Função para obter valor aninhado
def get_nested_value(data, key_path):
    keys = key_path.split('.')
    value = data
    for key in keys:
        if isinstance(value, dict) and key in value:
            value = value[key]
        else:
            return None
    return value

# Função para definir valor aninhado
def set_nested_value(data, key_path, value):
    keys = key_path.split('.')
    current = data
    for i, key in enumerate(keys[:-1]):
        if key not in current:
            current[key] = {}
        current = current[key]
    current[keys[-1]] = value

# Extrair todas as chaves usadas nos HTMLs
html_dir = '/home/wagnerfb/Projetos/ERP/APP/src/app'
used_keys = set()

for root, dirs, files in os.walk(html_dir):
    for file in files:
        if file.endswith('.html'):
            filepath = os.path.join(root, file)
            with open(filepath, 'r', encoding='utf-8') as f:
                content = f.read()
                # Procurar por i18n.t('key') ou i18n.t("key")
                matches = re.findall(r"i18n\.t\(['\"]([^'\"]+)['\"]\)", content)
                used_keys.update(matches)

print(f"\n📊 Total de chaves usadas nos HTMLs: {len(used_keys)}")
print(f"📊 Total de chaves únicas: {len(set(used_keys))}\n")

# Verificar quais chaves faltam em cada JSON
missing_keys = {lang: [] for lang in json_files.keys()}

for key in sorted(used_keys):
    for lang in json_files.keys():
        value = get_nested_value(translations[lang], key)
        if value is None:
            missing_keys[lang].append(key)

# Mostrar chaves faltantes
print("=" * 80)
print("CHAVES FALTANTES POR IDIOMA")
print("=" * 80)

for lang, keys in missing_keys.items():
    print(f"\n🔴 {lang}: {len(keys)} chaves faltantes")
    if keys:
        for key in sorted(keys)[:20]:  # Mostrar primeiras 20
            print(f"   - {key}")
        if len(keys) > 20:
            print(f"   ... e mais {len(keys) - 20} chaves")

# Salvar lista completa de chaves faltantes
with open('/home/wagnerfb/Projetos/ERP/APP/missing-keys-report.txt', 'w', encoding='utf-8') as f:
    f.write("RELATÓRIO DE CHAVES FALTANTES\n")
    f.write("=" * 80 + "\n\n")
    for lang, keys in missing_keys.items():
        f.write(f"\n{lang} - {len(keys)} chaves faltantes:\n")
        f.write("-" * 80 + "\n")
        for key in sorted(keys):
            f.write(f"{key}\n")

print("\n\n✅ Relatório completo salvo em: missing-keys-report.txt")
print("\nChaves exemplo que faltam:")
print(f"  - auth.login.forgotPassword: {get_nested_value(translations['pt-BR'], 'auth.login.forgotPassword')}")
print(f"  - auth.login.loginButton: {get_nested_value(translations['pt-BR'], 'auth.login.loginButton')}")
print(f"  - auth.login.footer: {get_nested_value(translations['pt-BR'], 'auth.login.footer')}")
