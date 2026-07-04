import { Input } from "../ui/Input";
export function FieldRenderer({
  label,
  required,
}: {
  label: string;
  required?: boolean;
}) {
  return (
    <label>
      {label}
      {required && " *"}
      <Input aria-label={label} />
    </label>
  );
}
