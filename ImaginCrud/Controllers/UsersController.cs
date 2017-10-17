using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ImaginCrud.Models;

namespace ImaginCrud.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        //
        // GET: /Usuarios/

        public ActionResult Index()
        {
            var searchModel = new SearchModel<User>()
            {
                ItemsByPage = 10,
                Page = 1,
                EntityToFind = new Models.User()
            };
            return View(searchModel);
        }

        [HttpPost]
        public ActionResult GetUsersView(SearchModel<User> searchModel)
        {
            var users = Membership.GetAllUsers();
            List<User> listResult = new List<User>();
            foreach (MembershipUser user in users)
            {
                listResult.Add(new User { UserName = user.UserName, Email = user.Email });
            }
            ///Where clausules
            ///
            listResult = listResult.Where(user => string.IsNullOrWhiteSpace(searchModel.EntityToFind.Email) || user.Email.Contains(searchModel.EntityToFind.Email.Trim()))
                .Where(user => string.IsNullOrWhiteSpace(searchModel.EntityToFind.UserName) || user.UserName.Contains(searchModel.EntityToFind.UserName))
                .ToList();
            ///Pagining
            ///
            if (searchModel.Page <= 0)
                searchModel.Page = 1;
            if (listResult.Any())
                searchModel.TotalPages = (int)Math.Ceiling((double)listResult.Count() / (double)searchModel.ItemsByPage);
            if (string.IsNullOrWhiteSpace(searchModel.SortOrder) == false)
            {
                if (searchModel.SortOrder.Equals("UserName"))
                    listResult = searchModel.Descendant ? listResult.OrderByDescending(x => x.UserName).ToList() : listResult.OrderBy(x => x.UserName).ToList();
                else if (searchModel.SortOrder.Equals("Email"))
                    listResult = searchModel.Descendant ? listResult.OrderByDescending(x => x.Email).ToList() : listResult.OrderBy(x => x.Email).ToList();
            }
            listResult = listResult.Skip(searchModel.ItemsByPage * (searchModel.Page - 1)).Take(searchModel.ItemsByPage).ToList();
            searchModel.ListData = listResult;
            return Json(new
            {
                partialView = RenderRazorViewToString("UsersList", searchModel),
                totalPages = searchModel.TotalPages
            });
        }


        //
        // GET: /Category/Edit/5

        public ActionResult Edit(string id)
        {
            var userMembership = Membership.GetUser(id);
            User user = new User() { UserName = userMembership.UserName, Email = userMembership.Email, IsLockedOut = userMembership.IsLockedOut };
            if (user == null)
            {
                return HttpNotFound();
            }
            user.Roles = Roles.GetRolesForUser(id).ToList();
            var allRoles = System.Web.Security.Roles.GetAllRoles().Select(x => new RoleModel() { RoleName = x }).ToList();
            ViewBag.Roles = new MultiSelectList(allRoles, "RoleName", "RoleName", user.Roles);
            return View(user);
        }

        //
        // POST: /Product/Edit/5

        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (user.Roles != null)
            {
                foreach (var role in System.Web.Security.Roles.GetAllRoles())
                {
                    if (Roles.IsUserInRole(user.UserName, role) && user.Roles.Contains(role) == false)
                    {
                        Roles.RemoveUserFromRole(user.UserName, role);
                    }
                    if (Roles.IsUserInRole(user.UserName, role) == false && user.Roles.Contains(role))
                    {
                        Roles.AddUserToRole(user.UserName, role);
                    }
                }
            }

            return RedirectToAction("Index");
            //var allRoles = System.Web.Security.Roles.GetAllRoles().Select(x => new RoleModel() { RoleName = x }).ToList();
            //ViewBag.Roles = new MultiSelectList(allRoles, "RoleName", "RoleName");
            //return View(user);
        }
        //
        // GET: /Users/ChangePassword/5

        public ActionResult ChangePassword(string id)
        {
            var userMembership = Membership.GetUser(id);
            RegisterModel model = new RegisterModel();
            model.UserName = id;
            model.Email = userMembership.Email;
            return View(model);
        }

        //
        // POST: /Users/ChangePassword/5
        [HttpPost]
        public ActionResult ChangePassword(RegisterModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(model.UserName, userIsOnline: true);
                    currentUser.UnlockUser();
                    changePasswordSucceeded = currentUser.ChangePassword(currentUser.ResetPassword(), model.Password);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("Index");
                }
                //else
                //{
                //    ModelState.AddModelError("", "Problema cambiando la contraseña.");
                //}
            }
            return View(model);
            //var allRoles = System.Web.Security.Roles.GetAllRoles().Select(x => new RoleModel() { RoleName = x }).ToList();
            //ViewBag.Roles = new MultiSelectList(allRoles, "RoleName", "RoleName");
            //return View(user);
        }

    }
}
