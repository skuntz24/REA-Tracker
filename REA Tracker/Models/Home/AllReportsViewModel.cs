using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using QVICommonIntranet.Database;
namespace REA_Tracker.Models.Home
{

    public class AllReportsViewModel
    {
        public List<REATrackerDB.ReleaseVersion> Releases { get; set; }
        public REATrackerDB.ProductInfo ProductInfo;
        public Boolean HasAccessToProduct { get; set; }
        private REATrackerDB _sql = new REATrackerDB();
        public AllReportsViewModel(int UserID, int productID)
        {
            this.ProductInfo = _sql.GetProductBasic(productID);
            /*
            String Command = "SELECT * FROM ST_PRODUCT_RELEASE WHERE PRODUCT_ID=" + this.ProductID +
            " ORDER BY " +
            "CAST(ST_PRODUCT_RELEASE.MAJOR AS INT)DESC, " +
            "CAST(ST_PRODUCT_RELEASE.MINOR AS INT)DESC, " +
            "CAST(ST_PRODUCT_RELEASE.BUILD AS INT)DESC, " +
            "ST_PRODUCT_RELEASE.MISC";
            string GetProductNameCommand = "SELECT NAME FROM ST_PRODUCT WHERE PRODUCT_ID = " + this.ProductID;
            */
            //DataTable dt = sql.ProcessCommand(Command);
            using (DataTable dt = _sql.GetAllProductReleases(this.ProductInfo.ID))
            {
                this.Releases = new List<REATrackerDB.ReleaseVersion>();
                foreach (System.Data.DataRow row in dt.Rows)
                {

                    REATrackerDB.ReleaseVersion version = new REATrackerDB.ReleaseVersion(

                                    Convert.ToInt32(row["BUILD_ID"] == DBNull.Value ? -1 : row["BUILD_ID"]),
                                    Convert.ToInt32(row["MAJOR"] == DBNull.Value ? -1 : row["MAJOR"]),
                                    Convert.ToInt32(row["MINOR"] == DBNull.Value ? -1 : row["MINOR"]),
                                    Convert.ToInt32(row["BUILD"] == DBNull.Value ? -1 : row["BUILD"]),
                                    Convert.ToString(row["MISC"] == DBNull.Value ? "" : row["MISC"]),
                                    Convert.ToDateTime(row["BUILT_ON"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["BUILT_ON"])),
                                    Convert.ToInt32(row["IS_CUSTOMER_RELEASE"] == DBNull.Value ? -1 : row["IS_CUSTOMER_RELEASE"]),
                                    Convert.ToString(row["NOTES"] == DBNull.Value ? "" : row["NOTES"]),
                                    Convert.ToString(row["SCR_LIST"] == DBNull.Value ? "" : row["SCR_LIST"]),
                                    Convert.ToString(row["DB_VERSION"] == DBNull.Value ? "" : row["DB_VERSION"])
                            );

                    Releases.Add(version);
                }
            }
        }
        
        public Boolean CheckAccess(int UserID)
        {
            using (System.Data.DataTable dt = _sql.GetProductList(UserID, true))
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    if (Convert.ToInt32(row["PRODUCT_ID"]) == this.ProductInfo.ID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
