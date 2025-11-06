import React, { useEffect, useState } from "react";
import api from "../api/axios";
import useToast from "../utils/useToast";

interface ImageItem {
  id: string;
  originalFileName: string;
  relativePath: string;
}

const Image: React.FC = () => {
  const [images, setImages] = useState<ImageItem[]>([]);
  const [file, setFile] = useState<File | null>(null);
  const [loading, setLoading] = useState(false);
  const toast = useToast();

  const fetchImages = async () => {
    try {
      const { data } = await api.get("/image");
      setImages(data);
    } catch {
      toast.showToast("Failed to load images", "error");
    }
  };

  useEffect(() => {
    fetchImages();
  }, []);

  const handleUpload = async () => {
    if (!file) return toast.showToast("No file selected", "error");
    setLoading(true);
    try {
      const formData = new FormData();
      formData.append("file", file);
      await api.post("/image/upload", formData, {
        headers: { "Content-Type": "multipart/form-data" },
      });
      toast.showToast("Uploaded successfully", "success");
      setFile(null);
      fetchImages();
    } catch {
      toast.showToast("Upload failed", "error");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <h1 className="text-2xl font-bold text-emerald-400 mb-4">Upload Image</h1>
      <input type="file" onChange={(e) => setFile(e.target.files?.[0] || null)} />
      <button
        className="ml-2 px-3 py-1 bg-emerald-500 rounded hover:bg-emerald-400"
        onClick={handleUpload}
        disabled={loading}
      >
        {loading ? "Uploading..." : "Upload"}
      </button>

      <div className="mt-6 grid grid-cols-3 gap-4">
        {images.map((img) => (
          <div key={img.id} className="border border-gray-700 p-2 rounded">
            <img src={`${import.meta.env.VITE_API_URL}/Uploads/${img.relativePath}`} alt={img.originalFileName} />
            <p className="text-sm text-gray-200">{img.originalFileName}</p>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Image;
