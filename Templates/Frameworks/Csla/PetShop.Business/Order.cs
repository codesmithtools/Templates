// Yours, not generated: what Order does beyond its columns. Generated/Order.g.cs is the other half of the class, and
// generating again leaves this file as it is.
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace PetShop.Business;

public partial class Order
{
    /// <summary>What the lines come to.</summary>
    public decimal LinesTotal => LineItems.Sum(line => line.Subtotal);

    partial void OnCreated()
    {
        OrderDate = DateTime.Today;
        Courier = "UPS";
        Locale = "en-US";
    }

    // the order's total is its lines'
    partial void OnSaving() => TotalPrice = LinesTotal;

    partial void OnAddBusinessRules() => BusinessRules.AddRule(new HasLines(LineItemsProperty));

    // only an Admin deletes an order
    static partial void OnAddObjectAuthorizationRules(IAddObjectAuthorizationRulesContext context) =>
        Csla.Rules.BusinessRules.AddRule(typeof(Order), new IsInRole(AuthorizationActions.DeleteObject, "Admin"));

    /// <summary>An order has a line: checked when LineItems changes (CSLA checks a child property's rules then).</summary>
    private sealed class HasLines(Csla.Core.IPropertyInfo lines) : BusinessRule(lines)
    {
        protected override void Execute(IRuleContext context)
        {
            if (context.Target is Order { LineItems.Count: 0 })
                context.AddErrorResult("An order needs a line.");
        }
    }
}
