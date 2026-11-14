// Yours, not generated: adding a line numbers it.
namespace PetShop.Business;

public partial class LineItems
{
    /// <summary>A new line for an item, numbered after the others.</summary>
    public async Task<LineItem> AddLineAsync(string itemId, int quantity, decimal unitPrice)
    {
        int next = this.Select(line => line.LineNum).DefaultIfEmpty().Max() + 1;
        var line = await AddNewAsync();
        line.LineNum = next;
        line.ItemId = itemId;
        line.Quantity = quantity;
        line.UnitPrice = unitPrice;
        return line;
    }
}
