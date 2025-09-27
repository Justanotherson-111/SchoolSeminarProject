import React from "react";
import { Link } from "react-router-dom";

interface SidebarProps {
  active?: string;
}

const Sidebar: React.FC<SidebarProps> = ({ active }) => {
  const userRole = localStorage.getItem("role");

  return (
    <aside className="bg-gray-100 w-64 p-4 hidden md:block">
      <ul className="space-y-2">
        <li className={active === "dashboard" ? "font-bold" : ""}>
          <Link to="/dashboard">Dashboard</Link>
        </li>
        <li className={active === "myfiles" ? "font-bold" : ""}>
          <Link to="/my-files">My Files</Link>
        </li>
        <li className={active === "upload" ? "font-bold" : ""}>
          <Link to="/upload">Upload</Link>
        </li>
        {userRole === "Admin" && (
          <li className={active === "admin" ? "font-bold" : ""}>
            <Link to="/admin">Admin Panel</Link>
          </li>
        )}
      </ul>
    </aside>
  );
};

export default Sidebar;
