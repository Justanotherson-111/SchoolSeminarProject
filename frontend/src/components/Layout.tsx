import React, { useState, useEffect } from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";
import AnimatedBackground from "./AnimatedBackground";
import { motion, AnimatePresence } from "framer-motion";

const Layout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);
  const [menuOpen, setMenuOpen] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    const handleResize = () => setIsMobile(window.innerWidth < 768);
    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, []);

  const role = localStorage.getItem("role") ?? "User";

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    navigate("/login");
  };

  const links = [
    { path: "/dashboard", label: "Dashboard" },
    { path: "/upload", label: "Upload" },
    { path: "/files", label: "Files" },
    ...(role === "Admin" ? [{ path: "/admin", label: "Admin Panel" }] : []),
    { path: "/profile", label: "Profile" },
  ];

  return (
    <div className="relative min-h-screen text-gray-100 overflow-x-hidden bg-gradient-to-b from-[#071019] to-[#05060a]">
      {/* Animated background */}
      <AnimatedBackground />

      {/* Navbar */}
      <nav className="fixed top-0 left-0 right-0 z-30 bg-black/50 backdrop-blur-md border-b border-black/20">
        <div className="max-w-7xl mx-auto flex items-center justify-between px-4 sm:px-6 lg:px-8 py-3">
          <Link to="/dashboard" className="text-xl font-semibold text-emerald-400 select-none">
            Img2Txt
          </Link>

          {isMobile ? (
            <>
              <button
                aria-label="menu"
                onClick={() => setMenuOpen(!menuOpen)}
                className="p-2 rounded-md text-gray-200 hover:bg-white/5"
              >
                <svg className="w-6 h-6" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                  <path strokeWidth={1.5} strokeLinecap="round" strokeLinejoin="round"
                        d={menuOpen ? "M6 18L18 6M6 6l12 12" : "M4 6h16M4 12h16M4 18h16"} />
                </svg>
              </button>

              <AnimatePresence>
                {menuOpen && (
                  <motion.div
                    initial={{ opacity: 0, y: -8 }}
                    animate={{ opacity: 1, y: 0 }}
                    exit={{ opacity: 0, y: -6 }}
                    transition={{ duration: 0.2 }}
                    className="absolute top-full left-0 right-0 bg-black/80 backdrop-blur-md border-t border-black/20 z-40"
                  >
                    <div className="flex flex-col items-center py-3 space-y-2">
                      {links.map(l => (
                        <Link
                          key={l.path}
                          to={l.path}
                          onClick={() => setMenuOpen(false)}
                          className="text-gray-200 hover:text-emerald-300"
                        >
                          {l.label}
                        </Link>
                      ))}
                      <button onClick={logout} className="text-red-400 mt-1">Logout</button>
                    </div>
                  </motion.div>
                )}
              </AnimatePresence>
            </>
          ) : (
            <div className="flex items-center gap-6">
              {links.map(l => (
                <Link
                  key={l.path}
                  to={l.path}
                  className={`text-sm ${
                    location.pathname === l.path
                      ? "text-emerald-300 font-medium"
                      : "text-gray-200 hover:text-emerald-300"
                  }`}
                >
                  {l.label}
                </Link>
              ))}

              <button
                onClick={logout}
                className="ml-4 px-3 py-1 rounded-md bg-emerald-600 hover:bg-emerald-500 text-black font-medium"
              >
                Logout
              </button>
            </div>
          )}
        </div>
      </nav>

      {/* Page content */}
      <main className="relative z-20 pt-20 pb-16 px-4 sm:px-6 lg:px-8">
        <AnimatePresence mode="wait">
          <motion.div
            key={location.pathname}
            initial={{ opacity: 0, y: 12 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -8 }}
            transition={{ duration: 0.25 }}
          >
            <div className="max-w-6xl mx-auto">{children}</div>
          </motion.div>
        </AnimatePresence>
      </main>
    </div>
  );
};

export default Layout;
