import React from "react";

interface Props {
  textFiles: any[];
}

const TextFileList: React.FC<Props> = ({ textFiles }) => {
  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
      {textFiles.map((txt) => (
        <a
          key={txt.id}
          href={`/extracted/${txt.txtFilePath}`}
          target="_blank"
          rel="noopener noreferrer"
          className="block p-4 bg-gray-800 rounded-md hover:bg-gray-700 text-gray-200"
        >
          {txt.txtFilePath.split("/").pop()}
        </a>
      ))}
    </div>
  );
};

export default TextFileList;
