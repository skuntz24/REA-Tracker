using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using REA_Tracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using QVICommonIntranet.Database;
using QVICommonIntranet.Email.EmailREATracker;


namespace REA_Tracker.Controllers
{
    [Authorize(Roles = "Administrator")]
    [ValidateInput(false)]
    public class AdminController : Controller
    {
        private REATrackerDB _sql = new REATrackerDB();
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View();
        }

        //============================Application Settings==================================
        [HttpGet]
        public ActionResult ApplicationSettings()
        {
            //ViewBag.Message = msg;
            ApplicationSettingsViewModel model = new ApplicationSettingsViewModel();
            model.Populate();
            return View(model);
        }

        [HttpPost]
        public ActionResult ApplicationSettings(ApplicationSettingsViewModel model)
        {
            return View(model);
        }

        [HttpGet]
        public ActionResult ApplicationSettingsEdit(int? id)
        {
            if (id != null)
            {
                ApplicationSettingsEditViewModel model = new ApplicationSettingsEditViewModel((int)id);
                ViewBag.ID = model.ID;
                ViewBag.Variable = model.Variable;
                ViewBag.Value = model.Value;

                return View(model);
            }
            else
            {
                return RedirectToAction("ApplicationSettings", "Admin");
            }
        }

        [HttpPost]
        public ActionResult ApplicationSettingsEdit(ApplicationSettingsEditViewModel model, String button)
        {

            if (button == "Update")
            {
                if (model.Update())
                {
                    ViewBag.Message = "'" + model.Variable + "' was successfully updated.";
                    ViewBag.Result = true;
                }
                else
                {
                    ViewBag.Message = "'" + model.Variable + "' was not updated.<br/><br/><b>Error:</b>" + model.Error;
                    ViewBag.Result = false;
                }
                ViewBag.ID = model.ID;
                ViewBag.Variable = model.Variable;
                ViewBag.Value = model.Value;
            }
            else
            {

            }
            return View(model);
        }

        //============================User Manager=====================================

        public enum ManageMessageId
        {
            UpdateProfileSuccess,
            UpdateProductSuccess,
            UpdateRolesSuccess,
            UpdateCompanySuccessful,
            UserMissing,
            AddedToTeam,
            Error
        }

        [HttpGet]
        public ActionResult UserManager(ManageMessageId? msg) 
        {
            ViewBag.StatusMessage = msg == ManageMessageId.UserMissing ? "This user does not exist in the Database."
                : "";
            AdminIndexViewModel model = new AdminIndexViewModel();
            ViewBag.Users = model.getUsers();
            ViewBag.UserCount = model.getUsers().Count();
            return View(model);
        }

        [HttpPost]
        public ActionResult UserManager(AdminIndexViewModel model) 
        {
            ViewBag.Users = model.getUsers();
            ViewBag.UserCount = model.getUsers().Count();
            return View(model);
        }

        public ActionResult Add() 
        {
            return View(); 
        }

