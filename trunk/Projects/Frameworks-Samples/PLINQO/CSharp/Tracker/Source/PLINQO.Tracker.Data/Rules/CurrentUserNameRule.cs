using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Hosting;
using CodeSmith.Data.Rules;

namespace PLINQO.Tracker.Data.Rules
{
    public class CurrentUserNameRule : PropertyRuleBase
    {
        #region IRule Members

        public CurrentUserNameRule(string property)
            : base(property)
        {
            // lower priority because we need to assign before validate
            Priority = 10;
        }

        public CurrentUserNameRule(string property, EntityState assignState)
            : base(property, assignState)
        {
            // lower priority because we need to assign before validate
            Priority = 10;
        }
        #endregion

        public override void Run(RuleContext context)
        {
            context.Message = ErrorMessage;
            context.Success = true;
            // Only set if CanRun and if the value has not been manually changed.
            if (CanRun(context.TrackedObject) && !IsPropertyValueModified(context.TrackedObject.Original, context.TrackedObject.Current))
                SetPropertyValue(context.TrackedObject.Current, GetCurrentUserNameUserId());
        }

        private static int GetCurrentUserNameUserId()
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
                user = context.User.ByEmailAddress(currentUserName);
            }

            return user.Id;
        }
    }
}
