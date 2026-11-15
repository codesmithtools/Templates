// Yours, not generated: what Order's editor shows besides its columns. Generated/OrderEditPage.razor is the other half of
// the class, and generating again leaves this file as it is.
namespace PetShop.Web.Pages;

public partial class OrderEditPage
{
    /// <summary>The summary beside the form: Order.cs's LinesTotal, as the lines change.</summary>
    public override IEnumerable<Stat> Stats =>
    [
        new("Lines total", Formats.Money(Model.LinesTotal), Big: true),
        new("Lines", Model.LineItems.Count.ToString("N0")),
        new("Items", Model.LineItems.Sum(line => line.Quantity).ToString("N0")),
    ];
}
