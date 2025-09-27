import React from "react";
import ProtectedRoute from "../components/ProtectedRoute";
import UserTable from "../components/UserTable";

const AdminPanel: React.FC = () => {
  return (
    <ProtectedRoute roles={["Admin"]}>
      <div className="p-4">
        <h1 className="text-xl font-bold mb-4">Admin Panel</h1>
        <UserTable />
      </div>
    </ProtectedRoute>
  );
};

export default AdminPanel;
