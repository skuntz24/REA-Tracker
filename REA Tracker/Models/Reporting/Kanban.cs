using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using REA_Tracker.Helpers;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{
    public class KanbanViewModel
    {
        public struct KanbanSCR
        {
            public int trackingID;
            public string title;
            public int issueID;
            public int priorityID;
            public int statusID;
            public string watchers;
            public string related;
            public int lastUpdate;
            public string rank;
            public string assignedTo;
            public bool isRequired;
            public string managersNote;
            public string plannedRelease;
            public string actualRelease;
            public string product;
            //public string keywords; TODO: future
        }
        private REATrackerDB _sql = new REATrackerDB();
        public List<dynamic> KanbanCards = new List<dynamic>();
        public int UserID { get; set; }
        public int CurrentProductID { get; set; }
        public int ReportID { get; set; }
        public string ReportTitle { get; set; }
        public int ReportCount { get; set; }
        public string ErrorMessage { get; set; }

        public KanbanViewModel(int reportID)
        {
            //test SCR for card is 25965
            ReadData(reportID);
        }
        public KanbanViewModel()
        {
        }

        public void CreateKanban()
        {
            ReadData(this.ReportID);
        }

        private void ReadData(int reportID)
        {
            ReportID = reportID;
            KanbanCards.Clear();
            if (!_sql.DoesReportExists(reportID))
            {
                this.ReportTitle = "<UNKNOWN>";
                this.ErrorMessage = @"The report you are trying to display does not exist.";
                return;
            }
            else
            {
                ReportTitle = (string)_sql.ProcessScalarCommand($"SELECT NAME FROM ST_TABULAR_REPORT WHERE REPORT_ID = {ReportID}");
                this.ErrorMessage = "";
            }

            string firstname;
            string lastname;
            SearchViewModel svm = new SearchViewModel(reportID, this.UserID, this.CurrentProductID);
            int rowCount = svm.ProcessSearch();
            string trackingIDs = "";

            if (rowCount > 0)
            {
                //check to see if we have at least one column that contains "SCR" 
                if (svm.DataRows[0].Table.Columns.Contains("SCR"))
                {
                    foreach (DataRow dr in svm.DataRows)
                    {
                        trackingIDs += dr["SCR"].ToString() + ", ";
                    }
                    //remove the last comma
                    trackingIDs = trackingIDs.Trim().TrimEnd(',');
                }
                else
                {
                    //invalid report design
                    this.ErrorMessage = @"The report you are trying to display is missing the SCR column.";
                    return;
                }
            }

            using (DataTable dt = _sql.KanbanREAS(trackingIDs))
            {
                //this count is different that one about as it removes all of the close SCRs
                ReportCount = dt.Rows.Count;

                foreach (DataRow dr in dt.Rows)
                {
                    KanbanSCR ks = new KanbanSCR();
                    ks.trackingID = (int)dr["TRACKING_ID"];
                    ks.title = dr["TITLE"].ToString();
                    ks.priorityID = (int)dr["PRIORITY"];
                    ks.issueID = (int)dr["ISSUE_TYPE"];
                    ks.statusID = (int)dr["STATUS"];
                    ks.lastUpdate = (dr["LAST_UPDATE"] == DBNull.Value) ? -1 : (int)dr["LAST_UPDATE"];
                    ks.rank = (dr["RANK_ORDER"] == DBNull.Value) ? "?" : dr["RANK_ORDER"].ToString();
                    ks.isRequired = ((int)dr["REQUIRED"] == 1);
                    firstname = dr["FIRST_NAME"].ToString();
                    lastname = dr["LAST_NAME"].ToString();
                    if (!String.IsNullOrEmpty(firstname) && !String.IsNullOrEmpty(lastname))
                    {
                        ks.assignedTo =
                                    firstname[0].ToString().ToUpper() + lastname[0].ToString().ToUpper() + ":" +
                                    firstname + " " + lastname;
                    }
                    else
                    {
                        ks.assignedTo = "";
                    }
                    ks.managersNote = (dr["MANAGERS_NOTE"] == DBNull.Value) ? "" : dr["MANAGERS_NOTE"].ToString();
                    ks.plannedRelease = (dr["PLANNED_RELEASE"] == DBNull.Value) ? "" : dr["PLANNED_RELEASE"].ToString();
                    ks.actualRelease = (dr["ACTUAL_RELEASE"] == DBNull.Value) ? "" : dr["ACTUAL_RELEASE"].ToString();
                    ks.product = dr["PRODUCT_NAME"].ToString();
                    ks.watchers = "";
                    string command2 = $@"
SELECT DISTINCT FIRST_NAME, LAST_NAME, ST_PRODUCT_TEAM.ROLE, ST_ROLES.NAME, ST_PRODUCT_TEAM.PRODUCT 
FROM ST_WATCH_LIST 
INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_WATCH_LIST.USER_ID
INNER JOIN ST_PRODUCT_TEAM ON ST_PRODUCT_TEAM.USER_ID = ST_WATCH_LIST.USER_ID
INNER JOIN ST_TRACK ON ST_TRACK.TRACKING_ID = ST_WATCH_LIST.TRACKING_ID
INNER JOIN ST_ROLES ON ST_ROLES.ROLE_ID = ST_PRODUCT_TEAM.ROLE
WHERE ST_WATCH_LIST.TRACKING_ID={ks.trackingID} AND (ST_PRODUCT_TEAM.PRODUCT = ST_TRACK.PRODUCT) ORDER BY ST_PRODUCT_TEAM.ROLE, FIRST_NAME";
                    using (DataTable dt2 = _sql.ProcessCommand(command2))
                    {
                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            firstname = dr2["FIRST_NAME"].ToString();
                            lastname = dr2["LAST_NAME"].ToString();
                            ks.watchers +=
                                firstname[0].ToString().ToUpper() + lastname[0].ToString().ToUpper() + ":" +
                                firstname + " " + lastname + ":" +
                                dr2["ROLE"].ToString() + ";";                                
                        }
                    }

                    ks.related = "";
                    using (DataTable dt3 = _sql.GetRelatedREAs(ks.trackingID))
                    {
                        foreach (DataRow dr3 in dt3.Rows)
                        {
                            ks.related += dr3["SCR_ID"].ToString() + ",";
                        }
                    }
                    KanbanCards.Add(ks);
                }
            }
        } //end ReadData
    }
}