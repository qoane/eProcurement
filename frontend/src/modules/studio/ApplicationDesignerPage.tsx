import { useEffect, useMemo, useState } from "react";
import { Badge } from "../../components/ui/Badge";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { DataTable } from "../../components/ui/DataTable";
import { Input } from "../../components/ui/Input";
import { PageHeader } from "../../components/ui/PageHeader";
import { Select } from "../../components/ui/Select";
import { archiveApplication, createApplication, deleteApplication, getApplications, publishApplication, updateApplication } from "../../services/applicationsApi";
import type { ApplicationDesigner } from "../../types/api";

const moduleOptions = ["Supplier Management", "Requisitions", "Tenders", "Evaluation", "Contracts", "Reports", "Studio"];
const blank: ApplicationDesigner = { code: "PROCUREMENT", name: "Procurement", icon: "Briefcase", theme: "LCA Indigo", description: "Procurement workspace containing governed source-to-contract modules.", defaultLandingPage: "/app/suppliers", navigationRoot: "/app", modules: moduleOptions, status: "Draft" };

export function ApplicationDesignerPage() {
  const [apps, setApps] = useState<ApplicationDesigner[]>([]);
  const [form, setForm] = useState<ApplicationDesigner>(blank);
  const [message, setMessage] = useState("");
  const selected = useMemo(() => apps.find((x) => x.id === form.id), [apps, form.id]);
  async function load() { setApps((await getApplications()).data); }
  useEffect(() => { void load(); }, []);
  function toggleModule(module: string) { setForm((current) => ({ ...current, modules: current.modules.includes(module) ? current.modules.filter((x) => x !== module) : [...current.modules, module] })); }
  async function save() { form.id ? await updateApplication(form.id, form) : await createApplication(form); setMessage(`${form.name} saved.`); setForm(blank); await load(); }
  async function publish(id: string) { await publishApplication(id); setMessage("Application published."); await load(); }
  async function archive(id: string) { await archiveApplication(id); setMessage("Application archived."); await load(); }
  async function remove(id: string) { await deleteApplication(id); setMessage("Application deleted."); if (form.id === id) setForm(blank); await load(); }
  return (
    <div className="studio-page">
      <PageHeader title="Application Designer" description="Create application containers, assign modules, set navigation roots, and manage draft, published, archived, and deleted application lifecycles." actions={<Badge tone="info">Enterprise grid</Badge>} />
      {message && <Card><strong>{message}</strong></Card>}
      <div className="grid cols-2">
        <Card>
          <h2>{selected ? "Edit application" : "Create application"}</h2>
          <div className="form-grid">
            <label>Name<Input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} /></label>
            <label>Code<Input value={form.code} onChange={(e) => setForm({ ...form, code: e.target.value.toUpperCase().replace(/ /g, "-") })} /></label>
            <label>Icon<Input value={form.icon} onChange={(e) => setForm({ ...form, icon: e.target.value })} /></label>
            <label>Theme<Input value={form.theme} onChange={(e) => setForm({ ...form, theme: e.target.value })} /></label>
            <label>Default landing page<Input value={form.defaultLandingPage} onChange={(e) => setForm({ ...form, defaultLandingPage: e.target.value })} /></label>
            <label>Navigation root<Input value={form.navigationRoot} onChange={(e) => setForm({ ...form, navigationRoot: e.target.value })} /></label>
            <label>Status<Select value={form.status} onChange={(e) => setForm({ ...form, status: e.target.value })}><option>Draft</option><option>Active</option><option>Archived</option><option>Inactive</option></Select></label>
          </div>
          <label>Description<textarea className="input" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} /></label>
          <div className="studio-section-heading"><h3>Contained modules</h3></div>
          <div className="studio-card-grid">
            {moduleOptions.map((module) => <button type="button" className="studio-card" key={module} onClick={() => toggleModule(module)}><strong>{module}</strong><small>{form.modules.includes(module) ? "Included" : "Click to include"}</small></button>)}
          </div>
          <div className="actions"><Button onClick={save}>{form.id ? "Update" : "Create"} Application</Button><Button variant="secondary" onClick={() => setForm(blank)}>Reset</Button></div>
        </Card>
        <Card>
          <h2>Application container preview</h2>
          <p className="muted">{form.name} contains {form.modules.join(", ") || "no modules selected"}.</p>
          <pre>{JSON.stringify(form, null, 2)}</pre>
        </Card>
      </div>
      <Card>
        <h2>Applications</h2>
        <DataTable rows={apps} empty="No applications have been created." columns={[{ header: "Application", cell: (row) => <><strong>{row.name}</strong><p className="muted">{row.code} · {row.icon}</p></> }, { header: "Status", cell: (row) => <Badge>{row.status || "Draft"}</Badge> }, { header: "Theme", cell: (row) => row.theme }, { header: "Modules", cell: (row) => row.modules.join(", ") }, { header: "Navigation", cell: (row) => `${row.navigationRoot} → ${row.defaultLandingPage}` }, { header: "Actions", cell: (row) => <div className="actions"><Button variant="secondary" onClick={() => setForm(row)}>Edit</Button>{row.id && <Button onClick={() => publish(row.id!)}>Publish</Button>}{row.id && <Button variant="secondary" onClick={() => archive(row.id!)}>Archive</Button>}{row.id && <Button variant="secondary" onClick={() => remove(row.id!)}>Delete</Button>}</div> }]} />
      </Card>
    </div>
  );
}
