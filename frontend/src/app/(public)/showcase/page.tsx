import type { Metadata } from "next";
import Link from "next/link";
import { Navbar } from "@/components/layout";
import { Footer } from "@/components/layout";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { ShowcaseClient } from "./showcase-client";
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
  ArrowRight,
  Database,
  Server,
  Cloud,
  Layers,
  Code,
  Mail
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
    <div className="min-h-screen flex flex-col bg-gray-50/30">
      <Navbar />
      <main className="flex-1">
        {/* Hero Section */}
        <section className="relative overflow-hidden pt-20 pb-32 bg-white">
          <div className="absolute inset-0 -z-10 bg-[radial-gradient(ellipse_at_top,theme(colors.blue.50),white)] opacity-60" />
          <div className="container mx-auto px-4 sm:px-6 lg:px-8 relative">
            <div className="mx-auto max-w-4xl text-center space-y-8">
              <Badge variant="secondary" className="px-4 py-1.5 text-sm bg-blue-50 text-blue-700 border-blue-100 rounded-full font-medium">
                Live Demo Platform
              </Badge>
              <h1 className="text-5xl md:text-6xl lg:text-7xl font-bold tracking-tight text-gray-900 leading-tight">
                Reamp Platform <span className="text-blue-600 block sm:inline">Showcase</span>
              </h1>
              <p className="text-xl text-gray-600 max-w-2xl mx-auto leading-relaxed">
                Experience the next generation of real estate media management. 
                Seamlessly connecting agencies with professional studios.
              </p>
              <div className="flex flex-wrap justify-center gap-4 pt-4">
                <Link href="#agent-features">
                  <Button size="lg" className="h-12 px-8 text-base shadow-lg shadow-blue-500/20 bg-blue-600 hover:bg-blue-700">
                    Explore Features
                    <ArrowRight className="ml-2 h-4 w-4" />
                  </Button>
                </Link>
                <Link href="/listings">
                  <Button size="lg" variant="outline" className="h-12 px-8 text-base bg-white hover:bg-gray-50">
                    View Live Listings
                  </Button>
                </Link>
              </div>
            </div>
          </div>
        </section>

        <div className="container mx-auto px-4 sm:px-6 lg:px-8 py-20 max-w-7xl">
          {/* Agent Dashboard Section */}
          <section id="agent-features" className="scroll-mt-24">
            <div className="flex items-center gap-4 mb-12">
              <div className="p-3 bg-blue-100 rounded-2xl">
                <Building2 className="h-8 w-8 text-blue-600" />
              </div>
              <div>
                <h2 className="text-3xl font-bold text-gray-900">Agent Dashboard</h2>
                <p className="text-lg text-gray-500 mt-1">Manage properties, orders, and media assets</p>
              </div>
            </div>

            <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
              <FeatureCard 
                icon={FileText}
                title="Listing Management"
                description="Create and manage property listings with rich media"
                features={[
                  "Create unlimited property listings",
                  "Upload photos & floor plans",
                  "Track listing status"
                ]}
                color="blue"
              />
              <FeatureCard 
                icon={ShoppingBag}
                title="Order Services"
                description="Book professional photography shoots from the marketplace"
                features={[
                  "Browse available studios",
                  "Place and track orders",
                  "Review delivered media"
                ]}
                color="blue"
              />
              <FeatureCard 
                icon={ImageIcon}
                title="Media Library"
                description="Organize and link media to listings"
                features={[
                  "Cloud storage integration",
                  "Drag-and-drop organization",
                  "Multiple media roles"
                ]}
                color="blue"
              />
              <FeatureCard 
                icon={Users}
                title="Team Collaboration"
                description="Manage your agency team and permissions"
                features={[
                  "Multi-agent support",
                  "Role-based access",
                  "Shared management"
                ]}
                color="blue"
              />
            </div>
          </section>

          <Separator className="my-20" />

          {/* Studio Dashboard Section */}
          <section className="space-y-6">
            <div className="flex items-center gap-4 mb-12">
              <div className="p-3 bg-purple-100 rounded-2xl">
                <Camera className="h-8 w-8 text-purple-600" />
              </div>
              <div>
                <h2 className="text-3xl font-bold text-gray-900">Studio Dashboard</h2>
                <p className="text-lg text-gray-500 mt-1">Professional photography service management</p>
              </div>
            </div>

            <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
              <FeatureCard 
                icon={Package}
                title="Order Management"
                description="Accept and manage photography orders"
                features={[
                  "View incoming orders",
                  "Accept/decline workflows",
                  "Track deadlines"
                ]}
                color="purple"
              />
              <FeatureCard 
                icon={UserCheck}
                title="Staff Assignment"
                description="Assign photographers to shoots"
                features={[
                  "Manage studio staff",
                  "Skill-based assignment",
                  "Schedule management"
                ]}
                color="purple"
              />
              <FeatureCard 
                icon={Truck}
                title="Delivery"
                description="Package and deliver completed work"
                features={[
                  "Upload processed media",
                  "Secure delivery links",
                  "Access control"
                ]}
                color="purple"
              />
              <FeatureCard 
                icon={Calendar}
                title="Media Upload"
                description="Advanced upload capabilities"
                features={[
                  "Chunked large file upload",
                  "Real-time progress",
                  "Auto processing"
                ]}
                color="purple"
              />
            </div>
          </section>

          <Separator className="my-20" />

          {/* Technology Stack */}
          <section className="space-y-12">
            <div className="text-center space-y-4">
              <h2 className="text-3xl font-bold text-gray-900">Technology Stack</h2>
              <p className="text-lg text-gray-500 max-w-2xl mx-auto">
                Built with modern, scalable technologies to ensure performance, security, and reliability.
              </p>
            </div>

            <div className="grid md:grid-cols-3 gap-8">
              <TechCard 
                icon={Layers}
                title="Frontend"
                items={[
                  "Next.js 15 (App Router)",
                  "TypeScript & Tailwind CSS",
                  "shadcn/ui Components",
                  "TanStack Query",
                  "Zustand State"
                ]}
              />
              <TechCard 
                icon={Server}
                title="Backend"
                items={[
                  "ASP.NET Core 8",
                  "Entity Framework Core",
                  "Clean Architecture (DDD)",
                  "CQRS Pattern",
                  "SignalR Real-time"
                ]}
              />
              <TechCard 
                icon={Cloud}
                title="Infrastructure"
                items={[
                  "Docker Containerization",
                  "Cloudinary CDN",
                  "SQL Server",
                  "JWT Authentication",
                  "RESTful API"
                ]}
              />
            </div>
          </section>

          {/* CTA Section - Client Component */}
          <ShowcaseClient />
        </div>
      </main>
      <Footer />
    </div>
  );
}

