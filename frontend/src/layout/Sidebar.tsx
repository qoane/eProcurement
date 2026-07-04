import {
  BarChart3,
  ClipboardList,
  FileCog,
  FileText,
  Gauge,
  History,
  Settings,
  ShieldCheck,
  Users,
  Workflow,
} from "lucide-react";
import { navigate } from "../app/routes";
const groups = [
  ["Overview", [["Dashboard", "/app/dashboard", Gauge]]],
  [
    "Operations",
    [
      ["Suppliers", "/app/suppliers", Users],
      ["Workflow Tasks", "/app/tasks", ClipboardList],
      ["Audit Explorer", "/app/audit", History],
    ],
  ],
  [
    "Platform Studio",
    [
      ["Workflows", "/app/workflows", Workflow],
      ["Business Rules", "/app/rules", ShieldCheck],
      ["Dynamic Forms", "/app/forms", FileText],
      ["Configuration", "/app/configuration", FileCog],
    ],
  ],
  ["Insights", [["Reporting", "/app/reporting", BarChart3]]],
  ["System", [["Settings", "/app/configuration", Settings]]],
] as const;
export function Sidebar() {
  return (
    <aside className="sidebar">
      <div className="brand">
        BeeOnline Enterprise Platform<small>Configured for LCA</small>
      </div>
      {groups.map(([g, links]) => (
        <div className="navgroup" key={g}>
          <p>{g}</p>
          {links.map(([l, h, I]) => (
            <a
              className={`navlink ${location.pathname === h ? "active" : ""}`}
              href={h}
              onClick={(e) => {
                e.preventDefault();
                navigate(h);
              }}
              key={l}
            >
              <I size={18} />
              {l}
            </a>
          ))}
        </div>
      ))}
    </aside>
  );
}
