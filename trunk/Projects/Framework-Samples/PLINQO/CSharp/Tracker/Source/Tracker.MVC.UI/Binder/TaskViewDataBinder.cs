using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.Mvc;
using PLINQO.Mvc.UI.Controllers;
using PLINQO.Mvc.UI.Models;
using Tracker.Core.Data;

namespace PLINQO.Mvc.UI.Binder
{
    public class TaskViewDataBinder : DefaultModelBinder, IModelBinder
    {
        #region IModelBinder Members

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var taskViewData = new TaskViewData();
            int? id = null;
            if (controllerContext.RouteData.Values["Id"].ToString().Length > 0)
                id = Int32.Parse(controllerContext.RouteData.Values["Id"].ToString());

            using (var db = new TrackerDataContext())
            {
                if (id != null)
                {
                    taskViewData.Task = db.Task.GetByKey(id.Value);
                    taskViewData.Task.Detach();
                }
                else
                {
                    taskViewData.Task = new Task();
                }
            }
            

            return taskViewData;
        }

        #endregion
    }
}
