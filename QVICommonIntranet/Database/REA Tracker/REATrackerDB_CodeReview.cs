using System.ComponentModel.DataAnnotations;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;
using QVICommonIntranet.Email.EmailREATracker;
using QVICommonIntranet.Utility;

namespace QVICommonIntranet.Database
{
    public partial class REATrackerDB
    {
        /*
        public enum CommentsStatusType
        {
            [Display(Name = "Approved")]
            Approved = 0,
            [Display(Name = "Approved With Comments")]
            ApprovedWithComments = 1,
            [Display(Name = "Needs Work")]
            NeedsWork = 2,
        }
        */
        /// <summary>
        /// This is the status of the code review that the reviewer is doing
        /// </summary>
        public enum ReviewStatus
        {
            None = 0,
            Accepted = 1,               //the user has accepted to do the code review
            Approved = 2,               //the user has completed the code review and approved it
            ApprovedWithComments = 3,   //the user has completed the code review and approved it with some conditions
            NeedsWork = 4,              //the user has completed the code review and has not approved it, major things need to be fixed
            Closed = 5                  //the code review process has been completed
        }

        /// <summary>
        /// This is the status of the code review that was created
        /// </summary>
        public enum CodeReviewStatus
        {
            [Display(Name = "Closed")]
            Closed = 0,
            [Display(Name = "Open")]
            Open = 1
        }

        #region Code Review

        public int CreateCodeReview(int trackingID, string title, string fileList, string location, int createdByID, string notes)
        {
            int codeReviewID = -1;

            try
            {
                //TODO: fix
                //replace all new lines with | (bar) so it is easy to parse
                if (string.IsNullOrWhiteSpace(fileList))
                {
                    fileList = "";
                }
                else
                {
                    fileList = fileList.Trim();
                    fileList = fileList.Replace(System.Environment.NewLine, "|");
                }
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    String cmdtext = $@"
                INSERT INTO ST_CODE_REVIEW 
                        (TRACKING_ID, TITLE, FILE_LIST, CODE_LOCATION, CREATED_BY, NOTES, STATUS) 
                VALUES
                        (@TRACKING_ID, @TITLE, @FILE_LIST, @CODE_LOCATION, @CREATED_BY, @NOTES, @STATUS)
                SELECT CAST(scope_identity() AS int);
                ";

                    SqlCommand sqlCommander = new SqlCommand(cmdtext, connection);
                    sqlCommander.CommandText = cmdtext;
                    sqlCommander.CommandType = CommandType.Text;

                    //Create Sql parameters
                    sqlCommander.Parameters.Add("@TRACKING_ID", SqlDbType.Int);
                    sqlCommander.Parameters["@TRACKING_ID"].Value = trackingID;

                    sqlCommander.Parameters.Add("@TITLE", SqlDbType.NVarChar);
                    sqlCommander.Parameters["@TITLE"].Value = string.IsNullOrEmpty(title) ? (object)DBNull.Value : title;
                    sqlCommander.Parameters["@TITLE"].Size = 255;

                    sqlCommander.Parameters.Add("@CREATED_BY", SqlDbType.Int);
                    sqlCommander.Parameters["@CREATED_BY"].Value = createdByID;

                    sqlCommander.Parameters.Add("@FILE_LIST", SqlDbType.Text);
                    sqlCommander.Parameters["@FILE_LIST"].Value = string.IsNullOrEmpty(fileList) ? (object)DBNull.Value : fileList;

                    sqlCommander.Parameters.Add("@CODE_LOCATION", SqlDbType.NVarChar);
                    sqlCommander.Parameters["@CODE_LOCATION"].Value = string.IsNullOrEmpty(location) ? (object)DBNull.Value : location;
                    sqlCommander.Parameters["@CODE_LOCATION"].Size = 255;

                    sqlCommander.Parameters.Add("@NOTES", SqlDbType.Text);
                    sqlCommander.Parameters["@NOTES"].Value = string.IsNullOrEmpty(notes) ? (object)DBNull.Value : notes; 
                    
                    sqlCommander.Parameters.Add("@STATUS", SqlDbType.Int);
                    sqlCommander.Parameters["@STATUS"].Value = (int)CodeReviewStatus.Open;
                    codeReviewID = Convert.ToInt32(sqlCommander.ExecuteScalar());
                    //codeReviewID = Convert.ToInt32(ProcessScalarCommand(cmdtext)); //TODO FIX
                    if ((codeReviewID > 0) && String.IsNullOrWhiteSpace(LastError))
                    {
                        /*
                        DateTime createdOn = (DateTime)ProcessScalarCommand($"SELECT CREATED_ON FROM ST_CODE_REVIEW WHERE ID={codeReviewID}");
                        //the code review has been created, now add it to the REA's history
                        SqlParameter Description = new SqlParameter();
                        Description.SqlDbType = SqlDbType.VarChar;
                        Description.ParameterName = "DESCRIPTION";
                        Description.Value = "Related Code Review#[[CR_ID]]";
                        //CreateHistory(trackingID, (int)REATrackerDB.HistoryChangeType.chg_system, createdByID, createdOn, Description, null);
                        */
                    }
                    else
                    {
                        codeReviewID = -1;
                    }
                    connection.Close();
                }
            }
            catch(Exception ex)
            {
                Utility.Logging.WriteEntry(ex);
            }
            return codeReviewID;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codeReviewID">The ID of the code review, not the REA it is associated with</param>
        /// <returns></returns>
        public bool IsValidCodeReview(int codeReviewID)
        {
            //this query always returns 0 or 1, even if the Code Review is not found.
            //0 (false) = the Code Review has been found
            //1 (true) = the Code Review has NOT been found
            //false is the default answer.
            string cmdTxt = $"SELECT CASE WHEN COUNT(1) > 0 THEN 1 ELSE 0 END FROM ST_CODE_REVIEW WHERE ST_CODE_REVIEW.ID = {codeReviewID}";
            return ((int)ProcessScalarCommand(cmdTxt) == 1);
        }