function FeatureCard({ icon: Icon, title, description, features, color }: any) {
  const isBlue = color === 'blue';
  
  return (
    <Card className={`border-0 shadow-md hover:shadow-xl transition-all duration-300 hover:-translate-y-1 ${isBlue ? 'hover:shadow-blue-100' : 'hover:shadow-purple-100'}`}>
      <CardHeader>
        <div className={`w-12 h-12 rounded-xl flex items-center justify-center mb-4 ${isBlue ? 'bg-blue-100 text-blue-600' : 'bg-purple-100 text-purple-600'}`}>
          <Icon className="h-6 w-6" />
        </div>
        <CardTitle className="text-xl font-bold">{title}</CardTitle>
        <CardDescription className="text-sm line-clamp-2">{description}</CardDescription>
      </CardHeader>
      <CardContent>
        <ul className="space-y-3">
          {features.map((feature: string, i: number) => (
            <li key={i} className="flex items-start gap-2 text-sm text-gray-600">
              <CheckCircle2 className={`h-4 w-4 mt-0.5 flex-shrink-0 ${isBlue ? 'text-blue-500' : 'text-purple-500'}`} />
              <span className="leading-tight">{feature}</span>
            </li>
          ))}
        </ul>
      </CardContent>
    </Card>
  );
}

function TechCard({ icon: Icon, title, items }: any) {
  return (
    <Card className="border border-gray-100 shadow-sm hover:shadow-md transition-shadow">
      <CardHeader className="pb-2">
        <div className="flex items-center gap-3 mb-2">
          <div className="p-2 bg-gray-100 rounded-lg">
            <Icon className="h-5 w-5 text-gray-700" />
          </div>
          <CardTitle className="text-lg">{title}</CardTitle>
        </div>
      </CardHeader>
      <CardContent>
        <ul className="space-y-2">
          {items.map((item: string, i: number) => (
            <li key={i} className="flex items-center gap-2 text-sm text-gray-600">
              <div className="w-1.5 h-1.5 rounded-full bg-gray-300" />
              {item}
            </li>
          ))}
        </ul>
      </CardContent>
    </Card>
  );
}
