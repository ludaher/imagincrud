using ImaginCrud.Entities;
using ImaginCrud.Logic;
using ImaginCrud.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ImaginCrud.Controllers
{
    [Authorize]
    public class ProcessesController : BaseController
    {
        public ActionResult Processes()
        {
            var searchModel = new SearchModel<TypingProcess>();
            return View(searchModel);
        }

        public ActionResult ProcessesByProduct(int id)
        {
            var form = new FormLogic().FindById(id);
            var searchModel = new SearchModel<TypingProcess>()
            {
                EntityToFind = new TypingProcess() { Form = form }
            };
            var sections = new SectionLogic().FindByForm(id);
            ViewBag.sections = sections;
            return View(searchModel);
        }

        public ActionResult GetProcessesView(SearchModel<TypingProcess> searchModel)
        {
            var logic = new TypingProcessLogic();
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
                partialView = RenderRazorViewToString("ProcessesList", searchModel),
                totalPages = searchModel.TotalPages
            });
        }


        public ActionResult Forms()
        {
            var customers = new CustomerLogic().Find(null);
            var model = new SearchFormsModel()
            {
                ItemsByPage = 10,
                Page = 1,
                Customers = customers
            };
            return View(model);
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
        public ActionResult GetSectionFields(int id)
        {
            var section = new SectionLogic().FindById(id);
            return Json(new
            {
                Status = StatusEmum.Ok,
                UserMessage = "Se guardó correctamente el producto.",
                fields = section == null ? null : section.Fields.Select(x => AutoMapper.Mapper.Map<FieldModel>(x)),
            });
        }

        public ActionResult GetProcessHistory(int id, string processId)
        {
            var history = new TypingProcessLogic().GetHistory(id, processId);
            return Json(new
            {
                Status = StatusEmum.Ok,
                UserMessage = "Se guardó correctamente el producto.",
                history = history
            });
        }

        public ActionResult Priority()
        {
            var logicForms = new FormLogic();
            var forms = logicForms.Find(null).Select(x => new SelectListItem() { Text = x.Name, Value = x.FormId.ToString() }).ToList();
            ViewBag.Forms = forms;
            return View("ChangePriority", new PriorityModel());
        }

        public ActionResult ChangeIndividualPriority(PriorityModel model)
        {
            try
            {
                model.ErrorProcesses = 0;
                model.SuccessProcesses = 0;
                var logicForms = new FormLogic();
                var forms = logicForms.Find(null).Select(x => new SelectListItem() { Text = x.Name, Value = x.FormId.ToString() }).ToList();
                ViewBag.Forms = forms;
                var logicProcess = new TypingProcessLogic();
                var typingProcess = logicProcess.FindById(model.FileName, model.FormId);
                if (typingProcess == null)
                    throw new LogicException(string.Format("No se encontró el proceso {0} en el producto {1}", model.FileName, model.FormId));
                typingProcess.Priority = (int)model.Priority;
                logicProcess.Update(typingProcess);
                model.ErrorProcesses = 0;
                model.SuccessProcesses = 1;
            }
            catch (LogicException ex)
            {
                model.ErrorMessages = ex.Message;
            }
            return View("ChangePriority", model);
        }
        public ActionResult ChangePriorities(PriorityModel model)
        {
            var logicForms = new FormLogic();
            var forms = logicForms.Find(null).Select(x => new SelectListItem() { Text = x.Name, Value = x.FormId.ToString() }).ToList();
            ViewBag.Forms = forms;
            var logicProcess = new TypingProcessLogic();
            StringBuilder sbError = new StringBuilder();
            model.ErrorProcesses = 0;
            model.SuccessProcesses = 0;
            try
            {
                if (Request.Files.Count == 0)
                    throw new LogicException("Debe cargar un archivo.");
                using (System.IO.StreamReader reader = new System.IO.StreamReader(model.Files[0].InputStream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var typingProcess = logicProcess.FindById(line, model.FormId);
                        if (typingProcess == null)
                        {
                            model.ErrorProcesses++;
                            sbError.AppendFormat("No se encontró el proceso {0} en el producto {1}", model.FileName, model.FormId);
                            continue;
                        }
                        typingProcess.Priority = (int)model.Priority;
                        logicProcess.Update(typingProcess);
                        model.SuccessProcesses++;
                    }
                }
                if (sbError.Length > 0)
                    throw new LogicException(sbError.ToString());
            }
            catch (LogicException ex)
            {
                model.ErrorMessages = ex.Message;
            }
            return View("ChangePriority", model);
        }
    }
}