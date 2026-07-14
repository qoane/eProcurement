import { apiGet } from "./apiClient";

export type ReportingMetric = { code: string; label: string; value: number; unit: string; description?: string; status?: string; drilldownUrl?: string };
export type ReportRow = { area: string; metric: string; value: string; source: string };
export type ReportingDashboard = { code: string; title: string; metrics: ReportingMetric[]; rows: ReportRow[]; lastRefreshedAt?: string; warnings?: string[] };
export type ReportingFilters = { financialYearId?: string; department?: string; category?: string; supplierId?: string; from?: string; to?: string; status?: string };
const empty = (code: string, title: string): ReportingDashboard => ({ code, title, metrics: [], rows: [] });
function qs(filters: ReportingFilters = {}) {
  const p = new URLSearchParams();
  Object.entries(filters).forEach(([k, v]) => { if (v) p.set(k, v); });
  const s = p.toString();
  return s ? `?${s}` : "";
}
export function getReportingDashboard(code: string, title: string, filters?: ReportingFilters) {
  return apiGet<ReportingDashboard>(`/api/reporting/${code}${qs(filters)}`, empty(code, title));
}
export const reportCatalog = [
  ["executive-dashboard", "Executive dashboard"],
  ["procurement-activity", "Procurement activity"],
  ["procurement-planning", "Procurement planning"],
  ["requisitions", "Requisitions"],
  ["tenders", "Tenders"],
  ["supplier-performance", "Supplier performance"],
  ["purchase-orders", "Purchase orders"],
  ["po-invoice-analysis", "PO and invoice analysis"],
  ["spend-analysis", "Spend analysis"],
  ["compliance", "Compliance"],
  ["audit", "Audit"],
  ["workflow-bottlenecks", "Workflow bottlenecks"],
  ["management-kpis", "Management KPIs"],
  ["rfp-evidence", "RFP evidence center"],
] as const;
