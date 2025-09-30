import React from "react";

interface FileCardProps {
  name: string;
  downloadUrl: string;
}

const FileCard: React.FC<FileCardProps> = ({ name, downloadUrl }) => {
  return (
    <div className="border rounded p-3 shadow hover:shadow-lg transition flex justify-between items-center">
      <span className="truncate max-w-xs">{name}</span>
      <a href={downloadUrl} download className="bg-blue-500 text-white px-2 py-1 rounded">
        Download
      </a>
    </div>
  );
};

export default FileCard;
