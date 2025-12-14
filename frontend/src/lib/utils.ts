import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export * from "./utils/enum-labels";
export * from "./utils/activity-utils";
export * from "./utils/role-utils";
export * from "./utils/mutation-utils";
