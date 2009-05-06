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
using PLINQO.Tracker.Data;


namespace PLINQO.Mvc.UI.Controllers
{
    [HandleError]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to PLINQO!";
            
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

    }
}
