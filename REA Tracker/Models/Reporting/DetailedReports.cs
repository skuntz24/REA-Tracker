using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using REA_Tracker.Helpers;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{
    /*
     * NOTE THE INDEX'S MODEL IS IN ADMIN, REPORT MANAGER
     * AND THE DISPLAY'S MODEL IS THE SCR SEARCH MODEL
     */
    public class MetricReportViewModel 
    {
        private REATrackerDB _sql = new REATrackerDB();

        public List<dynamic> Reports { get; set; }
        public List<int> Totals { get; set; }
        public String Type { get; set; }
        public String CurrentProduct { get; set; }
        public int SelectedProduct { get; set; }
        public List<dynamic> Products{get; set;}
        public String ProductName { get; set; }
        public bool  displayAll{get; set;}

        public MetricReportViewModel(int CurrentUser, String argsType)
        {
            this.SelectedProduct = -1;
            if (String.IsNullOrEmpty(argsType))
            {
                Type = "problems";
                this.displayAll = false;
            }
            else 
            {
                this.Type = argsType;
            }
            this.populate();
            String Command =    "SELECT REA_PRODUCT.NAME "+
                                "FROM ST_USERS "+
                                "INNER JOIN REA_PRODUCT ON REA_PRODUCT.PRODUCT_ID = ST_USERS.DEFAULT_PRODUCT "+
                                "WHERE ST_USERS.USER_ID =" + CurrentUser;
            this.CurrentProduct = Convert.ToString(_sql.ProcessCommand(Command).Rows[0][0]);
            this.Products = new List<dynamic>();
            this.populateProducts();
        }
        public MetricReportViewModel() { }
        public void populateProducts()
        {

            //TODO: replace with ModelHelper.populatedProductList
            DataTable ProductsTable = _sql.GetProductList(true);
            this.Products = new List<dynamic>();
            for (int i = 0; i < ProductsTable.Rows.Count; i++)
            {
                this.Products.Add(new System.Dynamic.ExpandoObject());
                this.Products[i].Name = ProductsTable.Rows[i]["NAME"];
                this.Products[i].ID = ProductsTable.Rows[i]["PRODUCT_ID"];
                this.Products[i].Selected = (this.SelectedProduct == Convert.ToInt32(ProductsTable.Rows[i]["PRODUCT_ID"]) ? "selected" : "");
            }
        }
        public void populate()
        {
            this.Reports = new List<dynamic>();
            int totalLow = 0;
            int totalMedium = 0;
            int totalHigh = 0;
            int totalCritical = 0;
            int totalSumoSqaures = 0;
            int totalTotalOpen = 0;
            int totalDeffered = 0;
            DataTable dt;
            if( this.Type.ToLower() == "problems" )
            {
                this.displayAll = false;
                dt = _sql.GetMetricReportProblem();
            }
            else
            {
                this.displayAll = true;
                dt = _sql.GetMetricReportAll();
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                this.Reports.Add(new System.Dynamic.ExpandoObject());
                this.Reports[i].product = (String)dt.Rows[i]["Product"];
                this.Reports[i].low = (int)dt.Rows[i]["Low"];
                this.Reports[i].medium = (int)dt.Rows[i]["Medium"];
                this.Reports[i].high = (int)dt.Rows[i]["High"];
                this.Reports[i].critical = (int)dt.Rows[i]["Critical"];
                this.Reports[i].SumoSqaures = (int)dt.Rows[i]["Sum of Squares"];
                this.Reports[i].TotalOpen = (int)dt.Rows[i]["Total Open"];
                this.Reports[i].deffered = (int)dt.Rows[i]["Deferred"];
                this.Reports[i].productID = (int)dt.Rows[i]["ID"];

                totalLow += Convert.ToInt32(dt.Rows[i]["Low"]);
                totalMedium += Convert.ToInt32(dt.Rows[i]["Medium"]);
                totalHigh += Convert.ToInt32(dt.Rows[i]["High"]);
                totalCritical += Convert.ToInt32(dt.Rows[i]["Critical"]);
                totalSumoSqaures += Convert.ToInt32(dt.Rows[i]["Sum of Squares"]);
                totalTotalOpen += Convert.ToInt32(dt.Rows[i]["Total Open"]);
                totalDeffered += Convert.ToInt32(dt.Rows[i]["Deferred"]);

            }
            this.Totals = new List<int> (
                new int[]{totalLow, totalMedium, totalHigh, totalCritical, totalSumoSqaures, totalTotalOpen, totalDeffered }
                );

        }
        public void Process(int CurrentUser, String type) 
        {
            this.GetProductName();
            this.populateProducts();
            if (String.IsNullOrEmpty(type)||type.ToLower()==("problems") )
            {
                Type = "problems";
                this.displayAll = false;
            }
            else
            {
                this.Type = type;
                this.displayAll = true;
            }

            //All products
            if (this.SelectedProduct == 0 || this.SelectedProduct == -1)
            {
                String Command = "SELECT REA_PRODUCT.NAME " +
                                "FROM ST_USERS " +
                                "INNER JOIN REA_PRODUCT ON REA_PRODUCT.PRODUCT_ID = ST_USERS.DEFAULT_PRODUCT " +
                                "WHERE ST_USERS.USER_ID =" + CurrentUser;
                this.CurrentProduct = Convert.ToString(_sql.ProcessScalarCommand(Command));
                this.populate();
            }
            else
            {
                this.CurrentProduct = "";
                String products = String.Join(",",BuildTree(this.SelectedProduct, 1));
                this.Reports = new List<dynamic>();
                int totalLow = 0;
                int totalMedium = 0;
                int totalHigh = 0;
                int totalCritical = 0;
                int totalSumoSqaures = 0;
                int totalTotalOpen = 0;
                int totalDeffered = 0;
                DataTable dt;
                if (this.Type.ToLower() == "problems")
                {
                    dt= _sql.GetMetricReportProblem(products);
                }
                else
                {
                    dt = _sql.GetMetricReportAll(products);
                }
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        this.Reports.Add(new System.Dynamic.ExpandoObject());
                        this.Reports[i].product = (String)dt.Rows[i]["Product"];
                        this.Reports[i].low = (int)dt.Rows[i]["Low"];
                        this.Reports[i].medium = (int)dt.Rows[i]["Medium"];
                        this.Reports[i].high = (int)dt.Rows[i]["High"];
                        this.Reports[i].critical = (int)dt.Rows[i]["Critical"];
                        this.Reports[i].SumoSqaures = (int)dt.Rows[i]["Sum of Squares"];
                        this.Reports[i].TotalOpen = (int)dt.Rows[i]["Total Open"];
                        this.Reports[i].deffered = (int)dt.Rows[i]["Deferred"];
                        this.Reports[i].productID = (int)dt.Rows[i]["ID"];

                        totalLow += Convert.ToInt32(dt.Rows[i]["Low"]);
                        totalMedium += Convert.ToInt32(dt.Rows[i]["Medium"]);
                        totalHigh += Convert.ToInt32(dt.Rows[i]["High"]);
                        totalCritical += Convert.ToInt32(dt.Rows[i]["Critical"]);
                        totalSumoSqaures += Convert.ToInt32(dt.Rows[i]["Sum of Squares"]);
                        totalTotalOpen += Convert.ToInt32(dt.Rows[i]["Total Open"]);
                        totalDeffered += Convert.ToInt32(dt.Rows[i]["Deferred"]);
                    }
                    if (this.Reports.Where(u => u.product == this.ProductName) != null)
                    {
                        this.Reports.Insert(0, this.Reports.First(u => u.product == this.ProductName));
                        this.Reports.RemoveAt(this.Reports.LastIndexOf(this.Reports.Last(u => u.product == this.ProductName)));
                    }
                }
                this.Totals = new List<int>(
                    new int[] { totalLow, totalMedium, totalHigh, totalCritical, totalSumoSqaures, totalTotalOpen, totalDeffered }
                    );
                
            }
        }
        private void GetProductName() 
        {
            this.ProductName = 
                Convert.ToString(
                _sql.ProcessScalarCommand(
                    "SELECT NAME FROM REA_PRODUCT WHERE PRODUCT_ID = "+this.SelectedProduct)
                    );
        }
        private List<int> BuildTree(int root, int level)
        {
            List<int> TreeList = new List<int>();
            TreeList.Add(root);
            //Do you have CHILD_ID?
            String check = "SELECT COUNT(CHILD_ID) AS CHILDREN FROM REA_PRODUCT_RELATION WHERE PARENT_ID = " + Convert.ToString(root);
            DataTable checker = _sql.ProcessCommand(check);
            if (Convert.ToInt32(checker.Rows[0][0]) > 0)
            {
                String children = "SELECT CHILD_ID FROM REA_PRODUCT_RELATION WHERE PARENT_ID = " + Convert.ToString(root);
                DataTable childrenDT = _sql.ProcessCommand(children);
                foreach (System.Data.DataRow row in childrenDT.Rows)
                {
                    TreeList.AddRange(BuildTree(Convert.ToInt32(row[0]), level + 1));
                }
            }
            return TreeList;
        }
    }

    public class MetricHistoryViewModel
    {
        private REATrackerDB _sql = new REATrackerDB();

        public string productname { get; set; }

        public int MonthCount   { get; set; }
        public int DayCount     { get; set; }
        public int YearCount    { get; set; }

        public int StartMonth  {get; set;}
        public int StartDay    {get; set;}
        public int StartYear   {get ;set;}

        public int EndMonth    {get; set;}
        public int EndDay      {get; set;}
        public int EndYear     {get; set;}
        public int NumWorkingDays { get; set; }

        public int userID { get; set; }
        public int productID   {get; set;}
        public List<dynamic> Products           { get; set; }

        public List<int> XValues                { get; set; }

        public List<int> SubmittedChartYValues  { get; set; }
        public List<int> ResolvedChartYValues   { get; set; }
        public List<int> ClosedChartYValues     { get; set; }

        public List<int> SubmittedChartYMetrics { get; set; }
        public List<int> ResolvedChartYMetrics  { get; set; }
        public List<int> ClosedChartYMetrics    { get; set; }

        public List<int> SubmittedProblem      {get; set;}
        public List<int> SubmittedEnhancement  {get; set;}
        public List<int> SubmittedPlanned      {get; set;}
                                 
        public List<int> ResolvedProblem       {get; set;}
        public List<int> ResolvedEnhancement   {get; set;}
        public List<int> ResolvedPlanned       {get; set;}
                                
        public List<int> ClosedProblem         {get; set;}
        public List<int> ClosedEnhancement     {get; set;}
        public List<int> ClosedPlanned         {get; set;}

        public double ClosedRates                  { get; set; }
        public double SubmittedRates               { get; set; }
        public double ResolvedRates                { get; set; }

        public MetricHistoryViewModel( int userID )
        {
            this.init(userID);
        }
        public MetricHistoryViewModel()
        {

        }
        public void init(int userID) 
        {
            this.userID = userID;
            this.productID = Convert.ToInt32(_sql.ProcessScalarCommand("SELECT DEFAULT_PRODUCT FROM ST_USERS WHERE ST_USERS.USER_ID = " + userID));

            this.MonthCount = 12;
            this.DayCount = 31;
            this.YearCount = 1990;

            this.StartDay = 1;
            this.StartMonth = 1;
            this.StartYear = DateTime.Now.Year;

            this.EndDay = DateTime.Now.Day;
            this.EndMonth = DateTime.Now.Month;
            this.EndYear = DateTime.Now.Year;

            this.NumWorkingDays = 0; //default...set by GetRates()

            this.populateProducts();
            this.populateCharts();
            this.GetRates();
        }
        public void populateProducts()
        {
            this.Products = Helpers.ModelHelper.populatedProductList(this.userID, this.productID);

            //SCRTrackerHelp sql = new SCRTrackerHelp();
            //DataTable ProductsTable = sql.GetProductList(true);
            //this.Products = new List<dynamic>();
            //for( int i = 0; i<ProductsTable.Rows.Count; i++ )
            //{
            //    this.Products.Add( new System.Dynamic.ExpandoObject());
            //    this.Products[i].Name = ProductsTable.Rows[i]["NAME"];
            //    this.Products[i].ID = ProductsTable.Rows[i]["PRODUCT_ID"];
                
            //    if(this.productID == Convert.ToInt32(ProductsTable.Rows[i]["PRODUCT_ID"]))
            //    {
            //        this.Products[i].Selected = "selected";
            //        this.productname = Convert.ToString(ProductsTable.Rows[i]["NAME"]);
            //    }
            //    else
            //    {
            //        this.Products[i].Selected ="";
            //    }
            //}
        }

        public void populateCharts()
        {
            this.SubmittedProblem = new List<int>(new[] { 0, 0, 0, 0 });
            this.SubmittedEnhancement = new List<int>(new[] { 0, 0, 0, 0 });
            this.SubmittedPlanned = new List<int>(new[] { 0, 0, 0, 0 });

            this.ResolvedProblem = new List<int>(new[] { 0, 0, 0, 0 });
            this.ResolvedEnhancement = new List<int>(new[] { 0, 0, 0, 0 });
            this.ResolvedPlanned = new List<int>(new[] { 0, 0, 0, 0 });

            this.ClosedProblem = new List<int>(new[] { 0, 0, 0, 0 });
            this.ClosedEnhancement = new List<int>(new[] { 0, 0, 0, 0 });
            this.ClosedPlanned = new List<int>(new[] { 0, 0, 0, 0 });

            List<String> TypesOfMetrics = new List<String>(new[]{"Submitted","Resolved","Closed"});            
            foreach(String Title in TypesOfMetrics)
            {
                String Query =  "SELECT ST_TRACK.TRACKING_ID AS TRACKING_ID, REA_PRODUCT.NAME AS PRODUCT, "
	                            +"ST_TRACK.PRIORITY AS HP, ST_TRACK.ISSUE_TYPE AS HI, "
					            +"ST_TRACK.PRIORITY AS PRIORITY_ID, ST_PRIORITY.NAME AS PRIORITY, "
					            +"ST_TRACK.VP_MAJOR AS MAJOR, ST_TRACK.VP_MINOR AS MINOR, ST_ISSUE_TYPE.ISSUE_ID AS ISSUE_ID, "
					            +"ST_STATUS.NAME AS STATUS, ST_TRACK.TITLE AS TITLE, ST_TRACK.ASSIGNED_TO AS ASSIGNED_TO, "
					            +"ST_TRACK.PRODUCT AS PRODUCT_ID FROM ST_TRACK INNER JOIN "
                                +"REA_PRODUCT ON ST_TRACK.PRODUCT = REA_PRODUCT.PRODUCT_ID INNER JOIN "
                                +"ST_PRIORITY ON ST_TRACK.PRIORITY = ST_PRIORITY.ID INNER JOIN "
					            +"ST_ISSUE_TYPE ON ST_TRACK.ISSUE_TYPE = ST_ISSUE_TYPE.ISSUE_ID INNER JOIN "
                                +"ST_STATUS ON ST_TRACK.STATUS = ST_STATUS.STATUS_ID "
					            +"WHERE ("+ Title +"_ON >= '"+this.StartYear+"-"+this.StartMonth +"-"+this.StartDay+" 00:00:00' AND "
					            +Title+"_ON  <= '"+this.EndYear+"-"+this.EndMonth+"-"+ this.EndDay+" 23:59:59' ) "
					            +"AND PRODUCT="+this.productID+" ";
                DataTable dt = _sql.ProcessCommand(Query);
                for (int i = 0; i < dt.Rows.Count; i++ )
                {
                    if(Title == "Submitted")
                    {//HI is issuetype.
                        //HP is Priority.
                        if(Convert.ToInt32(dt.Rows[i]["HI"]) == 1)
                        {
                            this.SubmittedProblem[Convert.ToInt32(dt.Rows[i]["HP"]) - 1]++;
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["HI"]) == 2)
                        {
                            this.SubmittedEnhancement[Convert.ToInt32(dt.Rows[i]["HP"]) - 1]++;
                        }
                        else 
                        {
                            this.SubmittedPlanned[Convert.ToInt32(dt.Rows[i]["HP"]) - 1]++;
                        }
                    }
                    else if (Title == "Resolved")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["HI"]) == 1)
                        {
                            this.ResolvedProblem[Convert.ToInt32(dt.Rows[i]["HP"]) - 1]++;
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["HI"]) == 2)
                        {
                            this.ResolvedEnhancement[Convert.ToInt32(dt.Rows[i]["HP"]) - 1]++;
                        }
                        else
                        {
                            this.ResolvedPlanned[Convert.ToInt32(dt.Rows[i]["HP"]) - 1]++;
                        }
                    }
                    else 
                    {
                        if (Convert.ToInt32(dt.Rows[i]["HI"]) == 1)
                        {
                            this.ClosedProblem[Convert.ToInt32(dt.Rows[i]["HP"]) - 1]++;
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["HI"]) == 2)
                        {
                            this.ClosedEnhancement[Convert.ToInt32(dt.Rows[i]["HP"]) - 1]++;
                        }
                        else
                        {
                            this.ClosedPlanned[Convert.ToInt32(dt.Rows[i]["HP"]) - 1]++;
                        }
                    }
                    
                }
            }

        }
        public List<int> Metricform( List<int>argsInputCount ) 
        {
            List<int> Result = new List<int>();
            Result.Add(argsInputCount[0]);
            Result.Add(argsInputCount[1]*4);
            Result.Add(argsInputCount[2]*9);
            Result.Add(argsInputCount[3]*16);
            return Result;
        }
        public List<int> Total( List<int>Sub, List<int>Enhancement, List<int>Plan)
        {
            List<int> Result = new List<int>();
            Result.Add(Sub[0] + Enhancement[0] + Plan[0]);
            Result.Add(Sub[1] + Enhancement[1] + Plan[1]);
            Result.Add(Sub[2] + Enhancement[2] + Plan[2]);
            Result.Add(Sub[3] + Enhancement[3] + Plan[3]);
            return Result;
        }
        public void GetRates ()
        {
            DateTime start = new DateTime(StartYear, StartMonth, StartDay);
            DateTime end = new DateTime(EndYear, EndMonth, EndDay);
            NumWorkingDays = _sql.GetNumWorkingDays(start, end);            
            if (NumWorkingDays != 0)
            {
                double difference = Convert.ToInt32(NumWorkingDays);

                List<int> ClosedRateTotal = Total(this.ClosedProblem,this.ClosedPlanned,this.ClosedEnhancement);
                List<int> ResolvedRateTotal = Total(this.ResolvedProblem, this.ResolvedPlanned, this.ResolvedEnhancement);
                List<int> SubmittedRateTotal = Total(this.SubmittedPlanned, this.SubmittedProblem, this.SubmittedEnhancement);

                this.ClosedRates    = ((ClosedRateTotal[0] + ClosedRateTotal[1] + ClosedRateTotal[2] + ClosedRateTotal[3])/difference);
                this.ResolvedRates  = ((ResolvedRateTotal[0] + ResolvedRateTotal[1] + ResolvedRateTotal[2] + ResolvedRateTotal[3])/difference);
                this.SubmittedRates = ((SubmittedRateTotal[0] + SubmittedRateTotal[1] + SubmittedRateTotal[2] + SubmittedRateTotal[3])/difference);

            }

        }
    }
}