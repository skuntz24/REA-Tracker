using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{

    public class AttachmentManagerViewModel
    {
        //Inner Class Declarations
        public class FileInfo
        {
            public int FileID { get; set; }
            public int TrackingID { get; set; }
            public int AddedByID { get; set; }
            public String AddedByName { get; set; }
            public String Size { get; set; }
            public String FileName { get; set; }
            public String Description { get; set; }
            public bool isMarkedDelete { get; set; }
            public DateTime AddedOn { get; set; }

            public FileInfo(int ArgsFileID, int ArgsAddedBy, String ArgsAddedByName, int ArgsTrackingID, int ArgsSize, String ArgsFileName, DateTime ArgsFileDate, String ArgsDescription)
            {
                this.FileID = ArgsFileID;
                this.TrackingID = ArgsTrackingID;
                this.Size = this.DisplaySize(ArgsSize);
                this.AddedByID = ArgsAddedBy;
                this.AddedByName = ArgsAddedByName;
                this.AddedOn = ArgsFileDate;
                this.FileName = System.IO.Path.GetFileName(ArgsFileName);
                this.Description = ArgsDescription;
                this.isMarkedDelete = false;
            }
            public FileInfo() { }
            public String DisplaySize(int Size)
            {
                ///<summary>
                /// converts the size to the label of the file
                ///</summary>
                ///<param name="Size">
                /// the size of the file
                ///</param>
                String DisplaySizeText = "";
                if (Size >= 1000 && Size < 1000000)
                {
                    DisplaySizeText = Convert.ToString(Size / 1000) + " KB";
                }
                else if (Size >= 1000000 && Size <= 1000000000)
                {
                    DisplaySizeText = Convert.ToString(Size / 1000000) + " MB";
                }
                else if (Size >= 1000000000)
                {
                    DisplaySizeText = Convert.ToString(Size / 1000000000) + " GB";
                }
                else
                {
                    DisplaySizeText = Convert.ToString(Size) + " B";
                }
                return DisplaySizeText;
            }
        }
        // Variables 
        public List<FileInfo> ListOfFiles { get; set; }
        public String TopRows { get; set; }
        //Edit funtionality
        public String FileName { get; set; }
        public int FileID { get; set; }
        public String FileSCRID { get; set; }
        public String EditAttachmentDescription { get; set; }
        public HttpPostedFileBase File { get; set; }

        //Constructor
        public AttachmentManagerViewModel()
        {
            if (String.IsNullOrEmpty(this.TopRows) ||
                String.IsNullOrWhiteSpace(this.TopRows)
                )
            {
                this.TopRows = "100";
            }
            this.PopulateFiles();
        }
        //Functions
        public void GetAttachment(int ArgsFileID)
        {
            ///<summary>
            /// gets the attachment with the file ID
            ///</summary>
            ///<param name="ArgsFIleID">
            /// the file ID to search for
            ///</param>
            this.FileID = ArgsFileID;
            String Command = " SELECT TRACKING_ID, FILENAME, DESCRIPTION FROM REA_ATTACHMENT WHERE FILE_ID = " + this.FileID;
            REATrackerDB SQL = new REATrackerDB();
            DataTable dt = SQL.ProcessCommand(Command);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                this.FileName = System.IO.Path.GetFileName(Convert.ToString(row["FILENAME"]));
                this.FileSCRID = Convert.ToString(row["TRACKING_ID"]);
                this.EditAttachmentDescription = Convert.ToString(row["DESCRIPTION"]);
            }
        }
        public String UpdateAttachment(int AttachmentId)
        {
            ///<summary>
            /// Updates Attachments
            ///</summary>
            ///<param name="AttachmentId">
            /// the file id that the updates are for
            ///</param>
            String StringCommand = "UPDATE REA_ATTACHMENT SET ";
            StringCommand += ("FILENAME = " + ((String.IsNullOrEmpty(this.FileName)) ? "NULL" : "'" + System.IO.Path.GetFileName(this.FileName) + "'") + ", ");
            StringCommand += ("TRACKING_ID = " + ((String.IsNullOrEmpty(this.FileSCRID)) ? "NULL" : this.FileSCRID) + ", ");
            StringCommand += ("DESCRIPTION = " + ((String.IsNullOrEmpty(this.EditAttachmentDescription)) ?
                "NULL" :
                "'" + this.EditAttachmentDescription + "'") + ", ");
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["btracker"].ConnectionString);
            SqlCommand Command = new SqlCommand(StringCommand, conn);
            if (this.File != null)
            {
                int fileLen = this.File.ContentLength;
                StringCommand += "FILE_SIZE = " + fileLen + ",";
                byte[] fileContent = new Byte[fileLen];
                this.File.InputStream.Read(fileContent, 0, fileLen);
                StringCommand += "FILE_DATA = @FILE";
                Command.CommandText = StringCommand;
                Command.Parameters.AddWithValue("@FILE", fileContent);
            }
            conn.Open();
            StringCommand += (" WHERE FILE_ID =" + this.FileID);
            Command.CommandText = StringCommand;
            Command.ExecuteNonQuery();
            Command.Dispose();
            String DisplayValue = "Attachment Has been Updated";
            return DisplayValue;
        }

        public void PopulateFiles()
        {
            ///<summary>
            /// populates the files from the DB
            ///</summary>
            
            string sql_command =$@"
SELECT 
	{(this.TopRows.Equals("All") ? "" : "TOP(" + this.TopRows + ")")} FILE_ID, TRACKING_ID, FILENAME, DESCRIPTION, FILE_SIZE, FILE_DATE, ADDED_BY, 
	(USER1.FIRST_NAME + ' ' + USER1.LAST_NAME) AS ADDED_BY_NAME
FROM 
	REA_ATTACHMENT
LEFT JOIN ST_USERS USER1 ON USER1.USER_ID = REA_ATTACHMENT.ADDED_BY 
WHERE FILE_ID IS NOT NULL
ORDER BY FILE_DATE DESC;
";
            REATrackerDB sql = new REATrackerDB();
            DataTable FileDataTable = sql.ProcessCommand(sql_command);
            this.ListOfFiles = new List<FileInfo>();
            foreach (System.Data.DataRow row in FileDataTable.Rows)
            {
                int FileID = Convert.ToInt32(row["FILE_ID"] == DBNull.Value ? -1 : row["FILE_ID"]);
                int TrackingID = Convert.ToInt32(row["TRACKING_ID"] == DBNull.Value ? -1 : row["TRACKING_ID"]);
                int AddedByID = Convert.ToInt32(row["ADDED_BY"] == DBNull.Value ? -1 : row["ADDED_BY"]);
                String AddedByName = Convert.ToString(row["ADDED_BY_NAME"] == DBNull.Value ? "" : row["ADDED_BY_NAME"]);
                int Size = Convert.ToInt32(row["FILE_SIZE"] == DBNull.Value ? -1 : row["FILE_SIZE"]);
                String FileName = System.IO.Path.GetFileName(Convert.ToString(row["FILENAME"] == DBNull.Value ? "" : row["FILENAME"]));
                String Description = Convert.ToString(row["DESCRIPTION"] == DBNull.Value ? "" : row["DESCRIPTION"]);
                DateTime FileDate = Convert.ToDateTime(row["FILE_DATE"] == DBNull.Value ? "" : row["FILE_DATE"]);
                ListOfFiles.Add(new FileInfo(FileID, AddedByID, AddedByName, TrackingID, Size, FileName, FileDate, Description));
            }
        }
        public String Delete(int AttachmentId)
        {
            ///<summary>
            /// Deletes the attachment
            ///</summary>
            ///<param name="AttachmentId">
            /// The ID of the file to delete
            ///</param>
            if (this.ListOfFiles == null || this.ListOfFiles.Count == 0)
            {
                this.PopulateFiles();
            }
            String DisplayValue = "";
            if (this.ListOfFiles.First(x => x.FileID == AttachmentId) != null)
            {
                int TrackingIDToDel = this.ListOfFiles.First(x => x.FileID == AttachmentId).TrackingID;
                String Command = "DELETE FROM REA_ATTACHMENT WHERE FILE_ID = " + Convert.ToString(AttachmentId) + "; " +
                    " DELETE FROM REA_HISTORY WHERE REA_HISTORY.FILE_ID = " + Convert.ToString(AttachmentId) + " AND TRACKING_ID = " + Convert.ToString(TrackingIDToDel) +
                    " AND TYPE= 4;";
                REATrackerDB sql = new REATrackerDB();
                sql.ProcessCommand(Command);
                DisplayValue = "Attachment Named: " + this.ListOfFiles.First(x => x.FileID == AttachmentId).FileName +
                    " has been deleted from the data base.";
                this.ListOfFiles.RemoveAll(x => x.AddedByID == AttachmentId);
            }
            return DisplayValue;
        }

    }


}