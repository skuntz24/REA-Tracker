using REA_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using System.Web.UI;
using QVICommonIntranet.Database;
//using QVICommonIntranet.Email.EmailREATracker;
using static REA_Tracker.Models.DisplayReportsViewModel;

namespace REA_Tracker.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class REAController : Controller
    {
        private REATrackerDB _sql = new REATrackerDB();
        //=====================================================================
        //============================== Index ================================
        //=====================================================================
        // GET: /SCR/
        public ActionResult Index()
        {
            return View();
        }

        //=====================================================================
        //============================== Legacy ID Fix ========================
        //=====================================================================
        public ActionResult QuickSearchREA(int? id)
        {
            ActionResult result;
            if (id == null)
            {
                result = RedirectToAction("Display", "REA", new { id = id });
            }
            else
            {
                string command = "SELECT Count(*) FROM ST_TRACK WHERE TRACKING_ID = " + id + " OR TRACKING_ID2 = " + id;
                switch (Convert.ToInt32(new REATrackerDB().ProcessScalarCommand(command)))
                {
                    case 0:
                    case 1:
                        result = RedirectToAction("Display", "REA", new { id = id });
                        break;
                    default:
                        result = View(new QuickSearchREAViewModel((int)id));
                        break;
                }
            }
            return result;
        }
        //=====================================================================
        //============================= Display ===============================
        //=====================================================================
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Display(int? id)
        {
            //Default to missing ID
            ViewBag.NoId = true;
            ViewBag.Valid = false;

            DisplayREA model = new DisplayREA();
            //Something was passed in, now check to see if its a valid REA
            if (id != null)
            {
                ViewBag.Valid = _sql.IsValidREA((int)id);

                //we have a valid REA found in the system
                if (ViewBag.Valid)
                {
                    //Set up locks
                    if (Session["NewSCRLock"] != null && (int)id == Convert.ToInt32(Session["NewSCRLock"]))
                    {
                        Session["NewSCRLock"] = null;
                    }
                    if (Session["EditSCRLock" + (int)id] != null && Convert.ToInt32(Session["EditSCRLock" + (int)id]) == id)
                    {
                        Session["EditSCRLock" + (int)id] = null;
                    }

                    //Update the model
                    model.UserID = Convert.ToInt32(Session["st_userID"]);
                    model.ASPUserID = Convert.ToString(Session["asp_userID"]);
                    model.AccountType = Convert.ToString(Session["CurrentUserAccountType"]);
                    model.TrackingId = (int)id;
                    model.Initialize();
                    
                    //Update ViewBag values
                    ViewBag.NoId = false;
                    ViewBag.SCRnum = (int)id;
                    ViewBag.Title = "REA # " + id.ToString() + " - " + model.Title;
                    ViewBag.isInwatchList = model.IsWatching;

                    return View(model);
                }
/*
                if (valid)
                {
                    //Check if you have access
                    bool access = false;
                    if (Request.IsAuthenticated)
                    {
                        int hasAccess = 0;
                        REATrackerDB sql = new REATrackerDB();
                        DataTable dt = sql.GetProductList(model.UserID);
                        //check the st_user's product access
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (Convert.ToString(dt.Rows[i][1]) == model.Product)
                            {
                                hasAccess = 1;
                            }
                        }
                        //check if the st_product_access has permissions
                        sql.OpenConnection();
                        string text =   $@"SELECT ST_PRODUCT_ACCESS.Id FROM ST_PRODUCT_ACCESS 
                                            INNER JOIN ST_PRODUCT ON  ST_PRODUCT.PRODUCT_ID = ST_PRODUCT_ACCESS.ProductID 
                                            WHERE ST_PRODUCT.NAME = '{model.Product}' AND ST_PRODUCT_ACCESS.UserID = {model.UserID};";
                        DataTable stdt = sql.ProcessCommand(text);
                        //Checking full access
                        if (stdt.Rows.Count > 0 || sql.hasFullAccess())
                        {
                            hasAccess = 1;
                        }
                        sql.CloseConnection();
                        //if the user is not a Vendor or User
                        sql.OpenConnection();
                        string getTeam =
                            $@"SELECT ST_PRODUCT_TEAM.ROLE FROM ST_PRODUCT_TEAM 
                                    INNER JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_PRODUCT_TEAM.PRODUCT 
                                    WHERE ST_PRODUCT_TEAM.USER_ID = {model.UserID} AND ST_PRODUCT.NAME = '{model.Product}';";
                        DataTable teamDT = sql.ProcessCommand(getTeam);
                        sql.CloseConnection();
                        //is a Administrator or author
                        if (User.IsInRole("Administrator") || (model.SubmittedBy == Convert.ToString(Session["CurrentUserName"])))
                        {
                            access = true;
                        }
                        //has access and the product is not planned work, or is not a Power User
                        else if (hasAccess != 0)
                        {
                            if (model.Issue != "Planned Work" || Convert.ToString(Session["CurrentUserAccountType"]) == "Power User")
                            {
                                access = true;
                            }
                            else if (teamDT.Rows.Count > 0 && (Convert.ToInt32(teamDT.Rows[0]["ROLE"]) != 1 || Convert.ToInt32(teamDT.Rows[0]["ROLE"]) != 2))
                            {
                                access = true;
                            }
                        }
                        if ((teamDT.Rows.Count > 0))
                        {
                            model.canEditKeywords = true;
                            model.canEditManagersNote = (Convert.ToInt16(teamDT.Rows[0]["ROLE"]) == 5);
                        }
                    }
                    model.display = access;
                    if (model.Product != null)
                    {
                        return View(model);
                    }
                    }
                    */
            } //missing or unknown SCR or Bad instance
            return View();
        }
        
        [HttpPost]
        public ActionResult Display(DisplayREA model, string button)
        {
            ViewBag.href = "";
            //if (button != null && button.Equals("UpdateCodeReview"))
            //{
            //   bool val = model.UpdateCodeReview((int)model.TrackingId);
            //    return RedirectToAction("Display", "SCR", new { id = model.TrackingId });
            //}
            //else
            if (button != null && button.Equals("selfWatchList"))
            {
                AddToWatchList watch = new AddToWatchList(model.TrackingId);
                if (watch.isinWatchList(model.UserID))
                {
                    watch.Remove(model.TrackingId, model.UserID);
                    ViewBag.isInwatchList = false;
                }
                else
                {
                    watch.Process(model.TrackingId, model.UserID.ToString(), false);
                    ViewBag.isInwatchList = true;
                }
                return RedirectToAction("Display", "REA", new { id = model.TrackingId });
            }
            else if (button != null && button.Equals("CreateCodeReview"))
            {
                //HACK: TODO: remove
                AddToWatchList watch = new AddToWatchList(model.TrackingId);
                ViewBag.isInwatchList = watch.isinWatchList(model.UserID);

                bool success = model.SaveCodeReview();
                
                return RedirectToAction("Display", "REA", new { id = model.TrackingId });
            }
            else if (String.IsNullOrEmpty(button))
            {
                ModelState.Clear();
            }
            return View(model);
        }

        //=====================================================================
        //============================== Print ================================
        //=====================================================================     
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Print(int? id)
        {
            //Default to missing ID
            ViewBag.NoId = true;
            ViewBag.Valid = false;

            DisplayREA model = new DisplayREA();
            //Something was passed in, now check to see if its a valid SCR
            if (id != null)
            {
                ViewBag.Valid = _sql.IsValidREA((int)id);

                //we have a valid SCR found in the system
                if (ViewBag.Valid)
                {
                    //Set up locks
                    if (Session["NewSCRLock"] != null && (int)id == Convert.ToInt32(Session["NewSCRLock"]))
                    {
                        Session["NewSCRLock"] = null;
                    }
                    if (Session["EditSCRLock" + (int)id] != null && Convert.ToInt32(Session["EditSCRLock" + (int)id]) == id)
                    {
                        Session["EditSCRLock" + (int)id] = null;
                    }

                    //Update the model
                    model.UserID = Convert.ToInt32(Session["st_userID"]);
                    model.ASPUserID = Convert.ToString(Session["asp_userID"]);
                    model.AccountType = Convert.ToString(Session["CurrentUserAccountType"]);
                    model.TrackingId = (int)id;
                    model.Initialize();

                    //Update ViewBag values
                    ViewBag.NoId = false;
                    ViewBag.SCRnum = (int)id;
                    ViewBag.Title = Resources.strings.qviSCR + "# " + id.ToString() + " - " + model.Title;
                    ViewBag.isInwatchList = model.IsWatching;

                    return View(model);
                }
                    /*
                    ViewBag.NoId = false;
                    string cmdtext = "Select Count(1) from st_track where tracking_id = " + Convert.ToString(id);
                    bool valid = true;
                    //look up the id (i.e. SCR number) to ensure its valid
                    _sql.OpenConnection();
                    valid = Convert.ToBoolean(_sql.ProcessScalarCommand(cmdtext));
                    _sql.CloseConnection();
                    ViewBag.Valid = valid;
                    ViewBag.SCRnum = id;
                    if (valid)
                    {
                        DisplaySCR model = new DisplaySCR();
                        model.getViewSCR((int)id);
                        ViewBag.Title = "SCR# " + id.ToString() + " - " + model.Title;
                        //Check if you have access
                        bool access = false;
                        if (Request.IsAuthenticated)
                        {
                            int hasAccess = 0;
                            REATrackerDB sql = new REATrackerDB();
                            DataTable dt = sql.GetProductList(Convert.ToInt32(Session["st_userID"]));
                            //check the st_user's product access
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (Convert.ToString(dt.Rows[i][1]) == model.Product)
                                {
                                    hasAccess = 1;
                                }
                            }
                            //check if the st_product_access has permissions
                            sql.OpenConnection();
                            string text =
                                "SELECT ST_PRODUCT_ACCESS.Id " +
                                "FROM ST_PRODUCT_ACCESS " +
                                "INNER JOIN ST_PRODUCT ON  ST_PRODUCT.PRODUCT_ID = ST_PRODUCT_ACCESS.ProductID " +
                                "WHERE ST_PRODUCT.NAME = '" + Convert.ToString(model.Product) + "' AND ST_PRODUCT_ACCESS.UserID = " + Convert.ToString(Session["st_userID"]) + ";";
                            DataTable stdt = sql.ProcessCommand(text);
                            //Checking full access
                            if (stdt.Rows.Count > 0 || sql.hasFullAccess(Convert.ToString(Session["asp_userID"])))
                            {
                                hasAccess = 1;
                            }
                            sql.CloseConnection();
                            //if the user is not a Vendor or User
                            sql.OpenConnection();
                            string getTeam =
                                "SELECT ST_PRODUCT_TEAM.ROLE FROM ST_PRODUCT_TEAM " +
                                "INNER JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_PRODUCT_TEAM.PRODUCT " +
                                "WHERE ST_PRODUCT_TEAM.USER_ID = " + Convert.ToInt32(Session["st_userID"]) +
                                "AND ST_PRODUCT.NAME = '" + model.Product + "';";
                            DataTable teamDT = sql.ProcessCommand(getTeam);
                            sql.CloseConnection();
                            //is a Administrator or author
                            if (User.IsInRole("Administrator") || (model.SubmittedBy == Convert.ToString(Session["CurrentUserName"])))
                            {
                                access = true;
                            }
                            //has access and the product is not planned work, or is not a Power User
                            else if (hasAccess != 0)
                            {
                                if (model.Issue != "Planned Work" || Convert.ToString(Session["CurrentUserAccountType"]) == "Power User")
                                {
                                    access = true;
                                }
                                else if (teamDT.Rows.Count > 0 && (Convert.ToInt32(teamDT.Rows[0][0]) != 1 || Convert.ToInt32(teamDT.Rows[0][0]) != 2))
                                {
                                    access = true;
                                }
                            }
                        }
                        model.display = access;
                        model.TrackingId = (int)id;
                        Boolean isWatching = model.isinWatchList(Convert.ToInt32(Session["st_userID"]));
                        ViewBag.isInwatchList = isWatching;
                        model.populateWatching();
                        if (model.Product != null)
                        {
                            return View(model);
                        }
                    }
                    */
                }
            else
            {
                ViewBag.NoId = true;
            }
            //missing or unknown SCR or Bad instance
            return View();
        }
        //=====================================================================
        //============================== Search ===============================
        //=====================================================================
        [Authorize]
        [HttpGet]
        public ActionResult Search(int? id)
        {
            int USER_ID = Convert.ToInt32(Session["st_userID"]);
            REATrackerDB sql = new REATrackerDB();
            ViewBag.DisplayRawSQL = sql.HasRawSQL(USER_ID);
            SearchViewModel model;
            if (id == null)
            {
                model = new SearchViewModel(Convert.ToInt32(Session["st_userID"]),
                    Convert.ToInt32(Session["CurrentProduct"])
                    );
            }
            else
            {
                int CurrProd = Convert.ToInt32(Session["CurrentProduct"]);
                model = new SearchViewModel(Convert.ToInt32(id),
                    USER_ID,
                    CurrProd
                    );
                int rowcount = model.ProcessSearch();
                ViewBag.RowCount = rowcount;
                ViewBag.DisplayRawSQL = sql.HasRawSQL(USER_ID);
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Search(SearchViewModel model, String button)
        {
            int USER_ID = Convert.ToInt32(Session["st_userID"]);
            REATrackerDB sql = new REATrackerDB();
            ViewBag.DisplayRawSQL = sql.HasRawSQL(USER_ID);
            model.CurrentProductID = Convert.ToInt32(Session["CurrentProduct"]);
            model.UserID = USER_ID;
            model.init();
            int rowcount = model.ProcessSearch();
            ViewBag.RowCount = rowcount;
            ActionResult returnvalue = View(model);
            if (button == "Save")
            {
                int ReportID = model.ProcessNewReport(USER_ID);
                ViewBag.Message = "Report : " + model.NameOfReport + " has been created. Report ID = " + Convert.ToString(ReportID);
                ViewBag.DisplayRawSQL = sql.HasRawSQL(USER_ID);

                returnvalue = RedirectToAction("Display", "DetailedReports", new { id = ReportID });

            }
            else if (button == "Delete")
            {
                model.Delete();
                ViewBag.Message = "Report : " + model.NameOfReport + " has been deleted.";
                ViewBag.DisplayRawSQL = sql.HasRawSQL(USER_ID);

                returnvalue = View(model);
            }
            else if (button == "Update")
            {
                model.Update();
                ViewBag.Message = "Report : " + model.NameOfReport + " has been Updated.";
                ViewBag.DisplayRawSQL = sql.HasRawSQL(USER_ID);

                returnvalue = RedirectToAction("Display", "DetailedReports", new { id = model.ReportID });
            }

            //Send to Detailed Reports?
            return returnvalue;
        }

        //=====================================================================
        //=============================== New =================================
        //=====================================================================
        [HttpGet]
        public ActionResult New(int? id)
        {
            ViewBag.Message = new List<String>();
            for (int i = 0; i <= 5; i++)
            {
                ViewBag.Message.Add("");
            }
            int st_user_id = (int)Session["st_userID"];

            int CurrentProduct = (int)Session["CurrentProduct"];
            if (id != null)
            {
                if (id > 0)
                {
                    CurrentProduct = (int)id;
                }
            }
            
            NewREA model = new NewREA(st_user_id, CurrentProduct);
            //Session["Files"] = new List<REA_Tracker.Models.NewSCR.SCRAttachments>();
            if (String.IsNullOrEmpty(model.HiddenHashCode))
            {
                model.HiddenHashCode = "NewFiles" + DateTime.Now.ToString();
            }
            Session[model.HiddenHashCode] = new List<REA_Tracker.Models.NewREA.SCRAttachments>();
            model.InitGet();
            return View(model);
        }

        [HttpPost]
        public ActionResult New(NewREA model, string button)
        {
            //model.UserID = (int)Session["st_userID"];
            string alertMessage = "";
            model.Problem = Convert.ToString(model.Problem as object).Trim();
            model.Benefit = Convert.ToString(model.Benefit as object).Trim();
            model.Solution = Convert.ToString(model.Solution as object).Trim();
            ViewBag.href = "";
            int tab = 0;
            model.InitGet();
            bool clearModelState = true;
            ViewBag.Message = new List<String>();
            for (int i = 0; i <= 5; i++)
            {
                ViewBag.Message.Add("");
            }
            if (button == "Add")
            {
                ViewBag.href = "#Related";
                //add to related SCRs
                int exists = model.Add(false);
                tab = 4;
                if (exists == -1)
                {
                    alertMessage += "- The related SCR does not exist.\n";
                    ViewBag.Message[4] = "The related SCR does not exist.";
                }
                else if (exists == 0)
                {
                    alertMessage += "- The related SCR has been already added.\n";
                    ViewBag.Message[4] = "The related SCR has been already added.";
                }
            }
            else if (button == "Correlate")
            {
                ViewBag.href = "#Related";
                //add both ends to related SCRs
                int exists = model.Add(true);
                tab = 4;
                if (exists == -1)
                {
                    alertMessage += "- The related SCR does not exist.\n";
                    ViewBag.Message[4] = "The related SCR does not exist.";
                }
                else if (exists == 0)
                {
                    alertMessage += "- The Related SCR has been already correlated\n";
                    ViewBag.Message[4] = "The related SCR has been already correlated.";
                }
            }
            else if (button == "Upload")
            {
                ViewBag.href = "#Attachments";
                //add to Files
                int uploaded = model.Upload();
                if (uploaded == -2)
                {
                    alertMessage += "- A file description is required.\n";
                    ViewBag.Message[5] = "The description is missing.";
                }
                else if (uploaded == -1)
                {
                    alertMessage += "- The file is missing.\n";
                    ViewBag.Message[5] = "The file is missing.";
                }
                else if (uploaded == 0)
                {
                    alertMessage += "- The file is too large. The Max size is 19.1 MB\n";
                    ViewBag.Message[5] = "The file is too large. The Max size is 19.1 MB.";
                }
                tab = 5;
            }
            else if (button != null && button.Contains("RemoveFiles"))
            {
                ViewBag.href = "#Attachments";
                model.RemoveFiles(button);
                tab = 5;
            }
            else if (button != null && button.Contains("RemoveRelated"))
            {
                ViewBag.href = "#Related";
                model.RemoveRelated(button);
                tab = 4;
            }
            else if (button == "Save")
            {
                //Check if Enhancement and Customer Name
                bool validateValue = true;
                if (model.IssueID == 2)
                {
                    if (model.ValidateEnhancement() == false)
                    {
                        tab = 2;
                        validateValue = false;
                        ViewBag.message[2] = "Required.";
                        alertMessage += "- Enhancement Details are missing.\n";
                    }
                    
                    if (model.Problem == model.Solution)
                    {
                        tab = 2;
                        validateValue = false;
                        ViewBag.validatetext = "Entries can not be the same";
                        alertMessage += "- Enhancement Details are invalid.\n";

                    }
                    if (model.Solution == model.Benefit)
                    {
                        tab = 2;
                        validateValue = false;
                        ViewBag.validatetext = "Entries can not be the same";
                        alertMessage += "- Enhancement Details are invalid.\n";
                    }
                    if (model.Benefit == model.Problem)
                    {
                        tab = 2;
                        validateValue = false;
                        ViewBag.validatetext = "Entries can not be the same";
                        alertMessage += "- Enhancement Details are invalid.\n";
                    }
                    if ((model.Problem.Length > 0 && model.Problem.Length < 10) || (model.Solution.Length > 0 && model.Solution.Length < 10) || (model.Benefit.Length > 0 && model.Benefit.Length < 10))
                    {
                        tab = 2;
                        validateValue = false;
                        ViewBag.validateLength = "Please explain more";
                        alertMessage += "xxx";
                    }
                }

                if (model.IssueID == 3)
                {
                    if (model.ValidatePlannedWork() == false)
                    {
                        tab = 6;
                        validateValue = false;
                        ViewBag.message[2] = "Required.";
                        alertMessage += "- Planned Work \"Story\" field is missing.\n";
                    }
                }

                if (model.IssueID == 1)
                {
                    if (string.IsNullOrWhiteSpace(model.Details))
                    {
                        tab = 0;
                        validateValue = false;
                        ViewBag.Message[0] = "Details Required";
                        alertMessage += "Details section is empty";
                    }
                    else
                    {
                        if (model.Details.Length < 10)
                        {
                            tab = 0;
                            validateValue = false;
                            ViewBag.validateLength = "Please explain the problem in more detail.";
                            alertMessage += "Please add more detail";
                        }
                    }
                }

                if (model.ValidateCustomerInfo() == false)
                {
                    tab = 1;
                    validateValue = false;
                    ViewBag.ContactPersonMessage = "Required.";
                    alertMessage += "- Customer is missing.\n";
                }

                //Send and Save the SCR to the Database
                if (ModelState.IsValid && validateValue)
                {
                    model.UserID = Convert.ToInt32(Session["st_userID"]);
                    if (Session["NewSCRLock"] == null)
                    {
                        int id = model.Save(Convert.ToString(Session["asp_userID"]), Convert.ToInt32(Session["st_userID"]));
                        if (id != 0)
                        {
                            //Session["Files"] = null
                            Session["NewSCRLock"] = id;
                            Session[model.HiddenHashCode] = null;
                            //EmailInsert
                            //new EmailSubmittedNotification(id).Send();
                            return RedirectToAction("Display", "REA", new { id = id });
                        }
                        else
                        {
                            //ViewBag.Message[0] = "Some fields are invalid.";
                            alertMessage += "- Required Fields are missing.\n";
                            clearModelState = false;
                            //tab = 0;
                        }
                    }
                    else
                    {
                        return RedirectToAction("Display", "REA", new { id = Convert.ToInt32(Session["NewSCRLock"]) });
                    }
                }
                else
                {
                    clearModelState = false;
                    //ViewBag.Message[0] = "Some fields are invalid.";
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    //tab = 0;
                }
                if (!ModelState.IsValid)
                {
                    tab = 0;
                }
            }
            if (clearModelState)
            {
                ModelState.Clear();
            }
            ViewBag.tab = tab;
            ViewBag.AlertMessage = alertMessage;
            //model.Files = ((List<REA_Tracker.Models.NewSCR.SCRAttachments>)Session["Files"]);
            List<REA_Tracker.Models.NewREA.SCRAttachments> temp = (List<REA_Tracker.Models.NewREA.SCRAttachments>)Session[model.HiddenHashCode];
            model.Files = ((List<REA_Tracker.Models.NewREA.SCRAttachments>)Session[model.HiddenHashCode]);
            return View(model);
        }
        /*
                [HttpGet]
                public ActionResult New2()
                {
                    int st_user_id = (int)Session["st_userID"];
                    NewSCR model = new NewSCR(st_user_id);
                    Session["Files"] = new List<REA_Tracker.Models.NewSCR.SCRAttachments>();
                    model.InitGet();
                    model.SetVersionFound(model.ProductID, Convert.ToString(Session["CurrentUserAccountType"]));
                    return View(model);
                }

                [HttpPost]
                public ActionResult New2(NewSCR model, string button)
                {
                    model.Problem = Convert.ToString(model.Problem as object).Trim();
                    model.Benefit = Convert.ToString(model.Benefit as object).Trim();
                    model.Solution = Convert.ToString(model.Solution as object).Trim();
                    model.InitGet();
                    bool clearModelState = true;
                    ViewBag.href = "";
                // RELATE
                    if (button == "Add" || button == "Correlate")
                    {
                        ViewBag.href = "#Related";
                        //add to related SCRs
                        int exists = model.Add((button == "Add")?false:true);
                        if (exists == -1)
                        {
                            // "The related SCR does not exist.";
                            clearModelState = false;
                            ModelState.AddModelError(string.Empty, "The related SCR does not exist.");
                        }
                        else if (exists == 0)
                        {
                            // "The related SCR has been already added.";
                            clearModelState = false;
                            ModelState.AddModelError(string.Empty, "The related SCR has been already added.");
                        }
                    }
                //FILES
                    else if (button == "Upload")
                    {
                        //add to Files
                        int uploaded = model.Upload();
                        ViewBag.href = "#Attachments";
                        switch(uploaded)
                        {
                            case(-2): 
                                {
                                    // "The description is missing.";
                                    clearModelState = false;
                                    ModelState.AddModelError(string.Empty, "The description is missing.");
                                    break;
                                }
                            case(-1):
                                {
                                    // "The file is missing.";
                                    clearModelState = false;
                                    ModelState.AddModelError(string.Empty, "The file is missing.");
                                    break;
                                }
                            case(0): 
                                {
                                    // "The file is Too Large. The Max size is 19.1MB.";
                                    clearModelState = false;
                                    ModelState.AddModelError(string.Empty, "The file is Too Large. The Max size is 19.1MB.");
                                    break;
                                }
                        }
                    }
                    else if (button != null && button.Contains("RemoveFiles"))
                    {
                        ViewBag.href = "#Attachments";
                        model.RemoveFiles(button);
                    }
                    else if (button != null && button.Contains("RemoveRelated"))
                    {
                        ViewBag.href = "#Related";
                        model.RemoveRelated(button);
                    }
                    else if (button == "Save")
                    {
                        //Check if Enhancement and Customer Name
                        bool validateValue = true;
                        if (model.ValidateEnhancement() == false)
                        {
                            validateValue = false;
                            ViewBag.Enhancement = "Please enter a Problem, Benefit, and Solution";
                        }
                        if (model.ValidateCustomerInfo() == false)
                        {
                            validateValue = false;
                            ViewBag.ContactPersonMessage = "Please enter a Person to Contact";
                        }
                        //Send and Save the SCR to the Database
                        if (ModelState.IsValid && validateValue)
                        {
                            int id = model.Save(Convert.ToString(Session["asp_userID"]), Convert.ToInt32(Session["st_userID"]));
                            if (id != 0)
                            {
                                Session["Files"] = null;
                                return RedirectToAction("Display", "SCR", new { id = id });
                            }
                            else
                            {
                                // = "Some fields are invalid.";
                                clearModelState = false;
                                ModelState.AddModelError(string.Empty, "Some fields are invalid.");
                                clearModelState = false;
                            }
                        }
                        else 
                        {
                            clearModelState = false;
                        }
                    }
                    if (button == null || button == "")
                    {
                        model.SetVersionFound(model.ProductID, Convert.ToString(Session["CurrentUserAccountType"]));
                    }
                    if (clearModelState)
                    {
                        ModelState.Clear();
                    }
                    model.Files = ((List<REA_Tracker.Models.NewSCR.SCRAttachments>)Session["Files"]);
                    return View(model);
                }
                */
        //=====================================================================
        //============================== Edit =================================
        //=====================================================================
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            ViewBag.FullName = Convert.ToString(Session["CurrentUserName"]);
            ViewBag.href = "";
            ViewBag.Message = new List<String>();
            for (int i = 0; i <= 5; i++)
            {
                ViewBag.Message.Add("");
            }
            int st_user_id = (int)Session["st_userID"];
            EditREA model;

            if (id != null)
            {
                string TrackExists = "SELECT 1 FROM REA_TRACK WHERE TRACKING_ID = " + Convert.ToString(id);
                REATrackerDB sql = new REATrackerDB();
                string temp = Convert.ToString(sql.ProcessScalarCommand(TrackExists));
                if (temp == "1")
                {
                    model = new EditREA(st_user_id, Convert.ToString(Session["CurrentUserAccountType"]), (int)id);
                }
                else
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            else
            {
                return RedirectToAction("Dashboard", "Home");
            }
            Session["Files-" + Convert.ToString(id)] = model.Files;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EditREA model, string button)
        {
            ViewBag.href = "";
            if (button != null && (button.Equals("Add") || button.Equals("Correlate")))
            {
                ViewBag.href = "Related";
                ModelState.Clear();
                int addResult = model.Add(button.Equals("Correlate"));
                switch (addResult)
                {
                    case -1:
                        {
                            ViewBag.RelatedInputIDError = "REA Does not exist.";
                            break;
                        }
                    case 0:
                        {
                            ViewBag.RelatedInputIDError = "REA has already been added";
                            break;
                        }
                    case -2:
                        {
                            ViewBag.RelatedInputIDError = "REA is the same as the present SCR";
                            break;
                        }
                }
            }
            else if (button != null && button.Equals("Upload"))
            {
                ViewBag.href = "Attachments";
                ModelState.Clear();
                model.Upload();
            }

            else if (button != null && button.Contains("RemoveFiles"))
            {
                ViewBag.href = "Attachments";
                ModelState.Clear();
                model.RemoveFiles(button);
            }

            else if (button != null && button.Contains("RemoveRelated"))
            {
                ViewBag.href = "Related";
                ModelState.Clear();
                model.RemoveRelated(button);
            }

            else if (button != null && button.Equals("Save"))
            {
                //model.ParseStrings();
                model.Files = ((List<REA_Tracker.Models.EditREA.SCRAttachments>)Session["Files-" + Convert.ToString(model.TrackingId)]);

                if (Session["EditSCRLock" + model.TrackingId] == null)
                {
                    Session["EditSCRLock" + model.TrackingId] = model.TrackingId;
                    model.Save();
                }
                return RedirectToAction("Display", "REA", new { id = model.TrackingId });
            }
            else if (button != null && button.Equals("UpdateCodeReview"))
            {
                bool val = model.UpdateCodeReview((int)model.TrackingId);
                return RedirectToAction("Edit", "REA", new { id = model.TrackingId });
            }
            else if (button != null && button.Equals("CreateCodeReview"))
            {
                if (!model.SaveCodeReview())
                {
                    return RedirectToAction("Edit", "REA", new { id = model.TrackingId });
                }
            }
            else if (String.IsNullOrEmpty(button))
            {
                ModelState.Clear();
            }
            model.init();
            //            model.Files.Clear();

            //            List<REA_Tracker.Models.EditSCR.SCRAttachments> temp = (List<REA_Tracker.Models.EditSCR.SCRAttachments>)Session["Files-"+Convert.ToString(model.TrackingId)];
            model.Files = ((List<REA_Tracker.Models.EditREA.SCRAttachments>)Session["Files-" + Convert.ToString(model.TrackingId)]);

            return View(model);
        }
        //=====================================================================
        //======================== Display Reports ============================
        //=====================================================================
        public ActionResult DisplayReports(int? id)
        {
            DisplayReportsViewModel model = new DisplayReportsViewModel(
                Convert.ToInt32(Session["st_userID"]),
                User.IsInRole("Administrator"),
                 Convert.ToInt32(Session["CurrentProduct"])
                );

            return View(model);
        }
        //=====================================================================
        //======================== Quick Search Result ========================
        //=====================================================================
        [HttpGet]
        public ActionResult QuickSearchResult(String term)
        {
            String paraminfo = "";
            if (!String.IsNullOrEmpty(this.Request.Url.Query))
            {
                paraminfo = this.Request.Url.Query.Substring(6, this.Request.Url.Query.Length - 6);
            }

            paraminfo = Server.UrlDecode(paraminfo);
            int UserID = Convert.ToInt32(Session["st_userID"]);
            int product = Convert.ToInt32(Session["CurrentProduct"]);
            QuickSearchResultViewModel model = new QuickSearchResultViewModel(UserID, paraminfo, product);
            return View(model);
        }

        [HttpPost]
        public ActionResult QuickSearchResult(QuickSearchResultViewModel model, String term)
        {
            String paraminfo = "";
            if (!String.IsNullOrEmpty(this.Request.Url.Query))
            {
                paraminfo = this.Request.Url.Query.Substring(6, this.Request.Url.Query.Length - 6);
            }
            model = new QuickSearchResultViewModel(model.UserID, model.SearchTerm, (int)model.SelectedProductId);
            //Create a new model because of some problems with 
            // dynamic object

            return View(model);
        }
        //=====================================================================
        //============================= Mass Update ===========================
        //=====================================================================
        [HttpGet]
        public ActionResult MassUpdate(int? id, int? Product, int? type)
        {
            //Check if type exists or if product exits
            if (type != null)
            {
                if (type < 1)
                {
                    type = 1;
                }
                else if ((type > 12) && (type != 17))
                {
                    type = 12;
                }
            }
            MassUpdateViewModel Result;
            if (id != null)
            {
                Result = new MassUpdateViewModel(Convert.ToInt32(id),
                    Convert.ToInt32(Session["st_userID"]),
                    Convert.ToInt32(Session["CurrentProduct"])
                    );
            }
            else if (Product != null && type != null)
            {
                Result = new MassUpdateViewModel((int)Product, (int)type,
                    Convert.ToInt32(Session["st_userID"]),
                    Convert.ToInt32(Session["CurrentProduct"]));
            }
            else
            {
                Result = new MassUpdateViewModel();
            }
            return View(Result);
        }

        [HttpPost]
        public ActionResult MassUpdate(MassUpdateViewModel model)
        {
            model.ProcessMassUpdate();
            ActionResult action;
            if (model.ReportID != 0)
            {
                action = RedirectToAction("MassUpdate", "REA",
                    new { id = model.ReportID });
            }
            else
            {
                action = RedirectToAction("MassUpdate", "REA",
                    new { Product = model.Product, type = model.Type });
            }
            return action;
        }
        //=====================================================================
        //=========================== Add to WathList =========================
        //=====================================================================
        [HttpGet]
        public ActionResult AddtoWatchList(int id)
        {
            AddToWatchList model = new AddToWatchList(id);
            model.PopulateDepartmentList();
            model.PopulatePeopleList(model.department);
            ViewBag.Title = "Adding Users to Watchlist";
            return View(model);
        }

        [HttpPost]
        public ActionResult AddtoWatchList(AddToWatchList model, String button)
        {
            if (button == "Process")
            {
                model.Process(model.SCRID, model.UserIDs, model.EmailNotify);
            }
            model.PopulateDepartmentList();
            model.PopulatePeopleList(model.department);
            ViewBag.Title = "Adding Users To Watchlist";
            return View(model);
        }

        //=====================================================================
        //=========================== Add Keywords =========================
        //=====================================================================
        [HttpGet]
        public ActionResult AddKeywords(int id)
        {
            AddKeywords model = new AddKeywords(id);
            ViewBag.Title = Resources.strings.qviAddAndRemoveKeywords;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddKeywords(AddKeywords model, String button)
        {
            ViewBag.Title = Resources.strings.qviAddAndRemoveKeywords;
            if (button == "Update")
            {
                model.Update(model.TrackingID, model.KeywordIDs);
            }
            return View(model);
        }

        //=====================================================================
        //============================== Misc =================================
        //=====================================================================
        public FileResult Download(int? file_id)
        {
            REATrackerDB sql = new REATrackerDB();
            sql.OpenConnection();
            string cmdText = "SELECT ST_ATTACHMENT.FILENAME, ST_ATTACHMENT.FILE_DATA FROM ST_ATTACHMENT WHERE FILE_ID =" + Convert.ToString(file_id);
            DataTable dt = sql.ProcessCommand(cmdText);
            byte[] data = (dt.Rows.Count > 0 || dt.Rows[0][1] == DBNull.Value) ? (Byte[])dt.Rows[0][1] : null;
            string name = dt.Rows.Count > 0 ? System.IO.Path.GetFileName(Convert.ToString(dt.Rows[0][0])) : "No File Name";
            sql.CloseConnection();
            if (data != null)
            {
                return File(data, "application/octet-stream", name);
            }
            else
            {
                return null;
            }

        }
        public JsonResult GetCompanyNames(String term)
        {
            String command = "SELECT NAME FROM ST_COMPANY_LIST ORDER BY COMPANY_CODE ASC;";
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(command);
            List<String> CompanyList = new List<String>();
            foreach (System.Data.DataRow row in dt.Rows)
            {
                CompanyList.Add(Convert.ToString(row[0]));
            }
            var filteredItems = CompanyList.Where(
                item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0
                );
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
    }
}