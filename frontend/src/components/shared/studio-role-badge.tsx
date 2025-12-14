import { StudioRole } from "@/types";
import { Badge } from "@/components/ui/badge";
import { getStudioRoleName } from "@/lib/utils/role-utils";

interface StudioRoleBadgeProps {
  role: StudioRole | undefined | null;
  className?: string;
}

export function StudioRoleBadge({ role, className }: StudioRoleBadgeProps) {
  if (role === undefined || role === null) {
    return <Badge className={`bg-gray-50 text-gray-700 border-gray-200 ${className || ""}`}>Member</Badge>;
  }
  
  const roleValue = typeof role === 'string' ? parseInt(role, 10) : Number(role);
  const roleName = getStudioRoleName(role);
  
  const colorClasses: Record<StudioRole, string> = {
    [StudioRole.Owner]: "bg-purple-50 text-purple-700 border-purple-200",
    [StudioRole.Manager]: "bg-blue-50 text-blue-700 border-blue-200",
    [StudioRole.Photographer]: "bg-green-50 text-green-700 border-green-200",
    [StudioRole.Editor]: "bg-orange-50 text-orange-700 border-orange-200",
    [StudioRole.Member]: "bg-gray-50 text-gray-700 border-gray-200",
  };
  
  const colorClass = colorClasses[roleValue as StudioRole] || "bg-gray-50 text-gray-700 border-gray-200";
  
  return (
    <Badge className={`${colorClass} ${className || ""}`}>
      {roleName}
    </Badge>
  );
}
