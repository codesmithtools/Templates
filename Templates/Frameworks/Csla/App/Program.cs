// The business objects at work: on the EF Core DAL, against a copy of PetShop.db3 (each run starts from the sample's).
using System.Security.Claims;
using Csla;
using Csla.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetShop.Business;

string database = Path.Combine(AppContext.BaseDirectory, "PetShop.demo.db3");
File.Copy(Path.Combine(AppContext.BaseDirectory, "PetShop.db3"), database, overwrite: true);

var services = new ServiceCollection();
services.AddCsla();
services.AddPetShopDal(options => options.UseSqlite($"Data Source={database};Pooling=False"));
await using var provider = services.BuildServiceProvider();
var portals = provider.GetRequiredService<IDataPortalFactory>();

// a name/value list, and a read-only list by a foreign key
var categories = await portals.GetPortal<CategoryNameValueList>().FetchAsync();
Console.WriteLine($"Categories: {string.Join(", ", categories.Select(c => c.Value))}");
var fish = await portals.GetPortal<ProductList>().FetchAsync("FISH");
Console.WriteLine($"Fish: {string.Join(", ", fish.Select(p => p.Name))}");

// a new order: its rules come from the schema (Required, StringLength) and from Order.cs (a line)
var order = await portals.GetPortal<Order>().CreateAsync();
Console.WriteLine($"A new order breaks {order.BrokenRulesCollection.Count} rules, among them \"{order.BrokenRulesCollection.First().Description}\"");
order.UserId = "demo";
order.BillToFirstName = order.ShipToFirstName = "Ada";
order.BillToLastName = order.ShipToLastName = "Lovelace";
order.BillAddr1 = order.ShipAddr1 = "1 Analytical Way";
order.BillCity = order.ShipCity = "London";
order.BillState = order.ShipState = "London";
order.BillZip = order.ShipZip = "N1";
order.BillCountry = order.ShipCountry = "UK";
await order.LineItems.AddLineAsync("EST-1", 2, 16.50m);
await order.LineItems.AddLineAsync("EST-2", 1, 12.00m);
order.ShipCity = new string('x', 100);
Console.WriteLine($"A city of 100 characters: \"{order.BrokenRulesCollection.First().Description}\"");
order.ShipCity = "London";
Console.WriteLine($"Filled in, it's valid: {order.IsValid}");

// saved with its lines in one transaction; the database numbers it, Order.cs totals it
order = await order.SaveAsync();
Console.WriteLine($"Saved order {order.OrderId}: {order.LineItems.Count} lines, {order.TotalPrice:0.00}");

// fetched with its lines, changed and saved again
order = await portals.GetPortal<Order>().FetchAsync(order.OrderId);
order.LineItems[0].Quantity = 3;
order.LineItems.RemoveAt(1);
Console.WriteLine($"A saved line's number can't change: {!order.LineItems[0].CanWriteProperty(LineItem.LineNumProperty)}");
order = await order.SaveAsync();
var again = await portals.GetPortal<Order>().FetchAsync(order.OrderId);
Console.WriteLine($"Changed: {again.LineItems.Count} line, {again.TotalPrice:0.00}");

// Order.cs lets only an Admin delete an order
again.Delete();
try
{
    again = await again.SaveAsync();
}
catch (Exception e) when (e.GetBaseException() is Csla.Security.SecurityException security)
{
    Console.WriteLine($"Deleting without the Admin role: {security.Message}");
}
provider.GetRequiredService<ApplicationContext>().User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, "Admin")], "demo"));
again = await again.SaveAsync();
var orders = await portals.GetPortal<OrderList>().FetchAsync();
Console.WriteLine($"Deleted with its lines, as an Admin: {orders.Count} orders left");

// a child the Children setting made: a profile's accounts
var profiles = await portals.GetPortal<ProfileList>().FetchAsync();
var profile = await portals.GetPortal<Profile>().FetchAsync(profiles[0].UniqueID);
Console.WriteLine($"Profile {profile.Username} has {profile.Accounts.Count} account(s): {string.Join(", ", profile.Accounts.Select(a => a.Email))}");
