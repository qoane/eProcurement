import type { ReactNode } from "react";
export function Panel({
  title,
  children,
  actions,
}: {
  title?: string;
  children: ReactNode;
  actions?: ReactNode;
}) {
  return (
    <section className="panel">
      {title && (
        <div className="page-header">
          <div>
            <h2>{title}</h2>
          </div>
          {actions}
        </div>
      )}
      {children}
    </section>
  );
}
