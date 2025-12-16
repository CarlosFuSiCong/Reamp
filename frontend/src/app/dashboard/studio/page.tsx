"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import {
  Camera,
  Calendar,
  CheckCircle,
  Clock,
  Users,
  Mail,
  MoreVertical,
  UserPlus,
  Shield,
} from "lucide-react";
import { ordersApi } from "@/lib/api";
import {
  PageHeader,
  LoadingState,
  ErrorState,
  StudioRoleBadge,
  InvitationStatusBadge,
} from "@/components/shared";
import { StatsCard } from "@/components/dashboard";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
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
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { InviteStaffDialog } from "@/components/studios";
import { ConfirmDialog } from "@/components/shared/confirm-dialog";
import {
  useStudioMembers,
  useStudioInvitations,
  useUpdateStudioMemberRole,
  useRemoveStudioMember,
  useCancelInvitation,
  useProfile,
} from "@/lib/hooks";
import { OrderStatus, StudioRole, InvitationStatus } from "@/types";
import { canInviteStudioMembers, canManageStudioMember } from "@/lib/utils";

export default function StudioDashboardPage() {
  const { user: profile } = useProfile();
  const studioId = profile?.studioId || "";

  const [inviteDialogOpen, setInviteDialogOpen] = useState(false);
  const [selectedMember, setSelectedMember] = useState<string | null>(null);
  const [confirmRemoveOpen, setConfirmRemoveOpen] = useState(false);
  const [selectedInvitation, setSelectedInvitation] = useState<string | null>(null);
  const [confirmCancelOpen, setConfirmCancelOpen] = useState(false);

  // Orders data
  const {
    data: ordersData,
    isLoading: ordersLoading,
    error: ordersError,
  } = useQuery({
    queryKey: ["orders", { page: 1, pageSize: 100 }],
    queryFn: () => ordersApi.list({ page: 1, pageSize: 100 }),
  });

  // Team members data
  const {
    data: members,
    isLoading: membersLoading,
    error: membersError,
  } = useStudioMembers(studioId);
  const { data: invitations } = useStudioInvitations(studioId);

  // Mutations
  const updateRole = useUpdateStudioMemberRole();
  const removeMember = useRemoveStudioMember();
  const cancelInvitation = useCancelInvitation();

  const handleRemoveMember = () => {
    if (selectedMember) {
      removeMember.mutate({ studioId, memberId: selectedMember });
      setConfirmRemoveOpen(false);
      setSelectedMember(null);
    }
  };

  const handleCancelInvitation = () => {
    if (selectedInvitation) {
      cancelInvitation.mutate(selectedInvitation);
      setConfirmCancelOpen(false);
      setSelectedInvitation(null);
    }
  };

  if (ordersLoading || membersLoading) {
    return <LoadingState message="Loading studio dashboard..." />;
  }

  if (ordersError || membersError) {
    return <ErrorState message="Failed to load dashboard data" />;
  }

  const orders = ordersData?.items || [];
  const pendingOrders = orders.filter((o) => o.status === OrderStatus.Placed).length;
  const acceptedOrders = orders.filter((o) => o.status === OrderStatus.Accepted).length;
  const completedOrders = orders.filter((o) => o.status === OrderStatus.Completed).length;

  const canInvite = canInviteStudioMembers(profile?.studioRole);

  return (
    <div>
      <PageHeader
        title="Studio Dashboard"
        description={
          <div className="flex items-center flex-wrap gap-4">
            <span>Manage your photography studio and team</span>
            {profile?.studioRole !== undefined && profile?.studioRole !== null && (
              <div className="flex items-center gap-2">
                <Shield className="h-4 w-4 text-muted-foreground" />
                <span className="text-sm">Your role:</span>
                <StudioRoleBadge role={profile.studioRole} />
              </div>
            )}
          </div>
        }
      />

      <Tabs defaultValue="overview" className="space-y-6">
        <TabsList>
          <TabsTrigger value="overview">Overview</TabsTrigger>
          <TabsTrigger value="team">
            Team ({members?.length || 0})
          </TabsTrigger>
          <TabsTrigger value="invitations">
            Invitations ({invitations?.filter((i) => i.status === InvitationStatus.Pending).length || 0})
          </TabsTrigger>
        </TabsList>

        {/* Overview Tab */}
        <TabsContent value="overview" className="space-y-6">
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
            <StatsCard
              title="Pending Orders"
              value={pendingOrders}
              icon={Clock}
              description="Awaiting acceptance"
            />
            <StatsCard
              title="Active Orders"
              value={acceptedOrders}
              icon={Camera}
              description="In progress"
            />
            <StatsCard
              title="Completed"
              value={completedOrders}
              icon={CheckCircle}
              description="This month"
            />
            <StatsCard
              title="Team Members"
              value={members?.length || 0}
              icon={Users}
              description="Active staff"
            />
          </div>

          <Card className="p-6">
            <h3 className="text-lg font-semibold mb-4">Quick Actions</h3>
            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
              <Button variant="outline" className="h-24 flex flex-col gap-2" asChild>
                <a href="/dashboard/orders/marketplace">
                  <Camera className="h-6 w-6" />
                  <span>Browse Orders</span>
                </a>
              </Button>
              <Button variant="outline" className="h-24 flex flex-col gap-2" asChild>
                <a href="/dashboard/orders">
                  <Calendar className="h-6 w-6" />
                  <span>My Orders</span>
                </a>
              </Button>
              {canInvite && (
                <Button
                  variant="outline"
                  className="h-24 flex flex-col gap-2"
                  onClick={() => setInviteDialogOpen(true)}
                >
                  <UserPlus className="h-6 w-6" />
                  <span>Invite Staff</span>
                </Button>
              )}
            </div>
          </Card>
        </TabsContent>

        {/* Team Tab */}
        <TabsContent value="team" className="space-y-6">
          <Card className="p-6">
            <div className="flex items-center justify-between mb-6">
              <div>
                <h3 className="text-lg font-semibold">Team Members</h3>
                <p className="text-sm text-muted-foreground">
                  Manage your studio staff and their roles
                </p>
              </div>
              {canInvite && (
                <Button onClick={() => setInviteDialogOpen(true)}>
                  <UserPlus className="mr-2 h-4 w-4" />
                  Invite Staff
                </Button>
              )}
            </div>

            {members && members.length > 0 ? (
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Member</TableHead>
                    <TableHead>Email</TableHead>
                    <TableHead>Role</TableHead>
                    <TableHead>Skills</TableHead>
                    <TableHead>Joined</TableHead>
                    <TableHead className="text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {members.map((member) => (
                    <TableRow key={member.id}>
                      <TableCell>
                        <div className="flex items-center gap-3">
                          <Avatar>
                            <AvatarFallback>
                              {member.displayName.substring(0, 2).toUpperCase()}
                            </AvatarFallback>
                          </Avatar>
                          <span className="font-medium">{member.displayName}</span>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2 text-gray-600">
                          <Mail className="h-4 w-4" />
                          {member.email}
                        </div>
                      </TableCell>
                      <TableCell>
                        <StudioRoleBadge role={member.role} />
                      </TableCell>
                      <TableCell>
                        {member.skills && member.skills.length > 0 ? (
                          <div className="flex flex-wrap gap-1">
                            {member.skills.slice(0, 3).map((skill, idx) => (
                              <Badge key={idx} variant="outline" className="text-xs">
                                {skill}
                              </Badge>
                            ))}
                            {member.skills.length > 3 && (
                              <Badge variant="outline" className="text-xs">
                                +{member.skills.length - 3}
                              </Badge>
                            )}
                          </div>
                        ) : (
                          <span className="text-gray-400">No skills</span>
                        )}
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2 text-sm text-gray-600">
                          <Calendar className="h-4 w-4" />
                          {new Date(member.joinedAtUtc).toLocaleDateString()}
                        </div>
                      </TableCell>
                      <TableCell className="text-right">
                        {canManageStudioMember(profile?.studioRole, member.role) && (
                          <DropdownMenu>
                            <DropdownMenuTrigger asChild>
                              <Button variant="ghost" size="sm">
                                <MoreVertical className="h-4 w-4" />
                              </Button>
                            </DropdownMenuTrigger>
                            <DropdownMenuContent align="end">
                              <DropdownMenuItem
                                onClick={() => {
                                  updateRole.mutate({
                                    studioId,
                                    memberId: member.id,
                                    data: { newRole: StudioRole.Manager },
                                  });
                                }}
                              >
                                Promote to Manager
                              </DropdownMenuItem>
                              <DropdownMenuItem
                                onClick={() => {
                                  updateRole.mutate({
                                    studioId,
                                    memberId: member.id,
                                    data: { newRole: StudioRole.Staff },
                                  });
                                }}
                              >
                                Change to Staff
                              </DropdownMenuItem>
                              <DropdownMenuItem
                                onClick={() => {
                                  setSelectedMember(member.id);
                                  setConfirmRemoveOpen(true);
                                }}
                                className="text-red-600"
                              >
                                Remove Member
                              </DropdownMenuItem>
                            </DropdownMenuContent>
                          </DropdownMenu>
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            ) : (
              <div className="text-center py-12 text-gray-500">
                <Users className="mx-auto h-12 w-12 mb-4 text-gray-400" />
                <p className="text-lg font-medium">No team members yet</p>
                <p className="text-sm mt-1">Invite your first team member to get started</p>
              </div>
            )}
          </Card>
        </TabsContent>

        {/* Invitations Tab */}
        <TabsContent value="invitations" className="space-y-6">
          <Card className="p-6">
            <div className="flex items-center justify-between mb-6">
              <div>
                <h3 className="text-lg font-semibold">Pending Invitations</h3>
                <p className="text-sm text-muted-foreground">
                  Manage pending staff invitations
                </p>
              </div>
            </div>

            {invitations && invitations.length > 0 ? (
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Email</TableHead>
                    <TableHead>Role</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Invited By</TableHead>
                    <TableHead>Sent</TableHead>
                    <TableHead>Expires</TableHead>
                    <TableHead className="text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {invitations.map((invitation) => (
                    <TableRow key={invitation.id}>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <Mail className="h-4 w-4 text-gray-400" />
                          {invitation.inviteeEmail}
                        </div>
                      </TableCell>
                      <TableCell>{invitation.targetRoleName}</TableCell>
                      <TableCell>
                        <InvitationStatusBadge status={invitation.status} />
                      </TableCell>
                      <TableCell>{invitation.invitedByName}</TableCell>
                      <TableCell>
                        {new Date(invitation.createdAtUtc).toLocaleDateString()}
                      </TableCell>
                      <TableCell>
                        <span className={invitation.isExpired ? "text-red-600" : ""}>
                          {new Date(invitation.expiresAtUtc).toLocaleDateString()}
                        </span>
                      </TableCell>
                      <TableCell className="text-right">
                        {invitation.status === InvitationStatus.Pending && (
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => {
                              setSelectedInvitation(invitation.id);
                              setConfirmCancelOpen(true);
                            }}
                          >
                            Cancel
                          </Button>
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            ) : (
              <div className="text-center py-12 text-gray-500">
                <Mail className="mx-auto h-12 w-12 mb-4 text-gray-400" />
                <p className="text-lg font-medium">No invitations</p>
                <p className="text-sm mt-1">Pending invitations will appear here</p>
              </div>
            )}
          </Card>
        </TabsContent>
      </Tabs>

      {/* Dialogs */}
      <InviteStaffDialog
        studioId={studioId}
        open={inviteDialogOpen}
        onOpenChange={setInviteDialogOpen}
      />

      <ConfirmDialog
        open={confirmRemoveOpen}
        onOpenChange={setConfirmRemoveOpen}
        title="Remove Team Member"
        description="Are you sure you want to remove this member? They will lose access to the studio."
        onConfirm={handleRemoveMember}
        confirmText="Remove"
        variant="destructive"
      />

      <ConfirmDialog
        open={confirmCancelOpen}
        onOpenChange={setConfirmCancelOpen}
        title="Cancel Invitation"
        description="Are you sure you want to cancel this invitation?"
        onConfirm={handleCancelInvitation}
        confirmText="Cancel Invitation"
      />
    </div>
  );
}
