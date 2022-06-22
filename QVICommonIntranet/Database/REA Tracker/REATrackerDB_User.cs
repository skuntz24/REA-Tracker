using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace QVICommonIntranet.Database
{
    public partial class REATrackerDB
    {
        public bool CreateUser(String ArgsAspID, String ArgsLogin, String ArgsPassword,
         String ArgsFirstName, String ArgsLastName, int ArgsAccountType, int ArgsDefaultProduct,
         String ArgsProductAccess, Boolean ArgsFullAccess,
         Boolean ArgsIsManager, int ArgsManagerID, int ArgsDepartmentID, int ArgsCompanyID, String ArgsEmail,
         Boolean ArgsBlockWhere, String ArgsLanguageCode)
        {
            bool success = false;
            REATrackerDB sql = new REATrackerDB();

            int[] ArrayOfProductAccess;
            if (string.IsNullOrEmpty(ArgsProductAccess) || string.IsNullOrWhiteSpace(ArgsProductAccess))
            {
                ArrayOfProductAccess = new int[] { };
            }
            else
            {
                ArrayOfProductAccess = Array.ConvertAll(ArgsProductAccess.Split(' '), s => int.Parse(s));
            }
            if (!ArrayOfProductAccess.Contains(ArgsDefaultProduct))
            {
                if (!String.IsNullOrEmpty(ArgsProductAccess))
                {
                    ArgsProductAccess += " ";
                }
                ArgsProductAccess += Convert.ToString(ArgsDefaultProduct);
                ArrayOfProductAccess = Array.ConvertAll(ArgsProductAccess.Split(' '), s => int.Parse(s));
            }
            //insert into st_users table      

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                //SqlCommand sqlcommand = new SqlCommand();
                SqlTransaction transaction = connection.BeginTransaction();
                command.Connection = connection;
                //sqlcommand.Connection = connection;
                command.Transaction = transaction;
                command.Parameters.Add(new SqlParameter("@ASP_ID", ArgsAspID.Trim()));
                command.Parameters.Add(new SqlParameter("@LOGON", ArgsLogin.Trim()));
                command.Parameters.Add(new SqlParameter("@PASSWORD", ArgsPassword.Trim()));
                command.Parameters.Add(new SqlParameter("@FIRSTNAME", ArgsFirstName.Trim()));
                command.Parameters.Add(new SqlParameter("@LASTNAME", ArgsLastName.Trim()));
                command.Parameters.Add(new SqlParameter("@ACCOUNT_TYPE", ArgsAccountType));
                command.Parameters.Add(new SqlParameter("@DEFAULT_PRODUCT", ArgsDefaultProduct));
                command.Parameters.Add(new SqlParameter("@PROD_ACCESS", ArgsProductAccess.Trim()));
                command.Parameters.Add(new SqlParameter("@FULL_ACCESS", ArgsFullAccess));
                command.Parameters.Add(new SqlParameter("@IS_MANAGER", ArgsIsManager));
                command.Parameters.Add(new SqlParameter("@MANAGER_ID", ArgsManagerID));
                command.Parameters.Add(new SqlParameter("@DEPARTMENT_ID", ArgsDepartmentID));
                command.Parameters.Add(new SqlParameter("@COMPANY", ArgsCompanyID));
                command.Parameters.Add(new SqlParameter("@EMAIL", ArgsEmail.Trim()));
                command.Parameters.Add(new SqlParameter("@BLOCK_WHERE", ArgsBlockWhere));
                command.Parameters.Add(new SqlParameter("@LANGUAGECODE", ArgsLanguageCode));

                command.CommandText = @"INSERT INTO ST_USERS(
                                                        LOGON, 
                                                        PASSWORD,
                                                        FIRST_NAME,
                                                        LAST_NAME,
                                                        ACCOUNT_TYPE,
                                                        DEFAULT_PRODUCT,
                                                        PROD_ACCESS,
                                                        FULL_ACCESS,
                                                        IS_MANAGER,
                                                        MANAGER_ID,
                                                        DEPARTMENT_ID,
                                                        COMPANY,
                                                        EMAIL,
                                                        BLOCK_WHERE,
                                                        DETAILS,
                                                        TO_DO_DISPLAY,
                                                        WATCH_DISPLAY,
                                                        SUBMITTED_DISPLAY,
                                                        HOME_DISPLAY,
                                                        CUSTOM_SHOW,
                                                       PRODUCT_DISPLAY
)
                                        VALUES(
                                                        @LOGON,
                                                        @PASSWORD,
                                                        @FIRSTNAME,
                                                        @LASTNAME,
                                                        @ACCOUNT_TYPE,
                                                        @DEFAULT_PRODUCT,
                                                        @PROD_ACCESS,
                                                        @FULL_ACCESS,
                                                        @IS_MANAGER,
                                                        @MANAGER_ID,
                                                        @DEPARTMENT_ID,
                                                        @COMPANY,
                                                        @EMAIL,
                                                        @BLOCK_WHERE, 0,5,5,5,1,0,0
                                              );


                    DECLARE @ST_USERS_ID INT SET @ST_USERS_ID = SCOPE_IDENTITY();

                    UPDATE AspNetUsers
                                    SET
                                            AspNetUsers.ST_USER_ID = @ST_USERS_ID,
                                            AspNetUsers.FirstName = @FIRSTNAME,
                                            AspNetUsers.LastName = @LASTNAME,
                                            AspNetUsers.Email = @EMAIL,
                                            AspNetUsers.LanguageCode = @LANGUAGECODE
                                    WHERE
                                            AspNetUsers.Id = @ASP_ID;";


                int OutputValue = command.ExecuteNonQuery();
                //if the execution returns a value
                if (OutputValue != 0)
                {
                    command.CommandText = @"
                    DECLARE @ST_USERS_ID int SET @ST_USERS_ID= (SELECT MAX(USER_ID) FROM ST_USERS);

                    SELECT Id FROM AspNetRoles
                                    INNER JOIN ST_ACCOUNT_TYPE
                                    ON AspNetRoles.Name = ST_ACCOUNT_TYPE.NAME     
                                    WHERE ST_ACCOUNT_TYPE.ACCOUNT_ID = @ACCOUNT_TYPE

                                        
                   IF @@ROWCOUNT = 0
                   SET @ACCOUNT_TYPE = 2

                   UPDATE ST_USERS SET ACCOUNT_TYPE = @ACCOUNT_TYPE WHERE USER_ID = @ST_USERS_ID;

                   DECLARE @RoleID varchar(128)
                   SET @RoleID = (SELECT Id FROM AspNetRoles
                                    INNER JOIN ST_ACCOUNT_TYPE
                                    ON AspNetRoles.Name = ST_ACCOUNT_TYPE.NAME    
                                    WHERE ST_ACCOUNT_TYPE.ACCOUNT_ID = @ACCOUNT_TYPE)
                                        
                   INSERT AspNetUserRoles (UserId, RoleId) VALUES  (@ASP_ID,@RoleID)
                   
                   SELECT AspNetUsers.ST_USER_ID FROM  AspNetUsers
                                                 WHERE  AspNetUsers.Id = @ASP_ID";
                    try
                    {
                        transaction.Commit();
                        command.ExecuteScalar();
                        string userID = "SELECT AspNetUsers.ST_USER_ID as USER_ID FROM AspNetUsers WHERE AspNetUsers.Id = '" + ArgsAspID + "'";
                        DataTable userDt = sql.ProcessCommand(userID);
                        if (userDt.Rows.Count > 0)
                        {
                            int value = Convert.ToInt32(userDt.Rows[0]["USER_ID"]);
                            foreach (int Access in ArrayOfProductAccess)
                            {
                                this.InsertProductAcess(value, Access);
                            }
                        }
                        _lastError = "";
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        ProcessException(ex);
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            ProcessException(ex2);
                        }
                    }
                    finally
                    {
                        if (command != null) { command.Dispose(); }
                    }
                }
                else
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception e)
                    {
                        ProcessException(e);
                    }
                    finally
                    {
                        if (command != null) { command.Dispose(); }
                    }
                }

            }
            return success;
        }
                  
        /// <summary>
        /// / gets the roles, name from database
        /// </summary>
        /// <returns></returns>
        public DataTable RoleType()
        {
            string command = @"SELECT AspNetRoles.Id AS 'ASPID', AspNetRoles.Name AS 'ASPNAME' FROM AspNetRoles";
            return ProcessCommand(command);
            //return this.ProcessCommand("SELECT AspNetRoles.Id AS 'ASPID', AspNetRoles.Name AS 'ASPNAME' FROM AspNetRoles;");

        }

        /// <summary>
        /// gets the list of user roles
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        

        public DataTable Roles()
        {
            String Command = @"Select st_roles.NAME AS 'ROLE_NAME', st_roles.Role_id AS 'ROLE_ID' from st_roles";
            return this.ProcessCommand(Command);
        }

        public bool UpdateEditPostCompany(int company, string othercompany, int department, bool ismanager, int managerid, int st_user)
        {
            bool success = false;
            string strSql = @"UPDATE ST_USERS SET 
                                COMPANY=@COMPANY,
                                OTHER_COMPANY=@OTHERCOMPANY,
                                DEPARTMENT_ID=@DEPARTMENT,
                                IS_MANAGER=@ISMANAGER,
                                MANAGER_ID=@MANAGERID
                            WHERE
                                USER_ID=@ST_USER";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = strSql;
                    command.Parameters.Add(new SqlParameter("@COMPANY", SqlDbType.Int)); command.Parameters["@COMPANY"].Value = company;
                    command.Parameters.Add(new SqlParameter("@OTHERCOMPANY", SqlDbType.VarChar)); command.Parameters["@OTHERCOMPANY"].Value = othercompany == null ? (object)DBNull.Value : othercompany;
                    command.Parameters.Add(new SqlParameter("@DEPARTMENT", SqlDbType.Int)); command.Parameters["@DEPARTMENT"].Value = department;
                    command.Parameters.Add(new SqlParameter("@ISMANAGER", SqlDbType.Bit)); command.Parameters["@ISMANAGER"].Value = ismanager;
                    command.Parameters.Add(new SqlParameter("@MANAGERID", SqlDbType.Int)); command.Parameters["@MANAGERID"].Value = managerid;
                    command.Parameters.Add(new SqlParameter("@ST_USER", SqlDbType.Int)); command.Parameters["@ST_USER"].Value = st_user;

                    int num_rows = command.ExecuteNonQuery();
                    success = (num_rows == 1);
                }
                return success;
            }

        }   
       
    //when user is not in aspnet table
        public bool UpdateEditPostProfile(int ST_USERID, string FirstName, string LastName, string Logon, string Email, int defaultProduct, int BlockWhere, string Password, string UserAccountType, string EmployeeNum)
        {
            bool success = false;
            string strSql = @"UPDATE ST_USERS SET 
                                FIRST_NAME=@FIRST_NAME,
                                LAST_NAME=@LAST_NAME,
                                LOGON=@LOGON,
                                EMAIL=@EMAIL,
                                DEFAULT_PRODUCT=@DEFAULT_PRODUCT,
                                BLOCK_WHERE=@BLOCK_WHERE,
                                PASSWORD=@PASSWORD,
                                ACCOUNT_TYPE=@ACCOUNT_TYPE,
                                EMPLOYEE_NUM=@EMPLOYEE_NUM
                            WHERE
                                USER_ID=@USER_ID";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = strSql;
                    command.Parameters.Add(new SqlParameter("@FIRST_NAME", SqlDbType.VarChar)); command.Parameters["@FIRSTNAME"].Value = FirstName;
                    command.Parameters.Add(new SqlParameter("@LAST_NAME", SqlDbType.VarChar)); command.Parameters["@LASTNAME"].Value = LastName;
                    command.Parameters.Add(new SqlParameter("@LOGON", SqlDbType.VarChar)); command.Parameters["@LOGON"].Value = Logon;
                    command.Parameters.Add(new SqlParameter("@EMAIL", SqlDbType.VarChar)); command.Parameters["@EMAIL"].Value = Email;
                    command.Parameters.Add(new SqlParameter("@DEFAULT_PRODUCT", SqlDbType.Int)); command.Parameters["@DEFAULTPRODUCT"].Value = defaultProduct;
                    command.Parameters.Add(new SqlParameter("@BLOCK_WHERE", SqlDbType.Int)); command.Parameters["@BLOCKWHERE"].Value = BlockWhere;
                    command.Parameters.Add(new SqlParameter("@PASSWORD", SqlDbType.VarChar)); command.Parameters["@PASSWORD"].Value = Password;
                    command.Parameters.Add(new SqlParameter("@ACCOUNT_TYPE", SqlDbType.VarChar)); command.Parameters["@ACCOUNT_TYPE"].Value = UserAccountType;
                    command.Parameters.Add(new SqlParameter("@USER_ID", SqlDbType.Int)); command.Parameters["@USER_ID"].Value = ST_USERID;
                    command.Parameters.Add(new SqlParameter("@EMPLOYEE_NUM", SqlDbType.VarChar)); command.Parameters["@EMPLOYEE_NUM"].Value = string.IsNullOrEmpty(EmployeeNum) ? (object)DBNull.Value : EmployeeNum;

                    int num_rows = command.ExecuteNonQuery();
                    success = (num_rows == 1);
                }
                return success;
            }
        }

        public bool UpdateEditPostProfile(string aspID, int ST_USERID, string FirstName, string Logon, string LastName, string Email, int defaultProduct, int BlockWhere, string title, string Password, string UserAccountType, string EmployeeNum, string LanguageCode)
        {
            bool success = false;
            string strSql = @"UPDATE AspNetUsers SET
                                 UserName=@USERNAME,
                                 FIRSTNAME=@FIRSTNAME,
                                 LASTNAME=@LASTNAME,
                                 EMAIL=@EMAIL,
                                 LANGUAGECODE=@LANGUAGECODE
                              WHERE
                                ID=@ID 
                             ";
            strSql = strSql + @"UPDATE ST_USERS SET 
                                FIRST_NAME=@FIRST_NAME,
                                LAST_NAME=@LAST_NAME,
                                LOGON=@LOGON,
                                EMAIL=@EMAIL,
                                DEFAULT_PRODUCT=@DEFAULT_PRODUCT,
                                BLOCK_WHERE=@BLOCK_WHERE,
                                TITLE=@TITLE,
                                PASSWORD=@PASSWORD,
                                ACCOUNT_TYPE=@ACCOUNT_TYPE,
                                EMPLOYEE_NUM=@EMPLOYEE_NUM
                            WHERE
                                USER_ID=@USER_ID";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = strSql;
                    command.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar)); command.Parameters["@ID"].Value = aspID;
                    command.Parameters.Add(new SqlParameter("@FIRST_NAME", SqlDbType.VarChar)); command.Parameters["@FIRST_NAME"].Value = FirstName;
                    command.Parameters.Add(new SqlParameter("@LAST_NAME", SqlDbType.VarChar)); command.Parameters["@LAST_NAME"].Value = LastName;
                    command.Parameters.Add(new SqlParameter("@FIRSTNAME", SqlDbType.VarChar)); command.Parameters["@FIRSTNAME"].Value = FirstName;
                    command.Parameters.Add(new SqlParameter("@LASTNAME", SqlDbType.VarChar)); command.Parameters["@LASTNAME"].Value = LastName;
                    command.Parameters.Add(new SqlParameter("@LOGON", SqlDbType.VarChar)); command.Parameters["@LOGON"].Value = Logon;
                    command.Parameters.Add(new SqlParameter("@USERNAME", SqlDbType.VarChar)); command.Parameters["@USERNAME"].Value = Logon;
                    command.Parameters.Add(new SqlParameter("@EMAIL", SqlDbType.VarChar)); command.Parameters["@EMAIL"].Value = Email ==null? (object)DBNull.Value : Email;
                    command.Parameters.Add(new SqlParameter("@DEFAULT_PRODUCT", SqlDbType.Int)); command.Parameters["@DEFAULT_PRODUCT"].Value = defaultProduct;
                    command.Parameters.Add(new SqlParameter("@BLOCK_WHERE", SqlDbType.Int)); command.Parameters["@BLOCK_WHERE"].Value = BlockWhere;
                    command.Parameters.Add(new SqlParameter("@TITLE", SqlDbType.VarChar)); command.Parameters["@TITLE"].Value = title == null ? (object)DBNull.Value : title;
                    command.Parameters.Add(new SqlParameter("@PASSWORD", SqlDbType.VarChar)); command.Parameters["@PASSWORD"].Value = Password;
                    command.Parameters.Add(new SqlParameter("@ACCOUNT_TYPE", SqlDbType.VarChar)); command.Parameters["@ACCOUNT_TYPE"].Value = UserAccountType;
                    command.Parameters.Add(new SqlParameter("@USER_ID", SqlDbType.Int)); command.Parameters["@USER_ID"].Value = ST_USERID;
                    command.Parameters.Add(new SqlParameter("@EMPLOYEE_NUM", SqlDbType.VarChar)); command.Parameters["@EMPLOYEE_NUM"].Value = string.IsNullOrEmpty(EmployeeNum) ? (object)DBNull.Value : EmployeeNum;
                    command.Parameters.Add(new SqlParameter("@LANGUAGECODE", SqlDbType.VarChar)); command.Parameters["@LANGUAGECODE"].Value = LanguageCode;


                    int num_rows = command.ExecuteNonQuery();
                    success = (num_rows == 1);
                }
                return success;
            }
        }
        public DataTable UpdateUserRoles(int userID)
        {
            string command = @"SELECT ST_PRODUCT.NAME AS 'Product', 
                                      ST_ROLES.NAME AS 'Role' 
                              FROM 
                                    ST_PRODUCT 
                              INNER JOIN 
                                    ST_PRODUCT_TEAM ON ST_PRODUCT_TEAM.PRODUCT = ST_PRODUCT.PRODUCT_ID 
                              INNER JOIN 
                                    ST_ROLES ON ST_ROLES.ROLE_ID = ST_PRODUCT_TEAM.ROLE 
                              WHERE 
                                   ST_PRODUCT_TEAM.USER_ID = @USERID ORDER BY ST_PRODUCT.NAME ASC";

            command = command.Replace("@USERID", userID.ToString());
            return ProcessCommand(command);
        }

        /// <summary>
        /// gets all user details
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public DataTable GetUserDetails(int ID)
        {
            string command = $@"
            SELECT 
                ST_USERS.FIRST_NAME AS FIRST_NAME,
                ST_USERS.LAST_NAME AS LAST_NAME, 
                ST_USERS.EMAIL AS EMAIL,
                ST_USERS.PHONE_NUMBER AS PHONE_NUMBER,
                ST_USERS.PHONE_EXTENSION AS PHONE_EXTENSION,
                ST_USERS.ACCOUNT_TYPE AS ACCOUNT_TYPE,
                ST_USERS.FAX_NUMBER AS FAX_NUMBER,
                ST_USERS.OTHER_COMPANY AS OTHER_COMPANY, 
                ST_USERS.DEPARTMENT_ID AS DEPARTMENT, 
                ST_USERS.ADDRESS AS ADDRESS, 
                ST_USERS.CITY AS CITY,
                ST_USERS.ZIP_CODE AS ZIP_CODE, 
                ST_USERS.COUNTRY AS COUNTRY,
                ST_USERS.EMAIL_NOTIFY AS EMAIL_NOTIFY, 
                ST_USERS.EMAIL_PRIORITY AS EMAIL_PRIORITY,
                ST_USERS.TITLE AS TITLE, 
                ST_USERS.COMPANY AS COMPANY_ID,  
                ST_USERS.STATE AS STATE,
                CASE WHEN DEFAULT_PRODUCT IS NULL THEN 32 ELSE DEFAULT_PRODUCT END AS DEFAULT_PRODUCT,
                CASE WHEN DEFAULT_PAGE IS NULL THEN 'Index' ELSE DEFAULT_PAGE END AS DEFAULT_PAGE,
                ST_USERS.IS_MANAGER, 
                ST_USERS.ACCOUNT_TYPE, 
                ST_USERS.DISPLAY_NAME_PREF,
                ST_USERS.HOME_PAGE_LAYOUT,
                ST_PHOTOS.PHOTO
            FROM ST_USERS
            LEFT JOIN ST_PHOTOS on ST_PHOTOS.ID = ST_USERS.PHOTO_ID
            WHERE ST_USERS.USER_ID={ID}";

            return ProcessCommand(command);
        }

        public int GetUserAssignedCount(int userID)
        {
            int count = 0;
            string command = $@"SELECT COUNT(*) FROM REA_TRACK WHERE ASSIGNED_TO = {userID}";
            count = (int)ProcessScalarCommand(command);
            return count;
        }

        public DataTable GetLanguageList()
        {
            string command = @"SELECT LANGUAGE_CODE, LANGUAGE_NAME
                              FROM ST_LANGUAGES";

            return ProcessCommand(command);
        }



    }


}





