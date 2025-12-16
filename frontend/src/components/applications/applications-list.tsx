"use client";

import { useState } from "react";
import { useApplications, useReviewApplication } from "@/lib/hooks";
import { ApplicationStatus, ApplicationType, ApplicationListDto } from "@/types";
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
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { Eye, Check, X } from "lucide-react";
import { LoadingState, ErrorState } from "@/components/shared";
import { ApplicationDetailDialog } from "./application-detail-dialog";

export function ApplicationsList() {
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState<ApplicationStatus | undefined>();
  const [typeFilter, setTypeFilter] = useState<ApplicationType | undefined>();
  const [selectedApp, setSelectedApp] = useState<ApplicationListDto | null>(null);
  const [detailDialogOpen, setDetailDialogOpen] = useState(false);
  const [reviewDialogOpen, setReviewDialogOpen] = useState(false);
  const [reviewNotes, setReviewNotes] = useState("");
  const [reviewAction, setReviewAction] = useState<"approve" | "reject">("approve");

  const { data, isLoading, error } = useApplications(page, 20, statusFilter, typeFilter);
  const reviewMutation = useReviewApplication();

  const getStatusBadge = (status: ApplicationStatus) => {
    const variants = {
      [ApplicationStatus.Pending]: "bg-yellow-50 text-yellow-700 border-yellow-200",
      [ApplicationStatus.UnderReview]: "bg-blue-50 text-blue-700 border-blue-200",
      [ApplicationStatus.Approved]: "bg-green-50 text-green-700 border-green-200",
      [ApplicationStatus.Rejected]: "bg-red-50 text-red-700 border-red-200",
      [ApplicationStatus.Cancelled]: "bg-gray-50 text-gray-700 border-gray-200",
    };

    return (
      <Badge variant="outline" className={variants[status]}>
        {ApplicationStatus[status]}
      </Badge>
    );
  };

  const getTypeLabel = (type: ApplicationType) => {
    return type === ApplicationType.Agency ? "Agency" : "Studio";
  };

  const canReview = (status: ApplicationStatus) => {
    return status === ApplicationStatus.Pending || status === ApplicationStatus.UnderReview;
  };

  const handleViewDetails = (app: ApplicationListDto) => {
    setSelectedApp(app);
    setDetailDialogOpen(true);
  };

  const handleReviewClick = (app: ApplicationListDto, action: "approve" | "reject") => {
    setSelectedApp(app);
    setReviewAction(action);
    setReviewNotes("");
    setReviewDialogOpen(true);
  };

  const handleReviewConfirm = () => {
    if (!selectedApp) return;

    reviewMutation.mutate(
      {
        id: selectedApp.id,
        approved: reviewAction === "approve",
        notes: reviewNotes || undefined,
      },
      {
        onSuccess: () => {
          setReviewDialogOpen(false);
          setSelectedApp(null);
          setReviewNotes("");
        },
      }
    );
  };

  if (isLoading) {
    return <LoadingState message="Loading applications..." />;
  }

  if (error) {
    return <ErrorState message="Failed to load applications" />;
  }

  return (
    <div className="space-y-4">
      {/* Filters */}
      <div className="flex gap-4 items-center">
        <div className="w-48">
          <Select
            value={statusFilter?.toString() ?? "all"}
            onValueChange={(v) => {
              setStatusFilter(v === "all" ? undefined : Number(v));
              setPage(1);
            }}
          >
            <SelectTrigger>
              <SelectValue placeholder="All Status" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Status</SelectItem>
              <SelectItem value={ApplicationStatus.Pending.toString()}>Pending</SelectItem>
              <SelectItem value={ApplicationStatus.UnderReview.toString()}>Under Review</SelectItem>
              <SelectItem value={ApplicationStatus.Approved.toString()}>Approved</SelectItem>
              <SelectItem value={ApplicationStatus.Rejected.toString()}>Rejected</SelectItem>
            </SelectContent>
          </Select>
        </div>

        <div className="w-48">
          <Select
            value={typeFilter?.toString() ?? "all"}
            onValueChange={(v) => {
              setTypeFilter(v === "all" ? undefined : Number(v));
              setPage(1);
            }}
          >
            <SelectTrigger>
              <SelectValue placeholder="All Types" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Types</SelectItem>
              <SelectItem value={ApplicationType.Agency.toString()}>Agency</SelectItem>
              <SelectItem value={ApplicationType.Studio.toString()}>Studio</SelectItem>
            </SelectContent>
          </Select>
        </div>
      </div>

      {/* Table */}
      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Organization</TableHead>
              <TableHead>Type</TableHead>
              <TableHead>Applicant</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Created</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {data?.items.map((app) => (
              <TableRow key={app.id}>
                <TableCell>
                  <div className="font-medium">{app.organizationName}</div>
                  <div className="text-sm text-gray-500">{app.contactEmail}</div>
                </TableCell>
                <TableCell>{getTypeLabel(app.type)}</TableCell>
                <TableCell>
                  <div>{app.applicantName}</div>
                  <div className="text-sm text-gray-500">{app.applicantEmail}</div>
                </TableCell>
                <TableCell>{getStatusBadge(app.status)}</TableCell>
                <TableCell>{new Date(app.createdAtUtc).toLocaleDateString()}</TableCell>
                <TableCell className="text-right">
                  <div className="flex justify-end gap-2">
                    <Button variant="ghost" size="sm" onClick={() => handleViewDetails(app)}>
                      <Eye className="h-4 w-4" />
                    </Button>
                    {canReview(app.status) && (
                      <>
                        <Button
                          variant="ghost"
                          size="sm"
                          className="text-green-600 hover:text-green-700 hover:bg-green-50"
                          onClick={() => handleReviewClick(app, "approve")}
                        >
                          <Check className="h-4 w-4" />
                        </Button>
                        <Button
                          variant="ghost"
                          size="sm"
                          className="text-red-600 hover:text-red-700 hover:bg-red-50"
                          onClick={() => handleReviewClick(app, "reject")}
                        >
                          <X className="h-4 w-4" />
                        </Button>
                      </>
                    )}
                  </div>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      {/* Pagination */}
      {data && data.total > 20 && (
        <div className="flex justify-center gap-2">
          <Button
            variant="outline"
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            disabled={page === 1}
          >
            Previous
          </Button>
          <span className="px-4 py-2 text-sm">
            Page {page} of {Math.ceil(data.total / 20)}
          </span>
          <Button
            variant="outline"
            onClick={() => setPage((p) => p + 1)}
            disabled={page >= Math.ceil(data.total / 20)}
          >
            Next
          </Button>
        </div>
      )}

      {/* Detail Dialog */}
      {selectedApp && (
        <ApplicationDetailDialog
          applicationId={selectedApp.id}
          open={detailDialogOpen}
          onOpenChange={setDetailDialogOpen}
        />
      )}

      {/* Review Dialog */}
      <AlertDialog open={reviewDialogOpen} onOpenChange={setReviewDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>
              {reviewAction === "approve" ? "Approve" : "Reject"} Application
            </AlertDialogTitle>
            <AlertDialogDescription>
              {reviewAction === "approve"
                ? "This will approve the application and create the organization automatically."
                : "This will reject the application. The applicant will not be able to revert this."}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <div className="py-4">
            <Label htmlFor="review-notes">Notes (Optional)</Label>
            <Textarea
              id="review-notes"
              value={reviewNotes}
              onChange={(e) => setReviewNotes(e.target.value)}
              placeholder="Add any notes or feedback for the applicant..."
              className="mt-2"
              rows={4}
            />
          </div>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleReviewConfirm}
              disabled={reviewMutation.isPending}
              className={
                reviewAction === "approve"
                  ? "bg-green-600 hover:bg-green-700"
                  : "bg-red-600 hover:bg-red-700"
              }
            >
              {reviewMutation.isPending
                ? "Processing..."
                : reviewAction === "approve"
                  ? "Approve"
                  : "Reject"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
