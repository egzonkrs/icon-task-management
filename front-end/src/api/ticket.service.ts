import apiClient from "./axios.instance";
import type {
  ApiResponse,
  Ticket,
  TicketsEnvelope,
  CreateTicketRequest,
  UpdateTicketRequest,
  ChangeTicketStatusRequest,
  ReorderTicketsRequest,
  GetTicketsParams,
} from "@/types";

const TICKETS_BASE = "/tickets";

export const ticketService = {
  async getTickets(params?: GetTicketsParams): Promise<TicketsEnvelope> {
    const response = await apiClient.get<ApiResponse<TicketsEnvelope>>(
      TICKETS_BASE,
      { params }
    );
    return response.data.data!;
  },

  async getTicketById(id: string): Promise<Ticket> {
    const response = await apiClient.get<ApiResponse<Ticket>>(
      `${TICKETS_BASE}/${id}`
    );
    return response.data.data!;
  },

  async createTicket(data: CreateTicketRequest): Promise<string> {
    const response = await apiClient.post<ApiResponse<string>>(
      TICKETS_BASE,
      data
    );
    return response.data.data!;
  },

  async updateTicket(id: string, data: UpdateTicketRequest): Promise<boolean> {
    const response = await apiClient.put<ApiResponse<boolean>>(
      `${TICKETS_BASE}/${id}`,
      data
    );
    return response.data.data!;
  },

  async changeStatus(
    id: string,
    data: ChangeTicketStatusRequest
  ): Promise<boolean> {
    const response = await apiClient.patch<ApiResponse<boolean>>(
      `${TICKETS_BASE}/${id}/status`,
      data
    );
    return response.data.data!;
  },

  async completeTicket(id: string): Promise<boolean> {
    const response = await apiClient.post<ApiResponse<boolean>>(
      `${TICKETS_BASE}/${id}/complete`
    );
    return response.data.data!;
  },

  async deleteTicket(id: string): Promise<boolean> {
    const response = await apiClient.delete<ApiResponse<boolean>>(
      `${TICKETS_BASE}/${id}`
    );
    return response.data.data!;
  },

  async reorderTickets(data: ReorderTicketsRequest): Promise<boolean> {
    const response = await apiClient.post<ApiResponse<boolean>>(
      `${TICKETS_BASE}/reorder`,
      data
    );
    return response.data.data!;
  },
};
