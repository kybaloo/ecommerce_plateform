using ecommerce.Models.EnctityManager;
using ecommerce.Models.ViewModel;
using ecommerce.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecommerce.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Welcome()
        {
            //la méthode redirigeant vers la page appropiée une fois que l'on ce serait authentifié
            return View();
        }

        //[Authorize(Roles="Admin")]
        [AuthorizeRolesAttributes("Admin")]
        public ActionResult AdminOnly()
        {
            return View();
        }

        public ActionResult UnAuthorize()
        {
            return View();
        }

        [AuthorizeRolesAttributes("Admin")]
        public ActionResult ManagerUserPartial(string status = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                string LoginName = User.Identity.Name;
                UserManager UM = new UserManager();
                UserDataView UDV = UM.GetUserDataView(LoginName);

                string message = string.Empty;
                if (status.Equals("update"))
                {
                    message = "Update successful";
                }else if (status.Equals("delete"))
                {
                    message = "Delete successful";
                }
                ViewBag.Message = message;

                return PartialView(UDV);
            }
            return RedirectToAction("Index", "Home");
        }

        [AuthorizeRolesAttributes("Admin")]
        public ActionResult UpdateUserData(int userID, string loginName, string password, 
            string firstName, string lastName, string gender, int roleId = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                UserProfileView UPV = new UserProfileView();
                UPV.SYSUserID = userID;
                UPV.LoginName = loginName;
                UPV.Password = password;
                UPV.FirstName = firstName;
                UPV.LastName = lastName;
                UPV.Gender = gender;
                if (roleId > 0) UPV.LOOKUPRoleID = roleId;

                UserManager UM = new UserManager();
                UM.UpdateUserAccount(UPV);

                
            }
            return Json(new { success = true});
        }
    }
}