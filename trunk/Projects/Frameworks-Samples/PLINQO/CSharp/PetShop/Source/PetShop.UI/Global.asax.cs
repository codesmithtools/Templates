using System;
using System.Diagnostics;

namespace PetShop.UI
{
    public class Global : System.Web.HttpApplication
    {
        //// Carry over profile property values from an anonymous to an authenticated state 
        //protected void Profile_MigrateAnonymous(Object sender, ProfileMigrateEventArgs e)
        //{
        //    int uniqueID;
        //    if(int.TryParse(e.AnonymousID, out uniqueID) == false)
        //    {
        //        return;
        //    }

        //    using( var context = new PetShopDataContext())
        //    {
        //      var profile = context.Profile.GetProfile(uniqueID);

        ////    // Merge anonymous shopping cart items to the authenticated shopping cart items
        //    foreach (CartItemInfo cartItem in profile.CartMember.CartItems)
        //        Profile.ShoppingCart.Add(cartItem);

        ////    // Merge anonymous wishlist items to the authenticated wishlist items
        //    foreach (CartItemInfo cartItem in profile.WishList.CartItems)
        //        Profile.WishList.Add(cartItem);

        ////    // Clean up anonymous profile
        //    ProfileManager.DeleteProfile(e.AnonymousID);
        //    AnonymousIdentificationModule.ClearAnonymousIdentifier();

        ////    // Save profile
        ////    profile = Profile.Save();
        ////}

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError().GetBaseException();
            EventLog.WriteEntry(".NET Pet Shop 4.0", exception.ToString(), EventLogEntryType.Error);
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}