import { useMemo, useState } from "react";
import {
  ListTodo,
  FolderOpen,
  PlayCircle,
  CheckSquare,
  Plus,
  Search,
} from "lucide-react";
import { useAuth } from "@/hooks/useAuth";
import { useTickets } from "@/hooks/useTickets";
import { TicketBoard, TicketForm } from "@/components/tickets";
import { Button } from "@/components/ui";
import type { CreateTicketFormData, UpdateTicketFormData } from "@/lib/validation";

export function Dashboard() {
  const { isAuthenticated, isLoading: authLoading, openLoginModal } = useAuth();
  const { tickets, isLoading, createTicket, setFilter, filter } = useTickets();
  const [showCreateForm, setShowCreateForm] = useState(false);

  const stats = useMemo(() => {
    const total = tickets.length;
    const open = tickets.filter((t) => t.status === "Open").length;
    const inProgress = tickets.filter((t) => t.status === "InProgress").length;
    const completed = tickets.filter(
      (t) => t.status === "Done" || t.status === "Closed"
    ).length;
    return { total, open, inProgress, completed };
  }, [tickets]);

  const handleCreateSubmit = async (
    data: CreateTicketFormData | UpdateTicketFormData
  ) => {
    await createTicket({
      title: data.title,
      description: data.description || undefined,
      priority: data.priority || "Medium",
      dueDate: data.dueDate || undefined,
    });
  };

  if (authLoading) {
    return (
      <div className="flex h-[60vh] items-center justify-center">
        <div className="h-8 w-8 animate-spin border-4 border-accent-200 border-t-accent-500" />
      </div>
    );
  }

  if (!isAuthenticated) {
    return (
      <div className="flex h-[60vh] flex-col items-center justify-center gap-4 text-center">
        <div className="flex h-16 w-16 items-center justify-center bg-black">
          <ListTodo className="h-8 w-8 text-accent-500" />
        </div>
        <h1 className="text-3xl font-extrabold text-slate-900">
          Welcome to Icon
        </h1>
        <p className="max-w-md text-base font-medium text-slate-500">
          A simple task management system. Sign in to start
          organizing your work.
        </p>
        <Button onClick={openLoginModal} size="lg">
          Get Started
        </Button>
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-6">
      <div className="grid grid-cols-2 gap-0 sm:grid-cols-4 border border-slate-200">
        <StatCard
          icon={<ListTodo className="h-5 w-5" />}
          label="Total Tasks"
          value={stats.total}
          color="bg-slate-100 text-slate-700"
          hasBorder
        />
        <StatCard
          icon={<FolderOpen className="h-5 w-5" />}
          label="Open"
          value={stats.open}
          color="bg-accent-50 text-accent-600"
          hasBorder
        />
        <StatCard
          icon={<PlayCircle className="h-5 w-5" />}
          label="In Progress"
          value={stats.inProgress}
          color="bg-warning-50 text-warning-600"
          hasBorder
        />
        <StatCard
          icon={<CheckSquare className="h-5 w-5" />}
          label="Completed"
          value={stats.completed}
          color="bg-success-50 text-success-600"
        />
      </div>

      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400" />
          <input
            type="text"
            placeholder="Search tickets..."
            value={filter.search ?? ""}
            onChange={(e) =>
              setFilter({ ...filter, search: e.target.value || undefined })
            }
            className="w-full border border-slate-300 bg-white py-2.5 pl-10 pr-4 text-sm font-medium text-slate-900 placeholder:text-slate-400 focus:border-accent-500 focus:outline-none focus:ring-2 focus:ring-accent-500/20"
          />
        </div>

        <Button onClick={() => setShowCreateForm(true)}>
          <Plus className="h-4 w-4" />
          New Ticket
        </Button>
      </div>

      {isLoading ? (
        <div className="flex h-40 items-center justify-center">
          <div className="h-8 w-8 animate-spin border-4 border-accent-200 border-t-accent-500" />
        </div>
      ) : (
        <TicketBoard />
      )}

      <TicketForm
        isOpen={showCreateForm}
        onClose={() => setShowCreateForm(false)}
        onSubmit={handleCreateSubmit}
      />
    </div>
  );
}

interface StatCardProps {
  icon: React.ReactNode;
  label: string;
  value: number;
  color: string;
  hasBorder?: boolean;
}

function StatCard({ icon, label, value, color, hasBorder = false }: StatCardProps) {
  return (
    <div className={`flex items-center gap-3 bg-white p-4 ${hasBorder ? "border-r border-slate-200" : ""}`}>
      <div
        className={`flex h-10 w-10 items-center justify-center ${color}`}
      >
        {icon}
      </div>
      <div>
        <p className="text-2xl font-extrabold text-slate-900">{value}</p>
        <p className="text-xs font-bold text-slate-500 uppercase tracking-wide">{label}</p>
      </div>
    </div>
  );
}
