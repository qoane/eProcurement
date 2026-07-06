import { useEffect, useState } from "react";
import { Badge } from "../../components/ui/Badge";
import { Button } from "../../components/ui/Button";
import { EmptyState } from "../../components/ui/EmptyState";
import { Input } from "../../components/ui/Input";
import { Select } from "../../components/ui/Select";
import { PageRenderer } from "../ui-composition/PageRenderer";
import {
  archivePageDefinition,
  createPageDefinition,
  getPageDataSources,
  getPageDefinitions,
  publishPageDefinition,
  updatePageDefinition,
} from "../../services/pageDefinitionsApi";
import type {
  PageComponent,
  PageDataSourceOption,
  PageDesigner,
  PageType,
} from "../../types/api";

const pageTypes: PageType[] = [
  "Dashboard",
  "DataGrid",
  "DetailPage",
  "Form",
  "Wizard",
  "Report",
  "Timeline",
  "Kanban",
  "Calendar",
  "MasterDetail",
  "SplitView",
];
const layouts = [
  { template: "Single Column", regions: ["main"] },
  { template: "Two Column", regions: ["left", "right"] },
  { template: "Three Column", regions: ["left", "main", "right"] },
  { template: "Sidebar Layout", regions: ["sidebar", "main"] },
  { template: "Master Detail", regions: ["master", "detail"] },
  { template: "Dashboard", regions: ["hero", "metrics", "main"] },
  { template: "Wizard", regions: ["steps", "main", "actions"] },
];
type PagePurpose =
  | "data-input-form"
  | "read-only-list"
  | "editable-list"
  | "deletable-list"
  | "detail-edit-page";

const pagePurposeTemplates: {
  code: PagePurpose;
  label: string;
  description: string;
}[] = [
  {
    code: "data-input-form",
    label: "Data input form",
    description: "Create or update records through a guided form region.",
  },
  {
    code: "read-only-list",
    label: "Read-only list",
    description: "Browse rows with a safe view/open action only.",
  },
  {
    code: "editable-list",
    label: "Editable list",
    description: "List rows with edit action and update endpoint metadata.",
  },
  {
    code: "deletable-list",
    label: "List with delete",
    description:
      "List rows with delete action, confirmation, and endpoint metadata.",
  },
  {
    code: "detail-edit-page",
    label: "Detail/edit page",
    description: "Open one record with a detail form and save command.",
  },
];

const toolbox = [
  "Page Header",
  "Card",
  "Data Grid",
  "Form Region",
  "Tabs",
  "Timeline",
  "Metric Card",
  "Button Bar",
  "Empty State",
  "Workflow Status",
  "Audit Panel",
  "Comments",
  "Attachments",
  "Text",
  "Divider",
];
const rendererKey: Record<string, string> = {
  "Data Grid": "DataTable",
  "Metric Card": "Metric",
  "Form Region": "FormField",
};
const componentDefinitions = toolbox.map((name) => ({
  code: name.toUpperCase().replace(/ /g, "_"),
  name,
  description: `${name} designer component`,
  category: "Page Designer",
  rendererKey: rendererKey[name] || name.replace(/ /g, ""),
  properties: [],
  events: [],
  validations: [],
  designMetadata: { icon: "Component" },
}));

