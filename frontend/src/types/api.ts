export type WorkflowNode = {
  code: string;
  name: string;
  kind?: string;
  createsTask?: boolean;
  isStart?: boolean;
  isTerminal?: boolean;
};
export type WorkflowTransition = {
  fromNodeCode: string;
  actionCode: string;
  actionName: string;
  toNodeCode: string;
  requiredRuleCode?: string;
};
export type WorkflowVersion = {
  versionNumber?: number;
  status?: string;
  nodes?: WorkflowNode[];
  transitions?: WorkflowTransition[];
};
export type WorkflowDefinition = {
  code: string;
  name: string;
  entityType: string;
  versions?: WorkflowVersion[];
};
export type Supplier = {
  id?: string;
  referenceNumber: string;
  legalName?: string;
  tradingName?: string;
  status?: string;
  categories?: unknown[];
  documents?: unknown[];
  createdAt?: string;
};
export type WorkflowTask = {
  id: string;
  title?: string;
  name?: string;
  status?: string;
  assignedTo?: string;
  entityType?: string;
  entityId?: string;
  workflowInstanceId?: string;
  createdAt?: string;
  completedAt?: string;
};
export type AuditEvent = {
  id?: string;
  entityType?: string;
  entityId?: string;
  eventType?: string;
  actor?: string;
  createdAt?: string;
  occurredAt?: string;
  description?: string;
};
export type Rule = {
  code: string;
  name: string;
  appliesTo?: string;
  expression?: string;
  status?: string;
};
export type FormField = {
  code: string;
  label: string;
  fieldType: string;
  isRequired?: boolean;
  displayOrder?: number;
};
export type FormSection = {
  code: string;
  title: string;
  fields?: FormField[];
  displayOrder?: number;
};
export type FormDefinition = {
  code: string;
  name: string;
  entityType: string;
  versions?: { status?: string; sections?: FormSection[] }[];
};
