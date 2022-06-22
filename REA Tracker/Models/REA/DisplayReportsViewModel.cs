using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using QVICommonIntranet.Database;
using REA_Tracker.Helpers;

namespace REA_Tracker.Models
{
    public class DisplayReportsViewModel
    {
        //ToDo:Taken from report manager, create a file for it later
        public class Report
        {
            public int ReportID { get; set; }
            public String GroupID { get; set; }
            public int OwnerID { get; set; }
            public String OwnerName { get; set; }
            public String Title { get; set; }
            public String Order { get; set; }
            public int Count { get; set; }
            public Report() { }
            public Report(int argsReportID, String argsGroupID, String argOrder, int argsOwnerID, String argsOwnerName, String argsTitle)
            {
                this.ReportID = argsReportID;
                this.GroupID = argsGroupID;
                this.OwnerID = argsOwnerID;
                this.OwnerName = argsOwnerName;
                this.Title = argsTitle;
                this.Order = argOrder;
            }
        }
        public class ReportGroup
        {
            public int GroupID { get; set; }
            public String Name { get; set; }

            public ReportGroup() { }
            public ReportGroup(int argsGroupID, String argsName)
            {
                this.GroupID = argsGroupID;
                this.Name = argsName;
            }
        }

        public List<dynamic> OtherReports { get; set; }
        public List<dynamic> MyReports { get; set; }
        public List<dynamic> CurrentProductReports { get; set; }

        //For the side bar
        public List<ReportGroup> ReportGroups { get; set; }
        public List<Report> AllReports { get; set; }

        //Dropdown to chnage product in report page
        public List<dynamic> productList { get; set; }

        public int USER_ID { get; set; }
        public int CurrentProduct { get; set; }
        public bool isAdmin { get; set; }


