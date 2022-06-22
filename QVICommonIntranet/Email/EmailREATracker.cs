using System;
using System.Data;
using QVICommonIntranet.Email;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QVICommonIntranet.Database;
using System.Net.Mail;

namespace QVICommonIntranet.Email.EmailREATracker
{
    /// <summary>
    /// Base class for all email being sent that requires data from REA Tracker
    /// </summary>
    public class EmailREATracker : Email
    {
        /// <summary>
        /// The ConnectionString must be set if any emails are going to work that require access to the REA Tracker database
        /// The client should only use the "LiveServerReadOnly" connection string, as emailing does not require any permissions
        /// to change the database
        /// </summary>
        static public string ConnectionString { get; set; }

        protected REATrackerDB _reatracker;
        protected string _copyright = @"&copy; " + @DateTime.Now.Year + " Quality Vision International, Inc. All rights reserved.";

        protected string _disclaimer = @"The information contained herein is based on the experience and knowledge relating to the subject matter gained by Quality Vision International, Inc. prior to publication.
<br><br>
No patent license is granted by this information.
<br><br>
Quality Vision International, Inc. reserves the right to change this information without notice and makes no warranty, express or implied, with respect to this information. Quality Vision International shall not be liable for any loss or damage, including consequential or special damages, resulting from the use of this information, even if loss or damage is caused by negligence or other fault on the part of Quality Vision International, Inc.
<br><br>
No part of this document may be reproduced or disclosed in any form or for any purpose, other than personal use, without the written permission of Quality Vision International, Inc.
";

        public EmailREATracker() : base()
        {
            OpenConnectionToREATracker();
        }

        protected void OpenConnectionToREATracker()
        {
            _reatracker = new REATrackerDB(ConnectionString);
        }

        protected void CloseConnectionToREATracker()
        {
            _reatracker.CloseConnection();
            _reatracker = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Free any other managed objects here.
                //
                CloseConnectionToREATracker();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// This function creatures a hyperlink to an REA that can be used in the emails
        /// </summary>
        /// <param name="trackingID">Valid REA</param>
        /// <param name="title">The title of the REA</param>
        /// <returns>HTML code for creating the hypelink</returns>
        protected string CreateLink(string trackingID, string title)
        {
            string html = "<a href='http://reatracker.qvii.com/REA/Display/" + trackingID + "'> " + title + " </a>";
            return html;
        }
        protected string CreateBuildReportLink(string buildID, string title)
        {
            string html = "<a href='http://reatracker.qvii.com/Home/DashBoardReport/" + buildID + "'> " + title + " </a>";
            return html;
        }
        protected string CreateCodeReviewLink(string codeReviewID, string title)
        {
            string html = "<a href='http://reatracker.qvii.com/CodeReview/Display/" + codeReviewID + "'> " + title + " </a>";
            return html;
        }
    }


    /// <summary>
    /// This email is sent when a new REA has been submitted
    /// </summary>
    public class EmailSubmittedNotification : EmailREATracker
    {
        public int TRACKING_ID { get; set; } = 0;

        public EmailSubmittedNotification(int trackingID) : base()
        {
            TRACKING_ID = trackingID;
        }

