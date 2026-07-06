export function Badge({
  children,
  tone = "default",
}: {
  children: React.ReactNode;
  tone?: "default" | "success" | "warning" | "danger" | "info";
}) {
  return <span className={`badge ${tone}`}>{children}</span>;
}
const workflowStatusByValue: Record<number, string> = {
  0: "Draft",
  1: "Published",
  2: "Archived",
};

function statusLabel(status: unknown) {
  if (typeof status === "string" && status.trim()) return status;
  if (typeof status === "number") return workflowStatusByValue[status] ?? String(status);
  if (typeof status === "boolean") return status ? "Active" : "Inactive";
  return "Not configured";
}

export function StatusBadge({ status }: { status?: unknown }) {
  const label = statusLabel(status);
  const s = label.toLowerCase();
  const tone =
    s.includes("approved") ||
    s.includes("complete") ||
    s.includes("active") ||
    s.includes("published")
      ? "success"
      : s.includes("pending") ||
          s.includes("draft") ||
          s.includes("verification")
        ? "warning"
        : s.includes("reject") || s.includes("fail")
          ? "danger"
          : "info";
  return <Badge tone={tone}>{label}</Badge>;
}
