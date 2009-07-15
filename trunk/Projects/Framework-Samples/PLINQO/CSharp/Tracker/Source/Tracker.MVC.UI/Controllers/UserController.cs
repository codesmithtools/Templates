using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;
using System.Web.Security;
using CodeSmith.Data.Audit;
using CodeSmith.Data.Rules;
using PLINQO.Tracker.Data;
using Data = PLINQO.Tracker.Data;
using System.Data.Linq;
using IMultipleResults = System.Data.Linq.IMultipleResults;
using System.Transactions;

namespace PLINQO.Mvc.UI.Controllers
{
    public class UserViewData
    {
        public User User;
        public SelectList Roles;
        public List<Audit> Audits;
    }

    public class UserController : BaseController
    {

        public UserController()
            : this(null)
        {
        }

        public UserController(IMembershipService service)
        {
            MembershipService = service ?? new AccountMembershipService();
        }

        public IMembershipService MembershipService
        {
            get;
            private set;
        }

        public ActionResult Ajax()
        {
            return View(UIHelper.GetUserSelectList(null));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddRole(int userId, int roleId)
        {
            using (var context = new TrackerDataContext())
            {
                var userRole = new UserRole() { UserId = userId, RoleId = roleId };
                context.UserRole.InsertOnSubmit(userRole);
                context.SubmitChanges();
            }
            return RedirectToAction("Edit", new { id = userId });
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /User/Create

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(string emailAddress, string password, string confirmPassword)
        {
            try
            {
                if (confirmPassword == password)
                {
                    MembershipCreateStatus createStatus = MembershipService.CreateUser(emailAddress, password, emailAddress);
                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        using (var context = new TrackerDataContext())
                        {
                            var user = context.User.ByEmailAddress(emailAddress);
                            TryUpdateModel(user, null, null, new string[] { "FirstName, LastName" });
                            context.SubmitChanges();
                            HttpRuntime.Cache.Remove("Users");
                            return RedirectToAction("Edit", new { id = user.Id });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("_FORM", UIHelper.ErrorCodeToString(createStatus));
                    }

                    return View();
                }
                else
                {
                    ModelState.AddModelError("Password", "Passwords must match");
                    return View();
                }
            }
            catch (BrokenRuleException e)
            {
                foreach (BrokenRule rule in e.BrokenRules)
                    ModelState.AddModelError(rule.Context.Rule.TargetProperty, rule.Message);
                return View();
            }
        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id)
        {
            Data.User user = null;
            using (var context = new TrackerDataContext())
            {
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<Data.User>(u => u.UserRoleList);
                options.LoadWith<Data.UserRole>(r => r.Role);
                context.LoadOptions = options;

                user = context.User.ByKey(id);

            }

            if (null == user)
                return RedirectToAction("Index");
            else
                return View(user);
        }

        public ActionResult Get(int id)
        {
            User user = null;
            using (var context = new TrackerDataContext())
            {
                context.DeferredLoadingEnabled = false;
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<Data.User>(u => u.UserRoleList);
                options.LoadWith<Data.UserRole>(ur => ur.Role);
                context.LoadOptions = options;

                user = context.User.ByKey(id);
                user.Detach();
            }
            return Json(user);
        }

        //
        // GET: /User/

        public ActionResult Index()
        {
            List<Tracker.Data.User> users = null;
            using (var context = new TrackerDataContext())
            {
                users = context.User.OrderBy(u => u.EmailAddress).ToList();
            }
            return View(users);
        }

        public ActionResult Delete(int id)
        {
            var user = UIHelper.GetCurrentUser();

            if (user != null && user.Id != id)
            {
                using (var context = new TrackerDataContext())
                using (var scope = new TransactionScope())
                {
                    context.UserRole.Delete(r => r.UserId == id);
                    context.Audit.Delete(a => a.UserId == id);

                    var q1 = from a in context.Audit
                             join t in context.Task on a.TaskId equals t.Id
                             join u in context.User on t.CreatedId equals u.Id into created
                             from x in created.DefaultIfEmpty()
                             join u in context.User on t.AssignedId equals u.Id into assigned
                             from y in assigned.DefaultIfEmpty()
                             where t.AssignedId != null || t.CreatedId != null
                             select a;

                    context.Audit.Delete(q1);
                    context.Task.Delete(a => a.CreatedId == id || a.AssignedId == id);
                    context.User.Delete(id);
                    scope.Complete();
                }
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id)
        {
            return View(GetData(id));
        }

        //
        // POST: /User/Edit/5

        [ActionName("Edit"), AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserEdit(int id)
        {
            User user = null;
            var userViewData = new UserViewData();
            try
            {
                using (var context = new TrackerDataContext())
                {
                    var options = new DataLoadOptions();
                    options.LoadWith<Data.User>(u => u.UserRoleList);
                    options.LoadWith<Data.UserRole>(u => u.Role);
                    context.LoadOptions = options;

                    user = context.User.ByKey(id);
                    UpdateModel(user);
                    context.SubmitChanges();

                    var audit = new Audit(context.LastAudit);
                    audit.User = user;
                    context.Audit.InsertOnSubmit(audit);
                    context.SubmitChanges();

                    HttpRuntime.Cache.Remove("Users");
                }
                return RedirectToAction("Edit", new { id = id });
            }
            catch (BrokenRuleException e)
            {
                foreach (BrokenRule rule in e.BrokenRules)
                    ModelState.AddModelError(rule.Context.Rule.TargetProperty, rule.Message);
                return View(GetData(user));
            }
        }


        public ActionResult RemoveAvatar(int id)
        {
            using (var context = new TrackerDataContext())
            {
                var user = context.User.ByKey(id);
                user.AvatarType = null;
                user.Avatar = null;
                context.SubmitChanges();
            }
            return RedirectToAction("Edit", new { id = id });
        }

        public ActionResult RemoveRole(int userId, int roleId)
        {
            using (var context = new TrackerDataContext())
            {
                context.UserRole.Delete(r => r.RoleId == roleId && r.UserId == userId);
            }
            return RedirectToAction("Edit", new { id = userId });
        }

        public UserViewData GetData(int userId)
        {
            using (var context = new TrackerDataContext())
            {
                var options = new DataLoadOptions();
                options.LoadWith<Data.User>(u => u.UserRoleList);
                options.LoadWith<Data.UserRole>(u => u.Role);
                context.LoadOptions = options;
                context.ObjectTrackingEnabled = false;

                return GetData(context.User.ByKey(userId));
            }
        }

        public UserViewData GetData(User user)
        {
            var userViewData = new UserViewData();
            userViewData.User = user;
            using (var context = new TrackerDataContext())
            {
                userViewData.Roles = UIHelper.GetRoleSelectList(null, user.RoleList.ToList());
                userViewData.Audits = UIHelper.TransformAudits(context.Audit.ByUserId(user.Id).OrderByDescending(a => a.Date).ToList());
            }
            return userViewData;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Avatar(int id)
        {
            User user = null;
            using (var context = new TrackerDataContext())
            {
                user = context.User.ByKey(id);
                user.Detach();
            }

            if (null == user || string.IsNullOrEmpty(user.AvatarType) || null == user.Avatar || user.Avatar.Length == 0)
                return File(Server.MapPath("/lib/images/anonymous.gif"), "image/gif");

            return File(user.Avatar.ToArray(), user.AvatarType);
        }


        [AcceptVerbs(HttpVerbs.Post), ActionName("Avatar")]
        public ActionResult AvatarPost(int id)
        {
            if (Request.Files.Count != 1)
            {
                using (var context = new TrackerDataContext())
                {
                    var user = context.User.ByKey(id);
                    ModelState.AddModelError("file", "Must select a file to upload.");
                    return View("Edit", GetData(user));
                }
            }

            var file = Request.Files[0];
            var buffer = new byte[file.ContentLength];
            file.InputStream.Read(buffer, 0, buffer.Length);

            using (var context = new TrackerDataContext())
            {
                var user = context.User.ByKey(id);
                user.Avatar = new Binary(buffer);
                user.AvatarType = file.ContentType;
                context.SubmitChanges();

                var audit = new Audit(context.LastAudit);
                audit.User = user;
                context.Audit.InsertOnSubmit(audit);
                context.SubmitChanges();

            }

            return RedirectToAction("Edit", new { id = id });
        }

    }
}
