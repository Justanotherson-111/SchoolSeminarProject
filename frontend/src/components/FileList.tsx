import React, { useEffect, useState } from "react";
import api from "../api/axios";

interface FileItem {
  id: string;
  originalFileName: string;
  uploadedAt?: string;
}

const FileList: React.FC<{ readonly?: boolean }> = ({ readonly = false }) => {
  const [files, setFiles] = useState<FileItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [preview, setPreview] = useState<{ id: string; text: string } | null>(null);

  useEffect(() => {
    (async () => {
      try {
        const res = await api.get("/image");
        const items = res?.data?.items ?? [];
        setFiles(items.map((it: any) => ({
          id: it.id,
          originalFileName: it.originalFileName ?? it.OriginalFileName ?? it.fileName,
          uploadedAt: it.uploadedAt ?? it.UploadedAt
        })));
      } catch (err) {
        console.error(err);
        alert("Failed to load files.");
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  const handleDelete = async (id: string) => {
    if (!confirm("Delete this file?")) return;
    try {
      await api.delete(`/image/${id}`);
      setFiles(prev => prev.filter(f => f.id !== id));
    } catch (err) {
      console.error(err);
      alert("Delete failed");
    }
  };

  const handlePreview = async (id: string) => {
    try {
      const res = await api.get(`/image/${id}/text`);
      const text = res.data?.txt?.Text ?? res.data?.Text ?? res.data?.text ?? JSON.stringify(res.data);
      setPreview({ id, text });
    } catch (err) {
      console.error(err);
      alert("Preview failed");
    }
  };

  if (loading) return <div className="p-4 text-gray-400">Loading files...</div>;
  if (files.length === 0) return <div className="p-4 text-gray-400">No files found.</div>;

  return (
    <div className="space-y-3">
      {files.map(f => (
        <div key={f.id} className="flex items-center justify-between bg-black/40 border border-black/20 p-3 rounded-lg">
          <div>
            <div className="font-medium text-gray-100">{f.originalFileName}</div>
            <div className="text-xs text-gray-400">{f.uploadedAt ? new Date(f.uploadedAt).toLocaleString() : ""}</div>
          </div>
          <div className="flex items-center gap-2">
            <button onClick={() => handlePreview(f.id)} className="px-2 py-1 rounded bg-emerald-600 text-black">Preview</button>
            <a href={`/api/image/${f.id}/download`} className="px-2 py-1 border rounded border-gray-700 text-gray-200">Download</a>
            {!readonly && <button onClick={() => handleDelete(f.id)} className="px-2 py-1 rounded bg-red-600 text-white">Delete</button>}
          </div>
        </div>
      ))}

      {preview && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 p-4" onClick={() => setPreview(null)}>
          <div className="bg-black/80 p-4 rounded-lg max-w-3xl w-full" onClick={(e) => e.stopPropagation()}>
            <div className="flex justify-between items-center mb-3">
              <h3 className="text-lg font-semibold text-emerald-300">Preview</h3>
              <button onClick={() => setPreview(null)} className="text-gray-300">Close</button>
            </div>
            <pre className="whitespace-pre-wrap text-gray-200 max-h-[60vh] overflow-auto">{preview.text}</pre>
          </div>
        </div>
      )}
    </div>
  );
};

export default FileList;
