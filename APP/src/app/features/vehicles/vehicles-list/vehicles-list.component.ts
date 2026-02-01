import { Component, signal, computed, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import * as L from 'leaflet';
import { VehiclesService } from '../vehicles.service';
import { Vehicle, VehicleStatus } from '@core/models/vehicle.model';
import { AuthService } from '@core/services/auth.service';
import { I18nService } from '@core/services/i18n.service';
import { VehicleCreateModalComponent } from '../vehicle-create-modal/vehicle-create-modal.component';
import { VehicleEditModalComponent } from '../vehicle-edit-modal/vehicle-edit-modal.component';
import { environment } from '@environments/environment';

declare const google: any;

@Component({
  selector: 'app-vehicles-list',
  standalone: true,
  imports: [CommonModule, VehicleCreateModalComponent, VehicleEditModalComponent],
  templateUrl: './vehicles-list.component.html',
  styleUrls: ['./vehicles-list.component.scss']
})
export class VehiclesListComponent implements OnInit, OnDestroy {
  private readonly vehiclesService = inject(VehiclesService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly sanitizer = inject(DomSanitizer);
  protected readonly i18n = inject(I18nService);
  protected readonly hasGoogleMaps = !!environment.googleMapsApiKey;

  loading = signal<boolean>(true);
  vehicles = signal<Vehicle[]>([]);
  searchTerm = signal<string>('');
  statusFilter = signal<string>('all');
  trackingFilter = signal<string>('all');
  
  // Map view state
  showMapView = signal<boolean>(false);
  selectedVehicleForMap = signal<Vehicle | null>(null);
  
  // Real-time tracking simulation
  private trackingInterval: any = null;
  simulatedLat = signal<number>(0);
  simulatedLng = signal<number>(0);
  simulatedSpeed = signal<number>(0);
  trackingHistory = signal<{lat: number, lng: number}[]>([]);
  isSimulating = signal<boolean>(false);
  private routeDirection = 0;
  private routeWaypoints: {lat: number, lng: number}[] = [];
  private currentWaypointIndex = 0;
  
  // Leaflet map
  private map: L.Map | null = null;
  private vehicleMarker: L.Marker | null = null;
  private trailPolyline: L.Polyline | null = null;

  // Google Maps
  private googleMap: any = null;
  private googleMarker: any = null;
  private googlePolyline: any = null;
  private googleMapsLoader: Promise<void> | null = null;
  
  filteredVehicles = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    const status = this.statusFilter();
    const tracking = this.trackingFilter();
    let result = this.vehicles();
    
    if (term) {
      result = result.filter(vehicle =>
        vehicle.licensePlate?.toLowerCase().includes(term) ||
        vehicle.model?.toLowerCase().includes(term) ||
        vehicle.brand?.toLowerCase().includes(term) ||
        vehicle.vehicleType?.toLowerCase().includes(term) ||
        vehicle.driverName?.toLowerCase().includes(term)
      );
    }
    
    if (status !== 'all') {
      result = result.filter(vehicle => vehicle.status === parseInt(status));
    }
    
    if (tracking === 'enabled') {
      result = result.filter(vehicle => vehicle.trackingEnabled);
    } else if (tracking === 'disabled') {
      result = result.filter(vehicle => !vehicle.trackingEnabled);
    }
    
    return result;
  });
  
  hasData = computed(() => this.vehicles().length > 0);
  showCreateModal = signal<boolean>(false);
  showEditModal = signal<boolean>(false);
  selectedVehicle = signal<Vehicle | null>(null);

  ngOnInit(): void { this.loadVehicles(); }

  async loadVehicles(): Promise<void> {
    this.loading.set(true);
    try {
      const user = this.authService.currentUser();
      const companyId = user?.companyId ?? undefined;
      const response = await this.vehiclesService.getAll(companyId);
      this.vehicles.set(response.data || []);
    } catch (err) {
      console.error('Erro ao carregar veículos:', err);
    } finally {
      this.loading.set(false);
    }
  }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm.set(input.value);
  }

  onStatusChange(event: Event): void {
    this.statusFilter.set((event.target as HTMLSelectElement).value);
  }

  onTrackingChange(event: Event): void {
    this.trackingFilter.set((event.target as HTMLSelectElement).value);
  }

  clearFilters(): void {
    this.searchTerm.set('');
    this.statusFilter.set('all');
    this.trackingFilter.set('all');
  }

  openCreateModal(): void { this.showCreateModal.set(true); }
  closeCreateModal(): void { this.showCreateModal.set(false); }
  onVehicleCreated(): void { this.loadVehicles(); }

  openVehicleDetail(vehicle: Vehicle): void {
    this.router.navigate(['/vehicles', vehicle.id]);
  }
  
  openEditModal(vehicle: Vehicle): void {
    this.selectedVehicle.set(vehicle);
    this.showEditModal.set(true);
  }
  
  closeEditModal(): void {
    this.showEditModal.set(false);
    this.selectedVehicle.set(null);
  }
  
  onVehicleUpdated(): void { this.loadVehicles(); }
  
  async deleteVehicle(vehicle: Vehicle): Promise<void> {
    if (!confirm(`Deseja realmente excluir o veículo "${vehicle.licensePlate}"?`)) return;
    try {
      await this.vehiclesService.delete(vehicle.id);
      await this.loadVehicles();
    } catch (error) {
      console.error('Erro ao excluir veículo:', error);
      alert(this.i18n.t('common.errors.deleteVehicle'));
    }
  }

  async toggleTracking(vehicle: Vehicle): Promise<void> {
    try {
      if (vehicle.trackingEnabled) {
        await this.vehiclesService.disableTracking(vehicle.id);
      } else {
        await this.vehiclesService.enableTracking(vehicle.id);
      }
      await this.loadVehicles();
    } catch (error) {
      console.error('Erro ao alterar rastreamento:', error);
      alert(this.i18n.t('common.errors.toggleTracking'));
    }
  }

  openMapView(vehicle: Vehicle): void {
    this.selectedVehicleForMap.set(vehicle);
    this.showMapView.set(true);
    
    // Initialize map after DOM is ready
    setTimeout(() => {
      if (this.hasGoogleMaps) {
        this.initializeGoogleMap(vehicle).catch(() => {
          this.initializeMap(vehicle);
        });
      } else {
        this.initializeMap(vehicle);
      }
      this.startTrackingSimulation(vehicle);
    }, 100);
  }

  closeMapView(): void {
    this.stopTrackingSimulation();
    this.destroyMap();
    this.destroyGoogleMap();
    this.showMapView.set(false);
    this.selectedVehicleForMap.set(null);
  }
  
  ngOnDestroy(): void {
    this.stopTrackingSimulation();
    this.destroyMap();
    this.destroyGoogleMap();
  }

  private loadGoogleMapsScript(): Promise<void> {
    if (this.googleMapsLoader) return this.googleMapsLoader;
    if (!environment.googleMapsApiKey) {
      return Promise.reject(new Error('Google Maps API key not configured'));
    }

    this.googleMapsLoader = new Promise<void>((resolve, reject) => {
      const existing = document.querySelector('script[data-google-maps="true"]') as HTMLScriptElement | null;
      if (existing && typeof google !== 'undefined' && google?.maps) {
        resolve();
        return;
      }

      const callbackName = '__WMSGoogleMapsInit';
      (window as any)[callbackName] = () => {
        resolve();
      };

      const script = document.createElement('script');
      script.setAttribute('data-google-maps', 'true');
      script.async = true;
      script.defer = true;
      script.onerror = () => reject(new Error('Failed to load Google Maps script'));
      script.src = `https://maps.googleapis.com/maps/api/js?key=${environment.googleMapsApiKey}&callback=${callbackName}`;
      document.head.appendChild(script);
    });

    return this.googleMapsLoader;
  }

  private async initializeGoogleMap(vehicle: Vehicle): Promise<void> {
    await this.loadGoogleMapsScript();

    const lat = vehicle.lastLatitude || -23.550520;
    const lng = vehicle.lastLongitude || -46.633308;

    this.destroyGoogleMap();

    const mapElement = document.getElementById('google-map');
    if (!mapElement) throw new Error('google-map element not found');

    this.googleMap = new google.maps.Map(mapElement, {
      center: { lat, lng },
      zoom: 16,
      disableDefaultUI: true,
      zoomControl: true,
      styles: [
        { elementType: 'geometry', stylers: [{ color: '#1d2c4d' }] },
        { elementType: 'labels.text.fill', stylers: [{ color: '#8ec3b9' }] },
        { elementType: 'labels.text.stroke', stylers: [{ color: '#1a3646' }] },
        { featureType: 'administrative.country', elementType: 'geometry.stroke', stylers: [{ color: '#4b6878' }] },
        { featureType: 'administrative.land_parcel', elementType: 'labels.text.fill', stylers: [{ color: '#64779e' }] },
        { featureType: 'administrative.province', elementType: 'geometry.stroke', stylers: [{ color: '#4b6878' }] },
        { featureType: 'landscape.man_made', elementType: 'geometry.stroke', stylers: [{ color: '#334e87' }] },
        { featureType: 'landscape.natural', elementType: 'geometry', stylers: [{ color: '#023e58' }] },
        { featureType: 'poi', elementType: 'geometry', stylers: [{ color: '#283d6a' }] },
        { featureType: 'poi', elementType: 'labels.text.fill', stylers: [{ color: '#6f9ba5' }] },
        { featureType: 'poi', elementType: 'labels.text.stroke', stylers: [{ color: '#1d2c4d' }] },
        { featureType: 'poi.park', elementType: 'geometry.fill', stylers: [{ color: '#023e58' }] },
        { featureType: 'poi.park', elementType: 'labels.text.fill', stylers: [{ color: '#3C7680' }] },
        { featureType: 'road', elementType: 'geometry', stylers: [{ color: '#304a7d' }] },
        { featureType: 'road', elementType: 'labels.text.fill', stylers: [{ color: '#98a5be' }] },
        { featureType: 'road', elementType: 'labels.text.stroke', stylers: [{ color: '#1d2c4d' }] },
        { featureType: 'road.highway', elementType: 'geometry', stylers: [{ color: '#2c6675' }] },
        { featureType: 'road.highway', elementType: 'geometry.stroke', stylers: [{ color: '#255763' }] },
        { featureType: 'road.highway', elementType: 'labels.text.fill', stylers: [{ color: '#b0d5ce' }] },
        { featureType: 'road.highway', elementType: 'labels.text.stroke', stylers: [{ color: '#023e58' }] },
        { featureType: 'transit', elementType: 'labels.text.fill', stylers: [{ color: '#98a5be' }] },
        { featureType: 'transit', elementType: 'labels.text.stroke', stylers: [{ color: '#1d2c4d' }] },
        { featureType: 'transit.line', elementType: 'geometry.fill', stylers: [{ color: '#283d6a' }] },
        { featureType: 'transit.station', elementType: 'geometry', stylers: [{ color: '#3a4762' }] },
        { featureType: 'water', elementType: 'geometry', stylers: [{ color: '#0e1626' }] },
        { featureType: 'water', elementType: 'labels.text.fill', stylers: [{ color: '#4e6d70' }] }
      ]
    });

    this.googleMarker = new google.maps.Marker({
      position: { lat, lng },
      map: this.googleMap,
      title: vehicle.licensePlate,
      optimized: true
    });

    this.googlePolyline = new google.maps.Polyline({
      path: [{ lat, lng }],
      geodesic: true,
      strokeColor: '#22c55e',
      strokeOpacity: 0.9,
      strokeWeight: 4,
      map: this.googleMap
    });
  }

  private destroyGoogleMap(): void {
    if (this.googleMarker) {
      this.googleMarker.setMap(null);
      this.googleMarker = null;
    }
    if (this.googlePolyline) {
      this.googlePolyline.setMap(null);
      this.googlePolyline = null;
    }
    this.googleMap = null;
  }
  
  private initializeMap(vehicle: Vehicle): void {
    const lat = vehicle.lastLatitude || -23.550520;
    const lng = vehicle.lastLongitude || -46.633308;
    
    // Destroy existing map if any
    this.destroyMap();
    
    // Create map
    const mapElement = document.getElementById('leaflet-map');
    if (!mapElement) return;
    
    this.map = L.map('leaflet-map').setView([lat, lng], 16);
    
    // Add OpenStreetMap tile layer (dark theme)
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
      maxZoom: 19
    }).addTo(this.map);
    
    // Custom vehicle icon
    const vehicleIcon = L.divIcon({
      className: 'vehicle-marker',
      html: `
        <div class="relative">
          <div class="absolute -inset-4 bg-green-500/30 rounded-full animate-ping"></div>
          <div class="absolute -inset-2 bg-green-500/50 rounded-full animate-pulse"></div>
          <div class="relative w-10 h-10 bg-green-500 rounded-full flex items-center justify-center shadow-lg shadow-green-500/50">
            <svg class="w-6 h-6 text-white" fill="currentColor" viewBox="0 0 24 24">
              <path d="M18.92 6.01C18.72 5.42 18.16 5 17.5 5h-11c-.66 0-1.21.42-1.42 1.01L3 12v8c0 .55.45 1 1 1h1c.55 0 1-.45 1-1v-1h12v1c0 .55.45 1 1 1h1c.55 0 1-.45 1-1v-8l-2.08-5.99zM6.5 16c-.83 0-1.5-.67-1.5-1.5S5.67 13 6.5 13s1.5.67 1.5 1.5S7.33 16 6.5 16zm11 0c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5zM5 11l1.5-4.5h11L19 11H5z"/>
            </svg>
          </div>
        </div>
      `,
      iconSize: [40, 40],
      iconAnchor: [20, 20]
    });
    
    // Add vehicle marker
    this.vehicleMarker = L.marker([lat, lng], { icon: vehicleIcon }).addTo(this.map);
    
    // Add trail polyline
    this.trailPolyline = L.polyline([], {
      color: '#22c55e',
      weight: 4,
      opacity: 0.8,
      dashArray: '10, 5'
    }).addTo(this.map);
  }
  
  private destroyMap(): void {
    if (this.map) {
      this.map.remove();
      this.map = null;
      this.vehicleMarker = null;
      this.trailPolyline = null;
    }
  }
  
  private updateMapPosition(): void {
    const lat = this.simulatedLat();
    const lng = this.simulatedLng();

    if (this.googleMap && this.googleMarker) {
      this.googleMarker.setPosition({ lat, lng });
      this.googleMap.panTo({ lat, lng });

      if (this.googlePolyline) {
        const path = this.googlePolyline.getPath();
        path.push(new google.maps.LatLng(lat, lng));
        if (path.getLength() > 30) path.removeAt(0);
      }

      return;
    }

    if (!this.map || !this.vehicleMarker) return;

    this.vehicleMarker.setLatLng([lat, lng]);
    this.map.panTo([lat, lng], { animate: true, duration: 1 });

    if (this.trailPolyline) {
      const history = this.trackingHistory();
      const latLngs = history.map(p => [p.lat, p.lng] as L.LatLngTuple);
      this.trailPolyline.setLatLngs(latLngs);
    }
  }
  
  startTrackingSimulation(vehicle: Vehicle): void {
    const baseLat = vehicle.lastLatitude || -23.550520;
    const baseLng = vehicle.lastLongitude || -46.633308;
    
    this.simulatedLat.set(baseLat);
    this.simulatedLng.set(baseLng);
    this.simulatedSpeed.set(vehicle.currentSpeed || 45);
    this.trackingHistory.set([{lat: baseLat, lng: baseLng}]);
    this.isSimulating.set(true);
    
    // Create a realistic route (simulating a delivery path)
    this.generateRealisticRoute(baseLat, baseLng);
    this.currentWaypointIndex = 0;
    
    // Simulate movement every 1.5 seconds (smoother animation)
    this.trackingInterval = setInterval(() => {
      if (!this.isSimulating()) return;
      
      // Move towards next waypoint
      if (this.currentWaypointIndex < this.routeWaypoints.length) {
        const target = this.routeWaypoints[this.currentWaypointIndex];
        const currentLat = this.simulatedLat();
        const currentLng = this.simulatedLng();
        
        // Calculate distance to target
        const dLat = target.lat - currentLat;
        const dLng = target.lng - currentLng;
        const distance = Math.sqrt(dLat * dLat + dLng * dLng);
        
        // Move step (simulating ~40-60 km/h = ~15m per 1.5s)
        const stepSize = 0.00015 + Math.random() * 0.0001;
        
        if (distance < stepSize) {
          // Reached waypoint, move to next
          this.simulatedLat.set(target.lat);
          this.simulatedLng.set(target.lng);
          this.currentWaypointIndex++;
          
          // If reached end, loop back
          if (this.currentWaypointIndex >= this.routeWaypoints.length) {
            this.currentWaypointIndex = 0;
          }
        } else {
          // Move towards target
          const ratio = stepSize / distance;
          const newLat = currentLat + dLat * ratio;
          const newLng = currentLng + dLng * ratio;
          
          this.simulatedLat.set(newLat);
          this.simulatedLng.set(newLng);
        }
        
        // Update speed (varies slightly for realism)
        const baseSpeed = 45;
        const speedVariation = Math.floor(Math.random() * 20) - 10;
        this.simulatedSpeed.set(Math.max(20, baseSpeed + speedVariation));
        
        // Keep last 30 positions for trail
        const history = this.trackingHistory();
        const newHistory = [...history, {lat: this.simulatedLat(), lng: this.simulatedLng()}].slice(-30);
        this.trackingHistory.set(newHistory);
        
        // Update map marker position
        this.updateMapPosition();
      }
    }, 1500);
  }
  
  private generateRealisticRoute(startLat: number, startLng: number): void {
    // Generate waypoints simulating a delivery route (streets pattern)
    this.routeWaypoints = [];
    let lat = startLat;
    let lng = startLng;
    
    // Create a route with turns (simulating city streets)
    const segments = [
      { dir: 'north', steps: 8 },
      { dir: 'east', steps: 5 },
      { dir: 'north', steps: 6 },
      { dir: 'west', steps: 4 },
      { dir: 'north', steps: 7 },
      { dir: 'east', steps: 10 },
      { dir: 'south', steps: 5 },
      { dir: 'east', steps: 6 },
      { dir: 'south', steps: 8 },
      { dir: 'west', steps: 12 },
      { dir: 'south', steps: 6 },
      { dir: 'west', steps: 5 },
    ];
    
    const stepSize = 0.0003; // ~30 meters per waypoint
    
    for (const segment of segments) {
      for (let i = 0; i < segment.steps; i++) {
        // Add slight variation for realism (not perfectly straight)
        const variation = (Math.random() - 0.5) * 0.00005;
        
        switch (segment.dir) {
          case 'north':
            lat += stepSize;
            lng += variation;
            break;
          case 'south':
            lat -= stepSize;
            lng += variation;
            break;
          case 'east':
            lng += stepSize;
            lat += variation;
            break;
          case 'west':
            lng -= stepSize;
            lat += variation;
            break;
        }
        
        this.routeWaypoints.push({ lat, lng });
      }
    }
  }
  
  stopTrackingSimulation(): void {
    this.isSimulating.set(false);
    if (this.trackingInterval) {
      clearInterval(this.trackingInterval);
      this.trackingInterval = null;
    }
    this.trackingHistory.set([]);
  }
  
  getTrailPath(): string {
    const history = this.trackingHistory();
    if (history.length < 2) return '';
    
    // Convert lat/lng to SVG coordinates (simple projection)
    const points = history.map((p, i) => {
      const x = 400 + (p.lng - this.simulatedLng()) * 50000;
      const y = 200 + (this.simulatedLat() - p.lat) * 50000;
      return `${i === 0 ? 'M' : 'L'} ${x} ${y}`;
    });
    
    return points.join(' ');
  }
  
  getVehicleRotation(): number {
    const history = this.trackingHistory();
    if (history.length < 2) return 0;
    
    const last = history[history.length - 1];
    const prev = history[history.length - 2];
    
    const angle = Math.atan2(last.lng - prev.lng, last.lat - prev.lat) * (180 / Math.PI);
    return angle;
  }

  getStatusClass(status: number): string {
    switch (status) {
      case VehicleStatus.Available:
        return 'bg-green-100 dark:bg-green-900/30 text-green-600 dark:text-green-400';
      case VehicleStatus.InTransit:
        return 'bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400';
      case VehicleStatus.Maintenance:
        return 'bg-yellow-100 dark:bg-yellow-900/30 text-yellow-600 dark:text-yellow-400';
      case VehicleStatus.Inactive:
        return 'bg-slate-100 dark:bg-slate-900/30 text-slate-600 dark:text-slate-400';
      default:
        return 'bg-slate-100 dark:bg-slate-900/30 text-slate-600 dark:text-slate-400';
    }
  }

  getWhatsAppLink(phone?: string): string {
    if (!phone) return '';
    const cleanPhone = phone.replace(/\D/g, '');
    return `https://wa.me/55${cleanPhone}`;
  }

  formatLastUpdate(dateStr?: string): string {
    if (!dateStr) return '-';
    const date = new Date(dateStr);
    return date.toLocaleString('pt-BR');
  }

  getGoogleMapsUrl(vehicle: Vehicle): SafeResourceUrl {
    const lat = vehicle.lastLatitude || 40.4168; // Default: Madrid
    const lng = vehicle.lastLongitude || -3.7038;
    const zoom = 16;
    
    // Use Google Maps embed API com marcador no ponto do veículo
    // Mostra mapa de satélite/rua com o marcador na posição
    const mapUrl = `https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d1500!2d${lng}!3d${lat}!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x0%3A0x0!2zM${Math.abs(lat).toFixed(4)}!5e0!3m2!1spt-BR!2sbr!4v${Date.now()}`;
    
    return this.sanitizer.bypassSecurityTrustResourceUrl(mapUrl);
  }
}
