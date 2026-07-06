import {
  AppWindow,
  Bell,
  Blocks,
  BookOpenCheck,
  Bot,
  Braces,
  Building2,
  ChartNoAxesCombined,
  Database,
  FileBarChart,
  FileStack,
  FileText,
  FormInput,
  GitBranch,
  Grid2X2,
  LayoutDashboard,
  LayoutTemplate,
  ListTree,
  LockKeyhole,
  Menu,
  Network,
  Palette,
  PanelTop,
  Settings,
  Workflow,
} from "lucide-react";
import { navigate } from "../../app/routes";
import { Badge } from "../../components/ui/Badge";
import { Card } from "../../components/ui/Card";
import { EmptyState } from "../../components/ui/EmptyState";
import { PageHeader } from "../../components/ui/PageHeader";
import { ApplicationDesignerPage } from "./ApplicationDesignerPage";
import { EntityDesignerPage } from "./EntityDesignerPage";
import { PageDesignerPage } from "./PageDesignerPage";

type StudioModule = {
  title: string;
  slug: string;
  description: string;
  emptyState: string;
  icon: typeof AppWindow;
  group:
    | "Application Model"
    | "Experience"
    | "Automation"
    | "Data & Content"
    | "Operations";
};

export const studioModules: StudioModule[] = [
  {
    title: "Applications",
    slug: "applications",
    description:
      "Define enterprise applications, ownership, lifecycle status, and release boundaries.",
    emptyState: "No applications exist.",
    icon: AppWindow,
    group: "Application Model",
  },
  {
    title: "Business Processes",
    slug: "business-processes",
    description:
      "Model cross-functional processes that coordinate forms, workflows, rules, and approvals.",
    emptyState: "No business processes have been created.",
    icon: GitBranch,
    group: "Automation",
  },
  {
    title: "Entities",
    slug: "entities",
    description:
      "Configure business objects, fields, relationships, and enterprise data ownership.",
    emptyState: "No entities have been configured.",
    icon: Database,
    group: "Application Model",
  },
  {
    title: "Pages",
    slug: "pages",
    description:
      "Compose workspaces, record pages, list views, and task screens for each application.",
    emptyState: "No pages have been created.",
    icon: PanelTop,
    group: "Experience",
  },
  {
    title: "Layouts",
    slug: "layouts",
    description:
      "Manage responsive layout templates and reusable page regions.",
    emptyState: "No layouts have been created.",
    icon: LayoutTemplate,
    group: "Experience",
  },
  {
    title: "Navigation",
    slug: "navigation",
    description:
      "Design application navigation maps, hubs, sections, and role-aware entry points.",
    emptyState: "No navigation maps have been configured.",
    icon: Network,
    group: "Experience",
  },
  {
    title: "Menus",
    slug: "menus",
    description:
      "Configure menu groups, command surfaces, and contextual actions.",
    emptyState: "No menus have been configured.",
    icon: Menu,
    group: "Experience",
  },
  {
    title: "Dynamic Forms",
    slug: "dynamic-forms",
    description:
      "Build governed forms with sections, fields, validations, and submission behavior.",
    emptyState: "No dynamic forms have been configured.",
    icon: FormInput,
    group: "Experience",
  },
  {
    title: "Workflow Designer",
    slug: "workflow-designer",
    description:
      "Create workflow stages, transitions, actions, task routing, and service-level targets.",
    emptyState: "No workflows have been configured.",
    icon: Workflow,
    group: "Automation",
  },
  {
    title: "Business Rules",
    slug: "business-rules",
    description:
      "Centralize conditional logic, eligibility policies, calculations, and validation rules.",
    emptyState: "No business rules have been configured.",
    icon: Braces,
    group: "Automation",
  },
  {
    title: "Approval Matrices",
    slug: "approval-matrices",
    description:
      "Define approval paths by amount, risk, role, department, and delegated authority.",
    emptyState: "No approval matrices have been configured.",
    icon: BookOpenCheck,
    group: "Automation",
  },
  {
    title: "Lookups",
    slug: "lookups",
    description:
      "Manage controlled reference data used by forms, rules, reports, and integrations.",
    emptyState: "No lookups have been configured.",
    icon: ListTree,
    group: "Data & Content",
  },
  {
    title: "Document Types",
    slug: "document-types",
    description:
      "Govern document categories, requirements, expiry rules, and evidence metadata.",
    emptyState: "No document types have been configured.",
    icon: FileStack,
    group: "Data & Content",
  },
  {
    title: "Reports",
    slug: "reports",
    description:
      "Configure operational reports, filters, schedules, and export experiences.",
    emptyState: "No reports have been configured.",
    icon: FileBarChart,
    group: "Operations",
  },
  {
    title: "Dashboards",
    slug: "dashboards",
    description:
      "Design executive and operational dashboards with metrics, charts, and insight cards.",
    emptyState: "No dashboards have been configured.",
    icon: LayoutDashboard,
    group: "Operations",
  },
  {
    title: "Themes",
    slug: "themes",
    description:
      "Manage brand systems, color palettes, typography, density, and accessible interface tokens.",
    emptyState: "No themes have been configured.",
    icon: Palette,
    group: "Experience",
  },
  {
    title: "Notifications",
    slug: "notifications",
    description:
      "Configure notification templates, channels, event triggers, and delivery policies.",
    emptyState: "No notifications have been configured.",
    icon: Bell,
    group: "Operations",
  },
  {
    title: "Security",
    slug: "security",
    description:
      "Manage roles, permissions, data access policies, and administrative guardrails.",
    emptyState: "No security policies have been configured.",
    icon: LockKeyhole,
    group: "Operations",
  },
  {
    title: "Integrations",
    slug: "integrations",
    description:
      "Connect APIs, event contracts, identity providers, and external enterprise systems.",
    emptyState: "No integrations have been configured.",
    icon: Blocks,
    group: "Operations",
  },
  {
    title: "System Settings",
    slug: "system-settings",
    description:
      "Control environment settings, feature flags, numbering sequences, and platform defaults.",
    emptyState: "No system settings have been configured.",
    icon: Settings,
    group: "Operations",
  },
];

