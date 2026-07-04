import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { DataTable } from "../../components/ui/DataTable";
import { StatusBadge } from "../../components/ui/Badge";
import { Button } from "../../components/ui/Button";
import { navigate } from "../../app/routes";
import { getWorkflows } from "../../services/workflowsApi";
import type { WorkflowDefinition } from "../../types/api";
export function WorkflowListPage() {
  const [w, setW] = useState<WorkflowDefinition[]>([]);
  useEffect(() => {
    void getWorkflows().then((r) => setW(r.data));
  }, []);
  return (
    <>
      <PageHeader
        title="Workflows"
        description="Workflow definitions, versions and entity mappings."
        actions={
          <Button onClick={() => navigate("/app/workflows/designer")}>
            Open designer
          </Button>
        }
      />
      <section className="panel">
        <DataTable
          rows={w}
          columns={[
            { header: "Code", cell: (r) => r.code },
            { header: "Name", cell: (r) => r.name },
            { header: "Entity type", cell: (r) => r.entityType },
            { header: "Versions", cell: (r) => r.versions?.length || 0 },
            {
              header: "Status",
              cell: (r) => <StatusBadge status={r.versions?.[0]?.status} />,
            },
          ]}
        />
      </section>
    </>
  );
}
