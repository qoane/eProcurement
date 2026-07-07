import { apiGet, apiPost } from "./apiClient";

export type TenderSummary = { id: string; tenderNumber: string; title: string; tenderType: string; procurementMethod: string; status: string; publicationDate?: string | null; closingDate: string };
export type TenderLot = { id: string; lotNumber: string; title: string; description: string };
export type TenderDocument = { id: string; documentType: string; fileName: string; description: string; isRequired: boolean };
export type TenderClarificationResponse = { id: string; response: string; respondedBy: string; respondedAt: string };
export type TenderClarification = { id: string; question: string; askedBy: string; askedAt: string; isPublic: boolean; responses: TenderClarificationResponse[] };
export type TenderDetail = { tender: TenderSummary & { description: string; createdBy: string; createdAt: string; publishedBy?: string | null; publishedAt?: string | null }; lots: TenderLot[]; documents: TenderDocument[]; supplierInvitations: { id: string; supplierName: string; supplierEmail: string; notifiedAt?: string | null }[]; clarifications: TenderClarification[]; statusHistory: { id: string; fromStatus: string; toStatus: string; actor: string; changedAt: string; notes: string }[]; auditTimeline: { id: string; eventType: string; actor: string; details: string; occurredAt: string }[] };
export type CreateTender = { tenderNumber: string; title: string; description: string; tenderType: string; procurementMethod: string; closingDate: string; createdBy: string; lots: { lotNumber: string; title: string; description: string }[]; documents: { documentType: string; fileName: string; description: string; isRequired: boolean }[]; supplierInvitations: { supplierName: string; supplierEmail: string }[] };

export const getTenders = () => apiGet<TenderSummary[]>("/api/tenders", []);
export const getTender = (id: string) => apiGet<TenderDetail | null>(`/api/tenders/${id}`, null);
export const createTender = (body: CreateTender) => apiPost<TenderDetail>("/api/tenders", body);
export const publishTender = (id: string) => apiPost<TenderDetail>(`/api/tenders/${id}/publish`, { actor: "procurement@lca.org.ls" });
export const cancelTender = (id: string) => apiPost<TenderDetail>(`/api/tenders/${id}/cancel`, { actor: "procurement@lca.org.ls" });
export const createClarification = (id: string, question: string, askedBy: string) => apiPost<TenderClarification>(`/api/tenders/${id}/clarifications`, { question, askedBy, isPublic: true });
export const respondClarification = (id: string, clarificationId: string, response: string) => apiPost<TenderClarificationResponse>(`/api/tenders/${id}/clarifications/${clarificationId}/respond`, { response, respondedBy: "procurement@lca.org.ls" });
