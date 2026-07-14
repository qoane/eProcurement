import { useEffect, useState } from "react";
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap-icons/font/bootstrap-icons.css";
import "overlayscrollbars/overlayscrollbars.css";
import "admin-lte/dist/css/adminlte.min.css";
import "simple-datatables/dist/style.css";
import "../theme/globals.css";
import { PublicLayout } from "../layout/PublicLayout";
import { AuthLayout } from "../layout/AuthLayout";
import { AppShell } from "../layout/AppShell";
import { LandingPage } from "../modules/public/LandingPage";
import { PublicAwardsPage, PublicCalendarPage, PublicHelpPage, PublicPortalHomePage, PublicRegisterPage } from "../modules/public/PublicPortalPages";
import { LoginPage } from "../modules/public/LoginPage";
import { LatestOpportunitiesWidgetPage } from "../modules/public/LatestOpportunitiesWidgetPage";
import {
  OpportunitiesPage,
  OpportunityDetailPage,
} from "../modules/public/PublicOpportunitiesPage";
import { DashboardPage } from "../modules/dashboard/DashboardPage";
import { SupplierListPage } from "../modules/suppliers/SupplierListPage";
import { SupplierDetailPage } from "../modules/suppliers/SupplierDetailPage";
import { SupplierRegistrationPage } from "../modules/suppliers/SupplierRegistrationPage";
import { SupplierVerificationPage } from "../modules/suppliers/SupplierVerificationPage";
import { WorkflowTaskInboxPage } from "../modules/tasks/WorkflowTaskInboxPage";
import { WorkflowTaskDetailPage } from "../modules/tasks/WorkflowTaskDetailPage";
import { AuditExplorerPage } from "../modules/audit/AuditExplorerPage";
import { WorkflowListPage } from "../modules/workflows/WorkflowListPage";
import { WorkflowDetailPage } from "../modules/workflows/WorkflowDetailPage";
import { WorkflowDesignerPage } from "../modules/workflows/WorkflowDesignerPage";
import { BusinessRulesPage } from "../modules/rules/BusinessRulesPage";
import { RuleDesignerPage } from "../modules/rules/RuleDesignerPage";
import { DynamicFormsPage } from "../modules/forms/DynamicFormsPage";
import { FormDesignerPage } from "../modules/forms/FormDesignerPage";
import { ConfigurationPage } from "../modules/configuration/ConfigurationPage";
import { DashboardsPage, ReportingPage } from "../modules/reporting/ReportingPage";
import {
  NewTenderPage,
  TenderClarificationsWorkspacePage,
  TenderDetailPage,
  TendersPage,
} from "../modules/tenders/TendersPage";
import {
  PlanningPage,
  NewPlanningPage,
  PlanningDetailPage,
  BudgetsPage,
  CostCentresPage,
  ProcurementCategoriesPage,
} from "../modules/planning/PlanningPages";
import {
  RequisitionListPage,
  NewRequisitionPage,
  RequisitionDetailPage,
} from "../modules/requisitions/RequisitionPages";
import { StudioModulePage, StudioPage } from "../modules/studio/StudioPage";
import { EmptyState } from "../components/ui/EmptyState";
import {
  BidSubmissionDetailPage,
  BidSubmissionListPage,
  NewBidSubmissionPage,
} from "../modules/bids/BidSubmissionPages";
import {
  BidOpeningDetailPage,
  BidOpeningListPage,
  NewBidOpeningPage,
} from "../modules/bid-opening/BidOpeningPages";
import {
  EvaluationDetailPage,
  EvaluationListPage,
  EvaluationTemplateDetailPage,
  EvaluationTemplateListPage,
  NewEvaluationPage,
  NewEvaluationTemplatePage,
} from "../modules/evaluation/EvaluationPages";
import {
  AwardDetailPage,
  AwardListPage,
  NewAwardPage,
} from "../modules/awards/AwardPages";
import {
  NewPurchaseOrderPage,
  PurchaseOrderDetailPage,
  PurchaseOrderListPage,
} from "../modules/purchase-orders/PurchaseOrderPages";
import { ProcurementCaseDetailPage, ProcurementCaseListPage } from "../modules/procurement-cases/ProcurementCasePages";
import {
  ContractDetailPage,
  ContractListPage,
  ContractMilestonesPage,
  ContractPerformancePage,
  NewContractPage,
} from "../modules/contracts/ContractPages";
import { PageHeader } from "../components/ui/PageHeader";
import { InvoiceDetailPage, InvoiceListPage, InvoiceMatchingDetailPage, InvoiceMatchingListPage, NewInvoicePage, NewPurchaseOrderReturnPage, PurchaseOrderReturnDetailPage, PurchaseOrderReturnListPage } from "../modules/invoices/InvoicePages";
import { ContractIntegrationPage, DocumentIntegrationPage, IntegrationDashboardPage, IntegrationLogsPage } from "../modules/integrations/IntegrationPages";
import { AuthProvider, useAuth } from "../auth/AuthContext";
import { DocumentDetailPage, DocumentRequirementsPage, DocumentRetentionPage, DocumentsPage } from "../modules/documents/DocumentsPages";
import {
  SecurityPage,
  RolesPage,
  UsersPage,
} from "../modules/security/SecurityPages";
import { SupplierBidDetailPage, SupplierBidsPage, SupplierClarificationsPage, SupplierDashboardPage, SupplierDocumentsPage, SupplierNotificationsPage, SupplierOpportunitiesPage, SupplierProfilePage } from "../modules/supplier-portal/SupplierPortalPages";
import {
  NotificationsPage,
  NotificationTemplatesPage,
  NotificationLogsPage,
  NotificationPreferencesPage,
  ProfilePage,
  SettingsPage,
  NotificationEventMappingsPage,
  CommunicationsPage,
  CommunicationDetailPage,
} from "../modules/notifications/NotificationPages";
import { ErrorBoundary } from "../components/ui/ErrorBoundary";
import { DataArchivePage, DataExportsPage, DataGovernanceDashboardPage, DataQualityPage, MigrationBatchDetailPage, MigrationPage, PoliciesPage, PrivacyPage, ProcessingLogsPage, RetentionPage } from "../modules/data-governance/DataGovernancePages";
import { OperationsBackupsPage, OperationsChecklistPage, OperationsDashboardPage, OperationsHealthPage, OperationsLogsPage, OperationsPerformancePage } from "../modules/operations/OperationsPages";
import { SupportCaseDetailPage, SupportCaseListPage } from "../modules/support/SupportPages";
import { RfpEvidenceDashboardPage, ComplianceMatrixPage, ProposalCommitmentsPage, CoveragePage, EvidenceExportsPage, ArchitectureEvidencePage, IntegrationEvidencePage } from "../modules/rfp-evidence/RfpEvidencePages";
import { DemoControlCenterPage, DemoScriptPage, UatPage, TrainingPage, ImplementationPage, SupportMaintenancePage, HandoverPage, ReadinessPage } from "../modules/rfp-evidence/ReadinessAndHandoverPages";

