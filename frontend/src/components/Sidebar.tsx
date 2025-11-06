import React from "react";
import { NavLink } from "react-router-dom";
import { getUserRole } from "../utils/auth";

const Sidebar: React.FC = () => {
  const role = getUserRole();

  return (
    <div className="w-64 bg-gray-900 min-h-screen p-4 flex flex-col gap-4">
      <NavLink to="/dashboard" className={({ isActive }) => isActive ? "text-emerald-400 font-bold" : "text-gray-300"}>
        Dashboard
      </NavLink>
      <NavLink to="/upload" className={({ isActive }) => isActive ? "text-emerald-400 font-bold" : "text-gray-300"}>
        Upload Images
      </NavLink>
      <NavLink to="/files" className={({ isActive }) => isActive ? "text-emerald-400 font-bold" : "text-gray-300"}>
        Extracted Text
      </NavLink>
      <NavLink to="/profile" className={({ isActive }) => isActive ? "text-emerald-400 font-bold" : "text-gray-300"}>
        Profile
      </NavLink>
      {role === "Admin" && (
        <NavLink to="/admin" className={({ isActive }) => isActive ? "text-emerald-400 font-bold" : "text-gray-300"}>
          Admin Panel
        </NavLink>
      )}
    </div>
  );
};

export default Sidebar;
