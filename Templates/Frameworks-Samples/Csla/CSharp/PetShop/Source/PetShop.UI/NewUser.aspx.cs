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
            string userName = ((System.Web.UI.WebControls.CreateUserWizard)sender).UserName;
            Profile profile = Profile.GetProfile(userName);
            if (string.IsNullOrEmpty(profile.Username))
            {
                profile = Profile.NewProfile();
                profile.Username = userName;
                profile.ApplicationName = ".NET Pet Shop 4.0";
                profile.IsAnonymous = !User.Identity.IsAuthenticated;
                profile.LastActivityDate = DateTime.Now;
                profile.LastUpdatedDate = DateTime.Now;

                profile = profile.Save();
            }
        }
    }
}
