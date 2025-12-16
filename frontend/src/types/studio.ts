import { AccountStatus, StudioRole } from "./enums";

export interface Studio {
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

export interface Staff {
  id: string;
  studioId: string;
  email: string;
  firstName: string;
  lastName: string;
  phone?: string;
  role: StudioRole;
  status: AccountStatus;
  createdAtUtc: string;
  updatedAtUtc: string;
}
