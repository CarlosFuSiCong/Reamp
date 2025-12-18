"use client";

import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import {
  CheckCircle,
  XCircle,
  UserPlus,
  Play,
  Package,
  Loader2,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { ConfirmDialog } from "@/components/shared";
import { ordersApi } from "@/lib/api";
import { OrderStatus } from "@/types";

interface OrderActionsProps {
  orderId: string;
  currentStatus: OrderStatus;
  isAgent: boolean;
  isStudio: boolean;
  onSuccess?: () => void;
}

export function OrderActions({
  orderId,
  currentStatus,
  isAgent,
  isStudio,
  onSuccess,
}: OrderActionsProps) {
  const queryClient = useQueryClient();
  const [confirmDialog, setConfirmDialog] = useState<{
    open: boolean;
    action?: "accept" | "cancel" | "schedule" | "start" | "confirm";
  }>({ open: false });

  // Accept order mutation (Studio grabs order from marketplace)
  const acceptMutation = useMutation({
    mutationFn: () => ordersApi.acceptOrder(orderId),
    onSuccess: () => {
      toast.success("Order accepted successfully");
      queryClient.invalidateQueries({ queryKey: ["order", orderId] });
      queryClient.invalidateQueries({ queryKey: ["orders"] });
      onSuccess?.();
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to accept order");
    },
  });

  // Cancel order mutation (Agent only, before InProgress)
  const cancelMutation = useMutation({
    mutationFn: () => ordersApi.cancelOrder(orderId),
    onSuccess: () => {
      toast.success("Order cancelled");
      queryClient.invalidateQueries({ queryKey: ["order", orderId] });
      queryClient.invalidateQueries({ queryKey: ["orders"] });
      onSuccess?.();
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to cancel order");
    },
  });

  // Schedule order mutation (Studio assigns staff)
  const scheduleMutation = useMutation({
    mutationFn: () => ordersApi.scheduleOrder(orderId),
    onSuccess: () => {
      toast.success("Order scheduled, staff assigned");
      queryClient.invalidateQueries({ queryKey: ["order", orderId] });
      onSuccess?.();
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to schedule order");
    },
  });

  // Start shooting mutation (Studio starts work, locks order)
  const startShootingMutation = useMutation({
    mutationFn: () => ordersApi.startShooting(orderId),
    onSuccess: () => {
      toast.success("Shooting started, order is now locked");
      queryClient.invalidateQueries({ queryKey: ["order", orderId] });
      onSuccess?.();
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to start shooting");
    },
  });

  // Confirm delivery mutation (Agent confirms receipt)
  const confirmDeliveryMutation = useMutation({
    mutationFn: () => ordersApi.confirmDelivery(orderId),
    onSuccess: () => {
      toast.success("Delivery confirmed, order completed");
      queryClient.invalidateQueries({ queryKey: ["order", orderId] });
      queryClient.invalidateQueries({ queryKey: ["orders"] });
      onSuccess?.();
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to confirm delivery");
    },
  });

  const handleConfirm = (action: typeof confirmDialog.action) => {
    setConfirmDialog({ open: false });

    switch (action) {
      case "accept":
        acceptMutation.mutate();
        break;
      case "cancel":
        cancelMutation.mutate();
        break;
      case "schedule":
        scheduleMutation.mutate();
        break;
      case "start":
        startShootingMutation.mutate();
        break;
      case "confirm":
        confirmDeliveryMutation.mutate();
        break;
    }
  };

  const isLoading =
    acceptMutation.isPending ||
    cancelMutation.isPending ||
    scheduleMutation.isPending ||
    startShootingMutation.isPending ||
    confirmDeliveryMutation.isPending;

  // Determine available actions based on status and role
  const canAgentCancel =
    isAgent &&
    (currentStatus === OrderStatus.Placed ||
      currentStatus === OrderStatus.Accepted ||
      currentStatus === OrderStatus.Scheduled);

  const canStudioAccept = isStudio && currentStatus === OrderStatus.Placed;
  const canStudioSchedule = isStudio && currentStatus === OrderStatus.Accepted;
  const canStudioStart = isStudio && currentStatus === OrderStatus.Scheduled;
  const canAgentConfirm = isAgent && currentStatus === OrderStatus.AwaitingConfirmation;

  return (
    <>
      <div className="flex items-center gap-2">
        {/* Studio: Accept Order */}
        {canStudioAccept && (
          <Button
            onClick={() => setConfirmDialog({ open: true, action: "accept" })}
            disabled={isLoading}
          >
            {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            <CheckCircle className="mr-2 h-4 w-4" />
            Accept Order
          </Button>
        )}

        {/* Studio: Schedule Order (Assign Staff) */}
        {canStudioSchedule && (
          <Button
            onClick={() => setConfirmDialog({ open: true, action: "schedule" })}
            disabled={isLoading}
          >
            {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            <UserPlus className="mr-2 h-4 w-4" />
            Assign Staff & Schedule
          </Button>
        )}

        {/* Studio: Start Shooting */}
        {canStudioStart && (
          <Button
            onClick={() => setConfirmDialog({ open: true, action: "start" })}
            disabled={isLoading}
          >
            {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            <Play className="mr-2 h-4 w-4" />
            Start Shooting
          </Button>
        )}

        {/* Agent: Confirm Delivery */}
        {canAgentConfirm && (
          <Button
            onClick={() => setConfirmDialog({ open: true, action: "confirm" })}
            disabled={isLoading}
          >
            {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            <Package className="mr-2 h-4 w-4" />
            Confirm Delivery
          </Button>
        )}

        {/* Agent: Cancel Order */}
        {canAgentCancel && (
          <Button
            variant="destructive"
            onClick={() => setConfirmDialog({ open: true, action: "cancel" })}
            disabled={isLoading}
          >
            {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            <XCircle className="mr-2 h-4 w-4" />
            Cancel Order
          </Button>
        )}
      </div>

      {/* Confirm Dialogs */}
      <ConfirmDialog
        open={confirmDialog.open && confirmDialog.action === "accept"}
        onOpenChange={(open) => setConfirmDialog({ ...confirmDialog, open })}
        title="Accept Order"
        description="Are you sure you want to accept this order? You will be responsible for completing the shoot."
        onConfirm={() => handleConfirm("accept")}
        confirmText="Accept Order"
      />

      <ConfirmDialog
        open={confirmDialog.open && confirmDialog.action === "cancel"}
        onOpenChange={(open) => setConfirmDialog({ ...confirmDialog, open })}
        title="Cancel Order"
        description="Are you sure you want to cancel this order? This action cannot be undone."
        onConfirm={() => handleConfirm("cancel")}
        confirmText="Cancel Order"
        variant="destructive"
      />

      <ConfirmDialog
        open={confirmDialog.open && confirmDialog.action === "schedule"}
        onOpenChange={(open) => setConfirmDialog({ ...confirmDialog, open })}
        title="Schedule Order"
        description="Assign staff and confirm the shooting schedule. The agent can still cancel before shooting starts."
        onConfirm={() => handleConfirm("schedule")}
        confirmText="Schedule Order"
      />

      <ConfirmDialog
        open={confirmDialog.open && confirmDialog.action === "start"}
        onOpenChange={(open) => setConfirmDialog({ ...confirmDialog, open })}
        title="Start Shooting"
        description="Starting the shoot will lock the order. The agent will no longer be able to cancel."
        onConfirm={() => handleConfirm("start")}
        confirmText="Start Shooting"
      />

      <ConfirmDialog
        open={confirmDialog.open && confirmDialog.action === "confirm"}
        onOpenChange={(open) => setConfirmDialog({ ...confirmDialog, open })}
        title="Confirm Delivery"
        description="Confirm that you have received the delivery. This will complete the order."
        onConfirm={() => handleConfirm("confirm")}
        confirmText="Confirm Delivery"
      />
    </>
  );
}
