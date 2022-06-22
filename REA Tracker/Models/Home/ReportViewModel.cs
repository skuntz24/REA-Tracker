using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using REA_Tracker.Helpers;
using QVICommonIntranet.Database;
namespace REA_Tracker.Models.Home
{
    
        public class ReportViewModel
        {
            public int ReportID { get; set; }
            public SearchViewModel SearchModel { get; set; }
            //input items
            public String ProductID { get; set; }
            public int Type { get; set; }
            //Display Items
            public List<dynamic> productList { get; set; }
            public List<dynamic> reportRows { get; set; }
            //Report
            //Issue Type
            public int Problems { get; set; }
            public int Enhancements { get; set; }
            public int PlannedWork { get; set; }
            //status
            public int Critical { get; set; }
            public int High { get; set; }
            public int Medium { get; set; }
            public int Low { get; set; }
            //User ID
            public int UserID { get; set; }
            //Additional Parameters for Reports
            public int Rows { get; set; }
            public string priority { get; set; }
            public string issueType { get; set; }
            public string status { get; set; }
            public string dateField { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public bool displayAll { get; set; }
            //dashboard report added
            public int? MajorVersion { get; set; }
            public int? MinorVersion { get; set; }
            public int? metric { get; set; }
            public string VersionPlanned { get; set; }
            public string VersionReleased { get; set; }
            //Chart Data
            public String Title { get; set; }
            public List<String> yMetricValues { get; set; }
            public List<String> xValues { get; set; }
            public List<String> yValues { get; set; }
            public List<dynamic> ListOfMetrics { get; set; }
          
        public int hieght { get; set; }
            public int width { get; set; }
            //chart Data
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
            
            public int CurrentProductID { get; set; }
            //Sumitted Resolved and Closed int representation
            public int SubmittedID = 1;
            public int ResolvedID = -1;
            public int ClosedID = 9;

            //History for Expanded
            public DataTable[] HistoryTable { get; set; }

            //pie charts
            public ChartViewModel.PieChart DisplayChart { get; set; }
            public dynamic MajorMinorId { get; internal set; }

        //this one
        public ReportViewModel(int userID,
                string title, int? type, string product,
                string status, string issue, string priority
                , DateTime? startDate, DateTime? endDate, string datefield, int? metric, int? MajorVersion, int? MinorVersion
                , bool displayAll
                )
            {
                this.displayAll = displayAll;
                this.dateField = datefield;
                //Required parameters
                this.UserID = userID;
                this.ProductID = product;
                //optional parameters
                this.status = status;
                this.issueType = issue;
                this.priority = priority;
                this.StartDate = startDate;
                this.EndDate = endDate;

                this.MajorVersion = MajorVersion;
                this.MinorVersion = MinorVersion;
                this.metric = metric;

                ///init call

                if (type != null)
                {// Is a dashboard report
                    this.Type = (int)type;
                    this.init((int)type);
                }
                else
                {// Is a normal report
                    if (this.StartDate != null || this.EndDate != null)
                    {
                        initWithDate();
                    }
                    this.initDynamicParameters();
                }
            }

            /*
             *Init Functions
             */
            private void initDynamicParameters()
            {//Populating chart data 
                this.LowWeight = 1;
                this.MediumWeight = 4;
                this.HighWeight = 9;
                this.CritcalWeight = 16;

                this.productList = ModelHelper.populatedProductList(this.UserID, this.CurrentProductID);
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

                //Populating the report list
                this.reportRows = this.populateByParameters();

                //Creating chart
                this.CreateChartData(Title);
                //Create title
                this.populateTitle();
            }
            private void initWithDate()
            {
                //Populating products first 
                this.LowWeight = 1;
                this.MediumWeight = 4;
                this.HighWeight = 9;
                this.CritcalWeight = 16;

                this.productList = ModelHelper.populatedProductList(this.UserID, this.CurrentProductID);
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

                this.reportRows = this.populateReportByTime();
                this.CreateChartData(Title);
            }
            private void init(int type)
            {
                //Populating products first 
                this.LowWeight = 1;
                this.MediumWeight = 4;
                this.HighWeight = 9;
                this.CritcalWeight = 16;

                this.productList = ModelHelper.populatedProductList(this.UserID, this.CurrentProductID);
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
                this.CountReports();

                //populate the reports
                /*if (this.Type != null)
                {// If it s a report form the dashboard
                    */
                if ((type > 0 && type <= 5) || (type == 17))
                {
                    //this.displayAll = true;
                    this.reportRows = this.populatedReportListByStatus();
                    this.CreateChartData(Title);
                }
                else if (type >= 6 && type <= 8)
                {
                    //this.displayAll = true;
                    this.reportRows = this.populatedReportListByIssueType();
                    this.CreateChartData(Title);
                }
                else if (type >= 9 && type <= 12)
                {
                    //this.displayAll = true;
                    this.reportRows = this.populatedReportListByPriority();
                    this.CreateChartData(Title);
                }
                else if (type == 0)
                {
                    this.displayAll = true;
                    this.reportRows = this.populateReportListByLastSubmitted();
                    this.CreateChartData(Title);
                }
                else if (this.ReportID != 0)
                {
                    this.displayAll = true;
                    this.Title = Convert.ToString(new REATrackerDB().ProcessScalarCommand("SELECT NAME FROM ST_TABULAR_REPORT WHERE REPORT_ID =" + this.ReportID));
                    this.reportRows = this.populateReportListByReportID();
                    CreateChartData(this.Title);
                }
                else if (this.Type == 13)
                {// is a metric report 
                    this.reportRows = this.populateByParameters();
                    CreateChartData(this.Title);
                }
                else if (this.Type == 14)
                {// is a history metric report (has time in it)
                    this.displayAll = true;
                    this.reportRows = populateReportByTime();
                    CreateChartData(this.Title);
                }
                else if (type == 15)
                {

                    this.reportRows = populateDashboardMetricReport(true, (int)this.metric, (int)this.MajorVersion, (int)this.MinorVersion);
                    CreateChartData(this.Title);
                }
                else if (type == 16)
                {

                    this.reportRows = populateDashboardMetricReport(false, (int)this.metric, (int)this.MajorVersion, (int)this.MinorVersion);
                    CreateChartData(this.Title);
                }


                /*}
                else 
                {//if it geneates the rows by the parameters
                    this.reportRows = this.populateByParameters();
                    CreateChartData(this.Title);
                }*/
            }

            public void populateHistory()
            {
                this.HistoryTable = new DataTable[0];
                if (this.reportRows != null)
                {
                    REATrackerDB sql = new REATrackerDB();
                    this.HistoryTable = new DataTable[reportRows.Count];
                    for (int i = 0; i < reportRows.Count; i++)
                    {
                        string cmd =
                "SELECT ST_HISTORY.TYPE, ST_HISTORY.MADE_BY, ST_HISTORY.MADE_ON, ST_HISTORY.DESCRIPTION, ST_HISTORY.FILE_ID, (ST_USERS.FIRST_NAME+' '+ST_USERS.LAST_NAME) AS MADE_BY_NAME, ST_ATTACHMENT.FILENAME, ST_RELATED.SCR_ID AS RELATED_ID, ST_TRACK.TITLE AS TITLE FROM ST_HISTORY" +
                " LEFT JOIN ST_USERS ON ST_HISTORY.MADE_BY = ST_USERS.USER_ID" +
                " LEFT JOIN ST_ATTACHMENT ON ST_ATTACHMENT.FILE_ID = ST_HISTORY.FILE_ID" +
                " LEFT JOIN ST_RELATED ON ST_RELATED.RELATED_ID= ST_HISTORY.FILE_ID" +
                " LEFT JOIN ST_TRACK ON ST_TRACK.TRACKING_ID = ST_RELATED.SCR_ID" +
                " WHERE ST_HISTORY.TRACKING_ID= " + Convert.ToString(reportRows[i].SCR) +
                " ORDER BY ST_HISTORY.HISTORY_ID DESC";
                        this.HistoryTable[i] = sql.ProcessCommand(cmd);
                    }
                }

            }

            /*
             * populate functions
             */

            private List<dynamic> populateByParameters()
            {
                List<dynamic> value = new List<dynamic>();
                string command =
    @"SELECT ST_TRACK.TRACKING_ID, ST_PRODUCT.NAME, ST_TRACK.TITLE, 
    ST_ISSUE_TYPE.NAME AS 'ISSUE_TYPE_NAME',
    ST_priority.NAME AS 'priority_NAME', 
    ST_STATUS.NAME AS 'STATUS_NAME',
    ST_TRACK.ASSIGNED_TO,
    (ST_USERS.FIRST_NAME+' '+ST_USERS.LAST_NAME) AS 'ASSIGNED_TO_NAME',
	ST_TRACK.VP_MAJOR, ST_TRACK.VP_MINOR, ST_TRACK.VP_BUILD, ST_TRACK.VP_MISC,
	ST_TRACK.VF_MAJOR, ST_TRACK.VF_MINOR, ST_TRACK.VF_BUILD, ST_TRACK.VF_MISC,
	ST_TRACK.RESOLVED_ON ,
	ST_TRACK.VR_MAJOR, ST_TRACK.VR_MINOR, ST_TRACK.VR_BUILD, ST_TRACK.VR_MISC, st_track.priority,
    ST_TRACK.SUBMITTED_ON
FROM ST_TRACK
INNER JOIN ST_PRODUCT ON ST_TRACK.PRODUCT = ST_PRODUCT.PRODUCT_ID
INNER JOIN ST_ISSUE_TYPE ON ST_TRACK.ISSUE_TYPE= ST_ISSUE_TYPE.ISSUE_ID
INNER JOIN ST_STATUS ON ST_TRACK.STATUS = ST_STATUS.STATUS_ID
INNER JOIN ST_priority ON ST_priority.ID = ST_TRACK.priority
LEFT JOIN ST_USERS ON ST_USERS.USER_ID = ST_TRACK.ASSIGNED_TO
WHERE 
ST_TRACK.PRODUCT in (" + this.ProductID + ") ";
                //Creating the Command



                if (this.status != null)
                {
                    command +=
                        "AND ST_TRACK.STATUS IN (" + this.status + ") ";
                }
                else
                {
                    if (this.displayAll)
                    {
                        command +=
                        " AND (ST_TRACK.STATUS IN (1,2,3,5,6,7,8,11))";
                    }
                    else
                    {
                        command +=
                        " AND (ST_TRACK.STATUS IN (1,2,3,5,6,7))";
                    }

                }
                if (this.issueType != null && this.displayAll)
                {
                    command +=
                        "AND ST_TRACK.ISSUE_TYPE IN (" + this.issueType + ") ";
                }
                else if (!this.displayAll)
                {
                    command +=
                       "AND ST_TRACK.ISSUE_TYPE IN (1) ";
                }
                if (this.priority != null)
                {
                    command +=
                        "AND ST_TRACK.priority IN(" + this.priority + ") ";
                }

                //Executing the Command
                DataTable dt = new REATrackerDB().ProcessCommand(command);
                //Creating the list
                int i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    value.Add(new System.Dynamic.ExpandoObject());
                    value[i].SCR = Convert.ToString(row["TRACKING_ID"]);
                    value[i].Title = Convert.ToString(row["TITLE"]);
                    value[i].Priority = Convert.ToString(row["priority_NAME"]);
                    value[i].Status = Convert.ToString(row["STATUS_NAME"]);
                    value[i].IssueType = Convert.ToString(row["ISSUE_TYPE_NAME"]);
                    value[i].AssignedTo = Convert.ToString(row["ASSIGNED_TO_NAME"]);
                    value[i].VersionFound = Convert.ToString(row["VF_MAJOR"]) + "."
                        + Convert.ToString(row["VF_MINOR"]) + "."
                        + Convert.ToString(row["VF_BUILD"]) + " "
                        + Convert.ToString(row["VF_MINOR"])
                        ;
                    value[i].VersionFoundTableValue = getVersionValue((string)value[i].VersionFound);
                    value[i].SubmittedOn = DateTime.Parse(Convert.ToString(row["SUBMITTED_ON"])).ToString("MM/dd/yy");
                    value[i].AssignedToID = Convert.ToString(row["ASSIGNED_TO"] == DBNull.Value ? "" : Convert.ToString(row["ASSIGNED_TO"]));
                    value[i].IssueTypeID = IssueTypeNameToIndex((string)value[i].IssueType);
                    value[i].PriorityID = PriorityNameToIndex((string)row["priority_NAME"]);

                    switch (IssueTypeNameToIndex((string)(row["ISSUE_TYPE_NAME"] == DBNull.Value ? ""
                        : Convert.ToString(row["ISSUE_TYPE_NAME"]))))
                    {
                        case 0://Problem
                            this.ProblemCount[PriorityNameToIndex((string)row["priority_NAME"])]++;
                            break;
                        case 1://Enhancement
                            this.EnhancementCount[PriorityNameToIndex((string)row["priority_NAME"])]++;
                            break;
                        case 2://Planned Work
                            this.PlannedWorkCount[PriorityNameToIndex((string)row["priority_NAME"])]++;
                            break;
                        default:
                            break;
                    }
                    i++;
                }
                return value;
        }
          
