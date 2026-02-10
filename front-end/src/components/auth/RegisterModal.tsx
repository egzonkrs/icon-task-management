import { registerSchema } from "@/lib/validation";
import { useAuth } from "@/hooks/useAuth";
import { ApiClientError } from "@/api";
import { Button, Input, Modal } from "@/components/ui";
import { useForm } from "@/hooks/useForm";

export function RegisterModal() {
  const { showRegisterModal, closeAuthModals, openLoginModal, register } =
    useAuth();

  const {
    values: formData,
    errors,
    isSubmitting,
    setErrors,
    handleChange,
    handleBlur,
    handleSubmit,
  } = useForm({
    initialValues: {
      email: "",
      password: "",
      firstName: "",
      lastName: "",
    },
    schema: registerSchema,
    onSubmit: async (data) => {
      try {
        await register(data);
      } catch (err) {
        if (err instanceof ApiClientError) {
          const errorMessage = err.message.toLowerCase();
          if (errorMessage.includes("email")) {
            setErrors({ email: err.message });
          } else if (errorMessage.includes("password")) {
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

  return (
    <Modal
      isOpen={showRegisterModal}
      onClose={closeAuthModals}
      title="Create Account"
    >
      <form onSubmit={handleSubmit} className="flex flex-col gap-2">
        <div className="grid grid-cols-2 gap-3">
          <Input
            label="First Name"
            placeholder="John"
            value={formData.firstName}
            onChange={(e) => handleChange("firstName", e.target.value)}
            onBlur={() => handleBlur("firstName")}
            error={errors.firstName}
            autoFocus
          />
          <Input
            label="Last Name"
            placeholder="Doe"
            value={formData.lastName}
            onChange={(e) => handleChange("lastName", e.target.value)}
            onBlur={() => handleBlur("lastName")}
            error={errors.lastName}
          />
        </div>

        <Input
          label="Email"
          type="email"
          placeholder="you@example.com"
          value={formData.email}
          onChange={(e) => handleChange("email", e.target.value)}
          onBlur={() => handleBlur("email")}
          error={errors.email}
        />

        <Input
          label="Password"
          type="password"
          placeholder="Min. 8 characters"
          value={formData.password}
          onChange={(e) => handleChange("password", e.target.value)}
          onBlur={() => handleBlur("password")}
          error={errors.password}
        />

        <Button type="submit" isLoading={isSubmitting} className="mt-2">
          Create Account
        </Button>

        <p className="text-center text-sm font-medium text-slate-500">
          Already have an account?{" "}
          <button
            type="button"
            onClick={openLoginModal}
            className="font-bold text-accent-600 hover:text-accent-700 cursor-pointer"
          >
            Sign In
          </button>
        </p>
      </form>
    </Modal>
  );
}
