import { useEffect, useMemo, useState } from "react";
import { Badge } from "../../components/ui/Badge";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { DataTable } from "../../components/ui/DataTable";
import { Input } from "../../components/ui/Input";
import { PageHeader } from "../../components/ui/PageHeader";
import { Select } from "../../components/ui/Select";
import {
  createPageDefinition,
  deletePageDefinition,
  getPageDefinitions,
  updatePageDefinition,
} from "../../services/pageDefinitionsApi";
import type { PageDesigner, PageType } from "../../types/api";

const pageTypes: PageType[] = [
  "Dashboard",
  "DataGrid",
  "DetailPage",
  "Wizard",
  "Form",
  "SplitView",
  "Timeline",
  "Kanban",
  "Calendar",
  "Report",
];
const blank: PageDesigner = {
  code: "SUPPLIER-GRID",
  name: "Supplier Grid",
  description:
    "Metadata-driven supplier worklist for the future UI Composition Engine.",
  pageType: "DataGrid",
  datasource: {
    entity: "Supplier",
    mode: "Metadata",
    endpoint: "/api/suppliers",
    keyField: "id",
  },
  layout: {
    template: "responsive-grid",
    columns: 12,
    density: "Comfortable",
    regions: ["toolbar", "filters", "main"],
  },
  toolbar: [
    {
      code: "refresh",
      label: "Refresh",
      kind: "Button",
      actionCode: "refresh",
    },
  ],
  actions: [
    {
      code: "open",
      label: "Open supplier",
      kind: "Navigate",
      target: "/app/suppliers/{referenceNumber}",
    },
  ],
  filters: [
    { code: "status", label: "Status", field: "status", operator: "Equals" },
  ],
  columns: [
    {
      code: "referenceNumber",
      label: "Reference",
      field: "referenceNumber",
      displayOrder: 1,
      sortable: true,
      searchable: true,
    },
    {
      code: "legalName",
      label: "Legal name",
      field: "legalName",
      displayOrder: 2,
      sortable: true,
      searchable: true,
    },
  ],
  components: [
    {
      code: "supplier-grid",
      name: "Supplier grid",
      componentType: "DataTable",
      region: "main",
      displayOrder: 1,
    },
  ],
  permissions: [{ role: "ProcurementOfficer", access: "View" }],
  navigation: {
    route: "/app/suppliers",
    menuGroup: "Procurement Operations",
    showInNavigation: true,
  },
  status: "Draft",
  version: 1,
};

