using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using REA_Tracker.Models;
using QVICommonIntranet.Database;

namespace REA_Tracker.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private REATrackerDB _sql = new REATrackerDB();

        public class ApplicationUserManager : UserManager<ApplicationUser>
        {
            public ApplicationUserManager()
                : base(new UserStore<ApplicationUser>(new ApplicationDbContext()))
            {
                PasswordValidator = new MinimumLengthValidator(1);
            }
        }

        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
            this.UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager)
            {
                AllowOnlyAlphanumericUserNames = false
            };
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;

        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //===================================== Misc ==========================

        public ActionResult ViewUser(int? id) 
        {
            if (id!=null)
            {
                return View(new ViewUserModel((int)id));
            }
            else 
            {
                return View(new ViewUserModel());
            }
        }
    
        //GET
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Forgot() 
        {
            ForgotViewModel model = new ForgotViewModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Forgot(ForgotViewModel model) 
        {
            ViewBag.Sucess = model.sendEmail();
            return View(model);
        }
        //POST
//=====================================================================
//=========================== Logging in ==============================
//=====================================================================
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)       
        {
            ActionResult ar;

            if (System.Web.HttpContext.Current.Request.IsAuthenticated)
            {
                ar = RedirectToLocal("/Home/Index");
            }
            else
            {
                ViewBag.ReturnUrl = returnUrl;
                ar = View();
            }
            return ar;
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Session.Clear();
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                REATrackerDB sql = new REATrackerDB();

                if (user != null && !sql.IsExEmployee(Convert.ToInt32(user.ST_USER_ID)))
                {
                    int productID = 0;
                    bool isManager = false;
                    byte[] photo = null;
                    string homepage = null;
                    int display_name_pref = 0;
                    int homepage_layout = 0;


                    using (DataTable dt = sql.GetUserDetails((int)user.ST_USER_ID))
                    {
                        if (dt.Rows.Count > 0)
                        {
                            DataRow dr = dt.Rows[0];
                            productID = int.Parse(dr["DEFAULT_PRODUCT"].ToString());
                            isManager = bool.Parse(dr["IS_MANAGER"].ToString()) || (int.Parse(dr["ACCOUNT_TYPE"].ToString()) == 0);
                            display_name_pref = int.Parse(dr["DISPLAY_NAME_PREF"].ToString());
                            if (dr["PHOTO"] != DBNull.Value)
                            {
                                photo = (byte[])(dr["PHOTO"]);
                            }
                            homepage = dr["DEFAULT_PAGE"].ToString();

                            homepage_layout = int.Parse(dr["HOME_PAGE_LAYOUT"].ToString());
                        }
                       
                    }
                    await SignInAsync(user, model.RememberMe);
                    Session.Add("CurrentUserName", user.FirstName + " " + user.LastName);
                    Session.Add("st_userID", user.ST_USER_ID);
                    Session.Add("asp_userID", user.Id);
                    Session.Add("CurrentUserAccountType", user.Roles.First().Role.Name);
                    Session.Add("isManager", isManager);
                    Session.Add("CurrentProduct", productID);
                    Session.Add("DefaultProduct", productID);
                    Session.Add("DefaultHomePage", homepage);
                    Session.Add("HomePageLayout", homepage_layout);
                    Session.Add("photo", photo);
                    Session.Add("DisplayNamePref", display_name_pref);
                    System.Diagnostics.Debug.WriteLine("User " + Session["CurrentUserName"].ToString() + "[" + Session.SessionID.ToString() + "] has logged in.");

                    //!!!model.LogUser((int)user.ST_USER_ID);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username/password combination.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
//=====================================================================
//=========================== Create User =============================
//=====================================================================
        //
        // GET: /Account/Register
        [Authorize(Roles = "Administrator")]
        public ActionResult CreateUser(ManageMessageId? Message)
        {
            ViewBag.StatusMessage = Message == ManageMessageId.CreateSuccessful ? Resources.strings.qviYouHaveSuccessfullyCreatedAnAccount
                : Message == ManageMessageId.Error ? Resources.strings.qviAnErrorOccurredMessage
                : "";
            CreateUserViewModel model = new CreateUserViewModel();
            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(CreateUserViewModel model)
        {
            //PopulateUser();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);
                //ManageMessageId? message = null;
                if (result.Succeeded)
                {
                    //await SignInAsync(user, isPersistent: false);

                    //UserName and Password created
                    //message = ManageMessageId.CreateSuccessful;
                    var idManager = new IdentityManager();
                    String ASPID = UserManager.FindByName(user.UserName).Id;
                    REATrackerDB sql = new REATrackerDB();
                    //Add a ST_Users, Has to include
                    //  - Account Type
                    //  - Email
                    //  - Manager
                    //  - Deafult Product
                    //  - is_Manager
                    //  - Give Access
                    sql.CreateUser(ASPID, model.UserName, model.Password,  model.FirstName, model.LastName, model.AccountType,
                        model.defaultProduct, model.hiddenString, model.FullAccess,
                       model.isManager, 
                        model.ManagerID, model.DepartmentID, model.CompanyID, model.Email, model.BlockWhere == 1, model.LanguageCode
                        );
                    //RedirectToAction("CreateUser", new { Message = message });
                    return RedirectToAction("UserManager", "Admin");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private async void PopulateUser() 
        {
            string password = "tester";
            List<String> User = new List<string>(){ "testDev", "testLead", "testTest", "testTech", "testUser", "testUser", "testVendor" };
            foreach (string login in User)
            {
                var user = new ApplicationUser() { UserName = login };
                var result = await UserManager.CreateAsync(user, password);
            }
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

//=====================================================================
//=========================== Manage User =============================
//=====================================================================

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message, int? tab)
        {
            List<String> temp = new List<String>();
            var a = message == ManageMessageId.ChangePasswordSuccess ? Resources.strings.qviYourPasswordHasBeenChanged
                : message == ManageMessageId.SetPasswordSuccess ? Resources.strings.qviYourPasswordHadBeenSet
                : message == ManageMessageId.RemoveLoginSuccess ? Resources.strings.qviTheExternalLoginWasRemoved
                : message == ManageMessageId.ChangeNameSuccess ? Resources.strings.qviYourProfileHasBeenUpdated
                : message == ManageMessageId.WrongPassword ? Resources.strings.qviInvalidPassword
                : message == ManageMessageId.DisplayChangeSuccess ? Resources.strings.qviYourDisplaySettingsHaveBeenUpdated
                : message == ManageMessageId.Error ? Resources.strings.qviAnErrorHasOccurred
                : "";

            for (int i = 0; i < 4; i++)
            {
                temp.Add("");
            }
            if (tab != null)
            {
                temp[(int)tab] = a;
            }
            ViewBag.StatusMessage = temp;
            ViewBag.tab = tab;
            ViewBag.AccountType = (string)Session["CurrentUserAccountType"];
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            int userID = (int)Session["st_userID"];

            String[] Name = Convert.ToString(Session["CurrentUserName"]).Split(' ');

            System.Data.DataRow dr = new REATrackerDB().GetSettings(userID).Rows[0];
            ManageUserViewModel model = new ManageUserViewModel( Name[0], Name[1], Convert.ToString(dr["EMAIL"]), userID);

            ViewBag.EmployeeNum = dr["EMPLOYEE_NUM"].ToString();
            ViewBag.Products = model.getProducts(userID);
            ViewBag.Roles = model.getRoles(userID);
            ViewBag.Languages = model.getLanguages(userID);


            List<dynamic> list = new List<dynamic>();
            if (dr["DEFAULT_PAGE"] == DBNull.Value)
            {
                Session["DefaultHomePage"] = "Index";
            }
            else
            {
                Session["DefaultHomePage"] = dr["DEFAULT_PAGE"].ToString();
            }
            list.Add(new System.Dynamic.ExpandoObject());
            list[0].ID = "Index";
            list[0].IsDefault = (Session["DefaultHomePage"].Equals("Index"));
            list[0].Name = "To Do \\ Watch List";

            list.Add(new System.Dynamic.ExpandoObject());
            list[1].ID = "Dashboard";
            list[1].IsDefault = (Session["DefaultHomePage"].Equals("Dashboard"));
            list[1].Name = "Dashboard";



            ViewBag.Pages = list;
            return View(model);
        }
        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model, string button)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            // Takes in a string named button in the view and translates it to update a particular setting
            if (hasPassword)
            {
                int ST_USER_ID = (int)Session["st_userID"];
                string AspUserId = (string)Session["asp_userID"];
                //button is Password
                if (button.Equals("Password"))
                {
                    if (ModelState.IsValid)
                    {
                        if (model.OldPassword != null && model.NewPassword != null)
                        {
                            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                            if (result.Succeeded)
                            {
                                string cmd = "UPDATE ST_USERS SET PASSWORD = '" + Convert.ToString(model.NewPassword) + "' WHERE USER_ID = " + Convert.ToString(Session["st_userID"] + ";");
                                _sql.ProcessCommand(cmd);
                                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess, tab = 1 });
                            }
                            else
                            {
                                AddErrors(result);
                                return RedirectToAction("Manage", new { Message = ManageMessageId.WrongPassword, tab = 1 });
                            }
                        }
                    }
                }
                //button is Profile
                else if(button.Equals("Profile"))
                {
                    model.ManageEditProfile(AspUserId, ST_USER_ID);
                    ModelState.Clear();
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangeNameSuccess , tab = 0});
                }
                //button is Display
                else if (button.Equals("Display"))
                {
                    model.ManageEditDisplay(AspUserId, ST_USER_ID);
                    return RedirectToAction("Manage","Account", new { Message = ManageMessageId.DisplayChangeSuccess , tab = 2});
                }
                return RedirectToAction("Manage");
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

//=====================================================================
//============================== Log off ==============================
//=====================================================================

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        [HttpGet]
        public ActionResult MyTeam() 
        {
            REATrackerDB sql = new REATrackerDB();
            ActionResult nextPage;
            int SessionID = Convert.ToInt32(Session["st_userID"]);
            if (!sql.isAdminOrManager(SessionID))
            {
                nextPage = RedirectToLocal("");
            }
            else 
            {
                ViewToDoTeam model = new ViewToDoTeam(SessionID);
                nextPage = View(model);
            }
            return nextPage;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }


        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            ChangeNameSuccess,
            WrongPassword,
            DisplayChangeSuccess,
            CreateSuccessful,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                string url = Session["DefaultHomePage"]?.ToString();

                if (string.IsNullOrEmpty(url))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction(url, "Home");
                }
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}