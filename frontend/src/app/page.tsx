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
              <div className="mb-6 flex justify-center">
                <Badge variant="secondary" className="rounded-full px-4 py-1 text-sm bg-blue-50 text-blue-700 border-blue-100">
                  <Sparkles className="mr-2 h-3.5 w-3.5 fill-blue-700" />
                  #1 Real Estate Media Platform
                </Badge>
              </div>
              
              <h1 className="text-4xl font-bold tracking-tight text-gray-900 sm:text-6xl mb-6 bg-clip-text text-transparent bg-gradient-to-r from-gray-900 via-blue-800 to-gray-900">
                Media Management
                <span className="block text-blue-600">Simplified</span>
              </h1>
              
              <p className="mt-4 text-lg text-gray-600 max-w-xl mx-auto mb-8">
                Connect agencies with pro studios. Streamline bookings and delivery.
              </p>

              <div className="flex flex-col sm:flex-row items-center justify-center gap-4">
                {isAuthenticated && user ? (
                  <>
                    {user.role === UserRole.User && (
                      <>
                        <Link href="/apply">
                          <Button size="lg" className="h-12 px-8 text-base shadow-lg shadow-blue-500/20">
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
        <section className="py-20 bg-gray-50/50">
          <div className="container mx-auto px-4 sm:px-6 lg:px-8">
            <div className="grid md:grid-cols-3 gap-6 max-w-7xl mx-auto">
              <Card className="border-none shadow-sm bg-white hover:shadow-md transition-all">
                <CardHeader>
                  <div className="w-12 h-12 rounded-xl bg-blue-100 flex items-center justify-center mb-2">
                    <Building2 className="h-6 w-6 text-blue-600" />
                  </div>
                  <CardTitle className="text-lg">For Agencies</CardTitle>
                </CardHeader>
                <CardContent>
                  <CardDescription>
                    Book shoots instantly and manage assets in one place.
                  </CardDescription>
                </CardContent>
              </Card>

              <Card className="border-none shadow-sm bg-white hover:shadow-md transition-all">
                <CardHeader>
                  <div className="w-12 h-12 rounded-xl bg-purple-100 flex items-center justify-center mb-2">
                    <Camera className="h-6 w-6 text-purple-600" />
                  </div>
                  <CardTitle className="text-lg">For Studios</CardTitle>
                </CardHeader>
                <CardContent>
                  <CardDescription>
                    Showcase your portfolio and manage bookings efficiently.
                  </CardDescription>
                </CardContent>
              </Card>

              <Card className="border-none shadow-sm bg-white hover:shadow-md transition-all">
                <CardHeader>
                  <div className="w-12 h-12 rounded-xl bg-green-100 flex items-center justify-center mb-2">
                    <Users className="h-6 w-6 text-green-600" />
                  </div>
                  <CardTitle className="text-lg">Collaboration</CardTitle>
                </CardHeader>
                <CardContent>
                  <CardDescription>
                    Real-time updates and seamless communication tools.
                  </CardDescription>
                </CardContent>
              </Card>
            </div>
          </div>
        </section>

        {/* Feature Highlights */}
        <section className="py-20">
          <div className="container mx-auto px-4 sm:px-6 lg:px-8">
            <div className="max-w-7xl mx-auto">
              <div className="grid lg:grid-cols-2 gap-12 items-center">
                <div>
                  <h2 className="text-2xl font-bold tracking-tight text-gray-900 mb-4">
                    Built for professionals
                  </h2>
                  <p className="text-gray-600 mb-6">
                    Automated workflows to help you focus on growth.
                  </p>
                  
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    {[
                      { icon: Zap, title: "Fast Ordering" },
                      { icon: Shield, title: "Secure Delivery" },
                      { icon: BarChart, title: "Analytics" },
                      { icon: CheckCircle, title: "Quality Check" }
                    ].map((feature, index) => (
                      <div key={index} className="flex items-center gap-3 p-3 rounded-lg bg-gray-50">
                        <feature.icon className="h-5 w-5 text-blue-600" />
                        <span className="font-medium text-gray-900">{feature.title}</span>
                      </div>
                    ))}
                  </div>
                </div>
                
                <div className="relative">
                  <div className="absolute inset-0 bg-gradient-to-tr from-blue-200 to-purple-200 rounded-3xl transform rotate-3 blur-lg opacity-50"></div>
                  <div className="relative bg-white rounded-2xl shadow-xl p-6 border border-gray-100">
                     <div className="grid gap-4">
                        <div className="flex items-center justify-between border-b pb-4">
                          <div className="flex items-center gap-3">
                            <div className="h-8 w-8 rounded-full bg-gray-100"></div>
                            <div>
                              <div className="h-3 w-24 bg-gray-100 rounded"></div>
                            </div>
                          </div>
                          <div className="h-6 w-16 bg-blue-100 rounded-full"></div>
                        </div>
                        <div className="space-y-2">
                          <div className="h-3 w-full bg-gray-50 rounded"></div>
                          <div className="h-3 w-5/6 bg-gray-50 rounded"></div>
                        </div>
                        <div className="grid grid-cols-2 gap-3 mt-1">
                           <div className="h-24 bg-gray-100 rounded-lg"></div>
                           <div className="h-24 bg-gray-100 rounded-lg"></div>
                        </div>
                     </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </section>

        {/* CTA Section */}
        <section className="py-20 relative overflow-hidden">
          <div className="absolute inset-0 bg-blue-600"></div>
          <div className="absolute inset-0 bg-[linear-gradient(rgba(255,255,255,0.1)_1px,transparent_1px),linear-gradient(90deg,rgba(255,255,255,0.1)_1px,transparent_1px)] bg-[size:20px_20px] opacity-10"></div>
          <div className="container mx-auto px-4 sm:px-6 lg:px-8 relative">
            <div className="max-w-3xl mx-auto text-center">
              <h2 className="text-2xl font-bold tracking-tight text-white sm:text-3xl mb-4">
                Ready to transform your workflow?
              </h2>
              
              <div className="flex flex-col sm:flex-row gap-4 justify-center mt-8">
                {isAuthenticated && user ? (
                   <Link href={user.role === UserRole.User ? "/apply" : "/dashboard"}>
                    <Button size="lg" variant="secondary" className="h-12 px-8 font-semibold w-full sm:w-auto">
                      Go to Dashboard
                      <ArrowRight className="ml-2 h-4 w-4" />
                    </Button>
                  </Link>
                ) : (
                  <>
                    <Link href="/register">
                      <Button size="lg" variant="secondary" className="h-12 px-8 font-semibold w-full sm:w-auto">
                        Start for Free
                        <ArrowRight className="ml-2 h-4 w-4" />
                      </Button>
                    </Link>
                    <Link href="/contact">
                      <Button size="lg" variant="outline" className="h-12 px-8 w-full sm:w-auto text-white border-white hover:bg-white/10 hover:text-white">
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
