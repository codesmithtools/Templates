using System;
using System.Diagnostics;
using System.Web.Profile;
using System.Web.Security;
using PetShop.Business;

using PB = PetShop.Business;
using ProfileManager=System.Web.Profile.ProfileManager;

namespace PetShop.UI
{
    public class Global : System.Web.HttpApplication
    {
        // Carry over profile property values from an anonymous to an authenticated state 
        protected void Profile_MigrateAnonymous(Object sender, ProfileMigrateEventArgs e)
        {
            Profile anonymousProfile = PB.ProfileManager.Instance.GetAnonymousUser();
            Profile profile = PB.ProfileManager.Instance.GetCurrentUser(e.Context.User.Identity.Name);

            //Merge anonymous shopping cart items to the authenticated shopping cart items
            foreach (Cart item in anonymousProfile.ShoppingCart)
                profile.ShoppingCart.Add(item.ItemId, profile.UniqueID, true, item.Quantity);

            //Merge anonymous wishlist items to the authenticated wishlist items
            foreach (Cart item in anonymousProfile.WishList)
                profile.WishList.Add(item.ItemId, profile.UniqueID, false, item.Quantity);

            profile = profile.Save();

            //Clear the cart.
            anonymousProfile.ShoppingCart.Clear();
            anonymousProfile.WishList.Clear();
            anonymousProfile = anonymousProfile.Save();

            // Clean up anonymous profile
            //ProfileManager.DeleteProfile(e.AnonymousID);
            //AnonymousIdentificationModule.ClearAnonymousIdentifier();
        }

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
            //EventLog.WriteEntry(".NET Pet Shop 4.0", exception.ToString(), EventLogEntryType.Error);
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}