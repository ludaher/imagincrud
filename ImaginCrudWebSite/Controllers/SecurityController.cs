using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImaginCrud.Models;

namespace ImaginCrud.Controllers
{
    public class SecurityController : Controller
    {
        public ActionResult Roles()
        {
            var roleList = System.Web.Security.Roles.GetAllRoles().Select(x => new RoleModel() { RoleName = x }).ToList();
            return View(roleList);
        }

        public ActionResult ViewRoleForm()
        {
            return View();
        }
        public ActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateRole(RoleModel entity)
        {
            var roleList = System.Web.Security.Roles.GetAllRoles().Select(x => new RoleModel() { RoleName = x }).ToList();
            try
            {
                if (ModelState.IsValid)
                {
                    System.Web.Security.Roles.CreateRole(entity.RoleName);
                    return RedirectToAction("Roles");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(entity);
        }

        public ActionResult DeleteRole(string roleName)
        {
            var roleList = System.Web.Security.Roles.GetAllRoles().Select(x => new RoleModel() { RoleName = x }).ToList();
            try
            {
                if (ModelState.IsValid)
                {
                    System.Web.Security.Roles.DeleteRole(roleName);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Roles();
            }
            return RedirectToAction("Roles");
        }


    }
}
