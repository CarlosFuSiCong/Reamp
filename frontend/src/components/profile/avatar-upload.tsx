"use client";

import { useState } from "react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Loader2, Upload } from "lucide-react";

interface AvatarUploadProps {
  avatarUrl?: string;
  displayName?: string;
  onUpload: (file: File) => void;
  isUploading?: boolean;
}

export function AvatarUpload({ 
  avatarUrl, 
  displayName, 
  onUpload, 
  isUploading 
}: AvatarUploadProps) {
  const [file, setFile] = useState<File | null>(null);
  const [preview, setPreview] = useState<string | null>(null);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = e.target.files?.[0];
    if (selectedFile) {
      setFile(selectedFile);
      const reader = new FileReader();
      reader.onloadend = () => setPreview(reader.result as string);
      reader.readAsDataURL(selectedFile);
    }
  };

  const handleUpload = () => {
    if (file) {
      onUpload(file);
      setFile(null);
      setPreview(null);
    }
  };

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
              className="w-full max-w-xs"
            />
            {file && (
              <Button onClick={handleUpload} disabled={isUploading} size="sm">
                {isUploading ? (
                  <Loader2 className="h-4 w-4 animate-spin mr-2" />
                ) : (
                  <Upload className="h-4 w-4 mr-2" />
                )}
                Upload
              </Button>
            )}
          </div>
        </div>
      </CardContent>
    </Card>
  );
}

