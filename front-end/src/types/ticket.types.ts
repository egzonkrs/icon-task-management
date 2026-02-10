export type TicketStatus = "Open" | "InProgress" | "InReview" | "Done" | "Closed";
export type TicketPriority = "Low" | "Medium" | "High" | "Critical";

export interface Ticket {
  id: string;
  title: string;
  description: string | null;
  priority: TicketPriority;
  status: TicketStatus;
  dueDate: string | null;
  sortOrder: number;
  isCompleted: boolean;
  completedAt: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface TicketsEnvelope {
  items: Ticket[];
}

export interface CreateTicketRequest {
  title: string;
  description?: string;
  priority?: string;
  dueDate?: string;
}

export interface UpdateTicketRequest {
  title: string;
  description?: string;
  priority?: string;
  dueDate?: string;
}

export interface ChangeTicketStatusRequest {
  status: string;
}

export interface ReorderTicketsRequest {
  items: ReorderTicketItem[];
}

export interface ReorderTicketItem {
  id: string;
  sortOrder: number;
}

export interface GetTicketsParams {
  isCompleted?: boolean;
  search?: string;
}
