import { useState } from "react";

export type ToastType = "info" | "success" | "error";

export interface Toast {
  id: string;
  message: string;
  type?: ToastType;
}

export default function useToast() {
  const [toasts, setToasts] = useState<Toast[]>([]);

  const add = (message: string, type: ToastType = "info") => {
    const id = Date.now().toString();
    setToasts((prev) => [...prev, { id, message, type }]);
    setTimeout(() => remove(id), 4000);
  };

  const remove = (id: string) => {
    setToasts((prev) => prev.filter((toast) => toast.id !== id));
  };

  return { toasts, add, remove };
}
