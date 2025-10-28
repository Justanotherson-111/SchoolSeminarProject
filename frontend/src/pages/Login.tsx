import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import api, { setLocalAccessToken, extractRoleFromToken } from "../api/axios";

const Login: React.FC = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [busy, setBusy] = useState(false);
  const nav = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setBusy(true);
    try {
      const res = await api.post("/auth/login", { UserName: username, Password: password });
      const token = res?.data?.accessToken ?? res?.data?.token;
      if (!token) throw new Error("No token received");
      setLocalAccessToken(token);
      const role = extractRoleFromToken(token);
      if (role) localStorage.setItem("role", role);
      nav("/dashboard");
    } catch (err) {
      console.error(err);
      alert("Login failed. Check credentials.");
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center px-4">
      <div className="w-full max-w-md bg-black/60 border border-black/30 rounded-2xl p-8 backdrop-blur-md">
        <h1 className="text-2xl font-bold text-emerald-300 mb-4 text-center">Sign in</h1>

        <form onSubmit={handleLogin} className="space-y-4">
          <input value={username} onChange={e => setUsername(e.target.value)} required placeholder="Email or username"
                 className="w-full p-3 rounded-md bg-gray-900 border border-gray-800 text-gray-100" />
          <input value={password} onChange={e => setPassword(e.target.value)} required type="password" placeholder="Password"
                 className="w-full p-3 rounded-md bg-gray-900 border border-gray-800 text-gray-100" />
          <button type="submit" disabled={busy}
                  className="w-full py-2 rounded-md bg-emerald-500 hover:bg-emerald-400 text-black font-medium">
            {busy ? "Signing in..." : "Sign in"}
          </button>
        </form>

        <div className="mt-4 text-center text-sm text-gray-400">
          Don’t have an account? <Link to="/register" className="text-emerald-300">Register</Link>
        </div>
      </div>
    </div>
  );
};

export default Login;
