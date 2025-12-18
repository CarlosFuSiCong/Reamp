"use client";

import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Home, Search, ArrowLeft } from "lucide-react";

export default function NotFound() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100 flex items-center justify-center p-4">
      <Card className="w-full max-w-2xl shadow-xl border-gray-200">
        <CardHeader className="text-center pb-4">
          <div className="mb-6">
            <div className="inline-flex items-center justify-center w-24 h-24 rounded-full bg-blue-100 mb-4">
              <Search className="h-12 w-12 text-blue-600" aria-hidden="true" />
            </div>
          </div>
          <CardTitle className="text-4xl font-bold text-gray-900 mb-2">
            404 - Page Not Found
          </CardTitle>
          <CardDescription className="text-lg text-gray-600">
            The page you're looking for doesn't exist or has been moved
          </CardDescription>
        </CardHeader>
        
        <CardContent className="space-y-6">
          <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
            <p className="text-sm text-blue-900">
              <strong>Common reasons:</strong>
            </p>
            <ul className="mt-2 space-y-1 text-sm text-blue-800 list-disc list-inside">
              <li>The URL was mistyped</li>
              <li>The page has been removed or renamed</li>
              <li>You followed an outdated link</li>
            </ul>
          </div>

          <div className="flex flex-col sm:flex-row gap-3 pt-2">
            <Button
              onClick={() => window.history.back()}
              variant="outline"
              className="flex-1"
            >
              <ArrowLeft className="h-4 w-4 mr-2" />
              Go Back
            </Button>
            <Button asChild className="flex-1">
              <Link href="/">
                <Home className="h-4 w-4 mr-2" />
                Back to Home
              </Link>
            </Button>
          </div>

          <div className="text-center pt-4 border-t">
            <p className="text-sm text-gray-500 mb-3">Need help finding something?</p>
            <div className="flex flex-wrap justify-center gap-2">
              <Button asChild variant="link" size="sm">
                <Link href="/listings">Browse Listings</Link>
              </Button>
              <span className="text-gray-300">•</span>
              <Button asChild variant="link" size="sm">
                <Link href="/showcase">View Showcase</Link>
              </Button>
              <span className="text-gray-300">•</span>
              <Button asChild variant="link" size="sm">
                <Link href="/dashboard">Dashboard</Link>
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
