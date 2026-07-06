import { useEffect, useMemo, useState } from "react";
import { Badge } from "../../components/ui/Badge";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { EmptyState } from "../../components/ui/EmptyState";
import { Input } from "../../components/ui/Input";
import { PageHeader } from "../../components/ui/PageHeader";
import { Select } from "../../components/ui/Select";
import { PageRenderer } from "../ui-composition/PageRenderer";
import { archivePageDefinition, createPageDefinition, deletePageDefinition, getPageDefinitions, publishPageDefinition, updatePageDefinition } from "../../services/pageDefinitionsApi";
import type { PageComponent, PageDesigner, PageType } from "../../types/api";

const pageTypes: PageType[] = ["Dashboard", "DataGrid", "DetailPage", "Form", "Wizard", "Report", "Timeline", "Kanban", "Calendar", "MasterDetail", "SplitView"];
const layouts = [
  { template: "Single Column", regions: ["main"] },
  { template: "Two Column", regions: ["left", "right"] },
  { template: "Three Column", regions: ["left", "main", "right"] },
  { template: "Sidebar Layout", regions: ["sidebar", "main"] },
  { template: "Master Detail", regions: ["master", "detail"] },
  { template: "Dashboard", regions: ["hero", "metrics", "main"] },
  { template: "Wizard", regions: ["steps", "main", "actions"] },
];
const toolbox = ["Page Header", "Card", "Data Grid", "Form Region", "Tabs", "Timeline", "Metric Card", "Button Bar", "Empty State", "Workflow Status", "Audit Panel", "Comments", "Attachments", "Text", "Divider"];
const rendererKey: Record<string, string> = { "Data Grid": "DataTable", "Metric Card": "Metric", "Form Region": "FormField" };
const componentDefinitions = toolbox.map((name) => ({ code: name.toUpperCase().replace(/ /g, "_"), name, description: `${name} designer component`, category: "Page Designer", rendererKey: rendererKey[name] || name.replace(/ /g, ""), properties: [], events: [], validations: [], designMetadata: { icon: "Component" } }));

const blank: PageDesigner = {
  code: "SUPPLIER-LIST", name: "Supplier List", description: "Operational supplier page composed from SQL Server metadata.", applicationId: undefined, pageType: "DataGrid", route: "/app/suppliers", icon: "Users",
  datasource: { entity: "Supplier", mode: "Metadata", endpoint: "/api/suppliers", keyField: "id" }, layout: { template: "Single Column", columns: 12, density: "Comfortable", regions: ["main"] },
  toolbar: [{ code: "save", label: "Save", kind: "Button", actionCode: "save" }], actions: [{ code: "open", label: "Open supplier", kind: "Row", target: "/app/suppliers/{referenceNumber}" }], filters: [{ code: "status", label: "Status", field: "status", operator: "Equals" }],
  columns: [{ code: "referenceNumber", label: "Reference", field: "referenceNumber", displayOrder: 1, sortable: true, searchable: true }, { code: "legalName", label: "Legal name", field: "legalName", displayOrder: 2, sortable: true, searchable: true }, { code: "status", label: "Status", field: "status", displayOrder: 3, sortable: true }],
  components: [{ code: "page-header", name: "Supplier workspace", componentType: "PageHeader", region: "main", displayOrder: 1 }, { code: "supplier-grid", name: "Supplier grid", componentType: "DataTable", region: "main", displayOrder: 2 }], permissions: [{ role: "ProcurementOfficer", access: "View" }], navigation: { route: "/app/suppliers", menuGroup: "Procurement Operations", showInNavigation: true }, status: "Draft", version: 1,
};

function slug(value: string) { return value.toUpperCase().replace(/[^A-Z0-9]+/g, "-").replace(/(^-|-$)/g, ""); }
function configOf(component: PageComponent) { return (component.configuration || {}) as Record<string, string>; }

