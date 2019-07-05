using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using rkrApp.Models;
using System.Data.Entity.Migrations;
using System.Diagnostics;

namespace rkrApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = Int16.MaxValue;
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //protected void Session_Start(Object sender, EventArgs e) {
            
        //}

        //protected void Session_End(object sender, EventArgs e)
        // {
            
        //    if (Session["user"] != null)
        //    {
        //        rkrDBEntities db = new rkrDBEntities();
        //        int user_id = Convert.ToInt32(Session["user"]);
        //        Sessions ss = db.Sessions.OrderByDescending(p => p.date_enter).FirstOrDefault(p => p.id_user == user_id);
        //        ss.date_exit = DateTime.Now;
        //        db.Sessions.AddOrUpdate(ss);
        //        db.SaveChanges();
        //    }

        //}

    }
}
