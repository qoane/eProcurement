import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { DataTable } from "../../components/ui/DataTable";
import { Button } from "../../components/ui/Button";
import { navigate } from "../../app/routes";
import { getForms } from "../../services/formsApi";
import type { FormDefinition } from "../../types/api";
export function DynamicFormsPage() {
  const [f, setF] = useState<FormDefinition[]>([]);
  useEffect(() => {
    void getForms().then((r) => setF(r.data));
  }, []);
  return (
    <>
      <PageHeader
        title="Dynamic forms"
        description="Form definitions and published versions."
        actions={
          <Button onClick={() => navigate("/app/forms/designer")}>
            Form designer
          </Button>
        }
      />
      <section className="panel">
        <DataTable
          rows={f}
          searchable
          pageSize={8}
          striped
          compact
          columns={[
            {
              header: "Code",
              cell: (x) => x.code,
              sortable: true,
              width: "120px",
            },
            { header: "Name", cell: (x) => x.name, sortable: true },
            {
              header: "Entity type",
              cell: (x) => x.entityType,
              sortable: true,
            },
            { header: "Versions", cell: (x) => x.versions?.length || 0 },
          ]}
          empty="No dynamic forms are configured yet."
        />
      </section>
    </>
  );
}
