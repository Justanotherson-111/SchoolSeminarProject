import React from "react";
import UserTable from "../components/UserTable";

const AdminPanel: React.FC = () => {
  return (
    <div className="card max-w-4xl mx-auto">
      <h1 className="text-2xl font-semibold mb-4">Admin Panel</h1>
      <UserTable />
    </div>
  );
};

export default AdminPanel;
