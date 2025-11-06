import React, { type JSX } from "react";
import { Navigate } from "react-router-dom";
import { isAuthenticated, getUserRole } from "../utils/auth";

interface Props {
  children: JSX.Element;
  requiredRole?: string; // optional role check
}

const ProtectedRoute: React.FC<Props> = ({ children, requiredRole }) => {
  if (!isAuthenticated()) {
    return <Navigate to="/login" replace />;
  }

  if (requiredRole && getUserRole() !== requiredRole) {
    return <Navigate to="/" replace />; // redirect if role mismatch
  }

  return children;
};

export default ProtectedRoute;
