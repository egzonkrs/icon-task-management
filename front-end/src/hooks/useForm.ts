import { useState, useCallback, useRef, useEffect } from "react";
import { z } from "zod";

interface UseFormOptions<T> {
  initialValues: T;
  schema: z.ZodSchema<T>;
  onSubmit: (values: T) => Promise<void>;
  validateOnChange?: boolean;
}

export function useForm<T extends Record<string, any>>({
  initialValues,
  schema,
  onSubmit,
  validateOnChange = true,
}: UseFormOptions<T>) {
  const [values, setValues] = useState<T>(initialValues);
  const [errors, setErrors] = useState<Partial<Record<keyof T, string>>>({});
  const [touched, setTouched] = useState<Partial<Record<keyof T, boolean>>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [serverError, setServerError] = useState<string | null>(null);

  const initialValuesRef = useRef(initialValues);
  const schemaRef = useRef(schema);
  const onSubmitRef = useRef(onSubmit);

  useEffect(() => {
    initialValuesRef.current = initialValues;
    schemaRef.current = schema;
    onSubmitRef.current = onSubmit;
  });

  const validateField = useCallback(
    (name: keyof T, currentValues: T) => {
      const result = schemaRef.current.safeParse(currentValues);
      if (!result.success) {
        const fieldError = result.error.issues.find((issue) => issue.path[0] === name);
        if (fieldError) {
          setErrors((prev) => ({ ...prev, [name]: fieldError.message }));
          return;
        }
      }
      setErrors((prev) => ({ ...prev, [name]: undefined }));
    },
    []
  );

  const handleChange = useCallback(
    (name: keyof T, value: any) => {
      setValues((prev) => {
        const newValues = { ...prev, [name]: value };
        if (validateOnChange) {
          setTouched((prevTouched) => {
            if (prevTouched[name]) {
              validateField(name, newValues);
            }
            return prevTouched;
          });
        }
        return newValues;
      });
    },
    [validateOnChange, validateField]
  );

  const handleBlur = useCallback(
    (name: keyof T) => {
      setTouched((prev) => ({ ...prev, [name]: true }));
      setValues((current) => {
        validateField(name, current);
        return current;
      });
    },
    [validateField]
  );

  const handleSubmit = async (e?: React.FormEvent) => {
    e?.preventDefault();
    setErrors({});
    setServerError(null);

    const result = schemaRef.current.safeParse(values);
    if (!result.success) {
      const newErrors: Partial<Record<keyof T, string>> = {};
      const newTouched: Partial<Record<keyof T, boolean>> = {};

      result.error.issues.forEach((issue) => {
        const field = issue.path[0] as keyof T;
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
    } catch (err: any) {
      throw err;
    } finally {
      setIsSubmitting(false);
    }
  };

  const reset = useCallback((newValues?: T) => {
    setValues(newValues || initialValuesRef.current);
    setErrors({});
    setTouched({});
    setServerError(null);
  }, []);

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
