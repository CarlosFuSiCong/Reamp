import { OrderStatus, TaskStatus } from "./enums";

export enum ShootTaskType {
  None = 0,
  Photography = 1,
  Video = 2,
  Floorplan = 4,
}

export interface ShootOrder {
  id: string;
  agencyId: string;
  studioId?: string;
  listingId: string;
  assignedPhotographerId?: string;
  
  title: string;
  // Display-friendly fields
  listingTitle?: string;
  listingAddress?: string;
  studioName?: string;
  agencyName?: string;
  
  currency: string;
  totalAmount: number;
  status: OrderStatus;
  createdBy: string;
  createdAtUtc: string;
  scheduledStartUtc?: string;
  scheduledEndUtc?: string;
  schedulingNotes?: string;
  cancellationReason?: string;
  tasks?: ShootTask[]; // Optional for list views
  taskCount?: number;  // Used in list views instead of tasks array
}

export interface ShootTask {
  id: string;
  type: ShootTaskType;
  status: TaskStatus;
  price?: number;
  notes?: string;
  scheduledStartUtc?: string;
  scheduledEndUtc?: string;
  assigneeUserId?: string;
}
