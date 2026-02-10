import { useEffect } from "react";
import { loginSchema } from "@/lib/validation";
import { useAuth } from "@/hooks/useAuth";
import { ApiClientError } from "@/api";
import { Button, Input, Modal } from "@/components/ui";
import { useForm } from "@/hooks/useForm";

export function LoginModal() {
  const { showLoginModal, closeAuthModals, openRegisterModal, login } =
    useAuth();

  const {
    values: formData,
    errors,
    isSubmitting,
    setErrors,
    handleChange,
    handleBlur,
    handleSubmit,
    reset,
  } = useForm({
    initialValues: {
      email: "",
      password: "",
      rememberMe: false,
    },
    schema: loginSchema,
    onSubmit: async (data) => {
      try {
        await login(data);
      } catch (err) {
        if (err instanceof ApiClientError) {
          const errorMessage = err.message.toLowerCase();
          if (
            errorMessage.includes("email") ||
            errorMessage.includes("user") ||
            errorMessage.includes("account")
          ) {
            setErrors({ email: err.message });
          } else if (
            errorMessage.includes("password") ||
            errorMessage.includes("invalid") ||
            errorMessage.includes("credentials")
          ) {
            setErrors({ password: err.message });
          } else {
            setErrors({ email: err.message });
          }
        } else {
          setErrors({ email: "An unexpected error occurred. Please try again." });
        }
      }
    },
  });

  useEffect(() => {
    if (showLoginModal) {
      reset();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [showLoginModal]);

  return (
    <Modal isOpen={showLoginModal} onClose={closeAuthModals} title="Sign In">
      <form onSubmit={handleSubmit} className="flex flex-col gap-2">
        <Input
          label="Email"
          type="email"
          placeholder="you@example.com"
          value={formData.email}
          onChange={(e) => handleChange("email", e.target.value)}
          onBlur={() => handleBlur("email")}
          error={errors.email}
          autoFocus
        />

        <Input
          label="Password"
          type="password"
          placeholder="Enter your password"
          value={formData.password}
          onChange={(e) => handleChange("password", e.target.value)}
          onBlur={() => handleBlur("password")}
          error={errors.password}
        />

        <label className="flex items-center gap-2 text-sm font-medium text-slate-600 cursor-pointer">
          <input
            type="checkbox"
            checked={formData.rememberMe}
            onChange={(e) => handleChange("rememberMe", e.target.checked)}
            className="h-4 w-4 border-slate-300 text-accent-500 focus:ring-accent-500 cursor-pointer"
          />
          Remember me
        </label>

        <Button type="submit" isLoading={isSubmitting} className="mt-2">
          Sign In
        </Button>

        <p className="text-center text-sm font-medium text-slate-500">
          Don&apos;t have an account?{" "}
          <button
            type="button"
            onClick={openRegisterModal}
            className="font-bold text-accent-600 hover:text-accent-700 cursor-pointer"
          >
            Sign Up
          </button>
        </p>
      </form>
    </Modal>
  );
}
