import React from "react";
import { Link } from "react-router-dom";

const NotFound: React.FC = () => (
  <div className="flex flex-col items-center justify-center min-h-screen">
    <h1 className="text-3xl font-bold mb-4">404 - Page Not Found</h1>
    <Link to="/dashboard" className="text-blue-500 underline">Go back to dashboard</Link>
  </div>
);

export default NotFound;
