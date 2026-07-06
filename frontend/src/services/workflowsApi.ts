import { apiGet, apiPost } from "./apiClient";
import type { WorkflowDefinition } from "../types/api";
export const getWorkflows = () => apiGet<WorkflowDefinition[]>("/api/workflows", []);
export const publishWorkflow = (c: string) => apiPost(`/api/workflows/${c}/publish`, { actor: "admin@lca.org.ls" }, null);
export const archiveWorkflowVersion = (c: string, v: number) => apiPost(`/api/workflows/${c}/versions/${v}/archive`, { actor: "admin@lca.org.ls" }, null);
export const createWorkflow = (b: unknown) => apiPost("/api/workflows", b, null);
export const saveWorkflowDesigner = (b: unknown) => apiPost("/api/workflows/designer", b, null);
