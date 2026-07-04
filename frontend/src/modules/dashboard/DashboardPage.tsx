import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { MetricCard } from "../../components/ui/MetricCard";
import { DataTable } from "../../components/ui/DataTable";
import { Card } from "../../components/ui/Card";
import { EmptyState } from "../../components/ui/EmptyState";
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

const capabilities = [
  "Configurable Workflows",
  "Business Rules Engine",
  "Dynamic Forms",
  "Supplier Management",
  "Audit Trails",
  "Reporting",
];
const modules = [
  "Planning",
  "Requisitions",
  "Tenders",
  "Evaluation",
  "Awards",
  "Purchase Orders",
  "Contracts",
  "Supplier Management",
];
const lifecycle = [
  "Plan",
  "Source",
  "Submit",
  "Evaluate",
  "Award",
  "Manage",
  "Report",
];

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
        title="Executive dashboard"
        description="Operational visibility for configured procurement workflows, suppliers and audit activity."
      />
      <div className="grid cols-4 dashboard-metrics">
        <MetricCard label="Suppliers" value={s.length} />
        <MetricCard label="Workflow Tasks" value={t.length} />
        <MetricCard label="Workflow Definitions" value={w.length} />
        <MetricCard label="Audit Events" value={a.length} />
      </div>

      <section className="dashboard-section">
        <h2>Platform capabilities</h2>
        <div className="grid cols-3">
          {capabilities.map((x) => (
            <Card key={x}>
              <h3>{x}</h3>
              <p className="muted">
                Configured in the platform studio and surfaced through governed
                procurement operations.
              </p>
            </Card>
          ))}
        </div>
      </section>

      <section className="panel dashboard-section">
        <h2>Procurement lifecycle</h2>
        <div className="lifecycle-flow">
          {lifecycle.map((x) => (
            <span key={x}>{x}</span>
          ))}
        </div>
      </section>

      <div className="grid cols-2 dashboard-section">
        <section className="panel">
          <h2>My work</h2>
          {t.length ? (
            <DataTable
              rows={t.slice(0, 6)}
              columns={[
                { header: "Task", cell: (r) => r.title || r.name || r.id },
                { header: "Status", cell: (r) => r.status },
                { header: "Assigned", cell: (r) => r.assignedTo || "—" },
              ]}
            />
          ) : (
            <EmptyState
              title="No workflow tasks"
              message="There are no workflow tasks assigned or available from the API yet."
            />
          )}
        </section>
        <section className="panel">
          <h2>Recent activity</h2>
          {a.length ? (
            <DataTable
              rows={a.slice(0, 6)}
              columns={[
                { header: "Event", cell: (r) => r.eventType },
                { header: "Entity", cell: (r) => r.entityType },
                { header: "Actor", cell: (r) => r.actor },
                { header: "Date", cell: (r) => r.createdAt || r.occurredAt },
              ]}
            />
          ) : (
            <EmptyState
              title="No audit events"
              message="Audit events will appear here when the audit API returns activity."
            />
          )}
        </section>
      </div>

      <section className="dashboard-section">
        <h2>Module launcher</h2>
        <div className="module-launcher">
          {modules.map((x) => (
            <button key={x}>
              {x}
              <small>Not configured yet</small>
            </button>
          ))}
        </div>
      </section>
    </>
  );
}
