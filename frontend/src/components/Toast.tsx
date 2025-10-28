type Toast = { id: string; message: string; type?: "info" | "success" | "error" };

export default function ToastContainer({ toasts, onRemove }: { toasts: Toast[]; onRemove: (id: string) => void; }) {
  return (
    <div className="fixed right-4 top-20 z-50 flex flex-col gap-3">
      {toasts.map(t => (
        <div key={t.id} className={`px-4 py-2 rounded-md shadow-md max-w-xs text-sm ${t.type === "error" ? "bg-red-600 text-white" : t.type === "success" ? "bg-emerald-600 text-black" : "bg-gray-800 text-white"}`}>
          <div className="flex justify-between items-center gap-3">
            <div>{t.message}</div>
            <button onClick={() => onRemove(t.id)} className="opacity-80 hover:opacity-100">✕</button>
          </div>
        </div>
      ))}
    </div>
  );
}
