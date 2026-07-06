import { useEffect, useMemo, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { AdminCard } from "../../components/ui/AdminCard";
import { InfoBox } from "../../components/ui/InfoBox";
import { DataTable } from "../../components/ui/DataTable";
import { Button } from "../../components/ui/Button";
import { Badge } from "../../components/ui/Badge";
import {
  getConfigurationStudio,
  publishBusinessProcess,
} from "../../services/configurationApi";
import type { ConfigurationStudio } from "../../types/api";

type WorkflowMappingRow = {
  actionCode?: string;
  workflowDefinitionCode?: string;
  entityType?: string;
};

function asWorkflowMapping(row: unknown): WorkflowMappingRow {
  return row && typeof row === "object" ? (row as WorkflowMappingRow) : {};
}

const emptyStudio: ConfigurationStudio = {
  businessProcesses: [],
  documentRequirementSets: [],
  approvalMatrices: [],
  workflowMappings: [],
};

export function ConfigurationPage() {
  const [studio, setStudio] = useState<ConfigurationStudio>(emptyStudio);
  const [message, setMessage] = useState("");

  async function load() {
    setStudio((await getConfigurationStudio()).data);
  }

  useEffect(() => {
    void load();
  }, []);

  async function publish(code: string) {
    await publishBusinessProcess(code);
    setMessage(`${code} published from configuration.`);
    await load();
  }

  const publishedProcesses = useMemo(
    () =>
      studio.businessProcesses.filter(
        (process) => process.status?.toLowerCase() === "published",
      ).length,
    [studio.businessProcesses],
  );

  return (
    <>
      <PageHeader
        title="Administration Studio"
        description="Configuration-first orchestration for LCA eProcurement business processes, document requirements, approval matrices, and workflow mappings."
      />
      {message && (
        <AdminCard title="Configuration update" className="dashboard-section">
          <strong>{message}</strong>
        </AdminCard>
      )}
      <div className="grid cols-4 dashboard-section">
        <InfoBox
          icon="🔁"
          label="Business processes"
          value={studio.businessProcesses.length}
          variant="primary"
        />
        <InfoBox
          icon="📄"
          label="Document sets"
          value={studio.documentRequirementSets.length}
          variant="info"
        />
        <InfoBox
          icon="🧭"
          label="Approval matrices"
          value={studio.approvalMatrices.length}
          variant="warning"
        />
        <InfoBox
          icon="🚀"
          label="Published processes"
          value={publishedProcesses}
          variant="success"
        />
      </div>
      <div className="grid cols-2">
        <AdminCard
          title="Business processes"
          subtitle="Publish configured LCA eProcurement processes after their form, workflow, documents, and approval matrix are selected."
        >
          <DataTable
            rows={studio.businessProcesses}
            searchable
            compact
            striped
            columns={[
              {
                header: "Name",
                cell: (process) => process.name,
                sortable: true,
              },
              {
                header: "Code",
                cell: (process) => process.code,
                sortable: true,
              },
              {
                header: "Entity type",
                cell: (process) => process.entityType,
                sortable: true,
              },
              {
                header: "Status",
                cell: (process) => <Badge>{process.status}</Badge>,
                sortable: true,
              },
              {
                header: "Action",
                cell: (process) => (
                  <Button onClick={() => publish(process.code)}>Publish</Button>
                ),
                filterable: false,
              },
            ]}
          />
        </AdminCard>
        <AdminCard
          title="Document requirements"
          subtitle="Evidence requirements returned by the configuration API."
        >
          <DataTable
            rows={studio.documentRequirementSets}
            compact
            striped
            columns={[
              { header: "Set", cell: (set) => set.name, sortable: true },
              {
                header: "Required documents",
                cell: (set) =>
                  set.requirements
                    ?.map((requirement) => requirement.documentType)
                    .join(", ") || "—",
              },
            ]}
          />
        </AdminCard>
        <AdminCard
          title="Approval matrices"
          subtitle="Authority paths are displayed from configured matrix steps."
        >
          <DataTable
            rows={studio.approvalMatrices}
            compact
            striped
            columns={[
              {
                header: "Matrix",
                cell: (matrix) => matrix.name,
                sortable: true,
              },
              {
                header: "Approval path",
                cell: (matrix) =>
                  [...(matrix.steps || [])]
                    .sort((a, b) => a.sequence - b.sequence)
                    .map((step) => `${step.sequence}. ${step.role}`)
                    .join(" → ") || "—",
              },
            ]}
          />
        </AdminCard>
        <AdminCard
          title="Workflow mappings"
          subtitle="Mappings remain editable for action-to-workflow integration scenarios."
        >
          <DataTable
            rows={studio.workflowMappings.map(asWorkflowMapping)}
            compact
            striped
            columns={[
              {
                header: "Action",
                cell: (mapping) => mapping.actionCode || "—",
                sortable: true,
              },
              {
                header: "Workflow",
                cell: (mapping) => mapping.workflowDefinitionCode || "—",
                sortable: true,
              },
              {
                header: "Entity",
                cell: (mapping) => mapping.entityType || "—",
                sortable: true,
              },
            ]}
          />
        </AdminCard>
      </div>
    </>
  );
}
