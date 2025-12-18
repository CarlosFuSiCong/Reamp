import Link from "next/link";
import { Building2 } from "lucide-react";

export function Footer() {
  return (
    <footer className="bg-white border-t border-gray-100 mt-auto">
      <div className="container mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="flex flex-col md:flex-row justify-between items-center gap-4">
          <div className="flex items-center gap-2">
            <Building2 className="h-5 w-5 text-blue-600" />
            <span className="font-semibold text-gray-900">Reamp</span>
            <span className="text-gray-400 text-sm mx-2">|</span>
            <p className="text-sm text-gray-500">
              © {new Date().getFullYear()} All rights reserved.
            </p>
          </div>

          <div className="flex gap-6 text-sm text-gray-500">
             <Link href="/privacy" className="hover:text-blue-600 transition-colors">Privacy</Link>
             <Link href="/terms" className="hover:text-blue-600 transition-colors">Terms</Link>
             <Link href="/contact" className="hover:text-blue-600 transition-colors">Contact</Link>
          </div>
        </div>
      </div>
    </footer>
  );
}
