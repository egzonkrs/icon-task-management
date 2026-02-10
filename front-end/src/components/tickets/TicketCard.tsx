import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import {
  Calendar,
  CheckCircle2,
  GripVertical,
  Pencil,
  Trash2,
} from "lucide-react";
import { Badge } from "@/components/ui";
import type { Ticket, TicketPriority } from "@/types";

interface TicketCardProps {
  ticket: Ticket;
  onEdit: (ticket: Ticket) => void;
  onDelete: (id: string) => void;
  onComplete: (id: string) => void;
  onClick?: (ticket: Ticket) => void;
}

const priorityVariant: Record<TicketPriority, "slate" | "indigo" | "amber" | "coral"> = {
  Low: "slate",
  Medium: "indigo",
  High: "amber",
  Critical: "coral",
};

function formatDate(dateStr: string | null): string | null {
  if (!dateStr) return null;
  return new Date(dateStr).toLocaleDateString("en-GB", {
    day: "numeric",
    month: "short",
  });
}

function isOverdue(dateStr: string | null): boolean {
  if (!dateStr) return false;
  return new Date(dateStr) < new Date();
}

export function TicketCard({
  ticket,
  onEdit,
  onDelete,
  onComplete,
  onClick,
}: TicketCardProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: ticket.id });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  };

  const overdue = !ticket.isCompleted && isOverdue(ticket.dueDate);

  const handleCardClick = (e: React.MouseEvent) => {
    const target = e.target as HTMLElement;
    if (
      target.closest("button") ||
      target.closest("[data-drag-handle]")
    ) {
      return;
    }
    onClick?.(ticket);
  };

  return (
    <div
      ref={setNodeRef}
      style={style}
      className={`group border bg-white p-3.5 transition-all cursor-pointer hover:border-accent-400 ${
        isDragging
          ? "opacity-50 border-accent-400"
          : "border-slate-200"
      }`}
      onClick={handleCardClick}
    >
      <div className="flex items-start gap-2">
        <button
          data-drag-handle
          className="mt-0.5 cursor-grab touch-none text-slate-300 hover:text-slate-500 active:cursor-grabbing"
          {...attributes}
          {...listeners}
        >
          <GripVertical className="h-4 w-4" />
        </button>

        <div className="min-w-0 flex-1">
          <div className="flex items-start justify-between gap-2">
            <h4 className="text-sm font-bold text-slate-900 leading-snug line-clamp-2">
              {ticket.title}
            </h4>
            <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity shrink-0">
              {!ticket.isCompleted && (
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    onComplete(ticket.id);
                  }}
                  className="p-1 text-slate-400 hover:bg-success-50 hover:text-success-600 transition-colors cursor-pointer"
                  title="Mark as done"
                >
                  <CheckCircle2 className="h-3.5 w-3.5" />
                </button>
              )}
              <button
                onClick={(e) => {
                  e.stopPropagation();
                  onEdit(ticket);
                }}
                className="p-1 text-slate-400 hover:bg-accent-50 hover:text-accent-600 transition-colors cursor-pointer"
                title="Edit"
              >
                <Pencil className="h-3.5 w-3.5" />
              </button>
              <button
                onClick={(e) => {
                  e.stopPropagation();
                  onDelete(ticket.id);
                }}
                className="p-1 text-slate-400 hover:bg-danger-50 hover:text-danger-600 transition-colors cursor-pointer"
                title="Delete"
              >
                <Trash2 className="h-3.5 w-3.5" />
              </button>
            </div>
          </div>

          {ticket.description && (
            <p className="mt-1 text-xs font-medium text-slate-500 line-clamp-2">
              {ticket.description}
            </p>
          )}

          <div className="mt-2.5 flex items-center gap-2 flex-wrap">
            <Badge variant={priorityVariant[ticket.priority]}>
              {ticket.priority}
            </Badge>

            {ticket.dueDate && (
              <span
                className={`inline-flex items-center gap-1 text-xs font-bold ${
                  overdue ? "text-danger-600" : "text-slate-500"
                }`}
              >
                <Calendar className="h-3 w-3" />
                {formatDate(ticket.dueDate)}
              </span>
            )}

            {ticket.isCompleted && (
              <span className="inline-flex items-center gap-1 text-xs font-bold text-success-600">
                <CheckCircle2 className="h-3 w-3" />
                Done
              </span>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
