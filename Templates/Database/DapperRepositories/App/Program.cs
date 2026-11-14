// The generated repositories, on the PetShop sample database: reads from the file (read-only), and writes to a copy of
// it in memory.
using Microsoft.Data.Sqlite;
using PetShop.Data;

SqliteTypes.Register();

await using var petShop = new SqliteConnection("Data Source=../PetShop.db3;Mode=ReadOnly");
await petShop.OpenAsync();

var categories = new CategoryRepository(petShop);
var products = new ProductRepository(petShop);
var items = new ItemRepository(petShop);
var profiles = new ProfileRepository(petShop);

Console.WriteLine($"{(await categories.GetAllAsync()).Count} categories, {(await products.GetAllAsync()).Count} " +
    $"products, {(await items.GetAllAsync()).Count} items, {(await profiles.GetAllAsync()).Count} profiles");

Console.WriteLine("The first products:");
foreach (var product in (await products.GetAllAsync()).Take(3))
    Console.WriteLine($"  {product.ProductId}  {product.Name} ({product.CategoryId})");

Console.WriteLine($"Item EST-1: {await items.GetAsync("EST-1")}");
Console.WriteLine($"Profile 1: {await profiles.GetAsync(1)}");

// The copy: an order (the database numbers it) and a line of it, found by its composite key, changed and deleted.
await using var copy = new SqliteConnection("Data Source=:memory:");
await copy.OpenAsync();
petShop.BackupDatabase(copy);

var orders = new OrderRepository(copy);
var lineItems = new LineItemRepository(copy);
long orderId = await orders.InsertAsync(new Order
{
    UserId = "AdamBarr", OrderDate = new DateTime(2026, 11, 14, 9, 30, 0), Courier = "UPS", TotalPrice = 33.00m,
    AuthorizationNumber = 1, Locale = "en-US",
    ShipToFirstName = "Adam", ShipToLastName = "Barr", ShipAddr1 = "1 Main St", ShipCity = "Seattle", ShipState = "WA",
    ShipZip = "98101", ShipCountry = "USA",
    BillToFirstName = "Adam", BillToLastName = "Barr", BillAddr1 = "1 Main St", BillCity = "Seattle", BillState = "WA",
    BillZip = "98101", BillCountry = "USA",
});
Console.WriteLine($"Order {orderId}: {(await orders.GetAsync(orderId))?.OrderDate:yyyy-MM-dd HH:mm}");

await lineItems.InsertAsync(new LineItem
{
    OrderId = (int)orderId, LineNum = 1, ItemId = "EST-1", Quantity = 2, UnitPrice = 16.50m,
});
var line = await lineItems.GetAsync((int)orderId, 1);
Console.WriteLine($"LineItem ({orderId}, 1): {line}");
await lineItems.UpdateAsync(line! with { Quantity = 3 });
Console.WriteLine($"Changed: {await lineItems.GetAsync((int)orderId, 1)}");
Console.WriteLine($"Deleted: {await lineItems.DeleteAsync((int)orderId, 1)}, {await orders.DeleteAsync(orderId)}");
