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
import { Listing, ListingStatus } from "@/types";
import { MoreHorizontal, Edit, Eye, Archive, Trash } from "lucide-react";

interface ListingsTableProps {
  listings: Listing[];
  onPublish: (id: string) => void;
  onArchive: (id: string) => void;
  onDelete: (id: string) => void;
}

export function ListingsTable({
  listings,
  onPublish,
  onArchive,
  onDelete,
}: ListingsTableProps) {
  if (listings.length === 0) {
    return (
      <div className="rounded-md border">
        <div className="text-center py-12 text-muted-foreground">
          No listings found
        </div>
      </div>
    );
  }

  return (
    <div className="rounded-md border">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Title</TableHead>
            <TableHead>Address</TableHead>
            <TableHead>Price</TableHead>
            <TableHead>Type</TableHead>
            <TableHead>Status</TableHead>
            <TableHead className="w-[70px]"></TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {listings.map((listing) => (
            <TableRow key={listing.id}>
              <TableCell className="font-medium">{listing.title}</TableCell>
              <TableCell>
                {listing.city}, {listing.state}
              </TableCell>
              <TableCell>
                {listing.currency} {listing.price.toLocaleString()}
              </TableCell>
              <TableCell className="capitalize">
                {listing.listingType === 1 ? "For Sale" : "For Rent"}
              </TableCell>
              <TableCell>
                <StatusBadge status={listing.status} type="listing" />
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
                      <Link href={`/agent/listings/${listing.id}`}>
                        <Eye className="mr-2 h-4 w-4" />
                        View
                      </Link>
                    </DropdownMenuItem>
                    <DropdownMenuItem asChild>
                      <Link href={`/agent/listings/${listing.id}/edit`}>
                        <Edit className="mr-2 h-4 w-4" />
                        Edit
                      </Link>
                    </DropdownMenuItem>
                    <DropdownMenuSeparator />
                    {listing.status === ListingStatus.Draft && (
                      <DropdownMenuItem onClick={() => onPublish(listing.id)}>
                        <Eye className="mr-2 h-4 w-4" />
                        Publish
                      </DropdownMenuItem>
                    )}
                    {listing.status === ListingStatus.Active && (
                      <DropdownMenuItem onClick={() => onArchive(listing.id)}>
                        <Archive className="mr-2 h-4 w-4" />
                        Archive
                      </DropdownMenuItem>
                    )}
                    <DropdownMenuSeparator />
                    <DropdownMenuItem
                      className="text-destructive"
                      onClick={() => onDelete(listing.id)}
                    >
                      <Trash className="mr-2 h-4 w-4" />
                      Delete
                    </DropdownMenuItem>
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
