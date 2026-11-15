// Yours, not generated: LineItem's subtotal and its rules.
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace PetShop.Business;

public partial class LineItem
{
    /// <summary>Quantity times unit price.</summary>
    public decimal Subtotal => Quantity * UnitPrice;

    partial void OnCreated() => Quantity = 1;

    partial void OnAddBusinessRules()
    {
        BusinessRules.AddRule(new MinValue<int>(QuantityProperty, 1));
        // a warning, not an error: it saves
        BusinessRules.AddRule(new MaxValue<int>(QuantityProperty, 20) { Severity = RuleSeverity.Warning, MessageText = "Over 20 of an item: is the quantity right?" });
    }
}
