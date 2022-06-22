using QVICommonIntranet.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using QVICommonIntranet.Email.EmailREATracker;
namespace REA_Tracker.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {

        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /*
        [EmailAddress( ErrorMessage = "Must enter a valid email")]
        [Display(Name = "Email Address")]
        */
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword                   { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword               { get; set; }

        public bool historyDisplay                  { get; set; }

        public bool homeDisplay                     { get; set; }

        public bool retiredProductDisplay           { get; set; }

        public int latest_submitted_rows            { get; set; }

        public int to_do_list_rows                  { get; set; }

        public int watch_list_rows                  { get; set; }

        public int default_product { get; set; }
        public string default_home_page { get; set; }
        public int display_name_pref { get; set; }
        public int home_page_layout { get; set; }
        public string LanguageCode { get; set; }

        //Constructors
        public ManageUserViewModel() { }//Defualt empty param constructor
        public ManageUserViewModel(String argsFirstName, String argsLastName, String argsEmail, int argsST_USER_ID) //TODO: remove all arguments except for userID
        {
            //this.FirstName = argsFirstName;
            //this.LastName = argsLastName;
            //this.Email = argsEmail;
            this.initSettings(argsST_USER_ID);
        }
        //Getters
        public void initSettings(int userID)
        {
            REATrackerDB sql = new REATrackerDB();
            using (System.Data.DataTable dt = sql.GetSettings(userID))
            {
                //this should always be 1, userId is unique
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];

                    this.FirstName = dr["FIRST_NAME"].ToString();
                    this.LastName = dr["LAST_NAME"].ToString();
                    this.Email = dr["EMAIL"].ToString();
                    this.historyDisplay = (bool)dr["DETAILS"];
                    this.homeDisplay = (bool)dr["HOME_DISPLAY"];
                    this.retiredProductDisplay = (bool)dr["PRODUCT_DISPLAY"];
                    this.latest_submitted_rows = (int)dr["SUBMITTED_DISPLAY"];
                    this.to_do_list_rows = (int)dr["TO_DO_DISPLAY"];
                    this.watch_list_rows = (int)dr["WATCH_DISPLAY"];
                    this.display_name_pref = (int)dr["DISPLAY_NAME_PREF"];
                    this.home_page_layout = (int)dr["HOME_PAGE_LAYOUT"];
                    this.LanguageCode = dr["LanguageCode"].ToString();
                }
                else
                {
                    //this is really bad if it gets here....should never happen.
                }
            }
        }

        public List<dynamic> getRoles(int userID) //TODO: Fix this
        {
            ///<summary>
            /// Gets the roles that the user is in for each team 
            ///the user is on
            ///</summary>
            ///<param name="userID">
            /// the User ID 
            ///</param>
            List<dynamic> list = new List<dynamic>();
            REATrackerDB _sql = new REATrackerDB();
            //To-Do revist the product_access and prodict_team relation vendiagram 
            _sql.OpenConnection();
            using (System.Data.DataTable dt = _sql.UpdateUserRoles(userID))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].Product = (string)dt.Rows[i]["Product"];
                    list[i].Role = (string)dt.Rows[i]["Role"];
                    i++;
                }
            }
            _sql.CloseConnection();
            return list;
        }

        public List<dynamic> getLatestSubmittedList()
        {
            ///<summary>
            /// gets the latest Submitted List of REAS
            ///</summary>
            List<dynamic> list = new List<dynamic>();
            list.Add(new System.Dynamic.ExpandoObject());
            list[0].Value = -1;
            list[0].Name = "Last 7 Days";
            list[0].IsDefault = (this.latest_submitted_rows == list[0].Value);
            //21 is a magic number in the old code aswell
            for (int i = 1; i < 21; i++ )
            {
                list.Add(new System.Dynamic.ExpandoObject());
                list[i].Value = i;
                list[i].Name = Convert.ToString(i);
                list[i].IsDefault = (this.latest_submitted_rows == list[i].Value);
            }
            return list;
        }

        public List<dynamic> getToDoList()
        {
            ///<summary>
            /// Gets the todo list for the current user
            ///</summary>
            List<dynamic> list = new List<dynamic>();
            list.Add(new System.Dynamic.ExpandoObject());
            list[0].Value = -1;
            list[0].Name = "All";
            list[0].IsDefault = (this.to_do_list_rows == list[0].Value);
            //21 is a magic number in the old code aswell
            for (int i = 1; i < 21; i++)
            {
                list.Add(new System.Dynamic.ExpandoObject());
                list[i].Value = i;
                list[i].Name = Convert.ToString(i);
                list[i].IsDefault = (this.to_do_list_rows == list[i].Value);
            }
            return list;
        }

        public List<dynamic> getWatchList()
        {
            ///<summary>
            /// Get the watch list for the current user
            ///</summary>
            List<dynamic> list = new List<dynamic>();
            list.Add(new System.Dynamic.ExpandoObject());
            list[0].Value = -1;
            list[0].Name = "All";
            list[0].IsDefault = (this.watch_list_rows == list[0].Value);
            //21 is a magic number in the old code aswell not sure why
            // 21 is also half of 42?
            for (int i = 1; i < 21; i++)
            {
                list.Add(new System.Dynamic.ExpandoObject());
                list[i].Value = i;
                list[i].Name = Convert.ToString(i);
                list[i].IsDefault = (this.watch_list_rows == list[i].Value);
            }
            return list;
        }
        // method to handle name order preference
        public List<dynamic> getNameDisplayPrefList()
        {
            ///<summary>
            /// Get the Display Name Preference list for the current user
            ///</summary>
            List<dynamic> list = new List<dynamic>();
            int i = 0;
            list.Add(new System.Dynamic.ExpandoObject());
            list[i].Value    = 0;
            list[i].Name = "FirstName LastName";
            list[i].IsDefault = (this.display_name_pref == list[i].Value);
            i++;

            list.Add(new System.Dynamic.ExpandoObject());
            list[i].Value = 1;
            list[i].Name = "LastName, FirstName";
            list[i].IsDefault = (this.display_name_pref == list[i].Value);
            return list;

        }

        public List<dynamic> getProducts(int userID) //TODO: fix this
        {
            ///<summary>
            /// gets a list of products that the user has access to and populates
            /// the select list
            ///</summary>
            ///<param name="userID">
            /// The ID of teh user
            ///</param>
            REATrackerDB _sql = new REATrackerDB();
            int defaultProduct = (int)_sql.ProcessScalarCommand("SELECT DEFAULT_PRODUCT FROM ST_USERS WHERE USER_ID= " + userID);
            List<dynamic> list = new List<dynamic>();
            using (System.Data.DataTable dt = _sql.GetProductList(userID))
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

        public List<dynamic> getLanguages(int userID)
        {
            ///<summary>
            /// gets a list of languages that the user has access to and populates
            /// the select list
            ///</summary>
            ///<param name="userID">
            /// The ID of teh user
            ///</param>
            REATrackerDB _sql = new REATrackerDB();
            string defaultLanguage = (string)_sql.ProcessScalarCommandSCR("SELECT LanguageCode FROM AspNetUsers WHERE ST_USER_ID= " + userID);
            List<dynamic> list = new List<dynamic>();
            using (System.Data.DataTable dt = _sql.GetLanguageList())
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].ID = dt.Rows[i]["LANGUAGE_CODE"].ToString();
                    list[i].Name = dt.Rows[i]["LANGUAGE_NAME"].ToString();
                    list[i].IsDefault = (defaultLanguage == list[i].ID);
                    i++;
                }
            }
            return list;
        }


        //Modifiers (setters)
        public void ManageEditProfile(string AspUserID, int ST_USER_ID)
        {
            REATrackerDB scrDB = new REATrackerDB();

            string cmd = "UPDATE AspNetUsers SET FirstName = '" + this.FirstName.Trim() +
                "', LastName ='" + this.LastName.Trim() +
                "', Email='" + this.Email.Trim() +
                "', LanguageCode='" + this.LanguageCode.Trim() +
                "' WHERE id = '" + AspUserID + "';";

            cmd = cmd +
                " UPDATE ST_USERS SET FIRST_NAME = '" + this.FirstName +
                "', LAST_NAME = '" + this.LastName +
                "', EMAIL='" + this.Email +
                "', DEFAULT_PRODUCT =" + this.default_product +
                ", DEFAULT_PAGE = '" + this.default_home_page + "'" +
                ", DISPLAY_NAME_PREF = '" +this.display_name_pref + "'" +
                " WHERE USER_ID = " + ST_USER_ID + " ;";

            using (DataTable dt = scrDB.ProcessCommand(cmd))
            {
                HttpContext context = HttpContext.Current;
                context.Session["CurrentUserName"] = this.FirstName + " " + this.LastName;
                context.Session["DefaultHomePage"] = this.default_home_page;
                context.Session["DefaultProduct"] = this.default_product;
            }
        }
        //session is getting set after choosing name order in profile
        public void ManageEditDisplay(string AspUserID, int ST_USER_ID) //TODO: remove AspUserID
        {
            REATrackerDB sql = new REATrackerDB();

            string sqlcommand =
                "UPDATE ST_USERS SET DETAILS =" + Convert.ToInt32(this.historyDisplay) +
                ", HOME_DISPLAY = "+Convert.ToInt32(this.homeDisplay) +
                ", PRODUCT_DISPLAY = "+Convert.ToInt32(this.retiredProductDisplay) +
                ", SUBMITTED_DISPLAY ="+ Convert.ToString(this.latest_submitted_rows) +
                ", TO_DO_DISPLAY = "+ Convert.ToString(this.to_do_list_rows) +
                ", WATCH_DISPLAY = "+ Convert.ToString(this.watch_list_rows) +
                ", DISPLAY_NAME_PREF="+ Convert.ToInt32(this.display_name_pref) +
                ", HOME_PAGE_LAYOUT=" + Convert.ToInt32(this.home_page_layout) +
                " WHERE USER_ID=" +Convert.ToString(ST_USER_ID) +
                ";"
                ;

            sqlcommand = sqlcommand + "UPDATE AspNetUsers SET LanguageCode='" + this.LanguageCode.Trim() +
                "' WHERE id = '" + AspUserID + "';";

            using (DataTable dt = sql.ProcessCommand(sqlcommand))
            {
                HttpContext context = HttpContext.Current;
                context.Session["DisplayNamePref"] = this.display_name_pref;
                context.Session["HomeDisplay"] = this.homeDisplay;
                context.Session["ProductDisplay"] = this.retiredProductDisplay;
                context.Session["SubmittedDisplay"] = this.latest_submitted_rows;
                context.Session["TodoDisplay"] = this.to_do_list_rows;
                context.Session["WatchDisplay"] = this.watch_list_rows;
                context.Session["HomePageLayout"] = this.home_page_layout;
            }


        }
    }

    [NotMapped]
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName          { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password          { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe          { get; set; }

        public String BrowserType       { get; set; }

        public String BrowserVersion    { get; set; }

        public String UserAgent         { get; set; }

        public String IPAddress         { get; set; }

        public void LogUser( int UserID  )
        {
            ///<summary>
            /// Logs the user in
            ///</summary>
            ///<param name="UserID">
            /// The ID of the User
            ///</param>

            String[] BrowserArray = BrowserVersion.Split('.');
            String Major ="";
            String Minor ="";
            if(BrowserVersion.Length > 0)
            {
                Major = BrowserArray[0];
            }
            if(BrowserVersion.Length > 1)
            {
                Minor = BrowserArray[1];
            }

            String IP = this.IPAddress;
            //Logs the browser Type
            String Command = "INSERT INTO ST_BROWSER"+
                "( IP, USER_ID, TYPE, MAJOR, MINOR, USER_AGENT, LOGINTIME) "+
                " VALUES('"
                + this.IPAddress + "'," +
                Convert.ToString(UserID) +",'"+
                this.BrowserType+"',"+
                Major+","+Minor+",'"+
                this.UserAgent.Replace("'","''")+"','"+
                Convert.ToString(DateTime.Now)
                +"')";
            REATrackerDB sql = new REATrackerDB();
            sql.ProcessCommand(Command);

        }
    }

    public class CreateUserViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        //Create a ST_USER
        public string Email         { get; set; }

        public string FirstName     { get; set; }

        public string LastName      { get; set; }

        public int AccountType      { get; set; }

        public int RawSQLAccess     { get; set; }

        public int BlockWhere       { get; set; }

        public bool isManager       { get; set; }

        public int TreeLevel { get; set; }

        public int ManagerID        { get; set; }

        public int DepartmentID     { get; set; }

        public int CompanyID     { get; set; }

        //Products

        public List<dynamic> OtherActive { get; set; }

        public List<dynamic> OtherRetired { get; set; }

        public List<dynamic> AccessActive { get; set; }

        public List<dynamic> AccessRetired { get; set; }

        public List<dynamic> DepartmentList { get; set; }

        public List<dynamic> CompanyList { get; set; }

        public bool FullAccess { get; set; }

        public String hiddenString { get; set; }

        public String LanguageCode { get; set; }


        public CreateUserViewModel()
        {
            this.OtherActive = new List<dynamic>();
            this.OtherRetired = new List<dynamic>();
            this.AccessActive = new List<dynamic>();
            this.AccessRetired = new List<dynamic>();
            this.DepartmentList = this.GetDepartments();
            this.CompanyList = this.GetCompanies();
            this.AccountType = 1;
            this.LanguageCode = "en";
            foreach(var row in this.getProducts())
            {
                if (row.IsRetired)
                {
                    this.OtherRetired.Add(row);
                }
                else
                {
                    this.OtherActive.Add(row);
                }
            }
            // this.FirstName = "";
            // this.LastName ="";
            // this.Email = "";
            // this.RawSQLAccess = 0;
        }

        private List<dynamic> GetDepartments()
        {
            List<dynamic> DisplayListOfDepartments = new List<dynamic>();
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.GetAllDepartments();
            int i = 0;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                DisplayListOfDepartments.Add(new System.Dynamic.ExpandoObject());
                DisplayListOfDepartments[i].Name = Convert.ToString(row["NAME"]);
                DisplayListOfDepartments[i].ID = Convert.ToString(row["DEPARTMENT_ID"]);
                DisplayListOfDepartments[i].IsDefault = false;
                i++;
            }
            return DisplayListOfDepartments;
        }

        private List<dynamic> GetCompanies()
        {
            List<dynamic> list = new List<dynamic>();
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.GetAllCompanies();
            int i = 0;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                list.Add(new System.Dynamic.ExpandoObject());
                if (Convert.ToString(row["DESCRIPTION"]) != "")
                {
                    list[i].Name = Convert.ToString(row["NAME"]) + " - " + Convert.ToString(row["DESCRIPTION"]);
                }
                else
                {
                    list[i].Name = Convert.ToString(row["NAME"]);
                }

                list[i].ID = Convert.ToString(row["COMPANY_ID"]);
                list[i].IsDefault = false;
                i++;
            }
            return list;
        }

        [Required(ErrorMessage= "Default Product is Required.")]
        public int defaultProduct   { get; set; }
        public List<dynamic> getProducts()
        {
            ///<Summary>
            ///  Gets a list of products
            ///</Summary>
            List<dynamic> Products = new List<dynamic>();
            REATrackerDB sql = new REATrackerDB();
            String ProductsCommand = "SELECT ST_PRODUCT.PRODUCT_ID, ST_PRODUCT.NAME, ST_PRODUCT.RETIRED FROM ST_PRODUCT ORDER BY NAME ASC;";
            DataTable dtProducts = sql.ProcessCommand(ProductsCommand);
            int i = 0;
            foreach( System.Data.DataRow row in dtProducts.Rows )
            {
                Products.Add(new System.Dynamic.ExpandoObject());
                Products[i].ID = Convert.ToInt32(row["PRODUCT_ID"]);
                Products[i].Name = Convert.ToString(row["NAME"]);
                Products[i].IsDefault = false;
                Products[i].IsRetired = Convert.ToBoolean(row["RETIRED"]);
                i++;
            }
            return Products;
        }

        //Added in Default product and product access
        public List<dynamic> AccountTypes()
        {
            ///<summary>
            /// Gets a list of account types and returns a list of dynamic objects
            /// to display
            ///</summary>
            List<dynamic> AccountTypes = new List<dynamic>();
            REATrackerDB sql = new REATrackerDB();
            String Command = "SELECT ACCOUNT_ID, NAME FROM ST_ACCOUNT_TYPE";
            DataTable dtProducts = sql.ProcessCommand(Command);
            int i = 0;
            foreach (System.Data.DataRow row in dtProducts.Rows)
            {
                AccountTypes.Add(new System.Dynamic.ExpandoObject());
                AccountTypes[i].ID = Convert.ToInt32(row["ACCOUNT_ID"]);
                AccountTypes[i].Name = Convert.ToString(row["NAME"]);
                AccountTypes[i].IsDefault = false;
                i++;
            }
            return AccountTypes;
        }
        public List<dynamic> ManagerList()
        {
            ///<summary>
            /// Gets a list of mangers 
            /// in the options tag
            ///</summary>
            List<dynamic> Manager = new List<dynamic>();
            REATrackerDB sql = new REATrackerDB();
            DataTable dtProducts = sql.GetAllManagers();
            int i = 1;
            Manager.Add(new System.Dynamic.ExpandoObject());
            Manager[0].ID = -1;
            Manager[0].Name = "";
            Manager[0].IsDefault = false;
            foreach (System.Data.DataRow row in dtProducts.Rows)
            {
                Manager.Add(new System.Dynamic.ExpandoObject());
                Manager[i].ID = Convert.ToInt32(row["USER_ID"]);
                Manager[i].Name = Convert.ToString(row["FIRST_NAME"])+" "+Convert.ToString(row["LAST_NAME"]);
                Manager[i].IsDefault = false;
                i++;
            }
            return Manager;
        }

    }

    public class ViewUserModel
    {
        public bool Exists { get; set; }
        public String FullName { get; set; }
        public String AccountType { get; set; }
        public String Company { get; set; }
        public String Department { get; set; }
        public String Address { get; set; }
        public String Email { get; set; }
        public String PhoneNumber { get; set; }
        public String FaxNumber { get; set; }
        public String JobTitle { get; set; }
        public byte[] Photo { get; set; }

        private int AccountTypeID { get; set; }
        private int DepartmentID { get; set; }

        private int ID { get; set; }

        public ViewUserModel() { this.Exists = false; }

        public ViewUserModel(int ST_userID)
        {
            this.ID = ST_userID;
            this.init();
        }

        public void init()
        {
            ///<summary>
            /// Initates the model and populates the feilds
            ///</summary>
            REATrackerDB sql = new REATrackerDB();
            sql.OpenConnection();
            DataTable dt = sql.GetUserDetails(this.ID);
            this.Exists = false;
            if(dt.Rows.Count >0)
            {
                this.Exists = true;
                this.FullName = Convert.ToString(dt.Rows[0]["FIRST_NAME"]) + " " + Convert.ToString(dt.Rows[0]["LAST_NAME"]);
                this.Email = Convert.ToString(dt.Rows[0]["EMAIL"] == DBNull.Value ? null : dt.Rows[0]["EMAIL"]);
                this.PhoneNumber = "None";
                if (!(dt.Rows[0]["PHONE_NUMBER"] == DBNull.Value) && (!(Convert.ToString(dt.Rows[0]["PHONE_NUMBER"]).ToLower().Equals("none"))))
                {
                    this.PhoneNumber = Convert.ToString(dt.Rows[0]["PHONE_NUMBER"]);
                }
                if (!(dt.Rows[0]["PHONE_EXTENSION"] == DBNull.Value))
                {
                    int ext = 0;
                    if (int.TryParse(dt.Rows[0]["PHONE_EXTENSION"].ToString(), out ext))
                    {
                        this.PhoneNumber += " x" + ext.ToString();
                    }
                }
                this.AccountType = this.getAccountType(dt.Rows[0]["ACCOUNT_TYPE"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["ACCOUNT_TYPE"]));
                this.FaxNumber = Convert.ToString(dt.Rows[0]["FAX_NUMBER"] == DBNull.Value ? null : dt.Rows[0]["FAX_NUMBER"]);
                this.Company = this.GetCompany(dt.Rows[0]["COMPANY_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["COMPANY_ID"]));
                this.Department = this.GetDepartment(dt.Rows[0]["DEPARTMENT"] == DBNull.Value ? (int?)null : Convert.ToInt32(dt.Rows[0]["DEPARTMENT"]));
                this.Address = Convert.ToString(dt.Rows[0]["ADDRESS"] == DBNull.Value ? "" : dt.Rows[0]["ADDRESS"]); //+ '\n' + Convert.ToString(dt.Rows[0][10] == DBNull.Value ? "" : dt.Rows[0][10]) + ", " + Convert.ToString(this.GetState((int?)dt.Rows[0][17])) + " " + Convert.ToString(dt.Rows[0][11] == DBNull.Value ? "" : dt.Rows[0][11]);
                this.JobTitle = Convert.ToString(dt.Rows[0]["TITLE"] == DBNull.Value ? null : dt.Rows[0]["TITLE"]);
                this.Photo = (byte[])(dt.Rows[0]["PHOTO"] != DBNull.Value ? dt.Rows[0]["PHOTO"] : (object)null);
            }
            sql.CloseConnection();
        }
        private string GetState(int? input)
        {
            ///<summary>
            /// Gets the names of the state
            ///</summary>
            ///<param name="input">
            /// the id of the state
            ///</param>
            String cmdText = "SELECT NAME FROM ST_STATE_LIST WHERE STATE_ID=" + input;
            String value = "";
            if (input != null)
            {
                REATrackerDB sql = new REATrackerDB();
                sql.OpenConnection();
                DataTable dt = sql.ProcessCommand(cmdText);
                if (dt.Rows.Count > 0)
                {
                    value = Convert.ToString(dt.Rows[0][0]);
                }
                sql.CloseConnection();
            }
            return value;
        }
        private string GetCompany(int? input)
        {
            ///<summary>
            /// Gets the company name
            ///</summary>
            ///<param name="input">
            /// The id of the company
            ///</param>
            String cmdText = "SELECT NAME FROM ST_COMPANY_LIST WHERE COMPANY_ID=" + input;
            String value = "";
            if (input != null)
            {
                REATrackerDB sql = new REATrackerDB();
                sql.OpenConnection();
                DataTable dt = sql.ProcessCommand(cmdText);
                if (dt.Rows.Count > 0)
                {
                    value = Convert.ToString(dt.Rows[0][0]);
                }
                sql.CloseConnection();
            }
            return value;
        }
        private string GetDepartment(int? input)
        {
            ///<summary>
            /// Gets the department name
            ///</summary>
            ///<param name="input">
            /// The ID of the Department
            ///</param>
            String cmdText = "SELECT NAME FROM ST_DEPARTMENT WHERE DEPARTMENT_ID=" + input;
            String value = "";
            if (input != null)
            {
                REATrackerDB sql = new REATrackerDB();
                sql.OpenConnection();
                DataTable dt = sql.ProcessCommand(cmdText);
                if (dt.Rows.Count > 0)
                {
                    value = Convert.ToString(dt.Rows[0][0]);
                }
                sql.CloseConnection();
            }
            return value;
        }
        private string getAccountType(int? input)
        {
            ///<summary>
            /// Gets the account type
            ///</summary>
            ///<param name="input">
            /// gets the name of the account type of the input
            ///</param>
            String cmdText = "SELECT NAME FROM ST_ACCOUNT_TYPE WHERE ACCOUNT_ID="+input;
            String value = "";
            if(input != null)
            {
                REATrackerDB sql = new REATrackerDB();
                sql.OpenConnection();
                DataTable dt = sql.ProcessCommand(cmdText);
                if (dt.Rows.Count > 0)
                {
                    value = Convert.ToString(dt.Rows[0][0]);
                }
                sql.CloseConnection();
            }
            return value;
        }
    }

    public class ViewToDoTeam
    {
        public class toDoListItem
        {
            public int REANum { get; set; }
            public String product { get; set; }
            public String title { get; set; }
            public String priority { get; set; }
            public int priorityid { get; set; }
            public String status { get; set; }
            public int statusid { get; set; }
            public String type { get; set; }
            public int typeid { get; set; }
            public String plannedVersion { get; set; }
            public string plannedVersionid { get; set; }
            public int assginedTo { get; set; }

            public toDoListItem() { }

            public string getplannedVersion(string input)
            {
                string results = "";
                string[] resultsArray = input.Split('.');
                foreach(string version in resultsArray)
                {
                    results +=version.PadLeft(4,'0');
                }
                return results;
            }
        }
        public List<toDoListItem> TeamTodos { get; set;}
        public List<dynamic> TeamMembers { get; set; }
        private string UserList;
        public int UserID {get; set;}
        public int TotalCount { get; set; }

        public ViewToDoTeam( int argsUserID)
        {
            this.UserID = argsUserID;
            this.populateTeamMembers();
            this.populateTeamTodos();
        }

        /// <summary>
        /// Gets the heiarchy of the current user (assuming they are a manager)
        /// </summary>
        private void populateTeamMembers()
        {
            this.TeamMembers = new List<dynamic>();
            this.UserList = "";
            String Command = $@"
WITH MyCTE AS (
  SELECT (ST_USERS.FIRST_NAME+' '+ST_USERS.LAST_NAME) AS FULLNAME, ST_USERS.USER_ID, ST_USERS.IS_MANAGER,  0 AS TreeLevel, CAST( ST_USERS.USER_ID AS VARCHAR(255)) AS TreePath
    FROM ST_USERS
    WHERE ST_USERS.MANAGER_ID={this.UserID} AND ACCOUNT_TYPE <> -1

  UNION ALL

  SELECT (T2.FIRST_NAME+' '+T2.LAST_NAME) AS FULLNAME, T2.USER_ID, T2.IS_MANAGER, TreeLevel + 1, CAST(TreePath + ', ' + CAST(T2.USER_ID AS VARCHAR(255)) AS VARCHAR(255)) AS TreePath
    FROM ST_USERS T2

  INNER JOIN MyCTE itms ON itms.USER_ID = T2.MANAGER_ID
)
SELECT *
FROM  MyCTE 
ORDER BY TreePath;";
            REATrackerDB sql = new REATrackerDB();
            DataTable dtMember = sql.ProcessCommand(Command);
            for (int i = 0; i < dtMember.Rows.Count; i++)
            {
                TeamMembers.Add(new System.Dynamic.ExpandoObject());
                TeamMembers[i].TreeLevel = Convert.ToInt32(dtMember.Rows[i]["TreeLevel"]);
                string temp = Convert.ToString(dtMember.Rows[i]["FULLNAME"]);
                TeamMembers[i].Name = temp;
                TeamMembers[i].ID = Convert.ToInt32(dtMember.Rows[i]["USER_ID"]);
                TeamMembers[i].IsManager = Convert.ToInt32(dtMember.Rows[i]["IS_MANAGER"]);
                TeamMembers[i].WebName = System.Text.RegularExpressions.Regex.Replace(temp, "[^A-Za-z0-9]", "");
                this.UserList += TeamMembers[i].ID.ToString() + ",";
            }
            //remove the last comma
            UserList = UserList.TrimEnd(',');
        }
        private void populateTeamTodos()
        {
            ///<summary>
            /// populates the team todos
            ///</summary>
            this.TeamTodos = new List<toDoListItem>();
            REATrackerDB sql = new REATrackerDB();
            String Command =
            "SELECT"
            + " ST_PRODUCT.NAME AS PRODUCT, ST_TRACK.TITLE, ST_priority.NAME AS priority, ST_TRACK.PRIORITY AS PRIORITY_ID, ST_STATUS.NAME AS STATUS, ST_TRACK.STATUS AS STATUS_ID, ST_ISSUE_TYPE.NAME AS ISSUE_TYPE, ST_TRACK.ISSUE_TYPE AS ISSUE_TYPE_ID, "
            + " ST_TRACK.VP_MAJOR, ST_TRACK.VP_MINOR, ST_TRACK.VP_BUILD, ST_TRACK.VP_MISC, ST_TRACK.ASSIGNED_TO, ST_TRACK.TRACKING_ID"
            + " FROM ST_TRACK "
            + " INNER JOIN ST_PRODUCT ON ST_PRODUCT.PRODUCT_ID = ST_TRACK.PRODUCT "
            + " INNER JOIN ST_priority ON ST_priority.ID = ST_TRACK.priority "
            + " INNER JOIN ST_STATUS ON ST_STATUS.STATUS_ID = ST_TRACK.STATUS "
            + " INNER JOIN ST_ISSUE_TYPE ON ST_ISSUE_TYPE.ISSUE_ID = ST_TRACK.ISSUE_TYPE "
            + " WHERE ST_TRACK.ASSIGNED_TO IN "
            + " ( "
            + " SELECT "
            + " ST_USERS.USER_ID"
            + " FROM ST_USERS"
            + " WHERE ST_USERS.USER_ID IN ("+UserList+") AND ST_USERS.ACCOUNT_TYPE != -1"
            + " )";
            DataTable infoDt = sql.ProcessCommand(Command);
            for (int i = 0; i < infoDt.Rows.Count; i++ )
            {
                this.TeamTodos.Add(new toDoListItem());
                this.TeamTodos[i].REANum = Convert.ToInt32(infoDt.Rows[i]["TRACKING_ID"]);
                this.TeamTodos[i].product = Convert.ToString(infoDt.Rows[i]["PRODUCT"]);
                this.TeamTodos[i].plannedVersion = GetRelease(Convert.ToString(infoDt.Rows[i]["VP_MAJOR"]),
                                                                Convert.ToString(infoDt.Rows[i]["VP_MINOR"]),
                                                                Convert.ToString(infoDt.Rows[i]["VP_BUILD"]),
                                                                Convert.ToString(infoDt.Rows[i]["VP_MISC"]));
                this.TeamTodos[i].plannedVersionid = TeamTodos[i].getplannedVersion(Convert.ToString(this.TeamTodos[i].plannedVersion));
                this.TeamTodos[i].title     = Convert.ToString(infoDt.Rows[i]["TITLE"]);
                this.TeamTodos[i].priority = Convert.ToString(infoDt.Rows[i]["priority"]);
                this.TeamTodos[i].priorityid = Convert.ToInt32(infoDt.Rows[i]["priority_id"]);
                this.TeamTodos[i].status    = Convert.ToString(infoDt.Rows[i]["STATUS"]);
                this.TeamTodos[i].statusid = Convert.ToInt32(infoDt.Rows[i]["STATUS_ID"]);
                this.TeamTodos[i].type      = Convert.ToString(infoDt.Rows[i]["ISSUE_TYPE"]);
                this.TeamTodos[i].typeid = Convert.ToInt32(infoDt.Rows[i]["ISSUE_TYPE_id"]);
                this.TeamTodos[i].assginedTo = Convert.ToInt32(infoDt.Rows[i]["ASSIGNED_TO"]);
            }
            this.TotalCount = infoDt.Rows.Count;
            foreach(var TeamMate in this.TeamMembers)
            {
                TeamMate.Count =
                    (this.TeamTodos.FindAll(
                        x => x.assginedTo == Convert.ToInt32(TeamMate.ID)).Count
                        );
            }
        }

        private string GetRelease(string major, string minor, string build, string misc)
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
    }

    public class ForgotViewModel
    {
        //User inputs
        public string user_id {get; set;}
        public string email {get; set;}
        public int preoption { get; set; }
        //1- password
        //2- userid
        //3- other problems

        //Inputs for Other problems
        public string name { get; set; }
        public string emailinput { get; set; }
        public string subject { get; set; }
        public string details { get; set; }

        //Email inputs
        public string EmailBody {get; set;}

        public ForgotViewModel(){}

        public Boolean sendEmail()
        {
            bool result = false;
            switch(this.preoption)
            {
                case 1:
                    result = this.ForgotPassword();
                    break;
                case 2:
                    result = this.ForgotUserID();
                    break;
                case 3:
                    result = this.Otherissues();
                    break;
                default:
                    break;
            }
            return result;
        }

        private bool ForgotPassword()
        {
            EmailForgotPasword mail = new EmailForgotPasword(this.user_id);
            return mail.Send();
        }

        private bool ForgotUserID()
        {
            EmailForgotUserName mail = new EmailForgotUserName(this.email);
            return mail.Send();
        }

        private bool Otherissues()
        {
            EmailContactAdmin mail = new EmailContactAdmin(
                this.name,this.emailinput,this.subject,this.details);
            return mail.Send();
        }
    }
}
