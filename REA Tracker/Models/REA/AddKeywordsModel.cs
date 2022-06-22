using System;
using System.Collections.Generic;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{
    public class AddKeywords
    {
        public int TrackingID { get; set; }
        public String KeywordIDs { get; set; }
        public bool CloseWindow { get; set;}

        public List<dynamic> Keywords { get; set; }

        public AddKeywords()
        {
            
        }

        public AddKeywords(int trackingID)
        {
            this.TrackingID = trackingID;
            this.CloseWindow = false;
            setKeywordsAvailable();
        }

        private void setKeywordsAvailable()
        {
            REATrackerDB sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            using (System.Data.DataTable dt = sql.GetKeywordsAvailable(this.TrackingID))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].ID = Convert.ToString(row[0]);
                    list[i].Name = Convert.ToString(row[1]);
                    list[i].Description = Convert.ToString(row[2]);
                    list[i].Used = String.Equals(row[3].ToString(), "1");
                    i++;
                }
            }
            this.Keywords = list;
        }

        public void Update(int trackingID, string keywordIDs)
        {
            if (trackingID > 0)
            {
                REATrackerDB sql = new REATrackerDB();
                sql.UpdateKeywords(trackingID, keywordIDs);
            }
            setKeywordsAvailable();
            CloseWindow = true;
        }
    }
}