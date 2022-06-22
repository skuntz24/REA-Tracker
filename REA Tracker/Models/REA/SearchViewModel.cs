using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{
  public class SearchViewModel
    {
        public static string RELATED_REAS_STRING = "(SELECT STUFF((SELECT ',' + CAST(t.REA_ID AS varchar(10)) FROM ST_RELATED t WHERE (t.TRACKING_ID = REA_TRACK.TRACKING_ID) FOR XML PATH('')), 1, 1, ''))";
        public class Item
        {
            public string Name { get; set; }
            public int ID { get; set; }
            public bool Selected { get; set; }
            public int Order { get; set; }
            public string Group { get; set; }
            public Item() { }
        }
        public int ReportID { get; set; }
        //Display
        public int lastTab { get; set; }
        public List<dynamic> Employees { get; set; }
        public List<Item> Statuses { get; set; }
        public List<Item> Keywords { get; set; }
        public List<Item> Priorities { get; set; }
        public List<Item> IssueTypes { get; set; }
        public List<Item> Products { get; set; }
        //Input Fields
        //REA Tab
        public String REANumber { get; set; }
        public int REAComparator { get; set; }
        /* 0 = " " - Nothing
         * 1 = is
         * 2 = is at least
         * 3 = is at most
         * 4 = is between
         */
        public String REANumber2 { get; set; }
        public String LegacyREANumber { get; set; }
        public int LegacyREAComparator { get; set; }
        public String LegacyREANumber2 { get; set; }
        //Title Search
        public String TitleText { get; set; }
        public int TextTypeSearch { get; set; } //   any words, all words, any of
        //Selecting Products
        public List<Item> UnSelectedProducts { get; set; }
        public List<Item> SelectedProducts { get; set; }
        public String HiddenProductString { get; set; }
        public String ModuleName { get; set; }
        public String HistoryTerm { get; set; }
        public int HistorySearchLogic { get; set; }
        //Issue Type tab
        public List<Item> SelectedIssueTypes { get; set; }
        //priority tab
        public List<Item> SelectedPriority { get; set; }
        //Status tab
        public List<Item> SelectedStatus { get; set; }
        public List<Item> SelectedKeyword { get; set; }
        //Version Found Tab
        public Version VersionFound { get; set; }
        //Version Planned Tab
        public Version VersionPlanned { get; set; }
        //Version Resolved Tab
        public Version VersionResolved { get; set; }
        //Version Closed Tab
        public Version VersionClosed { get; set; }
        //Submitted By Tab
        public int? SubmittedByID { get; set; }
        public DMYDate SubmittedStartDate { get; set; }
        public DMYDate SubmittedEndDate { get; set; }
        public int SubmittedStartComparator { get; set; }
        // 0:"",
        // 1:"<",
        // 2:"<=",
        // 3:"=",
        // 4:">="
        public int SubmittedEndComparator { get; set; }
        /*For Req
         * 0 -  Do not include
         * 1 - Search for a No
         * 2 - Search for a Yes
         */
        public int? ReqDocChg { get; set; }
        public int? ReqReqChg { get; set; }
        public int? ReqDBChg { get; set; }        
        public int? Rank { get; set; }
        public int RankComparator { get; set; }
        /* 0 = "="
         * 1 = "<"
         * 2 = "<="
         * 3 = ">"
         * 4 = ">="
         */
        public int? Size { get; set; }
        public int SizeComparator { get; set; }
        /* 0 = "="
         * 1 = "<"
         * 2 = "<="
         * 3 = ">"
         * 4 = ">="
         */

        //Assigned To Tab
        public int? AssignedToID { get; set; }
        public DMYDate AssignedToStartDate { get; set; }
        public DMYDate AssignedToEndDate { get; set; }
        public int AssignedToStartComparator { get; set; }
        public int AssignedToEndComparator { get; set; }
        //Resolved By tab
        public int? ResolvedByID { get; set; }
        public DMYDate ResolvedStartDate { get; set; }
        public DMYDate ResolvedEndDate { get; set; }
        public int ResolvedStartComparator { get; set; }
        // 0:"",
        // 1:"<",
        // 2:"<=",
        // 3:"=",
        // 4:">="
        public int ResolvedEndComparator { get; set; }
        //Closed By Tab
        public int? ClosedByID { get; set; }
        public DMYDate ClosedStartDate { get; set; }
        public DMYDate ClosedEndDate { get; set; }
        public int ClosedStartComparator { get; set; }
        public int ClosedEndComparator { get; set; }
        //Modified 
        public DMYDate ModifiedAfter { get; set; }
        public DMYDate ModifiedBefore { get; set; }
        public int ModifiedAfterComparator { get; set; }
        public int ModifiedBeforeComparator { get; set; }
        //Customer Tab
        public int? CustomerReported { get; set; }
        public String CustomerCompany { get; set; }
        public String CustomerLocation { get; set; }
        public String CustomerContact { get; set; }
        public int CustomerLogic { get; set; }
        public String CustomerSyteLine { get; set; }
        public String CustomerMachineSN { get; set; }
        ////Display, where to place them   
        public String HiddenString { get; set; }
        public Dictionary<String, String> Translator { get; set; }
        public List<String> ListOfNtext { get; set; }
        public List<String> DisplayItems { get; set; }
        public String SQLDebuggingQuery { get; set; }
        public String DataDisplay { get; set; }
        public List<System.Data.DataRow> DataRows { get; set; }
        public DataTable ResultsTable { get; set; }
        //Sort By
        public List<String> SortByItems { get; set; }
        public String HiddenStringSortBy { get; set; }
        //Last Opened Panel 
        public String LastOpenPanel { get; set; }
        public String LastOpenCollapse { get; set; }
        //Where Tab
        public bool CanUseWhere { get; set; }
        public String Where { get; set; }

        //Save Attributes
        public int PublicReport { get; set; }
        public String NameOfReport { get; set; }
        public String MaxRows { get; set; }

        //sql
        public String SelectStatement { get; set; }
        public String SortByStatement { get; set; }
        public String RawSQL { get; set; }
        public String WhereSQLRows { get; set; }
        public String WhereSQLRowsForMassUpdate { get; set; }

        //Owner
        public int Owner { get; set; }
        public int UserID { get; set; }
        //Current Product for the -1
        public int CurrentProductID { get; set; }
        public int CurrentProductName { get; set; }

        //Chart Data
        public String Title { get; set; }
        public List<String> yMetricValues { get; set; }
        public List<String> xValues { get; set; }
        public List<String> yValues { get; set; }
        public int hieght { get; set; }
        public int width { get; set; }

        public int[] TotalCount { get; set; }
        public int[] ProblemCount { get; set; }//Low, med, high, crit
        public int[] EnhancementCount { get; set; }
        public int[] PlannedWorkCount { get; set; }

        public int[] TotalMetric { get; set; }
        public int[] ProblemMetric { get; set; }
        public int[] EnhancementMetric { get; set; }
        public int[] PlannedWorkMetric { get; set; }

        private int LowWeight { get; set; }
        private int MediumWeight { get; set; }
        private int HighWeight { get; set; }
        private int CritcalWeight { get; set; }

        public bool isWhereQueryFail { get; set; }

        public ChartViewModel.PieChart DisplayChart { get; set; }
        //History for Expanded
        public DataSet HistoryTables { get; set; }
        private string fullWhereQuery { get; set; }
        private REATrackerDB.DisplayName displayname;
        public DateTime MadeOnDate { get; set; }

        public SearchViewModel()
        {
        }
        public SearchViewModel(int argsST_UserID, int argsCurrentProduct)
        {
            this.HistorySearchLogic = 2;
            this.TextTypeSearch = 3;
            this.CurrentProductID = argsCurrentProduct;
            this.UserID = argsST_UserID;
            this.init();

        }
        public SearchViewModel(int argsReportID, int argsST_UserID, int argsCurrentProduct)
        {
            this.HistorySearchLogic = 2;
            this.TextTypeSearch = 3;
            this.CurrentProductID = argsCurrentProduct;
            this.UserID = argsST_UserID;
            this.ReportID = argsReportID;
            this.init();
            this.AutoFillGet();
        }
        // Init methods
        public void init()
        {
            REATrackerDB sql = new REATrackerDB();

            this.displayname = REATrackerDB.DisplayName.DefaultDisplay;
            this.CanUseWhere = false;

            string result = "SELECT BLOCK_WHERE, DISPLAY_NAME_PREF FROM ST_USERS WHERE ST_USERS.USER_ID =" + this.UserID;
            using (DataTable dt = sql.ProcessCommand(result))
            {
                try
                {
                    if (dt.Rows.Count > 0)
                    {
                        //'block_where'  [No_access =1, Has_access =0]
                        this.CanUseWhere = !(Convert.ToBoolean(dt.Rows[0]["BLOCK_WHERE"]));
                        this.displayname = (REATrackerDB.DisplayName)Convert.ToInt32(dt.Rows[0]["DISPLAY_NAME_PREF"]);
                    }
                }
                catch (Exception)
                {
                    //if something goes wrong we force the defaults
                    this.displayname = REATrackerDB.DisplayName.DefaultDisplay;
                    this.CanUseWhere = false;
                }
            }

            if (this.DataRows == null)
            {
                this.DataRows = new List<System.Data.DataRow>();
            }
            if (this.SelectedProducts == null)
            {
                this.SelectedProducts = new List<Item>();
            }
            if (this.SelectedIssueTypes == null)
            {
                this.SelectedIssueTypes = new List<Item>();
            }
            if (this.SelectedPriority == null)
            {
                this.SelectedPriority = new List<Item>();
            }
            if (this.SelectedStatus == null)
            {
                this.SelectedStatus = new List<Item>();
            }
            if (this.SelectedKeyword == null)
            {
                this.SelectedKeyword = new List<Item>();
            }
            if (this.DisplayItems == null)
            {
                this.DisplayItems = new List<String>();
            }
            if (this.Translator == null)
            {
                this.Translator = new Dictionary<string, string>();
                //Key is the display name
                //Value is the DB Column Name
                this.Translator.Add("REA Number", "REA_TRACK.TRACKING_ID AS 'REA'");
                this.Translator.Add("Product", "REA_PRODUCT.NAME AS 'Product'");
                this.Translator.Add("Title", "REA_TRACK.TITLE AS 'Title'");
                this.Translator.Add("Priority", "REA_TRACK.priority AS 'Priority'");
                this.Translator.Add("Issue Type", "REA_TRACK.ISSUE_TYPE AS 'Issue'");
                this.Translator.Add("Status", "REA_STATUS.NAME AS 'Status'");
                this.Translator.Add("Assigned To", "(ST_USERS_ASSIGNED_TO.FIRST_NAME +' '+ ST_USERS_ASSIGNED_TO.LAST_NAME) AS 'Assigned To'");
                this.Translator.Add("Resolved By", "(ST_USERS_RESOLVED_BY.FIRST_NAME +' '+ ST_USERS_RESOLVED_BY.LAST_NAME) AS 'Resolved By'");
                this.Translator.Add("Submitted By", "(ST_USERS_SUBMITTED_BY.FIRST_NAME +' '+ ST_USERS_SUBMITTED_BY.LAST_NAME) AS 'Submitted By'");
                this.Translator.Add("Closed By", "(ST_USERS_CLOSED_BY.FIRST_NAME +' '+ ST_USERS_CLOSED_BY.LAST_NAME) AS 'Closed By'");
                this.Translator.Add("Last Modified", "REA_TRACK.LAST_UPDATE AS 'Last Modified'");
                this.Translator.Add("Resolved On", "RESOLVED_ON AS 'Resolved On'");
                this.Translator.Add("Closed On", "CLOSED_ON AS 'Closed On'");
                this.Translator.Add("Submitted On", "REA_TRACK.SUBMITTED_ON AS 'Submitted On'");

                this.Translator.Add("Assigned By", "(ST_USERS_ASSIGNED_BY.FIRST_NAME +' '+ ST_USERS_ASSIGNED_BY.LAST_NAME) AS 'Assigned By'");
                this.Translator.Add("Closed Reason", "ST_CLOSED_REASON.NAME AS 'Closed Reason'");
                this.Translator.Add("Submitted To", "(ST_USERS_SUBMITTED_TO.FIRST_NAME +' '+ ST_USERS_SUBMITTED_TO.LAST_NAME) AS 'Submitted To'");

                this.Translator.Add("Manager's Note", "REA_TRACK.MANAGERS_NOTE AS 'Manager''s Note'");
                this.Translator.Add("Related REAS", "@RELATED@ AS 'Related REAs'"); //RELEATED_REAS_STRING
                this.Translator.Add("Status Date", "H2.MADE_ON AS 'Status Date'");

            }
            if (this.ListOfNtext == null)
            {
                this.ListOfNtext = new List<String>();
                this.ListOfNtext.Add("BENEFITS");
                this.ListOfNtext.Add("PROBLEM");
                this.ListOfNtext.Add("SOLUTION");
                this.ListOfNtext.Add("CUSTOMER_LOCATION");
                this.ListOfNtext.Add("CUSTOMER_COMPANY");
            }
            if (this.SortByItems == null)
            {
                this.SortByItems = new List<String>();
            }
            if (this.VersionClosed == null)
            {
                this.VersionClosed = new Version();
            }
            if (this.VersionFound == null)
            {
                this.VersionFound = new Version();
            }
            if (this.VersionPlanned == null)
            {
                this.VersionPlanned = new Version();
            }
            if (this.VersionResolved == null)
            {
                this.VersionResolved = new Version();
            }
            if (this.SubmittedEndDate == null)
            {
                this.SubmittedEndDate = new DMYDate();
            }
            if (this.SubmittedStartDate == null)
            {
                this.SubmittedStartDate = new DMYDate();
            }
            if (this.AssignedToEndDate == null)
            {
                this.AssignedToEndDate = new DMYDate();
            }
            if (this.AssignedToStartDate == null)
            {
                this.AssignedToStartDate = new DMYDate();
            }
            if (this.ResolvedEndDate == null)
            {
                this.ResolvedEndDate = new DMYDate();
            }
            if (this.ResolvedStartDate == null)
            {
                this.ResolvedStartDate = new DMYDate();
            }
            if (this.ClosedEndDate == null)
            {
                this.ClosedEndDate = new DMYDate();
            }
            if (this.ClosedStartDate == null)
            {
                this.ClosedStartDate = new DMYDate();
            }
            if (this.ModifiedAfter == null)
            {
                this.ModifiedAfter = new DMYDate();
            }
            if (this.ModifiedBefore == null)
            {
                this.ModifiedBefore = new DMYDate();
            }
            if (this.MadeOnDate == null)
            {
                this.MadeOnDate = new DateTime();
            }
            //Product Name and ID
            int i = 0;
            this.Products = new List<Item>();
            //DataTable ProductDataTable = new DataTable();
            DataSet dsProducts = sql.GetProductList2(this.UserID); //returns a dataset with 3 tables in it
            
            foreach(DataTable dt in dsProducts.Tables)
            {
                foreach (System.Data.DataRow product in dt.Rows)
                {
                    Item temp = new Item();
                    temp.ID = Convert.ToInt32(product["PRODUCT_ID"]);
                    temp.Name = Convert.ToString(product["NAME"]);
                    if (dt.TableName.Equals("Favorites"))
                    {
                        temp.Group = "Favorites";
                        temp.Order = 3;
                    }
                    else if (dt.TableName.Equals("Active"))
                    {
                        temp.Order = 2;
                        temp.Group = "Active";
                    }
                    else if (dt.TableName.Equals("Retired"))
                    {
                        temp.Group = "Retired";
                        temp.Order = 1;
                    }
                    else
                    {
                        temp.Group = "";
                        temp.Order = 0;
                    }
                    temp.Selected = false;
                    this.Products.Add(temp);
                    i++;
                }
            }

            if (this.UnSelectedProducts == null)
            {
                this.UnSelectedProducts = this.Products.ToList();
            }
            i = 0;
            //IssueType
            this.IssueTypes = new List<Item>();
            bool addToSelected = this.SelectedIssueTypes.Count == 0;
            foreach (System.Data.DataRow row in sql.GetIssueTypes().Rows)
            {
                this.IssueTypes.Add(new Item());
                this.IssueTypes[i].ID = Convert.ToInt32(row["ISSUE_ID"]);
                this.IssueTypes[i].Name = Convert.ToString(row["NAME"]);
                if (addToSelected)
                {
                    this.SelectedIssueTypes.Add(new Item());
                    this.SelectedIssueTypes[i].ID = Convert.ToInt32(row["ISSUE_ID"]);
                    this.SelectedIssueTypes[i].Name = Convert.ToString(row["NAME"]);
                    this.SelectedIssueTypes[i].Selected = false;
                }
                i++;
            }
            i = 0;
            //Priority
            this.Priorities = new List<Item>();
            addToSelected = this.SelectedPriority.Count == 0;
            foreach (System.Data.DataRow row in sql.GetPriorities().Rows)
            {
                this.Priorities.Add(new Item());
                this.Priorities[i].ID = Convert.ToInt32(row["PRIORITY_ID"]);
                this.Priorities[i].Name = Convert.ToString(row["NAME"]);
                if (addToSelected)
                {
                    this.SelectedPriority.Add(new Item());
                    this.SelectedPriority[i].ID = Convert.ToInt32(row["PRIORITY_ID"]);
                    this.SelectedPriority[i].Name = Convert.ToString(row["NAME"]);
                    this.SelectedPriority[i].Selected = false;
                }
                i++;
            }
            i = 0;
            //Status
            this.Statuses = new List<Item>();
            addToSelected = this.SelectedStatus.Count == 0;
            foreach (System.Data.DataRow row in sql.GetStatuses().Rows)
            {
                this.Statuses.Add(new Item());
                this.Statuses[i].ID = Convert.ToInt32(row["STATUS_ID"]);
                this.Statuses[i].Name = Convert.ToString(row["NAME"]);
                if (addToSelected)
                {
                    this.SelectedStatus.Add(new Item());
                    this.SelectedStatus[i].ID = Convert.ToInt32(row["STATUS_ID"]);
                    this.SelectedStatus[i].Name = Convert.ToString(row["NAME"]);
                    this.SelectedStatus[i].Selected = false;
                }
                i++;
            }
            i = 0;
            //Keywords
            this.Keywords = new List<Item>();
            addToSelected = this.SelectedKeyword.Count == 0;
            foreach (DataRow row in sql.GetKeywords().Rows)
            {
                this.Keywords.Add(new Item());
                this.Keywords[i].ID = Convert.ToInt32(row["ID"]);
                this.Keywords[i].Name = Convert.ToString(row["KEYWORD"]);
                if (addToSelected)
                {
                    this.SelectedKeyword.Add(new Item());
                    this.SelectedKeyword[i].ID = Convert.ToInt32(row["ID"]);
                    this.SelectedKeyword[i].Name = Convert.ToString(row["KEYWORD"]);
                    this.SelectedKeyword[i].Selected = false;
                }
                i++;
            }
            i = 0;
            //All Users' Name and thier ID
            //displaying name order preference for Mass update
            this.Employees = new List<dynamic>();
            //this returns the users in order by first than last name
            //set the parameter to 'true' in function call to support 'all users' in the select list. 
            using (DataTable dt = sql.GetUsers(true))
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    this.Employees.Add(new System.Dynamic.ExpandoObject());
                    this.Employees[i].ID = Convert.ToInt32(row["USER_ID"]);
                    this.Employees[i].AccountType = Convert.ToInt32(row["ACCOUNT_TYPE"]);
                    if (displayname == REATrackerDB.DisplayName.FirstNameLastName)
                    {
                        this.Employees[i].Name = Convert.ToString(row["FIRST_NAME"]) + " " + Convert.ToString(row["LAST_NAME"]);
                    }
                    else
                    {
                        this.Employees[i].Name = Convert.ToString(row["LAST_NAME"]) + ", " + Convert.ToString(row["FIRST_NAME"]);
                    }
                    this.Employees[i].IsDefault = false;
                    i++;
                }
                //reorder the list if we have to
                //lastname firstname order when displayname is 1
                if (displayname == REATrackerDB.DisplayName.LastNameFirstName)
                {
                    this.Employees = this.Employees.OrderBy(x => x.Name).ToList();
                }
            }
        }
        private void AutoFillGet()
        {
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.GetReport(this.ReportID);
            if (dt.Rows.Count > 0)
            {
                this.RawSQL = Convert.ToString(dt.Rows[0]["RAW_SQL"]);
            }
            if (dt.Rows.Count > 0
                && (dt.Rows[0]["RAW_SQL"] == DBNull.Value
                    ||
                    string.IsNullOrEmpty(this.RawSQL)
                    ||
                    string.IsNullOrWhiteSpace(this.RawSQL)
                    )
                )
            {
                if (dt.Rows[0]["OWNER"] != DBNull.Value)
                {
                    this.Owner = Convert.ToInt32(dt.Rows[0]["OWNER"]);
                }
                if (dt.Rows[0]["PUBLIC_REPORT"] != DBNull.Value)
                {
                    this.PublicReport = Convert.ToInt32(dt.Rows[0]["PUBLIC_REPORT"]);
                }
                if (dt.Rows[0]["NAME"] != DBNull.Value)
                {
                    this.NameOfReport = Convert.ToString(dt.Rows[0]["NAME"]);
                }
                if (dt.Rows[0]["REA_COMPARE"] != DBNull.Value)
                {
                    this.REAComparator = Convert.ToInt32(dt.Rows[0]["REA_COMPARE"]);
                }
                if (dt.Rows[0]["REA_NUM1"] != DBNull.Value)
                {
                    this.REANumber = Convert.ToString(dt.Rows[0]["REA_NUM1"]);
                }
                if (dt.Rows[0]["REA_NUM2"] != DBNull.Value)
                {
                    this.REANumber2 = Convert.ToString(dt.Rows[0]["REA_NUM2"]);
                }
                if (dt.Rows[0]["STATUS"] != DBNull.Value)
                {
                    char[] SplitChars = { '\n', ',' };
                    int[] StatusesFromDB = Array.ConvertAll(Convert.ToString(dt.Rows[0]["STATUS"]).Split(SplitChars), x => int.Parse(x));
                    foreach (int dbStatus in StatusesFromDB)
                    {
                        var temp = this.Statuses.Where(x => x.ID == dbStatus);
                        if (temp.Count() > 0)
                        {
                            this.SelectedStatus.Add(temp.First());
                            this.SelectedStatus.Where(X => X.ID == temp.First().ID).First().Selected = true;
                        }
                    }
                }
                if (dt.Rows[0]["PRODUCT"] != DBNull.Value)
                {
                    char[] SplitChars = { '\n', ',', ' ' };
                    string productInput = Convert.ToString(dt.Rows[0]["PRODUCT"]);

                    List<int> tempProductIntList = new List<int>();

                    string[] tempProductList = productInput.Split(SplitChars);
                    for (int i = 0; i < tempProductList.Length; i++)
                    {
                        int tempint = 0;
                        if (Int32.TryParse(tempProductList[i], out tempint))
                        {
                            tempProductIntList.Add(tempint);
                        }
                    }
                    int[] ProductsFromDB = tempProductIntList.ToArray();
                    //int i = 0;
                    foreach (int dbProduct in ProductsFromDB)
                    {
                        //var temp = this.Products.Where(x => x.ID == dbProduct);
                        //if( temp.Count() > 0)
                        // {
                        //this.SelectedProducts.Add( temp.First() );
                        //this.SelectedProducts.Where(X=>X.ID==temp.First().ID).First().Selected = true;
                        if (!String.IsNullOrEmpty(this.HiddenProductString)
                            )
                        {
                            this.HiddenProductString += ",";
                        }
                        else
                        {
                            this.HiddenProductString = "";
                        }
                        if (dbProduct == 0)
                        {
                            this.HiddenProductString = "";
                            break;
                        }
                        else
                        {
                            this.HiddenProductString += Convert.ToString(dbProduct);
                        }
                        // }
                        //i++;
                    }
                }
                if (dt.Rows[0]["DOC_CHANGE"] != DBNull.Value)
                {
                    this.ReqDocChg = Convert.ToInt32(dt.Rows[0]["DOC_CHANGE"]);
                }
                if (dt.Rows[0]["DB_CHANGE"] != DBNull.Value)
                {
                    this.ReqDBChg = Convert.ToInt32(dt.Rows[0]["DB_CHANGE"]);
                }
                if (dt.Rows[0]["REQ_DOC"] != DBNull.Value)
                {
                    this.ReqReqChg = Convert.ToInt32(dt.Rows[0]["REQ_DOC"]);
                }
                if (dt.Rows[0]["ASSIGN_TO"] != DBNull.Value)
                {
                    this.AssignedToID = Convert.ToInt32(dt.Rows[0]["ASSIGN_TO"]);
                }
                if (dt.Rows[0]["RESOLVED_BY"] != DBNull.Value)
                {
                    this.ResolvedByID = Convert.ToInt32(dt.Rows[0]["RESOLVED_BY"]);
                }
                if (dt.Rows[0]["CLOSED_BY"] != DBNull.Value)
                {
                    this.ClosedByID = Convert.ToInt32(dt.Rows[0]["CLOSED_BY"]);
                }
                if (dt.Rows[0]["MAX_ROWS"] != DBNull.Value)
                {
                    this.MaxRows = Convert.ToString(dt.Rows[0]["MAX_ROWS"]);
                }
                if (dt.Rows[0]["ISSUE_TYPE"] != DBNull.Value)
                {
                    char[] SplitChars = { '\n', ',' };
                    int[] issuesFromDB = Array.ConvertAll(Convert.ToString(dt.Rows[0]["ISSUE_TYPE"] != DBNull.Value ? dt.Rows[0]["ISSUE_TYPE"] : "").Split(SplitChars), x => int.Parse(x));
                    int k = 0;
                    foreach (int dbIssue in issuesFromDB)
                    {
                        var temp = this.IssueTypes.Where(x => x.ID == dbIssue);
                        if (temp.Count() > 0)
                        {
                            this.SelectedIssueTypes.Add(temp.First());
                            this.SelectedIssueTypes.Where(X => X.ID == temp.First().ID).First().Selected = true;
                        }
                        k++;
                    }
                }
                if (dt.Rows[0]["PRIORITIES"] != DBNull.Value)
                //dt.Rows[0]["priority"] != DBNull.Value)
                {
                    char[] SplitChars = { '\n', ',' };
                    int[] prioritiesFromDB = Array.ConvertAll(Convert.ToString(dt.Rows[0]["PRIORITIES"]).Split(SplitChars), x => int.Parse(x));
                    int k = 0;
                    foreach (int dbPriority in prioritiesFromDB)
                    {
                        var temp = this.Priorities.Where(x => x.ID == dbPriority);
                        if (temp.Count() > 0)
                        {
                            this.SelectedPriority.Add(temp.First());
                            this.SelectedPriority.Where(X => X.ID == temp.First().ID).First().Selected = true;
                        }
                        k++;
                    }
                }
                if (dt.Rows[0]["CUSTOMER_BUG"] != DBNull.Value)
                {
                    this.CustomerReported = Convert.ToInt32(dt.Rows[0]["CUSTOMER_BUG"]);
                }
                if (dt.Rows[0]["CUSTOMER_LOGIC"] != DBNull.Value)
                {
                    this.CustomerLogic = Convert.ToInt32(dt.Rows[0]["CUSTOMER_LOGIC"]);
                }
                if (dt.Rows[0]["CUSTOMER_NAME"] != DBNull.Value)
                {
                    this.CustomerContact = Convert.ToString(dt.Rows[0]["CUSTOMER_NAME"]);
                }
                if (dt.Rows[0]["CUSTOMER_COMPANY"] != DBNull.Value)
                {
                    this.CustomerCompany = Convert.ToString(dt.Rows[0]["CUSTOMER_COMPANY"]);
                }
                if (dt.Rows[0]["CUSTOMER_LOCATION"] != DBNull.Value)
                {
                    this.CustomerLocation = Convert.ToString(dt.Rows[0]["CUSTOMER_LOCATION"]);
                }
                if (dt.Rows[0]["TITLE_LOGIC"] != DBNull.Value)
                {
                    this.TextTypeSearch = Convert.ToInt32(dt.Rows[0]["TITLE_LOGIC"]);
                }
                if (dt.Rows[0]["SEARCH_TITLE"] != DBNull.Value)
                {
                    this.TitleText = Convert.ToString(dt.Rows[0]["SEARCH_TITLE"]);
                }
                if (dt.Rows[0]["WHERE_CLAUSE"] != DBNull.Value)
                {
                    this.Where = Convert.ToString(dt.Rows[0]["WHERE_CLAUSE"]);
                }
                //People
                if (dt.Rows[0]["SUB_BY"] != DBNull.Value)
                {
                    this.SubmittedByID = Convert.ToInt32(dt.Rows[0]["SUB_BY"]);
                }
                if (dt.Rows[0]["ASSIGN_TO"] != DBNull.Value)
                {
                    this.AssignedToID = Convert.ToInt32(dt.Rows[0]["ASSIGN_TO"]);
                }
                if (dt.Rows[0]["RESOLVED_BY"] != DBNull.Value)
                {
                    this.ResolvedByID = Convert.ToInt32(dt.Rows[0]["RESOLVED_BY"]);
                }
                if (dt.Rows[0]["CLOSED_BY"] != DBNull.Value)
                {
                    this.ClosedByID = Convert.ToInt32(dt.Rows[0]["CLOSED_BY"]);
                }
                //Get Sort By Statement
                if (dt.Rows[0]["SORT_BY_STATEMENT"] != DBNull.Value)
                {
                    this.HiddenStringSortBy = Convert.ToString(dt.Rows[0]["SORT_BY_STATEMENT"]);
                }
                if (dt.Rows[0]["SELECT_STATEMENT"] != DBNull.Value)
                {
                    this.HiddenString = Convert.ToString(dt.Rows[0]["SELECT_STATEMENT"]).Replace("\r", "");
                }
                if (dt.Rows[0]["SYTELINE_REF"] != DBNull.Value)
                {
                    this.CustomerSyteLine = Convert.ToString(dt.Rows[0]["SYTELINE_REF"]);
                }
                if (dt.Rows[0]["MACHINE_SN"] != DBNull.Value)
                {
                    this.CustomerMachineSN = Convert.ToString(dt.Rows[0]["MACHINE_SN"]);
                }
                if (dt.Rows[0]["HISTORY_SEARCH"] != DBNull.Value)
                {
                    this.HistoryTerm = Convert.ToString(dt.Rows[0]["HISTORY_SEARCH"]);
                }
                if (dt.Rows[0]["HISTORY_SEARCH_LOGIC"] != DBNull.Value)
                {
                    this.HistorySearchLogic = Convert.ToInt32(dt.Rows[0]["HISTORY_SEARCH_LOGIC"]);
                }

                if (dt.Rows[0]["RANK_ORDER"] != DBNull.Value)
                {
                    this.Rank = Convert.ToInt32(dt.Rows[0]["RANK_ORDER"]);
                }
                if (dt.Rows[0]["RANK_COMPARE"] != DBNull.Value)
                {
                    this.RankComparator = Convert.ToInt32(dt.Rows[0]["RANK_COMPARE"]);
                }
                if (dt.Rows[0]["SIZE"] != DBNull.Value)
                {
                    this.Size = Convert.ToInt32(dt.Rows[0]["SIZE"]);
                }
                if (dt.Rows[0]["SIZE_COMPARE"] != DBNull.Value)
                {
                    this.SizeComparator = Convert.ToInt32(dt.Rows[0]["SIZE_COMPARE"]);
                }
                if (dt.Rows[0]["KEYWORDS"] != DBNull.Value)
                {
                    char[] SplitChars = { '\n', ',' };
                    int[] KeywordsFromDB = Array.ConvertAll(Convert.ToString(dt.Rows[0]["KEYWORDS"]).Split(SplitChars), x => int.Parse(x));
                    foreach (int dbKeywords in KeywordsFromDB)
                    {
                        var temp = this.Keywords.Where(x => x.ID == dbKeywords);
                        if (temp.Count() > 0)
                        {
                            this.SelectedKeyword.Add(temp.First());
                            this.SelectedKeyword.Where(X => X.ID == temp.First().ID).First().Selected = true;
                        }
                    }
                }
                //Get Versions
                this.GetVersions(this.ReportID);
                //Get Dates
                this.GetDates(this.ReportID);
            }// Is a raw sql report
            else if (!string.IsNullOrEmpty(Convert.ToString(this.RawSQL)))
            {
                this.RawSQL = Convert.ToString(dt.Rows[0]["RAW_SQL"]);
                this.NameOfReport = Convert.ToString(dt.Rows[0]["NAME"]);
                this.Owner = Convert.ToInt32(dt.Rows[0]["OWNER"]);
                this.PublicReport = Convert.ToInt32(dt.Rows[0]["PUBLIC_REPORT"]);

            }
        }
        private void GetVersions(int REPORT_ID)
        {
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.GetReportVersions(REPORT_ID);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                if (Convert.ToString(row["FIELD"]) == "VERSION_FOUND")
                {
                    this.VersionFound = new Version();
                    if (row["MAJOR"] != DBNull.Value)
                    {
                        this.VersionFound.Major = Convert.ToInt32(row["MAJOR"]);
                        this.VersionFound.MajorCompare = Convert.ToString(row["MAJOR_OPERATOR"]);
                    }
                    if (row["MINOR"] != DBNull.Value)
                    {
                        this.VersionFound.Minor = Convert.ToInt32(row["MINOR"]);
                        this.VersionFound.MinorCompare = Convert.ToString(row["MINOR_OPERATOR"]);
                    }
                    if (row["BUILD"] != DBNull.Value)
                    {
                        this.VersionFound.Build = Convert.ToInt32(row["BUILD"]);
                        this.VersionFound.BuildCompare = Convert.ToString(row["BUILD_OPERATOR"]);
                    }
                    if (row["MISC"] != DBNull.Value)
                    {
                        this.VersionFound.Misc = Convert.ToString(row["MISC"]);
                    }
                }
                else if (Convert.ToString(row["FIELD"]) == "VERSION_RESOLVED")
                {
                    this.VersionResolved = new Version();
                    if (row["MAJOR"] != DBNull.Value)
                    {
                        this.VersionResolved.Major = Convert.ToInt32(row["MAJOR"]);
                        this.VersionResolved.MajorCompare = Convert.ToString(row["MAJOR_OPERATOR"]);
                    }
                    if (row["MINOR"] != DBNull.Value)
                    {
                        this.VersionResolved.Minor = Convert.ToInt32(row["MINOR"]);
                        this.VersionResolved.MinorCompare = Convert.ToString(row["MINOR_OPERATOR"]);
                    }
                    if (row["BUILD"] != DBNull.Value)
                    {
                        this.VersionResolved.Build = Convert.ToInt32(row["BUILD"]);
                        this.VersionResolved.BuildCompare = Convert.ToString(row["BUILD_OPERATOR"]);
                    }
                    if (row["MISC"] != DBNull.Value)
                    {
                        this.VersionResolved.Misc = Convert.ToString(row["MISC"]);
                    }
                }
                else if (Convert.ToString(row["FIELD"]) == "VERSION_CLOSED")
                {
                    this.VersionPlanned = new Version();
                    if (row["MAJOR"] != DBNull.Value)
                    {
                        this.VersionClosed.Major = Convert.ToInt32(row["MAJOR"]);
                        this.VersionClosed.MajorCompare = Convert.ToString(row["MAJOR_OPERATOR"]);
                    }
                    if (row["MINOR"] != DBNull.Value)
                    {
                        this.VersionClosed.Minor = Convert.ToInt32(row["MINOR"]);
                        this.VersionClosed.MinorCompare = Convert.ToString(row["MINOR_OPERATOR"]);
                    }
                    if (row["BUILD"] != DBNull.Value)
                    {
                        this.VersionClosed.Build = Convert.ToInt32(row["BUILD"]);
                        this.VersionClosed.BuildCompare = Convert.ToString(row["BUILD_OPERATOR"]);
                    }
                    if (row["MISC"] != DBNull.Value)
                    {
                        this.VersionClosed.Misc = Convert.ToString(row["MISC"]);
                    }
                }
                else if (Convert.ToString(row["FIELD"]) == "VERSION_PLANNED")
                {
                    this.VersionPlanned = new Version();
                    if (row["MAJOR"] != DBNull.Value)
                    {
                        this.VersionPlanned.Major = Convert.ToInt32(row["MAJOR"]);
                        this.VersionPlanned.MajorCompare = Convert.ToString(row["MAJOR_OPERATOR"]);
                    }
                    if (row["MINOR"] != DBNull.Value)
                    {
                        this.VersionPlanned.Minor = Convert.ToInt32(row["MINOR"]);
                        this.VersionPlanned.MinorCompare = Convert.ToString(row["MINOR_OPERATOR"]);
                    }
                    if (row["BUILD"] != DBNull.Value)
                    {
                        this.VersionPlanned.Build = Convert.ToInt32(row["BUILD"]);
                        this.VersionPlanned.BuildCompare = Convert.ToString(row["BUILD_OPERATOR"]);
                    }
                    if (row["MISC"] != DBNull.Value)
                    {
                        this.VersionPlanned.Misc = Convert.ToString(row["MISC"]);
                    }
                }
            }

        }
        private void GetDates(int REPORT_ID)
        {
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.GetReportDates(REPORT_ID);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                if (Convert.ToString(row["FIELD"]) == "SUB_START")
                {
                    if (row["OPERATOR"] != DBNull.Value)
                    {
                        this.SubmittedStartComparator = ComparatorToValue(Convert.ToString(row["OPERATOR"]));
                        if (row["MONTH"] != DBNull.Value)
                        {
                            this.SubmittedStartDate.Month = Convert.ToInt32(row["MONTH"]);
                        }
                        if (row["DAY"] != DBNull.Value)
                        {
                            this.SubmittedStartDate.Day = Convert.ToInt32(row["DAY"]);
                        }
                        if (row["YEAR"] != DBNull.Value)
                        {
                            this.SubmittedStartDate.Year = Convert.ToInt32(row["YEAR"]);
                        }
                    }
                }
                else if (Convert.ToString(row["FIELD"]) == "SUB_END")
                {
                    if (row["OPERATOR"] != DBNull.Value)
                    {
                        this.SubmittedEndComparator = ComparatorToValue(Convert.ToString(row["OPERATOR"]));
                        if (row["MONTH"] != DBNull.Value)
                        {
                            this.SubmittedEndDate.Month = Convert.ToInt32(row["MONTH"]);
                        }
                        if (row["DAY"] != DBNull.Value)
                        {
                            this.SubmittedEndDate.Day = Convert.ToInt32(row["DAY"]);
                        }
                        if (row["YEAR"] != DBNull.Value)
                        {
                            this.SubmittedEndDate.Year = Convert.ToInt32(row["YEAR"]);
                        }
                    }
                }
                else if (Convert.ToString(row["FIELD"]) == "ASN_START")
                {
                    if (row["OPERATOR"] != DBNull.Value)
                    {
                        this.AssignedToStartComparator = ComparatorToValue(Convert.ToString(row["OPERATOR"]));
                        if (row["MONTH"] != DBNull.Value)
                        {
                            this.AssignedToStartDate.Month = Convert.ToInt32(row["MONTH"]);
                        }
                        if (row["DAY"] != DBNull.Value)
                        {
                            this.AssignedToStartDate.Day = Convert.ToInt32(row["DAY"]);
                        }
                        if (row["YEAR"] != DBNull.Value)
                        {
                            this.AssignedToStartDate.Year = Convert.ToInt32(row["YEAR"]);
                        }
                    }
                }
                else if (Convert.ToString(row["FIELD"]) == "ASN_END")
                {
                    if (row["OPERATOR"] != DBNull.Value)
                    {
                        this.AssignedToEndComparator = ComparatorToValue(Convert.ToString(row["OPERATOR"]));
                        if (row["MONTH"] != DBNull.Value)
                        {
                            this.AssignedToEndDate.Month = Convert.ToInt32(row["MONTH"]);
                        }
                        if (row["DAY"] != DBNull.Value)
                        {
                            this.AssignedToEndDate.Day = Convert.ToInt32(row["DAY"]);
                        }
                        if (row["YEAR"] != DBNull.Value)
                        {
                            this.AssignedToEndDate.Year = Convert.ToInt32(row["YEAR"]);
                        }
                    }
                }
                else if (Convert.ToString(row["FIELD"]) == "RES_START")
                {
                    if (row["OPERATOR"] != DBNull.Value)
                    {
                        this.ResolvedStartComparator = ComparatorToValue(Convert.ToString(row["OPERATOR"]));
                        if (row["MONTH"] != DBNull.Value)
                        {
                            this.ResolvedStartDate.Month = Convert.ToInt32(row["MONTH"]);
                        }
                        if (row["DAY"] != DBNull.Value)
                        {
                            this.ResolvedStartDate.Day = Convert.ToInt32(row["DAY"]);
                        }
                        if (row["YEAR"] != DBNull.Value)
                        {
                            this.ResolvedStartDate.Year = Convert.ToInt32(row["YEAR"]);
                        }
                    }
                }
                else if (Convert.ToString(row["FIELD"]) == "RES_END")
                {
                    if (row["OPERATOR"] != DBNull.Value)
                    {
                        this.ResolvedEndComparator = ComparatorToValue(Convert.ToString(row["OPERATOR"]));
                        if (row["MONTH"] != DBNull.Value)
                        {
                            this.ResolvedEndDate.Month = Convert.ToInt32(row["MONTH"]);
                        }
                        if (row["DAY"] != DBNull.Value)
                        {
                            this.ResolvedEndDate.Day = Convert.ToInt32(row["DAY"]);
                        }
                        if (row["YEAR"] != DBNull.Value)
                        {
                            this.ResolvedEndDate.Year = Convert.ToInt32(row["YEAR"]);
                        }
                    }
                }
                else if (Convert.ToString(row["FIELD"]) == "CLO_START")
                {
                    if (row["OPERATOR"] != DBNull.Value)
                    {
                        this.ClosedStartComparator = ComparatorToValue(Convert.ToString(row["OPERATOR"]));
                        if (row["MONTH"] != DBNull.Value)
                        {
                            this.ClosedStartDate.Month = Convert.ToInt32(row["MONTH"]);
                        }
                        if (row["DAY"] != DBNull.Value)
                        {
                            this.ClosedStartDate.Day = Convert.ToInt32(row["DAY"]);
                        }
                        if (row["YEAR"] != DBNull.Value)
                        {
                            this.ClosedStartDate.Year = Convert.ToInt32(row["YEAR"]);
                        }
                    }
                }
                else if (Convert.ToString(row["FIELD"]) == "CLO_END")
                {
                    if (row["OPERATOR"] != DBNull.Value)
                    {
                        this.ClosedEndComparator = ComparatorToValue(Convert.ToString(row["OPERATOR"]));
                        if (row["MONTH"] != DBNull.Value)
                        {
                            this.ClosedEndDate.Month = Convert.ToInt32(row["MONTH"]);
                        }
                        if (row["DAY"] != DBNull.Value)
                        {
                            this.ClosedEndDate.Day = Convert.ToInt32(row["DAY"]);
                        }
                        if (row["YEAR"] != DBNull.Value)
                        {
                            this.ClosedEndDate.Year = Convert.ToInt32(row["YEAR"]);
                        }
                    }
                }
                else if (Convert.ToString(row["FIELD"]) == "MODIFIED_START")
                {
                    if (row["OPERATOR"] != DBNull.Value)
                    {
                        this.ModifiedAfterComparator = ComparatorToValue(Convert.ToString(row["OPERATOR"]));
                        if (row["MONTH"] != DBNull.Value)
                        {
                            this.ModifiedAfter.Month = Convert.ToInt32(row["MONTH"]);
                        }
                        if (row["DAY"] != DBNull.Value)
                        {
                            this.ModifiedAfter.Day = Convert.ToInt32(row["DAY"]);
                        }
                        if (row["YEAR"] != DBNull.Value)
                        {
                            this.ModifiedAfter.Year = Convert.ToInt32(row["YEAR"]);
                        }
                    }
                }
                else if (Convert.ToString(row["FIELD"]) == "MODIFIED_END")
                {
                    if (row["OPERATOR"] != DBNull.Value)
                    {
                        this.ModifiedBeforeComparator = ComparatorToValue(Convert.ToString(row["OPERATOR"]));
                        if (row["MONTH"] != DBNull.Value)
                        {
                            this.ModifiedBefore.Month = Convert.ToInt32(row["MONTH"]);
                        }
                        if (row["DAY"] != DBNull.Value)
                        {
                            this.ModifiedBefore.Day = Convert.ToInt32(row["DAY"]);
                        }
                        if (row["YEAR"] != DBNull.Value)
                        {
                            this.ModifiedBefore.Year = Convert.ToInt32(row["YEAR"]);
                        }
                    }
                }
            }

        }
        // Post Method
        public void ProcessSearchForMassUpdate()
        {
            //Compile the SQL String
            String Command;
            if (String.IsNullOrEmpty(this.RawSQL))
            {
                Command = this.CreateCommand();
            }
            else if (this.RawSQL != null
                && !this.RawSQL.ToLower().Contains("delete")
                && !this.RawSQL.ToLower().Contains("update")
                && !this.RawSQL.ToLower().Contains("insert"))
            {
                Command = this.RawSQL;
            }
            else
            {
                Command = "";
            }
        }
        public int ProcessSearch()
        {
            int rowCount = 0;
            //Compile the SQL String
            String Command;
            if (String.IsNullOrEmpty(this.RawSQL))
            {
                Command = this.CreateCommand();
            }
            else if (this.RawSQL != null
                && !this.RawSQL.ToLower().Contains("delete")
                && !this.RawSQL.ToLower().Contains("insert"))
            {//Stirng cannot be empty
                if (!(this.RawSQL.ToLower().Contains("update") &&
                    !this.RawSQL.Substring(0, this.RawSQL.ToLower().IndexOf("update")).ToLower().Contains("last")
                    )
                    )
                {
                    Command = this.RawSQL;
                }
                else
                {
                    Command = "";
                }
            }
            else
            {
                Command = "";
            }
            //Get the Data Table
            REATrackerDB sql = new REATrackerDB();
            if (Command.Length != 0)
            {
                sql.OpenConnection();
                bool try_again = false;
                DataTable dt = null;
                //the exception is not thrown from ProcessCommand so we should check the LastError

                try
                {
                    if (String.IsNullOrEmpty(this.RawSQL))
                    {
                        Command = Command.Replace("@RELATED@", RELATED_REAS_STRING);
                        /*
                        if (Command.Contains("ORDER BY"))
                        {
                            string[] temp = Command.Split(new string[] { "ORDER BY" }, StringSplitOptions.RemoveEmptyEntries);
                            //these is the select statement
                            temp[0] = temp[0].Replace("@VF@", VERSION_FOUND_STRING);
                            temp[0] = temp[0].Replace("@VP@", VERSION_PLANNED_STRING);
                            temp[0] = temp[0].Replace("@VR@", VERSION_RESOLVED_STRING);
                            temp[0] = temp[0].Replace("@VC@", VERSION_CLOSED_STRING);

                            //these is the order by statement
                            temp[1] = temp[1].Replace("@VF@", VERSION_FOUND_STRING);
                            temp[1] = temp[1].Replace("@VP@", VERSION_PLANNED_STRING);
                            temp[1] = temp[1].Replace("@VR@", VERSION_RESOLVED_STRING);
                            temp[1] = temp[1].Replace("@VC@", VERSION_CLOSED_STRING);

                            Command = temp[0] + " ORDER BY " + temp[1];
                        }
                        else
                        {
                            Command.Replace("@VF@", VERSION_FOUND_STRING);
                            Command.Replace("@VP@", VERSION_PLANNED_STRING);
                            Command.Replace("@VR@", VERSION_RESOLVED_STRING);
                            Command.Replace("@VC@", VERSION_CLOSED_STRING);

                        }*/
                    }
                    dt = sql.ProcessCommand(Command);
                    if (sql.LastError.Length > 0)
                    {
                        try_again = true;
                    }
                }

                catch (Exception ex)
                {
                    if (ex.Message.Contains("SELECT DISTINCT"))
                    {
                        try_again = true;
                    }
                    if (Where != null && Where.Trim().Length != 0)
                    {
                        this.isWhereQueryFail = true;
                    }
                }

                if (try_again)
                {
                    //remove DISTINCT --DISTINCT (REMOVED DUE TO SORTING COLUMNS NOT MATCHING)
                    Command = Command.Replace("DISTINCT", "");
                    dt = sql.ProcessCommand(Command);
                }
                ////Compile the IGgrid
                this.ResultsTable = dt;
                if (dt != null && !isWhereQueryFail)
                {
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        this.DataRows.Add(row);
                    }
                    rowCount = dt.Rows.Count;
                }
            }
            //Display the select tags and sort by
            //Format the command
            this.SQLDebuggingQuery = this.FormatCommand(Command);
            //Parse the string to display the selected display columns
            if (String.IsNullOrEmpty(this.HiddenString))
            {
                this.HiddenString = "";
            }            
            String[] parsed = this.HiddenString.Replace("\r", "").Split(new[] { "'," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (String columnName in parsed)
            {
                string temp = columnName.Trim();
                if (temp[temp.Length - 1] != '\'')
                {
                    temp = columnName + "'";
                }
                this.DisplayItems.Add(temp);
            }
            //Parse the string to display the sort by list
            if (String.IsNullOrEmpty(this.HiddenStringSortBy))
            {
                this.HiddenStringSortBy = "";
            }

            //used for display the sort fields in the control
            string tmpHiddenStringSortBy = HiddenStringSortBy.ToString();
            string[] separators = new string[] { "," };

            String[] parsedSortedBy = tmpHiddenStringSortBy.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (String columnName in parsedSortedBy)
            {
                string colName = columnName.Trim();
                this.SortByItems.Add(colName);
            }
            //Parse The String to Display the Products List
            if (String.IsNullOrEmpty(this.HiddenProductString) || String.IsNullOrWhiteSpace(this.HiddenProductString))
            {
                this.HiddenProductString = "";
            }
            String[] parsedProduct = this.HiddenProductString.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            parsedProduct = parsedProduct.Distinct().ToArray();
            foreach (String ProductID in parsedProduct)
            {
                Item newItem = new Item();
                newItem.ID = Convert.ToInt32(ProductID);

                if (ProductID == "-1")
                {
                    newItem.Name = "Current Product (" +
                        Products.First(x => x.ID == this.CurrentProductID
                            ).Name + ")";
                    this.SelectedProducts.RemoveAll(x => x.Name == newItem.Name);
                }
                else if (Products.Where(x => x.ID == newItem.ID).Count() != 0)
                {
                    newItem.Name = Products.First(x => x.ID == newItem.ID).Name;
                }
                this.SelectedProducts.Add(newItem);
            }
            return rowCount;
        }
        private String FormatCommand(String command)
        {
            String output = command.Trim();
            //output = output.ToUpper(); Can't do this because of case sensitive strings
            output = output.Replace("select ", "SELECT<br/>&#09;");
            output = output.Replace("SELECT ", "SELECT<br/>&#09;");
            if (output.Contains("DISTINCT"))
            {
                output = output.Replace("distinct", "DISTINCT<br/>&#09;");
                output = output.Replace("DISTINCT", "DISTINCT<br/>&#09;");
            }
            output = output.Replace("from ", "<br/>FROM<br/>&#09;");
            output = output.Replace("FROM ", "<br/>FROM<br/>&#09;");
            output = output.Replace("where ", "<br/>WHERE<br/>&#09;");
            output = output.Replace("WHERE ", "<br/>WHERE<br/>&#09;");
            output = output.Replace("order by ", "<br/>ORDER BY<br/>&#09;");
            output = output.Replace("ORDER BY ", "<br/>ORDER BY<br/>&#09;");
            output = output.Replace("left outer", "<br/>&#09;LEFT OUTER");
            output = output.Replace("LEFT OUTER", "<br/>&#09;LEFT OUTER");
            output = output.Replace("inner join", "<br/>&#09;INNER JOIN");
            output = output.Replace("INNER JOIN", "<br/>&#09;INNER JOIN");
            output = output.Replace("', ", "',<br/>&#09;");
            output = output.Replace("  ", " ");
            output = output.Replace(" AND", " <br/>&#09;AND ");
            output = output.Replace(" and", " <br/>&#09;AND ");
            output = output.Replace("SELECT<br/>&#09;MAX(HISTORY_ID) <br/>FROM<br/>&#09;REA_HISTORY <br/>WHERE<br/>&#09;TRACKING_ID = REA_TRACK.TRACKING_ID <br/>&#09;AND  TYPE = 22) ", "SELECT MAX(HISTORY_ID) FROM REA_HISTORY WHERE TRACKING_ID = ST_TRACK.TRACKING_ID AND TYPE = 22) "); //SEK SCR84313 needed to fix displayed SQL for new history field
            return output;
        }
        private List<int> PageSize(int RowCount)
        {
            List<int> PageSize = new List<int>();
            if (RowCount > 0)
            {
                PageSize.Add(1);
                if (RowCount >= 10)
                {
                    PageSize.Add(5);
                    PageSize.Add(10);
                    //Adds 10 untill it reaches 100
                    for (int i = 0; i < RowCount && i <= 100; i += 10)
                    {
                        if (i < RowCount)
                        {
                            PageSize.Add(i);
                        }
                        else
                        {
                            PageSize.Add(RowCount);
                        }
                    }
                    //Adds 100 untill it reaches 1000
                    for (int i = 1; i < RowCount; i += 100)
                    {

                    }
                }
            }
            return PageSize;
        }
        private String CreateCommand()
        {
            String Query = "SELECT DISTINCT ";
            int maxrowcount;
            if (Int32.TryParse(this.MaxRows, out maxrowcount))
            {
                MaxRows = Convert.ToString(maxrowcount);
                Query += (" TOP " + MaxRows + " ");
            }
            String DisplayQuery = "";
            if (this.HiddenString != null)
            {
                DisplayQuery = this.HiddenString.Replace('\n', ',').Replace("\r", "");
            }
            String FromQuery = " FROM REA_TRACK ";
            String WhereQuery = "";
            String SortByQuery = this.HiddenStringSortBy;
            if (this.Where != null)
            {
                this.Where = this.Where.Trim();
                if (this.Where.ToLower().StartsWith("where"))
                {
                    this.Where = this.Where.Substring(5, this.Where.Length - 5);
                }
            }

            WhereQuery += this.Where;

            if (String.IsNullOrEmpty(DisplayQuery) && this.ReportID != 0)
            {// check to see if it is using legacy data
                string oldDisplayColumncommand = "SELECT DISPLAY1, DISPLAY2, DISPLAY3,DISPLAY4,DISPLAY5,DISPLAY6,DISPLAY7,DISPLAY8 FROM ST_TABULAR_REPORT WHERE ST_TABULAR_REPORT.REPORT_ID =" + this.ReportID;
                REATrackerDB sql = new REATrackerDB();
                DataTable oldDisplayColumns = sql.ProcessCommand(oldDisplayColumncommand);
                if (oldDisplayColumns.Rows.Count != 0)
                {
                    int numOfLegacyColumns = 8;
                    DisplayItems.Add("REA_TRACK.TRACKING_ID AS 'REA'");
                    DisplayQuery += "REA_TRACK.TRACKING_ID AS 'REA'";
                    DataRow dr = oldDisplayColumns.Rows[0];
                    for (int i = 1; i < numOfLegacyColumns + 1; i++)
                    {
                        if ((dr["DISPLAY" + i] != DBNull.Value) && !(String.IsNullOrEmpty(Convert.ToString(dr["DISPLAY" + i]))) && this.Translator != null)
                        {
                            var countTemp = this.Translator.Values.Where(
                                x => x.ToLower().IndexOf(
                                    Convert.ToString(dr["DISPLAY" + i]).Replace("PLANNED_RELEASE", "Planned Release").Replace("VERSION_", "Version ").ToLower()
                                    )
                                    != -1
                                    );
                            if (countTemp.Count() > 0)
                            {
                                if (!string.IsNullOrEmpty(DisplayQuery))
                                {
                                    DisplayQuery += ",";
                                }
                                DisplayQuery += countTemp.First();
                                DisplayItems.Add(countTemp.First());
                            }
                        }
                    }

                }
            }
            if (String.IsNullOrEmpty(DisplayQuery))
            {// If it is still empty after everything, the user must have not selected things to display
                DisplayQuery = "REA_TRACK.TRACKING_ID AS 'REA', REA_PRODUCT.NAME AS 'Product', REA_TRACK.TITLE AS 'Title', REA_TRACK.priority AS 'Priority', REA_STATUS.NAME AS 'Status', " +
                    "(ST_USERS_ASSIGNED_TO.FIRST_NAME +' '+ ST_USERS_ASSIGNED_TO.LAST_NAME) AS 'Assigned To' ";
                DisplayItems.Add("REA_TRACK.TRACKING_ID AS 'REA'");
                DisplayItems.Add("REA_PRODUCT.NAME AS 'Product'");
                DisplayItems.Add("REA_TRACK.TITLE AS 'Title'");
                DisplayItems.Add("REA_TRACK.priority AS 'Priority'");
                DisplayItems.Add("REA_STATUS.NAME AS 'Status'");
                DisplayItems.Add("(ST_USERS_ASSIGNED_TO.FIRST_NAME +' '+ ST_USERS_ASSIGNED_TO.LAST_NAME) AS 'Assigned To'");
                //Fill With Columns that Dave tells you to fill with
            }
            if (String.IsNullOrEmpty(SortByQuery))
            {
                SortByQuery = "";
            }
            //Title
            //if(this.DisplayTitleText)
            //{
            //    DisplayQuery.Insert(this.DisplayColumnTitleText, "TITLE");
            //}
            int SCRNumberTemp;
            if ((this.REANumber != null) && (Int32.TryParse(this.REANumber, out SCRNumberTemp)) && this.REAComparator != 0)
            {
                /* 0 = " " - Nothing
                 * 1 = is exactly
                 * 2 = is at least
                 * 3 = is at most
                 * 4 = is between
                 */
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += "( REA_TRACK.TRACKING_ID";
                switch (this.REAComparator)
                {
                    case (1):
                        {
                            WhereQuery += (" = " + this.REANumber);
                            break;
                        }
                    case (2):
                        {
                            WhereQuery += (" >= " + this.REANumber);
                            break;
                        }
                    case (3):
                        {
                            WhereQuery += (" <= " + this.REANumber);
                            break;
                        }
                    case (4):
                        {
                            WhereQuery += (" >= " + this.REANumber + ") AND (REA_TRACK.TRACKING_ID <= " + this.REANumber2);
                            break;
                        }
                }
                WhereQuery += ") ";
            }

            if (!String.IsNullOrEmpty(this.TitleText))
            {
                //this.TitleText = this.TitleText.Replace("\"","\"\"" );
                this.TitleText = this.TitleText;
                String[] ArrayOfText = this.TitleText.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                //Make Command Parameter
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND";
                }
                if (this.TextTypeSearch == 1)//Any words
                {
                    WhereQuery += "(";
                    for (int i = 0; i < ArrayOfText.Count(); i++)
                    {
                        if (i == 0)
                        {
                            WhereQuery += " ( rea_track.TITLE LIKE '" + ArrayOfText[i] + "%') ";
                        }
                        else
                        {
                            WhereQuery += " OR ( rea_track.TITLE LIKE '" + ArrayOfText[i] + "%') ";
                        }
                    }
                    WhereQuery += ")";
                }
                else if (this.TextTypeSearch == 2)//All words
                {
                    WhereQuery += "(";
                    for (int i = 0; i < ArrayOfText.Count(); i++)
                    {
                        if (i == 0)
                        {
                            WhereQuery += " ( rea_track.TITLE LIKE '%" + ArrayOfText[i] + "%') ";
                        }
                        else
                        {
                            WhereQuery += " AND ( rea_track.TITLE LIKE '%" + ArrayOfText[i] + "%') ";
                        }
                    }
                    WhereQuery += ")";
                }
                else if (this.TextTypeSearch == 3)//Any of
                {
                    WhereQuery += "(";
                    for (int i = 0; i < ArrayOfText.Count(); i++)
                    {
                        if (i == 0)
                        {
                            WhereQuery += " ( rea_track.TITLE LIKE '%" + ArrayOfText[i] + "%') ";
                        }
                        else
                        {
                            WhereQuery += " OR ( rea_track.TITLE LIKE '%" + ArrayOfText[i] + "%') ";
                        }
                    }
                    WhereQuery += ")";
                }
            }
            //Products

            if (!String.IsNullOrEmpty(this.HiddenProductString))
            {
                String[] parsedProduct = this.HiddenProductString.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (parsedProduct.Count() > 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "REA_TRACK.PRODUCT IN (";
                    WhereQuery += this.HiddenProductString.Replace(
                            "-1",
                            Convert.ToString(this.CurrentProductID)
                            );
                    WhereQuery += ")";
                }
            }

            //History Title            
            if (!String.IsNullOrEmpty(this.HistoryTerm) && this.HistoryTerm.Length > 0)
            {
                this.HistoryTerm.Trim();

                String[] words = this.HistoryTerm.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                switch (this.HistorySearchLogic)
                {
                    case 0:
                        {//any words
                            WhereQuery += "(";
                            for (int i = 0; i < words.Count(); i++)
                            {
                                if (i > 0)
                                {
                                    WhereQuery += " OR ";
                                }
                                WhereQuery += ("(REA_HISTORY.DESCRIPTION LIKE '" + words[i].Replace("'", "''") + "%')");
                            }
                            WhereQuery += ")";
                            break;
                        }
                    case 1:
                        {//all words
                            WhereQuery += "(";
                            for (int i = 0; i < words.Count(); i++)
                            {
                                if (i > 0)
                                {
                                    WhereQuery += " AND ";
                                }
                                WhereQuery += ("(REA_HISTORY.DESCRIPTION LIKE '%" + words[i].Replace("'", "''") + "%')");
                            }
                            WhereQuery += ")";
                            break;
                        }
                    case 2:
                        {//any of
                            WhereQuery += "(";
                            for (int i = 0; i < words.Count(); i++)
                            {
                                if (i > 0)
                                {
                                    WhereQuery += " OR ";
                                }
                                WhereQuery += ("(REA_HISTORY.DESCRIPTION LIKE '%" + words[i].Replace("'", "''") + "%')");
                            }
                            WhereQuery += ")";
                            break;
                        }
                }
            }
            //Priority
            List<int> PriorityIDs = new List<int>();
            for (int i = 0; i < this.SelectedPriority.Count; i++)
            {
                if (this.SelectedPriority[i].Selected)
                {
                    PriorityIDs.Add(this.SelectedPriority[i].ID);
                }
            }
            if (PriorityIDs.Count > 0)
            {
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += "PRIORITY IN (";
                WhereQuery += Convert.ToString(PriorityIDs[0]);
                for (int i = 1; i < PriorityIDs.Count; i++)
                {
                    WhereQuery += ("," + Convert.ToString(PriorityIDs[i]));
                }
                WhereQuery += ")";
            }
            //Status
            List<int> StatusIDs = new List<int>();
            for (int i = 0; i < this.SelectedStatus.Count; i++)
            {
                if (this.SelectedStatus[i].Selected)
                {
                    StatusIDs.Add(this.SelectedStatus[i].ID);
                }
            }
            if (StatusIDs.Count > 0)
            {
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += "(STATUS IN (";
                WhereQuery += Convert.ToString(StatusIDs[0]);
                for (int i = 1; i < StatusIDs.Count; i++)
                {
                    WhereQuery += ("," + Convert.ToString(StatusIDs[i]));
                }
                WhereQuery += ")) ";
            }

            //Keyword
            List<int> KeywordIDs = new List<int>();
            for (int i = 0; i < this.SelectedKeyword.Count; i++)
            {
                if (this.SelectedKeyword[i].Selected)
                {
                    KeywordIDs.Add(this.SelectedKeyword[i].ID);
                }
            }
            if (KeywordIDs.Count > 0)
            {
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += "(ST_KEYWORD.ID IN (";
                WhereQuery += Convert.ToString(KeywordIDs[0]);
                for (int i = 1; i < KeywordIDs.Count; i++)
                {
                    WhereQuery += ("," + Convert.ToString(KeywordIDs[i]));
                }
                WhereQuery += ")) ";
            }

            //Submitted Info
            if (this.SubmittedByID != null && this.SubmittedByID > 0)
            {
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += "SUBMITTED_BY = " + Convert.ToString(this.SubmittedByID) + " ";
            }
            if (this.SubmittedStartComparator != 0 && this.SubmittedStartDate.Month != null && this.SubmittedStartDate.Month != 0 &&
                this.SubmittedStartComparator != 0 && this.SubmittedStartDate.Day != null && this.SubmittedStartDate.Day != 0 &&
                this.SubmittedStartComparator != 0 && this.SubmittedStartDate.Year != null && this.SubmittedStartDate.Year != 0
                )
            {
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += " CAST([SUBMITTED_ON] as date) " + Comparator(this.SubmittedStartComparator) + "  '"
                    + Convert.ToString(this.SubmittedStartDate.Month).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.SubmittedStartDate.Day).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.SubmittedStartDate.Year) + "'"
                    //+ GetSeconds(this.SubmittedStartComparator)
                    ;
            }
            else
            {
                if (this.SubmittedStartComparator != 0 && this.SubmittedStartDate.Month != null && this.SubmittedStartDate.Month != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "MONTH(SUBMITTED_ON) " + Comparator(this.SubmittedStartComparator) + " " + Convert.ToString(this.SubmittedStartDate.Month) + " ";
                }
                if (this.SubmittedStartComparator != 0 && this.SubmittedStartDate.Day != null && this.SubmittedStartDate.Day != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "DAY(SUBMITTED_ON) " + Comparator(this.SubmittedStartComparator) + " " + Convert.ToString(this.SubmittedStartDate.Day) + " ";
                }
                if (this.SubmittedStartComparator != 0 && this.SubmittedStartDate.Year != null && this.SubmittedStartDate.Year != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "YEAR(SUBMITTED_ON) " + Comparator(this.SubmittedStartComparator) + " " + Convert.ToString(this.SubmittedStartDate.Year) + " ";
                }
            }
            if (
                (this.SubmittedEndComparator != 0 && this.SubmittedEndDate.Month != null && this.SubmittedEndDate.Month != 0) &&
                (this.SubmittedEndComparator != 0 && this.SubmittedEndDate.Day != null && this.SubmittedEndDate.Day != 0) &&
                (this.SubmittedEndComparator != 0 && this.SubmittedEndDate.Year != null && this.SubmittedEndDate.Year != 0)
                )
            {
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += " CAST([SUBMITTED_ON] as date) " + Comparator(this.SubmittedEndComparator) + " '"
                    + Convert.ToString(this.SubmittedEndDate.Month).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.SubmittedEndDate.Day).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.SubmittedEndDate.Year) + "'"
                    //+GetSeconds(this.SubmittedEndComparator)+"' }"
                    ;
            }
            else
            {
                if (this.SubmittedEndComparator != 0 && this.SubmittedEndDate.Month != null && this.SubmittedEndDate.Month != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "MONTH(SUBMITTED_ON) " + Comparator(this.SubmittedEndComparator) + " " + Convert.ToString(this.SubmittedEndDate.Month) + " ";
                }
                if (this.SubmittedEndComparator != 0 && this.SubmittedEndDate.Day != null && this.SubmittedEndDate.Day != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "DAY(SUBMITTED_ON) " + Comparator(this.SubmittedEndComparator) + " " + Convert.ToString(this.SubmittedEndDate.Day) + " ";
                }
                if (this.SubmittedEndComparator != 0 && this.SubmittedEndDate.Year != null && this.SubmittedEndDate.Year != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "YEAR(SUBMITTED_ON) " + Comparator(this.SubmittedEndComparator) + " " + Convert.ToString(this.SubmittedEndDate.Year) + " ";
                }
            }

            //Assigned To
            if (this.AssignedToID != null && this.AssignedToID > 0)
            {
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += "ASSIGNED_TO = " + Convert.ToString(this.AssignedToID) + " ";
            }
            if ((this.AssignedToStartComparator != 0 && this.AssignedToStartDate.Month != null && this.AssignedToStartDate.Month != 0) &&
                (this.AssignedToStartComparator != 0 && this.AssignedToStartDate.Day != null && this.AssignedToStartDate.Day != 0) &&
                 (this.AssignedToStartComparator != 0 && this.AssignedToStartDate.Year != null && this.AssignedToStartDate.Year != 0)
                )
            {
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += " CAST([ASSIGNED_ON] as date)  " + Comparator(this.AssignedToStartComparator) + " '"
                    + Convert.ToString(this.AssignedToStartDate.Month).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.AssignedToStartDate.Day).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.AssignedToStartDate.Year) + "' "
                    // + GetSeconds(this.AssignedToStartComparator) + "' }"
                    ;
            }
            else
            {
                if (this.AssignedToStartComparator != 0 && this.AssignedToStartDate.Month != null && this.AssignedToStartDate.Month != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "MONTH(ASSIGNED_ON) " + Comparator(this.AssignedToStartComparator) + " " + Convert.ToString(this.AssignedToStartDate.Month) + " ";
                }
                if (this.AssignedToStartComparator != 0 && this.AssignedToStartDate.Day != null && this.AssignedToStartDate.Day != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "DAY(ASSIGNED_ON) " + Comparator(this.AssignedToStartComparator) + " " + Convert.ToString(this.AssignedToStartDate.Day) + " ";
                }
                if (this.AssignedToStartComparator != 0 && this.AssignedToStartDate.Year != null && this.AssignedToStartDate.Year != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "YEAR(ASSIGNED_ON) " + Comparator(this.AssignedToStartComparator) + " " + Convert.ToString(this.AssignedToStartDate.Year) + " ";
                }
            }
            if
                (
                (this.AssignedToEndComparator != 0 && this.AssignedToEndDate.Month != null && this.AssignedToEndDate.Month != 0) &&
                (this.AssignedToEndComparator != 0 && this.AssignedToEndDate.Day != null && this.AssignedToEndDate.Day != 0) &&
                (this.AssignedToEndComparator != 0 && this.AssignedToEndDate.Year != null && this.AssignedToEndDate.Year != 0)
                )
            {

                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += " CAST([ASSIGNED_ON] as date) " + Comparator(this.AssignedToEndComparator) + " '"
                    + Convert.ToString(this.AssignedToEndDate.Month).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.AssignedToEndDate.Day).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.AssignedToEndDate.Year) + "'"
                    //+ GetSeconds(this.AssignedToEndComparator) + "' }"
                    ;
            }
            else
            {
                if (this.AssignedToEndComparator != 0 && this.AssignedToEndDate.Month != null && this.AssignedToEndDate.Month != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "MONTH(ASSIGNED_ON )" + Comparator(this.AssignedToEndComparator) + " " + Convert.ToString(this.AssignedToEndDate.Month) + " ";
                }
                if (this.AssignedToEndComparator != 0 && this.AssignedToEndDate.Day != null && this.AssignedToEndDate.Day != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "DAY(ASSIGNED_ON) " + Comparator(this.AssignedToEndComparator) + " " + Convert.ToString(this.AssignedToEndDate.Day) + " ";
                }
                if (this.AssignedToEndComparator != 0 && this.AssignedToEndDate.Year != null && this.AssignedToEndDate.Year != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "YEAR(ASSIGNED_ON) " + Comparator(this.AssignedToEndComparator) + " " + Convert.ToString(this.AssignedToEndDate.Year) + " ";
                }
            }


            //Resolved info
            if (this.ResolvedByID != null && this.ResolvedByID > 0)
            {
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += "RESOLVED_BY = " + Convert.ToString(this.ResolvedByID) + " ";
            }
            if ((this.ResolvedStartComparator != 0 && this.ResolvedStartDate.Month != null && this.ResolvedStartDate.Month != 0) &&
                (this.ResolvedStartComparator != 0 && this.ResolvedStartDate.Day != null && this.ResolvedStartDate.Day != 0) &&
                 (this.ResolvedStartComparator != 0 && this.ResolvedStartDate.Year != null && this.ResolvedStartDate.Year != 0)
                )
            {
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += " CAST([RESOLVED_ON] as date) " + Comparator(this.ResolvedStartComparator) + " '"
                    + Convert.ToString(this.ResolvedStartDate.Month).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.ResolvedStartDate.Day).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.ResolvedStartDate.Year) + "'"
                    //+ GetSeconds(this.ResolvedStartComparator) + "' }"
                    ;
            }
            else
            {
                if (this.ResolvedStartComparator != 0 && this.ResolvedStartDate.Month != null && this.ResolvedStartDate.Month != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "MONTH(RESOLVED_ON) " + Comparator(this.ResolvedStartComparator) + " " + Convert.ToString(this.ResolvedStartDate.Month) + " ";
                }
                if (this.ResolvedStartComparator != 0 && this.ResolvedStartDate.Day != null && this.ResolvedStartDate.Day != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "DAY(RESOLVED_ON) " + Comparator(this.ResolvedStartComparator) + " " + Convert.ToString(this.ResolvedStartDate.Day) + " ";
                }
                if (this.ResolvedStartComparator != 0 && this.ResolvedStartDate.Year != null && this.ResolvedStartDate.Year != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "YEAR(RESOLVED_ON) " + Comparator(this.ResolvedStartComparator) + " " + Convert.ToString(this.ResolvedStartDate.Year) + " ";
                }
            }
            if ((this.ResolvedEndComparator != 0 && this.ResolvedEndDate.Month != null && this.ResolvedEndDate.Month != 0) &&
                (this.ResolvedEndComparator != 0 && this.ResolvedEndDate.Day != null && this.ResolvedEndDate.Day != 0) &&
                (this.ResolvedEndComparator != 0 && this.ResolvedEndDate.Year != null && this.ResolvedEndDate.Year != 0)
                )
            {
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += " CAST([RESOLVED_ON] as date)  " + Comparator(this.ResolvedEndComparator) + " '"
                    + Convert.ToString(this.ResolvedEndDate.Month).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.ResolvedEndDate.Day).PadLeft(2, '0') + "-"
                     + Convert.ToString(this.ResolvedEndDate.Year) + "'"
                    //+ GetSeconds(this.ResolvedEndComparator) + "' }"
                    ;
            }
            else
            {
                if (this.ResolvedEndComparator != 0 && this.ResolvedEndDate.Month != null && this.ResolvedEndDate.Month != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "MONTH(RESOLVED_ON) " + Comparator(this.ResolvedEndComparator) + " " + Convert.ToString(this.ResolvedEndDate.Month) + " ";
                }
                if (this.ResolvedEndComparator != 0 && this.ResolvedEndDate.Day != null && this.ResolvedEndDate.Day != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "DAY(RESOLVED_ON) " + Comparator(this.ResolvedEndComparator) + " " + Convert.ToString(this.ResolvedEndDate.Day) + " ";
                }
                if (this.ResolvedEndComparator != 0 && this.ResolvedEndDate.Year != null && this.ResolvedEndDate.Year != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "YEAR(RESOLVED_ON) " + Comparator(this.ResolvedEndComparator) + " " + Convert.ToString(this.ResolvedEndDate.Year) + " ";
                }
            }


            //Closed info
            if (this.ClosedByID != null && this.ClosedByID > 0)
            {
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += "CLOSED_BY = " + Convert.ToString(this.ClosedByID) + " ";
            }

            if
                ((this.ClosedStartComparator != 0 && this.ClosedStartDate.Month != null && this.ClosedStartDate.Month != 0) &&
                 (this.ClosedStartComparator != 0 && this.ClosedStartDate.Day != null && this.ClosedStartDate.Day != 0) &&
                (this.ClosedStartComparator != 0 && this.ClosedStartDate.Year != null && this.ClosedStartDate.Year != 0)
                )
            {

                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += " CAST([CLOSED_ON] as date) " + Comparator(this.ClosedStartComparator) + " '"
                    + Convert.ToString(this.ClosedStartDate.Month).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.ClosedStartDate.Day).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.ClosedStartDate.Year).PadLeft(2, '0') + "'"
                    //+ GetSeconds(this.ClosedStartComparator) + "' }"
                    ;
            }
            else
            {
                if (this.ClosedStartComparator != 0 && this.ClosedStartDate.Month != null && this.ClosedStartDate.Month != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "MONTH(CLOSED_ON) " + Comparator(this.ClosedStartComparator) + " " + Convert.ToString(this.ClosedStartDate.Month) + " ";
                }
                if (this.ClosedStartComparator != 0 && this.ClosedStartDate.Day != null && this.ClosedStartDate.Day != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "DAY(CLOSED_ON) " + Comparator(this.ClosedStartComparator) + " " + Convert.ToString(this.ClosedStartDate.Day) + " ";
                }
                if (this.ClosedStartComparator != 0 && this.ClosedStartDate.Year != null && this.ClosedStartDate.Year != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "YEAR(CLOSED_ON) " + Comparator(this.ClosedStartComparator) + " " + Convert.ToString(this.ClosedStartDate.Year) + " ";
                }
            }

            if
                (
                (this.ClosedEndComparator != 0 && this.ClosedEndDate.Month != null && this.ClosedEndDate.Month != 0) &&
                (this.ClosedEndComparator != 0 && this.ClosedEndDate.Day != null && this.ClosedEndDate.Day != 0) &&
                (this.ClosedEndComparator != 0 && this.ClosedEndDate.Year != null && this.ClosedEndDate.Year != 0)
                )
            {
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += "CAST( [CLOSED_ON] as date ) " + Comparator(this.ClosedEndComparator) + "'"
                    + Convert.ToString(this.ClosedEndDate.Month).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.ClosedEndDate.Day).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.ClosedEndDate.Year) + "'"
                    // + GetSeconds(this.ClosedEndComparator) + "' }"
                    ;
            }
            else
            {
                if (this.ClosedEndComparator != 0 && this.ClosedEndDate.Month != null && this.ClosedEndDate.Month != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "MONTH(CLOSED_ON )" + Comparator(this.ClosedEndComparator) + " " + Convert.ToString(this.ClosedEndDate.Month) + " ";
                }
                if (this.ClosedEndComparator != 0 && this.ClosedEndDate.Day != null && this.ClosedEndDate.Day != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "DAY(CLOSED_ON) " + Comparator(this.ClosedEndComparator) + " " + Convert.ToString(this.ClosedEndDate.Day) + " ";
                }
                if (this.ClosedEndComparator != 0 && this.ClosedEndDate.Year != null && this.ClosedEndDate.Year != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "YEAR(CLOSED_ON)" + Comparator(this.ClosedEndComparator) + " " + Convert.ToString(this.ClosedEndDate.Year) + " ";
                }
            }

            //Modified

            if ((this.ModifiedAfterComparator != 0 && this.ModifiedAfter.Month != null && this.ModifiedAfter.Month != 0) &&
                 (this.ModifiedAfterComparator != 0 && this.ModifiedAfter.Day != null && this.ModifiedAfter.Day != 0) &&
                (this.ModifiedAfterComparator != 0 && this.ModifiedAfter.Year != null && this.ModifiedAfter.Year != 0)
                )
            {
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += " CAST([LAST_UPDATE] as date) " + Comparator(this.ModifiedAfterComparator) + " '"
                    + Convert.ToString(this.ModifiedAfter.Month).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.ModifiedAfter.Day).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.ModifiedAfter.Year) + "'"
                    //+ GetSeconds(this.ModifiedAfterComparator) + "' }"
                    ;
            }
            else
            {
                if (this.ModifiedAfterComparator != 0 && this.ModifiedAfter.Month != null && this.ModifiedAfter.Month != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "MONTH(LAST_UPDATE) " + Comparator(this.ModifiedAfterComparator) + Convert.ToString(this.ModifiedAfter.Month) + " ";
                }
                if (this.ModifiedAfterComparator != 0 && this.ModifiedAfter.Day != null && this.ModifiedAfter.Day != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "DAY(LAST_UPDATE) " + Comparator(this.ModifiedAfterComparator) + Convert.ToString(this.ModifiedAfter.Day) + " ";
                }
                if (this.ModifiedAfterComparator != 0 && this.ModifiedAfter.Year != null && this.ModifiedAfter.Year != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "YEAR(LAST_UPDATE) " + Comparator(this.ModifiedAfterComparator) + Convert.ToString(this.ModifiedAfter.Year) + " ";
                }
            }

            if ((this.ModifiedBeforeComparator != 0 && this.ModifiedBefore.Month != null && this.ModifiedBefore.Month != 0) &&
                (this.ModifiedBeforeComparator != 0 && this.ModifiedBefore.Day != null && this.ModifiedBefore.Day != 0) &&
                (this.ModifiedBeforeComparator != 0 && this.ModifiedBefore.Year != null && this.ModifiedBefore.Year != 0)
                )
            {
                if (WhereQuery.Length > 0)
                {
                    WhereQuery += " AND ";
                }
                WhereQuery += " CAST([LAST_UPDATE] as date) " + Comparator(this.ModifiedBeforeComparator) + " '"
                    + Convert.ToString(this.ModifiedBefore.Month).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.ModifiedBefore.Day).PadLeft(2, '0') + "-"
                    + Convert.ToString(this.ModifiedBefore.Year) + "'"

                    //+ GetSeconds(this.ModifiedBeforeComparator) + "' }"
                    ;
            }
            else
            {
                if (this.ModifiedBeforeComparator != 0 && this.ModifiedBefore.Month != null && this.ModifiedBefore.Month != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "MONTH(LAST_UPDATE ) " + Comparator(this.ModifiedBeforeComparator) + Convert.ToString(this.ModifiedBefore.Month) + " ";
                }
                if (this.ModifiedBeforeComparator != 0 && this.ModifiedBefore.Day != null && this.ModifiedBefore.Day != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "DAY(LAST_UPDATE) " + Comparator(this.ModifiedBeforeComparator) + Convert.ToString(this.ModifiedBefore.Day) + " ";
                }
                if (this.ModifiedBeforeComparator != 0 && this.ModifiedBefore.Year != null && this.ModifiedBefore.Year != 0)
                {
                    if (WhereQuery.Length > 0)
                    {
                        WhereQuery += " AND ";
                    }
                    WhereQuery += "YEAR(LAST_UPDATE) " + Comparator(this.ModifiedBeforeComparator) + Convert.ToString(this.ModifiedBefore.Year) + " ";
                }
            }


            //   END OF WHERE
            if (!String.IsNullOrEmpty(WhereQuery) && !String.IsNullOrWhiteSpace(WhereQuery))
            {
                WhereQuery = (" WHERE " + WhereQuery + " ");
            }
            // Start Display Query 
            Query += DisplayQuery;
            if (DisplayQuery.Contains("REA_PRODUCT.NAME") || SortByQuery.Contains("REA_PRODUCT.NAME"))
            {
                FromQuery += "INNER JOIN REA_PRODUCT ON REA_PRODUCT.PRODUCT_ID = REA_TRACK.PRODUCT ";
            }
            if (DisplayQuery.Contains("REA_CLOSED_REASON.NAME") || SortByQuery.Contains("REA_CLOSED_REASON.NAME"))
            {
                FromQuery += "LEFT OUTER JOIN REA_CLOSED_REASON ON REA_TRACK.CLOSED_REASON = REA_CLOSED_REASON.REASON_ID ";
            }
            if (DisplayQuery.Contains("ST_USERS_ASSIGNED_TO") || SortByQuery.Contains("ST_USERS_ASSIGNED_TO "))
            {
                FromQuery += "LEFT OUTER JOIN ST_USERS ST_USERS_ASSIGNED_TO ON REA_TRACK.ASSIGNED_TO = ST_USERS_ASSIGNED_TO.USER_ID ";
            }
            if (DisplayQuery.Contains("ST_USERS_SUBMITTED_BY") || SortByQuery.Contains("ST_USERS_SUBMITTED_BY"))
            {
                FromQuery += "LEFT OUTER JOIN ST_USERS ST_USERS_SUBMITTED_BY ON REA_TRACK.SUBMITTED_BY= ST_USERS_SUBMITTED_BY.USER_ID ";
            }
            if (DisplayQuery.Contains("ST_USERS_SUBMITTED_TO") || SortByQuery.Contains("ST_USERS_SUBMITTED_TO"))
            {
                FromQuery += "LEFT OUTER JOIN ST_USERS ST_USERS_SUBMITTED_TO ON REA_TRACK.SUBMITTED_TO= ST_USERS_SUBMITTED_TO.USER_ID ";
            }
            if (DisplayQuery.Contains("ST_USERS_ASSIGNED_BY") || SortByQuery.Contains("ST_USERS_ASSIGNED_BY"))
            {
                FromQuery += "LEFT OUTER JOIN ST_USERS ST_USERS_ASSIGNED_BY ON REA_TRACK.ASSIGNED_BY= ST_USERS_ASSIGNED_BY.USER_ID ";
            }
            if (DisplayQuery.Contains("ST_USERS_RESOLVED_BY") || SortByQuery.Contains("ST_USERS_RESOLVED_BY"))
            {
                FromQuery += "LEFT OUTER JOIN ST_USERS ST_USERS_RESOLVED_BY ON REA_TRACK.RESOLVED_BY= ST_USERS_RESOLVED_BY.USER_ID ";
            }
            if (DisplayQuery.Contains("ST_USERS_CLOSED_BY") || SortByQuery.Contains("ST_USERS_CLOSED_BY"))
            {
                FromQuery += "LEFT OUTER JOIN ST_USERS ST_USERS_CLOSED_BY ON REA_TRACK.CLOSED_BY= ST_USERS_CLOSED_BY.USER_ID ";
            }
            if (DisplayQuery.Contains("REA_STATUS.NAME") || SortByQuery.Contains("REA_STATUS.NAME"))
            {
                FromQuery += "LEFT OUTER JOIN REA_STATUS ON REA_TRACK.STATUS = REA_STATUS.STATUS_ID ";
            }
            if (WhereQuery.Contains("REA_HISTORY.DESCRIPTION"))    //SEK SCR84313 11-1-2021
            {
                FromQuery += "INNER JOIN REA_HISTORY ON REA_HISTORY.TRACKING_ID = REA_TRACK.TRACKING_ID ";
            }
            if (DisplayQuery.Contains("MADE_ON") || SortByQuery.Contains("MADE_ON"))    //SEK SCR84313 11-1-2021
            {
                FromQuery += "LEFT OUTER JOIN REA_HISTORY H2 ON H2.TRACKING_ID = REA_TRACK.TRACKING_ID AND H2.HISTORY_ID = (SELECT MAX(HISTORY_ID) FROM REA_HISTORY WHERE TRACKING_ID = REA_TRACK.TRACKING_ID AND TYPE = 22) ";
            }
            if (WhereQuery.Contains("REA_KEYWORD"))
            {
                FromQuery += "INNER JOIN REA_KEYWORD_REA_RELATION ON REA_KEYWORD_REA_RELATION.TRACKING_ID = REA_TRACK.TRACKING_ID ";
                FromQuery += "INNER JOIN REA_KEYWORD ON REA_KEYWORD.ID = REA_KEYWORD_REA_RELATION.KEYWORD_ID ";
            }
            Query += (FromQuery + WhereQuery);
            this.WhereSQLRows = (FromQuery + WhereQuery);
            this.fullWhereQuery = WhereQuery;

            //this.WhereSQLRowsForMassUpdate = this.WhereSQLRowsForMassUpdate.Replace("@RELATED@", RELATED_REAS_STRING);  


            if (!String.IsNullOrEmpty(SortByQuery))
            {
                Query += (" ORDER BY " + SortByQuery);
                this.WhereSQLRows += " ORDER BY " + SortByQuery;
                this.fullWhereQuery += " ORDER BY " + SortByQuery;
            }
            else
            {
                Query += (" ORDER BY REA_TRACK.TRACKING_ID DESC ");
                this.fullWhereQuery += (" ORDER BY REA_TRACK.TRACKING_ID DESC ");
            }

            this.SelectStatement = (String.IsNullOrEmpty(DisplayQuery) ? "" : DisplayQuery);

            this.SortByStatement = (String.IsNullOrEmpty(SortByQuery) ? "" : SortByQuery);

            return Query;
        }
        public String SortByFilter(String SortByItem)
        {
            int indexOfAsClause = SortByItem.IndexOf("AS '");
            String SubString = SortByItem;
            if (indexOfAsClause >= 0)
            {
                SubString = SortByItem.Substring(0, indexOfAsClause);
            }
            return SubString;
        }
        //User Comparator only for DateTime Comparator
        public String Comparator(int symbol)
        {
            String ReturnValue = "";
            switch (symbol)
            {
                case 1:
                    ReturnValue = "<";
                    break;
                case 2:
                    ReturnValue = "<=";
                    break;
                case 3:
                    ReturnValue = "=";
                    break;
                case 4:
                    ReturnValue = ">=";
                    break;
                case 5:
                    ReturnValue = ">";
                    break;
            }
            if (ReturnValue.Length == 0)
            {
                ReturnValue = "";
            }
            return ReturnValue;
        }
        public int ComparatorToValue(String symbol)
        {
            switch (symbol)
            {
                case "<":
                    return 1;
                case "<=":
                    return 2;
                case "=":
                    return 3;
                case ">=":
                    return 4;
                case ">":
                    return 5;
            }
            return -1;
        }
        public bool IsFrequentColumn(String word)
        {
            if (
                word.Contains("REA Number") ||
                word.Contains("Product") ||
                word.Contains("Title") ||
                word.Contains("Priority") ||
                word.Equals("Status") ||
                word.Contains("Assigned To") ||
                word.Contains("Resolved By") ||
                word.Contains("Submitted By") ||
                word.Contains("Closed By") ||
                word.Contains("Last Modified")
                )
            {
                return true;
            }
            return false;
        }
        public bool IsVersion(String type, String word)
        {
            if (word.Contains(type))
            {
                return true;
            }
            return false;
        }
        public bool IsEnhancement(String word)
        {
            if (
                word.Contains("Problem") ||
                word.Contains("Benefits") ||
                word.Contains("Solution")
                )
            {
                return true;
            }
            return false;
        }
        public bool IsCustomerDetails(String word)
        {
            if (word.Contains("Customer") || word.Contains("Syteline"))
            {
                return true;
            }
            return false;
        }
        public bool IsRest(String word)
        {
            if (
                !(this.IsCustomerDetails(word)) &&
                !(this.IsEnhancement(word)) &&
                !this.Translator[word].Contains("EST_HOURS") &&
                !this.Translator[word].Contains("ACT_HOURS") &&
                !this.Translator[word].Contains("VC_") &&
                !this.Translator[word].Contains("VP_") &&
                !this.Translator[word].Contains("VR_") &&
                !this.Translator[word].Contains("VF_") &&
                !(this.IsFrequentColumn(word))
              )
            {
                return true;
            }
            return false;
        }
        public string GetSeconds(int type)
        {
            // 0:"",
            // 1:"<",
            // 2:"<=",
            // 3:"=",
            // 4:">="
            string value = "";
            switch (type)
            {
                case 1:
                case 2:
                    value = "23:59:59";
                    break;
                case 4:
                case 5:
                    value = "00:00:00";
                    break;
                default:
                    break;
            }
            return value;
        }

        // Save the Tabular Report
        public int ProcessNewReport(int ST_USER_ID)
        {
            //Returns -1 if they try to update, delete, or 
            int ReportId;
            if (String.IsNullOrEmpty(this.RawSQL))
            {
                ReportId = this.SaveNewReport(ST_USER_ID);
            }
            else
            {
                if (
                    !this.RawSQL.ToLower().Contains("delete")
                    && !this.RawSQL.ToLower().Contains("update")
                    && !this.RawSQL.ToLower().Contains("insert")
                    )
                {
                    ReportId = this.SaveSQLReport(ST_USER_ID);
                }
                else
                {
                    ReportId = -1;
                }
            }
            return ReportId;
        }
        public int SaveSQLReport(int ST_USER_ID)
        {
            REATrackerDB sql = new REATrackerDB();
            string COMMAND = "EXEC Report_InsertSQLReport " + Convert.ToString(ST_USER_ID)
                + ", '" + this.NameOfReport.Replace("'", "''")
                + "', " + this.PublicReport
                + ", '" + this.RawSQL.Replace("'", "''") + "'";
            int ReportID =
                Convert.ToInt32(
                    sql.ProcessScalarCommand(COMMAND)
                );
            return ReportID;
        }
        public int SaveNewReport(int ST_USER_ID)
        {
            //ToDo: refactor this;

            int? maxrowint = null;
            if (!string.IsNullOrEmpty(this.MaxRows)) { maxrowint = Convert.ToInt32(this.MaxRows.Replace("TOP", "").Trim()); }
            REATrackerDB sql = new REATrackerDB();
            int ReportID = Convert.ToInt32(sql.CreateNewTabularReport(
                 //Returns the ID of the new instance of New tabular report
                 ST_USER_ID, this.PublicReport, this.NameOfReport,
                 this.REAComparator, Convert.ToInt32(this.REANumber), Convert.ToInt32(this.REANumber2),
                 this.SelectItemToString(this.SelectedStatus), this.HiddenProductString,
                 this.ReqDocChg, this.ReqDBChg, this.ReqReqChg,
                 this.Where, maxrowint, this.SelectItemToString(this.SelectedIssueTypes),
                 this.CustomerReported, Convert.ToInt32(this.CustomerLogic), this.CustomerContact,
                 this.TextTypeSearch, this.TitleText, this.SelectItemToString(this.SelectedPriority),
                 //Dates
                 this.SubmittedByID,
                 this.SubmittedStartDate.Month, this.SubmittedStartDate.Day, this.SubmittedStartDate.Year, Comparator(this.SubmittedStartComparator), "SUB_START",
                 this.SubmittedEndDate.Month, this.SubmittedEndDate.Day, this.SubmittedEndDate.Year, Comparator(this.SubmittedEndComparator), "SUB_END",
                 this.AssignedToID,
                 this.AssignedToStartDate.Month, this.AssignedToStartDate.Day, this.AssignedToStartDate.Year, Comparator(this.AssignedToStartComparator), "ASN_START",
                 this.AssignedToEndDate.Month, this.AssignedToEndDate.Day, this.AssignedToEndDate.Year, Comparator(this.AssignedToEndComparator), "ASN_END",
                 this.ResolvedByID,
                 this.ResolvedStartDate.Month, this.ResolvedStartDate.Day, this.ResolvedStartDate.Year, Comparator(this.ResolvedStartComparator), "RES_START",
                 this.ResolvedEndDate.Month, this.ResolvedEndDate.Day, this.ResolvedEndDate.Year, Comparator(this.ResolvedEndComparator), "RES_END",
                 this.ClosedByID,
                 this.ClosedStartDate.Month, this.ClosedStartDate.Day, this.ClosedStartDate.Year, Comparator(this.ClosedStartComparator), "CLO_START",
                 this.ClosedEndDate.Month, this.ClosedEndDate.Day, this.ClosedEndDate.Year, Comparator(this.ClosedEndComparator), "CLO_END",
                 this.ModifiedAfter.Month, this.ModifiedAfter.Day, this.ModifiedAfter.Year, this.Comparator(this.ModifiedAfterComparator), "MODIFIED_START",
                 this.ModifiedBefore.Month, this.ModifiedBefore.Day, this.ModifiedBefore.Year, this.Comparator(this.ModifiedBeforeComparator), "MODIFIED_END",
                 //Versions
                 this.VersionFound.Major, this.VersionFound.Minor, this.VersionFound.Build, this.VersionFound.Misc, this.VersionFound.MajorCompare, this.VersionFound.MinorCompare, this.VersionFound.BuildCompare,
                 "VERSION_FOUND",
                 this.VersionResolved.Major, this.VersionResolved.Minor, this.VersionResolved.Build, this.VersionResolved.Misc, this.VersionResolved.MajorCompare, this.VersionResolved.MinorCompare, this.VersionResolved.BuildCompare,
                 "VERSION_RESOLVED",
                 this.VersionClosed.Major, this.VersionClosed.Minor, this.VersionClosed.Build, this.VersionClosed.Misc, this.VersionClosed.MajorCompare, this.VersionClosed.MinorCompare, this.VersionClosed.BuildCompare,
                 "VERSION_CLOSED",
                 this.VersionPlanned.Major, this.VersionPlanned.Minor, this.VersionPlanned.Build, this.VersionPlanned.Misc, this.VersionPlanned.MajorCompare, this.VersionPlanned.MinorCompare, this.VersionPlanned.BuildCompare,
                 "VERSION_PLANNED",
                 //Display
                 this.SelectStatement,
                 //Sort By
                 this.SortByStatement,
                 //The Customer info
                 this.CustomerMachineSN, this.CustomerSyteLine, this.CustomerCompany, this.CustomerLocation, this.HistoryTerm, Convert.ToString(this.HistorySearchLogic),
                 this.Rank, this.RankComparator,
                 this.Size, this.SizeComparator, this.SelectItemToString(this.SelectedKeyword)
                 ));
            return ReportID;
        }
        private String StatusListToString()
        {
            String ListOfStatus = "";
            for (int i = 0; i < this.Statuses.Count; i++)
            {
                if (this.Statuses[i].Selected)
                {
                    if (!String.IsNullOrEmpty(ListOfStatus))
                    {
                        ListOfStatus += ",";
                    }
                    ListOfStatus += (this.Statuses[i].ID);
                }
            }
            return ListOfStatus;
        }
        private String PriorityListToString()
        {
            String ListOfPriorities = "";
            for (int i = 0; i < this.SelectedPriority.Count; i++)
            {
                if (this.SelectedPriority[i].Selected)
                {
                    ListOfPriorities += (this.SelectedPriority[i].ID + ",");
                }
            }
            return ListOfPriorities;
        }
        private String SelectItemToString(List<Item> ItemList)
        {
            String ReturnValue = "";
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (ItemList[i].Selected)
                {
                    if (!String.IsNullOrEmpty(ReturnValue))
                    {
                        ReturnValue += ",";
                    }
                    ReturnValue += (ItemList[i].ID);
                }
            }
            return ReturnValue;
        }
        //Delete The Tabular Report
        public void Delete()
        {
            //This comes from the advanced search page
            REATrackerDB sql = new REATrackerDB();
            sql.DeleteReport(this.ReportID);
        }
        //Update The Reports
        public Boolean isAdminOrManager(int UserID)
        {
            REATrackerDB sql = new REATrackerDB();
            return sql.isAdminOrManager(UserID);
        }
        public void Update()
        {
            if (this.RawSQL == null || this.RawSQL.Trim() == "")
            {
                this.UpdateReport();
            }
            else
            {
                this.UpdateSQLReport();
            }
        }
        private void UpdateReport()
        {
            //ToDo: refactor this;
            int? maxrowint = null;
            if (!string.IsNullOrEmpty(this.MaxRows)) { maxrowint = Convert.ToInt32(this.MaxRows.Replace("TOP", "").Trim()); }
            REATrackerDB sql = new REATrackerDB();
            sql.UpdateReport(
             this.ReportID, this.PublicReport, this.NameOfReport,
             this.REAComparator, Convert.ToInt32(this.REANumber), Convert.ToInt32(this.REANumber2),
             this.SelectItemToString(this.SelectedStatus), this.HiddenProductString,
             this.ReqDocChg, this.ReqDBChg, this.ReqReqChg,
             this.Where, maxrowint, this.SelectItemToString(this.SelectedIssueTypes),
             this.CustomerReported, Convert.ToInt32(this.CustomerLogic), this.CustomerContact,
             this.TextTypeSearch, this.TitleText, this.SelectItemToString(this.SelectedPriority),
             //Dates
             this.SubmittedByID,
             this.SubmittedStartDate.Month, this.SubmittedStartDate.Day, this.SubmittedStartDate.Year, Comparator(this.SubmittedStartComparator), "SUB_START",
             this.SubmittedEndDate.Month, this.SubmittedEndDate.Day, this.SubmittedEndDate.Year, Comparator(this.SubmittedEndComparator), "SUB_END",
             this.AssignedToID,
             this.AssignedToStartDate.Month, this.AssignedToStartDate.Day, this.AssignedToStartDate.Year, Comparator(this.AssignedToStartComparator), "ASN_START",
             this.AssignedToEndDate.Month, this.AssignedToEndDate.Day, this.AssignedToEndDate.Year, Comparator(this.AssignedToEndComparator), "ASN_END",
             this.ResolvedByID,
             this.ResolvedStartDate.Month, this.ResolvedStartDate.Day, this.ResolvedStartDate.Year, Comparator(this.ResolvedStartComparator), "RES_START",
             this.ResolvedEndDate.Month, this.ResolvedEndDate.Day, this.ResolvedEndDate.Year, Comparator(this.ResolvedEndComparator), "RES_END",
             this.ClosedByID,
             this.ClosedStartDate.Month, this.ClosedStartDate.Day, this.ClosedStartDate.Year, Comparator(this.ClosedStartComparator), "CLO_START",
             this.ClosedEndDate.Month, this.ClosedEndDate.Day, this.ClosedEndDate.Year, Comparator(this.ClosedEndComparator), "CLO_END",
             this.ModifiedAfter.Month, this.ModifiedAfter.Day, this.ModifiedAfter.Year, this.Comparator(this.ModifiedAfterComparator), "MODIFIED_START",
             this.ModifiedBefore.Month, this.ModifiedBefore.Day, this.ModifiedBefore.Year, this.Comparator(this.ModifiedBeforeComparator), "MODIFIED_END",
             //Versions
             this.VersionFound.Major, this.VersionFound.Minor, this.VersionFound.Build, this.VersionFound.Misc, this.VersionFound.MajorCompare, this.VersionFound.MinorCompare, this.VersionFound.BuildCompare,
             "VERSION_FOUND",
             this.VersionResolved.Major, this.VersionResolved.Minor, this.VersionResolved.Build, this.VersionResolved.Misc, this.VersionResolved.MajorCompare, this.VersionResolved.MinorCompare, this.VersionResolved.BuildCompare,
             "VERSION_RESOLVED",
             this.VersionClosed.Major, this.VersionClosed.Minor, this.VersionClosed.Build, this.VersionClosed.Misc, this.VersionClosed.MajorCompare, this.VersionClosed.MinorCompare, this.VersionClosed.BuildCompare,
             "VERSION_CLOSED",
             this.VersionPlanned.Major, this.VersionPlanned.Minor, this.VersionPlanned.Build, this.VersionPlanned.Misc, this.VersionPlanned.MajorCompare, this.VersionPlanned.MinorCompare, this.VersionPlanned.BuildCompare,
             "VERSION_PLANNED",
             //Display
             this.SelectStatement,
             //Sort By
             this.SortByStatement,
             //The Customer info
             this.CustomerMachineSN, this.CustomerSyteLine, this.CustomerCompany, this.CustomerLocation,
             this.HistoryTerm, Convert.ToString(this.HistorySearchLogic),
             this.Rank, this.RankComparator, this.Size, this.SizeComparator, this.SelectItemToString(this.SelectedKeyword)
             );
        }
        private void UpdateSQLReport()
        {
            REATrackerDB sql = new REATrackerDB();
            sql.ProcessCommand(
            "EXEC Report_UpdateSQLReport " + Convert.ToString(this.ReportID) + ", '" + this.NameOfReport.Replace("'", "''") + "', " + this.PublicReport + ", '" + this.RawSQL.Replace("'", "''") + "'"
            );
        }
        //populating the Graph
        private void populateGraphData()
        {
            //populate ProblemCount
            //populate EnhancemnetCount
            //populate PlannedWorkdCount
            //populate total Count
            bool containsIssue = false;
            int typeIndex = -1;
            bool containsPriority = false;
            int priorityIndex = -1;
            if (this.ResultsTable != null)
            {
                for (int i = 0; i < ResultsTable.Columns.Count; i++)
                {
                    if (ResultsTable.Columns[i].ColumnName.ToLower().Contains("type") ||
                        ResultsTable.Columns[i].ColumnName.ToLower().Contains("issue")
                        )
                    {
                        containsIssue = true;
                        typeIndex = i;
                    }
                    if (ResultsTable.Columns[i].ColumnName.ToLower().Contains("priority"))
                    {
                        containsPriority = true;
                        priorityIndex = i;
                    }
                }
            }

            if (containsIssue && containsPriority)
            {
                for (int i = 0; i < this.ResultsTable.Rows.Count; i++)
                {
                    DataRow row = this.ResultsTable.Rows[i];
                    int tempPriority = -1;
                    if (!Int32.TryParse(Convert.ToString(row[priorityIndex]), out tempPriority)
                        )
                    {
                        tempPriority = PriorityNameToIndex(Convert.ToString(row[priorityIndex]));
                    }
                    int IssueType = -1;
                    if (!Int32.TryParse(Convert.ToString(row[typeIndex]), out IssueType)
                        )
                    {
                        tempPriority = IssueTypeNameToIndex((string)(row[typeIndex] == DBNull.Value ? "" : row[typeIndex]));
                    }
                    switch (IssueType - 1)
                    {
                        case 0://Problem
                            this.ProblemCount[tempPriority - 1]++;
                            break;
                        case 1://Enhancement
                            this.EnhancementCount[tempPriority - 1]++;
                            break;
                        case 2://Planned Work
                            this.PlannedWorkCount[tempPriority - 1]++;
                            break;
                        default:
                            break;
                    }
                }
            }



        }
        public void populateGraph()
        {

            this.LowWeight = 1;
            this.MediumWeight = 4;
            this.HighWeight = 9;
            this.CritcalWeight = 16;

            this.TotalCount = new[] { 0, 0, 0, 0 };
            this.ProblemCount = new[] { 0, 0, 0, 0 };
            this.EnhancementCount = new[] { 0, 0, 0, 0 };
            this.PlannedWorkCount = new[] { 0, 0, 0, 0 };

            this.TotalMetric = new[] { 0, 0, 0, 0 };
            this.ProblemMetric = new[] { 0, 0, 0, 0 };
            this.EnhancementMetric = new[] { 0, 0, 0, 0 };
            this.PlannedWorkMetric = new[] { 0, 0, 0, 0 };
            this.yMetricValues = new List<String>();
            this.yValues = new List<String>();
            this.xValues = new List<String>();

            this.populateGraphData();

            this.Title = this.NameOfReport;
            this.xValues.Add("Total");
            this.TotalCount[0] = (this.ProblemCount[0] + this.EnhancementCount[0] + this.PlannedWorkCount[0]);
            this.TotalCount[1] = (this.ProblemCount[1] + this.EnhancementCount[1] + this.PlannedWorkCount[1]);
            this.TotalCount[2] = (this.ProblemCount[2] + this.EnhancementCount[2] + this.PlannedWorkCount[2]);
            this.TotalCount[3] = (this.ProblemCount[3] + this.EnhancementCount[3] + this.PlannedWorkCount[3]);
            this.yValues.Add(
                        (this.TotalCount[0]).ToString() + "," +
                        (this.TotalCount[1]).ToString() + "," +
                        (this.TotalCount[2]).ToString() + "," +
                        (this.TotalCount[3]).ToString()
                            );

            this.TotalMetric[0] = this.TotalCount[0] * this.LowWeight;
            this.TotalMetric[1] = this.TotalCount[1] * this.MediumWeight;
            this.TotalMetric[2] = this.TotalCount[2] * this.HighWeight;
            this.TotalMetric[3] = this.TotalCount[3] * this.CritcalWeight;
            this.yMetricValues.Add(
                        this.TotalMetric[0].ToString() + "," +
                        this.TotalMetric[1].ToString() + "," +
                        this.TotalMetric[2].ToString() + "," +
                        this.TotalMetric[3].ToString()
                );

            this.xValues.Add("Problem");
            this.yValues.Add(
                        this.ProblemCount[0].ToString() + "," +
                        this.ProblemCount[1].ToString() + "," +
                        this.ProblemCount[2].ToString() + "," +
                        this.ProblemCount[3].ToString()
                );

            this.ProblemMetric[0] = this.ProblemCount[0] * this.LowWeight;
            this.ProblemMetric[1] = this.ProblemCount[1] * this.MediumWeight;
            this.ProblemMetric[2] = this.ProblemCount[2] * this.HighWeight;
            this.ProblemMetric[3] = this.ProblemCount[3] * this.CritcalWeight;
            this.yMetricValues.Add(
                        this.ProblemMetric[0].ToString() + "," +
                        this.ProblemMetric[1].ToString() + "," +
                        this.ProblemMetric[2].ToString() + "," +
                        this.ProblemMetric[3].ToString()
                );

            this.xValues.Add("Enhancement");
            this.yValues.Add(
                        this.EnhancementCount[0].ToString() + "," +
                        this.EnhancementCount[1].ToString() + "," +
                        this.EnhancementCount[2].ToString() + "," +
                        this.EnhancementCount[3].ToString()
                );

            this.EnhancementMetric[0] = this.EnhancementCount[0] * this.LowWeight;
            this.EnhancementMetric[1] = this.EnhancementCount[1] * this.MediumWeight;
            this.EnhancementMetric[2] = this.EnhancementCount[2] * this.HighWeight;
            this.EnhancementMetric[3] = this.EnhancementCount[3] * this.CritcalWeight;
            this.yMetricValues.Add(
                        this.EnhancementMetric[0].ToString() + "," +
                        this.EnhancementMetric[1].ToString() + "," +
                        this.EnhancementMetric[2].ToString() + "," +
                        this.EnhancementMetric[3].ToString()
                );

            this.xValues.Add("Planned Work");
            this.yValues.Add(
                        this.PlannedWorkCount[0].ToString() + "," +
                        this.PlannedWorkCount[1].ToString() + "," +
                        this.PlannedWorkCount[2].ToString() + "," +
                        this.PlannedWorkCount[3].ToString()
                );
            this.PlannedWorkMetric[0] = this.PlannedWorkCount[0] * this.LowWeight;
            this.PlannedWorkMetric[1] = this.PlannedWorkCount[1] * this.MediumWeight;
            this.PlannedWorkMetric[2] = this.PlannedWorkCount[2] * this.HighWeight;
            this.PlannedWorkMetric[3] = this.PlannedWorkCount[3] * this.CritcalWeight;
            this.yMetricValues.Add(
                        this.PlannedWorkMetric[0].ToString() + "," +
                        this.PlannedWorkMetric[1].ToString() + "," +
                        this.PlannedWorkMetric[2].ToString() + "," +
                        this.PlannedWorkMetric[3].ToString()
                );
        }
        private int PriorityNameToIndex(String name)
        {
            int result = -1;
            if (name.ToLower() == "low")
            {
                result = 0;
            }
            else if (name.ToLower() == "medium")
            {
                result = 1;
            }
            else if (name.ToLower() == "high")
            {
                result = 2;
            }
            else if (name.ToLower() == "critical")
            {
                result = 3;
            }
            return result;
        }

        private int IssueTypeNameToIndex(String name)
        {
            int result = -1;
            if (name.ToLower() == "problem")
            {
                result = 0;
            }
            else if (name.ToLower() == "enhancement")
            {
                result = 1;
            }
            else if (name.ToLower() == "planned work")
            {
                result = 2;
            }
            return result;
        }

        public class Version
        {
            public int? Major { get; set; }
            public int? Minor { get; set; }
            public int? Build { get; set; }
            public String Misc { get; set; }

            public String MajorCompare { get; set; }
            public String MinorCompare { get; set; }
            public String BuildCompare { get; set; }
            public String MiscCompare { get; set; }

            public Version() { this.Misc = ""; }
            public Version(int major, int minor, int build, String misc)
            {
                this.Major = major;
                this.Minor = minor;
                this.Build = build;
                this.Misc = misc;
            }
        }
        //Date Month Year = DMY
        public class DMYDate
        {
            public int? Month { get; set; }
            public int? Day { get; set; }
            public int? Year { get; set; }
            public DMYDate() { }
            public DMYDate(int month, int day, int year)
            {
                this.Month = month;
                this.Day = day;
                this.Year = year;
            }
        }

        public void populateHistory()
        {
            this.HistoryTables = new DataSet("HISTORY");
            if (this.WhereSQLRows != null)
            {
                this.WhereSQLRows = this.WhereSQLRows.Trim();
                if (this.WhereSQLRows.ToLower().StartsWith("where"))
                {
                    this.WhereSQLRows = this.WhereSQLRows.Substring(5, this.WhereSQLRows.Length - 5);
                }
                REATrackerDB sql = new REATrackerDB();
                DataTable SCRList = sql.ProcessCommand("SELECT TRACKING_ID " + this.WhereSQLRows);
                foreach (DataRow scr in SCRList.Rows)
                {
                    int trackingID = (int)scr["TRACKING_ID"];
                    DataTable dt = HistoryTables.Tables.Add(trackingID.ToString());
                    string cmd = $@"
                    SELECT 
                        REA_HISTORY.TYPE, REA_HISTORY.MADE_BY, REA_HISTORY.MADE_ON, REA_HISTORY.DESCRIPTION, REA_HISTORY.FILE_ID, (ST_USERS.FIRST_NAME+' '+ST_USERS.LAST_NAME) AS MADE_BY_NAME, ST_ATTACHMENT.FILENAME, ST_RELATED.SCR_ID AS RELATED_ID, REA_TRACK.TITLE AS TITLE 
                    FROM
                        REA_HISTORY
                    LEFT JOIN ST_USERS ON REA_HISTORY.MADE_BY = ST_USERS.USER_ID
                    LEFT JOIN REA_ATTACHMENT ON ST_ATTACHMENT.FILE_ID = REA_HISTORY.FILE_ID
                    LEFT JOIN REA_RELATED ON REA_RELATED.RELATED_ID= REA_HISTORY.FILE_ID
                    LEFT JOIN REA_TRACK ON REA_TRACK.TRACKING_ID = REA_RELATED.SCR_ID
                    WHERE 
                        REA_HISTORY.TRACKING_ID = {trackingID}
                    ORDER BY
                        REA_HISTORY.HISTORY_ID DESC";

                    HistoryTables.Tables[trackingID.ToString()].Merge(sql.ProcessCommand(cmd));
                }
            }
        }
    }  
}