            //new report
            /// <summary>
            /// 
            /// </summary>
            /// <param name="code_freeze">true = code freeze, false = release</param>
            /// <param name="metric_type">matches per ISO names, 1 = critical count, 2 = high count</param>
            /// <param name="major">integer of major version</param>
            /// <param name="minor">integer of minor version</param>
            /// <returns></returns>
            private List<dynamic> populateDashboardMetricReport(bool code_freeze, int metric, int major, int minor)
            {
                List<dynamic> value = new List<dynamic>();

                //this determines the priority we want to filter on
                 string priority_check = "";

                if (metric == 1) //critical count metric (see ISO document)
               {
                    priority_check = "AND (PRIORITY = 4)";
                }
               else if(metric==2)
               {
                    priority_check = "AND (PRIORITY = 3)"; //high count metric
            }

            string command =
    @"SELECT 
    ST_TRACK.TRACKING_ID, ST_PRODUCT.NAME, ST_TRACK.TITLE, 
    ST_ISSUE_TYPE.NAME AS 'ISSUE_TYPE_NAME',
    ST_priority.NAME AS 'priority_NAME', 
    ST_STATUS.NAME AS 'STATUS_NAME',
    ST_TRACK.ASSIGNED_TO,
    (ST_USERS.FIRST_NAME+' '+ST_USERS.LAST_NAME) AS 'ASSIGNED_TO_NAME',
	ST_TRACK.VP_MAJOR, ST_TRACK.VP_MINOR, ST_TRACK.VP_BUILD, ST_TRACK.VP_MISC,
	ST_TRACK.VR_MAJOR, ST_TRACK.VR_MINOR, ST_TRACK.VR_BUILD, ST_TRACK.VR_MISC,
    (ISNULL(ST_TRACK.VF_MAJOR, '?') + '.' + ISNULL(ST_TRACK.VF_MINOR, '?') + '.' + ISNULL(ST_TRACK.VF_BUILD, '?') + ' ' + ISNULL(ST_TRACK.VF_MISC, '')) AS 'VERSION_FOUND',
    ST_TRACK.RESOLVED_ON ,
    ST_TRACK.SUBMITTED_ON,
    ST_TRACK.PRIORITY,
    ST_TRACK.MANAGERS_NOTE,
   COUNT(*) as 'Count', 
   COUNT(*) * PRIORITY * PRIORITY AS 'Metric' 
  

FROM ST_TRACK
INNER JOIN ST_PRODUCT ON ST_TRACK.PRODUCT = ST_PRODUCT.PRODUCT_ID
INNER JOIN ST_ISSUE_TYPE ON ST_TRACK.ISSUE_TYPE= ST_ISSUE_TYPE.ISSUE_ID
INNER JOIN ST_STATUS ON ST_TRACK.STATUS = ST_STATUS.STATUS_ID
INNER JOIN ST_priority ON ST_priority.ID = ST_TRACK.PRIORITY
LEFT JOIN ST_USERS ON ST_USERS.USER_ID = ST_TRACK.ASSIGNED_TO

WHERE 
    ST_TRACK.PRODUCT in (" + this.ProductID + @") AND (ISSUE_TYPE = 1) AND (STATUS IN (1,2,5,6,7)) " + priority_check + @" 

AND
((VP_MAJOR IS NULL AND VP_MINOR IS NULL AND VP_BUILD IS NULL AND VP_MISC IS NULL)
     OR" +
               "(VP_MAJOR = " + this.MajorVersion + " AND VP_MINOR = " + this.MinorVersion + "))" + @"
GROUP BY 
    ST_TRACK.TRACKING_ID,ST_PRODUCT.NAME, ST_TRACK.TITLE, ST_ISSUE_TYPE.NAME, ST_priority.NAME, ST_STATUS.NAME,ST_TRACK.ASSIGNED_TO, ST_USERS.FIRST_NAME, 
    ST_USERS.LAST_NAME,ST_TRACK.VP_MAJOR, ST_TRACK.VP_MINOR, ST_TRACK.VP_BUILD, ST_TRACK.VP_MISC,ST_TRACK.RESOLVED_ON, ST_TRACK.SUBMITTED_ON, ST_TRACK.PRIORITY,   
    ST_TRACK.VF_MAJOR, ST_TRACK.VF_MINOR, ST_TRACK.VF_BUILD, ST_TRACK.VF_MISC,ST_TRACK.VR_MAJOR, ST_TRACK.VR_MINOR, ST_TRACK.VR_BUILD,ST_TRACK.VR_MISC,
    ST_TRACK.MANAGERS_NOTE
";


                //Executing the Command

                DataTable dt = new REATrackerDB().ProcessCommand(command);
                
            //Creating the list
            int i = 0;
            foreach (DataRow row in dt.Rows)
                {
                    value.Add(new System.Dynamic.ExpandoObject());
                    value[i].SCR = Convert.ToString(row["TRACKING_ID"]);
                    value[i].Title = Convert.ToString(row["TITLE"]);
                    value[i].Priority = Convert.ToString(row["priority_NAME"]);
                    value[i].Status = Convert.ToString(row["STATUS_NAME"]);
                    value[i].IssueType = Convert.ToString(row["ISSUE_TYPE_NAME"]);
                    value[i].AssignedTo = Convert.ToString(row["ASSIGNED_TO_NAME"]);
                    value[i].VersionFound = Convert.ToString(row["VERSION_FOUND"]);
                    value[i].VersionFoundTableValue = getVersionValue((string)value[i].VersionFound);
                    value[i].SubmittedOn = DateTime.Parse(Convert.ToString(row["SUBMITTED_ON"])).ToString("MM/dd/yy");
                    value[i].AssignedToID = Convert.ToString(row["ASSIGNED_TO"] == DBNull.Value ? "" : Convert.ToString(row["ASSIGNED_TO"]));
                    value[i].IssueTypeID = IssueTypeNameToIndex((string)value[i].IssueType);
                    value[i].PriorityID = PriorityNameToIndex((string)row["priority_NAME"]);
                    value[i].VersionPlanned = arrangeVersion(Convert.ToString(row["VP_MAJOR"]),
                                                                Convert.ToString(row["VP_MINOR"]),
                                                                Convert.ToString(row["VP_BUILD"]),
                                                                Convert.ToString(row["VP_MISC"]));
                    value[i].VersionPlannedTableValue = getVersionValue((string)value[i].VersionPlanned);

                    value[i].VersionRelease = arrangeVersion(
                        Convert.ToString(row["VR_MAJOR"]),
                        Convert.ToString(row["VR_MINOR"]),
                        Convert.ToString(row["VR_BUILD"]),
                        Convert.ToString(row["VR_MISC"]));
                    value[i].VersionReleaseTableValue = getVersionValue((string)value[i].VersionRelease);
                    


                    //     this.ListOfMetrics[i].MajorMinorDisplay = Convert.ToString(this.ListOfMetrics[i].MajorVersion) + "." + Convert.ToString(this.ListOfMetrics[i].MinorVersion) + ".X";
                    //   this.ListOfMetrics[i].MajorMinorId = Convert.ToString(this.ListOfMetrics[i].MajorVersion) + "_" + Convert.ToString(this.ListOfMetrics[i].MinorVersion);
                    switch (IssueTypeNameToIndex((string)(row["ISSUE_TYPE_NAME"] == DBNull.Value ? ""
                           : Convert.ToString(row["ISSUE_TYPE_NAME"]))))
                    {
                        case 0://Problem
                            this.ProblemCount[PriorityNameToIndex((string)row["priority_NAME"])]++;
                            break;
                        case 1://Enhancement
                            this.EnhancementCount[PriorityNameToIndex((string)row["priority_NAME"])]++;
                            break;
                        case 2://Planned Work
                            this.PlannedWorkCount[PriorityNameToIndex((string)row["priority_NAME"])]++;
                            break;
                        default:
                            break;
                    }
                value[i].ManagersNote = Convert.ToString(row["MANAGERS_NOTE"]);
                i++;
                }
                return value;

            }



            private void populateTitle()
            {
                REATrackerDB sql = new REATrackerDB();
                //get the product name
                this.Title += Convert.ToString(sql.ProcessScalarCommand(
                    "SELECT ST_PRODUCT.NAME FROM ST_PRODUCT WHERE PRODUCT_ID IN ( " + this.ProductID + ")"
                    ));
                //get the status type
                if (this.status != null)
                {
                    string name =
                        Convert.ToString(
    sql.ProcessScalarCommand(
    "SELECT ST_STATUS.NAME FROM ST_STATUS WHERE ST_STATUS.STATUS_ID IN (" + this.status + ")"
                            ));

                    if (!string.IsNullOrEmpty(name))
                    {
                        this.Title +=
                        ", " + name
                        ;
                    }
                }
                //get the issue type
                if (this.issueType != null)
                {
                    string name =
                        Convert.ToString(
    sql.ProcessScalarCommand(
    "SELECT ST_ISSUE_TYPE.NAME FROM ST_ISSUE_TYPE WHERE ST_ISSUE_TYPE.ISSUE_ID IN (" + this.issueType + ")"
                            )
                        );
                    if (!string.IsNullOrEmpty(name))
                    {
                        this.Title +=
                        ", " + name
                        ;
                    }
                }
                //get the priority
                if (this.priority != null)
                {
                    string name =
                        Convert.ToString(
    sql.ProcessScalarCommand(
    "SELECT ST_priority.NAME FROM ST_priority WHERE ST_priority.ID IN (" + this.priority + ")"
                              )
                          );
                    if (!string.IsNullOrEmpty(name))
                    {
                        this.Title +=
                        ", " + name
                        ;
                    }
                }
            }

            private List<dynamic> populateReportListByReportID()
            {
                this.SearchModel = new SearchViewModel(this.ReportID, this.UserID, this.CurrentProductID);
                List<dynamic> Lister = new List<dynamic>();
                this.SearchModel.ProcessSearch();
                DataTable dt;
                if (String.IsNullOrEmpty(this.SearchModel.RawSQL))
                {
                    String Command =
                    "SELECT ST_TRACK.TRACKING_ID, ST_PRODUCT.NAME, ST_TRACK.TITLE, ST_ISSUE_TYPE.NAME, ST_priority.NAME, ST_STATUS.NAME, ST_TRACK.ASSIGNED_TO, "
                    + "ST_TRACK.VP_MAJOR, ST_TRACK.VP_MINOR, ST_TRACK.VP_BUILD, ST_TRACK.VP_MISC,"
                    + "ST_TRACK.VF_MAJOR, ST_TRACK.VF_MINOR, ST_TRACK.VF_BUILD, ST_TRACK.VF_MISC,"
                    + "ST_TRACK.RESOLVED_ON ,                                                    "
                    + "ST_TRACK.VR_MAJOR, ST_TRACK.VR_MINOR, ST_TRACK.VR_BUILD, ST_TRACK.VR_MISC, ST_TRACK.priority "
                    + "FROM ST_TRACK                                                             "
                    + "INNER JOIN ST_PRODUCT ON ST_TRACK.PRODUCT = ST_PRODUCT.PRODUCT_ID         "
                    + "INNER JOIN ST_ISSUE_TYPE ON ST_TRACK.ISSUE_TYPE= ST_ISSUE_TYPE.ISSUE_ID   "
                    + "INNER JOIN ST_STATUS ON ST_TRACK.STATUS = ST_STATUS.STATUS_ID             "
                    + "INNER JOIN ST_priority ON ST_priority.ID = ST_TRACK.priority              ";
                    if (this.SearchModel.WhereSQLRows.Contains("WHERE"))
                    {
                        Command += " WHERE "
                        + this.SearchModel.WhereSQLRows.Substring(this.SearchModel.WhereSQLRows.IndexOf("WHERE") + 5);
                    }
                    dt = new REATrackerDB().ProcessCommand(Command);
                }
                else
                {
                    dt = new REATrackerDB().ProcessCommand(this.SearchModel.RawSQL);
                }
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    Lister.Add(new System.Dynamic.ExpandoObject());
                    Lister[i].SCR = Convert.ToString(row[0] == DBNull.Value ? "" : row[0]);
                    Lister[i].Product = Convert.ToString(row[1] == DBNull.Value ? "" : row[1]);
                    Lister[i].Title = Convert.ToString(row[2] == DBNull.Value ? "" : row[2]);
                    Lister[i].IssueType = Convert.ToString(row[3] == DBNull.Value ? "" : row[3]);
                    Lister[i].Priority = Convert.ToString(row[4] == DBNull.Value ? "" : row[4]);
                    Lister[i].PriorityID = Convert.ToString(row["priority"]);
                    Lister[i].Status = Convert.ToString(row[5] == DBNull.Value ? "" : row[5]);
                    Lister[i].AssignedTo = getAssignedTo(Convert.ToInt32(row[0] == DBNull.Value ? "" : row[0]));
                    Lister[i].AssignedToID = Convert.ToString(row[6] == DBNull.Value ? "" : row[6]);
                    Lister[i].VersionPlanned = arrangeVersion(Convert.ToString(row[7] == DBNull.Value ? "" : row[7]),
                                                                Convert.ToString(row[8] == DBNull.Value ? "" : row[8]),
                                                                Convert.ToString(row[9] == DBNull.Value ? "" : row[9]),
                                                                Convert.ToString(row[10] == DBNull.Value ? "" : row[10]));
                    Lister[i].VersionPlannedTableValue = getVersionValue((string)Lister[i].VersionPlanned);
                    Lister[i].VersionFound = arrangeVersion(Convert.ToString(row[11] == DBNull.Value ? "" : row[11]),
                                                                Convert.ToString(row[12] == DBNull.Value ? "" : row[12]),
                                                                Convert.ToString(row[13] == DBNull.Value ? "" : row[13]),
                                                                Convert.ToString(row[14] == DBNull.Value ? "" : row[14]));
                    Lister[i].VersionFoundTableValue = getVersionValue((string)Lister[i].VersionFound);
                    Lister[i].ResolvedOn = Convert.ToString(row[15] == DBNull.Value ? "" : row[15]);
                    Lister[i].VersionRelease = arrangeVersion(Convert.ToString(row[16] == DBNull.Value ? "" : row[16]),
                                                                Convert.ToString(row[17] == DBNull.Value ? "" : row[17]),
                                                                Convert.ToString(row[18] == DBNull.Value ? "" : row[18]),
                                                                Convert.ToString(row[19] == DBNull.Value ? "" : row[19]));
                    Lister[i].VersionReleasedTableValue = getVersionValue((string)Lister[i].VersionRelease);
                    switch (IssueTypeNameToIndex((string)(row[3] == DBNull.Value ? "" : row[3])))
                    {
                        case 0://Problem
                            this.ProblemCount[PriorityNameToIndex((string)row[4])]++;
                            break;
                        case 1://Enhancement
                            this.EnhancementCount[PriorityNameToIndex((string)row[4])]++;
                            break;
                        case 2://Planned Work
                            this.PlannedWorkCount[PriorityNameToIndex((string)row[4])]++;
                            break;
                        default:
                            break;
                    }
                    i++;
                }
                return Lister;

            }

