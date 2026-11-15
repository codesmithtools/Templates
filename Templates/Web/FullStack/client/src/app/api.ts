// The API's client the UI calls: the page's own server (the API serves the UI; Vite's dev server passes /api on to
// it). Add what every call needs here, such as an Authorization header, with options.fetch.
import { createPetShopClient } from "../generated/petshop.ts";

export const api = createPetShopClient({ baseUrl: "" });