        /// <summary>
        /// Returns all the code reviews based on the REA number
        /// </summary>
        /// <param name="trackingID">The REA number</param>
        /// <returns></returns>
        public DataTable GetCodeReview(int trackingID)
        {
            String command = $@"
                    SELECT 
                        ST_CODE_REVIEW.ID, ST_CODE_REVIEW.TRACKING_ID, ST_CODE_REVIEW.CREATED_BY, ST_CODE_REVIEW.CREATED_ON, ST_CODE_REVIEW.TITLE, ST_CODE_REVIEW.FILE_LIST, ST_CODE_REVIEW.CODE_LOCATION, ST_CODE_REVIEW.NOTES, ST_CODE_REVIEW.STATUS, 
                        (USER1.FIRST_NAME + ' ' + USER1.LAST_NAME) AS CREATED_BY_NAME
                    FROM ST_CODE_REVIEW
                    LEFT JOIN ST_USERS USER1 ON USER1.USER_ID = ST_CODE_REVIEW.CREATED_BY
                    WHERE TRACKING_ID = {trackingID}";
            return this.ProcessCommand(command);
        }

        public bool SetCodeReviewStatus(int trackingID, int status)
        {
            String command = $@"UPDATE ST_CODE_REVIEW SET STATUS={status} WHERE TRACKING_ID={trackingID}";
            int num_rows = this.ExecuteNonQuery(command);
            return (num_rows == 1);
        }

