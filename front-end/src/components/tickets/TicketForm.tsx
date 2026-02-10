import { useEffect } from "react";
import {
  createTicketSchema,
  updateTicketSchema,
  type CreateTicketFormData,
  type UpdateTicketFormData,
} from "@/lib/validation";
import { ApiClientError } from "@/api";
import { Button, Input, TextArea, Select, Modal } from "@/components/ui";
import { useForm } from "@/hooks/useForm";
import type { Ticket } from "@/types";

interface TicketFormProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: CreateTicketFormData | UpdateTicketFormData) => Promise<void>;
  ticket?: Ticket | null;
}

const priorityOptions = [
  { label: "Low", value: "Low" },
  { label: "Medium", value: "Medium" },
  { label: "High", value: "High" },
  { label: "Critical", value: "Critical" },
];

function toDateInputValue(dateStr: string | null | undefined): string {
  if (!dateStr) return "";
  const date = new Date(dateStr);
  return date.toISOString().split("T")[0];
}

function getInitialFormData(ticket?: Ticket | null): CreateTicketFormData {
  return {
    title: ticket?.title ?? "",
    description: ticket?.description ?? "",
    priority: (ticket?.priority as CreateTicketFormData["priority"]) ?? "Medium",
    dueDate: toDateInputValue(ticket?.dueDate) ?? "",
  };
}

export function TicketForm({
  isOpen,
  onClose,
  onSubmit,
  ticket,
}: TicketFormProps) {
  const isEditing = Boolean(ticket);
  const schema = isEditing ? updateTicketSchema : createTicketSchema;

  const {
    values: formData,
    errors,
    isSubmitting,
    serverError,
    setServerError,
    handleChange,
    handleBlur,
    handleSubmit,
    reset,
  } = useForm({
    initialValues: getInitialFormData(ticket),
    schema,
    onSubmit: async (data) => {
      try {
        await onSubmit(data);
        onClose();
      } catch (err) {
        if (err instanceof ApiClientError) {
          setServerError(err.message);
        } else {
          setServerError("An unexpected error occurred.");
        }
      }
    },
  });

  useEffect(() => {
    if (isOpen) {
      reset(getInitialFormData(ticket));
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isOpen, ticket]);

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={isEditing ? "Edit Ticket" : "New Ticket"}
      maxWidth="max-w-lg"
    >
      <form onSubmit={handleSubmit} className="flex flex-col gap-2">
        {serverError && (
          <div className="bg-danger-50 border border-danger-200 px-4 py-3 text-sm font-semibold text-danger-700">
            {serverError}
          </div>
        )}

        <Input
          label="Title"
          placeholder="Enter ticket title"
          value={formData.title}
          onChange={(e) => handleChange("title", e.target.value)}
          onBlur={() => handleBlur("title")}
          error={errors.title}
          autoFocus
        />

        <TextArea
          label="Description"
          placeholder="Describe the ticket (optional)"
          value={formData.description ?? ""}
          onChange={(e) => handleChange("description", e.target.value)}
          onBlur={() => handleBlur("description")}
          rows={3}
          error={errors.description}
        />

        <div className="grid grid-cols-2 gap-3">
          <Select
            label="Priority"
            options={priorityOptions}
            value={formData.priority}
            onChange={(e) => handleChange("priority", e.target.value)}
            onBlur={() => handleBlur("priority")}
            error={errors.priority}
          />

          <Input
            label="Due Date"
            type="date"
            value={formData.dueDate ?? ""}
            onChange={(e) => handleChange("dueDate", e.target.value)}
            onBlur={() => handleBlur("dueDate")}
            error={errors.dueDate}
          />
        </div>

        <div className="flex justify-end gap-3 mt-2">
          <Button variant="secondary" type="button" onClick={onClose}>
            Cancel
          </Button>
          <Button type="submit" isLoading={isSubmitting}>
            {isEditing ? "Save Changes" : "Create Ticket"}
          </Button>
        </div>
      </form>
    </Modal>
  );
}
