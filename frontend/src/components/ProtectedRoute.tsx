import React from "react";
import { Navigate } from "react-router-dom";

interface Props {
  roles?: string[];
  children: React.ReactNode;
}

const ProtectedRoute: React.FC<Props> = ({ roles, children }) => {
  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role");

  if (!token) {
    // Not logged in → redirect to login
    return <Navigate to="/login" replace />;
  }

  if (roles && !roles.includes(role ?? "")) {
    // Logged in but role not allowed → block page
    return (
      <div className="flex items-center justify-center min-h-screen text-gray-200">
        <div className="text-center">
          <h1 className="text-3xl font-bold mb-2">Access Denied</h1>
          <p className="text-gray-400 mb-4">
            You do not have permission to view this page.
          </p>
          <button
            onClick={() => window.history.back()}
            className="px-4 py-2 rounded-md bg-emerald-600 hover:bg-emerald-500 text-black"
          >
            Go Back
          </button>
        </div>
      </div>
    );
  }

  return <>{children}</>;
};

export default ProtectedRoute;
