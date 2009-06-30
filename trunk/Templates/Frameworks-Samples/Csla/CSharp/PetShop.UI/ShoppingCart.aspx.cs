using System;
using System.Web.UI;

using PetShop.Business;

namespace PetShop.UI
{
    public partial class ShoppingCart : Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string itemId = Request.QueryString["addItem"];
                if (!string.IsNullOrEmpty(itemId))
                {
                    Profile profile = Profile.GetProfile(Page.User.Identity.Name);
                    if (!string.IsNullOrEmpty(profile.Username))
                    {
                        profile.ShoppingCart.Add(itemId, profile.UniqueID, true);
                        profile = profile.Save();
                    }
                    // Redirect to prevent duplictations in the cart if user hits "Refresh"
                    Response.Redirect("~/ShoppingCart.aspx", true);
                }
            }
        }
    }
}