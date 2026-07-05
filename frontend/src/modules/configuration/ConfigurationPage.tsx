import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { Card } from "../../components/ui/Card";
import { Button } from "../../components/ui/Button";
import { Badge } from "../../components/ui/Badge";
import { getConfigurationStudio, publishBusinessProcess } from "../../services/configurationApi";
import type { ConfigurationStudio } from "../../types/api";

export function ConfigurationPage() {
  const [studio, setStudio] = useState<ConfigurationStudio>({ businessProcesses: [], documentRequirementSets: [], approvalMatrices: [], workflowMappings: [] });
  const [message, setMessage] = useState("");
  async function load() { setStudio((await getConfigurationStudio()).data); }
  useEffect(() => { void load(); }, []);
  async function publish(code: string) { await publishBusinessProcess(code); setMessage(`${code} published from configuration.`); await load(); }
  return (
    <>
      <PageHeader title="Administration Studio" description="Configuration-first orchestration for business processes, document requirements, approval matrices, and workflow mappings." />
      {message && <Card><strong>{message}</strong></Card>}
      <div className="grid cols-2">
        <Card><h2>Business Processes</h2><p className="muted">Create Supplier Registration, select workflow, form, document requirements, approval matrix, then publish.</p>{studio.businessProcesses.map((p) => <div className="list-row" key={p.code}><div><strong>{p.name}</strong><p className="muted">{p.code} · {p.entityType}</p></div><Badge>{p.status}</Badge><Button onClick={() => publish(p.code)}>Publish</Button></div>)}</Card>
        <Card><h2>Document Requirements</h2>{studio.documentRequirementSets.map((s) => <div className="list-row" key={s.name}><div><strong>{s.name}</strong><p className="muted">{s.requirements?.map((r) => r.documentType).join(", ")}</p></div></div>)}</Card>
        <Card><h2>Approval Matrices</h2>{studio.approvalMatrices.map((m) => <div className="list-row" key={m.name}><div><strong>{m.name}</strong><p className="muted">{m.steps?.sort((a,b) => a.sequence - b.sequence).map((s) => `${s.sequence}. ${s.role}`).join(" → ")}</p></div></div>)}</Card>
        <Card><h2>Workflow Mapping</h2><p className="muted">Mappings remain editable for action-to-workflow integration scenarios.</p><pre>{JSON.stringify(studio.workflowMappings, null, 2)}</pre></Card>
      </div>
    </>
  );
}
