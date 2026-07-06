import type { ReactNode } from "react";

type InfoBoxVariant =
  "primary" | "success" | "warning" | "info" | "danger" | "neutral";

export function InfoBox({
  icon,
  label,
  value,
  trend,
  variant = "primary",
  className = "",
}: {
  icon?: ReactNode;
  label: ReactNode;
  value: ReactNode;
  trend?: ReactNode;
  variant?: InfoBoxVariant;
  className?: string;
}) {
  return (
    <div className={`info-box ${variant} ${className}`}>
      {icon && (
        <div className="info-box-icon" aria-hidden="true">
          {icon}
        </div>
      )}
      <div className="info-box-content">
        <span className="info-box-label">{label}</span>
        <strong className="info-box-value">{value}</strong>
        {trend && <span className="info-box-trend">{trend}</span>}
      </div>
    </div>
  );
}
