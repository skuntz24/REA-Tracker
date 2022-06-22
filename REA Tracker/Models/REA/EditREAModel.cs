using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using QVICommonIntranet.Database;
//using QVICommonIntranet.Email.EmailREATracker;

namespace REA_Tracker.Models
{
    public class EditREA
    {
        //Access
        public Boolean hasAccess { get; set; }
        public string AccountType { get; set; }
        public bool IsAdmin { get; set; }
        public int AccessType { get; set; }
        public int User_id { get; set; }
        //Fields
        public int TrackingId { get; set; }
        [Required]
        public string Title { get; set; }
        public int? productId { get; set; }
        public int? moduleId { get; set; }
        [Required(ErrorMessage = "Please enter in a major.")]
        [RegularExpression(@"(\d+)", ErrorMessage = "Please enter a postive number")]
        public int? versionFoundMajor { get; set; }
        [Required(ErrorMessage = "Please enter in a minor.")]
        [RegularExpression(@"(\d+)", ErrorMessage = "Please enter a postive number")]
        public string VersionFound { get; set; }

        public int? versionFoundMinor { get; set; }
        [RegularExpression(@"(\d+)", ErrorMessage = "Please enter a postive number")]
        public int? versionFoundBuild { get; set; }
        public string versionFoundMisc { get; set; }
        public int? issueType { get; set; }
        [Required]
        public int? priority { get; set; }
        public int? statusId { get; set; }
        [Required]
        public int? closedReasonId { get; set; }
        public int? isDocumentChange { get; set; }
        public bool? isDatabaseChange { get; set; }
        public bool? isRequirementsChange { get; set; }
        [RegularExpression(@"(\d+)?(\.\d+)?", ErrorMessage = "Please enter a postive number")]
        public double? plannedHourstoSpecify { get; set; }
        [RegularExpression(@"(\d+)?(\.\d+)?", ErrorMessage = "Please enter a postive number")]
        public double? actualHourstoSpecify { get; set; }
        [RegularExpression(@"(\d+)?(\.\d+)?", ErrorMessage = "Please enter a postive number")]
        public double? plannedHourstoFix { get; set; }
        [RegularExpression(@"(\d+)?(\.\d+)?", ErrorMessage = "Please enter a postive number")]
        public double? actualHourstoFix { get; set; }
        [RegularExpression(@"(\d+)?(\.\d+)?", ErrorMessage = "Please enter a postive number")]
        public double? plannedHourstoTest { get; set; }
        [RegularExpression(@"(\d+)?(\.\d+)?", ErrorMessage = "Please enter a postive number")]
        public double? actualHourstoTest { get; set; }
        [RegularExpression(@"(\d+)?", ErrorMessage = "Please enter a postive number")]
        public string VersionPlanned { get; set; }
        public int? versionPlannedMajor { get; set; }
        [RegularExpression(@"(\d+)?", ErrorMessage = "Please enter a postive number")]
        public int? versionPlannedMinor { get; set; }
        [RegularExpression(@"(\d+)?", ErrorMessage = "Please enter a postive number")]
        public int? versionPlannedBuild { get; set; }
        public string versionPlannedMisc { get; set; }
        [RegularExpression(@"(\d+)?", ErrorMessage = "Please enter a postive number")]
        public string VersionReleased { get; set; }
        public int? versionReleasedMajor { get; set; }
        [RegularExpression(@"(\d+)?", ErrorMessage = "Please enter a postive number")]
        public int? versionReleasedMinor { get; set; }
        [RegularExpression(@"(\d+)?", ErrorMessage = "Please enter a postive number")]
        public int? versionReleasedBuild { get; set; }
        public string versionReleasedMisc { get; set; }
        public int? submitedById { get; set; }
        public DateTime? submittedOn { get; set; }
        public DateTime? assignedOn { get; set; }
        public int? assignedToId { get; set; }
        public int? resolvedByID { get; set; }
        public int? closedByID { get; set; }
        public bool? customerBug { get; set; }
        [Required]
        public string customerName { get; set; }
        public int lastReleaseMajor { get; set; }
        public int lastReleaseMinor { get; set; }
        public int lastReleaseBuild { get; set; }
        [RegularExpression(@"(\d+)?", ErrorMessage = "Please enter a postive number")]
        public int? versionClosedMajor { get; set; }
        [RegularExpression(@"(\d+)?", ErrorMessage = "Please enter a postive number")]
        public int? versionClosedMinor { get; set; }
        [RegularExpression(@"(\d+)?", ErrorMessage = "Please enter a postive number")]
        public int? versionClosedBuild { get; set; }
        public string versionClosedMisc { get; set; }
        public string Syteline { get; set; }
        //[Required]
        public string benifits { get; set; }
        //[Required]
        public string problem { get; set; }
        //[Required]
        public string solution { get; set; }
        public string customerLocation { get; set; }
        public string customerCompany { get; set; }
        public string MachineSN { get; set; }
        public string Details { get; set; }
        public DateTime? ResolvedOn { get; set; }
        [Range(1, Int32.MaxValue, ErrorMessage = "Please enter a number equal to or larger then 1.")]
        public int? RelatedInputID { get; set; }
        public List<RelatedREA> RelatedREAIDs { get; set; }        
        public DateTime? ClosedOn { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public int? ApprovedByID { get; set; }
        public string ApprovedBy { get; set; }
        public bool sendToAll { get; set; }

        public int? Rank { get; set; }
        public int? Size { get; set; }
        public string ManagersNote { get; set; }
        public List<dynamic> CodeReviews { get; set; }

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

        private REATrackerDB.DisplayName displayname;
        //for display name preference

        /*
        [RegularExpression(@"(\d+)?(\.\d+)?(\.\d+)?(\s(\w*\W*)*)*", ErrorMessage = "Format : '#.#.# string'")]
        public String VersionPlanned { get; set; }
        [RegularExpression(@"(\d+)+(\.\d+)+(\.\d+)+(\s(\w*\W*)*)*", ErrorMessage = "Format : '#.#.# string'")]
        public String VersionReleased { get; set; }
        [Required]
        [RegularExpression(@"(\d+)?(\.\d+)?(\.\d+)?(\s(\w*\W*)*)*", ErrorMessage = "Format : '#.#.# string'")]
        public String VersionClosed { get; set; }
         */
        //Attachments
        public List<HttpPostedFileBase>
                        File
        { get; set; }
        public String FileDescription { get; set; }
        public List<SCRAttachments> Files { get; set; }
        public int Validator { get; set; }
        public int Rel_Coord { get; set; }
        public int SCCB { get; set; }
        public int DefaultTester { get; set; }
        public string UploadErrorMessage { get; set; }
        //bool can edit
        public bool canEditTitle { get; set; }
        public bool canEditproductId { get; set; }
        public bool canEditmoduleId { get; set; }
        public bool canEditversionFound { get; set; }
        public bool canEditissueType { get; set; }
        public bool canEditpriority { get; set; }
        public bool canEditstatusId { get; set; }
        public bool canEditclosedReasonId { get; set; }
        public bool canEditisDocumentChange { get; set; }
        public bool canEditisDatabaseChange { get; set; }
        public bool canEditisRequrementsChange { get; set; }
        public bool canEditplannedHourstoSpecify { get; set; }
        public bool canEditactualHourstoSpecify { get; set; }
        public bool canEditplannedHourstoFix { get; set; }
        public bool canEditactualHourstoFix { get; set; }
        public bool canEditplannedHourstoTest { get; set; }
        public bool canEditactualHourstoTest { get; set; }
        public bool canEditversionPlanned { get; set; }
        public bool canEditversionReleased { get; set; }
        public bool canEditsubmitedById { get; set; }
        public bool canEditApprovedById { get; set; }
        public bool canEditassignedToId { get; set; }
        public bool canEditresolvedByID { get; set; }
        public bool canEditclosedByID { get; set; }
        public bool canEditcustomerBug { get; set; }
        public bool canEditcustomerName { get; set; }
        public bool canEditSyteline { get; set; }
        public bool canEditNotesandResolution { get; set; }
        public bool canEditAttachements { get; set; }

        public bool canEditbenifits { get; set; }
        public bool canEditproblem { get; set; }
        public bool canEditsolution { get; set; }
        //public bool         canEditcustomerLocation         { get; set; }
        //public bool         canEditcustomerCompany          { get; set; }
        public bool canEditManagersNote { get; set; }

        //Display
        public List<dynamic> Products { get; set; }
        public List<dynamic> Issues { get; set; }
        public List<dynamic> Priorities { get; set; }
        public List<dynamic> ModuleList { get; set; }
        public List<dynamic> Statuses { get; set; }
        public List<dynamic> History { get; set; }
        public List<dynamic> Team { get; set; }
        public List<dynamic> ClosedReasons { get; set; }
        public List<dynamic> Keywords { get; set; }
        public List<dynamic> KeywordsAvailable { get; set; }

        //Remove Ids
        public List<int> RemoveRelatedID { get; set; }
        public List<int> RemoveFileID { get; set; }

        //Watching
        public String WatchingString { get; set; }

        //New Code Review
        public string CodeReviewTitle { get; set; }
        public string FileList { get; set; }
        public string CodeLocation { get; set; }
        public string Notes { get; set; }

        public string EditCodeReviewTitle { get; set; }
        public int EditCodeReviewId { get; set; }
        public string EditFileList { get; set; }
        public string EditCodeLocation { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string EditNotes { get; set; }
        public string CodeReviewStatus { get; set; }
        public string[] FileListSplit { get; set; }
        public List<dynamic> Reviewers { get; set; }

        //Constructors
        public EditREA()
        {
            HttpContext context = HttpContext.Current;
            //Populate storage variables
            if (this.RelatedREAIDs == null)
            {
                this.RelatedREAIDs = new List<RelatedREA>();
            }
            if (this.Files == null)
            {
                this.Files = new List<SCRAttachments>();
            }
            if (context.Session["Files-" + Convert.ToString(this.TrackingId)] == null)
            {
                context.Session.Add("Files-" + Convert.ToString(this.TrackingId), new List<SCRAttachments>());
            }
            if (this.RemoveRelatedID == null)
            {
                this.RemoveRelatedID = new List<int>();
            }
            if (this.RemoveFileID == null)
            {
                this.RemoveFileID = new List<int>();
            }
        }

        public EditREA(int argsUser_id, string argsAccountType, int TrackID)
        {
            if (this.Files == null)
            {
                this.Files = new List<SCRAttachments>();
            }
            if (this.RelatedREAIDs == null)
            {
                this.RelatedREAIDs = new List<RelatedREA>();
            }
            if (this.RemoveRelatedID == null)
            {
                this.RemoveRelatedID = new List<int>();
            }
            if (this.RemoveFileID == null)
            {
                this.RemoveFileID = new List<int>();
            }
            this.User_id = argsUser_id;
            this.AccountType = argsAccountType;
            if (this.AccountType == "Administrator")
            {
                this.IsAdmin = true;
            }
            this.TrackingId = TrackID;
            HttpContext context = HttpContext.Current;
            //Populate storage variables

            if (this.Files == null)
            {
                this.Files = new List<SCRAttachments>();
            }
            if (context.Session["Files-" + Convert.ToString(this.TrackingId)] == null)
            {
                context.Session.Add("Files-" + Convert.ToString(this.TrackingId), new List<SCRAttachments>());
            }
            this.init();
        }

        //  ==========================================================================
        //||    Get Methods                                                          ||  
        //  ==========================================================================

        public void init()
        {
            //set SCR Details
            if (this.productId == null || this.productId == 0)
            {
                this.setREA();
            }
            //set the product access
            this.setAccess();
            //Set products dropdownlist
            this.Products = this.setProducts(this.User_id);
            //Set the Issue
            this.Issues = this.setIssues();
            //Set the priority
            this.Priorities = this.setPriority();
            //Set Approved By and on
            this.setApproved((int)this.TrackingId);
            //Set the Statuses
            this.Statuses = this.setStatus();
            //Set the History
            this.setHistory(this.TrackingId);

            //get the displaynamepref from db 
            try
            {
                displayname = (REATrackerDB.DisplayName)Convert.ToInt32(new REATrackerDB().ProcessScalarCommand("SELECT DISPLAY_NAME_PREF FROM ST_USERS WHERE ST_USERS.USER_ID = " + this.User_id));
            }
            catch (Exception)
            {
                //if something goes wrong we assign it to default preference (firstname lastname)
                displayname = REATrackerDB.DisplayName.DefaultDisplay;
            }
            finally
            {
                //set the session variable (even though we dont use it)

            }
            //Set the select list for product team
            this.Team = this.selectListTeam(this.assignedToId);
            //Set related SCR
            this.setRelated();
            //Set Closed Reason
            this.ClosedReasons = this.setClosedReason();
            //Set default users
            this.setDefaultUsers((int)this.productId);
            //Set Attachments
            if (this.Files != null && this.Files.Count == 0 && this.RemoveFileID != null && this.RemoveFileID.Count == 0)
            {
                this.setAttachment();
            }
            this.populateWatching();
            this.populateLastRelease();
            this.setKeywords();
        }
        private void populateLastRelease()
        {
            DataTable dt = new REATrackerDB().ProcessCommand(
@"
Select  top 1 
ST_PRODUCT_RELEASE.MAJOR, ST_PRODUCT_RELEASE.MINOR, ST_PRODUCT_RELEASE.BUILD
 from ST_PRODUCT_RELEASE " +
" where PRODUCT_ID = " + this.productId +
 " order by BUILT_ON desc"
                );
            if (dt.Rows.Count > 0)
            {
                this.lastReleaseMajor = Convert.ToInt32(dt.Rows[0]["MAJOR"]);
                this.lastReleaseMinor = Convert.ToInt32(dt.Rows[0]["MINOR"]);
                this.lastReleaseBuild = Convert.ToInt32(dt.Rows[0]["BUILD"]);
            }
        }

        private void setREA()
        {
            string cmdText =
                "SELECT REA_TRACK.TITLE,PRODUCT, priority, STATUS, " +
                "SUBMITTED_BY, ASSIGNED_TO,RESOLVED_BY,CLOSED_BY, " +
                "SUBMITTED_ON, ASSIGNED_ON," +
                "RESOLVED_ON, CLOSED_ON, MANAGERS_NOTE " +
                "FROM REA_TRACK " +
                "WHERE TRACKING_ID ="
                + this.TrackingId + ";";
            REATrackerDB sql = new REATrackerDB();
            sql.OpenConnection();

            DataTable dt = sql.ProcessCommand(cmdText);
            if (dt.Rows.Count > 0)
            {
                this.Title = Convert.ToString(dt.Rows[0]["TITLE"]);
                this.productId = Convert.ToInt32(dt.Rows[0]["PRODUCT"]);
                this.priority = Convert.ToInt32(dt.Rows[0]["priority"]);
                this.statusId = Convert.ToInt32(dt.Rows[0]["STATUS"]);
                this.submitedById = (dt.Rows[0]["SUBMITTED_BY"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["SUBMITTED_BY"]));
                this.assignedToId = (dt.Rows[0]["ASSIGNED_TO"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["ASSIGNED_TO"]));
                this.resolvedByID = (dt.Rows[0]["RESOLVED_BY"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["RESOLVED_BY"]));
                this.closedByID = (dt.Rows[0]["CLOSED_BY"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["CLOSED_BY"]));
                this.submittedOn = (dt.Rows[0]["SUBMITTED_ON"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dt.Rows[0]["SUBMITTED_ON"]));
                this.assignedOn = (dt.Rows[0]["ASSIGNED_ON"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dt.Rows[0]["ASSIGNED_ON"]));
                this.ResolvedOn = (dt.Rows[0]["RESOLVED_ON"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dt.Rows[0]["RESOLVED_ON"]));
                this.ClosedOn = (dt.Rows[0]["CLOSED_ON"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dt.Rows[0]["CLOSED_ON"]));

                this.ManagersNote = dt.Rows[0]["MANAGERS_NOTE"] == DBNull.Value ? null : Convert.ToString(dt.Rows[0]["MANAGERS_NOTE"]);
            }
            sql.CloseConnection();
        }

        private void setAccess()
        {
            this.hasAccess = false;

            if (this.AccountType == "Administrator")
            {
                //Administrator always has access to everything
                this.hasAccess = true;
                this.AccessType = 5; //Manager type
                this.IsAdmin = true;
            }
            else
            {
                //Determine access rights based on the users current role on the team of the SCR

                string cmdText = $"SELECT ST_PRODUCT_TEAM.ROLE FROM ST_PRODUCT_TEAM WHERE ST_PRODUCT_TEAM.PRODUCT = {this.productId} AND ST_PRODUCT_TEAM.USER_ID= {this.User_id}";
                REATrackerDB sql = new REATrackerDB();
                sql.OpenConnection();
                //this has to be product team
                DataTable dt = sql.ProcessCommand(cmdText);
                if (dt.Rows.Count > 0)
                {
                    //the role on the hae been found.
                    this.AccessType = Convert.ToInt32(dt.Rows[0][0]);
                    this.hasAccess = true;
                }
                else
                {
                    //the user is not on the team, so access permission is based on their account type
                    if (this.AccountType == "Power User" || this.AccountType == "Employee" || this.submitedById == this.User_id)
                    {
                        this.AccessType = 2;//user role
                        this.hasAccess = true;
                    }
                    else
                    {
                        //no soup for you
                        this.AccessType = -1;
                    }
                }
                sql.CloseConnection();
            }
            //this is done separate on purpose
            this.canEditManagersNote = false;
            if (this.hasAccess && this.AccessType == 5)
            {
                this.canEditManagersNote = true;
            }

            //set editablity on certain feilds
            if (this.hasAccess)
            {
                string cmdText = "SELECT EDIT_PRODUCT, EDIT_VERSION, EDIT_MODULE, EDIT_TITLE, EDIT_PRIORITY, EDIT_ISSUE_TYPE, EDIT_STATUS, EDIT_DOC_CHANGE" +
                ", EDIT_DB_CHANGE, EDIT_REQ_DOC, EDIT_PLANNED_HOURS, EDIT_ACTUAL_HOURS, EDIT_SUBMITTED_BY, EDIT_ASSIGNED_TO, EDIT_RESOLVED_BY, EDIT_CLOSED_BY" +
                ", EDIT_NOTES_RES, EDIT_ATTACH_FILES, EDIT_PLANNED_RELEASE, EDIT_ACTUAL_RELEASE, EDIT_CUSTOMER_BUG, EDIT_CUSTOMER_NAME " +
                ", EDIT_CLOSED_REASON, EDIT_PLANNED_HOURS_TEST, EDIT_ACTUAL_HOURS_TEST, EDIT_SYTELINE_REF, EDIT_PLANNED_HOURS_SPEC, EDIT_ACTUAL_HOURS_SPEC " +
                "FROM ST_SCR_EDIT WHERE GROUP_ID=" + Convert.ToString(this.AccessType) + ";";
                REATrackerDB sql = new REATrackerDB();
                sql.OpenConnection();
                DataTable dt = sql.ProcessCommand(cmdText);
                if (dt.Rows.Count > 0)
                {
                    //default all the access rights based on the role
                    this.canEditproductId = Convert.ToBoolean(dt.Rows[0]["EDIT_PRODUCT"]);
                    this.canEditversionFound = Convert.ToBoolean(dt.Rows[0]["EDIT_VERSION"]);
                    this.canEditmoduleId = Convert.ToBoolean(dt.Rows[0]["EDIT_MODULE"]);
                    this.canEditTitle = Convert.ToBoolean(dt.Rows[0]["EDIT_TITLE"]);
                    this.canEditpriority = Convert.ToBoolean(dt.Rows[0]["EDIT_PRIORITY"]);
                    this.canEditissueType = Convert.ToBoolean(dt.Rows[0]["EDIT_ISSUE_TYPE"]);
                    this.canEditstatusId = Convert.ToBoolean(dt.Rows[0]["EDIT_STATUS"]);
                    this.canEditisDocumentChange = Convert.ToBoolean(dt.Rows[0]["EDIT_DOC_CHANGE"]);
                    this.canEditisDatabaseChange = Convert.ToBoolean(dt.Rows[0]["EDIT_DB_CHANGE"]);
                    this.canEditisRequrementsChange = Convert.ToBoolean(dt.Rows[0]["EDIT_REQ_DOC"]);
                    this.canEditplannedHourstoFix = Convert.ToBoolean(dt.Rows[0]["EDIT_PLANNED_HOURS"]);
                    this.canEditactualHourstoFix = Convert.ToBoolean(dt.Rows[0]["EDIT_ACTUAL_HOURS"]);
                    this.canEditsubmitedById = Convert.ToBoolean(dt.Rows[0]["EDIT_SUBMITTED_BY"]);
                    this.canEditassignedToId = Convert.ToBoolean(dt.Rows[0]["EDIT_ASSIGNED_TO"]);
                    this.canEditresolvedByID = this.IsAdmin ? true : Convert.ToBoolean(dt.Rows[0]["EDIT_RESOLVED_BY"]);
                    this.canEditclosedByID = Convert.ToBoolean(dt.Rows[0]["EDIT_CLOSED_BY"]);
                    this.canEditNotesandResolution = Convert.ToBoolean(dt.Rows[0]["EDIT_NOTES_RES"]);
                    this.canEditAttachements = Convert.ToBoolean(dt.Rows[0]["EDIT_ATTACH_FILES"]);
                    this.canEditversionPlanned = Convert.ToBoolean(dt.Rows[0]["EDIT_PLANNED_RELEASE"]);
                    this.canEditversionReleased = Convert.ToBoolean(dt.Rows[0]["EDIT_ACTUAL_RELEASE"]);
                    this.canEditcustomerBug = Convert.ToBoolean(dt.Rows[0]["EDIT_CUSTOMER_BUG"]);
                    this.canEditcustomerName = Convert.ToBoolean(dt.Rows[0]["EDIT_CUSTOMER_NAME"]);
                    this.canEditclosedReasonId = Convert.ToBoolean(dt.Rows[0]["EDIT_CLOSED_REASON"]);
                    this.canEditplannedHourstoTest = Convert.ToBoolean(dt.Rows[0]["EDIT_PLANNED_HOURS_TEST"]);
                    this.canEditactualHourstoTest = Convert.ToBoolean(dt.Rows[0]["EDIT_ACTUAL_HOURS_TEST"]);
                    this.canEditSyteline = Convert.ToBoolean(dt.Rows[0]["EDIT_SYTELINE_REF"]);
                    this.canEditplannedHourstoSpecify = Convert.ToBoolean(dt.Rows[0]["EDIT_PLANNED_HOURS_SPEC"]);
                    this.canEditactualHourstoSpecify = Convert.ToBoolean(dt.Rows[0]["EDIT_ACTUAL_HOURS_SPEC"]);

                    this.canEditproblem = true;
                    this.canEditsolution = true;
                    this.canEditbenifits = true;

                    /*
                    //specifically disable controls if the SCR is closed, unless you are the admin
                    if ((this.statusId == (int)REATrackerDB.StatusChangeID.status_closed) && (!this.IsAdmin))
                    {
                        //disable controls
                        this.canEditproductId = false;
                        this.canEditversionFound = false;
                        this.canEditmoduleId = false;
                        this.canEditTitle = false;
                        this.canEditpriority = false;
                        this.canEditissueType = false;
                        this.canEditstatusId = false;
                        //this.canEditisDocumentChange = false;
                        //this.canEditisDatabaseChange = false;
                        //this.canEditisRequrementsChange = false;
                        this.canEditplannedHourstoFix = false;
                        this.canEditactualHourstoFix = false;
                        this.canEditsubmitedById = false;
                        this.canEditassignedToId = false;
                        this.canEditresolvedByID = false;
                        this.canEditclosedByID = false;
                        //this.canEditNotesandResolution = false;
                        //this.canEditAttachements = false;
                        this.canEditversionPlanned = false;
                        this.canEditversionReleased = false;
                        //this.canEditcustomerBug = false;
                        this.canEditcustomerName = false;
                        //this.canEditclosedReasonId = false;
                        this.canEditplannedHourstoTest = false;
                        this.canEditactualHourstoTest = false;
                        this.canEditSyteline = false;
                        this.canEditplannedHourstoSpecify = false;
                        this.canEditactualHourstoSpecify = false;
                        this.canEditproblem = false;
                        this.canEditsolution = false;
                        this.canEditbenifits = false;
                    }
                    */
                }
                else
                {
                    this.canEditproductId = false;
                    this.canEditversionFound = false;
                    this.canEditmoduleId = false;
                    this.canEditTitle = false;
                    this.canEditpriority = false;
                    this.canEditissueType = false;
                    this.canEditstatusId = false;
                    this.canEditisDocumentChange = false;
                    this.canEditisDatabaseChange = false;
                    this.canEditisRequrementsChange = false;
                    this.canEditplannedHourstoFix = false;
                    this.canEditactualHourstoFix = false;
                    this.canEditsubmitedById = false;
                    this.canEditassignedToId = false;
                    this.canEditresolvedByID = false;
                    this.canEditclosedByID = false;
                    this.canEditNotesandResolution = false;
                    this.canEditAttachements = false;
                    this.canEditversionPlanned = false;
                    this.canEditversionReleased = false;
                    this.canEditcustomerBug = false;
                    this.canEditcustomerName = false;
                    this.canEditclosedReasonId = false;
                    this.canEditplannedHourstoTest = false;
                    this.canEditactualHourstoTest = false;
                    this.canEditSyteline = false;
                    this.canEditplannedHourstoSpecify = false;
                    this.canEditactualHourstoSpecify = false;
                    this.canEditproblem = false;
                    this.canEditsolution = false;
                    this.canEditbenifits = false;
                }
                sql.CloseConnection();
            }
        }

        private List<dynamic> setProducts(int userID)
        {
            REATrackerDB _sql = new REATrackerDB();
            int temp = 0;
            Int32.TryParse(Convert.ToString(_sql.ProcessScalarCommand("SELECT DEFAULT_PRODUCT FROM ST_USERS WHERE USER_ID= " + userID)), out temp);

            int defaultProduct = temp;
            //this.ProductID = defaultProduct;
            if (this.productId != null)
            {
                defaultProduct = (int)this.productId;
            }
            else
            {
                this.productId = defaultProduct;
            }
            List<dynamic> list = new List<dynamic>();
            using (System.Data.DataTable dt = _sql.GetProductList(userID, false))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].ID = (int)dt.Rows[i][0];
                    list[i].Name = dt.Rows[i][1].ToString();
                    list[i].IsDefault = (defaultProduct == list[i].ID);
                    i++;
                }
            }
            return list;
        }

