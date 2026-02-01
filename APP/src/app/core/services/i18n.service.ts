import { Injectable, signal, computed } from '@angular/core';

export type Language = 'pt-BR' | 'en-US' | 'es-ES';

export interface TranslationData {
  [key: string]: string | TranslationData;
}

@Injectable({
  providedIn: 'root'
})
export class I18nService {
  private readonly STORAGE_KEY = 'WMS_language';
  
  // Idioma atual como signal
  private currentLanguageSignal = signal<Language>('pt-BR');
  currentLanguage = this.currentLanguageSignal.asReadonly();
  
  // Signal para indicar se as traduções foram carregadas
  private translationsLoadedSignal = signal<boolean>(false);
  translationsLoaded = this.translationsLoadedSignal.asReadonly();

  // Traduções carregadas
  private translationsSignal = signal<Record<Language, TranslationData>>({
    'pt-BR': {},
    'en-US': {},
    'es-ES': {}
  });

  // Traduções atuais baseadas no idioma selecionado (computed para reatividade)
  private currentTranslations = computed(() => {
    return this.translationsSignal()[this.currentLanguageSignal()];
  });

  // Contador de versão para forçar reatividade nos templates
  private translationVersion = signal(0);

  constructor() {
    // Carregar idioma salvo ou usar padrão do navegador
    const savedLanguage = localStorage.getItem(this.STORAGE_KEY) as Language;
    if (savedLanguage) {
      this.currentLanguageSignal.set(savedLanguage);
    } else {
      const browserLang = this.detectBrowserLanguage();
      this.currentLanguageSignal.set(browserLang);
    }

    // Carregar traduções
    this.loadTranslations();
  }

  /**
   * Detecta o idioma do navegador
   */
  private detectBrowserLanguage(): Language {
    const browserLang = navigator.language || 'pt-BR';
    
    if (browserLang.startsWith('pt')) return 'pt-BR';
    if (browserLang.startsWith('es')) return 'es-ES';
    return 'en-US';
  }

  /**
   * Carrega todos os arquivos de tradução
   */
  private async loadTranslations(): Promise<void> {
    try {
      const [ptBR, enUS, esES] = await Promise.all([
        fetch('/assets/i18n/pt-BR.json').then(r => r.json()),
        fetch('/assets/i18n/en-US.json').then(r => r.json()),
        fetch('/assets/i18n/es-ES.json').then(r => r.json())
      ]);

      this.translationsSignal.set({
        'pt-BR': ptBR,
        'en-US': enUS,
        'es-ES': esES
      });
      // Marcar como carregado e incrementar versão
      this.translationsLoadedSignal.set(true);
      this.translationVersion.update(v => v + 1);
    } catch (error) {
      console.error('Erro ao carregar traduções:', error);
      // Mesmo com erro, marcar como carregado para não bloquear a UI
      this.translationsLoadedSignal.set(true);
    }
  }

  /**
   * Altera o idioma atual
   */
  setLanguage(lang: Language): void {
    this.currentLanguageSignal.set(lang);
    localStorage.setItem(this.STORAGE_KEY, lang);
    // Incrementar versão para forçar reatividade nos templates
    this.translationVersion.update(v => v + 1);
  }

  /**
   * Retorna o nome legível do idioma
   */
  getLanguageName(lang: Language): string {
    const names: Record<Language, string> = {
      'pt-BR': 'Português',
      'en-US': 'English',
      'es-ES': 'Español'
    };
    return names[lang];
  }

  /**
   * Obtém uma tradução pela chave
   * Suporta chaves aninhadas: 'common.buttons.save'
   */
  translate(key: string, params?: Record<string, string | number>): string {
    const lang = this.currentLanguageSignal();
    const translations = this.translationsSignal()[lang];
    
    // Se traduções não carregadas, retorna a chave
    if (!translations || Object.keys(translations).length === 0) {
      return key;
    }
    
    // Navegar pelas chaves aninhadas
    const keys = key.split('.');
    let value: any = translations;
    
    for (const k of keys) {
      if (value && typeof value === 'object' && k in value) {
        value = value[k];
      } else {
        console.warn(`Translation key not found: ${key}`);
        return key;
      }
    }

    // Se for string, aplicar parâmetros se houver
    if (typeof value === 'string' && params) {
      return this.interpolate(value, params);
    }

    return typeof value === 'string' ? value : key;
  }

  /**
   * Interpolação de parâmetros: "Hello {{name}}" + {name: 'John'} = "Hello John"
   */
  private interpolate(text: string, params: Record<string, string | number>): string {
    return text.replace(/\{\{(\w+)\}\}/g, (_, key) => {
      return params[key]?.toString() || '';
    });
  }

  /**
   * Atalho para tradução (usar nos componentes)
   * Acessa o signal de versão para garantir reatividade
   */
  t = (key: string, params?: Record<string, string | number>): string => {
    // Acessar signals para garantir que Angular detecte mudanças
    this.translationVersion();
    this.currentLanguageSignal();
    this.translationsLoadedSignal();
    return this.translate(key, params);
  };

  /**
   * Lista todos os idiomas disponíveis
   */
  getAvailableLanguages(): Language[] {
    return ['pt-BR', 'en-US', 'es-ES'];
  }

  /**
   * Aguarda o carregamento das traduções (usado pelo APP_INITIALIZER)
   */
  async waitForTranslations(): Promise<void> {
    // Se já carregou, retorna imediatamente
    if (this.translationsLoadedSignal()) {
      return;
    }
    
    // Aguarda até as traduções serem carregadas (máximo 5 segundos)
    return new Promise((resolve) => {
      const checkInterval = setInterval(() => {
        if (this.translationsLoadedSignal()) {
          clearInterval(checkInterval);
          resolve();
        }
      }, 50);
      
      // Timeout de segurança
      setTimeout(() => {
        clearInterval(checkInterval);
        resolve();
      }, 5000);
    });
  }
}
