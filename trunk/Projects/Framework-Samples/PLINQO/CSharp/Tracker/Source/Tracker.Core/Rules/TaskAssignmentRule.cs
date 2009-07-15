using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Web;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;
using System.Data.Linq;

namespace Tracker.Core.Data.Rules
{
    public class TaskAssignmentRule : IRule
    {

        #region IRule Members

        public string ErrorMessage
        {
            get { return "Newbs are not allowed edit tasks that are not assigned to them!"; }
        }

        public int Priority
        {
            get { return 1; }
        }

        public void Run(RuleContext context)
        {
            context.Success = true;

            if (HttpContext.Current != null)
            {
                var task = (Task) context.TrackedObject.Current;

                using (var dataContext = new TrackerDataContext())
                {
                    DataLoadOptions options = new DataLoadOptions();
                    options.LoadWith<UserRole>(r => r.Role);
                    dataContext.LoadOptions = options;

                    User currentUser = dataContext.User.ByEmailAddress(HttpContext.Current.User.Identity.Name);
                    context.Success = (null == dataContext.UserRole.GetUserRole("Newb", currentUser.Id));
                }
            }
        }

        public string TargetProperty
        {
            get { return String.Empty; }
        }

        #endregion
    }
}
