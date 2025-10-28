import React from "react";

const Dashboard: React.FC = () => {
  return (
    <div>
      <h1 className="text-3xl font-semibold mb-4">Welcome Back 👋</h1>
      <p className="text-gray-400">
        Manage your OCR images and extracted text here.
      </p>

      <div className="mt-6 grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
        <div className="bg-gray-800 rounded-lg p-4 shadow hover:shadow-lg transition">
          <h2 className="text-lg font-medium mb-2">Upload Image</h2>
          <p className="text-sm text-gray-400">Upload new image to extract text.</p>
        </div>

        <div className="bg-gray-800 rounded-lg p-4 shadow hover:shadow-lg transition">
          <h2 className="text-lg font-medium mb-2">View Extracted Files</h2>
          <p className="text-sm text-gray-400">Preview and download extracted text.</p>
        </div>

        <div className="bg-gray-800 rounded-lg p-4 shadow hover:shadow-lg transition">
          <h2 className="text-lg font-medium mb-2">Profile</h2>
          <p className="text-sm text-gray-400">Update your information and preferences.</p>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
