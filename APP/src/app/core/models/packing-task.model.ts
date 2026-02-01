export interface PackingTask {
  id: string;
  taskNumber: string;
  orderId: string;
  orderNumber: string;
  status: number;
  statusName: string;
  assignedTo: string;
  assignedToName: string;
  packageCount: number;
  completedAt?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface PackingTaskListResponse {
  success: boolean;
  data: PackingTask[];
  message?: string;
}

export interface PackingTaskResponse {
  success: boolean;
  data: PackingTask;
  message?: string;
}

export interface CreatePackingTaskRequest {
  taskNumber: string;
  orderId: string;
  assignedTo: string;
}

// Status enum matching backend WMSTaskStatus
export enum PackingTaskStatus {
  Pending = 1,
  Assigned = 2,
  InProgress = 3,
  Completed = 4,
  Cancelled = 5
}
