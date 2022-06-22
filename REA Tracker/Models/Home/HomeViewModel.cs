using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{
    public class HomeIndexViewModel
    {
        public int LayoutStyle { get; set; }
        int UserId { get; set; }
        int numDisplayToDo { get; set; }
        int numDisplayWatchList { get; set; }
        public List<ToDoItem> toDoListDisplay { get; set; }
        public List<WatchItem> watchListDisplay { get; set; }
        public List<dynamic> CRCreatedWatchlist { get; set; }
        public List<dynamic> CRCommentedWatchlist { get; set; }
        public List<string> TableId { get; set; }
        public FeedResult feedResult { get; set; }
        public bool deletedClosed { get; set; }
        protected REATrackerDB _sql = new REATrackerDB();
        public HomeIndexViewModel()
        {
            this.UserId = 0;
            ReadUserSettings();
            this.toDoListDisplay = new List<ToDoItem>();
            this.watchListDisplay = new List<WatchItem>();
            this.toDoListDisplay = this.toDoListDisplay.OrderBy(u => u.ProductName).ToList();
            this.watchListDisplay = this.watchListDisplay.OrderBy(u => u.ProductName).ToList();
        }

        public HomeIndexViewModel( int st_user_id )
        {
            this.UserId = st_user_id;
            ReadUserSettings();
            PopulateToDoList();
            PopulateWatchList();
        }

        protected void ReadUserSettings()
        {
            this.LayoutStyle = 0;
            this.numDisplayToDo = -1;
            this.numDisplayWatchList = -1;

            if (this.UserId != 0)
            {
                string cmdText = $"SELECT TO_DO_DISPLAY, WATCH_DISPLAY FROM ST_USERS WHERE USER_ID = {this.UserId}";
                using (DataTable dt = _sql.ProcessCommand(cmdText))
                {
                    if (dt.Rows.Count > 0)
                    {
                        this.numDisplayToDo = Convert.ToInt32(dt.Rows[0]["TO_DO_DISPLAY"]);
                        this.numDisplayWatchList = Convert.ToInt32(dt.Rows[0]["WATCH_DISPLAY"]);
                    }
                }
            }
        }

        public void CodeReviewWatchList(int user_id)
        {
            this.UserId = user_id;
            REATrackerDB sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            List<dynamic> commentedList = new List<dynamic>();
            string command = $@"SELECT DISTINCT (ST_CODE_REVIEW.ID),ST_CODE_REVIEW.TRACKING_ID, ST_CODE_REVIEW.TITLE, CREATED_ON, CREATED_BY, ST_CODE_REVIEW.ID, ST_PRODUCT.NAME
                                FROM ST_CODE_REVIEW
                                INNER JOIN ST_TRACK
                                ON ST_TRACK.TRACKING_ID = ST_CODE_REVIEW.TRACKING_ID
                                INNER JOIN ST_PRODUCT
                                ON ST_PRODUCT.PRODUCT_ID = ST_TRACK.PRODUCT
                                WHERE ST_CODE_REVIEW.CREATED_BY = {UserId} AND ST_CODE_REVIEW.STATUS=1";
            string commandTwo = $@"SELECT DISTINCT(ST_CODE_REVIEW.TRACKING_ID), ST_CODE_REVIEW.TITLE, CREATED_ON, CREATED_BY, ST_CODE_REVIEW.ID, ST_PRODUCT.NAME,
                                    ST_CODE_REVIEW_INFO.USER_ID
                                FROM ST_CODE_REVIEW
                                INNER JOIN ST_CODE_REVIEW_INFO
                                    ON ST_CODE_REVIEW.ID = ST_CODE_REVIEW_INFO.CODE_REVIEW_ID
                                INNER JOIN ST_TRACK
	                                ON ST_TRACK.TRACKING_ID = ST_CODE_REVIEW.TRACKING_ID
                                INNER JOIN ST_PRODUCT
	                                ON ST_PRODUCT.PRODUCT_ID = ST_TRACK.PRODUCT
                                WHERE USER_ID = {UserId}  AND ST_CODE_REVIEW.STATUS=1 ";

            using (System.Data.DataTable dt = sql.ProcessCommand(command))
            {
                int j = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[j].Id = Convert.ToInt32(row["ID"]);
                    list[j].TrackingId = Convert.ToInt32(row["TRACKING_ID"]);
                    list[j].Product = Convert.ToString(row["NAME"]);
                    list[j].Title = Convert.ToString(row["TITLE"]);
                    list[j].CreatedOn = Math.Floor((System.DateTime.Now - Convert.ToDateTime(row["CREATED_ON"])).TotalDays);
                    list[j].CreatedByID = Convert.ToInt32(row["CREATED_BY"]);
                    list[j].Reviewers = "";
                    List<dynamic> users = sql.GetReviewers(list[j].Id);
                    foreach (var item in users)
                    {
                        //TODO:
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
                this.CRCreatedWatchlist = list;
            }

            using (System.Data.DataTable dt = sql.ProcessCommand(commandTwo))
            {
                int j = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    commentedList.Add(new System.Dynamic.ExpandoObject());
                    commentedList[j].Id = Convert.ToInt32(row["ID"]);
                    commentedList[j].TrackingId = Convert.ToInt32(row["TRACKING_ID"]);
                    commentedList[j].Product = Convert.ToString(row["NAME"]);
                    commentedList[j].Title = Convert.ToString(row["TITLE"]);
                    commentedList[j].CreatedOn = Math.Floor((System.DateTime.Now - Convert.ToDateTime(row["CREATED_ON"])).TotalDays);
                    commentedList[j].CreatedByID = Convert.ToInt32(row["CREATED_BY"]);
                    commentedList[j].Reviewers = "";
                    List<dynamic> usersTwo = sql.GetReviewers(commentedList[j].Id);
                    foreach (var item in usersTwo)
                    {
                        REATrackerDB tempSql = new REATrackerDB();
                        REATrackerDB.ReviewStatus iconValue = tempSql.GetReviewStatus(commentedList[j].Id, item.UsersId);
                        if (iconValue == REATrackerDB.ReviewStatus.Approved)
                        {
                            commentedList[j].Reviewers += "<i title='Approved' class='fa qvi-color-medium fa-check-circle'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                        else if (iconValue == REATrackerDB.ReviewStatus.ApprovedWithComments)
                        {
                            commentedList[j].Reviewers += "<i title='Approved With Comments' class='fa qvi-color-low fa-comment-o'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                        else if (iconValue == REATrackerDB.ReviewStatus.NeedsWork)
                        {
                            commentedList[j].Reviewers += "<i title='Needs Work' class='fa qvi-color-critical fa-times-circle'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                        else
                        {
                            commentedList[j].Reviewers += "<i title='Accepted' class='fa fa-question-circle'></i> " + Convert.ToString(item.UsersName) + " " + "<br/>";
                        }
                    }
                    commentedList[j].Reviewers += "";
                    j++;
                }
                this.CRCommentedWatchlist = commentedList;
            }
        }

        public void remove()
        {
            bool remove = false;
            String removeVariable = "DELETE FROM REA_WATCH_LIST WHERE WATCH_ID = ";
            foreach(var item in watchListDisplay)
            {
                if(item.Remove)
                {
                    if (remove)
                    {
                        //already has a watch id set
                        removeVariable += " OR WATCH_ID = " + item.WatchListID;
                    }
                    else
                    {
                        remove = true;
                        removeVariable += item.WatchListID;
                    }
                }
            }
            if (remove)
            {
                REATrackerDB sql = new REATrackerDB();
                sql.ProcessCommand(removeVariable);
            }
        }

        public void removeClosed()
        {
            bool remove = false;
            String removeVariable = "DELETE FROM REA_WATCH_LIST WHERE WATCH_ID = ";
            foreach (var item in watchListDisplay)
            {
                if (item.StatusName.ToLower() == "closed" )
                {
                    if (remove)
                    {
                        //already has a watch id set
                        removeVariable += " OR WATCH_ID = " + item.WatchListID;
                    }
                    else
                    {
                        remove = true;
                        removeVariable += item.WatchListID;
                    }
                }
            }
            if (remove)
            {
                this.deletedClosed = true;
                REATrackerDB sql = new REATrackerDB();
                sql.ProcessCommand(removeVariable);
            }
        }

        private void PopulateToDoList()
        {
            this.toDoListDisplay = new List<ToDoItem>();
            if (this.UserId != 0)
            {
                string display_count = this.numDisplayToDo == -1 ? "" : " TOP " + Convert.ToString(this.numDisplayToDo);
                string cmdText = $@"
                    SELECT DISTINCT {display_count} TRACKING_ID, REA_Track.PRODUCT, REA_Track.ASSIGNED_TO,
                        REA_PRODUCT.NAME AS 'PRODUCT_NAME', 
                        REA_STATUS.NAME AS 'STATUS_NAME', 
                        REA_TRACK.TITLE, REA_TRACK.PRIORITY, REA_TRACK.STATUS,
                        MANAGERS_NOTE,
                        LAST_UPDATE
                    FROM 
                        REA_TRACK
                    INNER JOIN REA_PRODUCT ON REA_PRODUCT.PRODUCT_ID = REA_TRACK.PRODUCT
                    INNER JOIN REA_PRIORITY ON REA_PRIORITY.PRIORITY_ID = REA_TRACK.PRIORITY
                    INNER JOIN REA_STATUS ON REA_STATUS.STATUS_ID = REA_TRACK.STATUS
					INNER JOIN [BTRACKER_NEW].dbo.ST_USERS ON ST_USERS.USER_ID = REA_TRACK.ASSIGNED_TO
                    WHERE ASSIGNED_TO={this.UserId}  
                    ORDER BY 'PRODUCT_NAME', REA_TRACK.LAST_UPDATE DESC";

                using (DataTable dt = _sql.ProcessCommand(cmdText))
                {
                    this.TableId = new List<string>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        ToDoItem item = new ToDoItem();

                        item.TrackingID = Convert.ToInt32(dr["TRACKING_ID"]);
                        item.ProductID = Convert.ToInt32(dr["PRODUCT"]);
                        item.ProductName = dr["PRODUCT_NAME"].ToString().Trim();
                        item.Title = dr["TITLE"].ToString().Trim();
                        item.PriorityID = Convert.ToInt32(dr["PRIORITY"]);
                        //item.PriorityName = dr["PRIORITY_NAME"].ToString();
                        item.StatusID = Convert.ToInt32(dr["STATUS"]);
                        item.StatusName = dr["STATUS_NAME"].ToString().Trim();
                        //item.IssueName = dr["PRIORITY_NAME"].ToString();
                        item.LastUpdated = (dr["LAST_UPDATE"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(dr["LAST_UPDATE"]);
                        item.ManagersNote = dr["MANAGERS_NOTE"].ToString();
                        this.toDoListDisplay.Add(item);

                        //tableID is used for sorting, cant use the product name because spaces in names break the sorting
                        //so just use the world Table plus the ID of the product
                        if (!this.TableId.Contains($"Table{item.ProductID}"))
                        {
                            this.TableId.Add($"Table{item.ProductID}");
                        }
                    }
                }
                //dont need to resort here, cause its done in the query
                //this.toDoListDisplay = this.toDoListDisplay.OrderBy(u => u.ProductName).ToList();
            }
        }

        private void PopulateWatchList()
        {
            this.watchListDisplay = new List<WatchItem>();
            if (this.UserId != 0)
            {
                string display_count = this.numDisplayWatchList == -1 ? "" : " TOP " + Convert.ToString(this.numDisplayWatchList);
                string cmdText = $@"
                    SELECT DISTINCT {display_count} REA_WATCH_LIST.TRACKING_ID, REA_WATCH_LIST.WATCH_ID, 
                        ST_TRACK.PRODUCT,
                        ST_PRODUCT.NAME AS 'PRODUCT_NAME', 
                        ST_STATUS.NAME AS 'STATUS_NAME', 
                        ST_TRACK.TITLE, ST_TRACK.PRIORITY, ST_TRACK.STATUS, ST_TRACK.ISSUE_TYPE,
                        CLOSED_BY, (USER1.FIRST_NAME + ' ' + USER1.LAST_NAME) AS 'CLOSED_BY_NAME',
                        ASSIGNED_TO, (USER2.FIRST_NAME + ' ' + USER2.LAST_NAME) AS 'ASSIGNED_TO_NAME',
                        MANAGERS_NOTE,
                        LAST_UPDATE
                    FROM 
                        REA_WATCH_LIST
                    LEFT JOIN ST_TRACK ON ST_TRACK.TRACKING_ID = REA_WATCH_LIST.TRACKING_ID
                    INNER JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_TRACK.PRODUCT
                    INNER JOIN ST_PRIORITY ON ST_PRIORITY.ID = ST_TRACK.PRIORITY
                    INNER JOIN ST_STATUS ON ST_STATUS.STATUS_ID = ST_TRACK.STATUS
                    INNER JOIN ST_ISSUE_TYPE ON ST_ISSUE_TYPE.ISSUE_ID = ST_TRACK.ISSUE_TYPE
                    LEFT OUTER JOIN ST_USERS USER1 ON ST_TRACK.CLOSED_BY = USER1.USER_ID
                    LEFT OUTER JOIN ST_USERS USER2 ON ST_TRACK.ASSIGNED_TO = USER2.USER_ID
                    WHERE REA_WATCH_LIST.USER_ID = {UserId}
                    ORDER BY ST_TRACK.LAST_UPDATE DESC";
                using (DataTable dt = _sql.ProcessCommand(cmdText))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        WatchItem item = new WatchItem();
                        item.WatchListID = Convert.ToInt32(dr["WATCH_ID"]);
                        item.TrackingID = Convert.ToInt32(dr["TRACKING_ID"]);
                        item.ProductID = Convert.ToInt32(dr["PRODUCT"]);
                        item.ProductName = dr["PRODUCT_NAME"].ToString().Trim();
                        item.Title = dr["TITLE"].ToString().Trim();
                        item.PriorityID = Convert.ToInt32(dr["PRIORITY"]);
                        //item.PriorityName = dr["PRIORITY_NAME"].ToString();
                        item.StatusID = Convert.ToInt32(dr["STATUS"]);
                        item.StatusName = dr["STATUS_NAME"].ToString().Trim();
                        item.IssueID = Convert.ToInt32(dr["ISSUE_TYPE"]);
                        //item.IssueName = dr["PRIORITY_NAME"].ToString();
                        item.IsClosed = (item.StatusID == (int)REATrackerDB.StatusChangeID.status_closed);
                        item.ClosedByID = (dr["CLOSED_BY"] == DBNull.Value) ? 0 : Convert.ToInt32(dr["CLOSED_BY"]);
                        item.ClosedByName = Convert.ToString(dr["CLOSED_BY_NAME"]);
                        item.AssignedToID = (dr["ASSIGNED_TO"] == DBNull.Value) ? 0 : Convert.ToInt32(dr["ASSIGNED_TO"]);
                        item.AssignedToName = Convert.ToString(dr["ASSIGNED_TO_NAME"]);
                        item.LastUpdated = (dr["LAST_UPDATE"] == DBNull.Value) ? "" : Convert.ToDateTime(dr["LAST_UPDATE"]).ToString("MM/dd/yy");
                        item.ManagersNote = dr["MANAGERS_NOTE"].ToString();
                        this.watchListDisplay.Add(item);
                    }
                }
                this.watchListDisplay = this.watchListDisplay.OrderBy(u => u.LastUpdated).ToList();
            }
        }

        private string GetVersionValue(string Version)
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


        public class ToDoItem
        {
            public int TrackingID;
            public int ProductID;
            public string ProductName;
            public string Title;
            public int PriorityID;
            public string PriorityName;
            public int StatusID;
            public string StatusName;
            public int IssueID;
            public string IssueName;
            public string ManagersNote;
            public DateTime? LastUpdated;
            public ToDoItem() { }
        }

        public class WatchItem
        {
            public int WatchListID { get; set; }    //need the accessors for the postback
            public int TrackingID;
            public int ProductID;
            public string ProductName;
            public string Title;
            public int PriorityID;
            public string PriorityName;
            public int StatusID;
            public string StatusName;
            public int IssueID;
            public string IssueName;
            public int AssignedToID;
            public string AssignedToName;
            public bool IsClosed;
            public int ClosedByID;
            public string ClosedByName;
            public string ManagersNote;
            public string LastUpdated;
            public bool Remove { get; set; } //need the accessors for the postback
            public WatchItem() { }
        }
    }

}

   

   

