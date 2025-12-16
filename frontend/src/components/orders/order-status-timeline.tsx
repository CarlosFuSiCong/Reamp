import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { OrderStatus } from "@/types";
import { CheckCircle2, Circle, XCircle } from "lucide-react";
import { cn } from "@/lib/utils";

interface OrderStatusTimelineProps {
  currentStatus: OrderStatus;
  isCancelled?: boolean;
}

const orderSteps = [
  { status: OrderStatus.Placed, label: "Order Placed" },
  { status: OrderStatus.Accepted, label: "Accepted by Studio" },
  { status: OrderStatus.Scheduled, label: "Shoot Scheduled" },
  { status: OrderStatus.InProgress, label: "Shoot In Progress" },
  { status: OrderStatus.Completed, label: "Completed" },
];

export function OrderStatusTimeline({ currentStatus, isCancelled }: OrderStatusTimelineProps) {
  let currentIndex = orderSteps.findIndex((step) => step.status === currentStatus);

  if (currentIndex === -1 && isCancelled) {
    currentIndex = 0;
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Order Status</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="space-y-4">
          {orderSteps.map((step, index) => {
            const isCompleted = index < currentIndex;
            const isCurrent = index === currentIndex;
            const isPending = index > currentIndex;

            return (
              <div key={step.status} className="flex items-start gap-4">
                <div className="flex flex-col items-center">
                  <div
                    className={cn(
                      "flex items-center justify-center w-8 h-8 rounded-full border-2",
                      isCompleted && "border-chart-4 bg-chart-4 text-white",
                      isCurrent &&
                        !isCancelled &&
                        "border-primary bg-primary text-primary-foreground",
                      isCurrent && isCancelled && "border-destructive bg-destructive text-white",
                      isPending && "border-border bg-background"
                    )}
                  >
                    {isCompleted && <CheckCircle2 className="h-5 w-5" />}
                    {isCurrent && !isCancelled && <Circle className="h-5 w-5" />}
                    {isCurrent && isCancelled && <XCircle className="h-5 w-5" />}
                    {isPending && <Circle className="h-5 w-5 text-muted-foreground" />}
                  </div>
                  {index < orderSteps.length - 1 && (
                    <div
                      className={cn("w-0.5 h-8 mt-1", isCompleted ? "bg-chart-4" : "bg-border")}
                    />
                  )}
                </div>
                <div className="flex-1 pt-1">
                  <p
                    className={cn(
                      "font-medium",
                      (isCompleted || isCurrent) && "text-foreground",
                      isPending && "text-muted-foreground"
                    )}
                  >
                    {step.label}
                  </p>
                  {isCurrent && isCancelled && (
                    <p className="text-sm text-destructive mt-1">Order Cancelled</p>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      </CardContent>
    </Card>
  );
}
