import { forwardRef, type TextareaHTMLAttributes } from "react";

interface TextAreaProps extends TextareaHTMLAttributes<HTMLTextAreaElement> {
  label?: string;
  error?: string;
}

export const TextArea = forwardRef<HTMLTextAreaElement, TextAreaProps>(
  ({ label, error, className = "", id, placeholder, ...props }, ref) => {
    const inputId = id || label?.toLowerCase().replace(/\s+/g, "-");
    
    return (
      <div className="flex flex-col gap-1">
        {label && (
          <label
            htmlFor={inputId}
            className="text-sm font-bold text-slate-700"
          >
            {label}
          </label>
        )}
        <textarea
          ref={ref}
          id={inputId}
          placeholder={placeholder}
          className={`border border-slate-300 bg-white px-3.5 py-2.5 text-sm font-medium text-slate-900 placeholder:text-slate-400 transition-colors focus:border-accent-500 focus:outline-none focus:ring-2 focus:ring-accent-500/20 disabled:cursor-not-allowed disabled:bg-slate-50 disabled:text-slate-400 resize-none ${
            error ? "border-danger-500 focus:border-danger-500 focus:ring-danger-500/20" : ""
          } ${className}`}
          {...props}
        />
        <div className="h-4 min-h-4">
          {error && (
            <p className="text-xs font-medium text-danger-600 leading-tight tracking-tight" style={{ letterSpacing: '-0.01em' }}>{error}</p>
          )}
        </div>
      </div>
    );
  }
);

TextArea.displayName = "TextArea";
