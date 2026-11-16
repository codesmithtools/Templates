// Yours, not generated: orders for the demo. PetShop.db3 has none, so the web app's copy of it gets some at each start,
// made with the business objects: their rules run, and Order.cs totals them. Program.cs adds it.
using Csla;

namespace PetShop.Web;

public sealed class DemoData(IServiceScopeFactory scopes) : IHostedService
{
    private static readonly (string User, string First, string Last, string Address, string City, string State, string Zip, string Country)[] Customers =
    [
        ("ada", "Ada", "Lovelace", "12 St James's Square", "London", "London", "SW1Y 4JH", "UK"),
        ("grace", "Grace", "Hopper", "1 Navy Yard", "Arlington", "VA", "22202", "USA"),
        ("alan", "Alan", "Turing", "78 High Street", "Wilmslow", "Cheshire", "SK9 1AA", "UK"),
        ("margaret", "Margaret", "Hamilton", "77 Massachusetts Ave", "Cambridge", "MA", "02139", "USA"),
        ("katherine", "Katherine", "Johnson", "1 NASA Drive", "Hampton", "VA", "23666", "USA"),
        ("barbara", "Barbara", "Liskov", "32 Vassar Street", "Cambridge", "MA", "02139", "USA"),
        ("edsger", "Edsger", "Dijkstra", "Den Dolech 2", "Eindhoven", "Noord-Brabant", "5612 AZ", "Netherlands"),
        ("radia", "Radia", "Perlman", "500 Oracle Parkway", "Redwood City", "CA", "94065", "USA"),
        ("frances", "Frances", "Allen", "1101 Kitchawan Road", "Yorktown Heights", "NY", "10598", "USA"),
        ("donald", "Donald", "Knuth", "353 Jane Stanford Way", "Stanford", "CA", "94305", "USA"),
        ("hedy", "Hedy", "Lamarr", "6834 Hollywood Blvd", "Los Angeles", "CA", "90028", "USA"),
        ("tim", "Tim", "Berners-Lee", "32 Vassar Street", "Cambridge", "MA", "02139", "USA"),
    ];

    private static readonly string[] Couriers = ["UPS", "FedEx", "DHL", "USPS"];

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopes.CreateAsyncScope();
        // an authorization number is an Admin's to set (Order.cs)
        scope.ServiceProvider.GetRequiredService<ApplicationContext>().User = DemoUsers.Admin.Principal;
        var portals = scope.ServiceProvider.GetRequiredService<IDataPortalFactory>();
        if ((await portals.GetPortal<OrderListCount>().ExecuteAsync()).Count > 0)
            return;

        var items = (await portals.GetPortal<ItemList>().FetchAsync()).Where(i => i.ListPrice is not null).ToList();
        var random = new Random(2026);
        for (int i = 0; i < Customers.Length; i++)
        {
            var c = Customers[i];
            var order = await portals.GetPortal<Order>().CreateAsync();
            order.UserId = c.User;
            order.OrderDate = DateTime.Today.AddDays(-3 * (Customers.Length - i) + random.Next(3));
            order.BillToFirstName = order.ShipToFirstName = c.First;
            order.BillToLastName = order.ShipToLastName = c.Last;
            order.BillAddr1 = order.ShipAddr1 = c.Address;
            order.BillCity = order.ShipCity = c.City;
            order.BillState = order.ShipState = c.State;
            order.BillZip = order.ShipZip = c.Zip;
            order.BillCountry = order.ShipCountry = c.Country;
            order.Courier = Couriers[random.Next(Couriers.Length)];
            order.Locale = c.Country == "USA" ? "en-US" : "en-GB";
            order.AuthorizationNumber = random.Next(100_000, 999_999);
            foreach (var item in items.OrderBy(_ => random.Next()).Take(1 + random.Next(4)))
                await order.LineItems.AddLineAsync(item.ItemId, 1 + random.Next(3), item.ListPrice!.Value);
            foreach (var (status, days) in new[] { ("OP", 0), ("SH", 2), ("DL", 5) }.Take(i < 8 ? 3 : i < 10 ? 2 : 1))
            {
                var step = await order.OrderStatuses.AddNewAsync();
                step.LineNum = order.OrderStatuses.Count;
                step.Timestamp = order.OrderDate.AddDays(days).AddHours(9 + random.Next(8));
                step.Status = status;
            }
            order = await order.SaveAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