        public override bool Send()
        {
            bool sent = false;

            if (_reatracker.IsValidREA(TRACKING_ID))
            {
                using (DataTable dt = _reatracker.GetSubmittedREA(TRACKING_ID))
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        AddEmailAddresses(dr["SUBMITTED_EMAIL"].ToString());

                        string REA_TITLE = $"REA#{dr["TRACKING_ID"].ToString()} - {dr["TITLE"].ToString()}";
                        Subject = $"[REA Tracker] [{dr["PRODUCT"].ToString()}] [SUBMITTED] {REA_TITLE}";
                        HtmlTitle = REA_TITLE.Replace("<", "&lt;").Replace(">", "&gt;");
                        HtmlBody = @"
<table class='viewportWrapper' cellpadding='0' cellspacing='0' align='left' style='padding:3px' width=100%>
    <thead >
    <tr>
<td style= 'background-color: #FFFFFF; border:none' align=left colspan=5 class='text-primary'><h2>" + dr["PRODUCT"].ToString() + @"</h2></td>
</tr>
        <tr class='header'>
            <th style='border: 1px solid #ddd'; valign='center' ;align='left'>Details</th>
            <th style='border: 1px solid #ddd'; valign='center' ;align='center' class='qvi-col-priority'>Priority</th>
            <th style='border: 1px solid #ddd'; valign='center' ;align='center' class='qvi-col-type'>Type</th>
            <th style='border: 1px solid #ddd'; valign='center' align='center'; class='qvi-col-status'>Submitted By</th>
            <th style='border: 1px solid #ddd'; valign='center' align='center';class='qvi-col-version'>Version Found</th>
        </tr>
    </thead>
    <tbody>";
                        HtmlBody += @"
        <tr>
            <td style='border-left: 1px solid #ddd; border-top : 1px solid #ddd; border-bottom: 1px solid #ddd; padding:8px' valign='top'>
</br>
                <b>Title</b><br>
                <b>" + CreateLink(dr["TRACKING_ID"].ToString(), REA_TITLE) + @"</b><br><br>
	            <b>Initial Description</b><br>" + dr["DESCRIPTION"].ToString().Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "</br>") + @"
            </td>
            <td style='border:1px solid #ddd'; valign='top'; align='center'>" + dr["PRIORITY"].ToString() + @"</td>
            <td style='border:1px solid #ddd'; valign='top'; align='center'>" + dr["ISSUE_TYPE"].ToString() + @"</td>
            <td style='border:1px solid #ddd'; valign='top'; align='center'>" + dr["SUBMITTED_BY"].ToString() + @"</td>
            <td style='border:1px solid #ddd'; valign='top'; align='center'>" + dr["VERSION_FOUND"].ToString() + @"</td>
        </tr>
    </tbody>
</table>";
                        int priority = 0;
                        if (int.TryParse(dr["PRIORITY_ID"].ToString(), out priority))
                        {
                            switch (priority)
                            {
                                case 1: { _mail.Priority = MailPriority.Low; break; }
                                case 3: { _mail.Priority = MailPriority.High; break; }
                                case 4: { _mail.Priority = MailPriority.High; break; }
                                default: { _mail.Priority = MailPriority.Normal; break; }
                            }
                        }
                        sent = base.Send();
                    }
                }
            }
            return sent;
        }
    }

    /// <summary>
    /// This email is sent when the assign to field has been changed. In the status has been changed to DELIVERED
    /// than the EmailDeliveredNotification should be sent instead.
    /// </summary>
    public class EmailChangedAssignmentNotification : EmailREATracker
    {
        public int TRACKING_ID { get; set; } = 0;

        public EmailChangedAssignmentNotification(int trackingID) : base()
        {
            TRACKING_ID = trackingID;
        }

        public override bool Send()
        {
            bool sent = false;

            if (_reatracker.IsValidREA(TRACKING_ID))
            {
                using (DataTable dt = _reatracker.GetSubmittedREA(TRACKING_ID))
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        AddEmailAddresses(dr["ASSIGNED_TO_EMAIL"].ToString());

                        string REA_TITLE = $"REA#{dr["TRACKING_ID"].ToString()} - {dr["TITLE"].ToString()}";
                        //string lastnotes = dr["LAST_NOTES"].ToString(); remove?

                        string version = dr["VERSION_RESOLVED"].ToString();

                        Subject = $"[REA Tracker] [{dr["PRODUCT"].ToString()}] [ASSIGNED] {REA_TITLE}";
                        HtmlTitle = REA_TITLE.Replace("<", "&lt;").Replace(">", "&gt;");
                        HtmlBody = @"
<table class='viewportWrapper' cellpadding='0' cellspacing='0' align='left' style='padding:3px' width=100%>
                    <thead>
                    <tr>
<td style= 'background-color: #FFFFFF; border:none' class='text-primary' align=left colspan=5><h2>" + dr["PRODUCT"].ToString() + @"</h2></td>
</tr>

<tr class='header'>
        <th style='border: 1px solid #ddd; padding:3px'; align='left'>Details</th>
        <th style='border: 1px solid #ddd;  align='center' class='qvi-col-priority'>Priority</th>
        <th style='border: 1px solid #ddd; align='center' class='qvi-col-type'>Type</th>
        <th style='border: 1px solid #ddd; align='center' class='qvi-col-status'>Submitted By</th>
        <th style='border: 1px solid #ddd; valign='center' align='center' class='qvi-col-version'>Version Found</th>
        </tr>
    </thead>
    <tbody>
                       
        <tr>
	    <td style='border-left: 1px solid #ddd; border-top : 1px solid #ddd; border-bottom: 1px solid #ddd; padding:8px' valign='top'>
            <b>Title</b><br>
            <b>" + CreateLink(dr["TRACKING_ID"].ToString(), REA_TITLE) + @"</b><br><br>
            <b>Targeted Release</b><br>" + version + @"<br><br>
		    <b>Initial Description</b><br>" + dt.Rows[0]["DESCRIPTION"].ToString().Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "</br>") + @"
	    </td>
        <td style='border: 1px solid #ddd'; valign='top'; align='center'>" + dr["PRIORITY"].ToString() + @"</td>
        <td style='border: 1px solid #ddd'; valign='top';align='center'>" + dr["ISSUE_TYPE"].ToString() + @"</td>
        <td style='border: 1px solid #ddd'; valign='top'; align='center'>" + dr["SUBMITTED_BY"].ToString() + @"</td>
        <td style='border: 1px solid #ddd'; valign='top'; align='center'>" + dr["VERSION_FOUND"].ToString() + @"</td>
        </tr>
    </tbody>
</table>";

                        sent = base.Send();
                    }
                }
            }
            return sent;
        }
    }

    /// <summary>
    /// This email is sent when an REA has been kicked back, i.e the status is changed to IN PROCESS from BUILT/TESTING/ON HOLD/CLOSED state
    /// </summary>
    public class EmailREAKickback : EmailREATracker
    {
        public int TRACKING_ID { get; set; } = 0;
        public int? PID { get; set; } = 0;


        public EmailREAKickback(int trackingID, int? pid) : base()
        {
            TRACKING_ID = trackingID;
            PID = pid;
        }

        public override bool Send()
        {
            bool sent = false;
            REATrackerDB sql = new REATrackerDB();
            string email = "SELECT EMAIL, OWNER FROM ST_PRODUCT INNER JOIN ST_USERS ON OWNER = USER_ID  WHERE PRODUCT_ID=" + PID;
            //no kickback email is sent if owner is not set     
            using (DataTable dat = sql.ProcessCommand(email))
            {
                if (dat.Rows.Count > 0)
                {


                    AddEmailAddresses(Convert.ToString(dat.Rows[0]["EMAIL"]));
                    if (_reatracker.IsValidREA(TRACKING_ID))
                    {
                        using (DataTable dt = _reatracker.GetSubmittedREA(TRACKING_ID))
                        {
                            if (dt.Rows.Count > 0)
                            {
                                DataRow dr = dt.Rows[0];
                                string REA_TITLE = $"REA#{dr["TRACKING_ID"].ToString()} - {dr["TITLE"].ToString()}";
                                string lastnotes = dr["LAST_NOTES"].ToString();
                                string version = dr["VERSION_RESOLVED"].ToString();
                                Subject = $"[REA Tracker] [{dr["PRODUCT"].ToString()}] [KICKBACK] {REA_TITLE}";
                                HtmlTitle = REA_TITLE.Replace("<", "&lt;").Replace(">", "&gt;");
                                HtmlBody = @"
            <table class='viewportWrapper' cellpadding='0' cellspacing='0' align='left' style='padding:8px' width=100%>
                             <thead >
                             <tr>
            <td style= 'background-color: #FFFFFF; border:none' class='text-primary' align=left colspan=5><h2>" + dr["PRODUCT"].ToString() + @"</h2></td>
             </tr>

                    <tr class='header'>
                    <th style='border: 1px solid #ddd; padding:3px'; align='left'>Details</th>
                    <th style='border: 1px solid #ddd; padding:3px';align='center' class='qvi-col-priority'>Priority</th>
                    <th style='border: 1px solid #ddd; padding:3px';align='center' class='qvi-col-type'>Type</th>
                    <th style='border: 1px solid #ddd; padding:3px';align='center' class='qvi-col-status'>Submitted By</th>
                    <th style='border: 1px solid #ddd; padding:3px';align='center' class='qvi-col-version'>Version Found</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
            	    <td style='border-left: 1px solid #ddd; border-top : 1px solid #ddd; border-bottom: 1px solid #ddd; padding:8px' valign='top' >
                        <b>Title</b><br>
                        <b>" + CreateLink(dr["TRACKING_ID"].ToString(), REA_TITLE) + @"</b><br><br>
                        <b>Latest Notes</b><br>" + lastnotes + @"<br><br>
                        <b>Targeted Release</b><br>" + version + @"<br><br>
            		    <b>Initial Description</b><br>" + dt.Rows[0]["DESCRIPTION"].ToString().Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "</br>") + @"
            	    </td>
                    <td style='border: 1px solid #ddd'; valign='top'; align='center'>" + dr["PRIORITY"].ToString() + @"</td>
                    <td style='border: 1px solid #ddd'; valign='top'; align='center'>" + dr["ISSUE_TYPE"].ToString() + @"</td>
                    <td style='border: 1px solid #ddd'; valign='top'; align='center'>" + dr["SUBMITTED_BY"].ToString() + @"</td>
                    <td style='border: 1px solid #ddd'; valign='top'; align='center'>" + dr["VERSION_FOUND"].ToString() + @"</td>
                    </tr>
                </tbody>
            </table>";
                                sent = base.Send();
                            }
                        }

                    }
                }
            }
            return sent;
        }

    }


    /// <summary>
    /// Same output body as EmailChangedAssignmentNotification, the subject says "has been delivered."
    /// Also has the latest notes
    /// </summary>
    public class EmailDeliveredNotification : EmailREATracker
    {
        public int TRACKING_ID { get; set; } = 0;

        public EmailDeliveredNotification(int trackingID) : base()
        {
            TRACKING_ID = trackingID;
        }

        public override bool Send()
        {
            bool sent = false;

            if (_reatracker.IsValidREA(TRACKING_ID))
            {
                using (DataTable dt = _reatracker.GetSubmittedREA(TRACKING_ID))
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        AddEmailAddresses(dr["ASSIGNED_TO_EMAIL"].ToString());

                        string REA_TITLE = $"REA#{dr["TRACKING_ID"].ToString()} - {dr["TITLE"].ToString()}";
                        string lastnotes = dr["LAST_NOTES"].ToString();

                        string version = dr["VERSION_RESOLVED"].ToString();

                        Subject = $"[REA Tracker] [{dr["PRODUCT"].ToString()}] [DELIVERED] {REA_TITLE}";
                        HtmlTitle = REA_TITLE.Replace("<", "&lt;").Replace(">", "&gt;");
                        HtmlBody = @"
<table class='viewportWrapper' cellpadding='0' cellspacing='0' align='left' style='padding:8px' width=100%>
                 <thead >
                 <tr>
<td style= 'background-color: #FFFFFF; border:none' class='text-primary' align=left colspan=5><h2>" + dr["PRODUCT"].ToString() + @"</h2></td>
 </tr>

        <tr class='header'>
        <th style='border: 1px solid #ddd; padding:3px'; align='left'>Details</th>
        <th style='border: 1px solid #ddd; padding:3px';align='center' class='qvi-col-priority'>Priority</th>
        <th style='border: 1px solid #ddd; padding:3px';align='center' class='qvi-col-type'>Type</th>
        <th style='border: 1px solid #ddd; padding:3px';align='center' class='qvi-col-status'>Submitted By</th>
        <th style='border: 1px solid #ddd; padding:3px';align='center' class='qvi-col-version'>Version Found</th>
        </tr>
    </thead>
    <tbody>
        <tr>
	    <td style='border-left: 1px solid #ddd; border-top : 1px solid #ddd; border-bottom: 1px solid #ddd; padding:8px' valign='top' >
            <b>Title</b><br>
            <b>" + CreateLink(dr["TRACKING_ID"].ToString(), REA_TITLE) + @"</b><br><br>
            <b>Latest Notes</b><br>" + lastnotes + @"<br><br>
            <b>Targeted Release</b><br>" + version + @"<br><br>
		    <b>Initial Description</b><br>" + dt.Rows[0]["DESCRIPTION"].ToString().Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "</br>") + @"
	    </td>
        <td style='border: 1px solid #ddd'; valign='top'; align='center'>" + dr["PRIORITY"].ToString() + @"</td>
        <td style='border: 1px solid #ddd'; valign='top'; align='center'>" + dr["ISSUE_TYPE"].ToString() + @"</td>
        <td style='border: 1px solid #ddd'; valign='top'; align='center'>" + dr["SUBMITTED_BY"].ToString() + @"</td>
        <td style='border: 1px solid #ddd'; valign='top'; align='center'>" + dr["VERSION_FOUND"].ToString() + @"</td>
        </tr>
    </tbody>
</table>";
                        sent = base.Send();
                    }
                }
            }
            return sent;
        }

    }

    public class EmailBuildVerificationReport : EmailREATracker
    {
        private int BUILD_ID;

        public EmailBuildVerificationReport(int buildID)
        {
            BUILD_ID = buildID;
        }

        public override bool Send()
        {
            bool sent = false;

            using (DataTable dt = _reatracker.GetBuildRecord(BUILD_ID))
            {
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    //TODO: _mail.To.Add(dt.Rows[0]["DISTRIBUTION_LIST"].ToString());// Parse it, its not sperating it out
                    AddEmailAddresses(dr["DISTRIBUTION_LIST"].ToString());

                    Subject = "[REA Tracker] " + dr["NAME"].ToString() + " " + dr["BUILD_VERSION"].ToString() + " has been built.";

                    HtmlTitle = dr["NAME"].ToString() + " " + dr["BUILD_VERSION"].ToString() + " Build Verification Report";

                    HtmlBody = @"
                    <table class='viewportWrapper' cellpadding='0' cellspacing='0' align=left style='clear:both; margin-bottom=50px' width=100%>
                    <thead>
                    <tr>
				   <td style= 'background-color: #FFFFFF; border:none' class='text-primary' colspan=5 align=left ><h2>" + dr["NAME"].ToString() + @" Build Verification Report</h2></td>
                  </tr>
                    </thead>
                    <tbody>
                      <tr><td style='border:1px solid #ddd;padding:8px'; align='left'>Date</td><td style='border:1px solid #ddd;padding:8px'; align='left'>" + dr["BUILT_ON"].ToString() + @"</td></tr>
					  <tr><td style='border:1px solid #ddd;padding:8px' ;align='left'>Release&nbsp;Coordinator</td><td style='border:1px solid #ddd;padding:8px'; align='left'>" + dr["RELEASE_COORDINATOR"].ToString() + @"</td></tr>
					  <tr><td style='border:1px solid #ddd;padding:8px'; align='left'>Version</td><td style='border:1px solid #ddd;padding:8px'; align='left'>" + dr["BUILD_VERSION"].ToString() + @"</td></tr>
					  <tr><td style='border:1px solid #ddd;padding:8px'; align='left'>Copyright</td><td style='border:1px solid #ddd;padding:8px'; align='left'>" + _copyright + @"</td></tr>
					  <tr><td style='border:1px solid #ddd;padding:8px'; align='left'>Disclaimer</td><td style='border:1px solid #ddd; padding:8px'; align='left'>" + _disclaimer + @"</td></tr>
					  <tr><td style='border:1px solid #ddd;padding:8px'; align='left'>Additional&nbsp;Components</td><td style='border:1px solid #ddd'; align='left'>@@BUILD_COMPONENTS@@</td></tr>
					  <tr><td style='border:1px solid #ddd;padding:8px'; valign='top' align='left'>Notes</td><td style='border:1px solid #ddd'; valign='top' align='left'>" + dr["NOTES"].ToString().Replace("\n", "<br/>") + @"</td></tr>
                    </tbody>
                  </table>";


                    //List all of the related builds, making the names a link to the other reports
                    String strRelatedBuilds = "";
                    using (DataTable dtRelatedBuilds = _reatracker.GetRelatedBuilds(BUILD_ID))
                    {
                        if (dtRelatedBuilds.Rows.Count > 0)
                        {
                            foreach (DataRow drBuild in dtRelatedBuilds.Rows)
                            {
                                strRelatedBuilds += CreateBuildReportLink(drBuild["BUILD_ID"].ToString(), drBuild["NAME"].ToString() + " " + drBuild["BUILD_VERSION"].ToString()) + "<br/>";
                            }
                        }
                    }
                    HtmlBody = HtmlBody.Replace("@@BUILD_COMPONENTS@@", strRelatedBuilds);
                    //Build componets are in a different table its not that its this^
                    HtmlBody = HtmlBody.Replace("@@NOTES@@", dr["NOTES"].ToString().Replace("\n", "<br/>"));

                    HtmlBody += @"
                    <table class='viewportWrapper' cellpadding='0' cellspacing='0' align=left style='clear:both' width=100%>
                    <thead>
                    <tr>
                    </br> <td style= 'background-color: #FFFFFF; border:none' class='text-primary' align=left colspan=5><h2>Summary of the changes</h2></td>
                    </tr>
                            <tr class='header' align=left>
                                <th style='border:1px solid #ddd'; align='left'>Title</th>
                                <th style='border:1px solid #ddd'; align='center' class='qvi-col-priority'>Priority</th>
                                <th style='border:1px solid #ddd'; align='center' class='qvi-col-type'>Type</th>
                                <th style='border:1px solid #ddd'; align='center' class='qvi-col-status'>Customer</th>
                                <th style='border:1px solid #ddd'; align='center' class='qvi-col-version'>Version Found</th>
                            </tr>
                            </thead>
                        <tbody>";

                    using (DataTable dtREAs = _reatracker.GetBuildREAs(dr["REA_LIST"].ToString()))
                    {
                        if (dtREAs.Rows.Count > 0)
                        {
                            string reas = "";
                            string row_text = @"
                            <tr>
                                <td style='border: 1px solid #ddd' ; align='left'><b>@@LINK@@</b><br><br>
                                <b>Initial Description</b><br>
                                @@REA_DESCRIPTION@@
                                <br><br>
                                <b>Resolution</b><br>
                                @@REA_RESOLUTION@@
                                <br><br>
                                </td>
                                <td style='border:1px solid #ddd; padding:8px';  valign='top';  align='center'>@@REA_priority@@</td>
                                <td style='border:1px solid #ddd; padding:8px';  valign='top';  align='center'>@@REA_TYPE@@</td>
                                <td style='border:1px solid #ddd; padding:8px';  valign='top';  align='center'>@@REA_CUSTOMER@@</td>
                                <td style='border:1px solid #ddd; padding:8px';  valign='top';  align='center'>@@REA_FOUND_VERSION@@</td>
                            </tr>";

                            foreach (DataRow drREA in dtREAs.Rows)
                            {
                                string REA_TITLE = "REA#" + drREA["TRACKING_ID"].ToString() + " - " + drREA["TITLE"].ToString().Replace("<", "&lt;").Replace(">", "&gt;");
                                reas = row_text;
                                reas = reas.Replace("@@LINK@@", CreateLink(drREA["TRACKING_ID"].ToString(), REA_TITLE));
                                //reas = reas.Replace("@@TRACKING_ID@@", dr["TRACKING_ID"].ToString());
                                //reas = reas.Replace("@@TITLE@@", Convert.ToString(dr["TITLE"]).Replace("<", "&lt;").Replace(">", "&gt;"));
                                reas = reas.Replace("@@REA_priority@@", drREA["priority"].ToString());
                                reas = reas.Replace("@@REA_TYPE@@", drREA["ISSUE_TYPE"].ToString());
                                reas = reas.Replace("@@REA_CUSTOMER@@", drREA["CUSTOMER_NAME"].ToString());
                                reas = reas.Replace("@@REA_FOUND_VERSION@@", drREA["VERSION_FOUND"].ToString());

                                reas = reas.Replace("@@REA_DESCRIPTION@@", Convert.ToString(drREA["DESCRIPTION"]).Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "<br/>"));
                                reas = reas.Replace("@@REA_RESOLUTION@@", Convert.ToString(drREA["RESOLUTION"]).Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "<br/>"));

                                HtmlBody += reas;
                            }
                        }

                        string temp = @"
                        </tbody>
                        <tfoot>
                            <tr><td style='border:1px solid #ddd; padding:8px;' ; valign='top' colspan='5'>Total Records found: <strong>@@BUILD_REAS_COUNT@@</strong></td></tr>
                        </tfoot>
                        </table>";
                        HtmlBody += temp.Replace("@@BUILD_REAS_COUNT@@", dtREAs.Rows.Count.ToString());
                    }// using dtREAs
                    sent = base.Send();
                }//rows
            }//build
            return sent;
        }
    }

    public class EmailDailyWatchListUpdates : EmailREATracker
    {
        //Make a timer in a Windows Service project or figure out how to time it in the global asp
        public List<string> SentEmails { get; set; } = new List<string>();

        public DateTime watchListDate { get; protected set; }
        protected int? userID { get; set; } = null;

        public EmailDailyWatchListUpdates()
        {
            //default the date to yesterday
            watchListDate = DateTime.Now.AddDays(-1);
        }

        public EmailDailyWatchListUpdates(DateTime dtWatchList)
        {
            watchListDate = dtWatchList;
        }

        public EmailDailyWatchListUpdates(int userID, DateTime dtWatchList)
        {
            this.userID = userID;
            watchListDate = dtWatchList;
        }
        public EmailDailyWatchListUpdates(int userID)
        {
            this.userID = userID;
            watchListDate = DateTime.Now.AddDays(-1);
        }

        public override bool Send()
        {
            bool sent = false;

            //gets all the active users that are watching something
            using (DataTable dtUsers = _reatracker.GetUsersWatching(userID))
            {
                foreach (DataRow user in dtUsers.Rows)
                {
                    //clear any email information
                    base.Clear();

                    //this is constant for each email sent
                    Subject = "[REA Tracker] Watch List Update for " + watchListDate.ToShortDateString();

                    using (DataTable dtREAList = _reatracker.GetUserWatchList((int)user["USER_ID"], watchListDate))
                    {
                        if (dtREAList.Rows.Count > 0)
                        {
                            //track APPROX how many emails set, some accounts have multiple emails tied to an account
                            //so those will be just counted as 1, instead of N
                            SentEmails.Add(user["EMAIL"].ToString());

                            AddEmailAddresses(user["EMAIL"].ToString());

                            string current_product = "NONE";
                            string current_REA = "NONE";
                            string REA_TITLE = "";
                            string content = "</br>";
                            int count = 0;
                            //super table, everything goes in this one table
                            content += @"
                            <table class='viewportWrapper' cellpadding='0' cellspacing='0' align='left' style='padding:3px;' width=100%>
                                <thead>
                                    <tr>
                                        <td style= 'background-color: #FFFFFF; border:none' align=left colspan=5>
                                        <h2 class='text-info'>Watch List Update for " + watchListDate.ToShortDateString() + @"</h2><hr>
                                        </td>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>";
                            foreach (System.Data.DataRow rea in dtREAList.Rows)
                            {
                                if (current_product == "NONE")
                                {
                                    //first time through, set the product start a new table
                                    current_product = rea["PRODUCT"].ToString();
                                    content += @"
                                    <h3 class='text-primary'>" + current_product + @"</h3>
                                    <table class='table table-bordered table-hover'>
                                        <thead>
                                            <tr class='header'>
                                                <th align='left'>Details</th>
                                            </tr>
                                        </thead>
                                        <tbody>";
                                }
                                else if (current_product != rea["PRODUCT"].ToString())
                                {
                                    //product has change from the last row
                                    //end the current REA row
                                    content += "</ul></td></tr>\r\n";

                                    //end the current table
                                    content += @"
                                        </tbody>
                                        <tfoot>
                                            <tr><td colspan='1'>Total Records found: " + count + @"</td></tr>
                                        </tfoot>
                                    </table>";

                                    // set the new product, start new table
                                    count = 0;
                                    content += "\r\n";
                                    current_product = rea["PRODUCT"].ToString();
                                    content += @"
                                    <h3 class='text-primary'>" + current_product + @"</h3>
                                    <table class='table table-bordered table-hover'>
                                        <thead>
                                            <tr class='header'>
                                                <th align='left'>Details</th>
                                            </tr>
                                        </thead>
                                        <tbody>";

                                    //set up for new REA for the table section
                                    current_REA = "NONE";
                                }

                                if (current_REA == "NONE")
                                {
                                    current_REA = rea["TRACKING_ID"].ToString();
                                    //first time through, set the REA for a new row
                                    REA_TITLE = "REA#" + rea["TRACKING_ID"].ToString() + " - " + rea["TITLE"].ToString().Replace("<", "&lt;").Replace(">", "&gt;");
                                    content += @"
                                            <tr>
                                                <td align='left'>
                                                <b>Title</b><br>
                                                <b>" + CreateLink(rea["TRACKING_ID"].ToString(), REA_TITLE) + @"</b><br><br>
                                                <b>Changes</b><br>
                                                <ul>";
                                    count++;

                                }
                                else if (current_REA != rea["TRACKING_ID"].ToString())
                                {
                                    //end the current REA
                                    content += "</ul></td></tr>\r\n";

                                    //start the new REA row
                                    current_REA = rea["TRACKING_ID"].ToString();
                                    REA_TITLE = "REA#" + rea["TRACKING_ID"].ToString() + " - " + rea["TITLE"].ToString().Replace("<", "&lt;").Replace(">", "&gt;");
                                    content += @"
                                            <tr>
                                                <td align='left'>
                                                <b>Title</b><br>
                                                <b>" + CreateLink(rea["TRACKING_ID"].ToString(), REA_TITLE) + @"</b><br><br>
                                                <b>Changes</b><br>
                                                <ul>";
                                    count++;
                                }
                                content += "<li>" +
#if DEBUG
                                           "[" + rea["TYPE"].ToString() + "]" +
#endif
                                            Convert.ToString(rea["DESCRIPTION"]).Replace("\n", "<br/>") + "</li>\r\n";

                            }//for each REA

                            //need to finish the last REA
                            content += "</ul></td></tr>\r\n";

                            //need to finish the last table
                            content += @"
                                        </tbody>
                                        <tfoot>
                                            <tr><td colspan='1'>Total Records found: " + count + @"</td></tr>
                                        </tfoot>
                                    </table>";

                            //need to finish the super table
                            content += @"                                
                                    </td>
                                </tr>
                            </tbody>
                            </table>";
                            HtmlBody = content;
                            sent = base.Send();

                            System.Diagnostics.Debug.WriteLine("Email to " + user["EMAIL"].ToString() + " was " + sent.ToString());
                        }//have reas
                        else
                        {
                            sent = true;
                            System.Diagnostics.Debug.WriteLine("No REAs have changed for user " + user["EMAIL"].ToString());
                        }
                    }
                }//Each User
            }
            return sent;
        }
    }



    /// <summary>
    /// This email is sent when the product type of an REA has been changed.
    /// </summary>
    public class EmailREAProductChange : EmailREATracker
    {
        public int TRACKING_ID { get; set; } = 0;
        public int PREVIOUS_PRODUCT_ID { get; set; } = 0;

        public EmailREAProductChange(int trackingID, int productID) : base()
        {
            TRACKING_ID = trackingID;
            PREVIOUS_PRODUCT_ID = productID;
        }

        public override bool Send()
        {
            bool sent = false;

            if (_reatracker.IsValidREA(TRACKING_ID))
            {
                using (DataTable dt = _reatracker.GetSubmittedREA(TRACKING_ID, PREVIOUS_PRODUCT_ID))
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];

                        string REA_TITLE = "REA#" + dr["TRACKING_ID"].ToString() + " - " + dr["TITLE"].ToString().Replace("<", "&lt;").Replace(">", "&gt;");

                        AddEmailAddresses(dr["SUBMITTED_EMAIL"].ToString());
                        Subject = "[REA Tracker] REA#" + TRACKING_ID.ToString() + " has changed products from " + dr["OLD_PRODUCT_NAME"].ToString() + " to " + dr["PRODUCT"].ToString() + ".";
                        HtmlTitle = REA_TITLE;
                        HtmlBody = @"
<table class='viewportWrapper' align=left style='padding:3px' width=100%>
                    <thead>
                     <tr>
<td style= 'background-color: #FFFFFF; border:none' class='text-primary' align=left colspan=5><h2>" + dr["PRODUCT"].ToString() + @"</h2></td>
   </tr>
        <tr class='header'>
            <th style='border: 1px solid #ddd; padding:3px'; align='left'>Details</th>
            <th style='border: 1px solid #ddd; padding:3px'; align='center' class='qvi-col-priority'>Priority</th>
            <th style='border: 1px solid #ddd; padding:3px'; align='center' class='qvi-col-type'>Type</th>
            <th style='border: 1px solid #ddd; padding:3px'; align='center' class='qvi-col-status'>Submitted By</th>
            <th style='border: 1px solid #ddd; padding:3px'; align='center' class='qvi-col-version'>Version Found</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td align='left' style='border-bottom: 1px solid #ddd; padding:3px; border-left: 1px solid #ddd'>
            <b>Title</b><br>
            <b>" + CreateLink(dr["TRACKING_ID"].ToString(), REA_TITLE) + @"</b><br><br>
            <b>Latest Notes</b><br>" + dr["LAST_NOTES"].ToString() + @"<br><br>
            <b>Initial Description</b><br>" + dr["DESCRIPTION"].ToString().Replace("<", "&lt;").Replace(">", "&gt;") + @"
            </td>
            <td style='border: 1px solid #ddd; padding:3px' ; valign=top; align='center'>" + dr["PRIORITY"].ToString() + @"</td>
            <td style='border: 1px solid #ddd; padding:3px' ; valign=top; align='center'>" + dr["ISSUE_TYPE"].ToString() + @"</td>
            <td style='border: 1px solid #ddd; padding:3px'; valign=top; align='center'>" + dr["SUBMITTED_BY"].ToString() + @"</td>
            <td style='border: 1px solid #ddd; padding:3px'; valign=top; align='center'>" + dr["VERSION_FOUND"].ToString() + @"</td>
        </tr>
    </tbody>
</table>";
                        sent = base.Send();
                    }
                }
            }
            return sent;
        }
    }

    /// <summary>
    /// This email is sent by a user via the help page to the administrators
    /// </summary>
    public class EmailContactAdmin : EmailREATracker
    {
        public string EmailSenderName { get; set; }
        public string EmailSenderAddress { get; set; }
        public string EmailSubject { get; set; }
        public string EmailMessage { get; set; }

        public EmailContactAdmin(
            string argsSender, string argsSenderEmail,
            string argsSubject, string argsmessage
            )
        {
            this.EmailSenderName = argsSender;
            this.EmailSenderAddress = argsSenderEmail;
            this.EmailSubject = argsSubject;
            this.EmailMessage = argsmessage.Replace("\n", "<br/>");
        }

        public override bool Send()
        {
            bool sent = false;

            using (DataTable dt = _reatracker.ProcessCommand("SELECT VALUE FROM ST_APPLICATION WHERE VARIABLE ='AdminEmail'"))
            {
                if (dt.Rows.Count > 0)
                {
                    AddEmailAddresses(dt.Rows[0]["VALUE"].ToString());
                }
                else
                {
                    AddEmailAddresses(_contactAddress);
                }
            }


            //this._mail.CC.Add(this.SenderEmail);
            Subject = "[REA Tracker] " + EmailSubject;
            HtmlBody = @"
<h2 class='text-primary'>User Request</h2>
<p>
" + EmailMessage + @"
<p>
<hr>
" + EmailSenderName + @"<br>" +
EmailSenderAddress;

            sent = base.Send();

            return sent;
        }

    }

    public class EmailWelcomeUser : EmailREATracker
    {
        public string SenderEmail { get; set; }
        public int User { get; set; }
        public string emailSubject { get; set; }
        public string emailMessage { get; set; }

        public EmailWelcomeUser(int argsUserID,
             string argsSenderEmail, string argsUserSubject, string argsUserMessage)
        {
            this.User = argsUserID;
            this.SenderEmail = argsSenderEmail;
            this.emailSubject = argsUserSubject;
            this.emailMessage = argsUserMessage;
        }
        public EmailWelcomeUser()
        {

        }

        public override bool Send()
        {

            bool sent = false;
            string command = "SELECT FIRST_NAME, EMAIL, logon, password FROM ST_USERS WHERE USER_ID=" + this.User;
            using (DataTable dt = _reatracker.ProcessCommand(command))
            {
                if (dt.Rows.Count > 0)
                {
                    String logon = Convert.ToString(dt.Rows[0]["logon"]);
                    String password = Convert.ToString(dt.Rows[0]["password"]);
                    Subject = "[REA Tracker] " + this.emailSubject;
                    HtmlBody +=

                         @"<h4>" + Convert.ToString(dt.Rows[0]["FIRST_NAME"]) + "," + @"</h4>
				    <p>
				    " + (this.emailMessage.Replace("<", "&lt;").Replace(">", "&gt;").Replace("[[LOGON]];", logon).Replace("[[PASSWORD]];", password).Replace("\n", "<br/>"))
                        //.Replace("\n", "<br/>") 
                        + @"
				     <br>";

                    AddEmailAddresses(Convert.ToString(dt.Rows[0]["EMAIL"]));
                }

            }

            sent = base.Send();
            return sent;
        }

    }

    /// <summary>
    /// This is an email set to person of the username/login
    /// </summary>

    public class EmailForgotUserName : EmailREATracker
    {
        protected string _user_email;

        public EmailForgotUserName(string user_email)
        {
            this._user_email = user_email;

        }
        public override bool Send()
        {
            bool sent = false;
            //look up user info
            String command = "SELECT LOGON FROM ST_USERS WHERE EMAIL = '" + _user_email + "'";
            using (DataTable dt = _reatracker.ProcessCommand(command))
            {
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];

                    AddEmailAddresses(_user_email);
                    Subject = "[REA Tracker] Forgot User ID Request";
                    HtmlBody = @"
                <h2 class='text-primary'>Forgot User ID Request</h2>
				<p>
				Your Login for REA Tracker is <b>" + dr["LOGON"].ToString() + @"</b>.
				<p>
				<hr>
				REA Tracker Administrators<br> " + _contactAddress + "<br/>";

                }
                sent = base.Send();
            }
            return sent;
        }
    }

    /// <summary>
    /// This is an email set to person of their password
    /// </summary>
    public class EmailForgotPasword : EmailREATracker
    {
        protected string _userName { get; set; }

        public EmailForgotPasword(string userName)
        {
            _userName = userName;
        }
        public override bool Send()
        {
            bool sent = false;

            //look up user info
            String command = "SELECT PASSWORD, EMAIL FROM ST_USERS WHERE LOGON = '" + _userName + "'";
            using (DataTable dt = _reatracker.ProcessCommand(command))
            {
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    AddEmailAddresses(dr["EMAIL"].ToString());

                    Subject = "[REA Tracker] Forgot Password Request";
                    HtmlBody = @"
                    <h2 class='text-primary'>Forgot Password Request</h2>
				    <p>
				    Your Password for REA Tracker is <b>" + dr["PASSWORD"].ToString() + @"</b>.
				    <p>
				    <hr>
				    REA Tracker Administrators<br> " + _contactAddress + "<br/>";
                    sent = base.Send();
                }
            }
            return sent;
        }
    }

    public class EmailSendCustomMessage : EmailREATracker
    {

        public string SenderEmail { get; set; }
        public string ReceiverEmail { get; set; }
        public string EmailSubject { get; set; }
        public string EmailMessage { get; set; }

        public EmailSendCustomMessage(string argsReceiver, string argsSenderEmail, string argsSubject, string argsmessage)
        {
            ReceiverEmail = argsReceiver;
            SenderEmail = argsSenderEmail;
            EmailSubject = argsSubject;
            EmailMessage = argsmessage;
        }

        public EmailSendCustomMessage()
        {

        }

        public override bool Send()
        {
            bool sent = false;

            if (AddEmailAddresses(ReceiverEmail))
            {
                Subject = EmailSubject;
                HtmlBody = EmailMessage.Replace("\n", "</br>");

                sent = base.Send();
            }

            return sent;
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SendToAll()
        {
            bool sent = false;
#if DEBUG
            //debug mode email the admin
            string command = "SELECT EMAIL FROM ST_USERS WHERE ST_USERS.USER_ID = -1;";
#else
            string command = "SELECT EMAIL FROM ST_USERS WHERE ST_USERS.ACCOUNT_TYPE != -1;";
#endif
            using (DataTable dt = _reatracker.ProcessCommand(command))
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string tmp = dr["EMAIL"].ToString().Trim();
                        if (!String.IsNullOrWhiteSpace(tmp))
                        {
                            //"none" needs to be check before it gets here (that is an application level check)
                            string[] splitEmailList = tmp.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            foreach (string emailAddress in splitEmailList)
                            {
                                MailAddress ma;
                                try
                                {
                                    ma = new MailAddress(emailAddress);
                                    _mail.Bcc.Add(ma);
                                }
                                catch (Exception ex)
                                {
                                    //bad email address, oh well
                                    string output = QVICommonIntranet.Utility.Utility.ProcessException(ex);
                                    System.Diagnostics.Trace.WriteLine(output);
                                }
                            }
                        }
                    }
                    Subject = EmailSubject;
                    HtmlBody = EmailMessage.Replace("\n", "</br>");
                    sent = base.Send();
                }

            }
            return sent;
        }
    }

    #region Code Review Emails
    public class EmailSendCodeReviewRequest : EmailREATracker
    {
        protected int _codeReviewID { get; set; }
        protected string _comment { get; set; }

        public EmailSendCodeReviewRequest(int codeReviewID)
        {
            _codeReviewID = codeReviewID;
        }


        public EmailSendCodeReviewRequest(int codeReviewID, string comment)
        {
            _codeReviewID = codeReviewID;
            _comment = comment;
        }

        public override bool Send()
        {
            bool sent = false;

            if (_reatracker.IsValidCodeReview(_codeReviewID))
            {
                using (System.Data.DataTable dt = _reatracker.SpecificCodeReview(_codeReviewID))
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        string list = _reatracker.GetCodeReviewEmailRequestList(_codeReviewID);
                        AddEmailAddresses(list);
                        string REA_TITLE = $"REA#{row["TRACKING_ID"].ToString()} - {row["REA_TITLE"].ToString()}";
                        Subject = $"[REA Tracker] [{row["PRODUCT"].ToString()}] [Code Review Request] {REA_TITLE}";

                        //1234 - some title
                        HtmlTitle = REA_TITLE.Replace("<", "&lt;").Replace(">", "&gt;");
                        HtmlBody = $@"
                            <table class='viewportWrapper' cellpadding='0' cellspacing='0' align=left style='clear:both; margin-bottom=50px' width=100%>
                                <thead>
                                    <tr>
                                        <td style= 'background-color: #FFFFFF; border:none' class='text-primary' colspan=2 align=left ><h2>Code Review Request for REA#{row["TRACKING_ID"]}</h2></td>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr><td style='border:1px solid #ddd;padding:8px'; align='left'>ID#</td><td style='border:1px solid #ddd;padding:8px'; align='left'>{row["ID"]}</td></tr>
                                    <tr><td style='border:1px solid #ddd;padding:8px'; align='left'>REA#</td><td style='border:1px solid #ddd;padding:8px'; align='left'>{row["TRACKING_ID"]} - {row["REA_TITLE"].ToString().Replace("<", "&lt;").Replace(">", "&gt;")}</td></tr>
                                    <tr><td style='border:1px solid #ddd;padding:8px'; align='left'>Requested&nbsp;By</td><td style='border:1px solid #ddd;padding:8px'; align='left'>{row["CREATED_BY_NAME"]}</td></tr>
                                    <tr><td style='border:1px solid #ddd;padding:8px'; align='left'>Title&nbsp;/&nbsp;Activity</td><td style='border:1px solid #ddd;padding:8px'; align='left'>{row["TITLE"]}</td></tr>
                                    <tr><td style='border:1px solid #ddd;padding:8px'; align='left'>Location</td><td style='border:1px solid #ddd; padding:8px'; align='left'>{row["CODE_LOCATION"]}</td></tr>
                                    <tr><td style='border:1px solid #ddd;padding:8px'; align='left' valign='top'>Notes</td><td style='border:1px solid #ddd'; valign='top' align='left'>{row["NOTES"].ToString().Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "<br/>")}</td></tr>
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <td colspan=2 style='text-align:center' height=50>{CreateCodeReviewLink(row["ID"].ToString(), "View Code Review")}</td>
                                    </tr>
                                </tfoot>
                            </table>";
                        HtmlBody += $@"<br><h2 class='text-primary'>" + _comment + "</h2>";

                    }
                    sent = base.Send();
                }

            }
            return sent;
        }
    }

    public class EmailSendCodeReviewAccepted : EmailREATracker
    {
        protected int _codeReviewID { get; set; }
        protected int _userID { get; set; }

        public EmailSendCodeReviewAccepted(int codeReviewID, int userID)
        {
            _codeReviewID = codeReviewID;
            _userID = userID;
        }

        public override bool Send()
        {
            bool sent = false;

            if (_reatracker.IsValidCodeReview(_codeReviewID))
            {
                using (System.Data.DataTable dt = _reatracker.SpecificCodeReview(_codeReviewID))
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        string list = _reatracker.GetCodeReviewEmailRequestList(_codeReviewID);
                        AddEmailAddresses(list);
                        AddEmailAddresses(row["CREATED_BY_EMAIL"].ToString());
                        string REA_TITLE = $"REA#{row["TRACKING_ID"].ToString()} - {row["REA_TITLE"].ToString()}";
                        Subject = $"[REA Tracker] [{row["PRODUCT"].ToString()}] [Code Review Request] RE: {REA_TITLE}";
                        string OptionalMessage = "<p>" + HtmlBody + "</p>";

                        HtmlBody = $@"<h2 class='text-primary'>Accepted by {_reatracker.getFullName(_userID)}</h2>";

                        HtmlBody += OptionalMessage;

                        HtmlBody += $@"<br>{CreateCodeReviewLink(row["ID"].ToString(), "View Code Review")}";

                        sent = base.Send();
                    }
                }

            }
            return sent;
        }

    }

    public class EmailSendCodeReviewStatusChange: EmailREATracker
    {
        protected int _codeReviewID { get; set; }
        protected int _userID { get; set; }
        protected REATrackerDB.ReviewStatus _status { get; set; }

        public EmailSendCodeReviewStatusChange(int codeReviewID, int userID, int status)
        {
            _codeReviewID = codeReviewID;
            _userID = userID;
            _status = (REATrackerDB.ReviewStatus)status;
        }

        public override bool Send()
        {
            bool sent = false;

            if (_reatracker.IsValidCodeReview(_codeReviewID))
            {
                using (System.Data.DataTable dt = _reatracker.SpecificCodeReview(_codeReviewID))
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        using (System.Data.DataTable user = _reatracker.GetUserDetails((int)row["CREATED_BY"]))
                        {
                            AddEmailAddresses(user.Rows[0]["EMAIL"].ToString());
                        }
                        string REA_TITLE = $"REA#{row["TRACKING_ID"].ToString()} - {row["REA_TITLE"].ToString()}";
                        Subject = $"[REA Tracker] [{row["PRODUCT"].ToString()}] [Code Review Request] RE: {REA_TITLE}";

                        switch(_status)
                        {
                            case REATrackerDB.ReviewStatus.Approved:
                                {
                                    HtmlBody = $@"<h2 class='text-primary'>Has been <b>APPROVED</b> by {_reatracker.getFullName(_userID)}</h2>";
                                    break;
                                }
                            case REATrackerDB.ReviewStatus.ApprovedWithComments:
                                {
                                    HtmlBody = $@"<h2 class='text-primary'>Has been <b>APPROVED but with comments</b> by {_reatracker.getFullName(_userID)}.</h2>";
                                    break;
                                }
                            case REATrackerDB.ReviewStatus.NeedsWork:
                                {
                                    HtmlBody = $@"<h2 class='text-primary'>Has <b>NOT BEEN APRROVED</b> by {_reatracker.getFullName(_userID)}.</h2>";
                                    break;
                                }
                            default:
                                {
                                    HtmlBody = "** ERROR **";
                                    break;
                                }
                        }
                        HtmlBody += $@"<br>{CreateCodeReviewLink(row["ID"].ToString(), "View Code Review")}";

                        sent = base.Send();
                    }
                }

            }
            return sent;
        }

    }

    public class EmailSendCodeReviewNotification: EmailREATracker
    {
        protected int _codeReviewID { get; set; }
        protected string _comment { get; set; }

        public EmailSendCodeReviewNotification(int codeReviewID, string comment)
        {
            _codeReviewID = codeReviewID;
            _comment = comment;
        }

        public override bool Send()
        {
            bool sent = false;

            if (_reatracker.IsValidCodeReview(_codeReviewID))
            {
                using (System.Data.DataTable dt = _reatracker.SpecificCodeReview(_codeReviewID))
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        string list = _reatracker.GetCodeReviewEmailReviewerList(_codeReviewID);
                        AddEmailAddresses(list);

                        string REA_TITLE = $"REA#{row["TRACKING_ID"].ToString()} - {row["REA_TITLE"].ToString()}";
                        Subject = $"[REA Tracker] [{row["PRODUCT"].ToString()}] [Code Review Request] RE: {REA_TITLE}";

                        HtmlBody = $@"<h2 class='text-primary'>The author of the code review has responded or updated the resolution to some comments.</h2>";
                        HtmlBody += $@"<br><h2 class='text-primary'>" + _comment + "</h2>";

                        HtmlBody += $@"<br>{CreateCodeReviewLink(row["ID"].ToString(), "View Code Review")}";

                        sent = base.Send();
                    }
                }

            }
            return sent;
        }

    }

    #endregion
}
