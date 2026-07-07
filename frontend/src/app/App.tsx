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
import { TendersPage } from "../modules/tenders/TendersPage";
import { PlanningPage, NewPlanningPage, PlanningDetailPage, BudgetsPage, CostCentresPage, ProcurementCategoriesPage } from "../modules/planning/PlanningPages";
import { StudioModulePage, StudioPage } from "../modules/studio/StudioPage";
import { EmptyState } from "../components/ui/EmptyState";
import { PageHeader } from "../components/ui/PageHeader";
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
  if (p === "/app/suppliers") page = <SupplierListPage />;
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
  else if (p === "/app/settings") page = <ConfigurationPage />;
  else if (p === "/app/planning") page = <PlanningPage />;
  else if (p === "/app/planning/new") page = <NewPlanningPage />;
  else if (p.startsWith("/app/planning/")) page = <PlanningDetailPage id={decodeURIComponent(p.split("/").pop() || "")} />;
  else if (p === "/app/budgets") page = <BudgetsPage />;
  else if (p === "/app/cost-centres") page = <CostCentresPage />;
  else if (p === "/app/procurement-categories") page = <ProcurementCategoriesPage />;
  else if (p === "/app/requisitions")
    page = <NotConfiguredPage title="Requisitions" />;
  else if (p === "/app/tenders") page = <TendersPage />;
  else if (p === "/app/evaluation")
    page = <NotConfiguredPage title="Evaluation" />;
  else if (p === "/app/awards") page = <NotConfiguredPage title="Awards" />;
  else if (p === "/app/purchase-orders")
    page = <NotConfiguredPage title="Purchase Orders" />;
  else if (p === "/app/contracts")
    page = <NotConfiguredPage title="Contracts" />;
  return <AppShell>{page}</AppShell>;
}
export function App() {
  const [p, setP] = useState(location.pathname);
  useEffect(() => {
    const h = () => setP(location.pathname);
    addEventListener("popstate", h);
    return () => removeEventListener("popstate", h);
  }, []);
  return route(p);
}
