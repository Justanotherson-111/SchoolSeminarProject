import React from "react";
import Sidebar from "../components/Sidebar";
import Navbar from "../components/Navbar";
import UploadForm from "../components/UploadForm";

const Upload: React.FC = () => (
  <div className="flex flex-col md:flex-row min-h-screen">
    <Sidebar active="upload" />
    <div className="flex-1 p-4">
      <Navbar />
      <h1 className="text-xl font-bold mb-4">Upload File</h1>
      <UploadForm />
    </div>
  </div>
);

export default Upload;
