import React, { useState } from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";
import { motion, AnimatePresence } from "framer-motion";
import { Menu, X } from "lucide-react";
import { isAdmin, logout } from "../utils/auth";

const Navbar: React.FC = () => {
  const [menuOpen, setMenuOpen] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const role = isAdmin();

  const handleLogout = () => {
    logout();
    setMenuOpen(false);
    navigate("/login");
  };

  const menuItems = [
    { name: "Dashboard", path: "/dashboard" },
    { name: "Image Upload", path: "/upload" },
    { name: "Extracted Text", path: "/files" },
    { name: "Admin Panel", path: "/admin", adminOnly: true },
    { name: "Profile", path: "/profile" },
  ];

  return (
    <motion.nav
      initial={{ y: -40, opacity: 0 }}
      animate={{ y: 0, opacity: 1 }}
      transition={{ duration: 0.4, ease: "easeOut" }}
      className="w-full z-20 bg-gray-900/60 backdrop-blur-md border-b border-gray-800 fixed top-0 left-0"
    >
      <div className="max-w-7xl mx-auto flex justify-between items-center p-3 md:p-4 text-gray-100">
        {/* Logo / Title */}
        <Link
          to="/dashboard"
          className="text-xl font-bold text-emerald-400 tracking-wide"
        >
          Img2Txt
        </Link>

        {/* Desktop menu */}
        <div className="hidden md:flex gap-6 items-center">
          {menuItems.map((item) => {
            if (item.adminOnly && !role) {
              return (
                <span
                  key={item.name}
                  className="opacity-40 cursor-not-allowed select-none"
                >
                  {item.name}
                </span>
              );
            }

            const isActive = location.pathname === item.path;
            return (
              <Link
                key={item.name}
                to={item.path}
                className={`transition-colors ${
                  isActive
                    ? "text-emerald-400"
                    : "text-gray-300 hover:text-emerald-400"
                }`}
              >
                {item.name}
              </Link>
            );
          })}

          <button
            onClick={handleLogout}
            className="ml-4 bg-emerald-600 hover:bg-emerald-700 text-white px-4 py-1 rounded-md transition-colors"
          >
            Logout
          </button>
        </div>

        {/* Mobile toggle */}
        <button
          onClick={() => setMenuOpen(!menuOpen)}
          className="md:hidden p-2 rounded hover:bg-gray-800 transition-colors"
        >
          {menuOpen ? <X size={24} /> : <Menu size={24} />}
        </button>
      </div>

      {/* Mobile dropdown menu */}
      <AnimatePresence>
        {menuOpen && (
          <motion.div
            initial={{ opacity: 0, y: -10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            transition={{ duration: 0.25 }}
            className="md:hidden bg-gray-900/90 backdrop-blur-lg border-t border-gray-800"
          >
            <div className="flex flex-col items-center p-4 gap-3">
              {menuItems.map((item) => {
                if (item.adminOnly && !role)
                  return (
                    <span
                      key={item.name}
                      className="opacity-40 cursor-not-allowed select-none"
                    >
                      {item.name}
                    </span>
                  );

                const isActive = location.pathname === item.path;
                return (
                  <Link
                    key={item.name}
                    to={item.path}
                    className={`transition-colors ${
                      isActive
                        ? "text-emerald-400"
                        : "text-gray-200 hover:text-emerald-400"
                    }`}
                    onClick={() => setMenuOpen(false)}
                  >
                    {item.name}
                  </Link>
                );
              })}
              <button
                onClick={handleLogout}
                className="mt-2 w-full bg-emerald-600 hover:bg-emerald-700 text-white px-4 py-1 rounded-md transition-colors"
              >
                Logout
              </button>
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </motion.nav>
  );
};

export default Navbar;
