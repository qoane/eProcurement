import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { MetricCard } from "../../components/ui/MetricCard";
import { DataTable } from "../../components/ui/DataTable";
import { getSuppliers } from "../../services/suppliersApi";
import { getTasks } from "../../services/tasksApi";
import { getAuditEvents } from "../../services/auditApi";
import { getWorkflows } from "../../services/workflowsApi";
import type {
  AuditEvent,
  Supplier,
  WorkflowDefinition,
  WorkflowTask,
} from "../../types/api";
export function DashboardPage() {
  const [s, setS] = useState<Supplier[]>([]),
    [t, setT] = useState<WorkflowTask[]>([]),
    [a, setA] = useState<AuditEvent[]>([]),
    [w, setW] = useState<WorkflowDefinition[]>([]);
  useEffect(() => {
    void Promise.all([
      getSuppliers(),
      getTasks(),
      getAuditEvents(),
      getWorkflows(),
    ]).then(([s, t, a, w]) => {
      setS(s.data);
      setT(t.data);
      setA(a.data);
      setW(w.data);
    });
  }, []);
  return (
    <>
      <PageHeader
        title="Enterprise dashboard"
        description="Live operational overview from configured platform APIs."
      />
      <div className="grid cols-4">
        <MetricCard label="Total suppliers" value={s.length} />
        <MetricCard
          label="Pending workflow tasks"
          value={
            t.filter((x) => (x.status || "").toLowerCase().includes("pending"))
              .length
          }
        />
        <MetricCard label="Workflow definitions" value={w.length} />
        <MetricCard label="Recent audit events" value={a.length} />
      </div>
      <section className="panel" style={{ marginTop: 18 }}>
        <h2>Recent audit activity</h2>
        <DataTable
          rows={a.slice(0, 6)}
          columns={[
            { header: "Event", cell: (r) => r.eventType },
            { header: "Entity", cell: (r) => r.entityType },
            { header: "Actor", cell: (r) => r.actor },
            { header: "Date", cell: (r) => r.createdAt || r.occurredAt },
          ]}
        />
      </section>
    </>
  );
}
