using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Data.Rules;

namespace Tracker.Core.Data.Rules
{
    public class UserExistsRule : IRule
    {

        #region IRule Members

        public string ErrorMessage
        {
            get { return "UserName already exists"; }
        }

        public int Priority
        {
            get { return 1; }
        }

        public void Run(RuleContext context)
        {
            var currentUser = (User)context.TrackedObject.Current;

            using (var dataContext = new TrackerDataContext())
            {
                dataContext.ObjectTrackingEnabled = false;
                var user = dataContext.User.ByEmailAddress(currentUser.EmailAddress);
                context.Success = (null == user || user.Id == currentUser.Id);
            }
        }

        public string TargetProperty
        {
            get { return "Username"; }
        }

        #endregion
    }
}
