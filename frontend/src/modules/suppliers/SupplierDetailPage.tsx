import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { Panel } from "../../components/ui/Panel";
import { Button } from "../../components/ui/Button";
import { StatusBadge } from "../../components/ui/Badge";
import { Timeline } from "../../components/ui/Timeline";
import { getSupplier, submitSupplier } from "../../services/suppliersApi";
export function SupplierDetailPage({ reference }: { reference: string }) {
  const [d, setD] = useState<any>();
  const load = () => { void getSupplier(reference).then((r) => setD(r.data)); };
  useEffect(load, [reference]);
  const s = d?.supplier;
  return (
    <>
      <PageHeader title={s?.legalName || reference} description="Supplier profile, current workflow stage, documents, form data, audit timeline, and configured actions." actions={<Button onClick={async () => { await submitSupplier(reference); load(); }}>Submit supplier</Button>} />
      <div className="grid cols-2">
        <Panel title="Supplier profile"><p>Reference: <strong>{reference}</strong></p><StatusBadge status={s?.status} /><p className="muted">Categories: {(s?.categories || []).length}</p></Panel>
        <Panel title="Workflow"><p>Active stage: <strong>{d?.activeWorkflowStage || "None"}</strong></p>{(d?.availableActions ?? []).map((a: any) => <Button key={a.actionCode} variant="secondary">{a.actionName}</Button>)}</Panel>
      </div>
      <Panel title="Documents">{(d?.documents ?? []).map((x: any) => <p key={x.id}><strong>{x.documentType}</strong>: {x.fileName}</p>)}</Panel>
      <Panel title="Form submission data">{(d?.formSubmissions?.[0]?.values ?? []).map((v: any) => <p key={v.fieldCode}><strong>{v.fieldCode}</strong>: {v.value}</p>)}</Panel>
      <Panel title="Audit timeline"><Timeline items={(d?.auditTimeline ?? []).map((x: any) => ({ title: x.eventType || "Audit event", meta: x.actor, description: x.occurredAt }))} /></Panel>
    </>
  );
}
