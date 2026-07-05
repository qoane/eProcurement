import { apiGet, apiPost } from "./apiClient";
import type { ApprovalMatrix, BusinessProcessDefinition, ConfigurationStudio, DocumentRequirementSet } from "../types/api";
export const getConfigurationStudio = () => apiGet<ConfigurationStudio>("/api/configuration-studio", { businessProcesses: [], documentRequirementSets: [], approvalMatrices: [], workflowMappings: [] });
export const publishBusinessProcess = (code: string) => apiPost<BusinessProcessDefinition>(`/api/configuration-studio/business-processes/${code}/publish`, {}, {} as BusinessProcessDefinition);
export const createDocumentRequirementSet = (body: unknown) => apiPost<DocumentRequirementSet>("/api/configuration-studio/document-requirement-sets", body, {} as DocumentRequirementSet);
export const createApprovalMatrix = (body: unknown) => apiPost<ApprovalMatrix>("/api/configuration-studio/approval-matrices", body, {} as ApprovalMatrix);
