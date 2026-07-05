import { useEffect, useMemo, useState } from "react";
import { Badge } from "../../components/ui/Badge";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { DataTable } from "../../components/ui/DataTable";
import { Input } from "../../components/ui/Input";
import { PageHeader } from "../../components/ui/PageHeader";
import { Select } from "../../components/ui/Select";
import { createEntityDefinition, deleteEntityDefinition, getEntityDefinitions, updateEntityDefinition } from "../../services/entityDefinitionsApi";
import type { EntityDesigner } from "../../types/api";

const blank: EntityDesigner = {
  code: "SUPPLIER",
  name: "Supplier",
  displayName: "Supplier",
  pluralName: "Suppliers",
  defaultSearchField: "legalName",
  description: "Business entity metadata used by forms, rules, workflows, and future runtime generation.",
  properties: [
    { code: "legalName", name: "Legal Name", dataType: "Text", required: true, searchable: true },
    { code: "status", name: "Status", dataType: "Lookup", required: true, searchable: true },
  ],
  relationships: [{ code: "documents", name: "Documents", targetEntity: "Document", cardinality: "OneToMany", required: false }],
  validations: [{ code: "legalName-required", name: "Legal name required", expression: "legalName != null", message: "Legal name is required." }],
  status: "Draft",
  version: 1,
};

const dataTypes = ["Text", "Number", "Decimal", "Boolean", "Date", "DateTime", "Lookup", "Money", "Guid"];
const cardinalities = ["OneToOne", "OneToMany", "ManyToOne", "ManyToMany"];

