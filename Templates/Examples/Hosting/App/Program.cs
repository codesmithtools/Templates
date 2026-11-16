// The records are PetShop.db3's tables. They're in no file of this project: the build generated them (Records.csp),
// and generates them again when the .csp or a template changes.
using PetShop.Data;

var category = new Category { CategoryId = "FISH", Name = "Fish" };
var product = new Product { ProductId = "FI-SW-01", CategoryId = category.CategoryId, Name = "Angelfish" };
Console.WriteLine(category);
Console.WriteLine(product);

var records = typeof(Category).Assembly.GetTypes().Where(t => t.Namespace == "PetShop.Data").Select(t => t.Name).Order();
Console.WriteLine($"Generated as it built: {string.Join(", ", records)}");