        [HttpGet]
        public ActionResult Edit(int id, ManageMessageId? msg, int? tab) 
        {
            List<String> temp = new List<String>();
            var a= msg == ManageMessageId.UpdateProfileSuccess ? "This user's profile settings has been updated."
            : msg == ManageMessageId.UpdateProductSuccess ? "This user's product access has been updated."
            : msg == ManageMessageId.UpdateRolesSuccess ? "This user's product role has been updated."
            : msg == ManageMessageId.UpdateCompanySuccessful ? "This user's company information has been updated."
            : msg == ManageMessageId.AddedToTeam ? "This user has been successfully added to a new team."
            : "";
            for (int i = 0; i < 4; i++) 
            {
                temp.Add("");
            }
            if(tab!=null)
            {
                temp[(int)tab] = a;
            }
            ViewBag.StatusMessage = temp;
            ViewBag.tab = tab;
            //to-do Success messages for the user's profile update
            int existance = (Int32)_sql.ProcessScalarCommand("SELECT COUNT(1) FROM ST_USERS WHERE USER_ID = "+Convert.ToString(id));
            if(existance==1)
            {
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                string accountType = getAspUserID(id);
                EditUserManagerViewModel model = new EditUserManagerViewModel(id, accountType);
                ViewBag.Products = model.getProducts();
                ViewBag.Company = false;
                if (model.company == 0)
                {
                    ViewBag.Company = true;
                }
                ViewBag.Department = model.getDepartment();
                ViewBag.Manager = model.getManager();

                return View(model);
            }
            else
            {
                return RedirectToAction("UserManager", new { msg = ManageMessageId.UserMissing});
            }
        }
        private string getAspUserID(int StUserID)
        {
            string id = "";
            REATrackerDB _sql = new REATrackerDB();
            String cmdText = "SELECT AspNetUserRoles.RoleId " +
                                "FROM AspNetUserRoles " +
                                "INNER JOIN AspNetUsers ON AspNetUsers.Id = AspNetUserRoles.UserId " +
                                "WHERE AspNetUsers.ST_USER_ID = " + Convert.ToString(StUserID) + ";";
            //To-Do revist the product_access and prodict_team relation vendiagram 
            _sql.OpenConnection();
            id = Convert.ToString(_sql.ProcessScalarCommand(cmdText));
            _sql.CloseConnection();

            if (id == "")
            {
                string stUserTable = "SELECT AspNetRoles.Id " +
                                     "FROM AspNetRoles " +
                                    "INNER JOIN ST_ACCOUNT_TYPE ON AspNetRoles.Name = ST_ACCOUNT_TYPE.NAME " +
                                    "INNER JOIN ST_USERS ON ST_USERS.ACCOUNT_TYPE = ST_ACCOUNT_TYPE.ACCOUNT_ID " +
                                    "WHERE ST_USERS.USER_ID = " + Convert.ToString(StUserID) + " ;";
                _sql.OpenConnection();
                id = Convert.ToString(_sql.ProcessScalarCommand(stUserTable));
                _sql.CloseConnection();
            }

            return id;
        }
        [HttpPost]
        public async Task<ActionResult> Edit(EditUserManagerViewModel model, int id,string button, string hiddenString) 
        {
            ApplicationDbContext context = new ApplicationDbContext();
            int ST_USERID = id;
            string aspID = null;
            var temp = context.Users.Where(u => u.ST_USER_ID == id);
            if(temp.Count()>0)
            {
                aspID = temp.First().Id;
            }
            if (button.Equals("AddtoTeam"))
            {
                model.addTeam(ST_USERID);
                return RedirectToAction("Edit", new { msg = ManageMessageId.AddedToTeam, id = id, tab = 2 });
            }
            else if (button.Equals("Profile"))
            {
                //change the asp password
                if (aspID != null)//Check if the user has a asp account.
                {
                    UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
                    UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
                    String hashedNewPassword = UserManager.PasswordHasher.HashPassword(model.Password);
                    ApplicationUser cUser = await store.FindByIdAsync(aspID);
                    await store.SetPasswordHashAsync(cUser, hashedNewPassword);
                    await store.UpdateAsync(cUser);
                    //Change the rest of the profile

                    model.EditPostProfile(aspID, ST_USERID);
                    var idManager = new IdentityManager();
                      idManager.ClearUserRoles(aspID);
                    bool pass = idManager.AddUserToRoleById(aspID, model.AccountType);
                }
                else
                {
                    model.EditPostProfile(ST_USERID);
                }
                return RedirectToAction("Edit", new { msg = ManageMessageId.UpdateProfileSuccess, id = id, tab = 0 });
            }
            else if (button.Equals("DeletePhoto"))
            {
                model.DeletePhoto();
                return RedirectToAction("Edit", new { msg = ManageMessageId.UpdateProfileSuccess, id = id, tab = 0 });
            }
            else if (button.Equals("Product"))
            {
                //Change the product access
                model.EditPostProduct(id, hiddenString);
                return RedirectToAction("Edit", new { msg = ManageMessageId.UpdateProductSuccess, id = id, tab = 1 });
            }
            else if (button.Equals("Roles"))
            {
                //Change the product role
                model.EditPostProductRoles(ST_USERID);
                return RedirectToAction("Edit", new { msg = ManageMessageId.UpdateRolesSuccess, id = id, tab = 2 });
            }
            else if (button.Equals("Company"))
            {
                //Change the company information
                model.EditPostCompany(ST_USERID);
                return RedirectToAction("Edit", new { msg = ManageMessageId.UpdateCompanySuccessful, id = id, tab = 3 });
            }
            return View(model);
        }

//============================System Variables=================================

