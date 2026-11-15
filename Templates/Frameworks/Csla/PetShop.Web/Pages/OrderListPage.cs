// Yours, not generated: who an order's for, after its number; and two columns the list can do without.
namespace PetShop.Web.Pages;

public partial class OrderListPage
{
    public override IEnumerable<ListColumn<OrderInfo>> MoreColumns =>
        [new("Bill to", row => row.BillToFirstName + " " + row.BillToLastName, After: "Order ID")];

    public override IEnumerable<string> HiddenColumns => ["Authorization number", "Locale"];
}
