"use client";

import { SearchInput } from "@/components/shared";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { DeliveryStatus } from "@/types";

interface DeliveriesFiltersProps {
  searchQuery: string;
  onSearchChange: (value: string) => void;
  statusFilter: string;
  onStatusChange: (value: string) => void;
}

export function DeliveriesFilters({
  searchQuery,
  onSearchChange,
  statusFilter,
  onStatusChange,
}: DeliveriesFiltersProps) {
  return (
    <div className="flex items-center gap-4">
      <div className="flex-1">
        <SearchInput
          value={searchQuery}
          onChange={onSearchChange}
          placeholder="Search deliveries..."
        />
      </div>

      <Select value={statusFilter} onValueChange={onStatusChange}>
        <SelectTrigger className="w-[180px]">
          <SelectValue placeholder="Filter by status" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="all">All Status</SelectItem>
          <SelectItem value={DeliveryStatus.Draft.toString()}>Draft</SelectItem>
          <SelectItem value={DeliveryStatus.Published.toString()}>
            Published
          </SelectItem>
          <SelectItem value={DeliveryStatus.Revoked.toString()}>Revoked</SelectItem>
          <SelectItem value={DeliveryStatus.Expired.toString()}>Expired</SelectItem>
        </SelectContent>
      </Select>
    </div>
  );
}
