"use client";

import { useState } from "react";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { ArrowRight, Mail } from "lucide-react";
import { DemoRequestDialog } from "@/components/shared/demo-request-dialog";

export function ShowcaseClient() {
  const [demoDialogOpen, setDemoDialogOpen] = useState(false);

  return (
    <>
      {/* CTA Section */}
      <section className="mt-24">
        <div className="relative rounded-3xl overflow-hidden bg-gray-900 text-white p-12 text-center">
          <div className="absolute inset-0 bg-[linear-gradient(rgba(255,255,255,0.1)_1px,transparent_1px),linear-gradient(90deg,rgba(255,255,255,0.1)_1px,transparent_1px)] bg-[size:40px_40px] opacity-20"></div>
          <div className="relative z-10 space-y-8 max-w-3xl mx-auto">
            <h2 className="text-3xl md:text-4xl font-bold">Ready to Experience Reamp?</h2>
            <p className="text-xl text-gray-300">
              Want to try the platform? Request demo access and discover how Reamp transforms real estate media management.
            </p>
            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <Link href="/register">
                <Button size="lg" className="h-14 px-8 text-lg bg-blue-600 hover:bg-blue-700">
                  Get Started Free
                  <ArrowRight className="ml-2 h-5 w-5" />
                </Button>
              </Link>
              <Button
                size="lg"
                variant="outline"
                className="h-14 px-8 text-lg bg-white/10 backdrop-blur-sm text-white border-white/50 hover:bg-white hover:text-gray-900 hover:border-white transition-colors"
                onClick={() => setDemoDialogOpen(true)}
              >
                <Mail className="mr-2 h-5 w-5" />
                Request Demo Access
              </Button>
            </div>
          </div>
        </div>
      </section>

      <DemoRequestDialog open={demoDialogOpen} onOpenChange={setDemoDialogOpen} />
    </>
  );
}
