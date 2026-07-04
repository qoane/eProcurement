export function Timeline({
  items,
}: {
  items: { title: string; meta?: string; description?: string }[];
}) {
  return (
    <div className="timeline">
      {items.map((x, i) => (
        <div className="timeline-item" key={i}>
          <span className="dot" />
          <div>
            <strong>{x.title}</strong>
            {x.meta && <p className="muted">{x.meta}</p>}
            {x.description && <p>{x.description}</p>}
          </div>
        </div>
      ))}
    </div>
  );
}
