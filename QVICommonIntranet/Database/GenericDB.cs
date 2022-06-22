using System;
using System.Data;
using System.Data.SqlClient;
using QVICommonIntranet.Utility;

/// <summary>
/// This namespace contains the classes for client to hookup to the databases of various QVI Products
/// </summary>
namespace QVICommonIntranet.Database
{
    /// <summary>
    /// The GenericDB class contains the base functionality needed for any database. Deals with opening, closing, processing commands and tracking errors.
    /// Databases specific to the various QVI databases should derive from this class.
    /// </summary>
    public class GenericDB
    {
        /// <summary>
        /// Connection to the SQL Database
        /// </summary>
        private SqlConnection _sqlConnection = null;
        /// <summary>
        /// The connection string to establish the connection to the SQL Database
        /// </summary>
        protected string _connectionString = "";
        protected string _connectionStringSCR = "";
        /// <summary>
        /// 
        /// </summary>
        protected string _lastError = "";

        /// <summary>
        /// Returns the last error caught. Could be a detail exception or user defined error.
        /// </summary>
        public string LastError
        {
            get { return _lastError; }
        }

        /// <summary>
        /// Clears out any error information
        /// </summary>
        public void ClearError()
        {
            _lastError = "";
        }
        /// <summary>
        /// Returns the name of the database that is currently connected and opened, else returns UNKNOWN.
        /// </summary>
        public string DatabaseName
        {
            get
            {
                if (IsOpen())
                {
                    return _sqlConnection.Database;
                }
                else
                {
                    return "UNKNOWN";
                }
            }
        }

        /// <summary>
        /// Returns the name of the server that is currently connected and opened, else returns UNKNOWN.
        /// </summary>
        public string ServerName
        {
            get
            {
                if (IsOpen())
                {
                    return _sqlConnection.DataSource;
                }
                else
                {
                    return "UNKNOWN";
                }
            }
        }

        /// <summary>
        /// Sets the connection string and trys to open a connection to the database.
        /// If the connection fails, LastError will return error.
        /// </summary>
        /// <param name="connection_string">valid connetion string</param>
        /// <returns>true if the connection was open, else false</returns>
        public bool OpenConnection(string connection_string)
        {
            _connectionString = connection_string;

            bool success = OpenConnection();

            return success;
        }

        /// <summary>
        /// Opens a connection to the database using the last known connection string that was used.
        /// If the connection fails, LastError will return error.
        /// </summary>
        /// <returns>true if the connection was open, else false</returns>
        public bool OpenConnection()
        {
            bool success = false;

            try
            {

                _sqlConnection = new SqlConnection(_connectionString);
                _sqlConnection.Open();
                System.Diagnostics.Debug.WriteLine("Database: {0}", _sqlConnection.Database);
                System.Diagnostics.Debug.WriteLine("ServerVersion: {0}", _sqlConnection.ServerVersion);
                System.Diagnostics.Debug.WriteLine("State: {0}", _sqlConnection.State);
                success = true;
                _lastError = "";
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
            return success;
        }

        /// <summary>
        /// Closes the current database connection (if opened). Clears any LastError details.
        /// </summary>
        public void CloseConnection()
        {
            if (IsOpen())
            {
                _sqlConnection.Close();
                _sqlConnection = null;
            }
            _lastError = "";
        }

        /// <summary>
        /// Determines if the connection to the database is open or not.
        /// </summary>
        /// <returns>True if the state is not closed, else false.</returns>
        public bool IsOpen()
        {
            bool is_open = false;

            if (_sqlConnection != null)
            {
                is_open = (_sqlConnection.State != System.Data.ConnectionState.Closed);
            }
            return is_open;
        }

        /// <summary>
        /// Formats the exception into a text string and store as the LastError
        /// </summary>
        /// <param name="ex">Exception</param>
        protected void ProcessException(Exception ex)
        {
            _lastError = Logging.WriteEntry(ex);
        }

        /// <summary>
        /// Determines if a table exists in the database
        /// </summary>
        /// <param name="table_name">Name of a table</param>
        /// <returns>True if it exists, else false.</returns>
        public bool TableExists(string table_name)
        {
            bool exists = false;

            string sql = "SELECT count(*) FROM information_schema.TABLES WHERE (TABLE_CATALOG = '" + DatabaseName + "') AND (TABLE_NAME = '" + table_name + "')";

            exists = (((int)ProcessScalarCommand(sql)) == 1);
            return exists;
        }

        /// <summary>
        /// Exectures the query, returns the results from the first row. All other rows and columns are ignored.
        /// Any errors are set into LastError variable.
        /// </summary>
        /// <param name="sql_command">SQL query to run</param>
        /// <returns></returns>
        public object ProcessScalarCommand(string sql_command)
        {
            object return_value = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;

                try
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql_command;

                    return_value = cmd.ExecuteScalar();

                    _lastError = "";
                }
                catch (Exception ex)
                {
                    ProcessException(ex);
                }
                finally
                {
                    if (cmd != null) { cmd.Dispose(); }
                }
            }

            return return_value;
        }

        public object ProcessScalarCommandSCR(string sql_command)
        {
            object return_value = null;

            using (SqlConnection connection = new SqlConnection(_connectionStringSCR))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;

                try
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql_command;

                    return_value = cmd.ExecuteScalar();