            private void CountReports()
            {
                REATrackerDB sql = new REATrackerDB();
                int temp = 0;
                if (!string.IsNullOrEmpty(this.ProductID))
                {
                    string[] producttemp = this.ProductID.Split(',');
                    Int32.TryParse(producttemp[0], out temp);
                }
                if (temp != 0)
                {
                    DataTable dtp = sql.GetPriorityMetrics(temp); //Three rows of one
                    DataTable dti = sql.GetIssueMetrics(temp);    //Three rows of one
                    int i = 0;
                    foreach (System.Data.DataRow row in dti.Rows)
                    {
                        if (int.TryParse(row["ISSUE_TYPE"].ToString(), out i))
                        {
                            if (i == 1)
                            {
                                this.Problems = Convert.ToInt32(row[2]);
                            }
                            else if (i == 2)
                            {
                                this.Enhancements = Convert.ToInt32(row[2]);
                            }
                            else if (i == 3)
                            {
                                this.PlannedWork = Convert.ToInt32(row[2]);
                            }
                        }
                    }
                    if (dtp.Rows.Count == 4)
                    {
                        this.Low = Convert.ToInt32(dtp.Rows[3][2]);
                        this.Medium = Convert.ToInt32(dtp.Rows[2][2]);
                        this.High = Convert.ToInt32(dtp.Rows[1][2]);
                        this.Critical = Convert.ToInt32(dtp.Rows[0][2]);
                    }
                }
            }

