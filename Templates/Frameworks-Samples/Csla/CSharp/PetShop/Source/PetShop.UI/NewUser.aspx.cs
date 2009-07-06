using System;
using PetShop.Business;

namespace PetShop.UI
{
    public partial class NewUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
        {
            Profile profile = Profile.GetProfile(User.Identity.Name);
            if (string.IsNullOrEmpty(profile.Username))
            {
                profile = Profile.NewProfile();
                profile.Username = User.Identity.Name;
                profile.ApplicationName = ".NET Pet Shop 4.0";
                profile.IsAnonymous = !User.Identity.IsAuthenticated;
                profile.LastActivityDate = DateTime.Now;
                profile.LastUpdatedDate = DateTime.Now;

                profile = profile.Save();
            }
        }
    }
}
