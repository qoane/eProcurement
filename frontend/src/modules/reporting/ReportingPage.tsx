import { useEffect, useMemo, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { AdminCard } from "../../components/ui/AdminCard";
import { InfoBox } from "../../components/ui/InfoBox";
import { DataTable } from "../../components/ui/DataTable";
import { EmptyState } from "../../components/ui/EmptyState";
import { Input } from "../../components/ui/Input";
import { Select } from "../../components/ui/Select";
import { getReportingDashboard, reportCatalog, type ReportingDashboard, type ReportingFilters } from "../../services/reportingApi";

const titleByCode: Record<string, string> = { ...Object.fromEntries(reportCatalog), "procurement-activity": "Procurement activity" };
const apiCode = (code: string) => code === "procurement-activity" ? "executive-dashboard" : code;
const currency = new Intl.NumberFormat(undefined, { style: "currency", currency: "LSL", maximumFractionDigits: 0 });
function formatMetric(value: number, unit: string) {
  if (unit === "currency") return currency.format(value);
  if (unit === "days") return `${value} days`;
  return value.toLocaleString();
}
function FilterBar({ filters, setFilters }: { filters: ReportingFilters; setFilters: (f: ReportingFilters) => void }) {
  return <AdminCard title="Filters" subtitle="Apply common procurement report filters. Blank filters include all available data.">
    <div className="grid cols-3">
      <label>Financial year ID<Input value={filters.financialYearId ?? ""} onChange={(e) => setFilters({ ...filters, financialYearId: e.target.value })} /></label>
      <label>Department<Input value={filters.department ?? ""} onChange={(e) => setFilters({ ...filters, department: e.target.value })} /></label>
      <label>Category<Input value={filters.category ?? ""} onChange={(e) => setFilters({ ...filters, category: e.target.value })} /></label>
      <label>Supplier ID<Input value={filters.supplierId ?? ""} onChange={(e) => setFilters({ ...filters, supplierId: e.target.value })} /></label>
      <label>From<Input type="date" value={filters.from ?? ""} onChange={(e) => setFilters({ ...filters, from: e.target.value })} /></label>
      <label>To<Input type="date" value={filters.to ?? ""} onChange={(e) => setFilters({ ...filters, to: e.target.value })} /></label>
      <label>Status<Select value={filters.status ?? ""} onChange={(e) => setFilters({ ...filters, status: e.target.value })}>{[{ label: "All statuses", value: "" }, { label: "Draft", value: "Draft" }, { label: "Submitted", value: "Submitted" }, { label: "Approved", value: "Approved" }, { label: "Published", value: "Published" }, { label: "Closed", value: "Closed" }].map((o) => <option key={o.value} value={o.value}>{o.label}</option>)}</Select></label>
    </div>
  </AdminCard>;
}
function ReportView({ code, filters }: { code: string; filters: ReportingFilters }) {
  const resolvedCode = apiCode(code);
  const [report, setReport] = useState<ReportingDashboard>({ code: resolvedCode, title: titleByCode[code], metrics: [], rows: [] });
  useEffect(() => { void getReportingDashboard(resolvedCode, titleByCode[code], filters).then((r) => setReport(r.data)); }, [resolvedCode, code, filters]);
  const exportPath = `/api/reporting/${resolvedCode}/export.csv`;
  return <>
    <div className="grid cols-4 dashboard-section">
      {report.metrics.map((m, i) => <InfoBox key={m.code} icon={i % 2 ? "📈" : "📊"} label={m.label} value={formatMetric(m.value, m.unit)} variant={m.unit === "currency" ? "success" : "primary"} />)}
    </div>
    {report.metrics.length === 0 && <EmptyState title="No report data" message="No procurement records match the selected filters yet." />}
    <AdminCard title={`${report.title} details`} subtitle="CSV export is available now; the report contract can add PDF rendering later.">
      <p><a href={exportPath}>Export CSV</a></p>
      <DataTable rows={report.rows} striped compact columns={[{ header: "Area", cell: (r) => r.area, sortable: true }, { header: "Metric", cell: (r) => r.metric, sortable: true }, { header: "Value", cell: (r) => r.value, sortable: true }, { header: "Source", cell: (r) => r.source, sortable: true }]} />
    </AdminCard>
  </>;
}
export function ReportingPage({ reportCode = "executive-dashboard" }: { reportCode?: string }) {
  const [filters, setFilters] = useState<ReportingFilters>({});
  const stableFilters = useMemo(() => filters, [JSON.stringify(filters)]);
  return <>
    <PageHeader title={reportCode === "executive-dashboard" ? "Executive dashboard" : titleByCode[reportCode] ?? "Reporting"} description="Live procurement intelligence from planning, requisitions, tendering, suppliers, purchase orders, compliance and audit data." />
    <FilterBar filters={filters} setFilters={setFilters} />
    <ReportView code={reportCode} filters={stableFilters} />
  </>;
}
export function DashboardsPage() { return <ReportingPage reportCode="executive-dashboard" />; }
