using System;
using System.Collections.Generic;
using System.Data;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{

    public class ProductManagerViewModel
    {
        //Edit Teammates
        public List<dynamic> ListOfRoles { get; set; }
        public int NewMemmberID { get; set; }
        public int NewRoleID { get; set; }
        //Modules
        public String NewModuleName { get; set; }
        public String NewModuleDescription { get; set; }
        //Basic INformation
        public int productID { get; set; }
        public String CurrentProductName { get; set; }
        public int DefaultSCCBID = 135;//Default Tester 276 is the default test queue's st_user's id
        public int DefaultTesterID = 276;//SCCB  Default SCCB is 135 which is  SCCB Rochester
        //Displays
        public List<Module> ProductModules { get; set; }
        public List<dynamic> ProductTeam { get; set; }
        public List<dynamic> ProductName { get; set; }

        public String Message { get; set; }
        //Product basics
        public int Owner { get; set; }
        public int Vaildator { get; set; }
        public int DefaultTester { get; set; }
        public int ReleaseCoordinator { get; set; }
        public int ReleaseCoordinator2 { get; set; }
        public int SCCB { get; set; }
        public bool Retired { get; set; }
        public bool isInternal { get; set; }
        public int BillingCode { get; set; }
        public string DistributionList { get; set; }
        public string Description { get; set; }
        public bool UseMetrics { get; set; }
        //Create and Update Metrics
        public int MetricID { get; set; }
        public String BuildVersion { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public int CodeCriticalCount { get; set; }
        public int CodeHighCount { get; set; }
        public int CodeMetrics { get; set; }
        public int CodeDefectCount { get; set; }
        public int ReleaseCriticalCount { get; set; }
        public int ReleaseHighCount { get; set; }
        public int ReleaseMetric { get; set; }
        public int ReleaseDefectCount { get; set; }
        public bool Display { get; set; }
        public List<String> ListBuilds { get; set; }
        public List<dynamic> ListOfMetrics { get; set; }

        //Drop downlists for add/drop teammate and change roles
        public List<dynamic> ListOfAllEmployees { get; set; }
        public List<TeammateRowItem> ListOfTeammates { get; set; }
        //Constructor
        public ProductManagerViewModel(int productIDParam)
        {
            this.productID = productIDParam;
            this.init();
        }

        public ProductManagerViewModel()
        {
            //Needed for post
            this.init();
        }
        //============================================================================
        //      Get/init Functions                                                    |
        //============================================================================
        public void init()
        {
            ///<summary>
            /// the main init function
            ///</summary>
            this.ProductTeam = this.GetProductTeamList();
            this.ProductName = this.GetProductList();
           

            this.ProductModules = this.GetProductModulesList();
            this.ListOfRoles = this.RoleDropdown();
            this.PopulateProductBasics();
            this.setupProductTeamList();
            this.ListOfAllEmployees = this.GetListOfAllEmployees();

        }

        private void PopulateProductBasics()
        {
            ///<summary>
            /// populates the product basics
            ///</summary>
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.GetProductBasicDT(this.productID);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                this.Vaildator = Convert.ToInt32(row[0]);
                this.DefaultTester = Convert.ToInt32(row[1] == DBNull.Value ? 0 : row[1]);
                this.ReleaseCoordinator = Convert.ToInt32(row[2] == DBNull.Value ? 0 : row[2]);
                this.ReleaseCoordinator2 = Convert.ToInt32(row[3] == DBNull.Value ? 0 : row[3]);
                this.SCCB = Convert.ToInt32(row[4] == DBNull.Value ? 0 : row[4]);
                this.Retired = Convert.ToBoolean(row[5]);
                this.isInternal = Convert.ToBoolean(row[6]);
                this.BillingCode = Convert.ToInt32(row[7]);
                this.DistributionList = Convert.ToString(row[8] == DBNull.Value ? null : row[8]);
                this.Description = Convert.ToString(row[9] == DBNull.Value ? null : row[9]);
                this.CurrentProductName = Convert.ToString(row[10]);
                this.UseMetrics = Convert.ToBoolean(row["METRICS"]);
                this.Owner = Convert.ToInt32(row["OWNER"] ==DBNull.Value ? 0 : row["OWNER"]);
            }
        }

        private List<dynamic> GetProductTeamList()
        {
            ///<summary>
            /// populates the product team list
            ///</summary>
            List<dynamic> list = new List<dynamic>();
            String command =
                "SELECT ST_USERS.FIRST_NAME, ST_USERS.LAST_NAME, ST_USERS.LOGON, ST_PRODUCT_TEAM.USER_ID, ST_ROLES.NAME " +
                "FROM ST_PRODUCT_TEAM " +
                "INNER JOIN ST_USERS ON ST_USERS.USER_ID = ST_PRODUCT_TEAM.USER_ID " +
                "INNER JOIN ST_ROLES ON ST_ROLES.ROLE_ID = ST_PRODUCT_TEAM.ROLE " +
                "WHERE ST_PRODUCT_TEAM.PRODUCT = " + this.productID + " AND ST_USERS.ACCOUNT_TYPE != -1 " +
                "ORDER BY ST_USERS.FIRST_NAME ASC, ST_USERS.LAST_NAME ASC";
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(command);
            int i = 0;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                list.Add(new System.Dynamic.ExpandoObject());
                list[i].Name = Convert.ToString(row[0]) + " " + Convert.ToString(row[1]);
                list[i].Logon = Convert.ToString(row[2]);
                list[i].ID = Convert.ToInt32(row[3]);
                list[i].Role = Convert.ToString(row[4]);
                i++;
            }
            return list;
        }

        private List<dynamic> GetProductList()
        {
            ///<summary>
            /// returns the list of products
            ///</summary>
            List<dynamic> list = new List<dynamic>();
            String command =
                "SELECT ST_PRODUCT.PRODUCT_ID, ST_PRODUCT.NAME " +
                "FROM ST_PRODUCT order by NAME ASC;";
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(command);
            int i = 0;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                list.Add(new System.Dynamic.ExpandoObject());
                list[i].ID = Convert.ToInt32(row[0]);
                list[i].Name = Convert.ToString(row[1]);
                list[i].IsDefault = (list[i].ID == this.productID);
                i++;
            }
            return list;
        }

        private List<Module> GetProductModulesList()
        {
            ///<summary>
            /// returns a list of product modules
            ///</summary>
            List<Module> list = new List<Module>();
            String command =
                "SELECT ST_MODULE.MODULE_ID, ST_MODULE.NAME, ST_MODULE.DESCRIPTION, ST_MODULE.RETIRED " +
                "FROM ST_MODULE WHERE ST_MODULE.PRODUCT = " + Convert.ToString(this.productID);
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(command);
            int i = 0;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                list.Add(new Module());
                list[i].ID = Convert.ToInt32(row[0]);
                list[i].Name = Convert.ToString(row[1]);
                list[i].Description = Convert.ToString(row[2]);
                list[i].isRetired = Convert.ToBoolean(row[3]);
                i++;
            }
            return list;
        }

        public List<dynamic> GetProdName()
        {
            List<dynamic> list = new List<dynamic>();
            string command = "SELECT ST_PRODUCT.PRODUCT_ID, NAME, ID FROM ST_PRODUCT JOIN ST_PRODUCT_METRICS ON ST_PRODUCT.PRODUCT_ID = ST_PRODUCT_METRICS.PRODUCT_ID WHERE ID =" + this.MetricID;
            REATrackerDB sql = new REATrackerDB();

            DataTable dt = sql.ProcessCommand(command);
            int i = 0;
            foreach(System.Data.DataRow row in dt.Rows)
            {
                list.Add(new System.Dynamic.ExpandoObject());
                list[i].Name = Convert.ToString(row["NAME"]);
                list[i].ID = Convert.ToInt32(row["ID"]);
                list[i].IsDefault = (this.MetricID == list[i].ID);
                i++;
            }

            return list;
        }
        private List<dynamic> GetListOfAllEmployees()
        {
            ///<summary>
            /// returns a list of employees
            ///</summary>
            List<dynamic> list = new List<dynamic>();
            String command =
                "SELECT ST_USERS.FIRST_NAME, ST_USERS.LAST_NAME, ST_USERS.USER_ID " +
                "FROM ST_USERS " +
                "WHERE ST_USERS.ACCOUNT_TYPE != -1 " +
                "ORDER BY ST_USERS.FIRST_NAME ASC, ST_USERS.LAST_NAME ASC";
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(command);
            int i = 0;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                list.Add(new System.Dynamic.ExpandoObject());
                list[i].Name = Convert.ToString(row[0]) + " " + Convert.ToString(row[1]);
                list[i].ID = Convert.ToInt32(row[2]);
                list[i].IsDefault = false;
                i++;
            }
            return list;
        }

        public List<dynamic> GetListofAllNonteamMates()
        {
            ///<summary>
            /// returns a list of non teammates
            ///</summary>
            List<dynamic> List = new List<dynamic>();
            foreach (var person in this.ListOfAllEmployees)
            {
                var isMember = false;
                foreach (var Teammate in this.ListOfTeammates)
                {
                    if (person.Name == Teammate.Name)
                    {
                        isMember = true;
                    }
                }
                if (!isMember)
                {
                    List.Add(person);
                }
            }
            return List;
        }

        public List<dynamic> GetMetrics()
        {
            ///<summary>
            /// returns a list of metrics
            ///</summary>
            List<dynamic> metrics = new List<dynamic>();
            REATrackerDB sql = new REATrackerDB();
            String command = "SELECT * FROM ST_PRODUCT_METRICS WHERE PRODUCT_ID = " + Convert.ToString(this.productID);
            DataTable dt = sql.ProcessCommand(command);
            int i = 0;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                metrics.Add(new System.Dynamic.ExpandoObject());
                metrics[i].ProductID = Convert.ToInt32(row["PRODUCT_ID"]);
                metrics[i].ID = Convert.ToInt32(row["ID"]);
                metrics[i].MajorVersion = Convert.ToInt32(row["MAJOR_VERSION"]);
                metrics[i].MinorVersion = Convert.ToInt32(row["MINOR_VERSION"]);
                metrics[i].CodeCriticalCount = Convert.ToInt32(row["CODE_CRITICAL_COUNT"]);
                metrics[i].CodeHighCount = Convert.ToInt32(row["CODE_HIGH_COUNT"]);
                metrics[i].CodeMetrics = Convert.ToInt32(row["CODE_DEFECT_METRIC"]);
                metrics[i].CodeDefectCount = Convert.ToInt32(row["CODE_DEFECT_COUNT"]);
                metrics[i].ReleaseCriticalCount = Convert.ToInt32(row["RELEASE_CRITICAL_COUNT"]);
                metrics[i].ReleaseHighCount = Convert.ToInt32(row["RELEASE_HIGH_COUNT"]);
                metrics[i].ReleaseMetric = Convert.ToInt32(row["RELEASE_DEFECT_METRIC"]);
                metrics[i].ReleaseDefectCount = Convert.ToInt32(row["RELEASE_DEFECT_COUNT"]);
                metrics[i].Display = Convert.ToBoolean(row["DISPLAY"]);
               
                i++;
            }
            return metrics;
        }

        public void PopulateMetrics()
        {
            ///<summary>
            /// populates metrics
            ///</summary>
            String command = "SELECT DISTINCT CAST(MAJOR AS INT) AS MAJOR, CAST(MINOR AS INT) AS MINOR " +
                                "FROM ST_PRODUCT_RELEASE WHERE PRODUCT_ID = " + this.productID +
                                " ORDER BY CAST(MAJOR AS INT) DESC, CAST(MINOR AS INT) DESC";
            REATrackerDB sql = new REATrackerDB();
            DataTable dtbuilds = sql.ProcessCommand(command);
            this.ListBuilds = new List<String>();
            foreach (System.Data.DataRow row in dtbuilds.Rows)
            {
                this.ListBuilds.Add(Convert.ToString(row["MAJOR"]) + '.' + Convert.ToString(row["MINOR"]));
            }
            this.ListOfMetrics = this.GetMetrics();
        }

        public Boolean GetPopulateMetric(int MetricID)
        {
            ///<summary>
            /// checks if the metrics exists and populates them
            ///</summary>
            String command = "SELECT * FROM ST_PRODUCT_METRICS WHERE ID = " + Convert.ToString(MetricID) + ";";
            REATrackerDB sql = new REATrackerDB();
            DataTable MetricInstance = sql.ProcessCommand(command);
            Boolean exists = false;
            if (MetricInstance.Rows.Count != 0)
            {
                exists = true;
                System.Data.DataRow row = MetricInstance.Rows[0];
                this.MetricID = Convert.ToInt32(row["ID"]);
                this.BuildVersion = Convert.ToString(row["MAJOR_VERSION"]) + "." + Convert.ToString(row["MINOR_VERSION"]);
                this.productID = Convert.ToInt32(row["PRODUCT_ID"]);
                this.CodeCriticalCount = Convert.ToInt32(row["CODE_CRITICAL_COUNT"]);
                this.CodeHighCount = Convert.ToInt32(row["CODE_HIGH_COUNT"]);
                this.CodeMetrics = Convert.ToInt32(row["CODE_DEFECT_METRIC"]);
                this.CodeDefectCount = Convert.ToInt32(row["CODE_DEFECT_COUNT"]);

                this.ReleaseCriticalCount = Convert.ToInt32(row["RELEASE_CRITICAL_COUNT"]);
                this.ReleaseHighCount = Convert.ToInt32(row["RELEASE_HIGH_COUNT"]);
                this.ReleaseMetric = Convert.ToInt32(row["RELEASE_DEFECT_METRIC"]);
                this.ReleaseDefectCount = Convert.ToInt32(row["RELEASE_DEFECT_COUNT"]);
                this.Display = Convert.ToBoolean(row["DISPLAY"]);
            }
            this.PopulateMetrics();
            return exists;
        }

        public void CreateMetrics()
        {
            ///<summary>
            /// adds a new metric
            ///</summary>
            REATrackerDB sql = new REATrackerDB();
            String[] MajorAndMinor = this.BuildVersion.Split('.');
            this.MajorVersion = Convert.ToInt32(MajorAndMinor[0]);
            this.MinorVersion = Convert.ToInt32(MajorAndMinor[1]);
            sql.CreateMetric(
               this.productID
               , this.MajorVersion
               , this.MinorVersion
               , this.CodeCriticalCount
               , this.CodeHighCount
               , this.CodeMetrics
               , this.CodeDefectCount
               , this.ReleaseCriticalCount
               , this.ReleaseHighCount
               , this.ReleaseMetric
               , this.ReleaseDefectCount
               , this.Display
           );
        }

        public void UpdateMetric(int MetricId)
        {
            ///<summary>
            /// updates a new metric
            ///</summary>
            REATrackerDB sql = new REATrackerDB();
            String[] MajorAndMinor = this.BuildVersion.Split('.');
            this.MajorVersion = Convert.ToInt32(MajorAndMinor[0]);
            this.MinorVersion = Convert.ToInt32(MajorAndMinor[1]);
            sql.UpdateMetrics(
                MetricId
                , this.productID
                , this.MajorVersion
                , this.MinorVersion
                , this.CodeCriticalCount
                , this.CodeHighCount
                , this.CodeMetrics
                , this.CodeDefectCount
                , this.ReleaseCriticalCount
                , this.ReleaseHighCount
                , this.ReleaseMetric
                , this.ReleaseDefectCount
                , this.Display
                );
        }

        public void DeleteMetric(int MetricID)
        {
            ///<summary>
            /// removes a metric
            ///</summary>
            REATrackerDB sql = new REATrackerDB();
            sql.DeleteMetrics(MetricID);
        }


        //============================================================================
        //      Post Functions                                                        |
        //============================================================================
        public void post()
        {
            //Post back for home
            this.init();
        }

        public List<dynamic> GetProductTeamDefaulted(int defaultPerson)
        {
            List<dynamic> result = new List<dynamic>();
            int indexOfResult = 0;
            foreach (var person in this.ProductTeam)
            {
                result.Add(new System.Dynamic.ExpandoObject());
                result[indexOfResult].ID = person.ID;
                result[indexOfResult].Name = person.Name;
                result[indexOfResult].IsDefault = (person.ID == defaultPerson);
                indexOfResult++;
            }
            return result;
        }

        public List<dynamic> GetAllEmployeesDefaulted(int defaultPerson)
        {
            List<dynamic> result = new List<dynamic>();
            int indexOfResult = 0;
            foreach (var person in this.ListOfAllEmployees)
            {
                result.Add(new System.Dynamic.ExpandoObject());
                result[indexOfResult].ID = person.ID;
                result[indexOfResult].Name = person.Name;
                result[indexOfResult].IsDefault = (person.ID == defaultPerson);
                indexOfResult++;
            }
            return result;
        }
        //============================================================================
        //      Edit Product Information                                              |
        //============================================================================

        public void saveBasicInfo()
        {
            REATrackerDB sql = new REATrackerDB();
            sql.UpdateProductInformation(this.productID,
                this.CurrentProductName, this.Description,
                this.Vaildator, this.ReleaseCoordinator, this.ReleaseCoordinator2, this.DefaultTester, this.SCCB,
                this.isInternal, this.Retired,
                this.BillingCode, this.DistributionList, this.UseMetrics, this.Owner
                );
        }

        //============================================================================
        //      Edit Product Module                                                   |
        //============================================================================

        public void saveModules()
        {
            //save modules
            REATrackerDB sql = new REATrackerDB();
            foreach (var module in this.ProductModules)
            {
                if (module != null)
                {
                    int moduleID = module.ID;
                    sql.updateModule((int)module.ID, (String)module.Name, (String)module.Description, (bool)module.isRetired);
                }
            }
        }

        public void createModules()
        {
            //Creates a new modules 
            REATrackerDB sql = new REATrackerDB();
            sql.addModule(this.productID, this.NewModuleName, this.NewModuleDescription);

        }
        public class Module
        {
            public int ID { get; set; }
            public String Name { get; set; }
            public String Description { get; set; }
            public Boolean isRetired { get; set; }
            public Module() { }
            public Module(int id, String name, string description, Boolean isretired)
            {
                this.ID = id;
                this.Name = name;
                this.Description = description;
                this.isRetired = isretired;
            }
        }
        //============================================================================
        //      Edit Product Team                                                     |
        //============================================================================

        public void setupProductTeamList()
        {
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.GetUsersByProductID(this.productID);
            this.ListOfTeammates = new List<TeammateRowItem>();
            int i = 0;
            foreach (System.Data.DataRow UserInfo in dt.Rows)
            {
                this.ListOfTeammates.Add(new TeammateRowItem());
                this.ListOfTeammates[i].Name = Convert.ToString(UserInfo[0]) + " " + Convert.ToString(UserInfo[1]);
                this.ListOfTeammates[i].UserID = Convert.ToInt32(UserInfo[2]);
                this.ListOfTeammates[i].RoleID = Convert.ToInt32(UserInfo[3]);
                this.ListOfTeammates[i].RoleName = Convert.ToString(UserInfo[4]);
                this.ListOfTeammates[i].TeamTableID = Convert.ToInt32(UserInfo[5]);
                this.ListOfTeammates[i].Remove = false;
                i++;
            }
        }

        public void AddNewTeammate()
        {
            REATrackerDB sql = new REATrackerDB();
            sql.InsertTeammate(this.productID, this.NewMemmberID, this.NewRoleID);
        }

        public void UpdateTeammate()
        {
            REATrackerDB sql = new REATrackerDB();
            foreach (var item in this.ListOfTeammates)
            {
                sql.UpdateTeammate(item.TeamTableID, item.RoleID);
                if (item.Remove)
                {
                    sql.RemoveTeammate(item.TeamTableID);
                }
            }

        }

        public List<dynamic> RolesSelected(int argsRoleID)
        {
            List<dynamic> List = this.ListOfRoles;
            foreach (var Item in List)
            {
                if (Item.ID == argsRoleID)
                {
                    Item.IsDefault = true;
                }
                else
                {
                    Item.IsDefault = false;
                }
            }
            return List;
        }

        private List<dynamic> RoleDropdown()
        {
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.GetAllRoles();
            List<dynamic> List = new List<dynamic>();
            int i = 0;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                List.Add(new System.Dynamic.ExpandoObject());
                List[i].ID = Convert.ToInt32(row[0]);
                List[i].Name = Convert.ToString(row[1]);
                i++;
            }
            return List;
        }

        public class TeammateRowItem
        {
            public String Name { get; set; }
            public int UserID { get; set; }
            public int RoleID { get; set; }
            public String RoleName { get; set; }
            public int TeamTableID { get; set; }
            public bool Remove { get; set; }

            public TeammateRowItem() { }

            public TeammateRowItem(String argsName, int argsUserID, int argsRoleID, String argsRoleName, int argsTeamTableID)
            {
                this.Name = argsName;
                this.UserID = argsUserID;
                this.RoleID = argsRoleID;
                this.RoleName = argsRoleName;
                this.TeamTableID = argsTeamTableID;
                this.Remove = false;
            }
        }

        //============================================================================
        //      Edit Product Team                                                     |
        //============================================================================

        public int CreateNewProduct()
        {
            REATrackerDB sql = new REATrackerDB();
            return sql.CreateNewProduct(this.CurrentProductName, this.Description, this.Vaildator, this.ReleaseCoordinator,
                this.ReleaseCoordinator2, this.DefaultTester, this.SCCB, this.isInternal, this.BillingCode, this.DistributionList, this.Owner);
        }

        //============================================================================
        //      Utility                                                               |
        //============================================================================
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
    }


}