            private List<dynamic> populatedReportListByStatus()
            {
                int temp = 0;
                List<dynamic> List = new List<dynamic>();
                if (!string.IsNullOrEmpty(this.ProductID))
                {
                    string[] producttemp = this.ProductID.Split(',');
                    Int32.TryParse(producttemp[0], out temp);
                }
                if (temp != 0)
                {

                    REATrackerDB sql = new REATrackerDB();
                    DataTable dt = sql.GetREAByStatus(temp, this.Type);
                    int i = 0;
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        List.Add(new System.Dynamic.ExpandoObject());
                        List[i].SCR = Convert.ToString(row[0] == DBNull.Value ? "" : row[0]);
                        List[i].Product = Convert.ToString(row[1] == DBNull.Value ? "" : row[1]);
                        List[i].Title = Convert.ToString(row[2] == DBNull.Value ? "" : row[2]);
                        List[i].IssueType = Convert.ToString(row[3] == DBNull.Value ? "" : row[3]);
                        List[i].IssueTypeID = IssueTypeNameToIndex((string)List[i].IssueType);
                        List[i].Priority = Convert.ToString(row[4] == DBNull.Value ? "" : row[4]);
                        List[i].PriorityID = Convert.ToString(row["priority"]);
                        List[i].Status = Convert.ToString(row[5] == DBNull.Value ? "" : row[5]);
                        List[i].AssignedTo = getAssignedTo(Convert.ToInt32(row[0] == DBNull.Value ? "" : row[0]));
                        List[i].AssignedToID = Convert.ToString(row[6] == DBNull.Value ? "" : row[6]);
                        List[i].VersionPlanned = arrangeVersion(Convert.ToString(row[7] == DBNull.Value ? "" : row[7]),
                                                                    Convert.ToString(row[8] == DBNull.Value ? "" : row[8]),
                                                                    Convert.ToString(row[9] == DBNull.Value ? "" : row[9]),
                                                                    Convert.ToString(row[10] == DBNull.Value ? "" : row[10]));
                        List[i].VersionPlannedTableValue = getVersionValue((string)List[i].VersionPlanned);

                        List[i].VersionFound = arrangeVersion(Convert.ToString(row[11] == DBNull.Value ? "" : row[11]),
                                                                    Convert.ToString(row[12] == DBNull.Value ? "" : row[12]),
                                                                    Convert.ToString(row[13] == DBNull.Value ? "" : row[13]),
                                                                    Convert.ToString(row[14] == DBNull.Value ? "" : row[14]));
                        List[i].VersionFoundTableValue = getVersionValue((string)List[i].VersionFound);

                        List[i].ResolvedOn = Convert.ToString(row[15] == DBNull.Value ? "" : row[15]);
                        List[i].VersionRelease = arrangeVersion(Convert.ToString(row[16] == DBNull.Value ? "" : row[16]),
                                                                    Convert.ToString(row[17] == DBNull.Value ? "" : row[17]),
                                                                    Convert.ToString(row[18] == DBNull.Value ? "" : row[18]),
                                                                    Convert.ToString(row[19] == DBNull.Value ? "" : row[19]));
                        List[i].VersionReleaseTableValue = getVersionValue((string)List[i].VersionRelease);
                        List[i].ManagersNote = Convert.ToString(row["MANAGERS_NOTE"] == DBNull.Value ? "" : row["MANAGERS_NOTE"]);
                    switch (IssueTypeNameToIndex((string)(row[3] == DBNull.Value ? "" : row[3])))
                        {
                            case 0://Problem
                                this.ProblemCount[PriorityNameToIndex((string)row[4])]++;
                                break;
                            case 1://Enhancement
                                this.EnhancementCount[PriorityNameToIndex((string)row[4])]++;
                                break;
                            case 2://Planned Work
                                this.PlannedWorkCount[PriorityNameToIndex((string)row[4])]++;
                                break;
                            default:
                                break;
                        }
                        i++;
                    }
                }
                return List;
            }

