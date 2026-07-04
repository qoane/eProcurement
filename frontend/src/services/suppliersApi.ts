import { apiGet, apiPost } from "./apiClient";
import type { Supplier } from "../types/api";
export const getSuppliers = () => apiGet<Supplier[]>("/api/suppliers", []);
export async function getSupplier(ref: string) {
  const r = await getSuppliers();
  return { ...r, data: r.data.find((s) => s.referenceNumber === ref) };
}
export const submitSupplier = (ref: string) =>
  apiPost(`/api/suppliers/${ref}/submit`, { actor: "admin@lca.org.ls" }, null);
