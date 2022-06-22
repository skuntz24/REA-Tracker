using System;
using System.Data;
using System.Data.SqlClient;

namespace QVICommonIntranet.Database
{
    /// <summary>
    /// The functions in this file deal with various reporting
    /// </summary>
    public partial class REATrackerDB
    {
        #region Dashboard Builtin Reports
        /// <summary>
        /// This reports all the REAs for the specific product and status based on the index.
        /// </summary>
        /// <param name="productID">specific product ID</param>
        /// <param name="reportStatusIndex">the type of status report to display, this is not a 1 to 1 with the status ID</param>
        /// <returns></returns>
        public DataTable GetREAByStatus(int productID, int reportStatusIndex)
        {
            //base SQL query
            string strCmdText = @"
	        SELECT ST_TRACK.TRACKING_ID, ST_PRODUCT.NAME, ST_TRACK.TITLE, ST_ISSUE_TYPE.NAME, ST_PRIORITY.NAME, ST_STATUS.NAME, ST_TRACK.ASSIGNED_TO, 
		        ST_TRACK.VP_MAJOR, ST_TRACK.VP_MINOR, ST_TRACK.VP_BUILD, ST_TRACK.VP_MISC,
		        ST_TRACK.VF_MAJOR, ST_TRACK.VF_MINOR, ST_TRACK.VF_BUILD, ST_TRACK.VF_MISC,
		        ST_TRACK.RESOLVED_ON,
		        ST_TRACK.VR_MAJOR, ST_TRACK.VR_MINOR, ST_TRACK.VR_BUILD, ST_TRACK.VR_MISC,
		        ST_TRACK.PRIORITY,
                ST_TRACK.MANAGERS_NOTE
	        FROM ST_TRACK
	        INNER JOIN ST_PRODUCT ON ST_TRACK.PRODUCT = ST_PRODUCT.PRODUCT_ID
	        INNER JOIN ST_ISSUE_TYPE ON ST_TRACK.ISSUE_TYPE= ST_ISSUE_TYPE.ISSUE_ID
	        INNER JOIN ST_STATUS ON ST_TRACK.STATUS = ST_STATUS.STATUS_ID
	        INNER JOIN ST_PRIORITY ON ST_PRIORITY.ID = ST_TRACK.PRIORITY
	        WHERE (ST_TRACK.STATUS IN (@STATUS_LIST)) AND ST_TRACK.PRODUCT = @PRODUCT
	        ORDER BY ST_TRACK.ISSUE_TYPE ASC, ST_TRACK.PRIORITY DESC, ST_TRACK.TRACKING_ID ASC
";
            //convert the index to a list of status IDs
            string strStatusIDList = "1,2,5,6,7,8,11";
            switch(reportStatusIndex)
            {
                case 1: { strStatusIDList = "1"; break; }             //submitted
                case 17: { strStatusIDList = "2"; break; }           //approved
                case 2: { strStatusIDList = "5"; break; }             // in process
                case 3: { strStatusIDList = "6,7"; break; }           //fixed, delivered
                case 4: { strStatusIDList = "8,10,11"; break; }       //testing
                case 5: { strStatusIDList = "3"; break; }             //deferred
            }

            //replace variables with values
            strCmdText = strCmdText.Replace("@PRODUCT", productID.ToString());
            strCmdText = strCmdText.Replace("@STATUS_LIST", strStatusIDList);

            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }

        /// <summary>
        /// This reports all the REAs on the DEV side for the specific product and issue type
        /// </summary>
        /// <param name="productID">specific product ID</param>
        /// <param name="issueTypeID">specific issue type ID</param>
        /// <returns></returns>
        public DataTable GetREAByIssueType(int productID, int issueTypeID)
        {
            string strCmdText = @"
	        SELECT ST_TRACK.TRACKING_ID, ST_PRODUCT.NAME, ST_TRACK.TITLE, ST_ISSUE_TYPE.NAME, ST_PRIORITY.NAME, ST_STATUS.NAME, ST_TRACK.ASSIGNED_TO, 
		        ST_TRACK.VP_MAJOR, ST_TRACK.VP_MINOR, ST_TRACK.VP_BUILD, ST_TRACK.VP_MISC,
		        ST_TRACK.VF_MAJOR, ST_TRACK.VF_MINOR, ST_TRACK.VF_BUILD, ST_TRACK.VF_MISC,
		        ST_TRACK.RESOLVED_ON,
		        ST_TRACK.VR_MAJOR, ST_TRACK.VR_MINOR, ST_TRACK.VR_BUILD, ST_TRACK.VR_MISC, ST_TRACK.PRIORITY,
                ST_TRACK.MANAGERS_NOTE
	        FROM ST_TRACK
	        INNER JOIN ST_PRODUCT ON ST_TRACK.PRODUCT = ST_PRODUCT.PRODUCT_ID
	        INNER JOIN ST_ISSUE_TYPE ON ST_TRACK.ISSUE_TYPE= ST_ISSUE_TYPE.ISSUE_ID
	        INNER JOIN ST_STATUS ON ST_TRACK.STATUS = ST_STATUS.STATUS_ID
	        INNER JOIN ST_PRIORITY ON ST_PRIORITY.ID = ST_TRACK.PRIORITY
	        WHERE ST_TRACK.PRODUCT = @PRODUCT AND (ST_TRACK.ISSUE_TYPE IN (@ISSUE_LIST)) AND (STATUS IN (1,2,5,6,7))
	        ORDER BY ST_TRACK.ISSUE_TYPE ASC, ST_TRACK.PRIORITY DESC, ST_TRACK.TRACKING_ID ASC
";
            //replace variables with values
            strCmdText = strCmdText.Replace("@PRODUCT", productID.ToString());
            strCmdText = strCmdText.Replace("@ISSUE_LIST", issueTypeID.ToString());

            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }

