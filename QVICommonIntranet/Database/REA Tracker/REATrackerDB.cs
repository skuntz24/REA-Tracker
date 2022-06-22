using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;
using QVICommonIntranet.Email.EmailREATracker;
using System.Text;
using System.Threading.Tasks;

namespace QVICommonIntranet.Database
{
    public partial class REATrackerDB : GenericDB
    {
        public REATrackerDB()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["reatrackerdb"].ConnectionString;
            _connectionStringSCR = ConfigurationManager.ConnectionStrings["btracker"].ConnectionString;
            
        }

        public REATrackerDB(string connectionString)
        {

            _connectionString = connectionString;

        }

        /*
       #region SQL COLUMN HELPERS
       //submitted, approved, in process, fixed, delivered ||| testing, built
       private static string SQL_WHERE_STATUS_ALL = "REA_TRACK.STATUS IN (1,2,5,6,7,8,11)";
       private static string SQL_WHERE_STATUS_DEV = "REA_TRACK.STATUS IN (1,2,5,6,7)";
       private static string SQL_COL_PLANNED_RELEASE  = "(ISNULL(REA_TRACK.VP_MAJOR, '?') + '.' + ISNULL(REA_TRACK.VP_MINOR, '?') + '.' + ISNULL(REA_TRACK.VP_BUILD, '?') + ' ' + ISNULL(REA_TRACK.VP_MISC, '')) AS 'Planned Release'";
       private static string SQL_COL_FOUND_RELEASE = "(ISNULL(REA_TRACK.VF_MAJOR, '?') + '.' + ISNULL(REA_TRACK.VF_MINOR, '?') + '.' + ISNULL(REA_TRACK.VF_BUILD, '?') + ' ' + ISNULL(REA_TRACK.VF_MISC, '')) AS 'Version Found'";
       private static string SQL_COL_RESOLVED_RELEASE = "(ISNULL(REA_TRACK.VR_MAJOR, '?') + '.' + ISNULL(REA_TRACK.VR_MINOR, '?') + '.' + ISNULL(REA_TRACK.VR_BUILD, '?') + ' ' + ISNULL(REA_TRACK.VR_MISC, '')) AS 'Version Resolved'";
       private static string SQL_COL_CLOSED_RELEASE = "(ISNULL(REA_TRACK.VC_MAJOR, '?') + '.' + ISNULL(REA_TRACK.VC_MINOR, '?') + '.' + ISNULL(REA_TRACK.VC_BUILD, '?') + ' ' + ISNULL(REA_TRACK.VC_MISC, '')) AS 'Version Closed'";
       #endregion
       */

        public enum HistoryChangeType
        {
            chg_detail = 1,
            chg_note = 2,
            chg_resolution = 3,
            chg_system = 4,
            chg_assignto = 5,
            chg_issuetype = 6,
            chg_exception = 7,
            chg_related = 8,
            chg_priority = 9,
            chg_planned_release = 10,
            chg_actual_release = 11,
            chg_estimate_to_fix = 12,
            chg_actual_to_fix = 13,
            chg_estimate_to_test = 14,
            chg_actual_to_test = 15,
            chg_ser_priority = 16,
            chg_ser_problem = 17,
            chg_ser_solution = 18,
            chg_ser_benefits = 19,
            chg_estimate_to_spec = 20,
            chg_actual_to_spec = 21,
            chg_status = 22,
            chg_documentation = 23,
            chg_requirements = 24,
            chg_database = 25,
            chg_attachment_add = 26,
            chg_product = 27,
            chg_rank = 28,
            chg_size = 29
        }
        public enum StatusChangeID
        {
            status_submitted = 1,
            status_approved = 2,
            status_deferred = 3,
            status_rejected = 4,
            status_inprocess = 5,
            status_fixed = 6,
            status_delivered = 7,
            status_testing = 8,
            status_closed = 9,
            status_onhold = 10,
            status_built = 11,
            status_acknowledged = 12,
            status_pending = 13
        }
        public enum DisplayName
        {
            FirstNameLastName = 0,
            LastNameFirstName = 1,
            DefaultDisplay = FirstNameLastName
        }
        public enum IssueChangeType
        {
            Problems = 1,
            Enhancements = 2,
            PlannedWork = 3

        }
        public enum SupportType
        {
            [Display(Name = "Normal")]
            Normal = 0,
            [Display(Name = "Support")]
            Support = 1,
            [Display(Name = "R&D")]
            RandD = 2
        }
        public struct ProductMetricsInfo
        {
            public bool Display;
            public int MajorVersion;
            public int MinorVersion;
            public int ReleaseCriticalCount;
            public int ReleaseHighCount;
            public int ReleaseDefectCount;
            public int ReleaseDefectMetric;
            public int CodeFreezeCriticalCount;
            public int CodeFreezeHighCount;
            public int CodeFreezeDefectCount;
            public int CodeFreezeDefectMetric;

            public string Label;
            public string LabelID;
            public int CurrentHighCount;
            public int CurrentCriticalCount;
            public int CurrentDefectCount;
            public int CurrentDefectMetric;
            public int PostBuildHighCount;
            public int PostBuildCriticalCount;
            public int PostBuildDefectCount;
            public int PostBuildDefectMetric;
        }

        public struct ProductInfo
        {
            public int ID;
            public string Name;
            public string Description;
            public int? ValidatorID;
            public string ValidatorName;
            public int? ReleaseCoordinatorID;
            public string ReleaseCoordinatorName;
            public int? ReleaseCoordinator2ID;
            public string ReleaseCoordinator2Name;
            public int? DefaultTesterID;
            public string DefaultTesterName;
            public int? SCCBID;
            public string SCCBName;
            public bool IsInternal;
            public bool IsRetired;
            public int BillingCode;
            public string DistributionList;
            public bool HasMetrics;
            public int? OwnerID;
            public string OwnerName;
            public ProductMetricsInfo[] Metrics;
        }

        public struct ProductDashboardReports
        {
            public int Submitted;
            public int Approved;
            public int InProcess;
            public int FixedDelivered;
            public int Testing;
            public int Deferred;

            public int Problems;
            public int Enhancements;
            public int PlannedWork;

            public int Low;
            public int Medium;
            public int High;
            public int Critical;
        }

        public struct VersionInfo
        {
            public int? Major;
            public int? Minor;
            public int? Build;
            public string Misc;

            public override string ToString()
            {
                string output = "";
                output += (Major == null) ? "?" : (Major == -1) ? "?" : Major.ToString();
                output += ".";
                output += (Minor == null) ? "?" : (Minor == -1) ? "?" : Minor.ToString();
                output += ".";
                output += (Build == null) ? "?" : (Build == -1) ? "?" : Build.ToString();
                if (!string.IsNullOrWhiteSpace(Misc))
                {
                    //if the field is a number, show all 4 numbers as x.x.x.x
                    int result = 0;
                    if (int.TryParse(Misc, out result))
                    {
                        output += "." + result.ToString();
                    }
                    else
                    {
                        //the last is not a number, show as x.x.x x
                        output += " " + Misc;
                    }
                }
                return output;
            }

            public void Clear()
            {
                Major = null; Minor = null; Build = null; Misc = String.Empty;
            }
        }

        public class ReleaseVersion
        {
            public VersionInfo Version;
            public DateTime? BuiltOn { get; set; }
            public String ModalString { get; set; }


            public int Status { get; set; }
            /*
             * Status:
             * 1 - Internal Release Only
             * 2 - Customer Release
             * 3 - Customer Release for Limited Distribution
             * 4 - CustomerReelase For Restrocted Distribution
             * 5 - Recalled Customer Release
             * 6 - Manufacturing Distribution
             * 7 - Outgoing Inspection Distribution
             */
            public List<int> REAsFixed { get; set; }
            public String Notes { get; set; }
            public String DB_Version { get; set; }
            public int Build_id { get; set; }
            public ReleaseVersion(int ArgsBuildId, VersionInfo ArgsVersion, DateTime? ArgsbuiltOn, int ArgsCustomerRelease, String ArgsNotes, String ArgsREAList, String DB_Version)
            {
                this.Version = ArgsVersion;
                this.BuiltOn = ArgsbuiltOn;
                this.Status = ArgsCustomerRelease;
                this.Notes = ArgsNotes;
                this.REAsFixed = (String.IsNullOrEmpty(ArgsREAList) ? new List<int>() : ArgsREAList.Split(',').Select(x => Convert.ToInt32(x)).ToList());
                this.ModalString = "myModal" + Version.Major + "_" + Version.Minor + "_" + Version.Build;
                this.Build_id = ArgsBuildId;
                this.DB_Version = DB_Version;
            }

            public ReleaseVersion(int ArgsBuildId, int ArgsMajor, int ArgsMinor, int ArgsBuild, string ArgsMisc, DateTime? ArgsbuiltOn, int ArgsCustomerRelease, String ArgsNotes, String ArgsREAList, String DB_Version)
            {
                this.Version.Major = ArgsMajor;
                this.Version.Minor = ArgsMinor;
                this.Version.Build = ArgsBuild;
                this.Version.Misc = ArgsMisc;
                this.BuiltOn = ArgsbuiltOn;
                this.Status = ArgsCustomerRelease;
                this.Notes = ArgsNotes;
                this.REAsFixed = (String.IsNullOrEmpty(ArgsREAList) ? new List<int>() : ArgsREAList.Split(',').Select(x => Convert.ToInt32(x)).ToList());
                this.ModalString = "myModal" + Version.Major + "_" + Version.Minor + "_" + Version.Build;
                this.Build_id = ArgsBuildId;
                this.DB_Version = DB_Version;
            }
        }

        public struct HoursInfo
        {
            public double? Specification;
            public double? Implementation;
            public double? Testing;
        }

        public struct CustomerInfo
        {
            public bool IsReported;
            public string Company;
            public string Location;
            public string ContactPerson;
            public string Syteline;
            public string MachineSN;
        }

        public struct EnhancementInfo
        {
            public string Problem;
            public string Solution;
            public string Benefit;
        }

        public struct ProjectInfo
        {
            public int ID, OwnerID, Support;
            public string Code, Name, Description, OwnerName;
            public bool Active, IsValidID;
            public string CreateDate, StartDate, EndDate;
        }

        public struct REA
        {
            public string TransactionID;
            public int submitted_by;
            //General Settings
            public int product;
            public int versionfound_major;
            public int versionfound_minor;
            public int versionfound_build;
            public string versionfound_misc;
            public int assigned_to_ID;
            public int module_id;
            public int issue_id;
            public int priority_id;
            public string title;
            public int Requires_Document_change;
            public bool Requires_Requirement_change;
            public string Description;
            //Customer Information
            public bool CustomerBug;
            public string CustomerName;
            public string CustomerLocation;
            public string CustomerCompany;
            public string SytelineRef;
            public string MachineSN;
            //Enhancement details
            public string Problem;
            public string Solution;
            public string Benifit;
            //Planning Details
            public double? EstimateHoursToFix;
            public double? EstimateHoursToTest;
            public double? EstimateHoursToSpec;
            public int? VersionPlannedMajor;
            public int? VersionPlannedMinor;
            public int? VersionPlannedBuild;
            public string VersionPlannedMisc;
            public int? Rank;
            public int? Size;
            //
            public VersionInfo VersionFound;
            public VersionInfo VersionPlanned;
            public HoursInfo HoursEstimated;
            public CustomerInfo Customer;
            public EnhancementInfo Enhancement;

            public int SubmittedByID;
            public int ProductID;
            public int IssueTypeID;
            public int PriorityID;
            public int ModuleID;
            //public int SeverityID;

            public string Title;
            //public string Description;

