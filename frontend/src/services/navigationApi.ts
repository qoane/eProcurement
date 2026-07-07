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
  items: [],
};

export const getNavigation = () =>
  apiGet<NavigationDesigner>("/api/navigation/main", defaultNavigation);
export const saveNavigation = (body: NavigationDesigner) =>
  apiPut<NavigationDesigner>("/api/navigation/main", body, body);
