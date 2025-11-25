import { Injectable } from '@angular/core';

export interface GeoLocation {
  lat: number;
  lng: number;
}

export interface CepResponse {
  cep: string;
  logradouro: string;
  complemento: string;
  bairro: string;
  localidade: string;
  uf: string;
  ibge: string;
  gia: string;
  ddd: string;
  siafi: string;
  erro?: boolean;
}

@Injectable({ providedIn: 'root' })
export class GeocodingService {
  private readonly googleMapsApiKey = 'YOUR_GOOGLE_MAPS_API_KEY'; // TODO: Configurar no environment

  /**
   * Busca coordenadas a partir de um endereço usando Google Maps Geocoding API
   */
  async geocodeAddress(address: string): Promise<GeoLocation | null> {
    try {
      const url = `https://maps.googleapis.com/maps/api/geocode/json?address=${encodeURIComponent(address)}&key=${this.googleMapsApiKey}`;
      const response = await fetch(url);
      const data = await response.json();
      
      if (data.results && data.results.length > 0) {
        const location = data.results[0].geometry.location;
        return { lat: location.lat, lng: location.lng };
      }
      return null;
    } catch (error) {
      console.error('Erro ao buscar geolocalização:', error);
      return null;
    }
  }

  /**
   * Busca endereço completo a partir do CEP usando ViaCEP API (Brasil)
   */
  async searchByCep(cep: string): Promise<CepResponse | null> {
    try {
      const cleanCep = cep.replace(/\D/g, '');
      if (cleanCep.length !== 8) {
        throw new Error('CEP inválido');
      }

      const response = await fetch(`https://viacep.com.br/ws/${cleanCep}/json/`);
      const data: CepResponse = await response.json();
      
      if (data.erro) {
        return null;
      }
      
      return data;
    } catch (error) {
      console.error('Erro ao buscar CEP:', error);
      return null;
    }
  }

  /**
   * Gera URL do iframe Google Maps para um endereço
   */
  getMapEmbedUrl(address: string): string {
    const encodedAddress = encodeURIComponent(address);
    return `https://www.google.com/maps/embed/v1/place?key=${this.googleMapsApiKey}&q=${encodedAddress}`;
  }

  /**
   * Gera URL do iframe Google Maps para coordenadas
   */
  getMapEmbedUrlByCoords(lat: number, lng: number): string {
    return `https://www.google.com/maps/embed/v1/place?key=${this.googleMapsApiKey}&q=${lat},${lng}`;
  }
}
