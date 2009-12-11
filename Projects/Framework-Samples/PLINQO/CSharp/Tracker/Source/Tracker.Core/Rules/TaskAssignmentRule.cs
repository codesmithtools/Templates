using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Web;
using CodeSmith.Data.Attributes;
using CodeSmith.Data.Rules;
using System.Data.Linq;
using Tracker.Core.Data;

namespace Tracker.Core.Rules
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

                using (var db = new TrackerDataContext())
                {
                    var options = new DataLoadOptions();
                    options.LoadWith<UserRole>(r => r.Role);
                    db.LoadOptions = options;

                    User currentUser = db.User.GetByEmailAddress(HttpContext.Current.User.Identity.Name);
                    context.Success = (null == db.UserRole.GetUserRole("Newb", currentUser.Id));
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
