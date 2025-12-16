import Link from "next/link";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { ShootOrder } from "@/types";
import { getOrderStatusConfig } from "@/lib/utils/enum-labels";
import { Calendar, MapPin, Building2, Camera, ChevronRight } from "lucide-react";
import { formatDistanceToNow } from "date-fns";

interface OrderCardProps {
  order: ShootOrder;
  isAgent?: boolean;
  isStudio?: boolean;
}

export function OrderCard({ order, isAgent, isStudio }: OrderCardProps) {
  const statusConfig = getOrderStatusConfig(order.status);

  return (
    <Link href={`/dashboard/orders/${order.id}`}>
      <Card className="hover:shadow-md transition-shadow cursor-pointer">
        <CardHeader className="pb-3">
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <div className="flex items-center gap-2 mb-2">
                <Badge variant={statusConfig.variant as any}>
                  {statusConfig.label}
                </Badge>
                {order.scheduledStartUtc && (
                  <span className="text-xs text-muted-foreground flex items-center gap-1">
                    <Calendar className="h-3 w-3" />
                    {new Date(order.scheduledStartUtc).toLocaleDateString()}
                  </span>
                )}
              </div>
              <h3 className="font-semibold text-lg">
                {order.listingTitle || "Untitled Listing"}
              </h3>
              {order.listingAddress && (
                <p className="text-sm text-muted-foreground flex items-center gap-1 mt-1">
                  <MapPin className="h-3 w-3" />
                  {order.listingAddress}
                </p>
              )}
            </div>
            <ChevronRight className="h-5 w-5 text-muted-foreground" />
          </div>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-2 gap-4 text-sm">
            {isStudio && order.agencyName && (
              <div className="flex items-center gap-2">
                <Building2 className="h-4 w-4 text-muted-foreground" />
                <div>
                  <p className="text-xs text-muted-foreground">Agency</p>
                  <p className="font-medium">{order.agencyName}</p>
                </div>
              </div>
            )}
            {isAgent && order.studioName && (
              <div className="flex items-center gap-2">
                <Camera className="h-4 w-4 text-muted-foreground" />
                <div>
                  <p className="text-xs text-muted-foreground">Studio</p>
                  <p className="font-medium">{order.studioName}</p>
                </div>
              </div>
            )}

            <div>
              <p className="text-xs text-muted-foreground">Amount</p>
              <p className="font-semibold">
                {order.currency} ${order.totalAmount.toFixed(2)}
              </p>
            </div>

            {order.taskCount !== undefined && order.taskCount > 0 && (
              <div>
                <p className="text-xs text-muted-foreground">Tasks</p>
                <p className="font-medium">{order.taskCount} task(s)</p>
              </div>
            )}

            <div className="col-span-2">
              <p className="text-xs text-muted-foreground">Created</p>
              <p className="text-sm">
                {formatDistanceToNow(new Date(order.createdAtUtc), { addSuffix: true })}
              </p>
            </div>
          </div>
        </CardContent>
      </Card>
    </Link>
  );
}
