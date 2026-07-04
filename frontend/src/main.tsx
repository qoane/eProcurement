import React, { useEffect, useState } from "react";
import { createRoot } from "react-dom/client";
import { CheckCircle, Clock, FileText, ShieldCheck } from "lucide-react";
import "./styles.css";

type Supplier = { referenceNumber: string; legalName: string; status: string; categories: string[] };
type AuditEvent = { eventType: string; details: string };
type WorkflowTask = { id: string; nodeCode: string; status: string };
type WorkflowDefinition = { code: string; name: string };
type FormDefinition = { code: string; name: string };
const api = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5000";
async function getJson<T>(path: string, fallback: T): Promise<T> {
  try { const response = await fetch(`${api}${path}`); return response.ok ? await response.json() : fallback; } catch { return fallback; }
}
function Badge({ status }: { status: string }) { return <span className={`badge ${status.toLowerCase()}`}>{status}</span>; }
function App() {
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [audit, setAudit] = useState<AuditEvent[]>([]);
  const [tasks, setTasks] = useState<WorkflowTask[]>([]);
  const [workflows, setWorkflows] = useState<WorkflowDefinition[]>([]);
  const [forms, setForms] = useState<FormDefinition[]>([]);
  useEffect(() => { void Promise.all([
    getJson<Supplier[]>("/api/suppliers", []), getJson<AuditEvent[]>("/api/audit-events", []), getJson<WorkflowTask[]>("/api/workflow-tasks", []), getJson<WorkflowDefinition[]>("/api/workflows", []), getJson<FormDefinition[]>("/api/form-definitions/SUPPLIER-PROFILE/active", []).then(x => Array.isArray(x) ? x : [x as FormDefinition])
  ]).then(([s, a, t, w, f]) => { setSuppliers(s); setAudit(a); setTasks(t); setWorkflows(w); setForms(f.filter(Boolean)); }); }, []);
  return <div className="shell"><aside><h2>LCA eProcurement</h2><nav>{["Dashboard","Supplier list","Supplier registration","Supplier verification","Supplier detail","Audit timeline"].map((x) => <a key={x}>{x}</a>)}</nav></aside><main><header><div><p>Configurable procurement automation platform</p><h1>Supplier Onboarding Command Centre</h1></div><button>Register supplier</button></header><section className="cards"><article><Clock /><b>{suppliers.filter(s => s.status === "UnderVerification" || s.status === "Submitted").length}</b><span>Pending verification</span></article><article><CheckCircle /><b>{suppliers.filter(s => s.status === "Approved").length}</b><span>Approved suppliers</span></article><article><ShieldCheck /><b>{tasks.length}</b><span>Workflow tasks</span></article><article><FileText /><b>{forms.length}</b><span>Active forms</span></article></section><section className="grid"><div className="panel wide"><h2>Supplier verification queue</h2><table><thead><tr><th>Reference</th><th>Supplier</th><th>Categories</th><th>Status</th></tr></thead><tbody>{suppliers.map((s) => <tr key={s.referenceNumber}><td>{s.referenceNumber}</td><td>{s.legalName}</td><td>{s.categories.join(", ")}</td><td><Badge status={s.status} /></td></tr>)}</tbody></table></div><div className="panel"><h2>Audit timeline</h2><ol className="timeline">{audit.map((a, i) => <li key={`${a.eventType}-${i}`}><span>{i + 1}</span>{a.eventType}</li>)}</ol></div><div className="panel"><h2>Workflow definitions</h2>{workflows.map(w => <p key={w.code}>{w.name}</p>)}</div><div className="panel"><h2>Workflow tasks</h2>{tasks.map(t => <p key={t.id}>{t.nodeCode}: {t.status}</p>)}</div></section></main></div>;
}
createRoot(document.getElementById("root")!).render(<App />);
