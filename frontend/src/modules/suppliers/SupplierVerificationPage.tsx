import { useEffect, useMemo, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { Panel } from "../../components/ui/Panel";
import { Button } from "../../components/ui/Button";
import { DataTable } from "../../components/ui/DataTable";
import { StatusBadge } from "../../components/ui/Badge";
import { InfoBox } from "../../components/ui/InfoBox";
import { navigate } from "../../app/routes";
import { getTaskDetail, getTasks } from "../../services/tasksApi";
import type { Supplier, WorkflowTask } from "../../types/api";

type SupplierWorkflowTask = {
  task: WorkflowTask;
  supplier?: Supplier;
  documentsCount: number;
  availableActionsCount: number;
};

type TaskDetailResponse = {
  task?: WorkflowTask;
  supplier?: {
    supplier?: Supplier;
    documents?: unknown[];
  };
  availableActions?: unknown[];
};

function isOpenTask(task: WorkflowTask) {
  const status = task.status?.toLowerCase() ?? "";
  return (
    !task.completedAt && !status.includes("complete") && status !== "cancelled"
  );
}

export function SupplierVerificationPage() {
  const [items, setItems] = useState<SupplierWorkflowTask[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>();

  async function load() {
    setLoading(true);
    const tasksResult = await getTasks();
    setError(tasksResult.error);

    const openTasks = tasksResult.data.filter(isOpenTask);
    const detailResults: Array<SupplierWorkflowTask | null> = await Promise.all(
      openTasks.map(async (task): Promise<SupplierWorkflowTask | null> => {
        const detailResult = await getTaskDetail(task.id);
        const detail = detailResult.data as TaskDetailResponse | null;
        if (!detail?.supplier?.supplier) return null;
        return {
          task: detail.task ?? task,
          supplier: detail.supplier.supplier,
          documentsCount: detail.supplier.documents?.length ?? 0,
          availableActionsCount: detail.availableActions?.length ?? 0,
        } satisfies SupplierWorkflowTask;
      }),
    );

    setItems(
      detailResults.filter((item): item is SupplierWorkflowTask =>
        Boolean(item),
      ),
    );
    setLoading(false);
  }

  useEffect(() => {
    void load();
  }, []);

  const underVerification = useMemo(
    () =>
      items.filter((item) =>
        item.supplier?.status?.toLowerCase().includes("verification"),
      ).length,
    [items],
  );
  const documentChecks = items.filter(
    (item) => item.task.nodeCode === "DocumentCheck",
  ).length;
  const actionReady = items.filter(
    (item) => item.availableActionsCount > 0,
  ).length;

  return (
    <>
      <PageHeader
        title="Supplier verification"
        description="Review supplier onboarding workflow tasks that are ready for document checks, verification, and approval hand-off."
        actions={
          <Button variant="secondary" onClick={load}>
            Refresh
          </Button>
        }
      />
      <div className="grid cols-3 dashboard-section">
        <InfoBox
          icon="📋"
          label="Open supplier tasks"
          value={items.length}
          trend="Loaded from the workflow inbox"
          variant="primary"
        />
        <InfoBox
          icon="📄"
          label="Document checks"
          value={documentChecks}
          trend="Registration evidence awaiting review"
          variant="warning"
        />
        <InfoBox
          icon="✅"
          label="Action ready"
          value={actionReady}
          trend="Tasks with available workflow actions"
          variant="success"
        />
      </div>
      {error && <div className="alert alert-warning">{error}</div>}
      <Panel title="Supplier verification queue">
        <DataTable
          rows={items}
          searchable
          pageSize={10}
          striped
          compact
          empty={
            loading
              ? "Loading supplier verification tasks…"
              : "No supplier workflow tasks are currently waiting for verification."
          }
          columns={[
            {
              header: "Supplier",
              cell: (item) =>
                item.supplier?.legalName ??
                item.supplier?.referenceNumber ??
                "Unknown supplier",
              sortable: true,
            },
            {
              header: "Reference",
              cell: (item) => item.supplier?.referenceNumber ?? "—",
              sortable: true,
            },
            {
              header: "Stage",
              cell: (item) =>
                item.task.nodeCode || item.task.title || item.task.id,
              sortable: true,
            },
            {
              header: "Supplier status",
              cell: (item) => <StatusBadge status={item.supplier?.status} />,
              sortable: true,
            },
            {
              header: "Task status",
              cell: (item) => <StatusBadge status={item.task.status} />,
              sortable: true,
            },
            {
              header: "Documents",
              cell: (item) => item.documentsCount,
              sortable: true,
            },
            {
              header: "Assignee",
              cell: (item) =>
                item.task.assignedTo || item.task.assignedRole || "Team queue",
              sortable: true,
            },
            {
              header: "Action",
              cell: (item) => (
                <Button
                  variant="secondary"
                  onClick={() => navigate(`/app/tasks/${item.task.id}`)}
                >
                  Open verification
                </Button>
              ),
              filterable: false,
            },
          ]}
        />
      </Panel>
      {!loading && underVerification > 0 && (
        <p className="muted">
          {underVerification} supplier(s) are already in the UnderVerification
          status.
        </p>
      )}
    </>
  );
}
