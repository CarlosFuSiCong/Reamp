import { AccountStatus } from "./enums";

export interface Agency {
  id: string;
  name: string;
  abn?: string;
  email: string;
  phone?: string;
  addressLine1?: string;
  addressLine2?: string;
  city?: string;
  state?: string;
  postcode?: string;
  country?: string;
  status: AccountStatus;
  createdAtUtc: string;
  updatedAtUtc: string;
}

export interface AgencyDetail extends Agency {
  branchCount: number;
  branches?: AgencyBranch[];
}

export interface AgencyBranch {
  id: string;
  agencyId: string;
  name: string;
  email: string;
  phone?: string;
  addressLine1: string;
  addressLine2?: string;
  city: string;
  state: string;
  postcode: string;
  country: string;
  status: AccountStatus;
  createdAtUtc: string;
  updatedAtUtc: string;
}

export interface Client {
  id: string;
  agencyId: string;
  email: string;
  firstName: string;
  lastName: string;
  phone?: string;
  status: AccountStatus;
  createdAtUtc: string;
  updatedAtUtc: string;
}
