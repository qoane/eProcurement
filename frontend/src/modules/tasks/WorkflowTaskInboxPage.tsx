import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { DataTable } from "../../components/ui/DataTable";
import { StatusBadge } from "../../components/ui/Badge";
import { Tabs } from "../../components/ui/Tabs";
import { getTasks } from "../../services/tasksApi";
import type { WorkflowTask } from "../../types/api";
export function WorkflowTaskInboxPage() {
  const [t, setT] = useState<WorkflowTask[]>([]);
  useEffect(() => {
    void getTasks().then((r) => setT(r.data));
  }, []);
  return (
    <>
      <PageHeader
        title="Workflow task inbox"
        description="My tasks, team tasks and completed work from workflow APIs."
      />
      <Tabs items={["My tasks", "Team tasks", "Completed tasks"]} />
      <section className="panel">
        <DataTable
          rows={t}
          columns={[
            { header: "Task", cell: (r) => r.title || r.name || r.id },
            { header: "Entity", cell: (r) => r.entityType },
            { header: "Assignee", cell: (r) => r.assignedTo || "Unassigned" },
            {
              header: "Status",
              cell: (r) => <StatusBadge status={r.status} />,
            },
          ]}
        />
      </section>
    </>
  );
}
