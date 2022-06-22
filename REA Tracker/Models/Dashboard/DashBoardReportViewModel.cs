using System;
using System.Collections.Generic;
using System.Data;
using QVICommonIntranet.Database;
using System.Linq;

namespace REA_Tracker.Models
{

    public class DashBoardReportViewModel
    {
        public String BuildID { get; set; }
        public String ProductName { get; set; }
        public String BugFixes { get; set; }
        public String ReleaseCoordinatorName { get; set; }
        public int ReleaseCoordinatorID { get; set; }
        public String Release { get; set; }
        public DateTime BuiltOn { get; set; }
        public bool isCustomerRelease { get; set; }
        public String Notes { get; set; }
        public List<dynamic> SCRList { get; set; }
        public int ProductID { get; set; }
        public bool DisplayRelatedReports { get; set; }
        public String DBVersion { get; set; }

        public List<DashBoardReportViewModel> RelatedReports { get; set; }
        public List<dynamic> ReverseRelatedReports { get; set; }
        public string CurrentProduct { get; set; }
        public List<dynamic> ReverseVersionsReports { get; set; }

        public DashBoardReportViewModel()
        {

        }

        public DashBoardReportViewModel(String build)
        {
            this.BuildID = build;
            this.populateBuild();
            this.PopulateRelatedReports();
            this.PopulateReverseRelatedReports();
        }