export function EntityDesignerPage() {
  const [entities, setEntities] = useState<EntityDesigner[]>([]);
  const [form, setForm] = useState<EntityDesigner>(blank);
  const [message, setMessage] = useState("");
  const selected = useMemo(() => entities.find((x) => x.id === form.id), [entities, form.id]);
  async function load() { setEntities((await getEntityDefinitions()).data); }
  useEffect(() => { void load(); }, []);
  async function save() { form.id ? await updateEntityDefinition(form.id, form) : await createEntityDefinition(form); setMessage(`${form.displayName} metadata saved. No C# classes were generated.`); setForm(blank); await load(); }
  async function remove(id: string) { await deleteEntityDefinition(id); setMessage("Entity metadata deleted."); if (form.id === id) setForm(blank); await load(); }
  const propertyOptions = form.properties.map((p) => p.code);

  return (
    <div className="studio-page">
      <PageHeader title="Entity Designer" description="Define business entity metadata for properties, relationships, validation, display names, and default search fields without generating C# classes." actions={<Badge tone="info">Metadata only</Badge>} />
      {message && <Card><strong>{message}</strong></Card>}
      <div className="grid cols-2">
        <Card>
          <h2>{selected ? "Edit entity" : "Create entity"}</h2>
          <div className="form-grid">
            <label>Name<Input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value, displayName: e.target.value })} /></label>
            <label>Code<Input value={form.code} onChange={(e) => setForm({ ...form, code: e.target.value.toUpperCase().replace(/\s+/g, "-") })} /></label>
            <label>Display name<Input value={form.displayName} onChange={(e) => setForm({ ...form, displayName: e.target.value })} /></label>
            <label>Plural name<Input value={form.pluralName} onChange={(e) => setForm({ ...form, pluralName: e.target.value })} /></label>
            <label>Default search field<Select value={form.defaultSearchField} onChange={(e) => setForm({ ...form, defaultSearchField: e.target.value })}>{propertyOptions.map((p) => <option key={p}>{p}</option>)}</Select></label>
            <label>Status<Select value={form.status} onChange={(e) => setForm({ ...form, status: e.target.value })}><option>Draft</option><option>Active</option><option>Inactive</option><option>Archived</option></Select></label>
          </div>
          <label>Description<textarea className="input" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} /></label>
          <h3>Properties</h3>
          {form.properties.map((property, index) => <div className="form-grid" key={index}>
            <Input value={property.code} onChange={(e) => setForm({ ...form, properties: form.properties.map((p, i) => i === index ? { ...p, code: e.target.value } : p) })} />
            <Input value={property.name} onChange={(e) => setForm({ ...form, properties: form.properties.map((p, i) => i === index ? { ...p, name: e.target.value } : p) })} />
            <Select value={property.dataType} onChange={(e) => setForm({ ...form, properties: form.properties.map((p, i) => i === index ? { ...p, dataType: e.target.value } : p) })}>{dataTypes.map((x) => <option key={x}>{x}</option>)}</Select>
            <label><input type="checkbox" checked={!!property.required} onChange={(e) => setForm({ ...form, properties: form.properties.map((p, i) => i === index ? { ...p, required: e.target.checked } : p) })} /> Required</label>
          </div>)}
          <Button variant="secondary" onClick={() => setForm({ ...form, properties: [...form.properties, { code: "newField", name: "New Field", dataType: "Text" }] })}>Add property</Button>
          <h3>Relationships</h3>
          {form.relationships.map((relationship, index) => <div className="form-grid" key={index}>
            <Input value={relationship.code} onChange={(e) => setForm({ ...form, relationships: form.relationships.map((r, i) => i === index ? { ...r, code: e.target.value } : r) })} />
            <Input value={relationship.targetEntity} onChange={(e) => setForm({ ...form, relationships: form.relationships.map((r, i) => i === index ? { ...r, targetEntity: e.target.value } : r) })} />
            <Select value={relationship.cardinality} onChange={(e) => setForm({ ...form, relationships: form.relationships.map((r, i) => i === index ? { ...r, cardinality: e.target.value } : r) })}>{cardinalities.map((x) => <option key={x}>{x}</option>)}</Select>
          </div>)}
          <Button variant="secondary" onClick={() => setForm({ ...form, relationships: [...form.relationships, { code: "newRelationship", name: "New Relationship", targetEntity: "Supplier", cardinality: "ManyToOne" }] })}>Add relationship</Button>
          <h3>Validation</h3>
          {form.validations.map((validation, index) => <div className="form-grid" key={index}>
            <Input value={validation.code} onChange={(e) => setForm({ ...form, validations: form.validations.map((v, i) => i === index ? { ...v, code: e.target.value } : v) })} />
            <Input value={validation.expression} onChange={(e) => setForm({ ...form, validations: form.validations.map((v, i) => i === index ? { ...v, expression: e.target.value } : v) })} />
            <Input value={validation.message} onChange={(e) => setForm({ ...form, validations: form.validations.map((v, i) => i === index ? { ...v, message: e.target.value } : v) })} />
          </div>)}
          <Button variant="secondary" onClick={() => setForm({ ...form, validations: [...form.validations, { code: "new-validation", name: "New validation", expression: "field != null", message: "Field is required." }] })}>Add validation</Button>
          <div className="actions"><Button onClick={save}>{form.id ? "Update" : "Create"} Entity Metadata</Button><Button variant="secondary" onClick={() => setForm(blank)}>Reset</Button></div>
        </Card>
        <Card><h2>Runtime metadata preview</h2><p className="muted">Stored as metadata for future runtime generation. This designer does not emit source files.</p><pre>{JSON.stringify(form, null, 2)}</pre></Card>
      </div>
      <Card><h2>Entities</h2><DataTable rows={entities} empty="No entities have been defined." columns={[{ header: "Entity", cell: (row) => <><strong>{row.displayName}</strong><p className="muted">{row.code} · {row.pluralName}</p></> }, { header: "Default search", cell: (row) => row.defaultSearchField }, { header: "Properties", cell: (row) => row.properties.length }, { header: "Relationships", cell: (row) => row.relationships.length }, { header: "Validation", cell: (row) => row.validations.length }, { header: "Status", cell: (row) => <Badge>{row.status || "Draft"}</Badge> }, { header: "Actions", cell: (row) => <div className="actions"><Button variant="secondary" onClick={() => setForm(row)}>Edit</Button>{row.id && <Button variant="secondary" onClick={() => remove(row.id!)}>Delete</Button>}</div> }]} /></Card>
    </div>
  );
}
