import { useEffect, useState } from "react";

/**
 * Detects if the user is on mobile or desktop based on window width.
 * Automatically updates on window resize.
 */
export function useDeviceType() {
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);

  useEffect(() => {
    const handleResize = () => {
      setIsMobile(window.innerWidth < 768);
    };
    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, []);

  return { isMobile };
}
