import { apiGet, apiPost } from "./apiClient";
import type { WorkflowTask } from "../types/api";
export const getTasks = () => apiGet<WorkflowTask[]>("/api/workflow-tasks", []);
export const getTaskDetail = (id: string) => apiGet(`/api/workflow-tasks/${id}`, null);
export const executeTaskAction = (id: string, actionCode: string) =>
  apiPost(`/api/workflow-tasks/${id}/actions`, { actionCode, actor: "admin@lca.org.ls" }, null);
export const completeTask = (id: string) =>
  apiPost(`/api/workflow-tasks/${id}/complete`, { actor: "admin@lca.org.ls" }, null);
