export function Tabs({ items }: { items: string[] }) {
  return (
    <div className="tabs">
      {items.map((x, i) => (
        <span className={`tab ${i === 0 ? "active" : ""}`} key={x}>
          {x}
        </span>
      ))}
    </div>
  );
}
