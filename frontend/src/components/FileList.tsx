import React, { useEffect, useState } from "react";
import FileCard from "./FileCard";
import api from "../api/axios";

interface File {
  id: string;
  name: string;
  downloadUrl: string;
}

const FileList: React.FC = () => {
  const [files, setFiles] = useState<File[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchFiles = async () => {
      try {
        const res = await api.get("/files/my");
        setFiles(res.data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchFiles();
  }, []);

  if (loading) return <div>Loading files...</div>;

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      {files.map((f) => (
        <FileCard key={f.id} name={f.name} downloadUrl={f.downloadUrl} />
      ))}
    </div>
  );
};

export default FileList;
