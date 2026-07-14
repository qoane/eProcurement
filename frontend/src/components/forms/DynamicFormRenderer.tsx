import { useEffect, useMemo, useState } from "react";
import type { ChangeEvent } from "react";
import { getForms } from "../../services/formsApi";
import type { FormDefinition, FormField } from "../../types/api";

type Props = {
  entityType: string;
  entityId?: string;
  formCode?: string;
  formDefinitionId?: string;
  mode: "create" | "edit" | "view";
  initialData?: Record<string, unknown>;
  onSubmit?: (values: Record<string, unknown>) => void;
};

export function DynamicFormRenderer({ entityType, formCode, mode, initialData = {}, onSubmit }: Props) {
  const [forms, setForms] = useState<FormDefinition[]>([]);
  const [values, setValues] = useState<Record<string, unknown>>(initialData);
  useEffect(() => { getForms().then((r) => setForms(r.data)).catch(() => setForms([])); }, []);
  const form = useMemo(() => forms.find((x) => (formCode ? x.code === formCode : x.entityType === entityType)), [forms, formCode, entityType]);
  const version = form?.versions?.find((v) => String(v.status).toLowerCase() === "published") ?? form?.versions?.[0];
  const readonly = mode === "view";
  if (!form) return <div className="empty-state"><h3>No active form metadata</h3><p>Publish a form definition for {entityType} to render it here.</p></div>;
  const renderField = (field: FormField) => {
    const value = String(values[field.code] ?? "");
    const common = { id: field.code, value, disabled: readonly, placeholder: field.label, onChange: (event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => setValues((current) => ({ ...current, [field.code]: event.target.value })) };
    if (field.fieldType === "TextArea" || field.fieldType === "RichText") return <textarea {...common} className="form-control" />;
    if (field.fieldType === "Date" || field.fieldType === "DateTime") return <input {...common} type={field.fieldType === "Date" ? "date" : "datetime-local"} className="form-control" />;
    if (["Number", "Currency"].includes(field.fieldType)) return <input {...common} type="number" className="form-control" />;
    if (["Select", "SupplierPicker", "TenderPicker", "PurchaseOrderPicker", "ContractPicker", "UserPicker"].includes(field.fieldType)) return <select {...common} className="form-select"><option value="">Select...</option></select>;
    if (field.fieldType === "Checkbox") return <input id={field.code} type="checkbox" disabled={readonly} checked={Boolean(values[field.code])} onChange={(event) => setValues((current) => ({ ...current, [field.code]: event.target.checked }))} />;
    if (field.fieldType === "FileUpload") return <input id={field.code} type="file" disabled={readonly} className="form-control" />;
    return <input {...common} type="text" className="form-control" />;
  };
  return <form className="card" onSubmit={(event) => { event.preventDefault(); onSubmit?.(values); }}>
    <div className="card-header"><h3>{form.name}</h3><span className="badge bg-info">{mode}</span></div>
    <div className="card-body">
      {version?.sections?.sort((a,b)=>(a.displayOrder ?? 0)-(b.displayOrder ?? 0)).map((section) => <section key={section.code} className="mb-4"><h4>{section.title}</h4><div className="row g-3">{section.fields?.sort((a,b)=>(a.displayOrder ?? 0)-(b.displayOrder ?? 0)).map((field) => <div className="col-md-6" key={field.code}><label className="form-label" htmlFor={field.code}>{field.label}{field.isRequired ? " *" : ""}</label>{renderField(field)}</div>)}</div></section>)}
    </div>
    {!readonly && <div className="card-footer"><button className="btn primary" type="submit">Submit preview</button></div>}
  </form>;
}
