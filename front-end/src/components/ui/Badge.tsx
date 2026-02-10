import type { ReactNode } from "react";

type BadgeVariant =
  | "slate"
  | "indigo"
  | "amber"
  | "coral"
  | "primary"
  | "accent"
  | "success";

interface BadgeProps {
  variant?: BadgeVariant;
  children: ReactNode;
  className?: string;
}

const variantStyles: Record<BadgeVariant, string> = {
  slate: "bg-slate-100 text-slate-700 border-slate-200",
  indigo: "bg-indigo-50 text-indigo-700 border-indigo-200",
  amber: "bg-amber-50 text-amber-700 border-amber-200",
  coral: "bg-danger-50 text-danger-700 border-danger-200",
  primary: "bg-accent-100 text-accent-800 border-accent-200",
  accent: "bg-accent-100 text-accent-800 border-accent-200",
  success: "bg-success-50 text-success-700 border-success-200",
};

export function Badge({
  variant = "slate",
  children,
  className = "",
}: BadgeProps) {
  return (
    <span
      className={`inline-flex items-center border px-2 py-0.5 text-xs font-bold ${variantStyles[variant]} ${className}`}
    >
      {children}
    </span>
  );
}