        public DashBoardReportViewModel(String build, bool ArgsdisplaySubBuilds)
        {
          
            this.BuildID = build;
            this.populateBuild();
            this.DisplayRelatedReports = ArgsdisplaySubBuilds;
            this.PopulateRelatedReports();
            this.PopulateReverseRelatedReports();
        }
        private void populateBuild()
        {
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.GetDashBoardReport(this.BuildID);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                this.ProductName = Convert.ToString(row["NAME"]);
                
                this.ReleaseCoordinatorID = (row["RELEASE_COORDINATOR_ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["RELEASE_COORDINATOR_ID"]));
                this.ReleaseCoordinatorName = (row["RELEASE_COORDINATOR_NAME"] == DBNull.Value ? "" : Convert.ToString(row["RELEASE_COORDINATOR_NAME"]));
                this.Release = (row["VERSION_BUILT"] == DBNull.Value ? "" : Convert.ToString(row["VERSION_BUILT"]));
                this.BuiltOn = (row["BUILT_ON"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(row["BUILT_ON"]));
                this.isCustomerRelease = (row["IS_CUSTOMER_RELEASE"] == DBNull.Value ? false : Convert.ToBoolean(row["IS_CUSTOMER_RELEASE"]));
                this.Notes = (row["NOTES"] == DBNull.Value ? "" : Convert.ToString(row["NOTES"]));
                this.DBVersion = (row["DB_VERSION"] == DBNull.Value ? "" : Convert.ToString(row["DB_VERSION"]));
                this.DisplayRelatedReports = Convert.ToBoolean(row["DISPLAY_RELATED_REPORT"]);
                this.populateSCR(row["SCR_LIST"].ToString());
            }
        }
        private void populateSCR(String SCRs)
        {
            this.SCRList = new List<dynamic>();
            if (!String.IsNullOrEmpty(SCRs))
            {
                int i = 0; //index for SCRList
                REATrackerDB sql = new REATrackerDB();
                DataTable dt = sql.GetREAInfoForDashBoard(SCRs);
                foreach(DataRow dr in dt.Rows)
                {
                    this.SCRList.Add(new System.Dynamic.ExpandoObject());
                    this.SCRList[i].PriorityName = Convert.ToString(dr["PRIORITY"] == DBNull.Value ? "" : dr["PRIORITY"]);
                    this.SCRList[i].TrackingID = Convert.ToInt32(dr["TRACKING_ID"] == DBNull.Value ? 0 : dr["TRACKING_ID"]);
                    this.SCRList[i].ProductName = Convert.ToString(dr["PRODUCT"] == DBNull.Value ? "" : dr["PRODUCT"]);
                    this.SCRList[i].IssueTypeName = Convert.ToString(dr["ISSUE_TYPE"] == DBNull.Value ? "" : dr["ISSUE_TYPE"]);
                    this.SCRList[i].StatusName = Convert.ToString(dr["STATUS"] == DBNull.Value ? "" : dr["STATUS"]);
                    this.SCRList[i].SubmittedOn = Convert.ToString(dr["SUBMITTED_ON"] == DBNull.Value ? "" : dr["SUBMITTED_ON"]);
                    this.SCRList[i].SubmittedByID = Convert.ToInt32(dr["SUBMITTED_BY"] == DBNull.Value ? 0 : dr["SUBMITTED_BY"]);
                    this.SCRList[i].VersionFound = Convert.ToString(dr["VERSION_FOUND"] == DBNull.Value ? "" : dr["VERSION_FOUND"]);
                    this.SCRList[i].AssignedToID = Convert.ToInt32(dr["ASSIGNED_TO"] == DBNull.Value ? 0 : dr["ASSIGNED_TO"]);
                    this.SCRList[i].AssignedToName = Convert.ToString(dr["ASSIGNED_TO_NAME"] == DBNull.Value ? "" : dr["ASSIGNED_TO_NAME"]);
                    this.SCRList[i].ResolvedByID = Convert.ToInt32(dr["RESOLVED_BY"] == DBNull.Value ? 0 : dr["RESOLVED_BY"]);
                    this.SCRList[i].ResolvedOn = Convert.ToString(dr["RESOLVED_ON"] == DBNull.Value ? "" : dr["RESOLVED_ON"]);
                    this.SCRList[i].Title = Convert.ToString(dr["TITLE"] == DBNull.Value ? "" : dr["TITLE"]);
                    this.SCRList[i].Customer = Convert.ToString(dr["CUSTOMER_NAME"]);
                    List<int> templist = new List<int>();
                    DataTable RelatedREAs = sql.GetRelatedREAs(Convert.ToInt32(SCRList[i].TrackingID));
                    if (RelatedREAs.Rows.Count != 0)
                    {
                        foreach (DataRow drRelated in RelatedREAs.Rows)
                        {
                            templist.Add(Convert.ToInt32(drRelated["SCR_ID"]));
                        }
                    }
                    this.SCRList[i].RelatedREAList = templist;
                    i++;
                }
            }
        }
        public String getFullName(int StUserID)
        {
            REATrackerDB _sql = new REATrackerDB();
            String FullName = "";
            String cmdText = "SELECT ST_USERS.FIRST_NAME, ST_USERS.LAST_NAME FROM ST_USERS WHERE USER_ID = " + Convert.ToString(StUserID) + ";";
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmdText))
            {
                if (dt.Rows.Count > 0)
                {
                    FullName = Convert.ToString(dt.Rows[0][0]) + " " + Convert.ToString(dt.Rows[0][1]);
                }
            }
            return FullName;
        }
        public String getProblem(int TrackingID)
        {
            REATrackerDB sql = new REATrackerDB();
            return
                Convert.ToString(sql.ProcessScalarCommand(
                "SELECT TOP 1 DESCRIPTION FROM ST_HISTORY WHERE ((TRACKING_ID=" + TrackingID + ") AND ( TYPE=1)) ORDER BY HISTORY_ID ASC;"));
        }
        public String getSolution(int TrackingID)
        {
            REATrackerDB sql = new REATrackerDB();
            return
                Convert.ToString(sql.ProcessScalarCommand(
                "SELECT TOP 1 DESCRIPTION FROM ST_HISTORY WHERE ((TRACKING_ID=" + TrackingID + ") AND ( TYPE=3)) ORDER BY HISTORY_ID DESC;"));
        }
        public void PopulateRelatedReports()
        {
            REATrackerDB sql = new REATrackerDB();
            this.RelatedReports = new List<DashBoardReportViewModel>();      
            if (!String.IsNullOrEmpty(this.BuildID))
            { 
                int i = 0;
                string RelatedBuildId = "";
                using (DataTable dtRelatedBuilds = sql.GetRelatedBuilds(Convert.ToInt16(BuildID)))
                {
                    foreach (System.Data.DataRow drBuild in dtRelatedBuilds.Rows)
                    {
                        RelatedBuildId = Convert.ToString(drBuild["BUILD_ID"]);
                        DashBoardReportViewModel tempreport = new DashBoardReportViewModel(RelatedBuildId, false);
                        this.RelatedReports.Add(tempreport);
                        i++;
                    }
                }
            }

        }
        public void PopulateReverseRelatedReports()
        {
            this.ReverseRelatedReports = new List<dynamic>();
            this.ReverseVersionsReports = new List<dynamic>();
            if (!String.IsNullOrEmpty(this.BuildID))
            {
                REATrackerDB sql = new REATrackerDB();
                string Cmd = $@"SELECT DISTINCT RELEASE_RELATION_ID, ST_PRODUCT_RELEASE_RELATION.BUILD_ID AS BuildId, ST_PRODUCT_RELEASE.PRODUCT_ID, MAJOR, LEN(BUILD), MINOR, BUILD, NAME  
FROM ST_PRODUCT_RELEASE_RELATION
INNER JOIN ST_PRODUCT_RELEASE
ON ST_PRODUCT_RELEASE.BUILD_ID = ST_PRODUCT_RELEASE_RELATION.BUILD_ID
INNER JOIN ST_PRODUCT
ON ST_PRODUCT_RELEASE.PRODUCT_ID = ST_PRODUCT.PRODUCT_ID
WHERE RELATED_BUILD_ID={this.BuildID}
ORDER BY NAME, MAJOR, MINOR, LEN(BUILD), BUILD;";
                DataTable ReverseDT = sql.ProcessCommand(Cmd);
                for(int i = 0; i < ReverseDT.Rows.Count; i++)
                {
                    ReverseRelatedReports.Add(new System.Dynamic.ExpandoObject());
                    ReverseRelatedReports[i].ProductName = Convert.ToString(ReverseDT.Rows[i]["NAME"]);
                    ReverseRelatedReports[i].BuildId = Convert.ToInt32(ReverseDT.Rows[i]["BuildId"]);
                    ReverseRelatedReports[i].Version = (ReverseDT.Rows[i]["MAJOR"] == DBNull.Value ? "?" : Convert.ToString(ReverseDT.Rows[i]["MAJOR"])) +
                    (ReverseDT.Rows[i]["MINOR"] == DBNull.Value ? ".?" : "." + Convert.ToString(ReverseDT.Rows[i]["MINOR"])) +
                    (ReverseDT.Rows[i]["BUILD"] == DBNull.Value ? ".?" : "." + Convert.ToString(ReverseDT.Rows[i]["BUILD"]));
                }
            }
        }
    }

}