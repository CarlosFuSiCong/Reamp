"use client";

import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { applicationsApi } from "@/lib/api";
import { ApplicationStatus, ApplicationType, ApplicationListDto } from "@/types";
import { toast } from "sonner";

export function ApplicationsList() {
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState<ApplicationStatus | undefined>();
  const [typeFilter, setTypeFilter] = useState<ApplicationType | undefined>();
  const queryClient = useQueryClient();

  const { data, isLoading } = useQuery({
    queryKey: ["applications", page, statusFilter, typeFilter],
    queryFn: () => applicationsApi.getApplications(page, 20, statusFilter, typeFilter),
  });

  const reviewMutation = useMutation({
    mutationFn: ({ id, approved, notes }: { id: string; approved: boolean; notes?: string }) =>
      applicationsApi.reviewApplication(id, { approved, notes }),
    onSuccess: () => {
      toast.success("Application reviewed successfully");
      queryClient.invalidateQueries({ queryKey: ["applications"] });
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to review application");
    },
  });

  const getStatusColor = (status: ApplicationStatus) => {
    switch (status) {
      case ApplicationStatus.Pending:
        return "bg-yellow-100 text-yellow-800";
      case ApplicationStatus.UnderReview:
        return "bg-blue-100 text-blue-800";
      case ApplicationStatus.Approved:
        return "bg-green-100 text-green-800";
      case ApplicationStatus.Rejected:
        return "bg-red-100 text-red-800";
      case ApplicationStatus.Cancelled:
        return "bg-gray-100 text-gray-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  const getStatusLabel = (status: ApplicationStatus) => {
    return ApplicationStatus[status];
  };

  const getTypeLabel = (type: ApplicationType) => {
    return type === ApplicationType.Agency ? "Agency" : "Studio";
  };

  if (isLoading) {
    return <div className="p-6">Loading...</div>;
  }

  return (
    <div className="space-y-6">
      <div className="flex gap-4 items-center">
        <select
          value={statusFilter ?? ""}
          onChange={(e) => setStatusFilter(e.target.value ? Number(e.target.value) : undefined)}
          className="px-3 py-2 border border-gray-300 rounded-md"
        >
          <option value="">All Status</option>
          <option value={ApplicationStatus.Pending}>Pending</option>
          <option value={ApplicationStatus.UnderReview}>Under Review</option>
          <option value={ApplicationStatus.Approved}>Approved</option>
          <option value={ApplicationStatus.Rejected}>Rejected</option>
        </select>

        <select
          value={typeFilter ?? ""}
          onChange={(e) => setTypeFilter(e.target.value ? Number(e.target.value) : undefined)}
          className="px-3 py-2 border border-gray-300 rounded-md"
        >
          <option value="">All Types</option>
          <option value={ApplicationType.Agency}>Agency</option>
          <option value={ApplicationType.Studio}>Studio</option>
        </select>
      </div>

      <div className="bg-white shadow rounded-lg overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Organization
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Type
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Applicant
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Status
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Created
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                Actions
              </th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {data?.items.map((app) => (
              <tr key={app.id}>
                <td className="px-6 py-4 whitespace-nowrap">
                  <div className="text-sm font-medium text-gray-900">{app.organizationName}</div>
                  <div className="text-sm text-gray-500">{app.contactEmail}</div>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <span className="text-sm text-gray-900">{getTypeLabel(app.type)}</span>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <div className="text-sm text-gray-900">{app.applicantName}</div>
                  <div className="text-sm text-gray-500">{app.applicantEmail}</div>
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <span className={`px-2 py-1 text-xs rounded-full ${getStatusColor(app.status)}`}>
                    {getStatusLabel(app.status)}
                  </span>
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {new Date(app.createdAtUtc).toLocaleDateString()}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm">
                  {app.status === ApplicationStatus.Pending ||
                  app.status === ApplicationStatus.UnderReview ? (
                    <div className="flex gap-2">
                      <button
                        onClick={() =>
                          reviewMutation.mutate({ id: app.id, approved: true })
                        }
                        className="text-green-600 hover:text-green-900"
                      >
                        Approve
                      </button>
                      <button
                        onClick={() =>
                          reviewMutation.mutate({ id: app.id, approved: false })
                        }
                        className="text-red-600 hover:text-red-900"
                      >
                        Reject
                      </button>
                    </div>
                  ) : (
                    <span className="text-gray-400">-</span>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {data && data.total > 20 && (
        <div className="flex justify-center gap-2">
          <button
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            disabled={page === 1}
            className="px-4 py-2 border rounded-md disabled:opacity-50"
          >
            Previous
          </button>
          <span className="px-4 py-2">
            Page {page} of {Math.ceil(data.total / 20)}
          </span>
          <button
            onClick={() => setPage((p) => p + 1)}
            disabled={page >= Math.ceil(data.total / 20)}
            className="px-4 py-2 border rounded-md disabled:opacity-50"
          >
            Next
          </button>
        </div>
      )}
    </div>
  );
}
