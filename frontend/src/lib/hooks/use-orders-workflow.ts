import { useMemo } from "react";
import { ShootOrder, OrderStatus } from "@/types";

interface UseOrdersWorkflowReturn {
  needAssignment: ShootOrder[];
  readyToShoot: ShootOrder[];
  needUpload: ShootOrder[];
  awaitingConfirmation: ShootOrder[];
}

/**
 * Custom hook to group orders by Studio workflow steps
 */
export function useOrdersWorkflow(orders: ShootOrder[]): UseOrdersWorkflowReturn {
  return useMemo(() => {
    return {
      // Step 1: Accepted - Need to assign staff
      needAssignment: orders.filter((o) => o.status === OrderStatus.Accepted),

      // Step 2: Scheduled - Staff assigned, ready to shoot
      readyToShoot: orders.filter((o) => o.status === OrderStatus.Scheduled),

      // Step 3: InProgress & AwaitingDelivery - Shooting or waiting for upload
      needUpload: orders.filter(
        (o) =>
          o.status === OrderStatus.InProgress || o.status === OrderStatus.AwaitingDelivery
      ),

      // Step 4: AwaitingConfirmation - Uploaded, waiting for agent confirmation
      awaitingConfirmation: orders.filter((o) => o.status === OrderStatus.AwaitingConfirmation),
    };
  }, [orders]);
}
