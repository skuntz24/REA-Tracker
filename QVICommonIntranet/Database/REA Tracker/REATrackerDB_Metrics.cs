using System;
using System.Data;
using System.Data.SqlClient;

namespace QVICommonIntranet.Database
{
    /// <summary>
    /// This file contains the code mostely dealing with the calculations of metrics and counts.
    /// Count is the summation of number of records. For example, 5 High REAs is count of 5.
    /// Metrics is the sum of the squares formula applied to the priority of an REA.
    /// The values of the priority field never changed by are in the database (instead of hardcoded) just 
    /// in case the values need to change
    /// Low Priority = 1, Medium = 2, High = 3, Critical = 4
    /// Metric = low count * (low value * low value) + medium count * (medium value * medium value) + ...
    /// Metric for one Low REA = 1, one Medium REA = 4, one High REA = 9, one Critical REA = 16
    /// </summary>
    public partial class REATrackerDB
    {
        /// <summary>
        /// Calculates the metric for a specific product where the ISSUE_TYPE = 1 (bug) and 
        /// the STATUS IN (1,2,5,6,7) (submitted, approved, in process, fixed or delivered)
        /// Sample Output:
        /// PRIORITY	NAME	    Count	Metric
        /// 1	        Low	        155	    155
        /// 2	        Medium	    83	    332
        /// 3	        High	    4	    36
        /// 4	        Critical	3	    48
        /// </summary>
        /// <param name="product">ID for the product</param>
        /// <returns>DataTable with 4 rows w/ 4 columns</returns>
        public DataTable GetPriorityMetrics(int productID)
        {
            string strCmdText = @"
	        SELECT
		        ST_TRACK.PRIORITY,
		        ST_PRIORITY.NAME,
		        COUNT(*) as 'Count', 
		        COUNT(*) * PRIORITY * PRIORITY AS 'Metric'
	        FROM 
		        ST_TRACK 

	        INNER JOIN ST_PRIORITY ON ST_PRIORITY.ID = ST_TRACK.PRIORITY

	        WHERE 
		        ST_TRACK.PRODUCT IN (" + productID + @") AND (ISSUE_TYPE = 1) AND (STATUS IN (1,2,5,6,7))
	        GROUP BY 
		        ST_TRACK.PRIORITY, ST_PRIORITY.NAME
	
	        ORDER BY 
		        ST_TRACK.PRIORITY ASC
            ";
            System.Data.DataTable dt = ProcessCommand(strCmdText);

            return dt;
        }


