import React from "react";
import type { ReactElement } from "react";
import { Navigate } from "react-router-dom";

interface ProtectedRouteProps {
  children: ReactElement; // use the type
  roles?: string[]; // e.g. ["Admin"]
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children, roles }) => {
  const token = localStorage.getItem("token");
  const userRole = localStorage.getItem("role");

  if (!token) return <Navigate to="/login" replace />;
  if (roles && !roles.includes(userRole || "")) return <Navigate to="/dashboard" replace />;

  return children;
};

export default ProtectedRoute;
