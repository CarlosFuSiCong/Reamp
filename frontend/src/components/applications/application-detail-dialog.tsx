"use client";

import { useApplicationDetail } from "@/lib/hooks";
import { ApplicationStatus, ApplicationType } from "@/types";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Badge } from "@/components/ui/badge";
import { Building2, Camera, Calendar, Mail, Phone, MapPin, User } from "lucide-react";
import { LoadingState, ErrorState } from "@/components/shared";

interface ApplicationDetailDialogProps {
  applicationId: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function ApplicationDetailDialog({
  applicationId,
  open,
  onOpenChange,
}: ApplicationDetailDialogProps) {
  const { data: application, isLoading, error } = useApplicationDetail(applicationId);

  const getStatusBadge = (status: ApplicationStatus) => {
    switch (status) {
      case ApplicationStatus.Pending:
        return <Badge className="bg-yellow-50 text-yellow-700 border-yellow-200">Pending</Badge>;
      case ApplicationStatus.UnderReview:
        return <Badge className="bg-blue-50 text-blue-700 border-blue-200">Under Review</Badge>;
      case ApplicationStatus.Approved:
        return <Badge className="bg-green-50 text-green-700 border-green-200">Approved</Badge>;
      case ApplicationStatus.Rejected:
        return <Badge className="bg-red-50 text-red-700 border-red-200">Rejected</Badge>;
      case ApplicationStatus.Cancelled:
        return <Badge className="bg-gray-50 text-gray-700 border-gray-200">Cancelled</Badge>;
      default:
        return <Badge>{ApplicationStatus[status]}</Badge>;
    }
  };

  const getTypeIcon = (type: ApplicationType) => {
    return type === ApplicationType.Agency ? (
      <Building2 className="h-5 w-5 text-blue-600" />
    ) : (
      <Camera className="h-5 w-5 text-purple-600" />
    );
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[80vh] overflow-y-auto">
        {isLoading && <LoadingState message="Loading application details..." />}

        {error && <ErrorState message="Failed to load application details" />}

        {application && (
          <>
            <DialogHeader>
              <div className="flex items-start gap-3">
                {getTypeIcon(application.type)}
                <div className="flex-1">
                  <DialogTitle className="text-xl">{application.organizationName}</DialogTitle>
                  <DialogDescription className="mt-1">
                    {application.type === ApplicationType.Agency ? "Agency" : "Studio"} Application
                  </DialogDescription>
                </div>
                {getStatusBadge(application.status)}
              </div>
            </DialogHeader>

            <div className="space-y-6 py-4">
              {/* Applicant Information */}
              <div>
                <h3 className="text-sm font-semibold text-gray-900 mb-3">Applicant Information</h3>
                <div className="space-y-2 text-sm">
                  <div className="flex items-center gap-2 text-gray-600">
                    <User className="h-4 w-4" />
                    <span>{application.applicantName}</span>
                  </div>
                  <div className="flex items-center gap-2 text-gray-600">
                    <Mail className="h-4 w-4" />
                    <span>{application.applicantEmail}</span>
                  </div>
                </div>
              </div>

              {/* Organization Information */}
              <div>
                <h3 className="text-sm font-semibold text-gray-900 mb-3">
                  Organization Information
                </h3>
                <div className="space-y-2 text-sm">
                  <div className="flex items-center gap-2 text-gray-600">
                    <Mail className="h-4 w-4" />
                    <span>{application.contactEmail}</span>
                  </div>
                  <div className="flex items-center gap-2 text-gray-600">
                    <Phone className="h-4 w-4" />
                    <span>{application.contactPhone}</span>
                  </div>
                </div>
              </div>

              {/* Description */}
              {application.description && (
                <div>
                  <h3 className="text-sm font-semibold text-gray-900 mb-2">Description</h3>
                  <p className="text-sm text-gray-600">{application.description}</p>
                </div>
              )}

              {/* Address (Studio only) */}
              {application.type === ApplicationType.Studio && application.address && (
                <div>
                  <h3 className="text-sm font-semibold text-gray-900 mb-3">Studio Address</h3>
                  <div className="flex items-start gap-2 text-sm text-gray-600">
                    <MapPin className="h-4 w-4 mt-0.5" />
                    <div>
                      {application.address.street && <div>{application.address.street}</div>}
                      <div>
                        {[
                          application.address.city,
                          application.address.state,
                          application.address.postalCode,
                        ]
                          .filter(Boolean)
                          .join(", ")}
                      </div>
                      {application.address.country && <div>{application.address.country}</div>}
                    </div>
                  </div>
                </div>
              )}

              {/* Timeline */}
              <div>
                <h3 className="text-sm font-semibold text-gray-900 mb-3">Timeline</h3>
                <div className="space-y-2 text-sm">
                  <div className="flex items-center gap-2 text-gray-600">
                    <Calendar className="h-4 w-4" />
                    <span>Submitted: {new Date(application.createdAtUtc).toLocaleString()}</span>
                  </div>
                  {application.reviewedAtUtc && (
                    <div className="flex items-center gap-2 text-gray-600">
                      <Calendar className="h-4 w-4" />
                      <span>Reviewed: {new Date(application.reviewedAtUtc).toLocaleString()}</span>
                    </div>
                  )}
                </div>
              </div>

              {/* Review Information */}
              {application.reviewedBy && (
                <div>
                  <h3 className="text-sm font-semibold text-gray-900 mb-3">Review Information</h3>
                  <div className="space-y-2 text-sm">
                    <div className="flex items-center gap-2 text-gray-600">
                      <User className="h-4 w-4" />
                      <span>Reviewed by: {application.reviewerName}</span>
                    </div>
                    {application.reviewNotes && (
                      <div className="mt-2 p-3 bg-gray-50 rounded-md">
                        <p className="text-sm text-gray-700">{application.reviewNotes}</p>
                      </div>
                    )}
                  </div>
                </div>
              )}

              {/* Created Organization */}
              {application.createdOrganizationId && (
                <div className="p-3 bg-green-50 rounded-md border border-green-200">
                  <p className="text-sm text-green-800">
                    ✓ Organization created successfully (ID: {application.createdOrganizationId})
                  </p>
                </div>
              )}
            </div>
          </>
        )}
      </DialogContent>
    </Dialog>
  );
}
