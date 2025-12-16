"use client";

import { useState, useEffect, useRef } from "react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Loader2, Upload } from "lucide-react";
import { mediaApi } from "@/lib/api";
import { toast } from "sonner";

interface AvatarUploadProps {
  avatarAssetId?: string;
  displayName?: string;
  onUpload: (assetId: string) => void;
  isUploading?: boolean;
}

export function AvatarUpload({
  avatarAssetId,
  displayName,
  onUpload,
  isUploading,
}: AvatarUploadProps) {
  const [file, setFile] = useState<File | null>(null);
  const [preview, setPreview] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState(0);
  const [avatarUrl, setAvatarUrl] = useState<string>("");
  const prevIsUploadingRef = useRef(isUploading);

  useEffect(() => {
    if (avatarAssetId) {
      mediaApi
        .getUrl(avatarAssetId)
        .then((url) => setAvatarUrl(url))
        .catch((err) => {
          console.error("Failed to load avatar:", err);
          setAvatarUrl("");
        });
    } else {
      // eslint-disable-next-line react-hooks/set-state-in-effect
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
      const response = await mediaApi.upload(file, setUploadProgress);
      setUploading(false);
      onUpload(response.id);
    } catch (error: unknown) {
      toast.error(error?.message || "Failed to upload avatar");
      setFile(null);
      setPreview(null);
      setUploadProgress(0);
      setUploading(false);
    }
  };

  // Clear UI state when profile update completes (isUploading: true -> false)
  useEffect(() => {
    if (prevIsUploadingRef.current && !isUploading) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setFile(null);
      setPreview(null);
      setUploadProgress(0);
    }
    prevIsUploadingRef.current = isUploading;
  }, [isUploading]);

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
            <AvatarFallback>{displayName?.charAt(0).toUpperCase() || "U"}</AvatarFallback>
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
                  <div className="text-xs text-muted-foreground">Uploading: {uploadProgress}%</div>
                )}
              </div>
            )}
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
