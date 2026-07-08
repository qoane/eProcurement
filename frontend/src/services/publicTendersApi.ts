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
  downloadUrl?: string;
};

export type PublicTenderClarification = {
  id?: string;
  question?: string;
  answer?: string;
  response?: string;
  askedAt?: string;
  respondedAt?: string;
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
export const getPublicTenderCalendar = () =>
  apiGet<unknown[]>("/api/public/tender-calendar", []);

export const getLatestPublicTenders = (take = 5) =>
  apiGet<PublicTenderSummary[]>(`/api/public/widgets/latest-tenders?take=${encodeURIComponent(String(take))}`, []);
