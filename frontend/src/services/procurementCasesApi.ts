import { apiGet } from "./apiClient";
export type ProcurementCaseSummary = { id: string; caseNumber: string; title: string; currentLifecycleStage: string; tender?: string | null; supplierIfAwarded?: string | null; totalValue: number; status: string };
export type ProcurementCaseStage = { stage: string; recordNumber?: string | null; status: string; responsibleRole: string; date?: string | null; url?: string | null };
export type ProcurementCaseLink = { id: string; entityType: string; entityId: string; entityReference: string; relationshipType: string; createdAt: string };
export type ProcurementCaseDocument = { documentType: string; fileName: string; source: string; url?: string | null; date: string };
export type ProcurementCaseDetail = { case: { id: string; caseNumber: string; title: string; description: string; department: string; status: string; createdAt: string; createdBy: string }; links: ProcurementCaseLink[]; timeline: ProcurementCaseStage[]; auditTimeline: { id: string; eventType: string; entityReference: string; actor: string; details: string; occurredAt: string }[]; documents: ProcurementCaseDocument[]; notifications: { id: string; subject: string; status: string; createdAt: string }[]; summary: { publicNoticeLink?: string | null; tender?: string | null } };
export const getProcurementCases = () => apiGet<ProcurementCaseSummary[]>("/api/procurement-cases", []);
export const getProcurementCase = (id: string) => apiGet<ProcurementCaseDetail | null>(`/api/procurement-cases/${id}`, null);
