import { Bell, ClipboardList, Moon, Search, UserCircle } from "lucide-react";
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
      <strong>Home / {crumb}</strong>
      <div className="search">
        <Input
          aria-label="Global search"
          placeholder="Search suppliers, workflows, tasks, audit events…"
        />
      </div>
      <Moon />
      <Bell />
      <ClipboardList />
      <UserCircle />
    </header>
  );
}
