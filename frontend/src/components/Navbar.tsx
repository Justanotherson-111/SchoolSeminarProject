import React from "react";
import { Link, useNavigate } from "react-router-dom";

const Navbar: React.FC = () => {
  const navigate = useNavigate();
  const userRole = localStorage.getItem("role");

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    navigate("/login");
  };

  return (
    <nav className="bg-blue-600 text-white p-4 flex justify-between items-center">
      <Link to="/dashboard" className="font-bold text-lg">OCR App</Link>
      <div className="flex space-x-4">
        <Link to="/dashboard">Dashboard</Link>
        {userRole === "Admin" && <Link to="/admin">Admin Panel</Link>}
        <button onClick={logout} className="bg-red-500 px-2 py-1 rounded">Logout</button>
      </div>
    </nav>
  );
};

export default Navbar;
