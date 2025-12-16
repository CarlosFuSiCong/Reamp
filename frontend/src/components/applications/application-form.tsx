"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { applicationsApi } from "@/lib/api";
import { toast } from "sonner";
import { ApplicationType } from "@/types";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";

const agencySchema = z.object({
  organizationName: z.string().min(2, "Name must be at least 2 characters"),
  description: z.string().optional(),
  contactEmail: z.string().email("Invalid email"),
  contactPhone: z.string().min(1, "Phone is required"),
});

const studioSchema = agencySchema.extend({
  address: z
    .object({
      street: z.string().optional(),
      city: z.string().optional(),
      state: z.string().optional(),
      postalCode: z.string().optional(),
      country: z.string().optional(),
    })
    .optional(),
});

type AgencyFormData = z.infer<typeof agencySchema>;
type StudioFormData = z.infer<typeof studioSchema>;

interface ApplicationFormProps {
  type: ApplicationType;
  userEmail: string;
  onSuccess?: () => void;
}

export function ApplicationForm({ type, userEmail, onSuccess }: ApplicationFormProps) {
  const isStudio = type === ApplicationType.Studio;
  const schema = isStudio ? studioSchema : agencySchema;
  const queryClient = useQueryClient();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<AgencyFormData | StudioFormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      contactEmail: userEmail,
    },
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
      queryClient.invalidateQueries({ queryKey: ["applications"] });
      onSuccess?.();
    },
    onError: (error: unknown) => {
      const errorMessage = error?.message || "Failed to submit application";
      const errors = error?.errors;

      // Show main error message
      toast.error(errorMessage);

      // Show additional error details if available
      if (errors && Array.isArray(errors) && errors.length > 0) {
        errors.forEach((err: string) => {
          toast.error(err);
        });
      }
    },
  });

  const onSubmit = (data: AgencyFormData | StudioFormData) => {
    submitMutation.mutate(data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <div className="space-y-2">
        <Label htmlFor="organizationName">
          Organization Name <span className="text-red-500">*</span>
        </Label>
        <Input
          id="organizationName"
          {...register("organizationName")}
          placeholder="Enter organization name"
        />
        {errors.organizationName && (
          <p className="text-sm text-red-600">{errors.organizationName.message}</p>
        )}
      </div>

      <div className="space-y-2">
        <Label htmlFor="description">Description</Label>
        <Textarea
          id="description"
          {...register("description")}
          rows={3}
          placeholder="Brief description of your organization"
        />
      </div>

      <div className="space-y-2">
        <Label htmlFor="contactEmail">
          Contact Email <span className="text-red-500">*</span>
        </Label>
        <Input
          id="contactEmail"
          type="email"
          {...register("contactEmail")}
          placeholder="contact@example.com"
          disabled
          className="bg-gray-50 cursor-not-allowed"
        />
        <p className="text-xs text-gray-500">Using your account email</p>
        {errors.contactEmail && (
          <p className="text-sm text-red-600">{errors.contactEmail.message}</p>
        )}
      </div>

      <div className="space-y-2">
        <Label htmlFor="contactPhone">
          Contact Phone <span className="text-red-500">*</span>
        </Label>
        <Input
          id="contactPhone"
          type="tel"
          {...register("contactPhone")}
          placeholder="+1 234 567 8900"
        />
        {errors.contactPhone && (
          <p className="text-sm text-red-600">{errors.contactPhone.message}</p>
        )}
      </div>

      {isStudio && (
        <div className="space-y-4 pt-4 border-t">
          <h3 className="text-lg font-medium">Address (Optional)</h3>

          <div className="space-y-2">
            <Label htmlFor="street">Street</Label>
            <Input id="street" {...register("address.street")} />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="city">City</Label>
              <Input id="city" {...register("address.city")} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="state">State</Label>
              <Input id="state" {...register("address.state")} />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="postalCode">Postal Code</Label>
              <Input id="postalCode" {...register("address.postalCode")} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="country">Country</Label>
              <Input id="country" {...register("address.country")} />
            </div>
          </div>
        </div>
      )}

      <Button type="submit" disabled={submitMutation.isPending} className="w-full">
        {submitMutation.isPending ? "Submitting..." : "Submit Application"}
      </Button>
    </form>
  );
}
