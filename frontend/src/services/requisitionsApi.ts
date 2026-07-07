import { apiGet, apiPost } from "./apiClient";
export type RequisitionItem = { id:string; description:string; quantity:number; unitOfMeasure:string; estimatedUnitPrice:number; estimatedTotal:number; procurementCategoryId:string; procurementPlanItemId?:string };
export type Requisition = { id:string; requisitionNumber:string; title:string; description:string; department:string; costCentreId:string; financialYearId:string; requestedBy:string; requiredDate:string; priority:string; estimatedTotal:number; status:string; createdAt:string; submittedAt?:string; approvedAt?:string; rejectedAt?:string; items:RequisitionItem[] };
export type BudgetValidation = { budgetExists:boolean; hasItems:boolean; itemsHaveEstimates:boolean; budgetAvailable:boolean; requiredAmount:number; availableAmount:number; messages:string[] };
export const requisitionsApi = {
  list: () => apiGet<Requisition[]>("/api/requisitions", []),
  get: (id:string) => apiGet<Requisition | null>(`/api/requisitions/${id}`, null),
  create: (body: unknown) => apiPost<Requisition>("/api/requisitions", body),
  submit: (id:string) => apiPost<Requisition>(`/api/requisitions/${id}/submit`, { actor: "requester@lca.org.ls" }),
  approve: (id:string) => apiPost<Requisition>(`/api/requisitions/${id}/approve`, { actor: "approver@lca.org.ls" }),
  reject: (id:string, reason = "Rejected") => apiPost<Requisition>(`/api/requisitions/${id}/reject`, { actor: "approver@lca.org.ls", reason }),
  budgetValidation: (id:string) => apiGet<BudgetValidation>(`/api/requisitions/${id}/budget-validation`, { budgetExists:false,hasItems:false,itemsHaveEstimates:false,budgetAvailable:false,requiredAmount:0,availableAmount:0,messages:[] }),
};
