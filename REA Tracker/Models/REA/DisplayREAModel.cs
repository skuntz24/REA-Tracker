using System;
using System.Collections.Generic;
using System.Data;
using QVICommonIntranet.Database;
//using QVICommonIntranet.Email.EmailSCRTracker;

namespace REA_Tracker.Models
{
    public class DisplayREA
    {
        private REATrackerDB _sql = new REATrackerDB();

        /// <summary>
        /// UserID of the current user of the system
        /// </summary>
        public int UserID { get; set; }
        public string ASPUserID { get; set; }
        public string AccountType { get; set; }
        public bool HasAccess { get; private set; }
        /// <summary>
        /// The ID of the current SCR that to be displayed
        /// </summary>
        public int TrackingId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsWatching { get; private set; }
        public bool IsAuthor { get; private set; }
        public bool IsAdmin { get; set; }
        public bool IsManagerOrLeader { get; protected set; }
        public bool CanCreateCodeReview { get; protected set; }
        /// <summary>
        /// Allows certain sections to be displayed base on permissions
        /// </summary>
        public bool display { get; set; }
        public string Product { get; set; }
        public string Retired { get; set; }
        public string Closed { get; set; }
        public string Title { get; set; }
        public string Module { get; set; }
        public string VersionFound { get; set; }
        public string Issue { get; set; }
        public int IssueID { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public int StatusID { get; set; }
        public string Priority { get; set; }
        public string ReqDocChange { get; set; }
        public string ReqReqDoc { get; set; }
        public string ReqDBChange { get; set; }
        public string CustomerBug { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCompany { get; set; }
        public string CustomerLocation { get; set; }
        public string SyteLine { get; set; }
        public string Machine { get; set; }
        public string ManagersNote { get; set; }
        public string EstSpec { get; set; }
        public string ActSpec { get; set; }
        public string EstFix { get; set; }
        public string ActFix { get; set; }
        public string VersionPlanned { get; set; }
        public string VersionReleased { get; set; }
        public string EstTest { get; set; }
        public string ActTest { get; set; }
        public string Rank { get; set; }
        public string Size { get; set; }
        public string SubmittedBy { get; set; }
        public int SubmittedByID { get; set; }
        public string AssignedTo { get; set; }
        public int AssignedToID { get; set; }
        public string AssignedBy { get; set; }
        public string AssignedOn { get; set; }
        public string ResolvedBy { get; set; }
        public int ResolvedByID { get; set; }
        public string ClosedBy { get; set; }
        public string plannedRelease { get; set; }
        public string RetiredOn { get; set; }
        public string SubmittedOn { get; set; }
        public string SubmittedTo { get; set; }
        public string ApprovedOn { get; set; }
        public string ResolvedOn { get; set; }
        public string ClosedOn { get; set; }
        public string ClosedReason { get; set; }
        public string ClosedReasonTitle { get; set; }
        public string VersionClosed { get; set; }
        public string Problem { get; set; }
        public string Benifits { get; set; }
        public string Solution { get; set; }
        public string ApprovedBy { get; set; }
        public String WatchingString { get; set; }
        public bool canEditKeywords { get; set; }
        public string BuildList { get; set; }

        //New Code Review
        public string CodeReviewTitle { get; set; }
        public string FileList { get; set; }
        public string CodeLocation { get; set; }
        public string Notes { get; set; }

        //Display Lists of Items
        public List<dynamic> Related { get; set; }
        public List<dynamic> History { get; set; }
        public List<dynamic> Attached { get; set; }
        public List<dynamic> Keywords { get; set; }
        public List<dynamic> CodeReviews { get; set; }
        public List<dynamic> Reviewers { get; set; }
        //public List<dynamic> Watching { get; set; }
        public DisplayREA()
        {
            this.canEditKeywords = false;
            this.IsManagerOrLeader = false;
            this.IsAdmin = false;
            this.display = false;
            this.AccountType = "";
            this.HasAccess = false;
            this.CanCreateCodeReview = false;
        }

        public void Initialize()
        {
            GetREADetails();
            GetWatchList();

            //access permissions
            DetermineAccessRights();

            if (this.StatusID == (int)(REATrackerDB.StatusChangeID.status_fixed))
            {
                if (this.IsAdmin || this.IsManagerOrLeader || (this.ResolvedByID == this.UserID))
                {
                    this.CanCreateCodeReview = true;
                }
            }
            else if (this.StatusID == (int)(REATrackerDB.StatusChangeID.status_inprocess))
            {
                if (this.IsAdmin || this.IsManagerOrLeader || (this.AssignedToID == this.UserID))
                {
                    this.CanCreateCodeReview = true;
                }
            }


            //default title
            this.CodeReviewTitle = $"Code Review for SCR#{this.TrackingId} - {this.Title}";
        }
        private bool DetermineProductAccess()
        {
            bool hasProductAccess = false;

            if (_sql.IsActiveUser(this.UserID))
            {
                DataTable dt = _sql.GetProductList(this.UserID);
                //check the st_user's product access (meaning its in your list)
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Convert.ToString(dt.Rows[i][1]) == this.Product)
                    {
                        hasProductAccess = true;
                        break;
                    }
                }
            }

            if (!hasProductAccess)
            {
                //check if the st_product_access has permissions
                string text = $@"SELECT ST_PRODUCT_ACCESS.Id FROM ST_PRODUCT_ACCESS 
                                            INNER JOIN ST_PRODUCT ON  ST_PRODUCT.PRODUCT_ID = ST_PRODUCT_ACCESS.ProductID 
                                            WHERE ST_PRODUCT.NAME = '{this.Product}' AND ST_PRODUCT_ACCESS.UserID = {this.UserID};";
                DataTable stdt = _sql.ProcessCommand(text);
                //Checking full access
                if (stdt.Rows.Count > 0 || _sql.hasFullAccess(this.ASPUserID))
                {
                    hasProductAccess = true;
                }
            }
            return hasProductAccess;
        }

