import { z } from "zod";

const ticketPriorities = ["Low", "Medium", "High", "Critical"] as const;

export const createTicketSchema = z.object({
  title: z
    .string()
    .min(1, "Title is required")
    .max(200, "Title must not exceed 200 characters"),
  description: z
    .string()
    .max(2000, "Description must not exceed 2000 characters")
    .optional()
    .or(z.literal("")),
  priority: z.enum(ticketPriorities).optional().default("Medium"),
  dueDate: z.string().optional().or(z.literal("")),
});

export type CreateTicketFormData = z.infer<typeof createTicketSchema>;

export const updateTicketSchema = z.object({
  title: z
    .string()
    .min(1, "Title is required")
    .max(200, "Title must not exceed 200 characters"),
  description: z
    .string()
    .max(2000, "Description must not exceed 2000 characters")
    .optional()
    .or(z.literal("")),
  priority: z.enum(ticketPriorities).optional(),
  dueDate: z.string().optional().or(z.literal("")),
});

export type UpdateTicketFormData = z.infer<typeof updateTicketSchema>;
