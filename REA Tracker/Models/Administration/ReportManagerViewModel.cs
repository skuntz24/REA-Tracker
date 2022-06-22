using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{

    public class ReportManagerViewModel
    {
        public List<Report> nonGroupedReports { get; set; }

        public List<Report> GroupedReports { get; set; }

        public List<ReportGroup> ReportGroups { get; set; }

        public List<Report> AllReports { get; set; }

        public List<Report> CurrentReports { get; set; }

        public String NewReportOrder { get; set; }

        public int CurrentGroup { get; set; }

        public int MaxCurrentGroupRows { get; set; }

        public String NewGroupName { get; set; }
        //Inner classes report and report group
        public class Report
        {
            public int ReportID { get; set; }
            public String GroupID { get; set; }
            public int OwnerID { get; set; }
            public String OwnerName { get; set; }
            public String Title { get; set; }
            public String Order { get; set; }
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
        //constructor
        public ReportManagerViewModel()
        {
            this.init();
        }

        private void init()
        {
            ///<summary>
            /// main init function
            ///</summary>
            this.populateReports();
            this.populateGroups();
            this.populateCurrentReports();
            this.CaluateCurrentRowCount();
        }

        private void populateReports()
        {
            ///<summary>
            ///  populates the reports
            ///</summary>
            this.nonGroupedReports = new List<Report>();
            this.GroupedReports = new List<Report>();
            this.AllReports = new List<Report>();
            String Command =
                "SELECT"
                + " REA_REPORT_GROUP.REPORT_GROUP_ID AS GROUP_ID, REA_REPORT_GROUP.TITLE AS GROUP_NAME,"
                + " REA_REPORT_GROUP_LIST.ORDER_LIST AS LIST_ORDER,"
                + " OWNER AS OWNER_ID, (ST_USERS.FIRST_NAME +' '+ST_USERS.LAST_NAME) AS OWNER_NAME,"
                + " NAME, REA_TABULAR_REPORT.REPORT_ID"
                + " FROM REA_TABULAR_REPORT"
                + " INNER JOIN ST_USERS ON ST_USERS.USER_ID = OWNER"
                + " LEFT JOIN REA_REPORT_GROUP_LIST ON REA_REPORT_GROUP_LIST.REPORT_ID = REA_TABULAR_REPORT.REPORT_ID"
                + " LEFT JOIN REA_REPORT_GROUP ON  REA_REPORT_GROUP_LIST.REPORT_GROUP_ID = REA_REPORT_GROUP.REPORT_GROUP_ID"
                + " ORDER BY GROUP_ID DESC, LIST_ORDER ASC, NAME ASC";
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
                if (newReport.GroupID == null)
                {
                    this.nonGroupedReports.Add(newReport);
                }
                else
                {
                    this.GroupedReports.Add(newReport);
                }
            }
        }

        private void populateGroups()
        {
            ///<summary>
            /// populates the 
            ///</summary>
            this.ReportGroups = new List<ReportGroup>();
            String Command = "SELECT * FROM REA_REPORT_GROUP ORDER BY REPORT_GROUP_ID ASC";
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(Command);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                ReportGroup newGroup = new ReportGroup(Convert.ToInt32(row["REPORT_GROUP_ID"]), Convert.ToString(row["TITLE"]));
                this.ReportGroups.Add(newGroup);
            }
            if (this.ReportGroups.Count != 0)
            {
                this.CurrentGroup = this.ReportGroups[0].GroupID;
            }
        }

        public void populateCurrentReports()
        {
            this.CurrentReports = this.AllReports.Where(x => x.GroupID == Convert.ToString(this.CurrentGroup)).ToList<Report>();
        }

        public string getGroupName(int GroupID)
        {
            String Value = "";
            List<ReportGroup> Groups = ReportGroups.Where(x => x.GroupID == GroupID).ToList();
            if (Groups.Count > 0)
            {
                Value = Groups.ElementAt(0).Name;
            }
            return Value;
        }

        public void CaluateCurrentRowCount()
        {
            this.MaxCurrentGroupRows = 0;
            foreach (Report repo in this.GroupedReports)
            {
                if ((repo.GroupID == Convert.ToString(this.CurrentGroup)) && (Convert.ToInt32(repo.Order) > this.MaxCurrentGroupRows))
                {
                    this.MaxCurrentGroupRows = Convert.ToInt32(repo.Order);
                }
            }
        }

        public void saveReport()
        {
            String[] newReportOrderArray = NewReportOrder.Split(',');
            //Values are comma dilminated ReportIDs
            for (int i = 0; i < newReportOrderArray.Count(); i++)
            {
                int tempReportID = Convert.ToInt32(newReportOrderArray[i]);
                REATrackerDB sql = new REATrackerDB();
                if (Convert.ToInt32(newReportOrderArray[i]) != -1)
                {
                    if (GroupedReports.Where(x => x.ReportID == tempReportID).Count() == 0)
                    {
                        //New Instance of report under group
                        String Command = "INSERT INTO REA_REPORT_GROUP_LIST(REPORT_GROUP_ID, REPORT_ID, ORDER_LIST) " +
                            "VALUES (" + Convert.ToString(this.CurrentGroup) + "," + newReportOrderArray[i] + "," + (i + 1) + ")";
                        sql.ProcessCommand(Command);
                    }
                    //if the order doesn't match the current index
                    else if (GroupedReports.Where(x => x.ReportID == tempReportID).ElementAt(0).Order != Convert.ToString(i + 1))
                    {//update existing instantces
                        String Command = "UPDATE REA_REPORT_GROUP_LIST "
                            + "SET ORDER_LIST = " + (i + 1) + " WHERE REPORT_ID = " + newReportOrderArray[i] + " AND REPORT_GROUP_ID=" + CurrentGroup;
                        sql.ProcessCommand(Command);
                    }
                }
            }
            for (int i = 0; i < GroupedReports.Count(); i++)
            {
                if (
                    (GroupedReports[i].GroupID == Convert.ToString(this.CurrentGroup)) &&
                    (newReportOrderArray.Where(x => Convert.ToInt32(x) == GroupedReports[i].ReportID).Count() == 0)
                    )
                {
                    //Delete old report under group
                    REATrackerDB sql = new REATrackerDB();
                    String Command = "DELETE FROM REA_REPORT_GROUP_LIST WHERE REPORT_GROUP_ID = " + this.CurrentGroup +
                        " AND REPORT_ID = " + GroupedReports[i].ReportID;
                    sql.ProcessCommand(Command);

                }
            }
            this.populateReports();
            this.populateCurrentReports();
        }

        public void createReportGroup()
        {
            String Command = "INSERT INTO REA_REPORT_GROUP(TITLE) VALUES('" + this.NewGroupName.Replace("'", "''") + "')";
            REATrackerDB sql = new REATrackerDB();
            sql.ProcessCommand(Command);
            this.init();
            this.CurrentGroup = this.ReportGroups.Last().GroupID;
            this.populateCurrentReports();
        }

        public void DeleteReportCurrentGroup()
        {
            String command = "DELETE FROM REA_REPORT_GROUP WHERE REPORT_GROUP_ID = " + this.CurrentGroup;
            String command2 = "DELETE FROM REA_REPORT_GROUP_LIST WHERE REPORT_GROUP_ID = " + this.CurrentGroup;
            REATrackerDB sql = new REATrackerDB();
            sql.ProcessCommand(command);
            sql.ProcessCommand(command2);
            this.init();
        }

    }


}