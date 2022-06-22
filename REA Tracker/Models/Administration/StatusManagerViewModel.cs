using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{

    public class StatusManagerViewModel
    {
        public List<Tuple<int, string>> RoleNames { get; set; }
        public List<Tuple<int, string, string, int>> StatusNames { get; set; }
        public List<int[,]> StatusTables { get; set; }
        public List<String[]> DisplayEdit { get; set; }

        public int RoleID { get; set; }

        public int FromId { get; set; }
        public int ToId { get; set; }
        public int isException { get; set; }
        public int isDenied { get; set; }

        public StatusManagerViewModel()
        {
            this.RoleID = -1;
            this.init();
        }

        public StatusManagerViewModel(int argsId)
        {
            this.RoleID = argsId;
            this.init();
        }

        public void init()
        {
            ///<summary>
            /// mainfunction to call to populate the roles and status
            ///</summary>
            this.populateRole();
            this.populateStatus();
            if (RoleID != -1)
            {
                this.populateStatusTableByRole(this.RoleID);
            }
            else
            {
                this.populateStatusTables();
            }
        }

        private void populateRole()
        {
            ///<summary>
            /// populates the roles
            ///</summary>
            this.RoleNames = new List<Tuple<int, string>>();
            REATrackerDB sql = new REATrackerDB();
            String Command = "SELECT ROLE_ID, NAME FROM ST_ROLES ORDER BY NAME ASC";
            DataTable dt = sql.ProcessCommand(Command);
            foreach (DataRow row in dt.Rows)
            {
                this.RoleNames.Add(
                    new Tuple<int, string>(Convert.ToInt16(row[0]), Convert.ToString(row[1]))
                    );
            }
        }

        private void populateStatus()
        {
            ///<summary>
            /// populates the statuses
            ///</summary>
            this.StatusNames = new List<Tuple<int, string, string, int>>();
            REATrackerDB sql = new REATrackerDB();
            String Command = "SELECT STATUS_ID, NAME, DESCRIPTION, STATUS_ORDER FROM REA_STATUS ORDER BY STATUS_ORDER ASC";   //SEK SCR84313
            DataTable dt = sql.ProcessCommand(Command);
            foreach (DataRow row in dt.Rows)
            {
                this.StatusNames.Add(
                    new Tuple<int, string, string, int>(Convert.ToInt16(row[0]), Convert.ToString(row[1]), Convert.ToString(row[2]), Convert.ToInt16(row[3]))
                    );
            }
        }

        private void populateStatusTables()
        {
            ///<summary>
            /// populates the table of statuses
            ///</summary>
            if (RoleNames != null && StatusNames != null)
            {
                //Get Data from Database
                REATrackerDB sql = new REATrackerDB();
                String Command = "SELECT SC_ID, GROUP_ID, FROM_STATUS, TO_STATUS, EXCEPTION_CHANGE FROM REA_STATUS_CHANGE";
                DataTable dt = sql.ProcessCommand(Command);
                //Match the pairing from from status to to status
                int size = this.StatusNames.Count();
                this.StatusTables = new List<int[,]>();
                foreach (Tuple<int, string> Role in this.RoleNames)
                {
                    int[,] CurrentRoleTable = new int[size, size];
                    //Set the results to the row of data where the row with group id equal to the role
                    var results = from DataRow row in dt.Rows where (int)row["GROUP_ID"] == Role.Item1 select row;
                    for (int i = 0; i < results.Count(); i++)
                    {
                        DataRow rowAtindexi = results.ElementAt(i);
                        int valueToPlaceInTable = 1;
                        if (Convert.ToBoolean(rowAtindexi["EXCEPTION_CHANGE"]))
                        {
                            valueToPlaceInTable++;
                        }
                        // 0 - not allowed
                        // 1 - false, no exception change
                        // 2 - true, execption change
                        CurrentRoleTable[getStatusOrderByStatusId(Convert.ToInt32(rowAtindexi["FROM_STATUS"])) - 1, getStatusOrderByStatusId(Convert.ToInt32(rowAtindexi["TO_STATUS"])) - 1]
                            = valueToPlaceInTable;
                    }
                    this.StatusTables.Add(CurrentRoleTable);
                }
            }
        }

        private int getStatusOrderByStatusId(int StatusId)
        {
            int statusOrder = 1;
            foreach (Tuple<int, string, string, int> Status in this.StatusNames)
            {
                if (Status.Item1 == StatusId)
                    break;
                statusOrder++;
            }
            return statusOrder;
        }

        private void populateStatusTableByRole(int roleID)
        {
            ///<summary>
            /// populates the status table by each role
            ///</summary>
            ///<param name="roleID">
            /// the role id that the status table will populate by
            ///</param>
            if (RoleNames != null && StatusNames != null)
            {
                //Get Data from Database
                REATrackerDB sql = new REATrackerDB();
                String Command =
                    "SELECT SC_ID, FROMSTAT.NAME , TOSTAT.NAME, EXCEPTION_CHANGE, DENY_RIGHT " +
                    "FROM REA_STATUS_CHANGE " +
                    "INNER JOIN REA_STATUS as FROMSTAT ON FROM_STATUS = FROMSTAT.STATUS_ID " +
                    "INNER JOIN REA_STATUS AS TOSTAT ON TO_STATUS  = TOSTAT.STATUS_ID " +
                    "WHERE GROUP_ID = " + this.RoleID;
                DataTable dt = sql.ProcessCommand(Command);
                this.DisplayEdit = new List<String[]>();

                //int[,] CurrentRoleTable = new int[size, size];
                //Set the results to the row of data where the row with group id equal to the role
                foreach (DataRow row in dt.Rows)
                {
                    String[] CurrentRow = new String[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        CurrentRow[i] = Convert.ToString(row[i]);
                    }
                    this.DisplayEdit.Add(CurrentRow);
                }
            }
        }

        public Boolean createNewChange()
        {
            ///<summary>
            /// Check if the change is already in the the database
            /// Returns true if already exists
            /// if it does alert 
            ///</summary>

            String Command =
                "SELECT COUNT(*) FROM REA_STATUS_CHANGE WHERE REA_STATUS_CHANGE.FROM_STATUS = "
                + this.FromId + " AND REA_STATUS_CHANGE.TO_STATUS = " + this.ToId + " AND REA_STATUS_CHANGE.GROUP_ID =" + this.RoleID;
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(Command);
            Boolean value = false;
            if (dt.Rows.Count != 0 && Convert.ToInt32(dt.Rows[0][0]) >= 1)
            {
                value = true;
            }
            else
            {
                String CreateCommand =
                    "INSERT INTO REA_STATUS_CHANGE " +
                    "(ROW_VER,GROUP_ID, FROM_STATUS, TO_STATUS,EXCEPTION_CHANGE,DENY_RIGHT, PRODUCT) " +
                    " VALUES" +
                    "(0, " + this.RoleID + "," + this.FromId + "," + this.ToId + "," + this.isException + "," + this.isDenied + ",0)";
                sql.ProcessCommand(CreateCommand);
            }
            return value;
        }

        public void DeleteChange(int id)
        {
            ///<summary>
            /// Deletes the status change by id
            ///</summary>
            ///<param name="id">
            /// The id the the instance to delete
            ///</param>
            String Command = "DELETE FROM REA_STATUS_CHANGE WHERE SC_ID = " + id;
            REATrackerDB sql = new REATrackerDB();
            sql.ProcessCommand(Command);
        }

        public string GetStatusDescription(string StatusName)
        {
            string StatusDescription = "";
            foreach (Tuple<int, string, string, int> Status in this.StatusNames)
            {
                if (Status.Item2 == StatusName)
                {
                    StatusDescription = Status.Item3;
                    break;
                }
            }
            return StatusDescription;
        }
    }

    public class StatusChangeEdit
    {

        public int StatusChangeId { get; set; }
        public int SelectFrom { get; set; }
        public int SelectTo { get; set; }
        public int isExeception { get; set; }
        public int isDenied { get; set; }

        public String RoleName { get; set; }
        public int GroupID { get; set; }
        //Select Lists
        public List<Tuple<int, string>> StatusNames { get; set; }

        public StatusChangeEdit() { }
        public StatusChangeEdit(int StatusChangeId)
        {
            this.StatusChangeId = StatusChangeId;
            this.init();
        }
        private void init()
        {
            ///<summary>
            /// the mainfunction called to populate the status change items
            ///</summary>

            this.populateStatus();
            this.populateChange();
        }
        private void populateStatus()
        {
            ///<summary>
            /// populating the statuses
            ///</summary>
            this.StatusNames = new List<Tuple<int, string>>();
            REATrackerDB sql = new REATrackerDB();
            String Command = "SELECT STATUS_ID, NAME FROM REA_STATUS ORDER BY STATUS_ID ASC";
            DataTable dt = sql.ProcessCommand(Command);
            foreach (DataRow row in dt.Rows)
            {
                this.StatusNames.Add(
                    new Tuple<int, string>(Convert.ToInt16(row[0]), Convert.ToString(row[1]))
                    );
            }
        }
        private void populateChange()
        {
            ///<summary>
            /// populates the status change
            ///</summary>
            String command = "SELECT " +
               " SC_ID, FROM_STATUS, TO_STATUS , DENY_RIGHT ,EXCEPTION_CHANGE, ST_ROLES.NAME, GROUP_ID " +
               " FROM REA_STATUS_CHANGE " +
               " INNER JOIN ST_ROLES ON REA_STATUS_CHANGE.GROUP_ID = ST_ROLES.ROLE_ID " +
               " WHERE SC_ID = " + this.StatusChangeId;
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(command);
            if (dt.Rows.Count != 0)
            {
                this.SelectFrom = Convert.ToInt32(dt.Rows[0]["FROM_STATUS"]);
                this.SelectTo = Convert.ToInt32(dt.Rows[0]["TO_STATUS"]);
                this.isDenied = Convert.ToInt32(dt.Rows[0]["DENY_RIGHT"]);
                this.isExeception = Convert.ToInt32(dt.Rows[0]["EXCEPTION_CHANGE"]);
                this.RoleName = Convert.ToString(dt.Rows[0]["NAME"]);
                this.GroupID = Convert.ToInt32(dt.Rows[0]["GROUP_ID"]);
            }
        }
        public bool processChange()
        {
            ///<summary>
            /// updates the status change table
            ///</summary>
            String Command =
                "SELECT COUNT(*) FROM REA_STATUS_CHANGE WHERE REA_STATUS_CHANGE.FROM_STATUS = "
                + this.SelectFrom + " AND REA_STATUS_CHANGE.TO_STATUS = " + this.SelectTo + " AND REA_STATUS_CHANGE.GROUP_ID =" + this.GroupID;
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(Command);
            Boolean value = false;
            if (dt.Rows.Count != 0 && Convert.ToInt32(dt.Rows[0][0]) >= 1)
            {
                value = true;
            }
            else
            {
                String UpdateCommand = "UPDATE REA_STATUS_CHANGE SET FROM_STATUS=" + SelectFrom +
                 " ,TO_STATUS = " + SelectTo +
                 " ,EXCEPTION_CHANGE = " + isExeception +
                 " ,DENY_RIGHT = " + isDenied +
                 " WHERE SC_ID = " + this.StatusChangeId;
                sql.ProcessCommand(UpdateCommand);
            }
            return value;
        }
    }

    public class EditStatusListItemViewModel
    {
        public int id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public int SortOrder { get; set; }

        public EditStatusListItemViewModel() { }
        public EditStatusListItemViewModel(int id)
        {
            this.id = id;
        }
        public Boolean init()
        {
            ///<summary>
            /// populates the field based on the id passed into the model
            ///</summary>
            Boolean value = false;
            string cmd = "SELECT NAME, DESCRIPTION, STATUS_ORDER FROM REA_STATUS WHERE STATUS_ID=" + Convert.ToString(this.id);
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(cmd);
            if (dt.Rows.Count > 0)
            {
                this.Name = Convert.ToString(dt.Rows[0][0]);
                this.SortOrder = Convert.ToInt32(dt.Rows[0][2]);
                this.Description = dt.Rows[0][1] == DBNull.Value ? "" : Convert.ToString(dt.Rows[0][1]);
                value = true;
            }
            return value;
        }
        public String update()
        {
            ///<summary>
            /// updates the instance in the database
            ///</summary>
            bool send = true;
            String message = "";
            string cmd = "UPDATE REA_STATUS SET NAME ='" + Convert.ToString(this.Name) + "', DESCRIPTION = " + ((this.Description == "" || this.Description == null) ? ("NULL") : ("'" + Convert.ToString(this.Description) + "'")) + ", STATUS_ORDER = " + Convert.ToString(this.SortOrder) + " WHERE STATUS_ID = " + Convert.ToString(this.id);
            REATrackerDB sql = new REATrackerDB();
            if (this.Name == null)
            {
                message += "Name cannot be empty.\\n";
                send = false;
            }
            if (this.NameAlreadyExists(this.Name, this.id))
            {
                message += "Name " + this.Name + " already exists.\\n";
                send = false;
            }
            if (this.SortOrder < 0)
            {
                message += "Sort Order cannot be less then Zero.\\n";
                send = false;
            }
            if (send)
            {
                sql.ProcessCommand(cmd);
            }
            return message;
        }
        private bool NameAlreadyExists(String name, int id)
        {
            ///<summary>
            /// Checks to see if the name already exists
            ///</summary>
            ///<param name="name">
            /// the name that we are checking
            ///</param>
            ///<param name="id">
            /// the id of the instance we are updating
            ///</param>
            Boolean result = false;
            List<String> ListOfStatuses = new List<String>();
            String command = "SELECT NAME, STATUS_ID FROM REA_STATUS";
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(command);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                ListOfStatuses.Add(Convert.ToString(row[0]));
            }
            if (Convert.ToInt32(dt.Rows[ListOfStatuses.IndexOf(name)][1]) != id)
            {
                result = true;
            }
            return result;
        }
    }

}