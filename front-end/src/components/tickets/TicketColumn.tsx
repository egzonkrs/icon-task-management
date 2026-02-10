import { useDroppable } from "@dnd-kit/core";
import {
  SortableContext,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import { TicketCard } from "./TicketCard";
import type { Ticket, TicketStatus } from "@/types";

interface TicketColumnProps {
  status: TicketStatus;
  tickets: Ticket[];
  onEdit: (ticket: Ticket) => void;
  onDelete: (id: string) => void;
  onComplete: (id: string) => void;
  onClick?: (ticket: Ticket) => void;
  compact?: boolean;
}

const statusLabels: Record<TicketStatus, string> = {
  Open: "Open",
  InProgress: "In Progress",
  InReview: "In Review",
  Done: "Done",
  Closed: "Closed",
};

const statusColors: Record<TicketStatus, string> = {
  Open: "bg-slate-500",
  InProgress: "bg-warning-500",
  InReview: "bg-accent-500",
  Done: "bg-success-500",
  Closed: "bg-slate-700",
};

export function TicketColumn({
  status,
  tickets,
  onEdit,
  onDelete,
  onComplete,
  onClick,
  compact = false,
}: TicketColumnProps) {
  const { setNodeRef, isOver } = useDroppable({ 
    id: status,
    data: {
      type: 'column',
      status,
    },
  });

  return (
    <div
      ref={setNodeRef}
      className={`flex flex-col border bg-white/60 ${
        isOver ? "border-accent-400 bg-accent-50/40" : "border-slate-200"
      } transition-colors`}
    >
      <div className="flex items-center gap-2.5 border-b border-slate-200 px-4 py-3 pointer-events-none">
        <div className={`h-3 w-3 ${statusColors[status]}`} />
        <h3 className="text-sm font-extrabold text-slate-800 uppercase tracking-wide">
          {statusLabels[status]}
        </h3>
        <span className="ml-auto bg-slate-200 px-2.5 py-0.5 text-xs font-bold text-slate-700">
          {tickets.length}
        </span>
      </div>

      <div className={`flex-1 p-2 ${compact ? "min-h-[80px]" : "min-h-[120px]"}`}>
        {tickets.length > 0 ? (
          <SortableContext
            items={tickets.map((t) => t.id)}
            strategy={verticalListSortingStrategy}
          >
            <div className="flex flex-col gap-2">
              {tickets.map((ticket) => (
                <TicketCard
                  key={ticket.id}
                  ticket={ticket}
                  onEdit={onEdit}
                  onDelete={onDelete}
                  onComplete={onComplete}
                  onClick={onClick}
                />
              ))}
            </div>
          </SortableContext>
        ) : (
          <div className={`flex items-center justify-center h-full ${compact ? "min-h-[80px]" : "min-h-[120px]"}`}>
            <p className="text-xs font-semibold text-slate-400">
              {isOver ? "Drop here" : "No tickets"}
            </p>
          </div>
        )}
      </div>
    </div>
  );
}
