"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useAuth } from "@/lib/hooks";
import { UserRole } from "@/types";
import Link from "next/link";
import { AuthLayout } from "@/components/auth/auth-layout";
import { FormField } from "@/components/auth/form-field";
import { ErrorAlert } from "@/components/auth/error-alert";
import { SubmitButton } from "@/components/auth/submit-button";

const registerSchema = z
  .object({
    email: z.string().email("Invalid email address"),
    password: z
      .string()
      .min(6, "Password must be at least 6 characters")
      .regex(/[^a-zA-Z0-9]/, "Password must contain at least one special character"),
    confirmPassword: z.string(),
    role: z.nativeEnum(UserRole),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords don't match",
    path: ["confirmPassword"],
  });

type RegisterFormData = z.infer<typeof registerSchema>;

export default function RegisterPage() {
  const { register: registerUser, isRegistering, registerError } = useAuth();
  const [errorMessage, setErrorMessage] = useState<string>("");

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterFormData>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      role: UserRole.User,
    },
    mode: "onChange",
  });

  const onSubmit = async (data: RegisterFormData) => {
    try {
      setErrorMessage("");
      await registerUser(data);
    } catch (error: unknown) {
      console.error("Registration failed:", error);

      let message = "Registration failed. Please try again.";
      const err = error as { statusCode?: number; message?: string; errors?: string[] };

      if (err.statusCode === 409) {
        message = "This email is already registered. Please use a different email or sign in.";
      } else if (err.statusCode === 400) {
        message = "Invalid input. Please check all fields.";
      } else if (err.statusCode === 500) {
        message = "Server error. Please try again later.";
      } else if (err.message) {
        message = err.message;
      } else if (err.errors && Array.isArray(err.errors)) {
        message = err.errors.join("; ");
      }

      setErrorMessage(message);
    }
  };

  return (
    <AuthLayout
      title="Create your account"
      subtitle={
        <>
          Already have an account?{" "}
          <Link href="/login" className="font-medium text-blue-600 hover:text-blue-500">
            Sign in
          </Link>
        </>
      }
    >
      <form className="mt-8 space-y-6" onSubmit={handleSubmit(onSubmit)}>
        <ErrorAlert message={errorMessage || (registerError as { message?: string })?.message} />

        <div className="space-y-4">
          <FormField
            label="Email address"
            name="email"
            type="email"
            placeholder="you@example.com"
            autoComplete="email"
            error={errors.email?.message}
            register={register("email")}
          />

          <FormField
            label="Password"
            name="password"
            type="password"
            placeholder="••••••••"
            autoComplete="new-password"
            error={errors.password?.message}
            helperText="Must be at least 6 characters and contain a special character (!@#$%^&*)"
            register={register("password")}
          />

          <FormField
            label="Confirm password"
            name="confirmPassword"
            type="password"
            placeholder="••••••••"
            autoComplete="new-password"
            error={errors.confirmPassword?.message}
            register={register("confirmPassword")}
          />
        </div>

        <SubmitButton isLoading={isRegistering} loadingText="Creating account...">
          Create account
        </SubmitButton>
      </form>
    </AuthLayout>
  );
}
