// Login.tsx
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api/axios";
import type { ToastType } from "../utils/useToast";

interface Props {
  showToast: (message: string, type?: ToastType) => void;
}

const Login: React.FC<Props> = ({ showToast }) => {
  const [form, setForm] = useState({ email: "", password: "" });
  const [busy, setBusy] = useState(false);
  const nav = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setBusy(true);
    try {
      const { data } = await api.post("/auth/login", {
        Email: form.email,
        Password: form.password,
      });

      localStorage.setItem("access_token", data.accessToken);
      localStorage.setItem("refresh_token", data.refreshToken);
      showToast("Login successful", "success");

      nav("/dashboard");
    } catch (err) {
      console.error(err);
      showToast("Login failed", "error");
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center px-4">
      <div className="w-full max-w-md bg-black/60 border border-black/30 rounded-2xl p-8 backdrop-blur-md">
        <h1 className="text-2xl font-bold text-emerald-300 mb-4 text-center">Login</h1>
        <form onSubmit={handleSubmit} className="space-y-3">
          <input
            name="email"
            placeholder="Email"
            type="email"
            onChange={handleChange}
            className="w-full p-3 rounded-md bg-gray-900 border border-gray-800 text-gray-100"
            required
          />
          <input
            name="password"
            placeholder="Password"
            type="password"
            onChange={handleChange}
            className="w-full p-3 rounded-md bg-gray-900 border border-gray-800 text-gray-100"
            required
          />
          <button
            type="submit"
            disabled={busy}
            className="w-full py-2 rounded-md bg-emerald-500 hover:bg-emerald-400 text-black font-medium"
          >
            {busy ? "Logging in..." : "Login"}
          </button>
        </form>
        <div className="mt-4 text-center text-sm text-gray-400">
          <a className="text-emerald-300" href="/register">Create account →</a>
        </div>
      </div>
    </div>
  );
};

export default Login;