        public DataTable GetREAByPriority(int productID, int priorityID, bool displayAll)
        {
            string strCmdText = @"
	        SELECT ST_TRACK.TRACKING_ID, ST_PRODUCT.NAME, ST_TRACK.TITLE, ST_ISSUE_TYPE.NAME, ST_PRIORITY.NAME, ST_STATUS.NAME, ST_TRACK.ASSIGNED_TO, 
		        ST_TRACK.VP_MAJOR, ST_TRACK.VP_MINOR, ST_TRACK.VP_BUILD, ST_TRACK.VP_MISC,
		        ST_TRACK.VF_MAJOR, ST_TRACK.VF_MINOR, ST_TRACK.VF_BUILD, ST_TRACK.VF_MISC,
		        ST_TRACK.RESOLVED_ON,
		        ST_TRACK.VR_MAJOR, ST_TRACK.VR_MINOR, ST_TRACK.VR_BUILD, ST_TRACK.VR_MISC, ST_TRACK.PRIORITY,
                ST_TRACK.MANAGERS_NOTE
	        FROM ST_TRACK
	        INNER JOIN ST_PRODUCT ON ST_TRACK.PRODUCT = ST_PRODUCT.PRODUCT_ID
	        INNER JOIN ST_ISSUE_TYPE ON ST_TRACK.ISSUE_TYPE= ST_ISSUE_TYPE.ISSUE_ID
	        INNER JOIN ST_STATUS ON ST_TRACK.STATUS = ST_STATUS.STATUS_ID
	        INNER JOIN ST_PRIORITY ON ST_PRIORITY.ID = ST_TRACK.PRIORITY
	        WHERE ST_TRACK.PRODUCT = @PRODUCT AND (ST_TRACK.PRIORITY IN (@PRIORITY_LIST)) AND (STATUS IN (@STATUS_LIST)) AND (ISSUE_TYPE IN (@ISSUE_LIST))
	        ORDER BY ST_TRACK.ISSUE_TYPE ASC, ST_TRACK.PRIORITY DESC, ST_TRACK.TRACKING_ID ASC
";

            //replace variables with values
            strCmdText = strCmdText.Replace("@PRODUCT", productID.ToString());
            strCmdText = strCmdText.Replace("@PRIORITY_LIST", priorityID.ToString());
            if (displayAll)
            {
                strCmdText = strCmdText.Replace("@STATUS_LIST", "1,2,5,6,7,8,11");
                strCmdText = strCmdText.Replace("@ISSUE_LIST", "1,2,3");    //problems, enhancements, planned work
            }
            else
            {
                strCmdText = strCmdText.Replace("@STATUS_LIST", "1,2,5,6,7");
                strCmdText = strCmdText.Replace("@ISSUE_LIST", "1");        //problems
            }
            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }


        #endregion

        #region Custom Reports
        /// <summary>
        /// Deletes the report form the table
        /// </summary>
        /// <param name="reportID"></param>
        public void DeleteReport(int reportID)
        {
            string strCmdText = @"
            DELETE FROM ST_DATE_RANGE WHERE REPORT_ID = @REPORT_ID;
            DELETE FROM ST_VERSION_RANGE WHERE REPORT_ID = @REPORT_ID;
            DELETE FROM ST_TABULAR_REPORT WHERE REPORT_ID = @REPORT_ID;
            ";
            strCmdText = strCmdText.Replace("@REPORT_ID", reportID.ToString());
            this.ProcessCommand(strCmdText);
        }
        #endregion

