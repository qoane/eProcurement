import { useMemo, useState } from "react";

export type Column<T> = {
  header: string;
  cell: (row: T) => React.ReactNode;
  sortable?: boolean;
  filterable?: boolean;
  className?: string;
  width?: string;
};

type DataTableProps<T> = {
  rows: T[];
  columns: Column<T>[];
  empty?: string;
  searchable?: boolean;
  pageSize?: number;
  toolbarActions?: React.ReactNode;
  striped?: boolean;
  compact?: boolean;
};

type SortState = { header: string; direction: "asc" | "desc" } | null;

function getCellText(value: React.ReactNode): string {
  if (value === null || value === undefined || typeof value === "boolean")
    return "";
  if (
    typeof value === "string" ||
    typeof value === "number" ||
    typeof value === "bigint"
  ) {
    return String(value);
  }
  if (Array.isArray(value)) return value.map(getCellText).join(" ");
  if (typeof value === "object" && "props" in value) {
    const props = (value as { props?: { children?: React.ReactNode } }).props;
    return getCellText(props?.children);
  }
  return "";
}

export function DataTable<T>({
  rows,
  columns,
  empty = "No records returned by the API.",
  searchable = false,
  pageSize,
  toolbarActions,
  striped = false,
  compact = false,
}: DataTableProps<T>) {
  const [query, setQuery] = useState("");
  const [sort, setSort] = useState<SortState>(null);
  const [page, setPage] = useState(1);
  const hasToolbar = searchable || Boolean(toolbarActions);
  const safePageSize = pageSize && pageSize > 0 ? pageSize : undefined;

  const filteredRows = useMemo(() => {
    if (!searchable || !query.trim()) return rows;
    const normalizedQuery = query.trim().toLowerCase();
    const searchableColumns = columns.filter(
      (column) => column.filterable !== false,
    );
    return rows.filter((row) =>
      searchableColumns.some((column) =>
        getCellText(column.cell(row)).toLowerCase().includes(normalizedQuery),
      ),
    );
  }, [columns, query, rows, searchable]);

  const sortedRows = useMemo(() => {
    if (!sort) return filteredRows;
    const column = columns.find(
      (candidate) => candidate.header === sort.header,
    );
    if (!column) return filteredRows;
    return [...filteredRows].sort((a, b) => {
      const left = getCellText(column.cell(a)).toLowerCase();
      const right = getCellText(column.cell(b)).toLowerCase();
      const result = left.localeCompare(right, undefined, {
        numeric: true,
        sensitivity: "base",
      });
      return sort.direction === "asc" ? result : -result;
    });
  }, [columns, filteredRows, sort]);

  const totalPages = safePageSize
    ? Math.max(1, Math.ceil(sortedRows.length / safePageSize))
    : 1;
  const currentPage = Math.min(page, totalPages);
  const visibleRows = safePageSize
    ? sortedRows.slice(
        (currentPage - 1) * safePageSize,
        currentPage * safePageSize,
      )
    : sortedRows;

  function toggleSort(column: Column<T>) {
    if (!column.sortable) return;
    setPage(1);
    setSort((current) => {
      if (current?.header !== column.header)
        return { header: column.header, direction: "asc" };
      if (current.direction === "asc")
        return { header: column.header, direction: "desc" };
      return null;
    });
  }

  function updateQuery(value: string) {
    setQuery(value);
    setPage(1);
  }

  return (
    <div
      className={`data-table${striped ? " data-table-striped" : ""}${compact ? " data-table-compact" : ""}`}
    >
      {hasToolbar && (
        <div className="data-table-toolbar">
          {searchable && (
            <label className="data-table-search">
              <span aria-hidden="true">⌕</span>
              <input
                aria-label="Search table"
                placeholder="Search table…"
                value={query}
                onChange={(event) => updateQuery(event.target.value)}
              />
            </label>
          )}
          {toolbarActions && (
            <div className="data-table-actions">{toolbarActions}</div>
          )}
        </div>
      )}
      {!rows.length ? (
        <div className="empty">{empty}</div>
      ) : (
        <>
          <div className="data-table-scroll">
            <table>
              <thead>
                <tr>
                  {columns.map((c) => (
                    <th
                      key={c.header}
                      className={c.className}
                      style={{ width: c.width }}
                    >
                      {c.sortable ? (
                        <button
                          className="data-table-sort"
                          type="button"
                          onClick={() => toggleSort(c)}
                        >
                          {c.header}
                          <span aria-hidden="true">
                            {sort?.header === c.header
                              ? sort.direction === "asc"
                                ? "▲"
                                : "▼"
                              : "↕"}
                          </span>
                        </button>
                      ) : (
                        c.header
                      )}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {visibleRows.map((r, i) => (
                  <tr key={i}>
                    {columns.map((c) => (
                      <td key={c.header} className={c.className}>
                        {c.cell(r)}
                      </td>
                    ))}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          {!visibleRows.length && (
            <div className="empty">No records match your search.</div>
          )}
          {safePageSize && totalPages > 1 && (
            <div className="data-table-footer">
              <span>
                Showing {(currentPage - 1) * safePageSize + 1}-
                {Math.min(currentPage * safePageSize, sortedRows.length)} of{" "}
                {sortedRows.length}
              </span>
              <div
                className="data-table-pagination"
                aria-label="Table pagination"
              >
                <button
                  type="button"
                  onClick={() => setPage((p) => Math.max(1, p - 1))}
                  disabled={currentPage === 1}
                >
                  Prev
                </button>
                {Array.from(
                  { length: totalPages },
                  (_, index) => index + 1,
                ).map((pageNumber) => (
                  <button
                    key={pageNumber}
                    type="button"
                    className={
                      pageNumber === currentPage ? "active" : undefined
                    }
                    onClick={() => setPage(pageNumber)}
                  >
                    {pageNumber}
                  </button>
                ))}
                <button
                  type="button"
                  onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                  disabled={currentPage === totalPages}
                >
                  Next
                </button>
              </div>
            </div>
          )}
        </>
      )}
    </div>
  );
}
