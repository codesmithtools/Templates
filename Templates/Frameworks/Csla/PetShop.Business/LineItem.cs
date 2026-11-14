// Yours, not generated: LineItem's subtotal and its rule.
using Csla.Rules.CommonRules;

namespace PetShop.Business;

public partial class LineItem
{
    /// <summary>Quantity times unit price.</summary>
    public decimal Subtotal => Quantity * UnitPrice;

    partial void OnAddBusinessRules() => BusinessRules.AddRule(new MinValue<int>(QuantityProperty, 1));
}