        #region Kanban Reports
        /// <summary>
        /// Give the list of REAS, return the data that is used to display a Kanban report
        /// </summary>
        /// <param name="trackingIDs">REAS (comma delimited)</param>
        /// <returns></returns>
        public DataTable KanbanREAS(string trackingIDs)
        {
            /*
            string command =
                $@"SELECT ST_TRACK.TRACKING_ID, ST_TRACK.TITLE, ST_TRACK.PRIORITY, ST_TRACK.ISSUE_TYPE, ST_TRACK.STATUS, DATEDIFF(DAY, LAST_UPDATE, GETDATE()) AS LAST_UPDATE, ST_TRACK.RANK_ORDER, USER2.FIRST_NAME, USER2.LAST_NAME 
                FROM ST_TRACK 
                LEFT OUTER JOIN ST_USERS USER2 ON ST_TRACK.ASSIGNED_TO = USER2.USER_ID
                WHERE TRACKING_ID in ({trackingIDs}) AND (ST_TRACK.STATUS <> 9 AND ST_TRACK.STATUS <> 4 AND ST_TRACK.STATUS <> 3)
                ORDER BY STATUS"; //no closed or rejected or deferred
            */

            //SQL 2010: ISNULL(CHARINDEX('REQUIRED', ST_KEYWORD.Keyword), 0) AS REQUIRED
            //SQL 2014: IIF(ST_KEYWORD.Keyword = 'REQUIRED', 1, 0) AS REQUIRED
            string command =
                $@"
SELECT 
temp.TRACKING_ID, temp.TITLE, temp.PRIORITY, temp.ISSUE_TYPE, temp.STATUS, temp.LAST_UPDATE, temp.RANK_ORDER,
		temp.FIRST_NAME, temp.LAST_NAME, temp.MANAGERS_NOTE,
				SUM(temp.REQUIRED) AS REQUIRED, temp.PLANNED_RELEASE, temp.ACTUAL_RELEASE, temp.PRODUCT_NAME
FROM
				(
SELECT ST_TRACK.TRACKING_ID, ST_TRACK.TITLE, ST_TRACK.PRIORITY, ST_TRACK.ISSUE_TYPE, ST_TRACK.STATUS, DATEDIFF(DAY, LAST_UPDATE, GETDATE()) AS LAST_UPDATE, ST_TRACK.RANK_ORDER,
		USER2.FIRST_NAME, USER2.LAST_NAME, ST_TRACK.MANAGERS_NOTE, 
                (ISNULL(ST_TRACK.VP_MAJOR,'?')+'.'+ISNULL(ST_TRACK.VP_MINOR,'?')+'.'+ISNULL(ST_TRACK.VP_BUILD,'?')+' '+ISNULL(ST_TRACK.VP_MISC,'')) AS 'PLANNED_RELEASE',
                (ISNULL(ST_TRACK.VR_MAJOR,'?')+'.'+ISNULL(ST_TRACK.VR_MINOR,'?')+'.'+ISNULL(ST_TRACK.VR_BUILD,'?')+' '+ISNULL(ST_TRACK.VR_MISC,'')) AS 'ACTUAL_RELEASE',
                ST_PRODUCT.NAME as 'PRODUCT_NAME',
				ISNULL(CHARINDEX('REQUIRED', ST_KEYWORD.Keyword), 0) AS REQUIRED
                FROM ST_TRACK 
                LEFT OUTER JOIN ST_USERS USER2 ON ST_TRACK.ASSIGNED_TO = USER2.USER_ID
                LEFT OUTER JOIN ST_PRODUCT ON ST_TRACK.PRODUCT = ST_PRODUCT.PRODUCT_ID
				LEFT OUTER JOIN ST_KEYWORD_REA_RELATION ON ST_TRACK.TRACKING_ID = ST_KEYWORD_REA_RELATION.TRACKING_ID
				LEFT OUTER JOIN ST_KEYWORD ON ST_KEYWORD_REA_RELATION.KEYWORD_ID =  ST_KEYWORD.ID
                WHERE ST_TRACK.TRACKING_ID in ({trackingIDs}) AND (ST_TRACK.STATUS <> 9 AND ST_TRACK.STATUS <> 4 AND ST_TRACK.STATUS <> 3)
				) as temp
GROUP BY temp.TRACKING_ID, temp.TITLE, temp.PRIORITY, temp.ISSUE_TYPE, temp.STATUS, temp.LAST_UPDATE, temp.RANK_ORDER,
		temp.FIRST_NAME, temp.LAST_NAME, temp.MANAGERS_NOTE, temp.PLANNED_RELEASE, temp.ACTUAL_RELEASE, temp.PRODUCT_NAME
ORDER BY STATUS, REQUIRED DESC
";
            return this.ProcessCommand(command);
        }
        #endregion

        #region SPD Manager Reports
        public DataTable GetByResolvedGroup(string user_range, string years_range)
        {
            string strCmdText = $@"
SELECT
    USER_ID, ST_USERS.FIRST_NAME + ' ' + LAST_NAME as 'Developer', YEAR(RESOLVED_ON) AS 'Year', COUNT(*) AS '# Resolved'
FROM
    ST_TRACK
INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_TRACK.RESOLVED_BY
WHERE 
    YEAR([RESOLVED_ON]) IN ({years_range}) AND RESOLVED_BY IN ({user_range}) AND ST_USERS.ACCOUNT_TYPE >= 0
 
GROUP BY USER_ID, FIRST_NAME, LAST_NAME, YEAR(RESOLVED_ON)
ORDER BY LAST_NAME, YEAR(RESOLVED_ON);"; //, YEAR(RESOLVED_ON), COUNT(*) desc";

            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }

        public DataTable GetByResolvedGroup(string user_range, DateTime start_date, DateTime? end_date)
        {
            //custom
            string date_range = $@"CAST([RESOLVED_ON] as date) >= '{start_date.ToShortDateString()}'";
            if (end_date != null)
            {
                date_range += $@"AND  CAST([RESOLVED_ON] as date) <= '{end_date.Value.ToShortDateString()}'";
            }

            string strCmdText = $@"
SELECT
    USER_ID, ST_USERS.FIRST_NAME + ' ' + LAST_NAME as 'Developer', YEAR(RESOLVED_ON) AS 'Year', COUNT(*) AS '# Resolved'
FROM
    ST_TRACK
INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_TRACK.RESOLVED_BY
WHERE 
    {date_range} AND RESOLVED_BY IN ({user_range}) AND ST_USERS.ACCOUNT_TYPE >= 0
 
GROUP BY USER_ID, FIRST_NAME, LAST_NAME, YEAR(RESOLVED_ON)
ORDER BY LAST_NAME, YEAR(RESOLVED_ON);"; //, YEAR(RESOLVED_ON), COUNT(*) desc";

            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }

