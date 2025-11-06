import React from "react";
import api from "../api/axios";

interface Props {
  users: any[];
  refresh: () => void;
  showToast: (msg: string, type?: "success" | "error") => void;
}

const UserTable: React.FC<Props> = ({ users, refresh, showToast }) => {
  const handleDelete = async (id: string) => {
    try {
      await api.delete(`/admin/user/${id}`);
      showToast("User deleted", "success");
      refresh();
    } catch {
      showToast("Delete failed", "error");
    }
  };

  return (
    <div className="overflow-x-auto">
      <table className="min-w-full bg-gray-800 rounded-md overflow-hidden">
        <thead className="bg-gray-700 text-gray-200">
          <tr>
            <th className="px-4 py-2 text-left">Username</th>
            <th className="px-4 py-2 text-left">Email</th>
            <th className="px-4 py-2">Actions</th>
          </tr>
        </thead>
        <tbody className="text-gray-100">
          {users.map((u) => (
            <tr key={u.id} className="border-b border-gray-700">
              <td className="px-4 py-2">{u.userName}</td>
              <td className="px-4 py-2">{u.email}</td>
              <td className="px-4 py-2 text-center">
                <button onClick={() => handleDelete(u.id)} className="bg-red-600 hover:bg-red-500 px-2 py-1 rounded-md text-black">
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default UserTable;
