import React, { useState } from "react";
import api from "../api/axios";

interface Props {
  refresh: () => void;
  showToast: (msg: string, type?: "success" | "error") => void;
}

const UploadBox: React.FC<Props> = ({ refresh, showToast }) => {
  const [file, setFile] = useState<File | null>(null);
  const [busy, setBusy] = useState(false);

  const handleUpload = async () => {
    if (!file) return;
    setBusy(true);
    try {
      const formData = new FormData();
      formData.append("file", file);
      await api.post("/image/upload", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });
      showToast("Image uploaded!", "success");
      setFile(null);
      refresh();
    } catch (err) {
      console.error(err);
      showToast("Upload failed", "error");
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="bg-gray-800 p-4 rounded-md mb-4 flex flex-col sm:flex-row gap-2 items-center">
      <input type="file" accept="image/*" onChange={(e) => setFile(e.target.files?.[0] || null)} className="text-gray-100" />
      <button
        onClick={handleUpload}
        disabled={busy || !file}
        className="bg-emerald-500 hover:bg-emerald-400 px-4 py-2 rounded-md text-black font-medium"
      >
        {busy ? "Uploading..." : "Upload"}
      </button>
    </div>
  );
};

export default UploadBox;
