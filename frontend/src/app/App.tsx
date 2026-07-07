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
import { LoginPage } from "../modules/public/LoginPage";
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
import { ReportingPage } from "../modules/reporting/ReportingPage";
import {
  NewTenderPage,
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
import { BidSubmissionDetailPage, BidSubmissionListPage, NewBidSubmissionPage } from "../modules/bids/BidSubmissionPages";
import { BidOpeningDetailPage, BidOpeningListPage, NewBidOpeningPage } from "../modules/bid-opening/BidOpeningPages";
import { EvaluationDetailPage, EvaluationListPage, EvaluationTemplateDetailPage, EvaluationTemplateListPage, NewEvaluationPage, NewEvaluationTemplatePage } from "../modules/evaluation/EvaluationPages";
import { AwardDetailPage, AwardListPage, NewAwardPage } from "../modules/awards/AwardPages";
import { NewPurchaseOrderPage, PurchaseOrderDetailPage, PurchaseOrderListPage } from "../modules/purchase-orders/PurchaseOrderPages";
import { ContractDetailPage, ContractListPage, ContractMilestonesPage, ContractPerformancePage, NewContractPage } from "../modules/contracts/ContractPages";
import { PageHeader } from "../components/ui/PageHeader";
import { AuthProvider } from "../auth/AuthContext";
import { SecurityPage, RolesPage, UsersPage } from "../modules/security/SecurityPages";
import { NotificationsPage, NotificationTemplatesPage, NotificationLogsPage, NotificationPreferencesPage, ProfilePage, SettingsPage } from "../modules/notifications/NotificationPages";
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

function route(p: string) {
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
  let page: React.ReactNode = <DashboardPage />;
  if (p === "/app" || p === "/app/dashboard") page = <DashboardPage />;
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
  else if (p === "/app/dashboards")
    page = <NotConfiguredPage title="Dashboards" />;
  else if (p === "/app/settings") page = <SettingsPage />;
  else if (p === "/app/notifications") page = <NotificationsPage />;
  else if (p === "/app/notification-templates") page = <NotificationTemplatesPage />;
  else if (p === "/app/notification-logs") page = <NotificationLogsPage />;
  else if (p === "/app/notification-preferences") page = <NotificationPreferencesPage />;
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
  else if (p === "/app/tenders") page = <TendersPage />;
  else if (p === "/app/tenders/new") page = <NewTenderPage />;
  else if (p === "/app/tenders/clarifications")
    page = <NotConfiguredPage title="Clarifications" />;
  else if (p.startsWith("/app/tenders/"))
    page = (
      <TenderDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />
    );
  else if (p === "/app/bids") page = <BidSubmissionListPage />;
  else if (p === "/app/bids/new") page = <NewBidSubmissionPage />;
  else if (p.startsWith("/app/bids/"))
    page = <BidSubmissionDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/bid-opening") page = <BidOpeningListPage />;
  else if (p === "/app/bid-opening/new") page = <NewBidOpeningPage />;
  else if (p.startsWith("/app/bid-opening/"))
    page = <BidOpeningDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/evaluation") page = <EvaluationListPage />;
  else if (p === "/app/evaluation/new") page = <NewEvaluationPage />;
  else if (p === "/app/evaluation/templates") page = <EvaluationTemplateListPage />;
  else if (p === "/app/evaluation/templates/new") page = <NewEvaluationTemplatePage />;
  else if (p.startsWith("/app/evaluation/templates/")) page = <EvaluationTemplateDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p.startsWith("/app/evaluation/")) page = <EvaluationDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/awards") page = <AwardListPage />;
  else if (p === "/app/awards/new") page = <NewAwardPage />;
  else if (p.startsWith("/app/awards/")) page = <AwardDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/purchase-orders") page = <PurchaseOrderListPage />;
  else if (p === "/app/purchase-orders/new") page = <NewPurchaseOrderPage />;
  else if (p.startsWith("/app/purchase-orders/")) page = <PurchaseOrderDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/contracts") page = <ContractListPage />;
  else if (p === "/app/contracts/new") page = <NewContractPage />;
  else if (p.endsWith("/milestones") && p.startsWith("/app/contracts/")) page = <ContractMilestonesPage id={decodeURIComponent(p.split("/")[p.split("/").length - 2] || "")} />;
  else if (p.endsWith("/performance") && p.startsWith("/app/contracts/")) page = <ContractPerformancePage id={decodeURIComponent(p.split("/")[p.split("/").length - 2] || "")} />;
  else if (p.startsWith("/app/contracts/")) page = <ContractDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/security") page = <SecurityPage />;
  else if (p === "/app/users") page = <UsersPage />;
  else if (p === "/app/roles") page = <RolesPage />;
  else if (p === "/app/integrations")
    page = <NotConfiguredPage title="Integrations" />;
  return <AppShell>{page}</AppShell>;
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
