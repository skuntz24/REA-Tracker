using System;
using System.Collections.Generic;
using System.Data;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{

    public class KeywordManagerViewModel
    {
        public List<dynamic> GetAll { get; set; }

        public int ID { get; protected set; }
        public String Keyword { get; set; }
        public String Description { get; set; }
        public String Usage { get; protected set; }
        public String Error { get; protected set; }

        public KeywordManagerViewModel()
        {
            this.GetAll = new List<dynamic>();
            this.init();
        }

        public void init()
        {
            if (this.GetAll != null)
            {
                this.GetAll.Clear();
            }
            //String command = "SELECT ID, Keyword, Description FROM REA_KEYWORD;";
            String command = @"SELECT REA_KEYWORD.ID, REA_KEYWORD.Keyword, REA_KEYWORD.Description, COUNT(REA_KEYWORD_SCR_RELATION.KEYWORD_ID) AS 'Usage' FROM REA_KEYWORD
                                LEFT JOIN REA_KEYWORD_REA_RELATION ON REA_KEYWORD_REA_RELATION.KEYWORD_ID = REA_KEYWORD.ID
                                GROUP BY REA_KEYWORD.ID, REA_KEYWORD.Keyword, REA_KEYWORD.Description";

            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(command);
            List<dynamic> list = new List<dynamic>();
            int i = 0;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                list.Add(new System.Dynamic.ExpandoObject());
                list[i].ID = Convert.ToString(row[0]);
                list[i].Keyword = Convert.ToString(row[1]);
                list[i].Description = Convert.ToString(row[2] == DBNull.Value ? "None" : row[2]);
                list[i].Usage = Convert.ToString(row[3]);
                i++;
            }
            this.GetAll = list;
        }

        public bool CreateNew()
        {
            REATrackerDB sql = new REATrackerDB();
            bool success = false;

            if (sql.InsertKeyword(this.Keyword, this.Description) > 0)
            {
                success = true;
                Error = string.Empty;
                init();
            }
            else
            {
                Error = sql.LastError;
            }
            return success;
        }
    }

    public class KeywordManagerEditViewModel
    {
        public int ID { get; set; }
        public String Keyword { get; set; }
        public String Description { get; set; }
        public String Usage { get; set; }
        public String Error { get; protected set; }

        public KeywordManagerEditViewModel()
        {

        }

        public KeywordManagerEditViewModel(int keywordID)
        {
            init(keywordID);
        }

        public void init(int keywordID)
        {
            String command = @"SELECT REA_KEYWORD.ID, REA_KEYWORD.Keyword, REA_KEYWORD.Description, COUNT(REA_KEYWORD_REA_RELATION.KEYWORD_ID) AS 'Usage' FROM REA_KEYWORD
                                LEFT JOIN REA_KEYWORD_REA_RELATION ON REA_KEYWORD_REA_RELATION.KEYWORD_ID = REA_KEYWORD.ID WHERE REA_KEYWORD.ID = " + keywordID + @"
                                GROUP BY REA_KEYWORD.ID, REA_KEYWORD.Keyword, REA_KEYWORD.Description";

            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(command);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                ID = Convert.ToInt16(row[0]);
                Keyword = Convert.ToString(row[1]);
                Description = row[2] == DBNull.Value ? "" : Convert.ToString(row[2]);
                Usage = Convert.ToString(row[3]);
            }
        }

        public bool Update()
        {
            REATrackerDB sql = new REATrackerDB();
            bool success = false;

            if (sql.UpdateKeyword(this.ID, this.Keyword, this.Description) > 0)
            {
                success = true;
                Error = string.Empty;
                init(this.ID);
            }
            else
            {
                Error = sql.LastError;
            }
            return success;
        }

        public bool Delete()
        {
            REATrackerDB sql = new REATrackerDB();
            bool success = false;
            if (sql.DeleteKeyword(this.ID) > 0)
            {
                success = true;
                Error = string.Empty;
            }
            else
            {
                Error = sql.LastError;
            }
            return success;
        }
    }

    public class KeywordManagerKeywordDetailsViewModel
    {
        public List<dynamic> GetAll { get; set; }

        //Keyword info
        public int ID { get; protected set; }
        public String Keyword { get; protected set; }
        public String Description { get; protected set; }

        //All SCRs
        public int TRACKING_ID { get; protected set; }
        public String Product { get; set; }
        public String Title { get; set; }

        public KeywordManagerKeywordDetailsViewModel(int keywordID)
        {
            this.GetAll = new List<dynamic>();
            this.init(keywordID);
        }

        public void init(int keywordID)
        {
            if (this.GetAll != null)
            {
                this.GetAll.Clear();
            }

            REATrackerDB sql = new REATrackerDB();

            String command = @"SELECT REA_KEYWORD.ID, REA_KEYWORD.Keyword, REA_KEYWORD.Description FROM REA_KEYWORD WHERE REA_KEYWORD.ID = " + keywordID;
            DataTable dt = sql.ProcessCommand(command);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                ID = Convert.ToInt16(row[0]);
                Keyword = Convert.ToString(row[1]);
                Description = row[2] == DBNull.Value ? "" : Convert.ToString(row[2]);
            }
            dt.Rows.Clear();
            dt.Clear();

            command = @"SELECT [REA_PRODUCT].NAME, [REA_TRACK].TRACKING_ID, [REA_TRACK].TITLE FROM [REA_KEYWORD_REA_RELATION]
                                INNER JOIN REA_TRACK ON REA_TRACK.TRACKING_ID = REA_KEYWORD_REA_RELATION.[TRACKING_ID]
                                INNER JOIN REA_PRODUCT ON REA_TRACK.PRODUCT = REA_PRODUCT.[PRODUCT_ID]
                                WHERE [REA_KEYWORD_REA_RELATION].[KEYWORD_ID] = " + keywordID; ;

            dt = sql.ProcessCommand(command);
            List<dynamic> list = new List<dynamic>();
            int i = 0;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                list.Add(new System.Dynamic.ExpandoObject());
                list[i].Product = Convert.ToString(row[0]);
                list[i].TRACKING_ID = Convert.ToString(row[1]);
                list[i].Title = Convert.ToString(row[2]);
                i++;
            }
            this.GetAll = list;
        }
    }


}