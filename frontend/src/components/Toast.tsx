import React from "react";
import type { Toast as ToastType } from "../utils/useToast";

interface Props {
  toasts: ToastType[];
  onRemove: (id: string) => void;
}

const ToastContainer: React.FC<Props> = ({ toasts, onRemove }) => {
  return (
    <div className="fixed top-5 right-5 flex flex-col gap-3 z-50">
      {toasts.map((t) => (
        <div
          key={t.id}
          className={`px-4 py-2 rounded shadow-lg text-white ${
            t.type === "success"
              ? "bg-emerald-500"
              : t.type === "error"
              ? "bg-red-500"
              : "bg-gray-700"
          }`}
        >
          {t.message}
          <button
            onClick={() => onRemove(t.id)}
            className="ml-3 font-bold"
          >
            ×
          </button>
        </div>
      ))}
    </div>
  );
};

export default ToastContainer;