export function PageDesignerPage() {
  const [pages, setPages] = useState<PageDesigner[]>([]);
  const [form, setForm] = useState<PageDesigner>(blank);
  const [message, setMessage] = useState("");
  const selected = useMemo(
    () => pages.find((x) => x.id === form.id),
    [pages, form.id],
  );
  async function load() {
    setPages((await getPageDefinitions()).data);
  }
  useEffect(() => {
    void load();
  }, []);
  async function save() {
    form.id
      ? await updatePageDefinition(form.id, form)
      : await createPageDefinition(form);
    setMessage(
      `${form.name} page metadata saved for the UI Composition Engine.`,
    );
    setForm(blank);
    await load();
  }
  async function remove(id: string) {
    await deletePageDefinition(id);
    setMessage("Page metadata deleted.");
    if (form.id === id) setForm(blank);
    await load();
  }
  const updateList = <
    K extends
      | "toolbar"
      | "actions"
      | "filters"
      | "columns"
      | "components"
      | "permissions",
  >(
    key: K,
    index: number,
    patch: Partial<PageDesigner[K][number]>,
  ) =>
    setForm({
      ...form,
      [key]: form[key].map((item, i) =>
        i === index ? { ...item, ...patch } : item,
      ),
    });

  return (
    <div className="studio-page">
      <PageHeader
        title="Page Designer"
        description="Define metadata-only pages for dashboards, grids, detail pages, wizards, forms, split views, timelines, kanban boards, calendars, and reports. Layouts are stored in SQL Server metadata and are not hardcoded."
        actions={<Badge tone="info">UI Composition ready</Badge>}
      />
      {message && (
        <Card>
          <strong>{message}</strong>
        </Card>
      )}
      <div className="grid cols-2">
        <Card>
          <h2>{selected ? "Edit page" : "Create page"}</h2>
          <div className="form-grid">
            <label>
              Name
              <Input
                value={form.name}
                onChange={(e) => setForm({ ...form, name: e.target.value })}
              />
            </label>
            <label>
              Code
              <Input
                value={form.code}
                onChange={(e) =>
                  setForm({
                    ...form,
                    code: e.target.value.toUpperCase().replace(/\s+/g, "-"),
                  })
                }
              />
            </label>
            <label>
              Page type
              <Select
                value={form.pageType}
                onChange={(e) =>
                  setForm({ ...form, pageType: e.target.value as PageType })
                }
              >
                {pageTypes.map((x) => (
                  <option key={x}>{x}</option>
                ))}
              </Select>
            </label>
            <label>
              Status
              <Select
                value={form.status}
                onChange={(e) => setForm({ ...form, status: e.target.value })}
              >
                <option>Draft</option>
                <option>Active</option>
                <option>Inactive</option>
                <option>Archived</option>
              </Select>
            </label>
          </div>
          <label>
            Description
            <textarea
              className="input"
              value={form.description}
              onChange={(e) =>
                setForm({ ...form, description: e.target.value })
              }
            />
          </label>
          <h3>Datasource & navigation</h3>
          <div className="form-grid">
            <Input
              value={form.datasource.entity}
              onChange={(e) =>
                setForm({
                  ...form,
                  datasource: { ...form.datasource, entity: e.target.value },
                })
              }
            />
            <Input
              value={form.datasource.endpoint || ""}
              onChange={(e) =>
                setForm({
                  ...form,
                  datasource: { ...form.datasource, endpoint: e.target.value },
                })
              }
            />
            <Input
              value={form.navigation.route}
              onChange={(e) =>
                setForm({
                  ...form,
                  navigation: { ...form.navigation, route: e.target.value },
                })
              }
            />
            <Input
              value={form.navigation.menuGroup || ""}
              onChange={(e) =>
                setForm({
                  ...form,
                  navigation: { ...form.navigation, menuGroup: e.target.value },
                })
              }
            />
          </div>
          <h3>Layout</h3>
          <div className="form-grid">
            <Input
              value={form.layout.template}
              onChange={(e) =>
                setForm({
                  ...form,
                  layout: { ...form.layout, template: e.target.value },
                })
              }
            />
            <Input
              type="number"
              value={form.layout.columns || 12}
              onChange={(e) =>
                setForm({
                  ...form,
                  layout: { ...form.layout, columns: Number(e.target.value) },
                })
              }
            />
            <Input
              value={form.layout.density || ""}
              onChange={(e) =>
                setForm({
                  ...form,
                  layout: { ...form.layout, density: e.target.value },
                })
              }
            />
            <Input
              value={(form.layout.regions || []).join(",")}
              onChange={(e) =>
                setForm({
                  ...form,
                  layout: {
                    ...form.layout,
                    regions: e.target.value
                      .split(",")
                      .map((x) => x.trim())
                      .filter(Boolean),
                  },
                })
              }
            />
          </div>
          <h3>Toolbar</h3>
          {form.toolbar.map((item, index) => (
            <div className="form-grid" key={index}>
              <Input
                value={item.code}
                onChange={(e) =>
                  updateList("toolbar", index, { code: e.target.value })
                }
              />
              <Input
                value={item.label}
                onChange={(e) =>
                  updateList("toolbar", index, { label: e.target.value })
                }
              />
              <Input
                value={item.actionCode || ""}
                onChange={(e) =>
                  updateList("toolbar", index, { actionCode: e.target.value })
                }
              />
            </div>
          ))}
          <Button
            variant="secondary"
            onClick={() =>
              setForm({
                ...form,
                toolbar: [
                  ...form.toolbar,
                  { code: "new-command", label: "New Command", kind: "Button" },
                ],
              })
            }
          >
            Add toolbar item
          </Button>
          <h3>Actions</h3>
          {form.actions.map((item, index) => (
            <div className="form-grid" key={index}>
              <Input
                value={item.code}
                onChange={(e) =>
                  updateList("actions", index, { code: e.target.value })
                }
              />
              <Input
                value={item.label}
                onChange={(e) =>
                  updateList("actions", index, { label: e.target.value })
                }
              />
              <Input
                value={item.target || ""}
                onChange={(e) =>
                  updateList("actions", index, { target: e.target.value })
                }
              />
            </div>
          ))}
          <Button
            variant="secondary"
            onClick={() =>
              setForm({
                ...form,
                actions: [
                  ...form.actions,
                  { code: "new-action", label: "New Action", kind: "Command" },
                ],
              })
            }
          >
            Add action
          </Button>
          <h3>Filters, columns, components, permissions</h3>
          {form.filters.map((item, index) => (
            <div className="form-grid" key={`f-${index}`}>
              <Input
                value={item.code}
                onChange={(e) =>
                  updateList("filters", index, { code: e.target.value })
                }
              />
              <Input
                value={item.field}
                onChange={(e) =>
                  updateList("filters", index, { field: e.target.value })
                }
              />
              <Input
                value={item.label}
                onChange={(e) =>
                  updateList("filters", index, { label: e.target.value })
                }
              />
            </div>
          ))}
          <Button
            variant="secondary"
            onClick={() =>
              setForm({
                ...form,
                filters: [
                  ...form.filters,
                  { code: "new-filter", label: "New Filter", field: "status" },
                ],
              })
            }
          >
            Add filter
          </Button>
          {form.columns.map((item, index) => (
            <div className="form-grid" key={`c-${index}`}>
              <Input
                value={item.code}
                onChange={(e) =>
                  updateList("columns", index, { code: e.target.value })
                }
              />
              <Input
                value={item.field}
                onChange={(e) =>
                  updateList("columns", index, { field: e.target.value })
                }
              />
              <Input
                value={item.label}
                onChange={(e) =>
                  updateList("columns", index, { label: e.target.value })
                }
              />
            </div>
          ))}
          <Button
            variant="secondary"
            onClick={() =>
              setForm({
                ...form,
                columns: [
                  ...form.columns,
                  { code: "new-column", label: "New Column", field: "field" },
                ],
              })
            }
          >
            Add column
          </Button>
          {form.components.map((item, index) => (
            <div className="form-grid" key={`m-${index}`}>
              <Input
                value={item.code}
                onChange={(e) =>
                  updateList("components", index, { code: e.target.value })
                }
              />
              <Input
                value={item.componentType}
                onChange={(e) =>
                  updateList("components", index, {
                    componentType: e.target.value,
                  })
                }
              />
              <Input
                value={item.region || ""}
                onChange={(e) =>
                  updateList("components", index, { region: e.target.value })
                }
              />
            </div>
          ))}
          <Button
            variant="secondary"
            onClick={() =>
              setForm({
                ...form,
                components: [
                  ...form.components,
                  {
                    code: "new-component",
                    name: "New Component",
                    componentType: "Card",
                    region: "main",
                  },
                ],
              })
            }
          >
            Add component
          </Button>
          {form.permissions.map((item, index) => (
            <div className="form-grid" key={`p-${index}`}>
              <Input
                value={item.role}
                onChange={(e) =>
                  updateList("permissions", index, { role: e.target.value })
                }
              />
              <Input
                value={item.access || ""}
                onChange={(e) =>
                  updateList("permissions", index, { access: e.target.value })
                }
              />
            </div>
          ))}
          <Button
            variant="secondary"
            onClick={() =>
              setForm({
                ...form,
                permissions: [
                  ...form.permissions,
                  { role: "Administrator", access: "Manage" },
                ],
              })
            }
          >
            Add permission
          </Button>
          <div className="actions">
            <Button onClick={save}>
              {form.id ? "Update" : "Create"} Page Metadata
            </Button>
            <Button variant="secondary" onClick={() => setForm(blank)}>
              Reset
            </Button>
          </div>
        </Card>
        <Card>
          <h2>Metadata preview</h2>
          <p className="muted">
            The future UI Composition Engine can render this definition without
            hardcoded page layout logic.
          </p>
          <pre>{JSON.stringify(form, null, 2)}</pre>
        </Card>
      </div>
      <Card>
        <h2>Pages</h2>
        <DataTable
          rows={pages}
          empty="No pages have been defined."
          columns={[
            {
              header: "Page",
              cell: (row) => (
                <>
                  <strong>{row.name}</strong>
                  <p className="muted">
                    {row.code} · {row.navigation.route}
                  </p>
                </>
              ),
            },
            { header: "Type", cell: (row) => row.pageType },
            { header: "Datasource", cell: (row) => row.datasource.entity },
            { header: "Components", cell: (row) => row.components.length },
            {
              header: "Permissions",
              cell: (row) =>
                row.permissions.map((p) => p.role).join(", ") || "—",
            },
            {
              header: "Status",
              cell: (row) => <Badge>{row.status || "Draft"}</Badge>,
            },
            {
              header: "Actions",
              cell: (row) => (
                <div className="actions">
                  <Button variant="secondary" onClick={() => setForm(row)}>
                    Edit
                  </Button>
                  {row.id && (
                    <Button variant="secondary" onClick={() => remove(row.id!)}>
                      Delete
                    </Button>
                  )}
                </div>
              ),
            },
          ]}
        />
      </Card>
    </div>
  );
}
