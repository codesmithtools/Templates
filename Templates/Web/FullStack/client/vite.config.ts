// The web UI's build: into the API's wwwroot (npm run build), which the API serves. In development (npm run dev),
// Vite serves it and passes the API's paths on to the API (dotnet run --project ../App), at PETSHOP_API.
import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

const api = process.env.PETSHOP_API ?? "http://localhost:5000";

export default defineConfig({
  plugins: [react()],
  build: {
    outDir: "../App/wwwroot",
    emptyOutDir: true,
  },
  server: {
    proxy: { "/api": api, "/openapi": api },
  },
});
