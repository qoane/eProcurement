import { apiGet, apiPost } from "./apiClient";
export type AnyRecord = Record<string, unknown>;
export const dataGovernanceApi = {
  dashboard: () => apiGet<AnyRecord>("/api/data-governance", {}),
  policies: () => apiGet<AnyRecord[]>("/api/data-governance/policies", []),
  retention: () => apiGet<AnyRecord[]>("/api/data-governance/retention-rules", []),
  privacy: () => apiGet<AnyRecord[]>("/api/data-governance/privacy-classifications", []),
  logs: () => apiGet<AnyRecord[]>("/api/data-governance/processing-logs", []),
  templates: () => apiGet<AnyRecord[]>("/api/migration/templates", []),
  batches: () => apiGet<AnyRecord[]>("/api/migration/batches", []),
  batch: (id: string) => apiGet<AnyRecord>(`/api/migration/batches/${id}`, {}),
  qualityChecks: () => apiGet<AnyRecord[]>("/api/data-quality/checks", []),
  qualityResults: () => apiGet<AnyRecord[]>("/api/data-quality/results", []),
  runQuality: () => apiPost<AnyRecord[]>("/api/data-quality/run", {}, []),
  archiveBatches: () => apiGet<AnyRecord[]>("/api/data-archive/batches", []),
  exports: () => apiGet<AnyRecord[]>("/api/data-exports", []),
  requestExport: (body: AnyRecord) => apiPost<AnyRecord>("/api/data-exports", body, {}),
};
