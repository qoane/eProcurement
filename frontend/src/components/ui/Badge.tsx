export function Badge({
  children,
  tone = "default",
}: {
  children: React.ReactNode;
  tone?: "default" | "success" | "warning" | "danger" | "info";
}) {
  return <span className={`badge ${tone}`}>{children}</span>;
}
export function StatusBadge({ status }: { status?: string }) {
  const s = (status || "Not configured").toLowerCase();
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
  return <Badge tone={tone}>{status || "Not configured"}</Badge>;
}
