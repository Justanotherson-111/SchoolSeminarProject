// src/api/axios.ts
import axios, { AxiosHeaders, AxiosError } from "axios";
import type { InternalAxiosRequestConfig } from "axios";

const BASE = import.meta.env.VITE_BACKEND_URL || "/api";

// API instance and auth instance
const api = axios.create({
  baseURL: BASE,
  withCredentials: true,
  headers: { "Content-Type": "application/json" },
});
const authApi = axios.create({
  baseURL: BASE,
  withCredentials: true,
  headers: { "Content-Type": "application/json" },
});

// --- token helpers ---
export function getLocalAccessToken(): string | null {
  return localStorage.getItem("token");
}
export function setLocalAccessToken(token: string) {
  localStorage.setItem("token", token);
}
export function clearLocalAccessToken() {
  localStorage.removeItem("token");
  localStorage.removeItem("role");
}
export function logoutLocal() {
  clearLocalAccessToken();
  window.location.href = "/login";
}

// --- decode JWT to extract roles (best-effort) ---
function parseJwtPayload(token: string) {
  try {
    const payload = token.split(".")[1];
    // base64url decode
    const base64 = payload.replace(/-/g, "+").replace(/_/g, "/");
    const decoded = atob(base64);

    // convert binary string to UTF-8 string safely
    const jsonString = decodeURIComponent(
      decoded
        .split("")
        .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
        .join("")
    );

    return JSON.parse(jsonString);
  } catch {
    return null;
  }
}


export function extractRoleFromToken(token: string | null): string | null {
  if (!token) return null;
  const payload = parseJwtPayload(token);
  if (!payload) return null;

  // common claim names to check for role(s)
  const roleCandidates = [
    "role",
    "roles",
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/roles",
  ];

  for (const k of roleCandidates) {
    const v = payload[k];
    if (!v) continue;
    if (Array.isArray(v)) return v[0];
    if (typeof v === "string") return v;
  }

  // sometimes roles are embedded under "http://schemas..." keys as array of strings
  // if none found, return null
  return null;
}

// --- request interceptor: attach access token if present ---
api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = getLocalAccessToken();
  if (token) {
    if (!config.headers || !(config.headers instanceof AxiosHeaders)) {
      config.headers = new AxiosHeaders(config.headers);
    }
    (config.headers as AxiosHeaders).set("Authorization", `Bearer ${token}`);
  }
  return config;
});

// --- response interceptor: handle 401 + refresh ---
let isRefreshing = false;
type FailedRequest = {
  resolve: (value?: unknown) => void;
  reject: (error: unknown) => void;
  config: InternalAxiosRequestConfig;
};
const failedQueue: FailedRequest[] = [];

const processQueue = (error: unknown = null, token: string | null = null) => {
  failedQueue.forEach((item) => {
    if (error) {
      item.reject(error);
    } else {
      if (token) {
        if (!item.config.headers || !(item.config.headers instanceof AxiosHeaders)) {
          item.config.headers = new AxiosHeaders(item.config.headers);
        }
        (item.config.headers as AxiosHeaders).set("Authorization", `Bearer ${token}`);
      }
      item.resolve(api(item.config));
    }
  });
  failedQueue.length = 0;
};

api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError & { config?: InternalAxiosRequestConfig }) => {
    const originalRequest = error.config;

    if (!error.response || error.response.status !== 401) {
      return Promise.reject(error);
    }

    // prevent infinite retry
    if (!originalRequest || (originalRequest as any)._retry) {
      logoutLocal();
      return Promise.reject(error);
    }
    (originalRequest as any)._retry = true;

    return new Promise((resolve, reject) => {
      failedQueue.push({ resolve, reject, config: originalRequest });

      if (!isRefreshing) {
        isRefreshing = true;
        authApi
          .post("/auth/refresh")
          .then((res) => {
            // backend may return accessToken or token
            const newToken = res?.data?.accessToken ?? res?.data?.token;
            if (!newToken) throw new Error("No access token returned during refresh");

            setLocalAccessToken(newToken);

            // optionally set role if it's encoded in token
            const role = extractRoleFromToken(newToken);
            if (role) localStorage.setItem("role", role);

            processQueue(null, newToken);
          })
          .catch((err) => {
            processQueue(err, null);
            logoutLocal();
          })
          .finally(() => {
            isRefreshing = false;
          });
      }
    });
  }
);

export default api;
