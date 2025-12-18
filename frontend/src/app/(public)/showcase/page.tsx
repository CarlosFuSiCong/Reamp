import type { Metadata } from "next";
import Link from "next/link";
import { Navbar } from "@/components/layout";
import { Footer } from "@/components/layout";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { 
  Building2, 
  Camera, 
  Users, 
  Package, 
  Calendar,
  Image as ImageIcon,
  FileText,
  UserCheck,
  ShoppingBag,
  Truck,
  CheckCircle2,
  ArrowRight
} from "lucide-react";

export const metadata: Metadata = {
  title: "Platform Showcase - Reamp",
  description: "Explore Reamp's comprehensive real estate media management platform with role-based dashboards for Agents and Studios.",
  openGraph: {
    title: "Platform Showcase - Reamp",
    description: "Professional real estate photography marketplace platform",
    type: "website",
  },
};

export default function ShowcasePage() {
  return (
    <div className="min-h-screen flex flex-col">
      <Navbar />
      
      <main className="flex-1 bg-gradient-to-b from-gray-50 to-white">
        {/* Hero Section */}
        <section className="border-b bg-white">
          <div className="container mx-auto px-4 py-16 max-w-6xl">
          <div className="text-center space-y-4">
            <Badge variant="secondary" className="mb-2">
              Demo Platform
            </Badge>
            <h1 className="text-4xl md:text-5xl font-bold tracking-tight">
              Reamp Platform Showcase
            </h1>
            <p className="text-xl text-gray-600 max-w-3xl mx-auto">
              A comprehensive real estate media management platform connecting real estate agents with professional photography studios
            </p>
          </div>
        </div>
      </section>

      {/* Main Content */}
      <div className="container mx-auto px-4 py-12 max-w-6xl">
        <div className="grid gap-8">
          {/* Agent Dashboard Section */}
          <section className="space-y-6">
            <div className="flex items-center gap-3">
              <Building2 className="h-8 w-8 text-blue-600" />
              <div>
                <h2 className="text-3xl font-bold">Agent Dashboard</h2>
                <p className="text-gray-600">Manage properties, orders, and media assets</p>
              </div>
            </div>

            <div className="grid md:grid-cols-2 gap-6">
              {/* Agent Features */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <FileText className="h-5 w-5" />
                    Listing Management
                  </CardTitle>
                  <CardDescription>
                    Create and manage property listings with rich media
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <ul className="space-y-2 text-sm text-gray-600">
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Create unlimited property listings</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Upload and manage property photos & floor plans</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Track listing status and performance</span>
                    </li>
                  </ul>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <ShoppingBag className="h-5 w-5" />
                    Order Photography Services
                  </CardTitle>
                  <CardDescription>
                    Book professional photography shoots from the marketplace
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <ul className="space-y-2 text-sm text-gray-600">
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Browse available studios and services</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Place and track photography orders</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Receive and review delivered media</span>
                    </li>
                  </ul>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <ImageIcon className="h-5 w-5" />
                    Media Library
                  </CardTitle>
                  <CardDescription>
                    Organize and link media to listings
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <ul className="space-y-2 text-sm text-gray-600">
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Cloud storage integration (Cloudinary)</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Drag-and-drop media organization</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Multiple role support (gallery, cover, floor plans)</span>
                    </li>
                  </ul>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Users className="h-5 w-5" />
                    Team Collaboration
                  </CardTitle>
                  <CardDescription>
                    Manage your agency team and permissions
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <ul className="space-y-2 text-sm text-gray-600">
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Multi-agent agency support</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Role-based access control</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Shared listing management</span>
                    </li>
                  </ul>
                </CardContent>
              </Card>
            </div>

            {/* Agent Demo Login */}
            <Card className="border-blue-200 bg-blue-50">
              <CardHeader>
                <CardTitle className="text-blue-900">Try Agent Dashboard</CardTitle>
                <CardDescription className="text-blue-700">
                  Login with demo credentials to explore agent features
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  <div className="bg-white rounded-lg p-4 space-y-2">
                    <p className="text-sm font-medium text-gray-700">Demo Credentials:</p>
                    <div className="grid grid-cols-2 gap-4 text-sm">
                      <div>
                        <span className="text-gray-600">Email:</span>
                        <code className="ml-2 px-2 py-1 bg-gray-100 rounded">agent1@reamp.com</code>
                      </div>
                      <div>
                        <span className="text-gray-600">Password:</span>
                        <code className="ml-2 px-2 py-1 bg-gray-100 rounded">Test@123</code>
                      </div>
                    </div>
                  </div>
                  <Link href="/login">
                    <Button className="w-full" size="lg">
                      Login as Agent
                      <ArrowRight className="ml-2 h-4 w-4" />
                    </Button>
                  </Link>
                </div>
              </CardContent>
            </Card>
          </section>

          <div className="border-t my-8" />

          {/* Studio Dashboard Section */}
          <section className="space-y-6">
            <div className="flex items-center gap-3">
              <Camera className="h-8 w-8 text-purple-600" />
              <div>
                <h2 className="text-3xl font-bold">Studio Dashboard</h2>
                <p className="text-gray-600">Professional photography service management</p>
              </div>
            </div>

            <div className="grid md:grid-cols-2 gap-6">
              {/* Studio Features */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Package className="h-5 w-5" />
                    Order Management
                  </CardTitle>
                  <CardDescription>
                    Accept and manage photography orders
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <ul className="space-y-2 text-sm text-gray-600">
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>View incoming orders from agents</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Accept or decline based on availability</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Track order status and deadlines</span>
                    </li>
                  </ul>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <UserCheck className="h-5 w-5" />
                    Staff Assignment
                  </CardTitle>
                  <CardDescription>
                    Assign photographers to shoots
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <ul className="space-y-2 text-sm text-gray-600">
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Manage studio staff and skills</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Assign staff based on expertise</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Schedule management</span>
                    </li>
                  </ul>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Truck className="h-5 w-5" />
                    Delivery Management
                  </CardTitle>
                  <CardDescription>
                    Package and deliver completed work
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <ul className="space-y-2 text-sm text-gray-600">
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Upload processed photos and media</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Create delivery packages with access control</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Secure delivery links (public/token/private)</span>
                    </li>
                  </ul>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Calendar className="h-5 w-5" />
                    Media Upload
                  </CardTitle>
                  <CardDescription>
                    Advanced upload capabilities
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <ul className="space-y-2 text-sm text-gray-600">
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Chunked upload for large files</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Real-time upload progress via SignalR</span>
                    </li>
                    <li className="flex items-start gap-2">
                      <CheckCircle2 className="h-4 w-4 text-green-600 mt-0.5 flex-shrink-0" />
                      <span>Automatic cloud processing</span>
                    </li>
                  </ul>
                </CardContent>
              </Card>
            </div>

            {/* Studio Demo Login */}
            <Card className="border-purple-200 bg-purple-50">
              <CardHeader>
                <CardTitle className="text-purple-900">Try Studio Dashboard</CardTitle>
                <CardDescription className="text-purple-700">
                  Login with demo credentials to explore studio features
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  <div className="bg-white rounded-lg p-4 space-y-2">
                    <p className="text-sm font-medium text-gray-700">Demo Credentials:</p>
                    <div className="grid grid-cols-2 gap-4 text-sm">
                      <div>
                        <span className="text-gray-600">Email:</span>
                        <code className="ml-2 px-2 py-1 bg-gray-100 rounded">staff1@reamp.com</code>
                      </div>
                      <div>
                        <span className="text-gray-600">Password:</span>
                        <code className="ml-2 px-2 py-1 bg-gray-100 rounded">Test@123</code>
                      </div>
                    </div>
                  </div>
                  <Link href="/login">
                    <Button className="w-full" size="lg" variant="secondary">
                      Login as Studio Staff
                      <ArrowRight className="ml-2 h-4 w-4" />
                    </Button>
                  </Link>
                </div>
              </CardContent>
            </Card>
          </section>

          <div className="border-t my-8" />

          {/* Technology Stack */}
          <section className="space-y-6">
            <div className="text-center space-y-2">
              <h2 className="text-2xl font-bold">Technology Stack</h2>
              <p className="text-gray-600">Built with modern, scalable technologies</p>
            </div>

            <div className="grid md:grid-cols-3 gap-6">
              <Card>
                <CardHeader>
                  <CardTitle className="text-lg">Frontend</CardTitle>
                </CardHeader>
                <CardContent>
                  <ul className="space-y-1 text-sm text-gray-600">
                    <li>• Next.js 15 (App Router)</li>
                    <li>• TypeScript</li>
                    <li>• Tailwind CSS</li>
                    <li>• shadcn/ui Components</li>
                    <li>• TanStack Query</li>
                    <li>• Zustand State Management</li>
                  </ul>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle className="text-lg">Backend</CardTitle>
                </CardHeader>
                <CardContent>
                  <ul className="space-y-1 text-sm text-gray-600">
                    <li>• ASP.NET Core 8</li>
                    <li>• Entity Framework Core</li>
                    <li>• SQL Server</li>
                    <li>• Clean Architecture (DDD)</li>
                    <li>• CQRS Pattern</li>
                    <li>• SignalR for Real-time</li>
                  </ul>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle className="text-lg">Infrastructure</CardTitle>
                </CardHeader>
                <CardContent>
                  <ul className="space-y-1 text-sm text-gray-600">
                    <li>• Docker & Docker Compose</li>
                    <li>• Cloudinary CDN</li>
                    <li>• JWT Authentication</li>
                    <li>• Role-based Authorization</li>
                    <li>• RESTful API</li>
                    <li>• Cookie-based Sessions</li>
                  </ul>
                </CardContent>
              </Card>
            </div>
          </section>

          {/* Quick Links */}
          <section className="mt-12">
            <Card className="bg-gradient-to-r from-blue-50 to-purple-50 border-none">
              <CardHeader className="text-center">
                <CardTitle className="text-2xl">Ready to Explore?</CardTitle>
                <CardDescription className="text-base">
                  Start by browsing public listings or login to test the dashboard
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="flex flex-wrap gap-4 justify-center">
                  <Link href="/listings">
                    <Button size="lg" variant="outline">
                      Browse Listings
                    </Button>
                  </Link>
                  <Link href="/login">
                    <Button size="lg">
                      Login to Dashboard
                    </Button>
                  </Link>
                </div>
              </CardContent>
            </Card>
          </section>
        </div>
      </main>
      <Footer />
    </div>
  );
}
