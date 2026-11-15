// Yours, not generated: LineItem.cs's subtotal in the order's lines, and their total.
namespace PetShop.Web.Pages;

public partial class LineItemsGrid
{
    public override IEnumerable<GridColumn<LineItem>> Columns => [new("Subtotal", line => Formats.Money(line.Subtotal))];

    public override IEnumerable<Stat> Totals => [new("Total", Formats.Money(Items.Sum(line => line.Subtotal)))];
}