            private List<dynamic> populatedReportListByIssueType()
            {
                int temp = 0;
                List<dynamic> List = new List<dynamic>();
                if (!string.IsNullOrEmpty(this.ProductID))
                {
                    string[] producttemp = this.ProductID.Split(',');
                    Int32.TryParse(producttemp[0], out temp);
                }
                if (temp != 0)
                {
                    REATrackerDB sql = new REATrackerDB();
                    DataTable dt = sql.GetREAByIssueType(temp, this.Type - 5);
                    int i = 0;
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        List.Add(new System.Dynamic.ExpandoObject());
                        List[i].SCR = Convert.ToString(row[0] == DBNull.Value ? "" : row[0]);
                        List[i].Product = Convert.ToString(row[1] == DBNull.Value ? "" : row[1]);
                        List[i].Title = Convert.ToString(row[2] == DBNull.Value ? "" : row[2]);
                        List[i].IssueType = Convert.ToString(row[3] == DBNull.Value ? "" : row[3]);
                        List[i].IssueTypeID = IssueTypeNameToIndex((string)List[i].IssueType);
                        List[i].Priority = Convert.ToString(row[4] == DBNull.Value ? "" : row[4]);
                        List[i].PriorityID = Convert.ToString(row["priority"]);
                        List[i].Status = Convert.ToString(row[5] == DBNull.Value ? "" : row[5]);
                        List[i].AssignedTo = getAssignedTo(Convert.ToInt32(row[0] == DBNull.Value ? "" : row[0]));
                        List[i].AssignedToID = Convert.ToString(row[6] == DBNull.Value ? "" : row[6]);
                        List[i].VersionPlanned = arrangeVersion(Convert.ToString(row[7] == DBNull.Value ? "" : row[7]),
                                                                    Convert.ToString(row[8] == DBNull.Value ? "" : row[8]),
                                                                    Convert.ToString(row[9] == DBNull.Value ? "" : row[9]),
                                                                    Convert.ToString(row[10] == DBNull.Value ? "" : row[10]));
                        List[i].VersionPlannedTableValue = getVersionValue((string)List[i].VersionPlanned);

                        List[i].VersionFound = arrangeVersion(Convert.ToString(row[11] == DBNull.Value ? "" : row[11]),
                                                                    Convert.ToString(row[12] == DBNull.Value ? "" : row[12]),
                                                                    Convert.ToString(row[13] == DBNull.Value ? "" : row[13]),
                                                                    Convert.ToString(row[14] == DBNull.Value ? "" : row[14]));
                        List[i].VersionFoundTableValue = getVersionValue((string)List[i].VersionFound);

                        List[i].ResolvedOn = Convert.ToString(row[15] == DBNull.Value ? "" : row[15]);
                        List[i].VersionRelease = arrangeVersion(Convert.ToString(row[16] == DBNull.Value ? "" : row[16]),
                                                                    Convert.ToString(row[17] == DBNull.Value ? "" : row[17]),
                                                                    Convert.ToString(row[18] == DBNull.Value ? "" : row[18]),
                                                                    Convert.ToString(row[19] == DBNull.Value ? "" : row[19]));
                        List[i].VersionReleaseTableValue = getVersionValue((string)List[i].VersionRelease);
                        List[i].ManagersNote = Convert.ToString(row["MANAGERS_NOTE"] == DBNull.Value ? "" : row["MANAGERS_NOTE"]);
                        switch (IssueTypeNameToIndex((string)(row[3] == DBNull.Value ? "" : row[3])))
                        {
                            case 0://Problem
                                this.ProblemCount[PriorityNameToIndex((string)row[4])]++;
                                break;
                            case 1://Enhancement
                                this.EnhancementCount[PriorityNameToIndex((string)row[4])]++;
                                break;
                            case 2://Planned Work
                                this.PlannedWorkCount[PriorityNameToIndex((string)row[4])]++;
                                break;
                            default:
                                break;
                        }
                        i++;
                    }
                }
                return List;
            }

            private List<dynamic> populatedReportListByPriority()
            {
                int temp = 0;
                List<dynamic> List = new List<dynamic>();
                if (!string.IsNullOrEmpty(this.ProductID))
                {
                    string[] producttemp = this.ProductID.Split(',');
                    Int32.TryParse(producttemp[0], out temp);
                }
                if (temp != 0)
                {
                    REATrackerDB sql = new REATrackerDB();
                    DataTable dt = sql.GetREAByPriority(temp, this.Type - 8, this.displayAll);
                    int i = 0;
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        List.Add(new System.Dynamic.ExpandoObject());
                        List[i].SCR = Convert.ToString(row[0] == DBNull.Value ? "" : row[0]);
                        List[i].Product = Convert.ToString(row[1] == DBNull.Value ? "" : row[1]);
                        List[i].Title = Convert.ToString(row[2] == DBNull.Value ? "" : row[2]);
                        List[i].IssueType = Convert.ToString(row[3] == DBNull.Value ? "" : row[3]);
                        List[i].Priority = Convert.ToString(row[4] == DBNull.Value ? "" : row[4]);
                        List[i].PriorityID = Convert.ToString(row["priority"]);
                        List[i].Status = Convert.ToString(row[5] == DBNull.Value ? "" : row[5]);
                        List[i].AssignedTo = getAssignedTo(Convert.ToInt32(row[0] == DBNull.Value ? "" : row[0]));
                        List[i].AssignedToID = Convert.ToString(row[6] == DBNull.Value ? "" : row[6]);
                        List[i].VersionPlanned = arrangeVersion(Convert.ToString(row[7] == DBNull.Value ? "" : row[7]),
                                                                    Convert.ToString(row[8] == DBNull.Value ? "" : row[8]),
                                                                    Convert.ToString(row[9] == DBNull.Value ? "" : row[9]),
                                                                    Convert.ToString(row[10] == DBNull.Value ? "" : row[10]));
                        List[i].VersionPlannedTableValue = getVersionValue((string)List[i].VersionPlanned);
                        List[i].VersionFound = arrangeVersion(Convert.ToString(row[11] == DBNull.Value ? "" : row[11]),
                                                                    Convert.ToString(row[12] == DBNull.Value ? "" : row[12]),
                                                                    Convert.ToString(row[13] == DBNull.Value ? "" : row[13]),
                                                                    Convert.ToString(row[14] == DBNull.Value ? "" : row[14]));
                        List[i].VersionFoundTableValue = getVersionValue((string)List[i].VersionFound);
                        List[i].ResolvedOn = Convert.ToString(row[15] == DBNull.Value ? "" : row[15]);
                        List[i].VersionRelease = arrangeVersion(Convert.ToString(row[16] == DBNull.Value ? "" : row[16]),
                                                                    Convert.ToString(row[17] == DBNull.Value ? "" : row[17]),
                                                                    Convert.ToString(row[18] == DBNull.Value ? "" : row[18]),
                                                                    Convert.ToString(row[19] == DBNull.Value ? "" : row[19]));
                        List[i].IssueTypeID = IssueTypeNameToIndex((string)List[i].IssueType);
                        List[i].VersionReleaseTableValue= getVersionValue((string)List[i].VersionRelease);
                        List[i].ManagersNote = Convert.ToString(row["MANAGERS_NOTE"] == DBNull.Value ? "" : row["MANAGERS_NOTE"]);
                        switch (IssueTypeNameToIndex((string)(row[3] == DBNull.Value ? "" : row[3])))
                        {
                            case 0://Problem
                                this.ProblemCount[PriorityNameToIndex((string)row[4])]++;
                                break;
                            case 1://Enhancement
                                this.EnhancementCount[PriorityNameToIndex((string)row[4])]++;
                                break;
                            case 2://Planned Work
                                this.PlannedWorkCount[PriorityNameToIndex((string)row[4])]++;
                                break;
                            default:
                                break;
                        }
                        i++;
                    }
                }
                return List;
            }

            private List<dynamic> populateReportListByLastSubmitted()
            {
                int temp = 0;
                List<dynamic> List = new List<dynamic>();
                if (!string.IsNullOrEmpty(this.ProductID))
                {
                    string[] producttemp = this.ProductID.Split(',');
                    Int32.TryParse(producttemp[0], out temp);
                }
                if (temp != 0)
                {
                    REATrackerDB sql = new REATrackerDB();
                    int count;
                    if (this.Rows != -1)
                    {
                        count = this.Rows;
                    }
                    else
                    {
                        String cmd = "SELECT SUBMITTED_DISPLAY FROM ST_USERS WHERE ST_USERS.USER_ID = " + this.UserID;
                        count = Convert.ToInt32(sql.ProcessScalarCommand(cmd));
                    }
                    DataTable dt = sql.GetLatestSubmittedByCount(temp, count);
                    int i = 0;
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        List.Add(new System.Dynamic.ExpandoObject());
                        List[i].SCR = row["TRACKING_ID"].ToString();
                        List[i].Title = row["TITLE"].ToString();
                        List[i].Priority = row["priority_NAME"].ToString();
                        List[i].Status = row["STATUS_NAME"].ToString();
                        List[i].IssueType = row["ISSUE_TYPE_NAME"].ToString();
                        List[i].AssignedTo = row["ASSIGNED_TO"].ToString();
                        List[i].VersionFound = row["VERSION_FOUND"].ToString();
                        List[i].VersionFoundTableValue = getVersionValue((string)List[i].VersionFound);
                        List[i].SubmittedOn = DateTime.Parse(row["SUBMITTED_ON"].ToString()).ToString("MM/dd/yy");
                        List[i].AssignedToID = Convert.ToInt32(row["USER_ID"]);

                        List[i].IssueTypeID = IssueTypeNameToIndex((string)List[i].IssueType);
                        List[i].PriorityID = PriorityNameToIndex((string)row["priorityY_NAME"]);
                        List[i].ManagersNote = Convert.ToString(row["MANGERS_NOTE"] == DBNull.Value ? "" : row["MANGERS_NOTE"]);
                        switch (IssueTypeNameToIndex((string)(row["ISSUE_TYPE_NAME"] == DBNull.Value ? "": row["ISSUE_TYPE_NAME"])))
                        {
                            case 0://Problem
                                this.ProblemCount[PriorityNameToIndex((string)row["priority_NAME"])]++;
                                break;
                            case 1://Enhancement
                                this.EnhancementCount[PriorityNameToIndex((string)row["priority_NAME"])]++;
                                break;
                            case 2://Planned Work
                                this.PlannedWorkCount[PriorityNameToIndex((string)row["priority_NAME"])]++;
                                break;
                            default:
                                break;
                        }
                        i++;
                    }
                }
                return List;
            }

            private string getVersionValue(string Version)
            {
                string value = "";
                if (!string.IsNullOrEmpty(Version))
                {
                    string[] VersionValues = Version.Split(new[] { '.', ' ' });
                    for (int i = 0; i < VersionValues.Count(); i++)
                    {
                        int versionammount = -1;
                        bool isNum = Int32.TryParse(VersionValues[i], out versionammount);
                        if (i < 3
                            && isNum)
                        {
                            if (i != 0)
                            {
                                value += ".";
                            }
                            if (versionammount < 10 && versionammount != -1)
                            {
                                value += "0" + VersionValues[i];
                            }
                            else
                            {
                                value += VersionValues[i];
                            }
                        }
                        else if (i == 3)
                        {
                            value += " ";
                            value += VersionValues[i];
                        }
                        else
                        {
                            value += VersionValues[i];
                        }
                    }
                }
                return value;
            }

            private List<dynamic> populateReportByTime()
            {
                List<dynamic> List = new List<dynamic>();
                REATrackerDB sql = new REATrackerDB();

                String command =
    @"SELECT ST_TRACK.TRACKING_ID, ST_PRODUCT.NAME, ST_TRACK.TITLE, 
        ST_ISSUE_TYPE.NAME AS 'ISSUE_TYPE_NAME',
        ST_priority.NAME AS 'priority_NAME', 
        ST_STATUS.NAME AS 'STATUS_NAME',
        ST_TRACK.ASSIGNED_TO,
        (ST_USERS.FIRST_NAME+' '+ST_USERS.LAST_NAME) AS 'ASSIGNED_TO_NAME',
		ST_TRACK.VP_MAJOR, ST_TRACK.VP_MINOR, ST_TRACK.VP_BUILD, ST_TRACK.VP_MISC,
		ST_TRACK.VF_MAJOR, ST_TRACK.VF_MINOR, ST_TRACK.VF_BUILD, ST_TRACK.VF_MISC,
		ST_TRACK.RESOLVED_ON ,
		ST_TRACK.VR_MAJOR, ST_TRACK.VR_MINOR, ST_TRACK.VR_BUILD, ST_TRACK.VR_MISC, st_track.priority,
        ST_TRACK.SUBMITTED_ON
	FROM ST_TRACK
	INNER JOIN ST_PRODUCT ON ST_TRACK.PRODUCT = ST_PRODUCT.PRODUCT_ID
	INNER JOIN ST_ISSUE_TYPE ON ST_TRACK.ISSUE_TYPE= ST_ISSUE_TYPE.ISSUE_ID
	INNER JOIN ST_STATUS ON ST_TRACK.STATUS = ST_STATUS.STATUS_ID
	INNER JOIN ST_priority ON ST_priority.ID = ST_TRACK.priority
    LEFT JOIN ST_USERS ON ST_USERS.USER_ID = ST_TRACK.ASSIGNED_TO
    WHERE 
            ST_TRACK.PRODUCT =" + this.ProductID;
                if (!string.IsNullOrEmpty(this.issueType))
                {
                    command += " AND ST_TRACK.ISSUE_TYPE = " + this.issueType;
                }
                if (!string.IsNullOrEmpty(this.dateField))
                {
                    if (this.dateField.ToLower().Contains("submit"))
                    {
                        this.Title += "Submitted On";
                    }
                    else if (this.dateField.ToLower().Contains("resolve"))
                    {
                        this.Title += "Resolved On";
                    }
                    else if (this.dateField.ToLower().Contains("close"))
                    {
                        this.Title += "Closed On";
                    }
                    if (this.StartDate != null)
                    {
                        command += " AND " + this.dateField + " >= '" + this.StartDate + "'";
                    }
                    if (this.EndDate != null)
                    {
                        ((DateTime)this.EndDate).AddHours(23);
                        ((DateTime)this.EndDate).AddMinutes(59);
                        ((DateTime)this.EndDate).AddSeconds(59);
                        command += " AND " + this.dateField + " <= '" + this.EndDate + "'";
                    }
                }
                /*
                if(this.Type== this.SubmittedID)// Submitted
                {
                    this.Title += "Submitted";

                }
                else if (this.Type == this.ResolvedID)// Resolved
                {
                    this.Title += "Resolved ";
                    if (this.StartDate != null)
                    {
                        command += " AND RESOLVED_ON >= '" + this.StartDate + "'";
                    }
                    if (this.EndDate != null)
                    {
                        command += " AND RESOLVED_ON <= '" + this.EndDate + "'";
                    }

                }
                else if (this.Type == this.ClosedID)//Closed
                {
                    this.Title += "Closed ";
                    if (this.StartDate != null)
                    {
                        command += " AND CLOSED_ON >= '" + this.StartDate + "'";
                    }
                    if (this.EndDate != null)
                    {
                        command += " AND CLOSED_ON <= '" + this.EndDate + "'";
                    }
                }
                 */
                if (!string.IsNullOrEmpty(this.priority))
                {
                    command += " AND ST_TRACK.priority = " + this.priority;
                }

                //populating the temp list to return
                DataTable dt = sql.ProcessCommand(command);
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    List.Add(new System.Dynamic.ExpandoObject());
                    List[i].SCR = Convert.ToString(row["TRACKING_ID"]);
                    List[i].Title = Convert.ToString(row["TITLE"]);
                    List[i].Priority = Convert.ToString(row["priority_NAME"]);
                    List[i].Status = Convert.ToString(row["STATUS_NAME"]);
                    List[i].IssueType = Convert.ToString(row["ISSUE_TYPE_NAME"]);
                    List[i].AssignedTo = Convert.ToString(row["ASSIGNED_TO_NAME"]);
                    List[i].VersionFound = Convert.ToString(row["VF_MAJOR"]) + "."
                        + Convert.ToString(row["VF_MINOR"]) + "."
                        + Convert.ToString(row["VF_BUILD"]) + " "
                        + Convert.ToString(row["VF_MINOR"])
                        ;
                    List[i].VersionFoundTableValue = getVersionValue((string)List[i].VersionFound);
                    List[i].SubmittedOn = DateTime.Parse(Convert.ToString(row["SUBMITTED_ON"])).ToString("MM/dd/yy");
                    List[i].AssignedToID = Convert.ToString(row["ASSIGNED_TO"] == DBNull.Value ? "" : Convert.ToString(row["ASSIGNED_TO"]));
                    List[i].IssueTypeID = IssueTypeNameToIndex((string)List[i].IssueType);
                    List[i].PriorityID = PriorityNameToIndex((string)row["priority_NAME"]);

                    switch (IssueTypeNameToIndex((string)(row["ISSUE_TYPE_NAME"] == DBNull.Value ? ""
                        : Convert.ToString(row["ISSUE_TYPE_NAME"]))))
                    {
                        case 0://Problem
                            this.ProblemCount[PriorityNameToIndex((string)row["priority_NAME"])]++;
                            break;
                        case 1://Enhancement
                            this.EnhancementCount[PriorityNameToIndex((string)row["priority_NAME"])]++;
                            break;
                        case 2://Planned Work
                            this.PlannedWorkCount[PriorityNameToIndex((string)row["priority_NAME"])]++;
                            break;
                        default:
                            break;
                    }
                    i++;
                }
                //Create Title

                if (!string.IsNullOrEmpty(issueType))
                {
                    if (!string.IsNullOrEmpty(this.Title))
                    {
                        this.Title += ", ";
                    }
                    this.Title += Convert.ToString(sql.ProcessScalarCommand(
                    "SELECT ST_ISSUE_TYPE.NAME FROM ST_ISSUE_TYPE WHERE ST_ISSUE_TYPE.ISSUE_ID ="
                    + this.issueType)
                    ); // ISSUE TYPE 
                }

                if (!string.IsNullOrEmpty(this.priority))
                {
                    if (!string.IsNullOrEmpty(this.Title))
                    {
                        this.Title += ", ";
                    }
                    this.Title += Convert.ToString(sql.ProcessScalarCommand(
                    "SELECT ST_priority.NAME FROM ST_priority WHERE ST_priority.ID ="
                    + this.priority)
                    ); // priority
                }
                if (displayAll)
                {
                    this.Title += " of All SCRs";
                }
                else
                {
                    this.Title += " of Problem SCRs";
                }
                this.Title += " SCRs for " + Convert.ToString(
                    sql.ProcessScalarCommand(
                        " SELECT ST_PRODUCT.NAME FROM ST_PRODUCT WHERE PRODUCT_ID ="
                        + this.ProductID)
                        );//Product

                if (this.StartDate != null)
                {
                    this.Title += ". Starting on " + ((DateTime)this.StartDate).ToString("MM/dd/yy");
                }
                if (this.EndDate != null)
                {
                    this.Title += " and ending on " + ((DateTime)this.EndDate).ToString("MM/dd/yy");
                }
                return List;
            }

            /*
             Helper Functions
             */

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

            private string arrangeVersion(String major, String minor, String build, String misc)
            {
                string version = "";
                if (major != "")
                {
                    version += major;
                }
                else
                {
                    version += "?";
                }
                if (minor != "")
                {
                    version += ("." + minor);
                }
                else
                {
                    version += ".?";
                }
                if (build != "")
                {
                    version += ("." + build);
                }
                else
                {
                    version += ".?";
                }
                if (misc != "")
                {
                    version += (" " + misc);
                }
                return version;
            }

            private string getAssignedTo(int ID)
            {
                string fullname = "";
                string cmd = "SELECT ST_USERS.FIRST_NAME, ST_USERS.LAST_NAME FROM ST_USERS INNER JOIN ST_TRACK ON ST_TRACK.ASSIGNED_TO = ST_USERS.USER_ID " +
                    "WHERE ST_TRACK.TRACKING_ID = " + Convert.ToString(ID);
                REATrackerDB _sql = new REATrackerDB();
                _sql.OpenConnection();
                DataTable dt = _sql.ProcessCommand(cmd);
                if (dt.Rows.Count > 0)
                {
                    fullname = dt.Rows[0][0] + " " + dt.Rows[0][1];
                }
                _sql.CloseConnection();
                return fullname;
            }

            private void CreateChartData(String argsTitle)
            {
                this.Title = argsTitle;
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
                /*
                String [][] temp = new[]
                    {       //Rows :Total, Problem, Enhancement, Planned Work
                        //Columns : low, Medium, High, critical
                        new[] {
                            (this.ProblemCount[0]+this.EnhancementCount[0]+this.PlannedWorkCount[0]).ToString(),
                            (this.ProblemCount[1]+this.EnhancementCount[1]+this.PlannedWorkCount[1]).ToString(),
                            (this.ProblemCount[2]+this.EnhancementCount[2]+this.PlannedWorkCount[2]).ToString(),
                            (this.ProblemCount[3]+this.EnhancementCount[3]+this.PlannedWorkCount[3]).ToString()},

                        new[] { this.ProblemCount[0].ToString(),
                            this.ProblemCount[1].ToString(),
                            this.ProblemCount[2].ToString(),
                            this.ProblemCount[3].ToString()
                             },

                        new[] {this.EnhancementCount[0].ToString(),this.EnhancementCount[1].ToString(),
                            this.EnhancementCount[2].ToString(),this.EnhancementCount[3].ToString()},

                        new[] {this.PlannedWorkCount[0].ToString(),this.PlannedWorkCount[1].ToString(),this.PlannedWorkCount[2].ToString(),
                            this.PlannedWorkCount[3].ToString() } 
                    };
                */                //= FormmatArrayToString(temp);
            }
            private string FormmatArrayToString(String[][] param)
            {
                String results = "";
                for (int i = 0; i < param.Length; i++)
                {
                    if (i != 0)
                    {
                        results += ";";
                    }
                    for (int j = 0; j < param[i].Length; j++)
                    {
                        if (j != 0)
                        {
                            results += ",";
                        }
                        results += (param[i][j].ToString());
                    }
                }
                return results;
            }
            public Boolean IsManagerOrAdmin(int UserID)
            {
                return new REATrackerDB().isAdminOrManager(UserID);
            }
        }
}
