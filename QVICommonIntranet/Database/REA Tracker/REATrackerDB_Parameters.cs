using System;
using System.Data;
using System.Data.SqlClient;
using QVICommonIntranet.Utility;
using System.Linq;

namespace QVICommonIntranet.Database
{
    public partial class REATrackerDB
    {
        public bool DoesParameterExist(string variable)
        {
            bool doesExists = false;

            try
            {
                doesExists = ((int)ProcessScalarCommand($"SELECT COUNT(*) FROM REA_APPLICATION WHERE VARIABLE = '{variable}';") == 1);
            }
            catch (Exception ex)
            {
                Logging.WriteEntry($"DoesParameterExist({variable}) Error: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return doesExists;
        }

        public void CreateParameter(string variable, string value)
        {
            try
            {
                ProcessScalarCommand($"INSERT INTO [REA_APPLICATION] ([Variable], [Value]) VALUES ('{variable}', '{value}')");
            }
            catch (Exception ex)
            {
                Logging.WriteEntry($"CreateParameter({variable}, {value}) Error: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        public string GetParameter(string variable)
        {
            string value = "";
            try
            {
                value = (string)ProcessScalarCommand($"SELECT VALUE FROM REA_APPLICATION WHERE VARIABLE = '{variable}';");
            }
            catch (Exception ex)
            {
                Logging.WriteEntry($"GetParameter({variable}) Error: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return value;
        }

        public bool SetParameter(int id, string value)
        {
            bool success = false;

            string cmdText = "UPDATE REA_APPLICATION SET Value=@Value WHERE ID = @ID";

            if (!string.IsNullOrEmpty(value))
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = cmdText;

                        //add the parameters
                        SqlParameter paramValue = new SqlParameter("Value", value);
                        command.Parameters.Add(paramValue);

                        SqlParameter paramID = new SqlParameter("ID", id);
                        command.Parameters.Add(paramID);

                        try
                        {
                            success = command.ExecuteNonQuery() > 0;

                            ClearError();
                        }
                        catch (Exception ex)
                        {
                            //something went wrong, most likely the value is already in the table
                            ProcessException(ex);
                        }
                    }
                }
            }
            return success;
        }

        public bool GetParameterBool(string variable)
        {
            bool value = false;
            try
            {
                string s = GetParameter(variable);
                value = Convert.ToBoolean(Convert.ToInt16(s));
            }
            catch (Exception ex)
            {
                Logging.WriteEntry($"GetParameterBool({variable}) Error: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return value;
        }

        public bool IsEmailEnabled()
        {
            bool enabled = GetParameterBool("ENABLE_EMAIL");

            return enabled;
        }

        public bool IsSPDManager(int userID)
        {
            bool isSPDManager = false;

            if (DoesParameterExist("SPD_MANAGERS"))
            {
                try
                {
                    string managers = GetParameter("SPD_MANAGERS");

                    isSPDManager = managers.Split(',').Contains(userID.ToString());
                }
                catch
                {
                    isSPDManager = false;
                }
            }
            return isSPDManager;
        }
    }
}
