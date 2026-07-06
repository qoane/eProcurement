import { Sidebar } from "./Sidebar";
import { TopBar } from "./TopBar";

function toTitle(segment: string) {
  return segment
    .split("-")
    .filter(Boolean)
    .map((part) => part.charAt(0).toUpperCase() + part.slice(1))
    .join(" ");
}

function getBreadcrumbs() {
  const segments = location.pathname
    .replace(/^\/app\/?/, "")
    .split("/")
    .filter(Boolean);
  const trail = segments.length ? segments : ["dashboard"];

  return ["Home", ...trail.map(toTitle)];
}

export function AppShell({ children }: { children: React.ReactNode }) {
  const breadcrumbs = getBreadcrumbs();
  const title = breadcrumbs[breadcrumbs.length - 1] || "Dashboard";

  return (
    <div className="shell admin-shell">
      <Sidebar />
      <div className="main app-main">
        <TopBar />
        <div className="breadcrumb-bar" aria-label="Breadcrumb">
          <div>
            <h1>{title}</h1>
            <ol className="breadcrumb-list">
              {breadcrumbs.map((crumb, index) => (
                <li
                  key={`${crumb}-${index}`}
                  aria-current={
                    index === breadcrumbs.length - 1 ? "page" : undefined
                  }
                >
                  {crumb}
                </li>
              ))}
            </ol>
          </div>
        </div>
        <section className="content app-content">{children}</section>
      </div>
    </div>
  );
}