        private void DetermineAccessRights()
        {
            canEditKeywords = AccountType.Equals("Administrator") || AccountType.Equals("Power User");

            this.IsAdmin = AccountType.Equals("Administrator");
            if (this.IsAdmin || this.IsAuthor)
            {
                this.HasAccess = true;
            }

            //still dont have access
            if (!this.HasAccess)
            {
                bool hasProductAccess = DetermineProductAccess();
                //the user has access to the product
                if (hasProductAccess)
                {
                    //if they are on the Team (but not as a user or vendor)
                    string getTeam =
                        $@"SELECT ST_PRODUCT_TEAM.ROLE FROM ST_PRODUCT_TEAM 
                                    INNER JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_PRODUCT_TEAM.PRODUCT 
                                    WHERE ST_PRODUCT_TEAM.USER_ID = {this.UserID} AND ST_PRODUCT.NAME = '{this.Product}';";
                    DataTable teamDT = _sql.ProcessCommand(getTeam);

                    //has access and the SCR is not planned work, or is not a Power User
                    if (!this.Issue.Equals("Planned Work") || this.AccountType.Equals("Power User"))
                    {
                        this.HasAccess = true;
                    }
                    else if (teamDT.Rows.Count > 0 && (Convert.ToInt32(teamDT.Rows[0]["ROLE"]) != 1 || Convert.ToInt32(teamDT.Rows[0]["ROLE"]) != 2)) //user (2), vendor (1)
                    {
                        this.HasAccess = true;
                    }

                    if (teamDT.Rows.Count > 0)
                    {
                        int role = Convert.ToInt32(teamDT.Rows[0]["ROLE"]);
                        this.IsManagerOrLeader = ((role == 5) || (role == 7));
                    }

                }
            }
/*
            bool hasAccess = false;
            DataTable dt = _sql.GetProductList(this.UserID);
            //check the st_user's product access
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToString(dt.Rows[i][1]) == this.Product)
                {
                    hasAccess = true;
                }
            }
            //check if the st_product_access has permissions
            string text = $@"SELECT ST_PRODUCT_ACCESS.Id FROM ST_PRODUCT_ACCESS 
                                            INNER JOIN ST_PRODUCT ON  ST_PRODUCT.PRODUCT_ID = ST_PRODUCT_ACCESS.ProductID 
                                            WHERE ST_PRODUCT.NAME = '{this.Product}' AND ST_PRODUCT_ACCESS.UserID = {this.UserID};";
            DataTable stdt = _sql.ProcessCommand(text);
            //Checking full access
            if (stdt.Rows.Count > 0 || _sql.hasFullAccess(this.ASPUserID))
            {
                hasAccess = true;
            }

            //if the user is not a Vendor or User
            bool access = false;
            string getTeam =
                $@"SELECT ST_PRODUCT_TEAM.ROLE FROM ST_PRODUCT_TEAM 
                                    INNER JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_PRODUCT_TEAM.PRODUCT 
                                    WHERE ST_PRODUCT_TEAM.USER_ID = {this.UserID} AND ST_PRODUCT.NAME = '{this.Product}';";
            DataTable teamDT = _sql.ProcessCommand(getTeam);
            //is user the author
            if (this.SubmittedByID == this.UserID)
            {
                access = true;
            }
            //has access and the product is not planned work, or is not a Power User
            else if (hasAccess != false)
            {
                if (!this.Issue.Equals("Planned Work") || this.AccountType.Equals("Power User"))
                {
                    access = true;
                }
                else if (teamDT.Rows.Count > 0 && (Convert.ToInt32(teamDT.Rows[0]["ROLE"]) != 1 || Convert.ToInt32(teamDT.Rows[0]["ROLE"]) != 2))
                {
                    access = true;
                }
            }
            this.display = access;
            */
        }
        protected void GetREADetails()
        {
            //Get REA Properties
            GetREARecord();
            GetApprovedBy();
            GetOldEnhancementRequest();
            GetRelatedREAs();
            GetHistory();
            GetAttachments();
            GetKeywords();
            GetCodeReviews();
            GetREABuilds();
        }
        
