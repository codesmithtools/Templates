using CodeSmith.Data.Linq;
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
using Tracker.Core.Data;
using Data = Tracker.Core.Data;
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
            using (var db = new TrackerDataContext())
            {
                var options = new DataLoadOptions();
                options.LoadWith<User>(u => u.UserRoleList);

                var user = db.User.GetByKey(userId);
                var role = db.Role.GetByKey(roleId);
                user.RoleList.Add(role);
                db.SubmitChanges();
            }
            return RedirectToAction("Edit", new { id = userId });
        }

        public ActionResult RemoveRole(int userId, int roleId)
        {
            using (var db = new TrackerDataContext())
            {
                var options = new DataLoadOptions();
                options.LoadWith<User>(u => u.UserRoleList);
                options.LoadWith<UserRole>(r => r.Role);
                db.LoadOptions = options;

                var user = db.User.GetByKey(userId);
                var role = user.RoleList.FirstOrDefault(r => r.Id == roleId);

                user.RoleList.Remove(role);
                db.SubmitChanges();
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
                        using (var db = new TrackerDataContext())
                        {
                            var user = db.User.GetByEmailAddress(emailAddress);
                            TryUpdateModel(user, null, null, new string[] { "FirstName, LastName" });
                            db.SubmitChanges();
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
            using (var db = new TrackerDataContext())
            {
                var options = new DataLoadOptions();
                options.LoadWith<Data.User>(u => u.UserRoleList);
                options.LoadWith<Data.UserRole>(r => r.Role);
                db.LoadOptions = options;

                user = db.User.GetByKey(id);

            }

            if (null == user)
                return RedirectToAction("Index");
            else
                return View(user);
        }

        public ActionResult Get(int id)
        {
            User user = null;
            using (var db = new TrackerDataContext())
            {
                db.DeferredLoadingEnabled = false;
                var options = new DataLoadOptions();
                options.LoadWith<Data.User>(u => u.UserRoleList);
                options.LoadWith<Data.UserRole>(ur => ur.Role);
                db.LoadOptions = options;

                user = db.User.GetByKey(id);
                user.Detach();
            }
            return Json(user);
        }

        //
        // GET: /User/

        public ActionResult Index()
        {
            List<User> users = null;
            using (var db = new TrackerDataContext())
            {
                users = db.User.OrderBy(u => u.EmailAddress).ToList();
            }
            return View(users);
        }

        public ActionResult Delete(int id)
        {
            var user = UIHelper.GetCurrentUser();

            if (user != null && user.Id != id)
            {
                using (var db = new TrackerDataContext())
                using (var scope = new TransactionScope())
                {
                    db.UserRole.Delete(r => r.UserId == id);
                    db.Audit.Delete(a => a.UserId == id);

                    var q1 = from a in db.Audit
                             join t in db.Task on a.TaskId equals t.Id
                             join u in db.User on t.CreatedId equals u.Id into created
                             from x in created.DefaultIfEmpty()
                             join u in db.User on t.AssignedId equals u.Id into assigned
                             from y in assigned.DefaultIfEmpty()
                             where t.AssignedId != null || t.CreatedId != null
                             select a;

                    db.Audit.Delete(q1);
                    db.Task.Delete(a => a.CreatedId == id || a.AssignedId == id);
                    db.User.Delete(id);
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
                using (var db = new TrackerDataContext())
                {
                    var options = new DataLoadOptions();
                    options.LoadWith<Data.User>(u => u.UserRoleList);
                    options.LoadWith<Data.UserRole>(u => u.Role);
                    db.LoadOptions = options;

                    user = db.User.GetByKey(id);
                    UpdateModel(user);

                    if (!db.GetChangeSet().Updates.Contains(user))
                    {
                        var trackedUser = new CodeSmith.Data.TrackedObject {Current = user, IsChanged = true};
                        var ruleManager = new RuleManager();
                        ruleManager.Run(trackedUser);

                        if (ruleManager.BrokenRules.Count > 0)
                            throw new BrokenRuleException(ruleManager.BrokenRules);
                    }

                    db.SubmitChanges();

                    var audit = new Audit(db.LastAudit);
                    audit.User = user;
                    db.Audit.InsertOnSubmit(audit);
                    db.SubmitChanges();
                }
                return RedirectToAction("Edit", new { id = id });
            }
            catch (BrokenRuleException e)
            {
                if (user != null)
                    user.Detach();

                foreach (BrokenRule rule in e.BrokenRules)
                    ModelState.AddModelError(rule.Context.Rule.TargetProperty, rule.Message);
                return View(GetData(user));
            }
        }


        public ActionResult RemoveAvatar(int id)
        {
            using (var db = new TrackerDataContext())
            {
                var user = db.User.GetByKey(id);
                user.AvatarType = null;
                user.Avatar = null;
                db.SubmitChanges();
            }
            return RedirectToAction("Edit", new { id = id });
        }


        public UserViewData GetData(int userId)
        {
            using (var db = new TrackerDataContext())
            {
                var options = new DataLoadOptions();
                options.LoadWith<Data.User>(u => u.Avatar);
                options.LoadWith<Data.User>(u => u.UserRoleList);
                options.LoadWith<Data.UserRole>(u => u.Role);
                db.LoadOptions = options;
                db.ObjectTrackingEnabled = false;

                return GetData(db.User.GetByKey(userId));
            }
        }

        public UserViewData GetData(User user)
        {
            var userViewData = new UserViewData();
            userViewData.User = user;
            using (var db = new TrackerDataContext())
            {
                userViewData.Roles = UIHelper.GetRoleSelectList(null, user.RoleList.ToList());
                userViewData.Audits = UIHelper.TransformAudits(db.Audit.ByUserId(user.Id).OrderByDescending(a => a.Date).ToList());
            }
            return userViewData;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Avatar(int id)
        {
            User user = null;
            using (var db = new TrackerDataContext())
            {
                var userAvatar = (from u in db.User
                             where u.Id == id
                             select new
                                        {
                                            u.Avatar,
                                            u.AvatarType
                                        }).FirstOrDefault();

                if (string.IsNullOrEmpty(userAvatar.AvatarType) || null == userAvatar.Avatar || userAvatar.Avatar.Length == 0)
                    return File(Server.MapPath("/lib/images/anonymous.gif"), "image/gif");

                return File(userAvatar.Avatar.ToArray(), userAvatar.AvatarType);
            }
        }


        [AcceptVerbs(HttpVerbs.Post), ActionName("Avatar")]
        public ActionResult AvatarPost(int id)
        {
            if (Request.Files.Count != 1)
            {
                using (var db = new TrackerDataContext())
                {
                    var user = db.User.GetByKey(id);
                    ModelState.AddModelError("file", "Must select a file to upload.");
                    return View("Edit", GetData(user));
                }
            }

            var file = Request.Files[0];
            var buffer = new byte[file.ContentLength];
            file.InputStream.Read(buffer, 0, buffer.Length);

            using (var db = new TrackerDataContext())
            {
                var user = db.User.GetByKey(id);
                user.Avatar = new Binary(buffer);
                user.AvatarType = file.ContentType;
                db.SubmitChanges();

                var audit = new Audit(db.LastAudit);
                audit.User = user;
                db.Audit.InsertOnSubmit(audit);
                db.SubmitChanges();
            }

            return RedirectToAction("Edit", new { id = id });
        }

    }
}
