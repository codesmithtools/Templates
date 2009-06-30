﻿using System;
using PetShop.Business;

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
                    Profile profile = Profile.GetProfile(Page.User.Identity.Name);
                    if (!string.IsNullOrEmpty(profile.Username))
                    {
                        profile.WishList.Add(itemId, profile.UniqueID, false);
                        profile = profile.Save();
                    }

                    // Redirect to prevent duplictations in the wish list if user hits "Refresh"
                    Response.Redirect("~/WishList.aspx", true);
                }
            }
        }
    }
}
