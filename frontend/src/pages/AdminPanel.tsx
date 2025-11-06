import React, { useEffect, useState } from "react";
import api from "../api/axios";
import useToast from "../utils/useToast";

interface User {
  id: string;
  userName: string;
  email: string;
}

const AdminPanel: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(false);
  const toast = useToast();

  const fetchUsers = async () => {
    setLoading(true);
    try {
      const { data } = await api.get("/admin/users");
      setUsers(data);
    } catch {
      toast.showToast("Failed to fetch users.", "error");
    } finally {
      setLoading(false);
    }
  };

  const deleteUser = async (id: string) => {
    if (!confirm("Are you sure you want to delete this user?")) return;
    try {
      await api.delete(`/admin/user/${id}`);
      toast.showToast("User deleted.", "success");
      setUsers(users.filter((u) => u.id !== id));
    } catch {
      toast.showToast("Failed to delete user.", "error");
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  return (
    <div>
      <h1 className="text-2xl font-bold text-emerald-400 mb-4">Admin Panel</h1>
      {loading ? (
        <p>Loading...</p>
      ) : (
        <div className="overflow-x-auto">
          <table className="min-w-full bg-gray-800 text-gray-100 rounded-md overflow-hidden">
            <thead className="bg-gray-900">
              <tr>
                <th className="py-2 px-4">Username</th>
                <th className="py-2 px-4">Email</th>
                <th className="py-2 px-4">Actions</th>
              </tr>
            </thead>
            <tbody>
              {users.map((u) => (
                <tr key={u.id} className="border-b border-gray-700">
                  <td className="py-2 px-4">{u.userName}</td>
                  <td className="py-2 px-4">{u.email}</td>
                  <td className="py-2 px-4">
                    <button
                      onClick={() => deleteUser(u.id)}
                      className="px-2 py-1 bg-red-600 rounded hover:bg-red-500"
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default AdminPanel;
