import { apiGet, apiPost } from "./apiClient";
export type FinancialYear = { id:string; code:string; startDate:string; endDate:string; isActive:boolean };
export type PlanItem = { id:string; itemCode:string; description:string; procurementCategoryId:string; estimatedAmount:number; plannedQuarter:string; procurementMethod:string; status:string };
export type Plan = { id:string; planNumber:string; title:string; financialYearId:string; department:string; status:string; createdBy:string; createdAt:string; submittedAt?:string; approvedAt?:string; items:PlanItem[] };
export type BudgetLine = { id:string; costCentreId:string; procurementCategoryId:string; allocatedAmount:number; committedAmount:number; availableAmount:number };
export type Budget = { id:string; financialYearId:string; department:string; totalAmount:number; committedAmount:number; availableAmount:number; lines:BudgetLine[] };
export type CostCentre = { id:string; code:string; name:string; department:string; isActive:boolean };
export type ProcurementCategory = { id:string; code:string; name:string; isActive:boolean };
export type PlanningDashboard = { plans:number; draftPlans:number; submittedPlans:number; approvedPlans:number; totalPlannedValue:number; totalBudgetValue:number; availableBudget:number };
export const planningApi = {
  dashboard: () => apiGet<PlanningDashboard>("/api/procurement-plans/dashboard", { plans:0,draftPlans:0,submittedPlans:0,approvedPlans:0,totalPlannedValue:0,totalBudgetValue:0,availableBudget:0 }),
  financialYears: () => apiGet<FinancialYear[]>("/api/financial-years", []),
  plans: () => apiGet<Plan[]>("/api/procurement-plans", []),
  plan: (id:string) => apiGet<Plan | null>(`/api/procurement-plans/${id}`, null),
  createPlan: (body: unknown) => apiPost<Plan>("/api/procurement-plans", body),
  submitPlan: (id:string) => apiPost<Plan>(`/api/procurement-plans/${id}/submit`, { actor: "procurement@lca.org.ls" }),
  approvePlan: (id:string) => apiPost<Plan>(`/api/procurement-plans/${id}/approve`, { actor: "approver@lca.org.ls" }),
  budgets: () => apiGet<Budget[]>("/api/budgets", []),
  createBudget: (body: unknown) => apiPost<Budget>("/api/budgets", body),
  costCentres: () => apiGet<CostCentre[]>("/api/cost-centres", []),
  createCostCentre: (body: unknown) => apiPost<CostCentre>("/api/cost-centres", body),
  categories: () => apiGet<ProcurementCategory[]>("/api/procurement-categories", []),
  createCategory: (body: unknown) => apiPost<ProcurementCategory>("/api/procurement-categories", body),
};
