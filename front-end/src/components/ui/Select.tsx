import type { SelectHTMLAttributes } from "react";

interface SelectOption {
  label: string;
  value: string;
}

interface SelectProps extends SelectHTMLAttributes<HTMLSelectElement> {
  label?: string;
  error?: string;
  options: SelectOption[];
}

export function Select({ label, error, options, className = "", id, ...props }: SelectProps) {
  const selectId = id || label?.toLowerCase().replace(/\s+/g, "-");

  return (
    <div className="flex flex-col gap-1">
      {label && (
        <label htmlFor={selectId} className="text-sm font-bold text-slate-700">
          {label}
        </label>
      )}
      <select
        id={selectId}
        className={`border border-slate-300 bg-white px-3.5 py-2.5 text-sm font-medium text-slate-900 transition-colors cursor-pointer focus:border-accent-500 focus:outline-none focus:ring-2 focus:ring-accent-500/20 disabled:cursor-not-allowed disabled:bg-slate-50 disabled:text-slate-400 ${
          error ? "border-danger-500 focus:border-danger-500 focus:ring-danger-500/20" : ""
        } ${className}`}
        {...props}
      >
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
      <div className="h-4 min-h-4">
        {error && (
          <p className="text-xs font-medium text-danger-600 leading-tight tracking-tight" style={{ letterSpacing: '-0.01em' }}>{error}</p>
        )}
      </div>
    </div>
  );
}
