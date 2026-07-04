import { apiGet, apiPost } from "./apiClient";
import type { Rule } from "../types/api";
export const getRules = () => apiGet<Rule[]>("/api/business-rules", []);
export const createRule = (b: unknown) =>
  apiPost("/api/business-rules", b, null);
