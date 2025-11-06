import React, { useEffect, useState } from "react";
import api from "../api/axios";
import useToast from "../utils/useToast";

interface User {
  id: string;
  userName: string;
  email: string;
}

const Dashboard: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(false);
  const toast = useToast();

  useEffect(() => {
    const fetchUsers = async () => {
      setLoading(true);
      try {
        const { data } = await api.get("/admin/users");
        setUsers(data);
      } catch (err) {
        toast.showToast("Failed to fetch users.", "error");
      } finally {
        setLoading(false);
      }
    };
    fetchUsers();
  }, []);

  return (
    <div>
      <h1 className="text-2xl font-bold text-emerald-400 mb-4">Dashboard</h1>
      {loading ? (
        <p>Loading...</p>
      ) : (
        <div className="overflow-x-auto">
          <table className="min-w-full bg-gray-800 text-gray-100 rounded-md overflow-hidden">
            <thead className="bg-gray-900">
              <tr>
                <th className="py-2 px-4">Username</th>
                <th className="py-2 px-4">Email</th>
              </tr>
            </thead>
            <tbody>
              {users.map((u) => (
                <tr key={u.id} className="border-b border-gray-700">
                  <td className="py-2 px-4">{u.userName}</td>
                  <td className="py-2 px-4">{u.email}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default Dashboard;
