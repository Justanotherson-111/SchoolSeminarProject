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
      <ToastContainer toasts={toast.toasts} onRemove={toast.remove} />

      {/* Main Routes */}
      <Routes>
        {/* Redirect root to dashboard */}
        <Route path="/" element={<Navigate to="/dashboard" replace />} />

        {/* Public Routes */}
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        {/* Protected Routes */}
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
          path="/admin"
          element={
            <ProtectedRoute roles={["Admin"]}>
              <Layout>
                <AdminPanel />
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
