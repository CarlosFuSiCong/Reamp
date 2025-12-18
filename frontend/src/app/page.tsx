"use client";

import Link from "next/link";
import { useAuth } from "@/lib/hooks";
import { Button } from "@/components/ui/button";
import { Navbar, Footer } from "@/components/layout";
import { Building2, Camera, Users, CheckCircle, ArrowRight } from "lucide-react";
import { UserRole } from "@/types/enums";

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
                {user.role === UserRole.User && (
                  <>
                    <Link href="/apply">
                      <Button size="lg" className="gap-2">
                        <Building2 className="h-5 w-5" />
                        Apply for Organization
                        <ArrowRight className="h-4 w-4" />
                      </Button>
                    </Link>
                    <Link href="/dashboard/profile?tab=applications">
                      <Button size="lg" variant="outline">
                        View My Applications
                      </Button>
                    </Link>
                  </>
                )}
                {user.role === UserRole.Agent && (
                  <Link href="/dashboard/agency">
                    <Button size="lg" className="gap-2">
                      Go to Agency Dashboard
                      <ArrowRight className="h-4 w-4" />
                    </Button>
                  </Link>
                )}
                {user.role === UserRole.Staff && (
                  <Link href="/dashboard/studio">
                    <Button size="lg" className="gap-2">
                      Go to Studio Dashboard
                      <ArrowRight className="h-4 w-4" />
                    </Button>
                  </Link>
                )}
                {user.role === UserRole.Admin && (
                  <Link href="/admin">
                    <Button size="lg" className="gap-2">
                      Go to Admin Dashboard
                      <ArrowRight className="h-4 w-4" />
                    </Button>
                  </Link>
                )}
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
      <section className="container mx-auto px-4 sm:px-6 lg:px-8 py-20 bg-white">
        <div className="max-w-7xl mx-auto">
          <h2 className="text-3xl font-bold text-center mb-12">Why Choose Our Platform?</h2>
          <div className="grid md:grid-cols-3 gap-8">
            <div className="text-center p-6">
              <div className="bg-blue-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
                <Building2 className="h-8 w-8 text-blue-600" />
              </div>
              <h3 className="text-xl font-semibold mb-2">For Agencies</h3>
              <p className="text-gray-600">
                Connect with professional photographers and manage all your property listings in one
                place.
              </p>
            </div>
            <div className="text-center p-6">
              <div className="bg-purple-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
                <Camera className="h-8 w-8 text-purple-600" />
              </div>
              <h3 className="text-xl font-semibold mb-2">For Studios</h3>
              <p className="text-gray-600">
                Showcase your work and receive orders from real estate agencies seamlessly.
              </p>
            </div>
            <div className="text-center p-6">
              <div className="bg-green-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
                <Users className="h-8 w-8 text-green-600" />
              </div>
              <h3 className="text-xl font-semibold mb-2">Collaboration</h3>
              <p className="text-gray-600">
                Work together efficiently with built-in communication and project management tools.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* Features List Section */}
      <section className="container mx-auto px-4 sm:px-6 lg:px-8 py-20">
        <div className="max-w-7xl mx-auto">
          <h2 className="text-3xl font-bold text-center mb-12">Platform Features</h2>
          <div className="grid md:grid-cols-2 gap-8">
            <div className="flex gap-4">
              <CheckCircle className="h-6 w-6 text-green-600 flex-shrink-0 mt-1" />
              <div>
                <h3 className="text-lg font-semibold mb-1">Easy Listing Management</h3>
                <p className="text-gray-600">
                  Create, edit, and manage property listings with an intuitive interface.
                </p>
              </div>
            </div>
            <div className="flex gap-4">
              <CheckCircle className="h-6 w-6 text-green-600 flex-shrink-0 mt-1" />
              <div>
                <h3 className="text-lg font-semibold mb-1">Professional Portfolio</h3>
                <p className="text-gray-600">
                  Studios can showcase their best work and attract more clients.
                </p>
              </div>
            </div>
            <div className="flex gap-4">
              <CheckCircle className="h-6 w-6 text-green-600 flex-shrink-0 mt-1" />
              <div>
                <h3 className="text-lg font-semibold mb-1">Order Tracking</h3>
                <p className="text-gray-600">
                  Track all orders from placement to delivery with real-time updates.
                </p>
              </div>
            </div>
            <div className="flex gap-4">
              <CheckCircle className="h-6 w-6 text-green-600 flex-shrink-0 mt-1" />
              <div>
                <h3 className="text-lg font-semibold mb-1">Secure Media Delivery</h3>
                <p className="text-gray-600">
                  Deliver high-quality photos and videos securely to your clients.
                </p>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="container mx-auto px-4 sm:px-6 lg:px-8 py-20 bg-blue-50">
        <div className="max-w-3xl mx-auto text-center">
          <h2 className="text-3xl font-bold mb-4">Ready to Get Started?</h2>
          <p className="text-lg text-gray-600 mb-8">
            Join the platform and start managing your real estate photography workflow today.
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Link href="/listings">
              <Button size="lg" variant="outline" className="gap-2">
                Browse Properties
                <ArrowRight className="h-4 w-4" />
              </Button>
            </Link>
            {isAuthenticated && user ? (
              user.role === UserRole.User ? (
                <Link href="/apply">
                  <Button size="lg" className="gap-2">
                    <Building2 className="h-5 w-5" />
                    Apply Now
                    <ArrowRight className="h-4 w-4" />
                  </Button>
                </Link>
              ) : user.role === UserRole.Agent ? (
                <Link href="/dashboard/agency">
                  <Button size="lg" className="gap-2">
                    Go to Dashboard
                    <ArrowRight className="h-4 w-4" />
                  </Button>
                </Link>
              ) : user.role === UserRole.Staff ? (
                <Link href="/dashboard/studio">
                  <Button size="lg" className="gap-2">
                    Go to Dashboard
                    <ArrowRight className="h-4 w-4" />
                  </Button>
                </Link>
              ) : (
                <Link href="/admin">
                  <Button size="lg" className="gap-2">
                    Go to Dashboard
                    <ArrowRight className="h-4 w-4" />
                  </Button>
                </Link>
              )
            ) : (
              <Link href="/register">
                <Button size="lg" className="gap-2">
                  Create Account
                  <ArrowRight className="h-4 w-4" />
                </Button>
              </Link>
            )}
          </div>
        </div>
      </section>

      <Footer />
    </div>
  );
}
