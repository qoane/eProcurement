import { useEffect, useMemo, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { Panel } from "../../components/ui/Panel";
import { Button } from "../../components/ui/Button";
import { StatusBadge } from "../../components/ui/Badge";
import { getWorkflows, saveWorkflowDesigner, publishWorkflow, archiveWorkflowVersion } from "../../services/workflowsApi";
import type { WorkflowDefinition, WorkflowNode, WorkflowTransition } from "../../types/api";

type Selection = { type: "node"; code: string } | { type: "transition"; code: string };
const roles = ["Supplier", "ProcurementOfficer", "Evaluator", "Approver", "FinanceUser", "Auditor", "Administrator"];
const starterNodes: WorkflowNode[] = [
  { code: "Submitted", name: "Submitted", kind: "Start", isStart: true, positionX: 80, positionY: 170 },
  { code: "DocumentCheck", name: "Document Check", kind: "Task", createsTask: true, defaultAssignedRole: "ProcurementOfficer", positionX: 310, positionY: 120, assignedRolesJson: '["ProcurementOfficer"]' },
  { code: "Verification", name: "Verification", kind: "Task", createsTask: true, defaultAssignedRole: "Evaluator", positionX: 560, positionY: 120, assignedRolesJson: '["Evaluator"]' },
  { code: "Approval", name: "Approval", kind: "Task", createsTask: true, defaultAssignedRole: "Approver", positionX: 810, positionY: 120, assignedRolesJson: '["Approver"]' },
  { code: "Approved", name: "Approved", kind: "End", isTerminal: true, positionX: 1070, positionY: 80 },
  { code: "Rejected", name: "Rejected", kind: "End", isTerminal: true, positionX: 1070, positionY: 250 },
];
const starterTransitions: WorkflowTransition[] = [
  { fromNodeCode: "Submitted", actionCode: "Submit", actionName: "Submit for verification", toNodeCode: "DocumentCheck" },
  { fromNodeCode: "DocumentCheck", actionCode: "DocumentsAccepted", actionName: "Documents accepted", toNodeCode: "Verification", requiredRuleCode: "SUP-HAS-REG", businessRuleCodesJson: '["SUP-HAS-REG"]' },
  { fromNodeCode: "Verification", actionCode: "TaxVerified", actionName: "Tax verified", toNodeCode: "Approval", requiredRuleCode: "SUP-HAS-TAX", businessRuleCodesJson: '["SUP-HAS-TAX"]' },
  { fromNodeCode: "Approval", actionCode: "Approve", actionName: "Approve", toNodeCode: "Approved", requiredRuleCode: "SUP-HAS-CATEGORY", businessRuleCodesJson: '["SUP-HAS-CATEGORY"]' },
  { fromNodeCode: "Approval", actionCode: "Reject", actionName: "Reject", toNodeCode: "Rejected", conditionExpression: "review.outcome == 'Rejected'" },
];

export function WorkflowDesignerPage() {
  const [definitions, setDefinitions] = useState<WorkflowDefinition[]>([]);
  const [code, setCode] = useState("SUPPLIER-ONBOARDING");
  const selectedWorkflow = definitions.find((x) => x.code === code);
  const statusLabel = (status: unknown) => (typeof status === "number" ? ["Draft", "Published", "Archived"][status] : status);
  const selectedVersion = useMemo(() => selectedWorkflow?.versions?.find((x) => statusLabel(x.status) === "Draft") ?? selectedWorkflow?.versions?.find((x) => x.id === selectedWorkflow?.publishedVersionId) ?? selectedWorkflow?.versions?.[0], [selectedWorkflow]);
  const [nodes, setNodes] = useState<WorkflowNode[]>(starterNodes);
  const [transitions, setTransitions] = useState<WorkflowTransition[]>(starterTransitions);
  const [selection, setSelection] = useState<Selection>({ type: "node", code: "DocumentCheck" });
  const [message, setMessage] = useState("");

  useEffect(() => { void getWorkflows().then((r) => setDefinitions(r.data)); }, []);
  useEffect(() => {
    if (!selectedVersion) return;
    setNodes((selectedVersion.nodes?.length ? selectedVersion.nodes : starterNodes).map((n, i) => ({ ...n, positionX: n.positionX || 80 + i * 210, positionY: n.positionY || (n.isTerminal ? 250 : 120) })));
    setTransitions(selectedVersion.transitions?.length ? selectedVersion.transitions : starterTransitions);
  }, [selectedVersion?.versionNumber, code]);

  const updateNode = (patch: Partial<WorkflowNode>) => setNodes((items) => items.map((n) => n.code === selection.code ? { ...n, ...patch } : n));
  const updateTransition = (patch: Partial<WorkflowTransition>) => setTransitions((items) => items.map((t) => t.actionCode === selection.code ? { ...t, ...patch } : t));
  const selectedNode = selection.type === "node" ? nodes.find((n) => n.code === selection.code) : undefined;
  const selectedTransition = selection.type === "transition" ? transitions.find((t) => t.actionCode === selection.code) : undefined;
  const findNode = (nodeCode: string) => nodes.find((n) => n.code === nodeCode) ?? nodes[0];

  async function save() {
    await saveWorkflowDesigner({ code, name: selectedWorkflow?.name ?? "Supplier onboarding", entityType: "Supplier", versionNumber: selectedVersion?.versionNumber, nodes, transitions });
    setMessage("Designer graph saved to SQL Server workflow tables.");
    const refreshed = await getWorkflows(); setDefinitions(refreshed.data);
  }
  async function publish() { await save(); await publishWorkflow(code); setMessage("Workflow version published and activated for Supplier Registration."); const refreshed = await getWorkflows(); setDefinitions(refreshed.data); }
  async function archive() { if (!selectedVersion?.versionNumber) return; await archiveWorkflowVersion(code, selectedVersion.versionNumber); setMessage(`Version ${selectedVersion.versionNumber} archived.`); const refreshed = await getWorkflows(); setDefinitions(refreshed.data); }
  function addNode(kind: string) { const next = `${kind}${nodes.length + 1}`; setNodes([...nodes, { code: next, name: `${kind} node`, kind, createsTask: kind === "Task", positionX: 180 + nodes.length * 120, positionY: 330 }]); setSelection({ type: "node", code: next }); }
  function connect() { const from = selectedNode?.code ?? nodes[0].code; const to = nodes.find((n) => n.code !== from)?.code ?? from; const actionCode = `Action${transitions.length + 1}`; setTransitions([...transitions, { fromNodeCode: from, actionCode, actionName: "New transition", toNodeCode: to }]); setSelection({ type: "transition", code: actionCode }); }

  return <>
    <PageHeader title="Professional Workflow Designer" description="Drag nodes, connect transitions, configure actions, conditions, business rules and roles, then publish versioned workflow graphs for Supplier Registration." />
    <div className="toolbar"><select className="select" style={{ maxWidth: 300 }} value={code} onChange={(e) => setCode(e.target.value)}>{definitions.map((d) => <option key={d.code}>{d.code}</option>)}<option>SUPPLIER-ONBOARDING</option></select><Button onClick={() => addNode("Task")}>Add task</Button><Button variant="secondary" onClick={() => addNode("Automatic")}>Add automation</Button><Button variant="secondary" onClick={connect}>Connect transition</Button><Button onClick={save}>Save graph</Button><Button variant="secondary" onClick={publish}>Publish version</Button><Button variant="secondary" onClick={archive}>Archive version</Button></div>
    {message && <p className="badge success">{message}</p>}
    <div className="workflow-designer-shell">
      <Panel title="Nodes"><div className="workflow-node-list">{nodes.map((n) => <button key={n.code} className={`workflow-node-row ${selection.type === "node" && selection.code === n.code ? "active" : ""}`} onClick={() => setSelection({ type: "node", code: n.code })}><strong>{n.name}</strong><span>{n.kind} · {n.defaultAssignedRole || "No role"}</span></button>)}</div></Panel>
      <section className="workflow-canvas" onDragOver={(e) => e.preventDefault()} onDrop={(e) => { const nodeCode = e.dataTransfer.getData("node"); const rect = e.currentTarget.getBoundingClientRect(); setNodes((items) => items.map((n) => n.code === nodeCode ? { ...n, positionX: Math.round(e.clientX - rect.left - 90), positionY: Math.round(e.clientY - rect.top - 34) } : n)); }}>
        <svg className="workflow-links">{transitions.map((t) => { const a = findNode(t.fromNodeCode); const b = findNode(t.toNodeCode); return <g key={t.actionCode} onClick={() => setSelection({ type: "transition", code: t.actionCode })}><line x1={(a.positionX ?? 0) + 180} y1={(a.positionY ?? 0) + 34} x2={b.positionX ?? 0} y2={(b.positionY ?? 0) + 34} /><text x={((a.positionX ?? 0) + (b.positionX ?? 0)) / 2 + 80} y={((a.positionY ?? 0) + (b.positionY ?? 0)) / 2 + 20}>{t.actionName}</text></g>; })}</svg>
        {nodes.map((n) => <div key={n.code} draggable onDragStart={(e) => e.dataTransfer.setData("node", n.code)} onClick={() => setSelection({ type: "node", code: n.code })} className={`workflow-node-card ${n.kind?.toLowerCase()} ${selection.type === "node" && selection.code === n.code ? "selected" : ""}`} style={{ left: n.positionX, top: n.positionY }}><strong>{n.name}</strong><span>{n.code}</span><small>{n.createsTask ? `Task · ${n.defaultAssignedRole}` : n.kind}</small></div>)}
      </section>
      <Panel title="Properties">{selectedNode && <div className="form-grid one"><label>Name<input className="input" value={selectedNode.name} onChange={(e) => updateNode({ name: e.target.value })} /></label><label>Role<select className="select" value={selectedNode.defaultAssignedRole ?? ""} onChange={(e) => updateNode({ defaultAssignedRole: e.target.value, assignedRolesJson: JSON.stringify([e.target.value].filter(Boolean)) })}><option value="">No assignment</option>{roles.map((r) => <option key={r}>{r}</option>)}</select></label><label>Actions JSON<textarea className="input" value={selectedNode.actionConfigurationJson ?? "{}"} onChange={(e) => updateNode({ actionConfigurationJson: e.target.value })} /></label><label>Conditions JSON<textarea className="input" value={selectedNode.conditionConfigurationJson ?? "{}"} onChange={(e) => updateNode({ conditionConfigurationJson: e.target.value })} /></label><label>Business rules JSON<input className="input" value={selectedNode.businessRuleCodesJson ?? "[]"} onChange={(e) => updateNode({ businessRuleCodesJson: e.target.value })} /></label></div>}{selectedTransition && <div className="form-grid one"><label>Action name<input className="input" value={selectedTransition.actionName} onChange={(e) => updateTransition({ actionName: e.target.value })} /></label><label>From<select className="select" value={selectedTransition.fromNodeCode} onChange={(e) => updateTransition({ fromNodeCode: e.target.value })}>{nodes.map((n) => <option key={n.code}>{n.code}</option>)}</select></label><label>To<select className="select" value={selectedTransition.toNodeCode} onChange={(e) => updateTransition({ toNodeCode: e.target.value })}>{nodes.map((n) => <option key={n.code}>{n.code}</option>)}</select></label><label>Condition<input className="input" value={selectedTransition.conditionExpression ?? ""} onChange={(e) => updateTransition({ conditionExpression: e.target.value })} /></label><label>Required rule<input className="input" value={selectedTransition.requiredRuleCode ?? ""} onChange={(e) => updateTransition({ requiredRuleCode: e.target.value })} /></label><label>Action JSON<textarea className="input" value={selectedTransition.actionConfigurationJson ?? "{}"} onChange={(e) => updateTransition({ actionConfigurationJson: e.target.value })} /></label></div>}</Panel>
    </div>
    <Panel title="Version transitions"><table><tbody>{transitions.map((t) => <tr key={t.actionCode} onClick={() => setSelection({ type: "transition", code: t.actionCode })}><td><strong>{t.actionName}</strong><br />{t.fromNodeCode} → {t.toNodeCode}</td><td>{t.requiredRuleCode || t.conditionExpression || "Always"}</td><td>{selectedVersion && <StatusBadge status={selectedVersion.status} />}</td></tr>)}</tbody></table></Panel>
  </>;
}
