import { apiGet, apiPost, apiPut } from "./apiClient";

export type BidSubmission = { id: string; submissionNumber: string; tenderId: string; supplierId: string; status: string; submissionDate?: string; submittedBy: string; submittedAt?: string; lockedAt?: string; currentVersion: number; items: BidSubmissionItem[]; documents: BidSubmissionDocument[]; history: BidSubmissionHistory[]; declarations: BidSubmissionDeclaration[]; statusHistory: BidSubmissionStatusHistory[] };
export type BidSubmissionItem = { id: string; description: string; quantity: number; unitPrice: number; total: number; notes?: string };
export type BidSubmissionDocument = { id: string; documentType: string; filename: string; storageReference: string; uploadedBy: string; uploadedAt: string; version: number };
export type BidSubmissionHistory = { id: string; eventType: string; actor: string; details: string; occurredAt: string };
export type BidSubmissionDeclaration = { id: string; declarationType: string; accepted: boolean; acceptedBy: string; acceptedAt: string };
export type BidSubmissionStatusHistory = { id: string; fromStatus: string; toStatus: string; actor: string; changedAt: string; notes: string };
export const getBids = () => apiGet<BidSubmission[]>("/api/bids", []);
export const getBid = (id: string) => apiGet<BidSubmission | null>(`/api/bids/${id}`, null);
export const createBid = (body: unknown) => apiPost<BidSubmission>("/api/bids", body);
export const updateBid = (id: string, body: unknown) => apiPut<BidSubmission>(`/api/bids/${id}`, body, {} as BidSubmission);
export const submitBid = (id: string) => apiPost<BidSubmission>(`/api/bids/${id}/submit`, { actor: "supplier@demo.ls" });
export const withdrawBid = (id: string) => apiPost<BidSubmission>(`/api/bids/${id}/withdraw`, { actor: "supplier@demo.ls" });
export const uploadBidDocument = (id: string, body: unknown) => apiPost<BidSubmissionDocument>(`/api/bids/${id}/documents`, body);
