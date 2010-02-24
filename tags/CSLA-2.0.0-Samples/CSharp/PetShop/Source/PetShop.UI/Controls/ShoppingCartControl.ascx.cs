using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using PetShop.Business;
using PetShop.Controls;

namespace PetShop.UI.Controls
{
    public partial class ShoppingCartControl : UserControl
    {
        /// <summary>
        /// Handle Page load event
        /// </summary>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCart();
            }
        }

        /// <summary>
        /// Bind repeater to Cart object in Profile
        /// </summary>
        private void BindCart()
        {
            Profile profile = ProfileManager.Instance.GetCurrentUser(Page.User.Identity.Name);
            
            Business.CartList items = profile.ShoppingCart;
            if (items.Count > 0)
            {
                repShoppingCart.DataSource = items;
                repShoppingCart.DataBind();
                PrintTotal();
                plhTotal.Visible = true;
            }
            else
            {
                repShoppingCart.Visible = false;
                plhTotal.Visible = false;
                lblMsg.Text = "Your cart is empty.";
            }
        }

        /// <summary>
        /// Recalculate the total
        /// </summary>
        private void PrintTotal()
        {
            Profile profile = ProfileManager.Instance.GetCurrentUser(Page.User.Identity.Name);
            if (profile.ShoppingCart.Count > 0)
                ltlTotal.Text = profile.ShoppingCart.Total.ToString("c");
        }

        /// <summary>
        /// Calculate total
        /// </summary>
        protected void BtnTotal_Click(object sender, ImageClickEventArgs e)
        {
            Profile profile = ProfileManager.Instance.GetCurrentUser(Page.User.Identity.Name);
            
            foreach (RepeaterItem row in repShoppingCart.Items)
            {
                TextBox txtQuantity = (TextBox) row.FindControl("txtQuantity");
                ImageButton btnDelete = (ImageButton) row.FindControl("btnDelete");
                
                int quantity;
                if (int.TryParse(WebUtility.InputText(txtQuantity.Text, 10), out quantity))
                {

                    if (quantity > 0)
                        profile.ShoppingCart.SetQuantity(btnDelete.CommandArgument, quantity);
                    else if (quantity == 0)
                        profile.ShoppingCart.Remove(btnDelete.CommandArgument);
                }
            }

            profile = profile.Save();

            BindCart();
        }

        /// <summary>
        /// Handler for Delete/Move buttons
        /// </summary>
        protected void CartItem_Command(object sender, CommandEventArgs e)
        {
            Profile profile = ProfileManager.Instance.GetCurrentUser(Page.User.Identity.Name);
            switch (e.CommandName)
            {
                case "Del":
                    profile.ShoppingCart.Remove(e.CommandArgument.ToString());
                    break;
                case "Move":
                    profile.ShoppingCart.Remove(e.CommandArgument.ToString());
                    profile.WishList.Add(e.CommandArgument.ToString(), profile.UniqueID, false);
                    break;
            }

            profile = profile.Save();
         
            BindCart();
        }
    }
}