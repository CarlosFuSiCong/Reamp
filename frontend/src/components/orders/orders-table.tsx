import Link from "next/link";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { StatusBadge } from "@/components/shared";
import { ShootOrder, OrderStatus } from "@/types";
import { MoreHorizontal, Eye, XCircle } from "lucide-react";
import { formatDistanceToNow } from "date-fns";

interface OrdersTableProps {
  orders: ShootOrder[];
  onCancel?: (id: string) => void;
  isMarketplace?: boolean;
  isLoading?: boolean;
  searchQuery?: string;
  statusFilter?: string;
}

export function OrdersTable({ 
  orders, 
  onCancel, 
  isMarketplace = false,
  isLoading = false,
}: OrdersTableProps) {
  if (isLoading) {
    return (
      <div className="rounded-md border">
        <div className="text-center py-12 text-muted-foreground">Loading orders...</div>
      </div>
    );
  }

  if (orders.length === 0) {
    return (
      <div className="rounded-md border">
        <div className="text-center py-12 text-muted-foreground">
          {isMarketplace ? "No available orders in marketplace" : "No orders found"}
        </div>
      </div>
    );
  }

  const canCancelOrder = (status: OrderStatus) => {
    return status === OrderStatus.Placed || status === OrderStatus.Accepted || status === OrderStatus.Scheduled;
  };

  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Order ID</TableHead>
            <TableHead>Listing</TableHead>
            <TableHead>Studio</TableHead>
            <TableHead>Amount</TableHead>
            <TableHead>Tasks</TableHead>
            <TableHead>Status</TableHead>
            <TableHead>Created</TableHead>
            <TableHead className="w-[70px]"></TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {orders.map((order) => (
            <TableRow key={order.id}>
              <TableCell className="font-mono text-sm">#{order.id.substring(0, 8)}</TableCell>
              <TableCell className="max-w-[200px]">
                <div className="font-medium truncate">
                  {order.title || order.listingTitle || "Untitled Order"}
                </div>
                {order.listingAddress && (
                  <div className="text-xs text-muted-foreground truncate">
                    {order.listingAddress}
                  </div>
                )}
              </TableCell>
              <TableCell>{order.studioName || "Not assigned"}</TableCell>
              <TableCell className="font-semibold">
                {order.currency} {order.totalAmount.toLocaleString()}
              </TableCell>
              <TableCell>{order.taskCount ?? order.tasks?.length ?? 0} tasks</TableCell>
              <TableCell>
                <StatusBadge status={order.status} type="order" />
              </TableCell>
              <TableCell className="text-sm text-muted-foreground">
                {formatDistanceToNow(new Date(order.createdAtUtc), {
                  addSuffix: true,
                })}
              </TableCell>
              <TableCell>
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button variant="ghost" size="sm">
                      <MoreHorizontal className="h-4 w-4" />
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuItem asChild>
                      <Link href={`/dashboard/orders/${order.id}`}>
                        <Eye className="mr-2 h-4 w-4" />
                        View Details
                      </Link>
                    </DropdownMenuItem>
                    {canCancelOrder(order.status) && onCancel && (
                      <>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem
                          className="text-destructive"
                          onClick={() => onCancel(order.id)}
                        >
                          <XCircle className="mr-2 h-4 w-4" />
                          Cancel Order
                        </DropdownMenuItem>
                      </>
                    )}
                  </DropdownMenuContent>
                </DropdownMenu>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}
