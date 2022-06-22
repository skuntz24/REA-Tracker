using System;
using System.Data;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{

    public class PermissionMangerViewModel
    {
        public Boolean found { get; set; }
        public int EditID { get; set; }
        public int ColumnCount { get; set; }
        public int RowCount { get; set; }
        public string[,] Table { get; set; }
        public Tuple<int, string, bool>[] SelectList { get; set; }
        // Single instance variables
        public string[] ColumnDisplayNames { get; set; }
        public string[] ColumnCommandNames { get; set; }
        public Int16[] Permissions { get; set; }
        public string name { get; set; }

        public PermissionMangerViewModel()
        {
            this.EditID = -1;
            string Command = "SELECT " +
                "EDIT_ID,ST_ROLES.NAME AS 'SCR Field Name' " +
                ",[EDIT_PRODUCT] as 'Product' " +
                ",[EDIT_VERSION] as 'Version'" +
                ",[EDIT_MODULE]as 'Module'" +
                ",[EDIT_TITLE]as 'Title'" +
                ",[EDIT_PRIORITY]as 'Priority'" +
                ",[EDIT_ISSUE_TYPE]as 'Issue Type'" +
                ",[EDIT_STATUS]as 'Staus'" +
                ",[EDIT_DOC_CHANGE]as 'Document Change'" +
                ",[EDIT_DB_CHANGE]as 'Database Change'" +
                ",[EDIT_REQ_DOC]as 'Requirements Change'" +
                ",[EDIT_PLANNED_HOURS]as 'Planned Hours'" +
                ",[EDIT_ACTUAL_HOURS]as 'Actual Hours'" +
                ",[EDIT_SUBMITTED_BY]as 'Submitted By'" +
                ",[EDIT_ASSIGNED_TO]as 'Assigned To'" +
                ",[EDIT_RESOLVED_BY]as 'Resolved By'" +
                ",[EDIT_CLOSED_BY]as 'Closed By'" +
                ",[EDIT_QA_TEST_CASE]as 'QA Test Case'" +
                ",[EDIT_NOTES_RES]as 'Notes'" +
                ",[EDIT_ATTACH_FILES]as 'Attached Files'" +
                ",[EDIT_PLANNED_RELEASE]as 'Planned Release'" +
                ",[EDIT_ACTUAL_RELEASE]as 'Actual Release'" +
                ",[EDIT_CUSTOMER_BUG]as 'Customer Bug'" +
                ",[EDIT_CUSTOMER_NAME]as 'Customer Name'" +
                ",[EDIT_CLOSED_REASON]as 'Closed Reason'" +
                ",[EDIT_PLANNED_HOURS_TEST]as 'Planned Hours Test'" +
                ",[EDIT_ACTUAL_HOURS_TEST]as 'Actual Hours Test'" +
                ",[EDIT_SYTELINE_REF]as 'Syteline Refference'" +
                ",[EDIT_PLANNED_HOURS_SPEC]as 'Planned Hours Specify'" +
                ",[EDIT_ACTUAL_HOURS_SPEC]as 'Actual Hours Specify'" +
                "FROM ST_SCR_EDIT " +
                "INNER JOIN ST_ROLES ON ST_ROLES.ROLE_ID = ST_SCR_EDIT.GROUP_ID;";

            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(Command);
            ColumnCount = dt.Columns.Count;
            this.Table = new string[ColumnCount, dt.Rows.Count + 1];
            this.RowCount = dt.Rows.Count;

            this.populateSelectList(-1);

            if (dt.Rows.Count > 0)
            {
                //Column Generation place the column name on left hand side
                for (int i = 1; i < this.ColumnCount; i++)
                {//Turning each Column value into the name for the row
                    string value = Convert.ToString(dt.Columns[i].ColumnName);
                    this.Table[i, 0] = value;
                }
                //Body Generation
                for (int i = 0; i < this.ColumnCount; i++)
                {
                    for (int j = 0; j < this.RowCount; j++)
                    {
                        this.Table[i, j + 1] = Convert.ToString(dt.Rows[j][i]);
                    }
                }
            }

        }

        public PermissionMangerViewModel(int argsEditID)
        {
            this.EditID = argsEditID;
            String Command = "SELECT " +
                "ST_ROLES.NAME AS 'SCR Field Name' " +
                ",[EDIT_PRODUCT] as 'Product' " +
                ",[EDIT_VERSION] as 'Version'" +
                ",[EDIT_MODULE]as 'Module'" +
                ",[EDIT_TITLE]as 'Title'" +
                ",[EDIT_PRIORITY]as 'Priority'" +
                ",[EDIT_ISSUE_TYPE]as 'Issue Type'" +
                ",[EDIT_STATUS]as 'Staus'" +
                ",[EDIT_DOC_CHANGE]as 'Document Change'" +
                ",[EDIT_DB_CHANGE]as 'Database Change'" +
                ",[EDIT_REQ_DOC]as 'Requirements Change'" +
                ",[EDIT_PLANNED_HOURS]as 'Planned Hours'" +
                ",[EDIT_ACTUAL_HOURS]as 'Actual Hours'" +
                ",[EDIT_SUBMITTED_BY]as 'Submitted By'" +
                ",[EDIT_ASSIGNED_TO]as 'Assigned To'" +
                ",[EDIT_RESOLVED_BY]as 'Resolved By'" +
                ",[EDIT_CLOSED_BY]as 'Closed By'" +
                ",[EDIT_QA_TEST_CASE]as 'QA Test Case'" +
                ",[EDIT_NOTES_RES]as 'Notes'" +
                ",[EDIT_ATTACH_FILES]as 'Attached Files'" +
                ",[EDIT_PLANNED_RELEASE]as 'Planned Release'" +
                ",[EDIT_ACTUAL_RELEASE]as 'Actual Release'" +
                ",[EDIT_CUSTOMER_BUG]as 'Customer Bug'" +
                ",[EDIT_CUSTOMER_NAME]as 'Customer Name'" +
                ",[EDIT_CLOSED_REASON]as 'Closed Reason'" +
                ",[EDIT_PLANNED_HOURS_TEST]as 'Planned Hours Test'" +
                ",[EDIT_ACTUAL_HOURS_TEST]as 'Actual Hours Test'" +
                ",[EDIT_SYTELINE_REF]as 'Syteline Refference'" +
                ",[EDIT_PLANNED_HOURS_SPEC]as 'Planned Hours Specify'" +
                ",[EDIT_ACTUAL_HOURS_SPEC]as 'Actual Hours Specify'" +
                "FROM ST_SCR_EDIT " +
                "INNER JOIN ST_ROLES ON ST_ROLES.ROLE_ID = ST_SCR_EDIT.GROUP_ID WHERE EDIT_ID = " + this.EditID;

            DataTable dt = new REATrackerDB().ProcessCommand(Command);
            DataRow rowInfo;
            this.populateSelectList(this.EditID);
            if (dt.Rows.Count > 0)
            {
                int FieldNameIndex = 0;
                int extraColumns = 1;

                this.RowCount = dt.Rows.Count;
                ColumnCount = dt.Columns.Count;
                rowInfo = dt.Rows[0];
                this.ColumnDisplayNames = new String[ColumnCount - extraColumns];
                this.Permissions = new Int16[ColumnCount - extraColumns];

                this.name = Convert.ToString(rowInfo[FieldNameIndex]);

                for (int i = extraColumns; i < this.ColumnCount; i++)
                {

                    string value = Convert.ToString(dt.Columns[i].ColumnName);
                    this.ColumnDisplayNames[i - extraColumns] = value;
                    this.Permissions[i - extraColumns] = Convert.ToInt16(rowInfo[i]);

                }

            }

        }

        public void populateSelectList(int SelectedIndex)
        {//-1 for no selected
         //Getting Data for the select List
            string Command = "SELECT " +
               "EDIT_ID,ST_ROLES.NAME AS 'SCR Field Name' " +
               "FROM ST_SCR_EDIT " +
               "INNER JOIN ST_ROLES ON ST_ROLES.ROLE_ID = ST_SCR_EDIT.GROUP_ID;";
            DataTable dt = new REATrackerDB().ProcessCommand(Command);
            int RoleCount = dt.Rows.Count;
            this.SelectList = new Tuple<int, string, bool>[RoleCount];
            //Populating Select List
            for (int i = 0; i < RoleCount; i++)
            {
                DataRow rd = dt.Rows[i];
                int EditID = Convert.ToInt32(rd[0]);
                string Label = Convert.ToString(rd[1]);
                bool Selected = false;
                if (SelectedIndex != -1 && EditID == SelectedIndex)
                {
                    Selected = true;
                }
                this.SelectList[i] =
                    new Tuple<int, string, bool>(EditID, Label, Selected);
            }
        }

        public void update()
        {
            String Command = "UPDATE ST_SCR_EDIT SET" +
                "[EDIT_PRODUCT] = " + Permissions[0] +
                ",[EDIT_VERSION] =" + Permissions[1] +
                ",[EDIT_MODULE] =" + Permissions[2] +
                ",[EDIT_TITLE] =" + Permissions[3] +
                ",[EDIT_PRIORITY] =" + Permissions[4] +
                ",[EDIT_ISSUE_TYPE] =" + Permissions[5] +
                ",[EDIT_STATUS] =" + Permissions[6] +
                ",[EDIT_DOC_CHANGE] =" + Permissions[7] +
                ",[EDIT_DB_CHANGE] =" + Permissions[8] +
                ",[EDIT_REQ_DOC] =" + Permissions[9] +
                ",[EDIT_PLANNED_HOURS] =" + Permissions[10] +
                ",[EDIT_ACTUAL_HOURS] =" + Permissions[11] +
                ",[EDIT_SUBMITTED_BY] =" + Permissions[12] +
                ",[EDIT_ASSIGNED_TO] =" + Permissions[13] +
                ",[EDIT_RESOLVED_BY] =" + Permissions[14] +
                ",[EDIT_CLOSED_BY] =" + Permissions[15] +
                ",[EDIT_QA_TEST_CASE] =" + Permissions[16] +
                ",[EDIT_NOTES_RES] =" + Permissions[17] +
                ",[EDIT_ATTACH_FILES] =" + Permissions[18] +
                ",[EDIT_PLANNED_RELEASE] =" + Permissions[19] +
                ",[EDIT_ACTUAL_RELEASE] =" + Permissions[20] +
                ",[EDIT_CUSTOMER_BUG] =" + Permissions[21] +
                ",[EDIT_CUSTOMER_NAME] =" + Permissions[22] +
                ",[EDIT_CLOSED_REASON] =" + Permissions[23] +
                ",[EDIT_PLANNED_HOURS_TEST] =" + Permissions[24] +
                ",[EDIT_ACTUAL_HOURS_TEST] =" + Permissions[25] +
                ",[EDIT_SYTELINE_REF] =" + Permissions[26] +
                ",[EDIT_PLANNED_HOURS_SPEC] =" + Permissions[27] +
                ",[EDIT_ACTUAL_HOURS_SPEC] = " + Permissions[28] +
                " WHERE EDIT_ID = " + this.EditID;
            new REATrackerDB().ProcessCommand(Command);
        }
    }


}