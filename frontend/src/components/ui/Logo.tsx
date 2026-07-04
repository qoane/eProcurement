export function Logo({ compact = false }: { compact?: boolean }) {
  return (
    <div
      className={`logo ${compact ? "compact" : ""}`}
      aria-label="ProcuraFlow"
    >
      <span className="logo-mark" aria-hidden="true">
        <span />
      </span>
      {!compact && (
        <span className="logo-text">
          <strong>ProcuraFlow</strong>
          <small>Configured eProcurement</small>
        </span>
      )}
    </div>
  );
}
