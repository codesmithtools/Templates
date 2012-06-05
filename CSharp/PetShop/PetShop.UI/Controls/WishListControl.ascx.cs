using System;
using System.Linq;
using System.Data.Linq;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using PetShop.Core.Utility;
using Petshop.Data;
using Petshop.Data.Entities;


namespace PetShop.UI.Controls
{
    public partial class WishListControl : System.Web.UI.UserControl
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
            var profile = new Profile();
            using (var context = new PetshopDataContext())
            {
                profile = context.Profile.GetProfile(Page.User.Identity.Name);
            }

            if (!string.IsNullOrEmpty(profile.Username))
            {
                List<Cart> wishList = profile.WishList;
                if (wishList.Count > 0)
                {
                    repWishList.DataSource = wishList;
                    repWishList.DataBind();
                }
                else
                {
                    repWishList.Visible = false;
                    lblMsg.Text = "Your wish list is empty.";
                }
            }
        }

        /// <summary>
        /// Handler for Delete/Move buttons
        /// </summary>
        protected void CartItem_Command(object sender, CommandEventArgs e)
        {
            var profile = new Profile();
            using (var context = new PetshopDataContext())
            {
                profile = context.Profile.GetProfile(Page.User.Identity.Name);
                context.Profile.Detach(profile);
            }
            if (!string.IsNullOrEmpty(profile.Username))
            {
                switch (e.CommandName)
                {
                    case "Del":
                        CartHelper.Remove(profile.WishList, e.CommandArgument.ToString());
                        break;
                    case "Move":
                        CartHelper.MoveToCart(profile, e.CommandArgument.ToString());
                        break;
                }
            }

            BindCart();
        }
    }
}
