import { UserRole } from "@/types/enums";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";

const ROLE_COLORS: Record<UserRole, string> = {
  [UserRole.Admin]: "bg-destructive text-white border-transparent",
  [UserRole.Staff]: "bg-chart-2 text-white border-transparent",
  [UserRole.Agent]: "bg-chart-1 text-white border-transparent",
  [UserRole.User]: "bg-chart-4 text-foreground border-transparent",
  [UserRole.None]: "bg-muted text-muted-foreground border-border",
};

const ROLE_NAMES: Record<UserRole, string> = {
  [UserRole.Admin]: "Admin",
  [UserRole.Staff]: "Staff",
  [UserRole.Agent]: "Agent",
  [UserRole.User]: "User",
  [UserRole.None]: "None",
};

interface RoleBadgeProps {
  role: UserRole;
  className?: string;
}

export function RoleBadge({ role, className }: RoleBadgeProps) {
  return (
    <Badge className={cn(ROLE_COLORS[role], className)}>
      {ROLE_NAMES[role]}
    </Badge>
  );
}






