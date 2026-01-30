export interface PickingTask {
  id: string;
  taskNumber: string;
  pickingWaveId: string;
  waveNumber?: string;
  orderId: string;
  orderNumber?: string;
  priority: number;
  priorityName: string;
  status: number;
  statusName: string;
  assignedTo?: string;
  assignedToName?: string;
  completedAt?: string;
  createdAt: string;
  updatedAt?: string;
  totalLines: number;
  completedLines: number;
  totalQuantityToPick: number;
  totalQuantityPicked: number;
  lines: PickingLine[];
}

export interface PickingLine {
  id: string;
  productId: string;
  productName: string;
  productSku: string;
  locationId: string;
  locationCode: string;
  lotId?: string;
  lotNumber?: string;
  serialNumber?: string;
  quantityToPick: number;
  quantityPicked: number;
  status: number;
  statusName: string;
  pickedBy?: string;
  pickedAt?: string;
}

export interface PickingTaskListResponse {
  success: boolean;
  data: PickingTask[];
  message?: string;
}

export interface PickingTaskResponse {
  success: boolean;
  data: PickingTask;
  message?: string;
}

// Priority enum
export enum TaskPriority {
  Low = 1,
  Normal = 2,
  High = 3,
  Urgent = 4
}

// Status enum
export enum PickingTaskStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3
}
