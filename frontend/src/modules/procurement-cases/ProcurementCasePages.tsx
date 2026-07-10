import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { Badge } from "../../components/ui/Badge";
import { Button } from "../../components/ui/Button";
import { navigate } from "../../app/routes";
import { getProcurementCase, getProcurementCases, ProcurementCaseDetail, ProcurementCaseSummary } from "../../services/procurementCasesApi";

const money = (v: number) => new Intl.NumberFormat("en-LS", { style: "currency", currency: "LSL" }).format(v);
const date = (v?: string | null) => v ? new Date(v).toLocaleDateString() : "—";

export function ProcurementCaseListPage() {
  const [rows, setRows] = useState<ProcurementCaseSummary[]>([]);
  useEffect(() => { getProcurementCases().then(r => setRows(r.data)); }, []);
  return <><PageHeader title="Procurement Cases" description="End-to-end lifecycle trace from planning to contract." />
    <div className="card"><div className="card-body table-responsive"><table className="table table-hover align-middle"><thead><tr><th>Case number</th><th>Title</th><th>Stage</th><th>Tender</th><th>Supplier if awarded</th><th>Total value</th><th>Status</th></tr></thead><tbody>{rows.map(r => <tr key={r.id} role="button" onClick={() => navigate(`/app/procurement-cases/${r.id}`)}><td className="fw-semibold">{r.caseNumber}</td><td>{r.title}</td><td><Badge>{r.currentLifecycleStage}</Badge></td><td>{r.tender || "—"}</td><td>{r.supplierIfAwarded || "—"}</td><td>{money(r.totalValue)}</td><td>{r.status}</td></tr>)}</tbody></table></div></div></>;
}

export function ProcurementCaseDetailPage({ id }: { id: string }) {
  const [detail, setDetail] = useState<ProcurementCaseDetail | null>(null);
  useEffect(() => { getProcurementCase(id).then(r => setDetail(r.data)); }, [id]);
  if (!detail) return <><PageHeader title="Procurement Case" description="Loading case trace..." /></>;
  const links = (type: string) => detail.links.filter(l => l.relationshipType === type);
  return <><PageHeader title={detail.case.title} description={`${detail.case.caseNumber} • ${detail.case.department}`} />
    <div className="card mb-3"><div className="card-body"><div className="d-flex flex-wrap gap-2 align-items-center">{detail.timeline.map((s, i) => <div key={s.stage} className="text-center flex-fill"><div className={`rounded-pill px-2 py-2 ${s.status === "Completed" ? "bg-success text-white" : "bg-light"}`}>{s.stage}</div>{i < detail.timeline.length - 1 && <span className="text-muted">→</span>}</div>)}</div></div></div>
    <div className="row"><div className="col-lg-4"><Section title="Case summary"><p>{detail.case.description}</p><p><b>Status:</b> {detail.case.status}</p><p><b>Created:</b> {date(detail.case.createdAt)} by {detail.case.createdBy}</p>{detail.summary.publicNoticeLink && <Button onClick={() => navigate(detail.summary.publicNoticeLink!)}>Public notice link</Button>}</Section></div><div className="col-lg-8"><Section title="Lifecycle records"><table className="table table-sm"><tbody>{detail.timeline.map(s => <tr key={s.stage}><th>{s.stage}</th><td>{s.recordNumber || "—"}</td><td>{s.status}</td><td>{s.responsibleRole}</td><td>{date(s.date)}</td><td>{s.url && <Button onClick={() => navigate(s.url!)}>Open page</Button>}</td></tr>)}</tbody></table></Section></div></div>
    <div className="row"><div className="col-md-6"><Section title="Linked records"><ul>{detail.links.map(l => <li key={l.id}>{l.relationshipType}: {l.entityReference}</li>)}</ul></Section><Section title="Supplier participation"><ul>{links("BidSubmission").map(l => <li key={l.id}>{l.entityReference}</li>)}</ul></Section><Section title="Documents"><ul>{detail.documents.map((d,i) => <li key={i}>{d.source}: {d.fileName} ({d.documentType})</li>)}</ul></Section><Section title="Notifications"><ul>{detail.notifications.map(n => <li key={n.id}>{n.subject} — {n.status}</li>)}</ul></Section></div><div className="col-md-6"><Section title="Evaluation summary">{links("Evaluation").map(l => <p key={l.id}>{l.entityReference}</p>)}</Section><Section title="Award summary">{links("Award").map(l => <p key={l.id}>{l.entityReference}</p>)}</Section><Section title="PO summary">{links("PurchaseOrder").map(l => <p key={l.id}>{l.entityReference}</p>)}</Section><Section title="Contract summary">{links("Contract").map(l => <p key={l.id}>{l.entityReference}</p>)}</Section><Section title="Audit timeline"><ul>{detail.auditTimeline.map(a => <li key={a.id}>{date(a.occurredAt)} — {a.eventType}: {a.details}</li>)}</ul></Section></div></div>
  </>;
}
function Section({ title, children }: { title: string; children: React.ReactNode }) { return <div className="card mb-3"><div className="card-header"><h3 className="card-title">{title}</h3></div><div className="card-body">{children}</div></div>; }
