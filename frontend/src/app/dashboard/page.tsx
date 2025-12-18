"use client";

import { useProfile } from "@/lib/hooks";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { 
  Activity, 
  ArrowUpRight, 
  Building2, 
  Calendar, 
  DollarSign, 
  FileText, 
  Home, 
  Plus, 
  Settings, 
  ShoppingBag, 
  Users,
  Camera,
  Bell,
  CheckCircle2,
  Clock,
  TrendingUp,
  TrendingDown,
  Sparkles,
  Package,
  ArrowRight,
  MapPin,
  Image as ImageIcon
} from "lucide-react";
import Link from "next/link";
import { UserRole } from "@/types/enums";
import { LoadingState } from "@/components/shared";
import { format } from "date-fns";

export default function DashboardPage() {
  const { user: profile, isLoading } = useProfile();

  if (isLoading) {
    return <LoadingState message="Loading dashboard..." />;
  }

  if (!profile) {
    return null;
  }

  const currentDate = format(new Date(), "EEEE, MMMM d, yyyy");
  const currentTime = format(new Date(), "HH:mm");

  // Determine user context (Admin, Agency, Studio, Staff)
  const isAdmin = profile.role === UserRole.Admin;
  const isAgency = profile.agencyRole !== undefined && profile.agencyRole !== null;
  const isStudio = profile.studioRole !== undefined && profile.studioRole !== null;
  const isStaff = profile.role === UserRole.Staff;

  return (
    <div className="space-y-8">
      {/* Header Section with Gradient */}
      <div className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-blue-600 via-blue-700 to-indigo-700 p-8 shadow-xl">
        <div className="absolute inset-0 bg-grid-white/10 [mask-image:linear-gradient(0deg,transparent,rgba(255,255,255,0.2))]" />
        <div className="absolute -right-10 -top-10 h-40 w-40 rounded-full bg-white/10 blur-3xl" />
        <div className="absolute -bottom-10 -left-10 h-40 w-40 rounded-full bg-white/10 blur-3xl" />
        
        <div className="relative flex flex-col md:flex-row md:items-center justify-between gap-6">
          <div className="space-y-2">
            <div className="flex items-center gap-2 mb-2">
              <Badge variant="secondary" className="bg-white/20 text-white border-white/30 hover:bg-white/30">
                <Sparkles className="h-3 w-3 mr-1" />
                {isAdmin ? "Admin" : isAgency ? "Agency" : isStudio ? "Studio" : "Staff"}
              </Badge>
            </div>
            <h1 className="text-3xl md:text-4xl font-bold text-white tracking-tight animate-in fade-in slide-in-from-bottom-3 duration-700">
              Welcome back, {profile.displayName || profile.firstName}! 👋
            </h1>
            <p className="text-blue-100 text-sm md:text-base animate-in fade-in slide-in-from-bottom-4 duration-700 delay-100">
              Here's what's happening with your workspace today
            </p>
          </div>
          
          <div className="flex flex-col sm:flex-row items-start sm:items-center gap-3 animate-in fade-in slide-in-from-bottom-5 duration-700 delay-200">
            <div className="flex items-center gap-3 px-4 py-3 bg-white/10 backdrop-blur-sm rounded-xl border border-white/20 text-white shadow-lg">
              <Calendar className="h-5 w-5 text-blue-200" />
              <div className="text-left">
                <div className="text-xs font-medium text-blue-200">Today</div>
                <div className="text-sm font-semibold">{format(new Date(), "MMM d, yyyy")}</div>
              </div>
            </div>
            <Button className="bg-white text-blue-700 hover:bg-blue-50 shadow-lg hover:shadow-xl transition-all duration-200 hover:scale-105">
              <Plus className="h-4 w-4 mr-2" />
              New Action
            </Button>
          </div>
        </div>
      </div>

      {/* Stats Grid - Enhanced */}
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4">
        {isAdmin && (
          <>
            <StatsCard 
              title="Total Users" 
              value="1,234" 
              icon={Users} 
              trend="+12%" 
              trendUp={true}
              color="blue"
              bgPattern="users"
            />
            <StatsCard 
              title="Active Agencies" 
              value="45" 
              icon={Building2} 
              trend="+2" 
              trendUp={true}
              color="purple"
              bgPattern="building"
            />
            <StatsCard 
              title="Active Studios" 
              value="12" 
              icon={Camera} 
              trend="+1" 
              trendUp={true}
              color="pink"
              bgPattern="camera"
            />
            <StatsCard 
              title="Total Revenue" 
              value="$45,231" 
              icon={DollarSign} 
              trend="+8%" 
              trendUp={true}
              color="green"
              bgPattern="money"
            />
          </>
        )}
        
        {isAgency && !isAdmin && (
          <>
            <StatsCard 
              title="Active Listings" 
              value="24" 
              icon={Home} 
              trend="+4" 
              trendUp={true}
              color="blue"
            />
            <StatsCard 
              title="Pending Orders" 
              value="7" 
              icon={ShoppingBag} 
              trend="-1" 
              trendUp={false}
              description="Requires attention" 
              color="orange"
            />
            <StatsCard 
              title="Completed Shoots" 
              value="156" 
              icon={Camera} 
              trend="+12%" 
              trendUp={true}
              color="green"
            />
            <StatsCard 
              title="Total Spent" 
              value="$3,450" 
              icon={DollarSign} 
              trend="+2.5%" 
              trendUp={true}
              color="purple"
            />
          </>
        )}

        {isStudio && !isAdmin && (
          <>
            <StatsCard 
              title="New Orders" 
              value="12" 
              icon={ShoppingBag} 
              trend="+3" 
              trendUp={true}
              description="Waiting for acceptance" 
              color="blue"
            />
            <StatsCard 
              title="Active Shoots" 
              value="8" 
              icon={Camera} 
              description="On track" 
              color="green"
            />
            <StatsCard 
              title="Pending Deliveries" 
              value="5" 
              icon={FileText} 
              trend="-2" 
              trendUp={false}
              color="orange"
            />
            <StatsCard 
              title="Revenue" 
              value="$12,450" 
              icon={DollarSign} 
              trend="+15%" 
              trendUp={true}
              color="purple"
            />
          </>
        )}

        {!isAdmin && !isAgency && !isStudio && (
          <>
            <StatsCard 
              title="My Tasks" 
              value="5" 
              icon={CheckCircle2} 
              description="3 due today" 
              color="blue"
            />
            <StatsCard 
              title="Upcoming Shoots" 
              value="2" 
              icon={Calendar} 
              description="Next: Tomorrow" 
              color="purple"
            />
            <StatsCard 
              title="Completed" 
              value="48" 
              icon={CheckCircle2} 
              trend="+4" 
              trendUp={true}
              color="green"
            />
            <StatsCard 
              title="Performance" 
              value="98%" 
              icon={Activity} 
              trend="+1%" 
              trendUp={true}
              color="pink"
            />
          </>
        )}
      </div>

      {/* Main Content Grid */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Left Column - Main Content */}
        <div className="lg:col-span-2 space-y-6">
          {/* Chart Card */}
          <Card className="border-0 shadow-lg hover:shadow-xl transition-shadow duration-300">
            <CardHeader>
              <div className="flex items-center justify-between">
                <div>
                  <CardTitle className="text-xl">Performance Overview</CardTitle>
                  <CardDescription>Your activity trends over the last 30 days</CardDescription>
                </div>
                <Button variant="outline" size="sm">
                  View Details
                  <ArrowRight className="h-4 w-4 ml-2" />
                </Button>
              </div>
            </CardHeader>
            <CardContent>
              <div className="h-[240px] flex flex-col items-center justify-center text-muted-foreground bg-gradient-to-br from-blue-50 via-indigo-50 to-purple-50 rounded-xl border-2 border-dashed border-gray-200">
                <TrendingUp className="h-12 w-12 text-blue-400 mb-3" />
                <p className="text-sm font-medium">Chart visualization coming soon</p>
                <p className="text-xs text-gray-400 mt-1">Connect your data to see insights</p>
              </div>
            </CardContent>
          </Card>

          {/* Recent Activity */}
          <Card className="border-0 shadow-lg hover:shadow-xl transition-shadow duration-300">
            <CardHeader>
              <div className="flex items-center justify-between">
                <div>
                  <CardTitle className="text-xl">Recent Activity</CardTitle>
                  <CardDescription>Latest actions and updates across your account</CardDescription>
                </div>
                <Button variant="ghost" size="sm" className="text-blue-600 hover:text-blue-700">
                  View All
                  <ArrowRight className="h-4 w-4 ml-2" />
                </Button>
              </div>
            </CardHeader>
            <CardContent>
              <div className="space-y-6">
                {[
                  { 
                    icon: Home, 
                    color: "blue", 
                    title: "New listing created", 
                    desc: "Property at 123 Ocean Drive was added to the marketplace.",
                    time: "2 hours ago"
                  },
                  { 
                    icon: ImageIcon, 
                    color: "green", 
                    title: "Photos delivered", 
                    desc: "45 high-resolution photos uploaded for Villa Sunset project.",
                    time: "4 hours ago"
                  },
                  { 
                    icon: CheckCircle2, 
                    color: "purple", 
                    title: "Order completed", 
                    desc: "Photography session for Downtown Apartment finalized.",
                    time: "6 hours ago"
                  },
                ].map((activity, i) => (
                  <div 
                    key={i} 
                    className="flex items-start gap-4 group hover:bg-gray-50 p-3 rounded-lg -mx-3 transition-colors cursor-pointer"
                  >
                    <div className={`rounded-xl p-2.5 bg-${activity.color}-50 text-${activity.color}-600 group-hover:scale-110 transition-transform`}>
                      <activity.icon className="h-4 w-4" />
                    </div>
                    <div className="flex-1 space-y-1">
                      <p className="text-sm font-semibold text-gray-900 group-hover:text-blue-600 transition-colors">
                        {activity.title}
                      </p>
                      <p className="text-sm text-gray-600 leading-relaxed">
                        {activity.desc}
                      </p>
                      <div className="flex items-center gap-2 pt-1">
                        <Clock className="h-3 w-3 text-gray-400" />
                        <p className="text-xs text-gray-500 font-medium">
                          {activity.time}
                        </p>
                      </div>
                    </div>
                    <ArrowRight className="h-4 w-4 text-gray-400 opacity-0 group-hover:opacity-100 transition-opacity" />
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Right Column - Sidebar */}
        <div className="space-y-6">
          {/* Quick Actions */}
          <Card className="border-0 shadow-lg hover:shadow-xl transition-shadow duration-300">
            <CardHeader>
              <CardTitle className="text-xl">Quick Actions</CardTitle>
              <CardDescription>Frequently used actions</CardDescription>
            </CardHeader>
            <CardContent className="grid gap-3">
              {isAgency && (
                <Link href="/dashboard/listings/new">
                  <div className="group relative overflow-hidden rounded-xl border-2 border-blue-100 bg-gradient-to-br from-blue-50 to-indigo-50 p-4 hover:border-blue-300 hover:shadow-md transition-all duration-200 cursor-pointer">
                    <div className="flex items-center gap-3">
                      <div className="rounded-lg bg-blue-600 p-2 text-white group-hover:scale-110 transition-transform">
                        <Plus className="h-4 w-4" />
                      </div>
                      <div>
                        <p className="font-semibold text-gray-900">Create Listing</p>
                        <p className="text-xs text-gray-600">Add a new property</p>
                      </div>
                      <ArrowRight className="h-4 w-4 text-blue-600 ml-auto opacity-0 group-hover:opacity-100 group-hover:translate-x-1 transition-all" />
                    </div>
                  </div>
                </Link>
              )}
              
              <Link href="/dashboard/profile">
                <div className="group relative overflow-hidden rounded-xl border-2 border-purple-100 bg-gradient-to-br from-purple-50 to-pink-50 p-4 hover:border-purple-300 hover:shadow-md transition-all duration-200 cursor-pointer">
                  <div className="flex items-center gap-3">
                    <div className="rounded-lg bg-purple-600 p-2 text-white group-hover:scale-110 transition-transform">
                      <Settings className="h-4 w-4" />
                    </div>
                    <div>
                      <p className="font-semibold text-gray-900">Edit Profile</p>
                      <p className="text-xs text-gray-600">Update your details</p>
                    </div>
                    <ArrowRight className="h-4 w-4 text-purple-600 ml-auto opacity-0 group-hover:opacity-100 group-hover:translate-x-1 transition-all" />
                  </div>
                </div>
              </Link>

              <div className="group relative overflow-hidden rounded-xl border-2 border-orange-100 bg-gradient-to-br from-orange-50 to-amber-50 p-4 hover:border-orange-300 hover:shadow-md transition-all duration-200 cursor-pointer">
                <div className="flex items-center gap-3">
                  <div className="rounded-lg bg-orange-600 p-2 text-white group-hover:scale-110 transition-transform relative">
                    <Bell className="h-4 w-4" />
                    <span className="absolute -top-1 -right-1 h-3 w-3 rounded-full bg-red-500 border-2 border-white"></span>
                  </div>
                  <div>
                    <p className="font-semibold text-gray-900">Notifications</p>
                    <p className="text-xs text-gray-600">3 new alerts</p>
                  </div>
                  <ArrowRight className="h-4 w-4 text-orange-600 ml-auto opacity-0 group-hover:opacity-100 group-hover:translate-x-1 transition-all" />
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Schedule */}
          <Card className="border-0 shadow-lg hover:shadow-xl transition-shadow duration-300">
            <CardHeader>
              <CardTitle className="text-xl">Today's Schedule</CardTitle>
              <CardDescription>Your agenda for {format(new Date(), "MMM d")}</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                <div className="group relative overflow-hidden rounded-xl border-2 border-blue-100 bg-gradient-to-r from-blue-50 to-indigo-50 p-4 hover:border-blue-300 hover:shadow-md transition-all duration-200 cursor-pointer">
                  <div className="flex items-center gap-4">
                    <div className="flex flex-col items-center justify-center min-w-[4rem] rounded-lg bg-blue-600 text-white p-3">
                      <span className="text-xs font-bold uppercase tracking-wide">Today</span>
                      <span className="text-2xl font-bold">10:00</span>
                    </div>
                    <div className="flex-1">
                      <p className="text-sm font-semibold text-gray-900 mb-1">Team Meeting</p>
                      <p className="text-xs text-gray-600 flex items-center gap-1">
                        <Users className="h-3 w-3" />
                        Weekly sync with studio
                      </p>
                    </div>
                    <ArrowRight className="h-4 w-4 text-blue-600 opacity-0 group-hover:opacity-100 transition-opacity" />
                  </div>
                </div>

                <div className="group relative overflow-hidden rounded-xl border-2 border-purple-100 bg-gradient-to-r from-purple-50 to-pink-50 p-4 hover:border-purple-300 hover:shadow-md transition-all duration-200 cursor-pointer">
                  <div className="flex items-center gap-4">
                    <div className="flex flex-col items-center justify-center min-w-[4rem] rounded-lg bg-purple-600 text-white p-3">
                      <span className="text-xs font-bold uppercase tracking-wide">Today</span>
                      <span className="text-2xl font-bold">14:30</span>
                    </div>
                    <div className="flex-1">
                      <p className="text-sm font-semibold text-gray-900 mb-1">Shoot: 123 Main St</p>
                      <p className="text-xs text-gray-600 flex items-center gap-1">
                        <Camera className="h-3 w-3" />
                        Photography & Video
                      </p>
                    </div>
                    <ArrowRight className="h-4 w-4 text-purple-600 opacity-0 group-hover:opacity-100 transition-opacity" />
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}

function StatsCard({ 
  title, 
  value, 
  icon: Icon, 
  trend, 
  trendUp,
  description,
  color = "blue",
  bgPattern
}: { 
  title: string; 
  value: string; 
  icon: any; 
  trend?: string; 
  trendUp?: boolean;
  description?: string;
  color?: "blue" | "green" | "purple" | "pink" | "orange";
  bgPattern?: string;
}) {
  const colorClasses = {
    blue: {
      bg: "from-blue-500 to-blue-600",
      light: "bg-blue-50",
      text: "text-blue-600",
      icon: "text-blue-600"
    },
    green: {
      bg: "from-green-500 to-emerald-600",
      light: "bg-green-50",
      text: "text-green-600",
      icon: "text-green-600"
    },
    purple: {
      bg: "from-purple-500 to-purple-600",
      light: "bg-purple-50",
      text: "text-purple-600",
      icon: "text-purple-600"
    },
    pink: {
      bg: "from-pink-500 to-rose-600",
      light: "bg-pink-50",
      text: "text-pink-600",
      icon: "text-pink-600"
    },
    orange: {
      bg: "from-orange-500 to-amber-600",
      light: "bg-orange-50",
      text: "text-orange-600",
      icon: "text-orange-600"
    }
  };

  const colors = colorClasses[color];

  return (
    <Card className="relative overflow-hidden border-0 shadow-md hover:shadow-xl transition-all duration-300 group cursor-pointer hover:-translate-y-1">
      <div className="absolute top-0 right-0 w-32 h-32 opacity-5">
        <Icon className="w-full h-full" />
      </div>
      
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-3">
        <CardTitle className="text-sm font-semibold text-gray-600">
          {title}
        </CardTitle>
        <div className={`rounded-xl p-2.5 ${colors.light} ${colors.icon} group-hover:scale-110 transition-transform`}>
          <Icon className="h-5 w-5" />
        </div>
      </CardHeader>
      <CardContent className="space-y-2">
        <div className="text-3xl font-bold text-gray-900">{value}</div>
        {(trend || description) && (
          <div className="flex items-center gap-2">
            {trend && (
              <div className={`flex items-center gap-1 px-2 py-1 rounded-full text-xs font-semibold ${
                trendUp ? "bg-green-50 text-green-700" : "bg-red-50 text-red-700"
              }`}>
                {trendUp ? <TrendingUp className="h-3 w-3" /> : <TrendingDown className="h-3 w-3" />}
                {trend}
              </div>
            )}
            {description && (
              <p className="text-xs text-gray-600 font-medium">{description}</p>
            )}
            {!description && trend && (
              <p className="text-xs text-gray-500">vs last month</p>
            )}
          </div>
        )}
      </CardContent>
    </Card>
  );
}
