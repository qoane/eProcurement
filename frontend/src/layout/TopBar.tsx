import { Bell, Building2, Mail, Menu, Search, UserCircle } from "lucide-react";
import { Input } from "../components/ui/Input";

function toggleSidebar() {
  window.dispatchEvent(new Event("procuraflow:toggle-sidebar"));
}

export function TopBar() {
  return (
    <header className="topbar app-topbar">
      <button
        className="topbar-icon-button"
        type="button"
        aria-label="Toggle sidebar"
        onClick={toggleSidebar}
      >
        <Menu size={21} />
      </button>
      <div className="search topbar-search">
        <Search size={16} />
        <Input
          aria-label="Global search"
          placeholder="Search suppliers, workflows, tasks, audit events…"
        />
      </div>
      <div className="topbar-actions" aria-label="Workspace actions">
        <button
          className="topbar-icon-button has-badge"
          type="button"
          aria-label="Messages"
        >
          <Mail size={19} />
          <span className="topbar-badge">4</span>
        </button>
        <button
          className="topbar-icon-button has-badge"
          type="button"
          aria-label="Notifications"
        >
          <Bell size={19} />
          <span className="topbar-badge warning">7</span>
        </button>
        <div
          className="topbar-user-menu"
          role="button"
          tabIndex={0}
          aria-label="Organization and user menu"
        >
          <Building2 size={18} />
          <span className="topbar-org">Lesotho Communications Authority</span>
          <UserCircle size={24} />
        </div>
      </div>
    </header>
  );
}
