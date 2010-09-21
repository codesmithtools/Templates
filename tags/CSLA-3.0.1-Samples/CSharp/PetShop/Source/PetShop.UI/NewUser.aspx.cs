using System;
using PetShop.Business;

namespace PetShop.UI
{
    public partial class NewUser : System.Web.UI.Page
    {
        protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
        {
            string username = ((System.Web.UI.WebControls.CreateUserWizard) sender).UserName;

            ProfileManager.Instance.CreateUser(username, false);
        }
    }
}
