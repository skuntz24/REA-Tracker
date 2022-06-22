using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using REA_Tracker.Helpers;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models.Home
{
    public class ReleaseViewModel
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        //To Make Displaying Easier
        public String TabName { get; set; }
        public String DisplayVersion { get; set; }
        public string misc { get; set; }


        public List<REATrackerDB.ReleaseVersion> ListOfRecords { get; set; }
        public ReleaseViewModel(int major, int minor, int Product, int Index, String MISC)
        {
            this.Major = major;
            this.Minor = minor;
            this.misc = MISC;
            this.TabName = "tab" + Convert.ToString(Index);
            this.DisplayVersion = this.Major + "." + this.Minor + "." + "x";
            this.ListOfRecords = new List<REATrackerDB.ReleaseVersion>();
            String Command = "SELECT  " +
                " (ST_PRODUCT_RELEASE.MAJOR+'.'+ST_PRODUCT_RELEASE.MINOR+'.'+ST_PRODUCT_RELEASE.BUILD)AS VERSION, " +
                " ST_PRODUCT_RELEASE.MAJOR AS MAJOR, " +
                " ST_PRODUCT_RELEASE.MINOR AS MINOR, " +
                " ST_PRODUCT_RELEASE.BUILD AS BUILD, " +
                " ST_PRODUCT_RELEASE.MISC AS MISC, " +
                " ST_PRODUCT_RELEASE.BUILT_ON AS BUILT_ON, " +
                " ST_PRODUCT_RELEASE.IS_CUSTOMER_RELEASE, " +
                " ST_PRODUCT_RELEASE.NOTES, " +
                " ST_PRODUCT_RELEASE.SCR_LIST, " +
                " ST_PRODUCT_RELEASE.BUILD_ID AS BUILD_ID, " +
                " ST_PRODUCT_RELEASE.DB_VERSION AS DB_VERSION " +
                " FROM ST_PRODUCT_RELEASE " +
                " WHERE PRODUCT_ID = " + Product +
                " AND CAST(MAJOR AS INT) = " + this.Major +
                " AND CAST(MINOR AS INT) =" + this.Minor +
                " ORDER BY CAST(BUILD AS INT) DESC;";
            REATrackerDB vsql = new REATrackerDB();
            DataTable vdt = vsql.ProcessCommand(Command);
            foreach (System.Data.DataRow vrow in vdt.Rows)
            {
                int Vmajor = Convert.ToInt32(vrow["MAJOR"] == DBNull.Value ? -1 : vrow["MAJOR"]);
                int Vminor = Convert.ToInt32(vrow["MINOR"] == DBNull.Value ? -1 : vrow["MINOR"]);
                int Vbuild = Convert.ToInt32(vrow["BUILD"] == DBNull.Value ? -1 : vrow["BUILD"]);
                string Vmisc = Convert.ToString(vrow["MISC"] == DBNull.Value ? -1 : vrow["MISC"]);
                DateTime? BuiltOn = (vrow["BUILT_ON"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(vrow["BUILT_ON"]));
                int CustomerRelease = Convert.ToInt32(vrow["IS_CUSTOMER_RELEASE"] == DBNull.Value ? -1 : vrow["IS_CUSTOMER_RELEASE"]);
                String Notes = Convert.ToString(vrow["NOTES"] == DBNull.Value ? "" : vrow["NOTES"]);
                String SCRList = Convert.ToString(vrow["SCR_LIST"] == DBNull.Value ? "" : vrow["SCR_LIST"]);
                int Built_ID = Convert.ToInt32(vrow["BUILD_ID"] == DBNull.Value ? -1 : vrow["BUILD_ID"]);
                String DB_Version = Convert.ToString(vrow["DB_VERSION"] == DBNull.Value ? "" : vrow["DB_VERSION"]);
                this.ListOfRecords.Add(
                    new REATrackerDB.ReleaseVersion(Built_ID, Vmajor, Vminor, Vbuild, Vmisc, BuiltOn, CustomerRelease, Notes, SCRList, DB_Version)
                    );
            }

        }
    }
}