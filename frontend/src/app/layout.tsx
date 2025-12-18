import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import { QueryProvider } from "@/lib/providers/query-provider";
import { GoogleMapsProvider } from "@/components/maps";
import { Toaster } from "@/components/ui/toaster";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Reamp - Real Estate Media Platform",
  description: "Professional real estate photography and media management platform connecting agencies with photography studios",
  keywords: ["real estate", "photography", "property", "media", "listings"],
  openGraph: {
    title: "Reamp - Real Estate Media Platform",
    description: "Professional real estate photography and media management platform",
    type: "website",
    locale: "en_US",
    siteName: "Reamp",
  },
  twitter: {
    card: "summary_large_image",
    title: "Reamp - Real Estate Media Platform",
    description: "Professional real estate photography and media management platform",
  },
  robots: {
    index: true,
    follow: true,
    googleBot: {
      index: true,
      follow: true,
      "max-video-preview": -1,
      "max-image-preview": "large",
      "max-snippet": -1,
    },
  },
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className={`${geistSans.variable} ${geistMono.variable} antialiased`}>
        <QueryProvider>
          <GoogleMapsProvider>{children}</GoogleMapsProvider>
        </QueryProvider>
        <Toaster />
      </body>
    </html>
  );
}
