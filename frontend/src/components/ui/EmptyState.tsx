export function EmptyState({
  title = "Nothing configured yet",
  message,
}: {
  title?: string;
  message: string;
}) {
  return (
    <div className="empty">
      <strong>{title}</strong>
      <p>{message}</p>
    </div>
  );
}
