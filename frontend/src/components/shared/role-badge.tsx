import { UserRole } from "@/types/enums";
import { Badge } from "@/components/ui/badge";

const ROLE_COLORS = {
  [UserRole.Admin]: "bg-red-500",
  [UserRole.Staff]: "bg-blue-500",
  [UserRole.Client]: "bg-green-500",
  [UserRole.User]: "bg-gray-500",
  [UserRole.None]: "bg-gray-500",
};

const ROLE_NAMES = {
  [UserRole.Admin]: "Admin",
  [UserRole.Staff]: "Staff",
  [UserRole.Client]: "Client",
  [UserRole.User]: "User",
  [UserRole.None]: "None",
};

interface RoleBadgeProps {
  role: UserRole;
  className?: string;
}

export function RoleBadge({ role, className }: RoleBadgeProps) {
  return (
    <Badge className={`${ROLE_COLORS[role]} ${className || ""}`}>
      {ROLE_NAMES[role]}
    </Badge>
  );
}




