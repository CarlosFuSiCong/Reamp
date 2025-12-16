"use client";

import { format } from "date-fns";
import Link from "next/link";
import { Eye, Package } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { DeliveryPackageListDto, DeliveryStatus } from "@/types";
import { LoadingState } from "@/components/shared";

interface DeliveriesTableProps {
  deliveries: DeliveryPackageListDto[];
  isLoading: boolean;
  searchQuery: string;
  statusFilter: string;
}

export function DeliveriesTable({
  deliveries,
  isLoading,
  searchQuery,
  statusFilter,
}: DeliveriesTableProps) {
  if (isLoading) {
    return <LoadingState message="Loading deliveries..." />;
  }

  // Filter deliveries
  const filteredDeliveries = deliveries.filter((delivery) => {
    const matchesSearch = delivery.title
      .toLowerCase()
      .includes(searchQuery.toLowerCase());
    const matchesStatus =
      statusFilter === "all" || delivery.status.toString() === statusFilter;
    return matchesSearch && matchesStatus;
  });

  if (filteredDeliveries.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center h-64 border-2 border-dashed rounded-lg">
        <Package className="h-12 w-12 text-muted-foreground mb-4" />
        <p className="text-muted-foreground">
          {searchQuery || statusFilter !== "all"
            ? "No deliveries found matching your filters"
            : "No deliveries yet"}
        </p>
      </div>
    );
  }

  return (
    <div className="border rounded-lg">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Title</TableHead>
            <TableHead>Status</TableHead>
            <TableHead>Items</TableHead>
            <TableHead>Created</TableHead>
            <TableHead className="text-right">Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {filteredDeliveries.map((delivery) => (
            <TableRow key={delivery.id}>
              <TableCell>
                <div>
                  <p className="font-medium">{delivery.title}</p>
                  <p className="text-sm text-muted-foreground">
                    Order #{delivery.orderId.slice(0, 8)}
                  </p>
                </div>
              </TableCell>
              <TableCell>
                <DeliveryStatusBadge status={delivery.status} />
              </TableCell>
              <TableCell>
                <span className="text-sm">{delivery.itemCount} files</span>
              </TableCell>
              <TableCell>
                <span className="text-sm">
                  {format(new Date(delivery.createdAtUtc), "MMM d, yyyy")}
                </span>
              </TableCell>
              <TableCell className="text-right">
                <Link href={`/dashboard/deliveries/${delivery.id}`}>
                  <Button variant="ghost" size="sm">
                    <Eye className="h-4 w-4" />
                  </Button>
                </Link>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}

function DeliveryStatusBadge({ status }: { status: DeliveryStatus }) {
  const variants: Record<DeliveryStatus, { label: string; variant: any }> = {
    [DeliveryStatus.Draft]: { label: "Draft", variant: "secondary" },
    [DeliveryStatus.Published]: { label: "Published", variant: "default" },
    [DeliveryStatus.Revoked]: { label: "Revoked", variant: "destructive" },
    [DeliveryStatus.Expired]: { label: "Expired", variant: "outline" },
  };

  const config = variants[status];

  return <Badge variant={config.variant}>{config.label}</Badge>;
}
