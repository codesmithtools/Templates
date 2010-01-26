using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public FutureValue<int> TotalTasks
        {
            get;
            set;
        }

        public FutureValue<int> TotalTasksCreatedByMe
        {
            get;
            set;
        }

        public FutureValue<int> TotalTasksAssignedToMe
        {
            get;
            set;
        }

        public FutureValue<int> TotalTasksCompleted
        {
            get;
            set;
        }
    }
}
