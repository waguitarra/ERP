import { Component, signal, inject, output, input, viewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { OrdersService } from '../orders.service';
import { Order } from '@core/models/order.model';
import { OrderStatus, OrderPriority } from '@core/models/enums';
import { I18nService } from '@core/services/i18n.service';
import { Vehicle } from '@core/services/vehicles.service';
import { Driver } from '@core/services/drivers.service';
import { Warehouse } from '@core/services/warehouses.service';
import { Customer } from '@core/models/customer.model';
import { VehicleSelectorModalComponent } from '@shared/components/vehicle-selector-modal/vehicle-selector-modal.component';
import { DriverSelectorModalComponent } from '@shared/components/driver-selector-modal/driver-selector-modal.component';
import { WarehouseSelectorModalComponent } from '@shared/components/warehouse-selector-modal/warehouse-selector-modal.component';
import { CustomerSelectorModalComponent } from '@shared/components/customer-selector-modal/customer-selector-modal.component';

@Component({
  selector: 'app-order-edit-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, VehicleSelectorModalComponent, DriverSelectorModalComponent, WarehouseSelectorModalComponent, CustomerSelectorModalComponent],
  templateUrl: './order-edit-modal.component.html',
  styleUrls: ['./order-edit-modal.component.scss']
})
export class OrderEditModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly ordersService = inject(OrdersService);
  protected readonly i18n = inject(I18nService);
  vehicleModal = viewChild<VehicleSelectorModalComponent>('vehicleModal');
  driverModal = viewChild<DriverSelectorModalComponent>('driverModal');
  originWarehouseModal = viewChild<WarehouseSelectorModalComponent>('originModal');
  destinationWarehouseModal = viewChild<WarehouseSelectorModalComponent>('destModal');
  customerModal = viewChild<CustomerSelectorModalComponent>('customerModal');

  order = input.required<Order>();
  isOpen = signal<boolean>(false);
  loading = signal<boolean>(false);
  
  
  selectedVehicle = signal<Vehicle | null>(null);
  selectedDriver = signal<Driver | null>(null);
  selectedOriginWarehouse = signal<Warehouse | null>(null);
  selectedDestinationWarehouse = signal<Warehouse | null>(null);
  selectedCustomer = signal<Customer | null>(null);
  
  destinationType = signal<'warehouse' | 'customer'>('customer');
  
  orderUpdated = output<void>();
  
  OrderStatus = OrderStatus;
  OrderPriority = OrderPriority;

  form: FormGroup = this.fb.group({
    status: [OrderStatus.Draft],
    priority: [OrderPriority.Normal],
    expectedDate: [''],
    shippingAddress: [''],
    specialInstructions: [''],
    vehicleId: [''],
    driverId: [''],
    originWarehouseId: [''],
    destinationWarehouseId: [''],
    customerId: [''],
    shippingZipCode: [''],
    shippingCity: [''],
    shippingState: [''],
    shippingCountry: ['España'],
    trackingNumber: [''],
    estimatedDeliveryDate: ['']
  });

  open(): void {
    const ord = this.order();
    this.form.patchValue({
      status: ord.status,
      priority: ord.priority,
      expectedDate: ord.expectedDate ? new Date(ord.expectedDate).toISOString().split('T')[0] : '',
      shippingAddress: ord.shippingAddress || '',
      specialInstructions: ord.specialInstructions || ''
    });
    this.isOpen.set(true);
  }

  openVehicleSelector(): void {
    this.vehicleModal()?.open();
  }

  openDriverSelector(): void {
    this.driverModal()?.open();
  }

  openOriginWarehouseSelector(): void {
    this.originWarehouseModal()?.open();
  }

  openDestinationWarehouseSelector(): void {
    this.destinationWarehouseModal()?.open();
  }

  openCustomerSelector(): void {
    this.customerModal()?.open();
  }

  setDestinationType(type: 'warehouse' | 'customer'): void {
    this.destinationType.set(type);
    // Limpar seleção anterior
    if (type === 'customer') {
      this.selectedDestinationWarehouse.set(null);
      this.form.patchValue({ destinationWarehouseId: null });
    } else {
      this.selectedCustomer.set(null);
      this.form.patchValue({ customerId: null });
    }
  }

  onVehicleSelected(vehicle: Vehicle): void {
    this.form.patchValue({ vehicleId: vehicle.id });
    this.selectedVehicle.set(vehicle);
  }

  onDriverSelected(driver: Driver): void {
    this.form.patchValue({ driverId: driver.id });
    this.selectedDriver.set(driver);
  }

  onOriginWarehouseSelected(warehouse: Warehouse): void {
    this.form.patchValue({ originWarehouseId: warehouse.id });
    this.selectedOriginWarehouse.set(warehouse);
  }

  onDestinationWarehouseSelected(warehouse: Warehouse): void {
    this.form.patchValue({ destinationWarehouseId: warehouse.id });
    this.selectedDestinationWarehouse.set(warehouse);
  }

  onCustomerSelected(customer: Customer): void {
    this.form.patchValue({ customerId: customer.id });
    this.selectedCustomer.set(customer);
  }

  close(): void {
    this.isOpen.set(false);
    this.form.reset();
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    try {
      const formValue = this.form.value;
      const payload = {
        status: parseInt(formValue.status),
        priority: parseInt(formValue.priority),
        expectedDate: formValue.expectedDate ? new Date(formValue.expectedDate).toISOString() : undefined,
        shippingAddress: formValue.shippingAddress || undefined,
        specialInstructions: formValue.specialInstructions || undefined,
        // WMS Fields
        vehicleId: formValue.vehicleId || undefined,
        driverId: formValue.driverId || undefined,
        originWarehouseId: formValue.originWarehouseId || undefined,
        destinationWarehouseId: formValue.destinationWarehouseId || undefined,
        shippingZipCode: formValue.shippingZipCode || undefined,
        shippingCity: formValue.shippingCity || undefined,
        shippingState: formValue.shippingState || undefined,
        shippingCountry: formValue.shippingCountry || undefined,
        trackingNumber: formValue.trackingNumber || undefined,
        estimatedDeliveryDate: formValue.estimatedDeliveryDate ? new Date(formValue.estimatedDeliveryDate).toISOString() : undefined
      };

      await this.ordersService.update(this.order().id, payload);
      this.orderUpdated.emit();
      this.close();
    } catch (error) {
      console.error('Erro ao atualizar pedido:', error);
      alert('Erro ao atualizar pedido');
    } finally {
      this.loading.set(false);
    }
  }
}