        public DataTable GetByResolvedList(string user_range, string years_range)
        {
            string strCmdText = $@"
SELECT
    ST_TRACK.TRACKING_ID, ST_PRODUCT.NAME, ST_TRACK.TITLE, RESOLVED_ON, ST_USERS.FIRST_NAME + ' ' + LAST_NAME as 'Developer'
FROM
    ST_TRACK
INNER JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_TRACK.PRODUCT
INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_TRACK.RESOLVED_BY
WHERE 
    YEAR([RESOLVED_ON]) IN ({years_range}) AND RESOLVED_BY IN ({user_range}) AND ST_USERS.ACCOUNT_TYPE >= 0";

            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }

        public DataTable GetByResolvedList(string user_range, DateTime start_date, DateTime? end_date)
        {
            //custom
            string date_range = $@"CAST([RESOLVED_ON] as date) >= '{start_date.ToShortDateString()}'";
            if (end_date != null)
            {
                date_range += $@"AND  CAST([RESOLVED_ON] as date) <= '{end_date.Value.ToShortDateString()}'";
            }

            string strCmdText = $@"
SELECT
    ST_TRACK.TRACKING_ID, ST_PRODUCT.NAME, ST_TRACK.TITLE, RESOLVED_ON, ST_USERS.FIRST_NAME + ' ' + LAST_NAME as 'Developer'
FROM
    ST_TRACK
INNER JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_TRACK.PRODUCT
INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_TRACK.RESOLVED_BY
WHERE 
    {date_range} AND RESOLVED_BY IN ({user_range}) AND ST_USERS.ACCOUNT_TYPE >= 0";

            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }


        public DataTable GetBouncedList(string user_range, string years_range)
        {
            string strCmdText = $@"
SELECT * FROM (
SELECT 
	ST_HISTORY.TRACKING_ID, ST_PRODUCT.NAME, ST_TRACK.TITLE, RESOLVED_ON, ST_USERS.FIRST_NAME + ' ' + LAST_NAME as 'Developer', COUNT(*) AS 'Bounced' 

FROM ST_HISTORY
    INNER JOIN ST_TRACK ON ST_TRACK.TRACKING_ID = ST_HISTORY.TRACKING_ID
    INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_TRACK.RESOLVED_BY
	INNER JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_TRACK.PRODUCT
WHERE 
    ST_HISTORY.TYPE = 5 AND ST_HISTORY.MADE_BY = -1 AND ST_HISTORY.DESCRIPTION LIKE '%--- Actual Release Version%' 
    AND
    YEAR([RESOLVED_ON]) IN ({years_range}) AND RESOLVED_BY IN ({user_range}) AND ST_USERS.ACCOUNT_TYPE >= 0
    GROUP BY USER_ID, FIRST_NAME, LAST_NAME, RESOLVED_ON, ST_HISTORY.TRACKING_ID, ST_PRODUCT.NAME, ST_TRACK.TITLE
) as tmp
WHERE Bounced > 1";
            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }

        /// <summary>
        /// This returns a list of REAs that the usershas done a code review.
        /// If there are duplicate of REAs in the list it is because that user has done more than 
        /// one code review for that REA.
        /// </summary>
        /// <remarks>
        /// This is the query used for displaying the details of a particular developer, 
        /// so the user is usually just one person. The Year field is not displayed but the 
        /// number of columns matter in the UI
        /// </remarks>
        /// <param name="user_range">a comma delimited list of users</param>
        /// <param name="years_range">a comma delimited list of years</param>
        /// <returns>DataTable</returns>
        public DataTable GetReviewedList(string user_range, string years_range)
        {
            string strCmdText = $@"

SELECT 
	ST_TRACK.TRACKING_ID, ST_PRODUCT.NAME, ST_TRACK.TITLE, YEAR(CREATED_ON) AS 'Year', ST_USERS.FIRST_NAME + ' ' + LAST_NAME as 'Developer'

FROM
	ST_CODE_REVIEW_INFO
LEFT JOIN ST_CODE_REVIEW ON ST_CODE_REVIEW.ID = ST_CODE_REVIEW_INFO.CODE_REVIEW_ID
INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_CODE_REVIEW.CREATED_BY
LEFT JOIN ST_TRACK ON ST_TRACK.TRACKING_ID = ST_CODE_REVIEW.TRACKING_ID
INNER JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_TRACK.PRODUCT
WHERE
    YEAR([CREATED_ON]) IN ({years_range}) AND ST_CODE_REVIEW_INFO.USER_ID IN ({user_range}) 
";
            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }

