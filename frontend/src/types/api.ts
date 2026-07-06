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
  nodeCode?: string;
  assignedRole?: string;
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
export type BusinessProcessDefinition = {
  code: string;
  name: string;
  description: string;
  entityType: string;
  status: string;
  activeWorkflowDefinitionId?: string;
  activeFormDefinitionId?: string;
  activeDocumentRequirementSetId?: string;
  activeApprovalMatrixId?: string;
};
export type DocumentRequirement = {
  documentType: string;
  required: boolean;
  minimumFiles: number;
  maximumFiles: number;
  allowedExtensions: string;
  maximumFileSize: number;
  ruleCode?: string;
};
export type DocumentRequirementSet = {
  id?: string;
  name: string;
  description: string;
  entityType: string;
  requirements?: DocumentRequirement[];
};
export type ApprovalStep = {
  role: string;
  sequence: number;
  minimumAmount?: number;
  maximumAmount?: number;
  ruleCode?: string;
};
export type ApprovalMatrix = {
  id?: string;
  name: string;
  description: string;
  entityType: string;
  steps?: ApprovalStep[];
};
export type ConfigurationStudio = {
  businessProcesses: BusinessProcessDefinition[];
  documentRequirementSets: DocumentRequirementSet[];
  approvalMatrices: ApprovalMatrix[];
  workflowMappings: unknown[];
};

export type ApplicationDesigner = {
  id?: string;
  code: string;
  name: string;
  icon: string;
  theme: string;
  description: string;
  defaultLandingPage: string;
  navigationRoot: string;
  modules: string[];
  status?: string;
  version?: number;
};

export type EntityProperty = {
  code: string;
  name: string;
  dataType: string;
  required?: boolean;
  searchable?: boolean;
  defaultValue?: string;
};
export type EntityRelationship = {
  code: string;
  name: string;
  targetEntity: string;
  cardinality: string;
  required?: boolean;
};
export type EntityValidation = {
  code: string;
  name: string;
  expression: string;
  message: string;
};
export type EntityDesigner = {
  id?: string;
  code: string;
  name: string;
  description: string;
  displayName: string;
  pluralName: string;
  defaultSearchField: string;
  properties: EntityProperty[];
  relationships: EntityRelationship[];
  validations: EntityValidation[];
  status?: string;
  version?: number;
};

export type PageType =
  | "Dashboard"
  | "DataGrid"
  | "DetailPage"
  | "Wizard"
  | "Form"
  | "SplitView"
  | "Timeline"
  | "Kanban"
  | "Calendar"
  | "Report";
export type PageDatasource = {
  entity: string;
  mode?: string;
  endpoint?: string;
  keyField?: string;
};
export type PageLayout = {
  template: string;
  columns?: number;
  density?: string;
  regions?: string[];
};
export type PageToolbarItem = {
  code: string;
  label: string;
  kind?: string;
  icon?: string;
  actionCode?: string;
};
export type PageAction = {
  code: string;
  label: string;
  kind?: string;
  target?: string;
  confirmation?: string;
};
export type PageFilter = {
  code: string;
  label: string;
  field: string;
  operator?: string;
  defaultValue?: string;
};
export type PageColumn = {
  code: string;
  label: string;
  field: string;
  displayOrder?: number;
  sortable?: boolean;
  searchable?: boolean;
};
export type PageComponent = {
  code: string;
  name: string;
  componentType: string;
  region?: string;
  displayOrder?: number;
  configuration?: unknown;
};
export type PagePermission = { role: string; access?: string };
export type PageNavigation = {
  route: string;
  parentRoute?: string;
  menuGroup?: string;
  showInNavigation?: boolean;
};
export type PageDesigner = {
  id?: string;
  code: string;
  name: string;
  description: string;
  pageType: PageType;
  datasource: PageDatasource;
  layout: PageLayout;
  toolbar: PageToolbarItem[];
  actions: PageAction[];
  filters: PageFilter[];
  columns: PageColumn[];
  components: PageComponent[];
  permissions: PagePermission[];
  navigation: PageNavigation;
  status?: string;
  version?: number;
};
