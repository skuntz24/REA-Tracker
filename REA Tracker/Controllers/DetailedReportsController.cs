using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REA_Tracker.Models;
using QVICommonIntranet.Database;

namespace REA_Tracker.Controllers
{
    [Authorize]
    public class DetailedReportsController : Controller
    {
        public static List<int> access = new List<int> { 43, 254, 170, 67, 137 };
        // GET: DetailedReports
        [HttpGet]
        public ActionResult Index(int? id)
        {
            DisplayReportsViewModel model = 
                new DisplayReportsViewModel(
                    Convert.ToInt32(Session["st_userID"]),
                    User.IsInRole("Administrator"),
                     Convert.ToInt32(Session["CurrentProduct"])
                    );
            ViewBag.Message = "";
            ViewBag.Users = access;
            return View(model);
        }
        //post
        [HttpPost]
        public ActionResult Index(DisplayReportsViewModel model, string button, string product) 
        {
            if (product != null && button == null)
            {
                Session["CurrentProduct"] = Convert.ToInt32(product);
            }
            else 
            {
                int reportID = -1;
                Int32.TryParse(button, out reportID);
                model.USER_ID = Convert.ToInt32(Session["st_userID"]);
                model.CurrentProduct = Convert.ToInt32(Session["CurrentProduct"]);
                model.DeleteReport(reportID);
                ViewBag.Message = Resources.strings.qviSuccess;
            }
            model =
                   new DisplayReportsViewModel(
                       Convert.ToInt32(Session["st_userID"]),
                       User.IsInRole("Administrator"),
                        Convert.ToInt32(Session["CurrentProduct"])
                       );
            ViewBag.Users = access;
            return View(model);
        }

        public ActionResult Display(int ? id) 
        {
            REATrackerDB sql = new REATrackerDB();
            SearchViewModel model;
            if (id == null)
            {
                model = new SearchViewModel(Convert.ToInt32(Session["st_userID"]), Convert.ToInt32(Session["CurrentProduct"]));
                ViewBag.IsManagerOrAdmin = model.isAdminOrManager(Convert.ToInt32(Session["st_userID"]));
                model.populateGraph();
            }
            else 
            {
                model = new SearchViewModel(Convert.ToInt32(id), 
                        Convert.ToInt32(Session["st_userID"]),
                        Convert.ToInt32(Session["CurrentProduct"])
                        );
                int rowcount = model.ProcessSearch();
                ViewBag.RowCount = rowcount;
                REATrackerDB sqlHelp = new REATrackerDB();
                ViewBag.ReportTitle = sqlHelp.ProcessScalarCommand("SELECT NAME FROM REA_TABULAR_REPORT WHERE REPORT_ID = "+model.ReportID);
                ViewBag.IsManagerOrAdmin = model.isAdminOrManager(Convert.ToInt32(Session["st_userID"]));
                model.populateGraph();
            }
            return View(model);
        }

        public ActionResult Expanded(int ? id)
        {
            REATrackerDB sql = new REATrackerDB();
            SearchViewModel model;
            if (id == null)
            {
                model = new SearchViewModel(Convert.ToInt32(Session["st_userID"]), Convert.ToInt32(Session["CurrentProduct"]));
                ViewBag.IsManagerOrAdmin = model.isAdminOrManager(Convert.ToInt32(Session["st_userID"]));
                model.populateGraph();
            }
            else
            {
                model = new SearchViewModel(Convert.ToInt32(id),
                        Convert.ToInt32(Session["st_userID"]),
                        Convert.ToInt32(Session["CurrentProduct"])
                        );
                int rowcount = model.ProcessSearch();
                ViewBag.RowCount = rowcount;
                REATrackerDB sqlHelp = new REATrackerDB();
                ViewBag.ReportTitle = sqlHelp.ProcessScalarCommand("SELECT NAME FROM REA_TABULAR_REPORT WHERE REPORT_ID = " + model.ReportID);
                ViewBag.IsManagerOrAdmin = model.isAdminOrManager(Convert.ToInt32(Session["st_userID"]));
                model.populateGraph();
                model.populateHistory();
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult MetricHistory()
        {
            MetricHistoryViewModel model = new MetricHistoryViewModel(Convert.ToInt32(Session["st_userID"])); 
            return View(model);
        }

        [HttpPost]
        public ActionResult MetricHistory( MetricHistoryViewModel model ) 
        {
            model.userID = Convert.ToInt32(Session["st_userID"]);
            model.populateProducts();
            model.populateCharts();
            model.GetRates();

            model.MonthCount = 12;
            model.DayCount = 31;
            model.YearCount = 1990;

            return View(model);
        }

        [HttpGet]
        public ActionResult MetricReport(String type) 
        {
            MetricReportViewModel model = new MetricReportViewModel(Convert.ToInt32(Session["st_userID"]), type);
            if (type != null && type.ToLower() != "problems")
            {
                ViewBag.TitleReport = Resources.strings.qviMetricReportOfAllSCRs;
            }
            else
            {
                ViewBag.TitleReport = Resources.strings.qviMetricReportOfProblemSCRs;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult MetricReport( MetricReportViewModel model, String type ) 
        {
            model.Process(Convert.ToInt32(Session["st_userID"]), type);
            ViewBag.TitleReport = Resources.strings.qviMetricReport;
            ViewBag.TitleReport += (model.SelectedProduct!=-1 ? (" for " + model.ProductName): "");
            if (type != null && type.ToLower() == "problems" )
            {
                ViewBag.TitleReport += " of Problem REAs";
            }
            else
            {
                ViewBag.TitleReport += " of All REAs";
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Kanban(KanbanViewModel model)
        {
            return View(model);
        }

        [HttpGet]
        public ActionResult Kanban(int? id)
        {
            KanbanViewModel model = new KanbanViewModel();
            model.UserID = Convert.ToInt32(Session["st_userID"]);
            model.CurrentProductID = Convert.ToInt32(Session["CurrentProduct"]);
            if (id == null)
            {
                id = 0;
            }
            model.ReportID = (int)id;
            model.CreateKanban();

            return View(model);
        }

    }
}