        public DataTable GetReviewedList(string user_range, DateTime start_date, DateTime? end_date)
        {
            //custom
            string date_range = $@"CAST([CREATED_ON] as date) >= '{start_date.ToShortDateString()}' ";
            if (end_date != null)
            {
                date_range += $@"AND  CAST([CREATED_ON] as date) <= '{end_date.Value.ToShortDateString()}' ";
            }

            string strCmdText = $@"
SELECT 
	ST_TRACK.TRACKING_ID, ST_PRODUCT.NAME, ST_TRACK.TITLE, '{start_date.ToShortDateString()} to {end_date.Value.ToShortDateString()}' AS 'Year', ST_USERS.FIRST_NAME + ' ' + LAST_NAME as 'Developer'

FROM
	ST_CODE_REVIEW_INFO
LEFT JOIN ST_CODE_REVIEW ON ST_CODE_REVIEW.ID = ST_CODE_REVIEW_INFO.CODE_REVIEW_ID
INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_CODE_REVIEW.CREATED_BY
LEFT JOIN ST_TRACK ON ST_TRACK.TRACKING_ID = ST_CODE_REVIEW.TRACKING_ID
INNER JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_TRACK.PRODUCT
WHERE
     {date_range} AND ST_CODE_REVIEW_INFO.USER_ID IN ({user_range})

";

            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }


        public DataTable GetProductCloseRate(string product_range, DateTime start_date, DateTime? end_date)
        {
            //custom
            string date_range = $@"CAST([CREATED_ON] as date) >= '{start_date.ToShortDateString()}' ";
            if (end_date != null)
            {
                date_range += $@"AND  CAST([CREATED_ON] as date) <= '{end_date.Value.ToShortDateString()}' ";
            }

            string strCmdText = $@"
SELECT 
	ST_PRODUCT.NAME as 'PRODUCT', 
	ST_HISTORY.TRACKING_ID as 'REA #', 
	ST_HISTORY.MADE_ON as 'BUILT ON', 
	ST_TRACK.CLOSED_ON as 'CLOSED ON', 
	( 
		SELECT COUNT(*) AS NUMDAYS FROM CALENDAR WHERE (CALENDAR.DT >= ST_HISTORY.MADE_ON AND CALENDAR.DT <= ST_TRACK.CLOSED_ON) AND IsWorkDay=1
	) AS 'BUILT TO CLOSE'
	
FROM ST_HISTORY 
	INNER JOIN ST_TRACK ON ST_TRACK.TRACKING_ID = ST_HISTORY.TRACKING_ID
	INNER JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_TRACK.PRODUCT
WHERE 
	{date_range} 
	AND ST_TRACK.CLOSED_REASON=2
	AND ST_HISTORY.TYPE=2 AND ST_HISTORY.MADE_BY=-1 AND ST_HISTORY.DESCRIPTION LIKE '%Built in release%'
	AND ST_TRACK.PRODUCT IN ({product_range}) 
ORDER BY
	ST_PRODUCT.NAME, 
	ST_HISTORY.TRACKING_ID   
";
            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }

        public DataTable GetProductCloseRate(string product_range, string years_range)
        {
            string strCmdText = $@"
SELECT 
	ST_PRODUCT.NAME as 'PRODUCT', 
	ST_HISTORY.TRACKING_ID as 'REA #', 
	ST_HISTORY.MADE_ON as 'BUILT ON', 
	ST_TRACK.CLOSED_ON as 'CLOSED ON', 
	( 
		SELECT COUNT(*) AS NUMDAYS FROM CALENDAR WHERE (CALENDAR.DT >= ST_HISTORY.MADE_ON AND CALENDAR.DT <= ST_TRACK.CLOSED_ON) AND IsWorkDay=1
	) AS 'BUILT TO CLOSE'
	
FROM ST_HISTORY 
	INNER JOIN ST_TRACK ON ST_TRACK.TRACKING_ID = ST_HISTORY.TRACKING_ID
	INNER JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_TRACK.PRODUCT
WHERE 
	YEAR([CLOSED_ON]) IN ({years_range}) 
	AND ST_TRACK.CLOSED_REASON=2
	AND ST_HISTORY.TYPE=2 AND ST_HISTORY.MADE_BY=-1 AND ST_HISTORY.DESCRIPTION LIKE '%Built in release%'
	AND ST_TRACK.PRODUCT IN ({product_range})
ORDER BY
	ST_PRODUCT.NAME, 
	ST_HISTORY.TRACKING_ID 
";
            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }
        #endregion

        #region Time Reports
        /// <summary>
        /// 
        /// </summary>
        /// <param name="employee_number">Single employee ID number</param>
        /// <param name="years_range">years, common-delimited</param>
        /// <returns></returns>
        public DataTable GetHoursByWeek(int userID, string years_range)
        {
            string date_range = "";
            if (years_range.Length > 0)
            {
                date_range = $@"AND YEAR(WeekEndingDate) IN ({years_range})";
            }

            string strCmdText = $@"
SELECT *, (
	ISNULL([58100], 0) +
	ISNULL([58110], 0) + 
	ISNULL([58120], 0) + 
	ISNULL([58130], 0) + 
	ISNULL([58170], 0) + 
	ISNULL([58190], 0) +
	ISNULL([58312], 0) + 
	ISNULL([99999], 0)) AS 'Total',
	((
	ISNULL([58100], 0) +
	ISNULL([58110], 0) + 
	ISNULL([58120], 0) + 
	ISNULL([58130], 0) + 
	ISNULL([58170], 0) + 
	ISNULL([58190], 0) +
	ISNULL([58312], 0) + 
	ISNULL([99999], 0)) - 40 - (ISNULL([58170], 0) + ISNULL([58190], 0))) AS 'PST/ET'
FROM
(
	SELECT WeekEndingDate, ACCOUNT, SUM(HOURS) AS 'Hours' FROM dbo.TST_TIME_ENTRY 
	INNER JOIN TST_TIMECARDS on TST_TIMECARDS.ID = TST_TIME_ENTRY.TIMECARD_ID

	WHERE TST_TIMECARDS.UserID={userID} {date_range}
	GROUP BY WeekEndingDate, ACCOUNT


) AS SourceTable PIVOT(SUM([Hours]) FOR [Account] IN ([58100],
                                                         [58110],
                                                         [58120],
														 [58130],
														 [58170],
                                                         [58190],
                                                         [58312],
														 [99999])) AS PivotTable;
";

            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }

