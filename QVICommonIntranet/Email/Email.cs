using System.Net.Mail;
using System.Net;
using System;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using QVICommonIntranet.Utility;
using System.Diagnostics;

/// <summary>
/// This namespace contains the classes for client to hookup to the QVI email servers
/// </summary>
namespace QVICommonIntranet.Email
{
    public class Email : IDisposable
    {
        /// <summary>
        /// The function figures out the correct IP address for QVI mail server
        /// </summary>
        /// <returns>IP Address</returns>
        private string GetSMTPHostAddress()
        {
            //this is the default address just in case we fail getting it from the DNS server
            //this is the IP address of MERCURY
            string ip_address = "64.128.118.11";

            try
            {
                IPAddress[] ips = Dns.GetHostAddresses("mail-relay.qvii.net");
                if (ips.Length > 0)
                {
                    //we assume the first address is the correct one, current it only returns on address
                    ip_address = ips[0].ToString();
                }
                else
                {
                    Logging.WriteEntry($"Couldn't get the IP address for the email server. Using default IP address ({ip_address})", System.Diagnostics.EventLogEntryType.Warning);
                }
            }
            catch (System.Exception ex)
            {
                Logging.WriteEntry($"GetSMTPHostAddress failed ({ex.Message})", System.Diagnostics.EventLogEntryType.Error);
            }
            return ip_address;
        }

        /// <summary>
        /// Enable/disable the sending of the emails
        /// </summary>
        public bool EnableEmail { get; set; } = true;

        //future make these private
        private SmtpClient _smtp;
        protected MailMessage _mail;

        /// <summary>
        /// Default email address that the emails are sent from
        /// </summary>
        static protected string _contactAddress = "bugTrak.qvi@qvii.com";

        public string Subject { get; set; }
        public string HtmlTitle { get; set; }
        public string HtmlBody { get; set; }

        public string ToAddressList {
                    get
                    {
                        return _mail.To.ToString();
                    }
                    set
                    {
                        AddEmailAddresses(value);
                    }
                }
        /// <summary>
        /// Default constructor, FUTURE: make singleton
        /// </summary>
        public Email()
        {
            try
            {
                _smtp = new SmtpClient(GetSMTPHostAddress(), 25);
                _smtp.UseDefaultCredentials = false;

                _mail = new MailMessage();
                _mail.From = new MailAddress(_contactAddress, "SCR Tracker");
                _mail.IsBodyHtml = true;
                _mail.BodyEncoding = System.Text.Encoding.UTF8;

                //load in the body;
                _mail.Body = "";
            }
            catch (Exception ex)
            {
                string output = QVICommonIntranet.Utility.Utility.ProcessException(ex);
                System.Diagnostics.Trace.WriteLine(output);
            }
        }
        //Todo: wrap in try catch and return bool
        protected bool AddEmailAddresses(string listOfEmails)
        {
            bool success = false;
#if DEBUG
            //debug mode so just email to ADMIN
            _mail.To.Add("dmf@qvii.com");
            _mail.To.Add(_contactAddress);
            success = true;
#else
            if (!String.IsNullOrWhiteSpace(listOfEmails))
            {
                //"none" needs to be check before it gets here (that is an application level check)
                string[] splitEmailList = listOfEmails.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach(string emailAddress in splitEmailList)
                {
                    MailAddress ma;
                    try
                    {
                        ma = new MailAddress(emailAddress);
                        _mail.To.Add(ma);
                        success = true;
                    }
                    catch(Exception ex)
                    {
                        //bad email address, oh well
                        string output = QVICommonIntranet.Utility.Utility.ProcessException(ex);
                        System.Diagnostics.Trace.WriteLine(output);
                    }
                }
            }
#endif
            return success;
        }

        /// <summary>
        /// This clears the To, Bcc, Subject and Body fields for the mail. The From field is not changed.
        /// </summary>
        protected void Clear()
        {
            _mail.To.Clear();
            _mail.Bcc.Clear();
            _mail.Body = String.Empty;
            _mail.Subject = String.Empty;
        }

        public virtual bool Send()
        {
            bool sent = false;
            string _copyright = @"&copy; " + @DateTime.Now.Year + " Quality Vision International, Inc. All rights reserved.";
          
            //if either the Subject or Title is null, then copy it to the other field
            if (String.IsNullOrWhiteSpace(Subject) && !String.IsNullOrWhiteSpace(HtmlTitle))
            {
                Subject = HtmlTitle;
            }
            if (String.IsNullOrWhiteSpace(HtmlTitle) && !String.IsNullOrWhiteSpace(Subject))
            {
                HtmlTitle = Subject;
            }

            if (!String.IsNullOrWhiteSpace(HtmlTitle) && !String.IsNullOrWhiteSpace(HtmlTitle))
            {
                if (!String.IsNullOrWhiteSpace(HtmlBody))
                {
                    try
                    {
                        //set the subject if not already set
                        if (String.IsNullOrWhiteSpace(_mail.Subject))
                        {
#if DEBUG
                            _mail.Subject = "[DEBUG] " + Subject;
#else
                            _mail.Subject = Subject;
#endif
                        }
                        if (String.IsNullOrWhiteSpace(_mail.Body))
                        {
                            string temp = QVICommonIntranet.Resources.CommonResources.EmailShell.Replace("@@BODY@@", HtmlBody);    
                            _mail.Body = temp.Replace("@@TITLE@@", HtmlTitle).Replace("@@COPYRIGHT@@", _copyright);
                            
                        }

                        if (EnableEmail)
                        {
                            DateTime start = DateTime.Now;
                            System.Threading.Tasks.Task.Run(() => { _smtp.Send(_mail); });
                            //await _smtp.SendMailAsync(_mail);
                            DateTime end = DateTime.Now;
                            TimeSpan span = end.Subtract(start);
                            if (span.TotalSeconds > 5)
                            {
                                Utility.Logging.WriteEntry($"Took {span.TotalSeconds} seconds to send email.", EventLogEntryType.Warning);
                            }
                        }
                        sent = true;
                    }
                    catch(Exception ex)
                    {
                        string output = QVICommonIntranet.Utility.Utility.ProcessException(ex);
                        System.Diagnostics.Trace.WriteLine(output);
                    }
                    finally
                    {
                        //dont dispose, just in case there is anotehr email to send
                    }
                }
                else
                {
                    //error, cant have a blank body either
                }
            }
            else
            {
                //error, change have a blank subject/title
            }
            return sent;
        }

        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
                if (_mail != null)
                {
                    _mail.Dispose();
                    _mail = null;
                }

                if (_smtp != null)
                {
                    _smtp.Dispose();
                    _smtp = null;
                }
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
    }
}
