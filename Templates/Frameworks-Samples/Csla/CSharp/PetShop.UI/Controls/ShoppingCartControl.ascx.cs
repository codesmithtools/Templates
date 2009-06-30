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
            if (string.IsNullOrEmpty(Page.User.Identity.Name))
                Response.Redirect("~/SignIn.aspx");

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
            Profile profile = Profile.GetProfile(Page.User.Identity.Name);
            if (!string.IsNullOrEmpty(profile.Username))
            {
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
        }

        /// <summary>
        /// Recalculate the total
        /// </summary>
        private void PrintTotal()
        {
            Profile profile = Profile.GetProfile(Page.User.Identity.Name);
            if (!string.IsNullOrEmpty(profile.Username))
            {
                if (profile.ShoppingCart.Count > 0)
                    ltlTotal.Text = profile.ShoppingCart.Total.ToString("c");
            }
        }

        /// <summary>
        /// Calculate total
        /// </summary>
        protected void BtnTotal_Click(object sender, ImageClickEventArgs e)
        {
            Profile profile = Profile.GetProfile(Page.User.Identity.Name);
            if (!string.IsNullOrEmpty(profile.Username))
            {
                TextBox txtQuantity;
                ImageButton btnDelete;
                int qty = 0;
                foreach (RepeaterItem row in repShoppingCart.Items)
                {
                    txtQuantity = (TextBox) row.FindControl("txtQuantity");
                    btnDelete = (ImageButton) row.FindControl("btnDelete");
                    if (int.TryParse(WebUtility.InputText(txtQuantity.Text, 10), out qty))
                    {

                        if (qty > 0)
                            profile.ShoppingCart.SetQuantity(btnDelete.CommandArgument, qty);
                        else if (qty == 0)
                            profile.ShoppingCart.Remove(btnDelete.CommandArgument);
                    }
                }

                profile = profile.Save(true);
            }

            BindCart();
        }

        /// <summary>
        /// Handler for Delete/Move buttons
        /// </summary>
        protected void CartItem_Command(object sender, CommandEventArgs e)
        {
            Profile profile = Profile.GetProfile(Page.User.Identity.Name);
            if (!string.IsNullOrEmpty(profile.Username))
            {
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

                profile = profile.Save(true);
            }

            BindCart();
        }
    }
}