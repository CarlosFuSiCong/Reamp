import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { ShootTask, ShootTaskType } from "@/types";
import { getTaskTypeLabel } from "@/lib/utils/enum-labels";
import { Calendar, DollarSign, CheckCircle2 } from "lucide-react";
import { format } from "date-fns";

interface OrderTasksCardProps {
  tasks: ShootTask[];
  currency: string;
}

export function OrderTasksCard({ tasks, currency }: OrderTasksCardProps) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Photography Tasks</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="space-y-4">
          {tasks.map((task, index) => (
            <div
              key={task.id}
              className="flex items-start justify-between gap-4 p-4 rounded-lg border"
            >
              <div className="flex-1 space-y-2">
                <div className="flex items-center gap-2">
                  <span className="font-medium">{getTaskTypeLabel(task.type)}</span>
                  <Badge variant="outline">{task.status === 4 ? "Done" : "Pending"}</Badge>
                </div>
                {task.notes && <p className="text-sm text-muted-foreground">{task.notes}</p>}
                {task.scheduledStartUtc && (
                  <div className="flex items-center gap-2 text-sm text-muted-foreground">
                    <Calendar className="h-4 w-4" />
                    <span>{format(new Date(task.scheduledStartUtc), "PPp")}</span>
                  </div>
                )}
              </div>
              {task.price && (
                <div className="text-right">
                  <p className="font-semibold">
                    {currency} {task.price.toFixed(2)}
                  </p>
                </div>
              )}
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  );
}

interface OrderSummaryCardProps {
  totalAmount: number;
  currency: string;
  tasksCompleted: number;
  totalTasks: number;
}

export function OrderSummaryCard({
  totalAmount,
  currency,
  tasksCompleted,
  totalTasks,
}: OrderSummaryCardProps) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Order Summary</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2 text-muted-foreground">
            <DollarSign className="h-4 w-4" />
            <span>Total Amount</span>
          </div>
          <span className="text-2xl font-bold">
            {currency} {totalAmount.toFixed(2)}
          </span>
        </div>
        <div className="flex items-center justify-between pt-4 border-t">
          <div className="flex items-center gap-2 text-muted-foreground">
            <CheckCircle2 className="h-4 w-4" />
            <span>Tasks Progress</span>
          </div>
          <span className="font-semibold">
            {tasksCompleted} / {totalTasks}
          </span>
        </div>
      </CardContent>
    </Card>
  );
}

interface OrderInfoCardProps {
  title: string;
  icon?: React.ReactNode;
  children: React.ReactNode;
}

export function OrderInfoCard({ title, icon, children }: OrderInfoCardProps) {
  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          {icon}
          {title}
        </CardTitle>
      </CardHeader>
      <CardContent>{children}</CardContent>
    </Card>
  );
}
