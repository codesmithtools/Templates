using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using PetShop.Core.Controls;
using PetShop.Core.Model;
using PetShop.Core.Data;
using PetShop.Core.Utility;

namespace PetShop.UI
{
    public partial class CheckOut : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (billingForm.Address == null)
            {
                using (var context = new PetShopDataContext())
                {
                    var profile = context.Profile.GetProfile(User.Identity.Name);
                    billingForm.Address = new Address(profile);
                }
            }
        }

        /// <summary>
        /// Process the order
        /// </summary>
        protected void wzdCheckOut_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            using (var context = new PetShopDataContext())
            {
                var profile = context.Profile.GetProfile(User.Identity.Name);
                if (profile.ShoppingCart.Count > 0)
                {
                    // display ordered items
                    CartListOrdered.Bind(profile.ShoppingCart);

                    // display total and credit card information
                    ltlTotalComplete.Text = ltlTotal.Text;
                    ltlCreditCardComplete.Text = ltlCreditCard.Text;

                    #region Create Order

                    var order = new Order();

                    order.UserId = profile.UniqueID.ToString();
                    order.OrderDate = DateTime.Now;
                    order.CreditCard = GetCreditCard();
                    order.Courier = order.CreditCard.CardType;
                    order.TotalPrice = CartHelper.GetTotal(profile.ShoppingCart);
                    order.AuthorizationNumber = 0;
                    order.Locale = "en-us";

                    #region Shipping Information

                    order.ShipAddr1 = billingForm.Address.Address1;
                    order.ShipAddr2 = billingForm.Address.Address2;
                    order.ShipCity = billingForm.Address.City;
                    order.ShipState = billingForm.Address.State;
                    order.ShipZip = billingForm.Address.Zip;
                    order.ShipCountry = billingForm.Address.Country;
                    order.ShipToFirstName = billingForm.Address.FirstName;
                    order.ShipToLastName = billingForm.Address.LastName;

                    #endregion

                    #region Billing Information

                    order.BillAddr1 = shippingForm.Address.Address1;
                    order.BillAddr2 = shippingForm.Address.Address2;
                    order.BillCity = shippingForm.Address.City;
                    order.BillState = shippingForm.Address.State;
                    order.BillZip = shippingForm.Address.Zip;
                    order.BillCountry = shippingForm.Address.Country;
                    order.BillToFirstName = shippingForm.Address.FirstName;
                    order.BillToLastName = shippingForm.Address.LastName;

                    #endregion
                    context.Order.InsertOnSubmit(order);
                    context.SubmitChanges();

                    #endregion

                    int itemsOnBackOrder = 0;
                    //Decrement and check the Inventory.
                    foreach (Cart cart in profile.ShoppingCart)
                    {
                        var inventory = context.Inventory.GetByKey(cart.ItemId);

                        if (cart.Quantity > inventory.Qty)
                        {
                            itemsOnBackOrder += cart.Quantity - inventory.Qty;
                        }

                        inventory.Qty -= cart.Quantity;
                        context.SubmitChanges();
                    }

                    if (itemsOnBackOrder > 0)
                    {
                        ItemsOnBackOrder.Text = string.Format("<br /><p style=\"color:red;\"><b>Backorder ALERT:</b> {0} items are on backorder.</p>", itemsOnBackOrder);
                    }

                    CartHelper.SaveOrderLineItems(profile.ShoppingCart, order.OrderId);

                    // destroy cart
                    CartHelper.ClearCart(profile.ShoppingCart);
                }
                else
                {
                    lblMsg.Text =
                        "<p><br>Can not process the order. Your cart is empty.</p><p class=SignUpLabel><a class=linkNewUser href=Default.aspx>Continue shopping</a></p>";
                    wzdCheckOut.Visible = false;
                }
            }
        }

        /// <summary>
        /// Create CreditCardInfo object from user input
        /// </summary>
        private CreditCard GetCreditCard()
        {
            string type = WebUtility.InputText(listCCType.SelectedValue, 40);
            string exp = WebUtility.InputText(txtExpDate.Text, 7);
            string number = WebUtility.InputText(txtCCNumber.Text, 20);
            return new CreditCard(type, number, exp);
        }

        /// <summary>
        /// Changing Wiszard steps
        /// </summary>
        protected void wzdCheckOut_ActiveStePChanged(object sender, EventArgs e)
        {
            if (wzdCheckOut.ActiveStepIndex == 3)
            {
                billingConfirm.Address = billingForm.Address;
                shippingConfirm.Address = shippingForm.Address;

                using (var context = new PetShopDataContext())
                {
                    var profile = context.Profile.GetProfile(User.Identity.Name);
                    ltlTotal.Text = CartHelper.GetTotal(profile.ShoppingCart).ToString("c");
                }

                if (txtCCNumber.Text.Length > 4)
                    ltlCreditCard.Text = txtCCNumber.Text.Substring(txtCCNumber.Text.Length - 4, 4);
            }
        }

        /// <summary>
        /// Handler for "Ship to Billing Addredd" checkbox.
        /// Prefill/Clear shipping address form.
        /// </summary>
        protected void chkShipToBilling_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShipToBilling.Checked)
                shippingForm.Address = billingForm.Address;
        }

        /// <summary>
        /// Custom validator to check CC expiration date
        /// </summary>
        protected void ServerValidate(object source, ServerValidateEventArgs value)
        {
            DateTime dt;
            if (DateTime.TryParse(value.Value, out dt))
                value.IsValid = (dt > DateTime.Now);
            else
                value.IsValid = false;
        }
    }
}