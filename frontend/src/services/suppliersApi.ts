import { apiGet, apiPost } from "./apiClient";
import type { Supplier } from "../types/api";
export const getSuppliers = () => apiGet<Supplier[]>("/api/suppliers", []);
export const getSupplier = (ref: string) => apiGet(`/api/suppliers/${encodeURIComponent(ref)}`, null);
export const getSupplierRegistrationConfiguration = () => apiGet("/api/suppliers/registration/configuration", null);
export const registerSupplier = (body: unknown) => apiPost("/api/suppliers/registration", body, null);
export const submitSupplier = (ref: string) =>
  apiPost(`/api/suppliers/${ref}/submit`, { actor: "admin@lca.org.ls" }, null);
