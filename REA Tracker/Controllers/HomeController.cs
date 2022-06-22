using REA_Tracker.Models;
using REA_Tracker.Models.Home;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using QVICommonIntranet.Database;
using QVICommonIntranet.Email.EmailREATracker;
using HelperMethods.Infrastructure;


namespace REA_Tracker.Controllers
{
    public class HomeController : Controller
    {
        private REATrackerDB sql = new REATrackerDB();
        private int _currentProduct = 9;


        //=====================================================================
        //============================== Index ================================
        //=====================================================================
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            HomeIndexViewModel model = new HomeIndexViewModel(Convert.ToInt32(Session["st_userID"]));
            //model.feedResult = rssFeedForHome(); should we add a rss feed y/n?
            model.CodeReviewWatchList(Convert.ToInt32(Session["st_userID"]));
            ViewBag.tab = 0;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(HomeIndexViewModel model, String button)
        {
            bool new_model = false;

            if (button.Equals("RemoveWatchList"))
            {
                model.remove();
                ModelState.Clear();
                new_model = true;
            }
            if (button.ToLower().Equals("removeclosed"))
            {
                model.removeClosed();
                new_model = true;
            }
            if (new_model)
            {
                HomeIndexViewModel m = new HomeIndexViewModel(Convert.ToInt32(Session["st_userID"]));
                m.CodeReviewWatchList(Convert.ToInt32(Session["st_userID"]));
                ViewBag.tab = 1;
                return View(m);

            }
            return RedirectToAction("Index", "Home");
        }

        private FeedResult rssFeedForHome()
        {
            NewsManager newsManager = new NewsManager();
            var postItems = newsManager.GenerateNews()
                .Select(p => new SyndicationItem(p.Title, p.Body, new Uri(p.Url)));

            string link = Request.Url.GetLeftPart(UriPartial.Authority).ToString();

            var feed = new SyndicationFeed("REA Tracker", "View Currently Assigned REA", new Uri(link), postItems)
            {
                Language = "en-US"
            };

            return new FeedResult(new Rss20FeedFormatter(feed));
        }

        //=====================================================================
        //=============================== Help ================================
        //=====================================================================
        /// <summary>
        /// This triggers when the user goes to the help page
        /// </summary>
        /// <param name="tab">optional anchor name to go directly to the tab (e.g. ?tab=iso)</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Help(String tab)
        {
            ViewBag.tab = tab;
            HelpViewModel model = new HelpViewModel();
            return View(model);
        }

        /// <summary>
        /// This post triggers when the user wants to send a message via the Contact tab
        /// </summary>
        /// <param name="model">current model</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Help(HelpViewModel model)
        {
            model.send();
            return View(model);
        }

        //=====================================================================
        //=========================== Dashboard ===============================
        //=====================================================================
        //DashBoard
        private void CalculateDashboardStatus()
        {
            using (System.Data.DataTable dt = sql.GetStatusMetrics(_currentProduct))
            {
                //Status Table
                ViewBag.Status = new List<dynamic>();
                int i = 0;
                foreach (System.Data.DataColumn column in dt.Columns)
                {
                    ViewBag.Status.Add(new System.Dynamic.ExpandoObject());
                    ViewBag.Status[i].Name = column.ColumnName;
                    ViewBag.Status[i].Value = dt.Rows[0][i].ToString();
                    ViewBag.Status[i].Type = i;
                    i++;
                }

                ViewBag.Status[1].Type = 17; //Approved
            }

            //ViewBag.Status.Add(new System.Dynamic.ExpandoObject());
            //ViewBag.Status[0].Name = "Submitted";
            //ViewBag.Status[0].Value = "6";

            //ViewBag.Status.Add(new System.Dynamic.ExpandoObject());
            //ViewBag.Status[1].Name = "In Process";
            //ViewBag.Status[1].Value = "9";

            //ViewBag.Status.Add(new System.Dynamic.ExpandoObject());
            //ViewBag.Status[2].Name = "Fixed or Delivered";
            //ViewBag.Status[2].Value = "5";

            //ViewBag.Status.Add(new System.Dynamic.ExpandoObject());
            //ViewBag.Status[3].Name = "Testing";
            //ViewBag.Status[3].Value = "12";

            //ViewBag.Status.Add(new System.Dynamic.ExpandoObject());
            //ViewBag.Status[4].Name = "Deferred";
            //ViewBag.Status[4].Value = "0";

        }

