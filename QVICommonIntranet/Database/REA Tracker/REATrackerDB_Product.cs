using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QVICommonIntranet.Database
{
    public partial class REATrackerDB
    {
        /// <summary>
        /// Returns a list of products in alphabetical order, defaulting to active products only
        /// </summary>
        /// <param name="showRetired">(Optional)Show retired products</param>
        /// <returns>DataTable with the list of products, or null</returns>
        public DataTable GetProductList(bool showRetired = false)
        {
            System.Data.DataTable dt = new DataTable("REA_PRODUCT");
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    ClearError();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM REA_PRODUCT";
                    command.CommandText += " ORDER BY REA_PRODUCT.NAME ASC";
                    using (SqlDataAdapter reader = new SqlDataAdapter(command))
                    {
                        //dt = new DataTable();
                        reader.Fill(dt);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="showRetired"></param>
        /// <returns></returns>
        public DataTable GetProductList(int userID, bool? showRetired = null)
        {
            System.Data.DataTable dt;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {

                    bool full_access = Convert.ToBoolean(ProcessScalarCommand("SELECT FULL_ACCESS FROM BTRACKER_NEW.dbo.ST_USERS WHERE USER_ID=" + userID));
                    DataTable product_access = ProcessCommand("SELECT * FROM REA_PRODUCT_ACCESS WHERE USERID = " + userID);

                    command.Connection = connection;
                    command.CommandType = CommandType.Text;

                    if (full_access)
                    {
                        command.CommandText = "SELECT REA_PRODUCT.PRODUCT_ID, REA_PRODUCT.NAME FROM REA_PRODUCT ";
                    }
                    else
                    {
                        string product_access_string = "";
                        if (product_access.Rows.Count != 0)
                        {
                            for (int i = 0; i < product_access.Rows.Count; i++)
                            {
                                if (i != 0)
                                {
                                    product_access_string += ",";
                                }
                                product_access_string += (Convert.ToString(product_access.Rows[i]["ProductID"]));
                            }
                        }
                        else
                        {
                            product_access_string += Convert.ToString(ProcessScalarCommand("SELECT DEFAULT_PRODUCT FROM BTRACKER_NEW.dbo.ST_USERS WHERE USER_ID= " + userID));
                        }
                        command.CommandText = "SELECT REA_PRODUCT.PRODUCT_ID, REA_PRODUCT.NAME FROM REA_PRODUCT WHERE REA_PRODUCT.PRODUCT_ID IN (" + product_access_string + ")";
                        if (showRetired != null)
                        {
                            if (!(Boolean)showRetired)
                            {
                                command.CommandText += " AND ST_PRODUCT.RETIRED = 0";
                            }
                        }
                        else
                        {
                            String UserPrefCommand = "SELECT ST_USERS.PRODUCT_DISPLAY FROM BTRACKER_NEW.dbo.ST_USERS WHERE ST_USERS.USER_ID = " + userID;
                            Boolean displayRetiredPref = Convert.ToBoolean(this.ProcessScalarCommand(UserPrefCommand));
                            if (!displayRetiredPref)
                            {
                                command.CommandText += " AND ST_PRODUCT.RETIRED = 0";
                            }
                        }
                    }

                    command.CommandText += " ORDER BY NAME";

                    //SqlParameter parameter = new SqlParameter();
                    //parameter.ParameterName = "@userID";
                    //parameter.SqlDbType = SqlDbType.Int;
                    //parameter.Direction = ParameterDirection.Input;
                    //parameter.Value = product;
                    //command.Parameters.Add(parameter);
                    //parameter.ParameterName = "@active";
                    //parameter.SqlDbType = SqlDbType.Int;
                    //parameter.Direction = ParameterDirection.Input;
                    //parameter.Value = product;
                    //command.Parameters.Add(parameter);

                    using (SqlDataAdapter reader = new SqlDataAdapter(command))
                    {
                        dt = new DataTable();
                        reader.Fill(dt);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// checks the full access first and gives the products lists for the users
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public DataSet GetProductList2(int userID)
        {
            DataSet ds = new DataSet("PRODUCTS");
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    ClearError();
                    bool showRetired = false;
                    bool fullAccess = false;
                    //the first thing to check is if the user is an Admin or has full access flag enable
                    using (System.Data.DataTable dtAccess = ProcessCommand("SELECT FULL_ACCESS, ACCOUNT_TYPE, PRODUCT_DISPLAY FROM ST_USERS WHERE ST_USERS.USER_ID = " + userID))
                    {
                        try
                        {
                            if (dtAccess.Rows.Count > 0)
                            {
                                showRetired = bool.Parse(dtAccess.Rows[0]["PRODUCT_DISPLAY"].ToString());
                                fullAccess = (dtAccess.Rows[0]["ACCOUNT_TYPE"].ToString().Equals("0") || (bool)dtAccess.Rows[0]["FULL_ACCESS"]);
                            }
                            else
                            {
                                Utility.Logging.WriteEntry($"(GetProductList2) No row found for User ID {userID}", System.Diagnostics.EventLogEntryType.Warning);
                            }
                        }
                        catch (Exception ex)
                        {
                            //if blows up, stick with the default                            
                            ProcessException(ex);
                        }
                    }

                    //The first table to create is the Favorites
                    string query = @"
                            SELECT REA_PRODUCT.PRODUCT_ID, REA_PRODUCT.NAME FROM REA_PRODUCT
	                        INNER JOIN REA_PRODUCT_ACCESS ON REA_PRODUCT_ACCESS.ProductID = REA_PRODUCT.PRODUCT_ID
	                        WHERE REA_PRODUCT_ACCESS.Favorite = 1 AND REA_PRODUCT_ACCESS.UserID=" + userID.ToString() + @"
                            ORDER BY REA_PRODUCT.NAME";
                    using (DataTable dtFavorite = ProcessCommand(query))
                    {
                        dtFavorite.TableName = "Favorites";
                        ds.Tables.Add(dtFavorite);
                    }

                    //The second table to create is for Active products only
                    if (fullAccess)
                    {
                        query = "SELECT REA_PRODUCT.PRODUCT_ID, REA_PRODUCT.NAME FROM REA_PRODUCT ORDER BY REA_PRODUCT.NAME";
                    }
                    else
                    {
                        query = @"
                            SELECT REA_PRODUCT.PRODUCT_ID, REA_PRODUCT.NAME FROM REA_PRODUCT
	                        INNER JOIN REA_PRODUCT_ACCESS ON REA_PRODUCT_ACCESS.ProductID = REA_PRODUCT.PRODUCT_ID
	                        WHERE REA_PRODUCT_ACCESS.UserID=" + userID.ToString() + @"
                            ORDER BY REA_PRODUCT.NAME";
                    }
                    using (DataTable dtActive = ProcessCommand(query))
                    {
                        dtActive.TableName = "Active";
                        ds.Tables.Add(dtActive);
                    }

                }
            }
            return ds;
        }

        /// <summary>
        /// creates a new product
        /// </summary>
        /// <param name="ProductName"></param>
        /// <param name="Description"></param>
        /// <param name="Validator"></param>
        /// <param name="ReleaseCoordinator"></param>
        /// <param name="ReleaseCoordinator2"></param>
        /// <param name="DefaultTester"></param>
        /// <param name="SCCB"></param>
        /// <param name="isInteral"></param>
        /// <param name="BillingCode"></param>
        /// <param name="DistributionList"></param>
        /// <returns></returns>
        public int CreateNewProduct(String ProductName, String Description, int Validator, int ReleaseCoordinator,
            int ReleaseCoordinator2, int DefaultTester, int SCCB, bool isInteral, int BillingCode, String DistributionList, int owner)
        {
            int productID = -1;            
            string command = @"
            INSERT INTO ST_PRODUCT 
                (ROW_VER, NAME, DESCRIPTION, VALIDATOR, RELEASE_COORDINATOR, RELEASE_COORDINATOR_2, DEFAULT_TESTER, SCCB, IS_INTERNAL, BILLING_CODE, RETIRED, DISTRIBUTION_LIST, OWNER)
            VALUES
                (1, @NAME, @DESCRIPTION, @VALIDATOR, @RELEASE_COORDINATOR, @RELEASECOORDINATOR_2, @DEFAULT_TESTER, @SCCB, @IS_INTERNAL, @BILLING_CODE, 0, @DISTRIBUTION_LIST, @OWNER);

            SELECT CAST(scope_identity() AS int)
            ";

            command = command.Replace("@NAME", "'" + ProductName.Trim() + "'");
            if (string.IsNullOrEmpty(Description))
            {
                command = command.Replace("@DESCRIPTION", "NULL");
            }
            else
            {
                command = command.Replace("@DESCRIPTION", "'" + Description.Trim() + "'");
            }
            command = command.Replace("@VALIDATOR", Validator.ToString());
            command = command.Replace("@RELEASE_COORDINATOR", ReleaseCoordinator.ToString());
            command = command.Replace("@OWNER", owner.ToString());
            //had to change the spelling of second release coordinator so that it doesnt get replace when doing the first one
            if (ReleaseCoordinator2 <= 0)
            {
                command = command.Replace("@RELEASECOORDINATOR_2", "NULL");
            }
            else
            {
                command = command.Replace("@RELEASECOORDINATOR_2", ReleaseCoordinator2.ToString());
            }
            command = command.Replace("@DEFAULT_TESTER", DefaultTester.ToString());
            if (SCCB <= 0)
            {
                command = command.Replace("@SCCB", "NULL");
            }
            else
            {
                command = command.Replace("@SCCB", SCCB.ToString());
            }
            command = command.Replace("@IS_INTERNAL", "'" + isInteral.ToString() + "'"); //True/False has to be in quotes
            command = command.Replace("@BILLING_CODE", BillingCode.ToString());
            if (string.IsNullOrEmpty(DistributionList))
            {
                command = command.Replace("@DISTRIBUTION_LIST", "NULL");
            }
            else
            {
                command = command.Replace("@DISTRIBUTION_LIST", "'" + DistributionList + "'");
            }
            productID = (int) ProcessScalarCommand(command);

            return productID;
            /*
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(_connectionString);
            command.Connection = connection;
            command.CommandText = "INSERT INTO ST_PRODUCT(ROW_VER, NAME, DESCRIPTION, VALIDATOR, RELEASE_COORDINATOR, RELEASE_COORDINATOR_2, DEFAULT_TESTER, SCCB, IS_INTERNAL, BILLING_CODE, RETIRED, DISTRIBUTION_LIST ) VALUES(1, '"
                 + ProductName + "', '" + Description + "', " + Validator + ", " + ReleaseCoordinator + ", " + ReleaseCoordinator2 + ", " + DefaultTester + " , " + SCCB + ", '" + isInteral + "' , " + BillingCode + " , 1 , '" + DistributionList + "');";
            connection.Open();

            SqlParameter NAME = new SqlParameter();
            SqlParameter DESCRIPTION = new SqlParameter();
            SqlParameter VALIDATOR = new SqlParameter();
            SqlParameter RELEASE_COORDINATOR = new SqlParameter();
            SqlParameter RELEASE_COORDINATOR_2 = new SqlParameter();
            SqlParameter DEFAULT_TESTER = new SqlParameter();
            SqlParameter SOFTWARE_CHANGE_COMMITEE_BOARD = new SqlParameter();
            SqlParameter IS_INTERNAL = new SqlParameter();
            SqlParameter BILLING_CODE = new SqlParameter();
            SqlParameter DISTRIBUTION_LIST = new SqlParameter();

            NAME.SqlDbType = SqlDbType.VarChar;
            DESCRIPTION.SqlDbType = SqlDbType.VarChar;
            VALIDATOR.SqlDbType = SqlDbType.Int;
            RELEASE_COORDINATOR.SqlDbType = SqlDbType.Int;
            RELEASE_COORDINATOR_2.SqlDbType = SqlDbType.Int;
            DEFAULT_TESTER.SqlDbType = SqlDbType.Int;
            SOFTWARE_CHANGE_COMMITEE_BOARD.SqlDbType = SqlDbType.Int;
            IS_INTERNAL.SqlDbType = SqlDbType.Bit;
            BILLING_CODE.SqlDbType = SqlDbType.Int;
            DISTRIBUTION_LIST.SqlDbType = SqlDbType.VarChar;

            NAME.ParameterName = "NAME";
            DESCRIPTION.ParameterName = "DESCRIPTION";
            VALIDATOR.ParameterName = "VALIDATOR";
            RELEASE_COORDINATOR.ParameterName = "RELEASE_COORDINATOR";
            RELEASE_COORDINATOR_2.ParameterName = "RELEASE_COORDINATOR_2";
            DEFAULT_TESTER.ParameterName = "DEFAULT_TESTER";
            SOFTWARE_CHANGE_COMMITEE_BOARD.ParameterName = "SOFTWARE_CHANGE_COMMITEE_BOARD";
            IS_INTERNAL.ParameterName = "IS_INTERNAL";
            BILLING_CODE.ParameterName = "BILLING_CODE";
            DISTRIBUTION_LIST.ParameterName = "DISTRIBUTION_LIST";

            NAME.Value = ProductName;
            DESCRIPTION.Value = (string.IsNullOrEmpty(Description) ? (object)DBNull.Value : Description);
            VALIDATOR.Value = Validator;
            RELEASE_COORDINATOR.Value = ReleaseCoordinator;
            RELEASE_COORDINATOR_2.Value = (ReleaseCoordinator2 == 0) ? (object)DBNull.Value : ReleaseCoordinator2;
            DEFAULT_TESTER.Value = DefaultTester;
            SOFTWARE_CHANGE_COMMITEE_BOARD.Value = SCCB;
            IS_INTERNAL.Value = isInteral;
            BILLING_CODE.Value = BillingCode;
            DISTRIBUTION_LIST.Value = (string.IsNullOrEmpty(DistributionList) ? (object)DBNull.Value : DistributionList);

            command.Parameters.Add(NAME);
            command.Parameters.Add(DESCRIPTION);
            command.Parameters.Add(VALIDATOR);
            command.Parameters.Add(RELEASE_COORDINATOR);
            command.Parameters.Add(RELEASE_COORDINATOR_2);
            command.Parameters.Add(DEFAULT_TESTER);
            command.Parameters.Add(SOFTWARE_CHANGE_COMMITEE_BOARD);
            command.Parameters.Add(IS_INTERNAL);
            command.Parameters.Add(BILLING_CODE);
            command.Parameters.Add(DISTRIBUTION_LIST);

            int ProductId = Convert.ToInt32(command.ExecuteScalar());//Gets int of new product
            connection.Close();
            return ProductId;
            */
        }

        #region TEAMMATE FUNCTIONS
        public DataTable GetProductTeam(int productID)
        {
            string strCmdText = @"
                SELECT 
                    ST_USERS.FIRST_NAME, ST_USERS.LAST_NAME, (ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME) AS 'FULL_NAME', ST_USERS.LOGON, ST_PRODUCT_TEAM.USER_ID, ST_ROLES.NAME, ST_USERS.ACCOUNT_TYPE 
                FROM 
                    ST_PRODUCT_TEAM 
                INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_PRODUCT_TEAM.USER_ID 
                INNER JOIN ST_ROLES ON ST_ROLES.ROLE_ID = ST_PRODUCT_TEAM.ROLE 
                WHERE 
                    ST_PRODUCT_TEAM.PRODUCT = @PRODUCT
                ORDER BY
                    ST_USERS.FIRST_NAME ASC, ST_USERS.LAST_NAME ASC
            ";
            strCmdText = strCmdText.Replace("@PRODUCT", productID.ToString());
            return this.ProcessCommand(strCmdText);
        }
        /// <summary>
        /// Adds a teammate to a product's team
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="userID"></param>
        /// <param name="roleID"></param>
        /// the role that user will take on the team
        public void InsertTeammate(int productID, int userID, int roleID)
        {
            String command = "INSERT INTO ST_PRODUCT_TEAM(ROW_VER, PRODUCT, USER_ID, ROLE)" +
                "VALUES(1, " + Convert.ToString(productID) + "," + Convert.ToString(userID) + "," + Convert.ToString(roleID) + " )";
            this.ProcessCommand(command);
        }

        ///<summary>
        /// Removes a instant from the st_product_team table
        ///</summary>
        ///<param name="TeamTableID">
        /// The if the the instant in the table
        ///</param>
        public void RemoveTeammate(int TeamTableID)
        {
            String command = "DELETE FROM ST_PRODUCT_TEAM WHERE TEAM_ID = " + Convert.ToString(TeamTableID);
            this.ProcessCommand(command);
        }

        /// <summary>
        /// Updates the teammates role on the team
        /// </summary>
        /// <param name="TeamTableID"></param>
        /// <param name="RoleID"></param>
        public void UpdateTeammate(int TeamTableID, int RoleID)
        {
            String command = "UPDATE ST_PRODUCT_TEAM SET ROLE = " + Convert.ToString(RoleID) + "WHERE TEAM_ID = " + Convert.ToString(TeamTableID);
            this.ProcessCommand(command);
        }
        #endregion

        #region PRODUCT METRIC FUNCTIONS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="argsProductid"></param>
        /// <param name="argsMajorVersion"></param>
        /// <param name="argsMinorVersion"></param>
        /// <param name="argsCodeCriticalCount"></param>
        /// <param name="argsCodeHighCount"></param>
        /// <param name="argsCodeMetric"></param>
        /// <param name="argsCodeDefectCount"></param>
        /// <param name="argsReleaseCriticalCount"></param>
        /// <param name="argsReleaseHighCount"></param>
        /// <param name="argsReleaseMetric"></param>
        /// <param name="argsReleaseDefectCount"></param>
        /// <param name="argsDisplay"></param>
        public void CreateMetric(int argsProductid, int argsMajorVersion, int argsMinorVersion,
            int argsCodeCriticalCount, int argsCodeHighCount, int argsCodeMetric, int argsCodeDefectCount,
            int argsReleaseCriticalCount, int argsReleaseHighCount, int argsReleaseMetric, int argsReleaseDefectCount,
            bool argsDisplay
            )
        {
            //Exception returned_ex = null;

            ///<summary>
            /// Inserts a new instance of Metric
            ///</summary>
            ///
            ///<param name="argsProductid">
            /// The id of the product
            ///</param>
            ///<param name="argsMajorVersion">
            /// The Major version number
            ///</param>
            ///<param name="argsMinorVersion">
            /// The Minor version number
            ///</param>
            ///<param name="argsCodeCriticalCount">
            /// The count of critical instances
            ///</param>
            ///<param name="argsCodeHighCount">
            /// The count of high instances
            ///</param>
            ///<param name="argsCodeMetric">
            /// The code metric
            ///</param>
            ///<param name="argsCodeDefectCount">
            /// The count of defects
            ///</param>
            ///<param name="argsReleaseCriticalCount">
            /// Count of how many critcal REAs were released
            ///</param>
            ///<param name="argsReleaseHighCount">
            /// Count of how many high REAs were released
            ///</param>
            ///<param name="argsReleaseMetric">
            /// The release metrics
            ///</param>

            String CommandString =
                "EXEC Metric_Insert " +
             /*
             "INSERT INTO [ST_PRODUCT_METRICS] "
            +"		("
            +"PRODUCT_ID			"	
            +",MAJOR_VERSION			"
            +",MINOR_VERSION			"
            +",CODE_CRITICAL_COUNT	"
            +",CODE_HIGH_COUNT		"
            +",CODE_DEFECT_METRIC	"
            +",CODE_DEFECT_COUNT		"
            +",RELEASE_CRITICAL_COUNT	"
            +",RELEASE_HIGH_COUNT		"
            +",RELEASE_DEFECT_METRIC			"
            +",RELEASE_DEFECT_COUNT	"
            +",DISPLAY)"
            +"VALUES	("+
         */
             "@PRODUCT_ID"
            + ",@MAJOR_VERSION"
            + ",@MINOR_VERSION"
            + ",@CODE_CRITICAL_COUNT"
            + ",@CODE_HIGH_COUNT"
            + ",@CODE_DEFECT_METRIC"
            + ",@CODE_DEFECT_COUNT"
            + ",@RELEASE_CRITICAL_COUNT"
            + ",@RELEASE_HIGH_COUNT"
            + ",@RELEASE_DEFECT_METRIC"
            + ",@RELEASE_DEFECT_COUNT"
            + ",@DISPLAY";

            SqlParameter Productid = new SqlParameter("PRODUCT_ID", SqlDbType.Int);
            SqlParameter MajorVersion = new SqlParameter("MAJOR_VERSION", SqlDbType.Int);
            SqlParameter MinorVersion = new SqlParameter("MINOR_VERSION", SqlDbType.Int);
            SqlParameter CodeCriticalCount = new SqlParameter("CODE_CRITICAL_COUNT", SqlDbType.Int);
            SqlParameter CodeHighCount = new SqlParameter("CODE_HIGH_COUNT", SqlDbType.Int);
            SqlParameter CodeMetric = new SqlParameter("CODE_DEFECT_METRIC", SqlDbType.Int);
            SqlParameter CodeDefectCount = new SqlParameter("CODE_DEFECT_COUNT", SqlDbType.Int);
            SqlParameter ReleaseCriticalCount = new SqlParameter("RELEASE_CRITICAL_COUNT", SqlDbType.Int);
            SqlParameter ReleaseHighCount = new SqlParameter("RELEASE_HIGH_COUNT", SqlDbType.Int);
            SqlParameter ReleaseMetric = new SqlParameter("RELEASE_DEFECT_METRIC", SqlDbType.Int);
            SqlParameter ReleaseDefectCount = new SqlParameter("RELEASE_DEFECT_COUNT", SqlDbType.Int);
            SqlParameter Display = new SqlParameter("DISPLAY", SqlDbType.Bit);

            Productid.Value = argsProductid;
            MajorVersion.Value = argsMajorVersion;
            MinorVersion.Value = argsMinorVersion;
            CodeCriticalCount.Value = argsCodeCriticalCount;
            CodeHighCount.Value = argsCodeHighCount;
            CodeMetric.Value = argsCodeMetric;
            CodeDefectCount.Value = argsCodeDefectCount;
            ReleaseCriticalCount.Value = argsReleaseCriticalCount;
            ReleaseHighCount.Value = argsReleaseHighCount;
            ReleaseMetric.Value = argsReleaseMetric;
            ReleaseDefectCount.Value = argsReleaseDefectCount;
            Display.Value = argsDisplay;
            using (
               SqlConnection connection = new SqlConnection(_connectionString)
               )
            {
                connection.Open();
                SqlCommand command = new SqlCommand(CommandString, connection);
                command.Parameters.Add(Productid);
                command.Parameters.Add(MajorVersion);
                command.Parameters.Add(MinorVersion);
                command.Parameters.Add(CodeCriticalCount);
                command.Parameters.Add(CodeHighCount);
                command.Parameters.Add(CodeMetric);
                command.Parameters.Add(CodeDefectCount);
                command.Parameters.Add(ReleaseCriticalCount);
                command.Parameters.Add(ReleaseHighCount);
                command.Parameters.Add(ReleaseMetric);
                command.Parameters.Add(ReleaseDefectCount);
                command.Parameters.Add(Display);

                command.ExecuteNonQuery();
                connection.Dispose();
            }
        }

        /// <summary>
        /// Updates a new instance of Metric
        /// </summary>
        /// <param name="argsMetricID"></param>
        /// <param name="argsProductid"></param>
        /// <param name="argsMajorVersion"></param>
        /// <param name="argsMinorVersion"></param>
        /// <param name="argsCodeCriticalCount"></param>
        /// <param name="argsCodeHighCount"></param>
        /// <param name="argsCodeMetric"></param>
        /// <param name="argsCodeDefectCount"></param>
        /// <param name="argsReleaseCriticalCount"></param>
        /// <param name="argsReleaseHighCount"></param>
        /// <param name="argsReleaseMetric"></param>
        /// <param name="argsReleaseDefectCount"></param>
        /// <param name="argsDisplay"></param>
        public void UpdateMetrics(
                int argsMetricID,
                int argsProductid, int argsMajorVersion, int argsMinorVersion,
                int argsCodeCriticalCount, int argsCodeHighCount, int argsCodeMetric, int argsCodeDefectCount,
                int argsReleaseCriticalCount, int argsReleaseHighCount, int argsReleaseMetric, int argsReleaseDefectCount,
                bool argsDisplay)
        {

            String CommandString = "EXEC Metric_Update"
                + " " + argsMetricID
                + "," + argsProductid
                + "," + argsMajorVersion
                + "," + argsMinorVersion
                + "," + argsCodeCriticalCount
                + "," + argsCodeHighCount
                + "," + argsCodeMetric
                + "," + argsCodeDefectCount
                + "," + argsReleaseCriticalCount
                + "," + argsReleaseHighCount
                + "," + argsReleaseMetric
                + "," + argsReleaseDefectCount
                + "," + (argsDisplay ? "1" : "0");
            this.ProcessCommand(CommandString);
            /*
        "@METRIC_ID"
        + ",@PRODUCT_ID"
        + ",@MAJOR_VERSION"
        + ",@MINOR_VERSION"
        + ",@CODE_CRITICAL_COUNT"
        + ",@CODE_HIGH_COUNT"
        + ",@CODE_DEFECT_METRIC"
        + ",@CODE_DEFECT_COUNT"
        + ",@RELEASE_CRITICAL_COUNT"
        + ",@RELEASE_HIGH_COUNT"
        + ",@RELEASE_DEFECT_METRIC"
        + ",@RELEASE_DEFECT_COUNT"
        + ",@DISPLAY";

        SqlParameter ID                     = new SqlParameter("METRIC_ID", SqlDbType.Int);
        SqlParameter Productid              = new SqlParameter("PRODUCT_ID", SqlDbType.Int);
        SqlParameter MajorVersion           = new SqlParameter("MAJOR_VERSION", SqlDbType.Int);
        SqlParameter MinorVersion           = new SqlParameter("MINOR_VERSION", SqlDbType.Int);
        SqlParameter CodeCriticalCount      = new SqlParameter("CODE_CRITICAL_COUNT", SqlDbType.Int);
        SqlParameter CodeHighCount          = new SqlParameter("CODE_HIGH_COUNT", SqlDbType.Int);
        SqlParameter CodeMetric             = new SqlParameter("CODE_DEFECT_METRIC", SqlDbType.Int);
        SqlParameter CodeDefectCount        = new SqlParameter("CODE_DEFECT_COUNT", SqlDbType.Int);
        SqlParameter ReleaseCriticalCount   = new SqlParameter("RELEASE_CRITICAL_COUNT", SqlDbType.Int);
        SqlParameter ReleaseHighCount       = new SqlParameter("RELEASE_HIGH_COUNT", SqlDbType.Int);
        SqlParameter ReleaseMetric          = new SqlParameter("RELEASE_DEFECT_METRIC", SqlDbType.Int);
        SqlParameter ReleaseDefectCount     = new SqlParameter("RELEASE_DEFECT_COUNT", SqlDbType.Int);
        SqlParameter Display                = new SqlParameter("DISPLAY", SqlDbType.Bit);

        ID.Value = argsMetricID;
        Productid.Value = argsProductid;
        MajorVersion.Value = argsMajorVersion;
        MinorVersion.Value = argsMinorVersion;
        CodeCriticalCount.Value = argsCodeCriticalCount;
        CodeHighCount.Value = argsCodeHighCount;
        CodeMetric.Value = argsCodeMetric;
        CodeDefectCount.Value = argsCodeDefectCount;
        ReleaseCriticalCount.Value = argsReleaseCriticalCount;
        ReleaseHighCount.Value = argsReleaseHighCount;
        ReleaseMetric.Value = argsReleaseMetric;
        ReleaseDefectCount.Value = argsReleaseDefectCount;
        Display.Value = argsDisplay;


        using (
            SqlConnection connection = new SqlConnection(_connectionString)
            )
        {
            connection.Open();
            SqlCommand command = new SqlCommand(CommandString, connection);
            command.Parameters.Add(ID);
            command.Parameters.Add(Productid);
            command.Parameters.Add(MajorVersion);
            command.Parameters.Add(MinorVersion);
            command.Parameters.Add(CodeCriticalCount);
            command.Parameters.Add(CodeHighCount);
            command.Parameters.Add(CodeMetric);
            command.Parameters.Add(CodeDefectCount);
            command.Parameters.Add(ReleaseCriticalCount);
            command.Parameters.Add(ReleaseHighCount);
            command.Parameters.Add(ReleaseMetric);
            command.Parameters.Add(ReleaseDefectCount);
            command.Parameters.Add(Display);

            command.ExecuteNonQuery();
            connection.Dispose();
        }
             * */
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="PRODUCT_ID"></param>
        /// <returns></returns>
        public DataTable GetMetrics(int PRODUCT_ID)
        {
            String Command = "SELECT *" +
                             //    "MAJOR_VERSION, MINOR_VERSION, " +
                             //   "CODE_CRITICAL_COUNT, CODE_HIGH_COUNT, " +
                             //   "CODE_METRIC, CODE_DEFECT_COUNT, " +
                             //   "RELEASE_CRITICAL_COUNT, RELEASE_HIGH_COUNT, " +
                             //   "RELEASE_METRIC, RELEASE_DEFECT_COUNT " +
                             "FROM ST_PRODUCT_METRICS " +
                             "WHERE PRODUCT_ID = " + Convert.ToString(PRODUCT_ID) +
                             " ORDER BY MAJOR_VERSION DESC, MINOR_VERSION DESC";

            return this.ProcessCommand(Command);
        }

        #endregion

        /// <summary>
        ///
        /// </summary>
        /// <param name="productID"></param>
        /// <returns>the VALIDATOR, DEFAULT_TESTER, RELEASE_COORDINATOR, RELEASE_COORDINATOR_2, SCCB, RETIRED, IS_INTERNAL, BILLING_CODE, disribution, description list in one row</returns>
        public ProductInfo GetProductBasic(int productID)
        {
            ProductInfo info = new ProductInfo();
            string command = @"
                SELECT 
                    ST_PRODUCT.*,
                    (USER1.FIRST_NAME + ' ' + USER1.LAST_NAME) AS 'Validator Name',
                    (USER2.FIRST_NAME + ' ' + USER2.LAST_NAME) AS 'Release Coordinator Name',
                    (USER3.FIRST_NAME + ' ' + USER3.LAST_NAME) AS 'Release Coordinator 2 Name',
                    (USER4.FIRST_NAME + ' ' + USER4.LAST_NAME) AS 'Default Tester Name',
                    (USER5.FIRST_NAME + ' ' + USER5.LAST_NAME) AS 'SCCB Name',
                    (USER6.FIRST_NAME + ' ' + USER6.LAST_NAME) AS 'Owner Name'
 
                FROM 
                    REA_PRODUCT 
                            INNER JOIN ST_USERS USER1 ON USER1.USER_ID = VALIDATOR
                            INNER JOIN ST_USERS USER2 ON USER2.USER_ID = RELEASE_COORDINATOR
                            LEFT JOIN ST_USERS USER3 ON USER3.USER_ID = RELEASE_COORDINATOR_2
                            LEFT JOIN ST_USERS USER4 ON USER4.USER_ID = DEFAULT_TESTER
                            LEFT JOIN ST_USERS USER5 ON USER5.USER_ID = SCCB
                            LEFT JOIN ST_USERS USER6 ON USER6.USER_ID = OWNER
                WHERE 
                    PRODUCT_ID = " + productID;

            using (DataTable dt = ProcessCommand(command))
            {
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    info.ID = (int)dr["PRODUCT_ID"];
                    info.Name = Convert.ToString(dr["NAME"]);
                    info.Description = Convert.ToString(dr["DESCRIPTION"]);
                    info.DistributionList = Convert.ToString(dr["DISTRIBUTION_LIST"]);

                    info.HasMetrics = Convert.ToBoolean(dr["METRICS"]);
                    info.IsInternal = Convert.ToBoolean(dr["IS_INTERNAL"]);
                    info.IsRetired = Convert.ToBoolean(dr["RETIRED"]);
                    info.BillingCode = Convert.ToInt32(dr["BILLING_CODE"]);

                    info.ValidatorID = Convert.ToInt32(dr["VALIDATOR"]);
                    info.ValidatorName = Convert.ToString(dr["Validator Name"]);

                    info.ReleaseCoordinatorID = (dr["RELEASE_COORDINATOR"] == DBNull.Value) ? 0 : Convert.ToInt32(dr["RELEASE_COORDINATOR"]);
                    info.ReleaseCoordinatorName = Convert.ToString(dr["Release Coordinator Name"]);

                    info.ReleaseCoordinator2ID = (dr["RELEASE_COORDINATOR_2"] == DBNull.Value) ? 0 : Convert.ToInt32(dr["RELEASE_COORDINATOR_2"]);
                    info.ReleaseCoordinator2Name = Convert.ToString(dr["Release Coordinator 2 Name"]);

                    info.DefaultTesterID = (dr["DEFAULT_TESTER"] == DBNull.Value) ? 0 : Convert.ToInt32(dr["DEFAULT_TESTER"]);
                    info.DefaultTesterName = Convert.ToString(dr["Default Tester Name"]);

                    info.SCCBID = (dr["SCCB"] == DBNull.Value) ? 0 : Convert.ToInt32(dr["SCCB"]);
                    info.SCCBName = Convert.ToString(dr["SCCB Name"]);

                    info.OwnerID = (dr["OWNER"] == DBNull.Value) ? 0 : Convert.ToInt32(dr["OWNER"]);
                    info.OwnerName = Convert.ToString(dr["Owner Name"]);

                }
            }

            if (info.HasMetrics)
            {
                //get the metric thresholds
                command = @"SELECT * FROM ST_PRODUCT_METRICS WHERE PRODUCT_ID = " + info.ID + " ORDER BY MAJOR_VERSION DESC, MINOR_VERSION DESC";
                using (DataTable dt = ProcessCommand(command))
                {
                    if (dt.Rows.Count > 0)
                    {
                        info.Metrics = new ProductMetricsInfo[dt.Rows.Count];
                        int i = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            info.Metrics[i].Display = (bool)dr["DISPLAY"];
                            info.Metrics[i].MajorVersion = (int)dr["MAJOR_VERSION"];
                            info.Metrics[i].MinorVersion = (int)dr["MINOR_VERSION"];

                            info.Metrics[i].ReleaseCriticalCount = (int)dr["RELEASE_CRITICAL_COUNT"];
                            info.Metrics[i].ReleaseHighCount = (int)dr["RELEASE_HIGH_COUNT"];
                            info.Metrics[i].ReleaseDefectCount = (int)dr["RELEASE_DEFECT_COUNT"];
                            info.Metrics[i].ReleaseDefectMetric = (int)dr["RELEASE_DEFECT_METRIC"];

                            info.Metrics[i].CodeFreezeCriticalCount = (int)dr["CODE_CRITICAL_COUNT"];
                            info.Metrics[i].CodeFreezeHighCount = (int)dr["CODE_HIGH_COUNT"];
                            info.Metrics[i].CodeFreezeDefectCount = (int)dr["CODE_DEFECT_COUNT"];
                            info.Metrics[i].CodeFreezeDefectMetric = (int)dr["CODE_DEFECT_METRIC"];

                            //get the actual values
                            using (DataTable dt2 = GetMetrics(info.ID, info.Metrics[i].MajorVersion, info.Metrics[i].MinorVersion))
                            {
                                info.Metrics[i].Label = info.Metrics[i].MajorVersion + "." + info.Metrics[i].MinorVersion + ".x";
                                info.Metrics[i].LabelID = info.Metrics[i].MajorVersion + "_" + info.Metrics[i].MinorVersion;
                                if (dt2.Rows.Count > 0)
                                {                    
                                    info.Metrics[i].CurrentHighCount = (int)dt2.Rows[0]["High"];
                                    info.Metrics[i].CurrentCriticalCount = (int)dt2.Rows[0]["Critical"];
                                    info.Metrics[i].CurrentDefectMetric = (int)dt2.Rows[0]["Sum of Squares"];
                                    info.Metrics[i].CurrentDefectCount = (int)dt2.Rows[0]["Total Open"];
                                }
                            }
                            //get the post build values
                            using (DataTable dt2 = GetMetrics2(info.ID, info.Metrics[i].MajorVersion, info.Metrics[i].MinorVersion))
                            {
                                if (dt2.Rows.Count > 0)
                                {
                                    info.Metrics[i].PostBuildHighCount = (int)dt2.Rows[0]["High"];
                                    info.Metrics[i].PostBuildCriticalCount = (int)dt2.Rows[0]["Critical"];
                                    info.Metrics[i].PostBuildDefectMetric = (int)dt2.Rows[0]["Sum of Squares"];
                                    info.Metrics[i].PostBuildDefectCount = (int)dt2.Rows[0]["Total Open"];
                                }
                            }
                            i++;
                        }
                    }
                }

            }
            return info;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns>the VALIDATOR, DEFAULT_TESTER, RELEASE_COORDINATOR, RELEASE_COORDINATOR_2, SCCB, RETIRED, IS_INTERNAL, BILLING_CODE, disribution, description list in one row</returns>
        public DataTable GetProductBasicDT(int productID)
        {
            String command = @"SELECT VALIDATOR, DEFAULT_TESTER, RELEASE_COORDINATOR, RELEASE_COORDINATOR_2, SCCB, RETIRED, IS_INTERNAL, BILLING_CODE, DISTRIBUTION_LIST, DESCRIPTION, NAME, METRICS, OWNER FROM ST_PRODUCT WHERE PRODUCT_ID = " + productID;

            return this.ProcessCommand(command);

        }

        public ProductDashboardReports GetProductReportCounts(int productID)
        {
            ProductDashboardReports rpt = new ProductDashboardReports();

            string command = @"
	        SELECT SUM(c1) as 'Problems', SUM(c2) as 'Enhancements', SUM(c3) as 'Planned Work' FROM
	        (
		        SELECT COUNT(*) AS 'c1', 0 AS 'c2', 0 as 'c3' FROM ST_TRACK WHERE ST_TRACK.PRODUCT = @PRODUCT_ID AND (STATUS IN (1,2,5,6,7)) AND ST_TRACK.ISSUE_TYPE = 1
		        UNION
		        SELECT 0 AS 'c1', COUNT(*) AS 'c2', 0 as 'c3' FROM ST_TRACK WHERE ST_TRACK.PRODUCT = @PRODUCT_ID AND (STATUS IN (1,2,5,6,7)) AND ST_TRACK.ISSUE_TYPE = 2
		        UNION
		        SELECT 0 AS 'c1', 0 AS 'c2', COUNT(*) as 'c3' FROM ST_TRACK WHERE ST_TRACK.PRODUCT = @PRODUCT_ID AND (STATUS IN (1,2,5,6,7)) AND ST_TRACK.ISSUE_TYPE = 3 
	        ) as temp";
            command = command.Replace("@PRODUCT_ID", productID.ToString());
            using (DataTable dt = ProcessCommand(command))
            {
                if (dt.Rows.Count > 0)
                {
                    rpt.Problems = (int)dt.Rows[0]["Problems"];
                    rpt.Enhancements = (int)dt.Rows[0]["Enhancements"];
                    rpt.PlannedWork = (int)dt.Rows[0]["Planned Work"];
                }
            }

            command = @"
	        SELECT SUM(c1) as 'Critical', SUM(c2) as 'High', SUM(c3) as 'Medium', SUM(c4) as 'Low' FROM
	        (
		        SELECT COUNT(*) AS 'c1', 0 AS 'c2', 0 as 'c3', 0 as 'c4' FROM ST_TRACK WHERE ST_TRACK.PRODUCT = @PRODUCT_ID AND (STATUS IN (1,2,5,6,7)) AND ST_TRACK.ISSUE_TYPE = 1 AND ST_TRACK.PRIORITY = 4
		        UNION
		        SELECT 0 AS 'c1', COUNT(*) AS 'c2', 0 as 'c3', 0 as 'c4' FROM ST_TRACK WHERE ST_TRACK.PRODUCT = @PRODUCT_ID AND (STATUS IN (1,2,5,6,7)) AND ST_TRACK.ISSUE_TYPE = 1 AND ST_TRACK.PRIORITY = 3
		        UNION
		        SELECT 0 AS 'c1', 0 AS 'c2', COUNT(*) as 'c3', 0 as 'c4' FROM ST_TRACK WHERE ST_TRACK.PRODUCT = @PRODUCT_ID AND (STATUS IN (1,2,5,6,7)) AND ST_TRACK.ISSUE_TYPE = 1 AND ST_TRACK.PRIORITY = 2
		        UNION
		        SELECT 0 AS 'c1', 0 AS 'c2', 0 as 'c3', COUNT(*) as 'c4' FROM ST_TRACK WHERE ST_TRACK.PRODUCT = @PRODUCT_ID AND (STATUS IN (1,2,5,6,7)) AND ST_TRACK.ISSUE_TYPE = 1 AND ST_TRACK.PRIORITY = 1
	        ) as temp";
            command = command.Replace("@PRODUCT_ID", productID.ToString());
            using (DataTable dt = ProcessCommand(command))
            {
                if (dt.Rows.Count > 0)
                {
                    rpt.Critical = (int)dt.Rows[0]["Critical"];
                    rpt.High = (int)dt.Rows[0]["High"];
                    rpt.Medium = (int)dt.Rows[0]["Medium"];
                    rpt.Low = (int)dt.Rows[0]["Low"];
                }
            }

            command = $@"
	        SELECT SUM(c1) as 'Submitted', SUM(c2) as 'Approved', SUM(c3) as 'In Process', SUM(c4) as 'Fixed or Delivered', SUM(c5) as 'Testing', SUM(c6) as 'Deferred' FROM
	        (
		        SELECT COUNT(*) AS 'c1', 0 AS 'c2', 0 as 'c3', 0 as 'c4', 0 as 'c5', 0 as 'c6' FROM ST_TRACK WHERE ST_TRACK.PRODUCT = {productID} AND ST_TRACK.STATUS = 1
		        UNION
		        SELECT 0 AS 'c1', COUNT(*) AS 'c2', 0 as 'c3', 0 as 'c4', 0 as 'c5', 0 as 'c6' FROM ST_TRACK WHERE ST_TRACK.PRODUCT = {productID} AND ST_TRACK.STATUS in (2)
		        UNION
		        SELECT 0 AS 'c1', 0 as 'c2', COUNT(*) AS 'c3', 0 as 'c4', 0 as 'c4', 0 as 'c6' FROM ST_TRACK WHERE ST_TRACK.PRODUCT = {productID} AND ST_TRACK.STATUS in (5)
		        UNION
		        SELECT 0 AS 'c1', 0 AS 'c2', 0 as 'c3', COUNT(*) as 'c4', 0 as 'c5', 0 as 'c6' FROM ST_TRACK WHERE ST_TRACK.PRODUCT = {productID} AND ST_TRACK.STATUS in (6, 7)
		        UNION
		        SELECT 0 AS 'c1', 0 AS 'c2', 0 as 'c3', 0 as 'c4', COUNT(*) as 'c5', 0 as 'c6' FROM ST_TRACK WHERE ST_TRACK.PRODUCT = {productID} AND ST_TRACK.STATUS in (8, 10, 11)
		        UNION
		        SELECT 0 AS 'c1', 0 AS 'c2', 0 as 'c3', 0 as 'c4', 0 as 'c5', COUNT(*) as 'c6' FROM ST_TRACK WHERE ST_TRACK.PRODUCT = {productID} AND ST_TRACK.STATUS = 3
	        ) as temp";
            
            using (DataTable dt = ProcessCommand(command))
            {
                if (dt.Rows.Count > 0)
                {
                    rpt.Submitted = (int)dt.Rows[0]["Submitted"];
                    rpt.Approved = (int)dt.Rows[0]["Approved"];
                    rpt.InProcess = (int)dt.Rows[0]["In Process"];
                    rpt.FixedDelivered = (int)dt.Rows[0]["Fixed or Delivered"];
                    rpt.Testing = (int)dt.Rows[0]["Testing"];
                    rpt.Deferred = (int)dt.Rows[0]["Deferred"];
                }
            }
            return rpt;
        }
        
        /// <summary>
        /// Finds the TOP 4 releases for a given product
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public DataTable GetProductReleases(int productID, int count = 0)
        {
            string command = @"
            SELECT DISTINCT @COUNT
                CAST(MAJOR AS INT) AS MAJOR, 
                CAST(MINOR AS INT) AS MINOR
            FROM 
                ST_PRODUCT_RELEASE 
            WHERE 
                PRODUCT_ID = @PRODUCT_ID
            ORDER BY CAST(MAJOR AS INT) DESC, CAST(MINOR AS INT) DESC
            ";
            command = command.Replace("@PRODUCT_ID", productID.ToString());
            if (count <= 0)
            {
                //default (or invalid number) just get all the releases
                command = command.Replace("@COUNT", "");
            }
            else
            {
                //just get the TOP n
                command = command.Replace("@COUNT", "TOP " + count.ToString());
            }
            return ProcessCommand(command);
        }

        public DataTable GetAllProductReleases(int productID)
        {
            string command = @"
            SELECT 
                ST_PRODUCT_RELEASE.* 
            FROM 
                ST_PRODUCT_RELEASE 
            WHERE 
                ST_PRODUCT_RELEASE.PRODUCT_ID = @PRODUCT_ID
            ORDER BY
                CAST(ST_PRODUCT_RELEASE.MAJOR AS INT)DESC,
                CAST(ST_PRODUCT_RELEASE.MINOR AS INT)DESC,
                CAST(ST_PRODUCT_RELEASE.BUILD AS INT)DESC, 
                ST_PRODUCT_RELEASE.MISC";
            command = command.Replace("@PRODUCT_ID", productID.ToString());

            return ProcessCommand(command);
        }

        /// <summary>
        /// Looks to see if there are any REAs in the Delivered status that are assigned to the primary release coordinator
        /// If there are, then a build can be made.
        /// </summary>
        /// <param name="productID"></param>
        /// <returns>true if a build can be done, else false</returns>
        public bool CanBuildRelease(int productID)
        {
            bool can_release = false;
            string command = @"
                SELECT DISTINCT 
                    TRACKING_ID 
                FROM 
                    ST_TRACK 
                INNER JOIN ST_PRODUCT ON ST_PRODUCT.RELEASE_COORDINATOR = ST_TRACK.ASSIGNED_TO
                WHERE 
                    ST_TRACK.PRODUCT = @PRODUCT_ID AND ST_TRACK.STATUS = 7";
            command = command.Replace("@PRODUCT_ID", productID.ToString());
            using (DataTable dt = ProcessCommand(command))
            {
                can_release = dt.Rows.Count > 0;
            }
            return can_release;
        }

        public bool UpdateProductInformation(ProductInfo info)
        {
            bool success = false;
            string cmdText = @"
            UPDATE ST_PRODUCT SET 
                NAME = @NAME,
                DESCRIPTION = @DESCRIPTION,
                VALIDATOR =  @VALIDATOR,
                RELEASE_COORDINATOR = @RELEASE_COORDINATOR,
                RETIRED = @RETIRED,
                DEFAULT_TESTER = @DEFAULT_TESTER,
                SCCB = @SCCB,
                IS_INTERNAL = @IS_INTERNAL,
                BILLING_CODE = @BILLING_CODE,
                RELEASE_COORDINATOR_2 = @RELEASE_COORDINATOR_2,
                DISTRIBUTION_LIST = @DISTRIBUTION_LIST,
                RETIRED_ON = @RETIRED_ON,
                METRICS = @METRICS,
                OWNER=@OWNER
            WHERE
                PRODUCT_ID = @PRODUCT_ID
            ";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = cmdText;
                    command.Parameters.Add(new SqlParameter("@NAME", SqlDbType.VarChar)); command.Parameters["@NAME"].Value = info.Name;
                    command.Parameters.Add(new SqlParameter("@DESCRIPTION", SqlDbType.VarChar)); command.Parameters["@DESCRIPTION"].Value = info.Description;
                    command.Parameters.Add(new SqlParameter("@VALIDATOR", SqlDbType.Int)); command.Parameters["@VALIDATOR"].Value = info.ValidatorID == 0 ? (object)DBNull.Value : info.ValidatorID;
                    command.Parameters.Add(new SqlParameter("@RELEASE_COORDINATOR", SqlDbType.Int)); command.Parameters["@RELEASE_COORDINATOR"].Value = info.ReleaseCoordinatorID == 0 ? (object)DBNull.Value : info.ReleaseCoordinatorID;
                    command.Parameters.Add(new SqlParameter("@RELEASE_COORDINATOR_2", SqlDbType.Int)); command.Parameters["@RELEASE_COORDINATOR_2"].Value = info.ReleaseCoordinator2ID == 0 ? (object)DBNull.Value : info.ReleaseCoordinator2ID;
                    command.Parameters.Add(new SqlParameter("@DEFAULT_TESTER", SqlDbType.Int)); command.Parameters["@DEFAULT_TESTER"].Value = info.DefaultTesterID == 0 ? (object)DBNull.Value : info.DefaultTesterID;
                    command.Parameters.Add(new SqlParameter("@SCCB", SqlDbType.Int)); command.Parameters["@SCCB"].Value = info.SCCBID == 0 ? (object)DBNull.Value : info.SCCBID;
                    command.Parameters.Add(new SqlParameter("@IS_INTERNAL", SqlDbType.Bit)); command.Parameters["@IS_INTERNAL"].Value = info.IsInternal;
                    command.Parameters.Add(new SqlParameter("@METRICS", SqlDbType.Bit)); command.Parameters["@METRICS"].Value = info.HasMetrics;
                    command.Parameters.Add(new SqlParameter("@RETIRED", SqlDbType.Bit)); command.Parameters["@RETIRED"].Value = info.IsRetired;
                    command.Parameters.Add(new SqlParameter("@PRODUCT_ID", SqlDbType.Int)); command.Parameters["@PRODUCT_ID"].Value = info.ID;                
                    command.Parameters.Add(new SqlParameter("@RETIRED_ON", SqlDbType.DateTime));command.Parameters["@RETIRED_ON"].Value = info.IsRetired ? (object)DBNull.Value : DateTime.Now;
                    command.Parameters.Add(new SqlParameter("@DISTRIBUTION_LIST", SqlDbType.VarChar)); command.Parameters["@DISTRIBUTION_LIST"].Value = string.IsNullOrEmpty(info.DistributionList) ? (object)DBNull.Value : info.DistributionList;
                    command.Parameters.Add(new SqlParameter("@BILLING_CODE", SqlDbType.Int)); command.Parameters["@BILLING_CODE"].Value = info.BillingCode;
                    command.Parameters.Add(new SqlParameter("@OWNER", SqlDbType.Int)); command.Parameters["@OWNER"].Value = info.OwnerID == 0 ? (object)DBNull.Value : info.OwnerID;
                    int num_rows = command.ExecuteNonQuery();

                    success = (num_rows == 1);
                }
            }

            return success;
        }
        /// <summary>
        /// Updates the product's informtaion
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="validator"></param>
        /// <param name="releaseCoordinator"></param>
        /// <param name="releaseCoordinator2"></param>
        /// <param name="DefaultTester"></param>
        /// <param name="sccb"></param>
        /// <param name="internalProduct"></param>
        /// <param name="retired"></param>
        /// <param name="billingCode"></param>
        /// <param name="DistributionList"></param>
        /// <param name="UseMetrics"></param>
        public void UpdateProductInformation
            (
            int productId,
            string name, string description,
            int validator, int releaseCoordinator, int releaseCoordinator2, int DefaultTester, int sccb,
            bool internalProduct, bool retired,
            int billingCode, string DistributionList, bool UseMetrics, int owner
            )
        {
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(_connectionString);
            command.Connection = connection;
            connection.Open();

            String SQlCommand = "exec ProductManager_UpdateProduct @PRODUCT_ID ,@NAME ,@DESCRIPTION ,@VALIDATOR ,@RELEASE_COORDINATOR , @RETIRED " +
                                ",@DEFAULT_TESTER ,@SCCB ,@IS_INTERNAL ,@BILLING_CODE ,@RELEASE_COORDINATOR_2 ,@DISTRIBUTION_LIST, @METRICS, @OWNER";
            command.CommandText = SQlCommand;
            //Createing the parameter
            SqlParameter PRODUCT_ID = new SqlParameter();
            SqlParameter NAME = new SqlParameter();
            SqlParameter DESCRIPTION = new SqlParameter();
            SqlParameter VALIDATOR = new SqlParameter();
            SqlParameter RELEASE_COORDINATOR = new SqlParameter();
            SqlParameter RETIRED = new SqlParameter();
            SqlParameter DEFAULT_TESTER = new SqlParameter();
            SqlParameter SCCB = new SqlParameter();
            SqlParameter IS_INTERNAL = new SqlParameter();
            SqlParameter BILLING_CODE = new SqlParameter();
            SqlParameter RELEASE_COORDINATOR_2 = new SqlParameter();
            SqlParameter DISTRIBUTION_LIST = new SqlParameter();
            SqlParameter METRICS = new SqlParameter("METRICS", UseMetrics);
            SqlParameter OWNER = new SqlParameter();
            //Declare type
            PRODUCT_ID.SqlDbType = SqlDbType.Int;
            NAME.SqlDbType = SqlDbType.VarChar;
            DESCRIPTION.SqlDbType = SqlDbType.VarChar;
            VALIDATOR.SqlDbType = SqlDbType.Int;
            RELEASE_COORDINATOR.SqlDbType = SqlDbType.Int;
            RETIRED.SqlDbType = SqlDbType.Bit;
            DEFAULT_TESTER.SqlDbType = SqlDbType.Int;
            SCCB.SqlDbType = SqlDbType.Int;
            IS_INTERNAL.SqlDbType = SqlDbType.Bit;
            BILLING_CODE.SqlDbType = SqlDbType.Int;
            RELEASE_COORDINATOR_2.SqlDbType = SqlDbType.Int;
            DISTRIBUTION_LIST.SqlDbType = SqlDbType.VarChar;
            METRICS.SqlDbType = SqlDbType.Bit;
            OWNER.SqlDbType = SqlDbType.Int;
            //Parameter name
            PRODUCT_ID.ParameterName = "PRODUCT_ID";
            NAME.ParameterName = "NAME";
            DESCRIPTION.ParameterName = "DESCRIPTION";
            VALIDATOR.ParameterName = "VALIDATOR";
            RELEASE_COORDINATOR.ParameterName = "RELEASE_COORDINATOR";
            RETIRED.ParameterName = "RETIRED";
            DEFAULT_TESTER.ParameterName = "DEFAULT_TESTER";
            SCCB.ParameterName = "SCCB";
            IS_INTERNAL.ParameterName = "IS_INTERNAL";
            BILLING_CODE.ParameterName = "BILLING_CODE";
            RELEASE_COORDINATOR_2.ParameterName = "RELEASE_COORDINATOR_2";
            DISTRIBUTION_LIST.ParameterName = "DISTRIBUTION_LIST";
            OWNER.ParameterName = "OWNER";
            //Value set 
            PRODUCT_ID.Value = productId;
            NAME.Value = name;
            DESCRIPTION.Value = String.IsNullOrEmpty(description) ? (object)DBNull.Value : description;
            VALIDATOR.Value = (validator == 0) ? (object)DBNull.Value : validator;
            RELEASE_COORDINATOR.Value = (releaseCoordinator == 0) ? (object)DBNull.Value : releaseCoordinator;
            RETIRED.Value = retired;
            DEFAULT_TESTER.Value = (DefaultTester == 0) ? (object)DBNull.Value : DefaultTester;
            SCCB.Value = (sccb == 0) ? (object)DBNull.Value : sccb;
            IS_INTERNAL.Value = internalProduct;
            BILLING_CODE.Value = billingCode;
            RELEASE_COORDINATOR_2.Value = (releaseCoordinator2 == 0) ? (object)DBNull.Value : releaseCoordinator2;
            DISTRIBUTION_LIST.Value = String.IsNullOrEmpty(DistributionList) ? (object)DBNull.Value : DistributionList;
            OWNER.Value = (owner == 0) ? (object)DBNull.Value : owner;
            //Add Value to the Command
            command.Parameters.Add(PRODUCT_ID);
            command.Parameters.Add(NAME);
            command.Parameters.Add(DESCRIPTION);
            command.Parameters.Add(VALIDATOR);
            command.Parameters.Add(RELEASE_COORDINATOR);
            command.Parameters.Add(RETIRED);
            command.Parameters.Add(DEFAULT_TESTER);
            command.Parameters.Add(SCCB);
            command.Parameters.Add(IS_INTERNAL);
            command.Parameters.Add(BILLING_CODE);
            command.Parameters.Add(RELEASE_COORDINATOR_2);
            command.Parameters.Add(DISTRIBUTION_LIST);
            command.Parameters.Add(METRICS);
            command.Parameters.Add(OWNER);
            //clean up
            command.ExecuteNonQuery();
            connection.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buildId"></param>
        /// <param name="relatedBuildID"></param>
        public void ProductRelationShip(int buildId, int relatedBuildID)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            //Create Build
            SqlParameter BUILD_ID = new SqlParameter();
            SqlParameter RELATEDBUILDID = new SqlParameter();
            //Set type
            BUILD_ID.SqlDbType = SqlDbType.Int;
            RELATEDBUILDID.SqlDbType = SqlDbType.Int;
            //place values
            RELATEDBUILDID.Value = relatedBuildID;
            BUILD_ID.Value = buildId;
            //Label parameters
            RELATEDBUILDID.ParameterName = "RELATED_BUILD_ID";
            BUILD_ID.ParameterName = "BUILD_ID";
            //Add Parametes
            using (SqlConnection connectionRelationShip = new SqlConnection(_connectionString))
            {
                connectionRelationShip.Open();
                String CommandText = "INSERT INTO ST_PRODUCT_RELEASE_RELATION (BUILD_ID, RELATED_BUILD_ID)" +
                                      "VALUES(" + buildId + ", " + relatedBuildID + ")";
                SqlCommand command = new SqlCommand(CommandText, connectionRelationShip);
                command.Parameters.Add(RELATEDBUILDID);
                command.Parameters.Add(BUILD_ID);
                //command.Transaction = transaction;
                command.ExecuteScalar();
            }
        }

        #region MODULE FUNCTIONS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="ModuleName"></param>
        /// <param name="ModuleDescription"></param>
        /// <param name="retired"></param>
        public void updateModule(int moduleID, String ModuleName, String ModuleDescription, bool retired)
        {
            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(_connectionString);
            command.Connection = connection;
            connection.Open();

            String SQlCommand = "UPDATE ST_MODULE SET NAME='" + ModuleName + "', DESCRIPTION='" + ModuleDescription + "' , RETIRED='" + retired + "' WHERE ST_MODULE.MODULE_ID=" + moduleID + "; ";
            command.CommandText = SQlCommand;
            SqlParameter MODULE_ID = new SqlParameter();
            SqlParameter NAME = new SqlParameter();
            SqlParameter DESCRIPTION = new SqlParameter();
            SqlParameter RETIRED = new SqlParameter();

            MODULE_ID.SqlDbType = SqlDbType.Int;
            NAME.SqlDbType = SqlDbType.VarChar;
            DESCRIPTION.SqlDbType = SqlDbType.VarChar;
            RETIRED.SqlDbType = SqlDbType.Bit;

            MODULE_ID.ParameterName = "MODULE_ID";
            NAME.ParameterName = "NAME";
            DESCRIPTION.ParameterName = "DESCRIPTION";
            RETIRED.ParameterName = "RETIRED";

            MODULE_ID.Value = moduleID;
            NAME.Value = ModuleName;
            DESCRIPTION.Value = String.IsNullOrEmpty(ModuleDescription) ? (object)DBNull.Value : ModuleDescription;
            RETIRED.Value = retired;

            command.Parameters.Add(MODULE_ID);
            command.Parameters.Add(NAME);
            command.Parameters.Add(DESCRIPTION);
            command.Parameters.Add(RETIRED);
            //clean up
            command.ExecuteNonQuery();
            connection.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="ModuleName"></param>
        /// <param name="ModuleDescription"></param>
        public void addModule(int productId, String ModuleName, String ModuleDescription)
        {

            SqlCommand command = new SqlCommand();
            SqlConnection connection = new SqlConnection(_connectionString);
            command.Connection = connection;
            connection.Open();

            String SQlCommand = "INSERT INTO ST_MODULE(ROW_VER, PRODUCT, NAME, DESCRIPTION, RETIRED) VALUES(1, " + productId + " , '" + ModuleName + "' , '" + ModuleDescription + "' , 0);";
            command.CommandText = SQlCommand;
            SqlParameter PRODUCT_ID = new SqlParameter();
            SqlParameter NAME = new SqlParameter();
            SqlParameter DESCRIPTION = new SqlParameter();

            PRODUCT_ID.SqlDbType = SqlDbType.Int;
            NAME.SqlDbType = SqlDbType.VarChar;
            DESCRIPTION.SqlDbType = SqlDbType.VarChar;

            PRODUCT_ID.ParameterName = "PRODUCT";
            NAME.ParameterName = "NAME";
            DESCRIPTION.ParameterName = "DESCRIPTION";

            PRODUCT_ID.Value = productId;
            NAME.Value = ModuleName;
            DESCRIPTION.Value = String.IsNullOrEmpty(ModuleDescription) ? (object)DBNull.Value : ModuleDescription;

            command.Parameters.Add(PRODUCT_ID);
            command.Parameters.Add(NAME);
            command.Parameters.Add(DESCRIPTION);
            //clean up
            command.ExecuteNonQuery();
            connection.Close();
        }

        public DataTable GetProductModuleList(int productID)
        {
            string command = @"
            SELECT *
            FROM 
                ST_MODULE 
            WHERE 
                PRODUCT = @PRODUCT_ID
            ORDER BY ST_MODULE.NAME
            ";
            command = command.Replace("@PRODUCT_ID", productID.ToString());

            return ProcessCommand(command);
        }
        #endregion
    }
}
