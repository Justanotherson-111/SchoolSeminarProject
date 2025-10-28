import React from "react";
import { Link, useLocation } from "react-router-dom";
import { isAdmin } from "../utils/auth";
import { motion } from "framer-motion";
import {
  Home,
  Upload,
  FileText,
  Shield,
  User,
  LogOut,
} from "lucide-react";

const Sidebar: React.FC = () => {
  const role = isAdmin();
  const location = useLocation();

  const navItems = [
    { name: "Dashboard", icon: Home, path: "/dashboard" },
    { name: "Image Upload", icon: Upload, path: "/image-page" },
    { name: "Extracted Text", icon: FileText, path: "/my-files" },
    { name: "Admin Panel", icon: Shield, path: "/admin", adminOnly: true },
    { name: "Profile", icon: User, path: "/profile" },
  ];

  return (
    <div className="h-full flex flex-col justify-between p-4">
      <div className="space-y-2">
        {navItems.map((item) => {
          const active = location.pathname === item.path;
          const Icon = item.icon;

          if (item.adminOnly && !role) {
            return (
              <div
                key={item.name}
                className="flex items-center gap-3 opacity-40 cursor-not-allowed select-none py-2 px-3 rounded-md"
              >
                <Icon size={20} />
                <span>{item.name}</span>
              </div>
            );
          }

          return (
            <Link
              key={item.name}
              to={item.path}
              className={`flex items-center gap-3 py-2 px-3 rounded-md transition-colors ${
                active
                  ? "bg-blue-600 text-white"
                  : "text-gray-300 hover:bg-gray-800"
              }`}
            >
              <Icon size={20} />
              <span>{item.name}</span>
            </Link>
          );
        })}
      </div>

      {/* Bottom logout */}
      <motion.button
        whileTap={{ scale: 0.95 }}
        onClick={() => {
          localStorage.clear();
          window.location.href = "/login";
        }}
        className="flex items-center gap-3 text-red-400 hover:text-red-500 transition-colors py-2 px-3 rounded-md"
      >
        <LogOut size={20} />
        <span>Logout</span>
      </motion.button>
    </div>
  );
};

export default Sidebar;
