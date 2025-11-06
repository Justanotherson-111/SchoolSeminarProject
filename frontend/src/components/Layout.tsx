import React from "react";
import Sidebar from "./Sidebar";

interface Props {
  children: React.ReactNode;
}

const Layout: React.FC<Props> = ({ children }) => {
  return (
    <div className="flex flex-col min-h-screen bg-gray-800 text-gray-100">
      {/* Top Navbar */}
      <header className="h-16 bg-gray-900 border-b border-gray-700 flex items-center px-6 shadow-md">
        <h1 className="text-xl font-bold text-emerald-400">OCR WebApp</h1>
      </header>

      <div className="flex flex-1">
        {/* Sidebar */}
        <Sidebar />

        {/* Main Content */}
        <main className="flex-1 p-6 overflow-auto">{children}</main>
      </div>
    </div>
  );
};

export default Layout;
