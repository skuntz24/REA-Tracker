using System;
using System.Collections.Generic;
using System.Data;
using QVICommonIntranet.Database;

namespace REA_Tracker.Models
{

    public class ProductHierarchyViewModel
    {
        public int NewParentProductID { get; set; }
        public int NewChildProductID { get; set; }
        public List<dynamic> ProductList { get; set; }
        public List<ProductRelation> ProductRelationList { get; set; }

        public ProductHierarchyViewModel()
        {
            this.init();
        }

        public void init()
        {
            ///<summary>
            /// main initiation function
            ///</summary>
            this.ProductList = this.GetProductList();
            this.ProductRelationList = this.GetRelation();
        }

        public List<ProductRelation> GetRelation()
        {
            ///<summary>
            /// returns a list of product relations
            ///</summary>
            List<ProductRelation> list = new List<ProductRelation>();
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.GetProductRelationShip();
            foreach (System.Data.DataRow row in dt.Rows)
            {
                list.Add(new ProductRelation(Convert.ToInt32(row[0]), Convert.ToInt32(row[1]), Convert.ToInt32(row[2])));
            }
            return list;
        }

        private List<dynamic> GetProductList()
        {
            ///<summary>
            /// returns a list of products 
            ///</summary>
            List<dynamic> list = new List<dynamic>();
            String command =
                "SELECT ST_PRODUCT.PRODUCT_ID, ST_PRODUCT.NAME " +
                "FROM ST_PRODUCT WHERE RETIRED != 1 order by NAME ASC;";
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand(command);
            int i = 0;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                list.Add(new System.Dynamic.ExpandoObject());
                list[i].ID = Convert.ToInt32(row[0]);
                list[i].Name = Convert.ToString(row[1]);
                list[i].IsDefault = "";
                i++;
            }
            return list;
        }
        //Post functions
        public void InsertProductRelationship()
        {
            ///<summary>
            /// Inserts a product relationship to the table
            ///</summary>
            REATrackerDB sql = new REATrackerDB();
            sql.InsertProductRelationShip(this.NewParentProductID, this.NewChildProductID);
        }

        public void RemoveRelation()
        {
            ///<summary>
            /// removes a relationship
            ///</summary>
            REATrackerDB sql = new REATrackerDB();
            foreach (ProductRelation rel in this.ProductRelationList)
            {
                if (rel.Remove)
                {
                    sql.RemoveRelation(rel.RelationID);
                }
            }
        }

        public String GetBuiltTree()
        {
            ///<summary>
            /// returns a built tree in string form
            ///</summary>
            String Root = "";
            REATrackerDB sql = new REATrackerDB();
            String GetAllParents = "SELECT DISTINCT PARENT_ID FROM ST_PRODUCT_RELATION WHERE PARENT_ID NOT IN(SELECT CHILD_ID FROM ST_PRODUCT_RELATION)";
            DataTable GetAllParentsDT = sql.ProcessCommand(GetAllParents);
            foreach (System.Data.DataRow row in GetAllParentsDT.Rows)
            {
                Root += BuildTree(Convert.ToInt32(row[0]), 1);
            }
            return Root;
        }

        private String BuildTree(int root, int level)
        {
            ///<summary>
            /// returns a html tree list
            ///</summary>
            ///<param name="root">
            /// the beginning of the tree 
            ///</param>
            ///<param name="level">
            /// the level that the current branch is on
            ///</param>

            REATrackerDB sql = new REATrackerDB();
            String TreeList = "";

            TreeList += "<ul id='BuiltTree'>";
            String productName = "SELECT NAME FROM ST_PRODUCT WHERE PRODUCT_ID = " + Convert.ToString(root);
            TreeList += ("<li>" + Convert.ToString(sql.ProcessScalarCommand(productName)) + "</li>");
            //Do you have CHILD_ID?
            String check = "SELECT COUNT(CHILD_ID) AS CHILDREN FROM ST_PRODUCT_RELATION WHERE PARENT_ID = " + Convert.ToString(root);
            DataTable checker = sql.ProcessCommand(check);
            if (Convert.ToInt32(checker.Rows[0][0]) > 0)
            {
                String children = "SELECT CHILD_ID FROM ST_PRODUCT_RELATION WHERE PARENT_ID = " + Convert.ToString(root);
                DataTable childrenDT = sql.ProcessCommand(children);
                foreach (System.Data.DataRow row in childrenDT.Rows)
                {
                    TreeList += BuildTree(Convert.ToInt32(row[0]), level + 1);
                }
            }
            TreeList += "</ul>";
            return TreeList;
        }

        public class ProductRelation
        {
            public int RelationID { get; set; }
            public int ParentID { get; set; }
            public int ChildID { get; set; }
            public string ParentName { get; set; }
            public string ChildName { get; set; }
            public bool Remove { get; set; }

            public ProductRelation() { }//Empty constructor for post 

            public ProductRelation(int relationShipID, int argsParentID, int argsChildID)
            {
                this.RelationID = relationShipID;
                this.ParentID = argsParentID;
                this.ChildID = argsChildID;
                this.ParentName = this.GetProductName(argsParentID);
                this.ChildName = this.GetProductName(argsChildID);
                this.Remove = false;
            }

            public ProductRelation(int relationShipID, int argsParentID, int argsChildID, string argsParentName, string argsChildName)
            {
                this.RelationID = relationShipID;
                this.ParentID = argsParentID;
                this.ChildID = argsChildID;
                this.ParentName = argsParentName;
                this.ChildName = argsChildName;
                this.Remove = false;
            }

            private string GetProductName(int ID)
            {
                ///<summary>
                /// gets a product name
                ///</summary>
                ///<param name="ID">
                /// THe id the product to get the name for
                ///</param>
                String name = "";
                REATrackerDB sql = new REATrackerDB();
                name = Convert.ToString(sql.ProcessScalarCommand("SELECT NAME FROM ST_PRODUCT WHERE PRODUCT_ID = " + Convert.ToString(ID)));
                return name;
            }
        }
    }


}