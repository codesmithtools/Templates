using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tracker.Core.Data;

namespace PLINQO.Mvc.UI.Models
{
    public class TaskListViewData
    {
        public List<Task> Tasks;
        public SelectList Statuses;
    }
}
