using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Text;
using System.Web.Routing;
using System.Data;
using QVICommonIntranet.Database;
using System.Web.Mvc.Html;
using System.Net;

namespace HelperMethods.Infrastructure
{
    public static class QVIHtmlHelper
    {
        static Random rnd = new Random((int)DateTime.Now.Millisecond);
        public enum Roles
        {
            None = 0,
            Vendor,
            User,
            Developer,
            Tester,
            Manager,
            TechWriter,
            Leader
        };
        public static MvcHtmlString WebPage(this HtmlHelper htmlHelper, string url)
        {
            return MvcHtmlString.Create(new WebClient().DownloadString(url));
        }

        //using stringbulder and mvchtmlcreate
        public static MvcHtmlString Nl2Br(this HtmlHelper htmlHelper, string text)
        {
            MvcHtmlString htmlReturn = default(MvcHtmlString); ;
            if (string.IsNullOrEmpty(text))
            {
                htmlReturn = MvcHtmlString.Create(text);
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i > 0)
                        builder.Append("<br/>");
                    builder.Append(HttpUtility.HtmlEncode(lines[i]));
                }
                htmlReturn = MvcHtmlString.Create(builder.ToString());
            }
            return htmlReturn;
        }

        //2nd helper for escape characters
        public static MvcHtmlString Nl2BrEscapeCharacters(this HtmlHelper htmlHelper, string text)
        {
            MvcHtmlString htmlReturn = default(MvcHtmlString); ;
            if (string.IsNullOrEmpty(text))
            {
                htmlReturn = MvcHtmlString.Create(text);
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                String[] splitOn = {"\\n"};
                string[] lines = text.Split( (string[]) splitOn,StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i > 0)
                        builder.Append("<br/>");
                    builder.Append(HttpUtility.HtmlEncode(lines[i]));
                }
                htmlReturn = MvcHtmlString.Create(builder.ToString());
            }
            return htmlReturn;
        }
        private static string N12Br(string text)
        {
            String htmlReturn = "";
            if (string.IsNullOrEmpty(text))
            {
                htmlReturn = "";
            }
            else
            {
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i > 0)
                        htmlReturn += "<br/>";
                    htmlReturn += lines[i];
                }
            }
            return htmlReturn;
        }
        //Gets fullname and returns a href, Dont use in large loops its bad
        public static HtmlString FullNameHref(this HtmlHelper help, int stUserID, String options)
        {
            string fullname = "";
            REATrackerDB sql = new REATrackerDB();
            DataTable dt = sql.ProcessCommand("Select FIRST_NAME, LAST_NAME FROM ST_USERS WHERE USER_ID = " + Convert.ToString(stUserID));
            if (dt.Rows.Count > 0)
            {
                fullname = Convert.ToString(dt.Rows[0][0]) + " " + Convert.ToString(dt.Rows[0][1]);
            }
            String output = "<a onclick=\"javascript: popUp(\"" + "/" + "Account" + "/" + "ViewUser" + "/" + stUserID + "\")\" " + options + ">" + fullname + "</a>";
            return new HtmlString(output);
        }

        public static HtmlString UserPopUpHref( this HtmlHelper help, int stUserID, String FullName, String Options) 
        {
            String output = "<a style='cursor:hand; cursor:pointer;' onclick=\"javascript: popUp('" + "/" + "Account" + "/" + "ViewUser" + "/" + stUserID + "')\" " + Options + ">" + FullName + "</a>";
            return new HtmlString(output);
        }
        //method added for metric reports
        public static HtmlString ViewReport(this HtmlHelper help, int productID, int type, int metric, string MajorVersion, string MinorVersion)
        {
            string output = "<a class='fa fa-lg fa-file-text' style='cursor:hand; color:Black; cursor:pointer;' onclick=\"javascript: window.open('" + "/" + "Home" + "/" + "Report" +"?"+ "product" + "=" + productID + "&" + "type" + "=" + type + "&" + "metric" + "=" + metric + "&" + "MajorVersion" + "=" + MajorVersion + "&"+ "MinorVersion" + "=" + MinorVersion + "')\" </a>"; 
            return new HtmlString(output);
        }
        //using tagbuilder
        public static MvcHtmlString Image(this HtmlHelper helper, string src, string alt)
        {
            // Build <img> tag
            TagBuilder tb = new TagBuilder("img");
            // Add "src" attribute
            tb.Attributes.Add("src", VirtualPathUtility.ToAbsolute(src));
            // Add "alt" attribute
            tb.Attributes.Add("alt", alt);
            // return MvcHtmlString. This class implements IHtmlString
            // interface. IHtmlStrings will not be html encoded.
            return new MvcHtmlString(tb.ToString(TagRenderMode.SelfClosing));
        }

        //using existing html helpers
        public static MvcHtmlString TextBox<TModel, TValue>(this HtmlHelper<TModel> help, 
            Expression<Func<TModel, TValue>> expression, bool isEditable = false)
        {
            MvcHtmlString html = default(MvcHtmlString);
            if (isEditable)
            {
                html = System.Web.Mvc.Html.InputExtensions.TextBoxFor(help, expression);
            }
            else
            {
                html = System.Web.Mvc.Html.DisplayExtensions.DisplayFor(help, expression);
            }
            return html;
        }
        // Get html attributes working

        public static MvcHtmlString TextBox<TModel, TValue>(this HtmlHelper<TModel> help, 
            Expression<Func<TModel, TValue>> expression, bool isEditable = false, object htmlAttributes = null)
        {
            MvcHtmlString html = default(MvcHtmlString);
            if (isEditable)
            {
                html = System.Web.Mvc.Html.InputExtensions.TextBoxFor(help, expression, new RouteValueDictionary(htmlAttributes));
            }
            else
            {
                html = System.Web.Mvc.Html.DisplayExtensions.DisplayFor(help, expression, new RouteValueDictionary(htmlAttributes));
            }
            return html;
        }

        //Icons
        public static HtmlString PriorityIcon(this HtmlHelper help, string p, string size = "", bool label = false)
        {
            bool IsAprilFools = (DateTime.Now.Month == 4) && (DateTime.Now.Day == 1);
            string output = p;
            string rotate = "";
            
            if (IsAprilFools)
            {
                int r = rnd.Next(1, 7);
                switch (r)
                {
                    case 1: { rotate = "fa-rotate-90"; break; }
                    case 2: { rotate = "fa-rotate-180"; break; }
                    case 3: { rotate = "fa-rotate-270"; break; }
                    case 4: { rotate = "fa-flip-horizontal"; break; }
                    case 5: { rotate = "fa-flip-vertical"; break; }
                    case 6: { rotate = "fa-spin"; break; }
                    default: { rotate = ""; break; }
                }
            }

            switch (p)
            {
                case "4":
                case "Critical":
                    {
                        output = $"<i class='glyphicon glyphicon-fire {size} {rotate} qvi-color-critical'  title='Critical'></i>";
                        if (label) { output += " &nbsp;&nbsp;Critical "; }
                        break;
                    }
                case "3":
                case "High":
                    {
                        output = $"<i class='fa fa-fire {size} {rotate} qvi-color-high' title='High'></i>";
                        if (label) { output += " &nbsp;&nbsp;High "; }
                        break;
                    }
                case "2":
                case "Medium":
                    {
                        output = $"<i class='fa fa-exclamation-triangle {size} {rotate} qvi-color-medium' title='Medium'></i>";
                        if (label) { output += " &nbsp;&nbsp;Medium "; }
                        break;
                    }
                case "1":
                case "Low":
                    {
                        output = $"<i class='fa fa-info-circle {size} {rotate} qvi-color-low' title='Low'></i>";
                        if (label) { output += " &nbsp;&nbsp;Low "; }
                        break;
                    }
            }
            return new HtmlString(output);
        }

        public static HtmlString ReleaseIcon(this HtmlHelper help, string p, string size = "", bool label = false)
        {
            string output = p;
            switch (p)
            {
                case "7":
                    {
                        output = $"<i style='color: #3399FF' class='fa fa-truck {size}' title='Outgoing Inspection Distribution'></i>";
                        if (label) { output += " &nbsp;&nbsp;Outgoing Inspection "; }
                        break;
                    }
                case "6":
                    {
                        output = $"<i style='color: #3399FF' class='fa fa-building {size}' title='Manufacturing Distribution'></i>";
                        if (label) { output += " &nbsp;&nbsp;Manufacturing "; }
                        break;
                    }
                case "5":
                    {
                        output = $"<i style='color: #CC0000' class='fa fa-ban {size}' title='Recalled Customer Release'></i>";
                        if (label) { output += " &nbsp;&nbsp;Recalled Customer Release "; }
                        break;
                    }
                case "4":
                    {
                        output = $"<i style='color: #CC0000' class='fa fa-users {size}' title='Customer Release for Restricted Distribution'></i>";
                        if (label) { output += " &nbsp;&nbsp;Customer Release for Restricted Distribution "; }
                        break;
                    }
                case "3":
                    {
                        output = $"<i style='color: #3399FF' class='fa fa-users {size}' title='Customer Release for Limited Distribution'></i>";
                        if (label) { output += " &nbsp;&nbsp;Customer Release for Limited Distribution "; }
                        break;
                    }
                case "2":
                    {
                        output = $"<i style='color: #00CC00' class='fa fa-users {size}' title='Customer Release'></i>";
                        if (label) { output += " &nbsp;&nbsp;Customer Release "; }
                        break;
                    }
                case "1":
                    {
                        output = $"<i style='color: #000000' class='fa fa-user {size}' title='Internal Release Only'></i>";
                        if (label) { output += " &nbsp;&nbsp;Internal Release Only "; }
                        break;
                    }
                default:
                    {
                        output = $"<i style='color: #000000' class='fa fa-frown {size}' title='Unknown Status'></i>";
                        if (label) { output += " &nbsp;&nbsp;Unknown Status "; }
                        break;
                    }
            }
            return new HtmlString(output);
        }
        public static HtmlString IssueIcon(this HtmlHelper help, string p, string size = "", bool label = false)
        {
            string output = p;

            switch (p)
            {
                case "3":
                case "Planned Work":
                    {
                        output = $"<i class='fa fa-tasks {size}' title='Planned Work'></i>";
                        if (label) { output += " &nbsp;&nbsp;Planned Work "; }
                        break;
                    }
                case "2":
                case "Enhancement":
                    {
                        output = $"<i class='fa fa-plus-circle {size}' title ='Enhancement'></i>";
                        if (label) { output += " &nbsp;&nbsp;Enhancement "; }
                        break;
                    }
                case "1":
                case "Problem":
                    {
                        output = $"<i class='fa fa-bug {size}' title='Problem'></i>";
                        if (label) { output += " &nbsp;&nbsp;Problem "; }
                        break;
                    }
            }
            return new HtmlString(output);
        }
        public static HtmlString CheckIcon(this HtmlHelper help, string p, string size = "")
        {
            string output = "";
            switch (p.ToLower())
            {
                case "true":
                case "":
                    {
                        output = "<i class='fa fa-check " + size + "'></i>";
                        break;
                    }
                case "false":
                    {
                        output = "No";
                        break;
                    }

            }
            return new HtmlString(output);
        }
        //report icon
        //public static HtmlString ReportIcon(this HtmlHelper help, string p, string size = "")
        //{
        //    string output = "<i style ='cursor:hand; cursor:pointer; color:black;padding-left:30px'; onclick =\'javascript: window.popUp('" + "/" + "Home" + "/" + "Report" + "?" + "product" + "')' class='fa fa-lg fa-file-text'></i>";
        //    switch (p.ToLower())
        //    {
        //        case "true":
        //        case "":
        //            {
        //                output = "<i class='fa fa-lg fa - file - text " + size + "'></i>";
        //                break;
        //            }
        //        case "false":
        //            {
        //                output = "No";
        //                break;
        //            }

        //    }
          
        //    return new HtmlString(output);
        //}
        public static HtmlString Changes(this HtmlHelper help, string p, string size = "")
        {
            string output = "No";
            if(p!=null)
            {
                output = p.ToLower() == "true" ? "Yes" : "No";
            }
            return new HtmlString(output);
        }
        public static HtmlString StatusToHiddenStatusID(this HtmlHelper help, string p)
        {
            string output = "<div class='hidden'>";
            if (p != null)
            {
                string command = "SELECT STATUS_ID FROM ST_STATUS WHERE ST_STATUS.NAME ='" + p+"'";
                output += new REATrackerDB().ProcessScalarCommand(command);
            }
            output += "</div>";
            return new HtmlString(output);
        }
       

        //Display String
        public static HtmlString ViewHistory(this HtmlHelper help, string description, int type, bool display_html = false)
        {
            string output = "";

            if (!display_html)
            {
                description = description.Replace("<", "&lt;");
                description = description.Replace(">", "&gt;");
            }
            if (description != "")
            {
                //1,2,16,17,18,19()normal 4,5,6 
                //3,  bold and italic
                //7, red bold italic
                //8 related scr, link to a new scr
                //9-15 set to hours and respective names
                //22 is added for status_change
                switch (type)
                {
                    case (int)REATrackerDB.HistoryChangeType.chg_detail:
                    case (int)REATrackerDB.HistoryChangeType.chg_note:
                        {
                            if (display_html)
                            {
                                output = $"{description}"; //future if we allow markup language
                            }
                            else
                            {
                                output = $@"<pre style='white-space: pre-wrap; word-break: break-word; background-color: unset; border: unset; border-radius: unset; padding: unset;'>{description}</pre>";
                            }
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_resolution:
                        {
                            //output = "<b><i>" + N12Br(description) + "</i></b>";
                            output = $@"<pre style='white-space: pre-wrap; word-break: break-word; background-color: unset; border: unset; border-radius: unset; padding: unset; font-weight: bold; font-size: 11pt; font-style: italic;'>{description}</pre>";
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_ser_priority:
                    case (int)REATrackerDB.HistoryChangeType.chg_ser_problem:
                    case (int)REATrackerDB.HistoryChangeType.chg_system:
                    case (int)REATrackerDB.HistoryChangeType.chg_product:
                    case (int)REATrackerDB.HistoryChangeType.chg_ser_solution:
                    case (int)REATrackerDB.HistoryChangeType.chg_ser_benefits:
                    case (int)REATrackerDB.HistoryChangeType.chg_assignto:
                    case (int)REATrackerDB.HistoryChangeType.chg_issuetype:
                    case (int)REATrackerDB.HistoryChangeType.chg_priority:
                    case (int)REATrackerDB.HistoryChangeType.chg_related:
                    case (int)REATrackerDB.HistoryChangeType.chg_attachment_add:
                    case (int)REATrackerDB.HistoryChangeType.chg_status:
                    case (int)REATrackerDB.HistoryChangeType.chg_rank:
                    case (int)REATrackerDB.HistoryChangeType.chg_size:
                        {
                            output = N12Br(description);
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_exception:
                        {
                            output = "<FONT COLOR='#ff0000'><b><i> EXECPTION STATUS: " + N12Br(description) + "</i></b></FONT>";
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_planned_release:
                        {
                            output = "Planned Release Version changed to " + description;
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_actual_release:
                        {
                            output = "Actual Release Version changed to " + description;
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_estimate_to_spec:
                        {
                            output = "Planned Hours To Specify changed to " + description;// +" hours.";
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_actual_to_spec:
                        {
                            output = "Actual Hours To Specify changed to " + description;// +" hours.";
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_estimate_to_fix:
                        {
                            output = "Planned Hours To Fix changed to " + description;// +" hours.";
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_actual_to_fix:
                        {
                            output = "Actual Hours To Fix changed to " + description;// +" hours.";
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_estimate_to_test:
                        {
                            output = "Planned Hours To Test changed to " + description;// +" hours.";
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_actual_to_test:
                        {
                            output = "Actual Hours To Test changed to " + description;// +" hours.";
                            break;
                        }
                    default:
                        {
                            output = N12Br($"[UNHANDLED TYPE = {type}] " + description);
                            break;
                        }

                }//Swtich

            }//if
            else
            {
                switch (type)
                {
                    case (int)REATrackerDB.HistoryChangeType.chg_planned_release:
                        {
                            output = "Planned Release Version has been cleared.";
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_actual_release:
                        {
                            output = "Actual Release Version has been cleared.";
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_estimate_to_spec:
                        {
                            output = "Planned Hours To Specify has been cleared. ";
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_actual_to_spec:
                        {
                            output = "Actual Hours To Specify has been cleared.";
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_estimate_to_fix:
                        {
                            output = "Planned Hours To Fix has been cleared.";
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_actual_to_fix:
                        {
                            output = "Actual Hours To Fix has been cleared.";
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_estimate_to_test:
                        {
                            output = "Planned Hours To Test has been cleared.";
                            break;
                        }
                    case (int)REATrackerDB.HistoryChangeType.chg_actual_to_test:
                        {
                            output = "Actual Hours To Test has been cleared.";
                            break;
                        }
                    default:
                        {
                            output = N12Br($"[UNHANDLED TYPE = {type}] No Description");
                            break;
                        }
                }//switch
            }//else
            return new HtmlString(output);
        }

        public static HtmlString DisplayMessage(this HtmlHelper help, bool success, string message)
        {
            string output = "";
            if (!string.IsNullOrWhiteSpace(message))
            {
                output += "<div class='form-group'>";
                output += "<div class='alert ";
                output += success ? "alert-success" : "alert-danger";
                output += " alert-dismissible role ='alert'>";
                output += "<button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button>";
                output += message + "</div>";
                output += "</div>";
            }   
            return new HtmlString(output);
        }

        public static HtmlString DisplaySelectList(this HtmlHelper help, string name, List<dynamic> List)
        {
            string output = "";
            output += "<select id='" + name + "' name='" + name + "' class='form-control'>";
            if( List != null)
            {
                foreach (var item in List)
                {
                    output += "<option value=" + (item.ID == null ? 0 : item.ID) + " " + (item.IsDefault == null ? "" : item.IsDefault) + "> " + (item.Name == null ? "": item.Name ) + " </option>";
                }
            }
            output += "</select>";
            return new HtmlString(output);
        }

        public static HtmlString DisplaySelectList(this HtmlHelper help, string name, List<dynamic> List, string attributes)
        {
            bool open_group = false;
          bool default_has_been_set = false;
            string output = "";
            output += "<select id='" + name + "' name='" + name + "' "+attributes+">";
            foreach (var item in List)
            {
                if( item.ID != null )
                {
                    if (Convert.ToString(item.ID) == "-1")
                    {
                        if (open_group)
                        {
                            //close the previous group
                            output += "</optgroup>";
                        }
                        //option group
                        output += "<optgroup label=\"" + item.Name + "\">";
                        open_group = true;
                    }
                    else
                    {
                        //this makes sure that the default is only set once, first come first serve
                        if (item.IsDefault && !default_has_been_set)
                        {
                            output += "<option value=" + item.ID + " selected>" + item.Name + "</option>";
                           default_has_been_set = true;
                          
                        }
                        else
                        {
                            output += "<option value=" + item.ID + ">" + item.Name + "</option>";
                        }
                    }
                }
            }
            if (open_group)
            {
                //close the previous group
                output += "</optgroup>";
            }

            output += "</select>";
            return new HtmlString(output);
        }

        public static HtmlString DisplaySelectListForSearch(this HtmlHelper help, string name, List<dynamic> List, List<dynamic> SelectedList)
        {
            bool open_group = false;
            string last_group = "";
            string output = "";
            output += $@"
            <select class=""form-control""
                    ondblclick=""javascript: SearchSelectMoveRows(form.LeftSelectTagProducts, form.RightSelectTagProducts)""
                    id=""{name}""
                    multiple=""multiple"" 
                    name=""{name}"" 
                    size=""14"" 
                    style=""width:300px"">
            ";
            
            var distinct_list = List.GroupBy(x => x.ID).Select(y => y.First());
            foreach (var item in distinct_list)
            {
                if (item.ID != null)
                {
                    if (last_group != item.Group)
                    {
                        if (open_group)
                        {
                            //close the previous group
                            output += @"</optgroup>";
                        }
                        //option group
                        output += $@"       

<optgroup label=""{item.Group}"">";
                        open_group = true;
                        last_group = item.Group;
                    }
                    if (SelectedList.Where(x => x.ID == item.ID).Count() == 0)
                    {
                        output += $@"<option value={item.ID}>{item.Name}</option>";
                    }
                }
            }
            if (open_group)
            {
                //close the previous group
                output += @"</optgroup>";
            }

            output += "</select>";
            return new HtmlString(output);
        }

        public static HtmlString DisplaySelectListNullable(this HtmlHelper help, string name, List<dynamic> List, string attributes)
        {
            //adds a nullable feild
            string output = "";
            output += "<select id='" + name + "' name='" + name + "' " + attributes + ">";
            output += "<option value=''>  </option>";
            foreach (var item in List)
            {
                if (item.IsDefault)
                {
                    output += "<option value=" + item.ID + " selected> " + item.Name + " </option>";
                }
                else
                {
                    output += "<option value = " + item.ID + " >" + item.Name + "</option>";
                }
            }
            output += "</select>";
            return new HtmlString(output);
        }

        public static HtmlString DisplaySelectListNullableDefault(this HtmlHelper help, 
            string name, int? DefaultValue, List<dynamic> List, string attributes) 
        {
            //adds a nullable feild
            string output = "";
            output += "<select id='" + name + "' name='" + name + "' " + attributes + ">";
            output += "<option value=''>  </option>";
            foreach (var item in List)
            {
                output += "<option value=" + item.ID + " ";
                if( DefaultValue != null && item.ID == DefaultValue )
                {
                    output += " selected ";
                }
                output += "> " + item.Name + " </option>";
            }
            output += "</select>";
            return new HtmlString(output);
        }
        //-------------------------
        // Access                   |
        //-------------------------
        public static HtmlString AccessForTextBox<TModel, TValue>(this HtmlHelper<TModel> help, 
            Expression<Func<TModel, TValue>> expression, bool isEditable = false, bool isAllowEdit = false, object htmlAttributes = null)
        {
            //MvcHtmlString html = default(MvcHtmlString);
            StringBuilder html = new System.Text.StringBuilder();
            if (isEditable)
            {
                html.Append(System.Web.Mvc.Html.InputExtensions.TextBoxFor(help, expression, new RouteValueDictionary(htmlAttributes)));
            }
            else
            {
                html.Append(System.Web.Mvc.Html.DisplayExtensions.DisplayFor(help, expression, new RouteValueDictionary(htmlAttributes)));
                html.Append(System.Web.Mvc.Html.InputExtensions.HiddenFor(help,expression));
            }
            HtmlString str = new HtmlString(html.ToString());
            if (!isAllowEdit)
            {
                int length = (str.ToHtmlString().Length) - 2;
                str = new HtmlString(str.ToHtmlString().Insert(length, "ReadOnly"));
            }
            return str;
        }

        public static HtmlString AccessForTextBox<TModel, TValue>(this HtmlHelper<TModel> help,
            Expression<Func<TModel, TValue>> expression, bool isEditable = false, object htmlAttributes = null)
        {
            //MvcHtmlString html = default(MvcHtmlString);
            StringBuilder html = new System.Text.StringBuilder();
            if (isEditable)
            {
                html.Append(System.Web.Mvc.Html.InputExtensions.TextBoxFor(help, expression, new RouteValueDictionary(htmlAttributes)));
            }
            else
            {
                html.Append(System.Web.Mvc.Html.DisplayExtensions.DisplayFor(help, expression, new RouteValueDictionary(htmlAttributes)));
                html.Append(System.Web.Mvc.Html.InputExtensions.HiddenFor(help, expression));
            }
            var str = new HtmlString(html.ToString());            
            return str;
        }

        public static HtmlString AccessForTextBox(this HtmlHelper help, 
            String name, String value, bool isEditable = false, string htmlAttributes = null) 
        {
            string output = "";
            //String.Format("var str = \"{0}\"", value);
            value = value.Replace("\"", "&quot;");
            if(isEditable)
            {
                output += @"<input type='text' id='" + name + "' name='" + name + "' value = \"" + value + "\"" + htmlAttributes + ">";
            }
            else
            {
                output += value;
                output += @"<input type='hidden' id=" + name + " name=" + name + " value= \"" + value + "\">";
            }
            return new HtmlString(output);
        }
        public static HtmlString AccessForSelectList(this HtmlHelper help,
            String name ,List<dynamic>ItemList ,bool isEditable = false, string htmlAttributes = null)
        {
            string output = "";
            if (ItemList.Count < 2)
            {
                isEditable = false;
            }
            if (isEditable)
            {
                output += "<select id = '" + name + "' name = '" + name + "' " + htmlAttributes + ">";
                foreach (var item in ItemList)
                {
                    if (item.IsDefault)
                    {
                        output += "<option value = " + item.ID + " selected>" + item.Name + "</option>";
                    }
                    else
                    {
                        output += "<option value = " + item.ID + " >" + item.Name + "</option>";
                    }
                }
                output+="</select>";
            }
            else
            {
                bool hasVal = false;
                foreach (var item in ItemList)
                {
                    if (item.IsDefault) 
                    {
                        hasVal = true;
                        output += "<input type='hidden' id="+name+" name="+name+" value= "+item.ID+" >";
                        output += item.Name;
                        break;
                    }
                }
                if(hasVal == false)
                {
                    output += "<input type='hidden' id=" + name + " name=" + name + " value >";
                }
            }
            return new HtmlString(output);
        }
        //Chart
        //--------------------------
        //Report Table              |
        //--------------------------
     
        public static HtmlString ReportTable(this HtmlHelper help, 
            List<int>argsProblem, List<int>argsEnhancement, List<int>argsPlanned,
            int? productID = null, string datefield="",
            DateTime? startDate = null, DateTime? endDate = null)
        {
            List<int> ProblemMetrics = GetMetricFromList(argsProblem);
            List<int> EnhancementMetrics = GetMetricFromList(argsEnhancement);
            List<int> PlannedMetrics = GetMetricFromList(argsPlanned);
            List<int> total = TotalResults(argsProblem, argsEnhancement, argsPlanned);
            List<int> MetricsTotal = GetMetricFromList(total);
            int startdate=0;
            int enddate = 0;
            bool timereport = false;
            if(productID != null  && !string.IsNullOrEmpty(datefield) && startDate != null && endDate != null )
            {
                timereport = true;
            }
            if(timereport)
            {
                startdate = Convert.ToInt32(
                    Convert.ToString(((DateTime)startDate).Year)+
                    ((DateTime)startDate).ToString("MM")+
                    ((DateTime)startDate).ToString("dd")
                );
                enddate = Convert.ToInt32(
                    Convert.ToString(((DateTime)endDate).Year)+
                    ((DateTime)endDate).ToString("MM")+
                    ((DateTime)endDate).ToString("dd")
                );
            }
            String output = "<table class='table table-bordered table-hover table-striped'> "
                +"<thead>"
                +"    <tr>"
                +"        <th></th>"
                +"        <th align='center' colspan='2'><b>&Sigma;</b>&nbsp;Total</th>"
                + "        <th align='center' colspan='2'><i class='fa fa-bug'></i>&nbsp;Problem</th>"//Add in icons
                + "        <th align='center' colspan='2'><i class='fa fa-plus-circle'></i>&nbsp;Enhancement</th>"
                + "        <th align='center' colspan='2'><i class='fa fa-tasks'></i>&nbsp;Planned Work</th>"
                +"    </tr>                                                                                      "
                +"    <tr>                                                                                       "
                +"        <th></th>                                                                              "
                +"        <th align='center'> Metric </th>                                                       "
                +"        <th align='center'> Count </th>                                                        "
                +"        <th align='center'> Metric </th>                                                       "
                +"        <th align='center'> Count </th>                                                        "
                +"        <th align='center'> Metric </th>                                                       "
                +"        <th align='center'> Count </th>                                                        "
                +"        <th align='center'> Metric </th>                                                       "
                +"        <th align='center'> Count </th>                                                        "
                +"    </tr>                                                                                      "
                +"</thead>                                                                                       "
                +"<tbody>                                                                                        "
                +"<tr>                                                                                           "
                +"      <td> <i class='fa fa-info-circle qvi-color-low'></i>&nbsp;<b>Low</b> </td>               "
                +"      <td align='center'>"+MetricsTotal[0]           + "   </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&priority=1' a>") + total[0] + (timereport ? "" : "<a>") + "   </td>                               "
                +"      <td align='center'>"+ProblemMetrics[0] +"  </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&issuetype=1&priority=1' a>") + argsProblem[0] + (timereport ? "" : "<a>") + "   </td>                               "
                +"      <td align='center'>"+EnhancementMetrics[0]     +"   </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&issuetype=-1&priority=1' a>") + argsEnhancement[0] + (timereport ? "" : "<a>") + "   </td>                               "
                +"      <td align='center'>"+PlannedMetrics[0]            +"   </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&issuetype=9&priority=1' a>") + argsPlanned[0] + (timereport ? "" : "<a>") + "   </td>                               "
                +"</tr>                                                                                          "
                +"<tr>                                                                                           "
                + "       <td> <i class='fa fa fa-exclamation-triangle  qvi-color-medium'></i>&nbsp;<b>Medium</b> </td>       "
                +"      <td align='center'>"+MetricsTotal[1]           +"   </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&priority=2' a>") + total[1] + (timereport ? "" : "<a>") + "   </td>                               "
                +"      <td align='center'>"+ProblemMetrics[1]         +"   </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&issuetype=1&priority=2' a>") + argsProblem[1] + (timereport ? "" : "<a>") + "   </td>                               "
                +"      <td align='center'>"+EnhancementMetrics[1]     +"   </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&issuetype=-1&priority=2' a>") + argsEnhancement[1] + (timereport ? "" : "<a>") + "   </td>                               "
                +"      <td align='center'>"+PlannedMetrics[1]          +"   </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&issuetype=9&priority=2' a>") + argsPlanned[1] + (timereport ? "" : "<a>") + "   </td>  "
                +"</tr>                                                                                          "
                +"<tr>                                                                                           "
                +"        <td> <i class='fa fa-fire qvi-color-high'></i>&nbsp;<b>High</b> </td>           "
                +"      <td align='center'>"+MetricsTotal[2]           +"   </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&priority=3' a>") + total[2] + (timereport ? "" : "<a>") + "   </td>                               "
                +"      <td align='center'>"+ProblemMetrics[2]         +"   </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&issuetype=1&priority=3' a>") + argsProblem[2] + (timereport ? "" : "<a>") + "   </td>                               "
                +"      <td align='center'>"+EnhancementMetrics[2]     +(timereport?"":"<a>")+"   </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&issuetype=-1&priority=3' a>") + argsEnhancement[2] + (timereport ? "" : "<a>") + "   </td>                               "
                +"      <td align='center'>"+PlannedMetrics[2]            +"   </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&issuetype=9&priority=3' a>") + argsPlanned[2] + (timereport ? "" : "<a>") + "   </td>  "
                +"</tr>                                                                                          "
                +"<tr>                                                                                           "
                + "        <td> <i class='glyphicon glyphicon-fire qvi-color-critical'></i>&nbsp;<b>Critical</b> </td>   "
                +"      <td align='center'>"+MetricsTotal[3]           +"   </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&priority=4' a>") + total[3] + (timereport ? "" : "<a>") + "   </td>                               "
                +"      <td align='center'>"+ProblemMetrics[3]         +"   </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&issuetype=1&priority=4' a>") + argsProblem[3] + (timereport ? "" : "<a>") + "   </td>                               "
                +"      <td align='center'>"+EnhancementMetrics[3]     + "   </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&issuetype=-1&priority=4' a>") + argsEnhancement[3] + (timereport ? "" : "<a>") + "   </td>                               "
                +"      <td align='center'>"+PlannedMetrics[3]            +"   </td>                               "
                + "      <td align='center'>" + (!timereport ? "" : "<a href='/Home/Report?product=" + productID + "&type=14&datefield=" + datefield + "&startDate=" + startdate + "&endDate=" + enddate + "&issuetype=9&priority=4' a>") + argsPlanned[3] + (timereport ? "" : "<a>") + "   </td>  "
                +"</tr>                                                                                          "
                +"</tbody>                                                                                       "
                +"<tfoot>                                                                                        "
                +"    <tr>                                                                                       "
                +"        <td> <b>&Sigma;</b>&nbsp;<b>Total</b> </td>                                            "
                + "        <td align='center'><b>"+(MetricsTotal[3] + MetricsTotal[2] + MetricsTotal[1] + MetricsTotal[0] )                     +"</b></td>"
                + "        <td align='center'><b>"+(total[3] + total[2] + total[1] + total[0])                                                  +"</b></td>"
                +"        <td align='center'> <b>"+(ProblemMetrics[3]+ProblemMetrics[2]+ProblemMetrics[1]+ProblemMetrics[0] )                   +"</b></td>"
                + "        <td align='center'><b>"+(argsProblem[3] + argsProblem[2] + argsProblem[1] + argsProblem[0])                          +"</b></td>"
                +"        <td align='center'> <b>"+(EnhancementMetrics[3]+EnhancementMetrics[2]+EnhancementMetrics[1]+EnhancementMetrics[0])    +"</b></td>"
                +"        <td align='center'> <b>"+(argsEnhancement[3]+argsEnhancement[2]+argsEnhancement[1]+argsEnhancement[0])                +"</b></td>"
                +"        <td align='center'> <b>"+(PlannedMetrics[3]+PlannedMetrics[2]+PlannedMetrics[1]+PlannedMetrics[0])                    +"</b></td>"
                +"        <td align='center'> <b>"+(argsPlanned[3]+argsPlanned[2]+argsPlanned[1]+argsPlanned[0])                                +"</b></td>"
                +"    </tr>"
                +"</tfoot> "
                +"</table> ";
            return new HtmlString(output);
        }

        private static List<int> GetMetricFromList(List<int> argsInput) 
        {
            List<int> Results = new List<int>();
            Results.Add(argsInput[0]);
            Results.Add(argsInput[1]*4);
            Results.Add(argsInput[2]*9);
            Results.Add(argsInput[3]*16);
            return Results;
        }
        private static List<int> TotalResults(List<int> argsProblem, List<int> argsEnhancement, List<int> argsPlanned) 
        {
            List<int> Results = new List<int>();
            Results.Add(argsProblem[0] + argsEnhancement[0] + argsPlanned[0]);
            Results.Add(argsProblem[1] + argsEnhancement[1] + argsPlanned[1]);
            Results.Add(argsProblem[2] + argsEnhancement[2] + argsPlanned[2]);
            Results.Add(argsProblem[3] + argsEnhancement[3] + argsPlanned[3]);
            return Results;
        }

        //=============================
        // Metrics                     |
        //=============================
        public static HtmlString MetricsHyperLink(this HtmlHelper help, int product, int priority, bool displayAll, int value, bool isWhite=false) 
        {
            //int offset = 8;
            //int typeValue = offset+priority;
            string output = "<a href ='/Home/Report?product=" + product + "&priority=" + (priority);
            if(displayAll)
            {
                output += "&displayAll=true";
            }
            output += "&type=13' target='_blank'";
            if (isWhite)
            {
                output+="style='color:#FFFFFF'";
            }
                output+=">" + value + "</a>";
            return new HtmlString(output);
        }
        //-------------------------
        // Version                 |
        //-------------------------
        public static HtmlString VersionHiddenSorting(this HtmlHelper help, String version)
        {
            //int offset = 8;
            //int typeValue = offset+priority;
            string output = "<p class='hidden'> ";
            string[] versionArray = version.Trim().Split(new[] { '.', ' ' });
            for(int i =0; i<4; i++)
            {
                if (i < versionArray.Length)
                {
                    if (i < 3)
                    {
                        output += versionArray[i].PadLeft(4, '0');
                    }
                    else
                    {
                        output += versionArray[i];
                    }
                }
                else
                {
                    break;
                }
            }
            output += "</p>";
            return new HtmlString(output);
        }
        public static HtmlString TeamTreeView(this HtmlHelper help, String treelist, List<dynamic> TeamMembers)
        {
            string output = "";
            foreach(var TeamMember in TeamMembers)
            {
                output += "<li>";
                output += "<a href='#" + TeamMember.ID + "' data-toggle='tab'>";
                var TreeLevel = TeamMember.TreeLevel;
                if(TeamMember.IsManager == 1)
                {
                    output += "<i class='fa fa-user-secret' ></i>";
                    for(int i = 0; i <= TreeLevel * 5; i++)
                    {
                        output+= "&nbsp;";
                    }
                    output += "<b>"+TeamMember.Name+"</b>";
                    output += "<span class='badge pull-right'>" + TeamMember.Count + "</span>";
                }
                else
                {
                    output += "<i class='fa fa-user' ></i>";
                    for (int i = 0; i <= TreeLevel * 5; i++)
                    {
                        output += "&nbsp;";
                    }
                    output += "" + TeamMember.Name + "";
                    output += "<span class='badge pull-right'>" + TeamMember.Count + "</span>";
                }
                output += "</a>";
                output += "</li>";
            }
            return new HtmlString(output);
        }
        public static HtmlString StrSpacer(this HtmlHelper help, int num)
        {
            string output = "";
            output += String.Concat(Enumerable.Repeat("&nbsp;", num));
            return new HtmlString(output);
        }

        public static HtmlString MyRaw(this HtmlHelper help, string str)
        {
            return new HtmlString(str);
        }

        public static HtmlString MakeUserIcon(this HtmlHelper help, string initials, string name, string color)
        {
            //string output = "<i class='fa kv-icon kv-icon-default' style='cursor: default' data-toggle='tooltip' data-placement='top' title='Unknown User'>??</i>";
            string output = $"<i class='fa kv-icon {color}' style='cursor: default' data-toggle='tooltip' data-placement='top' title='{name}'>{initials}</i>";
            return new HtmlString(output);
        }


        /// <summary>
        /// Gets the appropiate logo based on the current holiday
        /// </summary>
        /// <param name="help"></param>
        /// <returns>name of the file</returns>
        public static HtmlString GetLogo(this HtmlHelper help)
        {
            string logo = "realogo.png";
            if ((DateTime.Now.Month == 2) && (DateTime.Now.Day == 14))
            {
                logo = "logo_heart.png";
            }
            else if ((DateTime.Now.Month == 4) && (DateTime.Now.Day == 1))
            {
                logo = "logo_jester.png";
            }
            else if ((DateTime.Now.Month == 10) && (DateTime.Now.Day == 31))
            {
                logo = "logo_devil.png";
            }
            else if ((DateTime.Now.Month == 12))
            {
                logo = "logo_santa.png";
            }
            return new HtmlString(logo);
        }

        public static string GetTranslation(string translationKey, string parameter1)
        {
            string translation;
            translation = Resources.strings.ResourceManager.GetString(translationKey);
            translation = translation.Replace("%s", parameter1);
            return translation;
        }

    }
}