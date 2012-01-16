using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeSmith.Data.Future;
using CodeSmith.Data.Linq;
using Tracker.Core.Data;

namespace Tracker.MVC.UI.Models
{
    public class Dashboard
    {
        public User CurrentUser
        {
            get;
            set;
        }

        public IEnumerable<Task> TasksNotStarted
        {
            get;
            set;
        }

        public IEnumerable<Task> TasksCompleted
        {
            get;
            set;
        }

        public IEnumerable<Task> TasksInProgress
        {
            get;
            set;
        }

        public IEnumerable<Task> TasksWaitingOnSomeone
        {
            get;
            set;
        }

        public IFutureValue<int> TotalTasks
        {
            get;
            set;
        }

        public IFutureValue<int> TotalTasksCreatedByMe
        {
            get;
            set;
        }

        public IFutureValue<int> TotalTasksAssignedToMe
        {
            get;
            set;
        }

        public IFutureValue<int> TotalTasksCompleted
        {
            get;
            set;
        }
    }
}
