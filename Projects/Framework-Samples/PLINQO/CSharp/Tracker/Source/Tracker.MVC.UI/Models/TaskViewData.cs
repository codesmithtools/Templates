using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PLINQO.Mvc.UI.Binder;
using PLINQO.Tracker.Data;

namespace PLINQO.Mvc.UI.Models
{
    [ModelBinder(typeof(TaskViewDataBinder))]
    public class TaskViewData
    {
        public Task Task;
        public SelectList Statuses;
        public SelectList Priorities;
        public SelectList CreatedUsers;
        public SelectList AssignedUsers;
        public List<Audit> Audits;
    }
}
