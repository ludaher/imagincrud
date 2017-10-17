using ImaginCrud.Entities;
using ImaginCrud.Logic;
using ImaginCrud.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImaginCrud.Controllers
{
    [Authorize]
    public class CustomerController : BaseController
    {
        // GET: Customer
        public ActionResult Index()
        {
            var model = new SearchModel<Customer>()
            {
                ItemsByPage = 10,
                Page = 1,
                EntityToFind = new Customer()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult GetCustomersView(SearchModel<Customer> searchModel)
        {
            var logic = new CustomerLogic();
            var pagining = new Pagining()
            {
                Page = searchModel.Page,
                ItemsByPage = searchModel.ItemsByPage,
                SortBy = searchModel.SortOrder,
                IsDescendentOrder = searchModel.Descendant
            };
            var customers = logic.FindWithParameters(pagining, searchModel.EntityToFind);
            searchModel.ListData = customers;
            if (customers.Any())
                searchModel.TotalPages = (int)Math.Ceiling((double)pagining.TotalItems / (double)searchModel.ItemsByPage);
            return Json(new
            {
                Status = StatusEmum.Ok,
                partialView = RenderRazorViewToString("CustomerList", searchModel),
                totalPages = searchModel.TotalPages
            });
        }

        public ActionResult Create()
        {
            return View(new Customer());
        }
        [HttpPost]
        public ActionResult Create(Customer model)
        {
            try
            {
                var logic = new CustomerLogic();
                logic.Insert(model);
                return Redirect("Index");
            }catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Edit(Customer model)
        {
            var logic = new CustomerLogic();
            logic.Update(model);
            return Redirect("Index");
        }

        //
        // GET: /Category/Edit/5
        public ActionResult Edit(int id)
        {
            var logic = new CustomerLogic();
            var customer = logic.FindById(id);
            return View(new FormModel()
            {
                Customer = customer,
                ItemsByPage = 10,
                Page = 1,
            });
        }

        public ActionResult GetFormsView(SearchModel<Form> searchModel)
        {
            var logic = new FormLogic();
            var pagining = new Pagining()
            {
                Page = searchModel.Page,
                ItemsByPage = searchModel.ItemsByPage,
                SortBy = searchModel.SortOrder,
                IsDescendentOrder = searchModel.Descendant
            };
            var forms = logic.FindWithParameters(pagining, searchModel.EntityToFind);
            searchModel.ListData = forms;
            if (forms.Any())
                searchModel.TotalPages = (int)Math.Ceiling((double)pagining.TotalItems / (double)searchModel.ItemsByPage);
            return Json(new
            {
                Status = StatusEmum.Ok,
                partialView = RenderRazorViewToString("FormList", searchModel),
                totalPages = searchModel.TotalPages
            });
        }

    }
}