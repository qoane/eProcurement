import { useEffect, useState } from "react";
import { PageHeader } from "../../components/ui/PageHeader";
import { EmptyState } from "../../components/ui/EmptyState";
import { Button } from "../../components/ui/Button";
import { Input } from "../../components/ui/Input";
import { getActiveForm } from "../../services/formsApi";
export function SupplierRegistrationPage() {
  const [has, setHas] = useState(false);
  useEffect(() => {
    void getActiveForm("SUPPLIER_REGISTRATION").then((r) => setHas(!!r.data));
  }, []);
  return (
    <>
      <PageHeader
        title="Supplier registration"
        description="Professional registration workspace backed by dynamic forms when configured."
      />
      {has ? (
        <form className="panel form-grid">
          <label>
            Legal name
            <Input />
          </label>
          <label>
            Trading name
            <Input />
          </label>
          <label>
            Tax number
            <Input />
          </label>
          <label>
            Contact email
            <Input type="email" />
          </label>
          <Button>Save registration</Button>
        </form>
      ) : (
        <EmptyState
          title="No active supplier registration form"
          message="Publish a SUPPLIER_REGISTRATION dynamic form to render configured fields here."
        />
      )}
    </>
  );
}