        public bool GetCodeReviewStatus(int codeReviewID)
        {
            //TODO: how to handle invalid code review ID
            bool val = false;
            try
            {
                String command = $@"SELECT STATUS FROM ST_CODE_REVIEW WHERE ID={codeReviewID}";
                DataTable dt = this.ProcessCommand(command);
                val = ((Convert.ToInt32(dt.Rows[0][0]) == (int)REATrackerDB.CodeReviewStatus.Open));
            }
            catch (Exception ex)
            {
                Utility.Logging.WriteEntry(ex);
            }
            return val;                        
        }
        public DataTable GetOpenCodeReviews(int productID, int statusID = (int)REATrackerDB.CodeReviewStatus.Open)
        {
            String command = $@"SELECT DISTINCT
                                    (ST_TRACK.TRACKING_ID), (ST_TRACK.TITLE) AS REA_TITLE,
                                    ST_CODE_REVIEW.ID, ST_PRODUCT_TEAM.PRODUCT, ST_CODE_REVIEW.TITLE, 
                                    ST_CODE_REVIEW.CREATED_ON, ST_CODE_REVIEW.CREATED_BY, (USER1.FIRST_NAME + ' ' + USER1.LAST_NAME) AS CREATED_BY_NAME, USER1.EMAIL AS CREATED_BY_EMAIL
                                FROM ST_PRODUCT_TEAM 
                                    INNER JOIN ST_TRACK ON ST_PRODUCT_TEAM.PRODUCT = ST_TRACK.PRODUCT
                                    FULL OUTER JOIN ST_CODE_REVIEW ON ST_TRACK.TRACKING_ID = ST_CODE_REVIEW.TRACKING_ID
                                    LEFT JOIN ST_USERS USER1 ON USER1.USER_ID = ST_CODE_REVIEW.CREATED_BY
                                WHERE ST_PRODUCT_TEAM.PRODUCT={productID} AND (ST_PRODUCT_TEAM.ROLE = 3 OR ST_PRODUCT_TEAM.ROLE=5) AND ST_CODE_REVIEW.STATUS ={statusID}";
            return this.ProcessCommand(command);
        }
        public DataTable SpecificCodeReview(int codeReviewID)
        {
            String command = $@"
                    SELECT 
                        (ST_TRACK.TRACKING_ID), (ST_TRACK.TITLE) AS REA_TITLE, ST_PRODUCT.NAME AS PRODUCT,
                        ST_CODE_REVIEW.ID, ST_CODE_REVIEW.CREATED_BY, ST_CODE_REVIEW.CREATED_ON, (USER1.FIRST_NAME + ' ' + USER1.LAST_NAME) AS CREATED_BY_NAME, USER1.EMAIL AS CREATED_BY_EMAIL,
                        ST_CODE_REVIEW.TITLE, ST_CODE_REVIEW.FILE_LIST, ST_CODE_REVIEW.CODE_LOCATION, ST_CODE_REVIEW.NOTES, ST_CODE_REVIEW.STATUS
                    FROM ST_CODE_REVIEW 
                        FULL OUTER JOIN ST_TRACK ON ST_TRACK.TRACKING_ID = ST_CODE_REVIEW.TRACKING_ID
                        LEFT JOIN ST_USERS USER1 ON USER1.USER_ID = ST_CODE_REVIEW.CREATED_BY
                        LEFT JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_TRACK.PRODUCT
                    WHERE ID={codeReviewID}";
            return this.ProcessCommand(command);
        }

        /// <summary>
        /// This gets the emails of all of the potentional reviews of a code review. The reviewer must be an active developer, leader or manager on the Product.
        /// The author of the code review is removed from the list.
        /// </summary>
        /// <param name="codeReviewID">ID of code review</param>
        /// <returns>semi-colon separated list of emails</returns>
        public string GetCodeReviewEmailRequestList(int codeReviewID)
        {
            String command = $@"
                    SELECT DISTINCT
	                    ST_USERS.EMAIL + '; ' AS 'data()' 
                    FROM ST_PRODUCT_TEAM 
	                    INNER JOIN ST_TRACK ON ST_TRACK.PRODUCT = ST_PRODUCT_TEAM.PRODUCT
	                    INNER JOIN ST_CODE_REVIEW ON ST_CODE_REVIEW.TRACKING_ID = ST_TRACK.TRACKING_ID
	                    INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_PRODUCT_TEAM.USER_ID
                    WHERE 
	                    ST_CODE_REVIEW.ID = {codeReviewID} AND ST_PRODUCT_TEAM.ROLE IN (3,5,7) AND ST_USERS.ACCOUNT_TYPE <> -1
                        AND ST_CODE_REVIEW.CREATED_BY <> ST_USERS.USER_ID
                    FOR XML PATH('')
";
            string list = this.ProcessScalarCommand(command).ToString();
            return list;
        }

