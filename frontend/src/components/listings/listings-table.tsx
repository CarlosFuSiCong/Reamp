import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
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
import { getListingTypeLabel } from "@/lib/utils/enum-labels";
import { MoreHorizontal, Edit, Eye, Archive, Trash, MapPin, DollarSign, Home, Send } from "lucide-react";

interface ListingsTableProps {
  listings: Listing[];
  onPublish: (id: string) => void;
  onArchive: (id: string) => void;
  onDelete: (id: string) => void;
}

export function ListingsTable({ listings, onPublish, onArchive, onDelete }: ListingsTableProps) {
  if (listings.length === 0) {
    return (
      <div className="rounded-md border">
        <div className="text-center py-12 text-muted-foreground">No listings found</div>
      </div>
    );
  }

  return (
    <div className="overflow-hidden">
      <Table>
        <TableHeader>
          <TableRow className="bg-gray-50 hover:bg-gray-50">
            <TableHead className="font-semibold text-gray-700">Property</TableHead>
            <TableHead className="font-semibold text-gray-700">Location</TableHead>
            <TableHead className="font-semibold text-gray-700">Price</TableHead>
            <TableHead className="font-semibold text-gray-700">Type</TableHead>
            <TableHead className="font-semibold text-gray-700">Status</TableHead>
            <TableHead className="w-[70px]"></TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {listings.map((listing, index) => (
            <TableRow 
              key={listing.id}
              className="group hover:bg-blue-50/50 transition-colors"
            >
              {/* Property Title with Icon */}
              <TableCell>
                <div className="flex items-center gap-3">
                  <div className="h-10 w-10 rounded-lg bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center flex-shrink-0 shadow-sm">
                    <Home className="h-5 w-5 text-white" />
                  </div>
                  <div className="flex flex-col">
                    <Link 
                      href={`/dashboard/listings/${listing.id}`}
                      className="font-semibold text-gray-900 hover:text-blue-600 transition-colors line-clamp-1"
                    >
                      {listing.title}
                    </Link>
                    <span className="text-xs text-gray-500">
                      ID: {listing.id.substring(0, 8)}
                    </span>
                  </div>
                </div>
              </TableCell>

              {/* Location with Icon */}
              <TableCell>
                <div className="flex items-center gap-2 text-gray-700">
                  <MapPin className="h-4 w-4 text-gray-400 flex-shrink-0" />
                  <span className="line-clamp-1">
                    {listing.city}, {listing.state}
                  </span>
                </div>
              </TableCell>

              {/* Price with Icon and Formatting */}
              <TableCell>
                <div className="flex items-center gap-2">
                  <div className="h-8 w-8 rounded-md bg-green-50 flex items-center justify-center">
                    <DollarSign className="h-4 w-4 text-green-600" />
                  </div>
                  <div className="flex flex-col">
                    <span className="font-semibold text-gray-900">
                      {listing.currency} {listing.price.toLocaleString()}
                    </span>
                    <span className="text-xs text-gray-500">
                      {listing.listingType === 0 ? 'Sale' : 'Rent'}
                    </span>
                  </div>
                </div>
              </TableCell>

              {/* Type Badge */}
              <TableCell>
                <Badge 
                  variant="outline" 
                  className="font-medium bg-white border-gray-300"
                >
                  {getListingTypeLabel(listing.listingType)}
                </Badge>
              </TableCell>

              {/* Status Badge */}
              <TableCell>
                <StatusBadge status={listing.status} type="listing" />
              </TableCell>

              {/* Actions Dropdown */}
              <TableCell>
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button 
                      variant="ghost" 
                      size="sm"
                      className="opacity-0 group-hover:opacity-100 transition-opacity"
                    >
                      <MoreHorizontal className="h-4 w-4" />
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end" className="w-48">
                    <DropdownMenuItem asChild>
                      <Link href={`/dashboard/listings/${listing.id}`} className="cursor-pointer">
                        <Eye className="mr-2 h-4 w-4" />
                        View Details
                      </Link>
                    </DropdownMenuItem>
                    <DropdownMenuItem asChild>
                      <Link href={`/dashboard/listings/${listing.id}/edit`} className="cursor-pointer">
                        <Edit className="mr-2 h-4 w-4" />
                        Edit Listing
                      </Link>
                    </DropdownMenuItem>
                    <DropdownMenuSeparator />
                    {listing.status === ListingStatus.Draft && (
                      <DropdownMenuItem onClick={() => onPublish(listing.id)} className="text-green-600">
                        <Send className="mr-2 h-4 w-4" />
                        Publish
                      </DropdownMenuItem>
                    )}
                    {listing.status === ListingStatus.Active && (
                      <DropdownMenuItem onClick={() => onArchive(listing.id)} className="text-orange-600">
                        <Archive className="mr-2 h-4 w-4" />
                        Archive
                      </DropdownMenuItem>
                    )}
                    {(listing.status === ListingStatus.Draft || listing.status === ListingStatus.Active) && (
                      <DropdownMenuSeparator />
                    )}
                    <DropdownMenuItem
                      className="text-red-600 focus:text-red-600 focus:bg-red-50"
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
