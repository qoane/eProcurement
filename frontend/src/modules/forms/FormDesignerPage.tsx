import { useMemo, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { Input } from "../../components/ui/Input";
import { Button } from "../../components/ui/Button";
import { createForm, publishForm } from "../../services/formsApi";

type DesignerField = {
  id: string;
  code: string;
  label: string;
  fieldType: string;
  isRequired: boolean;
  validation: string;
  visibilityRule: string;
  expression: string;
  conditionalBehaviour: string;
};
type DesignerGroup = { id: string; title: string; fields: DesignerField[] };
type DesignerColumn = { id: string; title: string; groups: DesignerGroup[] };
type DesignerSection = { id: string; code: string; title: string; tab: string; columns: DesignerColumn[] };

const fieldPalette = ["text", "email", "number", "date", "textarea", "select", "checkbox"];
const newId = (prefix: string) => `${prefix}-${crypto.randomUUID?.() ?? Date.now()}`;
const slug = (value: string) => value.trim().replace(/[^a-zA-Z0-9]+/g, "-").replace(/^-|-$/g, "").toLowerCase();

const starterSections = (): DesignerSection[] => [
  {
    id: newId("section"),
    code: "organisation-profile",
    title: "Organisation profile",
    tab: "Profile",
    columns: [
      { id: newId("column"), title: "Legal details", groups: [{ id: newId("group"), title: "Supplier identity", fields: [
        { id: newId("field"), code: "legalName", label: "Legal name", fieldType: "text", isRequired: true, validation: "required", visibilityRule: "", expression: "", conditionalBehaviour: "" },
        { id: newId("field"), code: "registrationNumber", label: "Registration number", fieldType: "text", isRequired: true, validation: "required|min:3", visibilityRule: "", expression: "", conditionalBehaviour: "" }
      ] }] },
      { id: newId("column"), title: "Contact", groups: [{ id: newId("group"), title: "Primary contact", fields: [
        { id: newId("field"), code: "contactEmail", label: "Contact email", fieldType: "email", isRequired: true, validation: "required|email", visibilityRule: "", expression: "", conditionalBehaviour: "show when legalName is not empty" },
        { id: newId("field"), code: "contactPhone", label: "Contact phone", fieldType: "text", isRequired: false, validation: "", visibilityRule: "", expression: "", conditionalBehaviour: "" }
      ] }] }
    ]
  }
];

export function FormDesignerPage() {
  const [code, setCode] = useState("SUPPLIER-REGISTRATION-FORM");
  const [name, setName] = useState("Supplier Registration");
  const [entityType, setEntityType] = useState("Supplier");
  const [sections, setSections] = useState<DesignerSection[]>(starterSections);
  const [activeTab, setActiveTab] = useState("Profile");
  const [selectedFieldId, setSelectedFieldId] = useState<string>();
  const [message, setMessage] = useState<string>();

  const tabs = useMemo(() => Array.from(new Set(sections.map((s) => s.tab || "General"))), [sections]);
  const selected = sections.flatMap((s) => s.columns.flatMap((c) => c.groups.flatMap((g) => g.fields))).find((f) => f.id === selectedFieldId);
  const updateField = (patch: Partial<DesignerField>) => setSections((items) => items.map((s) => ({ ...s, columns: s.columns.map((c) => ({ ...c, groups: c.groups.map((g) => ({ ...g, fields: g.fields.map((f) => f.id === selectedFieldId ? { ...f, ...patch } : f) })) })) })));
  const moveField = (fieldId: string, targetGroupId: string) => setSections((items) => {
    let moving: DesignerField | undefined;
    const without = items.map((s) => ({ ...s, columns: s.columns.map((c) => ({ ...c, groups: c.groups.map((g) => ({ ...g, fields: g.fields.filter((f) => { if (f.id === fieldId) { moving = f; return false; } return true; }) })) })) }));
    if (!moving) return items;
    return without.map((s) => ({ ...s, columns: s.columns.map((c) => ({ ...c, groups: c.groups.map((g) => g.id === targetGroupId ? { ...g, fields: [...g.fields, moving!] } : g) })) }));
  });
  const addField = (fieldType: string, groupId: string) => setSections((items) => items.map((s) => ({ ...s, columns: s.columns.map((c) => ({ ...c, groups: c.groups.map((g) => g.id === groupId ? { ...g, fields: [...g.fields, { id: newId("field"), code: `${fieldType}${g.fields.length + 1}`, label: `New ${fieldType} field`, fieldType, isRequired: false, validation: "", visibilityRule: "", expression: "", conditionalBehaviour: "" }] } : g) })) })));
  const addSection = () => setSections((items) => [...items, { id: newId("section"), code: `section-${items.length + 1}`, title: `Section ${items.length + 1}`, tab: "General", columns: [{ id: newId("column"), title: "Column 1", groups: [{ id: newId("group"), title: "Group 1", fields: [] }] }] }]);
  const addColumn = (sectionId: string) => setSections((items) => items.map((s) => s.id === sectionId ? { ...s, columns: [...s.columns, { id: newId("column"), title: `Column ${s.columns.length + 1}`, groups: [{ id: newId("group"), title: "Group", fields: [] }] }] } : s));
  const addGroup = (columnId: string) => setSections((items) => items.map((s) => ({ ...s, columns: s.columns.map((c) => c.id === columnId ? { ...c, groups: [...c.groups, { id: newId("group"), title: `Group ${c.groups.length + 1}`, fields: [] }] } : c) })));

  async function save(publish: boolean) {
    const payload = { code, name, entityType, sections: sections.map((section, si) => ({ code: section.code || slug(section.title), title: section.title, displayOrder: si + 1, fields: section.columns.flatMap((column, ci) => column.groups.flatMap((group, gi) => group.fields.map((field, fi) => ({ code: field.code, label: field.label, fieldType: field.fieldType, displayOrder: (ci * 100) + (gi * 10) + fi + 1, isRequired: field.isRequired, validations: field.validation ? [{ validationType: field.validation, configurationJson: JSON.stringify({ section: section.title, tab: section.tab, column: column.title, group: group.title, expression: field.expression, conditionalBehaviour: field.conditionalBehaviour }), message: `${field.label} must satisfy ${field.validation}` }] : [], visibilityRules: field.visibilityRule ? [{ expression: field.visibilityRule }] : [] })))) })) };
    await createForm(payload);
    if (publish) await publishForm(code, "form.designer@demo.co.ls");
    setMessage(publish ? "Form saved and published for Supplier Registration." : "Draft form saved.");
  }

  return <>
    <PageHeader title="Drag-and-drop form designer" description="Design sections, columns, tabs, groups, fields, validation, visibility rules, expressions, conditional behaviour, preview, and publish." actions={<><Button variant="secondary" onClick={() => void save(false)}>Save draft</Button><Button onClick={() => void save(true)}>Publish</Button></>} />
    {message && <p className="success">{message}</p>}
    <div className="designer-shell">
      <aside className="designer-palette"><h3>Fields</h3>{fieldPalette.map((type) => <div key={type} className="palette-item" draggable onDragStart={(e) => e.dataTransfer.setData("fieldType", type)}>{type}</div>)}<Button variant="secondary" onClick={addSection}>Add section</Button></aside>
      <main className="designer-canvas">
        <div className="form-grid"><label>Code<Input value={code} onChange={(e) => setCode(e.target.value)} /></label><label>Name<Input value={name} onChange={(e) => setName(e.target.value)} /></label><label>Entity type<Input value={entityType} onChange={(e) => setEntityType(e.target.value)} /></label></div>
        <div className="tabs">{tabs.map((tab) => <button className={`tab ${activeTab === tab ? "active" : ""}`} onClick={() => setActiveTab(tab)} key={tab}>{tab}</button>)}</div>
        {sections.filter((s) => (s.tab || "General") === activeTab).map((section) => <section className="designer-section" key={section.id}><div className="designer-section-head"><Input value={section.title} onChange={(e) => setSections((xs) => xs.map((x) => x.id === section.id ? { ...x, title: e.target.value, code: slug(e.target.value) } : x))} /><Input value={section.tab} onChange={(e) => setSections((xs) => xs.map((x) => x.id === section.id ? { ...x, tab: e.target.value } : x))} /><Button variant="secondary" onClick={() => addColumn(section.id)}>Add column</Button></div><div className="designer-columns" style={{ gridTemplateColumns: `repeat(${section.columns.length}, minmax(0, 1fr))` }}>{section.columns.map((column) => <div className="designer-column" key={column.id}><strong>{column.title}</strong><Button variant="secondary" onClick={() => addGroup(column.id)}>Add group</Button>{column.groups.map((group) => <div className="designer-group" key={group.id} onDragOver={(e) => e.preventDefault()} onDrop={(e) => { const fieldType = e.dataTransfer.getData("fieldType"); const fieldId = e.dataTransfer.getData("fieldId"); if (fieldType) addField(fieldType, group.id); if (fieldId) moveField(fieldId, group.id); }}><h4>{group.title}</h4>{group.fields.map((field) => <button type="button" className={`designer-field ${selectedFieldId === field.id ? "active" : ""}`} key={field.id} draggable onDragStart={(e) => e.dataTransfer.setData("fieldId", field.id)} onClick={() => setSelectedFieldId(field.id)}><span>{field.label}</span><small>{field.fieldType}{field.isRequired ? " · required" : ""}</small></button>)}</div>)}</div>)}</div></section>)}
      </main>
      <aside className="designer-properties"><h3>Properties</h3>{selected ? <><label>Label<Input value={selected.label} onChange={(e) => updateField({ label: e.target.value })} /></label><label>Code<Input value={selected.code} onChange={(e) => updateField({ code: e.target.value })} /></label><label>Validation<Input value={selected.validation} onChange={(e) => updateField({ validation: e.target.value })} placeholder="required|email|min:3" /></label><label>Visibility rule<Input value={selected.visibilityRule} onChange={(e) => updateField({ visibilityRule: e.target.value })} placeholder="legalName != ''" /></label><label>Expression<Input value={selected.expression} onChange={(e) => updateField({ expression: e.target.value })} placeholder="concat(...)" /></label><label>Conditional behaviour<Input value={selected.conditionalBehaviour} onChange={(e) => updateField({ conditionalBehaviour: e.target.value })} /></label><label><input type="checkbox" checked={selected.isRequired} onChange={(e) => updateField({ isRequired: e.target.checked })} /> Required</label></> : <p>Select a field to edit validation, rules, expressions, and behaviour.</p>}<h3>Preview</h3><div className="preview-card">{sections.flatMap((s) => s.columns.flatMap((c) => c.groups.flatMap((g) => g.fields))).map((f) => <label key={f.id}>{f.label}{f.isRequired ? " *" : ""}<Input type={f.fieldType === "email" ? "email" : "text"} placeholder={f.visibilityRule ? `Visible when ${f.visibilityRule}` : ""} /></label>)}</div></aside>
    </div>
  </>;
}
