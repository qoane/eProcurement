import { Timeline } from "../ui/Timeline";
export function WorkflowTimeline({
  transitions,
}: {
  transitions: {
    fromNodeCode: string;
    actionName: string;
    toNodeCode: string;
  }[];
}) {
  return (
    <Timeline
      items={transitions.map((t) => ({
        title: t.actionName,
        meta: `${t.fromNodeCode} → ${t.toNodeCode}`,
      }))}
    />
  );
}
