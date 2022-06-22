using System;
using System.Collections.Generic;
using System.Data;
using QVICommonIntranet.Database;
using System.ComponentModel.DataAnnotations;

namespace REA_Tracker.Models
{
    public class ProjectManagerViewModel
    {
        public int ProjectID { get; set; }
        public bool IsValidID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public int Support { get; set; }
        public string CreateDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int OwnerID { get; set; }
        public string OwnerName { get; set; }
        public List<dynamic> AllProjects { get; set; }
        public List<dynamic> ProductUsage { get; set; }
        public REATrackerDB.SupportType SupportType { get; set; }

        public ProjectManagerViewModel(int id)
        {
            this.ProjectID = id;

            REATrackerDB db = new REATrackerDB();
            REATrackerDB.ProjectInfo prj = db.GetProjectInfo(id);
            if (prj.IsValidID)
            {
                this.IsValidID = true;
                this.Code = prj.Code;
                this.Name = prj.Name;
                this.Description = prj.Description;
                this.Active = prj.Active;
                this.Support= prj.Support;
                this.CreateDate = prj.CreateDate;
                this.StartDate = prj.StartDate;
                this.EndDate = prj.EndDate;
            }
            else
            {
                Init();
                IsValidID = false;
            }
/*
            REATrackerDB db = new REATrackerDB();
            using (DataTable dt = db.ProcessCommand("SELECT * FROM PROJECTS WHERE ID=" + id))
            {
                if (dt.Rows.Count > 0)
                {
                    this.IsValidID = true;
                    this.Code = dt.Rows[0]["Code"].ToString();
                    this.Name = dt.Rows[0]["Name"].ToString();
                    this.Description = dt.Rows[0]["Description"].ToString();
                    this.Active = GenericDB.CheckValue(dt.Rows[0]["Active"], false);
                    this.Support = GenericDB.CheckValue(dt.Rows[0]["SupportMode"], false);
                    if (dt.Rows[0]["StartDate"] != DBNull.Value)
                    {
                        this.StartDate = DateTime.Parse(dt.Rows[0]["StartDate"].ToString()).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        this.StartDate = "";
                    }
                    if (dt.Rows[0]["EndDate"] != DBNull.Value)
                    {
                        this.EndDate = DateTime.Parse(dt.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        this.EndDate = "";
                    }
                    this.CreateDate = DateTime.Parse(dt.Rows[0]["CreationDate"].ToString()).ToString("yyyy-MM-dd");

                    //copy the data that is going in to the UI
                    //this is used to save the current/original values of the project
                    this._original = new Project();
                    this._original.ID = id;
                    this._original.Code = this.Code;
                    this._original.Name = this.Name;
                    this._original.Description = this.Description;
                    this._original.Active = this.Active;
                    this._original.Support = this.Support;
                    this._original.StartDate = this.StartDate;
                    this._original.EndDate = this.EndDate;
                    this._original.CreateDate = this.CreateDate;
                    //TODO: owner stuff
                }
                else
                {
                    //doesnt exist, so init as if
                    Init();
                    IsValidID = false;
                }
            }
            //this.populate();
            */
        }

        public ProjectManagerViewModel()
        {
            Init();
            this.IsValidID = true;
        }


        protected void Init()
        {
            this.ProjectID = -1;            
            Code = Name = Description = "";
            PopulateAllProjects();

        }
        protected void PopulateAllProjects()
        {
            if (this.AllProjects != null)
            {
                this.AllProjects.Clear();
            }
            else
            {
                AllProjects = new List<dynamic>();
            }
            REATrackerDB sql = new REATrackerDB();
            using (DataTable dt = sql.GetProjects())
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
                        list[i].SupportName ="R&D";
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
                AllProjects = list;
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
            DataTable MemeberDT = sql.ProcessCommand(Command);
            for (int i = 0; i < MemeberDT.Rows.Count; i++)
            {
                ProductUsage.Add(new System.Dynamic.ExpandoObject());
                ProductUsage[i].ProductName = Convert.ToString(MemeberDT.Rows[i]["Product Name"]);
                ProductUsage[i].BillingCode = Convert.ToString(MemeberDT.Rows[i]["BILLING_CODE"]);
                ProductUsage[i].ProductID = Convert.ToString(MemeberDT.Rows[i]["PRODUCT_ID"]);
            }
        }
        
        public bool Update()
        {
            bool success = false;
            //first make sure we have a valid ID
            if ((this.IsValidID) && (this.ProjectID > 0))
            {
                REATrackerDB db = new REATrackerDB();
                success = db.UpdateProject(this.ProjectID, this.Name, this.Code, this.Description, this.Active, this.Support, this.StartDate, this.EndDate);
            }
            return success;
        }

        public bool Create()
        {
            bool success = false;
            REATrackerDB db = new REATrackerDB();

            success = db.AddProject(this.Name, this.Code, this.Description);

            return success;
        }
    }
}