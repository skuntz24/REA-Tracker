using System;
using System.Collections.Generic;
using System.Data;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{

    public class PriorityManagerViewModel
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public bool Before { get; set; }
        public int ExistingPriorityId { get; set; }
        public List<dynamic> DisplayPriorities { get; set; }

        public PriorityManagerViewModel()
        {
            this.DisplayPriorities = new List<dynamic>();
            this.init();
        }

        public void init()
        {
            ///<summary>
            /// the initiation function to populate the Priorities
            ///</summary>
            String command = "SELECT ID, NAME, DESCRIPTION, WEIGHT FROM REA_priority;";
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(command);
            List<dynamic> ListOfExistingItems = new List<dynamic>();
            int i = 0;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                ListOfExistingItems.Add(new System.Dynamic.ExpandoObject());
                ListOfExistingItems[i].ID = Convert.ToString(row[0]);
                ListOfExistingItems[i].Name = Convert.ToString(row[1]);
                ListOfExistingItems[i].Description = Convert.ToString(row[2] == DBNull.Value ? "None" : row[2]);
                ListOfExistingItems[i].Value = Convert.ToString(row[3]);
                i++;
            }
            this.DisplayPriorities = ListOfExistingItems;
        }

        public String add()
        {
            ///<summary>
            /// adds a new instance of Priorities
            ///</summary>
            String message = "";
            bool okToAdd = true;
            REATrackerDB sql = new REATrackerDB();
            List<String> ListOfNames = new List<String>();
            String command = "SELECT NAME, ID FROM REA_priority ORDER BY ID;";
            DataTable dtPriority = sql.ProcessCommand(command);
            if (String.IsNullOrEmpty(this.Name.Trim()))
            {
                okToAdd = false;
                message = "Must include a name.";
            }
            if (String.IsNullOrEmpty(this.Description.Trim()))
            {
                okToAdd = false;
                message = "Must include a description.";
            }

            if (okToAdd)
            {
                //sucessfully gets pass the check points
                int count = dtPriority.Rows.Count;
                //Get the weight of where the new row is going to have
                int NewWeight = count;
                //Determine if i place it before or after the weight
                if (!Before)
                {
                    NewWeight += 1;
                }
                sql.InsertPriority(Name, Description, NewWeight, NewWeight);
                message = "Successfully added " + Convert.ToString(this.Name) + " with a value of " + Convert.ToString(NewWeight) + ".";
            }
            return message;
        }

        private bool NameAlreadyExists(String name, int id)
        {
            ///<summary>
            /// checks to see if a name already exists
            ///</summary>
            ///<param name="name">
            /// the name we are checking
            /// </param>
            /// <param>
            /// the id of the instace we are checking
            /// </param>
            Boolean result = false;
            List<String> ListOfNames = new List<String>();
            String command = "SELECT NAME, ID FROM REA_priority ORDER BY ID;";
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(command);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                ListOfNames.Add(Convert.ToString(row[0]));
            }
            if (Convert.ToInt32(dt.Rows[ListOfNames.IndexOf(name)][1]) != id)
            {
                result = true;
            }
            return result;
        }
    }

}