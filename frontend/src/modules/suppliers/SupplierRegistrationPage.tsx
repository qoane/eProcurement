import { useEffect, useMemo, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { EmptyState } from "../../components/ui/EmptyState";
import { Button } from "../../components/ui/Button";
import { Input } from "../../components/ui/Input";
import { Card } from "../../components/ui/Card";
import { getForms } from "../../services/formsApi";
import { getConfigurationStudio } from "../../services/configurationApi";
import type { DocumentRequirementSet, FormDefinition } from "../../types/api";

export function SupplierRegistrationPage() {
  const [form, setForm] = useState<FormDefinition | null>(null);
  const [docs, setDocs] = useState<DocumentRequirementSet | null>(null);
  useEffect(() => {
    void Promise.all([getConfigurationStudio(), getForms()]).then(([studio, forms]) => {
      const process = studio.data.businessProcesses.find((p) => p.entityType === "Supplier" && p.status === "Published");
      setForm(forms.data.find((f) => f.entityType === process?.entityType && f.versions?.some((v) => v.status === "Published")) ?? null);
      setDocs(studio.data.documentRequirementSets.find((d) => d.id === process?.activeDocumentRequirementSetId) ?? null);
    });
  }, []);
  const sections = useMemo(() => form?.versions?.find((v) => v.status === "Published")?.sections ?? form?.versions?.[0]?.sections ?? [], [form]);
  return (
    <>
      <PageHeader title="Supplier registration" description="Supplier Management consumes the published business process, active dynamic form, document requirements, and workflow configuration from the platform." />
      {form ? (
        <div className="grid cols-2">
          <form className="panel form-grid">
            {sections.flatMap((section) => section.fields ?? []).sort((a, b) => (a.displayOrder ?? 0) - (b.displayOrder ?? 0)).map((field) => (
              <label key={field.code}>{field.label}{field.isRequired ? " *" : ""}<Input type={field.fieldType === "email" ? "email" : "text"} name={field.code} /></label>
            ))}
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
