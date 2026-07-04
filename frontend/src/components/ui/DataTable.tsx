export type Column<T> = { header: string; cell: (row: T) => React.ReactNode };
export function DataTable<T>({
  rows,
  columns,
  empty = "No records returned by the API.",
}: {
  rows: T[];
  columns: Column<T>[];
  empty?: string;
}) {
  if (!rows.length) return <div className="empty">{empty}</div>;
  return (
    <div style={{ overflowX: "auto" }}>
      <table>
        <thead>
          <tr>
            {columns.map((c) => (
              <th key={c.header}>{c.header}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {rows.map((r, i) => (
            <tr key={i}>
              {columns.map((c) => (
                <td key={c.header}>{c.cell(r)}</td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
