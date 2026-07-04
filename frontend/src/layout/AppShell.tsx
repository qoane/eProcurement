import { Sidebar } from "./Sidebar";
import { TopBar } from "./TopBar";
export function AppShell({ children }: { children: React.ReactNode }) {
  return (
    <div className="shell">
      <Sidebar />
      <div className="main">
        <TopBar />
        <section className="content">{children}</section>
      </div>
    </div>
  );
}
