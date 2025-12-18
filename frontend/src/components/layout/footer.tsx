import Link from "next/link";
import { Building2, Mail, Phone, MapPin, Facebook, Twitter, Instagram, Linkedin, ArrowRight } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

export function Footer() {
  return (
    <footer className="bg-gray-900 text-gray-300 border-t border-gray-800">
      <div className="container mx-auto px-4 sm:px-6 lg:px-8 py-16">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-12 gap-12 lg:gap-8 mb-16">
          {/* Brand Column */}
          <div className="lg:col-span-4 space-y-6">
            <Link href="/" className="flex items-center gap-2 w-fit group">
              <div className="bg-blue-600 rounded-lg p-2 text-white shadow-lg shadow-blue-500/20 group-hover:bg-blue-500 transition-colors">
                 <Building2 className="h-6 w-6" />
              </div>
              <span className="text-2xl font-bold text-white tracking-tight">Reamp</span>
            </Link>
            <p className="text-gray-400 max-w-sm leading-relaxed">
              The premier platform connecting real estate agencies with top-tier photography studios. 
              Streamline your workflow and elevate your property marketing.
            </p>
            <div className="flex gap-4 pt-2">
               {[Facebook, Twitter, Instagram, Linkedin].map((Icon, i) => (
                  <a key={i} href="#" className="bg-gray-800 p-2.5 rounded-full hover:bg-blue-600 hover:text-white transition-all duration-300 text-gray-400">
                     <Icon className="h-4 w-4" />
                  </a>
               ))}
            </div>
          </div>

          {/* Quick Links */}
          <div className="lg:col-span-2 space-y-6">
            <h3 className="text-white font-semibold text-lg">Platform</h3>
            <ul className="space-y-3">
              {[
                 { label: "Browse Properties", href: "/listings" },
                 { label: "Find Agencies", href: "/agencies" },
                 { label: "Find Studios", href: "/studios" },
                 { label: "Platform Demo", href: "/showcase" },
              ].map((link) => (
                <li key={link.href}>
                  <Link href={link.href} className="text-gray-400 hover:text-blue-400 transition-colors flex items-center gap-2 group">
                    <span className="w-1.5 h-1.5 rounded-full bg-gray-700 group-hover:bg-blue-500 transition-colors" />
                    {link.label}
                  </Link>
                </li>
              ))}
            </ul>
          </div>

          {/* Support Links */}
          <div className="lg:col-span-2 space-y-6">
            <h3 className="text-white font-semibold text-lg">Support</h3>
             <ul className="space-y-3">
              {[
                 { label: "Help Center", href: "/help" },
                 { label: "Terms of Service", href: "/terms" },
                 { label: "Privacy Policy", href: "/privacy" },
                 { label: "Contact Us", href: "/contact" },
              ].map((link) => (
                <li key={link.href}>
                  <Link href={link.href} className="text-gray-400 hover:text-blue-400 transition-colors flex items-center gap-2 group">
                     <span className="w-1.5 h-1.5 rounded-full bg-gray-700 group-hover:bg-blue-500 transition-colors" />
                     {link.label}
                  </Link>
                </li>
              ))}
            </ul>
          </div>

          {/* Newsletter / Contact */}
          <div className="lg:col-span-4 space-y-6 pl-0 lg:pl-8">
            <h3 className="text-white font-semibold text-lg">Stay Updated</h3>
            <p className="text-gray-400 text-sm">
               Subscribe to our newsletter for the latest updates and features.
            </p>
            <div className="flex gap-2">
               <Input 
                  placeholder="Enter your email" 
                  className="bg-gray-800 border-gray-700 text-white placeholder:text-gray-500 focus-visible:ring-blue-600 focus-visible:border-blue-600" 
               />
               <Button className="bg-blue-600 hover:bg-blue-700 text-white shrink-0">
                  Subscribe
               </Button>
            </div>
            
            <div className="pt-6 border-t border-gray-800 space-y-3">
               <div className="flex items-center gap-3 text-gray-400 hover:text-white transition-colors cursor-pointer group">
                  <div className="bg-gray-800 p-2 rounded-lg group-hover:bg-gray-700 transition-colors">
                     <Mail className="h-4 w-4 text-blue-500" />
                  </div>
                  <span className="text-sm">hello@reamp.com</span>
               </div>
               <div className="flex items-center gap-3 text-gray-400 hover:text-white transition-colors cursor-pointer group">
                  <div className="bg-gray-800 p-2 rounded-lg group-hover:bg-gray-700 transition-colors">
                     <Phone className="h-4 w-4 text-blue-500" />
                  </div>
                  <span className="text-sm">+61 481 727 786</span>
               </div>
               <div className="flex items-center gap-3 text-gray-400 hover:text-white transition-colors cursor-pointer group">
                  <div className="bg-gray-800 p-2 rounded-lg group-hover:bg-gray-700 transition-colors">
                     <MapPin className="h-4 w-4 text-blue-500" />
                  </div>
                  <span className="text-sm">Adelaide, Australia</span>
               </div>
            </div>
          </div>
        </div>

        {/* Bottom Bar */}
        <div className="pt-8 border-t border-gray-800 flex flex-col md:flex-row justify-between items-center gap-4">
          <p className="text-sm text-gray-500">
            © {new Date().getFullYear()} Reamp Platform. All rights reserved.
          </p>
          <div className="flex gap-6 text-sm">
             <Link href="/privacy" className="text-gray-500 hover:text-white transition-colors">Privacy</Link>
             <Link href="/terms" className="text-gray-500 hover:text-white transition-colors">Terms</Link>
             <Link href="/cookies" className="text-gray-500 hover:text-white transition-colors">Cookies</Link>
          </div>
        </div>
      </div>
    </footer>
  );
}
