import { apiGet, apiPut } from "./apiClient";

export type NavigationItem = {
  id?: string;
  code: string;
  label: string;
  itemType: "Group" | "Link";
  url?: string | null;
  icon: string;
  displayOrder: number;
  parentId?: string | null;
  isCollapsible: boolean;
  isExpandedByDefault: boolean;
  permissionsJson: string;
  visibilityRule: string;
  isVisible: boolean;
  children: NavigationItem[];
};
export type NavigationDesigner = {
  code: string;
  name: string;
  description: string;
  items: NavigationItem[];
};

export const defaultNavigation: NavigationDesigner = {
  code: "MAIN",
  name: "Main navigation",
  description: "Administrator-configured sidebar navigation.",
  items: [
    { code: "procurement", label: "Procurement", itemType: "Group", icon: "BriefcaseBusiness", displayOrder: 10, isCollapsible: true, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [
      { code: "suppliers", label: "Suppliers", itemType: "Link", url: "/app/suppliers", icon: "Users", displayOrder: 10, isCollapsible: false, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [] },
      { code: "tenders", label: "Tenders", itemType: "Link", url: "/app/tenders", icon: "ScrollText", displayOrder: 20, isCollapsible: false, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [] },
    ] },
    { code: "administration", label: "Administration", itemType: "Group", icon: "Settings", displayOrder: 20, isCollapsible: true, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [
      { code: "workflows", label: "Workflows", itemType: "Link", url: "/app/workflows/designer", icon: "Workflow", displayOrder: 10, isCollapsible: false, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [] },
      { code: "rules", label: "Rules", itemType: "Link", url: "/app/rules", icon: "ShieldCheck", displayOrder: 20, isCollapsible: false, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [] },
    ] },
    { code: "studio", label: "Studio", itemType: "Group", icon: "Blocks", displayOrder: 30, isCollapsible: true, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [
      { code: "pages", label: "Pages", itemType: "Link", url: "/app/studio/pages", icon: "PanelTop", displayOrder: 10, isCollapsible: false, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [] },
      { code: "entities", label: "Entities", itemType: "Link", url: "/app/studio/entities", icon: "Database", displayOrder: 20, isCollapsible: false, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [] },
      { code: "dashboards", label: "Dashboards", itemType: "Link", url: "/app/dashboards", icon: "LayoutDashboard", displayOrder: 30, isCollapsible: false, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [] },
    ] },
  ],
};

export const getNavigation = () => apiGet<NavigationDesigner>("/api/navigation/main", defaultNavigation);
export const saveNavigation = (body: NavigationDesigner) => apiPut<NavigationDesigner>("/api/navigation/main", body, body);
