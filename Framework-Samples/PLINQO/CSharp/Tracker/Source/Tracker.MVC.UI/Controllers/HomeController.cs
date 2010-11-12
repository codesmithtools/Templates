using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Xsl;
using CodeSmith.Data.Audit;
using CodeSmith.Data.Linq;
using Tracker.Core.Data;
using Tracker.MVC.UI.Models;


namespace PLINQO.Mvc.UI.Controllers
{
    [HandleError]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to PLINQO!";

            if (User.Identity.IsAuthenticated)
            {

                var dashboard = new Dashboard();

                using (var db = new TrackerDataContext())
                {
                    dashboard.CurrentUser = db.User.GetByEmailAddress(User.Identity.Name);
                    if (dashboard.CurrentUser != null)
                    {
                        dashboard.TasksNotStarted = db.Task.ByAssignedId(dashboard.CurrentUser.Id).ByStatus(Status.NotStarted).Future();
                        dashboard.TasksInProgress = db.Task.ByAssignedId(dashboard.CurrentUser.Id).ByStatus(Status.InProgress).Future();
                        dashboard.TasksCompleted = db.Task.ByAssignedId(dashboard.CurrentUser.Id).ByStatus(Status.Completed).Future();
                        dashboard.TasksWaitingOnSomeone = db.Task.ByAssignedId(dashboard.CurrentUser.Id).ByStatus(Status.WaitingOnSomeoneElse).Future();
                        dashboard.TotalTasks = db.Task.FutureCount();
                        dashboard.TotalTasksCreatedByMe = db.Task.ByCreatedId(dashboard.CurrentUser.Id).FutureCount();
                        dashboard.TotalTasksAssignedToMe = db.Task.ByAssignedId(dashboard.CurrentUser.Id).FutureCount();
                        dashboard.TotalTasksCompleted = db.Task.ByStatus(Status.Completed).FutureCount();
                    }

                    db.ExecuteFutureQueries();
                }

                return View(dashboard);
            }
            else
            {
                return View();
            }
        }

        public ActionResult About()
        {
            return View();
        }

    }
}
