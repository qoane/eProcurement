import { Sidebar } from "./Sidebar";
import { TopBar } from "./TopBar";
import { Breadcrumb, getRouteBreadcrumbs } from "../components/ui/Breadcrumb";

export function AppShell({ children }: { children: React.ReactNode }) {
  const breadcrumbs = getRouteBreadcrumbs();
  const title = breadcrumbs[breadcrumbs.length - 1] || "Dashboard";

  return (
    <div className="shell admin-shell">
      <Sidebar />
      <div className="main app-main">
        <TopBar />
        <Breadcrumb segments={breadcrumbs} title={title} />
        <section className="content app-content">{children}</section>
      </div>
    </div>
  );
}
