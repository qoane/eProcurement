import { apiGet } from "./apiClient";
import type { AuditEvent } from "../types/api";
export const getAuditEvents = () =>
  apiGet<AuditEvent[]>("/api/audit-events", []);