        /// <summary>
        /// This gets the emails of all of the reviwers of the code review
        /// </summary>
        /// <param name="codeReviewID">ID of a code review</param>
        /// <returns>semi-colon separated list of emails</returns>
        public string GetCodeReviewEmailReviewerList(int codeReviewID)
        {
            String command = $@"
                        SELECT 
	                        ST_USERS.EMAIL + '; ' AS 'data()' 
                        FROM ST_USERS
	                        INNER JOIN ST_CODE_REVIEW_INFO ON ST_CODE_REVIEW_INFO.USER_ID = ST_USERS.USER_ID
                        WHERE 
	                        ST_CODE_REVIEW_INFO.CODE_REVIEW_ID = {codeReviewID}

                        FOR XML PATH('')
";
            string list = this.ProcessScalarCommand(command).ToString();
            return list;
        }
        #endregion

        public DataTable GetCodeReviewInfo(int userID, int codeReviewID)
        {
            String command = $@"
                        SELECT 
                            CODE_REVIEW_ID, USER_ID, STATUS 
                        FROM 
                            ST_CODE_REVIEW_INFO 
                        WHERE USER_ID={userID} AND CODE_REVIEW_ID={codeReviewID}";
            return this.ProcessCommand(command);
        }

        public DataTable GetCodeReviewComments(int codeReviewID)
        {
            String command = $@"SELECT
                                    ID, MADE_BY, MADE_ON, (USER1.FIRST_NAME + ' ' + USER1.LAST_NAME) AS MADE_BY_NAME,
                                    LINE_NO, FILE_NAME, COMMENTS, RESOLUTION
                                FROM
                                    ST_CODE_REVIEW_HISTORY
                                LEFT JOIN ST_USERS USER1 ON USER1.USER_ID = ST_CODE_REVIEW_HISTORY.MADE_BY
                                WHERE 
                                    ST_CODE_REVIEW_ID={codeReviewID}
                                ORDER BY CAST(FILE_NAME AS nvarchar(100)), CAST(LINE_NO AS nvarchar(100))";
            return this.ProcessCommand(command);
        }

        /// <summary>
        /// OLD GetStatusInfo
        /// </summary>
        /// <param name="codeReviewID">The ID of the code review</param>
        /// <param name="userID">The user ID of the Reviewer</param>
        /// <returns></returns>
        public ReviewStatus GetReviewStatus(int codeReviewID, int userID)
        {
            ReviewStatus status = ReviewStatus.None;

            String command = $@"SELECT STATUS FROM ST_CODE_REVIEW_INFO WHERE CODE_REVIEW_ID={codeReviewID} AND USER_ID={userID}";
            DataTable dt = this.ProcessCommand(command);
            using (dt)
            {
                if (dt.Rows.Count > 0)
                {
                    status = (ReviewStatus)(dt.Rows[0]["STATUS"]);
                }
            }
            return status;
        }
        /// <summary>
        /// OLD GetStatusInfo
        /// </summary>
        /// <param name="id">The id of the reviewer's code review
        /// <returns></returns>
        public ReviewStatus GetReviewStatus(int id)
        {
            ReviewStatus status = ReviewStatus.None;

            String command = $@"SELECT STATUS FROM ST_CODE_REVIEW_INFO WHERE ID={id}";
            DataTable dt = this.ProcessCommand(command);
            using (dt)
            {
                if (dt.Rows.Count > 0)
                {
                    status = (ReviewStatus)(dt.Rows[0]["STATUS"]);
                }
            }
            return status;
        }

        public Boolean IsCreator(int codeReviewID, int userID)
        {
            String command = $@"SELECT CREATED_BY FROM ST_CODE_REVIEW WHERE ID={codeReviewID}";
            DataTable results = this.ProcessCommand(command);
            return (Convert.ToInt32(results.Rows[0][0]) == userID);
        }

        public Boolean IsReviewer(int codeReviewID, int userID)
        {
            bool reviewer = false;

            try
            {
                String command = $@"SELECT CASE WHEN COUNT(1) > 0 THEN 1 ELSE 0 END FROM ST_CODE_REVIEW_INFO WHERE CODE_REVIEW_ID={codeReviewID} AND USER_ID={userID}";
                reviewer = Convert.ToBoolean(this.ProcessScalarCommand(command));
            }
            catch (Exception ex)
            {
                Utility.Logging.WriteEntry(ex);
                reviewer = false;
            }
            return reviewer;
        }