        public DataTable GetHoursByWeek(int userID, DateTime? start_date, DateTime? end_date)
        {
            //create the 4 possible date clause.
            // 1) no start date, only end date
            // 2) start date and no end date
            // 3) start date and end date
            // 4) no dates at all
            string date_range = "";
            if (start_date != null)
            {
                date_range += $@"CAST([WeekEndingDate] as date) >= '{start_date?.ToShortDateString()}'";
            }
            if (end_date != null)
            {
                if (start_date != null)
                {
                    date_range += " AND ";
                }
                date_range += $@"CAST([WeekEndingDate] as date) <= '{end_date?.ToShortDateString()}'";
            }
            if (date_range.Length > 0)
            {
                date_range = " AND " + date_range;
            }

            string strCmdText = $@"
SELECT *, (
	ISNULL([58100], 0) +
	ISNULL([58110], 0) + 
	ISNULL([58120], 0) + 
	ISNULL([58130], 0) + 
	ISNULL([58170], 0) + 
	ISNULL([58190], 0) +
	ISNULL([58312], 0) + 
	ISNULL([99999], 0)) AS 'Total',
	((
	ISNULL([58100], 0) +
	ISNULL([58110], 0) + 
	ISNULL([58120], 0) + 
	ISNULL([58130], 0) + 
	ISNULL([58170], 0) + 
	ISNULL([58190], 0) +
	ISNULL([58312], 0) + 
	ISNULL([99999], 0)) - 40 - (ISNULL([58170], 0) + ISNULL([58190], 0))) AS 'PST/ET'
FROM
(
	SELECT WeekEndingDate, ACCOUNT, SUM(HOURS) AS 'Hours' FROM dbo.TST_TIME_ENTRY 
	INNER JOIN TST_TIMECARDS on TST_TIMECARDS.ID = TST_TIME_ENTRY.TIMECARD_ID
	WHERE TST_TIMECARDS.UserID={userID} {date_range}
	GROUP BY WeekEndingDate, ACCOUNT


) AS SourceTable PIVOT(SUM([Hours]) FOR [Account] IN ([58100],
                                                         [58110],
                                                         [58120],
														 [58130],
														 [58170],
                                                         [58190],
                                                         [58312],
														 [99999])) AS PivotTable;
";

            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }

        public DataTable GetHoursByYear(int userID, string years_range)
        {
            string date_range = "";
            if (years_range.Length > 0)
            {
                date_range = $@"AND YEAR(WeekEndingDate) IN ({years_range})";
            }

            string strCmdText = $@"
SELECT 
YEAR(T.WeekEndingDate) AS 'Year',
ISNULL(SUM(T.[58100]), 0) AS '58100',
ISNULL(SUM(T.[58110]), 0) AS '58110',
ISNULL(SUM(T.[58120]), 0) AS '58120',
ISNULL(SUM(T.[58130]), 0) AS '58130',
ISNULL(SUM(T.[58170]), 0) AS '58170',
ISNULL(SUM(T.[58190]), 0) AS '58190',
ISNULL(SUM(T.[58312]), 0) AS '58312',
ISNULL(SUM(T.[99999]), 0) AS '99999',
ROUND((ISNULL(SUM(T.[58100]), 0) + ISNULL(SUM(T.[58110]), 0) + ISNULL(SUM(T.[58120]), 0)  + ISNULL(SUM(T.[58130]), 0) + ISNULL(SUM(T.[58170]), 0) + ISNULL(SUM(T.[58190]), 0) + ISNULL(SUM(T.[58312]), 0) + ISNULL(SUM(T.[99999]), 0)), 2) AS 'TOTAL',

(count(T.Employee)*40) AS 'Exp Hours',

ROUND(
((ISNULL(SUM(T.[58100]), 0) + ISNULL(SUM(T.[58110]), 0) + ISNULL(SUM(T.[58120]), 0)  + ISNULL(SUM(T.[58130]), 0) + ISNULL(SUM(T.[58170]), 0) + ISNULL(SUM(T.[58190]), 0) + ISNULL(SUM(T.[58312]), 0) + ISNULL(SUM(T.[99999]), 0))
- (count(T.Employee)*40)
- (ISNULL(SUM(T.[58170]), 0) + ISNULL(SUM(T.[58190]), 0))
), 2) AS 'PST/ET'

FROM (
SELECT *
FROM
(
	SELECT 
		(ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME) AS 'Employee', WeekEndingDate, TST_TIME_ENTRY.ACCOUNT, SUM(HOURS) AS 'Hours' 
		
	FROM dbo.TST_TIME_ENTRY
	INNER JOIN TST_TIMECARDS on TST_TIMECARDS.ID = TST_TIME_ENTRY.TIMECARD_ID
	INNER JOIN ST_USERS ON ST_USERS.USER_ID = TST_TIMECARDS.UserID
	WHERE TST_TIMECARDS.UserID={userID} {date_range}
	GROUP BY (ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME), WeekEndingDate, TST_TIME_ENTRY.ACCOUNT


) AS SourceTable PIVOT(SUM([Hours]) FOR [Account] IN ([58100],
                                                         [58110],
                                                         [58120],
														 [58130],
														 [58170],
                                                         [58190],
                                                         [58312],
														 [99999])) AS PivotTable
														 ) AS T

GROUP BY YEAR(WeekEndingDate)
ORDER BY YEAR(WeekEndingDate)";

            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }

