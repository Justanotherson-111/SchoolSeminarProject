import React from "react";

interface Props {
  images: any[];
  refresh: () => void;
  showToast: (msg: string, type?: "success" | "error") => void;
}

const ImageList: React.FC<Props> = ({ images, refresh, showToast }) => {
  const handleDelete = async (id: string) => {
    try {
      await fetch(`/api/image/${id}`, { method: "DELETE" });
      showToast("Image deleted", "success");
      refresh();
    } catch {
      showToast("Delete failed", "error");
    }
  };

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
      {images.map((img) => (
        <div key={img.id} className="bg-gray-900 rounded-md p-2">
          <img src={`/uploads/${img.relativePath}`} alt={img.originalFileName} className="w-full h-40 object-cover rounded-md" />
          <div className="mt-2 flex justify-between items-center text-gray-200 text-sm">
            <span>{img.originalFileName}</span>
            <button onClick={() => handleDelete(img.id)} className="text-red-500 hover:text-red-400">
              Delete
            </button>
          </div>
        </div>
      ))}
    </div>
  );
};

export default ImageList;
