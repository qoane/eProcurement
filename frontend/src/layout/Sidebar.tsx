import { useEffect, useMemo, useState } from "react";
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

function getDefaultExpandedGroups() {
  return Object.fromEntries(groups.map(([group]) => [group, true]));
}

function getStoredExpandedGroups() {
  const defaults = getDefaultExpandedGroups();
  try {
    const saved = localStorage.getItem(storageKey);
    return saved ? { ...defaults, ...JSON.parse(saved) } : defaults;
  } catch {
    return defaults;
  }
}

function isActiveRoute(currentPath: string, href: string) {
  if (href === "/app/dashboard")
    return currentPath === href || currentPath === "/app";
  return currentPath === href || currentPath.startsWith(`${href}/`);
}

export function Sidebar() {
  const [expanded, setExpanded] = useState<Record<string, boolean>>(
    getStoredExpandedGroups,
  );
  const [compact, setCompact] = useState(
    () => localStorage.getItem(compactKey) === "true",
  );
  const currentPath = location.pathname;

  const activeGroups = useMemo(
    () =>
      Object.fromEntries(
        groups.map(([group, links]) => [
          group,
          links.some(([, href]) => isActiveRoute(currentPath, href)),
        ]),
      ),
    [currentPath],
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
          className="icon-button sidebar-mode-toggle"
          onClick={() => setCompact((x) => !x)}
          aria-label={compact ? "Expand sidebar" : "Use compact sidebar"}
          title={compact ? "Expand sidebar" : "Compact sidebar"}
        >
          {compact ? <PanelLeftOpen size={18} /> : <PanelLeftClose size={18} />}
        </button>
      </div>
      <nav className="sidebar-nav" aria-label="Primary navigation">
        {groups.map(([group, links]) => {
          const isExpanded = expanded[group];
          const groupId = `sidebar-group-${group.replace(/\W+/g, "-").toLowerCase()}`;

          return (
            <div
              className={`navgroup ${activeGroups[group] ? "has-active" : ""}`}
              key={group}
            >
              <button
                className="navgroup-toggle"
                onClick={() =>
                  setExpanded((current) => ({
                    ...current,
                    [group]: !current[group],
                  }))
                }
                aria-expanded={isExpanded}
                aria-controls={groupId}
                title={group}
              >
                <span className="navgroup-rail" aria-hidden="true" />
                {!compact && <span className="navgroup-label">{group}</span>}
                <ChevronDown
                  className={`navgroup-chevron ${isExpanded ? "" : "collapsed"}`}
                  size={16}
                  aria-hidden="true"
                />
              </button>
              {isExpanded && (
                <div className="navgroup-links" id={groupId}>
                  {links.map(([label, href, Icon]) => {
                    const active = isActiveRoute(currentPath, href);

                    return (
                      <a
                        className={`navlink ${active ? "active" : ""}`}
                        href={href}
                        aria-current={active ? "page" : undefined}
                        data-tooltip={label}
                        title={compact ? label : undefined}
                        onClick={(e) => {
                          e.preventDefault();
                          navigate(href);
                        }}
                        key={label}
                      >
                        <Icon size={18} aria-hidden="true" />
                        {!compact && <span>{label}</span>}
                      </a>
                    );
                  })}
                </div>
              )}
            </div>
          );
        })}
      </nav>
    </aside>
  );
}
