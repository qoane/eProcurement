import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { Panel } from "../../components/ui/Panel";
import { Button } from "../../components/ui/Button";
import { StatusBadge } from "../../components/ui/Badge";
import { Timeline } from "../../components/ui/Timeline";
import { getSupplier, submitSupplier } from "../../services/suppliersApi";
import { getAuditEvents } from "../../services/auditApi";
import type { AuditEvent, Supplier } from "../../types/api";
export function SupplierDetailPage({ reference }: { reference: string }) {
  const [s, setS] = useState<Supplier>(),
    [a, setA] = useState<AuditEvent[]>([]);
  useEffect(() => {
    void getSupplier(reference).then((r) => setS(r.data));
    void getAuditEvents().then((r) =>
      setA(
        r.data.filter(
          (x) => x.entityId === s?.id || x.entityType === "Supplier",
        ),
      ),
    );
  }, [reference, s?.id]);
  return (
    <>
      <PageHeader
        title={s?.legalName || reference}
        description="Supplier summary, documents, workflow and audit context."
        actions={
          <>
            <Button onClick={() => submitSupplier(reference)}>
              Submit supplier
            </Button>
            <Button variant="secondary">Approve supplier</Button>
          </>
        }
      />
      <div className="grid cols-2">
        <Panel title="Supplier summary">
          <p>
            Reference: <strong>{reference}</strong>
          </p>
          <StatusBadge status={s?.status} />
          <p className="muted">Categories: {(s?.categories || []).length}</p>
          <p className="muted">Documents: {(s?.documents || []).length}</p>
        </Panel>
        <Panel title="Workflow panel">
          <p className="muted">
            Workflow state is loaded from current workflow/task APIs when
            configured.
          </p>
        </Panel>
      </div>
      <Panel title="Audit timeline">
        <Timeline
          items={a.map((x) => ({
            title: x.eventType || "Audit event",
            meta: x.actor,
            description: x.createdAt || x.occurredAt,
          }))}
        />
      </Panel>
    </>
  );
}
