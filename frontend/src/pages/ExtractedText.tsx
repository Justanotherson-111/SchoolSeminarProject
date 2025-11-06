import React, { useEffect, useState } from "react";
import api from "../api/axios";
import useToast from "../utils/useToast";

interface TextFile {
  id: string;
  txtFilePath: string;
  language: string;
}

const ExtractedText: React.FC = () => {
  const [texts, setTexts] = useState<TextFile[]>([]);
  const [loading, setLoading] = useState(false);
  const toast = useToast();

  useEffect(() => {
    const fetchTexts = async () => {
      setLoading(true);
      try {
        const { data } = await api.get("/textfile"); // Consider passing imageId if needed
        setTexts(data);
      } catch {
        toast.showToast("Failed to load extracted texts", "error");
      } finally {
        setLoading(false);
      }
    };
    fetchTexts();
  }, []);

  return (
    <div>
      <h1 className="text-2xl font-bold text-emerald-400 mb-4">Extracted Text Files</h1>
      {loading ? (
        <p>Loading...</p>
      ) : (
        <ul className="space-y-2">
          {texts.map((t) => (
            <li key={t.id} className="border p-2 rounded bg-gray-800 text-gray-100">
              <a
                href={`${import.meta.env.VITE_API_URL}/ExtractedText/${t.txtFilePath}`}
                target="_blank"
                rel="noopener noreferrer"
                className="text-emerald-300 underline"
              >
                {t.txtFilePath}
              </a>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default ExtractedText;
