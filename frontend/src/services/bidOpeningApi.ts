import { apiGet, apiPost } from "./apiClient";

export type BidOpeningSummary = { id: string; sessionNumber: string; tenderId: string; tenderNumber: string; tenderTitle: string; scheduledAt: string; status: string; chairperson: string; submissionCount: number };
export type BidOpeningDetail = { session: BidOpeningSession; tender: { id: string; tenderNumber: string; title: string; closingDate: string; status: string }; committee: BidOpeningCommitteeMember[]; checklist: BidOpeningChecklistItem[]; submissions: BidOpeningSubmission[]; minutes: BidOpeningMinute[]; reports: BidOpeningReport[]; auditTimeline: { id: string; eventType: string; actor: string; details: string; occurredAt: string }[]; workflowInstance?: { currentNodeCode: string; status: string } | null };
export type BidOpeningSession = { id: string; sessionNumber: string; tenderId: string; title: string; scheduledAt: string; startedAt?: string; completedAt?: string; status: string; createdBy: string; createdAt: string; chairperson: string; notes?: string };
export type BidOpeningCommitteeMember = { id: string; name: string; email: string; role: string; attendanceConfirmed: boolean; confirmedAt?: string };
export type BidOpeningChecklistItem = { id: string; description: string; completed: boolean; completedBy?: string; completedAt?: string };
export type BidOpeningSubmission = { id: string; bidSubmissionId: string; supplierId: string; supplierName: string; submissionNumber: string; submittedAt?: string; status: string; openedAt?: string; openedBy?: string; notes?: string };
export type BidOpeningMinute = { id: string; minuteText: string; recordedBy: string; recordedAt: string };
export type BidOpeningReport = { id: string; reportNumber: string; generatedAt: string; generatedBy: string; summaryJson: string };

const actor = { actor: "chair@lca.org.ls" };
export const getBidOpenings = () => apiGet<BidOpeningSummary[]>("/api/bid-opening", []);
export const getBidOpening = (id: string) => apiGet<BidOpeningDetail | null>(`/api/bid-opening/${id}`, null);
export const createBidOpening = (body: unknown) => apiPost<BidOpeningDetail>("/api/bid-opening", body);
export const scheduleBidOpening = (id: string) => apiPost<BidOpeningDetail>(`/api/bid-opening/${id}/schedule`, actor);
export const startBidOpening = (id: string) => apiPost<BidOpeningDetail>(`/api/bid-opening/${id}/start`, actor);
export const openBidSubmission = (id: string, bidSubmissionId: string) => apiPost<BidOpeningDetail>(`/api/bid-opening/${id}/open-submission/${bidSubmissionId}`, actor);
export const completeBidOpening = (id: string) => apiPost<BidOpeningDetail>(`/api/bid-opening/${id}/complete`, actor);
export const referBidOpening = (id: string) => apiPost<BidOpeningDetail>(`/api/bid-opening/${id}/refer-to-evaluation`, actor);
export const cancelBidOpening = (id: string) => apiPost<BidOpeningDetail>(`/api/bid-opening/${id}/cancel`, actor);
export const addBidOpeningMinute = (id: string, minuteText: string) => apiPost<BidOpeningMinute>(`/api/bid-opening/${id}/minutes`, { minuteText, recordedBy: actor.actor });
