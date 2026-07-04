import { Card } from "../ui/Card";
import { StatusBadge } from "../ui/Badge";
import type { WorkflowTask } from "../../types/api";
export function TaskCard({ task }: { task: WorkflowTask }) {
  return (
    <Card>
      <strong>{task.title || task.name || task.id}</strong>
      <p className="muted">
        {task.entityType} · {task.assignedTo || "Unassigned"}
      </p>
      <StatusBadge status={task.status} />
    </Card>
  );
}
