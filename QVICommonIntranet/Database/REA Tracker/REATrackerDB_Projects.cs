using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QVICommonIntranet.Database
{
    public partial class REATrackerDB
    {
        public DataTable GetProjects()
        {
            System.Data.DataTable dt = new DataTable("PROJECTS");
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    ClearError();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT PROJECTS.*, (ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME) AS OwnerName FROM PROJECTS LEFT JOIN ST_USERS ON ST_USERS.USER_ID = PROJECTS.OwnerID ORDER BY PROJECTS.Code";
                    using (SqlDataAdapter reader = new SqlDataAdapter(command))
                    {
                        //dt = new DataTable();
                        reader.Fill(dt);
                    }
                }
            }
            return dt;
        }
        public ProjectInfo GetProjectInfo(int id)
        {
            ProjectInfo results = new ProjectInfo();
            using (DataTable dt = ProcessCommand("SELECT PROJECTS.*, (ST_USERS.FIRST_NAME + ' ' + ST_USERS.LAST_NAME) AS OwnerName FROM PROJECTS LEFT JOIN ST_USERS ON ST_USERS.USER_ID = PROJECTS.OwnerID WHERE ID=" + id + "ORDER BY PROJECTS.Code"))
            {
                results.ID = id;
                if (dt.Rows.Count > 0)
                {
                    results.IsValidID = true;
                    results.Code = dt.Rows[0]["Code"].ToString();
                    results.Name = dt.Rows[0]["Name"].ToString();
                    results.Description = dt.Rows[0]["Description"].ToString();
                    results.Active = GenericDB.CheckValue(dt.Rows[0]["Active"], false);
                    results.Support = GenericDB.CheckValue(dt.Rows[0]["SupportMode"], 0);
                    if (dt.Rows[0]["StartDate"] != DBNull.Value)
                    {
                        results.StartDate = DateTime.Parse(dt.Rows[0]["StartDate"].ToString()).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        results.StartDate = "";
                    }
                    if (dt.Rows[0]["EndDate"] != DBNull.Value)
                    {
                        results.EndDate = DateTime.Parse(dt.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        results.EndDate = "";
                    }
                    results.OwnerID = GenericDB.CheckValue(dt.Rows[0]["OwnerID"], -1);
                    results.OwnerName = GenericDB.CheckValue(dt.Rows[0]["OwnerName"], "");
                    results.CreateDate = DateTime.Parse(dt.Rows[0]["CreationDate"].ToString()).ToString("yyyy-MM-dd");
                    //TODO: owner stuff
                }
                else
                {
                    //doesnt exist, so init as if
                    results.IsValidID = false;
                }
            }
            return results;
        }

        public bool UpdateProject(int id, string Name, string Code, string Description, bool Active, int Support, string StartDate, string EndDate)
        {
            bool success = false;

            //first make sure we have a valid ID
            if (id > 0)
            {
                //get the original data from the database so we can compare
                ProjectInfo original = GetProjectInfo(id);
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    
                    bool changed = false;
                    string strSQL = "UPDATE [PROJECTS] SET ";
                    if (Name != original.Name)
                    {
                        strSQL += "[Name] = @Name";
                        command.Parameters.Add(new SqlParameter("Name", Name.Trim()));
                        changed = true;
                    }
                    if (Code != original.Code)
                    {
                        if (changed) { strSQL += " , "; }
                        strSQL += "[Code] = @Code";
                        command.Parameters.Add(new SqlParameter("Code", Code.Trim()));
                        changed = true;
                    }
                    if (((Description != null) && (original.Description == String.Empty)) || 
                        (Description != original.Description))
                    {
                        if (changed) { strSQL += " , "; }
                        strSQL += "[Description] = @Description";
                        if ((Description != null) && (Description != ""))
                        {
                            command.Parameters.Add(new SqlParameter("Description", Description));
                        }
                        else
                        {
                            command.Parameters.Add(new SqlParameter("Description", DBNull.Value));
                        }
                        changed = true;
                    }
                    if (Active != original.Active)
                    {
                        if (changed) { strSQL += " , "; }
                        strSQL += "[Active] = @Active";
                        command.Parameters.Add(new SqlParameter("Active", Active));
                        changed = true;
                    }
                    if (Support != original.Support)
                    {
                        if (changed) { strSQL += " , "; }
                        strSQL += "[SupportMode] = @SupportMode";
                        command.Parameters.Add(new SqlParameter("SupportMode", Support));
                        changed = true;
                    }

                    //
                    //SKIP CreateDate AS WE DONT ALLOW THE UPDATE OF THE CREATION DATE
                    //

                    if (((StartDate != null) && (original.StartDate == String.Empty)) || 
                        (StartDate != original.StartDate))
                    {
                        if (changed) { strSQL += " , "; }
                        strSQL += "[StartDate] = @StartDate";

                        if ((StartDate != null) && (StartDate != ""))
                        {
                            command.Parameters.Add(new SqlParameter("StartDate", StartDate));
                        }
                        else
                        {
                            command.Parameters.Add(new SqlParameter("StartDate", DBNull.Value));
                        }
                        changed = true;
                    }
                    if (((EndDate != null) && (original.EndDate == String.Empty)) || 
                        (EndDate != original.EndDate))
                    {
                        if (changed) { strSQL += " , "; }
                        strSQL += "[EndDate] = @EndDate";
                        if ((EndDate != null) && (EndDate != ""))
                        {
                            command.Parameters.Add(new SqlParameter("EndDate", EndDate));
                        }
                        else
                        {
                            command.Parameters.Add(new SqlParameter("EndDate", DBNull.Value));
                        }
                        changed = true;
                    }

                    strSQL += " WHERE ID = " + original.ID;
                    try
                    {
                        if (changed)
                        {
                            command.CommandType = CommandType.Text;
                            command.CommandText = strSQL;
                            success = (command.ExecuteNonQuery() > 0);
                        }
                        _lastError = "";
                    }
                    catch (Exception ex)
                    {
                        ProcessException(ex);
                    }
                    finally
                    {
                        command.Parameters.Clear();
                        connection.Close();
                        connection.Dispose();
                    }
                }//using
            }//if test valid ID
            return success;
        }

        public bool AddProject(string Name, string Code, string Description) //, bool Active, bool Support, string StartDate, string EndDate)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                string strSQL = @"INSERT INTO PROJECTS(Code, Name, Description) VALUES (@Code, @Name, @Description); SELECT CAST(scope_identity() AS int); ";
                command.Parameters.Add(new SqlParameter("Name", Name.Trim()));
                command.Parameters.Add(new SqlParameter("Code", Code.Trim()));
                if (string.IsNullOrWhiteSpace(Description))
                {
                    command.Parameters.Add(new SqlParameter("Description", DBNull.Value));
                }
                else
                {
                    command.Parameters.Add(new SqlParameter("Description", Description));
                }
                command.CommandType = CommandType.Text;
                command.CommandText = strSQL;
                int projectID = -1;
                try
                {
                    projectID = (int)command.ExecuteScalar();

                    _lastError = "";
                }
                catch (Exception ex)
                {
                    ProcessException(ex);
                    projectID = -1;
                }
                finally
                {
                    success = (projectID > 0);
                    command.Parameters.Clear();
                    connection.Close();
                    connection.Dispose();
                }

            }//using
            return success;
        }

    }
}
