import { Input } from "../ui/Input";
export function FieldRenderer({
  label,
  required,
  value,
  onChange,
}: {
  label: string;
  required?: boolean;
  value?: string;
  onChange?: (value: string) => void;
}) {
  return (
    <label>
      {label}
      {required && " *"}
      <Input
        aria-label={label}
        value={value}
        onChange={(event) => onChange?.(event.target.value)}
      />
    </label>
  );
}
