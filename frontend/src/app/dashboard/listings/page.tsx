"use client";

import { useState } from "react";
import Link from "next/link";
import { PageHeader, LoadingState, ErrorState, ConfirmDialog, Pagination } from "@/components/shared";
import { ListingsTable, ListingsFilters } from "@/components/listings";
import { Button } from "@/components/ui/button";
import {
  useListings,
  useDeleteListing,
  usePublishListing,
  useArchiveListing,
} from "@/lib/hooks/use-listings";
import { ListingStatus } from "@/types";
import { Plus } from "lucide-react";

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
    return <LoadingState />;
  }

  if (error) {
    return <ErrorState message="Failed to load listings" />;
  }

  return (
    <div className="space-y-6">
      <PageHeader
        title="My Listings"
        description="Manage your property listings"
        action={
          <Button asChild>
            <Link href="/dashboard/listings/new">
              <Plus className="mr-2 h-4 w-4" />
              New Listing
            </Link>
          </Button>
        }
      />

      <ListingsFilters
        searchKeyword={searchKeyword}
        onSearchChange={setSearchKeyword}
        statusFilter={statusFilter}
        onStatusChange={setStatusFilter}
      />

      <ListingsTable
        listings={data?.items || []}
        onPublish={handlePublish}
        onArchive={handleArchive}
        onDelete={openDeleteDialog}
      />

      {data && (
        <Pagination
          currentPage={data.page}
          totalItems={data.total}
          pageSize={data.pageSize}
          onPageChange={setPage}
        />
      )}

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
