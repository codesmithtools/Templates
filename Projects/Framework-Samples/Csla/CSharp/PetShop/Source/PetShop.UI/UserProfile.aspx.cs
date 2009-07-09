using System;
using PetShop.Business;

namespace PetShop.UI
{
    public partial class UserProfile : System.Web.UI.Page
    {
        /// <summary>
        /// Update profile
        /// </summary>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Profile profile = Profile.GetProfile(User.Identity.Name);
            if (!string.IsNullOrEmpty(profile.Username) && AddressForm.IsValid)
            {
                if (profile.Accounts.Count > 0)
                {
                    Account account = profile.Accounts[0];
                    UpdateAccount(ref account, AddressForm.Address);
                }
                else
                {
                    Account account = profile.Accounts.AddNew();
                    account.UniqueID = profile.UniqueID;

                    UpdateAccount(ref account, AddressForm.Address);
                }

                profile = profile.Save(true);
            }

            lblMessage.Text = "Your profile information has been successfully updated.<br>&nbsp;";
        }

        private void UpdateAccount(ref Account account, Address address)
        {
            account.FirstName = address.FirstName;
            account.LastName = address.LastName;
            account.Address1 = address.Address1;
            account.Address2 = address.Address2;
            account.City = address.City;
            account.State = address.State;
            account.Zip = address.Zip;
            account.Country = address.Country;
            account.Phone = address.Phone;
            account.Email = address.Email;
        }

        /// <summary>
        /// Handle Page load event 
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
                Response.Redirect("~/SignIn.aspx");

            if (!IsPostBack)
                BindUser();
        }

        /// <summary>
        /// Bind controls to profile
        /// </summary>
        private void BindUser()
        {
            Profile profile = Profile.GetProfile(User.Identity.Name);
            if(string.IsNullOrEmpty(profile.Username))
            {
                profile = Profile.NewProfile();
                profile.Username = User.Identity.Name;
                profile.ApplicationName = ".NET Pet Shop 4.0";
                profile.IsAnonymous = !User.Identity.IsAuthenticated;
                profile.LastActivityDate = DateTime.Now;
                profile.LastUpdatedDate = DateTime.Now;

                profile = profile.Save();
            }
            
            AddressForm.Address = new Address(profile);
        }
    }
}
