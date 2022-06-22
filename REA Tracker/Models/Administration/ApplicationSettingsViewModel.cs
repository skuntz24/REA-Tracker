using System;
using System.Collections.Generic;
using System.Data;
using QVICommonIntranet.Database;
using QVICommonIntranet.Email.EmailREATracker;
using QVICommonIntranet.Utility;
using System.ComponentModel.DataAnnotations;

namespace REA_Tracker.Models
{

    public class ApplicationSettingsViewModel
    {
       
        protected REATrackerDB _db = new REATrackerDB();
        public List<DataRow> Variables { get; set; }

        public ApplicationSettingsViewModel()
        {
        }

        public void Populate()
        {
            if (Variables != null)
            {
                Variables.Clear();
                Variables = null;

            }
            this.Variables = new List<DataRow>();

            DataTable dt = _db.ProcessCommand("SELECT * FROM REA_APPLICATION");
            if (dt.Rows.Count > 0)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    this.Variables.Add(dr);
                }
            }
        }
    }


    public class ApplicationSettingsEditViewModel
    {
        public int ID { get; set; }
        public String Variable { get; set; }
        public String Value { get; set; }
        public String Error { get; protected set; }

        public ApplicationSettingsEditViewModel()
        {

        }

        public ApplicationSettingsEditViewModel(int variableID)
        {
            init(variableID);
        }

        public void init(int variableID)
        {
            String command = $@"SELECT * FROM  REA_APPLICATION WHERE ID = {variableID}";

            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(command);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                ID = Convert.ToInt16(row["ID"]);
                Variable = Convert.ToString(row["Variable"]);
                Value = row["Value"] == DBNull.Value ? "" : Convert.ToString(row["Value"]);
            }
        }

        public bool Update()
        {
            REATrackerDB sql = new REATrackerDB();
            bool success = false;

            if (sql.SetParameter(this.ID, this.Value))
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
    }


    public class LockControlViewModel
    {
        public Boolean Lock { get; set; }
        public String Message { get; set; }

        public LockControlViewModel()
        {
            String Command = "SELECT VALUE FROM REA_APPLICATION WHERE VARIABLE ='LOCKED_STATE';";
            DataTable dt = new REATrackerDB().ProcessCommand(Command);
            this.Lock = Convert.ToBoolean(Convert.ToInt32(dt.Rows[0][0]));
            this.Message = Convert.ToString(System.Web.HttpContext.Current.Application["LockMessage"]);
        }

        public void LockUp()
        {
            String Command = "UPDATE REA_APPLICATION SET VALUE = 1 WHERE VARIABLE ='LOCKED_STATE';";
            new REATrackerDB().ProcessCommand(Command);
            System.Web.HttpContext.Current.Application.Lock();
            System.Web.HttpContext.Current.Application["LockMessage"] = this.Message;
            System.Web.HttpContext.Current.Application["LockStatus"] = true;
            System.Web.HttpContext.Current.Application.UnLock();
        }

        public void UnLock()
        {
            String Command = "UPDATE REA_APPLICATION SET VALUE = 0 WHERE VARIABLE ='LOCKED_STATE';";
            new REATrackerDB().ProcessCommand(Command);
            System.Web.HttpContext.Current.Application.Lock();
            System.Web.HttpContext.Current.Application["LockMessage"] = "REA Tracker is currently under maintenance.";
            System.Web.HttpContext.Current.Application["LockStatus"] = false;
            System.Web.HttpContext.Current.Application.UnLock();
        }
    }


    public class EmailSenderViewModel
    {
        public bool sendToAll { get; set; }
        public string messageRelay { get; set; }
        //------------------------------
        //----------Custom--------------
        //----------Contact-------------
        //------------------------------

        public string sendTo { get; set; }
        public string sendFromName { get; set; }
        public string sendFrom { get; set; }
        public string subject { get; set; }
        public string sendBody { get; set; }
        public string CustomMessage { get; set; }
        public string CustomSubject { get; set; }
        //------------------------------
        //-------------SCR--------------
        //------------------------------
        public int SendSCRID { get; set; }
        public int ChangeSCRID { get; set; }
        //------------------------------
        //-------------Build------------
        //------------------------------
        public int SendBuild { get; set; }

        //------------------------------
        //-------------Display----------
        //------------------------------
        public List<string> emails { get; set; }

        //------------------------------
        //-------------Admin------------
        //------------------------------
        public string adminAddress { get; set; }
        //------------Welcome Email-----
        public int newUserID { get; set; }
        public string newUserMessage { get; set; }
        public string newUserSubject { get; set; }
        public string logon { get; set; }
        public string password { get; set; }
       
        public int WatchUserID { get; set; }

      /// <summary>
      /// gets the list of users for the drop down list
      /// </summary>
      /// <returns></returns>
        public List<dynamic> GetListofUsers()
        {
            REATrackerDB model = new REATrackerDB();
            List<dynamic> list = new List<dynamic>();
            using (DataTable dt = model.GetUsers())
            {
                list.Add(new System.Dynamic.ExpandoObject());
                list[0].Name = "";
                list[0].ID = 0;

                int i = 1;
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new System.Dynamic.ExpandoObject());
                    list[i].Value = i;
                    list[i].ID = (int)row["USER_ID"];
                    list[i].Name = Convert.ToString(row["FIRST_NAME"]) + ' ' + Convert.ToString(row["LAST_NAME"]);
                    i++;
                }

                return list;
            }  
                
        }
        public EmailSenderViewModel()
        {
            this.sendToAll = false;
            string command = "SELECT VALUE FROM REA_APPLICATION WHERE VARIABLE = 'AdminEmail'";
            this.adminAddress =
                Convert.ToString(new REATrackerDB().ProcessScalarCommand(command));

        }

        public bool sendCustom()
        {
            EmailSendCustomMessage model = new EmailSendCustomMessage();
            model.EmailMessage = this.CustomMessage;
            model.ReceiverEmail = this.sendTo;
            model.SenderEmail = this.sendFrom;
            model.EmailSubject = this.CustomSubject;

            bool sent;
            if (this.sendToAll)
            {
                sent = model.SendToAll();
            }
            else
            {
                sent = model.Send();
            }
            return sent;
        }

        public bool sendWatchList()
        {
            EmailDailyWatchListUpdates model = new EmailDailyWatchListUpdates();
            bool value = model.Send();
            this.emails = model.SentEmails;
            return value;
        }
        public bool sendWatchList(int userID)
        {
            bool value = false;
            if (userID > 0)
            {
                EmailDailyWatchListUpdates model = new EmailDailyWatchListUpdates(userID);
                value = model.Send();
                this.emails = model.SentEmails;
            }
            return value;
        }

        public bool sendSubmitted()
        {
            EmailSubmittedNotification model = new EmailSubmittedNotification(this.SendSCRID);
            return model.Send();
        }

        public bool sendChanged()
        {
            EmailChangedAssignmentNotification model = new EmailChangedAssignmentNotification(this.ChangeSCRID);
            return model.Send();
        }

        public bool sendContactAdmin()
        {
            EmailContactAdmin model = new EmailContactAdmin(
                this.sendFromName, this.sendFrom, this.subject, this.sendBody
                );
            return model.Send();
        }

        public void editAdminEmail()
        {
            string command =
                "UPDATE REA_APPLICATION SET VALUE ='" + this.adminAddress.Replace("'", "''") +
                "' WHERE VARIABLE = 'AdminEmail';";
            new REATrackerDB().ProcessCommand(command);
        }
        public bool sendWelcomeEmail()
        {
            bool success = false;

            if (newUserID > 0)
            {
                   EmailWelcomeUser email = new EmailWelcomeUser(newUserID, this.sendFrom, this.newUserSubject, this.newUserMessage);
                    success = email.Send();
               
                    
               
            }
            return success;
        }
    }

}