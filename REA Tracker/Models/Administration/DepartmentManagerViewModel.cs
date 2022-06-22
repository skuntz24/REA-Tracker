using System;
using System.Data;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{

    public class DepartmentManagerViewModel
    {

        public int depID { get; set; }
        //Displays
        public Tuple<int, string>[] ListOfHeads { get; set; }

        public Tuple<int, string>[] ListOfCompanies { get; set; }

        public string[,] Table { get; set; }

        public int RowCount { get; set; }

        public int ColCount { get; set; }

        public string[] TableHead { get; set; }

        //Inputs
        public string Name { get; set; }

        public string CodeNumber { get; set; }

        public int SelectedHead { get; set; }

        public int SelectedCompany { get; set; }

        public DepartmentManagerViewModel(int id)
        {
            this.depID = id;
            this.populate();
        }

        public DepartmentManagerViewModel()
        {
            this.depID = -1;
            this.populate();
        }

        private void populate()
        {
            this.populateHeads();
            this.populateCompanies();
            if (this.depID == -1)
            {
                this.populateTable();
            }
            else
            {
                this.populateInstance();
            }

        }

        private void populateTable()
        {

            string Command = "SELECT "
            + "	ST_DEPARTMENT.DEPARTMENT_ID AS DEPARTMENT_ID,"
            + "	ST_DEPARTMENT.NAME AS 'Name',"
            + "	ST_DEPARTMENT.CODE AS 'Code',"
            + "	ST_COMPANY_LIST.NAME AS 'Company Name',"
            + "	(ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME) AS 'Department Head'"
            + "FROM ST_DEPARTMENT "
            + "	LEFT JOIN ST_COMPANY_LIST ON ST_DEPARTMENT.COMPANY_ID = ST_COMPANY_LIST.COMPANY_ID "
            + "	LEFT JOIN ST_USERS ON ST_DEPARTMENT.DEPARTMENT_HEAD_ID = ST_USERS.USER_ID "
            + "ORDER BY ST_DEPARTMENT.COMPANY_ID ASC;";

            DataTable dt = new REATrackerDB().ProcessCommand(Command);

            this.RowCount = dt.Rows.Count;

            this.ColCount = dt.Columns.Count;

            this.TableHead = new string[ColCount];

            for (int i = 0; i < this.ColCount; i++)
            {
                string value = Convert.ToString(dt.Columns[i].ColumnName);
                if (value == "DEPARTMENT_ID")
                {
                    value = "Edit";
                }
                this.TableHead[i] = value;
            }

            this.Table = new string[RowCount, ColCount];

            for (int i = 0; i < this.RowCount; i++)
            {
                DataRow dr = dt.Rows[i];
                for (int j = 0; j < this.ColCount; j++)
                {
                    string value = Convert.ToString(dr[j]);
                    if (j == 0)
                    {
                        value =
                        "<a href='/admin/DepartmentManager/" + value + "'>Edit</a>";
                    }
                    this.Table[i, j] = value;
                }
            }

        }

        private void populateInstance()
        {
            string command = "SELECT NAME, CODE, DEPARTMENT_HEAD_ID, COMPANY_ID FROM ST_DEPARTMENT WHERE DEPARTMENT_ID = " + this.depID;
            DataTable dt = new REATrackerDB().ProcessCommand(command);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                this.Name = Convert.ToString(dr["NAME"]);
                this.CodeNumber = Convert.ToString(dr["CODE"]);
                this.SelectedHead = Convert.ToInt32(dr["DEPARTMENT_HEAD_ID"]);
                this.SelectedCompany = Convert.ToInt32(dr["COMPANY_ID"]);
            }

        }

        private void populateHeads()
        {
            DataTable dt = new REATrackerDB().GetAllManagers();
            this.ListOfHeads = new Tuple<int, string>[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int idValue = Convert.ToInt32(dt.Rows[i]["USER_ID"]);
                string stringValue = Convert.ToString(dt.Rows[i]["First_Name"]) +
                                    " " + Convert.ToString(dt.Rows[i]["Last_Name"]);
                this.ListOfHeads[i] = new Tuple<int, string>(idValue, stringValue);
            }

        }

        private void populateCompanies()
        {
            string command = "SELECT ST_COMPANY_LIST.COMPANY_ID, ST_COMPANY_LIST.NAME, ST_COMPANY_LIST.DESCRIPTION, ST_COMPANY_LIST.COMPANY_CODE" +
                " FROM ST_COMPANY_LIST";
            DataTable dt = new REATrackerDB().ProcessCommand(command);
            this.ListOfCompanies = new Tuple<int, string>[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int idValue = Convert.ToInt32(dt.Rows[i]["COMPANY_ID"]);
                string stringValue = Convert.ToString(dt.Rows[i]["COMPANY_CODE"]) +
                                    " - " + Convert.ToString(dt.Rows[i]["NAME"])
                                    + " | " + Convert.ToString(dt.Rows[i]["DESCRIPTION"]);
                this.ListOfCompanies[i] = new Tuple<int, string>(idValue, stringValue);
            }
        }

        public void Update()
        {

            string command = "UPDATE ST_DEPARTMENT "
            + "SET NAME= '" + this.Name.Replace("'", "''")
            + "',CODE= " + this.CodeNumber
            + ",COMPANY_ID= " + this.SelectedCompany
            + ",DEPARTMENT_HEAD_ID= " + this.SelectedHead
            + "WHERE ( (DEPARTMENT_ID= " + this.depID + "))";
            new REATrackerDB().ProcessCommand(command);

        }

        public void Create()
        {

            string command = "INSERT INTO ST_DEPARTMENT (ROW_VER,NAME,CODE,COMPANY_ID,DEPARTMENT_HEAD_ID)" +
                    "VALUES(1,'" + this.Name.Replace("'", "''") + "'," + this.CodeNumber + "," + this.SelectedCompany + "," + this.SelectedHead + ");";
            new REATrackerDB().ProcessCommand(command);
        }

    }

}