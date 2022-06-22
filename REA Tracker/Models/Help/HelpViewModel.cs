using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{

    public class HelpViewModel
    {
        public bool Sucessful { get; set; }

        public string Name { get; set; }
        public string Subject { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }

        public List<dynamic> PriorityList { get; set; }
        public List<dynamic> IssueList { get; set; }
        public List<dynamic> ReleaseTypeList { get; set; }
        public List<dynamic> ProjectList { get; set; }
        public List<dynamic> ProductUsage { get; set; }
        public REATrackerDB.SupportType SupportType { get; set; }
        private REATrackerDB _db;

        public HelpViewModel()
        {
            this.Sucessful = false;
            string PriorityString = "SELECT ID, NAME, DESCRIPTION FROM ST_priority";
            string IssueString = "SELECT ISSUE_ID AS ID, NAME, DESCRIPTION FROM ST_ISSUE_TYPE";
            string ReleaseTypeString = "SELECT * FROM ST_PRODUCT_RELEASE_TYPE";

            this.PriorityList = new List<dynamic>();
            this.ReleaseTypeList = new List<dynamic>();
            this.IssueList = new List<dynamic>();
            this.ProjectList = new List<dynamic>();

            _db = new REATrackerDB();

            this.populate(PriorityString, this.PriorityList);
            this.populate(IssueString, this.IssueList);
            this.populate(ReleaseTypeString, this.ReleaseTypeList);

            using (DataTable dt = _db.GetProjects())
            {
                List<dynamic> list = new List<dynamic>();
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].ID = row["ID"].ToString();
                    list[i].Code = row["Code"].ToString();
                    list[i].Name = row["Name"].ToString();
                    list[i].Description = row["Description"].ToString();
                    list[i].Active = GenericDB.CheckValue(row["Active"], false);
                    list[i].Support = GenericDB.CheckValue(row["SupportMode"], 0);
                    var MaxValue = Enum.GetValues(typeof(REATrackerDB.SupportType)).Length;
                    if (list[i].Support == (int)REATrackerDB.SupportType.RandD)
                    {
                        list[i].SupportName = "R&D";
                    }
                    else if (list[i].Support > MaxValue)
                    {
                        list[i].SupportName = "Unknown";
                    }
                    else
                    {
                        list[i].SupportName = @Enum.GetName(typeof(REATrackerDB.SupportType), list[i].Support);
                    }
                    string tmp = row["CreationDate"].ToString();
                    list[i].CreateDate = DateTime.Parse(tmp).ToString("yyyy-MM-dd");
                    list[i].StartDate = ((row["StartDate"] == null) || (row["StartDate"].ToString().Length == 0)) ? "" : DateTime.Parse(row["StartDate"].ToString()).ToString("yyyy-MM-dd");
                    list[i].EndDate = ((row["EndDate"] == null) || (row["EndDate"].ToString().Length == 0)) ? "" : DateTime.Parse(row["EndDate"].ToString()).ToString("yyyy-MM-dd");
                    list[i].OwnerID = GenericDB.CheckValue(row["OwnerID"], -1);
                    list[i].OwnerName = GenericDB.CheckValue(row["OwnerName"], "");
                    i++;
                }
                ProjectList = list;
            }
            this.ProductUsage = new List<dynamic>();
            String Command = $@"
            SELECT 
	            ST_PRODUCT.PRODUCT_ID, ST_PRODUCT.NAME as 'Product Name', ST_PRODUCT.BILLING_CODE, 
	            PROJECTS.ID, PROJECTS.Name as 'Project Name'
            FROM 
	            ST_PRODUCT 
            INNER JOIN PROJECTS ON PROJECTS.Code = ST_PRODUCT.BILLING_CODE
            WHERE (ISNUMERIC(PROJECTS.Code) = 1)
            ORDER BY ST_PRODUCT.NAME";
            DataTable MemeberDT = _db.ProcessCommand(Command);
            for (int i = 0; i < MemeberDT.Rows.Count; i++)
            {
                ProductUsage.Add(new System.Dynamic.ExpandoObject());
                ProductUsage[i].ProductName = Convert.ToString(MemeberDT.Rows[i]["Product Name"]);
                ProductUsage[i].BillingCode = Convert.ToString(MemeberDT.Rows[i]["BILLING_CODE"]);
                ProductUsage[i].ProductID = Convert.ToString(MemeberDT.Rows[i]["PRODUCT_ID"]);
            }
        }

        public void populate(string command, List<dynamic> list)
        {
            using (DataTable dt = _db.ProcessCommand(command))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].ID = Convert.ToString(dt.Rows[i]["ID"]);
                    list[i].Name = dt.Rows[i]["Name"];
                    list[i].Description = dt.Rows[i]["Description"];
                }
            }
        }

        public void send()
        {
            //EmailContactAdmin Email =
                //new EmailContactAdmin(this.Name, this.Email, this.Subject, this.Message);
            //this.Sucessful = Email.Send();
        }
    }


}