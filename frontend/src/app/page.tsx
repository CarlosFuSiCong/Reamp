"use client";

import Link from "next/link";
import { useAuth } from "@/lib/hooks";
import { Button } from "@/components/ui/button";
import { Navbar, Footer } from "@/components/layout";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Building2,
  Camera,
  Users,
  CheckCircle,
  ArrowRight,
  Sparkles,
  Zap,
  Shield,
  BarChart,
} from "lucide-react";
import { UserRole } from "@/types/enums";
import { Badge } from "@/components/ui/badge";

export default function HomePage() {
  const { user, isAuthenticated } = useAuth();

  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />

      <main className="flex-grow">
        {/* Hero Section */}
        <section className="relative overflow-hidden pt-16 pb-24 lg:pt-32 lg:pb-40">
          <div className="absolute inset-0 -z-10 bg-[radial-gradient(45rem_50rem_at_top,theme(colors.blue.100),white)] opacity-20" />
          <div className="absolute inset-y-0 right-1/2 -z-10 mr-16 w-[200%] origin-bottom-left skew-x-[-30deg] bg-white shadow-xl shadow-blue-600/10 ring-1 ring-blue-50 sm:mr-28 lg:mr-0 lg:origin-center" />
          
          <div className="container mx-auto px-4 sm:px-6 lg:px-8 relative">
            <div className="mx-auto max-w-4xl text-center">
              <div className="mb-8 flex justify-center">
                <Badge variant="secondary" className="rounded-full px-4 py-1 text-sm bg-blue-50 text-blue-700 border-blue-100 hover:bg-blue-100 transition-colors">
                  <Sparkles className="mr-2 h-3.5 w-3.5 fill-blue-700" />
                  The #1 Platform for Real Estate Media
                </Badge>
              </div>
              
              <h1 className="text-5xl font-bold tracking-tight text-gray-900 sm:text-7xl mb-8 bg-clip-text text-transparent bg-gradient-to-r from-gray-900 via-blue-800 to-gray-900">
                Professional Real Estate
                <span className="block text-blue-600">Media Platform</span>
              </h1>
              
              <p className="mt-6 text-xl leading-8 text-gray-600 max-w-2xl mx-auto mb-10">
                Connect top-tier real estate agencies with professional photography studios. 
                Streamline your workflow, manage bookings, and deliver stunning results efficiently.
              </p>

              <div className="flex flex-col sm:flex-row items-center justify-center gap-4">
                {isAuthenticated && user ? (
                  <>
                    {user.role === UserRole.User && (
                      <>
                        <Link href="/apply">
                          <Button size="lg" className="h-12 px-8 text-base shadow-lg shadow-blue-500/20">
                            <Building2 className="mr-2 h-5 w-5" />
                            Apply for Organization
                          </Button>
                        </Link>
                        <Link href="/dashboard/profile?tab=applications">
                          <Button size="lg" variant="outline" className="h-12 px-8 text-base">
                            View Applications
                          </Button>
                        </Link>
                      </>
                    )}
                    {user.role === UserRole.Agent && (
                      <Link href="/dashboard/agency">
                        <Button size="lg" className="h-12 px-8 text-base shadow-lg shadow-blue-500/20">
                          Agency Dashboard
                          <ArrowRight className="ml-2 h-5 w-5" />
                        </Button>
                      </Link>
                    )}
                    {user.role === UserRole.Staff && (
                      <Link href="/dashboard/studio">
                        <Button size="lg" className="h-12 px-8 text-base shadow-lg shadow-blue-500/20">
                          Studio Dashboard
                          <ArrowRight className="ml-2 h-5 w-5" />
                        </Button>
                      </Link>
                    )}
                    {user.role === UserRole.Admin && (
                      <Link href="/admin">
                        <Button size="lg" className="h-12 px-8 text-base shadow-lg shadow-blue-500/20">
                          Admin Dashboard
                          <ArrowRight className="ml-2 h-5 w-5" />
                        </Button>
                      </Link>
                    )}
                  </>
                ) : (
                  <>
                    <Link href="/register">
                      <Button size="lg" className="h-12 px-8 text-base shadow-lg shadow-blue-500/20 hover:scale-105 transition-transform">
                        Get Started
                        <ArrowRight className="ml-2 h-5 w-5" />
                      </Button>
                    </Link>
                    <Link href="/showcase">
                      <Button size="lg" variant="outline" className="h-12 px-8 text-base hover:bg-gray-50">
                        View Demo
                      </Button>
                    </Link>
                  </>
                )}
              </div>
            </div>
          </div>
        </section>

        {/* Value Proposition Section */}
        <section className="py-24 bg-gray-50/50">
          <div className="container mx-auto px-4 sm:px-6 lg:px-8">
            <div className="text-center mb-16">
              <h2 className="text-3xl font-bold tracking-tight text-gray-900 sm:text-4xl">
                Everything you need to succeed
              </h2>
              <p className="mt-4 text-lg text-gray-600">
                A complete ecosystem for the real estate media industry
              </p>
            </div>

            <div className="grid md:grid-cols-3 gap-8 max-w-7xl mx-auto">
              <Card className="border-none shadow-md bg-white hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
                <CardHeader>
                  <div className="w-14 h-14 rounded-2xl bg-blue-100 flex items-center justify-center mb-4">
                    <Building2 className="h-7 w-7 text-blue-600" />
                  </div>
                  <CardTitle className="text-xl">For Agencies</CardTitle>
                </CardHeader>
                <CardContent>
                  <CardDescription className="text-base">
                    Streamline your property listings. Connect with vetted professionals, 
                    book shoots instantly, and manage all your media assets in one secure place.
                  </CardDescription>
                </CardContent>
              </Card>

              <Card className="border-none shadow-md bg-white hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
                <CardHeader>
                  <div className="w-14 h-14 rounded-2xl bg-purple-100 flex items-center justify-center mb-4">
                    <Camera className="h-7 w-7 text-purple-600" />
                  </div>
                  <CardTitle className="text-xl">For Studios</CardTitle>
                </CardHeader>
                <CardContent>
                  <CardDescription className="text-base">
                    Expand your client base. Showcase your portfolio to top agencies, 
                    manage bookings efficiently, and deliver high-quality work securely.
                  </CardDescription>
                </CardContent>
              </Card>

              <Card className="border-none shadow-md bg-white hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
                <CardHeader>
                  <div className="w-14 h-14 rounded-2xl bg-green-100 flex items-center justify-center mb-4">
                    <Users className="h-7 w-7 text-green-600" />
                  </div>
                  <CardTitle className="text-xl">Seamless Collaboration</CardTitle>
                </CardHeader>
                <CardContent>
                  <CardDescription className="text-base">
                    Built-in tools for smooth communication and real-time updates.
                  </CardDescription>
                </CardContent>
              </Card>
            </div>
          </div>
        </section>

        {/* Feature Highlights */}
        <section className="py-24">
          <div className="container mx-auto px-4 sm:px-6 lg:px-8">
            <div className="max-w-7xl mx-auto">
              <div className="grid lg:grid-cols-2 gap-16 items-center">
                <div>
                  <Badge variant="outline" className="mb-4 text-blue-600 border-blue-200 bg-blue-50">Platform Features</Badge>
                  <h2 className="text-3xl font-bold tracking-tight text-gray-900 sm:text-4xl mb-6">
                    Powerful tools built for professionals
                  </h2>
                  <p className="text-lg text-gray-600 mb-8">
                    We've automated the busywork so you can focus on what matters most: 
                    growing your business and delivering exceptional value.
                  </p>
                  
                  <div className="space-y-6">
                    {[
                      {
                        icon: Zap,
                        title: "Fast Ordering System",
                        description: "Book photographers in seconds with our streamlined booking flow."
                      },
                      {
                        icon: Shield,
                        title: "Secure Asset Delivery",
                        description: "Enterprise-grade security for your high-resolution photos and videos."
                      },
                      {
                        icon: BarChart,
                        title: "Real-time Analytics",
                        description: "Track performance, orders, and revenue with detailed dashboards."
                      },
                      {
                        icon: CheckCircle,
                        title: "Quality Assurance",
                        description: "Built-in review processes ensure every delivery meets your standards."
                      }
                    ].map((feature, index) => (
                      <div key={index} className="flex gap-4">
                        <div className="flex-shrink-0 mt-1">
                          <feature.icon className="h-6 w-6 text-blue-600" />
                        </div>
                        <div>
                          <h3 className="font-semibold text-gray-900">{feature.title}</h3>
                          <p className="text-gray-600">{feature.description}</p>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
                
                <div className="relative">
                  <div className="absolute inset-0 bg-gradient-to-tr from-blue-200 to-purple-200 rounded-3xl transform rotate-3 blur-lg opacity-50"></div>
                  <div className="relative bg-white rounded-2xl shadow-2xl p-8 border border-gray-100">
                     <div className="grid gap-6">
                        <div className="flex items-center justify-between border-b pb-4">
                          <div className="flex items-center gap-3">
                            <div className="h-10 w-10 rounded-full bg-gray-100"></div>
                            <div>
                              <div className="h-4 w-32 bg-gray-100 rounded"></div>
                              <div className="h-3 w-20 bg-gray-100 rounded mt-2"></div>
                            </div>
                          </div>
                          <div className="h-8 w-24 bg-blue-100 rounded-full"></div>
                        </div>
                        <div className="space-y-3">
                          <div className="h-4 w-full bg-gray-50 rounded"></div>
                          <div className="h-4 w-5/6 bg-gray-50 rounded"></div>
                          <div className="h-4 w-4/6 bg-gray-50 rounded"></div>
                        </div>
                        <div className="grid grid-cols-2 gap-4 mt-2">
                           <div className="h-32 bg-gray-100 rounded-lg"></div>
                           <div className="h-32 bg-gray-100 rounded-lg"></div>
                        </div>
                     </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </section>

        {/* CTA Section */}
        <section className="py-24 relative overflow-hidden">
          <div className="absolute inset-0 bg-blue-600"></div>
          <div className="absolute inset-0 bg-[linear-gradient(rgba(255,255,255,0.1)_1px,transparent_1px),linear-gradient(90deg,rgba(255,255,255,0.1)_1px,transparent_1px)] bg-[size:20px_20px] opacity-10"></div>
          <div className="container mx-auto px-4 sm:px-6 lg:px-8 relative">
            <div className="max-w-4xl mx-auto text-center">
              <h2 className="text-3xl font-bold tracking-tight text-white sm:text-4xl mb-6">
                Ready to transform your workflow?
              </h2>
              <p className="text-xl text-blue-100 mb-10">
                Join thousands of real estate professionals who trust Reamp for their media management.
              </p>
              
              <div className="flex flex-col sm:flex-row gap-4 justify-center">
                {isAuthenticated && user ? (
                   <Link href={user.role === UserRole.User ? "/apply" : "/dashboard"}>
                    <Button size="lg" variant="secondary" className="h-14 px-8 text-lg font-semibold w-full sm:w-auto">
                      Go to Dashboard
                      <ArrowRight className="ml-2 h-5 w-5" />
                    </Button>
                  </Link>
                ) : (
                  <>
                    <Link href="/register">
                      <Button size="lg" variant="secondary" className="h-14 px-8 text-lg font-semibold w-full sm:w-auto">
                        Start for Free
                        <ArrowRight className="ml-2 h-5 w-5" />
                      </Button>
                    </Link>
                    <Link href="/contact">
                      <Button size="lg" variant="outline" className="h-14 px-8 text-lg w-full sm:w-auto text-white border-white hover:bg-white/10 hover:text-white">
                        Contact Sales
                      </Button>
                    </Link>
                  </>
                )}
              </div>
            </div>
          </div>
        </section>
      </main>

      <Footer />
    </div>
  );
}
