using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using CodeSmith.Data.Rules;
using PLINQO.Mvc.UI.Binder;
using PLINQO.Mvc.UI.Models;
using Tracker.Core.Data;
using System.Data.Linq;

namespace PLINQO.Mvc.UI.Controllers
{

    public class TaskController : BaseController
    {

        public ActionResult Ajax()
        {
            return View(UIHelper.GetUserSelectList(null));
        }

        public ActionResult Create()
        {
            return View(GetData());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([Bind(Exclude="Id")]Task task)
        {
            try
            {
                Nullable<System.DateTime> date = new DateTime();
                
                using (var context = new TrackerDataContext())
                {
                    context.Task.InsertOnSubmit(task);
                    context.SubmitChanges();

                    var audit = new Audit(context.LastAudit);
                    audit.Task = task;
                    context.Audit.InsertOnSubmit(audit);
                    context.SubmitChanges();
                }
                return RedirectToAction("Edit", new { id = task.Id });
            }
            catch (BrokenRuleException e)
            {
                foreach (BrokenRule rule in e.BrokenRules)
                    ModelState.AddModelError(rule.Context.Rule.TargetProperty, rule.Message);
                return View(GetData());
            }
        }

        public ActionResult CopyTask(int id)
        {
            Task copiedTask = null;
            using (var context = new TrackerDataContext())
            {
                Task task = context.Task.GetByKey(id);
                copiedTask = task.Clone();
                copiedTask.Id = 0;
                copiedTask.CreatedUser = context.User.GetByEmailAddress(System.Web.HttpContext.Current.User.Identity.Name);
                context.Task.InsertOnSubmit(copiedTask);
                context.SubmitChanges();
            }

            return RedirectToAction("Edit", new {id = copiedTask.Id});
        }

        public ActionResult Delete(int id)
        {
            using (var context = new TrackerDataContext())
            {
                context.Audit.Delete(a => a.TaskId == id);
                context.Task.Delete(id);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            Task task = null;
            using (var context = new TrackerDataContext())
            {
                var options = new DataLoadOptions();
                options.LoadWith<Task>(t => t.AssignedUser);
                options.LoadWith<Task>(t => t.CreatedUser);
                context.LoadOptions = options;
                task = context.Task.GetByKey(id);
            }
            return View(task);
        }

        public ActionResult Edit(int id)
        {
            return View(GetData(id));
        }

        [ActionName("Edit"), AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditPost(int id)
        {
            Task task = null;
            try
            {
                using (var context = new TrackerDataContext())
                {
                    var options = new DataLoadOptions();
                    options.LoadWith<Task>(t => t.CreatedUser);
                    context.LoadOptions = options;

                    task = context.Task.GetByKey(id);
                    UpdateModel(task);
                    context.SubmitChanges();

                    var audit = new Audit(context.LastAudit);
                    audit.Task = task;
                    context.Audit.InsertOnSubmit(audit);
                    context.SubmitChanges();
                }

                return RedirectToAction("Edit", new { id = task.Id });
            }
            catch (BrokenRuleException e)
            {
                foreach (BrokenRule rule in e.BrokenRules)
                    ModelState.AddModelError(rule.Context.Rule.TargetProperty, rule.Message);
                task.Detach();
                return View(GetData(task));
            }
        }

        public ActionResult Get(int id)
        {
            Task task = null;
            using(var context = new TrackerDataContext())
            {
                var options = new DataLoadOptions();
                options.LoadWith<Task>(t => t.CreatedUser);
                options.LoadWith<Task>(t => t.AssignedUser);
                context.LoadOptions = options;

                task = context.Task.GetByKey(id);
                task.Detach();
            }
            return Json(task);
        }

        public ActionResult Index()
        {
            var taskListViewData = new TaskListViewData();
            using (var context = new TrackerDataContext())
            {
                var options = new DataLoadOptions();
                options.LoadWith<Task>(t => t.AssignedUser);
                options.LoadWith<Task>(t => t.CreatedUser);
                context.LoadOptions = options;

                taskListViewData.Tasks = context.Task.OrderByDescending(t => t.CreatedDate).ToList();
                taskListViewData.Statuses = UIHelper.GetStatusSelectList(Status.NotStarted);
            }
            return View(taskListViewData);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateStatus(Status status, List<int> selectedTasks)
        {
            using (var context = new TrackerDataContext())
            {
                context.Task.Update(t => selectedTasks.Contains(t.Id), t2 => new Task { Status = status });
            }

            return RedirectToAction("Index");
        }

        #region "GetData"
        public TaskViewData GetData()
        {
            return GetData(null);
        }

        public TaskViewData GetData(int id)
        {
            using (var context = new TrackerDataContext())
            {
                var options = new DataLoadOptions();
                options.LoadWith<Task>(t => t.AssignedUser);
                options.LoadWith<Task>(t => t.CreatedUser);
                context.LoadOptions = options;
                context.ObjectTrackingEnabled = false;
                var task = context.Task.GetByKey(id);
                return GetData(task);
            }
        }

        public TaskViewData GetData(Task task)
        {
            var taskViewData = new TaskViewData();

            if( null == task)
                task = new Task();

            taskViewData.Task = task;

            taskViewData.AssignedUsers = UIHelper.GetUserSelectList(task.AssignedId);
            taskViewData.CreatedUsers = UIHelper.GetUserSelectList(task.CreatedId);
            taskViewData.Statuses = UIHelper.GetStatusSelectList(task.Status);
            taskViewData.Priorities = UIHelper.GetPrioritySelectList(task.Priority);

            using (var context = new TrackerDataContext())
            {
                context.ObjectTrackingEnabled = false;
                taskViewData.Audits = UIHelper.TransformAudits(context.Audit.ByTaskId(task.Id).OrderByDescending(a => a.Date).ToList());
            }
            return taskViewData;
        }

        #endregion

    }
}
