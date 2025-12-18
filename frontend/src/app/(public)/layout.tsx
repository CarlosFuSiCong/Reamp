import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Browse Properties - Reamp",
  description: "Browse our curated collection of professionally photographed real estate properties.",
  openGraph: {
    title: "Browse Properties - Reamp",
    description: "Find your dream property from our professional listings.",
    type: "website",
  },
};

export default function PublicLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return children;
}

