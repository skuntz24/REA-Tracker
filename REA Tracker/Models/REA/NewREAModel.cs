using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{
    public class NewREA
    {
        //      <type>          <name>              < Variables ><-Arrays->
        //Inputs
        //General Settings
        [Required(ErrorMessage = "Required.")]
        public string Title { get; set; }
        public int ProductID { get; set; }
        [Range(0, Int32.MaxValue, ErrorMessage = "Please enter in a postive integer in the Version Found Major.")]
        public int VersionFoundMajor { get; set; }
        [Range(0, Int32.MaxValue, ErrorMessage = "Please enter in a postive integer in the Version Found Minor.")]
        public int VersionFoundMinor { get; set; }
        [Range(0, Int32.MaxValue, ErrorMessage = "Please enter in a postive integer in the Version Found Build.")]
        public int VersionFoundBuild { get; set; }
        public string VersionFoundMisc { get; set; }
        public int IssueID { get; set; }
        [IntegerNoZeroAttribute(ErrorMessage = "Required.")]
        [Required(ErrorMessage = "Required.")]
        public int PriorityID { get; set; }
        public int ModuleID { get; set; }
        //[Required(ErrorMessage = "Required.")]
        public string Details { get; set; }
        public int IsDocumentChange { get; set; }//Req_doc_change (0=no,1=yes,2=done,3=tbd)
        public bool IsRequirementChange { get; set; }//Req_req_doc 
        //Customer Information
        public bool CustomerReported { get; set; }
        public string CompanyName { get; set; }
        public string Location { get; set; }
        public string ContactPerson { get; set; }
        public string Syteline { get; set; }
        public string Machine { get; set; }
        //Enhancement Details
        //[MinLength(10, ErrorMessage = "Please enter more detailed Problem")]
        public string Problem { get; set; }
        //[MinLength(10, ErrorMessage = "Please enter more detailed Solution")]
        public string Solution { get; set; }
        //[MinLength(10, ErrorMessage = "Please enter more detailed Benefit")]
        public string Benefit { get; set; }
        public string Background { get; set; }
        public string Story { get; set; }
        public string Acceptance { get; set; }

        public String HiddenHashCode { get; set; }
        //Planning details
        //[DoubleNoNegativeNullableAttribute(ErrorMessage="Please enter a postive double.")]
        [Range(0, Double.MaxValue, ErrorMessage = "Please enter in a postive number.")]
        public double? EstimateSpecifiedHours
        { get; set; }
        //[DoubleNoNegativeNullableAttribute(ErrorMessage = "Please enter a postive double.")]
        [Range(0, Double.MaxValue, ErrorMessage = "Please enter in a postive number.")]
        public double? EstimateFixHours { get; set; }
        //[DoubleNoNegativeNullableAttribute(ErrorMessage = "Please enter a postive double.")]
        [Range(0, Double.MaxValue, ErrorMessage = "Please enter in a postive number.")]
        public double? EstimateTestHours { get; set; }
        //[IntegerNoNegativeNullableAttribute(ErrorMessage = "Please enter in a postive integer in the Version Planned Major.")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please enter in a postive integer in the Version Planned Major.")]
        public int? VersionPlannedMajor { get; set; }
        //[IntegerNoNegativeNullableAttribute(ErrorMessage = "Please enter in a postive integer in the Version Planned Minor.")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please enter in a postive integer in the Version Planned Minor.")]
        public int? VersionPlannedMinor { get; set; }
        //[IntegerNoNegativeNullableAttribute(ErrorMessage = "Please enter in a postive integer in the Version Planned Build.")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Please enter in a postive integer in the Version Planned Build.")]
        public int? VersionPlannedBuild { get; set; }
        public string VersionPlannedMisc { get; set; }
        public int? Rank { get; set; }
        public int? Size { get; set; }

        //Related SCR
        public int? RelatedInputID { get; set; }
        public List<RelatedREA> RelatedREAIDs { get; set; }
        //Attachments
        public List<HttpPostedFileBase>
                        File
        { get; set; }
        public String FileDescription { get; set; }
        public List<SCRAttachments>
                                Files
        { get; set; }
        public string UploadErrorMessage { get; set; }

        //Display Items
        //Drop Downs
        public List<dynamic> ProductList { get; set; }
        public List<dynamic> IssueList { get; set; }
        public List<dynamic> PriorityList { get; set; }
        public List<dynamic> ModuleList { get; set; }
        //Validation items
        //List of Existing builds
        public List<string> ExistingBuilds { get; set; }
        //Private Items
        public int UserID { get; set; }
        //constructors 
        public NewREA()
        {
            HttpContext context = HttpContext.Current;
            this.IssueID = 1;
            this.IsDocumentChange = 3;//set to TBD
            //Populate storage variables
            if (this.RelatedREAIDs == null)
            {
                this.RelatedREAIDs = new List<RelatedREA>();
            }
            if (this.Files == null)
            {
                this.Files = new List<SCRAttachments>();
            }
            if (context.Session["Files"] == null)
            {
                context.Session.Add("Files", new List<SCRAttachments>());
            }
        }
        public NewREA(int StUserID, int CurrentProduct)
        {
            this.ProductID = CurrentProduct;
            this.IssueID = 1;
            this.IsDocumentChange = 3;//set to TBD
            HttpContext context = HttpContext.Current;
            if (String.IsNullOrEmpty(this.HiddenHashCode))
            {
                this.HiddenHashCode = "NewFiles" + DateTime.Now.ToString();
            }
            //Populate storage variables
            if (this.RelatedREAIDs == null)
            {
                this.RelatedREAIDs = new List<RelatedREA>();
            }
            if (this.Files == null)
            {
                this.Files = new List<SCRAttachments>();
            }
            if (context.Session[this.HiddenHashCode] == null)
            {
                context.Session.Add(this.HiddenHashCode, new List<SCRAttachments>());
            }
            this.UserID = StUserID;
        }
        //  ==========================================================================
        //||    Get Methods                                                          ||  
        //  ==========================================================================
        public void InitGet()

        {
            //Populate Products List
            this.ProductList = this.getProducts(this.UserID);
            this.ExistingBuilds = this.getBuilds(this.ProductID);
            this.PriorityList = this.getPriority();
        }
        //Sets Product
        private List<dynamic> getProducts(int userID)
        {
            REATrackerDB _sql = new REATrackerDB();
            //this.ProductID = defaultProduct;
            if (this.ProductID == 0)
            {
                int temp = 0;
                Int32.TryParse(Convert.ToString(
                    _sql.ProcessScalarCommand("SELECT DEFAULT_PRODUCT FROM ST_USERS WHERE USER_ID= " + userID)
                    ), out temp);
                this.ProductID = temp;
            }
            List<dynamic> list = new List<dynamic>();
            using (System.Data.DataTable dt = _sql.GetProductList(userID))
            {
                int i = 0;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].ID = (int)dt.Rows[i][0];
                    list[i].Name = dt.Rows[i][1].ToString();
                    list[i].IsDefault = (this.ProductID == list[i].ID) ? "Selected" : "";
                    i++;
                }
            }
            return list;
        }
        private List<String> getBuilds(int ProductID)
        {
            String cmdText = "EXEC GetVersionsByProduct " + Convert.ToString(ProductID) + ";";
            List<String> Release = new List<String>();
            REATrackerDB sql = new REATrackerDB();
            sql.OpenConnection();
            DataTable dt = sql.ProcessCommand(cmdText);
            foreach (DataRow row in dt.Rows)
            {
                Release.Add(row["Product Releases"].ToString());
            }
            sql.CloseConnection();

            return Release;
        }
        private List<dynamic> getPriority()
        {
            int defaultPriority = -1;
            if (this.PriorityID != 0)
            {
                defaultPriority = this.PriorityID;
            }
            REATrackerDB _sql = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            String cmdText = "SELECT REA_priority.PRIORITY_ID, REA_priority.NAME, REA_priority.DESCRIPTION FROM REA_priority;";
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmdText))
            {
                int i = 1;//index to list
                int j = 0;//index to datatable
                list.Add(new System.Dynamic.ExpandoObject());
                list[0].ID = "";
                list[0].Name = "";
                list[0].IsDefault = "";
                list[0].Description = "";
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].ID = (int)dt.Rows[j][0];
                    list[i].Name = dt.Rows[j][1].ToString();
                    list[i].IsDefault = (defaultPriority == list[i].ID) ? " selected " : "";
                    list[i].Description = dt.Rows[j][2];
                    i++;
                    j++;
                }
            }
            return list;
        }

        //  ==========================================================================
        //||    Post Methods                                                         ||  
        //  ==========================================================================
        public int Add(bool Corrolate)
        {
            bool exists = false;
            int state = 1;
            bool changed = false;

            RelatedREA toAdd = new RelatedREA(-1, Convert.ToInt32(this.RelatedInputID), Corrolate);
            exists = toAdd.getFields();

            if (!exists)
            {
                state = -1;
            }
            else
            {
                foreach (RelatedREA SCR in this.RelatedREAIDs)
                {
                    if ((SCR.otherTrackingId == this.RelatedInputID) && (SCR.correlated == Corrolate))
                    {
                        state = 0;
                    }
                    else if ((SCR.otherTrackingId == this.RelatedInputID) && (SCR.correlated != Corrolate))
                    {
                        SCR.correlated = Corrolate;
                        changed = true;
                    }
                }
                if (state > 0 && !changed)
                {
                    this.RelatedREAIDs.Add(toAdd);
                }
            }
            return state;
        }
        public int Upload()
        {
            //1     = Uploaded
            //0     = File too large
            //-1    = File is missing
            //-2    = Description is missing

            int MAX_SIZE_BYTES = (100 * 1000000); //100 MB maximum size
            int value = 0;

            HttpContext context = HttpContext.Current;
            if (context.Session[this.HiddenHashCode] != null && ((List<SCRAttachments>)context.Session[this.HiddenHashCode]).Count() > 0)
            {
                this.Files = (List<SCRAttachments>)context.Session[this.HiddenHashCode];
            }
            if (this.File[0] != null)
            {
                if (this.File[0].ContentLength < MAX_SIZE_BYTES)
                {
                    if (this.FileDescription != null && this.FileDescription.Trim() != null && this.FileDescription.Trim() != "")
                    {

                        SCRAttachments nfile = new SCRAttachments(this.File[0], this.FileDescription);
                        this.Files.Add(nfile);
                        value = 1;//Successful 
                    }
                    else
                    {
                        value = -2;//missing description
                    }
                }
                //file too large
                else
                {
                    this.UploadErrorMessage = "File is too large. File limit is ~100 MB";
                }
            }
            else
            {
                value = -1;//missing file
            }
            context.Session[this.HiddenHashCode] = this.Files;
            return value;
        }
        public void RemoveFiles(string button)
        {
            HttpContext context = HttpContext.Current;
            List<SCRAttachments> temp = ((List<SCRAttachments>)context.Session[this.HiddenHashCode]);
            for (int i = 0; i < this.Files.Count; i++)
            {
                if (Files[i].remove)
                {
                    Files.RemoveAt(i);
                    ((List<SCRAttachments>)context.Session[this.HiddenHashCode]).RemoveAt(i);
                    i--;
                }
            }
            this.Files = (List<SCRAttachments>)context.Session[this.HiddenHashCode];
        }
        public void RemoveRelated(string button)
        {
            int i = getIndexToRemove(button);
            if (i != -1)
            {
                RelatedREAIDs.RemoveAt(i);
            }
        }
        private int getIndexToRemove(string button)
        {
            string[] words = button.Split('-');
            int value = 0;
            if (!Int32.TryParse(words[words.Length - 1], out value))
            {
                value = -1;
            }
            return value;
        }
        public int Save(string aspID, int stID)
        {
            int NewTrackingId = 0;
            if (PriorityID == -1 || this.Title == null || this.Title.Trim() == "")
            {// get vaildators for issue type.
                return 0;
            }

            QVICommonIntranet.Database.REATrackerDB.REA newSCR = new QVICommonIntranet.Database.REATrackerDB.REA();
            //Set up the new SCR
            newSCR.TransactionID = "InsertSCR" + aspID;
            newSCR.submitted_by = stID;
            newSCR.product = this.ProductID;
            newSCR.versionfound_major = this.VersionFoundMajor;
            newSCR.versionfound_minor = this.VersionFoundMinor;
            newSCR.versionfound_build = this.VersionFoundBuild;
            newSCR.versionfound_misc = this.VersionFoundMisc;
            newSCR.assigned_to_ID = this.getAssignedTo(this.IssueID, this.ProductID);
            newSCR.module_id = this.ModuleID;
            newSCR.issue_id = this.IssueID;
            newSCR.priority_id = this.PriorityID;
            newSCR.title = this.Title;
            newSCR.Requires_Document_change = this.IsDocumentChange;
            newSCR.Requires_Requirement_change = this.IsRequirementChange;
            string enhancement_details = "Problem/Opportunity" + "\n" + "--------------------------------------------------------------------------------" + "\n" + this.Problem + "\n" + "\n"
                                        + "Proposed Solution" + "\n" + "--------------------------------------------------------------------------------" + "\n" + this.Solution + "\n" + "\n"
                                        + "Benefit" + "\n" + "--------------------------------------------------------------------------------" + "\n" + this.Benefit;

            string planned_work_details = "Background/Rational" + "\n" + "--------------------------------------------------------------------------------" + "\n" + (String.IsNullOrWhiteSpace(this.Background) ? "None" : this.Background) + "\n" + "\n"
                                        + "Story" + "\n" + "--------------------------------------------------------------------------------" + "\n" + this.Story + "\n" + "\n"
                                        + "Acceptance Criteria" + "\n" + "--------------------------------------------------------------------------------" + "\n" + (String.IsNullOrWhiteSpace(this.Acceptance) ? "None" : this.Acceptance);

            switch (IssueID)
            {
                case 2:
                    {
                        newSCR.Description = enhancement_details;
                        newSCR.Problem = this.Problem;
                        newSCR.Solution = this.Solution;
                        newSCR.Benifit = this.Benefit;
                        break;
                    }
                case 3:
                    {
                        newSCR.Description = planned_work_details;
                        newSCR.Problem = this.Background;
                        newSCR.Solution = this.Story;
                        newSCR.Benifit = this.Acceptance;
                        break;
                    }
                default:
                    {
                        newSCR.Description = this.Details;
                        newSCR.Problem = "";
                        newSCR.Solution = "";
                        newSCR.Benifit = "";
                        break;
                    }
            }
            newSCR.CustomerBug = this.CustomerReported;
            newSCR.MachineSN = this.Machine;
            if (this.ModuleID != 0)
            {
                newSCR.module_id = this.ModuleID;
            }
            if (this.CustomerReported)
            {
                newSCR.CustomerName = this.ContactPerson;
                newSCR.CustomerLocation = this.Location;
                newSCR.CustomerCompany = this.CompanyName;
            }
            else
            {
                newSCR.CustomerName = "";
                newSCR.CustomerLocation = "";
                newSCR.CustomerCompany = "";
            }
            newSCR.SytelineRef = this.Syteline;
            newSCR.EstimateHoursToFix = this.EstimateFixHours;
            newSCR.EstimateHoursToSpec = this.EstimateSpecifiedHours;
            newSCR.EstimateHoursToTest = this.EstimateTestHours;
            newSCR.VersionPlannedMajor = this.VersionPlannedMajor;
            newSCR.VersionPlannedMinor = this.VersionPlannedMinor;
            newSCR.VersionPlannedBuild = this.VersionPlannedBuild;
            newSCR.VersionPlannedMisc = this.VersionPlannedMisc;
            newSCR.Rank = this.Rank;
            newSCR.Size = this.Size;

            //Execute the SQL Code
            REATrackerDB sql = new REATrackerDB();
            NewTrackingId = sql.InsertREA(newSCR);



            this.ProcessAttachments(NewTrackingId, stID);
            this.ProcessRelatedREA(NewTrackingId, stID);
            DisplayReportsViewModel.AddToWatchList watch = new DisplayReportsViewModel.AddToWatchList(NewTrackingId);
            if (watch.isinWatchList(this.UserID))
            {
                watch.Remove(NewTrackingId, Convert.ToInt32(this.UserID));
            }
            else
            {
                watch.Process(NewTrackingId, Convert.ToString(this.UserID), false);
            }
            return NewTrackingId;
        }

        public bool ValidateEnhancement()
        {
            bool value = true;
            if (this.IssueID == 2 && (String.IsNullOrWhiteSpace(this.Problem) ||
                String.IsNullOrWhiteSpace(this.Benefit) ||
                String.IsNullOrWhiteSpace(this.Solution))
              )
            {
                value = false;
            }
            return value;
        }

        public bool ValidateDetails()
        {
            bool value = false;
            if ((this.IssueID == 1) && (string.IsNullOrWhiteSpace(this.Details) == false))
            {
                value = true;
            }
            return value;
        }

        public bool ValidatePlannedWork()
        {
            bool value = false;

            if ((this.IssueID == 3) && (string.IsNullOrWhiteSpace(this.Story) == false))
            {
                value = true;
            }
            return value;
        }

        public bool ValidateCustomerInfo()
        {
            bool value = true;
            if (this.CustomerReported == true && String.IsNullOrWhiteSpace(this.ContactPerson))
            {
                value = false;
            }
            return value;
        }
        private int getAssignedTo(int type, int productID)
        {
            int id = 0;
            int SystemID = -1;
            int SCCBID = 135;
            //if the issue is an enhancement assign to sccb
            string VALIDATOR = "";
            string SCCB = "";
            string cmd = "SELECT ST_PRODUCT.VALIDATOR, ST_PRODUCT.SCCB FROM ST_PRODUCT " +
                "WHERE ST_PRODUCT.PRODUCT_ID= " + Convert.ToString(productID) + ";";
            REATrackerDB _sql = new REATrackerDB();
            _sql.OpenConnection();
            DataTable dt = _sql.ProcessCommand(cmd);
            if (dt.Rows.Count > 0)
            {
                VALIDATOR = Convert.ToString(dt.Rows[0][0]);
                SCCB = Convert.ToString(dt.Rows[0][1]);
            }
            _sql.CloseConnection();
            if (type == 2)
            {
                //if null == assigned to sccb ID = 135
                if (SCCB != "" && SCCB != null)
                {
                    id = Convert.ToInt32(SCCB);
                }
                else
                {
                    id = SCCBID;
                }
            }
            //else submit to validator 
            else
            {
                //if null == assigned to system ID = -1
                if (VALIDATOR != "" && VALIDATOR != null)
                {
                    id = Convert.ToInt32(VALIDATOR);
                }
                else
                {
                    id = SystemID;
                }
            }
            return id;
        }
        private void ProcessRelatedREA(int NewTrackingID, int SubmittedByID)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["btracker"].ConnectionString);
            conn.Open();

            foreach (var item in this.RelatedREAIDs)
            {//ADD
                SqlParameter Title = new SqlParameter();
                Title.SqlDbType = SqlDbType.VarChar;
                Title.ParameterName = "TITLE";
                Title.Value = item.title.ToString();
                string sqlString = "EXEC InsertRelated " +
                    SubmittedByID.ToString() + ", " +
                    Convert.ToString(item.otherTrackingId) + ", " +
                    Convert.ToString(NewTrackingID) + ", " +
                    "@TITLE" + ";";
                SqlCommand Command = new SqlCommand(sqlString, conn);
                Command.Parameters.Add(Title);
                //Execute
                Command.ExecuteNonQuery();
                Command.Dispose();
                if (item.correlated)
                {//CORRELATE
                    SqlParameter CorrelatedTitle = new SqlParameter();
                    CorrelatedTitle.SqlDbType = SqlDbType.VarChar;
                    CorrelatedTitle.ParameterName = "TITLE";
                    CorrelatedTitle.Value = item.title.ToString();
                    string CorrelatedsqlString = "EXEC InsertRelated " +
                        SubmittedByID.ToString() + ", " +
                        Convert.ToString(NewTrackingID) + ", " +
                        Convert.ToString(item.otherTrackingId) + ", " +
                        "@TITLE" + ";";
                    SqlCommand CorrelatedCommand = new SqlCommand(CorrelatedsqlString, conn);
                    CorrelatedCommand.Parameters.Add(CorrelatedTitle);
                    //Execute
                    CorrelatedCommand.ExecuteNonQuery();
                    CorrelatedCommand.Dispose();
                }
            }
            conn.Close();
        }
        private void ProcessAttachments(int NewTrackigID, int stID)
        {
            HttpContext context = HttpContext.Current;
            this.Files = (List<SCRAttachments>)context.Session[this.HiddenHashCode];
            if (this.Files != null && this.File.Count > 0)
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["btracker"].ConnectionString);
                conn.Open();
                foreach (SCRAttachments item in this.Files)
                {

                    //Setting up the Byte value for the image
                    //byte[] uploadedFile = new byte[item.file.InputStream.Length];
                    //item.file.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                    byte[] uploadedFile = item.Data;
                    //creating the SQL parameter
                    SqlParameter fileParam = new SqlParameter();
                    fileParam.SqlDbType = SqlDbType.Image;
                    fileParam.ParameterName = "FILE";
                    fileParam.Value = uploadedFile;
                    SqlParameter fileDes = new SqlParameter();
                    fileDes.SqlDbType = SqlDbType.VarChar;
                    fileDes.ParameterName = "DESCRIPTION";
                    fileDes.Value = item.Description;

                    SqlParameter fileType = new SqlParameter();
                    fileType.SqlDbType = SqlDbType.Int;
                    fileType.ParameterName = "TYPE";
                    fileType.Value = 26;
                    //Create the string 
                    String sqlString =
                        "EXEC InsertAttachFiles " + Convert.ToString(stID) + "," + Convert.ToString(NewTrackigID) + ",'" +
                        System.IO.Path.GetFileName(item.file.FileName).Replace("'", "''") + "'," +
                        " @DESCRIPTION" + "," + item.file.InputStream.Length.ToString() + "," +
                        " @FILE" + "," + Convert.ToString(stID) + "," + "@TYPE";

                    SqlCommand Command = new SqlCommand(sqlString, conn);
                    Command.Parameters.Add(fileParam);
                    Command.Parameters.Add(fileDes);
                    Command.Parameters.Add(fileType);
                    //execute
                    Command.ExecuteNonQuery();
                    Command.Dispose();

                }
                conn.Close();
            }
        }
        //Mod the XML Fuile for the windows 8 browser
        private void UpdateXML()
        {
            List<String> NewestSCR = new List<String>();
            REATrackerDB _sql = new REATrackerDB();
            DataTable dt = _sql.ProcessCommand("SELECT TOP 5 ST_TRACK.TRACKING_ID, ST_TRACK.TITLE FROM ST_TRACK ORDER BY TRACKING_ID DESC;");
            foreach (System.Data.DataRow row in dt.Rows)
            {
                NewestSCR.Add("SCR #" + Convert.ToString(row[0]) + " : " + Convert.ToString(row[1]));
            }
            int i = 1;
            foreach (String text in NewestSCR)
            {
                XmlDocument doc = new XmlDocument();
                string path = HttpContext.Current.Server.MapPath("Content/contoso" + Convert.ToString(i) + ".xml");
                doc.Load(path);
                XmlNodeList textTag = doc.SelectNodes("title/visual/binding/text");
                foreach (XmlNode Nodes in textTag)
                {
                    XmlAttribute att = Nodes.Attributes["id"];
                    if (att != null)
                    {
                        att.Value = text;
                    }
                }
                i++;
            }

        }
        //  ==========================================================================
        //||    Classes                                                              ||  
        //  ==========================================================================
        public class RelatedREA
        {
            public int otherTrackingId { get; set; }
            public int thisSCRID { get; set; }
            public string product { get; set; }
            public string title { get; set; }
            public string issue { get; set; }
            public Boolean correlated { get; set; }
            public string Status { get; set; }
            public String Priority { get; set; }
            public String AssignTo { get; set; }
            public String PlannedRelease { get; set; }
            public bool remove { get; set; }
            //-----------------------------------------------------------------------------
            // Constructors                                                               ||
            //-----------------------------------------------------------------------------
            public RelatedREA()
            {
            }
            public RelatedREA(int SCRID, int TrackID, bool correlate)
            {
                this.otherTrackingId = TrackID;
                this.thisSCRID = SCRID;
                this.correlated = correlate;
                this.AssignTo = getAssignedTo(TrackID);

            }
            public bool getFields()
            {
                bool exists = false;
                string cmd =
                    "SELECT ST_TRACK.TITLE, ST_ISSUE_TYPE.NAME, ST_STATUS.NAME, ST_priority.NAME, ST_TRACK.VP_MAJOR, ST_TRACK.VP_MINOR, ST_TRACK.VP_BUILD, ST_TRACK.VP_MISC, ST_PRODUCT.NAME " +
                    "FROM ST_TRACK " +
                    "INNER JOIN ST_PRODUCT ON ST_TRACK.PRODUCT = ST_PRODUCT.PRODUCT_ID " +
                    "INNER JOIN ST_ISSUE_TYPE ON ST_TRACK.ISSUE_TYPE = ST_ISSUE_TYPE.ISSUE_ID " +
                    "INNER JOIN ST_priority ON ST_TRACK.priority = ST_priority.ID " +
                    "INNER JOIN ST_STATUS ON ST_TRACK.STATUS = ST_STATUS.STATUS_ID " +
                    "WHERE ST_TRACK.TRACKING_ID = " +
                    Convert.ToString(otherTrackingId);
                REATrackerDB sql = new REATrackerDB();
                sql.OpenConnection();
                DataTable dt = sql.ProcessCommand(cmd);
                if (dt.Rows.Count != 0)
                {
                    exists = true;
                    this.title = Convert.ToString(dt.Rows[0][0]);
                    this.issue = Convert.ToString(dt.Rows[0][1]);
                    this.Status = Convert.ToString(dt.Rows[0][2]);
                    this.Priority = Convert.ToString(dt.Rows[0][3]);
                    this.PlannedRelease = arrangeVersion(Convert.ToString(dt.Rows[0][4]), Convert.ToString(dt.Rows[0][5]), Convert.ToString(dt.Rows[0][6]), Convert.ToString(dt.Rows[0][7]));
                    this.product = Convert.ToString(dt.Rows[0][8]);
                }
                sql.CloseConnection();
                return exists;
            }
            private string getAssignedTo(int ID)
            {
                string fullname = "";
                string cmd = "SELECT ST_USERS.FIRST_NAME, ST_USERS.LAST_NAME FROM ST_USERS INNER JOIN ST_TRACK ON ST_TRACK.ASSIGNED_TO = ST_USERS.USER_ID " +
                    "WHERE ST_TRACK.TRACKING_ID = " + Convert.ToString(ID);
                REATrackerDB _sql = new REATrackerDB();
                _sql.OpenConnection();
                DataTable dt = _sql.ProcessCommand(cmd);
                if (dt.Rows.Count > 0)
                {
                    fullname = dt.Rows[0][0] + " " + dt.Rows[0][1];
                }
                _sql.CloseConnection();
                return fullname;
            }
            private string arrangeVersion(String major, String minor, String build, String misc)
            {
                string version = "";
                if (major != "")
                {
                    version += major;
                }
                else
                {
                    version += "?";
                }
                if (minor != "")
                {
                    version += ("." + minor);
                }
                else
                {
                    version += ".?";
                }
                if (build != "")
                {
                    version += ("." + build);
                }
                else
                {
                    version += ".?";
                }
                if (misc != "")
                {
                    version += (" " + misc);
                }
                return version;
            }
            public string getRemoveValue(int i)
            {
                return "RemoveRelated-" + i;
            }
        }

        public class SCRAttachments
        {
            public HttpPostedFileBase file { get; set; }
            public String Description { get; set; }
            public DateTime Datetime { get; set; }
            public bool remove { get; set; }
            public Byte[] Data { get; set; }
            //-----------------------------------------------------------------------------
            // Constructors                                                               ||
            //-----------------------------------------------------------------------------
            public SCRAttachments() { }
            public SCRAttachments(HttpPostedFileBase fileParam, String des)
            {
                this.file = fileParam;
                this.Description = des;
                MemoryStream target = new MemoryStream();
                this.file.InputStream.CopyTo(target);
                this.Data = target.ToArray();
            }
            public String getSize()
            {
                int size = file.ContentLength;
                string display = "";
                double displaySize;
                if (size < 1000)
                {
                    displaySize = size;
                    display = displaySize.ToString("F1") + " B";
                }
                else if (size < 1000000)
                {
                    displaySize = size / 1000;
                    display = displaySize.ToString("F1") + " KB";
                }
                else if (size < 1000000000)
                {
                    displaySize = size / 1000000;
                    display = displaySize.ToString("F1") + " MB";
                }
                return display;
            }
        }

        public class IntegerNoZeroAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                int inputInt = 0;
                if (value != null && int.TryParse(value.ToString(), out inputInt))
                {
                    if (inputInt > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public class IntegerNoNegativeAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                int inputInt = 0;
                if (value != null && int.TryParse(value.ToString(), out inputInt))
                {
                    if (inputInt >= 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public class IntegerNoNegativeNullableAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                int inputInt = 0;
                if (value == null || (value != null && int.TryParse(value.ToString(), out inputInt)))
                {
                    if (inputInt >= 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public class DoubleNoNegativeNullableAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                double inputDouble = 0.0;
                if (value == null || (value != null && double.TryParse(value.ToString(), out inputDouble)))
                {
                    if (inputDouble >= 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}