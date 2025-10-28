import React from "react";
import { Link } from "react-router-dom";

const NotFound: React.FC = () => {
  return (
    <div className="min-h-screen flex items-center justify-center">
      <div className="text-center">
        <h1 className="text-6xl font-bold">404</h1>
        <p className="text-gray-400 mt-2">Page not found</p>
        <Link to="/dashboard" className="mt-4 inline-block text-blue-500">Go Home</Link>
      </div>
    </div>
  );
};

export default NotFound;
