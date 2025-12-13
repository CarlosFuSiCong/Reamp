import { create } from "zustand";
import { persist, createJSONStorage, StateStorage } from "zustand/middleware";
import { User } from "@/types";
import { getCookie, setCookie, deleteCookie } from "@/lib/utils/cookies";

const cookieStorage: StateStorage = {
  getItem: (name: string): string | null => {
    const value = getCookie(name);
    return value ? decodeURIComponent(value) : null;
  },
  setItem: (name: string, value: string): void => {
    setCookie(name, encodeURIComponent(value), 7);
  },
  removeItem: (name: string): void => {
    deleteCookie(name);
  },
};

interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  setUser: (user: User | null) => void;
  setLoading: (loading: boolean) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      isAuthenticated: false,
      isLoading: true,

      setUser: (user) =>
        set({
          user,
          isAuthenticated: !!user,
          isLoading: false,
        }),

      setLoading: (loading) =>
        set({
          isLoading: loading,
        }),

      logout: () =>
        set({
          user: null,
          isAuthenticated: false,
          isLoading: false,
        }),
    }),
    {
      name: "reamp-auth-storage",
      storage: createJSONStorage(() => cookieStorage),
      onRehydrateStorage: () => (state) => {
        state?.setLoading(false);
      },
    }
  )
);
