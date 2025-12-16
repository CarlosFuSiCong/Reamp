import { AgencyRole } from "@/types";
import { Badge } from "@/components/ui/badge";
import { getAgencyRoleName } from "@/lib/utils/role-utils";
import { cn } from "@/lib/utils";

interface AgencyRoleBadgeProps {
  role: AgencyRole | undefined | null;
  className?: string;
}

export function AgencyRoleBadge({ role, className }: AgencyRoleBadgeProps) {
  if (role === undefined || role === null) {
    return (
      <Badge className={cn("bg-muted/50 text-foreground border-border", className)}>Agent</Badge>
    );
  }

  const roleValue = typeof role === "string" ? parseInt(role, 10) : Number(role);
  const roleName = getAgencyRoleName(role);

  const colorClasses: Record<AgencyRole, string> = {
    [AgencyRole.Owner]: "bg-primary/10 text-primary border-primary/20",
    [AgencyRole.Manager]: "bg-chart-2/10 text-chart-2 border-chart-2/20 dark:text-chart-2",
    [AgencyRole.Agent]: "bg-chart-4/10 text-chart-4 border-chart-4/20 dark:text-chart-4",
  };

  const colorClass =
    colorClasses[roleValue as AgencyRole] || "bg-muted/50 text-foreground border-border";

  return <Badge className={cn(colorClass, className)}>{roleName}</Badge>;
}
