using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using PetShop.Core.Controls;
using PetShop.Core.Data;
using System.Collections.Generic;
using PetShop.Core.Utility;

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
            using (var context = new PetShopDataContext())
            {
                var profile = context.Profile.GetProfile(Page.User.Identity.Name);
                if (!string.IsNullOrEmpty(profile.Username))
                {
                    List<Cart> items = profile.ShoppingCart;
                    items.ForEach(i => i.Detach());
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
        }

        /// <summary>
        /// Recalculate the total
        /// </summary>
        private void PrintTotal()
        {
            using (var context = new PetShopDataContext())
            {
                var profile = context.Profile.GetProfile(Page.User.Identity.Name);
                if (!string.IsNullOrEmpty(profile.Username))
                {
                    if (profile.ShoppingCart.Count > 0)
                        ltlTotal.Text = CartHelper.GetTotal(profile.ShoppingCart).ToString("c");
                }
            }
        }

        /// <summary>
        /// Calculate total
        /// </summary>
        protected void BtnTotal_Click(object sender, ImageClickEventArgs e)
        {
            using (var context = new PetShopDataContext())
            {
                var profile = context.Profile.GetProfile(Page.User.Identity.Name);
                if (!string.IsNullOrEmpty(profile.Username))
                {
                    TextBox txtQuantity;
                    ImageButton btnDelete;
                    int qty = 0;
                    foreach (RepeaterItem row in repShoppingCart.Items)
                    {
                        txtQuantity = (TextBox)row.FindControl("txtQuantity");
                        btnDelete = (ImageButton)row.FindControl("btnDelete");
                        if (int.TryParse(WebUtility.InputText(txtQuantity.Text, 10), out qty))
                        {

                            if (qty > 0)
                                CartHelper.SetQuantity(profile.ShoppingCart, btnDelete.CommandArgument, qty);
                            else if (qty == 0)
                                CartHelper.Remove(profile.ShoppingCart, btnDelete.CommandArgument);
                        }
                    }
                }
            }
            BindCart();
        }

        /// <summary>
        /// Handler for Delete/Move buttons
        /// </summary>
        protected void CartItem_Command(object sender, CommandEventArgs e)
        {
            using (var context = new PetShopDataContext())
            {
                var profile = context.Profile.GetProfile(Page.User.Identity.Name);
                if (!string.IsNullOrEmpty(profile.Username))
                {
                    switch (e.CommandName)
                    {
                        case "Del":
                            CartHelper.Remove(profile.ShoppingCart, e.CommandArgument.ToString());
                            break;
                        case "Move":
                            CartHelper.MoveToWishList(profile, e.CommandArgument.ToString());
                            break;
                    }
                }
            }
            BindCart();
        }
    }
}