const groupIcons = {
  "Application Model": Building2,
  Experience: Grid2X2,
  Automation: Bot,
  "Data & Content": FileText,
  Operations: ChartNoAxesCombined,
} as const;

export function StudioPage() {
  const groups = Array.from(
    new Set(studioModules.map((module) => module.group)),
  );
  return (
    <div className="studio-page">
      <PageHeader
        title="ProcuraFlow Studio"
        description="Configure enterprise applications from one low-code command center. Supplier Management is the first application built on this platform foundation."
        actions={<Badge tone="info">Studio Preview</Badge>}
      />
      <section className="studio-hero card">
        <div>
          <span className="studio-kicker">Low-code configuration platform</span>
          <h2>
            Design applications, processes, experiences, automation, and
            governance without leaving the platform.
          </h2>
          <p>
            Studio organizes the core configuration areas used to build
            enterprise-grade applications: data models, pages, workflows, rules,
            approvals, reporting, security, and integrations.
          </p>
        </div>
        <div
          className="studio-hero-panel"
          aria-label="Studio configuration layers"
        >
          <span>Application Model</span>
          <span>Experience Builder</span>
          <span>Automation & Governance</span>
          <span>Operations & Insights</span>
        </div>
      </section>
      {groups.map((group) => {
        const Icon = groupIcons[group];
        return (
          <section className="studio-section" key={group}>
            <div className="studio-section-heading">
              <Icon size={21} aria-hidden="true" />
              <h2>{group}</h2>
            </div>
            <div className="studio-card-grid">
              {studioModules
                .filter((module) => module.group === group)
                .map((module) => {
                  const ModuleIcon = module.icon;
                  return (
                    <button
                      className="studio-card"
                      onClick={() => navigate(`/app/studio/${module.slug}`)}
                      key={module.slug}
                    >
                      <span className="studio-card-icon">
                        <ModuleIcon size={22} aria-hidden="true" />
                      </span>
                      <strong>{module.title}</strong>
                      <small>{module.description}</small>
                    </button>
                  );
                })}
            </div>
          </section>
        );
      })}
    </div>
  );
}

export function StudioModulePage({ slug }: { slug: string }) {
  const module = studioModules.find((item) => item.slug === slug);
  if (!module)
    return (
      <>
        <PageHeader
          title="Studio module not found"
          description="Select an available configuration module from ProcuraFlow Studio."
        />
        <EmptyState
          title="Module unavailable"
          message="This Studio module is not registered in the current application shell."
        />
      </>
    );
  if (slug === "applications") return <ApplicationDesignerPage />;
  if (slug === "entities") return <EntityDesignerPage />;
  if (slug === "pages") return <PageDesignerPage />;
  const Icon = module.icon;
  return (
    <div className="studio-page">
      <PageHeader
        title={module.title}
        description={module.description}
        actions={
          <button
            className="btn secondary"
            onClick={() => navigate("/app/studio")}
          >
            Back to Studio
          </button>
        }
      />
      <Card className="studio-module-shell">
        <div className="studio-module-icon">
          <Icon size={30} aria-hidden="true" />
        </div>
        <div>
          <Badge>{module.group}</Badge>
          <h2>{module.title} configuration</h2>
          <p className="muted">
            This shell establishes the workspace for managing{" "}
            {module.title.toLowerCase()} as ProcuraFlow Studio expands.
          </p>
        </div>
      </Card>
      <EmptyState
        title={module.emptyState}
        message="Use ProcuraFlow Studio to create governed configuration records when this module is enabled."
      />
    </div>
  );
}
