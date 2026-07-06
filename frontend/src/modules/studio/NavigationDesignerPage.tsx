import { useEffect, useMemo, useState } from "react";
import { Badge } from "../../components/ui/Badge";
import { Card } from "../../components/ui/Card";
import { EmptyState } from "../../components/ui/EmptyState";
import { PageHeader } from "../../components/ui/PageHeader";
import { defaultNavigation, getNavigation, saveNavigation, type NavigationDesigner, type NavigationItem } from "../../services/navigationApi";

const emptyItem = (parentId?: string | null): NavigationItem => ({ code: `item-${Date.now()}`, label: "New item", itemType: parentId ? "Link" : "Group", url: parentId ? "/app/" : null, icon: parentId ? "Circle" : "Folder", displayOrder: 10, parentId, isCollapsible: !parentId, isExpandedByDefault: true, permissionsJson: "[]", visibilityRule: "", isVisible: true, children: [] });
function flatten(items: NavigationItem[], depth = 0): Array<{ item: NavigationItem; depth: number }> { return items.flatMap((item) => [{ item, depth }, ...flatten(item.children, depth + 1)]); }
function update(items: NavigationItem[], code: string, change: (item: NavigationItem) => NavigationItem): NavigationItem[] { return items.map((item) => item.code === code ? change(item) : { ...item, children: update(item.children, code, change) }); }
function remove(items: NavigationItem[], code: string): NavigationItem[] { return items.filter((item) => item.code !== code).map((item) => ({ ...item, children: remove(item.children, code) })); }
function addChild(items: NavigationItem[], code: string): NavigationItem[] { return update(items, code, (item) => ({ ...item, itemType: "Group", isCollapsible: true, children: [...item.children, emptyItem(item.id ?? item.code)] })); }

export function NavigationDesignerPage() {
  const [nav, setNav] = useState<NavigationDesigner>(defaultNavigation);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState("");
  const rows = useMemo(() => flatten(nav.items), [nav.items]);
  useEffect(() => { getNavigation().then((r) => setNav(r.data)); }, []);
  const setItems = (items: NavigationItem[]) => setNav((current) => ({ ...current, items }));
  const onSave = async () => { setSaving(true); const r = await saveNavigation(nav); setNav(r.data); setMessage(r.error ? `Saved locally only: ${r.error}` : "Navigation saved to SQL Server."); setSaving(false); };
  return <div className="studio-page">
    <PageHeader title="Navigation Designer" description="Configure SQL Server-backed sidebar groups, collapsible sections, nested menus, icons, order, permissions, and visibility rules without hardcoding navigation." actions={<><Badge tone="info">Admin</Badge><button className="btn" onClick={onSave} disabled={saving}>{saving ? "Saving…" : "Save navigation"}</button></>} />
    <Card className="studio-module-shell"><div><h2>{nav.name}</h2><p className="muted">{nav.description}</p></div></Card>
    <div className="toolbar-row"><button className="btn secondary" onClick={() => setItems([...nav.items, emptyItem(null)])}>Add group</button><button className="btn secondary" onClick={() => setNav(defaultNavigation)}>Load example</button>{message && <span className="muted">{message}</span>}</div>
    {rows.length === 0 ? <EmptyState title="No navigation items" message="Add a group to begin designing the application sidebar." /> : <div className="card table-card"><table className="data-table"><thead><tr><th>Item</th><th>Type</th><th>Icon</th><th>URL</th><th>Order</th><th>Permissions JSON</th><th>Visibility Rule</th><th>Visible</th><th></th></tr></thead><tbody>{rows.map(({ item, depth }) => <tr key={item.code}><td style={{ paddingLeft: 16 + depth * 22 }}><input className="input" value={item.label} onChange={(e) => setItems(update(nav.items, item.code, (x) => ({ ...x, label: e.target.value, code: e.target.value.toLowerCase().replace(/[^a-z0-9]+/g, "-").replace(/^-|-$/g, "") || x.code })))} /></td><td><select className="input" value={item.itemType} onChange={(e) => setItems(update(nav.items, item.code, (x) => ({ ...x, itemType: e.target.value as NavigationItem["itemType"] })))}><option>Group</option><option>Link</option></select></td><td><input className="input" value={item.icon} onChange={(e) => setItems(update(nav.items, item.code, (x) => ({ ...x, icon: e.target.value })))} /></td><td><input className="input" value={item.url || ""} onChange={(e) => setItems(update(nav.items, item.code, (x) => ({ ...x, url: e.target.value })))} disabled={item.itemType === "Group"} /></td><td><input className="input" type="number" value={item.displayOrder} onChange={(e) => setItems(update(nav.items, item.code, (x) => ({ ...x, displayOrder: Number(e.target.value) })))} /></td><td><input className="input" value={item.permissionsJson} onChange={(e) => setItems(update(nav.items, item.code, (x) => ({ ...x, permissionsJson: e.target.value })))} /></td><td><input className="input" value={item.visibilityRule} placeholder="Leave blank to show" onChange={(e) => setItems(update(nav.items, item.code, (x) => ({ ...x, visibilityRule: e.target.value })))} /></td><td><input type="checkbox" checked={item.isVisible} onChange={(e) => setItems(update(nav.items, item.code, (x) => ({ ...x, isVisible: e.target.checked })))} /></td><td><button className="btn secondary" onClick={() => setItems(addChild(nav.items, item.code))}>Nest</button> <button className="btn ghost" onClick={() => setItems(remove(nav.items, item.code))}>Remove</button></td></tr>)}</tbody></table></div>}
  </div>;
}
