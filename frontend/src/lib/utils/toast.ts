import { toast } from "sonner";

/**
 * Toast notification utilities for consistent UX feedback
 */

export const showToast = {
  /**
   * Success notification
   */
  success: (message: string, description?: string) => {
    toast.success(message, {
      description,
      duration: 4000,
    });
  },

  /**
   * Error notification
   */
  error: (message: string, description?: string) => {
    toast.error(message, {
      description,
      duration: 5000,
    });
  },

  /**
   * Info notification
   */
  info: (message: string, description?: string) => {
    toast.info(message, {
      description,
      duration: 4000,
    });
  },

  /**
   * Warning notification
   */
  warning: (message: string, description?: string) => {
    toast.warning(message, {
      description,
      duration: 4500,
    });
  },

  /**
   * Loading notification with promise
   */
  promise: <T,>(
    promise: Promise<T>,
    messages: {
      loading: string;
      success: string;
      error: string;
    }
  ) => {
    return toast.promise(promise, messages);
  },

  /**
   * Custom notification
   */
  custom: (message: string, options?: Parameters<typeof toast>[1]) => {
    toast(message, options);
  },
};

/**
 * Common toast messages for the application
 */
export const toastMessages = {
  // Listings
  listing: {
    created: "Listing created successfully",
    updated: "Listing updated successfully",
    deleted: "Listing deleted successfully",
    published: "Listing published successfully",
    archived: "Listing archived successfully",
    error: "Failed to process listing",
  },

  // Orders
  order: {
    created: "Order created successfully",
    updated: "Order updated successfully",
    deleted: "Order cancelled successfully",
    assigned: "Staff assigned successfully",
    statusChanged: "Order status updated",
    error: "Failed to process order",
  },

  // Profile
  profile: {
    updated: "Profile updated successfully",
    error: "Failed to update profile",
  },

  // Auth
  auth: {
    loginSuccess: "Welcome back!",
    loginError: "Login failed",
    logoutSuccess: "Logged out successfully",
    registerSuccess: "Account created successfully",
    registerError: "Registration failed",
  },

  // Media
  media: {
    uploaded: "Media uploaded successfully",
    deleted: "Media deleted successfully",
    error: "Failed to process media",
  },

  // Deliveries
  delivery: {
    created: "Delivery package created successfully",
    updated: "Delivery updated successfully",
    error: "Failed to process delivery",
  },

  // Applications
  application: {
    submitted: "Application submitted successfully",
    approved: "Application approved",
    rejected: "Application rejected",
    error: "Failed to process application",
  },

  // Team
  team: {
    memberInvited: "Team member invited successfully",
    memberRemoved: "Team member removed successfully",
    invitationAccepted: "Invitation accepted",
    error: "Failed to process team action",
  },

  // Generic
  generic: {
    success: "Operation completed successfully",
    error: "An error occurred",
    saved: "Changes saved successfully",
    copied: "Copied to clipboard",
    networkError: "Network error. Please check your connection",
  },
};
