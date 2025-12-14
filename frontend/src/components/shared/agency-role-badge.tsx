import { AgencyRole } from "@/types";
import { Badge } from "@/components/ui/badge";
import { getAgencyRoleName } from "@/lib/utils/role-utils";

interface AgencyRoleBadgeProps {
  role: AgencyRole | undefined | null;
  className?: string;
}

export function AgencyRoleBadge({ role, className }: AgencyRoleBadgeProps) {
  if (role === undefined || role === null) {
    return <Badge className={`bg-gray-50 text-gray-700 border-gray-200 ${className || ""}`}>Agent</Badge>;
  }
  
  const roleValue = typeof role === 'string' ? parseInt(role, 10) : Number(role);
  const roleName = getAgencyRoleName(role);
  
  const colorClasses: Record<AgencyRole, string> = {
    [AgencyRole.Owner]: "bg-purple-50 text-purple-700 border-purple-200",
    [AgencyRole.Manager]: "bg-blue-50 text-blue-700 border-blue-200",
    [AgencyRole.Agent]: "bg-green-50 text-green-700 border-green-200",
  };
  
  const colorClass = colorClasses[roleValue as AgencyRole] || "bg-gray-50 text-gray-700 border-gray-200";
  
  return (
    <Badge className={`${colorClass} ${className || ""}`}>
      {roleName}
    </Badge>
  );
}
