import { apiGet, apiPost, apiPut } from "./apiClient";

export type SupplierDashboard = { profileStatus: string; missingDocuments: string[]; openOpportunities: number; draftBids: number; submittedBids: number; clarificationsAwaitingResponse: number; unreadNotifications: number };
export type SupplierProfile = { id: string; referenceNumber: string; legalName: string; tradingName?: string; registrationNumber?: string; taxNumber?: string; contactPerson?: string; email?: string; phone?: string; address?: string; status: string; categories: string[]; documents: { id: string; documentType: string; fileName: string; uploadedAt: string }[] };
export type Opportunity = { reference: string; title: string; tenderType: string; procurementMethod: string; category: string; publishedAt: string; closingDate: string; status: string; slug: string; tenderNumber: string; publicUrl: string };
export type OpportunityDetail = Opportunity & { description: string; documents: { documentType: string; fileName: string; publicUrl: string; isDownloadable: boolean; publishedAt: string }[]; clarifications: { question: string; response: string; publishedAt: string }[] };
export type Bid = { id: string; submissionNumber: string; tenderId: string; supplierId: string; status: string; submissionDate?: string; submittedBy: string; submittedAt?: string; items: { description: string; quantity: number; unitPrice: number; total: number }[]; documents: { documentType: string; filename: string }[] };
export type Clarification = { id: string; tenderNumber: string; tenderTitle: string; question: string; askedAt: string; responses: { response: string; respondedAt: string }[] };
export type Notification = { id: string; subject: string; body: string; status: string; createdAt: string; relatedUrl?: string };

export const supplierDashboard = () => apiGet<SupplierDashboard>("/api/supplier-portal/dashboard", { profileStatus: "", missingDocuments: [], openOpportunities: 0, draftBids: 0, submittedBids: 0, clarificationsAwaitingResponse: 0, unreadNotifications: 0 });
export const supplierProfile = () => apiGet<SupplierProfile>("/api/supplier-portal/profile", {} as SupplierProfile);
export const updateSupplierProfile = (body: Partial<SupplierProfile>) => apiPut<SupplierProfile>("/api/supplier-portal/profile", body, {} as SupplierProfile);
export const supplierDocuments = () => apiGet<SupplierProfile["documents"]>("/api/supplier-portal/documents", []);
export const supplierOpportunities = () => apiGet<Opportunity[]>("/api/supplier-portal/opportunities", []);
export const supplierOpportunity = (ref: string) => apiGet<OpportunityDetail>(`/api/supplier-portal/opportunities/${encodeURIComponent(ref)}`, {} as OpportunityDetail);
export const askSupplierClarification = (ref: string, question: string) => apiPost(`/api/supplier-portal/opportunities/${encodeURIComponent(ref)}/clarifications`, { question });
export const supplierClarifications = () => apiGet<Clarification[]>("/api/supplier-portal/clarifications", []);
export const supplierBids = () => apiGet<Bid[]>("/api/supplier-portal/bids", []);
export const supplierBid = (id: string) => apiGet<Bid>(`/api/supplier-portal/bids/${id}`, {} as Bid);
export const createSupplierBid = (tenderId: string) => apiPost<Bid>("/api/supplier-portal/bids", { tenderId, items: [], declarations: [{ declarationType: "Supplier declaration", accepted: true, acceptedBy: "supplier" }] });
export const supplierNotifications = () => apiGet<Notification[]>("/api/supplier-portal/notifications", []);
