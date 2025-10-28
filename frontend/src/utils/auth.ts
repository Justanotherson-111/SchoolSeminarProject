// src/utils/auth.ts
import api, {
  getLocalAccessToken,
  setLocalAccessToken,
  clearLocalAccessToken,
  extractRoleFromToken,
  logoutLocal,
} from "../api/axios";

export interface User {
  id: string;
  username: string;
  email: string;
  role: string;
}

// 🧠 Get current user info from backend
export async function fetchUserProfile(): Promise<User | null> {
  try {
    const res = await api.get("/auth/profile");
    return res.data as User;
  } catch {
    return null;
  }
}

// 🧾 Login user and store token + role
export async function login(username: string, password: string): Promise<boolean> {
  try {
    const res = await api.post("/auth/login", { username, password });
    const token = res.data?.accessToken ?? res.data?.token;
    if (!token) throw new Error("No token received");

    setLocalAccessToken(token);

    // extract role for local use
    const role = extractRoleFromToken(token);
    if (role) localStorage.setItem("role", role);

    return true;
  } catch (err) {
    console.error("Login failed:", err);
    return false;
  }
}

// 🧍‍♂️ Logout user and redirect
export function logout() {
  clearLocalAccessToken();
  logoutLocal();
}

// 🧩 Check authentication
export function isAuthenticated(): boolean {
  const token = getLocalAccessToken();
  return !!token;
}

// 🛡️ Check if admin
export function isAdmin(): boolean {
  const role = localStorage.getItem("role");
  return role?.toLowerCase() === "admin";
}