        public DataTable GetHours(int userID, string years_range)
        {
            string date_range = "";
            if (years_range.Length > 0)
            {
                date_range = $@"AND YEAR(WeekEndingDate) IN ({years_range})";
            }

            //not displaying description as it is currently not collected
            //not displaying notes as it is currently not collected
            //not displaying tracking_id as it coverted from task
            //not displaying employee as this is run only for one person

            string strCmdText = $@"
	            SELECT TST_TIME_ENTRY.ID, WeekEndingDate, ACCOUNT, PROJECT, Description, TASK, SUBTASK, HOURS
                FROM dbo.TST_TIME_ENTRY
            	INNER JOIN TST_TIMECARDS on TST_TIMECARDS.ID = TST_TIME_ENTRY.TIMECARD_ID
            	WHERE TST_TIMECARDS.UserID={userID} {date_range}
                ORDER BY WeekEndingDate ASC, ACCOUNT ASC, PROJECT ASC";

            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;

        }

        public DataTable GetHours(int userID, DateTime? start_date, DateTime? end_date)
        {
            //create the 4 possible date clause.
            // 1) no start date, only end date
            // 2) start date and no end date
            // 3) start date and end date
            // 4) no dates at all
            string date_range = "";
            if (start_date != null)
            {
                date_range += $@"CAST([WeekEndingDate] as date) >= '{start_date?.ToShortDateString()}'";
            }
            if (end_date != null)
            {
                if (start_date != null)
                {
                    date_range += " AND ";
                }
                date_range += $@"CAST([WeekEndingDate] as date) <= '{end_date?.ToShortDateString()}'";
            }
            if (date_range.Length > 0)
            {
                date_range = " AND " + date_range;
            }

            //not displaying description as it is currently not collected
            //not displaying notes as it is currently not collected
            //not displaying tracking_id as it coverted from task
            //not displaying employee as this is run only for one person

            string strCmdText = $@"
	            SELECT TST_TIME_ENTRY.ID, WeekEndingDate, ACCOUNT, PROJECT, Description, TASK, SUBTASK, HOURS
                FROM dbo.TST_TIME_ENTRY
            	INNER JOIN TST_TIMECARDS on TST_TIMECARDS.ID = TST_TIME_ENTRY.TIMECARD_ID
            	WHERE TST_TIMECARDS.UserID={userID} {date_range}
                ORDER BY WeekEndingDate ASC, ACCOUNT ASC, PROJECT ASC";

            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;

        }
        public int DoesTimeCardExist(int userID, DateTime weekend)
        {
            int timecardID = 0;

            string cmdtext = $@"SELECT ID FROM [dbo].[TST_TIMECARDS] WHERE UserID={userID} and [WeekEndingDate]='{weekend.ToShortDateString()}'";

            try
            {
                timecardID = (int)ProcessScalarCommand(cmdtext);
            }
            catch
            {

            }
            return timecardID;
        }

        public bool DoesTimeCardExist(int timecardID)
        {
            string cmdtext = $@"SELECT count(*) FROM [dbo].[TST_TIMECARDS] WHERE ID={timecardID}";
            bool exists = false;
            try
            {
                exists = ((int)ProcessScalarCommand(cmdtext) == 1);
            }
            catch
            {

            }
            return exists;
        }

