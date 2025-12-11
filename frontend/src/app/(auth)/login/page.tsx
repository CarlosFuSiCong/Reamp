"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useAuth } from "@/lib/hooks";
import Link from "next/link";
import { AuthLayout } from "@/components/auth/auth-layout";
import { FormField } from "@/components/auth/form-field";
import { ErrorAlert } from "@/components/auth/error-alert";
import { SubmitButton } from "@/components/auth/submit-button";

const loginSchema = z.object({
  email: z.string().email("Invalid email address"),
  password: z.string().min(6, "Password must be at least 6 characters"),
  rememberMe: z.boolean().optional(),
});

type LoginFormData = z.infer<typeof loginSchema>;

export default function LoginPage() {
  const { login, isLoggingIn, loginError } = useAuth();
  const [errorMessage, setErrorMessage] = useState<string>("");

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    try {
      setErrorMessage("");
      console.log("Attempting login with:", data.email);
      await login(data);
      console.log("Login successful");
    } catch (error: any) {
      console.error("Login error:", error);
      const message = error?.message || "Login failed. Please try again.";
      const errors = error?.errors;
      
      if (errors?.length) {
        setErrorMessage(`${message}: ${errors.join(", ")}`);
      } else {
        setErrorMessage(message);
      }
    }
  };

  return (
    <AuthLayout
      title="Sign in to Reamp"
      subtitle={
        <>
          Or{" "}
          <Link href="/register" className="font-medium text-blue-600 hover:text-blue-500">
            create a new account
          </Link>
        </>
      }
    >
      <form className="mt-8 space-y-6" onSubmit={handleSubmit(onSubmit)}>
        <ErrorAlert message={errorMessage || loginError?.message} />

        <div className="space-y-4 rounded-md shadow-sm">
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
            autoComplete="current-password"
            error={errors.password?.message}
            register={register("password")}
          />
        </div>

        <div className="flex items-center justify-between">
          <div className="flex items-center">
            <input
              {...register("rememberMe")}
              type="checkbox"
              className="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500"
            />
            <label htmlFor="rememberMe" className="ml-2 block text-sm text-gray-900">
              Remember me
            </label>
          </div>

          <div className="text-sm">
            <Link
              href="/forgot-password"
              className="font-medium text-blue-600 hover:text-blue-500"
            >
              Forgot password?
            </Link>
          </div>
        </div>

        <SubmitButton isLoading={isLoggingIn} loadingText="Signing in...">
          Sign in
        </SubmitButton>
      </form>
    </AuthLayout>
  );
}
