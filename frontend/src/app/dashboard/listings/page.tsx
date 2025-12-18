"use client";

import { useState } from "react";
import Link from "next/link";
import {
  PageHeader,
  LoadingState,
  ErrorState,
  ConfirmDialog,
  Pagination,
} from "@/components/shared";
import { ListingsTable, ListingsFilters } from "@/components/listings";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  useListings,
  useDeleteListing,
  usePublishListing,
  useArchiveListing,
} from "@/lib/hooks/use-listings";
import { ListingStatus } from "@/types";
import { Plus, Home, LayoutGrid } from "lucide-react";

export default function AgentListingsPage() {
  const [statusFilter, setStatusFilter] = useState<ListingStatus | "all">("all");
  const [searchKeyword, setSearchKeyword] = useState("");
  const [page, setPage] = useState(1);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [selectedListingId, setSelectedListingId] = useState<string | null>(null);

  const { data, isLoading, error } = useListings({
    status: statusFilter === "all" ? undefined : statusFilter,
    keyword: searchKeyword || undefined,
    page,
    pageSize: 20,
  });

  const deleteMutation = useDeleteListing();
  const publishMutation = usePublishListing();
  const archiveMutation = useArchiveListing();

  const handleDelete = () => {
    if (selectedListingId) {
      deleteMutation.mutate(selectedListingId);
      setDeleteDialogOpen(false);
      setSelectedListingId(null);
    }
  };

  const handlePublish = (id: string) => {
    publishMutation.mutate(id);
  };

  const handleArchive = (id: string) => {
    archiveMutation.mutate(id);
  };

  const openDeleteDialog = (id: string) => {
    setSelectedListingId(id);
    setDeleteDialogOpen(true);
  };

  if (isLoading) {
    return <LoadingState message="Loading listings..." />;
  }

  if (error) {
    return <ErrorState message="Failed to load listings" />;
  }

  const listings = data?.items || [];
  const hasListings = listings.length > 0;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <div className="flex items-center gap-3">
            <div className="h-10 w-10 rounded-xl bg-gradient-to-br from-blue-600 to-indigo-700 flex items-center justify-center text-white">
              <LayoutGrid className="h-5 w-5" />
            </div>
            <div>
              <h1 className="text-3xl font-bold tracking-tight text-gray-900">My Listings</h1>
              <p className="text-muted-foreground mt-0.5">
                Manage your property listings
              </p>
            </div>
          </div>
        </div>
        <Button asChild className="shadow-lg hover:shadow-xl transition-all">
          <Link href="/dashboard/listings/new">
            <Plus className="mr-2 h-4 w-4" />
            New Listing
          </Link>
        </Button>
      </div>

      {/* Filters Card */}
      <Card className="border-0 shadow-md">
        <CardContent className="pt-6">
          <ListingsFilters
            searchKeyword={searchKeyword}
            onSearchChange={setSearchKeyword}
            statusFilter={statusFilter}
            onStatusChange={setStatusFilter}
          />
        </CardContent>
      </Card>

      {/* Listings Table Card */}
      <Card className="border-0 shadow-lg">
        <CardContent className="p-0">
          {hasListings ? (
            <ListingsTable
              listings={listings}
              onPublish={handlePublish}
              onArchive={handleArchive}
              onDelete={openDeleteDialog}
            />
          ) : (
            <div className="text-center py-16 px-4">
              <div className="mx-auto h-24 w-24 rounded-full bg-gradient-to-br from-blue-100 to-indigo-100 flex items-center justify-center mb-6">
                <Home className="h-12 w-12 text-blue-600" />
              </div>
              <h3 className="text-xl font-semibold text-gray-900 mb-2">No listings found</h3>
              <p className="text-gray-600 mb-6 max-w-md mx-auto">
                {searchKeyword || statusFilter !== "all"
                  ? "Try adjusting your filters to find what you're looking for"
                  : "Create your first property listing to get started"}
              </p>
              {!searchKeyword && statusFilter === "all" && (
                <Button asChild size="lg" className="shadow-lg">
                  <Link href="/dashboard/listings/new">
                    <Plus className="mr-2 h-5 w-5" />
                    Create Your First Listing
                  </Link>
                </Button>
              )}
            </div>
          )}
        </CardContent>
      </Card>

      {/* Pagination */}
      {data && data.total > data.pageSize && (
        <div className="flex justify-center">
          <Pagination
            currentPage={data.page}
            totalItems={data.total}
            pageSize={data.pageSize}
            onPageChange={setPage}
          />
        </div>
      )}

      {/* Delete Confirmation Dialog */}
      <ConfirmDialog
        open={deleteDialogOpen}
        onOpenChange={setDeleteDialogOpen}
        title="Delete Listing"
        description="Are you sure you want to delete this listing? This action cannot be undone."
        onConfirm={handleDelete}
        confirmText="Delete"
        variant="destructive"
      />
    </div>
  );
}
