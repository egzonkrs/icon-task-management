import {
  createContext,
  useCallback,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
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

interface TicketState {
  tickets: Ticket[];
  isLoading: boolean;
  error: string | null;
  filter: GetTicketsParams;
}

interface TicketContextValue extends TicketState {
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
  const [state, setState] = useState<TicketState>({
    tickets: [],
    isLoading: false,
    error: null,
    filter: {},
  });

  const fetchTickets = useCallback(async () => {
    setState((prev) => ({ ...prev, isLoading: true, error: null }));
    try {
      const envelope = await ticketService.getTickets(state.filter);
      setState((prev) => ({
        ...prev,
        tickets: envelope.items,
        isLoading: false,
      }));
    } catch (err) {
      const message = err instanceof Error ? err.message : "Failed to load tickets";
      setState((prev) => ({ ...prev, error: message, isLoading: false }));
    }
  }, [state.filter]);

  useEffect(() => {
    if (isAuthenticated) {
      fetchTickets();
    } else {
      setState((prev) => ({ ...prev, tickets: [], isLoading: false }));
    }
  }, [isAuthenticated, fetchTickets]);

  const createTicket = useCallback(
    async (data: CreateTicketRequest) => {
      await ticketService.createTicket(data);
      await fetchTickets();
    },
    [fetchTickets]
  );

  const updateTicket = useCallback(
    async (id: string, data: UpdateTicketRequest) => {
      await ticketService.updateTicket(id, data);
      await fetchTickets();
    },
    [fetchTickets]
  );

  const deleteTicket = useCallback(
    async (id: string) => {
      await ticketService.deleteTicket(id);
      await fetchTickets();
    },
    [fetchTickets]
  );

  const changeStatus = useCallback(
    async (id: string, status: TicketStatus) => {
      await ticketService.changeStatus(id, { status });
      await fetchTickets();
    },
    [fetchTickets]
  );

  const completeTicket = useCallback(
    async (id: string) => {
      await ticketService.completeTicket(id);
      await fetchTickets();
    },
    [fetchTickets]
  );

  const reorderTickets = useCallback(
    async (items: ReorderTicketItem[]) => {
      setState((prev) => {
        const updated = [...prev.tickets];
        for (const item of items) {
          const ticket = updated.find((t) => t.id === item.id);
          if (ticket) {
            ticket.sortOrder = item.sortOrder;
          }
        }
        return { ...prev, tickets: updated };
      });

      try {
        await ticketService.reorderTickets({ items });
      } catch {
        await fetchTickets();
      }
    },
    [fetchTickets]
  );

  const setFilter = useCallback((filter: GetTicketsParams) => {
    setState((prev) => ({ ...prev, filter }));
  }, []);

  const getTicketsByStatus = useCallback(
    (status: TicketStatus) =>
      state.tickets
        .filter((t) => t.status === status)
        .sort((a, b) => a.sortOrder - b.sortOrder),
    [state.tickets]
  );

  const value = useMemo<TicketContextValue>(
    () => ({
      ...state,
      fetchTickets,
      createTicket,
      updateTicket,
      deleteTicket,
      changeStatus,
      completeTicket,
      reorderTickets,
      setFilter,
      getTicketsByStatus,
    }),
    [
      state,
      fetchTickets,
      createTicket,
      updateTicket,
      deleteTicket,
      changeStatus,
      completeTicket,
      reorderTickets,
      setFilter,
      getTicketsByStatus,
    ]
  );

  return (
    <TicketContext.Provider value={value}>{children}</TicketContext.Provider>
  );
}
