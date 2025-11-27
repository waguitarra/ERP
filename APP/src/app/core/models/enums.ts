// Enums globais do sistema - Alinhados com backend

export enum OrderType {
  Inbound = 1,
  Outbound = 2,
  Transfer = 3,
  Return = 4
}

export enum OrderStatus {
  Draft = 0,
  Pending = 1,
  Confirmed = 2,
  InProgress = 3,
  PartiallyFulfilled = 4,
  Fulfilled = 5,
  Shipped = 6,
  Delivered = 7,
  Cancelled = 8,
  OnHold = 9
}

export enum OrderSource {
  Manual = 1,
  ERP = 2,
  Ecommerce = 3,
  EDI = 4
}

export enum OrderPriority {
  Low = 0,
  Normal = 1,
  High = 2,
  Urgent = 3
}

export enum PurchaseOrderPriority {
  Low = 1,
  Normal = 2,
  High = 3,
  Urgent = 4
}

export enum VehicleStatus {
  Available = 0,
  InUse = 1,
  Maintenance = 2,
  OutOfService = 3
}

export enum LocationType {
  Pallet = 0,
  Shelf = 1,
  Floor = 2,
  Bulk = 3,
  Rack = 4,
  Bin = 5,
  DriveIn = 6,
  Cantilever = 7
}

export enum ZoneType {
  Receiving = 0,
  Storage = 1,
  Picking = 2,
  Packing = 3,
  Shipping = 4,
  Staging = 5,
  Returns = 6,
  Quarantine = 7,
  Refrigerated = 8,
  Hazmat = 9
}

export enum UserRole {
  Admin = 0,
  CompanyAdmin = 1,
  CompanyUser = 2
}

export enum AppointmentType {
  Inbound = 1,
  Outbound = 2
}

export enum AppointmentStatus {
  Scheduled = 0,
  CheckedIn = 1,
  InProgress = 2,
  Completed = 3,
  Cancelled = 4,
  NoShow = 5
}

export enum ReceiptStatus {
  Draft = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3
}

export enum PutawayStatus {
  Pending = 0,
  Assigned = 1,
  InProgress = 2,
  Completed = 3,
  Cancelled = 4
}

export enum PickingWaveStatus {
  Pending = 0,
  Released = 1,
  InProgress = 2,
  Completed = 3,
  Cancelled = 4
}

export enum PickingStrategy {
  Discrete = 0,
  Batch = 1,
  Wave = 2,
  Zone = 3
}

export enum LotStatus {
  Available = 0,
  Reserved = 1,
  Expired = 2,
  Blocked = 3
}

export enum MovementType {
  Inbound = 0,
  Outbound = 1,
  Transfer = 2,
  Adjustment = 3,
  Return = 4,
  Putaway = 5,
  Picking = 6,
  CycleCount = 7
}
