using System;
using System.Collections.Generic;
using System.Data;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{
    public class BuildVerificationTestReportModel
    {
        public String BuildID { get; set; }
        public String ProductName { get; set; }
        public String Release { get; set; }
        //public String ReleaseCoordinatorName { get; set; }
        //public int ReleaseCoordinatorID { get; set; }
        //public DateTime BuiltOn { get; set; }
        //public bool isCustomerRelease { get; set; }
        //public String Notes { get; set; }
        //public int ProductID { get; set; }
        //public bool DisplayRelatedReports { get; set; }
        public List<dynamic> SCRList { get; set; }
        public List<dynamic> ComponentList { get; set; }
        public BuildVerificationTestReportModel()
        {

        }

        public BuildVerificationTestReportModel(String build)
        {
            this.BuildID = build;
            this.populateBuild();
        }

        private void populateBuild()
        {
            string tmpCompleteList = "";
            this.ComponentList = new List<dynamic>();
            int i = 0;

            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.GetDashBoardReport(this.BuildID); //this only returns 1 row
            foreach (DataRow row in dt.Rows)
            {
                this.ProductName = Convert.ToString(row["NAME"]);
                this.Release = Convert.ToString(row["VERSION_BUILT"]);
                //this.ReleaseCoordinatorID = (row["RELEASE_COORDINATOR_ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["RELEASE_COORDINATOR_ID"]));
                //this.ReleaseCoordinatorName = (row["RELEASE_COORDINATOR_NAME"] == DBNull.Value ? "" : Convert.ToString(row["RELEASE_COORDINATOR_NAME"]));
                //this.BuiltOn = (row["BUILT_ON"] == DBNull.Value ? Convert.ToDateTime(null) : Convert.ToDateTime(row["BUILT_ON"]));
                //this.isCustomerRelease = (row["IS_CUSTOMER_RELEASE"] == DBNull.Value ? false : Convert.ToBoolean(row["IS_CUSTOMER_RELEASE"]));
                //this.Notes = (row["NOTES"] == DBNull.Value ? "" : Convert.ToString(row["NOTES"]));
                //this.DisplayRelatedReports = Convert.ToBoolean(row["DISPLAY_RELATED_REPORT"]);
                tmpCompleteList += row["SCR_LIST"];

                DataTable dtRelated = sql.GetRelatedBuilds(Convert.ToInt16(this.BuildID));
                foreach (DataRow drRelated in dtRelated.Rows)
                {
                    DataTable dtRelatedBuild = sql.GetDashBoardReport(Convert.ToInt32(drRelated["BUILD_ID"]).ToString()); //this only returns 1 row
                    foreach (DataRow drRelatedBuild in dtRelatedBuild.Rows)
                    {
                        this.ComponentList.Add(new System.Dynamic.ExpandoObject());
                        this.ComponentList[i].BuildID = Convert.ToInt32(drRelated["BUILD_ID"]);
                        this.ComponentList[i].Name = drRelatedBuild["NAME"].ToString(); 
                        this.ComponentList[i].Release = drRelatedBuild["VERSION_BUILT"].ToString();
                        this.ComponentList[i].TotalSCRS = 0;
                        this.ComponentList[i].OpenSCRS = 0;

                        if (drRelatedBuild["SCR_LIST"] != DBNull.Value)
                        {
                            if (drRelatedBuild["SCR_LIST"].ToString().Length > 2)
                            {
                                this.ComponentList[i].TotalSCRS = Convert.ToInt32(drRelatedBuild["SCR_COUNT"]);
                                this.ComponentList[i].OpenSCRS = (int)sql.ProcessScalarCommand($"SELECT count(*) FROM ST_TRACK WHERE TRACKING_ID IN ({drRelatedBuild["SCR_LIST"].ToString()}) AND STATUS<>9");

                                tmpCompleteList += "," + drRelatedBuild["SCR_LIST"].ToString();
                            }
                        }
                        i++;
                    }
                }

                //we have all of the SCRs for all of the products that were components
                this.populateSCR(tmpCompleteList);
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
                foreach (DataRow dr in dt.Rows)
                {
                    this.SCRList.Add(new System.Dynamic.ExpandoObject());
                    this.SCRList[i].PriorityID = Convert.ToString(dr["PRIORITY_ID"] == DBNull.Value ? "" : dr["PRIORITY_ID"]);
                    this.SCRList[i].PriorityName = Convert.ToString(dr["PRIORITY"] == DBNull.Value ? "" : dr["PRIORITY"]);
                    this.SCRList[i].TrackingID = Convert.ToInt32(dr["TRACKING_ID"] == DBNull.Value ? 0 : dr["TRACKING_ID"]);
                    this.SCRList[i].ProductName = Convert.ToString(dr["PRODUCT"] == DBNull.Value ? "" : dr["PRODUCT"]);
                    this.SCRList[i].IssueTypeID = Convert.ToString(dr["ISSUE_TYPE_ID"] == DBNull.Value ? "" : dr["ISSUE_TYPE_ID"]);
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
                }//foreach
            }//if test
        }

    }//class
}//namespace