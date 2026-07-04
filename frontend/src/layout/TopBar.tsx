import { Bell, Search, UserCircle } from "lucide-react";
import { Input } from "../components/ui/Input";
export function TopBar() {
  const crumb =
    location.pathname
      .replace("/app/", "")
      .split("/")
      .filter(Boolean)
      .join(" / ") || "dashboard";
  return (
    <header className="topbar">
      <strong>ProcuraFlow / {crumb}</strong>
      <div className="search">
        <Search size={16} />
        <Input
          aria-label="Global search"
          placeholder="Search suppliers, workflows, tasks, audit events…"
        />
      </div>
      <span className="topbar-org">Lesotho Communications Authority</span>
      <Bell size={19} />
      <UserCircle size={22} />
    </header>
  );
}
