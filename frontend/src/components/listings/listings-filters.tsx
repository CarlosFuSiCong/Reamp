import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { SearchInput } from "@/components/shared";
import { ListingStatus } from "@/types";

interface ListingsFiltersProps {
  searchKeyword: string;
  onSearchChange: (value: string) => void;
  statusFilter: ListingStatus | "all";
  onStatusChange: (value: ListingStatus | "all") => void;
}

export function ListingsFilters({
  searchKeyword,
  onSearchChange,
  statusFilter,
  onStatusChange,
}: ListingsFiltersProps) {
  return (
    <div className="flex flex-col sm:flex-row gap-4">
      <div className="flex-1">
        <SearchInput
          value={searchKeyword}
          onChange={onSearchChange}
          placeholder="Search listings..."
        />
      </div>
      <Select
        value={statusFilter === "all" ? "all" : statusFilter.toString()}
        onValueChange={(value) =>
          onStatusChange(value === "all" ? "all" : (parseInt(value) as ListingStatus))
        }
      >
        <SelectTrigger className="w-full sm:w-[180px]">
          <SelectValue placeholder="Filter by status" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="all">All Status</SelectItem>
          <SelectItem value={ListingStatus.Draft.toString()}>Draft</SelectItem>
          <SelectItem value={ListingStatus.Active.toString()}>Active</SelectItem>
          <SelectItem value={ListingStatus.Pending.toString()}>Pending</SelectItem>
          <SelectItem value={ListingStatus.Archived.toString()}>Archived</SelectItem>
        </SelectContent>
      </Select>
    </div>
  );
}