        public int CreateTimeCard(int userID, int companyID, int deptID, DateTime weekend, float expected, string group, string filename, Byte[] bytes)
        {
            int timecardID = 0;

            if ((timecardID = DoesTimeCardExist(userID, weekend)) > 0)
            {
                return timecardID;
            }

            string cmdtext = $@"
            INSERT INTO [dbo].[TST_TIMECARDS] 
                ([UserID],[CompanyID],[DeptID],[Group],[WeekEndingDate],[ExpectedHours],[FILE_NAME],[FILE_DATA])
            VALUES 
                (@USER_ID, @COMPANY_ID, @DEPT_ID, @GROUP, @WEEKEND, @EXP_HOURS, @FILE_NAME, @EXCEL_FILE)

            SELECT CAST(scope_identity() AS int);";

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    SqlCommand sqlCommander = new SqlCommand(cmdtext, connection);
                    sqlCommander.CommandText = cmdtext;
                    sqlCommander.CommandType = CommandType.Text;

                    //Create Sql parameters
                    sqlCommander.Parameters.Add("@USER_ID", SqlDbType.Int);
                    sqlCommander.Parameters["@USER_ID"].Value = userID;

                    sqlCommander.Parameters.Add("@COMPANY_ID", SqlDbType.Int);
                    sqlCommander.Parameters["@COMPANY_ID"].Value = companyID;

                    sqlCommander.Parameters.Add("@DEPT_ID", SqlDbType.Int);
                    sqlCommander.Parameters["@DEPT_ID"].Value = deptID;

                    sqlCommander.Parameters.Add("@EXP_HOURS", SqlDbType.Float);
                    sqlCommander.Parameters["@EXP_HOURS"].Value = expected;

                    sqlCommander.Parameters.Add("@WEEKEND", SqlDbType.Date);
                    sqlCommander.Parameters["@WEEKEND"].Value = weekend.ToShortDateString();

                    sqlCommander.Parameters.Add("@GROUP", SqlDbType.NVarChar);
                    sqlCommander.Parameters["@GROUP"].Value = string.IsNullOrEmpty(group) ? (object)DBNull.Value : group;
                    sqlCommander.Parameters["@GROUP"].Size = 10;

                    sqlCommander.Parameters.Add("@FILE_NAME", SqlDbType.NVarChar);
                    sqlCommander.Parameters["@FILE_NAME"].Value = string.IsNullOrEmpty(filename) ? (object)DBNull.Value : filename;
                    sqlCommander.Parameters["@FILE_NAME"].Size = 255;

                    sqlCommander.Parameters.Add("@EXCEL_FILE", SqlDbType.Image);
                    sqlCommander.Parameters["@EXCEL_FILE"].Value = (bytes.Length == 0) ? (object)DBNull.Value : bytes;                    

                    timecardID = Convert.ToInt32(sqlCommander.ExecuteScalar());

                    connection.Close();
                }
                
            }
            catch (Exception ex)
            {
                Utility.Logging.WriteEntry(ex);
            }
            return timecardID;
        }

        public int CreateTimeEntry(int timeCardID, int account, string description, string project, string task, string subtask, double hours, int? trackingID = null)
        {
            int timeEntryID = 0;

            string cmdtext = $@"
            INSERT INTO [dbo].[TST_TIME_ENTRY] 
                ([TIMECARD_ID],[ACCOUNT],[DESCRIPTION],[PROJECT],[TASK],[SUBTASK],[HOURS],[TRACKING_ID])
            VALUES 
                (@TIMECARD_ID, @ACCOUNT, @DESCRIPTION, @PROJECT, @TASK, @SUBTASK, @HOURS, @TRACKING_ID)

            SELECT CAST(scope_identity() AS int);";

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    SqlCommand sqlCommander = new SqlCommand(cmdtext, connection);
                    sqlCommander.CommandText = cmdtext;
                    sqlCommander.CommandType = CommandType.Text;

                    //Create Sql parameters
                    sqlCommander.Parameters.Add("@TIMECARD_ID", SqlDbType.Int);
                    sqlCommander.Parameters["@TIMECARD_ID"].Value = timeCardID;

                    sqlCommander.Parameters.Add("@ACCOUNT", SqlDbType.Int);
                    sqlCommander.Parameters["@ACCOUNT"].Value = account;

                    sqlCommander.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar);
                    sqlCommander.Parameters["@DESCRIPTION"].Value = string.IsNullOrEmpty(description) ? (object)DBNull.Value : description;
                    sqlCommander.Parameters["@DESCRIPTION"].Size = int.MaxValue;

                    sqlCommander.Parameters.Add("@PROJECT", SqlDbType.NVarChar);
                    sqlCommander.Parameters["@PROJECT"].Value = string.IsNullOrEmpty(project) ? (object)DBNull.Value : project;
                    sqlCommander.Parameters["@PROJECT"].Size = 50;

                    sqlCommander.Parameters.Add("@TASK", SqlDbType.NVarChar);
                    sqlCommander.Parameters["@TASK"].Value = string.IsNullOrEmpty(task) ? (object)DBNull.Value : task;
                    sqlCommander.Parameters["@TASK"].Size = 100;

                    sqlCommander.Parameters.Add("@SUBTASK", SqlDbType.NVarChar);
                    sqlCommander.Parameters["@SUBTASK"].Value = string.IsNullOrEmpty(subtask) ? (object)DBNull.Value : subtask;
                    sqlCommander.Parameters["@SUBTASK"].Size = 50;

                    sqlCommander.Parameters.Add("@HOURS", SqlDbType.Float);
                    sqlCommander.Parameters["@HOURS"].Value = hours;

                    sqlCommander.Parameters.Add("@TRACKING_ID", SqlDbType.Int);
                    sqlCommander.Parameters["@TRACKING_ID"].Value = ((trackingID == null) || ((int)trackingID == 0)) ? (object)DBNull.Value : (int)trackingID;


                    timeEntryID = Convert.ToInt32(sqlCommander.ExecuteScalar());

                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                Utility.Logging.WriteEntry(ex);
            }

            return timeEntryID;
        }
        #endregion
    }
}
