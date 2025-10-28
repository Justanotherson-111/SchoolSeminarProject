import React from "react";
import FileList from "../components/FileList";

const ExtractedText: React.FC = () => {
  return (
    <div className="card max-w-2xl mx-auto">
      <h2 className="text-xl font-semibold mb-3">Extracted Files</h2>
      <FileList readonly />
    </div>
  );
};

export default ExtractedText;
