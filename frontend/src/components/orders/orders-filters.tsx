import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { SearchInput } from "@/components/shared";
import { OrderStatus } from "@/types";
import { orderStatusConfig } from "@/lib/utils/enum-labels";

interface OrdersFiltersProps {
  searchKeyword: string;
  onSearchChange: (value: string) => void;
  statusFilter: OrderStatus | "all";
  onStatusChange: (value: OrderStatus | "all") => void;
}

export function OrdersFilters({
  searchKeyword,
  onSearchChange,
  statusFilter,
  onStatusChange,
}: OrdersFiltersProps) {
  return (
    <div className="flex flex-col sm:flex-row gap-4">
      <div className="flex-1">
        <SearchInput
          value={searchKeyword}
          onChange={onSearchChange}
          placeholder="Search by listing or client name..."
        />
      </div>
      <Select
        value={statusFilter === "all" ? "all" : statusFilter.toString()}
        onValueChange={(value) =>
          onStatusChange(value === "all" ? "all" : (parseInt(value) as OrderStatus))
        }
      >
        <SelectTrigger className="w-full sm:w-[180px]">
          <SelectValue placeholder="Filter by status" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="all">All Status</SelectItem>
          {Object.entries(orderStatusConfig).map(([key, config]) => (
            <SelectItem key={key} value={key}>
              {config.label}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
}
