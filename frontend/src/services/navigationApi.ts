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
  items: [{ code: "system", label: "System", itemType: "Group", icon: "Settings", displayOrder: 150, isCollapsible: true, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [
    { code: "notifications", label: "Notifications", itemType: "Link", url: "/app/notifications", icon: "Bell", displayOrder: 1, isCollapsible: false, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [] },
    { code: "notification-templates", label: "Notification Templates", itemType: "Link", url: "/app/notification-templates", icon: "MailCog", displayOrder: 2, isCollapsible: false, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [] },
    { code: "notification-logs", label: "Notification Logs", itemType: "Link", url: "/app/notification-logs", icon: "ListChecks", displayOrder: 3, isCollapsible: false, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [] },
    { code: "settings", label: "Settings", itemType: "Link", url: "/app/settings", icon: "Settings", displayOrder: 4, isCollapsible: false, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [] }
  ] }],
};

export const getNavigation = () =>
  apiGet<NavigationDesigner>("/api/navigation/main", defaultNavigation);
export const saveNavigation = (body: NavigationDesigner) =>
  apiPut<NavigationDesigner>("/api/navigation/main", body, body);
