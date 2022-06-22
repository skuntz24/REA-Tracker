using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.Security;
using System.Timers;
using System.Net;
using System.Net.Sockets;
using QVICommonIntranet.Database;
using System.Configuration;

namespace REA_Tracker
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            string NAME_OF_PRODUCTION_SERVER = "SWN-WE01";
            //"SWN-WE01";
            try
            {
                EventLog evtLog = new EventLog();
                evtLog.Source = "REA Tracker";
                evtLog.WriteEntry("Starting REA Tracker");
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            REATrackerDB sql = new REATrackerDB();

            Application.Add("ReleaseMode", (NAME_OF_PRODUCTION_SERVER == Server.MachineName));
            //"Version"
            Application.Add("Version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            //"Application"
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            Application.Add("Name", ((AssemblyTitleAttribute)attributes[0]).Title.ToString());
            //"Copyright"
            AssemblyCopyrightAttribute objCopyright = (AssemblyCopyrightAttribute)AssemblyCopyrightAttribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute));
            Application.Add("Copyright", objCopyright.Copyright.ToString());
            Application.Add("LockMessage", "REA Tracker is currently under maintenance.");
            Boolean locked = Convert.ToBoolean(Convert.ToInt16(sql.ProcessCommand("SELECT VALUE FROM REA_Application WHERE VARIABLE ='LOCKED_STATE';").Rows[0][0]));
            Application.Add("LockStatus", locked);

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Session_Start " + Session.SessionID.ToString());
            bool islocked = Convert.ToBoolean(Application["LockStatus"]);
            //EmailSCRTracker.ConnectionString = ConfigurationManager.ConnectionStrings["btracker"].ConnectionString;


        }
        protected void Session_End(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Session_End " + Session.SessionID.ToString());
        }

    }
}
