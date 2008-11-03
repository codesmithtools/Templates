using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;
using System.Web.Routing;
using System.Web.DynamicData;
using Petshop.Data;

namespace Petshop.UI
{
    public class Global : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            MetaModel model = new MetaModel();

            
            model.RegisterContext(typeof(Petshop.Data.PetshopDataContext), new ContextConfiguration() { ScaffoldAllTables = false});

            routes.Add(new DynamicDataRoute("{table}/{action}.aspx")
            {
                Constraints = new RouteValueDictionary(new { action = "List|Details|Edit|Insert" }),
                Model = model
            });
        }

        void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
        }

    }
}
