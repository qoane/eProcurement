import { apiGet } from "./apiClient";

export type PublicTenderCategory = {
  id?: string;
  code?: string;
  name?: string;
  title?: string;
  description?: string;
};

export type PublicTenderDocument = {
  id?: string;
  fileName?: string;
  name?: string;
  title?: string;
  description?: string;
  documentType?: string;
  url?: string;
  publicUrl?: string;
  downloadUrl?: string;
};

export type PublicTenderClarification = {
  id?: string;
  question?: string;
  answer?: string;
  response?: string;
  askedAt?: string;
  respondedAt?: string;
  publishedAt?: string;
  isPublic?: boolean;
  responses?: { id?: string; response?: string; respondedAt?: string }[];
};

export type PublicTenderSummary = {
  id?: string;
  reference?: string;
  referenceNumber?: string;
  tenderNumber?: string;
  title: string;
  tenderType?: string;
  procurementMethod?: string;
  category?: string;
  categoryName?: string;
  status?: string;
  publicUrl?: string;
  bidSubmissionUrl?: string;
  publishedAt?: string | null;
  publishedDate?: string | null;
  publicationDate?: string | null;
  closingDate?: string | null;
  description?: string;
};

export type PublicTenderDetail = PublicTenderSummary & {
  documents?: PublicTenderDocument[];
  publicDocuments?: PublicTenderDocument[];
  clarifications?: PublicTenderClarification[];
  publicClarifications?: PublicTenderClarification[];
};

export const getPublicTenders = () =>
  apiGet<PublicTenderSummary[]>("/api/public/tenders", []);
export const getPublicTender = (reference: string) =>
  apiGet<PublicTenderDetail | null>(
    `/api/public/tenders/${encodeURIComponent(reference)}`,
    null,
  );
export const getPublicTenderCategories = () =>
  apiGet<PublicTenderCategory[]>("/api/public/tender-categories", []);
export type PublicTenderCalendarItem = { reference: string; title: string; publishedAt?: string | null; closingDate?: string | null; category?: string; status?: string };
export type PublicAwardNotice = { awardNumber: string; tenderNumber: string; tenderTitle: string; supplierName: string; awardAmount: number; currency?: string; status?: string; publishedAt?: string | null; createdAt?: string | null };

export const getPublicTenderCalendar = () =>
  apiGet<PublicTenderCalendarItem[]>("/api/public/tender-calendar", []);

export const getLatestPublicTenders = (take = 5) =>
  apiGet<PublicTenderSummary[]>(
    `/api/public/widgets/latest-tenders?take=${encodeURIComponent(String(take))}`,
    [],
  );

export const getPublicAwardNotices = () =>
  apiGet<PublicAwardNotice[]>("/api/public/awards", []);
