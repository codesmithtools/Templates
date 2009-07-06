using System;
using PetShop.Core.Data;
using PetShop.Core;
using PetShop.Core.Utility;

namespace PetShop.UI
{
    public partial class WishList : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string itemId = Request.QueryString["addItem"];
                if (!string.IsNullOrEmpty(itemId))
                {
                    Profile profile = null;
                    using (var context = new PetShopDataContext())
                    {
                        profile = context.Profile.GetProfile(Page.User.Identity.Name);
                        profile.Detach();
                    }

                    if (!string.IsNullOrEmpty(profile.Username))
                    {
                        CartHelper.Add(profile.WishList, itemId, profile.UniqueID, false);
                    }

                    // Redirect to prevent duplictations in the wish list if user hits "Refresh"
                    Response.Redirect("~/WishList.aspx", true);
                }
            }
        }
    }
}
