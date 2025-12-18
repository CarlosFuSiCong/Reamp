"use client";

import { useEffect } from "react";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { AlertCircle, Home, RefreshCw } from "lucide-react";

export default function Error({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  useEffect(() => {
    // Log the error to an error reporting service
    console.error("Application error:", error);
  }, [error]);

  return (
    <div className="min-h-screen bg-gradient-to-br from-red-50 to-gray-100 flex items-center justify-center p-4">
      <Card className="w-full max-w-2xl shadow-xl border-red-200">
        <CardHeader className="text-center pb-4">
          <div className="mb-6">
            <div className="inline-flex items-center justify-center w-24 h-24 rounded-full bg-red-100 mb-4">
              <AlertCircle className="h-12 w-12 text-red-600" aria-hidden="true" />
            </div>
          </div>
          <CardTitle className="text-4xl font-bold text-gray-900 mb-2">
            Something Went Wrong
          </CardTitle>
          <CardDescription className="text-lg text-gray-600">
            We encountered an unexpected error while processing your request
          </CardDescription>
        </CardHeader>
        
        <CardContent className="space-y-6">
          {error.digest && (
            <div className="bg-gray-100 border border-gray-200 rounded-lg p-4 font-mono text-xs text-gray-700">
              <p className="font-semibold mb-1">Error ID:</p>
              <p className="break-all">{error.digest}</p>
            </div>
          )}

          <div className="bg-red-50 border border-red-200 rounded-lg p-4">
            <p className="text-sm text-red-900">
              <strong>What you can do:</strong>
            </p>
            <ul className="mt-2 space-y-1 text-sm text-red-800 list-disc list-inside">
              <li>Try refreshing the page</li>
              <li>Go back to the previous page</li>
              <li>Return to the home page</li>
              <li>Contact support if the problem persists</li>
            </ul>
          </div>

          <div className="flex flex-col sm:flex-row gap-3 pt-2">
            <Button
              onClick={reset}
              variant="default"
              className="flex-1"
            >
              <RefreshCw className="h-4 w-4 mr-2" />
              Try Again
            </Button>
            <Button asChild variant="outline" className="flex-1">
              <Link href="/">
                <Home className="h-4 w-4 mr-2" />
                Back to Home
              </Link>
            </Button>
          </div>

          <div className="text-center pt-4 border-t">
            <p className="text-sm text-gray-500">
              If this error persists, please contact our support team with the Error ID above.
            </p>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
