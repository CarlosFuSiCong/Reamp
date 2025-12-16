"use client";

import { useState, useRef } from "react";
import { Upload, X, Image as ImageIcon, Film, FileIcon, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";
import { mediaApi } from "@/lib/api";
import type { MediaAssetDetailDto } from "@/types";
import { toast } from "sonner";

const CHUNK_SIZE = 5 * 1024 * 1024; // 5MB per chunk

interface PendingFile {
  id: string;
  file: File;
  preview?: string;
  status: "pending" | "uploading" | "completed" | "error";
  progress: number;
  error?: string;
  asset?: MediaAssetDetailDto;
}

interface DeliveryMediaUploaderProps {
  ownerStudioId: string;
  onUploadComplete?: (assets: MediaAssetDetailDto[]) => void;
  accept?: string;
  maxFiles?: number;
  maxSizeMB?: number;
}

export function DeliveryMediaUploader({
  ownerStudioId,
  onUploadComplete,
  accept = "image/*,video/*",
  maxFiles = 100,
  maxSizeMB = 500,
}: DeliveryMediaUploaderProps) {
  const [pendingFiles, setPendingFiles] = useState<PendingFile[]>([]);
  const [isUploading, setIsUploading] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const generateId = () => Math.random().toString(36).substring(7);

  const formatFileSize = (bytes: number): string => {
    if (bytes < 1024) return bytes + " B";
    if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(2) + " KB";
    return (bytes / (1024 * 1024)).toFixed(2) + " MB";
  };

  const getFileIcon = (file: File) => {
    if (file.type.startsWith("image/")) return ImageIcon;
    if (file.type.startsWith("video/")) return Film;
    return FileIcon;
  };

  const handleFileSelect = async (selectedFiles: FileList | null) => {
    if (!selectedFiles) return;

    const newFiles: PendingFile[] = [];
    const maxSizeBytes = maxSizeMB * 1024 * 1024;

    for (let i = 0; i < selectedFiles.length; i++) {
      const file = selectedFiles[i];

      // Check file size
      if (file.size > maxSizeBytes) {
        toast.error(`${file.name} exceeds maximum size of ${maxSizeMB}MB`);
        continue;
      }

      // Check total files limit
      if (pendingFiles.length + newFiles.length >= maxFiles) {
        toast.error(`Maximum ${maxFiles} files allowed`);
        break;
      }

      // Generate preview for images
      let preview: string | undefined;
      if (file.type.startsWith("image/")) {
        preview = URL.createObjectURL(file);
      }

      newFiles.push({
        id: generateId(),
        file,
        preview,
        status: "pending",
        progress: 0,
      });
    }

    setPendingFiles((prev) => [...prev, ...newFiles]);

    if (newFiles.length > 0) {
      toast.success(`${newFiles.length} file(s) added. Click "Upload All" to start uploading.`);
    }
  };

  const removeFile = (id: string) => {
    setPendingFiles((prev) => {
      const file = prev.find((f) => f.id === id);
      if (file?.preview) {
        URL.revokeObjectURL(file.preview);
      }
      return prev.filter((f) => f.id !== id);
    });
  };

  const uploadSingleFile = async (pendingFile: PendingFile): Promise<MediaAssetDetailDto> => {
    const { file } = pendingFile;

    // Step 1: Initiate upload session
    const totalChunks = Math.ceil(file.size / CHUNK_SIZE);
    const session = await mediaApi.initiateChunkedUpload({
      fileName: file.name,
      totalSize: file.size,
      contentType: file.type,
      totalChunks,
      ownerStudioId,
    });

    // Update progress
    setPendingFiles((prev) =>
      prev.map((f) => (f.id === pendingFile.id ? { ...f, status: "uploading", progress: 0 } : f))
    );

    // Step 2: Upload chunks
    for (let chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++) {
      const start = chunkIndex * CHUNK_SIZE;
      const end = Math.min(start + CHUNK_SIZE, file.size);
      const chunk = file.slice(start, end);

      await mediaApi.uploadChunk(session.id, chunkIndex, chunk);

      // Update progress
      const progress = Math.round(((chunkIndex + 1) / totalChunks) * 100);
      setPendingFiles((prev) =>
        prev.map((f) => (f.id === pendingFile.id ? { ...f, progress } : f))
      );
    }

    // Step 3: Complete upload
    const asset = await mediaApi.completeChunkedUpload(session.id);

    // Update status
    setPendingFiles((prev) =>
      prev.map((f) =>
        f.id === pendingFile.id ? { ...f, status: "completed", progress: 100, asset } : f
      )
    );

    return asset;
  };

  const handleUploadAll = async () => {
    const filesToUpload = pendingFiles.filter((f) => f.status === "pending");

    if (filesToUpload.length === 0) {
      toast.error("No files to upload");
      return;
    }

    setIsUploading(true);

    try {
      const uploadedAssets: MediaAssetDetailDto[] = [];

      for (const file of filesToUpload) {
        try {
          const asset = await uploadSingleFile(file);
          uploadedAssets.push(asset);
        } catch (error: any) {
          setPendingFiles((prev) =>
            prev.map((f) =>
              f.id === file.id
                ? { ...f, status: "error", error: error.message || "Upload failed" }
                : f
            )
          );
          toast.error(`Failed to upload ${file.file.name}`);
        }
      }

      if (uploadedAssets.length > 0) {
        toast.success(`${uploadedAssets.length} file(s) uploaded successfully`);
        onUploadComplete?.(uploadedAssets);
      }
    } catch (error: any) {
      toast.error("Upload failed: " + error.message);
    } finally {
      setIsUploading(false);
    }
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    handleFileSelect(e.dataTransfer.files);
  };

  const completedCount = pendingFiles.filter((f) => f.status === "completed").length;
  const errorCount = pendingFiles.filter((f) => f.status === "error").length;
  const pendingCount = pendingFiles.filter((f) => f.status === "pending").length;

  return (
    <div className="space-y-4">
      {/* Upload Area */}
      <div
        className="border-2 border-dashed rounded-lg p-8 text-center hover:border-primary/50 transition-colors cursor-pointer"
        onDrop={handleDrop}
        onDragOver={(e) => e.preventDefault()}
        onClick={() => fileInputRef.current?.click()}
      >
        <Upload className="mx-auto h-12 w-12 text-muted-foreground mb-4" />
        <p className="text-sm text-muted-foreground mb-2">
          Click to select files or drag and drop here
        </p>
        <p className="text-xs text-muted-foreground">
          Maximum {maxFiles} files, up to {maxSizeMB}MB each
        </p>
        <input
          ref={fileInputRef}
          type="file"
          multiple
          accept={accept}
          className="hidden"
          onChange={(e) => handleFileSelect(e.target.files)}
          disabled={isUploading}
        />
      </div>

      {/* File List */}
      {pendingFiles.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle>Selected Files ({pendingFiles.length})</CardTitle>
            <CardDescription>
              {pendingCount > 0 && `${pendingCount} pending • `}
              {completedCount > 0 && `${completedCount} completed • `}
              {errorCount > 0 && `${errorCount} failed`}
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-2 max-h-96 overflow-y-auto">
              {pendingFiles.map((file) => {
                const Icon = getFileIcon(file.file);
                return (
                  <div
                    key={file.id}
                    className="flex items-center gap-3 p-3 border rounded-lg bg-card"
                  >
                    {/* Preview or Icon */}
                    <div className="flex-shrink-0">
                      {file.preview ? (
                        <img
                          src={file.preview}
                          alt={file.file.name}
                          className="w-12 h-12 object-cover rounded"
                        />
                      ) : (
                        <div className="w-12 h-12 bg-muted rounded flex items-center justify-center">
                          <Icon className="w-6 h-6 text-muted-foreground" />
                        </div>
                      )}
                    </div>

                    {/* File Info */}
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium truncate">{file.file.name}</p>
                      <p className="text-xs text-muted-foreground">
                        {formatFileSize(file.file.size)}
                      </p>

                      {/* Progress Bar */}
                      {file.status === "uploading" && (
                        <Progress value={file.progress} className="mt-2 h-1" />
                      )}

                      {/* Error Message */}
                      {file.status === "error" && (
                        <p className="text-xs text-red-600 mt-1">{file.error}</p>
                      )}
                    </div>

                    {/* Status/Actions */}
                    <div className="flex-shrink-0">
                      {file.status === "pending" && (
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => removeFile(file.id)}
                          disabled={isUploading}
                        >
                          <X className="h-4 w-4" />
                        </Button>
                      )}
                      {file.status === "uploading" && (
                        <Loader2 className="h-4 w-4 animate-spin text-primary" />
                      )}
                      {file.status === "completed" && (
                        <div className="text-green-600">✓</div>
                      )}
                      {file.status === "error" && (
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => removeFile(file.id)}
                        >
                          <X className="h-4 w-4" />
                        </Button>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>

            {/* Upload Button */}
            {pendingCount > 0 && (
              <div className="mt-4 flex justify-end">
                <Button onClick={handleUploadAll} disabled={isUploading}>
                  {isUploading ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Uploading...
                    </>
                  ) : (
                    <>
                      <Upload className="mr-2 h-4 w-4" />
                      Upload All ({pendingCount})
                    </>
                  )}
                </Button>
              </div>
            )}
          </CardContent>
        </Card>
      )}
    </div>
  );
}
