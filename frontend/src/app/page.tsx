"use client";

import Link from "next/link";
import { useAuth } from "@/lib/hooks";
import { Button } from "@/components/ui/button";
import { Navbar } from "@/components/layout";
import { Building2, Camera, Users, CheckCircle, ArrowRight } from "lucide-react";

export default function HomePage() {
  const { user, isAuthenticated } = useAuth();

  return (
    <div className="min-h-screen bg-gradient-to-b from-white to-gray-50">
      <Navbar />

      {/* Hero Section */}
      <section className="container mx-auto px-4 sm:px-6 lg:px-8 py-20">
        <div className="max-w-4xl mx-auto text-center">
          <h1 className="text-5xl font-bold tracking-tight text-gray-900 sm:text-6xl">
            Professional Real Estate
            <span className="text-blue-600"> Media Platform</span>
          </h1>
          <p className="mt-6 text-lg leading-8 text-gray-600 max-w-2xl mx-auto">
            Connect real estate agencies with professional photography studios. Streamline your
            property photography workflow and deliver stunning results.
          </p>

          {/* CTA Buttons */}
          <div className="mt-10 flex items-center justify-center gap-4">
            {isAuthenticated && user ? (
              <>
                <Link href="/profile/apply">
                  <Button size="lg" className="gap-2">
                    <Building2 className="h-5 w-5" />
                    Apply for Organization
                    <ArrowRight className="h-4 w-4" />
                  </Button>
                </Link>
                <Link href="/profile?tab=applications">
                  <Button size="lg" variant="outline">
                    View My Applications
                  </Button>
                </Link>
              </>
            ) : (
              <>
                <Link href="/register">
                  <Button size="lg" className="gap-2">
                    Get Started
                    <ArrowRight className="h-4 w-4" />
                  </Button>
                </Link>
                <Link href="/login">
                  <Button size="lg" variant="outline">
                    Sign In
                  </Button>
                </Link>
              </>
            )}
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="container mx-auto px-4 sm:px-6 lg:px-8 py-16">
        <div className="max-w-5xl mx-auto">
          <h2 className="text-3xl font-bold text-center mb-12">How It Works</h2>
          <div className="grid md:grid-cols-3 gap-8">
            {/* Feature 1 */}
            <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
              <div className="flex items-center justify-center h-12 w-12 rounded-lg bg-blue-100 text-blue-600 mb-4">
                <Building2 className="h-6 w-6" />
              </div>
              <h3 className="text-lg font-semibold mb-2">Apply as Agency</h3>
              <p className="text-gray-600 text-sm">
                Submit your application to create a real estate agency. Once approved, you can
                manage your team and property listings.
              </p>
            </div>

            {/* Feature 2 */}
            <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
              <div className="flex items-center justify-center h-12 w-12 rounded-lg bg-purple-100 text-purple-600 mb-4">
                <Camera className="h-6 w-6" />
              </div>
              <h3 className="text-lg font-semibold mb-2">Apply as Studio</h3>
              <p className="text-gray-600 text-sm">
                Register your photography studio to receive and fulfill property photography
                orders from agencies.
              </p>
            </div>

            {/* Feature 3 */}
            <div className="bg-white p-6 rounded-lg shadow-sm border border-gray-200">
              <div className="flex items-center justify-center h-12 w-12 rounded-lg bg-green-100 text-green-600 mb-4">
                <Users className="h-6 w-6" />
              </div>
              <h3 className="text-lg font-semibold mb-2">Collaborate & Grow</h3>
              <p className="text-gray-600 text-sm">
                Invite team members, manage orders, and deliver high-quality photography services
                seamlessly.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* Benefits Section */}
      <section className="container mx-auto px-4 sm:px-6 lg:px-8 py-16 bg-white">
        <div className="max-w-5xl mx-auto">
          <div className="grid md:grid-cols-2 gap-12 items-center">
            <div>
              <h2 className="text-3xl font-bold mb-6">Why Choose Reamp?</h2>
              <ul className="space-y-4">
                {[
                  "Admin-approved organizations ensure quality",
                  "Easy team member invitation system",
                  "Streamlined order management workflow",
                  "Professional media delivery platform",
                  "Secure and reliable infrastructure",
                ].map((benefit, index) => (
                  <li key={index} className="flex items-start gap-3">
                    <CheckCircle className="h-5 w-5 text-green-600 mt-0.5 flex-shrink-0" />
                    <span className="text-gray-700">{benefit}</span>
                  </li>
                ))}
              </ul>
            </div>
            <div className="bg-gradient-to-br from-blue-50 to-purple-50 p-8 rounded-2xl">
              <div className="space-y-4">
                <div className="bg-white p-4 rounded-lg shadow-sm">
                  <h4 className="font-semibold text-sm text-gray-900 mb-1">For Agencies</h4>
                  <p className="text-xs text-gray-600">
                    Manage properties, create orders, and work with multiple studios
                  </p>
                </div>
                <div className="bg-white p-4 rounded-lg shadow-sm">
                  <h4 className="font-semibold text-sm text-gray-900 mb-1">For Studios</h4>
                  <p className="text-xs text-gray-600">
                    Receive orders, upload deliverables, and grow your business
                  </p>
                </div>
                <div className="bg-white p-4 rounded-lg shadow-sm">
                  <h4 className="font-semibold text-sm text-gray-900 mb-1">For Admins</h4>
                  <p className="text-xs text-gray-600">
                    Review applications, monitor system activity, and ensure quality
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="container mx-auto px-4 sm:px-6 lg:px-8 py-20">
        <div className="max-w-3xl mx-auto text-center bg-gradient-to-r from-blue-600 to-purple-600 rounded-2xl p-12 text-white">
          <h2 className="text-3xl font-bold mb-4">Ready to Get Started?</h2>
          <p className="text-lg mb-8 text-blue-50">
            Join the platform and start managing your real estate photography workflow today.
          </p>
          {isAuthenticated && user ? (
            <Link href="/profile/apply">
              <Button size="lg" variant="secondary" className="gap-2">
                <Building2 className="h-5 w-5" />
                Apply Now
                <ArrowRight className="h-4 w-4" />
              </Button>
            </Link>
          ) : (
            <Link href="/register">
              <Button size="lg" variant="secondary" className="gap-2">
                Create Account
                <ArrowRight className="h-4 w-4" />
              </Button>
            </Link>
          )}
        </div>
      </section>

      {/* Footer */}
      <footer className="border-t bg-white">
        <div className="container mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="flex flex-col md:flex-row items-center justify-between gap-4">
            <div className="flex items-center gap-2">
              <Building2 className="h-5 w-5 text-blue-600" />
              <span className="font-semibold">Reamp</span>
            </div>
            <p className="text-sm text-gray-600">
              © 2025 Reamp. Professional Real Estate Media Platform.
            </p>
          </div>
        </div>
      </footer>
    </div>
  );
}
