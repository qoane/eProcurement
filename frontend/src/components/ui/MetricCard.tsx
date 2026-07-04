import { Card } from "./Card";
export function MetricCard({
  label,
  value,
  meta,
}: {
  label: string;
  value: number | string;
  meta?: string;
}) {
  return (
    <Card className="metric">
      <div>
        <span className="muted">{label}</span>
        <strong>{value}</strong>
        {meta && <p className="muted">{meta}</p>}
      </div>
    </Card>
  );
}
