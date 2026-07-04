import { apiGet, apiPost } from "./apiClient";
import type { WorkflowTask } from "../types/api";
export const getTasks = () => apiGet<WorkflowTask[]>("/api/workflow-tasks", []);
export const completeTask = (id: string) =>
  apiPost(
    `/api/workflow-tasks/${id}/complete`,
    { actor: "admin@lca.org.ls" },
    null,
  );
