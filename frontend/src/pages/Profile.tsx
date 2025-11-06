import React, { useEffect, useState } from "react";
import { clearTokens, getUserRole } from "../utils/auth";
import api from "../api/axios";
import useToast from "../utils/useToast";
import { useNavigate } from "react-router-dom";

interface User {
  id: string;
  userName: string;
  email: string;
  role: string;
}

const Profile: React.FC = () => {
  const [user, setUser] = useState<User | null>(null);
  const toast = useToast();
  const nav = useNavigate();

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const { data } = await api.get("/admin/users"); // backend can add /me endpoint ideally
        const me = data.find((u: User) => u.role === getUserRole()); // fallback
        setUser(me);
      } catch {
        toast.showToast("Failed to fetch profile.", "error");
      }
    };
    fetchProfile();
  }, []);

  const handleLogout = () => {
    clearTokens();
    toast.showToast("Logged out.", "info");
    nav("/login");
  };

  if (!user) return <p>Loading profile...</p>;

  return (
    <div>
      <h1 className="text-2xl font-bold text-emerald-400 mb-4">Profile</h1>
      <p>
        <strong>Username:</strong> {user.userName}
      </p>
      <p>
        <strong>Email:</strong> {user.email}
      </p>
      <p>
        <strong>Role:</strong> {user.role}
      </p>
      <button
        onClick={handleLogout}
        className="mt-4 px-4 py-2 bg-red-600 text-white rounded hover:bg-red-500"
      >
        Logout
      </button>
    </div>
  );
};

export default Profile;