        private void CalculateDashboardRates()
        {
            using (System.Data.DataTable dt = sql.GetRates(_currentProduct))
            {
                double r1 = 0, r2 = 0, r3 = 0;

                if (dt.Rows.Count > 0) { r1 = double.Parse(dt.Rows[0][2].ToString()); }
                if (dt.Rows.Count > 1) { r2 = double.Parse(dt.Rows[1][2].ToString()); }
                if (dt.Rows.Count > 2) { r3 = double.Parse(dt.Rows[2][2].ToString()); }

                ViewBag.Rates = new List<dynamic>();
                ViewBag.Rates.Add(new System.Dynamic.ExpandoObject());
                ViewBag.Rates[0].Name = Resources.strings.qviSubmittedPerDay;
                ViewBag.Rates[0].Value = r1.ToString("F2");
                ViewBag.Rates[0].Change = "+??%";
                ViewBag.Rates[0].ChangeColor = "";

                ViewBag.Rates.Add(new System.Dynamic.ExpandoObject());
                ViewBag.Rates[1].Name = Resources.strings.qviResolvedPerDay;
                ViewBag.Rates[1].Value = r2.ToString("F2");
                ViewBag.Rates[1].Change = "+??%";
                ViewBag.Rates[1].ChangeColor = "";

                ViewBag.Rates.Add(new System.Dynamic.ExpandoObject());
                ViewBag.Rates[2].Name = Resources.strings.qviClosedPerDay;
                ViewBag.Rates[2].Value = r3.ToString("F2");
                ViewBag.Rates[2].Change = "+??%";
                ViewBag.Rates[2].ChangeColor = "";

            }
        }

        private void CalculateDashboardMetrics(int Critical, int High, int Medium, int Low)
        {
            using (System.Data.DataTable dt = sql.GetPriorityMetrics(_currentProduct))
            {
                //Status Table
                ViewBag.Metrics = new List<dynamic>();
                int nTotalCount = 0, nTotalMetric = 0, i = 0;
                ViewBag.Metrics.Add(new System.Dynamic.ExpandoObject());
                ViewBag.Metrics[0].Name = Resources.strings.qviCritical;
                ViewBag.Metrics[0].Count = Critical;
                ViewBag.Metrics[0].Metric = 0;
                ViewBag.Metrics[0].PriorityID = 3;

                ViewBag.Metrics.Add(new System.Dynamic.ExpandoObject());
                ViewBag.Metrics[1].Name = Resources.strings.qviHigh;
                ViewBag.Metrics[1].Count = High;
                ViewBag.Metrics[1].Metric = 0;
                ViewBag.Metrics[1].PriorityID = 2;

                ViewBag.Metrics.Add(new System.Dynamic.ExpandoObject());
                ViewBag.Metrics[2].Name = Resources.strings.qviMedium;
                ViewBag.Metrics[2].Count = Medium;
                ViewBag.Metrics[2].Metric = 0;
                ViewBag.Metrics[2].PriorityID = 1;

                ViewBag.Metrics.Add(new System.Dynamic.ExpandoObject());
                ViewBag.Metrics[3].Name = Resources.strings.qviLow;
                ViewBag.Metrics[3].Count = Low;
                ViewBag.Metrics[3].Metric = 0;
                ViewBag.Metrics[3].PriorityID = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    if (int.TryParse(row["priority"].ToString(), out i))
                    {
                        ViewBag.Metrics[4 - i].Count = int.Parse(row["Count"].ToString());
                        ViewBag.Metrics[4 - i].Metric = int.Parse(row["Metric"].ToString());

                        nTotalCount += ViewBag.Metrics[4 - i].Count;
                        nTotalMetric += ViewBag.Metrics[4 - i].Metric;
                    }
                }
                ViewBag.MetricsTotalCount = nTotalCount;
                ViewBag.MetricsTotalMetric = nTotalMetric;
            }
        }

        private int getStatusID(int REAid)
        {
            string command = "SELECT STATUS FROM ST_TRACK WHERE TRACKING_ID =" + REAid;
            return Convert.ToInt32(new REATrackerDB().ProcessScalarCommand(command));
        }

        private string formattversion(string input)
        {
            string value = "";
            string[] parsedinput = input.Split('.');
            foreach (string s in parsedinput)
            {
                value += s.PadLeft(3, '0') + ".";
            }
            return value;
        }

