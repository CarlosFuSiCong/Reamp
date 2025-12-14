import { InvitationStatus } from "@/types";
import { Badge } from "@/components/ui/badge";

interface InvitationStatusBadgeProps {
  status: InvitationStatus;
  className?: string;
}

export function InvitationStatusBadge({ status, className }: InvitationStatusBadgeProps) {
  const statusConfig = {
    [InvitationStatus.Pending]: {
      label: "Pending",
      className: "bg-yellow-50 text-yellow-700 border-yellow-200"
    },
    [InvitationStatus.Accepted]: {
      label: "Accepted",
      className: "bg-green-50 text-green-700 border-green-200"
    },
    [InvitationStatus.Rejected]: {
      label: "Rejected",
      className: "bg-red-50 text-red-700 border-red-200"
    },
    [InvitationStatus.Cancelled]: {
      label: "Cancelled",
      className: "bg-gray-50 text-gray-700 border-gray-200"
    },
    [InvitationStatus.Expired]: {
      label: "Expired",
      className: "bg-gray-50 text-gray-700 border-gray-200"
    },
  };

  const config = statusConfig[status] || { label: String(status), className: "" };

  return (
    <Badge className={`${config.className} ${className || ""}`}>
      {config.label}
    </Badge>
  );
}
