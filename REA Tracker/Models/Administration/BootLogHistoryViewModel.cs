using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{
    public class BootLogHistoryViewModel
    {
        [IntegerNoZero(ErrorMessage = "Please enter a number greater then zero.")]
        public string inputdays { get; set; }
        public List<DataRow> Logs { get; set; }

        public BootLogHistoryViewModel()
        {
            this.inputdays = "1";
            this.Logs = new List<DataRow>();
            this.DisplayLogs();
        }

        public BootLogHistoryViewModel(int input = 1)
        {
            this.inputdays = Convert.ToString(input);
            this.Logs = new List<DataRow>();
            this.DisplayLogs();
        }

        public void DisplayLogs()
        {
            ///<summary>
            /// main function to call to populate the rest of the model
            ///</summary>
            this.populateLogs();
            this.FilterLogs();
        }

        private void populateLogs()
        {
            ///<summary>
            ///populates the log from st_boot_log
            ///</summary>
            REATrackerDB _sql = new REATrackerDB();
            string cmd = "SELECT IPADDRESS, MACHINE_NAME, LOG_DATE, SQL_SERVER_DATE FROM REA_BOOT_LOG ORDER BY LOG_DATE DESC";
            this.Logs = new List<DataRow>();
            using (System.Data.DataTable dt = _sql.ProcessCommand(cmd))
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    this.Logs.Add(row);
                }
            }
        }

        private void FilterLogs()
        {
            ///<summary>
            /// Calculates the differene between  between the logs and 
            /// if there is a large enought difference display it
            ///</summary>
            DateTime now = DateTime.Now.Date;
            List<DataRow> newLog = new List<DataRow>();
            for (int i = 0; i < this.Logs.Count; i++)
            {
                DataRow dr = Logs.ElementAt(i);
                int diff = now.Subtract(Convert.ToDateTime(dr["Log_Date"])).Days;
                int tempInput;
                Int32.TryParse(inputdays, out tempInput);
                if (diff <= tempInput) //if the difference is less then the threshhold passed in
                {
                    newLog.Add(dr);
                }
            }
            this.Logs = newLog;
        }
    }

}