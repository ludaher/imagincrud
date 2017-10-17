using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DynamicLinq;
using ImaginCrud.Models;

namespace ImaginCrud.Controllers
{
    public class UsersController : Controller
    {
        //
        // GET: /Usuarios/

        public ActionResult Index()
        {
            var searchModel = new SearchModel<User>()
            {
                SortOrder = "UserName",
                Ascendant = true,
                ItemsByPage = 10,
                Page = 1
            };
            var users = Membership.GetAllUsers();
            List<User> listResult = new List<User>();
            foreach (MembershipUser user in users)
            {
                listResult.Add(new User { UserName = user.UserName, Email = user.Email });
            }
            if (listResult.Any())
                searchModel.TotalPages = (int)Math.Ceiling((double)listResult.Count() / (double)searchModel.ItemsByPage);
            listResult = listResult.Take(searchModel.ItemsByPage).ToList();
            searchModel.ListData = listResult;
            return View(searchModel);
        }

        [HttpPost]
        public ActionResult Index(SearchModel<User> searchModel)
        {
            var ascendant = searchModel.Ascendant;
            var sortOrder = searchModel.SortOrder;
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
                .OrderBy(user => user.UserName).ToList();
            ///Pagining
            ///
            if (searchModel.Page <= 0)
                searchModel.Page = 1;
            if (listResult.Any())
                searchModel.TotalPages = (int)Math.Ceiling((double)listResult.Count() / (double)searchModel.ItemsByPage);
            listResult = listResult.Skip(searchModel.ItemsByPage * (searchModel.Page - 1)).Take(searchModel.ItemsByPage).ToList();
            searchModel.ListData = listResult;
            return View(searchModel);
        }


        //
        // GET: /Category/Edit/5

        public ActionResult Edit(string id)
        {
            var userMembership = Membership.GetUser(id);
            User user = new User() { UserName = userMembership.UserName, Email = userMembership.Email };
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
            if (ModelState.IsValid)
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
                return RedirectToAction("Index");
            }
            var allRoles = System.Web.Security.Roles.GetAllRoles().Select(x => new RoleModel() { RoleName = x }).ToList();
            ViewBag.Roles = new MultiSelectList(allRoles, "RoleName", "RoleName");
            return View(user);
        }

    }
}
