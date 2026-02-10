import { useContext } from "react";
import { TicketContext } from "@/context/TicketContext";

export function useTickets() {
  const context = useContext(TicketContext);
  if (context === null) {
    throw new Error("useTickets must be used within a TicketProvider");
  }
  return context;
}
