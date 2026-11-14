// Using the generated client: the API answers are checked against their schemas, and so are the requests. Node runs
// it as it is (npm start), with the API running (dotnet run --project App).
import { createPetShopClient } from "./generated/petshop.ts";

const petShop = createPetShopClient({ baseUrl: process.env.PETSHOP_API ?? "http://localhost:5000" });

const categories = await petShop.categories.list();
for (const category of categories) {
  console.log(`${category.categoryId}: ${category.name ?? "(no name)"}`);
}

const pelican = await petShop.products.get("BD-01");
if (pelican) {
  await petShop.products.update(pelican.productId, { ...pelican, descn: "Will sit in your garden and admire nature." });
}
