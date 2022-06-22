using System;
using System.Collections.Generic;
using System.Data;
using QVICommonIntranet.Database;

namespace REA_Tracker.Helpers
{
    /// <summary>
    /// This class contains a bunch of helper functions used across multiple Model classes
    /// Eventually possible could create a base model class
    /// </summary>
    public static class ModelHelper
    {
        /// <summary>
        /// This function returns a dynamic list that has 3 variables for i
        /// If the ID = -1, then the Name value is the name of the group, this typically will be "Favorites", "Active" or "Retired"
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="defaultProductID">0 means all products, else the specific product</param>
        /// <param name="addAllProductsEntry"></param>
        /// <returns></returns>
        public static List<dynamic> populatedProductList(int userID, int defaultProductID = 0, bool addAllProductsEntry = false)
        {
            REATrackerDB _sql = new REATrackerDB();

            if (defaultProductID < 0)
            {
                defaultProductID = 0;
            }
            
            if ((defaultProductID == 0) && (!addAllProductsEntry))
            {
                if (!Int32.TryParse(Convert.ToString(_sql.ProcessScalarCommand("SELECT CASE WHEN DEFAULT_PRODUCT IS NULL THEN 32 ELSE DEFAULT_PRODUCT END AS DEFAULT_PRODUCT FROM ST_USERS WHERE USER_ID= " + userID)), out defaultProductID))
                {
                    //unsuccessfull, set the default product to 0, but this should never happen cause the SQL forces it to 32 (REA Tracker)
                    defaultProductID = 0;
                }
            }
            
            List<dynamic> list = new List<dynamic>();
            int i = 0;

            if (addAllProductsEntry)
            {
                list.Add(new System.Dynamic.ExpandoObject());
                list[i].ID = 0;
                list[i].Name = "ALL PRODUCTS";
                list[i].IsDefault = false;
                i++;
            }

            if (userID == 0)
            {
                string msg = "UserID = 0\r\n";
                msg += $"User: {System.Web.HttpContext.Current.Request.ServerVariables["LOGON_USER"]}\r\n";
                msg += $"IP Address: {System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]}\r\n";
                msg += $"Browser: {System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"]}\r\n";
                msg += $"------------------------------------------------------------\r\n";
                QVICommonIntranet.Utility.Logging.WriteEntry(msg, System.Diagnostics.EventLogEntryType.Warning);
            }
            using (System.Data.DataSet ds = _sql.GetProductList2(userID))
            {
                using (DataTable dt = ds.Tables["Favorites"])
                {
                    if (dt.Rows.Count > 0)
                    {
                        //add the option group "Favorites"
                        list.Add(new System.Dynamic.ExpandoObject());
                        list[i].ID = -1;
                        list[i].Name = "Favorites";
                        list[i].IsDefault = false;
                        i++;

                        //add each product in favorites
                        foreach (DataRow row in dt.Rows)
                        {
                            list.Add(new System.Dynamic.ExpandoObject());
                            list[i].ID = (int)row["PRODUCT_ID"];
                            list[i].Name = row["NAME"].ToString();
                            list[i].IsDefault = (bool)(defaultProductID == list[i].ID);
                            i++;
                        }
                    }
                }

                using (DataTable dt = ds.Tables["Active"])
                {
                    if (dt.Rows.Count > 0)
                    {
                        //add the option group "Active"
                        list.Add(new System.Dynamic.ExpandoObject());
                        list[i].ID = -1;
                        list[i].Name = "Active";
                        list[i].IsDefault = false;
                        i++;

                        //add each product in favorites
                        foreach (DataRow row in dt.Rows)
                        {
                            list.Add(new System.Dynamic.ExpandoObject());
                            list[i].ID = (int)row["PRODUCT_ID"];
                            list[i].Name = row["NAME"].ToString();
                            list[i].IsDefault = (bool)(defaultProductID == list[i].ID);
                            i++;
                        }
                    }
                }

            }
            return list;

        }

    }
}