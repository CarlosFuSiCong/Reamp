"use client";

import { useForm } from "react-hook-form";
import { useMutation, useQueryClient, useQuery } from "@tanstack/react-query";
import { adminApi, CreateAgencyDto } from "@/lib/api/admin";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { toast } from "sonner";

interface CreateAgencyFormProps {
  onSuccess: () => void;
}

export function CreateAgencyForm({ onSuccess }: CreateAgencyFormProps) {
  const queryClient = useQueryClient();
  const { register, handleSubmit, formState: { errors }, setValue, watch } = useForm<CreateAgencyDto>();

  // Register ownerUserId with validation
  register("ownerUserId", { required: "Owner user is required" });

  const { data: users } = useQuery({
    queryKey: ["admin-users"],
    queryFn: () => adminApi.getUsers(),
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateAgencyDto) => adminApi.createAgency(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin-agencies"] });
      queryClient.invalidateQueries({ queryKey: ["admin-stats"] });
      toast.success("Agency created successfully");
      onSuccess();
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to create agency");
    },
  });

  const onSubmit = (data: CreateAgencyDto) => {
    createMutation.mutate(data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div className="space-y-2">
        <Label htmlFor="name">Agency Name *</Label>
        <Input
          id="name"
          {...register("name", { required: "Agency name is required" })}
          placeholder="Enter agency name"
        />
        {errors.name && <p className="text-sm text-red-600">{errors.name.message}</p>}
      </div>

      <div className="space-y-2">
        <Label htmlFor="description">Description</Label>
        <Textarea
          id="description"
          {...register("description")}
          placeholder="Enter agency description"
          rows={3}
        />
      </div>

      <div className="space-y-2">
        <Label htmlFor="contactEmail">Contact Email *</Label>
        <Input
          id="contactEmail"
          type="email"
          {...register("contactEmail", { required: "Contact email is required" })}
          placeholder="contact@agency.com"
        />
        {errors.contactEmail && <p className="text-sm text-red-600">{errors.contactEmail.message}</p>}
      </div>

      <div className="space-y-2">
        <Label htmlFor="contactPhone">Contact Phone *</Label>
        <Input
          id="contactPhone"
          {...register("contactPhone", { required: "Contact phone is required" })}
          placeholder="+1234567890"
        />
        {errors.contactPhone && <p className="text-sm text-red-600">{errors.contactPhone.message}</p>}
      </div>

      <div className="space-y-2">
        <Label htmlFor="ownerUserId">Owner User *</Label>
        <Select
          onValueChange={(value) => setValue("ownerUserId", value, { shouldValidate: true })}
        >
          <SelectTrigger>
            <SelectValue placeholder="Select owner user" />
          </SelectTrigger>
          <SelectContent>
            {users?.map((user) => (
              <SelectItem key={user.id} value={user.id}>
                {user.displayName || user.email}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
        <p className="text-sm text-muted-foreground">
          The selected user will become the agency owner with full permissions.
        </p>
        {errors.ownerUserId && <p className="text-sm text-red-600">{errors.ownerUserId.message}</p>}
      </div>

      <div className="flex justify-end gap-2">
        <Button
          type="button"
          variant="outline"
          onClick={onSuccess}
          disabled={createMutation.isPending}
        >
          Cancel
        </Button>
        <Button type="submit" disabled={createMutation.isPending}>
          {createMutation.isPending ? "Creating..." : "Create Agency"}
        </Button>
      </div>
    </form>
  );
}
