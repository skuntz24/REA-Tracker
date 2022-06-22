using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{
    public class AdminIndexViewModel
    {
        public String SearchUserID          { get; set; }
        public String SearchEmail           { get; set; }
        public String SearchFirstName       { get; set; }
        public String SearchLastName       { get; set; }
        public Boolean ShowEX { get;set; }

        public AdminIndexViewModel(){ }
        public List<dynamic>getUsers()
        {
            ///<summary>
            ///populates the users to display
            ///</summary>
            List<dynamic> ListofUsers = new List<dynamic>();
            REATrackerDB _sql = new REATrackerDB();
            using (System.Data.DataTable dt = _sql.GetUsers(ShowEX))
            {
                int i = 0;
                foreach(var row in dt.Rows)
                {
                    ListofUsers.Add(new System.Dynamic.ExpandoObject());
                    ListofUsers[i].UserID = (int)dt.Rows[i]["USER_ID"];
                    ListofUsers[i].Login = (string)dt.Rows[i]["LOGON"];
                    ListofUsers[i].FirstName = (string)dt.Rows[i]["FIRST_NAME"];
                    ListofUsers[i].LastName = (string)dt.Rows[i]["LAST_NAME"];
                    ListofUsers[i].FullName = ((string)dt.Rows[i]["FIRST_NAME"] +" "+ (string)dt.Rows[i]["LAST_NAME"]);
                    ListofUsers[i].EmailAddress = Convert.ToString(dt.Rows[i]["EMAIL"]).Replace(";","\n");
                    ListofUsers[i].AccountType = Convert.ToString(dt.Rows[i]["ACCOUNT_TYPE_NAME"]) == "UNKNOWN USER" ? "" : Convert.ToString(dt.Rows[i]["ACCOUNT_TYPE_NAME"]);
                    //To display 'other company' and its name in the user manager page
                    if (Convert.ToString(dt.Rows[i]["COMPANY_ID"]) == "0" )
                    {
                        ListofUsers[i].Company = Convert.ToString((dt.Rows[i]["COMPANY_NAME"]) + " - " + Convert.ToString((dt.Rows[i]["OTHER_COMPANY"])));
                    }
                    if(Convert.ToString(dt.Rows[i]["COMPANY_ID"]) == "0" && Convert.ToString(dt.Rows[i]["OTHER_COMPANY"]) !="" && Convert.ToString(dt.Rows[i]["OTHER_COMPANY"]) !=null)
                    {
                        ListofUsers[i].Company = Convert.ToString((dt.Rows[i]["COMPANY_NAME"]) + " - " + Convert.ToString((dt.Rows[i]["OTHER_COMPANY"])));
                    }
                    else
                    {
                        ListofUsers[i].Company = Convert.ToString((dt.Rows[i]["COMPANY_NAME"]));
                    }

                    if (Convert.ToString(dt.Rows[i]["COMPANY_ID"]) != "0")
                    {
                        ListofUsers[i].Company = Convert.ToString((dt.Rows[i]["COMPANY_NAME"]));
                    }
                    ListofUsers[i].Department = Convert.ToString(dt.Rows[i]["DEPARTMENT_NAME"]);
                    ListofUsers[i].Boss = Convert.ToString(dt.Rows[i]["BOSS"]) == "System Account" ? "" : Convert.ToString(dt.Rows[i]["BOSS"]);
                    i++;
                }
            }
            if (!String.IsNullOrEmpty(this.SearchUserID))
            {
                ListofUsers = ListofUsers.Where(x => x.Login.ToLower().Contains(this.SearchUserID.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(this.SearchEmail))
            {
                ListofUsers = ListofUsers.Where(x => x.EmailAddress.ToLower().Contains(this.SearchEmail.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(this.SearchFirstName))
            {
                ListofUsers = ListofUsers.Where(x => x.FirstName.ToLower().Contains(this.SearchFirstName.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(this.SearchLastName))
            {
                ListofUsers = ListofUsers.Where(x => x.LastName.ToLower().Contains(this.SearchLastName.ToLower())).ToList();
            }
            return ListofUsers;
        }
    }

    public class EditUserManagerViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /*[EmailAddress(ErrorMessage = "Must enter a valid email")]
        [Display(Name = "Email Address")]*/
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string Password { get; set; }

        [Display(Name = "Account type")]
        public string AccountType { get; set; }

        [Display(Name = "Employee Number")]
        public string EmployeeNum { get; set; }

        [Display(Name = "Logon")]
        public string Logon { get; set; }

        [Display(Name = "Default Product")]
        public int defaultProduct { get; set; }

        [Display(Name = "Company")]
        public int company {get; set; }

        [Display(Name = "Other Company")]
        public string othercompany { get; set; }

        [Display(Name = "Title")]
        public string title { get; set; }

        [Display(Name = "Department")]
        public int department{ get; set; }

        [Display(Name = "Is a Manager")]
        public bool ismanager{ get; set; }

        [Display(Name = "Manager ID")]
        public int managerid { get; set; }

        [Display(Name = "Raw SQL Access")]
        public int RawSQLAccess { get; set; }

        [Display(Name = "Where Clause Access")]
        public int BlockWhere { get; set; }

        [Display(Name = "Access")]
        public List<int> access {get; set;}

        [Display(Name ="Give Full Access")]
        public Boolean fullAccess { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Language")]
        public string LanguageCode { get; set; }

        public HttpPostedFileBase photo { get; set; }

        public byte[] CurrentPhoto { get; set; }

        //Display list
        public int newProductTeam { get; set; }

        public List<dynamic> OtherActive { get; set; }

        public List<dynamic> OtherRetired { get; set; }

        public List<dynamic> AccessActive { get; set; }

        public List<dynamic> AccessRetired { get; set; }

        public List<dynamic> AccountTypes { get; set; }

        public List<RoleItem> UserRoles { get; set; }

        public List<RoleItem> AllRoles { get; set; }
        //Inputs
        public int newRole { get; set; }

        public int AddProductID { get; set; }

        public int userID { get; set; }

        public EditUserManagerViewModel()
        {
            AllRoles = UserRoles = new List<RoleItem>();
            this.OtherActive = new List<dynamic>();
            this.OtherRetired = new List<dynamic>();
            this.AccessActive = new List<dynamic>();
            this.AccessRetired = new List<dynamic>();
        }

        public EditUserManagerViewModel(int UserID, string accountType)
        {

            this.userID = UserID;
            this.AccountType = accountType;
            REATrackerDB sql = new REATrackerDB();
            using (DataTable dt = sql.GetSettings(UserID))
            {
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];

                    this.EmployeeNum = dr["EMPLOYEE_NUM"].ToString();
                    this.FirstName = dr["FIRST_NAME"].ToString();
                    this.LastName = dr["LAST_NAME"].ToString();
                    this.Email = dr["EMAIL"].ToString();
                    this.Password = dr["PASSWORD"].ToString();
                    this.defaultProduct = int.Parse(dr["DEFAULT_PRODUCT"].ToString());
                    this.Logon = dr["LOGON"].ToString();

                    //this eventually should not be used
                    this.title = dr["TITLE"].ToString();
                    this.department = int.Parse(dr["DEPARTMENT_ID"].ToString());
                    this.company = int.Parse(dr["COMPANY"].ToString());
                    this.othercompany = dr["OTHER_COMPANY"].ToString();
                    this.ismanager = (bool)dr["IS_MANAGER"];
                    this.managerid = int.Parse(dr["MANAGER_ID"].ToString());

                    this.fullAccess = (bool)dr["FULL_ACCESS"];
                    this.BlockWhere = 1;
                    if (dr["BLOCK_WHERE"] != DBNull.Value)
                    {
                        this.BlockWhere = (bool)dr["BLOCK_WHERE"] ? 1 : 0;
                    }
                    this.RawSQLAccess = (bool)dr["EnableRawSQL"] ? 1 : 0;
                    this.access = new List<int>();
                    using (DataSet dtProductAccess = sql.GetProductList2(UserID))
                    {
                        foreach (DataRow row in dtProductAccess.Tables["Active"].Rows)
                        {
                            access.Add((int)row["PRODUCT_ID"]);
                        }
                    }
                }
                else
                {
                    //no user information found...error
                }
            }
            this.populateAccountType();
            this.populateAllRoles();
            this.UserRoles = this.getUsersRoles();
            this.populateAccessLists();
            this.populatePhoto();
        }

        //   =============================================================================
        //  || UserManger/Edit [Get]                                                     ||
        //   =============================================================================

        //products partial
        private void  populateAccountType()
        {
            ///<summary>
            /// populates the account types to edit
            ///</summary>
            REATrackerDB _sql = new REATrackerDB();
            _sql.OpenConnection();
            List<dynamic> list = new List<dynamic>();
            using (System.Data.DataTable dt = _sql.RoleType())
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].ID = Convert.ToString(dt.Rows[i]["ASPID"]);
                    list[i].Name = Convert.ToString(dt.Rows[i]["ASPNAME"]);
                    list[i].IsDefault = (AccountType == list[i].ID);
                    i++;
                }
            }
            _sql.CloseConnection();
            this.AccountTypes = list;
        }

        public void populateAllRoles()
        {
            ///<summary>
            /// populates all roles
            ///</summary>
            this.AllRoles = new List<RoleItem>();
            REATrackerDB _sql = new REATrackerDB();
            _sql.OpenConnection();
            using (System.Data.DataTable dt = _sql.Roles())
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    AllRoles.Add(new RoleItem((int)dt.Rows[i]["ROLE_ID"], (String)dt.Rows[i]["ROLE_NAME"]));
                    i++;
                }
            }
            _sql.CloseConnection();
        }

       
        public void populateAccessLists()
        {
            ///<summary>
            /// populates the list of what access people have
            ///</summary>
            ///
            //Pre-Condition: UsersRole must be populated
            //Post-Condition: AccessRetired, AccessActive, OtherActive, and OtherRetired are populated
            this.OtherActive = new List<dynamic>();
            this.OtherRetired = new List<dynamic>();
            this.AccessActive = new List<dynamic>();
            this.AccessRetired = new List<dynamic>();
            REATrackerDB _sql = new REATrackerDB();
            string cmd = "SELECT PRODUCT_ID, NAME, RETIRED FROM ST_PRODUCT ORDER BY NAME ASC";
            _sql.OpenConnection();
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmd))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    bool exist = false;
                    if(access!=null)
                    {
                        exist = access.Contains((int)dt.Rows[i][0]);
                    }
                    if (exist)
                    {
                        //add it to access list
                        if ((bool)dt.Rows[i][2])
                        {
                            //retired
                            this.AccessRetired.Add(new System.Dynamic.ExpandoObject());
                            this.AccessRetired[this.AccessRetired.Count - 1].ID = (int)dt.Rows[i][0];
                            this.AccessRetired[this.AccessRetired.Count - 1].Name = Convert.ToString((String)dt.Rows[i][1]);
                        }
                        else
                        {
                            //active
                            this.AccessActive.Add(new System.Dynamic.ExpandoObject());
                            this.AccessActive[this.AccessActive.Count - 1].ID = (int)dt.Rows[i][0];
                            this.AccessActive[this.AccessActive.Count - 1].Name = Convert.ToString((String)dt.Rows[i][1]);
                        }
                    }
                    else
                    {
                        //add it to other list
                        if ((bool)dt.Rows[i][2])
                        {
                            //retired
                            this.OtherRetired.Add(new System.Dynamic.ExpandoObject());
                            this.OtherRetired[this.OtherRetired.Count - 1].ID = (int)dt.Rows[i][0];
                            this.OtherRetired[this.OtherRetired.Count - 1].Name = Convert.ToString((String)dt.Rows[i][1]);
                        }
                        else
                        {
                            //active
                            this.OtherActive.Add(new System.Dynamic.ExpandoObject());
                            this.OtherActive[this.OtherActive.Count - 1].ID = (int)dt.Rows[i][0];
                            this.OtherActive[this.OtherActive.Count - 1].Name = Convert.ToString((String)dt.Rows[i][1]);
                        }
                    }
                    i++;
                }
            }
            _sql.CloseConnection();
        }
        //getters
        public List<dynamic> getProducts()
        {
            ///<summary>
            /// gets the list of products
            ///</summary>
            REATrackerDB _sql = new REATrackerDB();
            int temp = 0;
            Int32.TryParse(Convert.ToString(_sql.ProcessScalarCommand("SELECT DEFAULT_PRODUCT FROM ST_USERS WHERE USER_ID= " + userID)), out temp);
            int defaultProduct = temp;
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

        public List<dynamic> getLanguages()
        {
            ///<summary>
            /// gets a list of languages that the user has access to and populates
            /// the select list
            ///</summary>
            ///<param name="userID">
            /// The ID of teh user
            ///</param>
            REATrackerDB _sql = new REATrackerDB();
            string defaultLanguage = (string)_sql.ProcessScalarCommand("SELECT ISNULL(LanguageCode, 'en') As LanguageCode FROM AspNetUsers WHERE ST_USER_ID= " + userID);
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

        private List<RoleItem> getUsersRoles()
        {
            ///<summary>
            /// gets the list of user roles
            ///</summary>
            List<RoleItem> list = new List<RoleItem>();
            REATrackerDB _sql = new REATrackerDB();
            String sqlCommand = "SELECT ST_PRODUCT.NAME AS 'Product', " +
                "ST_PRODUCT.PRODUCT_ID AS 'Product id', "+
                "ST_ROLES.NAME AS 'Role Name', "+
                "ST_ROLES.ROLE_ID AS 'Role' FROM ST_PRODUCT " +
                "INNER JOIN ST_PRODUCT_TEAM ON ST_PRODUCT_TEAM.PRODUCT = ST_PRODUCT.PRODUCT_ID " +
                "INNER JOIN ST_ROLES ON ST_ROLES.ROLE_ID = ST_PRODUCT_TEAM.ROLE " +
                "WHERE ST_PRODUCT_TEAM.USER_ID = " + Convert.ToInt32(userID) +
                " ORDER BY ST_PRODUCT.NAME ASC;";
            //To-Do revist the product_access and prodict_team relation vendiagram 
            _sql.OpenConnection();
            using (System.Data.DataTable dt = _sql.ProcessCommand(sqlCommand))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new RoleItem((int)dt.Rows[i]["Role"],Convert.ToString(dt.Rows[i]["Role Name"])));
                    list[i].ProductID = (int)dt.Rows[i]["Product id"];
                    list[i].ProductName = Convert.ToString(dt.Rows[i]["Product"]);
                    i++;
                }
            }
            _sql.CloseConnection();
            return list;
        }

        public SelectList GetRoleSelectList(int defaultRole)
        {
            ///<summary>
            /// gets a select list depending on the role
            ///</summary>
            ///<param name="defualtRole">
            /// the role that the current user is in
            ///</param>
            List<SelectListItem> Roles =new List<SelectListItem>();
            foreach(var item in AllRoles)
            {
                Roles.Add(new SelectListItem(){Text = item.RoleName, Value = item.RoleID.ToString()});
            }
            return new SelectList(Roles, "Value", "Text", defaultRole);
        }

        public List<dynamic> getNewAccessProducts()
        {
            ///<summary>
            /// Gets the list of new acess products
            ///</summary>

            REATrackerDB _sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            using (System.Data.DataTable dt = _sql.GetProductList(userID))
            {
                int i = 0;//index of list
                int j = 0;//index of datatable
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    var temp = UserRoles.Where(u=>u.ProductID == (int)dt.Rows[j][0]);
                    if(temp != null && temp.Count()==0)
                    {
                        list.Add(new System.Dynamic.ExpandoObject());
                        list[i].ID = (int)dt.Rows[j][0];
                        list[i].Name = dt.Rows[j][1].ToString();
                        list[i].IsDefault = (defaultProduct == list[i].ID);
                        i++;
                    }
                    j++;
                }
            }
            return list;
        }

        //sub classes
        public class RoleItem
        {
            public int RoleID { get; set; }
            public int ProductID { get; set; }
            public String RoleName { get; set; }
            public String ProductName { get; set; }
            public bool Delete { get; set; }
            public RoleItem( int roleID, String NAME)
            {
                this.RoleID = roleID;
                this.RoleName = NAME;
                this.Delete = false;
            }
            public RoleItem() { }
        }
        //Company partial
        //getters
        public List<dynamic> getCompany()
        {
            ///<summary>
            /// gets a list of companies
            ///</summary>
            ///
            REATrackerDB _sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();

            using (System.Data.DataTable dt = _sql.GetAllCompanies())
            {

                int i= 0;
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
                    list[i].ID =(int)row["COMPANY_ID"];
                    list[i].IsDefault = (this.company == list[i].ID);
                    i++;
                }
            }
            return list;
        }


        public List<dynamic> getDepartment()
        {
            ///<summary>
            /// Gets a list of deparments
            ///</summary>

            REATrackerDB _sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            using (System.Data.DataTable dt = _sql.GetAllDepartments())
            {
                list.Add(new System.Dynamic.ExpandoObject());
                list[0].Name = "No Declared Department";
                list[0].ID = -1;
                list[0].IsDefault = (this.department == list[0].ID);
                int i = 1;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].Name = Convert.ToString(dt.Rows[i-1][2]+" "+dt.Rows[i-1][1]);
                    list[i].ID = (int)dt.Rows[i-1][0];
                    list[i].IsDefault = ( this.department == list[i].ID);
                    i++;
                }
            }
            return list;
        }

        public List<dynamic> getManager()
        {
            ///<summary>
            /// gets a list of managers
            ///</summary>

            REATrackerDB _sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            using (System.Data.DataTable dt = _sql.GetAllManagers())
            {
                int i = 1;
                list.Add(new System.Dynamic.ExpandoObject());
                list[0].ID = -1;
                list[0].Name = "";
                list[0].IsDefault = false;

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].Name = Convert.ToString(row["FIRST_NAME"]) + " " + Convert.ToString(row["LAST_NAME"]);
                    list[i].ID = (Convert.ToInt32(row["USER_ID"]));
                    list[i].IsDefault = (this.managerid == list[i].ID);
                    i++;
                }
            }
            return list;
        }

        public void populatePhoto()
        {
            this.CurrentPhoto = new REATrackerDB().getPhoto(this.userID);
        }

        //   =============================================================================
        //  || UserManger/Edit [Post]                                                    ||
        //   =============================================================================
        //change profile
        public void EditPostProfile(string aspID, int ST_USERID)
        {
            ///<summary>
            ///Edits a profile
            ///</summary>
            ///<param name="aspID">
            /// The asp ID of the user you are editing
            ///</param>
            ///<param name="ST_USERID">
            /// The ST_USER ID of the user you are editing
            ///</param>
            REATrackerDB sql = new REATrackerDB();
            //Updating AspUsers' attributes
            //User Role
            var idManager = new IdentityManager();
            idManager.ClearUserRoles(aspID);
            bool pass = idManager.AddUserToRoleById(aspID, AccountType);

            //user's account type
            string UserAccountType = Convert.ToString(this.getSTAccountType(this.AccountType));
            sql.UpdateEditPostProfile(aspID, ST_USERID, this.FirstName, this.Logon, this.LastName, this.Email, this.defaultProduct, this.BlockWhere, this.title, this.Password, UserAccountType, this.EmployeeNum, this.LanguageCode);
            sql.OpenConnection();
           
            sql.CloseConnection();
            HttpContext context = HttpContext.Current;
            if (context.Session["st_userID"].ToString() == ST_USERID.ToString())
            {
                context.Session["CurrentUserName"] = this.FirstName + " " + this.LastName;
                //context.Session["DefaultHomePage"] = this.default_home_page;
                context.Session["DefaultProduct"] = this.defaultProduct;
            }
            // Convert the HttpPostdfilebase into a bit array

            if (this.photo != null)
            {
                byte[] uploadedFile = new byte[this.photo.InputStream.Length];
                this.photo.InputStream.Read(((byte[])uploadedFile), 0, uploadedFile.Length);
                //making the call
                sql.saveProfilePhoto(uploadedFile, ST_USERID);
            }
        }
        public void EditPostProfile(int ST_USERID)
        {
            ///<summary>
            /// Edits the profile of a user
            ///</summary>
            ///<param name="ST_USERID">
            /// the st_user_id of the user
            ///</param>
            REATrackerDB sql = new REATrackerDB();
            sql.OpenConnection();
            string UserAccountType = Convert.ToString(this.getSTAccountType(this.AccountType));
            sql.UpdateEditPostProfile(ST_USERID, this.FirstName, this.LastName, this.Logon, this.Email, this.defaultProduct, this.BlockWhere, this.Password, UserAccountType, this.EmployeeNum);
            sql.CloseConnection();

            //we have to check to see if the current user is the user profile we are editing, if so then update the context
            HttpContext context = HttpContext.Current;
            if (context.Session["st_userID"].ToString() == ST_USERID.ToString())
            {
                context.Session["CurrentUserName"] = this.FirstName + " " + this.LastName;
                //context.Session["DefaultHomePage"] = this.default_home_page;
                context.Session["DefaultProduct"] = this.defaultProduct;
            }
        }
        private int getSTAccountType(string aspRoleID)
        {
            ///<summary>
            /// gets the account type from the st_table
            ///</summary>
            ///<param name="aspRoleID">
            /// the asp id
            ///</param>

            REATrackerDB sql = new REATrackerDB();
            sql.OpenConnection();
            string cmd =
                "SELECT ST_ACCOUNT_TYPE.ACCOUNT_ID FROM ST_ACCOUNT_TYPE "+
                "INNER JOIN AspNetRoles ON ST_ACCOUNT_TYPE.NAME = AspNetRoles.Name " +
                "WHERE AspNetRoles.Id = '"+aspRoleID+"';";


            int Account_ID = Convert.ToInt32(sql.ProcessScalarCommand(cmd));
            sql.CloseConnection();
            return Account_ID;
        }
        //change in product access
        public void EditPostProduct(int ST_USER_ID, string hiddenString)
        {
            ///<summary>
            ///Edit the post product
            ///</summary>
            ///<param name="ST_USER_ID">
            /// the st_user ID
            ///</param>
            ///<param name="hiddenString">
            ///The hidden string populated in the hiddden feild that list the products
            ///</param>
            String cmd = "";
            REATrackerDB sql = new REATrackerDB();
            sql.OpenConnection();
            if (fullAccess)
            {
                cmd += " UPDATE ST_USERS SET Full_Access = 1 WHERE USER_ID = " + ST_USER_ID.ToString();
            }
            else
            {
                cmd += " UPDATE ST_USERS SET Full_Access = 0 WHERE USER_ID = " + ST_USER_ID.ToString();
            }
            sql.ProcessCommand(cmd);
            cmd += " UPDATE ST_USERS SET PROD_ACCESS = '" +hiddenString.TrimEnd() +"' WHERE USER_ID = " + ST_USER_ID.ToString()+"; ";
            sql.ProcessCommand(cmd);
            //remove all Access
            cmd +=" DELETE FROM ST_PRODUCT_ACCESS WHERE UserID = "+ST_USER_ID.ToString()+"; ";
            sql.ProcessCommand(cmd);
            //inject new Access
            List<int> newaccess = ParseHiddenString(hiddenString);
            for(int i=0; i<newaccess.Count(); i++)
            {
                cmd += " INSERT INTO ST_PRODUCT_ACCESS(UserID, ProductID) VALUES("+ST_USER_ID.ToString()+","+newaccess[i]+");";
            }
            sql.ProcessCommand(cmd);
            sql.CloseConnection();
        }
        private List<int> ParseHiddenString(string hs)
        {
            ///<summary>
            ///Parses the Hidden String
            ///</summary>
            ///<param name="hs">
            ///The hidden string
            ///</param>
            List<int> ListOfAccess = new List<int>();
            String number = "";
            int output = 0;
            for (int i = 0; i < hs.Count(); i++ )
            {
                if (hs[i] == ' ' && number != "")
                {
                    Int32.TryParse(number, out output);
                    ListOfAccess.Add(output);
                    number = "";
                }
                else
                {
                    number += hs[i];
                }
            }
            if ( !String.IsNullOrEmpty(number) )
            {
                Int32.TryParse(number, out output);
                ListOfAccess.Add(output);
            }
            return ListOfAccess;
        }
        //Add to team
        public void addTeam(int USER_ID)
        {
            ///<summary>
            /// adds a memeber to the team
            ///</summary>
            ///<param name="USER_ID">
            /// The user to add to the team
            ///</param>
            string cmd = "INSERT INTO ST_PRODUCT_TEAM(PRODUCT, USER_ID,ROLE) VALUES (" + AddProductID + "," + USER_ID + "," + newRole + ");";
            REATrackerDB sql = new REATrackerDB();
            sql.OpenConnection();
            sql.ProcessCommand(cmd);
            
            sql.CloseConnection();
        }
        //change in product role     
        public void EditPostProductRoles(int ST_USER_ID)
        {
            ///<summary>
            ///Edits the product roles
            ///</summary>
            ///<param name="ST_USER_ID">
            /// The user id to change the current role
            ///</param>
            string cmd = "";
            REATrackerDB sql = new REATrackerDB();
            sql.OpenConnection();
            //remove all Current Roles
            cmd += "DELETE FROM ST_PRODUCT_TEAM WHERE USER_ID = " + ST_USER_ID.ToString() + "; ";
            //inject new Roles
            foreach (var i in this.UserRoles)
            {
                if(i.Delete==false)
                {
                    cmd += "INSERT INTO ST_PRODUCT_TEAM(PRODUCT, USER_ID, ROLE) VALUES (" +
                        i.ProductID.ToString() + "," +
                        ST_USER_ID.ToString() +
                        "," + i.RoleID.ToString() + ");";
                }
            }
            sql.ProcessCommand(cmd);
            sql.CloseConnection();
        }
        //change in company
        public void EditPostCompany(int st_user)
        {
            ///<summary>
            /// Edits the Company information of a user
            ///</summary>
            ///<param name="st_user">
            /// the st_user id
            ///</param>
      
            REATrackerDB sql = new REATrackerDB();
            sql.OpenConnection();

            sql.UpdateEditPostCompany(this.company, this.othercompany, this.department, this.ismanager, this.managerid, st_user);
      
            sql.CloseConnection();
        }
        //clear photo
        public void DeletePhoto()
        {
            new REATrackerDB().ProcessCommand(
                "DELETE FROM ST_PHOTOS WHERE USER_ID = "+this.userID
                );
        }
    }

    public class SessionManagerViewModel
    {

        private List<Tuple<string, string>> AllBrowserTypes  { get; set; }
        private List<int> BrowserCount                       { get; set; }
        public int TotalCount                               { get; set; }

        public List<string> TopBrowserName { get; set; }
        private List<string> TopBrowserCount { get; set; }

        private List<List<string>> MajorBrowserLabel { get; set; }
        private List<List<string>> MajorBrowserDataSet { get; set; }

        public string TopBrowserXValues { get; set; }
        public string TopBrowserYValues { get; set; }

        public List<string> BrowserXValues  { get; set; }
        public List<string> BrowserYValues   { get; set; }


        public SessionManagerViewModel()
        {
            this.populateBrowser();
            this.populateGraphData();

        }

        private void populateBrowser()
        {
            //last 30 days
            this.AllBrowserTypes = new List<Tuple<string, string>>();
            String command =
                "SELECT DISTINCT [TYPE], [MAJOR] FROM [REA_BROWSER] WHERE (LOGINTIME > GETDATE() - 30) ORDER BY TYPE, MAJOR DESC";
            DataTable dt = new REATrackerDB().ProcessCommand(command);
            for (int i = 0; i < dt.Rows.Count; i++ )
            {
                this.AllBrowserTypes.Add(
                    new Tuple<string,string>(
                        Convert.ToString(dt.Rows[i]["TYPE"]),
                        Convert.ToString(dt.Rows[i]["MAJOR"])
                        )
                );
            }
            this.populateCount();
        }

        private void populateCount()
        {
            this.BrowserCount = new List<int>();
            String command = "SELECT COUNT(*) FROM REA_BROWSER WHERE (LOGINTIME > GETDATE() - 30) AND TYPE =";
            for (int i = 0; i < AllBrowserTypes.Count;i++ )
            {
                int count = Convert.ToInt32(
                    new REATrackerDB().ProcessScalarCommand
                    (
                        command+"'"+AllBrowserTypes[i].Item1+
                        "' AND MAJOR = "+AllBrowserTypes[i].Item2
                    )
                );
                BrowserCount.Add(
                    count
                );
                TotalCount += count;
            }
        }

        private void populateGraphData()
        {
            TopBrowserName = new List<string>();
            TopBrowserCount = new List<string>();
            MajorBrowserLabel = new List<List<string>>();
            MajorBrowserDataSet = new List<List<string>>();
            BrowserXValues = new List<string>();
            BrowserYValues = new List<string>();

            for (int i = 0; i < this.AllBrowserTypes.Count(); i++)
            {
                //populating top broswer data and count
                bool isFireFox = AllBrowserTypes[i].Item1=="Firefox";
                bool isChrome = AllBrowserTypes[i].Item1=="Chrome";
                string Firefoxlabel = "";
                string Chromelabel = "";
                if(isFireFox)
                {
                    int upperBound = (((Convert.ToInt32(AllBrowserTypes[i].Item2) / 10) ) * 10)+9;
                    int lowerBound = (((Convert.ToInt32(AllBrowserTypes[i].Item2) / 10) ) * 10);
                    Firefoxlabel = lowerBound + "-" + upperBound;
                }
                else if(isChrome)
                {
                    int upperBound = (((Convert.ToInt32(AllBrowserTypes[i].Item2) / 10) ) * 10)+9;
                    int lowerBound = (((Convert.ToInt32(AllBrowserTypes[i].Item2) / 10) ) * 10);
                    Chromelabel = lowerBound + "-" + upperBound;
                }
                //Check if it exists i the TopBroswer data set
                if (!TopBrowserName.Contains(AllBrowserTypes[i].Item1))
                {//If not then add it to the data set
                    this.TopBrowserName.Add(AllBrowserTypes[i].Item1);
                    this.TopBrowserCount.Add(BrowserCount[i].ToString());
                    if(!isFireFox && !isChrome)
                    {// if its not fox use the basic one
                        MajorBrowserLabel.Add(new List<string>() { AllBrowserTypes[i].Item2 + ".X.X" });
                        MajorBrowserDataSet.Add(new List<string>() { Convert.ToString(BrowserCount[i]) });
                    }
                    else
                    {
                        //if its firefox or chrome
                        if (isFireFox)
                        {
                            MajorBrowserLabel.Add(
                                new List<string>() {
                                    Convert.ToString(Firefoxlabel)
                                    }
                                );
                        }
                        else //If its chrome
                        {
                            MajorBrowserLabel.Add(
                            new List<string>() {
                                Convert.ToString(Chromelabel)
                                }
                            );
                        }
                        MajorBrowserDataSet.Add(new List<string>() { Convert.ToString(BrowserCount[i]) });
                    }
                }
                else
                {
                    int? indexIntoBroswerCount = null;
                    for (int k = 0; k < TopBrowserName.Count;k++ )
                    {
                        if(TopBrowserName[k]==
                            Convert.ToString(AllBrowserTypes[i].Item1))
                        {
                            TopBrowserCount[k]= Convert.ToString(
                                    Convert.ToInt32(TopBrowserCount[k])+ BrowserCount[i]
                                    );

                            indexIntoBroswerCount = k;
                            break;
                        }
                    }
                    if(indexIntoBroswerCount != null)
                    {//Indexes into the Browser then adds the major info
                        if (!isFireFox && ! isChrome)
                        {
                            MajorBrowserLabel[(int)indexIntoBroswerCount].Add(AllBrowserTypes[i].Item2 + ".X.X");
                            MajorBrowserDataSet[(int)indexIntoBroswerCount].Add(Convert.ToString(BrowserCount[i]));
                        }
                        else
                        {//if its firefox or chrome
                            bool found = false;
                            int indexFound = -1;
                            for (int k = 0;
                                k < this.MajorBrowserLabel[(int)indexIntoBroswerCount].Count;
                                k++ )
                            {
                                string[] label = this.MajorBrowserLabel[(int)indexIntoBroswerCount][k].Split('-');
                                if(label.Length==2)
                                {
                                    int lowerBound = Convert.ToInt32(label[0]);
                                    int upperBound = Convert.ToInt32(label[1]);
                                    int curentNumber = Convert.ToInt32(AllBrowserTypes[i].Item2);
                                    if ( curentNumber >= lowerBound &&
                                            curentNumber <= upperBound
                                        )
                                    {
                                        found = true;
                                        indexFound = k;
                                        break;
                                    }
                                }
                            }
                            if (found)
                            {
                                MajorBrowserDataSet[(int)indexIntoBroswerCount][indexFound] =
                                    Convert.ToString(Convert.ToInt32(MajorBrowserDataSet[(int)indexIntoBroswerCount][indexFound])+
                                    BrowserCount[i]);
                            }
                            else
                            {
                                if(isFireFox)
                                {
                                    MajorBrowserLabel[(int)indexIntoBroswerCount].Add(Convert.ToString(Firefoxlabel));
                                    MajorBrowserDataSet[(int)indexIntoBroswerCount].Add(Convert.ToString(BrowserCount[i]));
                                }
                                else if(isChrome)
                                {
                                    MajorBrowserLabel[(int)indexIntoBroswerCount].Add(Convert.ToString(Chromelabel));
                                    MajorBrowserDataSet[(int)indexIntoBroswerCount].Add(Convert.ToString(BrowserCount[i]));
                                }
                            }
                        }
                    }

                }
            }
            this.TopBrowserXValues = string.Join(",", TopBrowserCount);
            this.TopBrowserYValues = string.Join(",", TopBrowserName);
            for (int k = 0; k < MajorBrowserLabel.Count;k++ )
            {
                BrowserXValues.Add(string.Join(",", MajorBrowserDataSet[k]));
                BrowserYValues.Add( string.Join(",", MajorBrowserLabel[k]));
            }
        }

        public void Filter()
        {
            string command = "SELECT ID,USER_AGENT FROM REA_BROWSER";
            DataTable dt = new REATrackerDB().ProcessCommand(command);

            for (int i = 0; i < dt.Rows.Count;i++ )
            {
                string browser = "";
                string version = "";
                int nameOffset = 0;
                int verOffset = 0;
                int ix = 0;
                int major = -1;

                string useragent = Convert.ToString(dt.Rows[i]["USER_AGENT"]);
                //Opera
                if( useragent.IndexOf("Opera")!=-1)
                {
                    browser ="Opera";
                    version = useragent.Substring(verOffset+6);
                    if((verOffset=useragent.IndexOf("Version"))!=-1)
                    {
                        version = useragent.Substring(verOffset+8);
                    }
                }
                else if (useragent.IndexOf("OPR")!=-1)
                {
                    browser = "Opera";
                    version = useragent.Substring(verOffset+4);
                }
                //MSIE
                else if(useragent.IndexOf("MSIE")!=-1)
                {
                    browser = "Internet Explorer";
                    version =useragent.Substring(verOffset+5);
                    int tempMajor = -1;
                    string temp =
                        useragent.Substring(useragent.IndexOf("MSIE") + 5);
                    int nextnumber = temp.IndexOf('.');
                    string majorString = temp.Substring(0, nextnumber);
                    Int32.TryParse(majorString, out tempMajor);
                    if (tempMajor != -1 && tempMajor != 0)
                    {
                        major = tempMajor;
                    }
                }
                //Edge
                else if(useragent.IndexOf("Edge")!=-1)
                {
                    browser = "Internet Explorer";
                    version = useragent.Substring(verOffset + 5);
                    int tempMajor = -1;
                    string temp =
                        useragent.Substring(useragent.IndexOf("Edge") + 5);
                    int nextnumber = temp.IndexOf('.');
                    string majorString = temp.Substring(0, nextnumber);
                    Int32.TryParse(majorString, out tempMajor);
                    if (tempMajor != -1 && tempMajor != 0)
                    {
                        major = tempMajor;
                    }
                }
                //Chrome
                else if (useragent.IndexOf("Chrome")!=-1)
                {
                    browser = "Chrome";
                    version = useragent.Substring(verOffset+7);
                    int tempMajor = -1;
                    string temp =
                        useragent.Substring(useragent.IndexOf("Chrome") + 7);
                    int nextnumber = temp.IndexOf('.');
                    string majorString = temp.Substring(0, nextnumber);
                    Int32.TryParse(majorString, out tempMajor);
                    if (tempMajor != -1 && tempMajor != 0)
                    {
                        major = tempMajor;
                    }
                }
                //Safari
                else if(useragent.IndexOf("Safari")!=-1)
                {
                    browser = "Safari";
                    version=useragent.Substring(verOffset+7);
                    if( (verOffset = useragent.IndexOf("Version"))!=-1 )
                    {
                        version = useragent.Substring(verOffset +8);
                    }
                    int tempMajor = -1;
                    string temp =
                        useragent.Substring(useragent.IndexOf("Version") + 8);
                    int nextnumber = temp.IndexOf('.');
                    string majorString = temp.Substring(0, nextnumber);
                    Int32.TryParse(majorString, out tempMajor);
                    if (tempMajor != -1 && tempMajor != 0)
                    {
                        major = tempMajor;
                    }
                }
                //firefox
                else if (useragent.IndexOf("Firefox")!=-1)
                {
                    browser = "Firefox";
                    version = useragent.Substring(verOffset+8);
                    int tempMajor = -1;
                    string temp =
                        useragent.Substring(useragent.IndexOf("Firefox")+8);
                    int nextnumber = temp.IndexOf('.');
                    string majorString = temp.Substring(0, nextnumber);
                    Int32.TryParse(majorString, out tempMajor);
                    if (tempMajor != -1 && tempMajor!=0)
                    {
                        major = tempMajor;
                    }
                }
                //MSIE11+
                else if (useragent.IndexOf("Trident/") != -1)
                {
                    browser = "Internet Explorer";
                    version =useragent.Substring(useragent.IndexOf("rv:")+3);
                    int tempMajor = -1;
                    string temp =
                        useragent.Substring(useragent.IndexOf("rv:") + 3);
                    int nextnumber = temp.IndexOf('.');
                    string majorString = temp.Substring(0, nextnumber);
                    Int32.TryParse(majorString, out tempMajor);
                    if (tempMajor != -1 && tempMajor != 0)
                    {
                        major = tempMajor;
                    }

                }
                //other browsers
                else if
                (
                    (nameOffset = useragent.LastIndexOf(' ')+1) <
                    ( verOffset = useragent.LastIndexOf('/'))
                )
                {
                    browser = useragent.Substring(nameOffset, verOffset);
                    version = useragent.Substring(verOffset + 1);

                }
                if ((ix = version.IndexOf(';')) != -1)
                {
                    version = version.Substring(0, ix);
                }
                if ((ix = version.IndexOf(' ')) != -1)
                {
                    version = version.Substring(0, ix);
                }
                if ((ix = version.IndexOf(')')) != -1)
                {
                    version = version.Substring(0, ix);
                }

                if(major == -1)
                {
                    int temp = -1;
                    Int32.TryParse(version, out temp);
                    if(temp!=-1)
                    {
                        major = temp;
                    }
                }
                string updateCommand =
                    "UPDATE ST_BROWSER SET " +
                        " TYPE = '" + browser +
                        "' ";
                if(major!=-1 && major!=0)
                {
                    updateCommand += ",MAJOR =" + major;
                }
                updateCommand+=
                " WHERE ID = "+
                Convert.ToString(dt.Rows[i]["ID"]);
                new REATrackerDB().ProcessCommand(updateCommand);
            }
        }
    }

    //==============================Validators=====================================
    public class IntegerNoZeroAttribute : ValidationAttribute {
        public override bool IsValid(object value)
        {
            int getal=0;
            if(value==null||int.TryParse(value.ToString(), out getal))
            {
                if (getal > 0)
                    return true;
            }
            return false;
        }
    }
}