import { useEffect, useMemo, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { DataTable } from "../../components/ui/DataTable";
import { Button } from "../../components/ui/Button";
import { AdminCard } from "../../components/ui/AdminCard";
import { InfoBox } from "../../components/ui/InfoBox";
import { navigate } from "../../app/routes";
import { getForms } from "../../services/formsApi";
import type { FormDefinition } from "../../types/api";

export function DynamicFormsPage() {
  const [forms, setForms] = useState<FormDefinition[]>([]);

  useEffect(() => {
    void getForms().then((result) => setForms(result.data));
  }, []);

  const publishedVersions = useMemo(
    () =>
      forms.reduce((total, form) => total + (form.versions?.length || 0), 0),
    [forms],
  );
  const entityTypes = new Set(
    forms.map((form) => form.entityType).filter(Boolean),
  );

  return (
    <>
      <PageHeader
        title="Dynamic forms"
        description="Governed form definitions and published versions for LCA eProcurement services."
        actions={
          <Button onClick={() => navigate("/app/forms/designer")}>
            Form designer
          </Button>
        }
      />
      <div className="grid cols-3 dashboard-section">
        <InfoBox
          icon="🧾"
          label="Form definitions"
          value={forms.length}
          variant="primary"
        />
        <InfoBox
          icon="📌"
          label="Published versions"
          value={publishedVersions}
          variant="success"
        />
        <InfoBox
          icon="🏷️"
          label="Entity coverage"
          value={entityTypes.size}
          variant="info"
        />
      </div>
      <AdminCard
        title="LCA eProcurement forms"
        subtitle="Definitions remain driven by the Form Configuration Engine API."
      >
        <DataTable
          rows={forms}
          searchable
          pageSize={8}
          striped
          compact
          columns={[
            {
              header: "Code",
              cell: (form) => form.code,
              sortable: true,
              width: "120px",
            },
            { header: "Name", cell: (form) => form.name, sortable: true },
            {
              header: "Entity type",
              cell: (form) => form.entityType,
              sortable: true,
            },
            {
              header: "Versions",
              cell: (form) => form.versions?.length || 0,
              sortable: true,
            },
          ]}
          empty="No dynamic forms are configured yet."
        />
      </AdminCard>
    </>
  );
}