        [AllowAnonymous]
        public ActionResult VariableManager() 
        {
            return View();
        }
//============================Boot Log History=================================
        [HttpGet]
        public ActionResult BootLogHistory() 
        {
            BootLogHistoryViewModel model = new BootLogHistoryViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult BootLogHistory(BootLogHistoryViewModel model) 
        {
            if (ModelState.IsValid) 
            {
                model.DisplayLogs();
            }
            return View(model);
        }

//============================Priority Manager=================================
        [HttpGet]
        public ActionResult PriorityManager(string msg) 
        {
            PriorityManagerViewModel model = new PriorityManagerViewModel();
            ViewBag.Message = msg;
            return View(model);
        }
        [HttpPost]
        public ActionResult PriorityManager(PriorityManagerViewModel model) 
        {
            String message = model.add();
            return RedirectToAction("PriorityManager", new { msg = message });
        }
//============================Status Manager===================================
        [HttpGet]
        public ActionResult StatusManager(int? id, bool? message) 
        {
            StatusManagerViewModel model;
            ViewBag.Display = false;
            if(message!=null && (bool)message)
            {
                ViewBag.Display = true;
            }
            if (id != null)
            {
                model = new StatusManagerViewModel((int)id);
            }
            else 
            {
                model = new StatusManagerViewModel();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult StatusManager(StatusManagerViewModel model, string button)
        {
            ActionResult final;
            if(button == ("Update"))
            {
                final = View(model);
            }
            else if(button == ("Create"))
            {
                bool created = model.createNewChange();//Returns true if already exits
                if (created)
                {
                    final = RedirectToAction("StatusManager", new { id = model.RoleID, message = true });
                }
                else 
                {
                    final = RedirectToAction("StatusManager", new { id = model.RoleID });
                }
            }
            else if(button != null && button.Contains("Delete"))
            {
                String[] param = button.Split(' ');
                model.DeleteChange(Convert.ToInt32(param[1]));
                final = RedirectToAction("StatusManager", new { id = model.RoleID });
            }
            else
            {
                final = RedirectToAction("StatusManager", new { id = model.RoleID });
            }
            return final;
        }

        [HttpGet]
        public ActionResult StatusChangeEdit(int id, bool? displaymessage) 
        {
            StatusChangeEdit model = new StatusChangeEdit(id);
            ViewBag.DisplayMessage = false;
            if( displaymessage!=null && (bool)displaymessage )
            {
                ViewBag.DisplayMessage = true;
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult StatusChangeEdit(StatusChangeEdit model)
        {
            bool exists = model.processChange();
            ActionResult view;
            if (exists)
            {
                view = RedirectToAction("StatusChangeEdit", 
                    new { id = model.StatusChangeId, displaymessage = exists });
            }
            else 
            {
                view = RedirectToAction("StatusManager", new { id = model.GroupID});
            }
            return view;
        }

        [HttpGet]
        public ActionResult EditStatusListItem(int id)
        {
            EditStatusListItemViewModel model = new EditStatusListItemViewModel(id);
            bool result = model.init();
            if (result)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("StatusManager", "Admin", new { msg = "That status id does not exist." });
            }
        }

        [HttpPost]
        public ActionResult EditStatusListItem(EditStatusListItemViewModel model)
        {

            String message = model.update();
            ViewBag.Message = message;
            if (message == "")
            {
                return RedirectToAction("StatusManager", "Admin", new { msg = model.Name + " has been successfully updated." });
            }
            return View(model);
        }

        //============================Keyword Manager===============================
        [HttpGet]
        public ActionResult KeywordManager()
        {
            KeywordManagerViewModel model = new KeywordManagerViewModel();
            return View(model);
        }

        //[HttpGet]
        //public ActionResult KeywordManager(string msg)
        //{
        //    KeywordManagerViewModel model = new KeywordManagerViewModel();

        //    if (!string.IsNullOrWhiteSpace(msg))
        //    {
        //        //this show only come from deleting a keyword
        //        ViewBag.Result = true;
        //        ViewBag.Message = msg;
        //    }

        //    return View(model);
        //}

        [HttpPost]
        public ActionResult KeywordManager(KeywordManagerViewModel model, String button)
        {
            if (button == "Add")
            {
                if (model.CreateNew())
                {
                    ViewBag.Message = "'" + model.Keyword + "' was successfully added.";
                    ViewBag.Result = true;
                }
                else
                {
                    ViewBag.Message = "'" + model.Keyword + "' was not added.<br/><br/><b>Error:</b>" + model.Error;
                    ViewBag.Result = false;
                }
            }
            else if (button == "Update")
            {
               // model.UpdateIssueType();
            }
           // model.init();
            return View(model);
        }

        [HttpGet]
        public ActionResult KeywordManagerKeywordDetails(int id)
        {
            KeywordManagerKeywordDetailsViewModel model = new KeywordManagerKeywordDetailsViewModel(id);
            ViewBag.Title = Resources.strings.qviKeywordDetails;
            ViewBag.ID = model.ID;
            ViewBag.Keyword = model.Keyword;
            ViewBag.Description = model.Description;
            return View(model);
        }

        [HttpGet]
        public ActionResult KeywordManagerEdit(int id)
        {
            KeywordManagerEditViewModel model = new KeywordManagerEditViewModel(id);
            ViewBag.Title = Resources.strings.qviEditKeyword;
            ViewBag.ID = model.ID;
            ViewBag.Keyword = model.Keyword;
            ViewBag.Description = model.Description;
            ViewBag.Usage = model.Usage;
            return View(model);
        }

        [HttpPost]
        public ActionResult KeywordManagerEdit(KeywordManagerEditViewModel model, String button)
        {
            ViewBag.Title = Resources.strings.qviEditKeyword;

            if (button == "Update")
            {
                if (model.Update())
                {
                    ViewBag.Message = "'" + model.Keyword + "'" + Resources.strings.qviWasSuccessfullyUpdated;
                    ViewBag.Result = true;
                }
                else
                {
                    ViewBag.Message = "'" + model.Keyword + "'" + Resources.strings.qviWasNotUpdated + "<br/><br/><b>" + Resources.strings.qviError + ":</b>" + model.Error;
                    ViewBag.Result = false;
                }
                ViewBag.ID = model.ID;
                ViewBag.Keyword = model.Keyword;
                ViewBag.Description = model.Description;
                ViewBag.Usage = model.Usage;
            }
            else if (button == "Delete")
            {
                if (model.Delete())
                {
                    return RedirectToAction("KeywordManager", "Admin");
                }
                else
                {
                    ViewBag.Message = "'" + model.Keyword + "'" + Resources.strings.qviWasNotUpdated + "<br/><br/><b>" + Resources.strings.qviError + ":</b>" + model.Error;
                    ViewBag.Result = false;
                }
                ViewBag.ID = model.ID;
                ViewBag.Keyword = model.Keyword;
                ViewBag.Description = model.Description;
                ViewBag.Usage = model.Usage;
            }
            return View(model);
        }

//============================Product Manager==================================
        [HttpGet]
        public ActionResult ProductManager(int? id) 
        {
            int defaultProductID;
            REATrackerDB sql = new REATrackerDB();
            if(id == null)
            {
                String command = "SELECT ST_USERS.DEFAULT_PRODUCT FROM ST_USERS WHERE ST_USERS.USER_ID = "+Convert.ToString(Session["st_userID"])+";";
                defaultProductID = Convert.ToInt32(sql.ProcessCommand(command).Rows[0][0]);
            }
            else
            {
                defaultProductID = Convert.ToInt32(id);
            }
            return View(new ProductManagerViewModel(defaultProductID));
        }

        [HttpPost]
        public ActionResult ProductManager(ProductManagerViewModel model) 
        {
            model.post();
            return View(model);
        }

        [HttpGet]
        public ActionResult ProductManagerEditInformation(int? id, bool? success)
        {
            if (success != null && success == true)
            {
                ViewBag.Message = "Your changes have been saved.";
            }
            int defaultProductID;
            REATrackerDB sql = new REATrackerDB();
            if (id == null)
            {
                String command = "SELECT ST_USERS.DEFAULT_PRODUCT FROM ST_USERS WHERE ST_USERS.USER_ID = " + Convert.ToString(Session["st_userID"]) + ";";
                defaultProductID = Convert.ToInt32(sql.ProcessCommand(command).Rows[0][0]);
            }
            else
            {
                defaultProductID = Convert.ToInt32(id);
            }
            ProductManagerViewModel model = new ProductManagerViewModel(defaultProductID);
            return View(model);
        }

        [HttpPost]
        public ActionResult ProductManagerEditInformation(ProductManagerViewModel model)
        {
            model.saveBasicInfo();
            return RedirectToAction("ProductManager", "Admin", new { id = model.productID, success = true });
        }

        [HttpGet]
        public ActionResult ProductManagerModules(int? id, bool? success) 
        {
            if (success != null && success == true)
            {
                ViewBag.Message = Resources.strings.qviYourChangesHaveBeenSaved;
            }
            int defaultProductID;
            REATrackerDB sql = new REATrackerDB();
            if (id == null)
            {
                String command = "SELECT ST_USERS.DEFAULT_PRODUCT FROM ST_USERS WHERE ST_USERS.USER_ID = " + Convert.ToString(Session["st_userID"]) + ";";
                defaultProductID = Convert.ToInt32(sql.ProcessCommand(command).Rows[0][0]);
            }
            else
            {
                defaultProductID = Convert.ToInt32(id);
            }
            ProductManagerViewModel model = new ProductManagerViewModel(defaultProductID);
            return View(model);
        }

        //Do Post WHen doing new Product or makea  new Product page

        [HttpGet]
        public ActionResult ProductManagerEditModule(int? id, string msg)
        {
            if (!String.IsNullOrEmpty(msg))
            {
                ViewBag.Message = msg;
            }
            int defaultProductID;
            REATrackerDB sql = new REATrackerDB();
            if (id == null)
            {
                String command = "SELECT ST_USERS.DEFAULT_PRODUCT FROM ST_USERS WHERE ST_USERS.USER_ID = " + Convert.ToString(Session["st_userID"]) + ";";
                defaultProductID = Convert.ToInt32(sql.ProcessCommand(command).Rows[0][0]);
            }
            else
            {
                defaultProductID = Convert.ToInt32(id);
            }
            ProductManagerViewModel model = new ProductManagerViewModel(defaultProductID);
            return View(model);
        }

        [HttpPost]
        public ActionResult ProductManagerEditModule(ProductManagerViewModel model, String button)
        {
            if( button == "CreateModule" )
            {
                model.createModules();
                return RedirectToAction("ProductManagerEditModule", "Admin", new { id = model.productID, msg = Resources.strings.qviSuccess});
            }
            else if (button == "UpdateModule")
            {
                model.saveModules();
                return RedirectToAction("ProductManagerEditModule", "Admin", new { id = model.productID, msg = Resources.strings.qviSuccess });
            }
            else 
            {
                return RedirectToAction("ProductManagerEditModule", "Admin", new { id = model.productID});
            }
        }

        [HttpGet]
        public ActionResult ProductManagerEditTeam(int? id, string msg)
        {
            if (!String.IsNullOrEmpty(msg))
            {
                ViewBag.Message = msg;
            }
            int defaultProductID;
            REATrackerDB sql = new REATrackerDB();
            if (id == null)
            {
                String command = "SELECT ST_USERS.DEFAULT_PRODUCT FROM ST_USERS WHERE ST_USERS.USER_ID = " + Convert.ToString(Session["st_userID"]) + ";";
                defaultProductID = Convert.ToInt32(sql.ProcessCommand(command).Rows[0][0]);
            }
            else
            {
                defaultProductID = Convert.ToInt32(id);
            }
            ProductManagerViewModel model = new ProductManagerViewModel(defaultProductID);
            return View(model);
        }

        [HttpPost]
        public ActionResult ProductManagerEditTeam(ProductManagerViewModel model, String button) 
        {
            if (button == "AddUser")
            {
                model.AddNewTeammate();
                return RedirectToAction("ProductManagerEditTeam", "Admin", new { id = model.productID, msg = Resources.strings.qviSuccess });
            }
            else if (button == "UpdateAllUsers")
            {
                model.UpdateTeammate();
                return RedirectToAction("ProductManagerEditTeam", "Admin", new { id = model.productID, msg = Resources.strings.qviSuccess });
            }
            else
            {
                return RedirectToAction("ProductManagerEditTeam", "Admin", new { id = model.productID });
            }
        }

        [HttpGet]
        public ActionResult ProductManagerCreate()
        {
            int defaultProductID = 1;
            ProductManagerViewModel model = new ProductManagerViewModel(defaultProductID);
            return View(model);
        }

        [HttpPost]
        public ActionResult ProductManagerCreate(ProductManagerViewModel model, string button)
        {
            if((button != null) && (button =="Create"))
            {
                int newproductID = model.CreateNewProduct();
                return RedirectToAction("ProductManager", "Admin", new { id=newproductID });
            }
            return RedirectToAction("ProductManager", "Admin");
        }

        [HttpGet]
        public ActionResult ProductManagerMetrics(int? id) 
        {
            int defaultProductID;
            REATrackerDB sql = new REATrackerDB();
            if (id == null)
            {
                String command = "SELECT ST_USERS.DEFAULT_PRODUCT FROM ST_USERS WHERE ST_USERS.USER_ID = " + Convert.ToString(Session["st_userID"]) + ";";
                defaultProductID = Convert.ToInt32(sql.ProcessCommand(command).Rows[0][0]);
            }
            else
            {
                defaultProductID = Convert.ToInt32(id);
            }
            ProductManagerViewModel model = new ProductManagerViewModel(defaultProductID);
            model.PopulateMetrics();
            return View(model);
        }
       
        [HttpPost]
        public ActionResult ProductManagerMetrics(ProductManagerViewModel model, string button) 
        {
            //To-do Figure Out what to return
            ActionResult nextPage;
            if(button != null && button == "Create")
            {
               model.CreateMetrics();
            }
            else if(button != null && button=="Update")
            {
                model.UpdateMetric(model.MetricID);
            }
            else if( button != null && button.Contains("Delete"))
            {
                int id = 0;
                Int32.TryParse(button.Replace("Delete ", ""), out id);
                if(id != 0)
                {
                    model.DeleteMetric(id);
                }    
            }
            nextPage = RedirectToAction("ProductManagerMetrics", "Admin", new { id = model.productID });
            return nextPage;
        }

        [HttpGet]
        public ActionResult ProductManagerMetricEdit(int id) 
        {
            ProductManagerViewModel model = new ProductManagerViewModel();
            if (!model.GetPopulateMetric(id))
            {
                ViewBag.Message = "The Metric does not exists";
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ProductManagerMetricEdit(ProductManagerViewModel model,int id ) 
        {
            model.UpdateMetric(id);
            return RedirectToAction("ProductManagerMetrics", "Admin", new { id = model.productID });
        }
//============================Product Hierarchy==================================
        [HttpGet]
        public ActionResult ProductHierarchy(String msg) 
        {
            ViewBag.Message = msg;
            ProductHierarchyViewModel model = new ProductHierarchyViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ProductHierarchy(ProductHierarchyViewModel model, String button) 
        {
            if( button == "Create" )
            {
               model.InsertProductRelationship();
            }
            else if (button == "Remove") 
            {
                model.RemoveRelation();
            }
            return RedirectToAction("ProductHierarchy", "Admin", new { msg = Resources.strings.qviSuccess});
        }
//============================Attachment Manager===============================
        [HttpGet]
        public ActionResult AttachmentManager() 
        {
            AttachmentManagerViewModel model = new AttachmentManagerViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult AttachmentManager( AttachmentManagerViewModel model, string button ) 
        {
            int num = -1;
            ActionResult NextPage = RedirectToAction("AttachmentManager", "Admin");
            if (!String.IsNullOrEmpty(button)&& Int32.TryParse(button, out num))
            {
                if( num != -1 ) 
                {
                    ViewBag.Message = model.Delete(num);
                    AttachmentManagerViewModel NewModel = new AttachmentManagerViewModel();
                    NextPage = View(NewModel);
                }
            }
            else if( String.IsNullOrEmpty(button) )
            {
                model.PopulateFiles();
                NextPage = View(model);
            }
            return NextPage;
        }

        [HttpGet]
        public ActionResult AttachmentManagerEdit(int id) 
        {
            AttachmentManagerViewModel model = new AttachmentManagerViewModel();
            model.GetAttachment(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult AttachmentManagerEdit(AttachmentManagerViewModel model)
        {
            model.UpdateAttachment(model.FileID);
            model.GetAttachment(model.FileID);
            return View(model);
        }
//============================Report Manager===============================
        [HttpGet]
        public ActionResult ReportManager() 
        {
            ReportManagerViewModel model = new ReportManagerViewModel();
            return View(model); 
        }

        [HttpPost]
        public ActionResult ReportManager(ReportManagerViewModel model, String button) 
        {
            ActionResult Result;
            if (button == "Save")
            {
                model.saveReport();
            }
            else if (button == "New")
            {
                model.createReportGroup();
            }
            else if (button == "Remove")
            {
                model.DeleteReportCurrentGroup();
            }
            else 
            {
                model.populateCurrentReports();
            }
            model.CaluateCurrentRowCount();
            Result = View(model);
            return Result;
        }

//============================Permission Manager===========================
        [HttpGet]
        public ActionResult PermissionManager(int? id)
        {
            PermissionMangerViewModel model;
            if(id == null)
            {
                model = new PermissionMangerViewModel();
            }
            else
            {
                model  = new PermissionMangerViewModel((int) id);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult PermissionManager(PermissionMangerViewModel model, string button)
        {
            ActionResult result;
            
            if (button == "ChangeRole" && model.EditID != -1 )
            {
                result = RedirectToAction("PermissionManager", "Admin", new { id = model.EditID });
            }
            else 
            {
                if (button == "Update")
                {
                    model.update();
                }
                RouteData.Values.Remove("id");
                result = RedirectToAction("PermissionManager", "Admin");
            }
            return result;
        }
//============================Department Manager===============================
        [HttpGet]
        public ActionResult DepartmentManager(int? id) 
        {
            DepartmentManagerViewModel model;
            if(id==null)
            {
                model = new DepartmentManagerViewModel();
            }
            else 
            {
                model = new DepartmentManagerViewModel((int) id);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult DepartmentManager(DepartmentManagerViewModel model, string button) 
        {
            ActionResult result;
            if(button == "Create")
            {
                model.Create();
            }
            else if (button == "Update" )
            {
                model.Update();
            }
            RouteData.Values.Remove("id");
            result = RedirectToAction("DepartmentManager","Admin");
            return result;
        }
//============================Application Settings=============================
//============================Lock Control=====================================
        [HttpGet]
        public ActionResult LockControl() 
        {
            LockControlViewModel model = new LockControlViewModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult LockControl(LockControlViewModel model) 
        {
            if(model.Lock)
            {
                model.LockUp();
            }
            else
            {
                model.UnLock();
            }
            return View(model);
        }
//============================Email Sender=====================================
        // GET: Email
        [HttpGet]
        public ActionResult Email()
        {
            EmailSenderViewModel model = new EmailSenderViewModel();
            ViewBag.Message = model.messageRelay;
            return View(model);
        }
        [HttpPost]
        public ActionResult Email(EmailSenderViewModel model, String button)
        {
            Boolean sent = false;
            if (button == "WatchUser")
            {
                if (model.WatchUserID == -1)
                {
                    sent = model.sendWatchList();
                }
                else if (model.WatchUserID > 0)
                {
                    sent = model.sendWatchList(model.WatchUserID);
                }

                if (sent)
                {
                    ViewBag.Success = true;
                    ViewBag.Message = Resources.strings.qviSuccess;
                }
                else
                {
                    ViewBag.Success = false;
                    ViewBag.Message = Resources.strings.qviFailure;
                }
            }
            else if (button == "Custom")
            {
                if (!model.sendToAll && string.IsNullOrWhiteSpace(model.sendTo))
                {
                    ViewBag.Success = false;
                    ViewBag.Message = Resources.strings.qviPleaseAddAnEmailMessage;
                }
                else
                {
                    sent = model.sendCustom();
                    if (sent)
                    {
                        ViewBag.Success = true;
                        ViewBag.Message = Resources.strings.qviSuccess;
                    }
                    else
                    {
                        ViewBag.Success = false;
                        ViewBag.Message = Resources.strings.qviFailure;
                    }
                }
            }
            else if (button == "Welcome Email")
            {
                if (model.newUserID == 0)
                {
                    ViewBag.Success = false;
                    ViewBag.Message = Resources.strings.qviPleaseSelectAUser;
                }
                else
                {
                    sent = model.sendWelcomeEmail();
                    if (sent)
                    {
                        ViewBag.Success = true;
                        ViewBag.Message = Resources.strings.qviSuccess;
                    }
                    else
                    {
                        ViewBag.Success = false;
                        ViewBag.Message = Resources.strings.qviFailure;
                    }
                }
            }
            else
            {
                ViewBag.Success = true;
                ViewBag.Message = null;
            }
            return View(model);
          
        }
       


        [HttpGet]
        public ActionResult SessionManager()
        {
            return View(new SessionManagerViewModel());
        }
        [HttpPost]
        public ActionResult SessionManager(SessionManagerViewModel model) 
        {
            model.Filter();
            return RedirectToAction("SessionManager","Admin");
        }


        //============================Department Manager===============================
        [HttpGet]
        public ActionResult ProjectManager(int? id)
        {
            ProjectManagerViewModel model;
            if (id == null)
            {
                model = new ProjectManagerViewModel();
            }
            else
            {
                model = new ProjectManagerViewModel((int)id);
            }
            return View(model);
         }
        
        [HttpPost]
        public ActionResult ProjectManager(ProjectManagerViewModel model, string button)
        {
            ActionResult result;
            if (button == "Add")
            {
                model.Create();
            }
            else if (button == "Update")
            {
                model.Update();
            }
            RouteData.Values.Remove("id");
            result = RedirectToAction("ProjectManager", "Admin");
            return result;
        }
        //++++++++++++++++++++++++++++Private Methods++++++++++++++++++++++++++++++++++
    }
}