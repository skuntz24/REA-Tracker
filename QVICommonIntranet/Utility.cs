using System;
using System.Diagnostics;

namespace QVICommonIntranet.Utility
{
    public class Utility
    {
        /// <summary>
        /// This takes in any Exception class and traverses the InnerException to format
        /// a string of all of the sub exceptions that were found.
        /// </summary>
        /// <param name="ex">Any Exception</param>
        /// <returns>Formatted string of all exceptions found</returns>
        static public string ProcessException(Exception ex)
        {
            string output = ex.ToString() + "\r\n";
            Exception inner = ex.InnerException;
            while (inner != null)
            {
                output += "\tInner Exception: " + inner.Message + "\r\n";
                inner = inner.InnerException;
            }
            return output;
        }
    }

    public class Logging
    {
        private static string source = "REA Tracker";
        private static string log = "Application";

        /// <summary>
        /// Writes an Exception to the log, if the log is doesnt exist it is created first.
        /// The log entry is of Error type
        /// </summary>
        /// <param name="ex">Any Exception</param>
        /// <returns>Exception that was written out</returns>
        public static string WriteEntry(Exception ex)
        {
            string output = Utility.ProcessException(ex);

            if (CreateEventSource())
            {
                try
                {
                    WriteEntry(source, output, EventLogEntryType.Error);
                }
                catch
                {
                    //TODO:
                }
            }
            return output;
        }

        /// <summary>
        /// Writes an Exception and customer message to the log, if the log is doesnt exist it is created first.
        /// The log entry is of Error type
        /// </summary>
        /// <param name="ex">Any Exception</param>
        /// <param name="message">Any additional information added to the end of the exception</param>
        /// <returns>Exception that was written out</returns>
        public static string WriteEntry(Exception ex, string message)
        {
            string output = Utility.ProcessException(ex);
            output += "\r\n" + message;
            
            if (CreateEventSource())
            {
                try
                {
                    WriteEntry(source, output, EventLogEntryType.Error);
                }
                catch
                {
                    //TODO
                }
            }
            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">The string to write to the event log.</param>
        /// <param name="type">One of the EventLogEntryType values.</param>
        public static void WriteEntry(string message, EventLogEntryType type)
        {
            if (CreateEventSource())
            {
                try
                {
                    WriteEntry(source, message, type);
                }
                catch
                {
                    //TODO
                }
            }
        }
        #region PRIVATE COVER EventLog FUNCTIONS
        /// <summary>
        /// 
        /// </summary>
        /// <exception>
        /// None...all are trapped
        /// </exception>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        private static void WriteEntry(string source, string message, EventLogEntryType type)
        {
            if (CreateEventSource())
            {
                try
                {
                    EventLog.WriteEntry(source, message, type);
                }
                catch
                {
                    //TODO
                }
            }
        }
        /// <summary>
        /// Internal function to reduce the number of locations that we actually try to create the source
        /// </summary>
        /// <exception>
        /// None...all are trapped
        /// </exception>
        /// <returns>true if the source was create or already exists, else false</returns>
        private static bool CreateEventSource()
        {
            bool success = false;
            try
            {
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, log);
                }
                success = true;
            }
            catch
            {
                //TODO
            }
            return success;
        }
        #endregion
    }
}
