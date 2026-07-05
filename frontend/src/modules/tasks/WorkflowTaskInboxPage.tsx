import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { DataTable } from "../../components/ui/DataTable";
import { StatusBadge } from "../../components/ui/Badge";
import { Tabs } from "../../components/ui/Tabs";
import { Button } from "../../components/ui/Button";
import { navigate } from "../../app/routes";
import { getTasks } from "../../services/tasksApi";
import type { WorkflowTask } from "../../types/api";
export function WorkflowTaskInboxPage() {
  const [t, setT] = useState<WorkflowTask[]>([]);
  useEffect(() => { void getTasks().then((r) => setT(r.data)); }, []);
  return (
    <>
      <PageHeader title="Workflow task inbox" description="Supplier verification and approval tasks loaded from the workflow API." />
      <Tabs items={["My tasks", "Team tasks", "Completed tasks"]} />
      <section className="panel">
        <DataTable rows={t} columns={[
          { header: "Stage", cell: (r) => r.nodeCode || r.title || r.name || r.id },
          { header: "Role", cell: (r) => r.assignedRole || "Team queue" },
          { header: "Assignee", cell: (r) => r.assignedTo || "Unassigned" },
          { header: "Status", cell: (r) => <StatusBadge status={r.status} /> },
          { header: "Action", cell: (r) => <Button variant="secondary" onClick={() => navigate(`/app/tasks/${r.id}`)}>Open</Button> },
        ]} />
      </section>
    </>
  );
}