function NotConfiguredPage({ title }: { title: string }) {
  return (
    <>
      <PageHeader
        title={title}
        description="This procurement module is ready to be connected when the capability is configured."
      />
      <EmptyState
        title="Not configured yet"
        message={`${title} will appear here after the module is enabled and connected to platform APIs.`}
      />
    </>
  );
}

function legacyPublicRedirect(path: string) {
  if (path === "/opportunities") return "/public/opportunities";
  if (path.startsWith("/opportunities/")) return `/public${path}`;
  if (path === "/supplier/register") return "/public/register";
  return null;
}

function AccessDeniedPage() { return <EmptyState title="Access denied" message="You are signed in, but your account does not have permission to view this page." />; }
function ProtectedShell({ path, children }: { path: string; children: React.ReactNode }) {
  const { currentUser, hasPermission } = useAuth();
  if (!currentUser) { history.replaceState(null, "", `/login`); queueMicrotask(() => dispatchEvent(new PopStateEvent("popstate"))); return null; }
  const rules: [RegExp, string][] = [
    [/^\/app\/security/, "Security.View"], [/^\/app\/users/, "Security.Users"], [/^\/app\/roles/, "Security.Roles"],
    [/^\/app\/audit/, "Audit.View"], [/^\/app\/notifications/, "Notifications.View"], [/^\/app\/notification-logs/, "NotificationLogs.View"],
    [/^\/app\/notification-templates/, "NotificationTemplates.Manage"], [/^\/app\/integrations/, "Integrations.View"],
    [/^\/app\/suppliers/, "Supplier.View"], [/^\/app\/planning/, "Planning.View"], [/^\/app\/budgets/, "Budget.View"],
    [/^\/app\/requisitions/, "Requisition.View"], [/^\/app\/tenders/, "Tender.View"], [/^\/app\/bids/, "Bid.View"],
    [/^\/app\/bid-opening/, "BidOpening.View"], [/^\/app\/evaluation/, "Evaluation.View"], [/^\/app\/awards/, "Award.View"],
    [/^\/app\/purchase-orders/, "PurchaseOrder.View"], [/^\/app\/invoices/, "Invoice.View"], [/^\/app\/invoice-matching/, "InvoiceMatching.View"], [/^\/app\/purchase-order-returns/, "PurchaseOrderReturn.View"], [/^\/app\/contracts/, "Contract.View"], [/^\/app\/workflows|^\/app\/tasks/, "Workflow.View"],
    [/^\/app\/rules/, "Studio.Rules"], [/^\/app\/forms/, "Studio.Forms"], [/^\/app\/studio/, "Studio.View"], [/^\/app\/configuration/, "Settings.View"],
    [/^\/app\/reporting|^\/app\/dashboards/, "Reporting.View"], [/^\/app\/documents|^\/app\/document-retention|^\/app\/document-requirements/, "Document.View"],
    [/^\/app\/operations/, "Operations.View"], [/^\/app\/rfp-evidence/, "RfpEvidence.View"], [/^\/app\/demo/, "Demo.View"], [/^\/app\/uat/, "Uat.View"], [/^\/app\/training/, "Training.View"], [/^\/app\/implementation/, "Implementation.View"], [/^\/app\/handover/, "Handover.View"], [/^\/app\/readiness/, "Readiness.View"], [/^\/app\/support-maintenance/, "SupportMaintenance.View"], [/^\/app\/support/, "SupportCase.View"],
    [/^\/app\/data-governance/, "DataGovernance.View"], [/^\/app\/migration/, "Migration.View"], [/^\/app\/data-quality/, "DataQuality.View"], [/^\/app\/data-archive/, "DataArchive.View"], [/^\/app\/data-exports/, "DataExport.View"]
  ];
  const required = rules.find(([regex]) => regex.test(path))?.[1];
  if (required && !hasPermission(required)) return <AppShell><AccessDeniedPage /></AppShell>;
  return <AppShell>{children}</AppShell>;
}

