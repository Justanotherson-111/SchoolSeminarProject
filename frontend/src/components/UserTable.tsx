import React, { useEffect, useState } from "react";
import api from "../api/axios";

interface User { id: string; userName?: string; email?: string; fullName?: string; }

const UserTable: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    (async () => {
      try {
        const res = await api.get("/admin/users");
        setUsers(res.data ?? []);
      } catch (err) {
        console.error(err);
        alert("Failed to load users or unauthorized");
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  const handleDelete = async (id: string) => {
    if (!confirm("Delete user?")) return;
    try {
      await api.delete(`/admin/users/${id}`);
      setUsers(prev => prev.filter(u => u.id !== id));
    } catch (err) {
      console.error(err);
      alert("Delete failed");
    }
  };

  if (loading) return <div className="p-4 text-gray-400">Loading users...</div>;

  return (
    <div className="overflow-x-auto">
      <table className="w-full text-left border-collapse">
        <thead className="text-xs text-gray-400">
          <tr>
            <th className="p-2">ID</th>
            <th className="p-2">Username</th>
            <th className="p-2">Email</th>
            <th className="p-2">Full name</th>
            <th className="p-2">Actions</th>
          </tr>
        </thead>
        <tbody>
          {users.map(u => (
            <tr key={u.id} className="border-y border-black/20">
              <td className="p-2 text-sm text-gray-200">{u.id}</td>
              <td className="p-2 text-sm text-gray-200">{u.userName}</td>
              <td className="p-2 text-sm text-gray-200">{u.email}</td>
              <td className="p-2 text-sm text-gray-200">{u.fullName}</td>
              <td className="p-2">
                <button onClick={() => handleDelete(u.id)} className="px-2 py-1 rounded bg-red-600 text-white">Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default UserTable;
