using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using System.Xml.Xsl;
using Tracker.Core.Data;
using IMultipleResults=System.Data.Linq.IMultipleResults;
using System.Data.Linq;
using System.Security.Principal;


namespace PLINQO.Mvc.UI
{
    public class UIHelper
    {
        public static User GetCurrentUser()
        {
            var user = new User();

            string currentUserName = String.Empty;

            IPrincipal currentUser = null;
            HttpContext current = HttpContext.Current;
            if (current != null)
                currentUser = current.User;

            if ((currentUser != null) && (currentUser.Identity != null))
                currentUserName = currentUser.Identity.Name;

            using (var context = new TrackerDataContext())
            {
                context.ObjectTrackingEnabled = false;
                user = context.User.GetByEmailAddress(currentUserName);
            }

            return user;
        }
        
        public static List<Audit> TransformAudits(List<Audit> audits)
        {
            var xslt = new XslCompiledTransform();
            xslt.Load(HttpContext.Current.Server.MapPath(@"/lib/xslts/auditnote.xslt"));
            foreach (Audit audit in audits)
            {
                var buffer = new StringBuilder();
                using (var sr = new StringReader(audit.Content))
                using (var xmlReader = XmlReader.Create(sr))
                using (var writer = new StringWriter(buffer))
                {
                    var args = new XsltArgumentList();
                    args.AddParam("age", string.Empty, audit.CreatedDate.ToAgeString(1));
                    xslt.Transform(xmlReader, args, writer);
                    audit.HtmlContent = buffer.ToString();
                }
                
            }
            return audits;
        }

        public static void RefreshData()
        {
            using (var context = new TrackerDataContext())
            {
                IMultipleResults results = context.ExecuteQuery(
                    context.User.OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                    , context.Status.OrderBy(s => s.Name)
                    , context.Role.OrderBy(r => r.Name));

                HttpRuntime.Cache.Insert("Users", results.GetResult<User>().ToList());
                HttpRuntime.Cache.Insert("Statuses", results.GetResult<Status>().ToList());
                HttpRuntime.Cache.Insert("Roles", results.GetResult<Role>().ToList());
            }
        }

        public static SelectList GetRoleSelectList(int? selectedValue, List<Role> userRoles)
        {
            var roles = HttpRuntime.Cache["Roles"] as List<Role>;
            if (null == roles)
            {
                RefreshData();
                roles = (HttpRuntime.Cache["Roles"] as List<Role>);
            }

            var filteredRoles = new List<Role>();
            roles.ForEach(delegate(Role role)
            {
              if (!userRoles.Exists(ur => ur.Id == role.Id))
                  filteredRoles.Add(role);
            });

            return new SelectList(filteredRoles, "Id", "Name", selectedValue);
        }

        public static SelectList GetPrioritySelectList(Priority selectedValue)
        {
            var selectListItems = new List<SelectListItem>();
            foreach (string priority in Enum.GetNames(typeof(Priority)))
            {
                var selectListItem = new SelectListItem()
                                         {
                                             Text = priority,
                                         };

                selectListItems.Add(selectListItem);
            }
            var selectList = new SelectList(selectListItems, "Text", "Text", selectedValue);

            return selectList;
        }

        public static SelectList GetUserSelectList(int? selectedValue)
        {
            var users = HttpRuntime.Cache["Users"] as List<User>;
            if (null == users)
            {
                RefreshData();
                users = HttpRuntime.Cache["Users"] as List<User>;
            }
            return new SelectList(users, "Id", "FullName", selectedValue);
        }

        public static SelectList GetStatusSelectList(int? selectedValue)
        {
            var statuses = (List<Status>)HttpRuntime.Cache["Statuses"] as List<Status>;
            if (null == statuses)
            {
                RefreshData();
                statuses = HttpRuntime.Cache["Statuses"] as List<Status>;
            }
            return new SelectList(statuses, "Id", "Name", selectedValue);
        }

        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}
