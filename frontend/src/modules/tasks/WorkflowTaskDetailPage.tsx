import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { Panel } from "../../components/ui/Panel";
import { Button } from "../../components/ui/Button";
import { StatusBadge } from "../../components/ui/Badge";
import { Timeline } from "../../components/ui/Timeline";
import { executeTaskAction, getTaskDetail } from "../../services/tasksApi";
export function WorkflowTaskDetailPage({ id }: { id?: string }) {
  const [detail, setDetail] = useState<any>(null);
  const load = () => { if (id) void getTaskDetail(id).then((r) => setDetail(r.data)); };
  useEffect(load, [id]);
  const supplier = detail?.supplier?.supplier;
  return (
    <>
      <PageHeader title={`Workflow task ${detail?.task?.nodeCode ?? id ?? ""}`} description="Verification workspace assembled from supplier, document, form, history, and workflow configuration." />
      <div className="grid cols-2">
        <Panel title="Supplier information"><p><strong>{supplier?.legalName}</strong></p><p>{supplier?.referenceNumber}</p><StatusBadge status={supplier?.status} /></Panel>
        <Panel title="Submitted documents">{(detail?.supplier?.documents ?? []).map((d: any) => <p key={d.id}><strong>{d.documentType}</strong> — {d.fileName}</p>)}</Panel>
      </div>
      <Panel title="Form submission data">{(detail?.supplier?.formSubmissions?.[0]?.values ?? []).map((v: any) => <p key={v.fieldCode}><strong>{v.fieldCode}</strong>: {v.value}</p>)}</Panel>
      <Panel title="Available workflow actions">{(detail?.availableActions ?? []).map((a: any) => <Button key={a.actionCode} onClick={async () => { await executeTaskAction(id!, a.actionCode); load(); }}>{a.actionName}</Button>)}</Panel>
      <Panel title="Workflow history"><Timeline items={(detail?.history ?? []).map((h: any) => ({ title: h.eventType, meta: h.actor, description: `${h.nodeCode} · ${h.details}` }))} /></Panel>
    </>
  );
}
