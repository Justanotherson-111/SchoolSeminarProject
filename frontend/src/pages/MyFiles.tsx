import React from "react";
import Sidebar from "../components/Sidebar";
import Navbar from "../components/Navbar";
import FileList from "../components/FileList";

const MyFiles: React.FC = () => (
  <div className="flex flex-col md:flex-row min-h-screen">
    <Sidebar active="myfiles" />
    <div className="flex-1 p-4">
      <Navbar />
      <h1 className="text-xl font-bold mb-4">My Files</h1>
      <FileList />
    </div>
  </div>
);

export default MyFiles;
