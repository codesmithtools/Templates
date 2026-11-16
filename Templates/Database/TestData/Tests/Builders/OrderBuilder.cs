// Yours: changes to the generated OrderBuilder (Generated/Builders/OrderBuilder.cs). Generating again keeps them.
using Bogus;
using PetShop.Data;

namespace PetShop.Tests;

public sealed partial class OrderBuilder
{
    // the shop's couriers, and its one locale
    partial void Configure(Faker<Order> faker) => faker
        .RuleFor(o => o.Courier, f => f.PickRandom("UPS", "FedEx", "USPS", "DHL"))
        .RuleFor(o => o.Locale, "en-US");

    // an order is billed to whom it ships to: a rule makes one column, and this sees the whole row
    partial void OnBuilt(ref Order order) => order = order with
    {
        BillToFirstName = order.ShipToFirstName,
        BillToLastName = order.ShipToLastName,
        BillAddr1 = order.ShipAddr1,
        BillAddr2 = order.ShipAddr2,
        BillCity = order.ShipCity,
        BillState = order.ShipState,
        BillZip = order.ShipZip,
        BillCountry = order.ShipCountry,
    };
}
