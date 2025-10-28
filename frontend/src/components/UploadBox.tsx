import React, { useCallback, useState } from "react";
import api from "../api/axios";

export default function UploadBox({ onUploaded }: { onUploaded?: () => void }) {
  const [drag, setDrag] = useState(false);
  const [progress, setProgress] = useState<number | null>(null);

  const uploadFile = async (file: File) => {
    const fd = new FormData();
    fd.append("file", file);
    try {
      setProgress(0);
      await api.post("/image/upload", fd, {
        headers: { "Content-Type": "multipart/form-data" },
        onUploadProgress: (e) => {
          if (e.total) setProgress(Math.round((e.loaded / e.total) * 100));
        },
      });
      setProgress(null);
      onUploaded && onUploaded();
      alert("Uploaded! OCR will run shortly.");
    } catch (err) {
      console.error(err);
      alert("Upload failed.");
      setProgress(null);
    }
  };

  const onDrop = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    setDrag(false);
    if (e.dataTransfer.files && e.dataTransfer.files[0]) uploadFile(e.dataTransfer.files[0]);
  }, []);

  return (
    <div>
      <div
        onDragOver={(e) => { e.preventDefault(); setDrag(true); }}
        onDragLeave={() => setDrag(false)}
        onDrop={onDrop}
        className={`border-2 border-dashed rounded-2xl p-8 text-center ${drag ? "border-emerald-500 bg-white/2" : "border-gray-700 bg-black/20"}`}
      >
        <div className="text-gray-300">Drag & drop an image here, or</div>
        <label className="mt-3 inline-flex items-center cursor-pointer bg-emerald-600 hover:bg-emerald-500 px-4 py-2 rounded text-black">
          <input type="file" accept="image/*" className="sr-only" onChange={(e) => { const f = e.target.files?.[0]; if (f) uploadFile(f); }} />
          Choose a file
        </label>
      </div>

      {progress !== null && (
        <div className="mt-3">
          <div className="w-full bg-gray-800 rounded">
            <div style={{ width: `${progress}%` }} className="bg-emerald-500 text-black p-1 text-sm rounded">
              {progress}%
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
