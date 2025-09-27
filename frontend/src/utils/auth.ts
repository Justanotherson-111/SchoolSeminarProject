export function getToken() {
    return localStorage.getItem("token");
}

export function getUserRole() {
    return localStorage.getItem("role");
}

export function saveAuth(token: string, role: string) {
    localStorage.setItem("token", token);
    localStorage.setItem("role", role);
}

export function clearAuth() {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
}
