using ecommerce.Models.DB;
using ecommerce.Models.EnctityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ecommerce.Security
{
    // Ceci pour representer un extends (:)
    public class AuthorizeRolesAttributes : AuthorizeAttribute
    {

        private readonly string[] userAssignedRoles;
        public AuthorizeRolesAttributes(params string[] roles)
        {
            this.userAssignedRoles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorise = false;
            using (ecommerceEntities db = new ecommerceEntities())
            {
                Console.WriteLine("Roles ===== " + userAssignedRoles);
                UserManager UM = new UserManager();
                foreach (var role in userAssignedRoles)
                {
                    authorise = UM.isUserRole(httpContext.User.Identity.Name, role);
                    Console.WriteLine("statut ==== " + authorise);
                    if (authorise)
                    {

                        return authorise;
                    }
                }

            }
            return authorise;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Home/UnAuthorize");
        }

    }
}