                    _lastError = "";
                }
                catch (Exception ex)
                {
                    ProcessException(ex);
                }
                finally
                {
                    if (cmd != null) { cmd.Dispose(); }
                }
            }

            return return_value;
        }


        /// <summary>
        /// Executes the query and returns the results into a DataTable
        /// Any errors are set into LastError variable.
        /// </summary>
        /// <param name="sql_command">SQL query to run</param>
        /// <returns></returns>
        public DataTable ProcessCommand(string sql_command)
        {
            SqlDataAdapter reader = null;
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                try
                {
                    cmd.Connection = connection; // _sqlConnection;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql_command;
                    reader = new SqlDataAdapter(cmd);

                    dt = new DataTable();
                    reader.Fill(dt);
                    _lastError = "";
                }
                catch (Exception ex)
                {
                    ProcessException(ex);
                    _lastError += "SQL: " + sql_command + "\r\n";
                    Utility.Logging.WriteEntry("SQL: " + sql_command, System.Diagnostics.EventLogEntryType.Information);
                }
                finally
                {
                    if (reader != null) { reader.Dispose(); }
                    if (cmd != null) { cmd.Dispose(); }
                }
            }
            return dt;
        }

        public DataTable ProcessCommandSCR(string sql_command)
        {
            SqlDataAdapter reader = null;
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(_connectionStringSCR))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                try
                {
                    cmd.Connection = connection; // _sqlConnection;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql_command;
                    reader = new SqlDataAdapter(cmd);

                    dt = new DataTable();
                    reader.Fill(dt);
                    _lastError = "";
                }
                catch (Exception ex)
                {
                    ProcessException(ex);
                    _lastError += "SQL: " + sql_command + "\r\n";
                    Utility.Logging.WriteEntry("SQL: " + sql_command, System.Diagnostics.EventLogEntryType.Information);
                }
                finally
                {
                    if (reader != null) { reader.Dispose(); }
                    if (cmd != null) { cmd.Dispose(); }
                }
            }
            return dt;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText">SQL query to run</param>
        /// <returns>Number of rows affected</returns>
        public int ExecuteNonQuery(string cmdText)
        {
            int num_rows = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = cmdText;

                    num_rows = command.ExecuteNonQuery();
                }
            }
            return num_rows;
        }

        #region "TRANSACTION COMMANDS"
        /// <summary>
        /// Executes the query and returns the results into a DataTable
        /// Any errors are set into LastError variable.
        /// </summary>
        /// <param name="id">transaction name</param>
        /// <param name="sql_command">SQL query to run</param>
        /// <returns></returns>
        public DataTable ProcessCommand(string id, string sql_command)
        {
            DataTable dt = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                SqlTransaction transaction = connection.BeginTransaction(id);
                SqlDataAdapter reader = null;

                cmd.Connection = connection;
                cmd.Transaction = transaction;

                try
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql_command;
                    reader = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    reader.Fill(dt);

                    transaction.Commit();
                    _lastError = "";
                }
                catch (Exception ex)
                {
                    ProcessException(ex);
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        ProcessException(ex2);
                    }
                }
                finally
                {
                    if (reader != null) { reader.Dispose(); }
                    if (cmd != null) { cmd.Dispose(); }
                }
            }

            return dt;
        }

        /// <summary>
        /// Exectures the query, returns the results from the first row. All other rows and columns are ignored.
        /// Any errors are set into LastError variable.
        /// </summary>
        /// <param name="id">transaction name</param>
        /// <param name="sql_command">SQL query to run</param>
        /// <returns></returns>
        public object ProcessScalarCommand(string id, string sql_command)
        {
            object return_value = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                SqlTransaction transaction = connection.BeginTransaction(id);
                SqlDataAdapter reader = null;

                cmd.Connection = connection;
                cmd.Transaction = transaction;

                try
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql_command;

                    return_value = cmd.ExecuteScalar();

                    transaction.Commit();
                    _lastError = "";
                }
                catch (Exception ex)
                {
                    ProcessException(ex);
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        ProcessException(ex2);
                    }
                }
                finally
                {
                    if (reader != null) { reader.Dispose(); }
                    if (cmd != null) { cmd.Dispose(); }
                }
            }

            return return_value;
        }

        /// <summary>
        /// Executes a SQL query
        /// </summary>
        /// <param name="id">transaction name</param>
        /// <param name="sql_command">SQL query to run</param>
        /// <returns>true if success, else false</returns>
        public bool ExecuteSqlTransaction(string id, string sql_command)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                SqlTransaction transaction = connection.BeginTransaction(id);

                cmd.Connection = connection;
                cmd.Transaction = transaction;

                try
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql_command;
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                    success = true;
                    _lastError = "";
                }
                catch (Exception ex)
                {
                    ProcessException(ex);
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        ProcessException(ex2);
                    }
                }
            }

            return success;
        }
        #endregion

        #region "STATIC UTILITY FUNCTIONS"
        static public int CheckValue(object value, int default_value)
        {
            int return_value = (value != DBNull.Value) ? Convert.ToInt32(value) : default_value;
            return return_value;
        }

        static public bool CheckValue(object value, bool default_value)
        {
            bool return_value = (value != DBNull.Value) ? Convert.ToBoolean(value) : default_value;
            return return_value;
        }

        static public string CheckValue(object value, string default_value)
        {
            string return_value = (value != DBNull.Value) ? Convert.ToString(value) : default_value;
            return return_value;
        }
        static public DateTime CheckValue(object value, DateTime default_value)
        {
            DateTime return_value = (value != DBNull.Value) ? Convert.ToDateTime(value) : default_value;
            return return_value;
        }
        #endregion
    }

}
