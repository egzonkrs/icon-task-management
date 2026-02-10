import { createContext, useEffect, useState, type ReactNode } from "react";
import { ticketService } from "@/api";
import { useAuth } from "@/hooks/useAuth";
import type {
  Ticket,
  TicketStatus,
  CreateTicketRequest,
  UpdateTicketRequest,
  ReorderTicketItem,
  GetTicketsParams,
} from "@/types";

interface TicketContextValue {
  tickets: Ticket[];
  isLoading: boolean;
  error: string | null;
  filter: GetTicketsParams;
  fetchTickets: () => Promise<void>;
  createTicket: (data: CreateTicketRequest) => Promise<void>;
  updateTicket: (id: string, data: UpdateTicketRequest) => Promise<void>;
  deleteTicket: (id: string) => Promise<void>;
  changeStatus: (id: string, status: TicketStatus) => Promise<void>;
  completeTicket: (id: string) => Promise<void>;
  reorderTickets: (items: ReorderTicketItem[]) => Promise<void>;
  setFilter: (filter: GetTicketsParams) => void;
  getTicketsByStatus: (status: TicketStatus) => Ticket[];
}

export const TicketContext = createContext<TicketContextValue | null>(null);

export function TicketProvider({ children }: { children: ReactNode }) {
  const { isAuthenticated } = useAuth();
  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<GetTicketsParams>({});

  async function fetchTickets() {
    setIsLoading(true);
    setError(null);
    try {
      const envelope = await ticketService.getTickets(filter);
      setTickets(envelope.items);
    } catch (err) {
      const message = err instanceof Error ? err.message : "Failed to load tickets";
      setError(message);
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    if (isAuthenticated) {
      fetchTickets();
    } else {
      setTickets([]);
      setIsLoading(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isAuthenticated, filter]);

  async function createTicket(data: CreateTicketRequest) {
    await ticketService.createTicket(data);
    await fetchTickets();
  }

  async function updateTicket(id: string, data: UpdateTicketRequest) {
    await ticketService.updateTicket(id, data);
    await fetchTickets();
  }

  async function deleteTicket(id: string) {
    await ticketService.deleteTicket(id);
    await fetchTickets();
  }

  async function changeStatus(id: string, status: TicketStatus) {
    await ticketService.changeStatus(id, { status });
    await fetchTickets();
  }

  async function completeTicket(id: string) {
    await ticketService.completeTicket(id);
    await fetchTickets();
  }

  // optimistic update for smooth drag-and-drop UX
  async function reorderTickets(items: ReorderTicketItem[]) {
    setTickets((prev) => {
      const updated = [...prev];
      for (const item of items) {
        const ticket = updated.find((t) => t.id === item.id);
        if (ticket) {
          ticket.sortOrder = item.sortOrder;
        }
      }
      return updated;
    });

    try {
      await ticketService.reorderTickets({ items });
    } catch {
      await fetchTickets();
    }
  }

  function getTicketsByStatus(status: TicketStatus): Ticket[] {
    return tickets
      .filter((t) => t.status === status)
      .sort((a, b) => a.sortOrder - b.sortOrder);
  }

  const value: TicketContextValue = {
    tickets,
    isLoading,
    error,
    filter,
    fetchTickets,
    createTicket,
    updateTicket,
    deleteTicket,
    changeStatus,
    completeTicket,
    reorderTickets,
    setFilter,
    getTicketsByStatus,
  };

  return <TicketContext.Provider value={value}>{children}</TicketContext.Provider>;
}
