import type { ReactNode } from "react";

type SmallBoxVariant =
  "primary" | "success" | "warning" | "info" | "danger" | "neutral";

export function SmallBox({
  label,
  value,
  icon,
  footer,
  variant = "primary",
  className = "",
}: {
  label: ReactNode;
  value: ReactNode;
  icon?: ReactNode;
  footer?: ReactNode;
  variant?: SmallBoxVariant;
  className?: string;
}) {
  return (
    <article className={`small-box ${variant} ${className}`}>
      <div className="small-box-content">
        <strong>{value}</strong>
        <span>{label}</span>
      </div>
      {icon && (
        <div className="small-box-icon" aria-hidden="true">
          {icon}
        </div>
      )}
      {footer && <div className="small-box-footer">{footer}</div>}
    </article>
  );
}
