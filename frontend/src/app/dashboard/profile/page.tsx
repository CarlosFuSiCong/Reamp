"use client";

import { use, useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { User, Lock, FileText, Mail, Briefcase, UserCircle2 } from "lucide-react";
import { profilesApi } from "@/lib/api/profiles";
import { useAuthStore } from "@/lib/stores/auth-store";
import { LoadingState, ErrorState } from "@/components/shared";
import {
  AvatarUpload,
  ProfileInfoForm,
  ChangePasswordForm,
  MyInvitations,
  StaffSkillsManager,
} from "@/components/profile";
import { MyApplications } from "@/components/applications";
import { useUpdateProfile, useUpdateAvatar, useChangePassword } from "@/lib/hooks/use-profile";
import { useStaffByUserProfileId } from "@/lib/hooks/use-staff";
import { UserRole } from "@/types/enums";
import { getUserRoleLabel } from "@/lib/utils/enum-labels";

export default function ProfilePage({ searchParams }: { searchParams: Promise<{ tab?: string }> }) {
  const { user } = useAuthStore();
  const resolvedSearchParams = use(searchParams);
  const defaultTab = resolvedSearchParams?.tab || "profile";

  const {
    data: profile,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["profile"],
    queryFn: profilesApi.getMe,
  });

  const [profileFormData, setProfileFormData] = useState({
    firstName: "",
    lastName: "",
    displayName: "",
  });

  useEffect(() => {
    if (profile) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setProfileFormData({
        firstName: profile.firstName || "",
        lastName: profile.lastName || "",
        displayName: profile.displayName || "",
      });
    }
  }, [profile]);

  const updateProfileMutation = useUpdateProfile();
  const updateAvatarMutation = useUpdateAvatar();
  const changePasswordMutation = useChangePassword();

  // Fetch staff data if user is a staff member
  const isStaffUser = profile?.role === UserRole.Staff;
  const { data: staffData } = useStaffByUserProfileId(isStaffUser ? profile?.id || null : null);

  if (isLoading) {
    return <LoadingState message="Loading profile..." />;
  }

  if (error || !profile) {
    return <ErrorState message="Failed to load profile data" />;
  }

  return (
    <div className="space-y-6">
      {/* Header with Icon */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <div className="flex items-center gap-3 mb-2">
            <div className="h-10 w-10 rounded-xl bg-blue-600 flex items-center justify-center text-white shadow-md">
              <UserCircle2 className="h-5 w-5" />
            </div>
            <h1 className="text-3xl font-bold tracking-tight text-gray-900">Profile Settings</h1>
          </div>
          <div className="flex items-center gap-3 text-sm text-gray-600 ml-[52px]">
            <span>{profile.displayName || profile.firstName || "User"}</span>
            <span>•</span>
            <span>{user?.email}</span>
            <span>•</span>
            <Badge variant="outline" className="text-xs">
              {getUserRoleLabel(profile.role)}
            </Badge>
          </div>
        </div>
      </div>

      {/* Tabs Section */}
      <Card className="shadow-lg">
        <CardContent className="p-6">
          <Tabs defaultValue={defaultTab} className="space-y-6">
            <TabsList className="w-full justify-start h-auto p-1 bg-gray-100">
              <TabsTrigger 
                value="profile" 
                className="data-[state=active]:bg-white data-[state=active]:shadow-sm"
              >
                <User className="h-4 w-4 mr-2" />
                Profile
              </TabsTrigger>
              <TabsTrigger 
                value="security"
                className="data-[state=active]:bg-white data-[state=active]:shadow-sm"
              >
                <Lock className="h-4 w-4 mr-2" />
                Security
              </TabsTrigger>
              {isStaffUser && staffData && (
                <TabsTrigger 
                  value="skills"
                  className="data-[state=active]:bg-white data-[state=active]:shadow-sm"
                >
                  <Briefcase className="h-4 w-4 mr-2" />
                  Skills
                </TabsTrigger>
              )}
              <TabsTrigger 
                value="invitations"
                className="data-[state=active]:bg-white data-[state=active]:shadow-sm"
              >
                <Mail className="h-4 w-4 mr-2" />
                Invitations
              </TabsTrigger>
              <TabsTrigger 
                value="applications"
                className="data-[state=active]:bg-white data-[state=active]:shadow-sm"
              >
                <FileText className="h-4 w-4 mr-2" />
                Applications
              </TabsTrigger>
            </TabsList>

            {/* Profile Tab */}
            <TabsContent value="profile" className="space-y-6 mt-6">
              <div className="space-y-6">
                <AvatarUpload
                  avatarAssetId={profile.avatarAssetId}
                  displayName={profile.displayName}
                  onUpload={(assetId) => updateAvatarMutation.mutate({ profileId: profile.id, assetId })}
                  isUploading={updateAvatarMutation.isPending}
                />

                <ProfileInfoForm
                  initialData={profileFormData}
                  email={user?.email || ""}
                  role={profile.role}
                  onSubmit={(data) =>
                    updateProfileMutation.mutate({
                      profileId: profile.id,
                      data: {
                        firstName: data.firstName,
                        lastName: data.lastName,
                      },
                    })
                  }
                  isSubmitting={updateProfileMutation.isPending}
                  isSuccess={updateProfileMutation.isSuccess}
                />
              </div>
            </TabsContent>

            {/* Security Tab */}
            <TabsContent value="security" className="mt-6">
              <ChangePasswordForm
                onSubmit={(data) => changePasswordMutation.mutate(data)}
                isSubmitting={changePasswordMutation.isPending}
                isSuccess={changePasswordMutation.isSuccess}
              />
            </TabsContent>

            {/* Skills Tab */}
            {isStaffUser && staffData && (
              <TabsContent value="skills" className="mt-6">
                <StaffSkillsManager staffId={staffData.id} currentSkills={staffData.skills} />
              </TabsContent>
            )}

            {/* Invitations Tab */}
            <TabsContent value="invitations" className="mt-6">
              <MyInvitations />
            </TabsContent>

            {/* Applications Tab */}
            <TabsContent value="applications" className="mt-6">
              <MyApplications />
            </TabsContent>
          </Tabs>
        </CardContent>
      </Card>
    </div>
  );
}
