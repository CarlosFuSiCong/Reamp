"use client";

import { use, useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { User, Lock, FileText, Mail, Briefcase, UserCircle, Shield, Settings } from "lucide-react";
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
      {/* Modern Header with Profile Info */}
      <div className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-purple-600 via-purple-700 to-indigo-700 p-8 shadow-xl">
        <div className="absolute inset-0 bg-grid-white/10 [mask-image:linear-gradient(0deg,transparent,rgba(255,255,255,0.2))]" />
        <div className="absolute -right-10 -top-10 h-40 w-40 rounded-full bg-white/10 blur-3xl" />
        <div className="absolute -bottom-10 -left-10 h-40 w-40 rounded-full bg-white/10 blur-3xl" />
        
        <div className="relative flex flex-col md:flex-row md:items-center gap-6">
          <div className="flex items-center gap-6">
            <div className="h-24 w-24 rounded-full bg-gradient-to-br from-purple-600 to-pink-700 flex items-center justify-center text-white shadow-2xl">
              <UserCircle className="h-16 w-16" />
            </div>
            <div className="text-white space-y-2">
              <h1 className="text-3xl font-bold tracking-tight">
                {profile.displayName || profile.firstName || "User"}
              </h1>
              <p className="text-purple-100 text-sm">{user?.email}</p>
              <div className="flex items-center gap-2">
                <Badge variant="secondary" className="bg-white/20 text-white border-white/30 hover:bg-white/30">
                  <Shield className="h-3 w-3 mr-1" />
                  {getUserRoleLabel(profile.role)}
                </Badge>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Tabs Section */}
      <Card className="border-0 shadow-lg">
        <CardContent className="p-6">
          <Tabs defaultValue={defaultTab} className="space-y-6">
            <TabsList className="grid w-full grid-cols-2 lg:grid-cols-5 h-auto p-1 bg-gray-100">
              <TabsTrigger 
                value="profile" 
                className="data-[state=active]:bg-white data-[state=active]:shadow-sm py-3 px-4"
              >
                <User className="h-4 w-4 mr-2" />
                <span className="hidden sm:inline">Profile</span>
                <span className="sm:hidden">Profile</span>
              </TabsTrigger>
              <TabsTrigger 
                value="security"
                className="data-[state=active]:bg-white data-[state=active]:shadow-sm py-3 px-4"
              >
                <Lock className="h-4 w-4 mr-2" />
                <span className="hidden sm:inline">Security</span>
                <span className="sm:hidden">Security</span>
              </TabsTrigger>
              {isStaffUser && staffData && (
                <TabsTrigger 
                  value="skills"
                  className="data-[state=active]:bg-white data-[state=active]:shadow-sm py-3 px-4"
                >
                  <Briefcase className="h-4 w-4 mr-2" />
                  <span className="hidden sm:inline">Skills</span>
                  <span className="sm:hidden">Skills</span>
                </TabsTrigger>
              )}
              <TabsTrigger 
                value="invitations"
                className="data-[state=active]:bg-white data-[state=active]:shadow-sm py-3 px-4"
              >
                <Mail className="h-4 w-4 mr-2" />
                <span className="hidden sm:inline">Invitations</span>
                <span className="sm:hidden">Invites</span>
              </TabsTrigger>
              <TabsTrigger 
                value="applications"
                className="data-[state=active]:bg-white data-[state=active]:shadow-sm py-3 px-4"
              >
                <FileText className="h-4 w-4 mr-2" />
                <span className="hidden sm:inline">Applications</span>
                <span className="sm:hidden">Apps</span>
              </TabsTrigger>
            </TabsList>

            {/* Profile Tab */}
            <TabsContent value="profile" className="space-y-6 mt-6">
              <Card className="border-0 shadow-md bg-gradient-to-br from-blue-50 to-indigo-50">
                <CardContent className="p-6">
                  <div className="flex items-center gap-3 mb-6">
                    <div className="h-10 w-10 rounded-xl bg-gradient-to-br from-blue-600 to-indigo-700 flex items-center justify-center text-white">
                      <User className="h-5 w-5" />
                    </div>
                    <div>
                      <h3 className="font-semibold text-lg text-gray-900">Profile Information</h3>
                      <p className="text-sm text-gray-600">Update your personal details and avatar</p>
                    </div>
                  </div>
                  
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
                </CardContent>
              </Card>
            </TabsContent>

            {/* Security Tab */}
            <TabsContent value="security" className="mt-6">
              <Card className="border-0 shadow-md bg-gradient-to-br from-red-50 to-orange-50">
                <CardContent className="p-6">
                  <div className="flex items-center gap-3 mb-6">
                    <div className="h-10 w-10 rounded-xl bg-gradient-to-br from-red-600 to-orange-700 flex items-center justify-center text-white">
                      <Lock className="h-5 w-5" />
                    </div>
                    <div>
                      <h3 className="font-semibold text-lg text-gray-900">Security Settings</h3>
                      <p className="text-sm text-gray-600">Manage your password and security preferences</p>
                    </div>
                  </div>
                  
                  <ChangePasswordForm
                    onSubmit={(data) => changePasswordMutation.mutate(data)}
                    isSubmitting={changePasswordMutation.isPending}
                    isSuccess={changePasswordMutation.isSuccess}
                  />
                </CardContent>
              </Card>
            </TabsContent>

            {/* Skills Tab */}
            {isStaffUser && staffData && (
              <TabsContent value="skills" className="mt-6">
                <Card className="border-0 shadow-md bg-gradient-to-br from-purple-50 to-pink-50">
                  <CardContent className="p-6">
                    <div className="flex items-center gap-3 mb-6">
                      <div className="h-10 w-10 rounded-xl bg-gradient-to-br from-purple-600 to-pink-700 flex items-center justify-center text-white">
                        <Briefcase className="h-5 w-5" />
                      </div>
                      <div>
                        <h3 className="font-semibold text-lg text-gray-900">Professional Skills</h3>
                        <p className="text-sm text-gray-600">Manage your photography and videography skills</p>
                      </div>
                    </div>
                    
                    <StaffSkillsManager staffId={staffData.id} currentSkills={staffData.skills} />
                  </CardContent>
                </Card>
              </TabsContent>
            )}

            {/* Invitations Tab */}
            <TabsContent value="invitations" className="mt-6">
              <Card className="border-0 shadow-md bg-gradient-to-br from-green-50 to-emerald-50">
                <CardContent className="p-6">
                  <div className="flex items-center gap-3 mb-6">
                    <div className="h-10 w-10 rounded-xl bg-gradient-to-br from-green-600 to-emerald-700 flex items-center justify-center text-white">
                      <Mail className="h-5 w-5" />
                    </div>
                    <div>
                      <h3 className="font-semibold text-lg text-gray-900">Invitations</h3>
                      <p className="text-sm text-gray-600">View and manage your pending invitations</p>
                    </div>
                  </div>
                  
                  <MyInvitations />
                </CardContent>
              </Card>
            </TabsContent>

            {/* Applications Tab */}
            <TabsContent value="applications" className="mt-6">
              <Card className="border-0 shadow-md bg-gradient-to-br from-yellow-50 to-amber-50">
                <CardContent className="p-6">
                  <div className="flex items-center gap-3 mb-6">
                    <div className="h-10 w-10 rounded-xl bg-gradient-to-br from-yellow-600 to-amber-700 flex items-center justify-center text-white">
                      <FileText className="h-5 w-5" />
                    </div>
                    <div>
                      <h3 className="font-semibold text-lg text-gray-900">Applications</h3>
                      <p className="text-sm text-gray-600">Track your agency and studio applications</p>
                    </div>
                  </div>
                  
                  <MyApplications />
                </CardContent>
              </Card>
            </TabsContent>
          </Tabs>
        </CardContent>
      </Card>
    </div>
  );
}
