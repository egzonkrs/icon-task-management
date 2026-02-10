import { useEffect } from "react";
import {
  Calendar,
  CheckCircle2,
  Clock,
  FileText,
  Flag,
  X,
} from "lucide-react";
import { Badge } from "@/components/ui";
import { Button } from "@/components/ui";
import type { Ticket, TicketPriority, TicketStatus } from "@/types";

interface TicketDetailModalProps {
  ticket: Ticket | null;
  isOpen: boolean;
  onClose: () => void;
  onEdit: (ticket: Ticket) => void;
  onDelete: (id: string) => void;
  onChangeStatus: (id: string, status: TicketStatus) => void;
}

const priorityVariant: Record<TicketPriority, "slate" | "indigo" | "amber" | "coral"> = {
  Low: "slate",
  Medium: "indigo",
  High: "amber",
  Critical: "coral",
};

const statusLabels: Record<TicketStatus, string> = {
  Open: "Open",
  InProgress: "In Progress",
  InReview: "In Review",
  Done: "Done",
  Closed: "Closed",
};

function formatDate(dateStr: string | null): string | null {
  if (!dateStr) return null;
  return new Date(dateStr).toLocaleDateString("en-GB", {
    day: "numeric",
    month: "short",
    year: "numeric",
  });
}

function formatDateTime(dateStr: string | null): string | null {
  if (!dateStr) return null;
  return new Date(dateStr).toLocaleString("en-GB", {
    day: "numeric",
    month: "short",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function isOverdue(dateStr: string | null): boolean {
  if (!dateStr) return false;
  return new Date(dateStr) < new Date();
}

export function TicketDetailModal({
  ticket,
  isOpen,
  onClose,
  onEdit,
  onDelete,
  onChangeStatus: _onChangeStatus,
}: TicketDetailModalProps) {
  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = "hidden";
    } else {
      document.body.style.overflow = "";
    }
    return () => {
      document.body.style.overflow = "";
    };
  }, [isOpen]);

  useEffect(() => {
    const handleEsc = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        onClose();
      }
    };
    if (isOpen) {
      window.addEventListener("keydown", handleEsc);
    }
    return () => window.removeEventListener("keydown", handleEsc);
  }, [isOpen, onClose]);

  if (!isOpen || !ticket) return null;

  const overdue = !ticket.isCompleted && isOverdue(ticket.dueDate);

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      <div
        className="fixed inset-0 bg-slate-900/60 backdrop-blur-sm transition-opacity cursor-pointer"
        onClick={onClose}
      />
      <div className="relative z-10 w-full max-w-4xl mx-4 bg-white border border-slate-200 max-h-[90vh] flex flex-col">
        <div className="flex items-start justify-between border-b border-slate-200 px-6 py-4">
          <div className="flex-1 min-w-0">
            <h2 className="text-2xl font-extrabold text-slate-900 mb-2">
              {ticket.title}
            </h2>
            <div className="flex items-center gap-3 flex-wrap">
              <Badge variant={priorityVariant[ticket.priority]}>
                <Flag className="h-3 w-3 mr-1" />
                {ticket.priority}
              </Badge>
              <Badge variant="slate">
                {statusLabels[ticket.status]}
              </Badge>
              {ticket.isCompleted && (
                <Badge variant="success">
                  <CheckCircle2 className="h-3 w-3 mr-1" />
                  Completed
                </Badge>
              )}
            </div>
          </div>
          <button
            onClick={onClose}
            className="p-2 text-slate-400 hover:bg-slate-100 hover:text-slate-600 transition-colors cursor-pointer ml-4"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        <div className="flex-1 overflow-y-auto px-6 py-5">
          <div className="mb-6">
            <div className="flex items-center gap-2 mb-2">
              <FileText className="h-4 w-4 text-slate-500" />
              <h3 className="text-sm font-bold text-slate-700 uppercase tracking-wide">
                Description
              </h3>
            </div>
            {ticket.description ? (
              <p className="text-sm font-medium text-slate-700 whitespace-pre-wrap">
                {ticket.description}
              </p>
            ) : (
              <p className="text-sm font-medium text-slate-400 italic">
                No description provided
              </p>
            )}
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
            <div>
              <div className="flex items-center gap-2 mb-2">
                <Calendar className="h-4 w-4 text-slate-500" />
                <h3 className="text-sm font-bold text-slate-700 uppercase tracking-wide">
                  Due Date
                </h3>
              </div>
              {ticket.dueDate ? (
                <p
                  className={`text-sm font-medium ${
                    overdue ? "text-danger-600" : "text-slate-700"
                  }`}
                >
                  {formatDate(ticket.dueDate)}
                  {overdue && (
                    <span className="ml-2 text-xs font-bold text-danger-600">
                      (Overdue)
                    </span>
                  )}
                </p>
              ) : (
                <p className="text-sm font-medium text-slate-400 italic">
                  No due date set
                </p>
              )}
            </div>

            <div>
              <div className="flex items-center gap-2 mb-2">
                <Clock className="h-4 w-4 text-slate-500" />
                <h3 className="text-sm font-bold text-slate-700 uppercase tracking-wide">
                  Status
                </h3>
              </div>
              <p className="text-sm font-medium text-slate-700">
                {statusLabels[ticket.status]}
              </p>
            </div>

            <div>
              <div className="flex items-center gap-2 mb-2">
                <Clock className="h-4 w-4 text-slate-500" />
                <h3 className="text-sm font-bold text-slate-700 uppercase tracking-wide">
                  Created
                </h3>
              </div>
              <p className="text-sm font-medium text-slate-700">
                {formatDateTime(ticket.createdAt)}
              </p>
            </div>

            <div>
              <div className="flex items-center gap-2 mb-2">
                <Clock className="h-4 w-4 text-slate-500" />
                <h3 className="text-sm font-bold text-slate-700 uppercase tracking-wide">
                  Last Updated
                </h3>
              </div>
              <p className="text-sm font-medium text-slate-700">
                {formatDateTime(ticket.updatedAt)}
              </p>
            </div>

            {ticket.completedAt && (
              <div>
                <div className="flex items-center gap-2 mb-2">
                  <CheckCircle2 className="h-4 w-4 text-slate-500" />
                  <h3 className="text-sm font-bold text-slate-700 uppercase tracking-wide">
                    Completed
                  </h3>
                </div>
                <p className="text-sm font-medium text-slate-700">
                  {formatDateTime(ticket.completedAt)}
                </p>
              </div>
            )}
          </div>
        </div>

        <div className="flex items-center justify-between border-t border-slate-200 px-6 py-4">
          <div className="flex items-center gap-2">
            <Button
              variant="secondary"
              onClick={() => {
                onDelete(ticket.id);
                onClose();
              }}
            >
              Delete
            </Button>
          </div>
          <div className="flex items-center gap-2">
            <Button variant="secondary" onClick={onClose}>
              Close
            </Button>
            <Button
              onClick={() => {
                onEdit(ticket);
                onClose();
              }}
            >
              Edit
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
