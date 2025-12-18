"use client";

import { useState, useCallback, useRef } from "react";
import { Upload, X, CheckCircle, AlertCircle, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Progress } from "@/components/ui/progress";
import { mediaApi } from "@/lib/api";
import type { UploadProgressEvent, MediaAssetDetailDto } from "@/types";
import { toast } from "sonner";

const CHUNK_SIZE = 5 * 1024 * 1024; // 5MB per chunk

interface ChunkedUploadProps {
  ownerStudioId: string;
  onUploadComplete?: (asset: MediaAssetDetailDto) => void;
  onUploadError?: (error: string) => void;
  accept?: string;
  maxFiles?: number;
  maxSizeMB?: number;
}

interface FileUploadState {
  file: File;
  progress: number;
  status: "pending" | "uploading" | "completed" | "error" | "cancelled";
  sessionId?: string;
  uploadedChunks: number;
  totalChunks: number;
  error?: string;
  asset?: MediaAssetDetailDto;
}

export function ChunkedUpload({
  ownerStudioId,
  onUploadComplete,
  onUploadError,
  accept = "image/*,video/*",
  maxFiles = 10,
  maxSizeMB = 100,
}: ChunkedUploadProps) {
  const [files, setFiles] = useState<Map<string, FileUploadState>>(new Map());
  const [isDragging, setIsDragging] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const abortControllersRef = useRef<Map<string, AbortController>>(new Map());

  const calculateChunks = (fileSize: number): number => {
    return Math.ceil(fileSize / CHUNK_SIZE);
  };

  const uploadFile = useCallback(
    async (file: File) => {
      const fileId = `${file.name}-${file.lastModified}`;
      const totalChunks = calculateChunks(file.size);

      // Initialize file state
      setFiles((prev) => {
        const newMap = new Map(prev);
        newMap.set(fileId, {
          file,
          progress: 0,
          status: "pending",
          uploadedChunks: 0,
          totalChunks,
        });
        return newMap;
      });

      try {
        // Step 1: Initiate upload session
        const session = await mediaApi.initiateChunkedUpload({
          ownerStudioId,
          fileName: file.name,
          contentType: file.type,
          totalSize: file.size,
          totalChunks,
        });

        setFiles((prev) => {
          const newMap = new Map(prev);
          const fileState = newMap.get(fileId);
          if (fileState) {
            newMap.set(fileId, {
              ...fileState,
              sessionId: session.sessionId,
              status: "uploading",
            });
          }
          return newMap;
        });

        // Step 2: Upload chunks
        const abortController = new AbortController();
        abortControllersRef.current.set(fileId, abortController);

        for (let chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++) {
          // Check if cancelled
          if (abortController.signal.aborted) {
            throw new Error("Upload cancelled");
          }

          const start = chunkIndex * CHUNK_SIZE;
          const end = Math.min(start + CHUNK_SIZE, file.size);
          const chunk = file.slice(start, end);

          await mediaApi.uploadChunk(session.sessionId, chunkIndex, chunk);

          // Update progress
          const progress = ((chunkIndex + 1) / totalChunks) * 100;
          setFiles((prev) => {
            const newMap = new Map(prev);
            const fileState = newMap.get(fileId);
            if (fileState) {
              newMap.set(fileId, {
                ...fileState,
                progress,
                uploadedChunks: chunkIndex + 1,
              });
            }
            return newMap;
          });
        }

        // Step 3: Complete upload
        const asset = await mediaApi.completeChunkedUpload(session.sessionId);

        setFiles((prev) => {
          const newMap = new Map(prev);
          const fileState = newMap.get(fileId);
          if (fileState) {
            newMap.set(fileId, {
              ...fileState,
              progress: 100,
              status: "completed",
              asset,
            });
          }
          return newMap;
        });

        toast.success(`${file.name} uploaded successfully`);
        onUploadComplete?.(asset);
      } catch (error: any) {
        const errorMessage = error?.message || "Upload failed";
        
        setFiles((prev) => {
          const newMap = new Map(prev);
          const fileState = newMap.get(fileId);
          if (fileState) {
            newMap.set(fileId, {
              ...fileState,
              status: errorMessage.includes("cancelled") ? "cancelled" : "error",
              error: errorMessage,
            });
          }
          return newMap;
        });

        if (!errorMessage.includes("cancelled")) {
          toast.error(`Failed to upload ${file.name}: ${errorMessage}`);
          onUploadError?.(errorMessage);
        }

        // Cancel session if it was created
        const fileState = files.get(fileId);
        if (fileState?.sessionId) {
          try {
            await mediaApi.cancelUploadSession(fileState.sessionId);
          } catch {
            // Ignore cancellation errors
          }
        }
      } finally {
        abortControllersRef.current.delete(fileId);
      }
    },
    [ownerStudioId, files, onUploadComplete, onUploadError]
  );

  const handleFileSelect = useCallback(
    (selectedFiles: FileList | null) => {
      if (!selectedFiles) return;

      const fileArray = Array.from(selectedFiles);

      // Validate max files
      if (files.size + fileArray.length > maxFiles) {
        toast.error(`Maximum ${maxFiles} files allowed`);
        return;
      }

      // Validate file sizes
      const maxSizeBytes = maxSizeMB * 1024 * 1024;
      const invalidFiles = fileArray.filter((f) => f.size > maxSizeBytes);
      if (invalidFiles.length > 0) {
        toast.error(`Some files exceed ${maxSizeMB}MB limit`);
        return;
      }

      // Upload files
      fileArray.forEach((file) => uploadFile(file));
    },
    [files.size, maxFiles, maxSizeMB, uploadFile]
  );

  const handleDrop = useCallback(
    (e: React.DragEvent<HTMLDivElement>) => {
      e.preventDefault();
      setIsDragging(false);
      handleFileSelect(e.dataTransfer.files);
    },
    [handleFileSelect]
  );

  const handleDragOver = useCallback((e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    setIsDragging(true);
  }, []);

  const handleDragLeave = useCallback(() => {
    setIsDragging(false);
  }, []);

  const cancelUpload = useCallback((fileId: string) => {
    const abortController = abortControllersRef.current.get(fileId);
    if (abortController) {
      abortController.abort();
      toast.info("Upload cancelled");
    }
  }, []);

  const removeFile = useCallback((fileId: string) => {
    setFiles((prev) => {
      const newMap = new Map(prev);
      newMap.delete(fileId);
      return newMap;
    });
  }, []);

  const clearCompleted = useCallback(() => {
    setFiles((prev) => {
      const newMap = new Map(prev);
      Array.from(newMap.entries()).forEach(([fileId, fileState]) => {
        if (fileState.status === "completed" || fileState.status === "error") {
          newMap.delete(fileId);
        }
      });
      return newMap;
    });
  }, []);

  const fileArray = Array.from(files.entries());

  return (
    <div className="space-y-4">
      {/* Drop Zone */}
      <div
        onDrop={handleDrop}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        className={`
          border-2 border-dashed rounded-lg p-8 text-center cursor-pointer
          transition-colors duration-200
          ${isDragging ? "border-blue-500 bg-blue-50" : "border-gray-300 hover:border-gray-400"}
        `}
        onClick={() => fileInputRef.current?.click()}
      >
        <Upload className="mx-auto h-12 w-12 text-gray-400 mb-4" />
        <p className="text-sm text-gray-600 mb-2">
          <span className="font-semibold text-blue-600">Click to upload</span> or drag and drop
        </p>
        <p className="text-xs text-gray-500">
          Max {maxFiles} files, {maxSizeMB}MB each
        </p>
        <input
          ref={fileInputRef}
          type="file"
          multiple
          accept={accept}
          className="hidden"
          onChange={(e) => handleFileSelect(e.target.files)}
        />
      </div>

      {/* File List */}
      {fileArray.length > 0 && (
        <div className="space-y-2">
          <div className="flex items-center justify-between">
            <h3 className="text-sm font-medium">Uploads ({fileArray.length})</h3>
            {fileArray.some(([, f]) => f.status === "completed" || f.status === "error") && (
              <Button variant="ghost" size="sm" onClick={clearCompleted}>
                Clear completed
              </Button>
            )}
          </div>

          {fileArray.map(([fileId, fileState]) => (
            <Card key={fileId} className="p-4">
              <div className="space-y-2">
                <div className="flex items-start justify-between gap-2">
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-medium truncate">{fileState.file.name}</p>
                    <p className="text-xs text-muted-foreground">
                      {(fileState.file.size / (1024 * 1024)).toFixed(2)} MB
                    </p>
                  </div>

                  <div className="flex items-center gap-2">
                    {fileState.status === "uploading" && (
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => cancelUpload(fileId)}
                      >
                        <X className="h-4 w-4" />
                      </Button>
                    )}
                    {(fileState.status === "completed" || fileState.status === "error") && (
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => removeFile(fileId)}
                      >
                        <X className="h-4 w-4" />
                      </Button>
                    )}
                  </div>
                </div>

                {fileState.status === "uploading" && (
                  <div className="space-y-1">
                    <Progress value={fileState.progress} className="h-2" />
                    <p className="text-xs text-muted-foreground">
                      {fileState.uploadedChunks} / {fileState.totalChunks} chunks •{" "}
                      {Math.round(fileState.progress)}%
                    </p>
                  </div>
                )}

                {fileState.status === "completed" && (
                  <div className="flex items-center gap-2 text-green-600">
                    <CheckCircle className="h-4 w-4" />
                    <span className="text-sm">Upload complete</span>
                  </div>
                )}

                {fileState.status === "error" && (
                  <div className="flex items-center gap-2 text-red-600">
                    <AlertCircle className="h-4 w-4" />
                    <span className="text-sm">{fileState.error || "Upload failed"}</span>
                  </div>
                )}

                {fileState.status === "pending" && (
                  <div className="flex items-center gap-2 text-blue-600">
                    <Loader2 className="h-4 w-4 animate-spin" />
                    <span className="text-sm">Preparing...</span>
                  </div>
                )}
              </div>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
