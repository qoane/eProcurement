import { useEffect, useMemo, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { EmptyState } from "../../components/ui/EmptyState";
import { Button } from "../../components/ui/Button";
import { Input } from "../../components/ui/Input";
import { Card } from "../../components/ui/Card";
import { registerSupplier, getSupplierRegistrationConfiguration } from "../../services/suppliersApi";
import type { DocumentRequirementSet, FormDefinition } from "../../types/api";

export function SupplierRegistrationPage() {
  const [form, setForm] = useState<FormDefinition | null>(null);
  const [docs, setDocs] = useState<DocumentRequirementSet | null>(null);
  const [message, setMessage] = useState<string>();
  useEffect(() => {
    void getSupplierRegistrationConfiguration().then((r) => {
      const data = r.data as { form?: FormDefinition; documentRequirements?: DocumentRequirementSet } | null;
      setForm(data?.form ?? null);
      setDocs(data?.documentRequirements ?? null);
    });
  }, []);
  const sections = useMemo(() => form?.versions?.find((v) => v.status === "Published")?.sections ?? form?.versions?.[0]?.sections ?? [], [form]);
  return (
    <>
      <PageHeader title="Supplier registration" description="Supplier Management consumes the published business process, active dynamic form, document requirements, and workflow configuration from the platform." />
      {form ? (
        <div className="grid cols-2">
          <form className="panel form-grid" onSubmit={async (event) => {
              event.preventDefault();
              const fd = new FormData(event.currentTarget);
              const values = Object.fromEntries(sections.flatMap((s) => s.fields ?? []).map((f) => [f.code, String(fd.get(f.code) ?? "")]));
              const referenceNumber = String(fd.get("referenceNumber") || `SUP-${Date.now()}`);
              const documents = (docs?.requirements ?? []).map((d) => ({ documentType: d.documentType, fileName: String(fd.get(`doc_${d.documentType}`) || `${d.documentType}.pdf`) }));
              const result = await registerSupplier({ referenceNumber, actor: "supplier@demo.co.ls", values, documents });
              const workflowInstanceId = (result.data as { workflowInstanceId?: string } | null)?.workflowInstanceId;
              setMessage(workflowInstanceId ? `Registration submitted. Workflow instance: ${workflowInstanceId}` : "Registration could not be submitted.");
            }}>
            <label>Supplier reference *<Input name="referenceNumber" defaultValue={`SUP-LCA-${new Date().getFullYear()}-${Date.now().toString().slice(-4)}`} /></label>
            {sections.flatMap((section) => section.fields ?? []).sort((a, b) => (a.displayOrder ?? 0) - (b.displayOrder ?? 0)).map((field) => (
              <label key={field.code}>{field.label}{field.isRequired ? " *" : ""}<Input type={field.fieldType === "email" ? "email" : "text"} name={field.code} /></label>
            ))}
            {docs?.requirements?.map((r) => <label key={`upload-${r.documentType}`}>{r.documentType} file metadata{r.required ? " *" : ""}<Input name={`doc_${r.documentType}`} defaultValue={`${r.documentType}.pdf`} /></label>)}
            {message && <p className="success">{message}</p>}
            <Button>Submit through configured process</Button>
          </form>
          <Card><h2>Required documents</h2>{docs?.requirements?.map((r) => <p key={r.documentType}><strong>{r.documentType}</strong>: {r.minimumFiles}-{r.maximumFiles} file(s), {r.allowedExtensions}, max {Math.round(r.maximumFileSize / 1048576)}MB</p>)}</Card>
        </div>
      ) : (
        <EmptyState title="No published supplier registration process" message="Publish a business process with active form, document requirements, approval matrix, and workflow in Administration Studio." />
      )}
    </>
  );
}
