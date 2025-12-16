"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { AgencyRole } from "@/types";
import { useSendAgencyInvitation } from "@/lib/hooks";
import { getAgencyRoleName } from "@/lib/utils";

const schema = z.object({
  inviteeEmail: z.string().email("Invalid email address"),
  targetRole: z.nativeEnum(AgencyRole),
  agencyBranchId: z.string().optional(),
});

type FormData = z.infer<typeof schema>;

interface InviteMemberDialogProps {
  agencyId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  branches?: Array<{ id: string; name: string }>;
}

export function InviteMemberDialog({
  agencyId,
  open,
  onOpenChange,
  branches = [],
}: InviteMemberDialogProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    watch,
    reset,
  } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      targetRole: AgencyRole.Agent,
    },
  });

  const sendInvitation = useSendAgencyInvitation();
  const selectedRole = watch("targetRole");

  const onSubmit = async (data: FormData) => {
    try {
      await sendInvitation.mutateAsync({
        agencyId,
        data: {
          inviteeEmail: data.inviteeEmail,
          targetRole: data.targetRole,
          agencyBranchId: data.agencyBranchId || undefined,
        },
      });
      reset();
      onOpenChange(false);
    } catch (error) {
      // Error handled by hook
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle>Invite Team Member</DialogTitle>
          <DialogDescription>Send an invitation to join your agency team</DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 py-4">
          <div className="space-y-2">
            <Label htmlFor="inviteeEmail">
              Email Address <span className="text-red-500">*</span>
            </Label>
            <Input
              id="inviteeEmail"
              type="email"
              placeholder="member@example.com"
              {...register("inviteeEmail")}
            />
            {errors.inviteeEmail && (
              <p className="text-sm text-red-600">{errors.inviteeEmail.message}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="targetRole">
              Role <span className="text-red-500">*</span>
            </Label>
            <Select
              value={selectedRole?.toString()}
              onValueChange={(value) => setValue("targetRole", Number(value) as AgencyRole)}
            >
              <SelectTrigger>
                <SelectValue placeholder="Select a role" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value={AgencyRole.Agent.toString()}>
                  {getAgencyRoleName(AgencyRole.Agent)}
                </SelectItem>
                <SelectItem value={AgencyRole.Manager.toString()}>
                  {getAgencyRoleName(AgencyRole.Manager)}
                </SelectItem>
                <SelectItem value={AgencyRole.Owner.toString()}>
                  {getAgencyRoleName(AgencyRole.Owner)}
                </SelectItem>
              </SelectContent>
            </Select>
            {errors.targetRole && (
              <p className="text-sm text-red-600">{errors.targetRole.message}</p>
            )}
            <p className="text-xs text-gray-500">
              {selectedRole === AgencyRole.Owner && "Full control over the agency"}
              {selectedRole === AgencyRole.Manager && "Can manage team and settings"}
              {selectedRole === AgencyRole.Agent && "Can manage listings and orders"}
            </p>
          </div>

          {branches.length > 0 && (
            <div className="space-y-2">
              <Label htmlFor="agencyBranchId">Branch (Optional)</Label>
              <Select onValueChange={(value) => setValue("agencyBranchId", value)}>
                <SelectTrigger>
                  <SelectValue placeholder="Select a branch" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="">No specific branch</SelectItem>
                  {branches.map((branch) => (
                    <SelectItem key={branch.id} value={branch.id}>
                      {branch.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <p className="text-xs text-gray-500">Assign the member to a specific branch</p>
            </div>
          )}

          <div className="flex justify-end gap-3 pt-4">
            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
              Cancel
            </Button>
            <Button type="submit" disabled={sendInvitation.isPending}>
              {sendInvitation.isPending ? "Sending..." : "Send Invitation"}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
