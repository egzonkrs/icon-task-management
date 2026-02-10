import { useState } from "react";
import {
  DndContext,
  DragOverlay,
  PointerSensor,
  useSensor,
  useSensors,
  closestCorners,
  type DragStartEvent,
  type DragEndEvent,
} from "@dnd-kit/core";
import { arrayMove } from "@dnd-kit/sortable";
import { TicketColumn } from "./TicketColumn";
import { TicketCard } from "./TicketCard";
import { TicketForm } from "./TicketForm";
import { TicketDetailModal } from "./TicketDetailModal";
import { useTickets } from "@/hooks/useTickets";
import type { Ticket, TicketStatus } from "@/types";

const ALL_STATUSES: TicketStatus[] = ["Open", "InProgress", "InReview", "Done", "Closed"];

export function TicketBoard() {
  const {
    tickets,
    getTicketsByStatus,
    changeStatus,
    reorderTickets,
    updateTicket,
    deleteTicket,
    completeTicket,
  } = useTickets();

  const [activeTicket, setActiveTicket] = useState<Ticket | null>(null);
  const [editingTicket, setEditingTicket] = useState<Ticket | null>(null);
  const [viewingTicket, setViewingTicket] = useState<Ticket | null>(null);
  const [showEditForm, setShowEditForm] = useState(false);
  const [showDetailModal, setShowDetailModal] = useState(false);

  // min 5px move before drag starts to avoid accidental drags
  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: { distance: 5 },
    })
  );

  function findColumnForTicket(ticketId: string): TicketStatus | null {
    const ticket = tickets.find((t) => t.id === ticketId);
    return ticket ? ticket.status : null;
  }

  function handleDragStart(event: DragStartEvent) {
    const ticket = tickets.find((t) => t.id === String(event.active.id));
    setActiveTicket(ticket ?? null);
  }

  async function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event;
    setActiveTicket(null);

    if (!over) return;

    const activeId = String(active.id);
    const overId = String(over.id);

    const isOverColumn = ALL_STATUSES.includes(overId as TicketStatus);

    const sourceColumn = findColumnForTicket(activeId);
    const targetColumn = isOverColumn
      ? (overId as TicketStatus)
      : findColumnForTicket(overId);

    if (!sourceColumn || !targetColumn) return;

    if (sourceColumn !== targetColumn) {
      await changeStatus(activeId, targetColumn);
    } else if (activeId !== overId && !isOverColumn) {
      const columnTickets = getTicketsByStatus(sourceColumn);
      const oldIndex = columnTickets.findIndex((t) => t.id === activeId);
      const newIndex = columnTickets.findIndex((t) => t.id === overId);

      if (oldIndex !== -1 && newIndex !== -1) {
        const reordered = arrayMove(columnTickets, oldIndex, newIndex);
        const items = reordered.map((t: Ticket, i: number) => ({
          id: t.id,
          sortOrder: i,
        }));
        await reorderTickets(items);
      }
    }
  }

  function handleEdit(ticket: Ticket) {
    setEditingTicket(ticket);
    setShowEditForm(true);
  }

  async function handleDelete(id: string) {
    await deleteTicket(id);
  }

  async function handleComplete(id: string) {
    await completeTicket(id);
  }

  function handleViewTicket(ticket: Ticket) {
    setViewingTicket(ticket);
    setShowDetailModal(true);
  }

  async function handleEditSubmit(data: Record<string, any>) {
    if (!editingTicket) return;
    await updateTicket(editingTicket.id, {
      title: data.title as string,
      description: (data.description as string) || undefined,
      priority: (data.priority as string) || undefined,
      dueDate: (data.dueDate as string) || undefined,
    });
  }

  return (
    <>
      <DndContext
        sensors={sensors}
        collisionDetection={closestCorners}
        onDragStart={handleDragStart}
        onDragEnd={handleDragEnd}
      >
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <TicketColumn
            status="Open"
            tickets={getTicketsByStatus("Open")}
            onEdit={handleEdit}
            onDelete={handleDelete}
            onComplete={handleComplete}
            onClick={handleViewTicket}
          />
          <TicketColumn
            status="InProgress"
            tickets={getTicketsByStatus("InProgress")}
            onEdit={handleEdit}
            onDelete={handleDelete}
            onComplete={handleComplete}
            onClick={handleViewTicket}
          />
          <TicketColumn
            status="InReview"
            tickets={getTicketsByStatus("InReview")}
            onEdit={handleEdit}
            onDelete={handleDelete}
            onComplete={handleComplete}
            onClick={handleViewTicket}
          />
          {/* done + closed share a column */}
          <div className="flex flex-col gap-3">
            <TicketColumn
              status="Done"
              tickets={getTicketsByStatus("Done")}
              onEdit={handleEdit}
              onDelete={handleDelete}
              onComplete={handleComplete}
              onClick={handleViewTicket}
              compact
            />
            <TicketColumn
              status="Closed"
              tickets={getTicketsByStatus("Closed")}
              onEdit={handleEdit}
              onDelete={handleDelete}
              onComplete={handleComplete}
              onClick={handleViewTicket}
              compact
            />
          </div>
        </div>

        {/* drag overlay preview */}
        <DragOverlay>
          {activeTicket && (
            <div className="rotate-2 scale-105">
              <TicketCard
                ticket={activeTicket}
                onEdit={() => {}}
                onDelete={() => {}}
                onComplete={() => {}}
              />
            </div>
          )}
        </DragOverlay>
      </DndContext>

      <TicketForm
        key={editingTicket?.id ?? "create"}
        isOpen={showEditForm}
        onClose={() => {
          setShowEditForm(false);
          setEditingTicket(null);
        }}
        onSubmit={handleEditSubmit}
        ticket={editingTicket}
      />

      <TicketDetailModal
        ticket={viewingTicket}
        isOpen={showDetailModal}
        onClose={() => {
          setShowDetailModal(false);
          setViewingTicket(null);
        }}
        onEdit={handleEdit}
        onDelete={handleDelete}
        onChangeStatus={changeStatus}
      />
    </>
  );
}
