import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { MetricCard } from "../../components/ui/MetricCard";
import { getSuppliers } from "../../services/suppliersApi";
import { getTasks } from "../../services/tasksApi";
import { getAuditEvents } from "../../services/auditApi";
export function ReportingPage() {
  const [c, setC] = useState([0, 0, 0]);
  useEffect(() => {
    void Promise.all([getSuppliers(), getTasks(), getAuditEvents()]).then((r) =>
      setC(r.map((x) => x.data.length)),
    );
  }, []);
  return (
    <>
      <PageHeader
        title="Reporting"
        description="Reporting shell using real API counts only."
      />
      <div className="grid cols-3">
        <MetricCard label="Supplier records" value={c[0]} />
        <MetricCard label="Workflow tasks" value={c[1]} />
        <MetricCard label="Audit events" value={c[2]} />
      </div>
    </>
  );
}