        public DisplayReportsViewModel() { }
        public DisplayReportsViewModel(int ArgsUserid, bool isAdmin, int argsCurrentProduct)
        {
            this.USER_ID = ArgsUserid;
            this.isAdmin = isAdmin;
            this.CurrentProduct = argsCurrentProduct;
            this.init(isAdmin);
        }
        private void init(bool isAdmin)
        {
            if (this.MyReports == null)
            {
                this.MyReports = new List<dynamic>();
            }
            if (this.OtherReports == null)
            {
                this.OtherReports = new List<dynamic>();
            }
            REATrackerDB SQL = new REATrackerDB();
            this.productList = ModelHelper.populatedProductList(this.USER_ID, this.CurrentProduct);
            DataTable dtuserreports = SQL.GetAllUsersReports(this.USER_ID);//SQL.GetAllReports(isAdmin);
            DataTable dtnonuserreports = SQL.GetOtherUsersReports(isAdmin, this.USER_ID);
            //Get ALL The Reports
            foreach (System.Data.DataRow row in dtuserreports.Rows)
            {
                this.MyReports.Add(new System.Dynamic.ExpandoObject());
                this.MyReports[this.MyReports.Count() - 1].Name = Convert.ToString(row["NAME"]);
                this.MyReports[this.MyReports.Count() - 1].OwnerID = Convert.ToString(row["OWNER"]);
                this.MyReports[this.MyReports.Count() - 1].OwnerName = Convert.ToString(row["OWNER_NAME"]);
                this.MyReports[this.MyReports.Count() - 1].ReportID = Convert.ToString(row["REPORT_ID"]);
                this.MyReports[this.MyReports.Count() - 1].IsPublic = Convert.ToBoolean(row["PUBLIC_REPORT"]);
                this.MyReports[this.MyReports.Count() - 1].PublicReport = this.MyReports[this.MyReports.Count() - 1].IsPublic ? "Public" : "Private";
                this.MyReports[this.MyReports.Count() - 1].Style = this.MyReports[this.MyReports.Count() - 1].IsPublic ? "normal" : "none";
            }
            //else Sort them into other reports
            foreach (System.Data.DataRow row in dtnonuserreports.Rows)
            {
                this.OtherReports.Add(new System.Dynamic.ExpandoObject());
                this.OtherReports[this.OtherReports.Count() - 1].Name = Convert.ToString(row["NAME"]);
                this.OtherReports[this.OtherReports.Count() - 1].OwnerID = Convert.ToString(row["OWNER"]);
                this.OtherReports[this.OtherReports.Count() - 1].OwnerName = Convert.ToString(row["OWNER_NAME"]);
                this.OtherReports[this.OtherReports.Count() - 1].ReportID = Convert.ToString(row["REPORT_ID"]);
                this.OtherReports[this.OtherReports.Count() - 1].IsPublic = Convert.ToBoolean(row["PUBLIC_REPORT"]);
                this.OtherReports[this.OtherReports.Count() - 1].PublicReport = this.OtherReports[this.OtherReports.Count() - 1].IsPublic ? "Public" : "Private";
                this.OtherReports[this.OtherReports.Count() - 1].Style = this.OtherReports[this.OtherReports.Count() - 1].IsPublic ? "normal" : "none";
            }
            this.getGroupList();
        }
        public void getCurrentProductReports(bool isAdmin)
        {
            REATrackerDB SQL = new REATrackerDB();
            DataTable dt = SQL.GetAllReports(isAdmin);
            //Get ALL The Reports
            foreach (System.Data.DataRow row in dt.Rows)
            {
                //Sort them into Current Products
                if (row["PRODUCT"] != DBNull.Value)
                {
                    int[] ProductsFromDB = Array.ConvertAll(Convert.ToString(row["PRODUCT"]).Split(','), x => int.Parse(x));
                    if (ProductsFromDB.Contains(this.CurrentProduct))
                    {
                        this.CurrentProductReports.Add(new System.Dynamic.ExpandoObject());
                        this.CurrentProductReports[this.CurrentProductReports.Count - 1].Name = Convert.ToString(row["NAME"]);
                        this.CurrentProductReports[this.CurrentProductReports.Count - 1].OwnerID = Convert.ToString(row["OWNER"]);
                        this.CurrentProductReports[this.CurrentProductReports.Count - 1].OwnerName = Convert.ToString(row["OWNER_NAME"]);
                        this.CurrentProductReports[this.CurrentProductReports.Count - 1].ReportID = Convert.ToString(row["REPORT_ID"]);
                        this.CurrentProductReports[this.CurrentProductReports.Count - 1].PublicReport = Convert.ToString(row["PUBLIC_REPORT"]);
                    }
                }
            }
        }
        public void getGroupList()
        {
            this.populateReports();
            this.populateGroups();
            this.populateCount();
        }
        private void populateReports()
        {
            this.AllReports = new List<Report>();
            String Command =
                "SELECT"
                + " ST_REPORT_GROUP.REPORT_GROUP_ID AS GROUP_ID, ST_REPORT_GROUP.TITLE AS GROUP_NAME,"
                + " ST_REPORT_GROUP_LIST.ORDER_LIST AS LIST_ORDER,"
                + " OWNER AS OWNER_ID, (ST_USERS.FIRST_NAME +' '+ST_USERS.LAST_NAME) AS OWNER_NAME,"
                + " NAME, ST_TABULAR_REPORT.REPORT_ID"
                + " FROM ST_TABULAR_REPORT"
                + " INNER JOIN ST_USERS ON ST_USERS.USER_ID = OWNER"
                + " INNER JOIN ST_REPORT_GROUP_LIST ON ST_REPORT_GROUP_LIST.REPORT_ID = ST_TABULAR_REPORT.REPORT_ID"
                + " INNER JOIN ST_REPORT_GROUP ON  ST_REPORT_GROUP_LIST.REPORT_GROUP_ID = ST_REPORT_GROUP.REPORT_GROUP_ID"
                + " ORDER BY GROUP_ID ASC, LIST_ORDER ASC, NAME ASC";
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(Command);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                Report newReport = new Report(
                        Convert.ToInt32(row["REPORT_ID"]), row["GROUP_ID"] == DBNull.Value ? null : Convert.ToString(row["GROUP_ID"]),
                        row["LIST_ORDER"] == DBNull.Value ? null : Convert.ToString(row["LIST_ORDER"]),
                        Convert.ToInt32(row["OWNER_ID"]), Convert.ToString(row["OWNER_NAME"]),
                        Convert.ToString(row["NAME"])
                    );
                this.AllReports.Add(newReport);
            }
        }
        private void populateGroups()
        {
            this.ReportGroups = new List<ReportGroup>();
            String Command = "SELECT * FROM ST_REPORT_GROUP ORDER BY REPORT_GROUP_ID ASC";
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(Command);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                ReportGroup newGroup = new ReportGroup(Convert.ToInt32(row["REPORT_GROUP_ID"]), Convert.ToString(row["TITLE"]));
                this.ReportGroups.Add(newGroup);
            }
        }
        private void populateCount()
        {
            foreach (Report repo in this.AllReports)
            {
                repo.Count = this.getReportCount(repo.ReportID);
            }
        }

        /// <summary>
        /// This function returns the number of records found in the report that is passed in. The report does not exist or there is an error, -1 is returned
        /// </summary>
        /// <param name="ReportID"></param>
        /// <returns>-1 if an error, else the number of rows</returns>
        private int getReportCount(int ReportID)
        {
            int count = -1;
            try
            {

                //TODO: FIX THIS
                String command = "SELECT COUNT(*) ";
                SearchViewModel temp = new SearchViewModel(ReportID, this.USER_ID, this.CurrentProduct);
                temp.ProcessSearchForMassUpdate();
                command += temp.WhereSQLRows;
                int locationOfSortByStatement = command.ToLower().IndexOf(" order by ");
                if (locationOfSortByStatement != -1)
                {
                    command = command.Remove(locationOfSortByStatement);
                }
                REATrackerDB sql = new REATrackerDB();
                DataTable dt = sql.ProcessCommand(command);
                count = dt.Rows.Count == 0 ? 0 : Convert.ToInt32(dt.Rows[0][0]);
            }
            catch (Exception)
            {
                //something didnt go correctly, should not get here
            }

            return count;
        }
        public void DeleteReport(int reportID)
        {
            //this comes from the report list
            REATrackerDB sql = new REATrackerDB();
            //Delete the report
            sql.DeleteReport(reportID);
        }
        public class QuickSearchResultViewModel
        {
            public String SearchTerm { get; set; }
            public List<dynamic> Result { get; set; }
            public int RowCount { get; set; }
            public int? SelectedProductId { get; set; }
            public List<dynamic> Products { get; set; }
            public int UserID { get; set; }

            public QuickSearchResultViewModel(int argsUserID, String argsTerm, int? argsProductID = null)
            {
                if (String.IsNullOrEmpty(argsTerm))
                {
                    this.SearchTerm = "";
                }
                else
                {
                    this.SearchTerm = argsTerm.Trim();
                }
                this.RowCount = 0;
                this.UserID = argsUserID;
                this.SelectedProductId = 0; //default to all products
                if (argsProductID != null)
                {
                    this.SelectedProductId = argsProductID;
                }
                this.populateProducts();
                this.init();
            }
            public QuickSearchResultViewModel()
            {
                this.Products = new List<dynamic>();
                this.populateProducts();
                this.RowCount = 0;
                this.init();
            }
            public void init()
            {
                REATrackerDB sql = new REATrackerDB();
                // Search Database
                if (!String.IsNullOrEmpty(this.SearchTerm))
                {
                    DataTable dt = sql.QuickSearchResult(this.SearchTerm, (int)this.SelectedProductId);
                    this.Result = new List<dynamic>();
                    int i = 0;
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        Result.Add(new System.Dynamic.ExpandoObject());
                        Result[i].Product = Convert.ToString(row["PRODUCT"] != DBNull.Value ? row["PRODUCT"] : "");
                        Result[i].Title = Convert.ToString(row["TITLE"] != DBNull.Value ? row["TITLE"] : "");
                        Result[i].Issue = Convert.ToString(row["ISSUE"] != DBNull.Value ? row["ISSUE"] : "");
                        Result[i].Status = Convert.ToString(row["STATUS"] != DBNull.Value ? row["STATUS"] : "");
                        Result[i].Priority = Convert.ToString(row["priority"] != DBNull.Value ? row["priority"] : "");
                        Result[i].SubmittedByName = Convert.ToString(row["SUBMITTED_BY_NAME"] != DBNull.Value ? row["SUBMITTED_BY_NAME"] : "");
                        Result[i].SubmittedByID = Convert.ToInt32(row["SUBMITTED_BY_ID"] != DBNull.Value ? row["SUBMITTED_BY_ID"] : 0);
                        Result[i].AssignedToName = Convert.ToString(row["ASSIGNED_TO_NAME"] != DBNull.Value ? row["ASSIGNED_TO_NAME"] : "");
                        Result[i].AssignedToID = Convert.ToInt32(row["ASSIGNED_TO_ID"] != DBNull.Value ? row["ASSIGNED_TO_ID"] : 0);
                        Result[i].TrackingID = Convert.ToString(row["TRACKING_ID"] != DBNull.Value ? row["TRACKING_ID"] : "");
                        Result[i].IssueID = Convert.ToString(row["ISSUE_ID"] != DBNull.Value ? row["ISSUE_ID"] : "");
                        Result[i].StatusID = Convert.ToString(row["STATUS_ID"] != DBNull.Value ? row["STATUS_ID"] : "");
                        Result[i].PriorityID = Convert.ToString(row["priority_ID"] != DBNull.Value ? row["priority_ID"] : "");
                        i++;
                    }
                    RowCount = i;
                }
            }
            public void populateProducts()
            {
                this.Products = new List<dynamic>();
                int productID = 0;
                if (this.SelectedProductId != null)
                {
                    productID = (int)this.SelectedProductId;
                }
                this.Products = ModelHelper.populatedProductList(this.UserID, productID, true);
                /*
                REATrackerHelp sql = new REATrackerHelp();
                if (this.SelectedProductId == null)
                {
                    DataTable Default_product = (sql.ProcessCommand("SELECT DEFAULT_PRODUCT FROM ST_USERS WHERE USER_ID = " + Convert.ToString(UserID)));
                    if (Default_product.Rows.Count > 0)
                    {
                        this.SelectedProductId = Convert.ToInt32(Default_product.Rows[0][0]);
                    }
                    else
                    {
                        this.SelectedProductId = -1;
                    }
                }
                Boolean ShowProducts = true;
                DataTable DtProducts = sql.GetProductList(ShowProducts);
                this.Products.Add(new System.Dynamic.ExpandoObject());
                this.Products[0].ID = 0;
                this.Products[0].Name = "";
                this.Products[0].IsDefault = (this.SelectedProductId == null || this.SelectedProductId == 0) ? " selected " : "";
                int j = 1;
                foreach (System.Data.DataRow Product in DtProducts.Rows)
                {
                    this.Products.Add(new System.Dynamic.ExpandoObject());
                    this.Products[j].ID = Convert.ToInt32(Product["PRODUCT_ID"]);
                    this.Products[j].Name = Convert.ToString(Product["NAME"]);
                    this.Products[j].IsDefault = (this.SelectedProductId == Convert.ToInt32(Product["PRODUCT_ID"])) ? " selected " : "";
                    j++;
                }
                */
                //this.Products.Sort((x, y) => String.Compare(x.Name, y.Name));
            }
        }
        public class MassUpdateViewModel
        {
            public int UserID { get; set; }
            public int ReportID { get; set; }
            public SearchViewModel SearchModel { get; set; }
            public List<dynamic> DisplayRows { get; set; }

            //New Planned Release
            public int? VPMajor { get; set; }
            public int? VPMinor { get; set; }
            public int? VPBuild { get; set; }
            public String VPMisc { get; set; }

            public int IssueType { get; set; }
            public int Priority { get; set; }
            public int Status { get; set; }
            public int AssignedTo { get; set; }

            public Boolean updateVP { get; set; }
            public Boolean updateVPMajor { get; set; }
            public Boolean updateVPMinor { get; set; }
            public Boolean updateVPBuild { get; set; }
            public Boolean updateVPMisc { get; set; }

            public Boolean updateIssueType { get; set; }
            public Boolean updatePriority { get; set; }
            public Boolean updateStatus { get; set; }
            public Boolean updateAssignedTo { get; set; }

            public String ClosedNotes { get; set; }
            public int ClosedReason { get; set; }
            public int VCMajor { get; set; }
            public int VCMinor { get; set; }
            public int VCBuild { get; set; }

            public int CurrentProductID { get; set; }
            public String SelectedSCRs { get; set; }

            public int Product { get; set; }
            public int Type { get; set; }

            public MassUpdateViewModel()
            {
            }
            public MassUpdateViewModel(int ReportID, int argsUserID, int argsCurrentProductID)
            {
                this.CurrentProductID = argsCurrentProductID;
                this.UserID = argsUserID;
                this.init(ReportID);
            }
            public MassUpdateViewModel(int ProductID, int type, int argsUserID, int argsCurrentProductID)
            {
                this.CurrentProductID = argsCurrentProductID;
                this.UserID = argsUserID;
                this.init(ProductID, type);
                this.Product = ProductID;
                this.Type = type;
            }
            public void init(int ReportID)
            {
                this.ReportID = ReportID;
                //SCRs Populated in Search Model
                this.SearchModel = new SearchViewModel(ReportID, this.UserID, this.CurrentProductID);
                this.SearchModel.ProcessSearchForMassUpdate();
                this.DisplayRows = new List<dynamic>();
                this.PopulateSCRs();
            }
            public void init(int productID, int type)
            {
                this.SearchModel = new SearchViewModel(this.UserID,
                    productID
                    );
                this.DisplayRows = new List<dynamic>();
                string typeOfMassUpdate = "";
                switch (type)
                {
                    case 1:
                        typeOfMassUpdate = "ST_TRACK.STATUS = 1";
                        break;
                    case 2:
                        typeOfMassUpdate = "ST_TRACK.STATUS IN (5)";
                        break;
                    case 3:
                        typeOfMassUpdate = "ST_TRACK.STATUS IN (6,7)";
                        break;
                    case 4:
                        typeOfMassUpdate = "ST_TRACK.STATUS IN (8,10,11)";
                        break;
                    case 5:
                        typeOfMassUpdate = "ST_TRACK.STATUS IN (3)";
                        break;
                    case 6:
                        typeOfMassUpdate = "ST_TRACK.ISSUE_TYPE = 1 AND ST_TRACK.STATUS IN (1,2,5,6,7)";
                        break;
                    case 7:
                        typeOfMassUpdate = "ST_TRACK.ISSUE_TYPE = 1 AND ST_TRACK.STATUS IN (1,2,5,6,7)";
                        break;
                    case 8:
                        typeOfMassUpdate = "ST_TRACK.ISSUE_TYPE = 1 AND ST_TRACK.STATUS IN (1,2,5,6,7)";
                        break;
                    case 9:
                        typeOfMassUpdate = "ST_TRACK.priority = 1 AND (STATUS IN (1,2,5,6,7)) AND (ISSUE_TYPE = 1)";
                        break;
                    case 10:
                        typeOfMassUpdate = "ST_TRACK.priority = 2 AND (STATUS IN (1,2,5,6,7)) AND (ISSUE_TYPE = 1)";
                        break;
                    case 11:
                        typeOfMassUpdate = "ST_TRACK.priority = 3 AND (STATUS IN (1,2,5,6,7)) AND (ISSUE_TYPE = 1)";
                        break;
                    case 12:
                        typeOfMassUpdate = "ST_TRACK.priority = 4 AND (STATUS IN (1,2,5,6,7)) AND (ISSUE_TYPE = 1)";
                        break;
                    case 17:
                        typeOfMassUpdate = "ST_TRACK.STATUS IN (2)";
                        break;

                }
                typeOfMassUpdate += " AND PRODUCT =" + productID;
                this.PopulateDashBoardSCRs(typeOfMassUpdate);
            }

            public void PopulateSCRs()
            {
                String Command =
                    @"SELECT 
                        ST_TRACK.TRACKING_ID, ST_TRACK.TITLE, ST_TRACK.PRIORITY, ST_TRACK.STATUS, ST_TRACK.ASSIGNED_TO, ST_TRACK.ISSUE_TYPE, ST_TRACK.PRODUCT, ST_TRACK.ISSUE_TYPE, 
                        ST_PRODUCT.NAME, ST_TRACK.VP_MAJOR, ST_TRACK.VP_MINOR, ST_TRACK.VP_BUILD, ST_TRACK.VP_MISC "
                    + this.SearchModel.WhereSQLRowsForMassUpdate + ";";             //SEK SCR78145 2-2-2022

                //this is needed because the "FROM" stuff is part of the variable that holds WHERE clause
                //this is a temporary work around
                if (!this.SearchModel.WhereSQLRowsForMassUpdate.Contains("INNER JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_TRACK.PRODUCT"))
                {
                    Command = Command.Replace("FROM ST_TRACK", "FROM ST_TRACK INNER JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_TRACK.PRODUCT ");
                }

                REATrackerDB sql = new REATrackerDB();
                DataTable dt = sql.ProcessCommand(Command);
                int rowCount = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    this.DisplayRows.Add(new System.Dynamic.ExpandoObject());
                    this.DisplayRows[rowCount].SCR = Convert.ToString(row["TRACKING_ID"]);
                    this.DisplayRows[rowCount].Title = Convert.ToString(row["TITLE"]);
                    this.DisplayRows[rowCount].Priority = Convert.ToString(row["priority"]);
                    this.DisplayRows[rowCount].Status = this.GetStatus(Convert.ToString(row["STATUS"]));
                    this.DisplayRows[rowCount].AssignedTo = (row["ASSIGNED_TO"] != DBNull.Value ? sql.getFullName(Convert.ToInt32(row["ASSIGNED_TO"])) : "");
                    this.DisplayRows[rowCount].PlannedRealease = formattVersion(
                        Convert.ToString(isDBNull(row["VP_MAJOR"])),
                        Convert.ToString(isDBNull(row["VP_MINOR"])),
                        Convert.ToString(isDBNull(row["VP_BUILD"])),
                        Convert.ToString(isDBNull(row["VP_MISC"]))
                        );
                    this.DisplayRows[rowCount].IssueType = Convert.ToString(row["ISSUE_TYPE"]);
                    this.DisplayRows[rowCount].ProductName = Convert.ToString(row["NAME"]);
                    rowCount++;
                }
            }

            public void PopulateDashBoardSCRs(string whereString)
            {
                String Command =
                    "SELECT ST_TRACK.TRACKING_ID, ST_TRACK.TITLE, ST_TRACK.priority, ST_TRACK.STATUS, ST_TRACK.ASSIGNED_TO, ST_TRACK.ISSUE_TYPE, ST_TRACK.PRODUCT, ST_TRACK.ISSUE_TYPE, ST_PRODUCT.NAME," +
                    " ST_TRACK.VP_MAJOR, ST_TRACK.VP_MINOR, ST_TRACK.VP_BUILD, ST_TRACK.VP_MISC " +
                    "FROM ST_TRACK INNER JOIN ST_PRODUCT ON ST_TRACK.PRODUCT = ST_PRODUCT.PRODUCT_ID " +
                       " WHERE " + whereString;
                REATrackerDB sql = new REATrackerDB();
                DataTable dt = sql.ProcessCommand(Command);
                int rowCount = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    this.DisplayRows.Add(new System.Dynamic.ExpandoObject());
                    this.DisplayRows[rowCount].SCR = Convert.ToString(row["TRACKING_ID"]);
                    this.DisplayRows[rowCount].Title = Convert.ToString(row["TITLE"]);
                    this.DisplayRows[rowCount].Priority = Convert.ToString(row["priority"]);
                    this.DisplayRows[rowCount].Status = this.GetStatus(Convert.ToString(row["STATUS"]));
                    this.DisplayRows[rowCount].AssignedTo = (row["ASSIGNED_TO"] != DBNull.Value ? sql.getFullName(Convert.ToInt32(row["ASSIGNED_TO"])) : "");
                    this.DisplayRows[rowCount].PlannedRealease = formattVersion(
                        Convert.ToString(isDBNull(row["VP_MAJOR"])),
                        Convert.ToString(isDBNull(row["VP_MINOR"])),
                        Convert.ToString(isDBNull(row["VP_BUILD"])),
                        Convert.ToString(isDBNull(row["VP_MISC"]))
                        );
                    this.DisplayRows[rowCount].IssueType = Convert.ToString(row["ISSUE_TYPE"]);
                    this.DisplayRows[rowCount].ProductName = Convert.ToString(row["NAME"]);
                    rowCount++;
                }
            }

            private object isDBNull(object item)
            {
                object returnValue = item;
                if (item == DBNull.Value)
                {
                    returnValue = "";
                }
                return returnValue;
            }

            private String formattVersion(String Major, String Minor, String Build, String Misc)
            {
                String Version = "";
                Version += (String.IsNullOrEmpty(Major) ? "?." : (Major + "."));
                Version += (String.IsNullOrEmpty(Minor) ? "?." : (Minor + "."));
                Version += (String.IsNullOrEmpty(Build) ? "? " : (Build + " "));
                Version += (String.IsNullOrEmpty(Misc) ? "?" : (Misc));
                return Version;
            }

            private String GetStatus(String input)
            {
                String Value = "";
                if (!String.IsNullOrEmpty(input))
                {
                    REATrackerDB sql = new REATrackerDB();
                    String Command = "SELECT NAME FROM ST_STATUS WHERE STATUS_ID = " + input;
                    Value = Convert.ToString(sql.ProcessScalarCommand(Command));
                }
                return Value;
            }

            public void ProcessMassUpdate()
            {
                DateTime NOW = DateTime.Now;

                if (this.VPMisc != null)
                {
                    this.VPMisc = this.VPMisc.Replace("'", "''");
                }
                if (this.ClosedNotes != null)
                {
                    this.ClosedNotes = this.ClosedNotes.Replace("'", "''");
                }
                if (this.SelectedSCRs != null && this.SelectedSCRs != "")
                {
                    String command = "";

                    /*if( this.updateVP )
                    {
                        command += (" VP_MAJOR = "  + ((this.VPMajor == null)? "NULL" : "'"+ this.VPMajor +"'" ));
                        command += (", VP_MINOR = " + ((this.VPMinor == null)? "NULL" : "'" + this.VPMinor + "'"));
                        command += (", VP_BUILD = " + ((this.VPBuild == null)? "NULL" : "'" + this.VPBuild + "'"));
                        command += (", VP_MISC = "  + ((this.VPMisc  == null)? "NULL" : "'" + this.VPMisc + "'"));
                    }*/
                    if (this.updateVPMajor)
                    {
                        command += (command != "" ? "," : "");
                        command += (" VP_MAJOR = " + ((this.VPMajor == null) ? "NULL" : "'" + this.VPMajor + "'"));
                    }
                    if (this.updateVPMinor)
                    {
                        command += (command != "" ? "," : "");
                        command += (" VP_MINOR = " + ((this.VPMinor == null) ? "NULL" : "'" + this.VPMinor + "'"));
                    }
                    if (this.updateVPBuild)
                    {
                        command += (command != "" ? "," : "");
                        command += (" VP_BUILD = " + ((this.VPBuild == null) ? "NULL" : "'" + this.VPBuild + "'"));
                    }
                    if (this.updateVPMisc)
                    {
                        command += (command != "" ? "," : "");
                        command += (" VP_MISC = " + ((this.VPMisc == null) ? "NULL" : "'" + this.VPMisc + "'"));
                    }
                    if (this.updateIssueType)
                    {
                        command += (command != "" ? "," : "");
                        command += (" ISSUE_TYPE = " + this.IssueType);
                    }
                    if (this.updatePriority)
                    {
                        command += (command != "" ? "," : "");
                        command += (" priority = " + this.Priority);
                    }
                    if (this.updateStatus)
                    {
                        command += (command != "" ? "," : "");
                        command += (" STATUS = " + this.Status);
                        if (this.Status == (int)REATrackerDB.StatusChangeID.status_closed)
                        {
                            //Set the closed version
                            command += ",VC_MAJOR = " + ("'" + this.VCMajor + "'");
                            command += ",VC_MINOR = " + ("'" + this.VCMinor + "'");
                            command += ",VC_BUILD = " + ("'" + this.VCBuild + "'");
                            //Default values
                            command += (@",ASSIGNED_TO = NULL ");
                            command += ",EST_HOURS_TO_FIX = ISNULL( EST_HOURS_TO_FIX, '0' ), ACT_HOURS_TO_FIX=ISNULL(ACT_HOURS_TO_FIX,'0'),ACT_HOURS_TO_TEST=ISNULL(ACT_HOURS_TO_TEST,'0')";
                            //ADD SAVE THE REASON CLOSED
                            command += ", CLOSED_BY = " + this.UserID;
                            command += ", CLOSED_ON = '" + NOW + "'";
                            command += ", CLOSED_REASON = " + this.ClosedReason;

                        }
                    }
                    if (this.updateAssignedTo && ((this.updateStatus == false) ||
                        (this.updateStatus && (this.Status != (int)REATrackerDB.StatusChangeID.status_closed))))
                    {
                        command += (command != "" ? "," : "");
                        command += (" ASSIGNED_TO = " + this.AssignedTo);
                    }
                    if (command != "")
                    {
                        command += " ,LAST_UPDATE = '" + NOW + "'";
                        REATrackerDB sql = new REATrackerDB();
                        List<String> SelectedSCRids = new List<string>(this.SelectedSCRs.Split(','));
                        for (int i = 0; i < SelectedSCRids.Count; i++)
                        {
                            DataTable SCRInfo = sql.ProcessCommand(
                                "SELECT ST_TRACK.TRACKING_ID, ST_priority.ID AS SERVERITY, ST_STATUS.NAME AS STATUS, ST_ISSUE_TYPE.NAME AS ISSUE_TYPE, " +
                                "(ST_USERS.FIRST_NAME+' '+ST_USERS.LAST_NAME)AS AssignedTo " +
                                "FROM ST_TRACK " +
                                "INNER JOIN ST_priority ON ST_priority.ID = ST_TRACK.priority " +
                                "INNER JOIN ST_STATUS ON ST_STATUS.STATUS_ID = ST_TRACK.STATUS " +
                                "INNER JOIN ST_ISSUE_TYPE ON ST_ISSUE_TYPE.ISSUE_ID = ST_TRACK.ISSUE_TYPE " +
                                "LEFT JOIN ST_USERS ON ST_USERS.USER_ID = ST_TRACK.ASSIGNED_TO " +
                                "WHERE TRACKING_ID = " + SelectedSCRids[i] + ";");
                            if (this.updateVP)
                            {
                                SqlParameter Description = new SqlParameter();
                                Description.SqlDbType = SqlDbType.VarChar;
                                Description.ParameterName = "DESCRIPTION";
                                Description.Value = (this.VPMajor == null ? "?" : Convert.ToString(this.VPMajor)) + "."
                                                    + (this.VPMinor == null ? "?" : Convert.ToString(this.VPMinor)) + "."
                                                    + (this.VPBuild == null ? "?" : Convert.ToString(this.VPBuild)) + " "
                                                    + (this.VPMisc == null ? "?" : Convert.ToString(this.VPMisc));
                                sql.CreateHistory(Convert.ToInt32(SelectedSCRids[i]), (int)REATrackerDB.HistoryChangeType.chg_planned_release, this.UserID,
                                                    NOW, Description, null);
                            }
                            if (this.updateIssueType)
                            {
                                SqlParameter Description = new SqlParameter();
                                Description.SqlDbType = SqlDbType.VarChar;
                                Description.ParameterName = "DESCRIPTION";
                                Description.Value =
                                    "SCR issue type was changed from " + (Convert.ToString(SCRInfo.Rows[0]["ISSUE_TYPE"])) + " to " + GetIssueType(this.IssueType) + ".";
                                sql.CreateHistory(Convert.ToInt32(SelectedSCRids[i]), (int)REATrackerDB.HistoryChangeType.chg_issuetype, this.UserID, NOW, Description, null);
                            }
                            if (this.updatePriority)
                            {
                                SqlParameter Description = new SqlParameter();
                                Description.SqlDbType = SqlDbType.VarChar;
                                Description.ParameterName = "DESCRIPTION";
                                Description.Value =
                                    "SCR Priority was changed from " + GetPriority(Convert.ToInt32(SCRInfo.Rows[0]["SERVERITY"])) + " to " + GetPriority(this.Priority) + ".";
                                sql.CreateHistory(Convert.ToInt32(SelectedSCRids[i]), (int)REATrackerDB.HistoryChangeType.chg_priority, this.UserID, NOW, Description, null);
                            }
                            if (this.updateStatus)
                            {
                                SqlParameter Description = new SqlParameter();
                                Description.SqlDbType = SqlDbType.VarChar;
                                Description.ParameterName = "DESCRIPTION";
                                Description.Value = GetStatusDescription();
                                sql.CreateHistory(Convert.ToInt32(SelectedSCRids[i]), (int)REATrackerDB.HistoryChangeType.chg_detail, this.UserID, NOW, Description, null);

                                //ToDo: Refactor Note - Make a API for SCRs to make it easier to get and set information of a particular SCR
                                string GetHoursCommand = " SELECT EST_HOURS_TO_FIX, ACT_HOURS_TO_FIX , ACT_HOURS_TO_TEST FROM ST_TRACK WHERE TRACKING_ID = " + SelectedSCRids[i];
                                DataTable dtHours = sql.ProcessCommand(GetHoursCommand);
                                if (dtHours.Rows.Count != 0)
                                {
                                    Description.Value = "0";
                                    if (dtHours.Rows[0]["EST_HOURS_TO_FIX"] == DBNull.Value)
                                    {
                                        sql.CreateHistory(Convert.ToInt32(SelectedSCRids[i]), (int)REATrackerDB.HistoryChangeType.chg_estimate_to_fix, this.UserID, NOW, Description, null);
                                    }
                                    if (dtHours.Rows[0]["ACT_HOURS_TO_FIX"] == DBNull.Value)
                                    {
                                        sql.CreateHistory(Convert.ToInt32(SelectedSCRids[i]), (int)REATrackerDB.HistoryChangeType.chg_actual_to_fix, this.UserID, NOW, Description, null);
                                    }
                                    if (dtHours.Rows[0]["ACT_HOURS_TO_TEST"] == DBNull.Value)
                                    {
                                        sql.CreateHistory(Convert.ToInt32(SelectedSCRids[i]), (int)REATrackerDB.HistoryChangeType.chg_actual_to_test, this.UserID, NOW, Description, null);
                                    }
                                }
                                Description.Value = (this.VCMajor + "." + this.VCMinor + "." + this.VCBuild);
                                sql.CreateHistory(Convert.ToInt32(SelectedSCRids[i]), (int)REATrackerDB.HistoryChangeType.chg_actual_release, this.UserID, NOW, Description, null);


                                if (this.Status == (int)REATrackerDB.StatusChangeID.status_closed)
                                {// enter in close details
                                    if (!String.IsNullOrWhiteSpace(this.ClosedNotes))
                                    {
                                        Description.Value = this.ClosedNotes;
                                        sql.CreateHistory(Convert.ToInt32(SelectedSCRids[i]), (int)REATrackerDB.HistoryChangeType.chg_resolution, this.UserID, NOW, Description, null);
                                    }

                                    Description.Value = "Assigned to nobody";
                                    sql.CreateHistory(Convert.ToInt32(SelectedSCRids[i]), (int)REATrackerDB.HistoryChangeType.chg_assignto, this.UserID, NOW, Description, null);


                                }
                            }
                            if (this.updateAssignedTo && ((this.updateStatus == false) ||
                                (this.updateStatus && (this.Status != (int)REATrackerDB.StatusChangeID.status_closed))))
                            {
                                SqlParameter Description = new SqlParameter();
                                Description.SqlDbType = SqlDbType.VarChar;
                                Description.ParameterName = "DESCRIPTION";
                                DataTable dt = sql.ProcessCommand("SELECT FIRST_NAME, LAST_NAME FROM ST_USERS WHERE (USER_ID= " + Convert.ToString((int)this.AssignedTo) + " );");
                                String fullname = "";
                                if (dt.Rows.Count > 0)
                                {
                                    fullname = Convert.ToString(dt.Rows[0][0]) + " " + Convert.ToString(dt.Rows[0][1]);
                                }
                                Description.Value = "Assigned to " + fullname + ".";
                                sql.CreateHistory(Convert.ToInt32(SelectedSCRids[i]), (int)REATrackerDB.HistoryChangeType.chg_assignto, this.UserID, NOW, Description, null);
                            }
                        }
                        command = "UPDATE ST_TRACK SET" + command + " WHERE TRACKING_ID IN ( " + this.SelectedSCRs + " )";
                        sql.ProcessCommand(command);
                    }
                }
            }
            private String GetIssueType(int IssueID)
            {
                String Result = "";
                if (IssueID == 1)
                {
                    Result = "Problem";
                }
                else if (IssueID == 2)
                {
                    Result = "Enhancement";
                }
                else if (IssueID == 3)
                {
                    Result = "Planned Work";
                }
                return Result;
            }
            private String GetPriority(int PriorityID)
            {
                String result = "";
                if (PriorityID == 1)
                {
                    result = "Low";
                }
                else if (PriorityID == 2)
                {
                    result = "Medium";
                }
                else if (PriorityID == 3)
                {
                    result = "High";
                }
                else if (PriorityID == 4)
                {
                    result = "Critical";
                }
                return result;
            }
            private String GetStatusDescription()
            {
                string localMessage = "";
                HttpContext context = HttpContext.Current;
                if (this.Status == (int)REATrackerDB.StatusChangeID.status_submitted)
                {
                    localMessage = "Set to Submitted by " + Convert.ToString(context.Session["CurrentUserName"]); ;
                }
                else if (this.Status == (int)REATrackerDB.StatusChangeID.status_approved)
                {
                    localMessage = "Set to Approved by " + Convert.ToString(context.Session["CurrentUserName"]); ;
                }
                else if (this.Status == (int)REATrackerDB.StatusChangeID.status_deferred)
                {
                    localMessage = "Set to Deferred by " + Convert.ToString(context.Session["CurrentUserName"]); ;
                }
                else if (this.Status == (int)REATrackerDB.StatusChangeID.status_rejected)
                {
                    localMessage = "Set to Rejected by " + Convert.ToString(context.Session["CurrentUserName"]); ;
                }
                else if (this.Status == (int)REATrackerDB.StatusChangeID.status_inprocess)
                {
                    localMessage = "Set to In Process by " + Convert.ToString(context.Session["CurrentUserName"]); ;
                }
                else if (this.Status == (int)REATrackerDB.StatusChangeID.status_fixed)
                {
                    localMessage = "Set to Fixed by " + Convert.ToString(context.Session["CurrentUserName"]); ;
                }
                else if (this.Status == (int)REATrackerDB.StatusChangeID.status_delivered)
                {
                    localMessage = "Set to Delivered by " + Convert.ToString(context.Session["CurrentUserName"]); ;
                }
                else if (this.Status == (int)REATrackerDB.StatusChangeID.status_testing)
                {
                    localMessage = "Set to Testing by " + Convert.ToString(context.Session["CurrentUserName"]); ;
                }
                else if (this.Status == (int)REATrackerDB.StatusChangeID.status_closed)
                {
                    localMessage = "Set to Closed by " + Convert.ToString(context.Session["CurrentUserName"]); ;
                }
                else if (this.Status == (int)REATrackerDB.StatusChangeID.status_onhold)
                {
                    localMessage = "Set to On Hold by " + Convert.ToString(context.Session["CurrentUserName"]); ;
                }
                return localMessage;
            }

            public Boolean IsManagerOrAdmin()
            {
                //Boolean value = false;
                REATrackerDB sql = new REATrackerDB();
                return sql.isAdminOrManager(this.UserID);
            }

            public void ProcessExternalMassUpdate(String SetClause, String WhereClause)
            {
                String command = "UPDATE ST_TRACKER SET " + SetClause + "WHERE ";
                REATrackerDB sql = new REATrackerDB();
                sql.ProcessCommand(command);
            }


        }
        public class AddToWatchList
        {
            public int SCRID { get; set; }
            public String UserIDs { get; set; }
            public Boolean EmailNotify { get; set; }
            public List<dynamic> people { get; set; }
            public int department { get; set; }
            public List<dynamic> departments { get; set; }
            public String SearchUserID { get; set; }
            public String SearchFirstName { get; set; }
            public String SearchLastName { get; set; }

            public AddToWatchList() { }
            public AddToWatchList(int id)
            {
                this.SCRID = id;
            }

            public void Process(int SCRID, String UserIDs, Boolean EmailNotify)
            {
                if (!String.IsNullOrEmpty(UserIDs))
                {
                    String[] Users = UserIDs.Split(',');
                    REATrackerDB sql = new REATrackerDB();
                    int newWatchid = Convert.ToInt32(sql.ProcessScalarCommand("SELECT MAX(WATCH_ID) FROM REA_WATCH_LIST"));
                    newWatchid++;

                    foreach (String User in Users)
                    {
                        String command = "INSERT INTO REA_WATCH_LIST(USER_ID,TRACKING_ID,ROW_VER, EMAIL_NOTIFY, WATCH_ID) VALUES("
                            + User + "," + SCRID + ",1," + (EmailNotify ? "1," : "0,") + newWatchid + ")";
                        sql.ProcessCommand(command);
                        newWatchid++;
                    }
                }
            }
            public void PopulateDepartmentList()
            {
                REATrackerDB sql = new REATrackerDB();
                String Command = "SELECT DEPARTMENT_ID, NAME FROM ST_DEPARTMENT ";
                DataTable dt = sql.ProcessCommand(Command);
                this.departments = new List<dynamic>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    this.departments.Add(new System.Dynamic.ExpandoObject());
                    this.departments[i].Name = Convert.ToString(dt.Rows[i]["NAME"]);
                    this.departments[i].ID = Convert.ToInt32(dt.Rows[i]["DEPARTMENT_ID"]);
                    this.departments[i].selected = (this.department == Convert.ToInt32(dt.Rows[i]["DEPARTMENT_ID"]) ? "selected" : "");
                }
            }
            public void PopulatePeopleList(int department)
            {
                REATrackerDB sql = new REATrackerDB();
                //Get list of users with the scr on the watch list already
                String UsersWatching = "";
                DataTable watchingdt = sql.ProcessCommand("Select USER_ID FROM REA_WATCH_LIST WHERE TRACKING_ID =" + this.SCRID);
                for (int i = 0; i < watchingdt.Rows.Count; i++)
                {
                    if (i != 0)
                    {
                        UsersWatching += ",";
                    }
                    UsersWatching += Convert.ToString(watchingdt.Rows[i]["USER_ID"]);
                }
                //populate the list without those people
                String Command = "SELECT ST_USERS.USER_ID, ST_USERS.LOGON, ST_USERS.FIRST_NAME, ST_USERS.LAST_NAME FROM ST_USERS";
                String Where = "";
                if (department != 0)
                {
                    if (String.IsNullOrEmpty(Where))
                    {
                        Where += " WHERE ";
                    }
                    Where += " DEPARTMENT_ID = '" + department + "'";
                }
                if (!String.IsNullOrEmpty(this.SearchUserID))
                {
                    if (String.IsNullOrEmpty(Where))
                    {
                        Where += " WHERE ";
                    }
                    else
                    {
                        Where += " AND ";
                    }
                    Where += " LOGON LIKE '%" + this.SearchUserID + "%' ";
                }
                if (!String.IsNullOrEmpty(this.SearchFirstName))
                {
                    if (String.IsNullOrEmpty(Where))
                    {
                        Where += " WHERE ";
                    }
                    else
                    {
                        Where += " AND ";
                    }
                    Where += " FIRST_NAME LIKE '%" + this.SearchFirstName + "%' ";
                }
                if (!String.IsNullOrEmpty(this.SearchLastName))
                {
                    if (String.IsNullOrEmpty(Where))
                    {
                        Where += " WHERE ";
                    }
                    else
                    {
                        Where += " AND ";
                    }
                    Where += " LAST_NAME LIKE '%" + this.SearchLastName + "%' ";
                }
                if (!String.IsNullOrEmpty(UsersWatching))
                {
                    if (String.IsNullOrEmpty(Where))
                    {
                        Where += " WHERE ST_USERS.USER_ID NOT IN (" + UsersWatching + ")";
                    }
                    else
                    {
                        Where += " AND ST_USERS.USER_ID NOT IN (" + UsersWatching + ")";
                    }
                }
                if (String.IsNullOrEmpty(Where))
                {
                    Where += " WHERE ";
                }
                else
                {
                    Where += " AND ";
                }
                Where += " ACCOUNT_TYPE != -1  ORDER BY FIRST_NAME, LAST_NAME ASC ";
                DataTable dt = sql.ProcessCommand(Command + Where);
                this.people = new List<dynamic>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    this.people.Add(new System.Dynamic.ExpandoObject());
                    this.people[i].ID = dt.Rows[i]["USER_ID"];
                    this.people[i].Logon = Convert.ToString(dt.Rows[i]["LOGON"]);
                    this.people[i].Name = Convert.ToString(dt.Rows[i]["FIRST_NAME"]) + " " + Convert.ToString(dt.Rows[i]["LAST_NAME"]);
                }
            }
            public void Remove(int SCRID, int UserID)
            {
                String Command = "DELETE FROM REA_WATCH_LIST WHERE USER_ID = " + UserID + " AND TRACKING_ID = " + SCRID;
                new REATrackerDB().ProcessCommand(Command);
            }
            public Boolean isinWatchList(int USER_ID)
            {
                Boolean value = false;
                REATrackerDB sql = new REATrackerDB();
                if (sql.ProcessCommand("SELECT * FROM REA_WATCH_LIST WHERE USER_ID = " + USER_ID + " AND TRACKING_ID =" + this.SCRID).Rows.Count > 0)
                {
                    value = true;
                }
                return value;
            }
        }
        public class QuickSearchREAViewModel
        {
            public int ID { get; set; }

            public DataTable SCRIDs { get; set; }


            public QuickSearchREAViewModel()
            {
                //for the post back 
                this.SCRIDs = new DataTable();
            }

            public QuickSearchREAViewModel(int id)
            {
                this.ID = id;
                this.SCRIDs = new DataTable(); //Protective
                this.populateSCRIDs(this.ID);
            }

            public void populateSCRIDs(int id)
            {
                string command =
     "SELECT DISTINCT ST_PRODUCT.NAME AS PRODUCT, ST_TRACK.SYTELINE_REF AS SYTELINE_ID, ST_TRACK.TRACKING_ID AS TRACKING_ID, ST_TRACK.TITLE AS TITLE," +
                    " ST_ISSUE_TYPE.NAME AS ISSUE, ST_priority.NAME AS priority, ST_STATUS.NAME AS STATUS, " +
                    " (USER1.FIRST_NAME + ' ' + USER1.LAST_NAME) AS SUBMITTED_BY_NAME, " +
                    " ST_TRACK.SUBMITTED_BY AS SUBMITTED_BY_ID, " +
                    " (USER2.FIRST_NAME + ' ' + USER2.LAST_NAME) AS ASSIGNED_TO_NAME, " +
                    " ST_TRACK.ASSIGNED_TO AS ASSIGNED_TO_ID, " +
                    " ST_TRACK.PRIORITY AS 'PRIORITY_ID', ST_TRACK.ISSUE_TYPE AS 'ISSUE_ID', ST_TRACK.STATUS AS 'STATUS_ID', " +
                    " ST_TRACK.TRACKING_ID2 AS 'LegacyID'" +
                    " FROM ST_TRACK " +
                    " INNER JOIN ST_HISTORY ON ST_TRACK.TRACKING_ID = ST_HISTORY.TRACKING_ID " +
                    "INNER JOIN ST_PRODUCT ON ST_TRACK.PRODUCT = ST_PRODUCT.PRODUCT_ID " +
                    "INNER JOIN ST_ISSUE_TYPE ON ST_TRACK.ISSUE_TYPE = ST_ISSUE_TYPE.ISSUE_ID " +
                    "INNER JOIN ST_priority ON ST_TRACK.priority = ST_priority.ID " +
                    "INNER JOIN ST_STATUS ON ST_TRACK.STATUS = ST_STATUS.STATUS_ID " +
                    "INNER JOIN ST_USERS USER1 ON ST_TRACK.SUBMITTED_BY = USER1.USER_ID " +
                    "LEFT OUTER JOIN ST_USERS USER2 ON ST_TRACK.ASSIGNED_TO = USER2.USER_ID " +
                    "WHERE ST_TRACK.TRACKING_ID = " + this.ID + " OR ST_TRACK.TRACKING_ID2 =" + this.ID +
                    " ORDER BY TRACKING_ID ASC;";
                this.SCRIDs = new REATrackerDB().ProcessCommand(command);
            }

        }
    }
}