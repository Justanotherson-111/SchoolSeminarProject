import React from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import ProtectedRoute from "./components/ProtectedRoute";

import Dashboard from "./pages/Dashboard";
import Image from "./pages/Image";
import ExtractedText from "./pages/ExtractedText";
import AdminPanel from "./pages/AdminPanel";
import Profile from "./pages/Profile";
import Login from "./pages/Login";
import Register from "./pages/Register";
import NotFound from "./pages/NotFound";
import Layout from "./components/Layout";
import ToastContainer from "./components/Toast";
import useToast from "./utils/useToast";

const App: React.FC = () => {
  const toast = useToast();

  return (
    <BrowserRouter>
      {/* Toast Notifications */}
      {toast.toasts.length > 0 && (
        <ToastContainer toasts={toast.toasts} onRemove={toast.remove} />
      )}

      <Routes>
        {/* Root redirect */}
        <Route path="/" element={<Navigate to="/dashboard" replace />} />

        {/* Public pages */}
        <Route path="/login" element={<Login showToast={toast.showToast} />} />
        <Route path="/register" element={<Register showToast={toast.showToast} />} />

        {/* Protected pages */}
        <Route
          path="/dashboard"
          element={
            <ProtectedRoute>
              <Layout>
                <Dashboard />
              </Layout>
            </ProtectedRoute>
          }
        />

        <Route
          path="/upload"
          element={
            <ProtectedRoute>
              <Layout>
                <Image />
              </Layout>
            </ProtectedRoute>
          }
        />

        <Route
          path="/files"
          element={
            <ProtectedRoute>
              <Layout>
                <ExtractedText />
              </Layout>
            </ProtectedRoute>
          }
        />

        <Route
          path="/profile"
          element={
            <ProtectedRoute>
              <Layout>
                <Profile />
              </Layout>
            </ProtectedRoute>
          }
        />

        {/* Admin-only page */}
        <Route
          path="/admin"
          element={
            <ProtectedRoute requiredRole="Admin">
              <Layout>
                <AdminPanel />
              </Layout>
            </ProtectedRoute>
          }
        />

        {/* 404 Page */}
        <Route
          path="*"
          element={
            <Layout>
              <NotFound />
            </Layout>
          }
        />
      </Routes>
    </BrowserRouter>
  );
};

export default App;
