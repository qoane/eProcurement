import { apiGet, apiPost } from "./apiClient";
import type { Rule, RuleDesignerMetadata, RuleExecutionLog, RuleSimulationResult, RuleValidationResult } from "../types/api";
export const getRules = () => apiGet<Rule[]>("/api/business-rules", []);
export const getRuleDesignerMetadata = () => apiGet<RuleDesignerMetadata>("/api/business-rules/designer-metadata", { categories: [], fields: [], functions: [], operators: [] });
export const getRuleHistory = (ruleCode?: string) => apiGet<RuleExecutionLog[]>(`/api/business-rules/history${ruleCode ? `?ruleCode=${encodeURIComponent(ruleCode)}` : ""}`, []);
export const createRule = (b: unknown) => apiPost<Rule>("/api/business-rules", b, {} as Rule);
export const validateRule = (b: unknown) => apiPost<RuleValidationResult>("/api/business-rules/validate", b, { isValid: false, errors: [], warnings: [] });
export const simulateRule = (b: unknown) => apiPost<RuleSimulationResult>("/api/business-rules/simulate", b, { passed: false, validation: { isValid: false, errors: [], warnings: [] }, trace: {} });
export const publishRule = (code: string, actor = "admin") => apiPost<Rule>(`/api/business-rules/${code}/publish`, { actor }, {} as Rule);