export function PageDesignerPage() {
  const [pages, setPages] = useState<PageDesigner[]>([]); const [form, setForm] = useState<PageDesigner>(blank); const [selectedCode, setSelectedCode] = useState("page-header"); const [message, setMessage] = useState(""); const [preview, setPreview] = useState(false);
  const selectedComponent = form.components.find((x) => x.code === selectedCode) || form.components[0];
  async function load() { setPages((await getPageDefinitions()).data); }
  useEffect(() => { void load(); }, []);
  const regions = form.layout.regions?.length ? form.layout.regions : ["main"];
  async function save() { const saved = form.id ? await updatePageDefinition(form.id, form) : await createPageDefinition(form); setForm(saved.data); setMessage(`${saved.data.name} saved as SQL Server PageDefinition metadata.`); await load(); }
  async function publish() { if (!form.id) await save(); const id = form.id || (await getPageDefinitions()).data.find((x) => x.code === form.code)?.id; if (id) { const res = await publishPageDefinition(id); setForm(res.data); setMessage(`${res.data.name} published for runtime rendering.`); await load(); } }
  async function archive() { if (form.id) { const res = await archivePageDefinition(form.id); setForm(res.data); setMessage(`${res.data.name} archived.`); await load(); } }
  function addComponent(type: string, region = regions[0]) { const code = slug(`${type}-${form.components.length + 1}`).toLowerCase(); const component = { code, name: type, componentType: rendererKey[type] || type.replace(/ /g, ""), region, displayOrder: form.components.length + 1, configuration: { title: type, datasource: form.datasource.entity, visibilityRule: "", width: "12", height: "auto", cssClass: "", permissions: "ProcurementOfficer", expressions: "" } }; setForm({ ...form, components: [...form.components, component] }); setSelectedCode(code); }
  function patchComponent(patch: Partial<PageComponent>, configuration?: Record<string, string>) { setForm({ ...form, components: form.components.map((c) => c.code === selectedComponent?.code ? { ...c, ...patch, configuration: configuration ? { ...configOf(c), ...configuration } : c.configuration } : c) }); }
  const statusTone = form.status === "Active" ? "success" : form.status === "Archived" ? "warning" : "info";
  return <div className="studio-page page-designer">
    <PageHeader title="Page Designer" description="Create metadata-driven application pages without writing React code. Properties, layouts, components, permissions, publishing, and preview are persisted through PageDefinition APIs." actions={<><Badge tone={statusTone}>{form.status || "Draft"}</Badge><Button onClick={save}>Save</Button><Button onClick={publish}>Publish</Button><Button variant="secondary" onClick={() => setPreview(!preview)}>Preview</Button><Button variant="secondary" onClick={archive}>History</Button></>} />
    {message && <Card><strong>{message}</strong></Card>}
    <div className="grid cols-3">
      <Card><h2>Toolbox</h2><Select value={form.id || "new"} onChange={(e) => { const page = pages.find((p) => p.id === e.target.value); setForm(page || blank); setSelectedCode((page || blank).components[0]?.code || ""); }}><option value="new">Create new page</option>{pages.map((p) => <option value={p.id} key={p.id}>{p.name}</option>)}</Select><div className="stack" style={{ marginTop: 16 }}>{toolbox.map((item) => <Button key={item} variant="secondary" onClick={() => addComponent(item)}>{item}</Button>)}</div></Card>
      <Card><h2>Canvas</h2><div className="form-grid"><label>Name<Input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} /></label><label>Code<Input value={form.code} onChange={(e) => setForm({ ...form, code: slug(e.target.value) })} /></label><label>Application<Input placeholder="ApplicationId" value={form.applicationId || ""} onChange={(e) => setForm({ ...form, applicationId: e.target.value || undefined })} /></label><label>Route<Input value={form.route} onChange={(e) => setForm({ ...form, route: e.target.value, navigation: { ...form.navigation, route: e.target.value } })} /></label><label>Page Type<Select value={form.pageType} onChange={(e) => setForm({ ...form, pageType: e.target.value as PageType })}>{pageTypes.map((x) => <option key={x}>{x}</option>)}</Select></label><label>Datasource<Input value={form.datasource.entity} onChange={(e) => setForm({ ...form, datasource: { ...form.datasource, entity: e.target.value } })} /></label><label>Icon<Input value={form.icon} onChange={(e) => setForm({ ...form, icon: e.target.value })} /></label><label>Permissions<Input value={form.permissions.map((p) => p.role).join(", ")} onChange={(e) => setForm({ ...form, permissions: e.target.value.split(",").map((role) => ({ role: role.trim(), access: "View" })).filter((p) => p.role) })} /></label></div><label>Description<textarea className="input" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} /></label><h3>Layout</h3><div className="grid">{layouts.map((l) => <Button key={l.template} variant={form.layout.template === l.template ? "primary" : "secondary"} onClick={() => setForm({ ...form, layout: { ...form.layout, template: l.template, regions: l.regions } })}>{l.template}</Button>)}</div><div className="designer-canvas">{regions.map((region) => <section className="panel" key={region}><h3>{region}</h3>{form.components.filter((c) => c.region === region).sort((a,b)=>(a.displayOrder||0)-(b.displayOrder||0)).map((c) => <button className="input" key={c.code} onClick={() => setSelectedCode(c.code)}><strong>{c.name}</strong><br/><span>{c.componentType}</span></button>)}{!form.components.some((c) => c.region === region) && <EmptyState title="Region ready" message="Add components from the toolbox to compose this page region." />}</section>)}</div></Card>
      <Card><h2>Properties</h2>{selectedComponent ? <div className="form-grid"><label>Title<Input value={configOf(selectedComponent).title || selectedComponent.name} onChange={(e) => patchComponent({ name: e.target.value }, { title: e.target.value })} /></label><label>Datasource<Input value={configOf(selectedComponent).datasource || form.datasource.entity} onChange={(e) => patchComponent({}, { datasource: e.target.value })} /></label><label>Visibility Rule<Input value={configOf(selectedComponent).visibilityRule || ""} onChange={(e) => patchComponent({}, { visibilityRule: e.target.value })} /></label><label>Width<Input value={configOf(selectedComponent).width || "12"} onChange={(e) => patchComponent({}, { width: e.target.value })} /></label><label>Height<Input value={configOf(selectedComponent).height || "auto"} onChange={(e) => patchComponent({}, { height: e.target.value })} /></label><label>CSS Class<Input value={configOf(selectedComponent).cssClass || ""} onChange={(e) => patchComponent({}, { cssClass: e.target.value })} /></label><label>Permissions<Input value={configOf(selectedComponent).permissions || ""} onChange={(e) => patchComponent({}, { permissions: e.target.value })} /></label><label>Expressions<textarea className="input" value={configOf(selectedComponent).expressions || ""} onChange={(e) => patchComponent({}, { expressions: e.target.value })} /></label></div> : <EmptyState title="Select a component" message="Choose a canvas component to edit metadata-backed properties." />}</Card>
    </div>
    {preview && <Card><h2>Preview</h2><PageRenderer page={form} componentDefinitions={componentDefinitions} /></Card>}
  </div>;
}
