import { apiGet, apiPost, apiPut, apiDelete } from "./apiClient";
export type DocumentSummary = { id:string; documentNumber:string; entityType:string; entityId:string; documentType:string; title:string; classification:string; status:string; currentVersion:number; isPublic:boolean; uploadedAt:string };
export type DocumentDetail = { record: DocumentSummary & { description?: string }; versions: DocumentVersion[]; currentVersion?: DocumentVersion };
export type DocumentVersion = { id:string; versionNumber:number; originalFileName:string; contentType:string; fileSize:number; contentHash:string; isCurrent:boolean; uploadedAt:string };
export type DocumentRetentionPolicy = { id?: string; entityType:string; documentType:string; retentionPeriodDays:number; archiveAfterDays:number; deleteAfterDays?:number; isActive:boolean; actor:string };
export type RequirementValidation = { passed:boolean; errors:string[]; warnings:string[]; items:{ documentType:string; minimumFiles:number; maximumFiles:number; currentFiles:number; passed:boolean; message?:string }[] };
export const documentsApi = {
  list: (query = "") => apiGet<DocumentSummary[]>(`/api/documents${query}`, []),
  detail: (id: string) => apiGet<DocumentDetail | null>(`/api/documents/${id}`, null),
  versions: (id: string) => apiGet<DocumentVersion[]>(`/api/documents/${id}/versions`, []),
  publish: (id: string) => apiPost(`/api/documents/${id}/publish`, {}, null),
  unpublish: (id: string) => apiPost(`/api/documents/${id}/unpublish`, {}, null),
  archive: (id: string, reason = "Manual archive") => apiPost(`/api/documents/${id}/archive`, { reason }, null),
  delete: (id: string) => apiDelete(`/api/documents/${id}`),
  accessLog: (id: string) => apiGet(`/api/documents/${id}/access-log`, []),
  retention: () => apiGet<DocumentRetentionPolicy[]>(`/api/document-retention-policies`, []),
  saveRetention: (policy: DocumentRetentionPolicy) => policy.id ? apiPut(`/api/document-retention-policies/${policy.id}`, policy, policy) : apiPost(`/api/document-retention-policies`, policy, policy),
  validate: (entityType: string, entityId: string, setId: string) => apiGet<RequirementValidation>(`/api/document-requirements/${entityType}/${entityId}/validate?documentRequirementSetId=${setId}`, { passed:false, errors:[], warnings:[], items:[] }),
};