const blank: PageDesigner = {
  code: "SUPPLIER-LIST",
  name: "Supplier List",
  description: "Operational supplier page composed from SQL Server metadata.",
  applicationId: undefined,
  pageType: "DataGrid",
  route: "/app/suppliers",
  icon: "Users",
  datasource: {
    entity: "Supplier",
    mode: "Metadata",
    endpoint: "/api/suppliers",
    keyField: "id",
  },
  layout: {
    template: "Single Column",
    columns: 12,
    density: "Comfortable",
    regions: ["main"],
  },
  toolbar: [
    { code: "save", label: "Save", kind: "Button", actionCode: "save" },
  ],
  actions: [
    {
      code: "open",
      label: "Open supplier",
      kind: "Row",
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
    {
      code: "status",
      label: "Status",
      field: "status",
      displayOrder: 3,
      sortable: true,
    },
  ],
  components: [
    {
      code: "page-header",
      name: "Supplier workspace",
      componentType: "PageHeader",
      region: "main",
      displayOrder: 1,
    },
    {
      code: "supplier-grid",
      name: "Supplier grid",
      componentType: "DataTable",
      region: "main",
      displayOrder: 2,
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

function slug(value: string) {
  return value
    .toUpperCase()
    .replace(/[^A-Z0-9]+/g, "-")
    .replace(/(^-|-$)/g, "");
}
function configOf(component: PageComponent) {
  return (component.configuration || {}) as Record<string, string>;
}
function endpointFrom(
  source: PageDataSourceOption | undefined,
  kind: "list" | "get" | "create" | "update" | "delete",
  fallback: string | undefined,
) {
  if (kind === "list") return source?.listEndpoint || fallback || "";
  if (kind === "get") return source?.getEndpoint || `${fallback || ""}/{id}`;
  if (kind === "create") return source?.createEndpoint || fallback || "";
  if (kind === "update")
    return source?.updateEndpoint || `${fallback || ""}/{id}`;
  return source?.deleteEndpoint || `${fallback || ""}/{id}`;
}
function defaultColumns(entity: string) {
  const label = entity || "Record";
  return [
    {
      code: "reference",
      label: "Reference",
      field: "referenceNumber",
      displayOrder: 1,
      sortable: true,
      searchable: true,
    },
    {
      code: "name",
      label: `${label} name`,
      field: "name",
      displayOrder: 2,
      sortable: true,
      searchable: true,
    },
    {
      code: "status",
      label: "Status",
      field: "status",
      displayOrder: 3,
      sortable: true,
    },
  ];
}

export function PageDesignerPage() {
  const [pages, setPages] = useState<PageDesigner[]>([]);
  const [dataSources, setDataSources] = useState<PageDataSourceOption[]>([]);
  const [form, setForm] = useState<PageDesigner>(blank);
  const [selectedCode, setSelectedCode] = useState("page-header");
  const [message, setMessage] = useState("");
  const [preview, setPreview] = useState(false);
  const [activePanel, setActivePanel] = useState<
    "toolbox" | "canvas" | "inspector"
  >("canvas");
  const components = form.components ?? [];
  const selectedComponent =
    components.find((x) => x.code === selectedCode) || components[0];
  async function load() {
    const [pageResult, dataSourceResult] = await Promise.all([
      getPageDefinitions(),
      getPageDataSources(),
    ]);
    const loaded = pageResult.data;
    setPages(loaded);
    setDataSources(dataSourceResult.data);
    setForm((current) => {
      if (current.id) return current;
      const existing = loaded.find((page) => page.code === current.code);
      return existing || current;
    });
  }
  useEffect(() => {
    void load();
  }, []);
  const regions = form.layout.regions?.length ? form.layout.regions : ["main"];
  const selectedDataSource = dataSources.find(
    (source) =>
      source.entity === form.datasource.entity &&
      (source.listEndpoint || "") === (form.datasource.endpoint || ""),
  );
  function applyDataSource(code: string) {
    const source = dataSources.find((item) => item.code === code);
    if (!source) return;
    setForm({
      ...form,
      datasource: {
        entity: source.entity,
        mode: source.mode,
        endpoint: source.listEndpoint,
        keyField: source.keyField,
      },
    });
  }
  async function save() {
    const latestPages = form.id ? pages : (await getPageDefinitions()).data;
    const existing = form.id
      ? undefined
      : latestPages.find((page) => page.code === form.code);
    const pageId = form.id || existing?.id;
    const saved = pageId
      ? await updatePageDefinition(pageId, { ...form, id: pageId })
      : await createPageDefinition(form);
    if (saved.error) {
      setMessage(`Unable to save page definition: ${saved.error}`);
      return undefined;
    }
    setForm(saved.data);
    setMessage(
      `${saved.data.name} saved as SQL Server PageDefinition metadata.`,
    );
    await load();
    return saved.data;
  }
  async function publish() {
    const current = form.id ? form : await save();
    const id =
      current?.id ||
      (await getPageDefinitions()).data.find((x) => x.code === form.code)?.id;
    if (id) {
      const res = await publishPageDefinition(id);
      if (res.error) {
        setMessage(`Unable to publish page definition: ${res.error}`);
        return;
      }
      setForm(res.data);
      setMessage(`${res.data.name} published for runtime rendering.`);
      await load();
    }
  }
  async function archive() {
    if (form.id) {
      const res = await archivePageDefinition(form.id);
      if (res.error) {
        setMessage(`Unable to archive page definition: ${res.error}`);
        return;
      }
      setForm(res.data);
      setMessage(`${res.data.name} archived.`);
      await load();
    }
  }
  function addComponent(type: string, region = regions[0]) {
    const code = slug(`${type}-${components.length + 1}`).toLowerCase();
    const component = {
      code,
      name: type,
      componentType: rendererKey[type] || type.replace(/ /g, ""),
      region,
      displayOrder: components.length + 1,
      configuration: {
        title: type,
        datasource: form.datasource.entity,
        visibilityRule: "",
        width: "12",
        height: "auto",
        cssClass: "",
        permissions: "ProcurementOfficer",
        expressions: "",
      },
    };
    setForm({ ...form, components: [...components, component] });
    setSelectedCode(code);
  }

  function selectedPurpose() {
    return (components
      .map((component) => configOf(component).pagePurpose)
      .find(Boolean) || "") as PagePurpose | "";
  }
  function applyPagePurpose(purpose: PagePurpose) {
    const source = selectedDataSource;
    const entity = form.datasource.entity || source?.entity || "Record";
    const listEndpoint = endpointFrom(source, "list", form.datasource.endpoint);
    const getEndpoint = endpointFrom(source, "get", listEndpoint);
    const createEndpoint = endpointFrom(source, "create", listEndpoint);
    const updateEndpoint = endpointFrom(source, "update", listEndpoint);
    const deleteEndpoint = endpointFrom(source, "delete", listEndpoint);
    const baseConfiguration = {
      datasource: entity,
      pagePurpose: purpose,
      keyField: form.datasource.keyField || source?.keyField || "id",
      listEndpoint,
      getEndpoint,
    };
    const header = {
      code: "page-header",
      name: `${entity} workspace`,
      componentType: "PageHeader",
      region: "main",
      displayOrder: 1,
      configuration: { ...baseConfiguration, title: `${entity} workspace` },
    };
    const table = {
      code: `${slug(entity).toLowerCase()}-table`,
      name: `${entity} list`,
      componentType: "DataTable",
      region: "main",
      displayOrder: 2,
      configuration: { ...baseConfiguration, title: `${entity} list` },
    };
    const formRegion = {
      code: `${slug(entity).toLowerCase()}-form`,
      name: `${entity} form`,
      componentType: "FormField",
      region: "main",
      displayOrder: 2,
      configuration: {
        ...baseConfiguration,
        title: `${entity} form`,
        createEndpoint,
        updateEndpoint,
      },
    };
    const canEdit =
      purpose === "editable-list" || purpose === "detail-edit-page";
    const canDelete = purpose === "deletable-list";
    setForm({
      ...form,
      pageType:
        purpose === "data-input-form"
          ? "Form"
          : purpose === "detail-edit-page"
            ? "DetailPage"
            : "DataGrid",
      datasource: {
        ...form.datasource,
        entity,
        endpoint: listEndpoint || form.datasource.endpoint,
        keyField: form.datasource.keyField || source?.keyField || "id",
      },
      layout: { ...form.layout, template: "Single Column", regions: ["main"] },
      toolbar:
        purpose === "read-only-list" ||
        purpose === "deletable-list" ||
        purpose === "editable-list"
          ? []
          : [
              {
                code: "save",
                label: "Save",
                kind: "Button",
                actionCode: "save",
              },
            ],
      actions: [
        { code: "open", label: "Open", kind: "Row", target: getEndpoint },
        ...(canEdit
          ? [
              {
                code: "edit",
                label: "Edit",
                kind: "Row",
                target: updateEndpoint,
              },
            ]
          : []),
        ...(canDelete
          ? [
              {
                code: "delete",
                label: "Delete",
                kind: "Row",
                target: deleteEndpoint,
                confirmation: `Delete this ${entity.toLowerCase()}? This action cannot be undone.`,
              },
            ]
          : []),
      ],
      columns:
        purpose === "data-input-form" || purpose === "detail-edit-page"
          ? []
          : defaultColumns(entity),
      components:
        purpose === "data-input-form" || purpose === "detail-edit-page"
          ? [header, formRegion]
          : [header, table],
      permissions: [
        {
          role: "ProcurementOfficer",
          access:
            canEdit || canDelete || purpose === "data-input-form"
              ? "Manage"
              : "View",
        },
      ],
    });
    setSelectedCode(
      purpose === "data-input-form" || purpose === "detail-edit-page"
        ? formRegion.code
        : table.code,
    );
    setMessage(
      `${pagePurposeTemplates.find((x) => x.code === purpose)?.label} template applied.`,
    );
  }
  function patchComponent(
    patch: Partial<PageComponent>,
    configuration?: Record<string, string>,
  ) {
    setForm({
      ...form,
      components: components.map((c) =>
        c.code === selectedComponent?.code
          ? {
              ...c,
              ...patch,
              configuration: configuration
                ? { ...configOf(c), ...configuration }
                : c.configuration,
            }
          : c,
      ),
    });
  }
  const statusTone =
    form.status === "Active"
      ? "success"
      : form.status === "Archived"
        ? "warning"
        : "info";
  return (
    <div className="page-designer designer-shell">
      <header className="designer-toolbar">
        <div className="designer-toolbar-title">
          <span className="studio-kicker">Low-code page builder</span>
          <div>
            <h1>Page Designer</h1>
            <p>
              Create metadata-driven application pages without writing React
              code.
            </p>
          </div>
        </div>
        <label className="designer-page-select">
          Page
          <Select
            value={form.id || "new"}
            onChange={(e) => {
              const page = pages.find((p) => p.id === e.target.value);
              setForm(page || blank);
              setSelectedCode((page || blank).components?.[0]?.code || "");
            }}
          >
            <option value="new">Create new page</option>
            {pages.map((p) => (
              <option value={p.id} key={p.id}>
                {p.name}
              </option>
            ))}
          </Select>
        </label>
        <div className="designer-toolbar-actions">
          <Badge tone={statusTone}>{form.status || "Draft"}</Badge>
          <Button onClick={save}>Save</Button>
          <Button onClick={publish}>Publish</Button>
          <Button
            variant={preview ? "primary" : "secondary"}
            onClick={() => setPreview(!preview)}
          >
            {preview ? "Design" : "Preview"}
          </Button>
          <Button variant="secondary" onClick={archive}>
            Archive
          </Button>
        </div>
      </header>

      {message && (
        <div className="designer-message">
          <strong>{message}</strong>
        </div>
      )}

      <nav className="designer-panel-tabs" aria-label="Designer panels">
        <button
          className={activePanel === "toolbox" ? "active" : ""}
          onClick={() => setActivePanel("toolbox")}
        >
          Toolbox
        </button>
        <button
          className={activePanel === "canvas" ? "active" : ""}
          onClick={() => setActivePanel("canvas")}
        >
          Canvas
        </button>
        <button
          className={activePanel === "inspector" ? "active" : ""}
          onClick={() => setActivePanel("inspector")}
        >
          Properties
        </button>
      </nav>

      <aside
        className={`designer-sidebar ${activePanel === "toolbox" ? "active" : ""}`}
      >
        <section className="designer-panel-block">
          <h2>Components</h2>
          <p className="muted">
            Add layout, data, workflow, and collaboration blocks to the selected
            page.
          </p>
          <div className="designer-toolbox-list">
            {toolbox.map((item) => (
              <Button
                key={item}
                variant="secondary"
                onClick={() => addComponent(item)}
              >
                {item}
              </Button>
            ))}
          </div>
        </section>
        <section className="designer-panel-block">
          <h3>Data source</h3>
          <label>
            Datasource
            <Select
              value={selectedDataSource?.code || "custom"}
              onChange={(e) => applyDataSource(e.target.value)}
            >
              <option value="custom">Custom / manual datasource</option>
              {dataSources.map((source) => (
                <option key={source.code} value={source.code}>
                  {source.label} ({source.mode})
                </option>
              ))}
            </Select>
          </label>
          <small className="muted">
            {selectedDataSource?.description ||
              "Choose a generated metadata datasource or custom API-backed datasource discovered from existing page definitions."}
          </small>
          <label>
            Endpoint
            <Input
              value={form.datasource.endpoint || ""}
              onChange={(e) =>
                setForm({
                  ...form,
                  datasource: {
                    ...form.datasource,
                    endpoint: e.target.value || undefined,
                    mode: form.datasource.mode || "CustomApi",
                  },
                })
              }
            />
          </label>
          <label>
            Key field
            <Input
              value={form.datasource.keyField || "id"}
              onChange={(e) =>
                setForm({
                  ...form,
                  datasource: {
                    ...form.datasource,
                    keyField: e.target.value || "id",
                  },
                })
              }
            />
          </label>
          <label>
            Entity
            <Input
              value={form.datasource.entity}
              onChange={(e) =>
                setForm({
                  ...form,
                  datasource: { ...form.datasource, entity: e.target.value },
                })
              }
            />
          </label>
        </section>
      </aside>

      <main
        className={`designer-canvas-area ${preview ? "previewing" : "designing"} ${activePanel === "canvas" ? "active" : ""}`}
      >
        <div className="designer-canvas-header">
          <div>
            <h2>{preview ? "Runtime Preview" : "Design Canvas"}</h2>
            <p>
              {preview
                ? "Full-size runtime rendering using the existing PageRenderer integration."
                : "Compose regions and select components to edit their metadata."}
            </p>
          </div>
          <Badge tone={preview ? "success" : "info"}>
            {preview ? "Preview mode" : form.layout.template}
          </Badge>
        </div>

        {preview ? (
          <div className="designer-runtime-preview">
            <PageRenderer
              page={form}
              componentDefinitions={componentDefinitions}
            />
          </div>
        ) : (
          <>
            <section className="designer-page-settings">
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
                      setForm({ ...form, code: slug(e.target.value) })
                    }
                  />
                </label>
                <label>
                  Application
                  <Input
                    placeholder="ApplicationId"
                    value={form.applicationId || ""}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        applicationId: e.target.value || undefined,
                      })
                    }
                  />
                </label>
                <label>
                  Route
                  <Input
                    value={form.route}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        route: e.target.value,
                        navigation: {
                          ...form.navigation,
                          route: e.target.value,
                        },
                      })
                    }
                  />
                </label>
                <label>
                  Page Type
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
                  Icon
                  <Input
                    value={form.icon}
                    onChange={(e) => setForm({ ...form, icon: e.target.value })}
                  />
                </label>
                <label>
                  Permissions
                  <Input
                    value={form.permissions.map((p) => p.role).join(", ")}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        permissions: e.target.value
                          .split(",")
                          .map((role) => ({
                            role: role.trim(),
                            access: "View",
                          }))
                          .filter((p) => p.role),
                      })
                    }
                  />
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
            </section>

            <section className="designer-layout-picker">
              <h3>Page purpose</h3>
              <p className="muted">
                Start from a guided template to pre-populate page type,
                components, toolbar, actions, columns, permissions, and endpoint
                metadata.
              </p>
              <div>
                {pagePurposeTemplates.map((template) => (
                  <Button
                    key={template.code}
                    variant={
                      selectedPurpose() === template.code
                        ? "primary"
                        : "secondary"
                    }
                    onClick={() => applyPagePurpose(template.code)}
                  >
                    {template.label}
                  </Button>
                ))}
              </div>
              <small className="muted">
                {pagePurposeTemplates.find((x) => x.code === selectedPurpose())
                  ?.description ||
                  "Choose the scenario that best describes how users should use this page."}
              </small>
            </section>
            <section className="designer-layout-picker">
              <h3>Layout</h3>
              <div>
                {layouts.map((l) => (
                  <Button
                    key={l.template}
                    variant={
                      form.layout.template === l.template
                        ? "primary"
                        : "secondary"
                    }
                    onClick={() =>
                      setForm({
                        ...form,
                        layout: {
                          ...form.layout,
                          template: l.template,
                          regions: l.regions,
                        },
                      })
                    }
                  >
                    {l.template}
                  </Button>
                ))}
              </div>
            </section>
            <div className="designer-canvas-surface">
              {regions.map((region) => (
                <section className="designer-region" key={region}>
                  <h3>{region}</h3>
                  {components
                    .filter((c) => c.region === region)
                    .sort(
                      (a, b) => (a.displayOrder || 0) - (b.displayOrder || 0),
                    )
                    .map((c) => (
                      <button
                        className={`designer-component-tile ${selectedComponent?.code === c.code ? "active" : ""}`}
                        key={c.code}
                        onClick={() => {
                          setSelectedCode(c.code);
                          setActivePanel("inspector");
                        }}
                      >
                        <strong>{c.name}</strong>
                        <span>{c.componentType}</span>
                      </button>
                    ))}
                  {!components.some((c) => c.region === region) && (
                    <EmptyState
                      title="Region ready"
                      message="Add components from the toolbox to compose this page region."
                    />
                  )}
                </section>
              ))}
            </div>
          </>
        )}
      </main>

      <aside
        className={`designer-inspector ${activePanel === "inspector" ? "active" : ""}`}
      >
        <h2>Properties</h2>
        {selectedComponent ? (
          <div className="form-grid one">
            <label>
              Title
              <Input
                value={
                  configOf(selectedComponent).title || selectedComponent.name
                }
                onChange={(e) =>
                  patchComponent(
                    { name: e.target.value },
                    { title: e.target.value },
                  )
                }
              />
            </label>
            <label>
              Datasource
              <Input
                value={
                  configOf(selectedComponent).datasource ||
                  form.datasource.entity
                }
                onChange={(e) =>
                  patchComponent({}, { datasource: e.target.value })
                }
              />
            </label>
            <label>
              Visibility Rule
              <Input
                value={configOf(selectedComponent).visibilityRule || ""}
                onChange={(e) =>
                  patchComponent({}, { visibilityRule: e.target.value })
                }
              />
            </label>
            <label>
              Width
              <Input
                value={configOf(selectedComponent).width || "12"}
                onChange={(e) => patchComponent({}, { width: e.target.value })}
              />
            </label>
            <label>
              Height
              <Input
                value={configOf(selectedComponent).height || "auto"}
                onChange={(e) => patchComponent({}, { height: e.target.value })}
              />
            </label>
            <label>
              CSS Class
              <Input
                value={configOf(selectedComponent).cssClass || ""}
                onChange={(e) =>
                  patchComponent({}, { cssClass: e.target.value })
                }
              />
            </label>
            <label>
              Permissions
              <Input
                value={configOf(selectedComponent).permissions || ""}
                onChange={(e) =>
                  patchComponent({}, { permissions: e.target.value })
                }
              />
            </label>
            <label>
              Expressions
              <textarea
                className="input"
                value={configOf(selectedComponent).expressions || ""}
                onChange={(e) =>
                  patchComponent({}, { expressions: e.target.value })
                }
              />
            </label>
          </div>
        ) : (
          <EmptyState
            title="Select a component"
            message="Choose a canvas component to edit metadata-backed properties."
          />
        )}
      </aside>

      <footer className="designer-diagnostics">
        <strong>Diagnostics</strong>
        <span>{components.length} components</span>
        <span>{regions.length} regions</span>
        <span>{form.datasource.entity || "No datasource"}</span>
        <span>{form.route}</span>
      </footer>
    </div>
  );
}
