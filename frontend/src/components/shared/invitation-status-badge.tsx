import { InvitationStatus } from "@/types";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";

interface InvitationStatusBadgeProps {
  status: InvitationStatus;
  className?: string;
}

export function InvitationStatusBadge({ status, className }: InvitationStatusBadgeProps) {
  const statusConfig = {
    [InvitationStatus.Pending]: {
      label: "Pending",
      className: "bg-chart-5/10 text-chart-5 border-chart-5/20 dark:text-chart-5",
    },
    [InvitationStatus.Accepted]: {
      label: "Accepted",
      className: "bg-chart-4/10 text-chart-4 border-chart-4/20 dark:text-chart-4",
    },
    [InvitationStatus.Rejected]: {
      label: "Rejected",
      className: "bg-destructive/10 text-destructive border-destructive/20",
    },
    [InvitationStatus.Cancelled]: {
      label: "Cancelled",
      className: "bg-muted/50 text-foreground border-border",
    },
    [InvitationStatus.Expired]: {
      label: "Expired",
      className: "bg-muted/50 text-muted-foreground border-border",
    },
  };

  const config = statusConfig[status] || { label: String(status), className: "" };

  return <Badge className={cn(config.className, className)}>{config.label}</Badge>;
}
