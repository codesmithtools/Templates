using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Data.Rules;
using Tracker.Core.Data;

namespace Tracker.Core.Rules
{
    public class TaskStartDateDueDateRule : IRule
    {
        public int Priority
        {
            get { return 1; }
        }

        public string TargetProperty
        {
            get { return "StartDate and EndDate"; }
        }

        public string ErrorMessage
        {
            get { return "Start Date must be earlier than End Date"; }
        }

        public void Run(RuleContext context)
        {
            context.Message = ErrorMessage;

            DateTime? startDate = ((Task) context.TrackedObject.Current).StartDate;
            DateTime? dueDate = ((Task) context.TrackedObject.Current).DueDate;

            if ((null == startDate) || (null == dueDate))
                context.Success = true;
            else
                context.Success = (dueDate >= startDate);
        }
    }
}