        public Boolean IsCommenter(int CodeReviewHistoryId, int userID)
        {
            String command = $@"SELECT MADE_BY FROM ST_CODE_REVIEW_HISTORY WHERE ID={CodeReviewHistoryId}";
            DataTable results = this.ProcessCommand(command);
            return (Convert.ToInt32(results.Rows[0][0])== userID);
        }
        public Boolean CommenterExists(int codeReviewID, int userID)
        {
            bool exists = false;
            String command = $@"SELECT MADE_BY FROM ST_CODE_REVIEW_HISTORY WHERE ST_CODE_REVIEW_ID={codeReviewID}";
            DataTable results = this.ProcessCommand(command);
            //bool exists = results.AsEnumerable().Any(c => c.Field<int>("MADE_BY") == userId);
            using (System.Data.DataTable dt = results)
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    if((int)row["MADE_BY"] == userID)
                    {
                        exists=true;
                        break;
                    }
                }
            }
            return exists;
        }

        /// <summary>
        /// Determines if the specified user is allowed to do the code review. The user must be a developer, leader or manager on the product team, or the user must be an Admin.
        /// </summary>
        /// <param name="codeReviewID"></param>
        /// <param name="userID"></param>
        /// <returns>true if allowed to review, else false</returns>
        public Boolean IsAllowedToReview(int codeReviewID, int userID)
        {
            bool allowed = false;

            String Command = $@"
                SELECT 
	                COUNT(*) AS 'ALLOWED'
                FROM ST_PRODUCT_TEAM 
	                INNER JOIN ST_TRACK ON ST_TRACK.PRODUCT = ST_PRODUCT_TEAM.PRODUCT
	                INNER JOIN ST_CODE_REVIEW ON ST_CODE_REVIEW.TRACKING_ID = ST_TRACK.TRACKING_ID
	                INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_PRODUCT_TEAM.USER_ID
                WHERE 
	                ST_CODE_REVIEW.ID = {codeReviewID} 
	                AND 
		                ((ST_PRODUCT_TEAM.USER_ID = {userID} AND (ST_PRODUCT_TEAM.ROLE IN (3,5,7)))
		                OR
		                ((ST_PRODUCT_TEAM.USER_ID = {userID}) AND (ST_USERS.ACCOUNT_TYPE = 0)))";

            allowed = Convert.ToBoolean(this.ProcessScalarCommand(Command));

            return allowed;
        }

        public Boolean IsAllowedToDelete(int codeReviewID, int userID)
        {
            //TODO: optimize
            bool allowed = false;

            if (IsActiveUser(userID))
            {
                string cmd1 = $"SELECT ACCOUNT_TYPE FROM ST_USERS WHERE USER_ID={userID}";
                allowed = ((int)ProcessScalarCommand(cmd1) == 0);
            }

            if (!allowed)
            {
                String Command = $@"
            SELECT
	            ST_PRODUCT_TEAM.ROLE
            FROM ST_PRODUCT_TEAM
	            INNER JOIN ST_TRACK ON ST_TRACK.PRODUCT = ST_PRODUCT_TEAM.PRODUCT
	            INNER JOIN ST_CODE_REVIEW ON ST_CODE_REVIEW.TRACKING_ID = ST_TRACK.TRACKING_ID
	            INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_PRODUCT_TEAM.USER_ID
            WHERE 
	            ((ST_CODE_REVIEW.ID = {codeReviewID} ) AND (ST_PRODUCT_TEAM.USER_ID = {userID}))";

                DataTable teamDT = ProcessCommand(Command);
                if (teamDT.Rows.Count > 0)
                {
                    int role = Convert.ToInt32(teamDT.Rows[0]["ROLE"]);
                    allowed = (role == 5);
                }
            }            
            return allowed;
        }

        public bool UpdateCodeReviewResolution(int commentID, string resolution)
        {
            bool success = false;

            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    //Create Sql parameters
                    SqlParameter sqlCommentID = new SqlParameter("ID", SqlDbType.Int);
                    SqlParameter sqlResolution = new SqlParameter("RESOLUTION", SqlDbType.NText);

                    //setting up values
                    sqlCommentID.Value = commentID;
                    sqlResolution.Value = resolution;

                    String cmdtext = $@"UPDATE ST_CODE_REVIEW_HISTORY SET RESOLUTION = @RESOLUTION WHERE ID=@ID";

                    SqlCommand sqlCommander = new SqlCommand(cmdtext, connection);
                    sqlCommander.CommandText = cmdtext;
                    sqlCommander.Parameters.Add(sqlCommentID);
                    sqlCommander.Parameters.Add(sqlResolution);

                    connection.Open();
                    int num_rows = sqlCommander.ExecuteNonQuery();

                    success = (num_rows == 1);
                }
            }
            catch(Exception ex)
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

        public bool UpdateCodeReviewComment(int commentID, string comments, string lineNumbers, string SelectedFile)
        {
            bool success = false;

            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    //Create Sql parameters
                    SqlParameter sqlCommentID = new SqlParameter("@ID", SqlDbType.Int);
                    SqlParameter sqlComments= new SqlParameter("@COMMENTS", SqlDbType.NText);
                    SqlParameter sqlLineNo = new SqlParameter("@LINE_NO", SqlDbType.NText);
                    SqlParameter sqlFileName = new SqlParameter("@FILE_NAME", SqlDbType.NText);

                    //setting up values
                    sqlCommentID.Value = commentID;
                    sqlComments.Value = comments;
                    sqlLineNo.Value = String.IsNullOrEmpty(lineNumbers) ? (object)DBNull.Value : lineNumbers; ;
                    sqlFileName.Value = String.IsNullOrEmpty(SelectedFile) ? (object)DBNull.Value : SelectedFile;

                    String cmdtext = $@"UPDATE ST_CODE_REVIEW_HISTORY SET LINE_NO =@LINE_NO, COMMENTS=@COMMENTS, FILE_NAME=@FILE_NAME WHERE ID=@ID";

                    SqlCommand sqlCommander = new SqlCommand(cmdtext, connection);
                    sqlCommander.CommandText = cmdtext;
                    sqlCommander.Parameters.Add(sqlCommentID);
                    sqlCommander.Parameters.Add(sqlComments);
                    sqlCommander.Parameters.Add(sqlLineNo);
                    sqlCommander.Parameters.Add(sqlFileName);
                    
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
            /*
            bool val = false;
            String command = $@"UPDATE ST_CODE_REVIEW_HISTORY
                                    SET LINE_NO ='{lineNumbers}', COMMENTS='{comments}', FILE_NAME='{SelectedFile}'
                                    WHERE ID={commentID}";
            int rdr = this.ExecuteNonQuery(command);
            while (rdr != 0)
            {
                val = true;
                return val;
            }
            return val;
            */
        }
        public int InsertCodeReviewComment(int codeReviewID, int userID, string comments, string lineNumbers, string SelectedFile)
        {
            int ID = 0;

            if (!string.IsNullOrWhiteSpace(comments))
            {
                /*
                String command = $@"
            INSERT INTO ST_CODE_REVIEW_HISTORY
                (LINE_NO, COMMENTS, FILE_NAME, ST_CODE_REVIEW_ID, MADE_BY) 
            VALUES
                ('{lineNumbers}','{comments}','{SelectedFile}',{codeReviewID},{userID});

            SELECT CAST(scope_identity() AS int);";

                ID = (int)ProcessScalarCommand(command);
                */
                SqlConnection connection = null;
                SqlCommand sqlCommander = null;
                String cmdtext = "";
                try
                {
                    using (connection = new SqlConnection(_connectionString))
                    {
                        //Create Sql parameters
                        SqlParameter sqlCodeReviewID = new SqlParameter("@CODE_REVIEW_ID", SqlDbType.Int);
                        SqlParameter sqlUserID = new SqlParameter("@USER_ID", SqlDbType.Int);
                        SqlParameter sqlComments = new SqlParameter("@COMMENTS", SqlDbType.NText);
                        SqlParameter sqlLineNo = new SqlParameter("@LINE_NO", SqlDbType.NText);
                        SqlParameter sqlFileName = new SqlParameter("@FILE_NAME", SqlDbType.NText);

                        //setting up values
                        sqlCodeReviewID.Value = codeReviewID;
                        sqlUserID.Value = userID;
                        sqlComments.Value = comments;
                        sqlLineNo.Value = String.IsNullOrEmpty(lineNumbers) ? (object)DBNull.Value : lineNumbers; ;
                        sqlFileName.Value = String.IsNullOrEmpty(SelectedFile) ? (object)DBNull.Value : SelectedFile;

                        cmdtext = $@"
                        INSERT INTO ST_CODE_REVIEW_HISTORY
                            (LINE_NO, COMMENTS, FILE_NAME, ST_CODE_REVIEW_ID, MADE_BY) 
                        VALUES
                            (@LINE_NO,@COMMENTS,@FILE_NAME,@CODE_REVIEW_ID,@USER_ID);

                        SELECT CAST(scope_identity() AS int);";

                        sqlCommander = new SqlCommand(cmdtext, connection);
                        sqlCommander.CommandText = cmdtext;
                        sqlCommander.Parameters.Add(sqlCodeReviewID);
                        sqlCommander.Parameters.Add(sqlUserID);
                        sqlCommander.Parameters.Add(sqlComments);
                        sqlCommander.Parameters.Add(sqlLineNo);
                        sqlCommander.Parameters.Add(sqlFileName);

                        connection.Open();
                        ID = (int)sqlCommander.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    Logging.WriteEntry(ex);
                    Logging.WriteEntry(cmdtext, System.Diagnostics.EventLogEntryType.Information);
                    ID = 0;
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                }
            }
            return ID;
        }
        public List<dynamic> GetReviewers(int codeReviewID)
        {
            //TODO: move list to upper layer
            String command = $@"
                    SELECT ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME as 'FULL_NAME', ST_CODE_REVIEW_INFO.USER_ID
                    FROM ST_CODE_REVIEW
                    inner JOIN ST_CODE_REVIEW_INFO ON ST_CODE_REVIEW.ID = ST_CODE_REVIEW_INFO.CODE_REVIEW_ID
                    inner join st_users on ST_USERS.USER_ID = ST_CODE_REVIEW_INFO.USER_ID
                    WHERE ST_CODE_REVIEW.ID ={codeReviewID}
                    group by ST_USERS.FIRST_NAME,ST_USERS.LAST_NAME, ST_CODE_REVIEW_INFO.STATUS, ST_CODE_REVIEW_INFO.USER_ID";
            DataTable rdr = this.ProcessCommand(command);
            //string reviewers = " ";
            List<dynamic> list = new List<dynamic>();
            int i = 0;
            using (System.Data.DataTable dt = rdr)
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].UsersId = Convert.ToInt32(row["USER_ID"]);
                    list[i].UsersName = Convert.ToString(row["FULL_NAME"]);
                    i++;
                }
            }
            //reviewers = reviewers.Trim(',');
            return list;
        }

        public DataTable GetCodeReviewers(int codeReviewID)
        {
            //TODO: move list to upper layer
            String command = $@"
                    SELECT 
                        ST_CODE_REVIEW_INFO.USER_ID, ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME as 'FULL_NAME', ST_CODE_REVIEW_INFO.STATUS
                    FROM 
                        ST_CODE_REVIEW
                    INNER JOIN ST_CODE_REVIEW_INFO ON ST_CODE_REVIEW.ID = ST_CODE_REVIEW_INFO.CODE_REVIEW_ID
                    INNER JOIN ST_USERS on ST_USERS.USER_ID = ST_CODE_REVIEW_INFO.USER_ID
                    WHERE ST_CODE_REVIEW.ID = {codeReviewID}
                    GROUP BY ST_USERS.FIRST_NAME,ST_USERS.LAST_NAME, ST_CODE_REVIEW_INFO.STATUS, ST_CODE_REVIEW_INFO.USER_ID";
            DataTable dt  = this.ProcessCommand(command);
            return dt;
        }

        public bool SetStatus(int codeReviewID, int userID, ReviewStatus status)
        {
            bool success = false;

            if (status != ReviewStatus.None)
            {
                String command = $@"UPDATE ST_CODE_REVIEW_INFO SET
                                    STATUS= {(int)status}
                                WHERE
                                    CODE_REVIEW_ID={codeReviewID} AND USER_ID={userID}";
                int num_rows = this.ExecuteNonQuery(command);

                success = (num_rows == 1);

            }

            return success;
        }

        public bool AcceptCodeReview(int codeReviewID, int userID)
        {
            bool success = false;

            String command = $@"INSERT INTO ST_CODE_REVIEW_INFO
                                    (CODE_REVIEW_ID, USER_ID, STATUS)
                                VALUES
                                    ({codeReviewID},{userID},{(int)ReviewStatus.Accepted})";

            int num_rows = this.ExecuteNonQuery(command);

            success = (num_rows == 1);

            return success;
        }

        public bool UpdateCodeReview(int codeReviewID, string title, string codelocation, string notes, string fileList)
        {
            bool success = false;

            try
            {
                //TODO: fix
                //replace all new lines with | (bar) so it is easy to parse
                if (string.IsNullOrWhiteSpace(fileList))
                {
                    fileList = "";
                }
                else
                {
                    fileList = fileList.Trim();
                    fileList = fileList.Replace(System.Environment.NewLine, "|");
                }
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    //Create Sql parameters
                    SqlParameter sqlTitle = new SqlParameter("TITLE", SqlDbType.VarChar);
                    SqlParameter sqlFileList = new SqlParameter("FILE_LIST", SqlDbType.NText);
                    SqlParameter sqlLocation = new SqlParameter("FILE_LOCATION", SqlDbType.NText);
                    SqlParameter sqlNotes = new SqlParameter("NOTES", SqlDbType.NText);

                    //setting up values
                    sqlTitle.Value = String.IsNullOrEmpty(title) ? (object)DBNull.Value : title;
                    sqlFileList.Value = fileList;
                    sqlLocation.Value = String.IsNullOrEmpty(codelocation) ? (object)DBNull.Value : codelocation;
                    sqlNotes.Value = String.IsNullOrEmpty(notes) ? (object)DBNull.Value : notes;

                    String cmdtext = $@"UPDATE ST_CODE_REVIEW SET TITLE=@TITLE, FILE_LIST=@FILE_LIST, CODE_LOCATION=@FILE_LOCATION, NOTES=@NOTES WHERE ID={codeReviewID}";

                    SqlCommand sqlCommander = new SqlCommand(cmdtext, connection);
                    sqlCommander.CommandText = cmdtext;
                    sqlCommander.Parameters.Add(sqlTitle);
                    sqlCommander.Parameters.Add(sqlFileList);
                    sqlCommander.Parameters.Add(sqlLocation);
                    sqlCommander.Parameters.Add(sqlNotes);
                    connection.Open();
                    int rows = Convert.ToInt32(sqlCommander.ExecuteNonQuery());
                    success = (1 == rows);
                    connection.Close();
                }


            }
            catch (Exception ex)
            {
                Utility.Logging.WriteEntry(ex);
            }

            /*
            filelist = filelist.Replace(System.Environment.NewLine, " | ");
            bool success = false;
            String command = $@"UPDATE ST_CODE_REVIEW SET TITLE='{title}', FILE_LIST='{filelist}', CODE_LOCATION='{codelocation}', NOTES='{notes}' WHERE ID={codeReviewID}";
            int num_rows = this.ExecuteNonQuery(command);
            if(num_rows != 0)
            {
                return success=true;
            }
            */
            return success;
        }

        public bool DeleteCodeReview(int codeReviewID)
        {
            bool success = false;

            string cmd = $@"
                DELETE FROM [ST_CODE_REVIEW_HISTORY] WHERE [ST_CODE_REVIEW_ID] = {codeReviewID};
                
                DELETE FROM [ST_CODE_REVIEW_INFO] WHERE [CODE_REVIEW_ID] = {codeReviewID};
                
                DELETE FROM [ST_CODE_REVIEW] WHERE [ST_CODE_REVIEW].[ID] = {codeReviewID};";

            success = ExecuteSqlTransaction("DeleteCodeReview", cmd);
            return success;
        }
    }
}