function route(p: string) {
  const redirectTo = legacyPublicRedirect(p);
  if (redirectTo) {
    history.replaceState(null, "", redirectTo);
    queueMicrotask(() => dispatchEvent(new PopStateEvent("popstate")));
    return null;
  }
  if (p === "/")
    return (
      <PublicLayout>
        <LandingPage />
      </PublicLayout>
    );
  if (p === "/login")
    return (
      <AuthLayout>
        <LoginPage />
      </AuthLayout>
    );
  if (p === "/widgets/latest-opportunities")
    return <LatestOpportunitiesWidgetPage />;
  if (p === "/public")
    return (
      <PublicLayout>
        <PublicPortalHomePage />
      </PublicLayout>
    );
  if (p === "/public/opportunities")
    return (
      <PublicLayout>
        <OpportunitiesPage />
      </PublicLayout>
    );
  if (p.startsWith("/public/opportunities/"))
    return (
      <PublicLayout>
        <OpportunityDetailPage
          reference={decodeURIComponent(p.split("/").pop() || "")}
        />
      </PublicLayout>
    );
  if (p === "/public/calendar") return <PublicLayout><PublicCalendarPage /></PublicLayout>;
  if (p === "/public/awards") return <PublicLayout><PublicAwardsPage /></PublicLayout>;
  if (p === "/public/register") return <PublicLayout><PublicRegisterPage /></PublicLayout>;
  if (p === "/public/help") return <PublicLayout><PublicHelpPage /></PublicLayout>;
  let page: React.ReactNode = <DashboardPage />;
  if (p === "/app/supplier/dashboard") page = <SupplierDashboardPage />;
  else if (p === "/app/supplier/profile") page = <SupplierProfilePage />;
  else if (p === "/app/supplier/documents") page = <SupplierDocumentsPage />;
  else if (p === "/app/supplier/opportunities") page = <SupplierOpportunitiesPage />;
  else if (p === "/app/supplier/bids") page = <SupplierBidsPage />;
  else if (p.startsWith("/app/supplier/bids/")) page = <SupplierBidDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/supplier/clarifications") page = <SupplierClarificationsPage />;
  else if (p === "/app/supplier/notifications") page = <SupplierNotificationsPage />;
  else if (p === "/app/supplier/invoices") page = <InvoiceListPage />;
  else if (p === "/app/supplier/invoices/new") page = <NewInvoicePage />;
  else if (p.startsWith("/app/supplier/invoices/")) page = <InvoiceDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app" || p === "/app/dashboard") page = <DashboardPage />;
  else if (p === "/app/operations") page = <OperationsDashboardPage />;
  else if (p === "/app/operations/health") page = <OperationsHealthPage />;
  else if (p === "/app/operations/performance") page = <OperationsPerformancePage />;
  else if (p === "/app/operations/logs") page = <OperationsLogsPage />;
  else if (p === "/app/operations/backups") page = <OperationsBackupsPage />;
  else if (p === "/app/operations/checklist") page = <OperationsChecklistPage />;
  else if (p === "/app/support") page = <SupportCaseListPage />;
  else if (p.startsWith("/app/support/")) page = <SupportCaseDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/suppliers") page = <SupplierListPage />;
  else if (p === "/app/suppliers/register") page = <SupplierRegistrationPage />;
  else if (p === "/app/suppliers/verification")
    page = <SupplierVerificationPage />;
  else if (p.startsWith("/app/suppliers/"))
    page = (
      <SupplierDetailPage
        reference={decodeURIComponent(p.split("/").pop() || "")}
      />
    );
  else if (p === "/app/tasks") page = <WorkflowTaskInboxPage />;
  else if (p.startsWith("/app/tasks/"))
    page = (
      <WorkflowTaskDetailPage
        id={decodeURIComponent(p.split("/").pop() || "")}
      />
    );
  else if (p === "/app/audit") page = <AuditExplorerPage />;
  else if (p === "/app/workflows") page = <WorkflowListPage />;
  else if (p === "/app/workflows/designer") page = <WorkflowDesignerPage />;
  else if (p.startsWith("/app/workflows/"))
    page = (
      <WorkflowDetailPage code={decodeURIComponent(p.split("/").pop() || "")} />
    );
  else if (p === "/app/rules") page = <BusinessRulesPage />;
  else if (p === "/app/rules/designer") page = <RuleDesignerPage />;
  else if (p === "/app/forms") page = <DynamicFormsPage />;
  else if (p === "/app/forms/designer") page = <FormDesignerPage />;
  else if (p === "/app/studio") page = <StudioPage />;
  else if (p.startsWith("/app/studio/"))
    page = (
      <StudioModulePage slug={decodeURIComponent(p.split("/").pop() || "")} />
    );
  else if (p === "/app/configuration") page = <ConfigurationPage />;
  else if (p === "/app/reporting") page = <ReportingPage />;
  else if (p === "/app/dashboards") page = <DashboardsPage />;
  else if (p.startsWith("/app/reporting/")) page = <ReportingPage reportCode={decodeURIComponent(p.split("/").pop() || "executive-dashboard")} />;

  else if (p === "/app/rfp-evidence") page = <RfpEvidenceDashboardPage />;
  else if (p === "/app/rfp-evidence/compliance-matrix") page = <ComplianceMatrixPage />;
  else if (p === "/app/rfp-evidence/proposal-commitments") page = <ProposalCommitmentsPage />;
  else if (p === "/app/rfp-evidence/coverage") page = <CoveragePage />;
  else if (p === "/app/rfp-evidence/exports") page = <EvidenceExportsPage />;
  else if (p === "/app/rfp-evidence/architecture") page = <ArchitectureEvidencePage />;
  else if (p === "/app/rfp-evidence/integrations") page = <IntegrationEvidencePage />;
  else if (p === "/app/demo") page = <DemoControlCenterPage />;
  else if (p === "/app/demo/script") page = <DemoScriptPage />;
  else if (p === "/app/uat" || p === "/app/uat/suites" || p.startsWith("/app/uat/suites/") || p.startsWith("/app/uat/runs")) page = <UatPage />;
  else if (p === "/app/training") page = <TrainingPage />;
  else if (p.startsWith("/app/training/")) page = <TrainingPage moduleCode={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/implementation") page = <ImplementationPage />;
  else if (p === "/app/support-maintenance") page = <SupportMaintenancePage />;
  else if (p === "/app/handover") page = <HandoverPage />;
  else if (p === "/app/readiness") page = <ReadinessPage />;
  else if (p === "/app/settings") page = <SettingsPage />;
  else if (p === "/app/data-governance") page = <DataGovernanceDashboardPage />;
  else if (p === "/app/data-governance/policies") page = <PoliciesPage />;
  else if (p === "/app/data-governance/retention") page = <RetentionPage />;
  else if (p === "/app/data-governance/privacy") page = <PrivacyPage />;
  else if (p === "/app/data-governance/processing-logs") page = <ProcessingLogsPage />;
  else if (p === "/app/migration" || p === "/app/migration/templates" || p === "/app/migration/batches") page = <MigrationPage />;
  else if (p.startsWith("/app/migration/batches/")) page = <MigrationBatchDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/data-quality") page = <DataQualityPage />;
  else if (p === "/app/data-archive") page = <DataArchivePage />;
  else if (p === "/app/data-exports") page = <DataExportsPage />;
  else if (p === "/app/documents") page = <DocumentsPage />;
  else if (p.startsWith("/app/documents/")) page = <DocumentDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/document-retention") page = <DocumentRetentionPage />;
  else if (p === "/app/document-requirements") page = <DocumentRequirementsPage />;
  else if (p === "/app/notifications") page = <NotificationsPage />;
  else if (p === "/app/notification-templates")
    page = <NotificationTemplatesPage />;
  else if (p === "/app/notification-logs") page = <NotificationLogsPage />;
  else if (p === "/app/notification-preferences")
    page = <NotificationPreferencesPage />;
  else if (p === "/app/notification-event-mappings") page = <NotificationEventMappingsPage />;
  else if (p === "/app/communications") page = <CommunicationsPage />;
  else if (p.startsWith("/app/communications/")) page = <CommunicationDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/profile") page = <ProfilePage />;
  else if (p === "/app/planning") page = <PlanningPage />;
  else if (p === "/app/planning/new") page = <NewPlanningPage />;
  else if (p.startsWith("/app/planning/"))
    page = (
      <PlanningDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />
    );
  else if (p === "/app/budgets") page = <BudgetsPage />;
  else if (p === "/app/cost-centres") page = <CostCentresPage />;
  else if (p === "/app/procurement-categories")
    page = <ProcurementCategoriesPage />;
  else if (p === "/app/requisitions") page = <RequisitionListPage />;
  else if (p === "/app/requisitions/new") page = <NewRequisitionPage />;
  else if (p.startsWith("/app/requisitions/"))
    page = (
      <RequisitionDetailPage
        id={decodeURIComponent(p.split("/").pop() || "")}
      />
    );
  else if (p === "/app/procurement-cases") page = <ProcurementCaseListPage />;
  else if (p.startsWith("/app/procurement-cases/")) page = <ProcurementCaseDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/tenders") page = <TendersPage />;
  else if (p === "/app/tenders/new") page = <NewTenderPage />;
  else if (p === "/app/tenders/clarifications")
    page = <TenderClarificationsWorkspacePage />;
  else if (p.match(/^\/app\/tenders\/[^/]+\/clarifications$/))
    page = <TenderDetailPage id={decodeURIComponent(p.split("/")[3] || "")} />;
  else if (p.startsWith("/app/tenders/"))
    page = (
      <TenderDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />
    );
  else if (p === "/app/bids") page = <BidSubmissionListPage />;
  else if (p === "/app/bids/new") page = <NewBidSubmissionPage />;
  else if (p.startsWith("/app/bids/"))
    page = (
      <BidSubmissionDetailPage
        id={decodeURIComponent(p.split("/").pop() || "")}
      />
    );
  else if (p === "/app/bid-opening") page = <BidOpeningListPage />;
  else if (p === "/app/bid-opening/new") page = <NewBidOpeningPage />;
  else if (p.startsWith("/app/bid-opening/"))
    page = (
      <BidOpeningDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />
    );
  else if (p === "/app/evaluation") page = <EvaluationListPage />;
  else if (p === "/app/evaluation/new") page = <NewEvaluationPage />;
  else if (p === "/app/evaluation/templates")
    page = <EvaluationTemplateListPage />;
  else if (p === "/app/evaluation/templates/new")
    page = <NewEvaluationTemplatePage />;
  else if (p.startsWith("/app/evaluation/templates/"))
    page = (
      <EvaluationTemplateDetailPage
        id={decodeURIComponent(p.split("/").pop() || "")}
      />
    );
  else if (p.startsWith("/app/evaluation/"))
    page = (
      <EvaluationDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />
    );
  else if (p === "/app/awards") page = <AwardListPage />;
  else if (p === "/app/awards/new") page = <NewAwardPage />;
  else if (p.startsWith("/app/awards/"))
    page = (
      <AwardDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />
    );
  else if (p === "/app/purchase-orders") page = <PurchaseOrderListPage />;
  else if (p === "/app/purchase-orders/new") page = <NewPurchaseOrderPage />;
  else if (p.startsWith("/app/purchase-orders/"))
    page = (
      <PurchaseOrderDetailPage
        id={decodeURIComponent(p.split("/").pop() || "")}
      />
    );
  else if (p === "/app/invoices") page = <InvoiceListPage />;
  else if (p === "/app/invoices/new") page = <NewInvoicePage />;
  else if (p.startsWith("/app/invoices/")) page = <InvoiceDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/invoice-matching") page = <InvoiceMatchingListPage />;
  else if (p.startsWith("/app/invoice-matching/")) page = <InvoiceMatchingDetailPage invoiceId={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/purchase-order-returns") page = <PurchaseOrderReturnListPage />;
  else if (p === "/app/purchase-order-returns/new") page = <NewPurchaseOrderReturnPage />;
  else if (p.startsWith("/app/purchase-order-returns/")) page = <PurchaseOrderReturnDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/contracts") page = <ContractListPage />;
  else if (p === "/app/contracts/new") page = <NewContractPage />;
  else if (p.endsWith("/milestones") && p.startsWith("/app/contracts/"))
    page = (
      <ContractMilestonesPage
        id={decodeURIComponent(p.split("/")[p.split("/").length - 2] || "")}
      />
    );
  else if (p.endsWith("/performance") && p.startsWith("/app/contracts/"))
    page = (
      <ContractPerformancePage
        id={decodeURIComponent(p.split("/")[p.split("/").length - 2] || "")}
      />
    );
  else if (p.startsWith("/app/contracts/"))
    page = (
      <ContractDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />
    );
  else if (p === "/app/security") page = <SecurityPage />;
  else if (p === "/app/users") page = <UsersPage />;
  else if (p === "/app/roles") page = <RolesPage />;
  else if (p === "/app/integrations") page = <IntegrationDashboardPage />;
  else if (p === "/app/integrations/contracts") page = <ContractIntegrationPage />;
  else if (p === "/app/integrations/document-management") page = <DocumentIntegrationPage />;
  else if (p === "/app/integrations/logs") page = <IntegrationLogsPage />;
  return <ProtectedShell path={p}>{page}</ProtectedShell>;
}
export function App() {
  const [p, setP] = useState(location.pathname);
  useEffect(() => {
    const h = () => setP(location.pathname);
    addEventListener("popstate", h);
    return () => removeEventListener("popstate", h);
  }, []);
  return <AuthProvider>{route(p)}</AuthProvider>;
}