        private List<dynamic> setIssues()
        {
            int defaultIssue = 1;
            if (this.issueType != null)
            {
                defaultIssue = (int)this.issueType;
            }
            REATrackerDB _sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            String cmdText = "SELECT ST_ISSUE_TYPE.ISSUE_ID,ST_ISSUE_TYPE.NAME, ST_ISSUE_TYPE.DESCRIPTION FROM ST_ISSUE_TYPE;";
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmdText))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].ID = (int)dt.Rows[i][0];
                    list[i].Name = dt.Rows[i][1].ToString();
                    list[i].IsDefault = (defaultIssue == list[i].ID);
                    list[i].Description = dt.Rows[i][2];
                    i++;
                }
            }
            return list;
        }

        private List<dynamic> setPriority()
        {
            int defaultPriority = -1;
            if (this.priority != null)
            {
                defaultPriority = (int)this.priority;
            }
            REATrackerDB _sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            String cmdText = "SELECT ST_priority.ID, ST_priority.NAME, ST_priority.DESCRIPTION FROM ST_priority;";
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmdText))
            {
                int i = 1;//index to list
                int j = 0;//index to datatable
                list.Add(new System.Dynamic.ExpandoObject());
                list[0].ID = "";
                list[0].Name = "";
                list[0].IsDefault = false;
                list[0].Description = "";
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].ID = (int)dt.Rows[j][0];
                    list[i].Name = dt.Rows[j][1].ToString();
                    list[i].IsDefault = (defaultPriority == list[i].ID);
                    list[i].Description = dt.Rows[j][2];
                    i++;
                    j++;
                }
            }
            return list;
        }

        private List<dynamic> setModule(int ProductID)
        {
            int defaultModule = -1;
            if (this.moduleId != null)
            {
                defaultModule = (int)this.moduleId;
            }
            REATrackerDB _sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            String cmdText = "SELECT ST_MODULE.MODULE_ID, ST_MODULE.NAME, ST_MODULE.DESCRIPTION FROM ST_MODULE WHERE PRODUCT = " + Convert.ToString(ProductID) + " AND RETIRED =0;";
            bool hasDefaultInList = false;
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmdText))
            {
                int i = 0; // keeps track of dt
                int j = 1; //keeps track of list
                list.Add(new System.Dynamic.ExpandoObject());
                list[0].ID = -1;
                list[0].Name = "No Module";
                list[0].IsDefault = (defaultModule == list[0].ID);
                list[0].Descrption = "";

                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[j].ID = (int)dt.Rows[i][0];
                    list[j].Name = dt.Rows[i][1].ToString();
                    list[j].IsDefault = (defaultModule == list[j].ID);
                    list[j].Description = dt.Rows[i][2];
                    if (defaultModule == list[j].ID)
                    {
                        hasDefaultInList = true;
                    }
                    i++;
                    j++;
                }
                if (!hasDefaultInList && defaultModule != -1)
                {
                    string command =
                        "SELECT ST_MODULE.MODULE_ID, ST_MODULE.NAME, ST_MODULE.DESCRIPTION FROM ST_MODULE WHERE ST_MODULE.MODULE_ID = "
                        + defaultModule;
                    DataTable tempDT = _sql.ProcessCommand(command);
                    if (tempDT.Rows.Count > 0)
                    {
                        list.Add(new System.Dynamic.ExpandoObject());
                        list[j].ID = (int)tempDT.Rows[0][0];
                        list[j].Name = tempDT.Rows[0][1].ToString();
                        list[j].IsDefault = true;
                        list[j].Description = tempDT.Rows[0][2];
                        j++;

                    }
                }
            }

            return list;
        }

        private List<dynamic> setStatus()
        {
            int defaultIssue = 1;
            if (this.statusId != null)
            {
                defaultIssue = (int)this.statusId;
            }

            REATrackerDB _sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            String cmdText = "";
            if (this.hasAccess == false && this.AccountType != "Administrator")
            {
                cmdText = "SELECT NAME FROM ST_STATUS WHERE (STATUS_ID= " + this.statusId + ") ORDER BY STATUS_ORDER ASC;";
            }
            else if (this.AccountType == "Administrator")
            {
                cmdText = "SELECT NAME, STATUS_ID FROM ST_STATUS ORDER BY STATUS_ORDER ASC;";
            }
            else if (this.hasAccess)
            {
                cmdText =
                "SELECT ST_STATUS.NAME AS STATUS, ST_STATUS.STATUS_ID AS STATUS_ID, " +
                              "ST_STATUS_CHANGE.EXCEPTION_CHANGE AS EXCEPTION, " +
                              "ST_STATUS_CHANGE.FROM_STATUS AS FROM_STATUS FROM ST_STATUS_CHANGE " +
                              "INNER JOIN ST_STATUS ON ST_STATUS_CHANGE.TO_STATUS = ST_STATUS.STATUS_ID " +
                              "WHERE ((ST_STATUS_CHANGE.FROM_STATUS=" + this.statusId + ") AND (ST_STATUS_CHANGE.GROUP_ID=" + this.AccessType + "))" +
                              "ORDER BY ST_STATUS.STATUS_ORDER ASC";
            }

            using (System.Data.DataTable dt = _sql.ProcessCommand(cmdText))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].ID = (int)dt.Rows[i][1];
                    list[i].Name = dt.Rows[i][0].ToString();
                    list[i].IsDefault = (defaultIssue == list[i].ID);
                    i++;
                }
            }

            if (list.Count == 0 || (list.Where(u => u.ID == this.statusId).Count() < 1))
            {
                list.Insert(0, new System.Dynamic.ExpandoObject());
                list[0].ID = this.statusId;
                list[0].Name = Convert.ToString(_sql.ProcessScalarCommand("SELECT NAME FROM ST_STATUS WHERE STATUS_ID = " + this.statusId + ";"));
                list[0].IsDefault = true;
                list.OrderByDescending(u => u["ID"]);
            }
            return list;
        }

        private void setAttachment()
        {

            REATrackerDB _sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            string cmd = "SELECT ST_ATTACHMENT.FILE_DATA, ST_ATTACHMENT.FILENAME, ST_ATTACHMENT.DESCRIPTION,ST_ATTACHMENT.FILE_ID FROM ST_ATTACHMENT where TRACKING_ID = " +
                Convert.ToString(this.TrackingId) +
                " ORDER BY FILE_DATE ASC";
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmd))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    string filename = System.IO.Path.GetFileName(Convert.ToString(dt.Rows[i][1]));
                    this.Files.Add(new SCRAttachments((byte[])(dt.Rows[i][0] == DBNull.Value ? null : dt.Rows[i][0]),
                        filename, Convert.ToString(dt.Rows[i][2]), Convert.ToInt32(dt.Rows[i][3])));
                    i++;
                }
            };
            HttpContext context = HttpContext.Current;
            context.Session["Files-" + Convert.ToString(this.TrackingId)] = this.Files;
        }

        //NOTE: duplice code in the display
        private void setKeywords()
        {
            REATrackerDB _sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            using (System.Data.DataTable dt = _sql.GetKeywords(this.TrackingId))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].ID = Convert.ToString(row[0]);
                    list[i].Name = Convert.ToString(row[1]);
                    list[i].Description = Convert.ToString(row[2]);
                    i++;
                }
            }
            this.Keywords = list;
        }

        private void setRelated()
        {
            if (this.RelatedREAIDs == null || this.RelatedREAIDs.Count == 0)
            {
                this.RelatedREAIDs = new List<RelatedREA>();
                String cmdText = "SELECT SCR_ID, RELATED_ID FROM ST_RELATED WHERE TRACKING_ID = " + this.TrackingId + ";";
                REATrackerDB sql = new REATrackerDB();
                sql.OpenConnection();
                DataTable dt = sql.ProcessCommand(cmdText);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bool exists = false;
                    RelatedREA toAdd = new RelatedREA(this.TrackingId, Convert.ToInt32(dt.Rows[i][0]), false);
                    exists = toAdd.getFields();
                    if (exists && !this.RemoveRelatedID.Contains(Convert.ToInt32(dt.Rows[i][1])))
                    {
                        toAdd.RelatedID = Convert.ToInt32(dt.Rows[i][1]);
                        this.RelatedREAIDs.Add(toAdd);
                    }
                }
                sql.CloseConnection();

            }
        }
        private void setApproved(int SCR_id)
        {
            if (this.ApprovedByID == null || this.ApprovedOn == null || String.IsNullOrEmpty(this.ApprovedBy))
            {
                REATrackerDB _sql = new REATrackerDB();
                // String FullName = "";
                String cmdText =
                    "SELECT ST_USERS.FIRST_NAME, ST_USERS.LAST_NAME, ST_USERS.USER_ID, ST_HISTORY.MADE_ON " +
                    "FROM ST_TRACK " +
                    "INNER JOIN ST_HISTORY ON ST_TRACK.TRACKING_ID = ST_HISTORY.TRACKING_ID " +
                    "INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_HISTORY.MADE_BY " +
                    "WHERE ST_TRACK.TRACKING_ID = " + Convert.ToString(SCR_id) + " AND ST_HISTORY.DESCRIPTION LIKE '%Set to Approve%' " +
                    "ORDER BY ST_HISTORY.MADE_ON DESC";
                using (System.Data.DataTable dt = _sql.ProcessCommand(cmdText))
                {
                    int i = 0;
                    List<dynamic> list = new List<dynamic>();
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        list.Add(new System.Dynamic.ExpandoObject());
                        list[i].ID = (int)dt.Rows[i]["USER_ID"];
                        list[i].Description = Convert.ToDateTime(dt.Rows[0]["MADE_ON"]);
                        list[i].Name = Convert.ToString(dt.Rows[i]["FIRST_NAME"]) + " " + Convert.ToString(dt.Rows[i]["LAST_NAME"]);
                        this.ApprovedOn = list[i].Description;
                        this.ApprovedByID = list[i].ID;

                    }
                }

            }

        }
        //private List<dynamic> setApproved(int SCR_id)
        //{
        //    List<dynamic> list = new List<dynamic>();
        //    int defaultApproved = -1;
        //    if (this.ApprovedByID != null)
        //    {
        //        defaultApproved = (int)this.ApprovedByID;
        //    }
        //    REATrackerHelp sql = new REATrackerHelp();

        //    String cmdText = "SELECT ST_USERS.USER_ID, ST_USERS.FIRST_NAME, ST_USERS.LAST_NAME,  ST_HISTORY.MADE_ON " +
        //            "FROM ST_TRACK " +
        //            "INNER JOIN ST_HISTORY ON ST_TRACK.TRACKING_ID = ST_HISTORY.TRACKING_ID " +
        //            "INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_HISTORY.MADE_BY " +
        //            "WHERE ST_TRACK.TRACKING_ID = " + Convert.ToString(SCR_id) + " AND ST_HISTORY.DESCRIPTION LIKE '%Set to Approve%' " +
        //            "ORDER BY ST_HISTORY.MADE_ON DESC";
        //    using (System.Data.DataTable dt = sql.ProcessCommand(cmdText))
        //    {
        //        int i = 0;
        //        foreach (System.Data.DataRow row in dt.Rows)
        //        {
        //            list.Add(new System.Dynamic.ExpandoObject());
        //            list[i].ID = (int)dt.Rows[i]["USER_ID"];
        //            list[i].Name = Convert.ToString(dt.Rows[i]["FIRST_NAME"]) + " " + Convert.ToString(dt.Rows[i]["LAST_NAME"]);
        //            list[i].IsDefault = (defaultApproved == list[i].ID);
        //            list[i].Description = Convert.ToDateTime(dt.Rows[i]["MADE_ON"]);
        //            i++;
        //        }
        //    }
        //    return list;
        //}
        private List<dynamic> setClosedReason()
        {
            int defaultModule = -1;
            if (this.closedReasonId != null)
            {
                defaultModule = (int)this.closedReasonId;
            }
            REATrackerDB _sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            String cmdText = "SELECT ST_CLOSED_REASON.REASON_ID, ST_CLOSED_REASON.NAME FROM ST_CLOSED_REASON;";
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmdText))
            {
                int i = 0; // keeps track of dt
                int j = 1; //keeps track of list
                list.Add(new System.Dynamic.ExpandoObject());
                list[0].ID = "";
                list[0].Name = "Reason:";
                list[0].IsDefault = false;
                list[0].Descrption = "";

                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[j].ID = (int)dt.Rows[i][0];
                    list[j].Name = dt.Rows[i][1].ToString();
                    list[j].IsDefault = (defaultModule == list[j].ID);
                    i++;
                    j++;
                }
            }

            return list;
        }

        public List<dynamic> selectListUser(int? selected)
        {
            REATrackerDB sql = new REATrackerDB();

            DataTable all = sql.GetUsers();
            List<dynamic> list = new List<dynamic>();
            bool selectedislisted = false;
            list.Add(new System.Dynamic.ExpandoObject());
            list[0].ID = "";
            list[0].Name = "";
            list[0].IsDefault = false;

            for (int i = 0; i < all.Rows.Count; i++)
            {
                list.Add(new System.Dynamic.ExpandoObject());
                list[i + 1].ID = (int)all.Rows[i]["USER_ID"];//0 user ID
                if (displayname == REATrackerDB.DisplayName.FirstNameLastName)
                {
                    list[i + 1].Name = all.Rows[i]["FIRST_NAME"] + " " + all.Rows[i]["LAST_NAME"];
                }
                else
                {
                    list[i + 1].Name = all.Rows[i]["LAST_NAME"] + ", " + all.Rows[i]["FIRST_NAME"];
                }
                list[i + 1].IsDefault = (selected == list[i + 1].ID);
                if (selected == null || (selected == list[i + 1].ID))
                {
                    selectedislisted = true;
                }
            }
            //this is to order the list according to the last names
            if (displayname == REATrackerDB.DisplayName.LastNameFirstName)
            {
                list = list.OrderBy(x => x.Name).ToList();
            }

            if (!selectedislisted)
            {
                // go get the user and add them
                string tempcommand = "";
                if (displayname == REATrackerDB.DisplayName.FirstNameLastName)
                {

                    tempcommand = "SELECT USER_ID, (FIRST_NAME+' '+LAST_NAME) FROM ST_USERS WHERE USER_ID = "
                    + Convert.ToString(selected);
                }
                else
                {
                    tempcommand = "SELECT USER_ID, (LAST_NAME+', '+FIRST_NAME) FROM ST_USERS WHERE USER_ID = "
                    + Convert.ToString(selected);
                }
                DataTable dt = new REATrackerDB().ProcessCommand(tempcommand);
                if (dt.Rows.Count > 0)
                {
                    System.Dynamic.ExpandoObject temp = new System.Dynamic.ExpandoObject();
                    list.Add(temp);
                    list[list.Count() - 1].ID = (int)dt.Rows[0][0];//0 user ID
                    list[list.Count() - 1].Name = Convert.ToString(dt.Rows[0][1]);
                    list[list.Count() - 1].IsDefault = true;
                }
            }

            list[0].IsDefault = (selected == null || selected == list[list.Count - 1].ID);

            return list;
        }


        public List<dynamic> selectListTeam(int? selected)
        {
            List<dynamic> list = null;

            REATrackerDB sql = new REATrackerDB();
            string cmdText = "";

            cmdText = "SELECT ST_PRODUCT_TEAM.USER_ID, ";
            if (displayname == REATrackerDB.DisplayName.FirstNameLastName)
            {
                cmdText += "(ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME) AS FULL_NAME ";
            }
            else
            {
                cmdText += "(LAST_NAME + ', '+ FIRST_NAME) AS FULL_NAME ";
            }

            cmdText += @"FROM ST_PRODUCT_TEAM 
                        INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_PRODUCT_TEAM.USER_ID
                        WHERE ST_PRODUCT_TEAM.PRODUCT = " + Convert.ToString(this.productId) +
                        @"AND (ST_USERS.ACCOUNT_TYPE != -1 ";

            if (selected != null)
            {
                cmdText += "OR ST_USERS.USER_ID IN (" + selected + ")) ";
            }
            else
            {
                cmdText += ") ";
            }

            cmdText += "ORDER BY FULL_NAME";

            list = new List<dynamic>();

            bool selectedislisted = false;
            sql.OpenConnection();
            DataTable dt = sql.ProcessCommand(cmdText);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(new System.Dynamic.ExpandoObject());
                list[i].ID = (int)dt.Rows[i]["USER_ID"];
                list[i].Name = dt.Rows[i]["FULL_NAME"];
                list[i].IsDefault = (selected == list[i].ID);
                if (!selectedislisted && (selected == null || (selected == list[i].ID)))
                {
                    selectedislisted = true;
                }
            }
            if (!selectedislisted && (selected != null))
            {
                string tempcommand = "";
                // go get the user and add them
                if (displayname == REATrackerDB.DisplayName.FirstNameLastName)
                {
                    tempcommand = "SELECT USER_ID, (FIRST_NAME+ ' ' +LAST_NAME) AS FULL_NAME FROM ST_USERS WHERE USER_ID = "
                    + Convert.ToString(selected);
                }
                else
                {
                    tempcommand = "SELECT USER_ID, (LAST_NAME+ ', ' +FIRST_NAME) AS FULL_NAME FROM ST_USERS WHERE USER_ID = "
                    + Convert.ToString(selected);
                }
                DataTable tempdt = new REATrackerDB().ProcessCommand(tempcommand);
                if (tempdt.Rows.Count > 0)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[list.Count() - 1].ID = (int)tempdt.Rows[0]["USER_ID"];
                    list[list.Count() - 1].Name = Convert.ToString(tempdt.Rows[0]["FULL_NAME"]);
                    list[list.Count() - 1].IsDefault = true;
                }
            }
            System.Dynamic.ExpandoObject temp = new System.Dynamic.ExpandoObject();
            list.Insert(0, temp);
            list[0].ID = -1;
            list[0].Name = "Unknown User";
            list[0].IsDefault = (selected == null);// || selected == list[list.Count - 1].ID); 
            sql.CloseConnection();

            return list;
        }


        //ST_HISTORY
        private void setHistory(int SCR_id)
        {
            REATrackerDB _sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            string cmd = "SELECT ST_HISTORY.TYPE, ST_HISTORY.MADE_BY, ST_HISTORY.MADE_ON, ST_HISTORY.DESCRIPTION, ST_HISTORY.FILE_ID FROM ST_HISTORY WHERE TRACKING_ID= " +
                Convert.ToString(SCR_id) +
                " ORDER BY ST_HISTORY.HISTORY_ID desc, ST_HISTORY.MADE_ON desc";
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmd))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].Type = Convert.ToInt32(dt.Rows[i]["TYPE"]);
                    list[i].MadeBy = dt.Rows[i][1] == DBNull.Value ? "" : this.getFullName(Convert.ToInt32(dt.Rows[i]["MADE_BY"]));
                    list[i].MadeOn = Convert.ToString(dt.Rows[i]["MADE_ON"]);
                    list[i].fileID = Convert.ToString(dt.Rows[i]["FILE_ID"]);
                    if (dt.Rows[0]["FILE_ID"] != null && Convert.ToString(dt.Rows[i]["FILE_ID"]) != "")
                    {
                        list[i].FileName = getFilename(Convert.ToInt32(list[i].fileID));
                    }
                    /*
                    if (list[i].Type == 8)
                    {
                        string scrID = Convert.ToString(getRelatedREAID(Convert.ToInt32(dt.Rows[i][4])));
                        list[i].Description = "Related SCR#<a href='" + scrID + "' target='_blank'>" + scrID + "</a> - " + Convert.ToString(dt.Rows[i][3]);
                    }*/
                    if (list[i].Type ==(int) REATrackerDB.HistoryChangeType.chg_related)
                    {
                        string scrID = Convert.ToString(getRelatedREAID(Convert.ToInt32(dt.Rows[i]["FILE_ID"])));
                        list[i].LinkSCR = scrID;
                        list[i].Title = Convert.ToString(dt.Rows[i]["DESCRIPTION"]);
                    }
                    else
                    {
                        list[i].Description = Convert.ToString(dt.Rows[i]["DESCRIPTION"]);
                    }
                    list[i].MadeById = dt.Rows[i]["MADE_BY"] == DBNull.Value ? -1 : Convert.ToInt32(dt.Rows[i]["MADE_BY"]);
                    i++;
                }
            }
            this.History = list;
        }
        private int getRelatedREAID(int RelatedID)
        {
            REATrackerDB sql = new REATrackerDB();
            sql.OpenConnection();

            string cmdtext = "SELECT ST_RELATED.SCR_ID FROM ST_RELATED WHERE ST_RELATED.RELATED_ID = " + Convert.ToString(RelatedID) + ";";
            DataTable dt = sql.ProcessCommand(cmdtext);

            sql.CloseConnection();
            int value = dt.Rows.Count == 0 ? 0 : Convert.ToInt32(dt.Rows[0][0]);
            return value;
        }
        //Other resources
        private String getFilename(int fileID)
        {
            REATrackerDB _sql = new REATrackerDB();
            String FullName = "";
            String cmdText = "SELECT ST_ATTACHMENT.FILENAME FROM ST_ATTACHMENT WHERE FILE_ID = " + Convert.ToString(fileID) + ";";
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmdText))
            {
                if (dt.Rows.Count > 0)
                {
                    FullName = System.IO.Path.GetFileName(Convert.ToString(dt.Rows[0][0]));
                }
            }
            return FullName;
        }
        public string getFullName(int stUserID)
        {
            string fullname = "";
            REATrackerDB sql = new REATrackerDB();
            sql.OpenConnection();
            DataTable dt = sql.ProcessCommand("Select FIRST_NAME, LAST_NAME FROM ST_USERS WHERE USER_ID = " + Convert.ToString(stUserID));
            if (dt.Rows.Count > 0)
            {
                fullname = Convert.ToString(dt.Rows[0][0]) + " " + Convert.ToString(dt.Rows[0][1]);
            }
            sql.CloseConnection();
            return fullname;
        }
        public string GetRelease(string major, string minor, string build, string misc)
        {
            string release = "";
            int tempInt = 0;
            if (Int32.TryParse(major, out tempInt))
            {
                release += major;
                if (Int32.TryParse(minor, out tempInt))
                {
                    release += ".";
                    release += minor;
                    if (Int32.TryParse(build, out tempInt))
                    {
                        release += ".";
                        release += build;

                    }
                }
            }
            if (!String.IsNullOrEmpty(misc))
            {
                release += " " + misc;
            }
            return release;
        }
        public void setDefaultUsers(int ProductID)
        {
            String cmdText = "SELECT VALIDATOR,RELEASE_COORDINATOR,SCCB,DEFAULT_TESTER FROM ST_PRODUCT WHERE (PRODUCT_ID= " + ProductID + ");";
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(cmdText);
            if (dt.Rows.Count != 0)
            {
                this.Validator = dt.Rows[0][0] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0][0]);
                this.Rel_Coord = dt.Rows[0][1] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0][1]);
                this.SCCB = dt.Rows[0][2] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0][2]);
                this.DefaultTester = dt.Rows[0][3] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0][3]);

            }
        }
        public void populateWatching()
        {
            //this.Watching = new List<dynamic>();
            String Command = "";
            if (displayname == REATrackerDB.DisplayName.FirstNameLastName)
            {
                Command = "select "
                             + " (st_users.FIRST_NAME+' '+st_users.LAST_NAME) as FULL_NAME "
                             + " from REA_WATCH_LIST"
                             + " inner join ST_USERS on REA_WATCH_LIST.USER_ID = ST_USERS.USER_ID"
                             + " where REA_WATCH_LIST.TRACKING_ID =" + this.TrackingId + " order by FULL_NAME asc;";
            }
            else
            {
                Command = "select "
                             + " (st_users.LAST_NAME+', '+st_users.FIRST_NAME) as FULL_NAME "
                             + " from REA_WATCH_LIST"
                             + " inner join ST_USERS on REA_WATCH_LIST.USER_ID = ST_USERS.USER_ID"
                             + " where REA_WATCH_LIST.TRACKING_ID =" + this.TrackingId + " order by FULL_NAME asc;";
            }

            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(Command);
            this.WatchingString = "";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i != 0)
                {
                    this.WatchingString += ", ";
                }
                this.WatchingString += Convert.ToString(dt.Rows[i]["FULL_NAME"]);
            }
        }
        //  ==========================================================================
        //||    Post Methods                                                         ||  
        //  ==========================================================================
        /*
         public void ParseStrings()
         {
             if (this.VersionPlanned != null)
             {
                 char[] delim = { '.' };
                 string[] words = this.VersionPlanned.Split(delim);
                 if (words.Length >= 1)
                 {
                     int temp = 0;
                     if (Int32.TryParse(words[0], out temp))
                     {
                         this.versionPlannedMajor = temp;
                         temp = 0;
                         if (words.Length >= 2 && Int32.TryParse(words[1], out temp))
                         {
                             this.versionPlannedMinor = temp;
                             temp = 0;
                             if (words.Length >= 3)
                             {
                                 char[] space = { ' ' };
                                 string[] buildAndMisc = words[2].Split(space);
                                 if (buildAndMisc.Length >= 1 && Int32.TryParse(buildAndMisc[0], out temp))
                                 {
                                     this.versionPlannedBuild = temp;
                                 }
                             }
                         }
                     }
                 }
                 char[] spaceArray = { ' ' };
                 words = this.VersionPlanned.Split(spaceArray);
                 if (words.Length >= 2)
                 {
                     string tempMisc = "";
                     for (int i = 1; i < words.Length; i++)
                     {
                         if (i > 1)
                         {
                             tempMisc += " ";
                         }
                         tempMisc += words[i];
                     }
                     this.versionPlannedMisc = tempMisc;
                 }
             }
             if (this.VersionReleased != null)
             {
                 char[] delim = { '.' };
                 string[] words = this.VersionReleased.Split(delim);
                 if (words.Length >= 1)
                 {
                     int temp = 0;
                     if (Int32.TryParse(words[0], out temp))
                     {
                         this.versionReleasedMajor = temp;
                         temp = 0;
                         if (words.Length >= 2 && Int32.TryParse(words[1], out temp))
                         {
                             this.versionReleasedMinor = temp;
                             temp = 0;
                             if (words.Length >= 3)
                             {
                                 char[] space = { ' ' };
                                 string[] buildAndMisc = words[2].Split(space);
                                 if (buildAndMisc.Length >= 1 && Int32.TryParse(buildAndMisc[0], out temp))
                                 {
                                     this.versionReleasedBuild = temp;
                                 }
                             }
                         }
                     }
                 }
                 char[] spaceArray = { ' ' };
                 words = this.VersionReleased.Split(spaceArray);
                 if (words.Length >= 2)
                 {
                     string tempMisc = "";
                     for (int i = 1; i < words.Length; i++)
                     {
                         if (i > 1)
                         {
                             tempMisc += " ";
                         }
                         tempMisc += words[i];
                     }
                     this.versionReleasedMisc = tempMisc;
                 }
             }
             if (this.VersionClosed != null)
             {
                 char[] delim = { '.' };
                 string[] words = this.VersionClosed.Split(delim);
                 if (words.Length >= 1)
                 {
                     int temp = 0;
                     if (Int32.TryParse(words[0], out temp))
                     {
                         this.versionClosedMajor = temp;
                         temp = 0;
                         if (words.Length >= 2 && Int32.TryParse(words[1], out temp))
                         {
                             this.versionClosedMinor = temp;
                             temp = 0;
                             if (words.Length >= 3)
                             {
                                 char[] space = { ' ' };
                                 string[] buildAndMisc = words[2].Split(space);
                                 if (buildAndMisc.Length >= 1 && Int32.TryParse(buildAndMisc[0], out temp))
                                 {
                                     this.versionClosedBuild = temp;
                                 }
                             }
                         }
                     }
                 }
                 char[] spaceArray = { ' ' };
                 words = this.VersionClosed.Split(spaceArray);
                 if (words.Length >= 2)
                 {
                     string tempMisc = "";
                     for (int i = 1; i < words.Length; i++)
                     {
                         if (i > 1)
                         {
                             tempMisc += " ";
                         }
                         tempMisc += words[i];
                     }
                     this.versionClosedMisc = tempMisc;
                 }
             }
             //String tempStringValue = "";
             //int tempInt = -1;
             ////Version Planned
             //if (!String.IsNullOrEmpty(this.VersionPlanned))
             //{
             //    for (int i = 0; i < this.VersionPlanned.Count(); i++)
             //    {
             //        if (this.VersionPlanned[i] != '.' && this.VersionPlanned[i] != ' ')
             //        {
             //            tempStringValue = tempStringValue + this.VersionPlanned[i];
             //        }
             //        else if ((this.VersionPlanned[i] == '.' || this.VersionPlanned[i] == ' ' || i == this.VersionPlanned.Count()) && Int32.TryParse(tempStringValue, out tempInt))
             //        {
             //            if (this.versionPlannedMajor == null)
             //            {
             //                this.versionPlannedMajor = tempInt;
             //            }
             //            else if (this.versionPlannedMinor == null)
             //            {
             //                this.versionPlannedMinor = tempInt;
             //            }
             //            else if (this.versionPlannedBuild == null)
             //            {
             //                this.versionPlannedBuild = tempInt;
             //            }
             //            tempStringValue = "";
             //        }
             //    }
             //}
             //if(tempStringValue != "")
             //{
             //    this.versionPlannedMisc = tempStringValue;
             //    tempStringValue = "";
             //}
             //tempStringValue = "";
             ////Version Released
             //if (!String.IsNullOrEmpty(this.VersionReleased))
             //{
             //    for (int i = 0; i < this.VersionReleased.Count(); i++)
             //    {
             //        if (this.VersionReleased[i] != '.' && this.VersionReleased[i] != ' ')
             //        {
             //            tempStringValue = tempStringValue + this.VersionReleased[i];
             //        }
             //        else if ((this.VersionReleased[i] == '.' || this.VersionReleased[i] == ' ' || i == this.VersionReleased.Count()) && Int32.TryParse(tempStringValue, out tempInt))
             //        {
             //            if (this.versionReleasedMajor == null)
             //            {
             //                this.versionReleasedMajor = tempInt;
             //            }
             //            else if (this.versionReleasedMinor == null)
             //            {
             //                this.versionReleasedMinor = tempInt;
             //            }
             //            else if (this.versionReleasedBuild == null)
             //            {
             //                this.versionReleasedBuild = tempInt;
             //            }
             //            tempStringValue = "";
             //        }
             //        else
             //        {
             //            this.versionReleasedMisc = tempStringValue;
             //            tempStringValue = "";
             //        }
             //    }
             //}
             //if (tempStringValue != "")
             //{
             //    this.versionReleasedMisc = tempStringValue;
             //    tempStringValue = "";
             //}
             //tempStringValue = "";
             ////Version Closed
             //if (!String.IsNullOrEmpty(this.VersionClosed))
             //{
             //    for (int i = 0; i < this.VersionClosed.Count(); i++)
             //    {
             //        if (this.VersionClosed[i] != '.' && this.VersionClosed[i] != ' ')
             //        {
             //            tempStringValue = tempStringValue + this.VersionClosed[i];
             //        }
             //        else if ((this.VersionClosed[i] == '.' || this.VersionClosed[i] == ' ' || i == this.VersionClosed.Count()) && Int32.TryParse(tempStringValue, out tempInt))
             //        {
             //            if (this.versionClosedMajor == null)
             //            {
             //                this.versionClosedMajor = tempInt;
             //            }
             //            else if (this.versionClosedMinor == null)
             //            {
             //                this.versionClosedMinor = tempInt;
             //            }
             //            else if (this.versionClosedBuild == null)
             //            {
             //                this.versionClosedBuild = tempInt;
             //            }
             //            tempStringValue = "";
             //        }
             //        else
             //        {
             //            this.versionClosedMisc = tempStringValue;
             //            tempStringValue = "";
             //        }
             //    }
             //}
             //if (tempStringValue != "")
             //{
             //    this.versionClosedMisc = tempStringValue;
             //    tempStringValue = "";
             //}
         }
     */
        //ST_RELATED
        public int Add(bool Corrolate)
        {
            bool exists = false;
            int state = 1;
            bool changed = false;

            RelatedREA toAdd = new RelatedREA(-1, Convert.ToInt32(this.RelatedInputID), Corrolate);
            exists = toAdd.getFields();

            if (!exists)
            {
                //Does not exists
                state = -1;
            }
            if (this.TrackingId == this.RelatedInputID)
            {
                // same as the SCRNumber
                state = -2;
            }
            else
            {
                foreach (RelatedREA SCR in this.RelatedREAIDs)
                {

                    if ((SCR.otherTrackingId == this.RelatedInputID) && (SCR.correlated == Corrolate))
                    {
                        //has already been addeded
                        state = 0;
                    }
                    else if ((SCR.otherTrackingId == this.RelatedInputID) && (SCR.correlated != Corrolate))
                    {
                        //Changed from previous state to new state
                        SCR.correlated = Corrolate;
                        changed = true;
                    }
                }
                if (state > 0 && !changed)
                {
                    //adding
                    this.RelatedREAIDs.Add(toAdd);
                    if (this.RemoveRelatedID.Contains(toAdd.RelatedID))
                    {
                        this.RemoveRelatedID.RemoveAll(x => x == toAdd.RelatedID);
                    }
                }
            }
            return state;
        }
        //ST_ATTACHMENTS
        public int Upload()
        {
            //1     = Uploaded
            //0     = File too large
            //-1    = File is missing
            //-2    = Description is missing

            int MAX_SIZE_BYTES = (100 * 1000000); //100 MB maximum size
            int value = 0;

            HttpContext context = HttpContext.Current;
            if (context.Session["Files-" + Convert.ToString(this.TrackingId)] != null && ((List<SCRAttachments>)context.Session["Files-" + Convert.ToString(this.TrackingId)]).Count() > 0)
            {
                this.Files = (List<SCRAttachments>)context.Session["Files-" + Convert.ToString(this.TrackingId)];
            }
            if (this.File[0] != null)
            {
                if (this.File[0].ContentLength < MAX_SIZE_BYTES)
                {
                    if (this.FileDescription != null && this.FileDescription.Trim() != null && this.FileDescription.Trim() != "")
                    {
                        this.Files.Add(new SCRAttachments(this.File[0], this.FileDescription));
                        value = 1;//Successful
                    }
                    else
                    {
                        value = -2;//missing description
                    }
                }
                //file too large
                else
                {
                    this.UploadErrorMessage = "File is too large. File limit is ~100 MB";
                }
            }
            else
            {
                value = -1;//missing file
            }
            context.Session["Files-" + Convert.ToString(this.TrackingId)] = this.Files;
            return value;
        }
        //Remove
        public void RemoveFiles(string button)
        {
            HttpContext context = HttpContext.Current;
            List<SCRAttachments> temp = (List<SCRAttachments>)context.Session["Files-" + Convert.ToString(this.TrackingId)];
            //if (context.Session["Files-" + Convert.ToString(this.TrackingId)] != null && ((List<SCRAttachments>)context.Session["Files-" + Convert.ToString(this.TrackingId)]).Count() > 0)
            //{
            //temp 
            //}
            for (int i = 0; i < this.Files.Count; i++)
            {
                if (this.Files[i].remove)
                {
                    if (temp[i].fileID != 0)
                    {
                        RemoveFileID.Add(temp[i].fileID);
                    }
                    temp.RemoveAt(i);
                    this.Files.RemoveAt(i);
                    i--;
                }
            }
            this.Files = temp;
            context.Session["Files-" + Convert.ToString(this.TrackingId)] = temp;
        }
        public void RemoveRelated(string button)
        {
            int i = getIndexToRemove(button);
            if (i != -1)
            {
                //if (RelatedREAIDs[i].RelatedID != null)
                //{
                this.RemoveRelatedID.Add(RelatedREAIDs[i].RelatedID);
                //}
                RelatedREAIDs.RemoveAt(i);
            }
        }
        private int getIndexToRemove(string button)
        {
            string[] words = button.Split('-');
            int value = 0;
            if (!Int32.TryParse(words[words.Length - 1], out value))
            {
                value = -1;
            }
            return value;
        }

        //Inserting values into st_code_review
        public bool SaveCodeReview()
        {
            bool success = false;
            try
            {
                REATrackerDB db = new REATrackerDB();
                int codeReviewID = db.CreateCodeReview(TrackingId, CodeReviewTitle, FileList, CodeLocation, User_id, Notes);
                if (codeReviewID > 0)
                {
                    success = true;
                    //EmailSendCodeReviewRequest email = new EmailSendCodeReviewRequest(codeReviewID);
                    //email.Send(); //dont care about the return
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }
        //UPDATING ST_CODE_REVIEW
        public bool UpdateCodeReview(int trackingId)
        {
            bool success = false;
            REATrackerDB db = new REATrackerDB();
            success = db.UpdateCodeReview(this.EditCodeReviewId, this.EditCodeReviewTitle, this.EditCodeLocation, this.EditNotes, this.EditFileList);
            TrackingId = trackingId;
            return success;
        }

        // GET CODE REVIEW HISTORY 
        public void getCodeReview(int SCR_id)
        {
            REATrackerDB sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            List<dynamic> ReviewersList = new List<dynamic>();
            using (System.Data.DataTable dt = sql.GetCodeReview(SCR_id))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].TrackingID = Convert.ToInt32(dt.Rows[i]["TRACKING_ID"]);
                    list[i].CreatedBy = this.getFullName(Convert.ToInt32(dt.Rows[i]["CREATED_BY"] == DBNull.Value ? -2 : dt.Rows[i]["CREATED_BY"]));
                    list[i].CreatedOn = dt.Rows[i]["CREATED_ON"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[i]["CREATED_ON"]).ToString("MM/dd/yyyy HH:mm:ss") : "";
                    list[i].Title = Convert.ToString(dt.Rows[i]["TITLE"]);
                    list[i].fileList = Convert.ToString(dt.Rows[i]["FILE_LIST"]);
                    string file = list[i].fileList;
                    String[] files = file.Split('|');
                    list[i].FileList = string.Join(System.Environment.NewLine, files);
                    list[i].codeLocation = Convert.ToString(dt.Rows[i]["CODE_LOCATION"]);
                    list[i].Notes = Convert.ToString(dt.Rows[i]["NOTES"]);
                    list[i].Status = (dt.Rows[i]["STATUS"]);
                    list[i].CodeReviewId = Convert.ToInt32(dt.Rows[i]["ID"]);
                    list[i].IsCreator = sql.IsCreator(list[i].CodeReviewId, User_id);
                    list[i].Reviewers = "";
                    List<dynamic> users = sql.GetReviewers(list[i].CodeReviewId);
                    foreach (var item in users)
                    {
                        REATrackerDB tempSql = new REATrackerDB();
                        REATrackerDB.ReviewStatus iconValue = tempSql.GetReviewStatus(list[i].CodeReviewId, item.UsersId);
                        if (iconValue == REATrackerDB.ReviewStatus.Approved)
                        {
                            list[i].Reviewers += "<i title='Approved' class='fa qvi-color-medium fa-check-circle'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                        else if (iconValue == REATrackerDB.ReviewStatus.ApprovedWithComments)
                        {
                            list[i].Reviewers += "<i title='Approved With Comments' class='fa qvi-color-low fa-comment-o'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                        else if (iconValue == REATrackerDB.ReviewStatus.NeedsWork)
                        {
                            list[i].Reviewers += "<i title='Needs Work' class='fa qvi-color-critical fa-times-circle'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                        else
                        {
                            list[i].Reviewers += "<i title='Accepted' class='fa fa-question-circle'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                    }
                    list[i].Reviewers += "";
                    i = i + 1;
                }
            }
            this.CodeReviews = list;
        }



        //Save

        public bool Save()
        {
            DateTime NOW = DateTime.Now;
            EditREA OldModel = new EditREA(this.User_id, this.AccountType, this.TrackingId);
            OldModel.init(); //TODO: double check
            REATrackerDB sql = new REATrackerDB();
            sql.OpenConnection();
            bool value = false;
            bool sendEmailAssign = false;
            string LocalUpdateSCR = "UPDATE ST_TRACK SET ROW_VER = ROW_VER ";
            SqlCommand EditCommand = sql.CreateSqlCommand();
            EditCommand.CommandText += LocalUpdateSCR;
            //As close to a 1 to 1 replica of the orginal
            //               ============================================================
            //             || Process Edit Record                                        ||
            //               ============================================================

            //Title
            if (this.Title != OldModel.Title)
            {//Add to Update SCR
                if (!String.IsNullOrEmpty(this.Title))
                {
                    EditCommand.CommandText += ",TITLE = @TITLE";
                    EditCommand.Parameters.AddWithValue("@TITLE", this.Title);
                    //Create a History instance
                    value = true;
                    SqlParameter Description = new SqlParameter();
                    Description.SqlDbType = SqlDbType.VarChar;
                    Description.ParameterName = "DESCRIPTION";
                    Description.Value = "SCR title was changed from \'" + OldModel.Title + "\' to \'" + this.Title + "\'.";
                    sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_note, this.User_id, NOW, Description, null);
                }
            }
            //Product
            if (this.productId.HasValue && (this.productId != OldModel.productId))
            {
                EditCommand.CommandText += ",PRODUCT = @PRODUCT";
                EditCommand.Parameters.AddWithValue("@PRODUCT", this.productId);
                value = true;
                //Get the name of the products
                string oldProductName = Convert.ToString(sql.ProcessScalarCommand("SELECT NAME FROM ST_PRODUCT WHERE (PRODUCT_ID = " + Convert.ToString((int)OldModel.productId) + " );"));
                string newProductName = Convert.ToString(sql.ProcessScalarCommand("SELECT NAME FROM ST_PRODUCT WHERE (PRODUCT_ID = " + Convert.ToString((int)this.productId) + " );"));
                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = "Product was changed from \'" + oldProductName + "\' to \'" + newProductName + "\'.";
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_product, this.User_id, NOW, Description, null);
            }
            //Version Found
            if (this.versionFoundMajor != OldModel.versionFoundMajor && this.versionFoundMajor != null)
            {
                EditCommand.CommandText += ",VF_MAJOR = @VF_MAJOR";
                EditCommand.Parameters.AddWithValue("@VF_MAJOR", this.versionFoundMajor == null ? (object)DBNull.Value : this.versionFoundMajor);
            }

            if (this.versionFoundMinor != OldModel.versionFoundMinor && this.versionFoundMinor != null)
            {
                EditCommand.CommandText += ",VF_MINOR = @VF_MINOR";
                EditCommand.Parameters.AddWithValue("@VF_MINOR", this.versionFoundMinor == null ? (object)DBNull.Value : this.versionFoundMinor);
            }

            if (this.versionFoundBuild != OldModel.versionFoundBuild && this.versionFoundBuild != null)
            {
                EditCommand.CommandText += ",VF_BUILD = @VF_BUILD";
                EditCommand.Parameters.AddWithValue("@VF_BUILD", this.versionFoundBuild == null ? (object)DBNull.Value : this.versionFoundBuild);
            }

            if (this.versionFoundMisc != OldModel.versionFoundMisc && !(this.versionFoundMisc == null && OldModel.versionFoundMisc == ""))
            {
                EditCommand.CommandText += ",VF_MISC = @VF_MISC";
                EditCommand.Parameters.AddWithValue("@VF_MISC", this.versionFoundMisc == null ? (object)DBNull.Value : this.versionFoundMisc);
            }

            //Issue Type

            if (this.issueType.HasValue && (this.issueType != OldModel.issueType))
            {
                EditCommand.CommandText += ",ISSUE_TYPE = @ISSUE_TYPE";
                EditCommand.Parameters.AddWithValue("@ISSUE_TYPE", this.issueType == null ? (object)DBNull.Value : this.issueType);
                value = true;
                string oldName = Convert.ToString(sql.ProcessScalarCommand("SELECT NAME FROM ST_ISSUE_TYPE WHERE (ISSUE_ID = " + Convert.ToString((int)OldModel.issueType) + " );"));
                string newName = Convert.ToString(sql.ProcessScalarCommand("SELECT NAME FROM ST_ISSUE_TYPE WHERE (ISSUE_ID = " + Convert.ToString((int)this.issueType) + " );"));
                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = "SCR issue type was changed from " + oldName + " to " + newName + ".";
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_issuetype, this.User_id, NOW, Description, null);
            }

            //Priority
            if (this.priority.HasValue && (this.priority != OldModel.priority))
            {
                EditCommand.CommandText += ",priority = @priority";
                EditCommand.Parameters.AddWithValue("@priority", this.priority == null ? (object)DBNull.Value : this.priority);
                value = true;
                string oldName = Convert.ToString(sql.ProcessScalarCommand("SELECT NAME FROM ST_priority WHERE (ID = " + Convert.ToString((int)OldModel.priority) + " );"));
                string newName = Convert.ToString(sql.ProcessScalarCommand("SELECT NAME FROM ST_priority WHERE (ID = " + Convert.ToString((int)this.priority) + " );"));
                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = "SCR Priority was changed from " + oldName + " to " + newName + ".";
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_priority, this.User_id, NOW, Description, null);
            }

            //Status change
            if (this.statusId != OldModel.statusId)
            {
                value = true;
                EditCommand.CommandText += ",STATUS = @STATUS";
                EditCommand.Parameters.AddWithValue("@STATUS", this.statusId == null ? (object)DBNull.Value : this.statusId);
                REATrackerDB db = new REATrackerDB(); //TODO: check
                HttpContext context = HttpContext.Current;
                string localMessage = "";
                if (this.statusId == (int)REATrackerDB.StatusChangeID.status_submitted)
                {
                    localMessage = "Set to Submitted by " + Convert.ToString(context.Session["CurrentUserName"]);
                }
                else if (this.statusId == (int)REATrackerDB.StatusChangeID.status_approved)
                {
                    localMessage = "Set to Approved by " + Convert.ToString(context.Session["CurrentUserName"]);
                    //Aproved On and By
                    EditCommand.CommandText += ",APPROVED_ON = @APPROVED_ON";
                    EditCommand.Parameters.AddWithValue("@APPROVED_ON", NOW);
                    EditCommand.CommandText += ",APPROVED_BY = @APPROVED_BY";
                    EditCommand.Parameters.AddWithValue("@APPROVED_BY", Convert.ToInt32(context.Session["st_userID"]));
                }
                else if (this.statusId == (int)REATrackerDB.StatusChangeID.status_deferred)
                {
                    localMessage = "Set to Defferred by " + Convert.ToString(context.Session["CurrentUserName"]);
                }
                else if (this.statusId == (int)REATrackerDB.StatusChangeID.status_rejected)
                {
                    localMessage = "Set to Rejected by " + Convert.ToString(context.Session["CurrentUserName"]);
                }
                else if (this.statusId == (int)REATrackerDB.StatusChangeID.status_inprocess)
                {
                    localMessage = "Set to In Process by " + Convert.ToString(context.Session["CurrentUserName"]);
                }
                else if (this.statusId == (int)REATrackerDB.StatusChangeID.status_fixed)
                {
                    localMessage = "Set to Fixed by " + Convert.ToString(context.Session["CurrentUserName"]);
                }
                else if (this.statusId == (int)REATrackerDB.StatusChangeID.status_delivered)
                {
                    db.SetCodeReviewStatus((int)TrackingId, (int)REATrackerDB.CodeReviewStatus.Closed);
                    localMessage = "Set to Delivered by " + Convert.ToString(context.Session["CurrentUserName"]);
                }
                else if (this.statusId == (int)REATrackerDB.StatusChangeID.status_testing)
                {
                    localMessage = "Set to Testing  by " + Convert.ToString(context.Session["CurrentUserName"]);
                }
                else if (this.statusId == (int)REATrackerDB.StatusChangeID.status_closed)
                {
                    db.SetCodeReviewStatus((int)TrackingId, (int)REATrackerDB.CodeReviewStatus.Closed);
                    localMessage = "Set to Closed by " + Convert.ToString(context.Session["CurrentUserName"]);
                }
                else if (this.statusId == (int)REATrackerDB.StatusChangeID.status_onhold)
                {
                    localMessage = "Set to On Hold  by " + Convert.ToString(context.Session["CurrentUserName"]);
                }
                else if (this.statusId == (int)REATrackerDB.StatusChangeID.status_acknowledged)
                {
                    localMessage = "Set to Acknowledged  by " + Convert.ToString(context.Session["CurrentUserName"]);
                }
                else if (this.statusId == (int)REATrackerDB.StatusChangeID.status_pending)
                {
                    localMessage = "Set to Pending  by " + Convert.ToString(context.Session["CurrentUserName"]);
                }
                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = localMessage;
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_status, this.User_id, NOW, Description, null);
            }

            if (this.closedReasonId != OldModel.closedReasonId)
            {
                EditCommand.CommandText += ",CLOSED_REASON = @CLOSED_REASON";
                EditCommand.Parameters.AddWithValue("@CLOSED_REASON", this.closedReasonId == null ? (object)DBNull.Value : this.closedReasonId);
            }

            if (this.closedByID != OldModel.closedByID)
            {
                EditCommand.CommandText += ",CLOSED_BY = @CLOSED_BY";
                EditCommand.Parameters.AddWithValue("@CLOSED_BY", this.closedByID == null ? (object)DBNull.Value : this.closedByID);
                EditCommand.CommandText += ",CLOSED_ON = @CLOSED_ON";
                EditCommand.Parameters.AddWithValue("@CLOSED_ON", this.closedByID == null ? (object)DBNull.Value : NOW);
            }

            //Closed Version
            if (this.versionClosedMajor != OldModel.versionClosedMajor)
            {
                EditCommand.CommandText += ",VC_MAJOR = @VC_MAJOR";
                EditCommand.Parameters.AddWithValue("@VC_MAJOR", this.versionClosedMajor == null ? (object)DBNull.Value : this.versionClosedMajor);
            }
            if (this.versionClosedMinor != OldModel.versionClosedMinor)
            {
                EditCommand.CommandText += ",VC_MINOR = @VC_MINOR";
                EditCommand.Parameters.AddWithValue("@VC_MINOR", this.versionClosedMinor == null ? (object)DBNull.Value : this.versionClosedMinor);
            }
            if (this.versionClosedBuild != OldModel.versionClosedBuild)
            {
                EditCommand.CommandText += ",VC_BUILD = @VC_BUILD";
                EditCommand.Parameters.AddWithValue("@VC_BUILD", this.versionClosedBuild == null ? (object)DBNull.Value : this.versionClosedBuild);
            }
            if (this.versionClosedMisc != OldModel.versionClosedMisc)
            {
                EditCommand.CommandText += ",VC_MISC = @VC_MISC";
                EditCommand.Parameters.AddWithValue("@VC_MISC", this.versionClosedMisc == null ? (object)DBNull.Value : this.versionClosedMisc);
            }


            if (this.moduleId != OldModel.moduleId)
            {
                EditCommand.CommandText += ",MODULE_ID = @MODULE_ID";
                EditCommand.Parameters.AddWithValue("@MODULE_ID", this.moduleId == null ? (object)DBNull.Value : this.moduleId);
            }

            //Document Change
            if (this.isDocumentChange != OldModel.isDocumentChange)
            {
                EditCommand.CommandText += ",REQ_DOC_CHANGE = @REQ_DOC_CHANGE";
                EditCommand.Parameters.AddWithValue("@REQ_DOC_CHANGE", this.isDocumentChange == null ? (object)DBNull.Value : this.isDocumentChange);
            }

            //Database Change
            if (this.isDatabaseChange != OldModel.isDatabaseChange)
            {
                EditCommand.CommandText += ",REQ_DB_CHANGE = @REQ_DB_CHANGE";
                EditCommand.Parameters.AddWithValue("@REQ_DB_CHANGE", this.isDatabaseChange == null ? (object)DBNull.Value : this.isDatabaseChange);
            }

            //Requirements Change
            if (this.isRequirementsChange != OldModel.isRequirementsChange)
            {
                EditCommand.CommandText += ",REQ_REQ_DOC = @REQ_REQ_DOC";
                EditCommand.Parameters.AddWithValue("@REQ_REQ_DOC", this.isRequirementsChange == null ? (object)DBNull.Value : this.isRequirementsChange);
            }

            //Customer Bug
            if (this.customerBug != OldModel.customerBug)
            {
                EditCommand.CommandText += ",CUSTOMER_BUG = @CUSTOMER_BUG";
                EditCommand.Parameters.AddWithValue("@CUSTOMER_BUG", this.customerBug == null ? (object)DBNull.Value : this.customerBug);
            }

            //Customer Compnay
            if (this.customerCompany != OldModel.customerCompany)
            {
                EditCommand.CommandText += ",CUSTOMER_COMPANY = @CUSTOMER_COMPANY";
                EditCommand.Parameters.AddWithValue("@CUSTOMER_COMPANY", this.customerCompany == null ? (object)DBNull.Value : this.customerCompany);
            }

            //Customer Location
            if (this.customerLocation != OldModel.customerLocation)
            {
                EditCommand.CommandText += ",CUSTOMER_LOCATION = @CUSTOMER_LOCATION";
                EditCommand.Parameters.AddWithValue("@CUSTOMER_LOCATION", this.customerLocation == null ? (object)DBNull.Value : this.customerLocation);
            }

            //Customer Name or contact person
            if (this.customerName != OldModel.customerName)
            {
                EditCommand.CommandText += ",CUSTOMER_NAME = @CUSTOMER_NAME";
                EditCommand.Parameters.AddWithValue("@CUSTOMER_NAME", this.customerName == null ? (object)DBNull.Value : this.customerName);
            }

            //Syteline
            if (this.Syteline != OldModel.Syteline)
            {
                EditCommand.CommandText += ",SYTELINE_REF = @SYTELINE";
                EditCommand.Parameters.AddWithValue("@SYTELINE", this.Syteline == null ? (object)DBNull.Value : this.Syteline);
            }
            //Machine SN
            if (this.MachineSN != OldModel.MachineSN && !(this.MachineSN == null && OldModel.MachineSN == ""))
            {
                EditCommand.CommandText += ",MACHINE_SN = @MACHINE_SN";
                EditCommand.Parameters.AddWithValue("MACHINE_SN", this.MachineSN == null ? (object)DBNull.Value : this.MachineSN);
            }

            if (this.issueType == (int)REATrackerDB.IssueChangeType.Enhancements)
            {
                //Enhancement Problem
                if (this.problem != OldModel.problem)
                {
                    EditCommand.CommandText += ",PROBLEM = @PROBLEM";
                    EditCommand.Parameters.AddWithValue("@PROBLEM", this.problem == null ? (object)DBNull.Value : this.problem);

                    SqlParameter Description = new SqlParameter();
                    Description.SqlDbType = SqlDbType.VarChar;
                    Description.ParameterName = "DESCRIPTION";
                    Description.Value = ("SCR problem was changed from '" + OldModel.problem + "' to '" + this.problem + "'.");
                    sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_ser_problem, this.User_id, NOW, Description, null);
                }

                //Enhancement Solution
                if (this.solution != OldModel.solution)
                {
                    EditCommand.CommandText += ",SOLUTION = @SOLUTION";
                    EditCommand.Parameters.AddWithValue("@SOLUTION", this.solution == null ? (object)DBNull.Value : this.solution);

                    SqlParameter Description = new SqlParameter();
                    Description.SqlDbType = SqlDbType.VarChar;
                    Description.ParameterName = "DESCRIPTION";
                    Description.Value = ("SCR solution was changed from '" + OldModel.solution + "' to '" + this.solution + "'.");
                    sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_ser_solution, this.User_id, NOW, Description, null);
                }

                //Enhancement Benifit
                if (this.benifits != OldModel.benifits)
                {
                    EditCommand.CommandText += ",BENEFITS = @BENEFITS";
                    EditCommand.Parameters.AddWithValue("@BENEFITS", this.benifits == null ? (object)DBNull.Value : this.benifits);

                    SqlParameter Description = new SqlParameter();
                    Description.SqlDbType = SqlDbType.VarChar;
                    Description.ParameterName = "DESCRIPTION";
                    Description.Value = ("SCR benefits was changed from '" + OldModel.benifits + "' to '" + this.benifits + "'.");
                    sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_ser_benefits, this.User_id, NOW, Description, null);
                }
            }
            if (this.issueType == (int)REATrackerDB.IssueChangeType.PlannedWork)
            {
                //Planned Work Background / Rational
                if (this.problem != OldModel.problem)
                {
                    EditCommand.CommandText += ",PROBLEM = @PROBLEM";
                    EditCommand.Parameters.AddWithValue("@PROBLEM", this.problem == null ? (object)DBNull.Value : this.problem);

                    SqlParameter Description = new SqlParameter();
                    Description.SqlDbType = SqlDbType.VarChar;
                    Description.ParameterName = "DESCRIPTION";
                    Description.Value = ("The Background was changed from '" + OldModel.problem + "' to '" + this.problem + "'.");
                    sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_ser_problem, this.User_id, NOW, Description, null);
                }

                //Planned Work Story
                if (this.solution != OldModel.solution)
                {
                    EditCommand.CommandText += ",SOLUTION = @SOLUTION";
                    EditCommand.Parameters.AddWithValue("@SOLUTION", this.solution == null ? (object)DBNull.Value : this.solution);

                    SqlParameter Description = new SqlParameter();
                    Description.SqlDbType = SqlDbType.VarChar;
                    Description.ParameterName = "DESCRIPTION";
                    Description.Value = ("The Story was changed from '" + OldModel.solution + "' to '" + this.solution + "'.");
                    sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_ser_solution, this.User_id, NOW, Description, null);
                }

                //Planned Work Acceptance Criteria
                if (this.benifits != OldModel.benifits)
                {
                    EditCommand.CommandText += ",BENEFITS = @BENEFITS";
                    EditCommand.Parameters.AddWithValue("@BENEFITS", this.benifits == null ? (object)DBNull.Value : this.benifits);

                    SqlParameter Description = new SqlParameter();
                    Description.SqlDbType = SqlDbType.VarChar;
                    Description.ParameterName = "DESCRIPTION";
                    Description.Value = ("The Acceptance Criteria was changed from '" + OldModel.benifits + "' to '" + this.benifits + "'.");
                    sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_ser_benefits, this.User_id, NOW, Description, null);
                }
            }

            //this here is just for changing the resolved by person cause it was set by accident, only admins can do this (in theory)
            bool statusHasChanged = (this.statusId != OldModel.statusId);
            bool resolvedByHasChanged = (this.resolvedByID != OldModel.resolvedByID);
            if (!statusHasChanged)
            {
                if ((resolvedByHasChanged) && (OldModel.resolvedByID != null) && (this.resolvedByID != null))
                {
                    EditCommand.CommandText += ",RESOLVED_BY = @RESOLVED_BY";
                    EditCommand.Parameters.AddWithValue("@RESOLVED_BY", this.resolvedByID);
                }
            }
            //first test to see if we are changing the status
            else if (OldModel.statusId < (int)REATrackerDB.StatusChangeID.status_fixed)
            {
                //in theory, a developer is switching it to fixed or delivered
                if ((this.statusId == (int)REATrackerDB.StatusChangeID.status_fixed) ||
                    (this.statusId == (int)REATrackerDB.StatusChangeID.status_delivered))
                {
                    //the status changed, then so set the resolved by
                    EditCommand.CommandText += ",RESOLVED_BY = @RESOLVED_BY";
                    EditCommand.Parameters.AddWithValue("@RESOLVED_BY", this.User_id);
                    EditCommand.CommandText += ",RESOLVED_ON = @RESOLVED_ON";
                    EditCommand.Parameters.AddWithValue("@RESOLVED_ON", NOW);
                    if (this.versionPlannedMajor is null)   //if there is no planned version, set it to found version
                    {
                        if (this.versionReleasedMajor is null == false)    //but only if it's not null
                        {
                            EditCommand.CommandText += ",VP_MAJOR = @VP_MAJOR";
                            EditCommand.Parameters.AddWithValue("@VP_MAJOR", this.versionReleasedMajor);
                        }
                        if (this.versionReleasedMinor is null == false)
                        {
                            EditCommand.CommandText += ",VP_MINOR = @VP_MINOR";
                            EditCommand.Parameters.AddWithValue("@VP_MINOR", this.versionReleasedMinor);
                        }
                        if (this.versionReleasedBuild is null == false)
                        {
                            EditCommand.CommandText += ",VP_BUILD = @VP_BUILD";
                            EditCommand.Parameters.AddWithValue("@VP_BUILD", this.versionReleasedBuild);
                        }
                        if (this.versionReleasedMisc is null == false)
                        {
                            EditCommand.CommandText += ",VP_MISC = @VP_MISC";
                            EditCommand.Parameters.AddWithValue("@VP_MISC", this.versionReleasedMisc);
                        }
                    }
                }
                else if (this.statusId > (int)REATrackerDB.StatusChangeID.status_delivered)
                {
                    //the user is setting it to built, testing, on hold, close
                    //there is no "resolved by" field cause this is the first time, so we are using the current user
                    if (OldModel.resolvedByID == null)
                    {
                        EditCommand.CommandText += ",RESOLVED_BY = @RESOLVED_BY";
                        EditCommand.Parameters.AddWithValue("@RESOLVED_BY", this.User_id);
                        EditCommand.CommandText += ",RESOLVED_ON = @RESOLVED_ON";
                        EditCommand.Parameters.AddWithValue("@RESOLVED_ON", NOW);
                    }
                    else
                    {
                        //the resolve was already set, so dont overwrite
                    }
                }
                else
                {
                    //status has changed but dont need to mark the SCR resolved
                }
            }
            else
            {
                //old status was fixed or greater
            }

            //Hours Specified
            if (this.actualHourstoSpecify != OldModel.actualHourstoSpecify)
            {
                value = true;
                /*if (this.actualHourstoSpecify == 0
                    &&
                    this.statusId != 1 &&
                    this.statusId != 2 &&
                    this.statusId != 7 
                    )
                {
                    this.actualHourstoSpecify = null;
                }*/
                EditCommand.CommandText += ",ACT_HOURS_TO_SPEC = @ACT_HOURS_TO_SPEC";
                EditCommand.Parameters.AddWithValue("@ACT_HOURS_TO_SPEC", this.actualHourstoSpecify == null ? (object)DBNull.Value : this.actualHourstoSpecify);
                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = Convert.ToString(this.actualHourstoSpecify);
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_actual_to_spec, this.User_id, NOW, Description, null);
            }
            if (this.plannedHourstoSpecify != OldModel.plannedHourstoSpecify)
            {
                value = true;
                EditCommand.CommandText += ",EST_HOURS_TO_SPEC = @EST_HOURS_TO_SPEC";
                EditCommand.Parameters.AddWithValue("@EST_HOURS_TO_SPEC", this.plannedHourstoSpecify == null ? (object)DBNull.Value : this.plannedHourstoSpecify);
                value = true;

                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = Convert.ToString(this.plannedHourstoSpecify);
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_estimate_to_spec, this.User_id, NOW, Description, null);
            }
            //Hours Test
            if (this.actualHourstoTest != OldModel.actualHourstoTest)
            {
                value = true;

                EditCommand.CommandText += ",ACT_HOURS_TO_TEST = @ACT_HOURS_TO_TEST";
                EditCommand.Parameters.AddWithValue("@ACT_HOURS_TO_TEST", this.actualHourstoTest == null ? (object)DBNull.Value : this.actualHourstoTest);
                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = Convert.ToString(this.actualHourstoTest);
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_actual_to_test, this.User_id, NOW, Description, null);
            }
            if (this.plannedHourstoTest != OldModel.plannedHourstoTest)
            {
                value = true;

                EditCommand.CommandText += ",EST_HOURS_TO_TEST = @EST_HOURS_TO_TEST";
                EditCommand.Parameters.AddWithValue("@EST_HOURS_TO_TEST", this.plannedHourstoTest == null ? (object)DBNull.Value : this.plannedHourstoTest);
                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = Convert.ToString(this.plannedHourstoTest);
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_estimate_to_test, this.User_id, NOW, Description, null);
            }
            //Hours Fixed
            if (this.actualHourstoFix != OldModel.actualHourstoFix)
            {

                EditCommand.CommandText += ",ACT_HOURS_TO_FIX = @ACT_HOURS_TO_FIX";
                EditCommand.Parameters.AddWithValue("@ACT_HOURS_TO_FIX", this.actualHourstoFix == null ? (object)DBNull.Value : this.actualHourstoFix);
                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = Convert.ToString(this.actualHourstoFix);
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_actual_to_fix, this.User_id, NOW, Description, null);
            }
            if (this.plannedHourstoFix != OldModel.plannedHourstoFix)
            {

                EditCommand.CommandText += ",EST_HOURS_TO_FIX = @EST_HOURS_TO_FIX";
                EditCommand.Parameters.AddWithValue("@EST_HOURS_TO_FIX", this.plannedHourstoFix == null ? (object)DBNull.Value : this.plannedHourstoFix);
                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = Convert.ToString(this.plannedHourstoFix);
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_estimate_to_fix, this.User_id, NOW, Description, null);
            }
            //DO THE MAJOR, MINOR, BUILD, AND MISC FOR EACH
            //Version Planned
            if (canEditversionPlanned)
            {
                bool hasChanged = false;
                //can the user actually change the Planned Version
                if (this.versionPlannedMajor != OldModel.versionPlannedMajor)
                {
                    hasChanged = true;
                    EditCommand.CommandText += ",VP_MAJOR = @VP_MAJOR";
                    EditCommand.Parameters.AddWithValue("@VP_MAJOR", this.versionPlannedMajor == null ? (object)DBNull.Value : this.versionPlannedMajor);
                }
                if (this.versionPlannedMinor != OldModel.versionPlannedMinor)
                {
                    hasChanged = true;
                    EditCommand.CommandText += ",VP_MINOR = @VP_MINOR";
                    EditCommand.Parameters.AddWithValue("@VP_MINOR", this.versionPlannedMinor == null ? (object)DBNull.Value : this.versionPlannedMinor);
                }
                if (this.versionPlannedBuild != OldModel.versionPlannedBuild)
                {
                    hasChanged = true;
                    EditCommand.CommandText += ",VP_BUILD = @VP_BUILD";
                    EditCommand.Parameters.AddWithValue("@VP_BUILD", this.versionPlannedBuild == null ? (object)DBNull.Value : this.versionPlannedBuild);
                }
                if (this.versionPlannedMisc != OldModel.versionPlannedMisc)
                {
                    hasChanged = true;
                    EditCommand.CommandText += ",VP_MISC = @VP_MISC";
                    EditCommand.Parameters.AddWithValue("@VP_MISC", this.versionPlannedMisc == null ? (object)DBNull.Value : this.versionPlannedMisc);
                }

                if (hasChanged)
                {
                    SqlParameter Description = new SqlParameter();
                    Description.SqlDbType = SqlDbType.VarChar;
                    Description.ParameterName = "DESCRIPTION";
                    Description.Value =
                        (this.versionPlannedMajor == null ? "?" : Convert.ToString(this.versionPlannedMajor)) + "." +
                        (this.versionPlannedMinor == null ? "?" : Convert.ToString(this.versionPlannedMinor)) + "." +
                        (this.versionPlannedBuild == null ? "?" : Convert.ToString(this.versionPlannedBuild)) +
                        (this.versionPlannedMisc == null ? "" : " " + Convert.ToString(this.versionPlannedMisc));
                    sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_planned_release, this.User_id, NOW, Description, null);
                    value = true;
                }
            }

            //DO THE MAJOR, MINOR, BUILD, AND MISC FOR EACH
            //Version Released
            if (canEditversionReleased) //do you have permission to edit it
            {
                //now check the status change, so you are developer marking it fixed or delivered
                //or a tester kicking it back
                bool allowChange = (this.statusId >= (int)REATrackerDB.StatusChangeID.status_fixed)
                                    ||
                                    ((OldModel.statusId >= (int)REATrackerDB.StatusChangeID.status_fixed) &&
                                    (this.statusId <= (int)REATrackerDB.StatusChangeID.status_inprocess));

                //if you are manager or admin, doesnt matter you can change it
                if (this.AccessType == 5)
                {
                    allowChange = true;
                }
                if (allowChange)
                {
                    bool hasChanged = false;
                    if (this.versionReleasedMajor != OldModel.versionReleasedMajor)
                    {
                        hasChanged = true;
                        EditCommand.CommandText += ",VR_MAJOR = @VR_MAJOR";
                        EditCommand.Parameters.AddWithValue("@VR_MAJOR", this.versionReleasedMajor == null ? (object)DBNull.Value : this.versionReleasedMajor);
                    }
                    if (this.versionReleasedMinor != OldModel.versionReleasedMinor)
                    {
                        hasChanged = true;
                        EditCommand.CommandText += ",VR_MINOR = @VR_MINOR";
                        EditCommand.Parameters.AddWithValue("@VR_MINOR", this.versionReleasedMinor == null ? (object)DBNull.Value : this.versionReleasedMinor);
                    }
                    if (this.versionReleasedBuild != OldModel.versionReleasedBuild)
                    {
                        hasChanged = true;
                        EditCommand.CommandText += ",VR_BUILD = @VR_BUILD";
                        EditCommand.Parameters.AddWithValue("@VR_BUILD", this.versionReleasedBuild == null ? (object)DBNull.Value : this.versionReleasedBuild);
                    }
                    if (this.versionReleasedMisc != OldModel.versionReleasedMisc)
                    {
                        hasChanged = true;
                        EditCommand.CommandText += ",VR_MISC = @VR_MISC";
                        EditCommand.Parameters.AddWithValue("@VR_MISC", this.versionReleasedMisc == null ? (object)DBNull.Value : this.versionReleasedMisc);
                    }

                    if (hasChanged)
                    {
                        SqlParameter Description = new SqlParameter();
                        Description.SqlDbType = SqlDbType.VarChar;
                        Description.ParameterName = "DESCRIPTION";
                        Description.Value =
                            (this.versionReleasedMajor == null ? "?" : Convert.ToString(this.versionReleasedMajor)) + "." +
                            (this.versionReleasedMinor == null ? "?" : Convert.ToString(this.versionReleasedMinor)) + "." +
                            (this.versionReleasedBuild == null ? "?" : Convert.ToString(this.versionReleasedBuild)) +
                            (this.versionReleasedMisc == null ? "" : " " + Convert.ToString(this.versionReleasedMisc));
                        sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_actual_release, this.User_id, NOW, Description, null);
                        value = true;
                    }
                }
            }
            EditCommand.CommandText += ", LAST_UPDATE = GETDATE()";
            //Resolution or notes
            if (this.Details != null && this.Details.Trim() != "" && this.statusId >= (int)REATrackerDB.StatusChangeID.status_fixed && OldModel.statusId <= (int)REATrackerDB.StatusChangeID.status_inprocess)
            {
                value = true;
                //resolution
                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = this.Details == null ? (object)DBNull.Value : this.Details;
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_resolution, this.User_id, NOW, Description, null);
            }
            else if (this.Details != null && this.Details.Trim() != "")
            {
                value = true;
                //notes
                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = this.Details == null ? (object)DBNull.Value : this.Details;
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_note, this.User_id, NOW, Description, null);
            }

            //Exeception
            if (this.statusId < 0)
            {
                value = true;
                HttpContext context = HttpContext.Current;
                string oldName = Convert.ToString(sql.ProcessScalarCommand("SELECT NAME FROM ST_STATUS WHERE (STATUS_ID= " + Convert.ToString((int)OldModel.statusId) + " );"));
                string newName = Convert.ToString(sql.ProcessScalarCommand("SELECT NAME FROM ST_STATUS WHERE (STATUS_ID= " + Convert.ToString((int)this.statusId) + " );"));
                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = Convert.ToString(context.Session["CurrentUserName"]) + " used an exception state from " + oldName + " to " + newName + ".";
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_exception, this.User_id, NOW, Description, null);
            }
            //Assigned To
            if (OldModel.statusId != (int)REATrackerDB.StatusChangeID.status_closed && this.statusId == (int)REATrackerDB.StatusChangeID.status_closed)
            {
                value = true;
                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = "Assigned to nobody";
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_assignto, this.User_id, NOW, Description, null);
                this.assignedToId = null;
            }

            if (this.assignedToId != OldModel.assignedToId
                &&
                    (this.assignedToId != -1
                    //&& OldModel.assignedToId != null
                    )
                )
            {
                if (this.assignedToId != null)
                {
                    value = true;
                    String fullname = "";
                    DataTable dt = sql.ProcessCommand("SELECT FIRST_NAME, LAST_NAME FROM ST_USERS WHERE (USER_ID= " + Convert.ToString((int)this.assignedToId) + " );");
                    if (dt.Rows.Count > 0)
                    {
                        fullname = Convert.ToString(dt.Rows[0][0]) + " " + Convert.ToString(dt.Rows[0][1]);
                    }
                    SqlParameter Description = new SqlParameter();
                    Description.SqlDbType = SqlDbType.VarChar;
                    Description.ParameterName = "DESCRIPTION";
                    Description.Value = "Assigned to " + fullname;
                    if ((OldModel.statusId == (int)REATrackerDB.StatusChangeID.status_fixed && this.statusId == (int)REATrackerDB.StatusChangeID.status_delivered))
                    {
                        sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_assignto, -1, NOW, Description, null);

                    }
                    else
                    {
                        sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_assignto, this.User_id, NOW, Description, null);

                    }

                    sendEmailAssign = true;

                    DataTable history
                        = sql.ProcessCommand(
                        "SELECT HISTORY_ID,TRACKING_ID,DESCRIPTION FROM ST_HISTORY WHERE (TRACKING_ID=" + this.TrackingId + ") ORDER BY HISTORY_ID ASC");

                }
                EditCommand.CommandText += ",ASSIGNED_TO = @ASSIGNED_TO";
                EditCommand.Parameters.AddWithValue("@ASSIGNED_TO", this.assignedToId == null ? (object)DBNull.Value : this.assignedToId);
            }

            if (this.Rank != OldModel.Rank)
            {
                EditCommand.CommandText += ",RANK_ORDER = @RANK_ORDER";
                EditCommand.Parameters.AddWithValue("@RANK_ORDER", this.Rank == null ? (object)DBNull.Value : this.Rank);

                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = "Rank has been changed from " + ((OldModel.Rank == null) ? "nothing" : OldModel.Rank.ToString()) + " to " + ((this.Rank == null) ? "nothing" : this.Rank.ToString());
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_rank, this.User_id, NOW, Description, null);
            }
            if (this.Size != OldModel.Size)
            {
                EditCommand.CommandText += ",SIZE = @SIZE";
                EditCommand.Parameters.AddWithValue("@SIZE", this.Size == null ? (object)DBNull.Value : this.Size);

                SqlParameter Description = new SqlParameter();
                Description.SqlDbType = SqlDbType.VarChar;
                Description.ParameterName = "DESCRIPTION";
                Description.Value = "Size has been changed from " + ((OldModel.Size == null) ? "nothing" : OldModel.Size.ToString()) + " to " + ((this.Size == null) ? "nothing" : this.Size.ToString());
                sql.CreateHistory(this.TrackingId, (int)REATrackerDB.HistoryChangeType.chg_size, this.User_id, NOW, Description, null);
            }

            if (this.canEditManagersNote)
            {
                if (this.ManagersNote != null)
                {
                    this.ManagersNote = this.ManagersNote.Trim();
                }
                if (this.ManagersNote != OldModel.ManagersNote)
                {
                    EditCommand.CommandText += ",MANAGERS_NOTE = @MANAGERS_NOTE";
                    EditCommand.Parameters.AddWithValue("@MANAGERS_NOTE", this.ManagersNote == null ? (object)DBNull.Value : this.ManagersNote);
                }
            }

            EditCommand.CommandText += (" WHERE TRACKING_ID = " + Convert.ToString(this.TrackingId) + ";");
            //               ============================================================
            //             || Execute changes                                            ||
            //               ============================================================

            EditCommand.ExecuteNonQuery();
            sql.CloseConnection();
            this.ProcessAttachments();
            this.ProcessRelatedREA();

            //Email things, kick back the SCR if the new status goes to in process from testing/closed/onhold/built
            if (this.statusId.HasValue && OldModel.statusId.HasValue)
            {
                if (
                    (
                        (REATrackerDB.StatusChangeID)this.statusId == REATrackerDB.StatusChangeID.status_inprocess
                    )
                    &&
                    (
                        ((REATrackerDB.StatusChangeID)OldModel.statusId == REATrackerDB.StatusChangeID.status_testing) ||
                        ((REATrackerDB.StatusChangeID)OldModel.statusId == REATrackerDB.StatusChangeID.status_closed) ||
                        ((REATrackerDB.StatusChangeID)OldModel.statusId == REATrackerDB.StatusChangeID.status_onhold) ||
                        ((REATrackerDB.StatusChangeID)OldModel.statusId == REATrackerDB.StatusChangeID.status_built)
                    )
                   )
                {

                    //EmailSCRKickback mail = new EmailSCRKickback(TrackingId, productId);
                    //mail.Send();
                }
            }

            if (this.statusId == (int)REATrackerDB.StatusChangeID.status_delivered && OldModel.statusId != (int)REATrackerDB.StatusChangeID.status_delivered) // if it is delivered
            {
                //EmailDeliveredNotification mail = new EmailDeliveredNotification(this.TrackingId);
                //mail.Send();
            }
            else if (sendEmailAssign && (this.assignedToId != this.User_id))
            {
                //new EmailChangedAssignmentNotification(this.TrackingId).Send();
            }
            if (OldModel.productId != this.productId)
            {//If product id changed
                //EmailSCRProductChange emailValidator = new EmailSCRProductChange(this.TrackingId, (Int32)OldModel.productId);
                //emailValidator.Send();
            }
            return value;
        }
        private void ProcessAttachments()
        {
            REATrackerDB sql = new REATrackerDB();
            sql.OpenConnection();
            //Remove all Files from list
            String DeleteStatement = "DELETE FROM ST_ATTACHMENT WHERE FILE_ID = ";

            foreach (int FileID in this.RemoveFileID)
            {
                sql.UpdateDeletedFileHistorys(this.TrackingId, FileID, this.User_id);
                sql.ProcessCommand(DeleteStatement + Convert.ToString(FileID));

            }

            sql.CloseConnection();
            //Add new Files loaded
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["btracker"].ConnectionString);
            conn.Open();
            if (this.Files != null)
            {
                foreach (SCRAttachments item in this.Files)
                {
                    if (item.file != null)
                    {
                        //Setting up the Byte value for the image
                        /*
                        byte[] uploadedFile = new byte[item.file.InputStream.Length];
                        item.file.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                         */
                        byte[] uploadedFile = item.Data;//new byte[item.file.InputStream.Length];
                        /*
                        using (Stream inputStream = item.file.InputStream)
                        {
                            MemoryStream memoryStream = inputStream as MemoryStream;
                            if (memoryStream == null)
                            {
                                memoryStream = new MemoryStream();
                                inputStream.CopyTo(memoryStream);
                            }
                            uploadedFile = memoryStream.ToArray();
                        }*/
                        //creating the SQL parameter
                        SqlParameter fileParam = new SqlParameter();
                        fileParam.SqlDbType = SqlDbType.Image;
                        fileParam.ParameterName = "FILE";
                        fileParam.Value = uploadedFile;
                        SqlParameter fileDes = new SqlParameter();
                        fileDes.SqlDbType = SqlDbType.VarChar;
                        fileDes.ParameterName = "DESCRIPTION";
                        fileDes.Value = item.Description;
                        //adding file type here
                        SqlParameter fileType = new SqlParameter();
                        fileType.SqlDbType = SqlDbType.Int;
                        fileType.ParameterName = "TYPE";
                        fileType.Value = REATrackerDB.HistoryChangeType.chg_attachment_add;

                        //Create the string 
                        String sqlString =
                            "EXEC InsertAttachFiles " + Convert.ToString(this.User_id) + "," + Convert.ToString(this.TrackingId) + ",'" +
                                System.IO.Path.GetFileName(item.file.FileName).Replace("'", "''") + "'," +
                            " @DESCRIPTION" + "," + item.file.InputStream.Length.ToString() + "," +
                            " @FILE" + "," + Convert.ToString(this.User_id) + "," + "@TYPE";
                        //creating the sql command
                        SqlCommand Command = new SqlCommand(sqlString, conn);
                        Command.Parameters.Add(fileParam);
                        Command.Parameters.Add(fileDes);
                        Command.Parameters.Add(fileType);
                        //execute
                        Command.ExecuteNonQuery();
                        Command.Dispose();
                    }
                }
            }

            conn.Close();
        }
        private void ProcessRelatedREA()
        {
            String sqlString = "DELETE FROM ST_RELATED WHERE RELATED_ID = ";
            REATrackerDB sql = new REATrackerDB();
            sql.OpenConnection();
            //Remove all RelatedREA from List
            foreach (int RelatedREAid in this.RemoveRelatedID)
            {
                sql.ProcessCommand(sqlString + RelatedREAid);
            }
            sql.CloseConnection();
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["btracker"].ConnectionString);
            conn.Open();
            //Add new Related SCRs
            foreach (RelatedREA item in this.RelatedREAIDs)
            {//ADD
                if (item.RelatedID == 0)
                {
                    SqlParameter Title = new SqlParameter();
                    Title.SqlDbType = SqlDbType.VarChar;
                    Title.ParameterName = "TITLE";
                    Title.Value = item.title.ToString();
                    string sqlStringRelated = "EXEC InsertRelated " +
                        this.User_id.ToString() + ", " +
                        Convert.ToString(item.otherTrackingId) + ", " +
                        Convert.ToString(this.TrackingId) + ", " +
                        "@TITLE" + ";";
                    SqlCommand Command = new SqlCommand(sqlStringRelated, conn);
                    Command.Parameters.Add(Title);
                    //Execute
                    Command.ExecuteNonQuery();
                    Command.Dispose();
                    if (item.correlated)
                    {//CORRELATE
                        SqlParameter CorrelatedTitle = new SqlParameter();
                        CorrelatedTitle.SqlDbType = SqlDbType.VarChar;
                        CorrelatedTitle.ParameterName = "TITLE";
                        CorrelatedTitle.Value = this.Title;
                        //item.title.ToString();
                        string CorrelatedsqlString = "EXEC InsertRelated " +
                            this.User_id.ToString() + ", " +
                            Convert.ToString(this.TrackingId) + ", " +
                            Convert.ToString(item.otherTrackingId) + ", " +
                            "@TITLE" + ";";
                        SqlCommand CorrelatedCommand = new SqlCommand(CorrelatedsqlString, conn);
                        CorrelatedCommand.Parameters.Add(CorrelatedTitle);
                        //Execute
                        CorrelatedCommand.ExecuteNonQuery();
                        CorrelatedCommand.Dispose();
                    }
                }
            }
            conn.Close();
        }

        private String getPriorityString(int argsPriority)
        {
            String Results = "";
            switch (argsPriority)
            {
                case 1:
                    Results = "Low";
                    break;
                case 2:
                    Results = "Medium";
                    break;
                case 3:
                    Results = "High";
                    break;
                case 4:
                    Results = "Critical";
                    break;
            }
            return Results;
        }
        private String getIssueTypeString(int argsType)
        {
            return "";
        }
        private String getStatusString(int argsStatus)
        {
            return "";
        }

        //  ==========================================================================
        //||    Classes                                                              ||  
        //  ==========================================================================
        public class RelatedREA
        {
            public int otherTrackingId { get; set; }
            public int thisSCRID { get; set; }
            public string product { get; set; }
            public string title { get; set; }
            public string issue { get; set; }
            public Boolean correlated { get; set; }
            public string Status { get; set; }
            public String Priority { get; set; }
            public String AssignTo { get; set; }
            public String PlannedRelease { get; set; }
            public String VersionReleased { get; set; }
            public string ClosedBy { get; set; }
            public string ClosedReason { get; set; }
            public bool remove { get; set; }

            public int RelatedID { get; set; }
            //-----------------------------------------------------------------------------
            // Constructors                                                               ||
            //-----------------------------------------------------------------------------
            public RelatedREA()
            {
            }
            public RelatedREA(int SCRID, int TrackID, bool correlate)
            {
                this.otherTrackingId = TrackID;
                this.thisSCRID = SCRID;
                this.correlated = correlate;
                this.AssignTo = getAssignedTo(TrackID);

            }
            public bool getFields()
            {
                bool exists = false;
                string cmd =
                    "SELECT REA_TRACK.TITLE, REA_STATUS.NAME AS STATUS, REA_priority.NAME, REA_PRODUCT.NAME AS PRODUCT, " +
                    "USER_CLOSED_BY.FIRST_NAME + ' ' + USER_CLOSED_BY.LAST_NAME AS CLOSED_BY_NAME, " +
                    "USER_CLOSED_BY.USER_ID AS CLOSED_BY_ID, REA_CLOSED_REASON.NAME AS CLOSED_REASON " +
                    "FROM REA_TRACK " +
                    "INNER JOIN REA_PRODUCT ON REA_TRACK.PRODUCT = REA_PRODUCT.PRODUCT_ID " +
                    "INNER JOIN REA_priority ON REA_TRACK.priority = REA_priority.ID " +
                    "INNER JOIN REA_STATUS ON REA_TRACK.STATUS = REA_STATUS.STATUS_ID " +
                    "LEFT OUTER JOIN BTRACKER_NEW.dbo.ST_USERS USER_CLOSED_BY ON REA_TRACK.CLOSED_BY = USER_CLOSED_BY.USER_ID " +
                    "LEFT OUTER JOIN REA_CLOSED_REASON ON REA_TRACK.CLOSED_REASON = REA_CLOSED_REASON.REASON_ID " +
                    "WHERE REA_TRACK.TRACKING_ID = " +
                    Convert.ToString(otherTrackingId);
                REATrackerDB sql = new REATrackerDB();
                sql.OpenConnection();
                DataTable dt = sql.ProcessCommand(cmd);
                if (dt.Rows.Count != 0)
                {
                    exists = true;
                    this.title = Convert.ToString(dt.Rows[0]["TITLE"]);
                    this.Status = Convert.ToString(dt.Rows[0]["STATUS"]);
                    this.product = Convert.ToString(dt.Rows[0]["PRODUCT"]);
                    this.ClosedBy = Convert.ToString(dt.Rows[0]["CLOSED_BY_NAME"]);
                    this.ClosedReason = Convert.ToString(dt.Rows[0]["CLOSED_REASON"]);
                }
                sql.CloseConnection();
                return exists;
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
            public string getRemoveValue(int i)
            {
                return "RemoveRelated-" + i;
            }
        }

        public class SCRAttachments
        {
            public HttpPostedFileBase file { get; set; }
            public String FileName { get; set; }
            public Byte[] Data { get; set; }
            public String Description { get; set; }
            public DateTime Datetime { get; set; }
            public int fileID { get; set; }
            public bool remove { get; set; }
            //-----------------------------------------------------------------------------
            // Constructors                                                               ||
            //-----------------------------------------------------------------------------
            public SCRAttachments() { }
            public SCRAttachments(HttpPostedFileBase fileParam, String des)
            {
                this.file = fileParam;
                this.Description = des;
                MemoryStream target = new MemoryStream();
                this.file.InputStream.CopyTo(target);
                this.Data = target.ToArray();
                this.FileName = System.IO.Path.GetFileName(fileParam.FileName);
            }
            public SCRAttachments(HttpPostedFileBase fileParam, String des, int id)
            {
                this.file = fileParam;
                this.Description = des;
                this.fileID = id;
                MemoryStream target = new MemoryStream();
                this.file.InputStream.CopyTo(target);
                this.Data = target.ToArray();
                this.FileName = System.IO.Path.GetFileName(fileParam.FileName);
            }

            public SCRAttachments(Byte[] DataParam, String name, String des, int id)
            {
                this.Description = des;
                this.fileID = id;
                this.Data = DataParam;
                this.FileName = System.IO.Path.GetFileName(name);
            }

            public String getSize()
            {

                string display = "";
                double displaySize;

                int size = 0;
                if (Data != null)
                {
                    size = (file == null ? Data.Length : file.ContentLength);
                }
                if (size < 1000)
                {
                    displaySize = size;
                    display = displaySize.ToString("F1") + " B";
                }
                else if (size < 1000000)
                {
                    displaySize = size / 1000;
                    display = displaySize.ToString("F1") + " KB";
                }
                else if (size < 1000000000)
                {
                    displaySize = size / 1000000;
                    display = displaySize.ToString("F1") + " MB";
                }
                return display;
            }
        }
    }
}