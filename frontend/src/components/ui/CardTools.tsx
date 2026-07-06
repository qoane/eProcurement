import type { ReactNode } from "react";

export function CardTools({
  children,
  className = "",
}: {
  children: ReactNode;
  className?: string;
}) {
  return <div className={`card-tools ${className}`}>{children}</div>;
}

export function CardToolLink({
  href = "#",
  children,
  external = false,
}: {
  href?: string;
  children: ReactNode;
  external?: boolean;
}) {
  return (
    <a className="card-tool-link" href={href} aria-label={String(children)}>
      {children}
      {external && <span aria-hidden="true">↗</span>}
    </a>
  );
}
