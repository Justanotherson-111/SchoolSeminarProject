// vite.config.js
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react-swc'

export default defineConfig({
  base: '/',              // relative paths
  plugins: [react()],
  build: {
    outDir: 'dist',        // explicitly define dist
    emptyOutDir: true
  }
})
