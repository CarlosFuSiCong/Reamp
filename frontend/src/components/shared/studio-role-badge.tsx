import { StudioRole } from "@/types";
import { Badge } from "@/components/ui/badge";
import { getStudioRoleName } from "@/lib/utils/role-utils";
import { cn } from "@/lib/utils";

interface StudioRoleBadgeProps {
  role: StudioRole | undefined | null;
  className?: string;
}

export function StudioRoleBadge({ role, className }: StudioRoleBadgeProps) {
  if (role === undefined || role === null) {
    return (
      <Badge className={cn("bg-muted/50 text-foreground border-border", className)}>
        Staff
      </Badge>
    );
  }
  
  const roleValue = typeof role === 'string' ? parseInt(role, 10) : Number(role);
  const roleName = getStudioRoleName(role);
  
  const colorClasses: Record<StudioRole, string> = {
    [StudioRole.Owner]: "bg-primary/10 text-primary border-primary/20",
    [StudioRole.Manager]: "bg-chart-2/10 text-chart-2 border-chart-2/20 dark:text-chart-2",
    [StudioRole.Staff]: "bg-chart-4/10 text-chart-4 border-chart-4/20 dark:text-chart-4",
  };
  
  const colorClass = colorClasses[roleValue as StudioRole] || "bg-muted/50 text-foreground border-border";
  
  return (
    <Badge className={cn(colorClass, className)}>
      {roleName}
    </Badge>
  );
}
