export function WorkflowStageBar({ stages }: { stages: string[] }) {
  return (
    <div className="stagebar">
      {stages.map((s) => (
        <span className="stage" key={s}>
          {s}
        </span>
      ))}
    </div>
  );
}
