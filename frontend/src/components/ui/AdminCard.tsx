import type { ReactNode } from "react";

export function AdminCard({
  title,
  subtitle,
  tools,
  footer,
  className = "",
  children,
}: {
  title: ReactNode;
  subtitle?: ReactNode;
  tools?: ReactNode;
  footer?: ReactNode;
  className?: string;
  children: ReactNode;
}) {
  return (
    <section className={`admin-card panel ${className}`}>
      <header className="admin-card-header">
        <div className="admin-card-title-group">
          <h2>{title}</h2>
          {subtitle && <p>{subtitle}</p>}
        </div>
        {tools && <div className="admin-card-tools">{tools}</div>}
      </header>
      <div className="admin-card-body">{children}</div>
      {footer && <footer className="admin-card-footer">{footer}</footer>}
    </section>
  );
}