        public DataTable GetChildrenMetrics(int PID)
        {
            string a = "SELECT DISTINCT ID, ST_PRODUCT.PRODUCT_ID AS 'PROD_ID', RELEASE_CRITICAL_COUNT AS 'Metric #1'," +
               "RELEASE_CRITICAL_COUNT AS 'Metric #1', RELEASE_HIGH_COUNT AS 'Metric #2', " +
               "RELEASE_DEFECT_METRIC AS 'Metric #3', RELEASE_DEFECT_COUNT AS 'Metric #4'  FROM ST_PRODUCT_METRICS " +
               "FULL JOIN ST_PRODUCT ON ST_PRODUCT_METRICS.PRODUCT_ID = ST_PRODUCT.PRODUCT_ID " +
               "WHERE ST_PRODUCT.PRODUCT_ID in ( " + PID +" )  ";


            System.Data.DataTable dt = ProcessCommand(a);
            return dt;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <returns></returns>
        /// function for viewallmetrics report, similar to scr_GetMetricsForProduct except we filter on major and minor numbers
        public DataTable GetPriorityMetrics(int productID, string major, string minor)
        {
            string strCmdText = @"
            SELECT 
                ST_TRACK.PRIORITY, 
                ST_PRIORITY.NAME, 
                COUNT(*) as 'Count', 
                COUNT(*) * PRIORITY * PRIORITY AS 'Metric'
            FROM 
                ST_TRACK 
            INNER JOIN ST_PRIORITY ON ST_PRIORITY.ID = ST_TRACK.PRIORITY 
            WHERE 
                ST_TRACK.PRODUCT IN (" + productID + @") AND (ISSUE_TYPE = 1) AND (STATUS IN (1,2,5,6,7))  
                AND ((VP_MAJOR IS NULL AND VP_MINOR IS NULL AND VP_BUILD IS NULL AND VP_MISC IS NULL) OR (VP_MAJOR = " + major + @" AND VP_MINOR = " + minor + @"))
            GROUP BY 
                ST_TRACK.PRIORITY, ST_PRIORITY.NAME 
            ORDER BY 
                ST_TRACK.PRIORITY ASC;
            ";
            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        

    }

    /// <summary>
    /// Sample Output:
    /// ISSUE_TYPE	NAME	        Count	Metric
    /// 1	        Problem	        245	    245
    /// 2	        Enhancement	    161	    644
    /// 3	        Planned Work	52	    468
    /// </summary>
    /// <param name="productID"></param>
    /// <returns>DataTable with 3 rows w/ 4 columns</returns>
    public DataTable GetIssueMetrics(int productID)
        {
            string strCmdText = @"
	        SELECT 
		        ST_TRACK.ISSUE_TYPE,
		        ST_ISSUE_TYPE.NAME,
		        COUNT(*) AS 'Count',
		        COUNT(*) * ISSUE_TYPE * ISSUE_TYPE AS 'Metric'
	        FROM
		        ST_TRACK
	        INNER JOIN ST_ISSUE_TYPE ON ST_ISSUE_TYPE.ISSUE_ID = ST_TRACK.ISSUE_TYPE
	        WHERE 
		        ST_TRACK.PRODUCT IN (" + productID + @") AND (STATUS IN (1,2,5,6,7))		
	        GROUP BY
		        ST_TRACK.ISSUE_TYPE, ST_ISSUE_TYPE.NAME
	        ORDER BY
		        ST_TRACK.ISSUE_TYPE ASC
            ";
            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }
        public DataTable GetStatusMetrics(int productID)
        {
            string strCmdText = $@"
	        SET NOCOUNT ON;

	        SELECT SUM(c1) as 'Submitted', SUM(c2) as 'Approved', SUM(c3) as 'In Process', SUM(c4) as 'Fixed or Delivered', SUM(c5) as 'Testing', SUM(c6) as 'Deferred' FROM
	        (
		        SELECT COUNT(*) AS 'c1', 0 AS 'c2', 0 as 'c3', 0 as 'c4', 0 as 'c5', 0 as 'c6' FROM REA_TRACK WHERE REA_TRACK.PRODUCT = {productID} AND REA_TRACK.STATUS = 1
		        UNION
		        SELECT 0 AS 'c1', COUNT(*) AS 'c2', 0 as 'c3', 0 as 'c4', 0 as 'c5', 0 as 'c6' FROM REA_TRACK WHERE REA_TRACK.PRODUCT = {productID} AND REA_TRACK.STATUS in (2)
		        UNION
		        SELECT 0 AS 'c1', 0 as 'c2', COUNT(*) AS 'c3', 0 as 'c4', 0 as 'c4', 0 as 'c6' FROM REA_TRACK WHERE REA_TRACK.PRODUCT = {productID} AND REA_TRACK.STATUS in (5)
		        UNION
		        SELECT 0 AS 'c1', 0 AS 'c2', 0 as 'c3', COUNT(*) as 'c4', 0 as 'c5', 0 as 'c6' FROM REA_TRACK WHERE REA_TRACK.PRODUCT = {productID} AND REA_TRACK.STATUS in (6, 7)
		        UNION
		        SELECT 0 AS 'c1', 0 AS 'c2', 0 as 'c3', 0 as 'c4', COUNT(*) as 'c5', 0 as 'c6' FROM REA_TRACK WHERE REA_TRACK.PRODUCT = {productID} AND REA_TRACK.STATUS in (8, 10, 11)
		        UNION
		        SELECT 0 AS 'c1', 0 AS 'c2', 0 as 'c3', 0 as 'c4', 0 as 'c5', COUNT(*) as 'c6' FROM REA_TRACK WHERE REA_TRACK.PRODUCT = {productID} AND REA_TRACK.STATUS = 3
	        ) as temp
            ";

            System.Data.DataTable dt = ProcessCommand(strCmdText);
            return dt;
        }
        /// <summary>
        /// Creates a SQL statement to return the metric count/sum of squares for products.
        /// Use the strWhereClause parameter to limit the query based on products, issue types and status
        /// </summary>
        /// <param name="strWhereClause">require where clause for the SQL statement</param>
        /// <returns></returns>
        private string CreateMetricCommand(string strWhereClause)
        {
            string sql =
 @"
	        SELECT 
		        [Product], 
		        [Low], 
		        [Medium], 
		        [High], 
		        [Critical], 
		        (LOW + MEDIUM*4 + HIGH*9 + CRITICAL*16) as 'Sum of Squares',
		        (LOW + MEDIUM + HIGH + CRITICAL) as 'Total Open',
		        [Deferred],
		        ID
	        FROM
	        (
		        SELECT 
                    REA_PRODUCT.NAME AS [Product],
		            [LOW] = SUM(CASE WHEN PRIORITY = 1 AND REA_TRACK.STATUS <> 3 THEN 1 ELSE 0 END),
		            [MEDIUM] = SUM(CASE WHEN PRIORITY = 2 AND REA_TRACK.STATUS <> 3  THEN 1 ELSE 0 END),
		            [HIGH] = SUM(CASE WHEN PRIORITY = 3 AND REA_TRACK.STATUS <> 3  THEN 1 ELSE 0 END),
		            [CRITICAL] = SUM(CASE WHEN PRIORITY = 4 AND REA_TRACK.STATUS <> 3  THEN 1 ELSE 0 END),
		            [Deferred] = SUM(CASE WHEN REA_TRACK.STATUS = 3 THEN 1 ELSE 0 END),
		            REA_PRODUCT.PRODUCT_ID AS ID
		        FROM REA_TRACK

		        INNER JOIN REA_PRODUCT ON PRODUCT = REA_PRODUCT.PRODUCT_ID ";

            sql += "WHERE " + strWhereClause;

            sql += @"		        
		        GROUP BY REA_PRODUCT.NAME, REA_PRODUCT.PRODUCT_ID
	        )A
	        ORDER BY [Product]";
            return sql;
        }
        /// <summary>
        /// Calculates the metrics for ALL ACTIVE products using all REAs that havent been closed (developer and test sides)
        /// </summary>
        /// <returns></returns>
        public DataTable GetMetricReportAll()
        {
            string strCmdText = CreateMetricCommand("(REA_TRACK.STATUS IN (1,2,3,5,6,7,8,11) AND REA_PRODUCT.RETIRED=0)");
            return this.ProcessCommand(strCmdText);
        }

        /// <summary>
        /// Calculates the metrics for ALL ACTIVE products only using Problem REAs that havent been built (developer side)
        /// </summary>
        /// <returns></returns>
        public DataTable GetMetricReportProblem()
        {
            string strCmdText = CreateMetricCommand("(REA_TRACK.STATUS IN (1,2,3,5,6,7) AND REA_TRACK.ISSUE_TYPE=1 AND REA_PRODUCT.RETIRED=0)");
            return this.ProcessCommand(strCmdText);
        }

        /// <summary>
        /// Calculates the metrics for the specified products only using Problem REAs that havent been built (developer side)
        /// </summary>
        /// <param name="productIDString">List of Product IDs in a common delimited string</param>
        /// <returns></returns>
        public DataTable GetMetricReportProblem(string productIDString)
        {
            string strCmdText = CreateMetricCommand("(REA_TRACK.STATUS IN (1,2,3,5,6,7) AND REA_TRACK.ISSUE_TYPE=1 AND REA_TRACK.PRODUCT IN (" + productIDString + "))");
            return this.ProcessCommand(strCmdText);
        }

        /// <summary>
        /// Calculates the metrics for the specified products using all REAs that havent been closed (developer and test sides)
        /// </summary>
        /// <param name="productIDString">List of Product IDs in a common delimited string</param>
        /// <returns></returns>
        public DataTable GetMetricReportAll(string productIDString)
        {
            string strCmdText = CreateMetricCommand("(ST_TRACK.STATUS IN (1,2,3,5,6,7,8,11) AND ST_TRACK.PRODUCT IN (" + productIDString + "))");
            return this.ProcessCommand(strCmdText);
        }

        public DataTable GetMetrics(int productID, int major, int minor)
        {
            string strWhere = "(ST_TRACK.STATUS IN (1,2,5,6,7) AND ST_TRACK.ISSUE_TYPE = 1 AND ST_TRACK.PRODUCT IN (" + productID + "))";
            strWhere += "AND ((VP_MAJOR IS NULL AND VP_MINOR IS NULL AND VP_BUILD IS NULL AND VP_MISC IS NULL) OR (VP_MAJOR = " + major + " AND VP_MINOR = " + minor + "))";

            string strCmdText = CreateMetricCommand(strWhere);
            return this.ProcessCommand(strCmdText);
        }

        public DataTable GetMetrics2(int productID, int major, int minor)
        {
            string strWhere = "(ST_TRACK.STATUS IN (1,2,5) AND ST_TRACK.ISSUE_TYPE = 1 AND ST_TRACK.PRODUCT IN (" + productID + "))";
            strWhere += "AND ((VP_MAJOR IS NULL AND VP_MINOR IS NULL AND VP_BUILD IS NULL AND VP_MISC IS NULL) OR (VP_MAJOR = " + major + " AND VP_MINOR = " + minor + "))";

            string strCmdText = CreateMetricCommand(strWhere);
            return this.ProcessCommand(strCmdText);
        }

        /// <summary>
        /// Calculates the number of working days (inclusive). Start date must be less than End date
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>Number of Working Days</returns>
        public int GetNumWorkingDays(DateTime start, DateTime end)
        {
            int days = 0;

            String command = $@"SELECT COUNT(*) AS NUMDAYS FROM CALENDAR WHERE (CALENDAR.DT >= '{start.ToShortDateString()}' AND CALENDAR.DT <= '{end.ToShortDateString()}') AND IsWorkDay=1";

            days = Convert.ToInt32(ProcessScalarCommand(command));

            return days;
        }
    }
}
