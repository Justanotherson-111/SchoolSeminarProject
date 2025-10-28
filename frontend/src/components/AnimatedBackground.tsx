import React from "react";
import { motion } from "framer-motion";

const AnimatedBackground: React.FC = () => {
  return (
    <div className="absolute inset-0 z-0 overflow-hidden pointer-events-none">
      {/* Floating blobs */}
      <motion.div
        className="absolute w-[600px] h-[400px] top-[-10%] left-[-10%] rounded-[36%] bg-gradient-to-br from-blue-500/30 via-purple-500/20 to-pink-500/10 blur-[60px] mix-blend-screen"
        animate={{ x: [0, 20, 0], y: [0, 15, 0], rotate: [0, 5, 0] }}
        transition={{ duration: 20, repeat: Infinity, repeatType: "loop", ease: "easeInOut" }}
      />
      <motion.div
        className="absolute w-[520px] h-[360px] bottom-[-8%] right-[-12%] rounded-[40%] bg-gradient-to-tr from-emerald-400/20 via-green-400/10 to-blue-400/5 blur-[50px] mix-blend-screen"
        animate={{ x: [0, -15, 0], y: [0, -10, 0], rotate: [0, -3, 0] }}
        transition={{ duration: 25, repeat: Infinity, repeatType: "loop", ease: "easeInOut" }}
      />
      <motion.div
        className="absolute w-[420px] h-[300px] top-[30%] left-[50%] rounded-[45%] bg-gradient-to-tr from-pink-400/10 via-purple-400/5 to-indigo-400/5 blur-[40px] mix-blend-screen"
        animate={{ x: [0, 10, 0], y: [0, -12, 0], rotate: [0, 2, 0] }}
        transition={{ duration: 22, repeat: Infinity, repeatType: "loop", ease: "easeInOut" }}
      />
    </div>
  );
};

export default AnimatedBackground;
