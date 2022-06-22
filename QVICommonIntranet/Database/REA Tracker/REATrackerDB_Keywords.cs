using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;

namespace QVICommonIntranet.Database
{
    public partial class REATrackerDB
    {
        public int InsertKeyword(string keyword, string description)
        {
            int num_rows = 0;

            //double check that keyword is not empty
            string cmdText = "INSERT INTO REA_KEYWORD(Keyword, Description) VALUES (@Keyword, @Description)";

            if (!string.IsNullOrEmpty(keyword))
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
                        SqlParameter paramKeyword = new SqlParameter("Keyword", keyword);
                        command.Parameters.Add(paramKeyword);

                        SqlParameter paramDescription = new SqlParameter("Description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                        command.Parameters.Add(paramDescription);

                        try
                        {
                            num_rows = command.ExecuteNonQuery();

                            ClearError();
                        }
                        catch (Exception ex)
                        {
                            //something went wrong, most likely the value is already in the table
                            ProcessException(ex);
                            num_rows = -1;
                        }
                    }
                }
            }
            return num_rows;
        }

        public int UpdateKeyword(int id, string keyword, string description)
        {
            int num_rows = 0;

            //double check that keyword is not empty
            string cmdText = "UPDATE REA_KEYWORD SET Keyword=@Keyword, Description=@Description WHERE ID = @ID";

            if (!string.IsNullOrEmpty(keyword))
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
                        SqlParameter paramKeyword = new SqlParameter("Keyword", keyword);
                        command.Parameters.Add(paramKeyword);

                        SqlParameter paramDescription = new SqlParameter("Description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                        command.Parameters.Add(paramDescription);

                        SqlParameter paramID = new SqlParameter("ID", id);
                        command.Parameters.Add(paramID);

                        try
                        {
                            num_rows = command.ExecuteNonQuery();

                            ClearError();
                        }
                        catch (Exception ex)
                        {
                            //something went wrong, most likely the value is already in the table
                            ProcessException(ex);
                            num_rows = -1;
                        }
                    }
                }
            }
            return num_rows;
        }

        public int DeleteKeyword(int id)
        {
            int num_rows = 0;

            //double check that keyword is not empty
            string cmdText = "DELETE FROM REA_KEYWORD WHERE ID = @ID";

            if (id > 0)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = cmdText;

                        SqlParameter paramID = new SqlParameter("ID", id);
                        command.Parameters.Add(paramID);

                        try
                        {
                            num_rows = command.ExecuteNonQuery();

                            ClearError();
                        }
                        catch (Exception ex)
                        {
                            //something went wrong, most likely the value doesnt exist
                            ProcessException(ex);
                            num_rows = -1;
                        }
                    }
                }
            }
            return num_rows;
        }

        public DataTable GetKeywords()
        {
            string command = @"
SELECT 
	REA_KEYWORD.ID, REA_KEYWORD.Keyword, REA_KEYWORD.Description
FROM 
	REA_KEYWORD
 ORDER BY REA_KEYWORD.Keyword ASC";
            return this.ProcessCommand(command);
        }

        public DataTable GetKeywords(int REA)
        {
            string command = @"
SELECT 
	REA_KEYWORD.ID, REA_KEYWORD.Keyword, REA_KEYWORD.Description
FROM 
	REA_KEYWORD_REA_RELATION
INNER JOIN REA_KEYWORD ON REA_KEYWORD.ID = REA_KEYWORD_REA_RELATION.KEYWORD_ID
WHERE 
	REA_KEYWORD_REA_RELATION.TRACKING_ID = " + REA + " ORDER BY REA_KEYWORD.Keyword ASC";
            return this.ProcessCommand(command);
        }

        public DataTable GetKeywordsAvailable(int REA)
        {
            string command = @"
SELECT
	REA_KEYWORD.ID, Keyword, Description, 
    (SELECT
	    Count(*) 
     FROM
	    REA_KEYWORD_REA_RELATION 
    WHERE
	    REA_KEYWORD_REA_RELATION.TRACKING_ID = " + REA + @" 
	    AND  REA_KEYWORD_REA_RELATION.KEYWORD_ID = REA_KEYWORD.ID
    ) AS Used
FROM
	REA_KEYWORD
LEFT JOIN REA_KEYWORD_REA_RELATION ON REA_KEYWORD.ID = REA_KEYWORD_REA_RELATION.ID;";

            return this.ProcessCommand(command);
        }

        public bool UpdateKeywords(int REA, string keywordIDs)
        {
            bool success = false;

            if (REA > 0)
            {
                int current_keyword_count = (int)this.ProcessScalarCommand("SELECT count(*) FROM REA_KEYWORD_REA_RELATION WHERE TRACKING_ID = " + REA);
                if (current_keyword_count > 0)
                {
                    if (this.ExecuteNonQuery("DELETE FROM REA_KEYWORD_REA_RELATION WHERE TRACKING_ID = " + REA) == current_keyword_count)
                    {
                        success = true;
                    }
                }
                else
                {
                    //nothing to delete
                    success = true;
                }

                if (success)
                {
                    if (!String.IsNullOrWhiteSpace(keywordIDs))
                    {
                        String[] keywords = keywordIDs.Split(',');

                        String command = String.Empty;
                        foreach (string id in keywords)
                        {
                            command += "INSERT INTO REA_KEYWORD_REA_RELATION (TRACKING_ID, KEYWORD_ID) VALUES (" + REA + ", " + id + "); ";
                        }

                        int num_row = this.ExecuteNonQuery(command);
                        if (num_row != keywords.Length)
                        {
                            success = false;
                        }
                    }
                }
            }

            return success;
        }

    }
}
