using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using REA_Tracker.Helpers;
using QVICommonIntranet.Database;
namespace REA_Tracker.Models
{
    public class DashBoardViewModel
    {
        private REATrackerDB _sql = new REATrackerDB();


        public REATrackerDB.ProductInfo ProductInfo;
        public REATrackerDB.ProductDashboardReports ProductReport;

        //input items
        public int status { get; set; }
        public int metric { get; set; }
     
        //Display Items
        public List<dynamic> productList { get; set; }
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
        public List<dynamic> ListOfMetrics { get; set; }

        public bool IsFavoriteProduct { get; set; }

        //Display Release
        public bool CanRelease { get; set; }
        public bool IsCoordinator { get; set; }
        public bool HasDeliveries { get; set; }
        public int NumberOfReleases = 4;        //max number of tabs to display

        public List<ReleaseViewModel> Releases { get; set; }
        //Display info
        public int metricsHigh { get; set; }
        public int metricsCritical { get; set; }

        public List<dynamic> Reviewers { get; set; }

        public List<dynamic> codeReviewList { get; set; }
        //public DashBoardViewModel(int userID, bool? FavoriteStatus)
        //{
        //    this.ProductID = 0;
        //    this.UserID = userID;
        //    this.IsFavoriteProduct = false;
        //    this.init(FavoriteStatus);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID">ID of the current user</param>
        /// <param name="productID">ID of the current product</param>
        /// <param name="newFavoriteStatus">the new favorite status for the product</param>
        public DashBoardViewModel(int userID, int productID, bool? newFavoriteStatus)
        {
            this.UserID = userID;

            //set all the information about the product
            this.ProductInfo = _sql.GetProductBasic(productID);

            this.ListOfMetrics = new List<dynamic>();
            int i = 0;
            if (ProductInfo.Metrics != null)
            {


                foreach (REATrackerDB.ProductMetricsInfo metric in this.ProductInfo.Metrics)
                {
                    if (metric.Display)
                    {
                        this.ListOfMetrics.Add(new System.Dynamic.ExpandoObject());

                        //all the static values(i.e. thresholds)
                        this.ListOfMetrics[i].MajorVersion = metric.MajorVersion.ToString();
                        this.ListOfMetrics[i].MinorVersion = metric.MinorVersion.ToString();
                        this.ListOfMetrics[i].CodeCriticalCount = metric.CodeFreezeCriticalCount;
                        this.ListOfMetrics[i].CodeHighCount = metric.CodeFreezeHighCount;
                        this.ListOfMetrics[i].CodeMetric = metric.CodeFreezeDefectMetric;
                        this.ListOfMetrics[i].CodeDefectCount = metric.CodeFreezeDefectCount;
                        this.ListOfMetrics[i].ReleaseCriticalCount = metric.ReleaseCriticalCount;
                        this.ListOfMetrics[i].ReleaseHighCount = metric.ReleaseHighCount;
                        this.ListOfMetrics[i].ReleaseMetric = metric.ReleaseDefectMetric;
                        this.ListOfMetrics[i].ReleaseDefectCount = metric.ReleaseDefectCount;

                        this.ListOfMetrics[i].MajorMinorDisplay = metric.Label;
                        this.ListOfMetrics[i].MajorMinorId = metric.LabelID;

                        //current values
                        this.ListOfMetrics[i].HighCount = metric.CurrentHighCount;
                        this.ListOfMetrics[i].CriticalCount = metric.CurrentCriticalCount;
                        this.ListOfMetrics[i].SumOfSquares = metric.CurrentDefectMetric;
                        this.ListOfMetrics[i].Sum = metric.CurrentDefectCount;
                        //post build values
                        this.ListOfMetrics[i].PostBuildHighCount = metric.PostBuildHighCount;
                        this.ListOfMetrics[i].PostBuildCriticalCount = metric.PostBuildCriticalCount;
                        this.ListOfMetrics[i].PostBuildSumOfSquares = metric.PostBuildDefectMetric;
                        this.ListOfMetrics[i].PostBuildSum = metric.PostBuildDefectCount;

                        i++;
                    }
                }
            }

            //Rights to access code reviews
            //get all code reviews
            List<dynamic> list = new List<dynamic>();
            List<dynamic> ReviewersList = new List<dynamic>();
            int j = 0;
            using (System.Data.DataTable dt = _sql.GetOpenCodeReviews(this.ProductInfo.ID))
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[j].Id = Convert.ToInt32(row["ID"]);
                    list[j].TrackingId = Convert.ToInt32(row["TRACKING_ID"]);
                    list[j].Product = Convert.ToInt32(row["PRODUCT"]);
                    list[j].Title = Convert.ToString(row["TITLE"]);
                    list[j].CreatedOn = Math.Floor((System.DateTime.Now - Convert.ToDateTime(row["CREATED_ON"])).TotalDays);
                    list[j].CreatedBy = Convert.ToString(row["CREATED_BY_NAME"]);
                    list[j].CreatedByID = Convert.ToInt32(row["CREATED_BY"]);
                    list[j].Reviewers = "";
                    List<dynamic> users = _sql.GetReviewers(list[j].Id);
                    foreach (var item in users)
                    {
                        //TODO: REMOVE THE UI, combine with the othe call
                        REATrackerDB tempSql = new REATrackerDB();
                        REATrackerDB.ReviewStatus iconValue = tempSql.GetReviewStatus(list[j].Id, item.UsersId);
                        if (iconValue == REATrackerDB.ReviewStatus.Approved)
                        {
                            list[j].Reviewers += "<i title='Approved' class='fa qvi-color-medium fa-check-circle'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                        else if (iconValue == REATrackerDB.ReviewStatus.ApprovedWithComments)
                        {
                            list[j].Reviewers += "<i title='Approved With Comments' class='fa qvi-color-low fa-comment-o'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                        else if (iconValue == REATrackerDB.ReviewStatus.NeedsWork)
                        {
                            list[j].Reviewers += "<i title='Needs Work' class='fa qvi-color-critical fa-times-circle'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                        else
                        {
                            list[j].Reviewers += "<i title='Accepted' class='fa fa-question-circle'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                    }
                    list[j].Reviewers += "";
                    j++;
                }
            }
            this.codeReviewList = list;


            //get all the counts for the various built in reports (left side of the dashboard)
            this.ProductReport = _sql.GetProductReportCounts(productID);
                this.Problems = this.ProductReport.Problems;
                this.Enhancements = this.ProductReport.Enhancements;
                this.PlannedWork = this.ProductReport.PlannedWork;
                this.Low = this.ProductReport.Low;
                this.Medium = this.ProductReport.Medium;
                this.High = this.ProductReport.High;
                this.Critical = this.ProductReport.Critical;

                this.DetermineCanRelease();
                this.productList = ModelHelper.populatedProductList(this.UserID, this.ProductInfo.ID);
            }
        
        public String BuildTree(int root, int level)
        {
            REATrackerDB sql = new REATrackerDB();
            String TreeList = "";

            TreeList += "<ul id='BuiltTree'>";
            String productName = "SELECT NAME FROM REA_PRODUCT WHERE PRODUCT_ID = " + Convert.ToString(root);
            TreeList += ("<li align='left'>" + Convert.ToString(sql.ProcessScalarCommand(productName)) + "</li>");
            //Do you have CHILD_ID?
            String check = "SELECT COUNT(CHILD_ID) AS CHILDREN FROM REA_PRODUCT_RELATION WHERE PARENT_ID = " + Convert.ToString(root);
            DataTable checker = sql.ProcessCommand(check);
            if (Convert.ToInt32(checker.Rows[0][0]) > 0)
            {//Yes? spawn it
                String children = "SELECT CHILD_ID FROM REA_PRODUCT_RELATION WHERE PARENT_ID = " + Convert.ToString(root);
                DataTable childrenDT = sql.ProcessCommand(children);
                foreach (System.Data.DataRow row in childrenDT.Rows)
                {
                    TreeList += BuildTree(Convert.ToInt32(row[0]), level + 1);
                }
            }
            TreeList += "</ul>";
            return TreeList;
        }

        public class ReleaseViewModel
        {
            public int Major { get; set; }
            public int Minor { get; set; }
            public string Misc { get; set; }
            //To Make Displaying Easier
            public String TabName { get; set; }
            public String DisplayVersion { get; set; }


            public int NumOfBuilds { get; set; }
            public List<REATrackerDB.ReleaseVersion> ListOfRecords { get; set; }
            public ReleaseViewModel(int major, int minor, int Product, int Index)
            {
                this.Major = major;
                this.Minor = minor;
                //this.Misc = MISC;
                this.TabName = "tab" + Convert.ToString(Index);
                this.DisplayVersion = this.Major + "." + this.Minor + "." + "x";
                //commenting for removing misc
                if (!string.IsNullOrEmpty(Misc))
                {
                    DisplayVersion += " " + this.Misc;
                }
                this.ListOfRecords = new List<REATrackerDB.ReleaseVersion>();
                String Command = "SELECT  " +
                    " (ST_PRODUCT_RELEASE.MAJOR+'.'+ST_PRODUCT_RELEASE.MINOR+'.'+ST_PRODUCT_RELEASE.BUILD)AS VERSION, " +
                    " ST_PRODUCT_RELEASE.MAJOR AS MAJOR, " +
                    " ST_PRODUCT_RELEASE.MINOR AS MINOR, " +
                    " ST_PRODUCT_RELEASE.BUILD AS BUILD, " +
                    " ST_PRODUCT_RELEASE.MISC AS MISC, " +
                    " ST_PRODUCT_RELEASE.BUILT_ON AS BUILT_ON, " +
                    " ST_PRODUCT_RELEASE.IS_CUSTOMER_RELEASE, " +
                    " ST_PRODUCT_RELEASE.NOTES, " +
                    " ST_PRODUCT_RELEASE.SCR_LIST, " +
                    " ST_PRODUCT_RELEASE.BUILD_ID AS BUILD_ID, " +
                    " ST_PRODUCT_RELEASE.DB_VERSION AS DB_VERSION" +
                    " FROM ST_PRODUCT_RELEASE " +
                    " WHERE PRODUCT_ID = " + Product +
                    " AND CAST(MAJOR AS INT) = " + this.Major +
                    " AND CAST(MINOR AS INT) = " + this.Minor +
                    " ORDER BY CAST(BUILD AS INT) DESC, BUILT_ON DESC;";
                REATrackerDB vsql = new REATrackerDB();
                DataTable vdt = vsql.ProcessCommand(Command);
                foreach (System.Data.DataRow vrow in vdt.Rows)
                {
                    REATrackerDB.VersionInfo vi;
                    vi.Major = GenericDB.CheckValue(vrow["MAJOR"], -1);
                    vi.Minor = GenericDB.CheckValue(vrow["MINOR"], -1);
                    vi.Build = GenericDB.CheckValue(vrow["BUILD"], -1);
                    vi.Misc = GenericDB.CheckValue(vrow["MISC"], string.Empty);
                    /*
                    int Vmajor = Convert.ToInt32(vrow["MAJOR"] == DBNull.Value ? -1 : vrow["MAJOR"]);
                    int Vminor = Convert.ToInt32(vrow["MINOR"] == DBNull.Value ? -1 : vrow["MINOR"]);
                    int Vbuild = Convert.ToInt32(vrow["BUILD"] == DBNull.Value ? -1 : vrow["BUILD"]);
                    string Vmisc = Convert.ToString(vrow["MISC"] == DBNull.Value ? "" : vrow["MISC"]);
                    */

                    DateTime? BuiltOn = (vrow["BUILT_ON"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(vrow["BUILT_ON"]));
                    int CustomerRelease = Convert.ToInt32(vrow["IS_CUSTOMER_RELEASE"] == DBNull.Value ? -1 : vrow["IS_CUSTOMER_RELEASE"]);
                    String Notes = Convert.ToString(vrow["NOTES"] == DBNull.Value ? "" : vrow["NOTES"]);
                    String SCRList = Convert.ToString(vrow["SCR_LIST"] == DBNull.Value ? "" : vrow["SCR_LIST"]);
                    int Built_ID = Convert.ToInt32(vrow["BUILD_ID"] == DBNull.Value ? -1 : vrow["BUILD_ID"]);
                    String DBVersion = Convert.ToString(vrow["DB_VERSION"] == DBNull.Value ? "" : vrow["DB_VERSION"]);
                    this.ListOfRecords.Add(
                        new REATrackerDB.ReleaseVersion(Built_ID, vi, BuiltOn, CustomerRelease, Notes, SCRList, DBVersion)
                        );

                    /*
                    this.ListOfRecords.Add(
                        new REATrackerDB.ReleaseVersion(Built_ID, Vmajor, Vminor, Vbuild, Vmisc, BuiltOn, CustomerRelease, Notes, SCRList)
                        );
                        */
                }

            }
        }

 

        private void DetermineCanRelease()
        {
            this.HasDeliveries = _sql.CanBuildRelease(this.ProductInfo.ID);
            if (this.ProductInfo.ReleaseCoordinatorID == this.UserID || this.ProductInfo.ReleaseCoordinator2ID == this.UserID)
            {
                this.IsCoordinator = true;
            }
            else
            {
                this.IsCoordinator = false;
            }
            if (this.HasDeliveries && this.IsCoordinator)
            {
                this.CanRelease = true;
            }
            else
            {
                this.CanRelease = false;
            }
        }


    }

}