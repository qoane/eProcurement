type FieldHintProps = {
  children: React.ReactNode;
  tone?: "info" | "warning";
};

export function FieldHint({ children, tone = "info" }: FieldHintProps) {
  return <small className={`field-hint ${tone}`}>{children}</small>;
}