            public bool NeedsDocumentationChange;
            public bool NeedsRequirementsChange;
            //public bool NeedsDatabaseChange;
        }
        public int InsertREA(REA data)
        {
            ///<summary>
            /// Insert a REA into the Database
            ///</summary>
            ///
            ///<param name="data">
            /// REA Struture that holds all the data to insert into the database.
            ///</param>

            //Creating SQL Parameters for certain variables that cna break SQL commands
            SqlParameter Title = new SqlParameter();
            SqlParameter vf_misc = new SqlParameter();
            SqlParameter Description = new SqlParameter();
            SqlParameter CustomerName = new SqlParameter();
            SqlParameter CustomerLocation = new SqlParameter();
            SqlParameter CustomerCompany = new SqlParameter();
            SqlParameter SyteLineRef = new SqlParameter();
            SqlParameter Problem = new SqlParameter();
            SqlParameter Benifit = new SqlParameter();
            SqlParameter Solution = new SqlParameter();
            SqlParameter vp_misc = new SqlParameter();
            SqlParameter Machine = new SqlParameter();
            //setting up the types

            Title.SqlDbType = SqlDbType.VarChar;
            vf_misc.SqlDbType = SqlDbType.VarChar;
            Description.SqlDbType = SqlDbType.NText;
            CustomerName.SqlDbType = SqlDbType.VarChar;
            CustomerLocation.SqlDbType = SqlDbType.VarChar;
            CustomerCompany.SqlDbType = SqlDbType.VarChar;
            SyteLineRef.SqlDbType = SqlDbType.NVarChar;
            Problem.SqlDbType = SqlDbType.NVarChar;
            Benifit.SqlDbType = SqlDbType.NVarChar;
            Solution.SqlDbType = SqlDbType.NVarChar;
            vp_misc.SqlDbType = SqlDbType.VarChar;
            Machine.SqlDbType = SqlDbType.VarChar;

            //setting parameter name
            Title.ParameterName = "TITLE";
            vf_misc.ParameterName = "VF_MISC";
            Description.ParameterName = "DESCRIPTION";
            CustomerName.ParameterName = "CUSTOMERNAME";
            CustomerLocation.ParameterName = "CUSTOMER_LOCATION";
            CustomerCompany.ParameterName = "CUSTOMER_COMPANY";
            SyteLineRef.ParameterName = "SYTELINE_REF";
            Problem.ParameterName = "PROBLEM";
            Benifit.ParameterName = "BENIFIT";
            Solution.ParameterName = "SOLUTION";
            vp_misc.ParameterName = "VP_MISC";
            Machine.ParameterName = "MACHINE_SN";

            Title.Value = data.title.Trim();
            vf_misc.Value = (String.IsNullOrEmpty(Convert.ToString(data.versionfound_misc as object).Trim())) ? (object)DBNull.Value : Convert.ToString(data.versionfound_misc).Trim();
            Description.Value = (String.IsNullOrEmpty(Convert.ToString(data.Description as object).Trim())) ? (object)DBNull.Value : Convert.ToString(data.Description).Trim();
            CustomerName.Value = (String.IsNullOrEmpty(Convert.ToString(data.CustomerName as object).Trim())) ? (object)DBNull.Value : Convert.ToString(data.CustomerName).Trim();
            CustomerLocation.Value = (String.IsNullOrEmpty(Convert.ToString(data.CustomerLocation as object).Trim())) ? (object)DBNull.Value : Convert.ToString(data.CustomerLocation).Trim();
            CustomerCompany.Value = (String.IsNullOrEmpty(Convert.ToString(data.CustomerCompany as object).Trim())) ? (object)DBNull.Value : Convert.ToString(data.CustomerCompany).Trim();
            SyteLineRef.Value = (String.IsNullOrEmpty(Convert.ToString(data.SytelineRef as object).Trim())) ? (object)DBNull.Value : Convert.ToString(data.SytelineRef).Trim();
            Problem.Value = (String.IsNullOrEmpty(Convert.ToString(data.Problem as object).Trim())) ? (object)DBNull.Value : Convert.ToString(data.Problem).Trim();
            Benifit.Value = (String.IsNullOrEmpty(Convert.ToString(data.Benifit as object).Trim())) ? (object)DBNull.Value : Convert.ToString(data.Benifit).Trim();
            Solution.Value = (String.IsNullOrEmpty(Convert.ToString(data.Solution as object).Trim())) ? (object)DBNull.Value : Convert.ToString(data.Solution).Trim();
            vp_misc.Value = (String.IsNullOrEmpty(Convert.ToString(data.VersionPlannedMisc as object).Trim())) ? (object)DBNull.Value : Convert.ToString(data.VersionPlannedMisc).Trim();
            Machine.Value = (String.IsNullOrEmpty(Convert.ToString(data.MachineSN as object).Trim()) ? (object)DBNull.Value : Convert.ToString(data.MachineSN).Trim());


            int tracking_id = 0;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                //Start
                String sqlString = "EXEC InsertREA " +
                    "'" + Convert.ToString(data.TransactionID) + "', " +
                    Convert.ToString(data.submitted_by) + ", " +
                    Convert.ToString(data.product) + ", " +
                    "'" + Convert.ToString(data.versionfound_major) + "', " +
                    "'" + Convert.ToString(data.versionfound_minor) + "', " +
                    "'" + Convert.ToString(data.versionfound_build) + "', " +
                    "" + "@VF_MISC" + ", " +
                    Convert.ToString(data.assigned_to_ID) + ", " +
                    Convert.ToString(data.module_id) + ", " +
                    Convert.ToString(data.issue_id) + ", " +
                    Convert.ToString(data.priority_id) + ", " +
                    "" + "@TITLE" + ", " +
                    Convert.ToString(data.Requires_Document_change) + ", " +
                    Convert.ToString(data.Requires_Requirement_change) + ", " +
                    "" + "@DESCRIPTION" + ", " +
                    //Customer Information
                    Convert.ToString(data.CustomerBug) + ", " +
                    "" + "@CUSTOMERNAME" + ", " +
                    "" + "@CUSTOMER_COMPANY" + ", " +
                    "" + "@CUSTOMER_LOCATION" + ", " +
                    "" + "@SYTELINE_REF" + ", " +
                    "" + "@MACHINE_SN" + ", " +
                    //Enhancement details
                    "" + "@PROBLEM" + ", " +
                    "" + "@SOLUTION" + ", " +
                    "" + "@BENIFIT" + ", " +
                                //Planning Details
                                (Convert.ToString(data.EstimateHoursToFix) == "" ? "NULL" : Convert.ToString(data.EstimateHoursToFix)) + ", " +
                                (Convert.ToString(data.EstimateHoursToTest) == "" ? "NULL" : Convert.ToString(data.EstimateHoursToTest)) + ", " +
                                (Convert.ToString(data.EstimateHoursToSpec) == "" ? "NULL" : Convert.ToString(data.EstimateHoursToSpec)) + ", " +
                         (Convert.ToString(data.VersionPlannedMajor) == "" ? "NULL" : ("'" + Convert.ToString(data.VersionPlannedMajor) + "'")) + ", " +
                    "" + (Convert.ToString(data.VersionPlannedMinor) == "" ? "NULL" : ("'" + Convert.ToString(data.VersionPlannedMinor) + "'")) + ", " +
                    "" + (Convert.ToString(data.VersionPlannedBuild) == "" ? "NULL" : ("'" + Convert.ToString(data.VersionPlannedBuild) + "'")) + ", " +
                    "" + "@VP_MISC" + ", " +
                    (Convert.ToString(data.Rank) == "" ? "NULL" : Convert.ToString(data.Rank)) + ", " +
                    (Convert.ToString(data.Size) == "" ? "NULL" : Convert.ToString(data.Size))

                    ;
                SqlCommand command = new SqlCommand(sqlString, connection);
                command.Parameters.Add(Title);
                command.Parameters.Add(vf_misc);
                command.Parameters.Add(Description);
                command.Parameters.Add(CustomerName);
                command.Parameters.Add(CustomerLocation);
                command.Parameters.Add(CustomerCompany);
                command.Parameters.Add(SyteLineRef);
                command.Parameters.Add(Problem);
                command.Parameters.Add(Benifit);
                command.Parameters.Add(Solution);
                command.Parameters.Add(vp_misc);
                command.Parameters.Add(Machine);
                tracking_id = Convert.ToInt32(command.ExecuteScalar());
                //End
                connection.Close();
            }
            return tracking_id;
        }


        /// <summary>
        /// get all the roles
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllRoles()
        {
            String Command = "SELECT ST_ROLES.ROLE_ID, ST_ROLES.NAME FROM ST_ROLES ORDER BY ROLE_ID";
            return this.ProcessCommand(Command);
        }

        /// <summary>
        /// Adds a teammate to a product's team
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="userID"></param>
        /// the user to add's ID
        /// <param name="roleID"></param>
        /// the role that user will take on the team

        public DataTable GetRates(int product)
        {
            ///<summary>
            /// Gets the rates for a product
            ///</summary>
            ///
            ///<param name="product">
            /// ID of the product we are viewing
            ///</param>
            string cmdText = @"SELECT 'Submitted', count(*) AS 'Count', (count(*) / 8.0) AS 'Per Day' FROM REA_TRACK
            INNER JOIN REA_PRODUCT ON REA_PRODUCT.PRODUCT_ID = REA_TRACK.PRODUCT
            WHERE 
            (SUBMITTED_ON BETWEEN GETDATE() - 8 AND GETDATE())
             AND 
            (REA_TRACK.PRODUCT IN ([CURRENT_PRODUCT]) )

            GROUP BY REA_PRODUCT.NAME

            union

            SELECT 'Resolved', count(*) AS 'Count', (count(*) / 8.0) AS 'Per Day' FROM REA_TRACK
            INNER JOIN REA_PRODUCT ON REA_PRODUCT.PRODUCT_ID = REA_TRACK.PRODUCT
            WHERE 
            (RESOLVED_ON BETWEEN GETDATE() - 8 AND GETDATE()) 
            AND 
            (REA_TRACK.PRODUCT IN ([CURRENT_PRODUCT]) )


            GROUP BY REA_PRODUCT.NAME

            union

            SELECT 'Closed', count(*) AS 'Count', (count(*) / 8.0) AS 'Per Day' FROM REA_TRACK
            INNER JOIN REA_PRODUCT ON REA_PRODUCT.PRODUCT_ID = REA_TRACK.PRODUCT
            WHERE 
            (CLOSED_ON BETWEEN GETDATE() - 8 AND GETDATE())
            AND
            (ASSIGNED_TO IS NULL) 
            AND 
            (REA_TRACK.PRODUCT IN ([CURRENT_PRODUCT]) )


            GROUP BY REA_PRODUCT.NAME";

            cmdText = cmdText.Replace("[CURRENT_PRODUCT]", product.ToString());

            System.Data.DataTable dt = ProcessCommand(cmdText);

            return dt;
        }

        public void InsertProductAcess(int ArgsUserID, int ArgsProductID)
        {
            ///<summary>
            /// Inserts a new Product Access
            ///</summary>
            ///<param name="ArgsUserID">
            /// The user ID from the st_users Table
            ///</param>
            ///<param name="ArgsProductID">
            /// The product ID 
            ///<param>
            String CommandString = "INSERT INTO REA_PRODUCT_ACCESS (UserID, ProductID)"
                + " VALUES (@ST_USER_ID, @PRODUCT_ID) ";
            SqlParameter UserID = new SqlParameter("ST_USER_ID", SqlDbType.Int);
            SqlParameter ProductID = new SqlParameter("PRODUCT_ID", SqlDbType.Int);
            UserID.Value = ArgsUserID;
            ProductID.Value = ArgsProductID;
            using (
                SqlConnection connection = new SqlConnection(_connectionString)
                )
            {
                connection.Open();
                SqlCommand command = new SqlCommand(CommandString, connection);
                command.Parameters.Add(UserID);
                command.Parameters.Add(ProductID);
                command.ExecuteNonQuery();
                connection.Dispose();
            }
        }


        /// <summary>
        /// Checks to see ifa  user has raw sql accesss
        /// </summary>
        /// <param name="USER_ID"></param>
        /// <returns></returns>
        public Boolean HasRawSQL(int USER_ID)
        {
            DataTable dt = this.ProcessCommand("SELECT ENableRawSQL FROM BTRACKER_NEW.dbo.ST_USERS WHERE USER_ID=" + Convert.ToString(USER_ID));
            return Convert.ToBoolean(dt.Rows[0][0] == DBNull.Value ? false : dt.Rows[0][0]);
        }
        /// <summary>
        /// Deletes the report form the table
        /// </summary>
        /// <param name="ReportID"></param>

        //=====================================DashBoard Reports=====================================
        public DataTable GetDashBoardReport(String buildId)
        {
            ///<summary>
            /// Gets the data to display
            ///</summary>
            ///<param name="buildId">
            /// The ID of the build
            ///</param>
            String Command = @"
SELECT
    REA_PRODUCT.NAME,
    REA_PRODUCT_RELEASE.RELEASE_COORDINATOR_ID,
    (ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME) AS RELEASE_COORDINATOR_NAME,
    (ISNULL(REA_PRODUCT_RELEASE.MAJOR, '?') + '.' + ISNULL(REA_PRODUCT_RELEASE.MINOR, '?') + '.' + ISNULL(REA_PRODUCT_RELEASE.BUILD, '?') + ' ' + ISNULL(REA_PRODUCT_RELEASE.MISC, '')) AS 'VERSION_BUILT',
    REA_PRODUCT_RELEASE.BUILT_ON,
    REA_PRODUCT_RELEASE.IS_CUSTOMER_RELEASE,
    REA_PRODUCT_RELEASE.NOTES,
    REA_PRODUCT_RELEASE.REA_LIST,
    (LEN(REA_PRODUCT_RELEASE.REA_LIST) - LEN(REPLACE(REA_PRODUCT_RELEASE.REA_LIST, ',', '')) + 1) AS 'REA_COUNT',
    DISPLAY_RELATED_REPORT,
    REA_PRODUCT_RELEASE.DB_VERSION
FROM REA_PRODUCT_RELEASE
    INNER JOIN REA_PRODUCT ON REA_PRODUCT_RELEASE.PRODUCT_ID = REA_PRODUCT.PRODUCT_ID
    INNER JOIN ST_USERS ON ST_USERS.USER_ID = REA_PRODUCT_RELEASE.RELEASE_COORDINATOR_ID
WHERE REA_PRODUCT_RELEASE.BUILD_ID = " + buildId;

            return this.ProcessCommand(Command);
        }

        public DataTable GetREAInfoForDashBoard(String REA)
        {
            ///<summary>
            ///     Gets addtioanl information for the dashboard
            ///</summary>
            ///<param name="REA">
            ///     List of REA ids comma delinated
            ///</param>
            String command = @"
SELECT
    REA_TRACK.PRIORITY AS PRIORITY_ID,
	REA_PRIORITY.NAME AS PRIORITY,
    REA_PRODUCT.NAME AS PRODUCT,
	REA_TRACK.TRACKING_ID AS TRACKING_ID,
    REA_TRACK.ISSUE_TYPE AS ISSUE_TYPE_ID,
	ST_ISSUE_TYPE.NAME AS ISSUE_TYPE,
	REA_STATUS.NAME AS STATUS,
	REA_TRACK.SUBMITTED_ON AS SUBMITTED_ON,
	REA_TRACK.SUBMITTED_BY AS SUBMITTED_BY,
    (ISNULL(REA_TRACK.VF_MAJOR, '?') + '.' + ISNULL(REA_TRACK.VF_MINOR, '?') + '.' + ISNULL(REA_TRACK.VF_BUILD, '?') + ' ' + ISNULL(REA_TRACK.VF_MISC, '')) AS 'VERSION_FOUND',
	REA_TRACK.ASSIGNED_TO AS ASSIGNED_TO,
	(user1.FIRST_NAME + ' ' + user1.LAST_NAME) AS ASSIGNED_TO_NAME,
	REA_TRACK.RESOLVED_BY AS RESOLVED_BY,
	REA_TRACK.RESOLVED_ON AS RESOLVED_ON,
	REA_TRACK.TITLE AS TITLE,
	REA_TRACK.CUSTOMER_NAME
FROM REA_TRACK
	INNER JOIN REA_PRODUCT 	ON REA_TRACK.PRODUCT = REA_PRODUCT.PRODUCT_ID
	INNER JOIN REA_PRIORITY 	ON REA_TRACK.PRIORITY = REA_PRIORITY.ID
	INNER JOIN ST_ISSUE_TYPE ON REA_TRACK.ISSUE_TYPE = ST_ISSUE_TYPE.ISSUE_ID
	INNER JOIN REA_STATUS 	ON REA_TRACK.STATUS = REA_STATUS.STATUS_ID
	LEFT OUTER JOIN ST_USERS user1 ON user1.USER_ID	= REA_TRACK.ASSIGNED_TO 
WHERE (REA_TRACK.TRACKING_ID IN ([REA_LIST]))
ORDER BY REA_TRACK.TRACKING_ID ASC;
";
            command = command.Replace("[REA_LIST]", REA);
            return this.ProcessCommand(command);
        }

        public DataTable GetRelatedREAs(int trackingID)
        {
            String command = $"SELECT * FROM ST_RELATED WHERE TRACKING_ID = {trackingID}";
            return this.ProcessCommand(command);
        }
        /// <summary>
        /// Deletes a metrics
        /// -Uses a SQL function
        /// </summary>
        /// <param name="ID"></param>
        public void DeleteMetrics(int ID)
        {
            String Command = "DELETE FROM REA_PRODUCT_METRICS WHERE ID=" + ID;
            this.ProcessCommand(Command);
        }

        //=====================================Admin Product Hierarchy===================================== 
        /// <summary>
        /// Gets all the product relationships
        /// </summary>
        /// <returns></returns>
        public DataTable GetProductRelationShip()
        {
            String Command = "SELECT REA_PRODUCT_RELATION.RELATION_ID,REA_PRODUCT_RELATION.PARENT_ID, REA_PRODUCT_RELATION.CHILD_ID " +
            "FROM REA_PRODUCT_RELATION inner join REA_PRODUCT on REA_PRODUCT_RELATION.PARENT_ID = REA_PRODUCT.PRODUCT_ID " +
            "ORDER BY REA_PRODUCT.NAME ASC";
            return this.ProcessCommand(Command);
        }

        /// <summary>
        /// Inserts the product relationship
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="ChildID"></param>
        public void InsertProductRelationShip(int ParentID, int ChildID)
        {
            String Command = "INSERT INTO REA_PRODUCT_RELATION(PARENT_ID, CHILD_ID) VALUES(" + Convert.ToString(ParentID) + "," + Convert.ToString(ChildID) + ")";
            this.ProcessCommand(Command);
        }

        public void RemoveRelation(int Relation_id)
        {
            String Command = "DELETE FROM REA_PRODUCT_RELATION WHERE RELATION_ID = " + Convert.ToString(Relation_id);
            this.ProcessCommand(Command);
        }
        /// <summary>
        /// Checks if a person has full access from the ASP table
        /// Made when thinking about moving the
        /// the ST_USERS table to ASP's log in system
        /// </summary>
        /// <param name="AspId"></param>
        /// <returns></returns>
        public bool hasFullAccess(string AspId)
        {
            string cmdTxt = $@"
            SELECT
	            ISNULL(
            (SELECT
	            CASE WHEN [FULL_ACCESS] > 0 THEN 1 ELSE 0 END 
            FROM 
	            ST_USERS 
            LEFT JOIN AspNetUsers ON AspNetUsers.ST_USER_ID = ST_USERS.USER_ID 
            WHERE AspNetUsers.Id = '{AspId}'), 0) AS [FULL_ACCESS]";

            bool hasAccess = ((int)ProcessScalarCommandSCR(cmdTxt) == 1);

            return hasAccess;
        }
        /// <summary>
        /// Checks if the user has full access or not
        /// from the ST_USER's ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool hasFullAccess(int userID)
        {
            string cmdTxt = $@"
            SELECT
	            ISNULL(
            (SELECT
	            CASE WHEN [FULL_ACCESS] > 0 THEN 1 ELSE 0 END 
            FROM 
	            ST_USERS 
            WHERE ST_USER_ID = {userID}), 0) AS [FULL_ACCESS]";

            bool hasAccess = ((int)ProcessScalarCommand(cmdTxt) == 1);
            /*
                        return hasAccess;
                        String cmdText =
                            "SELECT FULL_ACCESS FROM ST_USERS WHERE ST_USER_ID = " + Convert.ToString(st_user_id);
                        System.Data.DataTable dt;
                        using (SqlConnection connection = new SqlConnection(_connectionString))
                        {
                            connection.Open();

                            using (SqlCommand command = new SqlCommand())
                            {
                                command.Connection = connection;
                                command.CommandType = CommandType.Text;
                                command.CommandText = cmdText;
                                using (SqlDataAdapter reader = new SqlDataAdapter(command))
                                {
                                    dt = new DataTable();
                                    reader.Fill(dt);
                                }
                            }
                        }
                        bool hasAccess = false;
                        if (dt.Rows.Count > 0 && (bool)dt.Rows[0][0])
                        {
                            hasAccess = true;
                        }
            */
            return hasAccess;
        }

        //=====================================
        public DataTable GetRelatedBuilds(int buildID)
        {
            string sql = @"SELECT
                            REA_PRODUCT.NAME, 
                            (ISNULL(REA_PRODUCT_RELEASE.MAJOR, '?') + '.' + ISNULL(REA_PRODUCT_RELEASE.MINOR, '?') + '.' + ISNULL(REA_PRODUCT_RELEASE.BUILD, '?') + ' ' + ISNULL(REA_PRODUCT_RELEASE.MISC, '')) AS BUILD_VERSION,
                            REA_PRODUCT_RELEASE.BUILD_ID,
                            REA_PRODUCT.PRODUCT_ID, REA_PRODUCT_RELEASE_RELATION.RELEASE_RELATION_ID, REA_PRODUCT_RELEASE_RELATION.RELATED_BUILD_ID, 
                            (ISNULL(REA_PRODUCT_RELEASE.MAJOR, '?') + '.' + ISNULL(REA_PRODUCT_RELEASE.MINOR, '?') + '.' + ISNULL(REA_PRODUCT_RELEASE.BUILD, '?') + ' ' + ISNULL(REA_PRODUCT_RELEASE.MISC, '')) + ' ' + CONVERT(varchar, BUILT_ON, 101) AS BUILD_VERSION_WITH_DATE,
                          FROM
                            REA_PRODUCT_RELEASE_RELATION 

                          INNER JOIN REA_PRODUCT_RELEASE ON REA_PRODUCT_RELEASE.BUILD_ID = REA_PRODUCT_RELEASE_RELATION.RELATED_BUILD_ID
                          INNER JOIN REA_PRODUCT ON REA_PRODUCT.PRODUCT_ID = REA_PRODUCT_RELEASE.PRODUCT_ID

                          WHERE
                            REA_PRODUCT_RELEASE_RELATION.BUILD_ID = " + buildID.ToString() + @"
                          ORDER BY REA_PRODUCT.NAME";

            DataTable dt = ProcessCommand(sql);

            return dt;
        }

        public DataTable GetBuildsByProduct(int productID)
        {
            string sql = @"SELECT
                            REA_PRODUCT.NAME, REA_PRODUCT_RELEASE.BUILD_ID, 
                            (ISNULL(REA_PRODUCT_RELEASE.MAJOR, '?') + '.' + ISNULL(REA_PRODUCT_RELEASE.MINOR, '?') + '.' + ISNULL(REA_PRODUCT_RELEASE.BUILD, '?') + ' ' + ISNULL(REA_PRODUCT_RELEASE.MISC, '')) AS  BUILD_VERSION,
                            (ISNULL(REA_PRODUCT_RELEASE.MAJOR, '?') + '.' + ISNULL(REA_PRODUCT_RELEASE.MINOR, '?') + '.' + ISNULL(REA_PRODUCT_RELEASE.BUILD, '?') + ' ' + ISNULL(REA_PRODUCT_RELEASE.MISC, '')) + ' ' + CONVERT(varchar, BUILT_ON, 101) AS  BUILD_VERSION_WITH_DATE
                          FROM  REA_PRODUCT 

                          INNER JOIN REA_PRODUCT_RELEASE ON REA_PRODUCT_RELEASE.PRODUCT_ID = REA_PRODUCT.PRODUCT_ID

                          WHERE
                            REA_PRODUCT.PRODUCT_ID = " + productID.ToString() + @"
                          ORDER BY REA_PRODUCT_RELEASE.BUILD_ID DESC";

            DataTable dt = ProcessCommand(sql);

            return dt;
        }

        /// <summary>
        /// Gets the build REAs 
        /// </summary>
        /// <param name="rea_list"></param>
        /// <returns></returns>
        public DataTable GetBuildREAs(string rea_list)
        {
            //the problem with this is if an rea has multiple kick backs, the older reports will show the latest resolution, 
            //not the latest resolution at the time the build was created. should add a time component to this
            string sql = @"SELECT 
                            REA_TRACK.TRACKING_ID,
                            REA_TRACK.TITLE,
                            REA_PRIORITY.NAME AS 'PRIORITY',
                            ST_ISSUE_TYPE.NAME AS 'ISSUE_TYPE',
                            (ISNULL(REA_TRACK.VF_MAJOR, '?') + '.' + ISNULL(REA_TRACK.VF_MINOR, '?') + '.' + ISNULL(REA_TRACK.VF_BUILD, '?') + ' ' + ISNULL(REA_TRACK.VF_MISC, '')) AS 'VERSION_FOUND',
                            ISNULL(REA_TRACK.CUSTOMER_NAME, '') AS 'CUSTOMER_NAME',

(SELECT TOP 1 REA_HISTORY.DESCRIPTION FROM REA_HISTORY WHERE ((REA_HISTORY.TRACKING_ID = REA_TRACK.TRACKING_ID) AND (REA_HISTORY.TYPE=1)) ORDER BY REA_HISTORY.HISTORY_ID ASC) AS 'DESCRIPTION',
(SELECT TOP 1 REA_HISTORY.DESCRIPTION FROM REA_HISTORY WHERE ((REA_HISTORY.TRACKING_ID = REA_TRACK.TRACKING_ID) AND (REA_HISTORY.TYPE=3)) ORDER BY REA_HISTORY.HISTORY_ID DESC) AS 'RESOLUTION'

                          FROM 
                            REA_TRACK
                          INNER JOIN REA_PRIORITY ON REA_TRACK.PRIORITY = REA_PRIORITY.ID
                          INNER JOIN ST_ISSUE_TYPE ON REA_TRACK.ISSUE_TYPE = ST_ISSUE_TYPE.ISSUE_ID
                          WHERE 
                            REA_TRACK.TRACKING_ID IN (" + rea_list + ")";

            DataTable dt = ProcessCommand(sql);

            return dt;
        }

        /// <summary>
        /// gets a list of submitted REAs
        /// </summary>
        /// <param name="tracking_id"></param>
        /// <param name="oldproduct_id"></param>
        /// <returns></returns>
        public DataTable GetSubmittedREA(int tracking_id, int? oldproduct_id = null)
        {
            //the problem with this is if an rea has multiple kick backs, the older reports will show the latest resolution, 
            //not the latest resolution at the time the build was created. should add a time component to this
            string sql = @"SELECT
                            REA_PRODUCT.NAME AS 'PRODUCT', 
                            REA_TRACK.TRACKING_ID,
                            REA_TRACK.TITLE,
                            REA_PRIORITY.NAME AS 'PRIORITY',
                            REA_PRIORITY.ID AS 'PRIORITY_ID',
                            ST_ISSUE_TYPE.NAME AS 'ISSUE_TYPE',
                            (ISNULL(REA_TRACK.VF_MAJOR, '?') + '.' + ISNULL(REA_TRACK.VF_MINOR, '?') + '.' + ISNULL(REA_TRACK.VF_BUILD, '?') + ' ' + ISNULL(REA_TRACK.VF_MISC, '')) AS 'VERSION_FOUND',
                            ISNULL(REA_TRACK.CUSTOMER_NAME, '') AS 'CUSTOMER_NAME',
                            (ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME) AS 'SUBMITTED_BY',
                            (ISNULL(REA_TRACK.VR_MAJOR, '?') + '.' + ISNULL(REA_TRACK.VR_MINOR, '?') + '.' + ISNULL(REA_TRACK.VR_BUILD, '?') + ' ' + ISNULL(REA_TRACK.VR_MISC, '')) AS 'VERSION_RESOLVED',
(SELECT TOP 1 REA_HISTORY.DESCRIPTION FROM REA_HISTORY WHERE ((REA_HISTORY.TRACKING_ID = 
            REA_TRACK.TRACKING_ID) AND (REA_HISTORY.TYPE=1)) ORDER BY REA_HISTORY.HISTORY_ID ASC) AS 'DESCRIPTION',
(SELECT TOP 1 REA_HISTORY.DESCRIPTION FROM REA_HISTORY WHERE ((REA_HISTORY.TRACKING_ID = 
            REA_TRACK.TRACKING_ID) AND (REA_HISTORY.TYPE IN (2,3))) ORDER BY REA_HISTORY.HISTORY_ID DESC) AS 'LAST_NOTES',
ST_USERS1.EMAIL AS 'SUBMITTED_EMAIL',
ST_USERS2.EMAIL AS 'ASSIGNED_TO_EMAIL'" +

(oldproduct_id == null ? "" : ",REA_PRODUCT2.NAME AS 'OLD_PRODUCT_NAME'") +

                         @" FROM 
                            REA_TRACK

                          INNER JOIN REA_PRODUCT ON REA_TRACK.PRODUCT = REA_PRODUCT.PRODUCT_ID
                          INNER JOIN REA_PRIORITY ON REA_TRACK.priority = REA_PRIORITY.ID
                          INNER JOIN ST_ISSUE_TYPE ON REA_TRACK.ISSUE_TYPE = ST_ISSUE_TYPE.ISSUE_ID
                          INNER JOIN ST_USERS ON REA_TRACK.SUBMITTED_BY = ST_USERS.USER_ID
                          LEFT JOIN ST_USERS ST_USERS1 ON REA_PRODUCT.VALIDATOR =ST_USERS1.USER_ID
                          LEFT JOIN ST_USERS ST_USERS2 ON REA_TRACK.ASSIGNED_TO =ST_USERS2.USER_ID  "

                + (oldproduct_id == null ? "" : "LEFT JOIN REA_PRODUCT REA_PRODUCT2 ON REA_PRODUCT2.PRODUCT_ID = " + oldproduct_id)
+
@"
                          WHERE 
                            REA_TRACK.TRACKING_ID = " + tracking_id.ToString();

            DataTable dt = ProcessCommand(sql);

            return dt;
        }

        /// <summary>
        /// Checks if an REA exists
        /// </summary>
        /// <param name="tracking_id"></param>
        /// <returns></returns>
        public bool DoesREAExists(int tracking_id)
        {
            bool exists = false;

            string sql = "SELECT TRACKING_ID FROM REA_TRACK WHERE TRACKING_ID = " + tracking_id.ToString();

            DataTable dt = ProcessCommand(sql);
            if (dt.Rows.Count > 0)
            {
                exists = (int.Parse(dt.Rows[0][0].ToString()) == tracking_id);
            }
            return exists;
        }

        public bool DoesReportExists(int report_id)
        {
            bool exists = false;

            string sql = "SELECT REPORT_ID FROM REA_TABULAR_REPORT WHERE REPORT_ID = " + report_id.ToString();

            DataTable dt = ProcessCommand(sql);
            if (dt.Rows.Count > 0)
            {
                exists = (int.Parse(dt.Rows[0][0].ToString()) == report_id);
            }
            return exists;
        }

        /// <summary>
        /// Gets the list of users watching an REA
        /// </summary>
        /// <returns></returns>
        public DataTable GetUsersWatching()
        {
            string sql = @"SELECT DISTINCT
                                REA_WATCH_LIST.USER_ID,
                                ST_USERS.EMAIL
                            FROM 
                                REA_WATCH_LIST 
                            INNER JOIN ST_USERS ON ST_USERS.USER_ID = REA_WATCH_LIST.USER_ID 
                            WHERE 
                                (ST_USERS.ACCOUNT_TYPE<> -1)";

            DataTable dt = ProcessCommand(sql);

            return dt;
        }
        /// <summary>
        /// Inserts a priority to the table
        /// </summary>
        /// <param name="NameParam"></param>
        /// <param name="DescriptionParam"></param>
        /// <param name="CountParam"></param>
        /// the count of the new param
        /// <param name="WeightParam"></param>
        /// the weight of the new param
        public void InsertPriority(string NameParam, string DescriptionParam, int CountParam, int WeightParam)
        {
            SqlParameter Name = new SqlParameter();
            SqlParameter Description = new SqlParameter();
            //SqlParameter Count = new SqlParameter();
            SqlParameter Weight = new SqlParameter();

            Name.SqlDbType = SqlDbType.VarChar;
            Description.SqlDbType = SqlDbType.VarChar;
            //Count.SqlDbType = SqlDbType.Int;
            Weight.SqlDbType = SqlDbType.Int;

            Name.ParameterName = "NAME";
            Description.ParameterName = "DESCRIPTION";
            //Count.ParameterName = "COUNT";
            Weight.ParameterName = "WEIGHT";

            Name.Value = NameParam;
            Description.Value = DescriptionParam;
            //Count.Value = CountParam;
            Weight.Value = WeightParam;

            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            //SqlTransaction transaction = connection.BeginTransaction("insertPriority(" + HttpContext.Current.Session["st_userID"] + ") at " + DateTime.Now.ToShortTimeString());
            //Increment the Database

            //String CommandToIncrement = "admin_IncrementPriority @WEIGHT";
            //SqlCommand increment = new SqlCommand( CommandToIncrement, connection);
            //increment.Parameters.Add(Weight);


            command.Connection = connection;
            command.CommandText = "admin_InsertPriority @NAME, @DESCRIPTION, @WEIGHT";

            command.Parameters.Add(Name);
            command.Parameters.Add(Description);
            //command.Parameters.Add(Count);
            command.Parameters.Add(Weight);
            //command.Transaction = transaction;
            //try
            //{
            command.ExecuteScalar();
            //}
            //catch (Exception ex)
            //{
            //    ex.ToString();
            //    transaction.Rollback();
            //}
            connection.Close();
        }

        /// <summary>
        /// Gets the recent history of a REA
        /// </summary>
        /// <param name="tracking_id"></param>
        /// <returns></returns>
        /// 


        //not using anywhere
        public DataTable GetRecentHistory(int tracking_id)
        {

            string sql = @"
SELECT 
	REA_HISTORY.TYPE, 
	CASE REA_HISTORY.TYPE
		WHEN 2 THEN 'Notes were added: ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR(30))
		WHEN 4 THEN 'File was attached'
		WHEN 8 THEN 'REA was related'
		WHEN 10 THEN 'Planned Release Version changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		WHEN 11 THEN 'Actual Release Version changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		WHEN 12 THEN 'Planned Hours To Fix changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		WHEN 13 THEN 'Actual Hours To Fix changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		WHEN 14 THEN 'Planned Hours To Test changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		WHEN 15 THEN 'Actual Hours To Test changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		WHEN 20 THEN 'Planned Hours To Specify changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		WHEN 21 THEN 'Actual Hours To Specify changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		ELSE
			REA_HISTORY.DESCRIPTION
	END AS 'DESCRIPTION', 
	REA_HISTORY.MADE_ON, 
	DATEDIFF(DAY, MADE_ON, GETDATE())

FROM 
	REA_HISTORY 

WHERE 
	TRACKING_ID = " + tracking_id.ToString() + @" 
	AND DATEDIFF(DAY, MADE_ON, GETDATE()) <= 1
ORDER BY HISTORY_ID DESC";

            DataTable dt = ProcessCommand(sql);

            return dt;
        }

        /// <summary>
        /// gets the report information
        /// </summary>
        /// <param name="ReportID"></param>
        /// <returns></returns>
        public DataTable GetReport(int ReportID)
        {
            return this.ProcessCommand("SELECT * FROM REA_TABULAR_REPORT WHERE REPORT_ID=" + Convert.ToString(ReportID));
        }

        /// <summary>
        /// Gets the version of the report
        /// </summary>
        /// <param name="RepotID"></param>
        /// <returns></returns>
        public DataTable GetReportVersions(int RepotID)
        {
            return this.ProcessCommand("SELECT * FROM ST_VERSION_RANGE WHERE REPORT_ID = " + Convert.ToString(RepotID));
        }
        //=====================================Admin Issue Type Manager=====================================
        /// <summary>
        /// Updates a issue's name and dereaiption
        /// </summary>
        /// <param name="id"></param>
        /// the id of the issue
        /// <param name="name"></param>
        /// the new name of the issue
        /// <param name="dereaiption"></param>
        /// the new dereaiption
        public void UpdateIssueType(int id, string name, string dereaiption)
        {
            String InsertCommand = "UPDATE ST_ISSUE_TYPE SET NAME=@NAME, DESCRIPTION=@DESCRIPTION WHERE ISSUE_ID = @ISSUE_ID";
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(_connectionString);
            command.Connection = connection;
            connection.Open();
            command.CommandText = InsertCommand;

            SqlParameter NAME = new SqlParameter();
            SqlParameter DESCRIPTION = new SqlParameter();
            SqlParameter ISSUE_ID = new SqlParameter();

            NAME.ParameterName = "NAME";
            DESCRIPTION.ParameterName = "DESCRIPTION";
            ISSUE_ID.ParameterName = "ISSUE_ID";


            NAME.SqlDbType = SqlDbType.VarChar;
            DESCRIPTION.SqlDbType = SqlDbType.VarChar;
            ISSUE_ID.SqlDbType = SqlDbType.Int;

            NAME.Value = name;
            DESCRIPTION.Value = dereaiption;
            ISSUE_ID.Value = id;

            command.Parameters.Add(NAME);
            command.Parameters.Add(DESCRIPTION);
            command.Parameters.Add(ISSUE_ID);

            command.ExecuteNonQuery();
            connection.Close();
        }

        /// <summary>
        /// inserts a issue type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dereaiption"></param>
        /// the dereaiption
        public void InsertIssueType(string name, string dereaiption)
        {
            //get Issue Id set up
            String COMMAND = "SELECT MAX(ISSUE_ID) FROM ST_ISSUE_TYPE";
            SqlParameter ISSUE_ID = new SqlParameter();
            ISSUE_ID.SqlDbType = SqlDbType.Int;
            ISSUE_ID.ParameterName = "ISSUE_ID";
            ISSUE_ID.Value = Convert.ToInt32(this.ProcessScalarCommand(COMMAND));

            String InsertCommand = "INSERT INTO ST_ISSUE_TYPE(ISSUE_ID, NAME, DESCRIPTION, ROW_VER) VALUES(@ISSUE_ID, @NAME, @DESCRIPTION, 1)";
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(_connectionString);
            command.Connection = connection;
            connection.Open();
            command.CommandText = InsertCommand;
            SqlParameter NAME = new SqlParameter();
            SqlParameter DESCRIPTION = new SqlParameter();

            NAME.SqlDbType = SqlDbType.VarChar;
            DESCRIPTION.SqlDbType = SqlDbType.VarChar;

            NAME.Value = name;
            DESCRIPTION.Value = (String.IsNullOrEmpty(dereaiption) ? (object)DBNull.Value : dereaiption);

            NAME.ParameterName = "NAME";
            DESCRIPTION.ParameterName = "DESCRIPTION";

            command.Parameters.Add(NAME);
            command.Parameters.Add(DESCRIPTION);
            command.Parameters.Add(ISSUE_ID);

            command.ExecuteNonQuery();
            connection.Close();
        }
        /// <summary>
        /// Saves the profile photo
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <param name="userID"></param>
        public void saveProfilePhoto(byte[] uploadedFile, int userID)
        {
            SqlParameter fileParam = new SqlParameter();
            fileParam.SqlDbType = SqlDbType.Image;
            fileParam.ParameterName = "FILE";
            if (uploadedFile == null)
            {
                fileParam.Value = DBNull.Value;
            }
            else
            {
                fileParam.Value = uploadedFile;
            }
            SqlParameter userid = new SqlParameter();
            userid.SqlDbType = SqlDbType.Int;
            userid.ParameterName = "USER_ID";
            userid.Value = userID;
            //Save the photo and update the st_users table
            int count = Convert.ToInt32(this.ProcessScalarCommand(
                "SELECT COUNT(*) FROM ST_PHOTOS WHERE USER_ID = " + userID
                ));
            String sqlString;
            if (count == 0)
            {
                sqlString = "EXEC InsertPhoto @USER_ID, @FILE";
            }
            else
            {
                sqlString = "EXEC UpdatePhoto @USER_ID, @FILE";
            }

            //creating the sql command
            using (
               SqlConnection connection = new SqlConnection(_connectionString)
               )
            {
                connection.Open();
                SqlCommand Command = new SqlCommand(sqlString, connection);
                Command.Parameters.Add(fileParam);
                Command.Parameters.Add(userid);
                Command.ExecuteNonQuery();
                Command.Dispose();
                connection.Dispose();
            }

        }

        /// <summary>
        /// gets the stored photot from table
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public byte[] getPhoto(int userID)
        {
            byte[] temp = null;
            string command = "SELECT PHOTO FROM ST_PHOTOS WHERE ST_PHOTOS.USER_ID = " + userID;
            temp = (byte[])this.ProcessScalarCommand(command);
            return temp;
        }


        /// <summary>
        /// Gets the date time of the report
        /// </summary>
        /// <param name="RepotID"></param>
        /// <returns></returns>
        public DataTable GetReportDates(int RepotID)
        {
            return this.ProcessCommand("SELECT * FROM ST_DATE_RANGE WHERE REPORT_ID = " + Convert.ToString(RepotID));
        }



        public DataTable GetSettings(int userID)
        {
            string cmdTxt = @"SELECT 
                                ST_USERS.FIRST_NAME,
                                ST_USERS.LAST_NAME,
                                ST_USERS.LOGON,
                                ST_USERS.EMAIL,
                                ST_USERS.PASSWORD,
                                ST_USERS.ACCOUNT_TYPE,
                                ST_USERS.TITLE,
                                ST_USERS.DEPARTMENT_ID,
                                CASE WHEN COMPANY IS NULL THEN 0 ELSE COMPANY END AS COMPANY,
                                CASE WHEN OTHER_COMPANY IS NULL THEN '' ELSE OTHER_COMPANY END AS OTHER_COMPANY,
                                ST_USERS.IS_MANAGER,
                                ST_USERS.MANAGER_ID,
                                ST_USERS.DETAILS, 
                                ST_USERS.HOME_DISPLAY, 
                                ST_USERS.PRODUCT_DISPLAY, 
                                ST_USERS.SUBMITTED_DISPLAY, 
                                ST_USERS.TO_DO_DISPLAY, 
                                ST_USERS.WATCH_DISPLAY,
                                CASE WHEN DEFAULT_PRODUCT IS NULL THEN 32 ELSE DEFAULT_PRODUCT END AS DEFAULT_PRODUCT,
                                ST_USERS.DEFAULT_PAGE,
                                ST_USERS.FULL_ACCESS,
                                ST_USERS.BLOCK_WHERE,
                                ST_USERS.EnableRawSQL,
                                ST_USERS.DISPLAY_NAME_PREF,
                                ST_USERS.HOME_PAGE_LAYOUT,
                                CASE WHEN ST_USERS.EMPLOYEE_NUM IS NULL THEN '' ELSE ST_USERS.EMPLOYEE_NUM END AS EMPLOYEE_NUM,
                                CASE WHEN AspNetUsers.LanguageCode IS NULL THEN 'none' ELSE AspNetUsers.LanguageCode End AS LanguageCode
                                
                            FROM 
                                ST_USERS JOIN AspNetUsers on ST_USERS.USER_ID = AspNetUsers.ST_USER_ID WHERE USER_ID=" + userID.ToString();
            System.Data.DataTable dt = ProcessCommandSCR(cmdTxt);
            return dt;
        }

        public bool IsExEmployee(int UserID)
        {
            string cmdTxt = @"SELECT 
                CASE WHEN COUNT(1) > 0 THEN 0 ELSE 1 END
            FROM
                ST_USERS
            WHERE
                ST_USERS.USER_ID = " + UserID.ToString() + " AND ST_USERS.ACCOUNT_TYPE = -1";

            bool isEx = (int.Parse(ProcessScalarCommandSCR(cmdTxt).ToString()) == 0);
            return isEx;
        }

        public bool IsActiveUser(int UserID)
        {
            string cmdTxt = $@"
            SELECT 
                CASE WHEN COUNT(1) > 0 THEN 1 ELSE 0 END
            FROM
                ST_USERS
            WHERE
                ST_USERS.USER_ID = {UserID} AND ST_USERS.ACCOUNT_TYPE <> -1
            ";
            bool isActive = ((int)ProcessScalarCommandSCR(cmdTxt) == 1);
            return isActive;
        }

        public bool IsValidREA(int trackingID)
        {
            //this query always returns 0 or 1, even if the REA is not found.
            //0 (false) = the REA has been found
            //1 (true) = the REA has NOT been found
            //false is the default answer.
            string cmdTxt = @"SELECT CASE WHEN COUNT(1) > 0 THEN 1 ELSE 0 END FROM REA_TRACK WHERE REA_TRACK.TRACKING_ID = " + trackingID.ToString();
            return ((int)ProcessScalarCommand(cmdTxt) == 1);
        }


        public DataTable GetUsers(bool showExEmployees = false)
        {
            ///<summary>
            /// Gets the list of users from the datatable
            ///</summary>
            ///
            ///<param name="showExEmployees">
            /// boolean variable to display the ex employees
            ///</param>

            String cmdText = "SELECT BTRACKER_NEW.dbo.ST_USERS.USER_ID AS USER_ID, BTRACKER_NEW.dbo.ST_USERS.LOGON AS LOGON, BTRACKER_NEW.dbo.ST_USERS.FIRST_NAME AS FIRST_NAME," +
                      "BTRACKER_NEW.dbo.ST_USERS.LAST_NAME AS LAST_NAME, BTRACKER_NEW.dbo.ST_USERS.EMAIL AS EMAIL, BTRACKER_NEW.dbo.ST_USERS.ACCOUNT_TYPE AS ACCOUNT_TYPE, ST_ACCOUNT_TYPE.NAME AS ACCOUNT_TYPE_NAME," +
                      "ST_COMPANY_LIST.NAME as COMPANY_NAME, ST_COMPANY_LIST.COMPANY_ID as COMPANY_ID, ST_DEPARTMENT.NAME as DEPARTMENT_NAME, " +
                      "(USER1.FIRST_NAME +' '+ USER1.LAST_NAME) AS 'BOSS'," +
                      "ST_USERS.OTHER_COMPANY AS OTHER_COMPANY " +
                      "FROM BTRACKER_NEW.dbo.ST_USERS INNER JOIN BTRACKER_NEW.dbo.ST_ACCOUNT_TYPE ON BTRACKER_NEW.dbo.ST_USERS.ACCOUNT_TYPE = BTRACKER_NEW.dbo.ST_ACCOUNT_TYPE.ACCOUNT_ID " +
                      "LEFT JOIN BTRACKER_NEW.dbo.ST_COMPANY_LIST ON ST_USERS.COMPANY = ST_COMPANY_LIST.COMPANY_ID " +
                      "LEFT JOIN BTRACKER_NEW.dbo.ST_DEPARTMENT ON ST_USERS.DEPARTMENT_ID = ST_DEPARTMENT.DEPARTMENT_ID " +
                      "LEFT JOIN BTRACKER_NEW.dbo.ST_USERS USER1 ON USER1.USER_ID = BTRACKER_NEW.dbo.ST_USERS.MANAGER_ID ";

            if (!showExEmployees)
            {
                cmdText += " WHERE BTRACKER_NEW.dbo.ST_USERS.ACCOUNT_TYPE != -1 ";
            }
            cmdText += "ORDER BY ST_USERS.FIRST_NAME, ST_USERS.LAST_NAME; ";
            System.Data.DataTable dt;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = cmdText;
                    using (SqlDataAdapter reader = new SqlDataAdapter(command))
                    {
                        dt = new DataTable();
                        reader.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public DataTable GetUsersByProductID(int ProductID)
        {
            String command =
                "SELECT ST_USERS.FIRST_NAME, ST_USERS.LAST_NAME, REA_PRODUCT_TEAM.USER_ID, REA_PRODUCT_TEAM.ROLE, ST_ROLES.NAME, REA_PRODUCT_TEAM.TEAM_ID " +
                "FROM REA_PRODUCT_TEAM " +
                "INNER JOIN ST_USERS ON ST_USERS.USER_ID = REA_PRODUCT_TEAM.USER_ID " +
                "INNER JOIN ST_ROLES ON ST_ROLES.ROLE_ID = REA_PRODUCT_TEAM.ROLE " +
                "WHERE REA_PRODUCT_TEAM.PRODUCT = " + ProductID + " AND ST_USERS.ACCOUNT_TYPE != -1 " +
                "ORDER BY ST_USERS.FIRST_NAME ASC, ST_USERS.LAST_NAME ASC";
            DataTable dt = this.ProcessCommand(command);
            return dt;
        }

        public bool HasFullProductAccess(int userID)
        {
            //if the account is active it returns the proper value
            //if the account is not active or invalid userID, then 0 is returned
            string cmdTxt = @"SELECT  
    ISNULL(SUM(CAST(HAS_FULL_ACCESS AS INT)), 0)
FROM
    ST_USERS
WHERE
    ST_USERS.USER_ID = " + userID.ToString() + " AND ST_USERS.ACCOUNT_TYPE <> -1";
            return (bool)ProcessScalarCommand(cmdTxt);
        }


        public DataTable GetAllCompanies()
        {
            String cmdText =
                "SELECT ST_COMPANY_LIST.NAME, ST_COMPANY_LIST.COMPANY_ID, ST_COMPANY_LIST.DESCRIPTION FROM ST_COMPANY_LIST";
            System.Data.DataTable dt = ProcessCommand(cmdText);
            return dt;
        }

        public DataTable GetAllDepartments()
        {
            String cmdText =
                "SELECT ST_DEPARTMENT.DEPARTMENT_ID, ST_DEPARTMENT.NAME, ST_DEPARTMENT.CODE FROM ST_DEPARTMENT";
            System.Data.DataTable dt = ProcessCommand(cmdText);
            return dt;
        }

        public DataTable GetAllManagers()
        {
            String cmdText =
                "SELECT ST_USERS.USER_ID, ST_USERS.FIRST_NAME, ST_USERS.LAST_NAME FROM ST_USERS WHERE IS_MANAGER=1 ORDER BY ST_USERS.LAST_NAME ASC";
            System.Data.DataTable dt = ProcessCommand(cmdText);
            return dt;
        }

        //not used anywhere in the code
        public DataTable GetUserAndAccess(int userID)
        {
            ///<summary>
            /// gets the list of users and thier product access
            ///</summary>
            ///
            ///<param name="userID">
            /// the ID of the user we are currently viewing
            ///</param>
            String cmdText =
                "SELECT ST_USERs.FIRST_NAME, ST_USERs.LAST_NAME, ST_USERs.EMAIL, ST_USERs.DEFAULT_PRODUCT, ST_USERs.LOGON," +
                "ST_USERs.COMPANY, ST_USERs.OTHER_COMPANY, ST_USERs.TITLE, ST_USERs.DEPARTMENT_ID, ST_USERs.IS_MANAGER, ST_USERs.MANAGER_ID," +
                "REA_PRODUCT_ACCESS.ProductID, ST_USERS.PASSWORD, ST_USERS.BLOCK_WHERE, ST_USERS.FULL_ACCESS, ST_USERS.DEFAULT_PAGE " +
                "FROM ST_USERS " +
                "INNER JOIN REA_PRODUCT_ACCESS ON REA_PRODUCT_ACCESS.UserID = ST_USERs.USER_ID " +
                "WHERE ST_USERs.USER_ID = " + Convert.ToString(userID);
            System.Data.DataTable dt;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = cmdText;
                    using (SqlDataAdapter reader = new SqlDataAdapter(command))
                    {
                        dt = new DataTable();
                        reader.Fill(dt);
                    }
                }
            }
            //if st_access is empty
            if (dt.Rows.Count == 0)
            {
                cmdText =
                "SELECT ST_USERs.FIRST_NAME, ST_USERs.LAST_NAME, ST_USERs.EMAIL, ST_USERs.DEFAULT_PRODUCT, ST_USERs.LOGON, " +
                "ST_USERs.COMPANY, ST_USERs.OTHER_COMPANY, ST_USERs.TITLE, ST_USERs.DEPARTMENT_ID, ST_USERs.IS_MANAGER, ST_USERs.MANAGER_ID, " +
                "ST_USERs.DEFAULT_PRODUCT, ST_USERS.PASSWORD, ST_USERS.BLOCK_WHERE, ST_USERS.FULL_ACCESS, ST_USERS.DEFAULT_PAGE  " +
                "FROM ST_USERS " +
                "WHERE ST_USERs.USER_ID = " + Convert.ToString(userID);
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = cmdText;
                        using (SqlDataAdapter reader = new SqlDataAdapter(command))
                        {
                            dt = new DataTable();
                            reader.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }

        public bool AddProductAccessForUser(int userID, int productID)
        {
            String CommandString = "INSERT INTO REA_PRODUCT_ACCESS (UserID, ProductID) VALUES (@ST_USER_ID, @PRODUCT_ID) ";
            SqlParameter UserID = new SqlParameter("ST_USER_ID", SqlDbType.Int);
            SqlParameter ProductID = new SqlParameter("PRODUCT_ID", SqlDbType.Int);
            UserID.Value = userID;
            ProductID.Value = productID;
            bool success = false;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(CommandString, connection);
                command.Parameters.Add(UserID);
                command.Parameters.Add(ProductID);
                try
                {
                    if (command.ExecuteNonQuery() == 1)
                    {
                        success = true;
                    }
                }
                catch
                {
                    //if failed, most likely because of the constraint
                    success = false;
                }
                connection.Dispose();
            }
            return success;
        }
        //=====================================Search REA Save/Load==================================
        public int CreateNewTabularReport(
            //Returns the ID of the new instance of New tabular report
            int ArgsOwner, int ArgsPublicReport, string ArgsName,
            int ArgsREACompare, int? ArgsREANum1, int? ArgsREANum2,
            string ArgsStatus, string ArgsProduct,
            int? ArgsDocChange, int? ArgsDbChange, int? ArgsReqDoc,
            String ArgsWhereClause, int? ArgsMaxRows, string ArgsIssueType,
            int? ArgsCustomerBug, int? ArgsCustomerLogic, string ArgsCustomerName,

            int? ArgsTitleLogic, string ArgsSearchTitle, String ArgsPriorities,
            //Dates
            int? ArgsSubBy,
            int? ArgsSubMonth, int? ArgsSubDay, int? ArgsSubYear, string ArgsSubOperator1, string ArgsSubField1,
            int? ArgsSubMonth2, int? ArgsSubDay2, int? ArgsSubYear2, string ArgsSubOperator2, string ArgsSubField2,
            int? ArgsAssignedTo,
            int? ArgsAsnOnMonth, int? ArgsAsnOnDay, int? ArgsAsnOnYear, string ArgsAsnOnOperator1, string ArgsAsnOnField1,
            int? ArgsAsnOnMonth2, int? ArgsAsnOnDay2, int? ArgsAsnOnYear2, string ArgsAsnOnOperator2, string ArgsAsnOnField2,
            int? ArgsResolvedBy,
            int? ArgsResOnMonth, int? ArgsResOnDay, int? ArgsResOnYear, string ArgsResOperator1, string ArgsResOnField1,
            int? ArgsResOnMonth2, int? ArgsResOnDay2, int? ArgsResOnYear2, string ArgsResOperator2, string ArgsResOnField2,
            int? ArgsClosedBy,
            int? ArgsCloOnMonth, int? ArgsCloOnDay, int? ArgsCloOnYear, string ArgsCloOnOperator1, string ArgsCloOnField1,
            int? ArgsCloOnMonth2, int? ArgsCloOnDay2, int? ArgsCloOnYear2, string ArgsCloOnOperator2, string ArgsCloOnField2,
            int? ArgsModStartMonth, int? ArgsModStartDay, int? ArgsModStartYear, string ArgsModStartOperator, string ArgsModStartField,
            int? ArgsModEndMonth, int? ArgsModEndDay, int? ArgsModEndYear, string ArgsModEndOperator, string ArgsModEndField,
            //Versions
            int? ArgsVF_MAJOR, int? ArgsVF_MINOR, int? ArgsVF_BUILD, String ArgsVF_MISC, String VF_MAJOR_OPERATOR, String VF_MINOR_OPERATOR, String VF_BUILD_OPERATOR, String VF_FIELD,
            int? ArgsVR_MAJOR, int? ArgsVR_MINOR, int? ArgsVR_BUILD, String ArgsVR_MISC, String VR_MAJOR_OPERATOR, String VR_MINOR_OPERATOR, String VR_BUILD_OPERATOR, String VR_FIELD,
            int? ArgsVC_MAJOR, int? ArgsVC_MINOR, int? ArgsVC_BUILD, String ArgsVC_MISC, String VC_MAJOR_OPERATOR, String VC_MINOR_OPERATOR, String VC_BUILD_OPERATOR, String VC_FIELD,
            int? ArgsVP_MAJOR, int? ArgsVP_MINOR, int? ArgsVP_BUILD, String ArgsVP_MISC, String VP_MAJOR_OPERATOR, String VP_MINOR_OPERATOR, String VP_BUILD_OPERATOR, String VP_FIELD,
            //sql Statements
            string ArgsSelectStatement,
            string ArgsSortByStatement
            , string ArgsMachineSN,
            string ArgsSyteLineRef,
            string ArgsCustomerCompany,
            string ArgsCustomerLocation,
            string ArgsHistorySearch,
            string ArgsHistorySearchLogic,
            int? ArgsRank, int ArgsRankComparator,
            int? ArgsSize, int ArgsSizeComparator,
            string ArgsKeywords
            )
        {
            ///<summary>
            ///inserts a new Tabular Report
            ///</summary>
            String Command =
                "EXEC Report_InsertReport @OWNER, @PUBLIC_REPORT, @NAME," +
                " @REA_COMPARE, @REA_NUM1, @REA_NUM2," +
                " @STATUS, @PRODUCT, " +
                //@MODULE_ID,"+
                " @DOC_CHANGE, @DB_CHANGE, @REQ_DOC," +
                " @WHERE_CLAUSE, @MAX_ROWS, @ISSUE_TYPE," +
                " @CUSTOMER_BUG, @CUSTOMER_LOGIC, @CUSTOMER_NAME," +
                " @TITLE_LOGIC, @SEARCH_TITLE, @priorities," +
                //Version
                " @VF_MAJOR, @VF_MINOR, @VF_BUILD, @VF_MISC, @VF_MAJOR_OPERATOR, @VF_MINOR_OPERATOR, @VF_BUILD_OPERATOR, @VF_FIELD," +
                " @VR_MAJOR, @VR_MINOR, @VR_BUILD, @VR_MISC, @VR_MAJOR_OPERATOR, @VR_MINOR_OPERATOR, @VR_BUILD_OPERATOR, @VR_FIELD," +
                " @VC_MAJOR, @VC_MINOR, @VC_BUILD, @VC_MISC, @VC_MAJOR_OPERATOR, @VC_MINOR_OPERATOR, @VC_BUILD_OPERATOR, @VC_FIELD," +
                " @PLANNED_MAJOR_VER, @PLANNED_MINOR_VER, @PLANNED_BUILD_NUM, @PLANNED_MISC_VER_INFO, @VP_MAJOR_OPERATOR,@VP_MINOR_OPERATOR,@VP_BUILD_OPERATOR, @VP_FIELD," +
                //Dates
                " @SUB_BY, " +
                " @SUB_MONTH1, @SUB_DAY1, @SUB_YEAR1, @SUB_OPERATOR1, @SUB_FIELD1, " +
                " @SUB_MONTH2, @SUB_DAY2, @SUB_YEAR2, @SUB_OPERATOR2, @SUB_FIELD2, " +
                " @ASSIGN_TO, " +
                " @ASN_ON_MONTH1, @ASN_ON_DAY1, @ASN_ON_YEAR1, @ASN_OPERATOR1, @ASN_FIELD1," +
                " @ASN_ON_MONTH2, @ASN_ON_DAY2, @ASN_ON_YEAR2, @ASN_OPERATOR2, @ASN_FIELD2," +
                " @RESOLVED_BY," +
                " @RES_ON_MONTH1, @RES_ON_DAY1, @RES_ON_YEAR1, @RES_OPERATOR1, @RES_FIELD1," +
                " @RES_ON_MONTH2, @RES_ON_DAY2, @RES_ON_YEAR2, @RES_OPERATOR2, @RES_FIELD2," +
                " @CLOSED_BY, " +
                " @CLO_ON_MONTH1, @CLO_ON_DAY1, @CLO_ON_YEAR1, @CLO_OPERATOR1, @CLO_FIELD1," +
                " @CLO_ON_MONTH2, @CLO_ON_DAY2, @CLO_ON_YEAR2, @CLO_OPERATOR2, @CLO_FIELD2, " +
                " @MODIFIED_START_MONTH, @MODIFIED_START_DAY, @MODIFIED_START_YEAR, @MODIFIED_START_OPERATOR, @MODIFIED_START_FIELD," +
                " @MODIFIED_END_MONTH, @MODIFIED_END_DAY, @MODIFIED_END_YEAR, @MODIFIED_END_OPERATOR, @MODIFIED_END_FIELD, " +
                " @SELECT_STATEMENT, " +
                " @SORT_BY_STATEMENT,  @MACHINE_SN,@SYTELINE, @CUSTOMER_LOCATION, @CUSTOMER_COMPANY,@HISTORY_SEARCH, @HISTORY_SEARCH_LOGIC, " +
                " @RANK_ORDER, @RANK_COMPARE, @SIZE, @SIZE_COMPARE, @KEYWORDS"
                ;

            SqlParameter Owner = new SqlParameter(); //The only Required Field
            SqlParameter PublicReport = new SqlParameter();
            SqlParameter Name = new SqlParameter();
            SqlParameter REACompare = new SqlParameter();
            SqlParameter REANum1 = new SqlParameter();
            SqlParameter REANum2 = new SqlParameter();
            SqlParameter Status = new SqlParameter();
            SqlParameter Product = new SqlParameter();
            SqlParameter DocChange = new SqlParameter();
            SqlParameter DbChange = new SqlParameter();
            SqlParameter ReqDoc = new SqlParameter();
            SqlParameter WhereClause = new SqlParameter();
            SqlParameter MaxRows = new SqlParameter();
            SqlParameter IssueType = new SqlParameter();
            SqlParameter CustomerBug = new SqlParameter();
            SqlParameter CustomerLogic = new SqlParameter();
            SqlParameter CustomerName = new SqlParameter();
            SqlParameter TitleLogic = new SqlParameter();
            SqlParameter SearchTitle = new SqlParameter();
            SqlParameter Priorities = new SqlParameter();
            SqlParameter SubBy = new SqlParameter();
            SqlParameter SubMonth = new SqlParameter();
            SqlParameter SubDay = new SqlParameter();
            SqlParameter Sub_Year = new SqlParameter();
            SqlParameter SubMonth2 = new SqlParameter();
            SqlParameter SubDay2 = new SqlParameter();
            SqlParameter SubYear2 = new SqlParameter();
            SqlParameter AssignedTo = new SqlParameter();
            SqlParameter AsnOnMonth = new SqlParameter();
            SqlParameter AsnOnDay = new SqlParameter();
            SqlParameter AsnOnYear = new SqlParameter();
            SqlParameter AsnOnMonth2 = new SqlParameter();
            SqlParameter AsnOnDay2 = new SqlParameter();
            SqlParameter AsnOnYear2 = new SqlParameter();
            SqlParameter ResolvedBy = new SqlParameter();
            SqlParameter ResOnMonth = new SqlParameter();
            SqlParameter ResOnYear = new SqlParameter();
            SqlParameter ResOnDay = new SqlParameter();
            SqlParameter ResOnMonth2 = new SqlParameter();
            SqlParameter ResOnDay2 = new SqlParameter();
            SqlParameter ResOnYear2 = new SqlParameter();
            SqlParameter ClosedBy = new SqlParameter();
            SqlParameter CloOnMonth = new SqlParameter();
            SqlParameter CloOnDay = new SqlParameter();
            SqlParameter CloOnYear = new SqlParameter();
            SqlParameter CloOnMonth2 = new SqlParameter();
            SqlParameter CloOnDay2 = new SqlParameter();
            SqlParameter CloOnYear2 = new SqlParameter();
            SqlParameter CloOnOperator1 = new SqlParameter();
            SqlParameter ResOnOperator1 = new SqlParameter();
            SqlParameter AsnOnOperator1 = new SqlParameter();
            SqlParameter SubOperator1 = new SqlParameter();
            SqlParameter CloOnOperator2 = new SqlParameter();
            SqlParameter ResOnOperator2 = new SqlParameter();
            SqlParameter AsnOnOperator2 = new SqlParameter();
            SqlParameter SubOperator2 = new SqlParameter();
            SqlParameter CloOnField1 = new SqlParameter();
            SqlParameter ResOnField1 = new SqlParameter();
            SqlParameter AsnOnField1 = new SqlParameter();
            SqlParameter SubField1 = new SqlParameter();
            SqlParameter CloOnField2 = new SqlParameter();
            SqlParameter ResOnField2 = new SqlParameter();
            SqlParameter AsnOnField2 = new SqlParameter();
            SqlParameter SubField2 = new SqlParameter();
            SqlParameter ModStartMonth = new SqlParameter();
            SqlParameter ModStartDay = new SqlParameter();
            SqlParameter ModStartYear = new SqlParameter();
            SqlParameter ModEndMonth = new SqlParameter();
            SqlParameter ModEndYear = new SqlParameter();
            SqlParameter ModEndDay = new SqlParameter();
            SqlParameter ModStartOperator = new SqlParameter();
            SqlParameter ModEndOperator = new SqlParameter();
            SqlParameter ModStartField = new SqlParameter();
            SqlParameter ModEndField = new SqlParameter();

            SqlParameter VFMajor = new SqlParameter();
            SqlParameter VFMinor = new SqlParameter();
            SqlParameter VFBuild = new SqlParameter();
            SqlParameter VFMisc = new SqlParameter();
            SqlParameter VRMajor = new SqlParameter();
            SqlParameter VRMinor = new SqlParameter();
            SqlParameter VRBuild = new SqlParameter();
            SqlParameter VRMisc = new SqlParameter();
            SqlParameter VCMajor = new SqlParameter();
            SqlParameter VCMinor = new SqlParameter();
            SqlParameter VCBuild = new SqlParameter();
            SqlParameter VCMisc = new SqlParameter();
            SqlParameter VPMajor = new SqlParameter();
            SqlParameter VPMinor = new SqlParameter();
            SqlParameter VPBuild = new SqlParameter();
            SqlParameter VPMisc = new SqlParameter();
            SqlParameter VRMajorOperator = new SqlParameter();
            SqlParameter VRMinorOperator = new SqlParameter();
            SqlParameter VRBuildOperator = new SqlParameter();
            SqlParameter VRField = new SqlParameter();
            SqlParameter VCMajorOperator = new SqlParameter();
            SqlParameter VCMinorOperator = new SqlParameter();
            SqlParameter VCBuildOperator = new SqlParameter();
            SqlParameter VCField = new SqlParameter();
            SqlParameter VFMajorOperator = new SqlParameter();
            SqlParameter VFMinorOperator = new SqlParameter();
            SqlParameter VFBuildOperator = new SqlParameter();
            SqlParameter VFField = new SqlParameter();
            SqlParameter VPMajorOperator = new SqlParameter();
            SqlParameter VPMinorOperator = new SqlParameter();
            SqlParameter VPBuildOperator = new SqlParameter();
            SqlParameter VPField = new SqlParameter();

            SqlParameter SelectStatement = new SqlParameter();
            SqlParameter SortByStatement = new SqlParameter();
            //Customer info
            SqlParameter MachineSN = new SqlParameter();
            SqlParameter CustomerCompany = new SqlParameter();
            SqlParameter CustomerLocation = new SqlParameter();
            //history
            SqlParameter SyteLine = new SqlParameter();
            SqlParameter HistorySearch = new SqlParameter();
            SqlParameter HistorySearchLogic = new SqlParameter();

            SqlParameter Rank = new SqlParameter();
            SqlParameter RankCompare = new SqlParameter();
            SqlParameter Size = new SqlParameter();
            SqlParameter SizeCompare = new SqlParameter();
            SqlParameter Keywords = new SqlParameter();
            //Display

            //Add in the other fields, Don't forget to add them into the Sql Server too.
            //General
            Owner.SqlDbType = SqlDbType.Int;
            PublicReport.SqlDbType = SqlDbType.SmallInt;
            Name.SqlDbType = SqlDbType.VarChar;
            REACompare.SqlDbType = SqlDbType.Int;
            REANum1.SqlDbType = SqlDbType.Int;
            REANum2.SqlDbType = SqlDbType.Int;
            Status.SqlDbType = SqlDbType.VarChar;
            Product.SqlDbType = SqlDbType.VarChar;
            DocChange.SqlDbType = SqlDbType.Int;
            DbChange.SqlDbType = SqlDbType.Int;
            ReqDoc.SqlDbType = SqlDbType.Int;
            WhereClause.SqlDbType = SqlDbType.Text;
            MaxRows.SqlDbType = SqlDbType.Int;
            IssueType.SqlDbType = SqlDbType.VarChar;
            CustomerBug.SqlDbType = SqlDbType.Int;
            CustomerLogic.SqlDbType = SqlDbType.Int;
            CustomerName.SqlDbType = SqlDbType.VarChar;
            TitleLogic.SqlDbType = SqlDbType.Int;
            SearchTitle.SqlDbType = SqlDbType.VarChar;
            Priorities.SqlDbType = SqlDbType.VarChar;
            //Version
            //Major
            VFMajor.SqlDbType = SqlDbType.Int;
            VFMajorOperator.SqlDbType = SqlDbType.VarChar;
            VRMajor.SqlDbType = SqlDbType.Int;
            VRMajorOperator.SqlDbType = SqlDbType.VarChar;
            VCMajor.SqlDbType = SqlDbType.Int;
            VCMajorOperator.SqlDbType = SqlDbType.VarChar;
            VPMajor.SqlDbType = SqlDbType.Int;
            VPMajorOperator.SqlDbType = SqlDbType.VarChar;
            //Minor
            VFMinor.SqlDbType = SqlDbType.Int;
            VFMinorOperator.SqlDbType = SqlDbType.VarChar;
            VRMinor.SqlDbType = SqlDbType.Int;
            VRMinorOperator.SqlDbType = SqlDbType.VarChar;
            VCMinor.SqlDbType = SqlDbType.Int;
            VCMinorOperator.SqlDbType = SqlDbType.VarChar;
            VPMinor.SqlDbType = SqlDbType.Int;
            VPMinorOperator.SqlDbType = SqlDbType.VarChar;
            //Build
            VFBuild.SqlDbType = SqlDbType.Int;
            VFBuildOperator.SqlDbType = SqlDbType.VarChar;
            VRBuild.SqlDbType = SqlDbType.Int;
            VRBuildOperator.SqlDbType = SqlDbType.VarChar;
            VCBuild.SqlDbType = SqlDbType.Int;
            VCBuildOperator.SqlDbType = SqlDbType.VarChar;
            VPBuild.SqlDbType = SqlDbType.Int;
            VPBuildOperator.SqlDbType = SqlDbType.VarChar;
            //Misc
            VFMisc.SqlDbType = SqlDbType.VarChar;
            VRMisc.SqlDbType = SqlDbType.VarChar;
            VCMisc.SqlDbType = SqlDbType.VarChar;
            VPMisc.SqlDbType = SqlDbType.VarChar;
            //Field
            VFField.SqlDbType = SqlDbType.VarChar;
            VRField.SqlDbType = SqlDbType.VarChar;
            VCField.SqlDbType = SqlDbType.VarChar;
            VPField.SqlDbType = SqlDbType.VarChar;
            //Date
            //Submitted On
            SubBy.SqlDbType = SqlDbType.Int;
            SubMonth.SqlDbType = SqlDbType.Int;
            SubDay.SqlDbType = SqlDbType.Int;
            Sub_Year.SqlDbType = SqlDbType.Int;

            SubMonth2.SqlDbType = SqlDbType.Int;
            SubDay2.SqlDbType = SqlDbType.Int;
            SubYear2.SqlDbType = SqlDbType.Int;
            SubOperator1.SqlDbType = SqlDbType.VarChar;
            SubOperator2.SqlDbType = SqlDbType.VarChar;
            SubField1.SqlDbType = SqlDbType.VarChar;
            SubField2.SqlDbType = SqlDbType.VarChar;
            //Assigned On
            AssignedTo.SqlDbType = SqlDbType.Int;
            AsnOnMonth.SqlDbType = SqlDbType.Int;
            AsnOnDay.SqlDbType = SqlDbType.Int;
            AsnOnYear.SqlDbType = SqlDbType.Int;
            AsnOnMonth2.SqlDbType = SqlDbType.Int;
            AsnOnDay2.SqlDbType = SqlDbType.Int;
            AsnOnYear2.SqlDbType = SqlDbType.Int;
            AsnOnOperator1.SqlDbType = SqlDbType.VarChar;
            AsnOnOperator2.SqlDbType = SqlDbType.VarChar;
            AsnOnField1.SqlDbType = SqlDbType.VarChar;
            AsnOnField2.SqlDbType = SqlDbType.VarChar;
            //Resolved On
            ResolvedBy.SqlDbType = SqlDbType.Int;
            ResOnMonth.SqlDbType = SqlDbType.Int;
            ResOnYear.SqlDbType = SqlDbType.Int;
            ResOnDay.SqlDbType = SqlDbType.Int;
            ResOnMonth2.SqlDbType = SqlDbType.Int;
            ResOnDay2.SqlDbType = SqlDbType.Int;
            ResOnYear2.SqlDbType = SqlDbType.Int;
            ResOnOperator1.SqlDbType = SqlDbType.VarChar;
            ResOnOperator2.SqlDbType = SqlDbType.VarChar;
            ResOnField1.SqlDbType = SqlDbType.VarChar;
            ResOnField2.SqlDbType = SqlDbType.VarChar;
            //Closed On
            ClosedBy.SqlDbType = SqlDbType.Int;
            CloOnMonth.SqlDbType = SqlDbType.Int;
            CloOnDay.SqlDbType = SqlDbType.Int;
            CloOnYear.SqlDbType = SqlDbType.Int;
            CloOnMonth2.SqlDbType = SqlDbType.Int;
            CloOnDay2.SqlDbType = SqlDbType.Int;
            CloOnYear2.SqlDbType = SqlDbType.Int;
            CloOnOperator1.SqlDbType = SqlDbType.VarChar;
            CloOnOperator2.SqlDbType = SqlDbType.VarChar;
            CloOnField1.SqlDbType = SqlDbType.VarChar;
            CloOnField2.SqlDbType = SqlDbType.VarChar;
            //Modified On
            ModStartMonth.SqlDbType = SqlDbType.Int;
            ModStartDay.SqlDbType = SqlDbType.Int;
            ModStartYear.SqlDbType = SqlDbType.Int;
            ModEndMonth.SqlDbType = SqlDbType.Int;
            ModEndDay.SqlDbType = SqlDbType.Int;
            ModEndYear.SqlDbType = SqlDbType.Int;
            ModStartOperator.SqlDbType = SqlDbType.VarChar;
            ModEndOperator.SqlDbType = SqlDbType.VarChar;
            ModStartField.SqlDbType = SqlDbType.VarChar;
            ModEndField.SqlDbType = SqlDbType.VarChar;
            //SQl
            SelectStatement.SqlDbType = SqlDbType.VarChar;
            SortByStatement.SqlDbType = SqlDbType.VarChar;
            //Customer info
            MachineSN.SqlDbType = SqlDbType.VarChar;
            CustomerCompany.SqlDbType = SqlDbType.VarChar;
            CustomerLocation.SqlDbType = SqlDbType.VarChar;

            SyteLine.SqlDbType = SqlDbType.VarChar;
            HistorySearch.SqlDbType = SqlDbType.VarChar;
            HistorySearchLogic.SqlDbType = SqlDbType.Int;

            Rank.SqlDbType = SqlDbType.Int;
            RankCompare.SqlDbType = SqlDbType.Int;
            Size.SqlDbType = SqlDbType.Int;
            SizeCompare.SqlDbType = SqlDbType.Int;

            Keywords.SqlDbType = SqlDbType.VarChar;

            Owner.ParameterName = "OWNER";
            PublicReport.ParameterName = "PUBLIC_REPORT";
            Name.ParameterName = "NAME";
            REACompare.ParameterName = "REA_COMPARE";
            REANum1.ParameterName = "REA_NUM1";
            REANum2.ParameterName = "REA_NUM2";
            Status.ParameterName = "STATUS";
            Product.ParameterName = "PRODUCT";

            DocChange.ParameterName = "DOC_CHANGE";
            DbChange.ParameterName = "DB_CHANGE";
            ReqDoc.ParameterName = "REQ_DOC";
            WhereClause.ParameterName = "WHERE_CLAUSE";
            MaxRows.ParameterName = "MAX_ROWS";
            IssueType.ParameterName = "ISSUE_TYPE";
            CustomerBug.ParameterName = "CUSTOMER_BUG";
            CustomerLogic.ParameterName = "CUSTOMER_LOGIC";
            CustomerName.ParameterName = "CUSTOMER_NAME";
            TitleLogic.ParameterName = "TITLE_LOGIC";
            SearchTitle.ParameterName = "SEARCH_TITLE";
            Priorities.ParameterName = "priorities";
            //Dates
            SubBy.ParameterName = "SUB_BY";
            SubMonth.ParameterName = "SUB_MONTH1";
            SubDay.ParameterName = "SUB_DAY1";
            Sub_Year.ParameterName = "SUB_YEAR1";
            SubMonth2.ParameterName = "SUB_MONTH2";
            SubDay2.ParameterName = "SUB_DAY2";
            SubYear2.ParameterName = "SUB_YEAR2";
            AssignedTo.ParameterName = "ASSIGN_TO";
            AsnOnMonth.ParameterName = "ASN_ON_MONTH1";
            AsnOnDay.ParameterName = "ASN_ON_DAY1";
            AsnOnYear.ParameterName = "ASN_ON_YEAR1";
            AsnOnMonth2.ParameterName = "ASN_ON_MONTH2";
            AsnOnDay2.ParameterName = "ASN_ON_DAY2";
            AsnOnYear2.ParameterName = "ASN_ON_YEAR2";
            ResolvedBy.ParameterName = "RESOLVED_BY";
            ResOnMonth.ParameterName = "RES_ON_MONTH1";
            ResOnDay.ParameterName = "RES_ON_DAY1";
            ResOnYear.ParameterName = "RES_ON_YEAR1";
            ResOnMonth2.ParameterName = "RES_ON_MONTH2";
            ResOnDay2.ParameterName = "RES_ON_DAY2";
            ResOnYear2.ParameterName = "RES_ON_YEAR2";
            ClosedBy.ParameterName = "CLOSED_BY";
            CloOnMonth.ParameterName = "CLO_ON_MONTH1";
            CloOnDay.ParameterName = "CLO_ON_DAY1";
            CloOnYear.ParameterName = "CLO_ON_YEAR1";
            CloOnMonth2.ParameterName = "CLO_ON_MONTH2";
            CloOnDay2.ParameterName = "CLO_ON_DAY2";
            CloOnYear2.ParameterName = "CLO_ON_YEAR2";

            ModStartMonth.ParameterName = "MODIFIED_START_MONTH";
            ModStartDay.ParameterName = "MODIFIED_START_DAY";
            ModStartYear.ParameterName = "MODIFIED_START_YEAR";
            ModEndMonth.ParameterName = "MODIFIED_END_MONTH";
            ModEndYear.ParameterName = "MODIFIED_END_DAY";
            ModEndDay.ParameterName = "MODIFIED_END_YEAR";
            ModStartOperator.ParameterName = "MODIFIED_START_OPERATOR";
            ModEndOperator.ParameterName = "MODIFIED_END_OPERATOR";
            ModStartField.ParameterName = "MODIFIED_START_FIELD";
            ModEndField.ParameterName = "MODIFIED_END_FIELD";

            CloOnOperator1.ParameterName = "CLO_OPERATOR1";
            ResOnOperator1.ParameterName = "RES_OPERATOR1";
            AsnOnOperator1.ParameterName = "ASN_OPERATOR1";
            SubOperator1.ParameterName = "SUB_OPERATOR1";
            CloOnOperator2.ParameterName = "CLO_OPERATOR2";
            ResOnOperator2.ParameterName = "RES_OPERATOR2";
            AsnOnOperator2.ParameterName = "ASN_OPERATOR2";
            SubOperator2.ParameterName = "SUB_OPERATOR2";
            CloOnField1.ParameterName = "CLO_FIELD1";
            ResOnField1.ParameterName = "RES_FIELD1";
            AsnOnField1.ParameterName = "ASN_FIELD1";
            SubField1.ParameterName = "SUB_FIELD1";
            CloOnField2.ParameterName = "CLO_FIELD2";
            ResOnField2.ParameterName = "RES_FIELD2";
            AsnOnField2.ParameterName = "ASN_FIELD2";
            SubField2.ParameterName = "SUB_FIELD2";
            //Versions
            VFMajor.ParameterName = "VF_MAJOR";
            VFMinor.ParameterName = "VF_MINOR";
            VFBuild.ParameterName = "VF_BUILD";
            VFMisc.ParameterName = "VF_MISC";

            VRMajor.ParameterName = "VR_MAJOR";
            VRMinor.ParameterName = "VR_MINOR";
            VRBuild.ParameterName = "VR_BUILD";
            VRMisc.ParameterName = "VR_MISC";

            VCMajor.ParameterName = "VC_MAJOR";
            VCMinor.ParameterName = "VC_MINOR";
            VCBuild.ParameterName = "VC_BUILD";
            VCMisc.ParameterName = "VC_MISC";

            VPMajor.ParameterName = "PLANNED_MAJOR_VER";
            VPMinor.ParameterName = "PLANNED_MINOR_VER";
            VPBuild.ParameterName = "PLANNED_BUILD_NUM";
            VPMisc.ParameterName = "PLANNED_MISC_VER_INFO";

            VRMajorOperator.ParameterName = "VR_MAJOR_OPERATOR";
            VRMinorOperator.ParameterName = "VR_MINOR_OPERATOR";
            VRBuildOperator.ParameterName = "VR_BUILD_OPERATOR";
            VRField.ParameterName = "VR_FIELD";
            VCMajorOperator.ParameterName = "VC_MAJOR_OPERATOR";
            VCMinorOperator.ParameterName = "VC_MINOR_OPERATOR";
            VCBuildOperator.ParameterName = "VC_BUILD_OPERATOR";
            VCField.ParameterName = "VC_FIELD";
            VFMajorOperator.ParameterName = "VF_MAJOR_OPERATOR";
            VFMinorOperator.ParameterName = "VF_MINOR_OPERATOR";
            VFBuildOperator.ParameterName = "VF_BUILD_OPERATOR";
            VFField.ParameterName = "VF_FIELD";
            VPMajorOperator.ParameterName = "VP_MAJOR_OPERATOR";
            VPMinorOperator.ParameterName = "VP_MINOR_OPERATOR";
            VPBuildOperator.ParameterName = "VP_BUILD_OPERATOR";
            VPField.ParameterName = "VP_FIELD";
            SelectStatement.ParameterName = "SELECT_STATEMENT";
            SortByStatement.ParameterName = "SORT_BY_STATEMENT";
            //Customer info
            MachineSN.ParameterName = "MACHINE_SN";
            CustomerCompany.ParameterName = "CUSTOMER_COMPANY";
            CustomerLocation.ParameterName = "CUSTOMER_LOCATION";

            SyteLine.ParameterName = "SYTELINE";
            HistorySearch.ParameterName = "HISTORY_SEARCH";
            HistorySearchLogic.ParameterName = "HISTORY_SEARCH_LOGIC";

            Rank.ParameterName = "RANK_ORDER";
            RankCompare.ParameterName = "RANK_COMPARE";
            Size.ParameterName = "SIZE";
            SizeCompare.ParameterName = "SIZE_COMPARE";

            Keywords.ParameterName = "KEYWORDS";

            Owner.Value = (ArgsOwner == 0 ? (object)DBNull.Value : ArgsOwner);
            PublicReport.Value = ((ArgsPublicReport != 1 && ArgsPublicReport != 0) ? (object)DBNull.Value : ArgsPublicReport);
            Name.Value = (ArgsName == null ? (object)DBNull.Value : ArgsName);
            REACompare.Value = (ArgsREACompare == 0 ? (object)DBNull.Value : ArgsREACompare);
            REANum1.Value = (ArgsREANum1 == null || ArgsREANum1 == 0 ? (object)DBNull.Value : ArgsREANum1);
            REANum2.Value = (ArgsREANum2 == null || ArgsREANum2 == 0 ? (object)DBNull.Value : ArgsREANum2);
            Status.Value = (String.IsNullOrEmpty(ArgsStatus) ? (object)DBNull.Value : ArgsStatus);
            Product.Value = (String.IsNullOrEmpty(ArgsProduct) ? (object)DBNull.Value : ArgsProduct);

            DocChange.Value = (ArgsDocChange == null ? -1 : ArgsDocChange);
            DbChange.Value = (ArgsDbChange == null ? -1 : ArgsDbChange);
            MaxRows.Value = (ArgsMaxRows == null ? (object)DBNull.Value : ArgsMaxRows);
            ReqDoc.Value = (ArgsReqDoc == null ? -1 : ArgsReqDoc);
            WhereClause.Value = (String.IsNullOrEmpty(ArgsWhereClause) || String.IsNullOrWhiteSpace(ArgsWhereClause) ? (object)DBNull.Value : ArgsWhereClause);
            IssueType.Value = (String.IsNullOrEmpty(ArgsIssueType) ? (object)DBNull.Value : ArgsIssueType);
            CustomerBug.Value = (ArgsCustomerBug == null ? (object)DBNull.Value : ArgsCustomerBug);
            CustomerLogic.Value = ((ArgsCustomerLogic == 0 || String.IsNullOrEmpty(ArgsCustomerName)) ? (object)DBNull.Value : ArgsCustomerLogic);
            CustomerName.Value = (ArgsCustomerName == null ? (object)DBNull.Value : ArgsCustomerName);
            TitleLogic.Value = ((ArgsTitleLogic == 0 || String.IsNullOrEmpty(ArgsSearchTitle)) ? (object)DBNull.Value : ArgsTitleLogic);
            SearchTitle.Value = (ArgsSearchTitle == null ? (object)DBNull.Value : ArgsSearchTitle);
            Priorities.Value = (String.IsNullOrEmpty(ArgsPriorities) ? (object)DBNull.Value : ArgsPriorities);
            //Dates
            SubBy.Value = (ArgsSubBy == null ? (object)DBNull.Value : ArgsSubBy);
            SubMonth.Value = (ArgsSubMonth == null ? (object)DBNull.Value : ArgsSubMonth);
            SubDay.Value = (ArgsSubDay == null ? (object)DBNull.Value : ArgsSubDay);
            Sub_Year.Value = (ArgsSubYear == null ? (object)DBNull.Value : ArgsSubYear);
            SubMonth2.Value = (ArgsSubMonth2 == null ? (object)DBNull.Value : ArgsSubMonth2);
            SubDay2.Value = (ArgsSubDay2 == null ? (object)DBNull.Value : ArgsSubDay2);
            SubYear2.Value = (ArgsSubYear2 == null ? (object)DBNull.Value : ArgsSubYear2);
            AssignedTo.Value = (ArgsAssignedTo == null ? (object)DBNull.Value : ArgsAssignedTo);
            AsnOnMonth.Value = (ArgsAsnOnMonth == null ? (object)DBNull.Value : ArgsAsnOnMonth);
            AsnOnDay.Value = (ArgsAsnOnDay == null ? (object)DBNull.Value : ArgsAsnOnDay);
            AsnOnYear.Value = (ArgsAsnOnYear == null ? (object)DBNull.Value : ArgsAsnOnYear);
            AsnOnMonth2.Value = (ArgsAsnOnMonth2 == null ? (object)DBNull.Value : ArgsAsnOnMonth2);
            AsnOnDay2.Value = (ArgsAsnOnDay2 == null ? (object)DBNull.Value : ArgsAsnOnDay2);
            AsnOnYear2.Value = (ArgsAsnOnYear2 == null ? (object)DBNull.Value : ArgsAsnOnYear2);
            ResolvedBy.Value = (ArgsResolvedBy == null ? (object)DBNull.Value : ArgsResolvedBy);
            ResOnMonth.Value = (ArgsResOnMonth == null ? (object)DBNull.Value : ArgsResOnMonth);
            ResOnDay.Value = (ArgsResOnDay == null ? (object)DBNull.Value : ArgsResOnDay);
            ResOnYear.Value = (ArgsResOnYear == null ? (object)DBNull.Value : ArgsResOnYear);
            ResOnMonth2.Value = (ArgsResOnMonth2 == null ? (object)DBNull.Value : ArgsResOnMonth2);
            ResOnDay2.Value = (ArgsResOnDay2 == null ? (object)DBNull.Value : ArgsResOnDay2);
            ResOnYear2.Value = (ArgsResOnYear2 == null ? (object)DBNull.Value : ArgsResOnYear2);
            ClosedBy.Value = (ArgsClosedBy == null ? (object)DBNull.Value : ArgsClosedBy);
            CloOnMonth.Value = (ArgsCloOnMonth == null ? (object)DBNull.Value : ArgsCloOnMonth);
            CloOnDay.Value = (ArgsCloOnDay == null ? (object)DBNull.Value : ArgsCloOnDay);
            CloOnYear.Value = (ArgsCloOnYear == null ? (object)DBNull.Value : ArgsCloOnYear);
            CloOnMonth2.Value = (ArgsCloOnMonth2 == null ? (object)DBNull.Value : ArgsCloOnMonth2);
            CloOnDay2.Value = (ArgsCloOnDay2 == null ? (object)DBNull.Value : ArgsCloOnDay2);
            CloOnYear2.Value = (ArgsCloOnYear2 == null ? (object)DBNull.Value : ArgsCloOnYear2);
            ModStartMonth.Value = (ArgsModStartMonth == null ? (object)DBNull.Value : ArgsModStartMonth);
            ModStartDay.Value = (ArgsModStartDay == null ? (object)DBNull.Value : ArgsModStartDay);
            ModStartYear.Value = (ArgsModStartYear == null ? (object)DBNull.Value : ArgsModStartYear);
            ModEndMonth.Value = (ArgsModEndMonth == null ? (object)DBNull.Value : ArgsModEndMonth);
            ModEndYear.Value = (ArgsModEndDay == null ? (object)DBNull.Value : ArgsModEndDay);
            ModEndDay.Value = (ArgsModEndYear == null ? (object)DBNull.Value : ArgsModEndYear);
            ModStartOperator.Value = (String.IsNullOrEmpty(ArgsModStartOperator) ? (object)DBNull.Value : ArgsModStartOperator);
            ModEndOperator.Value = (String.IsNullOrEmpty(ArgsModEndOperator) ? (object)DBNull.Value : ArgsModEndOperator);
            ModStartField.Value = (String.IsNullOrEmpty(ArgsModStartField) ? (object)DBNull.Value : ArgsModStartField);
            ModEndField.Value = (String.IsNullOrEmpty(ArgsModEndField) ? (object)DBNull.Value : ArgsModEndField);

            CloOnOperator1.Value = (String.IsNullOrEmpty(ArgsCloOnOperator1) ? (object)DBNull.Value : ArgsCloOnOperator1);
            ResOnOperator1.Value = (String.IsNullOrEmpty(ArgsResOperator1) ? (object)DBNull.Value : ArgsResOperator1);
            AsnOnOperator1.Value = (String.IsNullOrEmpty(ArgsAsnOnOperator1) ? (object)DBNull.Value : ArgsAsnOnOperator1);
            SubOperator1.Value = (String.IsNullOrEmpty(ArgsSubOperator1) ? (object)DBNull.Value : ArgsSubOperator1);
            CloOnOperator2.Value = (String.IsNullOrEmpty(ArgsCloOnOperator2) ? (object)DBNull.Value : ArgsCloOnOperator2);
            ResOnOperator2.Value = (String.IsNullOrEmpty(ArgsResOperator2) ? (object)DBNull.Value : ArgsResOperator2);
            AsnOnOperator2.Value = (String.IsNullOrEmpty(ArgsAsnOnOperator2) ? (object)DBNull.Value : ArgsAsnOnOperator2);
            SubOperator2.Value = (String.IsNullOrEmpty(ArgsSubOperator2) ? (object)DBNull.Value : ArgsSubOperator2);
            CloOnField1.Value = (String.IsNullOrEmpty(ArgsCloOnField1) ? (object)DBNull.Value : ArgsCloOnField1);
            ResOnField1.Value = (String.IsNullOrEmpty(ArgsResOnField1) ? (object)DBNull.Value : ArgsResOnField1);
            AsnOnField1.Value = (String.IsNullOrEmpty(ArgsAsnOnField1) ? (object)DBNull.Value : ArgsAsnOnField1);
            SubField1.Value = (String.IsNullOrEmpty(ArgsSubField1) ? (object)DBNull.Value : ArgsSubField1);
            CloOnField2.Value = (String.IsNullOrEmpty(ArgsCloOnField2) ? (object)DBNull.Value : ArgsCloOnField2);
            ResOnField2.Value = (String.IsNullOrEmpty(ArgsResOnField2) ? (object)DBNull.Value : ArgsResOnField2);
            AsnOnField2.Value = (String.IsNullOrEmpty(ArgsAsnOnField2) ? (object)DBNull.Value : ArgsAsnOnField2);
            SubField2.Value = (String.IsNullOrEmpty(ArgsSubField2) ? (object)DBNull.Value : ArgsSubField2);

            //Versions
            VFMajor.Value = (ArgsVF_MAJOR == null ? (object)DBNull.Value : ArgsVF_MAJOR);
            VFMinor.Value = (ArgsVF_MINOR == null ? (object)DBNull.Value : ArgsVF_MINOR);
            VFBuild.Value = (ArgsVF_BUILD == null ? (object)DBNull.Value : ArgsVF_BUILD);
            VFMisc.Value = (String.IsNullOrEmpty(ArgsVF_MISC) ? (object)DBNull.Value : ArgsVF_MISC);

            VRMajor.Value = (ArgsVR_MAJOR == null ? (object)DBNull.Value : ArgsVR_MAJOR);
            VRMinor.Value = (ArgsVR_MINOR == null ? (object)DBNull.Value : ArgsVR_MINOR);
            VRBuild.Value = (ArgsVR_BUILD == null ? (object)DBNull.Value : ArgsVR_BUILD);
            VRMisc.Value = (String.IsNullOrEmpty(ArgsVR_MISC) ? (object)DBNull.Value : ArgsVR_MISC);

            VCMajor.Value = (ArgsVC_MAJOR == null ? (object)DBNull.Value : ArgsVC_MAJOR);
            VCMinor.Value = (ArgsVC_MINOR == null ? (object)DBNull.Value : ArgsVC_MINOR);
            VCBuild.Value = (ArgsVC_BUILD == null ? (object)DBNull.Value : ArgsVC_BUILD);
            VCMisc.Value = (String.IsNullOrEmpty(ArgsVC_MISC) ? (object)DBNull.Value : ArgsVC_MISC);

            VPMajor.Value = (ArgsVP_MAJOR == null ? (object)DBNull.Value : ArgsVP_MAJOR);
            VPMinor.Value = (ArgsVP_MINOR == null ? (object)DBNull.Value : ArgsVP_MINOR);
            VPBuild.Value = (ArgsVP_BUILD == null ? (object)DBNull.Value : ArgsVP_BUILD);
            VPMisc.Value = (String.IsNullOrEmpty(ArgsVP_MISC) ? (object)DBNull.Value : ArgsVP_MISC);

            VRMajorOperator.Value = (String.IsNullOrEmpty(VR_MAJOR_OPERATOR) ? (object)DBNull.Value : VR_MAJOR_OPERATOR);
            VRMinorOperator.Value = (String.IsNullOrEmpty(VR_MINOR_OPERATOR) ? (object)DBNull.Value : VR_MINOR_OPERATOR);
            VRBuildOperator.Value = (String.IsNullOrEmpty(VR_BUILD_OPERATOR) ? (object)DBNull.Value : VR_BUILD_OPERATOR);
            VRField.Value = (String.IsNullOrEmpty(VR_FIELD) ? (object)DBNull.Value : VR_FIELD);
            VCMajorOperator.Value = (String.IsNullOrEmpty(VC_MAJOR_OPERATOR) ? (object)DBNull.Value : VC_MAJOR_OPERATOR);
            VCMinorOperator.Value = (String.IsNullOrEmpty(VC_MINOR_OPERATOR) ? (object)DBNull.Value : VC_MINOR_OPERATOR);
            VCBuildOperator.Value = (String.IsNullOrEmpty(VC_BUILD_OPERATOR) ? (object)DBNull.Value : VC_BUILD_OPERATOR);
            VCField.Value = (String.IsNullOrEmpty(VC_FIELD) ? (object)DBNull.Value : VC_FIELD);
            VFMajorOperator.Value = (String.IsNullOrEmpty(VF_MAJOR_OPERATOR) ? (object)DBNull.Value : VF_MAJOR_OPERATOR);
            VFMinorOperator.Value = (String.IsNullOrEmpty(VF_MINOR_OPERATOR) ? (object)DBNull.Value : VF_MINOR_OPERATOR);
            VFBuildOperator.Value = (String.IsNullOrEmpty(VF_BUILD_OPERATOR) ? (object)DBNull.Value : VF_BUILD_OPERATOR);
            VFField.Value = (String.IsNullOrEmpty(VF_FIELD) ? (object)DBNull.Value : VF_FIELD);
            VPMajorOperator.Value = (String.IsNullOrEmpty(VP_MAJOR_OPERATOR) ? (object)DBNull.Value : VP_MAJOR_OPERATOR);
            VPMinorOperator.Value = (String.IsNullOrEmpty(VP_MINOR_OPERATOR) ? (object)DBNull.Value : VP_MINOR_OPERATOR);
            VPBuildOperator.Value = (String.IsNullOrEmpty(VP_BUILD_OPERATOR) ? (object)DBNull.Value : VP_BUILD_OPERATOR);
            VPField.Value = (String.IsNullOrEmpty(VP_FIELD) ? (object)DBNull.Value : VP_FIELD);
            //SQL
            SelectStatement.Value = (String.IsNullOrEmpty(ArgsSelectStatement) ? (object)DBNull.Value : ArgsSelectStatement);
            SortByStatement.Value = (String.IsNullOrEmpty(ArgsSortByStatement) ? (object)DBNull.Value : ArgsSortByStatement);
            //Customer Info
            MachineSN.Value = (String.IsNullOrEmpty(ArgsMachineSN) ? (object)DBNull.Value : ArgsMachineSN);
            CustomerCompany.Value = (String.IsNullOrEmpty(ArgsCustomerCompany) ? (object)DBNull.Value : ArgsCustomerCompany);
            CustomerLocation.Value = (String.IsNullOrEmpty(ArgsCustomerLocation) ? (object)DBNull.Value : ArgsCustomerLocation);

            SyteLine.Value = (String.IsNullOrEmpty(ArgsSyteLineRef) ? (object)DBNull.Value : ArgsSyteLineRef);
            HistorySearch.Value = (String.IsNullOrEmpty(ArgsHistorySearch) ? (object)DBNull.Value : ArgsHistorySearch);
            HistorySearchLogic.Value = (String.IsNullOrEmpty(ArgsHistorySearchLogic) ? (object)DBNull.Value : ArgsHistorySearchLogic);

            Rank.Value = ((ArgsRank == null) ? (object)DBNull.Value : ArgsRank);
            RankCompare.Value = ArgsRankComparator;
            Size.Value = ((ArgsSize == null) ? (object)DBNull.Value : ArgsSize);
            SizeCompare.Value = ArgsSizeComparator;

            Keywords.Value = (String.IsNullOrEmpty(ArgsKeywords) ? (object)DBNull.Value : ArgsKeywords);

            int value = -1;
            using (
                SqlConnection connection = new SqlConnection(_connectionString)
                )
            {
                connection.Open();
                SqlCommand command = new SqlCommand(Command, connection);
                command.Parameters.Add(Owner);
                command.Parameters.Add(PublicReport);
                command.Parameters.Add(Name);
                command.Parameters.Add(REACompare);
                command.Parameters.Add(REANum1);
                command.Parameters.Add(REANum2);
                command.Parameters.Add(Status);
                command.Parameters.Add(Product);
                command.Parameters.Add(DocChange);
                command.Parameters.Add(DbChange);
                command.Parameters.Add(ReqDoc);
                command.Parameters.Add(WhereClause);
                command.Parameters.Add(MaxRows);
                command.Parameters.Add(IssueType);
                command.Parameters.Add(CustomerBug);
                command.Parameters.Add(CustomerLogic);
                command.Parameters.Add(CustomerName);
                command.Parameters.Add(TitleLogic);
                command.Parameters.Add(SearchTitle);
                command.Parameters.Add(Priorities);

                command.Parameters.Add(SubBy);
                command.Parameters.Add(SubMonth);
                command.Parameters.Add(SubDay);
                command.Parameters.Add(Sub_Year);
                command.Parameters.Add(SubMonth2);
                command.Parameters.Add(SubDay2);
                command.Parameters.Add(SubYear2);
                //Date
                command.Parameters.Add(AssignedTo);
                command.Parameters.Add(AsnOnMonth);
                command.Parameters.Add(AsnOnDay);
                command.Parameters.Add(AsnOnYear);
                command.Parameters.Add(AsnOnMonth2);
                command.Parameters.Add(AsnOnDay2);
                command.Parameters.Add(AsnOnYear2);

                command.Parameters.Add(ResolvedBy);
                command.Parameters.Add(ResOnMonth);
                command.Parameters.Add(ResOnYear);
                command.Parameters.Add(ResOnDay);
                command.Parameters.Add(ResOnMonth2);
                command.Parameters.Add(ResOnDay2);
                command.Parameters.Add(ResOnYear2);

                command.Parameters.Add(ClosedBy);
                command.Parameters.Add(CloOnMonth);
                command.Parameters.Add(CloOnDay);
                command.Parameters.Add(CloOnYear);
                command.Parameters.Add(CloOnMonth2);
                command.Parameters.Add(CloOnDay2);
                command.Parameters.Add(CloOnYear2);

                command.Parameters.Add(CloOnOperator1);
                command.Parameters.Add(ResOnOperator1);
                command.Parameters.Add(AsnOnOperator1);
                command.Parameters.Add(SubOperator1);

                command.Parameters.Add(CloOnOperator2);
                command.Parameters.Add(ResOnOperator2);
                command.Parameters.Add(AsnOnOperator2);
                command.Parameters.Add(SubOperator2);

                command.Parameters.Add(CloOnField1);
                command.Parameters.Add(ResOnField1);
                command.Parameters.Add(AsnOnField1);
                command.Parameters.Add(SubField1);

                command.Parameters.Add(CloOnField2);
                command.Parameters.Add(ResOnField2);
                command.Parameters.Add(AsnOnField2);
                command.Parameters.Add(SubField2);

                command.Parameters.Add(ModStartMonth);
                command.Parameters.Add(ModStartDay);
                command.Parameters.Add(ModStartYear);
                command.Parameters.Add(ModEndMonth);
                command.Parameters.Add(ModEndYear);
                command.Parameters.Add(ModEndDay);
                command.Parameters.Add(ModStartOperator);
                command.Parameters.Add(ModEndOperator);
                command.Parameters.Add(ModEndField);
                command.Parameters.Add(ModStartField);

                //Version             
                command.Parameters.Add(VFMajor);
                command.Parameters.Add(VFMinor);
                command.Parameters.Add(VFBuild);
                command.Parameters.Add(VFMisc);

                command.Parameters.Add(VRMajor);
                command.Parameters.Add(VRMinor);
                command.Parameters.Add(VRBuild);
                command.Parameters.Add(VRMisc);

                command.Parameters.Add(VCMajor);
                command.Parameters.Add(VCMinor);
                command.Parameters.Add(VCBuild);
                command.Parameters.Add(VCMisc);

                command.Parameters.Add(VPMajor);
                command.Parameters.Add(VPMinor);
                command.Parameters.Add(VPBuild);
                command.Parameters.Add(VPMisc);

                command.Parameters.Add(VRMajorOperator);
                command.Parameters.Add(VRMinorOperator);
                command.Parameters.Add(VRBuildOperator);
                command.Parameters.Add(VRField);

                command.Parameters.Add(VCMajorOperator);
                command.Parameters.Add(VCMinorOperator);
                command.Parameters.Add(VCBuildOperator);
                command.Parameters.Add(VCField);

                command.Parameters.Add(VFMajorOperator);
                command.Parameters.Add(VFMinorOperator);
                command.Parameters.Add(VFBuildOperator);
                command.Parameters.Add(VFField);

                command.Parameters.Add(VPMajorOperator);
                command.Parameters.Add(VPMinorOperator);
                command.Parameters.Add(VPBuildOperator);
                command.Parameters.Add(VPField);

                command.Parameters.Add(SelectStatement);
                command.Parameters.Add(SortByStatement);
                //Customer Info
                command.Parameters.Add(MachineSN);
                command.Parameters.Add(CustomerCompany);
                command.Parameters.Add(CustomerLocation);

                command.Parameters.Add(SyteLine);
                command.Parameters.Add(HistorySearch);
                command.Parameters.Add(HistorySearchLogic);

                command.Parameters.Add(Rank);
                command.Parameters.Add(RankCompare);
                command.Parameters.Add(Size);
                command.Parameters.Add(SizeCompare);

                command.Parameters.Add(Keywords);

                value = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
            return value;
        }

        public void UpdateReport
            (
                int ArgsReportID, int ArgsPublicReport, string ArgsName,
                int ArgsREACompare, int? ArgsREANum1, int? ArgsREANum2,
                string ArgsStatus, string ArgsProduct,
                int? ArgsDocChange, int? ArgsDbChange, int? ArgsReqDoc,
                String ArgsWhereClause, int? ArgsMaxRows, string ArgsIssueType,
                int? ArgsCustomerBug, int? ArgsCustomerLogic, string ArgsCustomerName,
                int? ArgsTitleLogic, string ArgsSearchTitle, String ArgsPriorities,
                //Dates
                int? ArgsSubBy,
                int? ArgsSubMonth, int? ArgsSubDay, int? ArgsSubYear, string ArgsSubOperator1, string ArgsSubField1,
                int? ArgsSubMonth2, int? ArgsSubDay2, int? ArgsSubYear2, string ArgsSubOperator2, string ArgsSubField2,
                int? ArgsAssignedTo,
                int? ArgsAsnOnMonth, int? ArgsAsnOnDay, int? ArgsAsnOnYear, string ArgsAsnOnOperator1, string ArgsAsnOnField1,
                int? ArgsAsnOnMonth2, int? ArgsAsnOnDay2, int? ArgsAsnOnYear2, string ArgsAsnOnOperator2, string ArgsAsnOnField2,
                int? ArgsResolvedBy,
                int? ArgsResOnMonth, int? ArgsResOnDay, int? ArgsResOnYear, string ArgsResOperator1, string ArgsResOnField1,
                int? ArgsResOnMonth2, int? ArgsResOnDay2, int? ArgsResOnYear2, string ArgsResOperator2, string ArgsResOnField2,
                int? ArgsClosedBy,
                int? ArgsCloOnMonth, int? ArgsCloOnDay, int? ArgsCloOnYear, string ArgsCloOnOperator1, string ArgsCloOnField1,
                int? ArgsCloOnMonth2, int? ArgsCloOnDay2, int? ArgsCloOnYear2, string ArgsCloOnOperator2, string ArgsCloOnField2,
                int? ArgsModStartMonth, int? ArgsModStartDay, int? ArgsModStartYear, string ArgsModStartOperator, string ArgsModStartField,
                int? ArgsModEndMonth, int? ArgsModEndDay, int? ArgsModEndYear, string ArgsModEndOperator, string ArgsModEndField,
                //Versions
                int? ArgsVF_MAJOR, int? ArgsVF_MINOR, int? ArgsVF_BUILD, String ArgsVF_MISC, String VF_MAJOR_OPERATOR, String VF_MINOR_OPERATOR, String VF_BUILD_OPERATOR, String VF_FIELD,
                int? ArgsVR_MAJOR, int? ArgsVR_MINOR, int? ArgsVR_BUILD, String ArgsVR_MISC, String VR_MAJOR_OPERATOR, String VR_MINOR_OPERATOR, String VR_BUILD_OPERATOR, String VR_FIELD,
                int? ArgsVC_MAJOR, int? ArgsVC_MINOR, int? ArgsVC_BUILD, String ArgsVC_MISC, String VC_MAJOR_OPERATOR, String VC_MINOR_OPERATOR, String VC_BUILD_OPERATOR, String VC_FIELD,
                int? ArgsVP_MAJOR, int? ArgsVP_MINOR, int? ArgsVP_BUILD, String ArgsVP_MISC, String VP_MAJOR_OPERATOR, String VP_MINOR_OPERATOR, String VP_BUILD_OPERATOR, String VP_FIELD,
                //sql Statements
                string ArgsSelectStatement,
                string ArgsSortByStatement,
                //Customer Report
                string ArgsMachineSN, string ArgsSyteLineRef,
                string ArgsCustomerCompany,
                string ArgsCustomerLocation,
                string ArgsHistorySearch, string ArgsHistorySearchLogic,
                int? ArgsRank, int ArgsRankComparator,
                int? ArgsSize, int ArgsSizeComparator,
                string ArgsKeywords
            )
        {
            ///<sumarry>
            /// Updates a report in the database
            ///</summary>
            //Report_UpdateReport
            String Command =
               "EXEC Report_UpdateReport @REPORT_ID, @PUBLIC_REPORT, @NAME," +
               " @REA_COMPARE, @REA_NUM1, @REA_NUM2," +
               " @STATUS, @PRODUCT, " +
               " @DOC_CHANGE, @DB_CHANGE, @REQ_DOC," +
               " @WHERE_CLAUSE, @MAX_ROWS, @ISSUE_TYPE," +
               " @CUSTOMER_BUG, @CUSTOMER_LOGIC, @CUSTOMER_NAME," +
               " @TITLE_LOGIC, @SEARCH_TITLE, @priorities," +
               //Version
               " @VF_MAJOR, @VF_MINOR, @VF_BUILD, @VF_MISC, @VF_MAJOR_OPERATOR, @VF_MINOR_OPERATOR, @VF_BUILD_OPERATOR, @VF_FIELD," +
               " @VR_MAJOR, @VR_MINOR, @VR_BUILD, @VR_MISC, @VR_MAJOR_OPERATOR, @VR_MINOR_OPERATOR, @VR_BUILD_OPERATOR, @VR_FIELD," +
               " @VC_MAJOR, @VC_MINOR, @VC_BUILD, @VC_MISC, @VC_MAJOR_OPERATOR, @VC_MINOR_OPERATOR, @VC_BUILD_OPERATOR, @VC_FIELD," +
               " @PLANNED_MAJOR_VER, @PLANNED_MINOR_VER, @PLANNED_BUILD_NUM, @PLANNED_MISC_VER_INFO, @VP_MAJOR_OPERATOR,@VP_MINOR_OPERATOR,@VP_BUILD_OPERATOR, @VP_FIELD," +
               //Dates
               " @SUB_BY, " +
               " @SUB_MONTH1, @SUB_DAY1, @SUB_YEAR1, @SUB_OPERATOR1, @SUB_FIELD1, " +
               " @SUB_MONTH2, @SUB_DAY2, @SUB_YEAR2, @SUB_OPERATOR2, @SUB_FIELD2, " +
               " @ASSIGN_TO, " +
               " @ASN_ON_MONTH1, @ASN_ON_DAY1, @ASN_ON_YEAR1, @ASN_OPERATOR1, @ASN_FIELD1," +
               " @ASN_ON_MONTH2, @ASN_ON_DAY2, @ASN_ON_YEAR2, @ASN_OPERATOR2, @ASN_FIELD2," +
               " @RESOLVED_BY," +
               " @RES_ON_MONTH1, @RES_ON_DAY1, @RES_ON_YEAR1, @RES_OPERATOR1, @RES_FIELD1," +
               " @RES_ON_MONTH2, @RES_ON_DAY2, @RES_ON_YEAR2, @RES_OPERATOR2, @RES_FIELD2," +
               " @CLOSED_BY, " +
               " @CLO_ON_MONTH1, @CLO_ON_DAY1, @CLO_ON_YEAR1, @CLO_OPERATOR1, @CLO_FIELD1," +
               " @CLO_ON_MONTH2, @CLO_ON_DAY2, @CLO_ON_YEAR2, @CLO_OPERATOR2, @CLO_FIELD2, " +
               " @MODIFIED_START_MONTH, @MODIFIED_START_DAY, @MODIFIED_START_YEAR, @MODIFIED_START_OPERATOR, @MODIFIED_START_FIELD," +
               " @MODIFIED_END_MONTH, @MODIFIED_END_DAY, @MODIFIED_END_YEAR, @MODIFIED_END_OPERATOR, @MODIFIED_END_FIELD, " +
               " @SELECT_STATEMENT, " +
               " @SORT_BY_STATEMENT,  @MACHINE_SN, @SYTELINE, @CUSTOMER_LOCATION, @CUSTOMER_COMPANY, @HISTORY_SEARCH, @HISTORY_SEARCH_LOGIC, " +
               " @RANK_ORDER, @RANK_COMPARE, @SIZE, @SIZE_COMPARE, @KEYWORDS"
               ;

            SqlParameter ReportID = new SqlParameter(); //The only Required Field
            SqlParameter PublicReport = new SqlParameter();
            SqlParameter Name = new SqlParameter();
            SqlParameter REACompare = new SqlParameter();
            SqlParameter REANum1 = new SqlParameter();
            SqlParameter REANum2 = new SqlParameter();
            SqlParameter Status = new SqlParameter();
            SqlParameter Product = new SqlParameter();
            SqlParameter DocChange = new SqlParameter();
            SqlParameter DbChange = new SqlParameter();
            SqlParameter ReqDoc = new SqlParameter();
            SqlParameter WhereClause = new SqlParameter();
            SqlParameter MaxRows = new SqlParameter();
            SqlParameter IssueType = new SqlParameter();
            SqlParameter CustomerBug = new SqlParameter();
            SqlParameter CustomerLogic = new SqlParameter();
            SqlParameter CustomerName = new SqlParameter();
            SqlParameter TitleLogic = new SqlParameter();
            SqlParameter SearchTitle = new SqlParameter();
            SqlParameter Priorities = new SqlParameter();
            SqlParameter SubBy = new SqlParameter();
            SqlParameter SubMonth = new SqlParameter();
            SqlParameter SubDay = new SqlParameter();
            SqlParameter Sub_Year = new SqlParameter();
            SqlParameter SubMonth2 = new SqlParameter();
            SqlParameter SubDay2 = new SqlParameter();
            SqlParameter SubYear2 = new SqlParameter();
            SqlParameter AssignedTo = new SqlParameter();
            SqlParameter AsnOnMonth = new SqlParameter();
            SqlParameter AsnOnDay = new SqlParameter();
            SqlParameter AsnOnYear = new SqlParameter();
            SqlParameter AsnOnMonth2 = new SqlParameter();
            SqlParameter AsnOnDay2 = new SqlParameter();
            SqlParameter AsnOnYear2 = new SqlParameter();
            SqlParameter ResolvedBy = new SqlParameter();
            SqlParameter ResOnMonth = new SqlParameter();
            SqlParameter ResOnDay = new SqlParameter();
            SqlParameter ResOnYear = new SqlParameter();
            SqlParameter ResOnMonth2 = new SqlParameter();
            SqlParameter ResOnDay2 = new SqlParameter();
            SqlParameter ResOnYear2 = new SqlParameter();
            SqlParameter ClosedBy = new SqlParameter();
            SqlParameter CloOnMonth = new SqlParameter();
            SqlParameter CloOnDay = new SqlParameter();
            SqlParameter CloOnYear = new SqlParameter();
            SqlParameter CloOnMonth2 = new SqlParameter();
            SqlParameter CloOnDay2 = new SqlParameter();
            SqlParameter CloOnYear2 = new SqlParameter();
            SqlParameter CloOnOperator1 = new SqlParameter();
            SqlParameter ResOnOperator1 = new SqlParameter();
            SqlParameter AsnOnOperator1 = new SqlParameter();
            SqlParameter SubOperator1 = new SqlParameter();
            SqlParameter CloOnOperator2 = new SqlParameter();
            SqlParameter ResOnOperator2 = new SqlParameter();
            SqlParameter AsnOnOperator2 = new SqlParameter();
            SqlParameter SubOperator2 = new SqlParameter();
            SqlParameter CloOnField1 = new SqlParameter();
            SqlParameter ResOnField1 = new SqlParameter();
            SqlParameter AsnOnField1 = new SqlParameter();
            SqlParameter SubField1 = new SqlParameter();
            SqlParameter CloOnField2 = new SqlParameter();
            SqlParameter ResOnField2 = new SqlParameter();
            SqlParameter AsnOnField2 = new SqlParameter();
            SqlParameter SubField2 = new SqlParameter();
            SqlParameter ModStartMonth = new SqlParameter();
            SqlParameter ModStartDay = new SqlParameter();
            SqlParameter ModStartYear = new SqlParameter();
            SqlParameter ModEndMonth = new SqlParameter();
            SqlParameter ModEndYear = new SqlParameter();
            SqlParameter ModEndDay = new SqlParameter();
            SqlParameter ModStartOperator = new SqlParameter();
            SqlParameter ModEndOperator = new SqlParameter();
            SqlParameter ModStartField = new SqlParameter();
            SqlParameter ModEndField = new SqlParameter();

            SqlParameter VFMajor = new SqlParameter();
            SqlParameter VFMinor = new SqlParameter();
            SqlParameter VFBuild = new SqlParameter();
            SqlParameter VFMisc = new SqlParameter();
            SqlParameter VRMajor = new SqlParameter();
            SqlParameter VRMinor = new SqlParameter();
            SqlParameter VRBuild = new SqlParameter();
            SqlParameter VRMisc = new SqlParameter();
            SqlParameter VCMajor = new SqlParameter();
            SqlParameter VCMinor = new SqlParameter();
            SqlParameter VCBuild = new SqlParameter();
            SqlParameter VCMisc = new SqlParameter();
            SqlParameter VPMajor = new SqlParameter();
            SqlParameter VPMinor = new SqlParameter();
            SqlParameter VPBuild = new SqlParameter();
            SqlParameter VPMisc = new SqlParameter();
            SqlParameter VRMajorOperator = new SqlParameter();
            SqlParameter VRMinorOperator = new SqlParameter();
            SqlParameter VRBuildOperator = new SqlParameter();
            SqlParameter VRField = new SqlParameter();
            SqlParameter VCMajorOperator = new SqlParameter();
            SqlParameter VCMinorOperator = new SqlParameter();
            SqlParameter VCBuildOperator = new SqlParameter();
            SqlParameter VCField = new SqlParameter();
            SqlParameter VFMajorOperator = new SqlParameter();
            SqlParameter VFMinorOperator = new SqlParameter();
            SqlParameter VFBuildOperator = new SqlParameter();
            SqlParameter VFField = new SqlParameter();
            SqlParameter VPMajorOperator = new SqlParameter();
            SqlParameter VPMinorOperator = new SqlParameter();
            SqlParameter VPBuildOperator = new SqlParameter();
            SqlParameter VPField = new SqlParameter();

            SqlParameter SelectStatement = new SqlParameter();
            SqlParameter SortByStatement = new SqlParameter();
            //Customer info
            SqlParameter MachineSN = new SqlParameter();
            SqlParameter SyteLine = new SqlParameter();
            SqlParameter CustomerCompany = new SqlParameter();
            SqlParameter CustomerLocation = new SqlParameter();
            SqlParameter HistorySearch = new SqlParameter();
            SqlParameter HistorySearchLogic = new SqlParameter();

            SqlParameter Rank = new SqlParameter();
            SqlParameter RankCompare = new SqlParameter();
            SqlParameter Size = new SqlParameter();
            SqlParameter SizeCompare = new SqlParameter();

            SqlParameter Keywords = new SqlParameter();

            //Display

            //Add in the other fields, Don't forget to add them into the Sql Server too.
            //General
            ReportID.SqlDbType = SqlDbType.Int;
            PublicReport.SqlDbType = SqlDbType.SmallInt;
            Name.SqlDbType = SqlDbType.VarChar;
            REACompare.SqlDbType = SqlDbType.Int;
            REANum1.SqlDbType = SqlDbType.Int;
            REANum2.SqlDbType = SqlDbType.Int;
            Status.SqlDbType = SqlDbType.VarChar;
            Product.SqlDbType = SqlDbType.VarChar;
            DocChange.SqlDbType = SqlDbType.Int;
            DbChange.SqlDbType = SqlDbType.Int;
            ReqDoc.SqlDbType = SqlDbType.Int;
            WhereClause.SqlDbType = SqlDbType.Text;
            MaxRows.SqlDbType = SqlDbType.Int;
            IssueType.SqlDbType = SqlDbType.VarChar;
            CustomerBug.SqlDbType = SqlDbType.Int;
            CustomerLogic.SqlDbType = SqlDbType.Int;
            CustomerName.SqlDbType = SqlDbType.VarChar;
            TitleLogic.SqlDbType = SqlDbType.Int;
            SearchTitle.SqlDbType = SqlDbType.VarChar;
            Priorities.SqlDbType = SqlDbType.VarChar;
            //Version
            //Major
            VFMajor.SqlDbType = SqlDbType.Int;
            VFMajorOperator.SqlDbType = SqlDbType.VarChar;
            VRMajor.SqlDbType = SqlDbType.Int;
            VRMajorOperator.SqlDbType = SqlDbType.VarChar;
            VCMajor.SqlDbType = SqlDbType.Int;
            VCMajorOperator.SqlDbType = SqlDbType.VarChar;
            VPMajor.SqlDbType = SqlDbType.Int;
            VPMajorOperator.SqlDbType = SqlDbType.VarChar;
            //Minor
            VFMinor.SqlDbType = SqlDbType.Int;
            VFMinorOperator.SqlDbType = SqlDbType.VarChar;
            VRMinor.SqlDbType = SqlDbType.Int;
            VRMinorOperator.SqlDbType = SqlDbType.VarChar;
            VCMinor.SqlDbType = SqlDbType.Int;
            VCMinorOperator.SqlDbType = SqlDbType.VarChar;
            VPMinor.SqlDbType = SqlDbType.Int;
            VPMinorOperator.SqlDbType = SqlDbType.VarChar;
            //Build
            VFBuild.SqlDbType = SqlDbType.Int;
            VFBuildOperator.SqlDbType = SqlDbType.VarChar;
            VRBuild.SqlDbType = SqlDbType.Int;
            VRBuildOperator.SqlDbType = SqlDbType.VarChar;
            VCBuild.SqlDbType = SqlDbType.Int;
            VCBuildOperator.SqlDbType = SqlDbType.VarChar;
            VPBuild.SqlDbType = SqlDbType.Int;
            VPBuildOperator.SqlDbType = SqlDbType.VarChar;
            //Misc
            VFMisc.SqlDbType = SqlDbType.VarChar;
            VRMisc.SqlDbType = SqlDbType.VarChar;
            VCMisc.SqlDbType = SqlDbType.VarChar;
            VPMisc.SqlDbType = SqlDbType.VarChar;
            //Field
            VFField.SqlDbType = SqlDbType.VarChar;
            VRField.SqlDbType = SqlDbType.VarChar;
            VCField.SqlDbType = SqlDbType.VarChar;
            VPField.SqlDbType = SqlDbType.VarChar;
            //Date
            //Submitted On
            SubBy.SqlDbType = SqlDbType.Int;
            SubMonth.SqlDbType = SqlDbType.Int;
            SubDay.SqlDbType = SqlDbType.Int;
            Sub_Year.SqlDbType = SqlDbType.Int;

            SubMonth2.SqlDbType = SqlDbType.Int;
            SubDay2.SqlDbType = SqlDbType.Int;
            SubYear2.SqlDbType = SqlDbType.Int;
            SubOperator1.SqlDbType = SqlDbType.VarChar;
            SubOperator2.SqlDbType = SqlDbType.VarChar;
            SubField1.SqlDbType = SqlDbType.VarChar;
            SubField2.SqlDbType = SqlDbType.VarChar;
            //Assigned On
            AssignedTo.SqlDbType = SqlDbType.Int;
            AsnOnMonth.SqlDbType = SqlDbType.Int;
            AsnOnDay.SqlDbType = SqlDbType.Int;
            AsnOnYear.SqlDbType = SqlDbType.Int;
            AsnOnMonth2.SqlDbType = SqlDbType.Int;
            AsnOnDay2.SqlDbType = SqlDbType.Int;
            AsnOnYear2.SqlDbType = SqlDbType.Int;
            AsnOnOperator1.SqlDbType = SqlDbType.VarChar;
            AsnOnOperator2.SqlDbType = SqlDbType.VarChar;
            AsnOnField1.SqlDbType = SqlDbType.VarChar;
            AsnOnField2.SqlDbType = SqlDbType.VarChar;
            //Resolved On
            ResolvedBy.SqlDbType = SqlDbType.Int;
            ResOnMonth.SqlDbType = SqlDbType.Int;
            ResOnDay.SqlDbType = SqlDbType.Int;
            ResOnYear.SqlDbType = SqlDbType.Int;
            ResOnMonth2.SqlDbType = SqlDbType.Int;
            ResOnDay2.SqlDbType = SqlDbType.Int;
            ResOnYear2.SqlDbType = SqlDbType.Int;
            ResOnOperator1.SqlDbType = SqlDbType.VarChar;
            ResOnOperator2.SqlDbType = SqlDbType.VarChar;
            ResOnField1.SqlDbType = SqlDbType.VarChar;
            ResOnField2.SqlDbType = SqlDbType.VarChar;
            //Closed On
            ClosedBy.SqlDbType = SqlDbType.Int;
            CloOnMonth.SqlDbType = SqlDbType.Int;
            CloOnDay.SqlDbType = SqlDbType.Int;
            CloOnYear.SqlDbType = SqlDbType.Int;
            CloOnMonth2.SqlDbType = SqlDbType.Int;
            CloOnDay2.SqlDbType = SqlDbType.Int;
            CloOnYear2.SqlDbType = SqlDbType.Int;
            CloOnOperator1.SqlDbType = SqlDbType.VarChar;
            CloOnOperator2.SqlDbType = SqlDbType.VarChar;
            CloOnField1.SqlDbType = SqlDbType.VarChar;
            CloOnField2.SqlDbType = SqlDbType.VarChar;
            //Modified On
            ModStartMonth.SqlDbType = SqlDbType.Int;
            ModStartDay.SqlDbType = SqlDbType.Int;
            ModStartYear.SqlDbType = SqlDbType.Int;
            ModEndMonth.SqlDbType = SqlDbType.Int;
            ModEndDay.SqlDbType = SqlDbType.Int;
            ModEndYear.SqlDbType = SqlDbType.Int;
            ModStartOperator.SqlDbType = SqlDbType.VarChar;
            ModEndOperator.SqlDbType = SqlDbType.VarChar;
            ModStartField.SqlDbType = SqlDbType.VarChar;
            ModEndField.SqlDbType = SqlDbType.VarChar;
            //SQl
            SelectStatement.SqlDbType = SqlDbType.VarChar;
            SortByStatement.SqlDbType = SqlDbType.VarChar;

            //Customer info
            MachineSN.SqlDbType = SqlDbType.VarChar;
            SyteLine.SqlDbType = SqlDbType.VarChar;
            CustomerCompany.SqlDbType = SqlDbType.VarChar;
            CustomerLocation.SqlDbType = SqlDbType.VarChar;
            HistorySearch.SqlDbType = SqlDbType.VarChar;
            HistorySearchLogic.SqlDbType = SqlDbType.Int;

            Rank.SqlDbType = SqlDbType.Int;
            RankCompare.SqlDbType = SqlDbType.Int;
            Size.SqlDbType = SqlDbType.Int;
            SizeCompare.SqlDbType = SqlDbType.Int;



            ReportID.ParameterName = "REPORT_ID";
            PublicReport.ParameterName = "PUBLIC_REPORT";
            Name.ParameterName = "NAME";
            REACompare.ParameterName = "REA_COMPARE";
            REANum1.ParameterName = "REA_NUM1";
            REANum2.ParameterName = "REA_NUM2";
            Status.ParameterName = "STATUS";
            Product.ParameterName = "PRODUCT";

            DocChange.ParameterName = "DOC_CHANGE";
            DbChange.ParameterName = "DB_CHANGE";
            ReqDoc.ParameterName = "REQ_DOC";
            WhereClause.ParameterName = "WHERE_CLAUSE";
            MaxRows.ParameterName = "MAX_ROWS";
            IssueType.ParameterName = "ISSUE_TYPE";
            CustomerBug.ParameterName = "CUSTOMER_BUG";
            CustomerLogic.ParameterName = "CUSTOMER_LOGIC";
            CustomerName.ParameterName = "CUSTOMER_NAME";
            TitleLogic.ParameterName = "TITLE_LOGIC";
            SearchTitle.ParameterName = "SEARCH_TITLE";
            Priorities.ParameterName = "Priorities";
            //Dates
            SubBy.ParameterName = "SUB_BY";
            SubMonth.ParameterName = "SUB_MONTH1";
            SubDay.ParameterName = "SUB_DAY1";
            Sub_Year.ParameterName = "SUB_YEAR1";
            SubMonth2.ParameterName = "SUB_MONTH2";
            SubDay2.ParameterName = "SUB_DAY2";
            SubYear2.ParameterName = "SUB_YEAR2";
            AssignedTo.ParameterName = "ASSIGN_TO";
            AsnOnMonth.ParameterName = "ASN_ON_MONTH1";
            AsnOnDay.ParameterName = "ASN_ON_DAY1";
            AsnOnYear.ParameterName = "ASN_ON_YEAR1";
            AsnOnMonth2.ParameterName = "ASN_ON_MONTH2";
            AsnOnDay2.ParameterName = "ASN_ON_DAY2";
            AsnOnYear2.ParameterName = "ASN_ON_YEAR2";
            ResolvedBy.ParameterName = "RESOLVED_BY";
            ResOnMonth.ParameterName = "RES_ON_MONTH1";
            ResOnDay.ParameterName = "RES_ON_DAY1";
            ResOnYear.ParameterName = "RES_ON_YEAR1";
            ResOnMonth2.ParameterName = "RES_ON_MONTH2";
            ResOnDay2.ParameterName = "RES_ON_DAY2";
            ResOnYear2.ParameterName = "RES_ON_YEAR2";
            ClosedBy.ParameterName = "CLOSED_BY";
            CloOnMonth.ParameterName = "CLO_ON_MONTH1";
            CloOnDay.ParameterName = "CLO_ON_DAY1";
            CloOnYear.ParameterName = "CLO_ON_YEAR1";
            CloOnMonth2.ParameterName = "CLO_ON_MONTH2";
            CloOnDay2.ParameterName = "CLO_ON_DAY2";
            CloOnYear2.ParameterName = "CLO_ON_YEAR2";

            ModStartMonth.ParameterName = "MODIFIED_START_MONTH";
            ModStartDay.ParameterName = "MODIFIED_START_DAY";
            ModStartYear.ParameterName = "MODIFIED_START_YEAR";
            ModEndMonth.ParameterName = "MODIFIED_END_MONTH";
            ModEndYear.ParameterName = "MODIFIED_END_DAY";
            ModEndDay.ParameterName = "MODIFIED_END_YEAR";
            ModStartOperator.ParameterName = "MODIFIED_START_OPERATOR";
            ModEndOperator.ParameterName = "MODIFIED_END_OPERATOR";
            ModStartField.ParameterName = "MODIFIED_START_FIELD";
            ModEndField.ParameterName = "MODIFIED_END_FIELD";

            CloOnOperator1.ParameterName = "CLO_OPERATOR1";
            ResOnOperator1.ParameterName = "RES_OPERATOR1";
            AsnOnOperator1.ParameterName = "ASN_OPERATOR1";
            SubOperator1.ParameterName = "SUB_OPERATOR1";
            CloOnOperator2.ParameterName = "CLO_OPERATOR2";
            ResOnOperator2.ParameterName = "RES_OPERATOR2";
            AsnOnOperator2.ParameterName = "ASN_OPERATOR2";
            SubOperator2.ParameterName = "SUB_OPERATOR2";
            CloOnField1.ParameterName = "CLO_FIELD1";
            ResOnField1.ParameterName = "RES_FIELD1";
            AsnOnField1.ParameterName = "ASN_FIELD1";
            SubField1.ParameterName = "SUB_FIELD1";
            CloOnField2.ParameterName = "CLO_FIELD2";
            ResOnField2.ParameterName = "RES_FIELD2";
            AsnOnField2.ParameterName = "ASN_FIELD2";
            SubField2.ParameterName = "SUB_FIELD2";
            //Versions
            VFMajor.ParameterName = "VF_MAJOR";
            VFMinor.ParameterName = "VF_MINOR";
            VFBuild.ParameterName = "VF_BUILD";
            VFMisc.ParameterName = "VF_MISC";

            VRMajor.ParameterName = "VR_MAJOR";
            VRMinor.ParameterName = "VR_MINOR";
            VRBuild.ParameterName = "VR_BUILD";
            VRMisc.ParameterName = "VR_MISC";

            VCMajor.ParameterName = "VC_MAJOR";
            VCMinor.ParameterName = "VC_MINOR";
            VCBuild.ParameterName = "VC_BUILD";
            VCMisc.ParameterName = "VC_MISC";

            VPMajor.ParameterName = "PLANNED_MAJOR_VER";
            VPMinor.ParameterName = "PLANNED_MINOR_VER";
            VPBuild.ParameterName = "PLANNED_BUILD_NUM";
            VPMisc.ParameterName = "PLANNED_MISC_VER_INFO";

            VRMajorOperator.ParameterName = "VR_MAJOR_OPERATOR";
            VRMinorOperator.ParameterName = "VR_MINOR_OPERATOR";
            VRBuildOperator.ParameterName = "VR_BUILD_OPERATOR";
            VRField.ParameterName = "VR_FIELD";
            VCMajorOperator.ParameterName = "VC_MAJOR_OPERATOR";
            VCMinorOperator.ParameterName = "VC_MINOR_OPERATOR";
            VCBuildOperator.ParameterName = "VC_BUILD_OPERATOR";
            VCField.ParameterName = "VC_FIELD";
            VFMajorOperator.ParameterName = "VF_MAJOR_OPERATOR";
            VFMinorOperator.ParameterName = "VF_MINOR_OPERATOR";
            VFBuildOperator.ParameterName = "VF_BUILD_OPERATOR";
            VFField.ParameterName = "VF_FIELD";
            VPMajorOperator.ParameterName = "VP_MAJOR_OPERATOR";
            VPMinorOperator.ParameterName = "VP_MINOR_OPERATOR";
            VPBuildOperator.ParameterName = "VP_BUILD_OPERATOR";
            VPField.ParameterName = "VP_FIELD";
            SelectStatement.ParameterName = "SELECT_STATEMENT";
            SortByStatement.ParameterName = "SORT_BY_STATEMENT";
            //Customer info
            MachineSN.ParameterName = "MACHINE_SN";
            SyteLine.ParameterName = "SYTELINE";
            CustomerCompany.ParameterName = "CUSTOMER_LOCATION";
            CustomerLocation.ParameterName = "CUSTOMER_COMPANY";
            HistorySearch.ParameterName = "HISTORY_SEARCH";
            HistorySearchLogic.ParameterName = "HISTORY_SEARCH_LOGIC";

            Rank.ParameterName = "RANK_ORDER";
            RankCompare.ParameterName = "RANK_COMPARE";
            Size.ParameterName = "SIZE";
            SizeCompare.ParameterName = "SIZE_COMPARE";

            Keywords.ParameterName = "KEYWORDS";

            ReportID.Value = (ArgsReportID == 0 ? (object)DBNull.Value : ArgsReportID);
            PublicReport.Value = ((ArgsPublicReport != 1 && ArgsPublicReport != 0) ? (object)DBNull.Value : ArgsPublicReport);
            Name.Value = (ArgsName == null ? (object)DBNull.Value : ArgsName);
            REACompare.Value = (ArgsREACompare == 0 ? (object)DBNull.Value : ArgsREACompare);
            REANum1.Value = (ArgsREANum1 == null || ArgsREANum1 == 0 ? (object)DBNull.Value : ArgsREANum1);
            REANum2.Value = (ArgsREANum2 == null || ArgsREANum2 == 0 ? (object)DBNull.Value : ArgsREANum2);
            Status.Value = (String.IsNullOrEmpty(ArgsStatus) ? (object)DBNull.Value : ArgsStatus);
            Product.Value = (String.IsNullOrEmpty(ArgsProduct) ? (object)DBNull.Value : ArgsProduct);

            DocChange.Value = (ArgsDocChange == null ? (object)DBNull.Value : ArgsDocChange);
            DbChange.Value = (ArgsDbChange == null ? (object)DBNull.Value : ArgsDbChange);
            MaxRows.Value = (ArgsMaxRows == null ? (object)DBNull.Value : ArgsMaxRows);
            ReqDoc.Value = (ArgsReqDoc == null ? (object)DBNull.Value : ArgsReqDoc);
            WhereClause.Value = (String.IsNullOrEmpty(ArgsWhereClause) || String.IsNullOrWhiteSpace(ArgsWhereClause) ? (object)DBNull.Value : ArgsWhereClause);
            IssueType.Value = (String.IsNullOrEmpty(ArgsIssueType) ? (object)DBNull.Value : ArgsIssueType);
            CustomerBug.Value = (ArgsCustomerBug == null ? (object)DBNull.Value : ArgsCustomerBug);
            CustomerLogic.Value = ((ArgsCustomerLogic == 0 || String.IsNullOrEmpty(ArgsCustomerName)) ? (object)DBNull.Value : ArgsCustomerLogic);
            CustomerName.Value = (ArgsCustomerName == null ? (object)DBNull.Value : ArgsCustomerName);
            TitleLogic.Value = ((ArgsTitleLogic == 0 || String.IsNullOrEmpty(ArgsSearchTitle)) ? (object)DBNull.Value : ArgsTitleLogic);
            SearchTitle.Value = (ArgsSearchTitle == null ? (object)DBNull.Value : ArgsSearchTitle);
            Priorities.Value = (String.IsNullOrEmpty(ArgsPriorities) ? (object)DBNull.Value : ArgsPriorities);
            //Dates
            SubBy.Value = (ArgsSubBy == null ? (object)DBNull.Value : ArgsSubBy);
            SubMonth.Value = (ArgsSubMonth == null ? (object)DBNull.Value : ArgsSubMonth);
            SubDay.Value = (ArgsSubDay == null ? (object)DBNull.Value : ArgsSubDay);
            Sub_Year.Value = (ArgsSubYear == null ? (object)DBNull.Value : ArgsSubYear);
            SubMonth2.Value = (ArgsSubMonth2 == null ? (object)DBNull.Value : ArgsSubMonth2);
            SubDay2.Value = (ArgsSubDay2 == null ? (object)DBNull.Value : ArgsSubDay2);
            SubYear2.Value = (ArgsSubYear2 == null ? (object)DBNull.Value : ArgsSubYear2);
            AssignedTo.Value = (ArgsAssignedTo == null ? (object)DBNull.Value : ArgsAssignedTo);
            AsnOnMonth.Value = (ArgsAsnOnMonth == null ? (object)DBNull.Value : ArgsAsnOnMonth);
            AsnOnDay.Value = (ArgsAsnOnDay == null ? (object)DBNull.Value : ArgsAsnOnDay);
            AsnOnYear.Value = (ArgsAsnOnYear == null ? (object)DBNull.Value : ArgsAsnOnYear);
            AsnOnMonth2.Value = (ArgsAsnOnMonth2 == null ? (object)DBNull.Value : ArgsAsnOnMonth2);
            AsnOnDay2.Value = (ArgsAsnOnDay2 == null ? (object)DBNull.Value : ArgsAsnOnDay2);
            AsnOnYear2.Value = (ArgsAsnOnYear2 == null ? (object)DBNull.Value : ArgsAsnOnYear2);
            ResolvedBy.Value = (ArgsResolvedBy == null ? (object)DBNull.Value : ArgsResolvedBy);
            ResOnMonth.Value = (ArgsResOnMonth == null ? (object)DBNull.Value : ArgsResOnMonth);
            ResOnDay.Value = (ArgsResOnDay == null ? (object)DBNull.Value : ArgsResOnDay);
            ResOnYear.Value = (ArgsResOnYear == null ? (object)DBNull.Value : ArgsResOnYear);
            ResOnMonth2.Value = (ArgsResOnMonth2 == null ? (object)DBNull.Value : ArgsResOnMonth2);
            ResOnDay2.Value = (ArgsResOnDay2 == null ? (object)DBNull.Value : ArgsResOnDay2);
            ResOnYear2.Value = (ArgsResOnYear2 == null ? (object)DBNull.Value : ArgsResOnYear2);
            ClosedBy.Value = (ArgsClosedBy == null ? (object)DBNull.Value : ArgsClosedBy);
            CloOnMonth.Value = (ArgsCloOnMonth == null ? (object)DBNull.Value : ArgsCloOnMonth);
            CloOnDay.Value = (ArgsCloOnDay == null ? (object)DBNull.Value : ArgsCloOnDay);
            CloOnYear.Value = (ArgsCloOnYear == null ? (object)DBNull.Value : ArgsCloOnYear);
            CloOnMonth2.Value = (ArgsCloOnMonth2 == null ? (object)DBNull.Value : ArgsCloOnMonth2);
            CloOnDay2.Value = (ArgsCloOnDay2 == null ? (object)DBNull.Value : ArgsCloOnDay2);
            CloOnYear2.Value = (ArgsCloOnYear2 == null ? (object)DBNull.Value : ArgsCloOnYear2);
            ModStartMonth.Value = (ArgsModStartMonth == null ? (object)DBNull.Value : ArgsModStartMonth);
            ModStartDay.Value = (ArgsModStartDay == null ? (object)DBNull.Value : ArgsModStartDay);
            ModStartYear.Value = (ArgsModStartYear == null ? (object)DBNull.Value : ArgsModStartYear);
            ModEndMonth.Value = (ArgsModEndMonth == null ? (object)DBNull.Value : ArgsModEndMonth);
            ModEndYear.Value = (ArgsModEndDay == null ? (object)DBNull.Value : ArgsModEndDay);
            ModEndDay.Value = (ArgsModEndYear == null ? (object)DBNull.Value : ArgsModEndYear);
            ModStartOperator.Value = (String.IsNullOrEmpty(ArgsModStartOperator) ? (object)DBNull.Value : ArgsModStartOperator);
            ModEndOperator.Value = (String.IsNullOrEmpty(ArgsModEndOperator) ? (object)DBNull.Value : ArgsModEndOperator);
            ModStartField.Value = (String.IsNullOrEmpty(ArgsModStartField) ? (object)DBNull.Value : ArgsModStartField);
            ModEndField.Value = (String.IsNullOrEmpty(ArgsModEndField) ? (object)DBNull.Value : ArgsModEndField);

            CloOnOperator1.Value = (String.IsNullOrEmpty(ArgsCloOnOperator1) ? (object)DBNull.Value : ArgsCloOnOperator1);
            ResOnOperator1.Value = (String.IsNullOrEmpty(ArgsResOperator1) ? (object)DBNull.Value : ArgsResOperator1);
            AsnOnOperator1.Value = (String.IsNullOrEmpty(ArgsAsnOnOperator1) ? (object)DBNull.Value : ArgsAsnOnOperator1);
            SubOperator1.Value = (String.IsNullOrEmpty(ArgsSubOperator1) ? (object)DBNull.Value : ArgsSubOperator1);
            CloOnOperator2.Value = (String.IsNullOrEmpty(ArgsCloOnOperator2) ? (object)DBNull.Value : ArgsCloOnOperator2);
            ResOnOperator2.Value = (String.IsNullOrEmpty(ArgsResOperator2) ? (object)DBNull.Value : ArgsResOperator2);
            AsnOnOperator2.Value = (String.IsNullOrEmpty(ArgsAsnOnOperator2) ? (object)DBNull.Value : ArgsAsnOnOperator2);
            SubOperator2.Value = (String.IsNullOrEmpty(ArgsSubOperator2) ? (object)DBNull.Value : ArgsSubOperator2);
            CloOnField1.Value = (String.IsNullOrEmpty(ArgsCloOnField1) ? (object)DBNull.Value : ArgsCloOnField1);
            ResOnField1.Value = (String.IsNullOrEmpty(ArgsResOnField1) ? (object)DBNull.Value : ArgsResOnField1);
            AsnOnField1.Value = (String.IsNullOrEmpty(ArgsAsnOnField1) ? (object)DBNull.Value : ArgsAsnOnField1);
            SubField1.Value = (String.IsNullOrEmpty(ArgsSubField1) ? (object)DBNull.Value : ArgsSubField1);
            CloOnField2.Value = (String.IsNullOrEmpty(ArgsCloOnField2) ? (object)DBNull.Value : ArgsCloOnField2);
            ResOnField2.Value = (String.IsNullOrEmpty(ArgsResOnField2) ? (object)DBNull.Value : ArgsResOnField2);
            AsnOnField2.Value = (String.IsNullOrEmpty(ArgsAsnOnField2) ? (object)DBNull.Value : ArgsAsnOnField2);
            SubField2.Value = (String.IsNullOrEmpty(ArgsSubField2) ? (object)DBNull.Value : ArgsSubField2);

            //Versions
            VFMajor.Value = (ArgsVF_MAJOR == null ? (object)DBNull.Value : ArgsVF_MAJOR);
            VFMinor.Value = (ArgsVF_MINOR == null ? (object)DBNull.Value : ArgsVF_MINOR);
            VFBuild.Value = (ArgsVF_BUILD == null ? (object)DBNull.Value : ArgsVF_BUILD);
            VFMisc.Value = (String.IsNullOrEmpty(ArgsVF_MISC) ? (object)DBNull.Value : ArgsVF_MISC);

            VRMajor.Value = (ArgsVR_MAJOR == null ? (object)DBNull.Value : ArgsVR_MAJOR);
            VRMinor.Value = (ArgsVR_MINOR == null ? (object)DBNull.Value : ArgsVR_MINOR);
            VRBuild.Value = (ArgsVR_BUILD == null ? (object)DBNull.Value : ArgsVR_BUILD);
            VRMisc.Value = (String.IsNullOrEmpty(ArgsVR_MISC) ? (object)DBNull.Value : ArgsVR_MISC);

            VCMajor.Value = (ArgsVC_MAJOR == null ? (object)DBNull.Value : ArgsVC_MAJOR);
            VCMinor.Value = (ArgsVC_MINOR == null ? (object)DBNull.Value : ArgsVC_MINOR);
            VCBuild.Value = (ArgsVC_BUILD == null ? (object)DBNull.Value : ArgsVC_BUILD);
            VCMisc.Value = (String.IsNullOrEmpty(ArgsVC_MISC) ? (object)DBNull.Value : ArgsVC_MISC);

            VPMajor.Value = (ArgsVP_MAJOR == null ? (object)DBNull.Value : ArgsVP_MAJOR);
            VPMinor.Value = (ArgsVP_MINOR == null ? (object)DBNull.Value : ArgsVP_MINOR);
            VPBuild.Value = (ArgsVP_BUILD == null ? (object)DBNull.Value : ArgsVP_BUILD);
            VPMisc.Value = (String.IsNullOrEmpty(ArgsVP_MISC) ? (object)DBNull.Value : ArgsVP_MISC);

            VRMajorOperator.Value = (String.IsNullOrEmpty(VR_MAJOR_OPERATOR) ? (object)DBNull.Value : VR_MAJOR_OPERATOR);
            VRMinorOperator.Value = (String.IsNullOrEmpty(VR_MINOR_OPERATOR) ? (object)DBNull.Value : VR_MINOR_OPERATOR);
            VRBuildOperator.Value = (String.IsNullOrEmpty(VR_BUILD_OPERATOR) ? (object)DBNull.Value : VR_BUILD_OPERATOR);
            VRField.Value = (String.IsNullOrEmpty(VR_FIELD) ? (object)DBNull.Value : VR_FIELD);
            VCMajorOperator.Value = (String.IsNullOrEmpty(VC_MAJOR_OPERATOR) ? (object)DBNull.Value : VC_MAJOR_OPERATOR);
            VCMinorOperator.Value = (String.IsNullOrEmpty(VC_MINOR_OPERATOR) ? (object)DBNull.Value : VC_MINOR_OPERATOR);
            VCBuildOperator.Value = (String.IsNullOrEmpty(VC_BUILD_OPERATOR) ? (object)DBNull.Value : VC_BUILD_OPERATOR);
            VCField.Value = (String.IsNullOrEmpty(VC_FIELD) ? (object)DBNull.Value : VC_FIELD);
            VFMajorOperator.Value = (String.IsNullOrEmpty(VF_MAJOR_OPERATOR) ? (object)DBNull.Value : VF_MAJOR_OPERATOR);
            VFMinorOperator.Value = (String.IsNullOrEmpty(VF_MINOR_OPERATOR) ? (object)DBNull.Value : VF_MINOR_OPERATOR);
            VFBuildOperator.Value = (String.IsNullOrEmpty(VF_BUILD_OPERATOR) ? (object)DBNull.Value : VF_BUILD_OPERATOR);
            VFField.Value = (String.IsNullOrEmpty(VF_FIELD) ? (object)DBNull.Value : VF_FIELD);
            VPMajorOperator.Value = (String.IsNullOrEmpty(VP_MAJOR_OPERATOR) ? (object)DBNull.Value : VP_MAJOR_OPERATOR);
            VPMinorOperator.Value = (String.IsNullOrEmpty(VP_MINOR_OPERATOR) ? (object)DBNull.Value : VP_MINOR_OPERATOR);
            VPBuildOperator.Value = (String.IsNullOrEmpty(VP_BUILD_OPERATOR) ? (object)DBNull.Value : VP_BUILD_OPERATOR);
            VPField.Value = (String.IsNullOrEmpty(VP_FIELD) ? (object)DBNull.Value : VP_FIELD);
            //SQL
            SelectStatement.Value = (String.IsNullOrEmpty(ArgsSelectStatement) ? (object)DBNull.Value : ArgsSelectStatement);
            SortByStatement.Value = (String.IsNullOrEmpty(ArgsSortByStatement) ? (object)DBNull.Value : ArgsSortByStatement);
            //Customer Info
            MachineSN.Value = (String.IsNullOrEmpty(ArgsMachineSN) ? (object)DBNull.Value : ArgsMachineSN);
            SyteLine.Value = (String.IsNullOrEmpty(ArgsSyteLineRef) ? (object)DBNull.Value : ArgsSyteLineRef);
            CustomerCompany.Value = (String.IsNullOrEmpty(ArgsCustomerCompany) ? (object)DBNull.Value : ArgsCustomerCompany);
            CustomerLocation.Value = (String.IsNullOrEmpty(ArgsCustomerLocation) ? (object)DBNull.Value : ArgsCustomerLocation);
            HistorySearch.Value = (String.IsNullOrEmpty(ArgsHistorySearch) ? (object)DBNull.Value : ArgsHistorySearch);
            HistorySearchLogic.Value = (String.IsNullOrEmpty(ArgsHistorySearchLogic) ? (object)DBNull.Value : ArgsHistorySearchLogic);

            Rank.Value = ((ArgsRank == null) ? (object)DBNull.Value : ArgsRank);
            RankCompare.Value = ArgsRankComparator;
            Size.Value = ((ArgsSize == null) ? (object)DBNull.Value : ArgsSize);
            SizeCompare.Value = ArgsSizeComparator;

            Keywords.Value = (String.IsNullOrEmpty(ArgsKeywords) ? (object)DBNull.Value : ArgsKeywords);

            int value = -1;
            using (
                SqlConnection connection = new SqlConnection(_connectionString)
                )
            {
                connection.Open();
                SqlCommand command = new SqlCommand(Command, connection);
                command.Parameters.Add(ReportID);
                command.Parameters.Add(PublicReport);
                command.Parameters.Add(Name);
                command.Parameters.Add(REACompare);
                command.Parameters.Add(REANum1);
                command.Parameters.Add(REANum2);
                command.Parameters.Add(Status);
                command.Parameters.Add(Product);
                command.Parameters.Add(DocChange);
                command.Parameters.Add(DbChange);
                command.Parameters.Add(ReqDoc);
                command.Parameters.Add(WhereClause);
                command.Parameters.Add(MaxRows);
                command.Parameters.Add(IssueType);
                command.Parameters.Add(CustomerBug);
                command.Parameters.Add(CustomerLogic);
                command.Parameters.Add(CustomerName);
                command.Parameters.Add(TitleLogic);
                command.Parameters.Add(SearchTitle);
                command.Parameters.Add(Priorities);

                command.Parameters.Add(SubBy);
                command.Parameters.Add(SubMonth);
                command.Parameters.Add(SubDay);
                command.Parameters.Add(Sub_Year);
                command.Parameters.Add(SubMonth2);
                command.Parameters.Add(SubDay2);
                command.Parameters.Add(SubYear2);
                //Date
                command.Parameters.Add(AssignedTo);
                command.Parameters.Add(AsnOnMonth);
                command.Parameters.Add(AsnOnDay);
                command.Parameters.Add(AsnOnYear);
                command.Parameters.Add(AsnOnMonth2);
                command.Parameters.Add(AsnOnDay2);
                command.Parameters.Add(AsnOnYear2);

                command.Parameters.Add(ResolvedBy);
                command.Parameters.Add(ResOnMonth);
                command.Parameters.Add(ResOnYear);
                command.Parameters.Add(ResOnDay);
                command.Parameters.Add(ResOnMonth2);
                command.Parameters.Add(ResOnDay2);
                command.Parameters.Add(ResOnYear2);

                command.Parameters.Add(ClosedBy);
                command.Parameters.Add(CloOnMonth);
                command.Parameters.Add(CloOnDay);
                command.Parameters.Add(CloOnYear);
                command.Parameters.Add(CloOnMonth2);
                command.Parameters.Add(CloOnDay2);
                command.Parameters.Add(CloOnYear2);

                command.Parameters.Add(CloOnOperator1);
                command.Parameters.Add(ResOnOperator1);
                command.Parameters.Add(AsnOnOperator1);
                command.Parameters.Add(SubOperator1);

                command.Parameters.Add(CloOnOperator2);
                command.Parameters.Add(ResOnOperator2);
                command.Parameters.Add(AsnOnOperator2);
                command.Parameters.Add(SubOperator2);

                command.Parameters.Add(CloOnField1);
                command.Parameters.Add(ResOnField1);
                command.Parameters.Add(AsnOnField1);
                command.Parameters.Add(SubField1);

                command.Parameters.Add(CloOnField2);
                command.Parameters.Add(ResOnField2);
                command.Parameters.Add(AsnOnField2);
                command.Parameters.Add(SubField2);

                command.Parameters.Add(ModStartMonth);
                command.Parameters.Add(ModStartDay);
                command.Parameters.Add(ModStartYear);
                command.Parameters.Add(ModEndMonth);
                command.Parameters.Add(ModEndYear);
                command.Parameters.Add(ModEndDay);
                command.Parameters.Add(ModStartOperator);
                command.Parameters.Add(ModEndOperator);
                command.Parameters.Add(ModEndField);
                command.Parameters.Add(ModStartField);

                //Version             
                command.Parameters.Add(VFMajor);
                command.Parameters.Add(VFMinor);
                command.Parameters.Add(VFBuild);
                command.Parameters.Add(VFMisc);

                command.Parameters.Add(VRMajor);
                command.Parameters.Add(VRMinor);
                command.Parameters.Add(VRBuild);
                command.Parameters.Add(VRMisc);

                command.Parameters.Add(VCMajor);
                command.Parameters.Add(VCMinor);
                command.Parameters.Add(VCBuild);
                command.Parameters.Add(VCMisc);

                command.Parameters.Add(VPMajor);
                command.Parameters.Add(VPMinor);
                command.Parameters.Add(VPBuild);
                command.Parameters.Add(VPMisc);

                command.Parameters.Add(VRMajorOperator);
                command.Parameters.Add(VRMinorOperator);
                command.Parameters.Add(VRBuildOperator);
                command.Parameters.Add(VRField);

                command.Parameters.Add(VCMajorOperator);
                command.Parameters.Add(VCMinorOperator);
                command.Parameters.Add(VCBuildOperator);
                command.Parameters.Add(VCField);

                command.Parameters.Add(VFMajorOperator);
                command.Parameters.Add(VFMinorOperator);
                command.Parameters.Add(VFBuildOperator);
                command.Parameters.Add(VFField);

                command.Parameters.Add(VPMajorOperator);
                command.Parameters.Add(VPMinorOperator);
                command.Parameters.Add(VPBuildOperator);
                command.Parameters.Add(VPField);

                command.Parameters.Add(SelectStatement);
                command.Parameters.Add(SortByStatement);

                //Customer Info
                command.Parameters.Add(MachineSN);
                command.Parameters.Add(SyteLine);
                command.Parameters.Add(CustomerCompany);
                command.Parameters.Add(CustomerLocation);
                command.Parameters.Add(HistorySearch);
                command.Parameters.Add(HistorySearchLogic);

                command.Parameters.Add(Rank);
                command.Parameters.Add(RankCompare);
                command.Parameters.Add(Size);
                command.Parameters.Add(SizeCompare);

                command.Parameters.Add(Keywords);

                value = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public DataTable GetAllReports(bool isAdmin)
        {
            String Command =
                "SELECT REPORT_ID, OWNER, (ST_USERS.FIRST_NAME+' '+ST_USERS.LAST_NAME) AS OWNER_NAME, PUBLIC_REPORT, NAME FROM REA_TABULAR_REPORT INNER JOIN ST_USERS ON REA_TABULAR_REPORT.OWNER = ST_USERS.USER_ID ";
            if (!isAdmin)
            {
                Command += " WHERE PUBLIC_REPORT = 1;";
            }
            return this.ProcessCommand(Command);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public DataTable GetAllUsersReports(int userid)
        {
            String Command =
               "SELECT REPORT_ID, OWNER, (ST_USERS.FIRST_NAME+' '+ST_USERS.LAST_NAME) AS OWNER_NAME, PUBLIC_REPORT, NAME FROM REA_TABULAR_REPORT INNER JOIN ST_USERS ON REA_TABULAR_REPORT.OWNER = ST_USERS.USER_ID ";
            Command += " WHERE OWNER = " + userid;
            Command += " ORDER BY NAME ASC ";
            return this.ProcessCommand(Command);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public DataTable GetOtherUsersReports(bool isAdmin, int userid)
        {
            String Command =
                "SELECT REPORT_ID, OWNER, (ST_USERS.FIRST_NAME+' '+ST_USERS.LAST_NAME) AS OWNER_NAME, PUBLIC_REPORT, NAME FROM REA_TABULAR_REPORT INNER JOIN ST_USERS ON REA_TABULAR_REPORT.OWNER = ST_USERS.USER_ID ";
            Command += " WHERE OWNER <> " + userid;
            if (!isAdmin)
            {
                Command += " AND PUBLIC_REPORT = 1;";
            }

            return this.ProcessCommand(Command);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="StUserID"></param>
        /// <returns></returns>
        public String getFullName(int StUserID)
        {
            String FullName = "";
            String cmdText = "SELECT ST_USERS.FIRST_NAME, ST_USERS.LAST_NAME FROM ST_USERS WHERE USER_ID = " + Convert.ToString(StUserID) + ";";
            using (System.Data.DataTable dt = this.ProcessCommand(cmdText))
            {
                if (dt.Rows.Count > 0)
                {
                    FullName = Convert.ToString(dt.Rows[0][0]) + " " + Convert.ToString(dt.Rows[0][1]);
                }
            }
            return FullName;
        }
        public DataTable QuickSearchResult(String Term, int ProductID)
        {

            String Where = "";
            String SQLWord = Term.Replace("'", "''");

            if (SQLWord.Contains("&&"))
            {
                int indexOfOperation = SQLWord.IndexOf("&&");
                string[] wordsSplit = SQLWord.Split(new string[] { "&&" }, StringSplitOptions.None);
                foreach (string word in wordsSplit)
                {
                    if (Where.Count() > 0)
                    {
                        Where += " AND ";
                    }
                    Where += "((REA_HISTORY.DESCRIPTION LIKE '%"
                        + word + "%') OR (REA_TRACK.TITLE LIKE '%"
                        + word + "%' ) OR (REA_TRACK.CUSTOMER_NAME LIKE '%"
                        + word + "%') )";
                    if (word.Contains("~"))
                    {
                        Where += " AND ((REA_HISTORY.DESCRIPTION NOT LIKE '%" + word + "%') AND (REA_TRACK.TITLE NOT LIKE '%" + word + "%') AND (REA_TRACK.CUSTOMER_NAME NOT LIKE '%" + word + "%'))";
                    }
                    int REAtemp = -1;
                    if (Int32.TryParse(word, out REAtemp))
                    {
                        Where += " OR REA_TRACK.TRACKING_ID = " + REAtemp;
                    }
                }
            }
            else if (SQLWord.Contains("||"))
            {
                int indexOfOperation = SQLWord.IndexOf("||");
                string[] wordsSplit = SQLWord.Split(new string[] { "||" }, StringSplitOptions.None);
                int counter = 0;
                foreach (string word in wordsSplit)
                {
                    if (counter > 0)
                    {
                        Where += " OR ";
                    }
                    Where += "((REA_HISTORY.DESCRIPTION LIKE '%" + word + "%') OR (REA_TRACK.TITLE LIKE '%" + word + "%' )  OR (REA_TRACK.CUSTOMER_NAME LIKE '%" + word + "%'))";
                    if (word.Contains("~"))
                    {
                        Where += " AND ((REA_HISTORY.DESCRIPTION NOT LIKE '%" + word + "%') AND (REA_TRACK.TITLE NOT LIKE '%" + word + "%') AND (REA_TRACK.CUSTOMER_NAME NOT LIKE '%" + word + "%'))";
                    }
                    int REAtemp = -1;
                    if (Int32.TryParse(word, out REAtemp))
                    {
                        Where += " OR REA_TRACK.TRACKING_ID = " + REAtemp;
                    }
                }
                counter++;
            }
            else
            {
                if (Where.Count() > 0)
                {
                    Where += " OR ";
                }
                Where += "((REA_HISTORY.DESCRIPTION LIKE '%" + SQLWord + "%') OR (REA_TRACK.TITLE LIKE '%" + SQLWord + "%' )  OR (REA_TRACK.CUSTOMER_NAME LIKE '%" + SQLWord + "%'))";
                if (Term.Contains("~"))
                {
                    Where += " AND ((REA_HISTORY.DESCRIPTION NOT LIKE '%" + SQLWord + "%') AND (REA_TRACK.TITLE NOT LIKE '%" + SQLWord + "%')AND (REA_TRACK.CUSTOMER_NAME NOT LIKE '%" + SQLWord + "%'))";
                }
                int REAtemp = -1;
                if (Int32.TryParse(SQLWord, out REAtemp))
                {
                    Where += " OR REA_TRACK.TRACKING_ID = " + REAtemp;
                }
            }
            if (ProductID > 0)
            {
                if (Where.Count() > 0)
                {
                    Where = "(" + Where + ")" + " AND ";
                }
                Where += " PRODUCT = " + ProductID;
            }
            if (Where.Count() != 0)
            {
                Where = (" WHERE " + Where);
            }
            //Create Command
            String Command;
            if (Term.Length > 0)
            {
                Command = "SELECT DISTINCT REA_PRODUCT.NAME AS PRODUCT, REA_TRACK.SYTELINE_REF AS SYTELINE_ID, REA_TRACK.TRACKING_ID AS TRACKING_ID, REA_TRACK.TITLE AS TITLE," +
                " REA_PRIORITY.NAME AS priority, REA_STATUS.NAME AS STATUS, " +
                " (USER1.FIRST_NAME + ' ' + USER1.LAST_NAME) AS SUBMITTED_BY_NAME, " +
                " REA_TRACK.SUBMITTED_BY AS SUBMITTED_BY_ID, " +
                " (USER2.FIRST_NAME + ' ' + USER2.LAST_NAME) AS ASSIGNED_TO_NAME, " +
                " REA_TRACK.ASSIGNED_TO AS ASSIGNED_TO_ID, " +
                " REA_TRACK.PRIORITY AS 'PRIORITY_ID', REA_TRACK.ISSUE_TYPE AS 'ISSUE_ID', REA_TRACK.STATUS AS 'STATUS_ID' " +
                " FROM REA_TRACK " +
                " INNER JOIN REA_HISTORY ON REA_TRACK.TRACKING_ID = REA_HISTORY.TRACKING_ID " +
                "INNER JOIN REA_PRODUCT ON REA_TRACK.PRODUCT = REA_PRODUCT.PRODUCT_ID " +
                "INNER JOIN REA_PRIORITY ON REA_TRACK.priority = REA_PRIORITY.ID " +
                "INNER JOIN REA_STATUS ON REA_TRACK.STATUS = REA_STATUS.STATUS_ID " +
                "INNER JOIN ST_USERS USER1 ON REA_TRACK.SUBMITTED_BY = USER1.USER_ID " +
                "LEFT OUTER JOIN ST_USERS USER2 ON REA_TRACK.ASSIGNED_TO = USER2.USER_ID " +
                Where +
                " ORDER BY TRACKING_ID ASC;";
            }
            else
            {
                //It really should never reach this, please prevent anyone from reaching here. 
                //This is here to prevent Errors from occuring
                //              - The Theriad
                Command =
                " SELECT DISTINCT REA_PRODUCT.NAME AS PRODUCT, REA_TRACK.SYTELINE_REF AS SYTELINE_ID, REA_TRACK.TRACKING_ID AS TRACKING_ID, REA_TRACK.TITLE AS TITLE," +
                " ST_ISSUE_TYPE.NAME AS ISSUE, REA_PRIORITY.NAME AS priority, REA_STATUS.NAME AS STATUS, " +
                " (USER1.FIRST_NAME + ' ' + USER1.LAST_NAME) AS SUBMITTED_BY_NAME, " +
                " REA_TRACK.SUBMITTED_BY AS SUBMITTED_BY_ID, " +
                " (USER2.FIRST_NAME + ' ' + USER2.LAST_NAME) AS ASSIGNED_TO_NAME, " +
                " REA_TRACK.ASSIGNED_TO AS ASSIGNED_TO_ID " +
                "FROM REA_TRACK WHERE TRACKING_ID IS NULL;";
            }
            return this.ProcessCommand(Command);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArgsTrackingID"></param>
        /// <param name="chngtype"></param>
        /// <param name="ArgsMadeBy"></param>
        /// <param name="ArgsMadeOn"></param>
        /// <param name="Description"></param>
        /// <param name="ArgsfileID"></param>
        public void CreateHistory(int ArgsTrackingID, int chngtype, int ArgsMadeBy, DateTime ArgsMadeOn, SqlParameter Description, int? ArgsfileID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //Create Sql parameters
                if (Description.Value == null)
                {
                    Description.Value = DBNull.Value;
                }
                SqlParameter TrackingId = new SqlParameter();
                SqlParameter Type = new SqlParameter();
                SqlParameter MadeBy = new SqlParameter();
                SqlParameter MadeOn = new SqlParameter();
                SqlParameter FileID = new SqlParameter();
                SqlParameter des = new SqlParameter(Description.ParameterName, Description.DbType);
                des.Value = Description.Value;

                //Setting up Types
                TrackingId.SqlDbType = SqlDbType.Int;
                Type.SqlDbType = SqlDbType.VarChar;
                MadeBy.SqlDbType = SqlDbType.Int;
                MadeOn.SqlDbType = SqlDbType.DateTime;
                FileID.SqlDbType = SqlDbType.Int;

                //setting paramter name
                TrackingId.ParameterName = "TRACKING_ID";
                Type.ParameterName = "TYPE";
                MadeBy.ParameterName = "MADE_BY";
                MadeOn.ParameterName = "MADE_ON";
                FileID.ParameterName = "FILE_ID";


                //setting up values
                TrackingId.Value = ArgsTrackingID;
                Type.Value = chngtype;
                MadeBy.Value = ArgsMadeBy;
                MadeOn.Value = ArgsMadeOn;
                FileID.Value = ArgsfileID == null ? (object)DBNull.Value : Convert.ToInt32(ArgsfileID);

                String cmdtext = "INSERT INTO REA_HISTORY (ROW_VER,TRACKING_ID,TYPE,MADE_BY,MADE_ON,DESCRIPTION,FILE_ID) " +
                    "VALUES (1,@TRACKING_ID, @TYPE, @MADE_BY, @MADE_ON, @DESCRIPTION , @FILE_ID);";
                SqlCommand sqlCommander = new SqlCommand(cmdtext, connection);
                sqlCommander.CommandText = cmdtext;
                sqlCommander.Parameters.Add(TrackingId);
                sqlCommander.Parameters.Add(Type);
                sqlCommander.Parameters.Add(MadeBy);
                sqlCommander.Parameters.Add(MadeOn);
                sqlCommander.Parameters.Add(FileID);
                sqlCommander.Parameters.Add(des);
                connection.Open();
                sqlCommander.ExecuteNonQuery();
                connection.Close();
            }
        }

        public SqlCommand CreateSqlCommand(string command = "")
        {
            ///<summary>
            /// Creates a SQL Command form a string
            ///</summary>
            SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            return new SqlCommand(command, conn);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="argsPRODUCT_ID"></param>
        /// <param name="argsMAJOR"></param>
        /// <param name="argsMINOR"></param>
        /// <param name="argsBUILD"></param>
        /// <param name="argsMisc"></param>
        /// <param name="argsIS_CUSTOMER_RELEASE"></param>
        /// <param name="argsRELEASE_COORDINATOR_ID"></param>
        /// <param name="argsQA"></param>
        /// <param name="argsNOTES"></param>
        /// <param name="argsREA_LIST"></param>
        /// <param name="argsRelatedBuilds"></param>
        /// <param name="argsDisplaySubBuild"></param>
        /// <returns></returns>
        public DataTable SaveProductRelease(int argsPRODUCT_ID,
                                                int argsMAJOR, int argsMINOR, int argsBUILD, string argsMisc,
                                                int argsIS_CUSTOMER_RELEASE, int argsRELEASE_COORDINATOR_ID, int argsQA, string argsNOTES, string argsREA_LIST, string argsDBVersion, List<int> argsRelatedBuilds,
                                                bool argsDisplaySubBuild
                                                )
        {
            DateTime NOW = DateTime.Now; //this is so that all of the entries have the same timestamp

            //Set all the REAs to Built and Set to Q.A
            String[] REAs = argsREA_LIST.Split(',');
            foreach (String rea in REAs)
            {
                //string ChangeAssignement = "EXEC AssignScr ";
                //this.ProcessCommand(ChangeAssignement);
                string ChangeStatus =
                    "UPDATE REA_TRACK SET STATUS = 11, ASSIGNED_TO = " + Convert.ToString(argsQA) +
                        ", VR_MAJOR ='" + Convert.ToString(argsMAJOR) + "', VR_MINOR ='" + Convert.ToString(argsMINOR) + "', VR_BUILD ='" + Convert.ToString(argsBUILD) + "' ";
                if (
                    !String.IsNullOrEmpty(argsMisc) &&
                    !String.IsNullOrWhiteSpace(argsMisc)
                  )
                {
                    ChangeStatus += " ,VR_MISC ='" + argsMisc.Trim() + "'";
                }
                ChangeStatus += " WHERE REA_TRACK.TRACKING_ID = " + rea + ";";
                this.ProcessCommand(ChangeStatus);
                // Make an entry in the history for built
                String HistoryDescriptionText = "'Built in release " + Convert.ToString(argsMAJOR) + "." + Convert.ToString(argsMINOR) + "." + Convert.ToString(argsBUILD) + " ";
                if (!String.IsNullOrEmpty(argsMisc) &&
                    !String.IsNullOrWhiteSpace(argsMisc)
                    )
                {
                    HistoryDescriptionText += " " + argsMisc.Trim();
                }
                HistoryDescriptionText += "'";
                String insertIntoHistory = "INSERT INTO REA_HISTORY (TRACKING_ID,ROW_VER,TYPE,MADE_BY,MADE_ON,DESCRIPTION) " +
                                        "VALUES(" + rea + ", 1, 2, -1, '" + NOW.ToString() + "', " + HistoryDescriptionText + ");";
                this.ProcessCommand(insertIntoHistory);
                //2 is app.chg_note denotes that it is a change of type notes
                // Make an entry in the history for assignment
                String AssignementText = "'Assigned to " + this.getFullName(argsQA) + " by Release Coordinator " + this.getFullName(argsRELEASE_COORDINATOR_ID) +
                    " --- Actual Release Version: " + Convert.ToString(argsMAJOR) + "." + Convert.ToString(argsMINOR) + "." + Convert.ToString(argsBUILD) + " ";
                if (
                     !String.IsNullOrEmpty(argsMisc) &&
                     !String.IsNullOrWhiteSpace(argsMisc)
                    )
                {
                    AssignementText += " " + argsMisc.Trim();
                }
                AssignementText += "'";
                String addAssignment = "INSERT INTO REA_HISTORY (TRACKING_ID,ROW_VER,TYPE,MADE_BY,MADE_ON,DESCRIPTION)" +
                                        "VALUES(" + rea + ", 1 , 5, -1, '" + NOW.ToString() + "', " + AssignementText + ");";
                //5 is app.chg_assignto denotes that it is a change of type assignement
                this.ProcessCommand(addAssignment);
            }
            //Create the release
            //SqlTransaction transaction = connection.BeginTransaction("InsertRelease"+HttpContext.Current.Session["st_userID"]+ DateTime.Now.ToShortTimeString());

            //Create parameters
            SqlParameter PRODUCT_ID = new SqlParameter("@PRODUCT_ID", SqlDbType.Int);
            SqlParameter MAJOR = new SqlParameter("@MAJOR", SqlDbType.VarChar);
            SqlParameter MINOR = new SqlParameter("@MINOR", SqlDbType.VarChar);
            SqlParameter BUILD = new SqlParameter("@BUILD", SqlDbType.VarChar);
            SqlParameter MISC = new SqlParameter("@MISC", SqlDbType.VarChar);
            SqlParameter IS_CUSTOMER_RELEASE = new SqlParameter("@IS_CUSTOMER_RELEASE", SqlDbType.Int);
            SqlParameter RELEASE_COORDINATOR_ID = new SqlParameter("@RELEASE_COORDINATOR_ID", SqlDbType.Int);
            SqlParameter NOTES = new SqlParameter("@NOTES", SqlDbType.NText);
            SqlParameter REA_LIST = new SqlParameter("@REA_LIST", SqlDbType.VarChar, 1000);
            SqlParameter BUILT_ON = new SqlParameter("@BUILT_ON", SqlDbType.DateTime);
            SqlParameter DISPLAY_RELATED_REPORT = new SqlParameter("@DISPLAY_RELATED_REPORT", SqlDbType.Bit);
            SqlParameter DB_VERSION = new SqlParameter("@DB_VERSION", SqlDbType.VarChar, 10);

            //set value
            PRODUCT_ID.Value = argsPRODUCT_ID;
            MAJOR.Value = argsMAJOR;
            MINOR.Value = argsMINOR;
            BUILD.Value = argsBUILD;
            MISC.Value = argsMisc == null ? (object)DBNull.Value : argsMisc.Trim();
            IS_CUSTOMER_RELEASE.Value = argsIS_CUSTOMER_RELEASE;
            RELEASE_COORDINATOR_ID.Value = argsRELEASE_COORDINATOR_ID;
            NOTES.Value = String.IsNullOrEmpty(argsNOTES) ? "" : argsNOTES.Trim();
            REA_LIST.Value = argsREA_LIST.Trim();
            BUILT_ON.Value = NOW;
            DB_VERSION.Value = String.IsNullOrEmpty(argsDBVersion) ? (object)DBNull.Value : argsDBVersion.Trim();
            DISPLAY_RELATED_REPORT.Value = argsDisplaySubBuild;
            //add each parameter
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                //String commandSQL = "EXEC BuildRelease_InsertRelease @PRODUCT_ID, @MAJOR, @MINOR, @BUILD, @MISC, @BUILT_ON, @IS_CUSTOMER_RELEASE, @RELEASE_COORDINATOR_ID, @NOTES, @LIST_OF_REA, @DISPLAY_RELATED_REPORT";
                string commandSQL = $@"
                    INSERT INTO [REA_PRODUCT_RELEASE] 
		                    (PRODUCT_ID, MAJOR, MINOR, BUILD, MISC, BUILT_ON, IS_CUSTOMER_RELEASE, RELEASE_COORDINATOR_ID, NOTES, REA_LIST, DISPLAY_RELATED_REPORT, DB_VERSION)
                    VALUES
                            (@PRODUCT_ID, @MAJOR, @MINOR, @BUILD, @MISC, @BUILT_ON, @IS_CUSTOMER_RELEASE, @RELEASE_COORDINATOR_ID, @NOTES, @REA_LIST, @DISPLAY_RELATED_REPORT, @DB_VERSION)
                    SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(commandSQL, connection);
                command.Parameters.Add(PRODUCT_ID);
                command.Parameters.Add(MAJOR);
                command.Parameters.Add(MINOR);
                command.Parameters.Add(BUILD);
                command.Parameters.Add(MISC);
                command.Parameters.Add(BUILT_ON);
                command.Parameters.Add(IS_CUSTOMER_RELEASE);
                command.Parameters.Add(RELEASE_COORDINATOR_ID);
                command.Parameters.Add(NOTES);
                command.Parameters.Add(REA_LIST);
                command.Parameters.Add(DISPLAY_RELATED_REPORT);
                command.Parameters.Add(DB_VERSION);
                //command.Transaction = transaction;

                //Attach to Command
                DataTable dt = null;
                using (SqlDataAdapter reader = new SqlDataAdapter(command))
                {
                    dt = new DataTable();
                    reader.Fill(dt);
                }
                //update the relationship table
                if (dt.Rows.Count > 0)
                {
                    foreach (int item in argsRelatedBuilds)
                    {
                        this.ProductRelationShip(Convert.ToInt32(dt.Rows[0][0]), item);
                    }
                }
                connection.Close();
                return dt;
            }
        }

        public bool UpdateProductRelease(int buildID, int customerReleaseID, string listOfREAs, string notes, bool display_related, string url, string DBVersion)
        {
            bool success = false;

            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    //Create Sql parameters
                    SqlParameter sqlBuildID = new SqlParameter("@BUILD_ID", SqlDbType.Int);
                    SqlParameter sqlIsCustomerRelease = new SqlParameter("@IS_CUSTOMER_RELEASE", SqlDbType.Int);
                    SqlParameter sqlREAList = new SqlParameter("@REA_LIST", SqlDbType.VarChar, 1000);
                    SqlParameter sqlNotes = new SqlParameter("@NOTES", SqlDbType.NText);
                    SqlParameter sqlDisplayRelated = new SqlParameter("@DISPLAY_RELATED_REPORT", SqlDbType.Bit);
                    SqlParameter sqlURL = new SqlParameter("@URL", SqlDbType.NVarChar, 300);
                    SqlParameter sqlDBVersion = new SqlParameter("@DB_VERSION", SqlDbType.VarChar, 10);

                    //setting up values
                    sqlBuildID.Value = buildID;
                    sqlIsCustomerRelease.Value = customerReleaseID;
                    sqlREAList.Value = listOfREAs.Trim();
                    sqlNotes.Value = string.IsNullOrWhiteSpace(notes) ? (object)DBNull.Value : notes.Trim();
                    sqlDisplayRelated.Value = display_related;
                    sqlURL.Value = string.IsNullOrWhiteSpace(url) ? (object)DBNull.Value : url.Trim();
                    sqlDBVersion.Value = string.IsNullOrWhiteSpace(DBVersion) ? (object)DBNull.Value : DBVersion.Trim();

                    String cmdtext = $@"UPDATE REA_PRODUCT_RELEASE SET 
                                            IS_CUSTOMER_RELEASE=@IS_CUSTOMER_RELEASE,
                                            REA_LIST=@REA_LIST,
                                            NOTES=@NOTES,
                                            DISPLAY_RELATED_REPORT=@DISPLAY_RELATED_REPORT,
                                            URL=@URL,
                                            DB_VERSION=@DB_VERSION
                                        WHERE 
                                            BUILD_ID=@BUILD_ID";
                    SqlCommand sqlCommander = new SqlCommand(cmdtext, connection);
                    sqlCommander.CommandText = cmdtext;
                    sqlCommander.Parameters.Add(sqlBuildID);
                    sqlCommander.Parameters.Add(sqlIsCustomerRelease);
                    sqlCommander.Parameters.Add(sqlREAList);
                    sqlCommander.Parameters.Add(sqlNotes);
                    sqlCommander.Parameters.Add(sqlDisplayRelated);
                    sqlCommander.Parameters.Add(sqlURL);
                    sqlCommander.Parameters.Add(sqlDBVersion);

                    connection.Open();
                    int num_rows = sqlCommander.ExecuteNonQuery();

                    success = (num_rows == 1);
                }
            }
            catch (Exception ex)
            {
                Utility.Logging.WriteEntry(ex);
                success = false;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
            return success;
        }
        //=====================================Search Get Displays===================================
        /// <summary>
        /// Gets all the Issue Types
        /// </summary>
        /// <returns></returns>
        public DataTable GetIssueTypes()
        {
            String Command = "SELECT ISSUE_ID, NAME FROM ST_ISSUE_TYPE;";
            return this.ProcessCommand(Command);
        }

        /// <summary>
        /// Gets all the statuses
        /// </summary>
        /// <returns></returns>
        public DataTable GetStatuses()
        {
            String Command = "SELECT STATUS_ID, NAME FROM REA_STATUS ORDER BY STATUS_ORDER;";
            return this.ProcessCommand(Command);
        }

        /// <summary>
        /// Gets all the prioirities
        /// </summary>
        /// <returns></returns>
        public DataTable GetPriorities()
        {
            String Command = "SELECT PRIORITY_ID, NAME FROM REA_priority";
            return this.ProcessCommand(Command);
        }
        //===============================================================================================
        /// <summary>
        /// Returns if the user is a manager or an admin
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public Boolean isAdminOrManager(int UserID)
        {
            String Command = "select IS_MANAGER, ACCOUNT_TYPE from ST_USERS where USER_ID =" + UserID;
            DataTable results = this.ProcessCommand(Command);
            return (Convert.ToBoolean(results.Rows[0][0]) || (Convert.ToInt32(results.Rows[0][1]) == 0));
        }

        /// <summary>
        /// updates the history section after deleting an attachment
        /// </summary>
        /// <param name="ArgsTrackingID"></param>
        /// <param name="ArgsfileID"></param>
        /// <param name="User_Id"></param>
        public void UpdateDeletedFileHistorys(int ArgsTrackingID, int ArgsfileID, int User_Id)
        {
            String Filename = "";

            //Update old History
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var Rec = ProcessScalarCommand("SELECT FILENAME FROM ST_ATTACHMENT WHERE FILE_ID = " + Convert.ToString(ArgsfileID));
                Filename = System.IO.Path.GetFileName(Convert.ToString(Rec == null ? "" : Rec));
                //Create Sql parameters
                SqlParameter TrackingId = new SqlParameter();
                SqlParameter Description = new SqlParameter();
                SqlParameter FileID = new SqlParameter();

                ////Setting up Types
                TrackingId.SqlDbType = SqlDbType.Int;
                Description.SqlDbType = SqlDbType.VarChar;
                FileID.SqlDbType = SqlDbType.Int;

                ////setting paramter name
                TrackingId.ParameterName = "TRACKING_ID";
                Description.ParameterName = "DESCRIPTION";
                FileID.ParameterName = "FILE_ID";

                ////setting up values
                TrackingId.Value = ArgsTrackingID;
                Description.Value = "File " + System.IO.Path.GetFileName(Filename) + " was attached.";
                FileID.Value = ArgsfileID;

                String cmdtext = "UPDATE REA_HISTORY SET FILE_ID = NULL, DESCRIPTION = @DESCRIPTION where TRACKING_ID = @TRACKING_ID and FILE_ID = @FILE_ID;";
                SqlCommand sqlCommander = new SqlCommand(cmdtext, connection);
                sqlCommander.CommandText = cmdtext;
                sqlCommander.Parameters.Add(TrackingId);
                sqlCommander.Parameters.Add(FileID);
                sqlCommander.Parameters.Add(Description);
                connection.Open();
                sqlCommander.ExecuteNonQuery();
                connection.Close();
            }
            // Create a New History   
            SqlParameter Description2 = new SqlParameter();
            Description2.SqlDbType = SqlDbType.VarChar;
            Description2.ParameterName = "DESCRIPTION";

            Description2.Value = "File " + System.IO.Path.GetFileName(Filename) + " was deleted.";
            CreateHistory(ArgsTrackingID, (int)HistoryChangeType.chg_attachment_add, User_Id, DateTime.Now, Description2, null);


        }
        //===================================== Favorite ======================
        /// <summary>
        /// update the favourite in the dashboard page
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="productID"></param>
        /// <param name="value"></param>
        public void UpdateFavorite(int userID, int productID, bool value)
        {
            DataTable AccessList = this.ProcessCommand("SELECT * FROM REA_PRODUCT_ACCESS WHERE UserID = " + userID + " AND ProductID = " + productID);
            string command = "";
            string valueString = "";
            if (value)
            {
                valueString = "1";
            }
            else
            {
                valueString = "0";
            }
            if (AccessList.Rows.Count > 0)
            {
                //Update the row
                command = "UPDATE REA_PRODUCT_ACCESS SET Favorite=" + valueString + " WHERE UserID = " + userID + " AND ProductID = " + productID;
            }
            else
            {
                //Create the instance
                command = "INSERT INTO REA_PRODUCT_ACCESS(UserID, ProductID, Favorite) VALUES(" + userID + "," + productID + "," + valueString + ")";
            }
            this.ProcessCommand(command);
        }
        public bool IsFavoriteProduct(int userID, int productID)
        {
            bool isFavorite = false;
            string command = "SELECT CASE WHEN SUM(CAST(Favorite AS INT)) > 0 THEN 1 ELSE 0 END AS 'Favorite' FROM REA_PRODUCT_ACCESS WHERE UserID = " + userID + " AND ProductID = " + productID;
            isFavorite = ((int)ProcessScalarCommand(command) == 1);
            return isFavorite;
        }
        /// <summary>
        /// removes product access for users
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool RemoveProductAccessForUser(int userID, int productID)
        {
            String CommandString = "DELETE FROM REA_PRODUCT_ACCESS WHERE (UserID = @ST_USER_ID) AND (ProductID = @PRODUCT_ID) ";
            SqlParameter UserID = new SqlParameter("ST_USER_ID", SqlDbType.Int);
            SqlParameter ProductID = new SqlParameter("PRODUCT_ID", SqlDbType.Int);
            UserID.Value = userID;
            ProductID.Value = productID;
            bool success = false;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(CommandString, connection);
                command.Parameters.Add(UserID);
                command.Parameters.Add(ProductID);
                try
                {
                    command.ExecuteNonQuery();
                    success = true;
                }
                catch
                {
                    success = false;
                }
                connection.Dispose();
            }
            return success;
        }

        /// <summary>
        /// Gets the latest REAs of a product that
        /// were submitted based off the number of days
        /// or default to 8 days
        /// </summary>
        /// <param name="product"></param>
        /// <param name="days"></param>
        /// default number of days that the parameter will be passed 
        /// <returns></returns>
        public DataTable GetLatestSubmittedByTime(int product, int days = 8)
        {
            System.Data.DataTable dt;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();


                using (SqlCommand command = new SqlCommand())
                {

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "rea_GetLatestSubmittedByTime";
                    SqlParameter parameter = new SqlParameter();
                    parameter.ParameterName = "@productID";
                    parameter.SqlDbType = SqlDbType.Int;
                    parameter.Direction = ParameterDirection.Input;
                    parameter.Value = product;
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter();
                    parameter.ParameterName = "@days";
                    parameter.SqlDbType = SqlDbType.Int;
                    parameter.Direction = ParameterDirection.Input;
                    parameter.Value = days;
                    command.Parameters.Add(parameter);

                    using (SqlDataAdapter reader = new SqlDataAdapter(command))
                    {
                        dt = new DataTable();
                        reader.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public DataTable getViewAllMetrics(int ProdID, int MetricID)
        {
            string children = " SELECT DISTINCT ID, REA_PRODUCT.PRODUCT_ID AS 'PROD_ID', NAME, MAJOR_VERSION, MINOR_VERSION, RELEASE_CRITICAL_COUNT AS 'Metric #1', RELEASE_HIGH_COUNT AS 'Metric #2',RELEASE_DEFECT_METRIC AS 'Metric #3', RELEASE_DEFECT_COUNT AS 'Metric #4' FROM REA_PRODUCT_RELATION FULL JOIN REA_PRODUCT ON REA_PRODUCT.PRODUCT_ID = CHILD_ID FULL JOIN REA_PRODUCT_METRICS ON REA_PRODUCT.PRODUCT_ID = REA_PRODUCT_METRICS.PRODUCT_ID "
                                 + " WHERE PARENT_ID = " + ProdID + " OR ID =" + MetricID + " ORDER BY MAJOR_VERSION ";
            return this.ProcessCommand(children);
        }


        /// <summary>
        ///  Gets the Latest REAs that are submitted by
        /// -pulls from a SQL function
        /// </summary>
        /// <param name="product"></param>
        /// <param name="count"></param>
        /// The last number of days that we are looking at
        ///   defaults to 10 REAs
        /// <returns></returns>
        public DataTable GetLatestSubmittedByCount(int product, int count = 10)
        {
            System.Data.DataTable dt;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();


                using (SqlCommand command = new SqlCommand())
                {

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "rea_GetLatestSubmittedByCount";
                    SqlParameter parameter = new SqlParameter();
                    parameter.ParameterName = "@productID";
                    parameter.SqlDbType = SqlDbType.Int;
                    parameter.Direction = ParameterDirection.Input;
                    parameter.Value = product;
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter();
                    parameter.ParameterName = "@count";
                    parameter.SqlDbType = SqlDbType.Int;
                    parameter.Direction = ParameterDirection.Input;
                    parameter.Value = count;
                    command.Parameters.Add(parameter);

                    using (SqlDataAdapter reader = new SqlDataAdapter(command))
                    {
                        dt = new DataTable();
                        reader.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public bool UpdateComponent(int releaseRelationId, int relatedBuildId)
        {
            bool success = false;
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    //Create Sql parameters
                    //SqlParameter sqlBuildID = new SqlParameter("@BUILD_ID", SqlDbType.Int);
                    SqlParameter sqlRelatedBuildID = new SqlParameter("@RELATED_BUILD_ID", SqlDbType.Int);
                    SqlParameter sqlReleaseRelationID = new SqlParameter("@RELEASE_RELATION_ID", SqlDbType.Int);

                    //setting up values
                    //sqlBuildID.Value = buildID;
                    sqlRelatedBuildID.Value = relatedBuildId;
                    sqlReleaseRelationID.Value = releaseRelationId;
                    String cmdtext = $@"UPDATE REA_PRODUCT_RELEASE_RELATION SET 
                                        RELATED_BUILD_ID=@RELATED_BUILD_ID
                                    WHERE 
                                        RELEASE_RELATION_ID=@RELEASE_RELATION_ID";
                    SqlCommand sqlCommander = new SqlCommand(cmdtext, connection);
                    sqlCommander.CommandText = cmdtext;
                    //sqlCommander.Parameters.Add(sqlBuildID);
                    sqlCommander.Parameters.Add(sqlRelatedBuildID);
                    sqlCommander.Parameters.Add(sqlReleaseRelationID);

                    connection.Open();
                    int num_rows = sqlCommander.ExecuteNonQuery();
                    success = (num_rows == 1);
                }
            }
            catch (Exception ex)
            {
                Utility.Logging.WriteEntry(ex);
                success = false;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
            return success;
        }

        #region Build Verification Report
        public DataTable GetBuildRecord(int buildID)
        {
            string sql = @"SELECT 
                            REA_PRODUCT_RELEASE.PRODUCT_ID,
                            REA_PRODUCT.NAME, 
                            REA_PRODUCT_RELEASE.RELEASE_COORDINATOR_ID, 
                            (ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME) AS RELEASE_COORDINATOR,
                            (ISNULL(REA_PRODUCT_RELEASE.MAJOR, '?') + '.' + ISNULL(REA_PRODUCT_RELEASE.MINOR, '?') + '.' + ISNULL(REA_PRODUCT_RELEASE.BUILD, '?') + ' ' + ISNULL(REA_PRODUCT_RELEASE.MISC, '')) AS BUILD_VERSION, 
                            REA_PRODUCT_RELEASE.BUILT_ON,
                            REA_PRODUCT_RELEASE.IS_CUSTOMER_RELEASE, 
                            REA_PRODUCT_RELEASE.NOTES, 
                            REA_PRODUCT_RELEASE.REA_LIST,
                            REA_PRODUCT_RELEASE.DB_VERSION,
                            REA_PRODUCT.DISTRIBUTION_LIST
                          FROM 
                            REA_PRODUCT_RELEASE
                          INNER JOIN REA_PRODUCT ON REA_PRODUCT_RELEASE.PRODUCT_ID = REA_PRODUCT.PRODUCT_ID
                          INNER JOIN ST_USERS ON ST_USERS.USER_ID = REA_PRODUCT_RELEASE.RELEASE_COORDINATOR_ID
                          WHERE 
                            REA_PRODUCT_RELEASE.BUILD_ID = " + buildID.ToString();

            DataTable dt = ProcessCommand(sql);

            return dt;
        }
        #endregion

        #region Watch List
        public DataTable GetUsersWatching(int? userID = null)
        {
            string sql = @"SELECT DISTINCT
                                REA_WATCH_LIST.USER_ID,
                                ST_USERS.EMAIL
                            FROM 
                                REA_WATCH_LIST 
                            INNER JOIN ST_USERS ON ST_USERS.USER_ID = REA_WATCH_LIST.USER_ID 
                            WHERE 
                                (ST_USERS.ACCOUNT_TYPE <> -1)";

            if (userID != null)
            {
                sql += " AND REA_WATCH_LIST.USER_ID = " + userID.ToString();
            }
            DataTable dt = ProcessCommand(sql);

            return dt;
        }

        public DataTable GetUserWatchList(int userID)
        {

            String sql =
            @"SELECT
                    REA_PRODUCT.NAME AS 'PRODUCT',
                    REA_WATCH_LIST.TRACKING_ID,
                    REA_TRACK.TITLE,
                 REA_HISTORY.TYPE, 
                 CASE REA_HISTORY.TYPE
                  WHEN 2 THEN 'Notes were added: ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR(30))
                  WHEN 4 THEN 'File was attached'
                  WHEN 8 THEN ISNULL('REA#' + (SELECT CAST(ST_RELATED.REA_ID AS NVARCHAR) FROM ST_RELATED WHERE ST_RELATED.RELATED_ID = REA_HISTORY.FILE_ID) + ' was related', 'REA removed.')
                  WHEN 10 THEN 'Planned Release Version changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
                  WHEN 11 THEN 'Actual Release Version changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
                  WHEN 12 THEN 'Planned Hours To Fix changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
                  WHEN 13 THEN 'Actual Hours To Fix changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
                  WHEN 14 THEN 'Planned Hours To Test changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
                  WHEN 15 THEN 'Actual Hours To Test changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
                  WHEN 20 THEN 'Planned Hours To Specify changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
                  WHEN 21 THEN 'Actual Hours To Specify changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
                  ELSE
                   REA_HISTORY.DESCRIPTION
                 END AS 'DESCRIPTION'
            FROM 
                REA_WATCH_LIST 
            INNER JOIN REA_TRACK ON REA_TRACK.TRACKING_ID = REA_WATCH_LIST.TRACKING_ID
            INNER JOIN REA_PRODUCT ON REA_TRACK.PRODUCT = REA_PRODUCT.PRODUCT_ID
            INNER JOIN REA_HISTORY ON REA_TRACK.TRACKING_ID = REA_HISTORY.TRACKING_ID
            WHERE 
                (
                    (REA_WATCH_LIST.USER_ID = " + userID.ToString() + @")
                AND 
                    (CONVERT(VARCHAR(10), REA_HISTORY.MADE_ON, 101) = CONVERT(VARCHAR(10), GETDATE() - 1, 101))
                )
            ORDER BY 
                REA_PRODUCT.NAME, REA_WATCH_LIST.TRACKING_ID ASC";
            //TODO : we may turn the flag for notifications back on

            DataTable dt = ProcessCommand(sql);

            return dt;
        }

        public DataTable GetUserWatchList(int userID, DateTime day)
        {
            String sql =
            @"SELECT
                    REA_PRODUCT.NAME AS 'PRODUCT',
                    REA_WATCH_LIST.TRACKING_ID,
                    REA_TRACK.TITLE,
	                REA_HISTORY.TYPE, 
	                CASE REA_HISTORY.TYPE
		                WHEN 2 THEN 'Notes were added: ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR(30))
		                WHEN 4 THEN 'File was attached'
                    	WHEN 8 THEN ISNULL('REA#' + (SELECT CAST(ST_RELATED.REA_ID AS NVARCHAR) FROM ST_RELATED WHERE ST_RELATED.RELATED_ID = REA_HISTORY.FILE_ID) + ' was related', 'REA removed.')
		                WHEN 10 THEN 'Planned Release Version changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		                WHEN 11 THEN 'Actual Release Version changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		                WHEN 12 THEN 'Planned Hours To Fix changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		                WHEN 13 THEN 'Actual Hours To Fix changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		                WHEN 14 THEN 'Planned Hours To Test changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		                WHEN 15 THEN 'Actual Hours To Test changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		                WHEN 20 THEN 'Planned Hours To Specify changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		                WHEN 21 THEN 'Actual Hours To Specify changed to ' + CAST(REA_HISTORY.DESCRIPTION AS NVARCHAR)
		                ELSE
			                REA_HISTORY.DESCRIPTION
	                END AS 'DESCRIPTION'

                            FROM 
                                REA_WATCH_LIST 
                            INNER JOIN REA_TRACK ON REA_TRACK.TRACKING_ID = REA_WATCH_LIST.TRACKING_ID
                            INNER JOIN REA_PRODUCT ON REA_TRACK.PRODUCT = REA_PRODUCT.PRODUCT_ID
                            INNER JOIN REA_HISTORY ON REA_TRACK.TRACKING_ID = REA_HISTORY.TRACKING_ID
                            WHERE 
                                (
                                    (REA_WATCH_LIST.USER_ID = " + userID.ToString() + @")
                                AND 
                                    (CONVERT(VARCHAR(10), REA_HISTORY.MADE_ON, 101) = CONVERT(VARCHAR(10), '" + day.ToString("MM/dd/yyyy") + @"', 101))
                                )
                            ORDER BY 
                                REA_PRODUCT.NAME, REA_WATCH_LIST.TRACKING_ID ASC";
            //TODO : we may turn the flag for notifications back on

            DataTable dt = ProcessCommand(sql);
            return dt;
        }

        public bool SetLanguage(int? UserId)
        {
            string languageCode;
            string cmdTxt = $@"select IsNull((Select LanguageCode from AspNetUsers where ST_USER_ID = '{UserId}'), 'none') As LanguageCode";

            languageCode = (string)ProcessScalarCommandSCR(cmdTxt);
            if (languageCode != "none")
            {
                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(languageCode);
                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = ci;
                System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = ci;
                System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
                System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            }
            return true;
        }

        #endregion

    }
}
