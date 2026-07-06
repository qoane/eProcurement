import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { AdminCard } from "../../components/ui/AdminCard";
import { InfoBox } from "../../components/ui/InfoBox";
import { DataTable } from "../../components/ui/DataTable";
import { getSuppliers } from "../../services/suppliersApi";
import { getTasks } from "../../services/tasksApi";
import { getAuditEvents } from "../../services/auditApi";

export function ReportingPage() {
  const [counts, setCounts] = useState([0, 0, 0]);

  useEffect(() => {
    void Promise.all([getSuppliers(), getTasks(), getAuditEvents()]).then(
      (results) => setCounts(results.map((result) => result.data.length)),
    );
  }, []);

  const rows = [
    {
      area: "Supplier Management",
      metric: "Supplier records",
      value: counts[0],
      source: "SupplierManagement API",
    },
    {
      area: "WorkflowEngine",
      metric: "Workflow tasks",
      value: counts[1],
      source: "Workflow task API",
    },
    {
      area: "AuditEngine",
      metric: "Audit events",
      value: counts[2],
      source: "Audit event API",
    },
  ];

  return (
    <>
      <PageHeader
        title="Reporting"
        description="LCA eProcurement reporting shell using live API counts only."
      />
      <div className="grid cols-3 dashboard-section">
        <InfoBox
          icon="🏢"
          label="Supplier records"
          value={counts[0]}
          variant="primary"
        />
        <InfoBox
          icon="✅"
          label="Workflow tasks"
          value={counts[1]}
          variant="success"
        />
        <InfoBox
          icon="🛡️"
          label="Audit events"
          value={counts[2]}
          variant="info"
        />
      </div>
      <AdminCard
        title="Operational report catalogue"
        subtitle="These totals are surfaced from platform APIs; report definitions can expand without hardcoding workflow rules in the frontend."
      >
        <DataTable
          rows={rows}
          striped
          compact
          columns={[
            { header: "Area", cell: (row) => row.area, sortable: true },
            { header: "Metric", cell: (row) => row.metric, sortable: true },
            { header: "Value", cell: (row) => row.value, sortable: true },
            { header: "Source", cell: (row) => row.source, sortable: true },
          ]}
        />
      </AdminCard>
    </>
  );
}
