import { useEffect, useMemo, useState } from "react";
import * as Icons from "lucide-react";
import { navigate } from "../app/routes";
import { Logo } from "../components/ui/Logo";
import {
  defaultNavigation,
  getNavigation,
  type NavigationItem,
} from "../services/navigationApi";
import { parsePermissions, useAuth } from "../auth/AuthContext";

const storageKey = "procuraflow.sidebar.groups";
const compactKey = "procuraflow.sidebar.compact";
const iconMap = Icons as unknown as Record<
  string,
  React.ComponentType<{
    size?: number;
    className?: string;
    "aria-hidden"?: string | boolean;
  }>
>;
function Icon({ name, size = 18 }: { name: string; size?: number }) {
  const C = iconMap[name] ?? Icons.Circle;
  return <C size={size} aria-hidden="true" />;
}
function visible(items: NavigationItem[], hasPermission: (code: string) => boolean): NavigationItem[] {
  return items
    .filter((x) => x.isVisible)
    .filter((x) => {
      const required = parsePermissions(x.permissionsJson);
      return required.length === 0 || required.some(hasPermission);
    })
    .sort((a, b) => a.displayOrder - b.displayOrder)
    .map((x) => ({ ...x, children: visible(x.children || [], hasPermission) }));
}
function getStoredExpandedGroups(items: NavigationItem[]) {
  const defaults = Object.fromEntries(
    items.map((g) => [g.code, g.isExpandedByDefault]),
  );
  try {
    const saved = localStorage.getItem(storageKey);
    return saved ? { ...defaults, ...JSON.parse(saved) } : defaults;
  } catch {
    return defaults;
  }
}
function isActiveRoute(currentPath: string, href?: string | null) {
  if (!href) return false;
  if (href === "/app/dashboard")
    return currentPath === href || currentPath === "/app";
  return currentPath === href || currentPath.startsWith(`${href}/`);
}
function itemHasActive(item: NavigationItem, currentPath: string): boolean {
  return (
    isActiveRoute(currentPath, item.url) ||
    item.children.some((child) => itemHasActive(child, currentPath))
  );
}

function NavLink({
  item,
  compact,
  level = 0,
}: {
  item: NavigationItem;
  compact: boolean;
  level?: number;
}) {
  const active = isActiveRoute(location.pathname, item.url);
  return (
    <a
      className={`navlink ${active ? "active" : ""}`}
      style={{ paddingLeft: compact ? undefined : 14 + level * 14 }}
      href={item.url || "#"}
      aria-current={active ? "page" : undefined}
      data-tooltip={item.label}
      title={compact ? item.label : undefined}
      onClick={(e) => {
        e.preventDefault();
        if (item.url) navigate(item.url);
      }}
    >
      <Icon name={item.icon} />
      {!compact && <span>{item.label}</span>}
    </a>
  );
}
function NavChildren({
  items,
  compact,
  level = 0,
}: {
  items: NavigationItem[];
  compact: boolean;
  level?: number;
}) {
  return (
    <>
      {items.map((item) =>
        item.itemType === "Group" ? (
          <NavGroup
            item={item}
            compact={compact}
            level={level}
            key={item.code}
          />
        ) : (
          <NavLink
            item={item}
            compact={compact}
            level={level}
            key={item.code}
          />
        ),
      )}
    </>
  );
}
function NavGroup({
  item,
  compact,
  level = 0,
}: {
  item: NavigationItem;
  compact: boolean;
  level?: number;
}) {
  const [expanded, setExpanded] = useState<Record<string, boolean>>({});
  useEffect(
    () =>
      setExpanded((current) => ({
        [item.code]: item.isExpandedByDefault,
        ...current,
      })),
    [item.code, item.isExpandedByDefault],
  );
  const isExpanded = expanded[item.code] ?? item.isExpandedByDefault;
  const active = itemHasActive(item, location.pathname);
  const groupId = `sidebar-group-${item.code}`;
  return (
    <div className={`navgroup ${active ? "has-active" : ""}`}>
      <button
        className="navgroup-toggle"
        onClick={() =>
          item.isCollapsible &&
          setExpanded((current) => ({ ...current, [item.code]: !isExpanded }))
        }
        aria-expanded={isExpanded}
        aria-controls={groupId}
        title={item.label}
        style={{ paddingLeft: compact ? undefined : 10 + level * 12 }}
      >
        <span className="navgroup-rail" aria-hidden="true" />
        {!compact && (
          <>
            <Icon name={item.icon} size={16} />
            <span className="navgroup-label">{item.label}</span>
          </>
        )}
        {item.isCollapsible && (
          <Icons.ChevronDown
            className={`navgroup-chevron ${isExpanded ? "" : "collapsed"}`}
            size={16}
            aria-hidden="true"
          />
        )}
      </button>
      {isExpanded && (
        <div className="navgroup-links" id={groupId}>
          <NavChildren
            items={item.children}
            compact={compact}
            level={level + 1}
          />
        </div>
      )}
    </div>
  );
}

export function Sidebar() {
  const { hasPermission } = useAuth();
  const [navigation, setNavigation] = useState(defaultNavigation);
  const [compact, setCompact] = useState(
    () => localStorage.getItem(compactKey) === "true",
  );
  const items = useMemo(() => visible(navigation.items, hasPermission), [navigation, hasPermission]);
  useEffect(() => {
    getNavigation().then((r) => setNavigation(r.data));
  }, []);
  useEffect(
    () =>
      localStorage.setItem(
        storageKey,
        JSON.stringify(getStoredExpandedGroups(items)),
      ),
    [items],
  );
  useEffect(() => localStorage.setItem(compactKey, String(compact)), [compact]);
  useEffect(() => {
    const toggle = () => setCompact((x) => !x);
    window.addEventListener("procuraflow:toggle-sidebar", toggle);
    return () =>
      window.removeEventListener("procuraflow:toggle-sidebar", toggle);
  }, []);
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
          {compact ? (
            <Icons.PanelLeftOpen size={18} />
          ) : (
            <Icons.PanelLeftClose size={18} />
          )}
        </button>
      </div>
      <nav className="sidebar-nav" aria-label="Primary navigation">
        <NavChildren items={items} compact={compact} />
      </nav>
    </aside>
  );
}
