using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{
    public class BuildReleaseViewModel
    {
        //Dropdowns
        public List<String> SelectMajor { get; set; }
        public List<String> SelectMinor { get; set; }
        public List<SubBuildItem> SubBuild { get; set; }
        public List<RelatedBinder> SubBuildValues { get; set; }
        public List<String> Testers { get; set; }
        //Inputs
        public int? Major { get; set; }
        public int? Minor { get; set; }
        public int? Build { get; set; }
        public string Misc { get; set; }//Added

        public string Version { get; set; }

        public int StUserID { get; set; }
        public int Tester { get; set; }
        public String DistibutionList { get; set; }
        public int ProductID { get; set; }
        public String ProductName { get; set; }
        public String NewNotes { get; set; }
        public Boolean DisplaySubBuild { get; set; }
        public List<int> SelectedReleases { get; set; }
        //Hidden
        public int MaxBuild { get; set; }
        //Display
        public String LatestBuild { get; set; }
        public String LatestDBVersion { get; set; }
        public int MajorVersionReleased { get; set; }
        public int MinorVersionReleased { get; set; }
        public int BuildVersionReleased { get; set; }

        public string MiscVersionReleased { get; set; }//Added

        public List<DisplaySCRLine> SCRs { get; set; }
        //Private Data
        public int ReleaseCoorindatorID { get; set; }
        //Edit
        public int ReleaseId { get; set; }
        public int CustomerRelease { get; set; }
        public DateTime BuiltOn { get; set; }
        public string BuiltByName { get; set; }
        public int ReleaseType { get; set; }
        public String ReleasedSCRs { get; set; }
        public String URL { get; set; }
        public List<dynamic> ReleaseTypes { get; set; }
        public String DBVersion { get; set; }
        public List<Component> Components { get; set; }

        public BuildReleaseViewModel(int productId, int UserID)
        {
            this.StUserID = UserID;
            this.ProductID = productId;
            this.init();

        }

        public BuildReleaseViewModel()
        {
            if (this.ProductID != 0 && this.StUserID != 0)
            {
                this.init();
            }
        }

        public void PopulateComponents()
        {
            REATrackerDB sql = new REATrackerDB();
            if (!String.IsNullOrEmpty(Convert.ToString(this.ReleaseId)))
            {
                this.Components = new List<Component>();
                using (DataTable dtRelatedBuilds = sql.GetRelatedBuilds(this.ReleaseId))
                {
                    foreach (System.Data.DataRow drBuild in dtRelatedBuilds.Rows)
                    {
                        Component aComponent = new Component();
                        DataTable versions = new DataTable();
                        aComponent.ComponentName = Convert.ToString(drBuild["NAME"]);
                        aComponent.ComponentVersion = Convert.ToString(drBuild["BUILD_VERSION_WITH_DATE"]);
                        aComponent.ReleaseId = Convert.ToString(this.ReleaseId);
                        aComponent.ReleaseRelationId = Convert.ToInt32(drBuild["RELEASE_RELATION_ID"]);
                        aComponent.RelatedBuildId = Convert.ToInt32(drBuild["RELATED_BUILD_ID"]);
                        aComponent.Versions = new List<string>();
                        aComponent.BuildId = new List<Int32>();
                        versions = sql.GetBuildsByProduct(Convert.ToInt32(drBuild["PRODUCT_ID"]));
                        foreach (System.Data.DataRow drV in versions.Rows)
                        {
                            aComponent.Versions.Add(Convert.ToString(drV["BUILD_VERSION_WITH_DATE"]));
                            aComponent.BuildId.Add(Convert.ToInt32(drV["BUILD_ID"]));
                        }
                        this.Components.Add(aComponent);
                    }
                }
            }

        }

        //Get
        public void init()
        {
            this.SelectMajor = new List<String>();
            this.SelectMinor = new List<String>();
            this.SubBuild = new List<SubBuildItem>();
            this.Testers = new List<String>();
            this.SelectedReleases = new List<int>();
            this.SCRs = new List<DisplaySCRLine>();
            this.SubBuildValues = new List<RelatedBinder>();
            this.SelectMajor = new List<String>();
            this.SelectMinor = new List<String>();
            this.Components = new List<Component>();
            // this.SelectBuild = "":
            REATrackerDB sql = new REATrackerDB();
            //Get name
            this.ProductName = Convert.ToString(sql.ProcessScalarCommand("SELECT NAME FROM ST_PRODUCT WHERE PRODUCT_ID= " + Convert.ToString(this.ProductID)));
            //Release Report page
            //Get the release coordinator
            String getReleaseCoordindator = "select release_coordinator from ST_PRODUCT where PRODUCT_ID = " + this.ProductID;
            int releaseCoordinator = Convert.ToInt32(sql.ProcessScalarCommand(getReleaseCoordindator));


            //Get the major and minor versions and place into a select list format
            String findMajor = "SELECT DISTINCT(VR_MAJOR) FROM ST_TRACK WHERE ((ASSIGNED_TO = " + Convert.ToString(releaseCoordinator) + ") AND (PRODUCT=" + Convert.ToString(this.ProductID) + ") AND (STATUS=7) AND (VR_MAJOR IS NOT NULL));";
            String findMinor = "SELECT DISTINCT(VR_MINOR) FROM ST_TRACK WHERE ((ASSIGNED_TO = " + Convert.ToString(releaseCoordinator) + ") AND (PRODUCT=" + Convert.ToString(this.ProductID) + ") AND (STATUS=7) AND (VR_MINOR IS NOT NULL));";
            DataTable dtMajors = sql.ProcessCommand(findMajor);
            if (this.Major == null)
            {
                if (dtMajors.Rows.Count > 0 && dtMajors.Rows[0][0] != DBNull.Value)
                {
                    this.Major = Convert.ToInt32(dtMajors.Rows[0][0]);
                }
                else
                {
                    this.Major = 0;
                }
            }
            foreach (System.Data.DataRow row in dtMajors.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    String option = "<option value =" + (Convert.ToInt32(row[0])) + (Convert.ToInt32(row[0]) == this.Major ? " Selected" : "") + ">" + (Convert.ToString(row[0])) + "</option>";
                    SelectMajor.Add(option);
                }
            }
            DataTable dtMinors = sql.ProcessCommand(findMinor);
            if (this.Minor == null)
            {
                if (dtMinors.Rows.Count > 0 && dtMinors.Rows[0][0] != DBNull.Value)
                {
                    this.Minor = Convert.ToInt32(dtMinors.Rows[0][0]);
                }
                else
                {
                    this.Minor = 0;
                }
            }


            //Get latest build
            String latestBuildCommand = @"SELECT TOP 1 [BUILD_ID],[PRODUCT_ID],[MAJOR],[MINOR],[BUILD],[MISC],[DB_VERSION] FROM [ST_PRODUCT_RELEASE] WHERE [PRODUCT_ID]=" + this.ProductID;
            if (this.Major != null && this.Minor != null)
            {
                latestBuildCommand += " AND [MAJOR] ='" + this.Major + "' AND [MINOR] = '" + this.Minor + "' ";
            }
            latestBuildCommand += " ORDER BY BUILT_ON DESC;";

            DataTable latestBuildDataTable = sql.ProcessCommand(latestBuildCommand);

            if (latestBuildDataTable.Rows.Count > 0)
            {
                this.LatestBuild =
                    Convert.ToString(latestBuildDataTable.Rows[0]["MAJOR"] == DBNull.Value ? "" : latestBuildDataTable.Rows[0]["MAJOR"]) +
                    "." + Convert.ToString(latestBuildDataTable.Rows[0]["MINOR"] == DBNull.Value ? "" : latestBuildDataTable.Rows[0]["MINOR"]) +
                    "." + Convert.ToString(latestBuildDataTable.Rows[0]["BUILD"] == DBNull.Value ? "" : latestBuildDataTable.Rows[0]["BUILD"]) +
                    (latestBuildDataTable.Rows[0]["MISC"] == DBNull.Value ? "" : " " + Convert.ToString(latestBuildDataTable.Rows[0]["MISC"]));

                this.LatestDBVersion = Convert.ToString(latestBuildDataTable.Rows[0]["DB_VERSION"] == DBNull.Value ? "" : latestBuildDataTable.Rows[0]["DB_VERSION"]);
            }


            foreach (System.Data.DataRow row in dtMinors.Rows)
            {
                String option = "<option value =" + (Convert.ToInt32(row[0] == DBNull.Value ? 0 : row[0])) + (Convert.ToInt32(row[0] == DBNull.Value ? 0 : row[0]) == this.Minor ? " Selected" : "") + ">" + (Convert.ToString(row[0])) + "</option>";
                SelectMinor.Add(option);
            }
            //Getting DistributionList
            String DistributionListCommand = "SELECT DISTRIBUTION_LIST FROM ST_PRODUCT WHERE PRODUCT_ID =" + Convert.ToString(this.ProductID) + ";";
            this.DistibutionList = Convert.ToString(sql.ProcessScalarCommand(DistributionListCommand));
            this.populate();
            //Max build?? not used in old site.
            String MaxBuildCommand = "SELECT MAX(VR_BUILD) FROM ST_TRACK WHERE ((VR_MAJOR=" + this.Major + ") AND (PRODUCT=" + this.ProductID + "));";
            DataTable maxBuildDataTable = sql.ProcessCommand(MaxBuildCommand);
            this.MaxBuild = maxBuildDataTable.Rows.Count > 0 ? (Convert.ToInt32(maxBuildDataTable.Rows[0][0] == DBNull.Value ? null : maxBuildDataTable.Rows[0][0])) : 0;
            //TODO Save Max Build in hidden field

            this.PopulateComponents();

        }

        public void populate()
        {
            //Fills Sub build with opt groups and option tags
            REATrackerDB sql = new REATrackerDB();
            //Sets the ReleaseReportInput section
            String myChildren =
            @"
;WITH Test AS
(
SELECT DISTINCT CHILD_ID 
FROM ST_PRODUCT_RELATION 
WHERE PARENT_ID = " + Convert.ToString(this.ProductID) +
@" UNION ALL
SELECT ST_PRODUCT_RELATION.CHILD_ID FROM ST_PRODUCT_RELATION
INNER JOIN Test ON ST_PRODUCT_RELATION.PARENT_ID = Test.CHILD_ID
WHERE PARENT_ID = Test.CHILD_ID
)SELECT DISTINCT PRODUCT_ID, NAME FROM Test
INNER JOIN ST_PRODUCT ON TEST.CHILD_ID = ST_PRODUCT.PRODUCT_ID
ORDER BY ST_PRODUCT.NAME
";
            DataTable dt = sql.ProcessCommand(myChildren);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                /*
                //Row gets eeach fo the Products
                String product_name_sql = "SELECT NAME FROM ST_PRODUCT WHERE PRODUCT_ID= " + Convert.ToString(row[0]);
                String subBuildProductName = Convert.ToString(sql.ProcessScalarCommand(product_name_sql));
                SubBuildItem Curr = new SubBuildItem("<label><strong>" + subBuildProductName + "</strong><label>");
                
                //productname is the optgroup
                String Version_Sql = 
                    "SELECT TOP 5 (MAJOR + '.' +MINOR + '.' + BUILD )AS VERSION_ID, BUILD_ID FROM ST_PRODUCT_RELEASE WHERE (PRODUCT_ID= " +
                    Convert.ToString(row[0]) +
                    ") ORDER BY BUILD_ID DESC";
                DataTable Versions = sql.ProcessCommand(Version_Sql);
                foreach(System.Data.DataRow Version in Versions.Rows )
                {
                    //Versions gets the last 5 version of the product.
                    this.SubBuildValues.Add(new RelatedBinder(false, Convert.ToInt32(Version[1])));
                    String IdName = "SubBuildValues[" + Convert.ToString(SubBuildValues.Count() - 1) + "]";
                    Curr.BuildVersions.Add(
                    "<label><input type='checkbox' id='" + IdName + ".selected' name='" +
                        IdName + ".selected' value='true'/>&nbsp;&nbsp;&nbsp;" 
                        + Convert.ToString(Version[0]) + "</label>" +
                    "<input id='" + IdName + ".BuildID' name='" + IdName +
                        ".BuildID' type='hidden' value ='" + Convert.ToString(Version[1]) + "' >"
                        );
                }//First Layer
                //add the SubBuild of those products
                */
                this.SubBuild.Add(this.populateHelper((int)row["PRODUCT_ID"]));
            }
            this.defaultTester();
            this.populateSCRs();
        }

        private SubBuildItem populateHelper(int CurrProduct)
        {
            REATrackerDB sql = new REATrackerDB();
            //Row gets eeach fo the Products

            //Create Current product
            String product_name_sql = "SELECT NAME FROM ST_PRODUCT WHERE PRODUCT_ID= " + Convert.ToString(CurrProduct);
            String subBuildProductName = Convert.ToString(sql.ProcessScalarCommand(product_name_sql));
            SubBuildItem result = new SubBuildItem(subBuildProductName);
            String Version_Sql = @"
SELECT
    CASE WHEN MISC IS NULL
        THEN(MAJOR + '.' + MINOR + '.' + BUILD)
        ELSE
            CASE WHEN ISNUMERIC(MISC) = 1
            THEN(MAJOR + '.' + MINOR + '.' + BUILD + '.' + MISC)
            ELSE(MAJOR + '.' + MINOR + '.' + BUILD + ' ' + MISC)
            END
        END
AS VERSION_ID,
BUILD_ID,
BUILT_ON
FROM ST_PRODUCT_RELEASE WHERE (PRODUCT_ID = " + CurrProduct + ") ORDER BY BUILD_ID DESC";

            DataTable Versions = sql.ProcessCommand(Version_Sql);
            foreach (System.Data.DataRow Version in Versions.Rows)
            {
                //Versions gets the last 5 version of the product.
                //this.SubBuildValues.Add(new RelatedBinder(false, Convert.ToInt32(Version[1])));
                //String IdName = "SubBuildValues[" + Convert.ToString(SubBuildValues.Count() - 1) + "]";
                // Create the Select tag
                result.BuildVersions.Add(
                    //Value, Name
                    new Tuple<string, string>(Convert.ToString(Version[1]), Convert.ToString(Version[0]) + " - BUILT ON " + Convert.ToDateTime(Version[2]).ToString("MM/dd/yyyy HH:mm:ss"))
                    );
                /*
                result.BuildVersions.Add(
                "<label><input type='checkbox' id='" + IdName + ".selected' name='" +
                    IdName + ".selected' value='true'/>&nbsp;&nbsp;&nbsp;"
                    + Convert.ToString(Version[0]) + "</label>" +
                "<input id='" + IdName + ".BuildID' name='" + IdName +
                    ".BuildID' type='hidden' value ='" + Convert.ToString(Version[1]) + "' >"
                    );*/
            }
            return result;
        }

        private void defaultTester()
        {
            //Gets default tester option tag
            //Werid but is the equal to lines 98-120 in ReleaseReportInput.ssp
            REATrackerDB sql = new REATrackerDB();
            String getProductInfo = "SELECT DEFAULT_TESTER, DISTRIBUTION_LIST FROM ST_PRODUCT WHERE PRODUCT_ID = " + Convert.ToString(this.ProductID);
            DataTable ProductInfoDataTable = sql.ProcessCommand(getProductInfo);
            //Get a list of all the tesers
            String getAllTesters = "SELECT DISTINCT ST_USERS.USER_ID,  (ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME) AS Tester  " +
                                    "FROM ST_USERS INNER JOIN ST_PRODUCT_TEAM ON " +
                                    "ST_USERS.USER_ID = ST_PRODUCT_TEAM.USER_ID " +
                                    "WHERE (ACCOUNT_TYPE <> -1 ) AND ( ((ST_PRODUCT_TEAM.ROLE= 4 ) AND (ST_PRODUCT_TEAM.PRODUCT=" + Convert.ToString(this.ProductID) + ")) ";
            if (ProductInfoDataTable.Rows.Count > 0 && ProductInfoDataTable.Rows[0][0] != DBNull.Value)
            {
                getAllTesters += (" OR (ST_USERS.USER_ID = " + Convert.ToString(ProductInfoDataTable.Rows[0][0]) + ")");
            }
            else
            {
                this.Testers.Add("<option value = 0 > </option>");
            }
            getAllTesters += ")";
            DataTable DefaultTesterDataTable = sql.ProcessCommand(getAllTesters);
            foreach (System.Data.DataRow row in DefaultTesterDataTable.Rows)
            {
                if (ProductInfoDataTable.Rows.Count > 0)
                {
                    this.Testers.Add("<option value =" + Convert.ToString(row[0]) + (Convert.ToString(row[0]) == Convert.ToString(ProductInfoDataTable.Rows[0][0]) ? " Selected " : "") + "> " + Convert.ToString(row[1]) + " </option>");
                }
                else
                {
                    this.Testers.Add("<option value =" + Convert.ToString(row[0]) + "> " + Convert.ToString(row[1]) + " </option>");
                }
            }
        }

        public void populateReleaseType()
        {
            this.ReleaseTypes = new List<dynamic>();
            string command = "SELECT ID, NAME FROM ST_PRODUCT_RELEASE_TYPE";
            DataTable dt = new REATrackerDB().ProcessCommand(command);
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                this.ReleaseTypes.Add(new System.Dynamic.ExpandoObject());
                this.ReleaseTypes[i].Value = Convert.ToString(dr["ID"]);
                this.ReleaseTypes[i].Name = Convert.ToString(dr["NAME"]);
                this.ReleaseTypes[i].Selected = (Convert.ToInt32(dr["ID"]) == this.ReleaseType ? "selected" : "");
                i++;
            }
        }

        public void populateSCRs()
        {
            REATrackerDB sql = new REATrackerDB();
            String ReleaseCoordinatorCommand = "SELECT RELEASE_COORDINATOR, RELEASE_COORDINATOR_2 FROM ST_PRODUCT WHERE (PRODUCT_ID=" + Convert.ToString(this.ProductID) + ")";
            this.ReleaseCoorindatorID = Convert.ToInt32(sql.ProcessScalarCommand(ReleaseCoordinatorCommand));
            String SCRInfoCommand =
                "SELECT ST_TRACK.TRACKING_ID AS TRACKING_ID, ST_ISSUE_TYPE.NAME AS ISSUE_TYPE, ST_TRACK.VR_MAJOR AS VR_MAJOR, " +
                "ST_TRACK.VR_MINOR AS VR_MINOR, ST_TRACK.VR_BUILD AS VR_BUILD, ST_TRACK.CUSTOMER_BUG AS COS_BUG, ST_TRACK.CUSTOMER_NAME AS COS_NAME, " +
                "ST_priority.NAME AS priority, ST_TRACK.REQ_DOC_CHANGE AS REQ_DOC_CHANGE, " +
                "ST_TRACK.REQ_DB_CHANGE AS REQ_DB_CHANGE, ST_TRACK.TITLE AS TITLE, " +
                "ST_TRACK.RESOLVED_BY AS RESOLVED_BY, ST_TRACK.ASSIGNED_TO AS ASSIGNED_TO FROM ST_TRACK INNER JOIN " +
                "ST_priority ON ST_TRACK.priority = ST_priority.ID JOIN ST_ISSUE_TYPE ON ST_TRACK.ISSUE_TYPE= ST_ISSUE_TYPE.ISSUE_ID " +
                "WHERE ((ST_TRACK.STATUS=7) AND (ST_TRACK.PRODUCT=" + Convert.ToString(this.ProductID) + ") " +
                " AND (ST_TRACK.VR_MAJOR=" + Convert.ToString(this.Major) + ") AND (ST_TRACK.VR_MINOR=" + Convert.ToString(this.Minor) + ") " +
                " AND (ST_TRACK.ASSIGNED_TO=" + Convert.ToString(this.ReleaseCoorindatorID) + " )) ORDER BY ST_TRACK.TRACKING_ID ASC";
            if (this.Major != null && this.Minor != null)
            {
                DataTable DeliveredSCRs = sql.ProcessCommand(SCRInfoCommand);
                int counter = 0;
                foreach (System.Data.DataRow row in DeliveredSCRs.Rows)
                {
                    this.SCRs.Add(new DisplaySCRLine(row));
                    counter++;
                }
            }
        }

        public bool GetRelease(int releaseID)
        {
            bool exists = false;
            REATrackerDB sql = new REATrackerDB();
            String command = "SELECT * FROM ST_PRODUCT_RELEASE WHERE BUILD_ID = " + releaseID;
            DataTable dt = sql.ProcessCommand(command);
            if (dt.Rows.Count > 0)
            {
                this.ReleaseId = releaseID;
                exists = true;
                this.ProductID = Convert.ToInt32(dt.Rows[0]["PRODUCT_ID"]);
                this.ProductName = Convert.ToString(sql.ProcessScalarCommand("SELECT NAME FROM ST_PRODUCT WHERE ST_PRODUCT.PRODUCT_ID = " + this.ProductID));
                this.Major = Convert.ToInt32(dt.Rows[0]["MAJOR"]);
                this.Minor = Convert.ToInt32(dt.Rows[0]["MINOR"]);
                this.Build = dt.Rows[0]["BUILD"] == DBNull.Value ? -1 : Convert.ToInt32(dt.Rows[0]["BUILD"]); //some builds are null
                this.Misc = Convert.ToString(dt.Rows[0]["MISC"]);
                this.Version = this.Major + "." + this.Minor +
                    (this.Build == -1 ? "" : Convert.ToString("." + this.Build)) +
                    (string.IsNullOrEmpty(this.Misc) ? "" : Convert.ToString(" " + this.Misc));
                this.BuiltOn = Convert.ToDateTime(dt.Rows[0]["BUILT_ON"]);
                this.ReleaseCoorindatorID = Convert.ToInt32(dt.Rows[0]["RELEASE_COORDINATOR_ID"]);
                this.BuiltByName = Convert.ToString(sql.ProcessScalarCommand("SELECT (FIRST_NAME+' '+ LAST_NAME) FROM ST_USERS WHERE ST_USERS.USER_ID = " + Convert.ToString(this.ReleaseCoorindatorID)));
                //this.Tester;
                this.NewNotes = Convert.ToString(dt.Rows[0]["NOTES"]);
                //SCR List populate selected items
                this.ReleasedSCRs = Convert.ToString(dt.Rows[0]["SCR_LIST"]);
                //Selected Releases
                this.DisplaySubBuild = Convert.ToBoolean(dt.Rows[0]["DISPLAY_RELATED_REPORT"]);
                this.URL = Convert.ToString(dt.Rows[0]["URL"] == DBNull.Value ? "" : dt.Rows[0]["URL"]);
                this.ReleaseType = Convert.ToInt32(dt.Rows[0]["IS_CUSTOMER_RELEASE"]);
                this.DBVersion = Convert.ToString(dt.Rows[0]["DB_VERSION"] == DBNull.Value ? "" : dt.Rows[0]["DB_VERSION"]);
            }
            this.populateReleaseType();
            return exists;
        }

        public bool UpdateComponents()
        {
            bool success = false;

            REATrackerDB sql = new REATrackerDB();
            try
            {
                foreach (Component aComponent in this.Components)
                {
                    success = sql.UpdateComponent(aComponent.ReleaseRelationId, aComponent.RelatedBuildId);
                }
            }
            catch
            {
                success = false;
            }
            finally
            {
                if (sql != null)
                {
                    sql.CloseConnection();
                    sql = null;
                }
            }
            return success;
        }

        public bool UpdateRelease()
        {
            bool success = false;

            REATrackerDB sql = new REATrackerDB();
            try
            {
                success = sql.UpdateProductRelease(this.ReleaseId, this.ReleaseType, this.ReleasedSCRs, this.NewNotes, this.DisplaySubBuild, this.URL, this.DBVersion);
            }
            catch
            {                
                success = false;
            }
            finally
            {
                if (sql != null)
                {
                    sql.CloseConnection();
                    sql = null;
                }
            }
            return success;
        }
        /*
        //private void GetRelatedBuilds(int buildId) 
        //{
        //    if(this.SubBuild != null && this.SubBuild.Count != 0)
        //    {
        //        String Command = "SELECT * FROM ST_PRODUCT_RELEASE_RELATION WHERE BUILD_ID = "+buildId+";";
        //        REATrackerHelp sql = new REATrackerHelp();
        //        DataTable RealatedBuildDT = sql.ProcessCommand(Command);
        //        foreach (System.Data.DataRow row in RealatedBuildDT.Rows)
        //        {
        //            int indexSubBuild;
        //            if (this.SubBuildValues.Where(x => x.BuildID == Convert.ToInt32(row["RELATED_BUILD_ID"])).Count() !=0 ) 
        //            {
        //                this.SubBuildValues.Where(x => x.BuildID == Convert.ToInt32(row["RELATED_BUILD_ID"])).First().selected = true;
        //            }
        //            if ( this.SubBuild.Where(x=>x.Contains("value='"+ Convert.ToInt32(row["RELATED_BUILD_ID"]) +"'")).Count() > 0 )
        //            {

        //                this.SubBuild.Where(x => x.Contains(
        //                        "value='" + Convert.ToInt32(row["RELATED_BUILD_ID"]) + "'")
        //                        ).First().Replace(
        //                                "value='" + Convert.ToInt32(row["RELATED_BUILD_ID"]) + "'",
        //                                "value='" + Convert.ToInt32(row["RELATED_BUILD_ID"]) + "' checked"
        //                                );
        //            }
        //        }
                
        //    }
        //}
        */


        //post
        public int save()
        {
            int Build_ID = 0;
            REATrackerDB sql = new REATrackerDB();
            //the old Site IS_CUSTOMER_RELEASE IS SET to 0
            String SCRList = "";
            foreach (var item in this.SCRs)
            {
                if (item.Selected)
                {
                    SCRList += item.TrackingID + ",";
                }
            }
            if (!String.IsNullOrEmpty(SCRList) && SCRList[SCRList.Count() - 1] == ',')
            {
                SCRList = SCRList.Remove((SCRList.Count() - 1));
            }
            this.SelectedReleases = new List<int>();
            if (this.SubBuildValues == null)
            {
                this.SubBuildValues = new List<RelatedBinder>();
            }
            foreach (var item in this.SubBuildValues)
            {
                this.SelectedReleases.Add(item.BuildID);
            }
            Build_ID = Convert.ToInt32(
                        sql.SaveProductRelease(this.ProductID,
                        (int)this.Major, (int)this.Minor, (int)this.Build, this.Misc, 1,
                        this.StUserID, this.Tester, this.NewNotes, SCRList, this.DBVersion, this.SelectedReleases, this.DisplaySubBuild
                            ).Rows[0][0]
                    );
            //sql.SendEmail("Build Release");
            return Build_ID;
        }

        public class SubBuildItem
        {
            public List<Tuple<string, string>> BuildVersions { get; set; }
            public string CurrentString { get; set; }

            public SubBuildItem()
            {
                this.init();
            }

            public SubBuildItem(string argsCurrent)
            {
                CurrentString = argsCurrent;
                this.init();
            }

            private void init()
            {
                this.BuildVersions = new List<Tuple<string, string>>();
            }

        }

        public class Component
        {
            public String ComponentName { get; set; }
            public String ComponentVersion { get; set; }
            public string ReleaseId { get; set; }
            public Int32 ReleaseRelationId { get; set; }
            public Int32 RelatedBuildId { get; set; }
            public List<String> Versions { get; set; }
            public List<Int32> BuildId { get; set; }
        }

        public class DisplaySCRLine
        {
            public bool Selected { get; set; }
            public String TrackingID { get; set; }
            public String VersionReleaseMajor { get; set; }
            public String VersionReleaseMinor { get; set; }
            public String VersionReleaseBuild { get; set; }
            public String CustomerBug { get; set; }
            public String CustomerName { get; set; }
            public String DocumentChange { get; set; }
            public String RequiredDataBaseChange { get; set; }
            public String ResolvedBy { get; set; }
            public String Title { get; set; }
            public String IssueType { get; set; }
            public String Priority { get; set; }
            public DisplaySCRLine() { }

            private string DocumentChangeToString(int x)
            {
                string value = "?";
                switch(x)
                {
                    case 0: { value = "NO"; break; }
                    case 1: { value = "YES"; break; }
                    case 2: { value = "DONE"; break; }
                    case 3: { value = "TBD"; break; }
                }
                return value;
            }
            private string DocumentChangeToString(object x)
            {
                string value = "?";
                try
                {
                    if (x != DBNull.Value)
                    {
                        value = DocumentChangeToString((int)x);
                    }
                }
                catch
                {

                }
                return value;
            }
            public DisplaySCRLine(System.Data.DataRow row)
            {
                this.Selected = false;
                this.TrackingID = Convert.ToString(row["TRACKING_ID"]);
                this.VersionReleaseMajor = (row["VR_MAJOR"] == DBNull.Value ? "?" : Convert.ToString(row["VR_MAJOR"]));
                this.VersionReleaseMinor = (row["VR_MINOR"] == DBNull.Value ? "?" : Convert.ToString(row["VR_MINOR"]));
                this.VersionReleaseBuild = (row["VR_BUILD"] == DBNull.Value ? "?" : Convert.ToString(row["VR_BUILD"]));
                this.CustomerBug = (Convert.ToBoolean(row["COS_BUG"]) == true ? "true" : "false");
                this.CustomerName = Convert.ToString(row["COS_NAME"] == DBNull.Value ? "???" : row["COS_NAME"]);
                this.DocumentChange = DocumentChangeToString(row["REQ_DOC_CHANGE"]);
                this.RequiredDataBaseChange = (Convert.ToBoolean(row["REQ_DB_CHANGE"]) == true ? "true" : "false");
                this.ResolvedBy = (row["RESOLVED_BY"] == DBNull.Value ? "???" : this.getFullName((Convert.ToInt32(row["RESOLVED_BY"]))));
                this.Title = Convert.ToString(row["TITLE"]);
                this.IssueType = Convert.ToString(row["ISSUE_TYPE"]);
                this.Priority = Convert.ToString(row["priority"]);
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
        }

        public class RelatedBinder
        {
            public int BuildID { get; set; }

            public Boolean selected { get; set; }

            public RelatedBinder(Boolean selected, int id)
            {
                this.BuildID = id;
                this.selected = selected;
            }

            public RelatedBinder() { }
        }
    }

}