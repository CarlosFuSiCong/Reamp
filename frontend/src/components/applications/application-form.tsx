"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useMutation } from "@tanstack/react-query";
import { applicationsApi } from "@/lib/api";
import { toast } from "sonner";
import { ApplicationType } from "@/types";

const agencySchema = z.object({
  organizationName: z.string().min(2, "Name must be at least 2 characters"),
  description: z.string().optional(),
  contactEmail: z.string().email("Invalid email"),
  contactPhone: z.string().min(1, "Phone is required"),
});

const studioSchema = agencySchema.extend({
  address: z.object({
    street: z.string().optional(),
    city: z.string().optional(),
    state: z.string().optional(),
    postalCode: z.string().optional(),
    country: z.string().optional(),
  }).optional(),
});

type AgencyFormData = z.infer<typeof agencySchema>;
type StudioFormData = z.infer<typeof studioSchema>;

interface ApplicationFormProps {
  type: ApplicationType;
  onSuccess?: () => void;
}

export function ApplicationForm({ type, onSuccess }: ApplicationFormProps) {
  const isStudio = type === ApplicationType.Studio;
  const schema = isStudio ? studioSchema : agencySchema;

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<AgencyFormData | StudioFormData>({
    resolver: zodResolver(schema),
  });

  const submitMutation = useMutation({
    mutationFn: (data: AgencyFormData | StudioFormData) => {
      if (isStudio) {
        return applicationsApi.submitStudioApplication(data as StudioFormData);
      }
      return applicationsApi.submitAgencyApplication(data as AgencyFormData);
    },
    onSuccess: () => {
      toast.success("Application submitted successfully");
      onSuccess?.();
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to submit application");
    },
  });

  const onSubmit = (data: AgencyFormData | StudioFormData) => {
    submitMutation.mutate(data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Organization Name *
        </label>
        <input
          {...register("organizationName")}
          className="w-full px-3 py-2 border border-gray-300 rounded-md"
          placeholder="Enter organization name"
        />
        {errors.organizationName && (
          <p className="mt-1 text-sm text-red-600">{errors.organizationName.message}</p>
        )}
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Description
        </label>
        <textarea
          {...register("description")}
          className="w-full px-3 py-2 border border-gray-300 rounded-md"
          rows={3}
          placeholder="Brief description of your organization"
        />
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Contact Email *
        </label>
        <input
          {...register("contactEmail")}
          type="email"
          className="w-full px-3 py-2 border border-gray-300 rounded-md"
          placeholder="contact@example.com"
        />
        {errors.contactEmail && (
          <p className="mt-1 text-sm text-red-600">{errors.contactEmail.message}</p>
        )}
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Contact Phone *
        </label>
        <input
          {...register("contactPhone")}
          type="tel"
          className="w-full px-3 py-2 border border-gray-300 rounded-md"
          placeholder="+1 234 567 8900"
        />
        {errors.contactPhone && (
          <p className="mt-1 text-sm text-red-600">{errors.contactPhone.message}</p>
        )}
      </div>

      {isStudio && (
        <div className="space-y-4 border-t pt-4">
          <h3 className="text-lg font-medium">Address (Optional)</h3>
          
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Street</label>
            <input
              {...register("address.street")}
              className="w-full px-3 py-2 border border-gray-300 rounded-md"
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">City</label>
              <input
                {...register("address.city")}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">State</label>
              <input
                {...register("address.state")}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Postal Code</label>
              <input
                {...register("address.postalCode")}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Country</label>
              <input
                {...register("address.country")}
                className="w-full px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>
          </div>
        </div>
      )}

      <div className="flex gap-3">
        <button
          type="submit"
          disabled={submitMutation.isPending}
          className="flex-1 bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 disabled:opacity-50"
        >
          {submitMutation.isPending ? "Submitting..." : "Submit Application"}
        </button>
      </div>
    </form>
  );
}
