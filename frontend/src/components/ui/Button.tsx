import type { ButtonHTMLAttributes, ReactNode } from "react";
export function Button({
  variant = "primary",
  children,
  ...p
}: ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: "primary" | "secondary";
  children: ReactNode;
}) {
  return (
    <button className={`btn ${variant}`} {...p}>
      {children}
    </button>
  );
}
