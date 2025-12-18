"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { User, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { ordersApi } from "@/lib/api/orders";
import { useStudioStaff } from "@/lib/hooks/use-studio-staff";
import { StaffSkills } from "@/types/enums";
import { toast } from "sonner";

const assignStaffSchema = z.object({
  photographerId: z.string().min(1, "Please select a photographer"),
});

type AssignStaffFormValues = z.infer<typeof assignStaffSchema>;

interface AssignStaffFormProps {
  orderId: string;
  studioId: string;
  onSuccess?: () => void;
  onCancel?: () => void;
}

export function AssignStaffForm({
  orderId,
  studioId,
  onSuccess,
  onCancel,
}: AssignStaffFormProps) {
  const queryClient = useQueryClient();
  const [selectedStaffId, setSelectedStaffId] = useState<string>("");

  // Fetch studio staff members
  const { data: staffData, isLoading: staffLoading } = useStudioStaff({
    studioId,
    pageSize: 100,
  });

  const form = useForm<AssignStaffFormValues>({
    resolver: zodResolver(assignStaffSchema),
    defaultValues: {
      photographerId: "",
    },
  });

  // Assign photographer mutation
  const assignMutation = useMutation({
    mutationFn: async (data: AssignStaffFormValues) => {
      await ordersApi.assignPhotographer(orderId, data.photographerId);
      // Schedule the order (dates already set when order was created)
      await ordersApi.scheduleOrder(orderId);
    },
    onSuccess: () => {
      toast.success("Photographer assigned successfully");
      queryClient.invalidateQueries({ queryKey: ["orders"] });
      queryClient.invalidateQueries({ queryKey: ["order", orderId] });
      onSuccess?.();
    },
    onError: (error: unknown) => {
      const err = error as { message?: string };
      toast.error(err.message || "Failed to assign photographer");
    },
  });

  const onSubmit = (data: AssignStaffFormValues) => {
    assignMutation.mutate(data);
  };

  const staffMembers = staffData?.items || [];
  const selectedStaff = staffMembers.find((s) => s.id === selectedStaffId);

  // Get skill badges from flags
  const getSkillBadges = (skills: number) => {
    const badges = [];
    if (skills & StaffSkills.Photographer) badges.push("Photography");
    if (skills & StaffSkills.Videographer) badges.push("Videography");
    if (skills & StaffSkills.VRMaker) badges.push("VR Maker");
    return badges;
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle>Assign Photographer</CardTitle>
        <CardDescription>
          Select a photographer for this photoshoot
        </CardDescription>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            {/* Photographer Selection */}
            <FormField
              control={form.control}
              name="photographerId"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Photographer</FormLabel>
                  <Select
                    disabled={staffLoading}
                    onValueChange={(value) => {
                      field.onChange(value);
                      setSelectedStaffId(value);
                    }}
                    value={field.value}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Select a photographer" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {staffLoading ? (
                        <div className="p-2 text-sm text-muted-foreground">
                          Loading staff...
                        </div>
                      ) : staffMembers.length === 0 ? (
                        <div className="p-2 text-sm text-muted-foreground">
                          No staff members found
                        </div>
                      ) : (
                        staffMembers.map((staff) => (
                          <SelectItem key={staff.id} value={staff.id}>
                            <div className="flex items-center gap-2">
                              <User className="h-4 w-4" />
                              <span>{staff.displayName}</span>
                            </div>
                          </SelectItem>
                        ))
                      )}
                    </SelectContent>
                  </Select>
                  <FormDescription>
                    Choose a photographer to assign to this order
                  </FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            {/* Selected Staff Info */}
            {selectedStaff && (
              <div className="rounded-lg border p-4 space-y-2">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="font-medium">{selectedStaff.displayName}</p>
                    <p className="text-sm text-muted-foreground">
                      {selectedStaff.firstName} {selectedStaff.lastName}
                    </p>
                  </div>
                  <Badge variant="secondary">
                    {selectedStaff.role === 1
                      ? "Staff"
                      : selectedStaff.role === 2
                      ? "Manager"
                      : "Owner"}
                  </Badge>
                </div>
                {selectedStaff.skills && selectedStaff.skills !== 0 && (
                  <div className="flex flex-wrap gap-2">
                    {getSkillBadges(selectedStaff.skills).map((skill) => (
                      <Badge key={skill} variant="outline">
                        {skill}
                      </Badge>
                    ))}
                  </div>
                )}
              </div>
            )}

            {/* Actions */}
            <div className="flex gap-3 justify-end">
              {onCancel && (
                <Button
                  type="button"
                  variant="outline"
                  onClick={onCancel}
                  disabled={assignMutation.isPending}
                >
                  Cancel
                </Button>
              )}
              <Button type="submit" disabled={assignMutation.isPending}>
                {assignMutation.isPending && (
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                )}
                Assign Photographer
              </Button>
            </div>
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}
