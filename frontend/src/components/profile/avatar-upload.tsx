"use client";

import { useState, useEffect } from "react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Loader2, Upload } from "lucide-react";
import { mediaApi } from "@/lib/api";
import { toast } from "sonner";

interface AvatarUploadProps {
  avatarAssetId?: string;  // Changed from avatarUrl to avatarAssetId
  displayName?: string;
  onUpload: (assetId: string) => void;
  isUploading?: boolean;
}

export function AvatarUpload({ 
  avatarAssetId, 
  displayName, 
  onUpload, 
  isUploading 
}: AvatarUploadProps) {
  const [file, setFile] = useState<File | null>(null);
  const [preview, setPreview] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState(0);
  const [avatarUrl, setAvatarUrl] = useState<string>("");

  // Fetch avatar URL when avatarAssetId changes
  useEffect(() => {
    if (avatarAssetId) {
      mediaApi.getUrl(avatarAssetId)
        .then(url => setAvatarUrl(url))
        .catch(err => {
          console.error("Failed to load avatar:", err);
          setAvatarUrl("");
        });
    } else {
      setAvatarUrl("");
    }
  }, [avatarAssetId]);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = e.target.files?.[0];
    if (selectedFile) {
      setFile(selectedFile);
      const reader = new FileReader();
      reader.onloadend = () => setPreview(reader.result as string);
      reader.readAsDataURL(selectedFile);
    }
  };

  const handleUpload = async () => {
    if (!file) return;
    
    setUploading(true);
    setUploadProgress(0);
    
    try {
      // First upload the file to get an assetId and publicUrl
      const response = await mediaApi.upload(file, setUploadProgress);
      
      // Then trigger the profile update with the assetId
      // Don't clear state yet - wait for the profile update to complete
      onUpload(response.id);
      
      // State will be cleared when isUploading becomes false (via useEffect)
    } catch (error: any) {
      toast.error(error?.message || "Failed to upload avatar");
      // Only clear state on error, since the upload failed
      setFile(null);
      setPreview(null);
      setUploadProgress(0);
      setUploading(false);
    }
    // Note: uploading state is NOT set to false here - we keep it true
    // until the entire process (media + profile update) completes
  };

  // Clear state when the entire upload process (media + profile update) completes
  useEffect(() => {
    // When profile update finishes (success or error), clear the UI state
    // We check !isUploading to detect when the mutation completes
    // We check uploading to ensure we only clean up after our own upload
    if (uploading && !isUploading) {
      // Profile update completed, now we can clear the UI state
      // Note: Don't show toast here - the mutation hook already handles notifications
      setFile(null);
      setPreview(null);
      setUploadProgress(0);
      setUploading(false);
    }
  }, [isUploading, uploading]);
  // Note: removed 'file' from dependencies to avoid re-triggering after state cleanup

  const isProcessing = uploading || isUploading;

  return (
    <Card>
      <CardHeader>
        <CardTitle>Avatar</CardTitle>
        <CardDescription>Update your profile picture</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="flex items-center gap-4">
          <Avatar className="h-24 w-24">
            <AvatarImage src={preview || avatarUrl || ""} />
            <AvatarFallback>
              {displayName?.charAt(0).toUpperCase() || "U"}
            </AvatarFallback>
          </Avatar>
          <div className="space-y-2">
            <Input
              type="file"
              accept="image/*"
              onChange={handleFileChange}
              disabled={isProcessing}
              className="w-full max-w-xs"
            />
            {file && (
              <div className="space-y-2">
                <Button onClick={handleUpload} disabled={isProcessing} size="sm">
                  {isProcessing ? (
                    <Loader2 className="h-4 w-4 animate-spin mr-2" />
                  ) : (
                    <Upload className="h-4 w-4 mr-2" />
                  )}
                  Upload
                </Button>
                {uploading && uploadProgress > 0 && (
                  <div className="text-xs text-muted-foreground">
                    Uploading: {uploadProgress}%
                  </div>
                )}
              </div>
            )}
          </div>
        </div>
      </CardContent>
    </Card>
  );
}

