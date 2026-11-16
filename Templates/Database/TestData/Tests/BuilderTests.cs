// Yours: what the builders do, as tests. Each makes rows the database takes, and the rows they refer to first.
using Microsoft.Data.Sqlite;
using PetShop.Data;
using Xunit;

namespace PetShop.Tests;

public sealed class BuilderTests : DatabaseTest
{
    [Fact]
    public async Task ARowComesWithTheRowsItRefersTo()
    {
        // an item: its product first, and the product's category, and its supplier
        var item = await Data.Items.CreateAsync(cancellationToken: CancellationToken);

        var product = await new ProductRepository(Connection).GetAsync(item.ProductId, CancellationToken);
        Assert.NotNull(product);
        Assert.NotNull(await new CategoryRepository(Connection).GetAsync(product.CategoryId, CancellationToken));
        Assert.NotNull(item.Supplier);
        Assert.NotNull(await new SupplierRepository(Connection).GetAsync(item.Supplier.Value, CancellationToken));
    }

    [Fact]
    public async Task TheDatabaseRefusesMadeUpForeignKeys()
    {
        // Build makes values only: its OrderId refers to no order
        var line = Data.LineItems.Build();

        await Assert.ThrowsAsync<SqliteException>(() => new LineItemRepository(Connection).InsertAsync(line, CancellationToken));
    }

    [Fact]
    public async Task RowsShareTheRowTheyAreGiven()
    {
        var order = await Data.Orders.CreateAsync(cancellationToken: CancellationToken);

        var first = await Data.LineItems.CreateAsync(order, cancellationToken: CancellationToken);
        var second = await Data.LineItems.CreateAsync(order, change: l => l with { Quantity = 3 }, cancellationToken: CancellationToken);

        Assert.Equal([order.OrderId, order.OrderId], [first.OrderId, second.OrderId]);
        Assert.NotEqual(first.LineNum, second.LineNum);
        Assert.Equal(3, second.Quantity);
    }

    [Fact]
    public async Task ReuseParentsTakesRowsThatAreThere()
    {
        var products = new ProductRepository(Connection);
        int before = (await products.GetAllAsync(CancellationToken)).Count;
        Data.ReuseParents = true;

        var item = await Data.Items.CreateAsync(cancellationToken: CancellationToken);

        Assert.Equal(before, (await products.GetAllAsync(CancellationToken)).Count);
        Assert.NotNull(await products.GetAsync(item.ProductId, CancellationToken));
    }

    [Fact]
    public async Task KeysAndUniqueColumnsAreNewEachTime()
    {
        // Category's key is a varchar(10); Profiles has a unique index on (Username, ApplicationName)
        var categories = new List<Category>();
        var profiles = new List<Profile>();
        for (int i = 0; i < 20; i++)
        {
            categories.Add(await Data.Categories.CreateAsync(cancellationToken: CancellationToken));
            profiles.Add(await Data.Profiles.CreateAsync(cancellationToken: CancellationToken));
        }

        Assert.Equal(20, categories.Select(c => c.CategoryId).Distinct().Count());
        Assert.Equal(20, profiles.Select(p => (p.Username, p.ApplicationName)).Distinct().Count());
    }

    [Fact]
    public void ValuesFitTheirColumns()
    {
        var supplier = Data.Suppliers.Build();

        Assert.True(supplier.Zip!.Length <= 5); // varchar(5)
        Assert.Equal(2, supplier.Status.Length); // varchar(2), not null
        Assert.True(Data.Items.Build().ListPrice < 100_000_000m); // decimal(10,2)
    }

    [Fact]
    public void TheSameSeedMakesTheSameRows()
    {
        Assert.Equal(new TestData(Connection, seed: 7).Orders.Build(), new TestData(Connection, seed: 7).Orders.Build());
        Assert.NotEqual(Data.Orders.Build(), Data.Orders.Build());
    }

    [Fact]
    public void TheSamplesRulesApply()
    {
        // Builders/AccountBuilder.cs and Builders/OrderBuilder.cs
        Assert.EndsWith("@example.com", Data.Accounts.Build().Email);
        var order = Data.Orders.Build();
        Assert.Contains(order.Courier, new[] { "UPS", "FedEx", "USPS", "DHL" });
        Assert.Equal((order.ShipToLastName, order.ShipCity), (order.BillToLastName, order.BillCity));
    }
}
