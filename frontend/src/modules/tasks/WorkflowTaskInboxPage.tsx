import { useEffect, useMemo, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { DataTable } from "../../components/ui/DataTable";
import { StatusBadge } from "../../components/ui/Badge";
import { Tabs } from "../../components/ui/Tabs";
import { Button } from "../../components/ui/Button";
import { AdminCard } from "../../components/ui/AdminCard";
import { InfoBox } from "../../components/ui/InfoBox";
import { navigate } from "../../app/routes";
import { getTasks } from "../../services/tasksApi";
import type { WorkflowTask } from "../../types/api";

export function WorkflowTaskInboxPage() {
  const [tasks, setTasks] = useState<WorkflowTask[]>([]);

  useEffect(() => {
    void getTasks().then((result) => setTasks(result.data));
  }, []);

  const completedTasks = useMemo(
    () =>
      tasks.filter(
        (task) =>
          task.completedAt || task.status?.toLowerCase().includes("complete"),
      ).length,
    [tasks],
  );
  const openTasks = Math.max(tasks.length - completedTasks, 0);
  const teamQueueTasks = tasks.filter((task) => !task.assignedTo).length;

  return (
    <>
      <PageHeader
        title="Workflow task inbox"
        description="Lesotho Communications Authority eProcurement verification, review, and approval tasks loaded from the workflow API."
      />
      <div className="grid cols-3 dashboard-section">
        <InfoBox
          icon="📥"
          label="Open LCA tasks"
          value={openTasks}
          trend="Pending officer or team action"
          variant="primary"
        />
        <InfoBox
          icon="👥"
          label="Team queue"
          value={teamQueueTasks}
          trend="Unassigned workflow items"
          variant="warning"
        />
        <InfoBox
          icon="✅"
          label="Completed tasks"
          value={completedTasks}
          trend="Closed through workflow outcomes"
          variant="success"
        />
      </div>
      <AdminCard
        title="Task queue"
        subtitle="Supplier Management workflow stages and assignments are preserved from the workflow service."
        tools={<Tabs items={["My tasks", "Team tasks", "Completed tasks"]} />}
      >
        <DataTable
          rows={tasks}
          searchable
          pageSize={10}
          striped
          compact
          empty="No workflow tasks are available from the API."
          columns={[
            {
              header: "Stage",
              cell: (task) =>
                task.nodeCode || task.title || task.name || task.id,
              sortable: true,
            },
            {
              header: "Role",
              cell: (task) => task.assignedRole || "Team queue",
              sortable: true,
            },
            {
              header: "Assignee",
              cell: (task) => task.assignedTo || "Unassigned",
              sortable: true,
            },
            {
              header: "Status",
              cell: (task) => <StatusBadge status={task.status} />,
              sortable: true,
            },
            {
              header: "Action",
              cell: (task) => (
                <Button
                  variant="secondary"
                  onClick={() => navigate(`/app/tasks/${task.id}`)}
                >
                  Open
                </Button>
              ),
              filterable: false,
            },
          ]}
        />
      </AdminCard>
    </>
  );
}
