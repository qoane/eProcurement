import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { Input } from "../../components/ui/Input";
import { DataTable } from "../../components/ui/DataTable";
import { Timeline } from "../../components/ui/Timeline";
import { getAuditEvents } from "../../services/auditApi";
import type { AuditEvent } from "../../types/api";
export function AuditExplorerPage() {
  const [a, setA] = useState<AuditEvent[]>([]);
  useEffect(() => {
    void getAuditEvents().then((r) => setA(r.data));
  }, []);
  return (
    <>
      <PageHeader
        title="Audit explorer"
        description="Trace entity changes, actors and event types."
      />
      <div className="grid cols-3">
        <Input placeholder="Entity type" />
        <Input placeholder="Actor" />
        <Input placeholder="Event type" />
      </div>
      <div className="grid cols-2" style={{ marginTop: 16 }}>
        <section className="panel">
          <Timeline
            items={a.slice(0, 8).map((x) => ({
              title: x.eventType || "Event",
              meta: x.actor,
              description: x.entityType,
            }))}
          />
        </section>
        <section className="panel">
          <DataTable
            rows={a}
            searchable
            pageSize={8}
            striped
            compact
            toolbarActions={
              <button
                className="btn secondary"
                type="button"
                onClick={() => window.print()}
              >
                Print
              </button>
            }
            columns={[
              { header: "Event", cell: (r) => r.eventType, sortable: true },
              { header: "Entity", cell: (r) => r.entityType, sortable: true },
              { header: "Actor", cell: (r) => r.actor, sortable: true },
              {
                header: "Date",
                cell: (r) => r.createdAt || r.occurredAt,
                sortable: true,
              },
            ]}
          />
        </section>
      </div>
    </>
  );
}
