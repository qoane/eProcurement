import { useEffect, useState } from "react";
import {
  BarChart3,
  BriefcaseBusiness,
  ChevronDown,
  ClipboardCheck,
  ClipboardList,
  FileCheck2,
  FileCog,
  FileText,
  Gauge,
  History,
  LayoutDashboard,
  ListChecks,
  PanelLeftClose,
  PanelLeftOpen,
  ScrollText,
  Settings,
  ShieldCheck,
  SquarePen,
  Users,
  Workflow,
} from "lucide-react";
import { navigate } from "../app/routes";
import { Logo } from "../components/ui/Logo";

const groups = [
  ["Overview", [["Dashboard", "/app/dashboard", Gauge]]],
  [
    "Procurement Operations",
    [
      ["Suppliers", "/app/suppliers", Users],
      ["Supplier Registration", "/app/suppliers/register", SquarePen],
      ["Supplier Verification", "/app/suppliers/verification", ClipboardCheck],
      ["Workflow Tasks", "/app/tasks", ClipboardList],
      ["Audit Explorer", "/app/audit", History],
    ],
  ],
  [
    "Procurement Modules",
    [
      ["Planning", "/app/planning", ListChecks],
      ["Requisitions", "/app/requisitions", FileText],
      ["Tenders", "/app/tenders", ScrollText],
      ["Evaluation", "/app/evaluation", ClipboardCheck],
      ["Awards", "/app/awards", FileCheck2],
      ["Purchase Orders", "/app/purchase-orders", BriefcaseBusiness],
      ["Contracts", "/app/contracts", FileCog],
    ],
  ],
  [
    "Platform Studio",
    [
      ["Workflow Designer", "/app/workflows/designer", Workflow],
      ["Business Rules", "/app/rules", ShieldCheck],
      ["Dynamic Forms", "/app/forms", FileText],
      ["Configuration", "/app/configuration", FileCog],
    ],
  ],
  [
    "Insights",
    [
      ["Reporting", "/app/reporting", BarChart3],
      ["Dashboards", "/app/dashboards", LayoutDashboard],
    ],
  ],
  ["System", [["Settings", "/app/settings", Settings]]],
] as const;

const storageKey = "procuraflow.sidebar.groups";
const compactKey = "procuraflow.sidebar.compact";

export function Sidebar() {
  const [expanded, setExpanded] = useState<Record<string, boolean>>(() => {
    const saved = localStorage.getItem(storageKey);
    return saved
      ? JSON.parse(saved)
      : Object.fromEntries(groups.map(([g]) => [g, true]));
  });
  const [compact, setCompact] = useState(
    () => localStorage.getItem(compactKey) === "true",
  );

  useEffect(
    () => localStorage.setItem(storageKey, JSON.stringify(expanded)),
    [expanded],
  );
  useEffect(() => localStorage.setItem(compactKey, String(compact)), [compact]);

  return (
    <aside className={`sidebar ${compact ? "compact" : ""}`}>
      <div className="sidebar-head">
        <Logo compact={compact} />
        <button
          className="icon-button"
          onClick={() => setCompact((x) => !x)}
          aria-label="Collapse sidebar"
        >
          {compact ? <PanelLeftOpen size={18} /> : <PanelLeftClose size={18} />}
        </button>
      </div>
      <nav>
        {groups.map(([g, links]) => (
          <div className="navgroup" key={g}>
            <button
              className="navgroup-toggle"
              onClick={() => setExpanded((x) => ({ ...x, [g]: !x[g] }))}
              title={g}
            >
              {!compact && <span>{g}</span>}
              {!compact && (
                <ChevronDown
                  className={expanded[g] ? "" : "collapsed"}
                  size={15}
                />
              )}
            </button>
            {(expanded[g] || compact) &&
              links.map(([l, h, I]) => (
                <a
                  className={`navlink ${location.pathname === h ? "active" : ""}`}
                  href={h}
                  title={l}
                  onClick={(e) => {
                    e.preventDefault();
                    navigate(h);
                  }}
                  key={l}
                >
                  <I size={18} />
                  {!compact && <span>{l}</span>}
                </a>
              ))}
          </div>
        ))}
      </nav>
    </aside>
  );
}
