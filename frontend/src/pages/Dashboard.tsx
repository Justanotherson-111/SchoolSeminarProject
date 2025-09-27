import React from "react";
import Navbar from "../components/Navbar";
import Sidebar from "../components/Sidebar";
import FileList from "../components/FileList";

const Dashboard: React.FC = () => {
  return (
    <div className="flex flex-col md:flex-row min-h-screen">
      <Sidebar active="dashboard" />
      <div className="flex-1 p-4">
        <Navbar />
        <h1 className="text-xl font-bold mb-4">My Files</h1>
        <FileList />
      </div>
    </div>
  );
};

export default Dashboard;
