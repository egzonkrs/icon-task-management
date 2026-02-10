import { useState, useRef } from "react";
import { z } from "zod";

interface UseFormOptions {
  initialValues: Record<string, any>;
  schema: z.ZodSchema;
  onSubmit: (values: any) => Promise<void>;
}

export function useForm({ initialValues, schema, onSubmit }: UseFormOptions) {
  const [values, setValues] = useState(initialValues);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [touched, setTouched] = useState<Record<string, boolean>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [serverError, setServerError] = useState<string | null>(null);

  const valuesRef = useRef(values);
  const initialValuesRef = useRef(initialValues);
  const schemaRef = useRef(schema);
  const onSubmitRef = useRef(onSubmit);

  valuesRef.current = values;
  initialValuesRef.current = initialValues;
  schemaRef.current = schema;
  onSubmitRef.current = onSubmit;

  function validateField(name: string, currentValues: Record<string, any>) {
    const result = schemaRef.current.safeParse(currentValues);
    if (!result.success) {
      const fieldError = result.error.issues.find((issue) => issue.path[0] === name);
      if (fieldError) {
        setErrors((prev) => ({ ...prev, [name]: fieldError.message }));
        return;
      }
    }
    setErrors((prev) => {
      const next = { ...prev };
      delete next[name];
      return next;
    });
  }

  function handleChange(name: string, value: any) {
    const newValues = { ...valuesRef.current, [name]: value };
    valuesRef.current = newValues;
    setValues(newValues);
  }

  function handleBlur(name: string) {
    setTouched((prev) => ({ ...prev, [name]: true }));
    validateField(name, valuesRef.current);
  }

  async function handleSubmit(e?: React.FormEvent) {
    if (e) e.preventDefault();
    setErrors({});
    setServerError(null);

    const result = schemaRef.current.safeParse(valuesRef.current);
    if (!result.success) {
      const newErrors: Record<string, string> = {};
      const newTouched: Record<string, boolean> = {};
      result.error.issues.forEach((issue) => {
        const field = String(issue.path[0]);
        if (!newErrors[field]) {
          newErrors[field] = issue.message;
        }
        newTouched[field] = true;
      });
      setErrors(newErrors);
      setTouched(newTouched);
      return;
    }

    setIsSubmitting(true);
    try {
      await onSubmitRef.current(result.data);
    } catch (err) {
      throw err;
    } finally {
      setIsSubmitting(false);
    }
  }

  function reset(newValues?: Record<string, any>) {
    const resetTo = newValues || initialValuesRef.current;
    valuesRef.current = resetTo;
    setValues(resetTo);
    setErrors({});
    setTouched({});
    setServerError(null);
  }

  return {
    values,
    errors,
    touched,
    isSubmitting,
    serverError,
    setValues,
    setErrors,
    setTouched,
    setServerError,
    handleChange,
    handleBlur,
    handleSubmit,
    reset,
  };
}
