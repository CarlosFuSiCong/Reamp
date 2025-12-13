"use client";

import { use, useState } from "react";
import { Users, Mail, Calendar, MoreVertical, UserPlus, Shield } from "lucide-react";
import { PageHeader, LoadingState, ErrorState } from "@/components/shared";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
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
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
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
import { StudioRole, InvitationStatus } from "@/types";

export default function StudioTeamPage({ params }: { params: Promise<{ studioId?: string }> }) {
  const resolvedParams = use(params);
  const studioId = resolvedParams?.studioId || "";
  const { user: profile } = useProfile();
  
  const [inviteDialogOpen, setInviteDialogOpen] = useState(false);
  const [selectedMember, setSelectedMember] = useState<string | null>(null);
  const [confirmRemoveOpen, setConfirmRemoveOpen] = useState(false);
  const [selectedInvitation, setSelectedInvitation] = useState<string | null>(null);
  const [confirmCancelOpen, setConfirmCancelOpen] = useState(false);

  const { data: members, isLoading: membersLoading, error: membersError } =
    useStudioMembers(studioId);
  const { data: invitations, isLoading: invitationsLoading } =
    useStudioInvitations(studioId);

  const updateRole = useUpdateStudioMemberRole();
  const removeMember = useRemoveStudioMember();
  const cancelInvitation = useCancelInvitation();

  const getRoleName = (role: StudioRole | undefined | null): string => {
    if (role === undefined || role === null) return "Member";
    
    const roleValue = typeof role === 'string' ? parseInt(role, 10) : Number(role);
    
    switch (roleValue) {
      case 0: return "Member";
      case 1: return "Editor";
      case 2: return "Photographer";
      case 3: return "Manager";
      case 4: return "Owner";
      default: return "Unknown";
    }
  };

  const getRoleBadge = (role: StudioRole | undefined | null) => {
    if (role === undefined || role === null) {
      return <Badge className="bg-gray-50 text-gray-700 border-gray-200">Member</Badge>;
    }
    
    const roleValue = typeof role === 'string' ? parseInt(role, 10) : Number(role);
    const roleName = getRoleName(role);
    
    switch (roleValue) {
      case 4: // Owner
        return <Badge className="bg-purple-50 text-purple-700 border-purple-200">{roleName}</Badge>;
      case 3: // Manager
        return <Badge className="bg-blue-50 text-blue-700 border-blue-200">{roleName}</Badge>;
      case 2: // Photographer
        return <Badge className="bg-green-50 text-green-700 border-green-200">{roleName}</Badge>;
      case 1: // Editor
        return <Badge className="bg-orange-50 text-orange-700 border-orange-200">{roleName}</Badge>;
      case 0: // Member
        return <Badge className="bg-gray-50 text-gray-700 border-gray-200">{roleName}</Badge>;
      default:
        return <Badge>{roleName}</Badge>;
    }
  };

  const getStatusBadge = (status: InvitationStatus) => {
    switch (status) {
      case InvitationStatus.Pending:
        return <Badge className="bg-yellow-50 text-yellow-700 border-yellow-200">Pending</Badge>;
      case InvitationStatus.Accepted:
        return <Badge className="bg-green-50 text-green-700 border-green-200">Accepted</Badge>;
      case InvitationStatus.Rejected:
        return <Badge className="bg-red-50 text-red-700 border-red-200">Rejected</Badge>;
      case InvitationStatus.Cancelled:
        return <Badge className="bg-gray-50 text-gray-700 border-gray-200">Cancelled</Badge>;
      case InvitationStatus.Expired:
        return <Badge className="bg-gray-50 text-gray-700 border-gray-200">Expired</Badge>;
      default:
        return <Badge>{status}</Badge>;
    }
  };

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

  if (membersLoading) {
    return <LoadingState message="Loading team members..." />;
  }

  if (membersError) {
    return <ErrorState message="Failed to load team members" />;
  }

  return (
    <div>
      <PageHeader
        title="Team Management"
        description={
          <div className="flex items-center gap-4">
            <span>Manage your studio team members and invitations</span>
            {profile?.studioRole !== undefined && profile?.studioRole !== null && (
              <div className="flex items-center gap-2">
                <Shield className="h-4 w-4 text-muted-foreground" />
                <span className="text-sm">Your role:</span>
                {getRoleBadge(profile.studioRole)}
                {[StudioRole.Owner, StudioRole.Manager].includes(profile.studioRole) && (
                  <Button 
                    onClick={() => setInviteDialogOpen(true)}
                    size="sm"
                    className="ml-2"
                  >
                    <UserPlus className="mr-2 h-4 w-4" />
                    Invite Staff
                  </Button>
                )}
              </div>
            )}
          </div>
        }
      />

      <Card className="p-6">
        <Tabs defaultValue="members">
          <TabsList>
            <TabsTrigger value="members">
              Members ({members?.length || 0})
            </TabsTrigger>
            <TabsTrigger value="invitations">
              Invitations ({invitations?.filter(i => i.status === InvitationStatus.Pending).length || 0})
            </TabsTrigger>
          </TabsList>

          <TabsContent value="members" className="mt-6">
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
                      <TableCell>{getRoleBadge(member.role)}</TableCell>
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
                              disabled={member.role === StudioRole.Owner}
                            >
                              Promote to Manager
                            </DropdownMenuItem>
                            <DropdownMenuItem
                              onClick={() => {
                                updateRole.mutate({
                                  studioId,
                                  memberId: member.id,
                                  data: { newRole: StudioRole.Member },
                                });
                              }}
                              disabled={member.role === StudioRole.Owner}
                            >
                              Change to Member
                            </DropdownMenuItem>
                            <DropdownMenuItem
                              onClick={() => {
                                setSelectedMember(member.id);
                                setConfirmRemoveOpen(true);
                              }}
                              disabled={member.role === StudioRole.Owner}
                              className="text-red-600"
                            >
                              Remove Member
                            </DropdownMenuItem>
                          </DropdownMenuContent>
                        </DropdownMenu>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            ) : (
              <div className="text-center py-12 text-gray-500">
                <Users className="mx-auto h-12 w-12 mb-4 text-gray-400" />
                <p className="text-lg font-medium">No team members yet</p>
                <p className="text-sm mt-1">
                  Invite your first team member to get started
                </p>
              </div>
            )}
          </TabsContent>

          <TabsContent value="invitations" className="mt-6">
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
                      <TableCell>{getStatusBadge(invitation.status)}</TableCell>
                      <TableCell>{invitation.invitedByName}</TableCell>
                      <TableCell>
                        {new Date(invitation.createdAtUtc).toLocaleDateString()}
                      </TableCell>
                      <TableCell>
                        <span
                          className={invitation.isExpired ? "text-red-600" : ""}
                        >
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
                <p className="text-sm mt-1">
                  Pending invitations will appear here
                </p>
              </div>
            )}
          </TabsContent>
        </Tabs>
      </Card>

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