        private string formattPriority(string input)
        {
            string output = input;

            switch (input)
            {
                case "Critical":
                    {
                        output = "4";
                        break;
                    }
                case "High":
                    {
                        output = @"3";
                        break;
                    }
                case "Medium":
                    {
                        output = @"2";
                        break;
                    }
                case "Low":
                    {
                        output = @"1";
                        break;
                    }
            }
            return output;
        }

        private string formattIssue(string input)
        {
            string output = input;

            switch (input)
            {

                case "Planned Work":
                    {
                        output = @"3";
                        break;
                    }
                case "Enhancement":
                    {
                        output = @"2";
                        break;
                    }
                case "Problem":
                    {
                        output = @"1";
                        break;
                    }
            }
            return output;
        }

        private void GetLatestSubmitted()
        {
            String cmd = "";
            int prefference = -1;
            ViewBag.DisplayBy = "Last 7 days";
            if (Request.IsAuthenticated)
            {
                ViewBag.isDefaultedDays = false;
                cmd = "SELECT SUBMITTED_DISPLAY FROM ST_USERS WHERE ST_USERS.USER_ID = " + Session["st_userID"];
                prefference = Convert.ToInt32(sql.ProcessScalarCommand(cmd));

            }

            if (prefference != -1)
            {
                ViewBag.DisplayBy = "Last " + prefference + " Instances";
            }

            using (System.Data.DataTable dt =
                (prefference == -1 ? sql.GetLatestSubmittedByTime(_currentProduct) :
                                       sql.GetLatestSubmittedByCount(_currentProduct, prefference)))
            {
                ViewBag.LastestSubmittedCount = dt.Rows.Count;

                //Status Table
                ViewBag.LatestSubmitted = new List<dynamic>();
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    ViewBag.LatestSubmitted.Add(new System.Dynamic.ExpandoObject());
                    ViewBag.LatestSubmitted[i].REA = row[0].ToString();
                    ViewBag.LatestSubmitted[i].Title = row[1].ToString();
                    ViewBag.LatestSubmitted[i].Priority = row[2].ToString();
                    ViewBag.LatestSubmitted[i].PriorityID = formattPriority(row[2].ToString()).PadLeft(3, '0');
                    ViewBag.LatestSubmitted[i].Status = row[3].ToString();
                    ViewBag.LatestSubmitted[i].StatusID = getStatusID(Convert.ToInt32(row[0])).ToString().PadLeft(3, '0');
                    ViewBag.LatestSubmitted[i].Issue = row[4].ToString();
                    ViewBag.LatestSubmitted[i].IssueID = formattIssue(row[4].ToString()).PadLeft(3, '0');
                    ViewBag.LatestSubmitted[i].AssignedTo = row[5].ToString();
                    ViewBag.LatestSubmitted[i].VersionFound = row[6].ToString();
                    ViewBag.LatestSubmitted[i].VersionFoundID = formattversion(row[6].ToString());
                    ViewBag.LatestSubmitted[i].SubmittedOn = DateTime.Parse(row[7].ToString()).ToString("MM/dd/yy");
                    ViewBag.LatestSubmitted[i].AssignedToID = Convert.ToInt32(row[8]);
                    ViewBag.LatestSubmitted[i].ManagersNote = row[9].ToString();
                    i++;
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="product">this is set when the user selects a new product via the dropdown list</param>
        /// <param name="id">this is set when the user passes the product via url, e.g. /Home/Dashboard/9</param>
        /// <param name="setFavorite">this is set when the user clicks on the favorite star</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Dashboard(int? product, int? id, bool? setFavorite)
        {
            //get the current product and user ID
            int productID = Convert.ToInt32(Session["CurrentProduct"]);
            int userID = Convert.ToInt32(Session["st_userID"]);

            //change product has happened via...
            if (product != null)
            {
                //dropdown list
                productID = (int)product;
            }
            else if (id != null)
            {
                //URL
                productID = (int)id;
            }
            DashBoardViewModel model = new DashBoardViewModel(userID, productID, setFavorite);

            if (model.ProductInfo.ID != 0)
            {
                _currentProduct = model.ProductInfo.ID;
                Session["CurrentProduct"] = model.ProductInfo.ID;
                //string test = Convert.ToString(Session["CurrentProduct"]);
            }
            ViewBag.Product = model.ProductInfo.Name;
            ViewBag.Title = ViewBag.Product + " Dashboard";
            ViewBag.ShowMetrics = false;
            CalculateDashboardStatus();

            CalculateDashboardRates();

            CalculateDashboardMetrics(model.Critical, model.High, model.Medium, model.Low);

            GetLatestSubmitted();

            return View(model);
        }

        //=====================================================================
        //============================== Report ===============================
        //=====================================================================
        [Authorize]
        public ActionResult Report(int? id, int? type, string product, string issuetype, string status,
            string priority, int? startDate, int? endDate, string datefield, int? MajorVersion, int? MinorVersion, int? metric,
            bool displayAll = false)
        {
            //int priorityOffset = 8;
            //Type determines (Status = 1) , (Issue type = 2) , (Priority = 3)
            //Details determines the sub division of the type
            //  Status      (Submittted,    Approved,           In Process,     Fixed or Deliverd,      Testing,     Deferred)
            //  Issue Type  (Problem,       Enhancement,        Planned Work)
            //  Priority    (Critical,      High, Medium,       Low)
            ReportViewModel model;
            string title = "";
            if (type != null)
            {
                if ((int)type > 17)
                {
                    type = 1;

                }
                else if (type <= 0)
                {
                    type = 1;
                }
                title = GetDisplayName((int)type, displayAll);
            }

            model = new ReportViewModel(
                    Convert.ToInt32(Session["st_userID"]),
                    title, type, product,
                    status, issuetype, priority,
                     CreateDate(startDate), CreateDate(endDate), datefield, metric, MajorVersion, MinorVersion,
                     displayAll
                    );
            int temp = _currentProduct;
            if (!string.IsNullOrEmpty(model.ProductID))
            {
                string[] producttemp = model.ProductID.Split(',');
                Int32.TryParse(producttemp[0], out temp);
            }
            ViewBag.Product = sql.ProcessScalarCommand("SELECT [NAME] FROM ST_PRODUCT WHERE [PRODUCT_ID]=" + temp);
            ViewBag.Title = ViewBag.Product;
            ViewBag.ShowMetrics = false;

            //ViewBag.metric=
            CalculateDashboardStatus();
            CalculateDashboardRates();
            CalculateDashboardMetrics(model.Critical, model.High, model.Medium, model.Low);
            //Change to Reports
            GetLatestSubmitted();
            ViewBag.DisplayName = "";

            string MetricValue = "";

            if (model.metric == 1)
            {
                MetricValue = "Critical REAs";
            }
            else if (model.metric == 2)
            {
                MetricValue = "High REAs";
            }
            else if (model.metric == 3)
            {
                MetricValue = "All Problem REAs";
            }
            else
            {
                MetricValue = "UNKNOWN METRIC";
            }
            string StatusType = "";

            if (type == 15)
            {
                StatusType = "Code Freeze";
            }
            else
            {
                StatusType = "Release";
            }


            if (((type >= 0 && type < 15) || (type == 17)) && startDate == null)
            {
                ViewBag.DisplayName = title + " For " + ViewBag.Product;
            }
            else if ((type == 15) || (type == 16))
            {
                ViewBag.DisplayName = MetricValue + " " + "For" + " " + ViewBag.Product + "  " + model.MajorVersion + "." + model.MinorVersion + " " + StatusType + " Metric";

            }
            else
            {
                ViewBag.DisplayName = model.Title;
            }
            ViewBag.IsManagerOrAdmin = model.IsManagerOrAdmin(Convert.ToInt32(Session["st_userID"]));

            return View(model);
        }
        private DateTime? CreateDate(int? date)
        {
            DateTime? value = null;
            if (date != null)
            {
                int year = (int)date / 10000;
                int month = (((int)date - (10000 * year)) / 100);
                int day = ((int)date - (10000 * year) - (100 * month));
                value = new DateTime(year, month, day);
            }

            return value;
        }
        private String GetDisplayName(int Type, bool displayAll)
        {
            String Name = "";
            switch (Type)
            {
                case 1:
                    Name += "Submitted REAs";
                    break;
                case 2:
                    Name += "In Process REAs";
                    break;
                case 3:
                    Name += "Fixed or Delivered REAs";
                    break;
                case 4:
                    Name += "Testing REAs";
                    break;
                case 5:
                    Name += "Deferred REAs";
                    break;
                case 6:
                    Name += "Problem REAs";
                    break;
                case 7:
                    Name += "Enhancement REAs";
                    break;
                case 8:
                    Name += "Planned Work REAs";
                    break;
                case 9:
                    Name += "Low REAs";
                    break;
                case 10:
                    Name += "Medium REAs";
                    break;
                case 11:
                    Name += "High REAs";
                    break;
                case 12:
                    Name += "Critical REAs";
                    break;
                case 13:
                    Name += "Metric Report";
                    if (displayAll)
                    {
                        Name += " of All REAs";
                    }
                    else
                    {
                        Name += " of Problem REAs";
                    }
                    break;
                case 14:
                    Name += "Metric History Report";
                    break;
                case 15:
                    Name += "CodeFreeze";
                    break;
                case 16:
                    Name += "Release";
                    break;
                case 17:
                    Name += "Approved";
                    break;
            }
            return Name;
        }

        [Authorize]
        public ActionResult ExpandedReport(int? id, int? type, string product, string issuetype, string status,
            string priority, int? startDate, int? endDate, string datefield, int? MajorVersion, int? MinorVersion, int? metric,
            bool displayAll = false)
        {
            ReportViewModel model;
            string title = "";
            if (type != null)
            {
                if ((int)type > 17)
                {
                    type = 17;

                }
                else if (type <= 0)
                {
                    type = 1;
                }
                title = GetDisplayName((int)type, displayAll);
            }

            model = new ReportViewModel(
                    Convert.ToInt32(Session["st_userID"]),
                    title, type, product,
                    status, issuetype, priority,
                     CreateDate(startDate), CreateDate(endDate), datefield, MajorVersion, MinorVersion, metric,
                     displayAll
                    );
            int temp = _currentProduct;
            if (!string.IsNullOrEmpty(model.ProductID))
            {
                string[] producttemp = model.ProductID.Split(',');
                Int32.TryParse(producttemp[0], out temp);
            }
            ViewBag.ShowMetrics = false;
            CalculateDashboardStatus();
            CalculateDashboardRates();
            CalculateDashboardMetrics(model.Critical, model.High, model.Medium, model.Low);
            //Change to Reports
            GetLatestSubmitted();
            if (type != null && startDate == null)
            {
                ViewBag.ReportTitle = title + " For " +
                    (sql.ProcessScalarCommand("SELECT [NAME] FROM ST_PRODUCT WHERE [PRODUCT_ID]=" + temp));
            }
            else
            {
                ViewBag.ReportTitle = model.Title;
            }
            ViewBag.IsManagerOrAdmin = model.IsManagerOrAdmin(Convert.ToInt32(Session["st_userID"]));
            //Expand the reports
            model.populateHistory();
            return View(model);
        }

        //=====================================================================
        //====================== Dashboard Report =============================
        //=====================================================================
        [Authorize]
        [HttpGet]
        public ActionResult DashBoardReport(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            DashBoardReportViewModel model = new DashBoardReportViewModel(Convert.ToString(id));
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult DashBoardReport(DashBoardReportViewModel model, String email)
        {
            if (email.Equals("send"))
            {
                //Verify and send to Validators
                //EmailBuildVerificationReport mail = new EmailBuildVerificationReport(int.Parse(model.BuildID));
                //mail.Send();



                //EmailSubmittedNotification mail2 = new EmailSubmittedNotification(51139);
                //mail2.Send();

                //EmailDailyWatchListUpdates mail3 = new EmailDailyWatchListUpdates();
                //mail3.Send();

            }
            return RedirectToAction("DashBoardReport", "Home", new { id = model.BuildID });
        }
        //=====================================================================
        //============= BuildVerificationTestReport Report ====================
        //=====================================================================
        [Authorize]
        [HttpGet]
        public ActionResult BuildVerificationTestReport(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            BuildVerificationTestReportModel model = new BuildVerificationTestReportModel(Convert.ToString(id));
            return View(model);
        }

        //=====================================================================
        //========================= Build Release =============================
        //=====================================================================
        [Authorize]
        [HttpGet]
        public ActionResult BuildRelease(int? id)
        {
            int UserID = Convert.ToInt32(Session["st_userID"]);
            int productID = Convert.ToInt32(id);
            if (id == null)
            {
                //Get default product
                int temp = 0;
                Int32.TryParse(
                    Convert.ToString(sql.ProcessScalarCommand(
                    "SELECT DEFAULT_PRODUCT FROM ST_USERS WHERE USER_ID= "
                    + Convert.ToString(UserID))), out temp);
                productID = temp;
            }
            BuildReleaseViewModel model = new BuildReleaseViewModel(productID, UserID);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult BuildRelease(BuildReleaseViewModel model, String button)
        {
            if (button != null)
            {
                //There is only one button that submits else is a change in major and minor release
                int build_id = model.save();
                if (build_id != 0)
                {
                    EmailBuildVerificationReport mail = new EmailBuildVerificationReport(build_id);
                    mail.Send();
                    return RedirectToAction("DashBoardReport", "Home", new { id = build_id });
                }
            }
            model.StUserID = Convert.ToInt32(Session["st_userID"]);
            model.init();
            return View(model);
        }

        //=====================================================================
        //========================== EditRelease ==============================
        //=====================================================================

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult EditRelease(int id)
        {
            int UserID = Convert.ToInt32(Session["st_userID"]);
            int ReleaseID = Convert.ToInt32(id);
            BuildReleaseViewModel model = new BuildReleaseViewModel();
            model.StUserID = UserID;
            bool Exists = model.GetRelease(id);
            model.PopulateComponents(); //SEK 10-1-2021
            ViewBag.Exists = Exists;
            ViewBag.Updated = false;
            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditRelease(BuildReleaseViewModel model)
        {
            model.UpdateRelease();
            model.UpdateComponents();
            bool Exists = model.GetRelease(model.ReleaseId);
            ViewBag.Exists = Exists;
            ViewBag.Updated = true;
            //return View(model);
            return RedirectToAction("DashBoard", "Home", new { id = model.ProductID });
        }

        //=====================================================================
        //======================== Show All Releases ==========================
        //=====================================================================
        [Authorize]
        [HttpGet]
        public ActionResult ShowAllRelease(int id)
        {
            int UserID = Convert.ToInt32(Session["st_userID"]);
            AllReportsViewModel model = new AllReportsViewModel(UserID, id);
            return View(model);
        }
        //=====================================================================
        //============================== Charts ================================
        //=====================================================================
        [Authorize]
        [HttpGet]
        public ActionResult ReportChart(String Title, String ArgsxValues,
            String ArgsyValues, String ArgsMetrics, int? ParamWidth, int? ParamHeight)
        {
            ChartViewModel.PieChart model =
                new ChartViewModel.PieChart(
                //"Hello", new[] { "peter", "paul", "mary" }, new[] { new[] { "1", "2", "3" } }, 0, 0);
                Title, ArgsxValues, ArgsyValues, ArgsMetrics, ParamWidth, ParamHeight);
            using (var ms = new System.IO.MemoryStream())
            {
                model.DisplayChart.SaveImage(ms, System.Web.UI.DataVisualization.Charting.ChartImageFormat.Png);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                return File(ms.ToArray(), "image/png", Title);
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult CustomReportChart(String Title, String ArgsxValues,
            String ArgsyValues, int colorEnum, int? ParamWidth, int? ParamHeight)
        {
            CustomChartViewModel.CustomPieChart model =
                new CustomChartViewModel.CustomPieChart(
                Title, ArgsxValues, ArgsyValues, colorEnum, ParamWidth, ParamHeight);
            using (var ms = new System.IO.MemoryStream())
            {
                model.DisplayChart.SaveImage(ms, System.Web.UI.DataVisualization.Charting.ChartImageFormat.Png);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                return File(ms.ToArray(), "image/png", Title);
            }
        }
        //[Authorize]
        //public ActionResult ReportNoSideBar(int? product, int? type, int? details)
        //{
        //    ReportViewModel model;
        //    if (product == null && type != null & details != null)
        //    {
        //        model = new ReportViewModel(Convert.ToInt32(Session["st_userID"]), (int)details, (int)type);
        //    }
        //    else
        //    {
        //        model = new ReportViewModel(Convert.ToInt32(Session["st_userID"]), (int)product, (int)details, (int)type);
        //    }
        //    _currentProduct = model.ProductID;
        //    ViewBag.Product = _sql.ProcessScalarCommand("SELECT [NAME] FROM ST_PRODUCT WHERE [PRODUCT_ID]=" + _currentProduct.ToString());
        //    ViewBag.Title = ViewBag.Product;
        //    ViewBag.ShowMetrics = false;
        //    CalculateDashboardStatus();
        //    CalculateDashboardRates();
        //    CalculateDashboardMetrics(model.Critical, model.High, model.Medium, model.Low);
        //    //Change to Reports
        //    GetLatestSubmitted();
        //    return View(model);
        //}
    }
}