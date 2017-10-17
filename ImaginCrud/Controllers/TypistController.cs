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
    public class TypistController : Controller
    {
        public ActionResult AssignedForms()
        {
            var userInFormLogic = new UserInFormLogic();
            var typingLogic = new TypingProcessLogic();
            var formLogic = new FormLogic();
            var userInFormList = userInFormLogic.FindByParameters(null, new Entities.UserInForm() { UserName = User.Identity.Name, UserFunction = (int)UserFunctions.Typist })
                .Distinct();
            var assignedFormsModel = new List<AssignedFormModel>();
            foreach (var userInForm in userInFormList)
            {
                var assignedForm = new AssignedFormModel();
                var form = formLogic.FindById(userInForm.FormId);
                if (form.ProductStatus != (int)ProductStatus.InCapture
                    && form.ProductStatus != (int)ProductStatus.Registered
                    && form.ProductStatus != (int)ProductStatus.Received)
                    continue;
                assignedForm.Form = form;
                assignedForm.TypingProcesses = typingLogic.FindWithParameters(null, new Entities.TypingProcess()
                {
                    FormId = userInForm.FormId,
                    TypingStatus = (int)Entities.ProcessStatus.InCapture,
                    ModifiedBy = User.Identity.Name
                });
                assignedFormsModel.Add(assignedForm);
            }
            return View("AssignedForms", assignedFormsModel);
        }

        public ActionResult AssignNewProcess(int id)
        {
            var model = new TypingProcess();
            var formLogic = new FormLogic();
            var form = formLogic.FindById(id);
            var processId = new TypingProcessLogic().AssignProcessToUserInCapture(id, User.Identity.Name, Entities.ProcessStatus.Pending, Entities.ProcessStatus.InCapture);
            if (string.IsNullOrWhiteSpace(processId))
            {
                ViewBag.Error = string.Format("No se encontraron procesos que pueda capturar el usuario {0} en el producto {1}", User.Identity.Name, id);
                return AssignedForms();
            }
            model.Form = form;
            model.TypingProcessId = processId;
            return RedirectToAction("CaptureForm", "Typist", new { id = form.FormId, process = processId });
        }

        public ActionResult CaptureForm(int id, string process)
        {
            var formDataLogic = new FormDataLogic();
            var userCaptures = formDataLogic.GetFormdatasByUser(User.Identity.Name, DateTime.Now.Date, DateTime.Now.Date.AddDays(1), id);
            var formsData = formDataLogic.FindWithParameters(null, new FormData()
            {
                FormId = id,
                TypingProcessId = process,
            });
            var model = formsData
                .Where(x => x.ModifiedBy.Equals(System.Web.HttpContext.Current.User.Identity.Name)).FirstOrDefault();
            if (model == null) model = new FormData();
            var form = new FormLogic().FindById(id);
            var typingProcess = new TypingProcessLogic().FindById(process, id);
            typingProcess.Form = form;
            typingProcess.CapturedRows = formsData
                .Where(x => x.RegisterType == (int)RegisterTypes.CaptureComplete)
                .Count();
            model.TypingProcessId = process;
            model.TypingProcess = typingProcess;
            model.FormId = id;
            var captureDataModel = new CaptureDataModel();
            captureDataModel.FormData = model;
            captureDataModel.UserCaptures = userCaptures.FirstOrDefault();
            return View("CaptureForm", captureDataModel);
        }


        [HttpPost]
        public ActionResult GetSections(int formId, string typingProcessId, int formDataId)
        {
            var formDataLogic = new FormDataLogic();
            var sections = new SectionLogic().FindByForm(formId);
            var formData = new FormDataLogic().FindById(formDataId);
            var pagining = new Pagining() { Page = 1, IsDescendentOrder = true, SortBy = "", ItemsByPage = 2 };
            var capturedProcesses = formDataLogic.FindWithParameters(pagining, new FormData()
            {
                FormId = formId,
                TypingProcessId = typingProcessId,
                RegisterType = (int)RegisterTypes.CaptureComplete,
            });
            var model = new CaptureModel();
            model.SetData(sections, formData, capturedProcesses.Any() ? capturedProcesses.ElementAt(0) : null);
            return Json(new
            {
                Status = StatusEmum.Ok,
                UserMessage = "Se guardó correctamente el producto.",
                model = model,
            });
        }

        [HttpPost]
        public ActionResult SaveFormData(CaptureModel entity, bool closeCapture)
        {
            var formDataLogic = new FormDataLogic();
            var formData = formDataLogic.FindById(entity.FormDataId) ?? new FormData()
            {
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedOn = DateTime.Now,
                RegisterType = (int)RegisterTypes.Capture,
                TypingProcessId = entity.TypingProcessId,
                FormId = entity.FormId,
                FormDataId = entity.FormDataId,
            };
            formData.CompletedSections = entity.CompletedSections;
            formData.Completed = closeCapture;
            if (closeCapture)
            {
                formData.RegisterType = (int)RegisterTypes.CaptureComplete;
            }
            formData.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            formData.FormDetails = entity.FormDetails;
            formData = formDataLogic.Update(formData);
            return Json(new
            {
                Status = closeCapture ? StatusEmum.OkWithMessage : StatusEmum.Ok,
                UserMessage = closeCapture ? "Captura completa, se asignará el siguiente formulario." : "Captura completa",
                FormDataId = formData.FormDataId
            });
        }

        [HttpPost]
        public ActionResult SaveSectionData(CaptureModel entity, bool closeCapture)
        {
            var formDataLogic = new FormDataLogic();
            var formData = formDataLogic.FindById(entity.FormDataId) ?? new FormData()
            {
                CreatedBy = System.Web.HttpContext.Current.User.Identity.Name,
                CreatedOn = DateTime.Now,
                RegisterType = (int)RegisterTypes.Capture,
                TypingProcessId = entity.TypingProcessId,
                FormId = entity.FormId,
                FormDataId = entity.FormDataId,
            };
            formData.CompletedSections = entity.CompletedSections;
            formData.Completed = closeCapture;
            if (closeCapture)
            {
                formData.RegisterType = (int)RegisterTypes.CaptureComplete;
            }
            formData.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            formData.FormDetails = entity.FormDetails;
            formData = formDataLogic.Update(formData);
            return Json(new
            {
                Status = closeCapture ? StatusEmum.OkWithMessage : StatusEmum.Ok,
                UserMessage = closeCapture ? "Captura completa, se asignará el siguiente formulario." : "Captura completa",
                FormDataId = formData.FormDataId
            });
        }
    }
}