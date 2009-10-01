using System;
using PetShop.Core.Data;

namespace PetShop.UI
{
    public partial class NewUser : System.Web.UI.Page
    {
        protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
        {
            using (var context = new PetShopDataContext())
            {
                string userName = ((System.Web.UI.WebControls.CreateUserWizard)sender).UserName;
                var profile = context.Profile.GetProfile(userName);
                if (null == profile)
                {
                    profile = new Profile();
                    profile.Username = userName;
                    profile.ApplicationName = ".NET Pet Shop 4.0";
                    profile.IsAnonymous = false;
                    profile.LastActivityDate = DateTime.Now;
                    profile.LastUpdatedDate = DateTime.Now;
                    context.Profile.InsertOnSubmit(profile);
                    context.SubmitChanges();
                }
            }
        }
    }
}
