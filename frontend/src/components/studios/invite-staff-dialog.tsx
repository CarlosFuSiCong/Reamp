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
import { StudioRole } from "@/types";
import { useSendStudioInvitation } from "@/lib/hooks";

const schema = z.object({
  inviteeEmail: z.string().email("Invalid email address"),
  targetRole: z.nativeEnum(StudioRole),
});

type FormData = z.infer<typeof schema>;

interface InviteStaffDialogProps {
  studioId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function InviteStaffDialog({
  studioId,
  open,
  onOpenChange,
}: InviteStaffDialogProps) {
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
      targetRole: StudioRole.Member,
    },
  });

  const sendInvitation = useSendStudioInvitation();
  const selectedRole = watch("targetRole");

  const onSubmit = async (data: FormData) => {
    try {
      await sendInvitation.mutateAsync({
        studioId,
        data: {
          inviteeEmail: data.inviteeEmail,
          targetRole: data.targetRole,
        },
      });
      reset();
      onOpenChange(false);
    } catch (error) {
      // Error handled by hook
    }
  };

  const getRoleLabel = (role: StudioRole): string => {
    switch (role) {
      case StudioRole.Owner:
        return "Owner";
      case StudioRole.Manager:
        return "Manager";
      case StudioRole.Member:
        return "Member";
      default:
        return "Unknown";
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle>Invite Staff Member</DialogTitle>
          <DialogDescription>
            Send an invitation to join your studio team
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 py-4">
          <div className="space-y-2">
            <Label htmlFor="inviteeEmail">
              Email Address <span className="text-red-500">*</span>
            </Label>
            <Input
              id="inviteeEmail"
              type="email"
              placeholder="staff@example.com"
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
              onValueChange={(value) => setValue("targetRole", Number(value) as StudioRole)}
            >
              <SelectTrigger>
                <SelectValue placeholder="Select a role" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value={StudioRole.Member.toString()}>
                  {getRoleLabel(StudioRole.Member)}
                </SelectItem>
                <SelectItem value={StudioRole.Manager.toString()}>
                  {getRoleLabel(StudioRole.Manager)}
                </SelectItem>
                <SelectItem value={StudioRole.Owner.toString()}>
                  {getRoleLabel(StudioRole.Owner)}
                </SelectItem>
              </SelectContent>
            </Select>
            {errors.targetRole && (
              <p className="text-sm text-red-600">{errors.targetRole.message}</p>
            )}
            <p className="text-xs text-gray-500">
              {selectedRole === StudioRole.Owner && "Full control over the studio"}
              {selectedRole === StudioRole.Manager && "Can manage team and settings"}
              {selectedRole === StudioRole.Member && "Basic staff access"}
            </p>
          </div>

          <div className="flex justify-end gap-3 pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
            >
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
