import React from "react";
import UploadBox from "../components/UploadBox";

const Image: React.FC = () => {
  return (
    <div className="card max-w-2xl mx-auto">
      <h2 className="text-xl font-semibold mb-3">Upload Image</h2>
      <UploadBox onUploaded={() => window.location.reload()} />
    </div>
  );
};

export default Image;
