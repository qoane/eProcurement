import { apiGet, apiPost } from "./apiClient";

export type TenderSummary = { id: string; tenderNumber: string; title: string; tenderType: string; procurementMethod: string; status: string; publicationDate?: string | null; closingDate: string };
export type TenderLot = { id: string; lotNumber: string; title: string; description: string };
export type TenderDocument = { id: string; documentType: string; fileName: string; description: string; isRequired: boolean; isPublic?: boolean; isDownloadable?: boolean; publicUrl?: string | null };
export type TenderClarificationResponse = { id: string; response: string; respondedBy: string; respondedAt: string; isPublished?: boolean; publishedAt?: string | null; publishedBy?: string | null };
export type TenderClarification = { id: string; question: string; askedBy: string; askedAt: string; isPublic: boolean; status?: string; visibility?: string; supplierName?: string; questionReference?: string; assignedOfficer?: string | null; responses: TenderClarificationResponse[] };
export type TenderClarificationWorkspace = { id: string; tenderId: string; tenderNumber: string; tenderTitle: string; questionReference: string; supplierName: string; askedAt: string; status: string; visibility: string; responseStatus: string; assignedOfficer?: string | null };
export type TenderDetail = { tender: TenderSummary & { description: string; createdBy: string; createdAt: string; publishedBy?: string | null; publishedAt?: string | null }; lots: TenderLot[]; documents: TenderDocument[]; supplierInvitations: { id: string; supplierName: string; supplierEmail: string; notifiedAt?: string | null }[]; clarifications: TenderClarification[]; statusHistory: { id: string; fromStatus: string; toStatus: string; actor: string; changedAt: string; notes: string }[]; auditTimeline: { id: string; eventType: string; actor: string; details: string; occurredAt: string }[] };
export type CreateTender = { tenderNumber: string; title: string; description: string; tenderType: string; procurementMethod: string; closingDate: string; createdBy: string; lots: { lotNumber: string; title: string; description: string }[]; documents: { documentType: string; fileName: string; description: string; isRequired: boolean }[]; supplierInvitations: { supplierName: string; supplierEmail: string }[] };

export const getTenders = () => apiGet<TenderSummary[]>("/api/tenders", []);
export const getTender = (id: string) => apiGet<TenderDetail | null>(`/api/tenders/${id}`, null);
export const getClarificationWorkspace = () => apiGet<TenderClarificationWorkspace[]>("/api/tenders/clarifications", []);
export const createTender = (body: CreateTender) => apiPost<TenderDetail>("/api/tenders", body);
export const publishTender = (id: string) => apiPost<TenderDetail>(`/api/tenders/${id}/publish`, { actor: "procurement@lca.org.ls" });
export const cancelTender = (id: string) => apiPost<TenderDetail>(`/api/tenders/${id}/cancel`, { actor: "procurement@lca.org.ls" });
export const createClarification = (id: string, question: string, askedBy: string) => apiPost<TenderClarification>(`/api/tenders/${id}/clarifications`, { question, askedBy, isPublic: true });
export const respondClarification = (id: string, clarificationId: string, response: string, isPublic = false, publish = false) => apiPost<TenderClarificationResponse>(`/api/tenders/${id}/clarifications/${clarificationId}/respond`, { response, respondedBy: "procurement@lca.org.ls", isPublic, publish });