        private void GetApprovedBy()
        {
            if (String.IsNullOrEmpty(this.ApprovedBy) || String.IsNullOrEmpty(this.ApprovedOn))
            {
                REATrackerDB _sql = new REATrackerDB();
                String FullName = "";
                String cmdText =
                    $@"SELECT (ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME) AS 'FULL_NAME', ST_HISTORY.MADE_ON 
                    FROM ST_TRACK 
                    INNER JOIN ST_HISTORY ON ST_TRACK.TRACKING_ID = ST_HISTORY.TRACKING_ID 
                    INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_HISTORY.MADE_BY 
                    WHERE ST_TRACK.TRACKING_ID = { this.TrackingId} AND ST_HISTORY.DESCRIPTION LIKE '%Set to Approve%' 
                    ORDER BY ST_HISTORY.MADE_ON DESC";
                using (System.Data.DataTable dt = _sql.ProcessCommand(cmdText))
                {
                    if (dt.Rows.Count > 0)
                    {
                        FullName = dt.Rows[0]["FULL_NAME"].ToString();
                        this.ApprovedOn = Convert.ToString(dt.Rows[0]["MADE_ON"]);
                    }
                }
                this.ApprovedBy = FullName;
            }
        }
        //ST_TRACK
        private void GetREARecord()
        {
            String cmdText =
                $@"SELECT REA_TRACK.TRACKING_ID AS TRACKING_ID, REA_PRODUCT.NAME AS PRODUCT, REA_TRACK.TITLE AS TITLE,
                REA_STATUS.NAME AS STATUS_BY_NAME, REA_STATUS.DESCRIPTION AS STATUS_DESCRIPTION, REA_TRACK.STATUS AS STATUS,
                REA_PRIORITY.NAME AS PRIORITY, 
                REA_TRACK.SUBMITTED_BY AS SUBMITTED_BY, 
                REA_TRACK.SUBMITTED_ON AS SUBMITTED_ON, REA_TRACK.ASSIGNED_TO AS ASSIGNED_TO, 
                REA_TRACK.ASSIGNED_BY AS ASSIGNED_BY, REA_TRACK.ASSIGNED_ON AS ASSIGNED_ON, 
                REA_TRACK.RESOLVED_BY AS RESOLVED_BY, REA_TRACK.RESOLVED_ON AS RESOLVED_ON, REA_TRACK.CLOSED_BY AS CLOSED_BY, 
                REA_TRACK.CLOSED_ON AS CLOSED_ON, 
                REA_TRACK.MANAGERS_NOTE 
                FROM REA_TRACK INNER JOIN REA_PRODUCT ON REA_TRACK.PRODUCT = REA_PRODUCT.PRODUCT_ID 
                INNER JOIN REA_STATUS ON REA_TRACK.STATUS = REA_STATUS.STATUS_ID 
                INNER JOIN REA_PRIORITY ON REA_TRACK.priority = REA_PRIORITY.PRIORITY_ID 
                WHERE REA_TRACK.TRACKING_ID = {this.TrackingId}";
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmdText))
            {
                if (dt.Rows.Count > 0)
                {
                    int temp = 0;
                    this.TrackingId = (int)dt.Rows[0]["TRACKING_ID"];
                    this.Product = Convert.ToString(dt.Rows[0]["PRODUCT"]);
                    this.Status = Convert.ToString(dt.Rows[0]["STATUS_BY_NAME"]);
                    this.StatusDescription = Convert.ToString(dt.Rows[0]["STATUS_DESCRIPTION"]);
                    this.StatusID = Convert.ToInt16(dt.Rows[0]["STATUS"]);
                    this.Priority = Convert.ToString(dt.Rows[0]["PRIORITY"]);
                    temp = 0;
                    Int32.TryParse(Convert.ToString(dt.Rows[0]["SUBMITTED_BY"] == DBNull.Value ? -2 : dt.Rows[0]["SUBMITTED_BY"]), out temp);
                    this.SubmittedByID = temp;
                    this.IsAuthor = (this.SubmittedByID == this.UserID);
                    this.SubmittedBy = this.getFullName(temp); //TODO: Fix this
                    temp = 0;
                    this.SubmittedOn = Convert.ToString(dt.Rows[0]["SUBMITTED_ON"]);
                    Int32.TryParse(Convert.ToString(dt.Rows[0]["ASSIGNED_TO"] == DBNull.Value ? -2 : dt.Rows[0]["ASSIGNED_TO"]), out temp);
                    this.AssignedToID = temp;
                    this.AssignedTo = this.getFullName(temp); //TODO: Fix this
                    temp = 0;
                    Int32.TryParse(Convert.ToString(dt.Rows[0]["ASSIGNED_BY"] == DBNull.Value ? -2 : dt.Rows[0]["ASSIGNED_BY"]), out temp);
                    this.AssignedBy = this.getFullName(temp); //TODO: Fix this
                    temp = 0;
                    this.AssignedOn = Convert.ToString(dt.Rows[0]["ASSIGNED_ON"]);
                    Int32.TryParse(Convert.ToString(dt.Rows[0]["RESOLVED_BY"] == DBNull.Value ? -2 : dt.Rows[0]["RESOLVED_BY"]), out temp);
                    this.ResolvedBy = this.getFullName(temp); //TODO: Fix this
                    this.ResolvedByID = temp;
                    temp = 0;
                    this.ResolvedOn = Convert.ToString(dt.Rows[0]["RESOLVED_ON"]);
                    Int32.TryParse(Convert.ToString(dt.Rows[0]["CLOSED_BY"] == DBNull.Value ? -2 : dt.Rows[0]["CLOSED_BY"]), out temp);
                    this.ClosedBy = this.getFullName(temp); //TODO: Fix this
                    temp = 0;
                    this.ClosedOn = Convert.ToString(dt.Rows[0]["CLOSED_ON"]);

                    this.Title = Convert.ToString(dt.Rows[0]["TITLE"]);

                    this.ManagersNote = Convert.ToString(dt.Rows[0]["MANAGERS_NOTE"]);
                }
            }
        }

        /// <summary>
        /// Find all of the official builds that this SCR was built in.
        /// No records means it was never built in an official release
        /// It could have multiple rows return, which means this SCR was in multiple releases probably cause it was kicked back
        /// </summary>
        private void GetREABuilds()
        {
            this.BuildList = "";
            string cmdText = $@"
SELECT
BUILD_ID,
(ISNULL(ST_PRODUCT_RELEASE.MAJOR,'?')+'.'+ISNULL(ST_PRODUCT_RELEASE.MINOR,'?')+'.'+ISNULL(ST_PRODUCT_RELEASE.BUILD,'?')+' '+ISNULL(ST_PRODUCT_RELEASE.MISC,'')) AS 'RELEASE'

FROM ST_PRODUCT_RELEASE WHERE ST_PRODUCT_RELEASE.SCR_LIST LIKE '%{this.TrackingId}%'";
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmdText))
            {
                if (dt.Rows.Count > 0)
                {
                    foreach(DataRow dr in dt.Rows)
                    {
                        //./Home/DashboardReport/BUILD_ID, RELEASE;
                        this.BuildList += $@"{dr[0].ToString()},{dr[1].ToString()};";
                    }
                }
            }
        }
        private string DocumentChangeToString(int x)
        {
            string value = "?";
            switch ((int)x)
            {
                case 0: { value = "No"; break; }
                case 1: { value = "Yes"; break; }
                case 2: { value = "Done"; break; }
                case 3: { value = "TBD"; break; }
            }
            return value;
        }
        private string DisplayClosedBy(string ClosedReason)
        {
            string reason = "";
            int num;
            if (Int32.TryParse(ClosedReason, out num) && num > 0)
            {
                String cmdText = "SELECT ST_CLOSED_REASON.NAME FROM ST_CLOSED_REASON WHERE ST_CLOSED_REASON.REASON_ID = " + ClosedReason + ";";
                using (System.Data.DataTable dt = _sql.ProcessCommand(cmdText))
                {
                    if (dt.Rows.Count > 0)
                    {
                        this.ClosedReasonTitle = Convert.ToString(dt.Rows[0][0]);
                        reason = (" - " + Convert.ToString(dt.Rows[0][0]) + " on " + Convert.ToString(this.VersionClosed));
                    }
                }
            }
            return reason;
        }
        //ST_ATTACHMENTS
        private void GetAttachments()
        {
            List<dynamic> list = new List<dynamic>();
            string cmd = $@"
                SELECT
                    FILE_ID, FILENAME, FILE_SIZE, FILE_DATE, FILE_DATA, ADDED_BY, (ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME) AS 'ADDED_BY_NAME', DESCRIPTION 
                FROM ST_ATTACHMENT 
                LEFT JOIN ST_USERS ON ST_USERS.USER_ID = ST_ATTACHMENT.ADDED_BY 
                WHERE TRACKING_ID = {this.TrackingId}                
                ORDER BY FILE_DATE ASC";
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmd))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].FileName = System.IO.Path.GetFileName(Convert.ToString(dt.Rows[i]["FILENAME"]));
                    list[i].FileSize = displayFileSize(Convert.ToDouble(dt.Rows[i]["FILE_SIZE"]));
                    list[i].FileDate = Convert.ToDateTime(dt.Rows[i]["FILE_DATE"]);
                    list[i].Data = (Byte[])(dt.Rows[i]["FILE_DATA"] == DBNull.Value ? null : dt.Rows[i]["FILE_DATA"]);
                    list[i].IsPicture = false;
                    if (list[i].Data != null)
                    {
                        string filename = list[i].FileName;
                        if (filename.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                            filename.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                            filename.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                        {
                            string base64String = Convert.ToBase64String(list[i].Data, 0, list[i].Data.Length);
                            list[i].ImageUrl = "data:image/png;base64," + base64String;
                            list[i].IsPicture = true;
                        }
                    }
                    else
                    {
                        list[i].ImageUrl = "";
                    }
                    int temp = 0;
                    Int32.TryParse(Convert.ToString(dt.Rows[i]["ADDED_BY"]), out temp);
                    list[i].AddedByID = temp;
                    list[i].AddedBy = Convert.ToString(dt.Rows[i]["ADDED_BY_NAME"]);
                    list[i].Description = Convert.ToString(dt.Rows[i]["DESCRIPTION"]);
                    list[i].id = Convert.ToInt32(dt.Rows[i]["FILE_ID"]);
                    i++;
                }
            }
            this.Attached = list;
        }
        private string displayFileSize(double size)
        {
            string display = "";
            double displaySize;

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
        //Keywords
        private void GetKeywords()
        {
            List<dynamic> list = new List<dynamic>();
            using (System.Data.DataTable dt = _sql.GetKeywords(this.TrackingId))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].ID = Convert.ToString(row["ID"]);
                    list[i].Name = Convert.ToString(row["KEYWORD"]);
                    list[i].Description = Convert.ToString(row["DESCRIPTION"]);
                    i++;
                }
            }
            this.Keywords = list;
        }
        //ST_RELATED
        private void GetRelatedREAs()
        {
            List<dynamic> list = new List<dynamic>();
            string cmd = $@"
                SELECT 
                    REA_RELATED.REA_ID, REA_PRODUCT.NAME AS PRODUCT_NAME, REA_TRACK.TITLE, REA_STATUS.NAME AS STATUS_NAME, REA_PRIORITY.NAME AS PRIORITY_NAME,
                    (USER_ASSIGNED_TO.FIRST_NAME + ' ' + USER_ASSIGNED_TO.LAST_NAME) AS ASSIGNED_TO_NAME,
                    USER_ASSIGNED_TO.USER_ID AS ASSIGNED_TO_ID,
                    (USER_CLOSED_BY.FIRST_NAME + ' ' + USER_CLOSED_BY.LAST_NAME) AS CLOSED_BY_NAME,
                    USER_CLOSED_BY.USER_ID AS CLOSED_BY_ID,
                    ST_TRACK.VP_MAJOR, ST_TRACK.VP_MINOR, ST_TRACK.VP_BUILD, ST_TRACK.VP_MISC,
                    ST_TRACK.VR_MAJOR, ST_TRACK.VR_MINOR, ST_TRACK.VR_BUILD, ST_TRACK.VR_MISC,
                    ST_CLOSED_REASON.NAME AS CLOSED_REASON
                FROM REA_RELATED 
                    INNER JOIN REA_TRACK ON REA_RELATED.REA_ID = REA_TRACK.TRACKING_ID 
                    INNER JOIN REA_PRODUCT ON REA_TRACK.PRODUCT = REA_PRODUCT.PRODUCT_ID 
                    INNER JOIN REA_PRIORITY ON REA_TRACK.PRIORITY = REA_PRIORITY.ID 
                    INNER JOIN REA_STATUS ON REA_TRACK.STATUS = REA_STATUS.STATUS_ID
                    LEFT OUTER JOIN BTRACKER_NEW.dbo.ST_USERS USER_ASSIGNED_TO ON REA_TRACK.ASSIGNED_TO = USER_ASSIGNED_TO.USER_ID
                    LEFT OUTER JOIN BTRACKER_NEW.dbo.ST_USERS USER_CLOSED_BY ON REA_TRACK.CLOSED_BY = USER_CLOSED_BY.USER_ID
                    LEFT OUTER JOIN  REA_CLOSED_REASON ON REA_TRACK.CLOSED_REASON = REA_CLOSED_REASON.REASON_ID
                WHERE ST_RELATED.TRACKING_ID= {this.TrackingId}";
            //ST_USERS.FIRST_NAME, ST_USERS.LAST_NAME
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmd)) //TODO: update sql.GetRelatedREAs with this command????
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].TrackingID = Convert.ToInt32(dt.Rows[i]["REA_ID"]);
                    list[i].Product = Convert.ToString(dt.Rows[i]["PRODUCT_NAME"]);
                    list[i].Title = Convert.ToString(dt.Rows[i]["TITLE"]);
                    list[i].Status = Convert.ToString(dt.Rows[i]["STATUS_NAME"]);
                    list[i].ClosedReason = Convert.ToString(dt.Rows[i]["CLOSED_REASON"]);
                    list[i].Priority = Convert.ToString(dt.Rows[i]["PRIORITY_NAME"]);
                    list[i].AssignedTo = Convert.ToString(dt.Rows[i]["ASSIGNED_TO_NAME"]);
                    list[i].ClosedBy = Convert.ToString(dt.Rows[i]["CLOSED_BY_NAME"]);
                    i++;
                }
            }
            this.Related = list;
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
        //ST_HISTORY   
        private void GetHistory()
        {
            List<dynamic> list = new List<dynamic>();
            string cmd =
             $@"SELECT 
                    ST_HISTORY.TYPE, 
                    ST_HISTORY.MADE_BY, (ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME) AS MADE_BY_NAME, ST_HISTORY.MADE_ON, 
                    ST_HISTORY.DESCRIPTION, ST_HISTORY.FILE_ID
                FROM ST_HISTORY 
                    LEFT JOIN ST_USERS ON ST_HISTORY.MADE_BY = ST_USERS.USER_ID
                WHERE TRACKING_ID= {this.TrackingId}
                 ORDER BY ST_HISTORY.HISTORY_ID DESC";

            using (System.Data.DataTable dt = _sql.ProcessCommand(cmd))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].Type = Convert.ToInt32(dt.Rows[i]["TYPE"]);
                    list[i].MadeBy = Convert.ToString(dt.Rows[i]["MADE_BY_NAME"]);
                    list[i].MadeOn = dt.Rows[i]["MADE_ON"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[i]["MADE_ON"]).ToString("MM/dd/yyyy HH:mm:ss") : "";
                    list[i].fileID = Convert.ToString(dt.Rows[i]["FILE_ID"]);
                    if (dt.Rows[i]["FILE_ID"] != null && Convert.ToString(dt.Rows[i]["FILE_ID"]) != "")
                    {
                        list[i].FileName = getFilename(Convert.ToInt32(list[i].fileID));
                    }
                    if (list[i].Type == (int)REATrackerDB.HistoryChangeType.chg_related)
                    {
                        string scrID = Convert.ToString(getRelatedREAID(Convert.ToInt32(dt.Rows[i]["FILE_ID"])));
                        list[i].LinkSCR = scrID;
                        list[i].Title = getTitle(Convert.ToInt32(scrID));
                    }
                    else
                    {
                        list[i].Description = Convert.ToString(dt.Rows[i]["DESCRIPTION"]);
                    }
                    list[i].MadeById = Convert.ToInt32(dt.Rows[i]["MADE_BY"] == DBNull.Value ? -2 : dt.Rows[i]["MADE_BY"]);
                    list[i].DisplayHtml = false; //Convert.ToBoolean(dt.Rows[i]["DISPLAY_HTML"]);
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
        private string getTitle(int SCRID)
        {
            return Convert.ToString(
                new REATrackerDB().ProcessScalarCommand(
                "SELECT TITLE FROM ST_TRACK WHERE TRACKING_ID = " + SCRID
                )
                );
        }

        //ST_ENHANCEMENT_REQUEST (this table is nolonger used, but needed for older records)
        private void GetOldEnhancementRequest()
        {
            string cmd = $"SELECT ST_ENHANCEMENT_REQUEST.CUSTOMER_COMPANY, ST_ENHANCEMENT_REQUEST.CUSTOMER_LOCATION FROM ST_ENHANCEMENT_REQUEST WHERE TRACKING_ID={this.TrackingId}";
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmd))
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    if (String.IsNullOrEmpty(this.CustomerCompany))
                    {
                        this.CustomerCompany = Convert.ToString(dt.Rows[0]["CUSTOMER_COMPANY"]);
                        this.CustomerBug = "true";
                    }
                    if (String.IsNullOrEmpty(this.CustomerLocation))
                    {
                        this.CustomerLocation = Convert.ToString(dt.Rows[0]["CUSTOMER_LOCATION"]);
                        this.CustomerBug = "true";
                    }
                }
            }

        }
        //Display User's FUll Name
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
        //WAtchList info 
        private void GetWatchList()
        {
            IsWatching = false;
            if (_sql.ProcessCommand($"SELECT * FROM REA_WATCH_LIST WHERE USER_ID ={this.UserID} AND TRACKING_ID ={this.TrackingId}").Rows.Count > 0)
            {
                IsWatching = true;
            }
            //this.Watching = new List<dynamic>();
            String Command = $@"select
                            (st_users.FIRST_NAME+' '+st_users.LAST_NAME) as name 
                            from REA_WATCH_LIST
                            inner join ST_USERS on REA_WATCH_LIST.USER_ID = ST_USERS.USER_ID
                            where REA_WATCH_LIST.TRACKING_ID ={this.TrackingId} order by st_users.LAST_NAME,st_users.FIRST_NAME asc;";

            using (DataTable dt = _sql.ProcessCommand(Command))
            {
                this.WatchingString = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i != 0)
                    {
                        this.WatchingString += ", ";
                    }
                    this.WatchingString += Convert.ToString(dt.Rows[i]["name"]);
                }
            }
        }

        public void GetCodeReviews()
        {
            REATrackerDB sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            List<dynamic> ReviewersList = new List<dynamic>();
            using (System.Data.DataTable dt = sql.GetCodeReview(this.TrackingId))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].TrackingID = Convert.ToInt32(dt.Rows[i]["TRACKING_ID"]);
                    list[i].CreatedByID = Convert.ToInt32(dt.Rows[i]["CREATED_BY"]);
                    list[i].CreatedBy = Convert.ToString(dt.Rows[i]["CREATED_BY_NAME"]);
                    list[i].CreatedOn = Convert.ToDateTime(dt.Rows[i]["CREATED_ON"]).ToString("MM /dd/yyyy HH:mm:ss");
                    list[i].Title = Convert.ToString(dt.Rows[i]["TITLE"]);
                    list[i].FileList = Convert.ToString(dt.Rows[i]["FILE_LIST"]);
                    list[i].codeLocation = Convert.ToString(dt.Rows[i]["CODE_LOCATION"]);
                    list[i].Notes = Convert.ToString(dt.Rows[i]["NOTES"]);
                    list[i].Status = (dt.Rows[i]["STATUS"]);
                    list[i].Id = Convert.ToInt32(dt.Rows[i]["ID"]);

                    list[i].Reviewers = new List<dynamic>();
                    int j = 0;
                    using (DataTable dtReviewers = sql.GetCodeReviewers(list[i].Id))
                    {
                        foreach (DataRow drReviewer in dtReviewers.Rows)
                        {
                            list[i].Reviewers.Add(new System.Dynamic.ExpandoObject());
                            list[i].Reviewers[j].UserID = Convert.ToInt32(drReviewer["USER_ID"]);
                            list[i].Reviewers[j].FullName = Convert.ToString(drReviewer["FULL_NAME"]);
                            list[i].Reviewers[j].Status = Convert.ToInt32(drReviewer["STATUS"]);
                            j++;
                        }
                    }
                    /*
                    list[i].Reviewers = "";
                    List<dynamic> users = sql.GetReviewers(list[i].Id);
                    foreach (var item in users)
                    {
                        SCRTrackerDB tempSql = new SCRTrackerDB();
                        SCRTrackerDB.ReviewStatus iconValue = tempSql.GetReviewStatus(list[i].Id, item.UsersId);
                        if (iconValue == SCRTrackerDB.ReviewStatus.Approved)
                        {
                            list[i].Reviewers += "<i title='Approved' class='fa qvi-color-medium fa-check-circle'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                        else if (iconValue == SCRTrackerDB.ReviewStatus.ApprovedWithComments)
                        {
                            list[i].Reviewers += "<i title='Approved With Comments' class='fa qvi-color-low fa-comment-o'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                        else if (iconValue == SCRTrackerDB.ReviewStatus.NeedsWork)
                        {
                            list[i].Reviewers += "<i title='Needs Work' class='fa qvi-color-critical fa-times-circle'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                        else
                        {
                            list[i].Reviewers += "<i title='Accepted' class='fa fa-question-circle'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                    }                    
                    list[i].Reviewers += "";
                    */
                    i = i + 1;
                }
            }
            this.CodeReviews = list;
        }

        public bool SaveCodeReview()
        {
            bool success = false;
            try
            {
                REATrackerDB db = new REATrackerDB();
                int codeReviewID = db.CreateCodeReview(TrackingId, CodeReviewTitle, FileList, CodeLocation, UserID, Notes);
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
    } //End Class
 } //End Namespace