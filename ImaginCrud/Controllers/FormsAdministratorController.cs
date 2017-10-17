using ImaginCrud.Entities;
using ImaginCrud.Logic;
using ImaginCrud.Models;
using ImaginCrud.Util;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ImaginCrud.Controllers
{
    [Authorize]
    public class FormsAdministratorController : BaseController
    {
        public const string NEW_FORM_MODEL = "NEW_FORM_MODEL";
        public const string FILE_DEMO = "FILE_DEMO";

        public ActionResult AddForm(int id)
        {
            var customer = new CustomerLogic().FindById(id);
            var model = new FormModel()
            {
                Customer = customer
            };
            return View(model);
        }

        public ActionResult AddFormWithoutCustomer()
        {
            var customers = new CustomerLogic().Find(null);
            var model = new FormModel()
            {
                Customers = customers
            };
            return View("AddForm", model);
        }

        public ActionResult LoadFormContent(int id, string file)
        {
            var form = new FormLogic().FindById(id);
            if (form == null)
                return null;

            var filesFolder = ConfigurationManager.AppSettings[Constants.PROCESSES_LOCAL_PATH];
            string fileName = string.Format("{0}.pdf", Path.GetFileNameWithoutExtension(file));
            var filePath = Path.Combine(filesFolder, id.ToString(), fileName);
            // stream file if exists    
            FileInfo info = new FileInfo(filePath);
            if (info.Exists)
                return File(info.OpenRead(), MimeType(fileName));
            return null;
        }


        private string MimeType(string filename)
        {
            string mime = "application/octetstream";
            var extension = Path.GetExtension(filename);
            if (extension != null)
            {
                RegistryKey rk = Registry.ClassesRoot.OpenSubKey(extension.ToLower());

                if (rk != null && rk.GetValue("Content Type") != null)
                    mime = rk.GetValue("Content Type").ToString();
            }
            return mime;
        }

        [HttpPost]
        public ActionResult LoadFile(System.Web.HttpPostedFileBase file)
        {
            Session["TEMP_FILE"] = PdfConverter.GetPdfFromFile(file);
            return Json(new { Status = StatusEmum.Ok, UserMessage = "Se cargó correctamemte el archivo.", FileName = file.FileName });
        }

        public ActionResult PreviewFile()
        {
            var data = (byte[])Session["TEMP_FILE"];
            if (data == null)
                return null;
            return File(data, MimeType("temp.pdf"));

        }

        [HttpPost]
        public ActionResult SaveFile(System.Web.HttpPostedFileBase file, int formId, string fileName)
        {
            var filesFolder = ConfigurationManager.AppSettings[Constants.PROCESSES_LOCAL_PATH];
            var path = Path.Combine(filesFolder, formId.ToString(), fileName);
            var pdfFile = PdfConverter.GetPdfFromFile(file);
            System.IO.File.WriteAllBytes(path, pdfFile);
            return Json(new { Status = StatusEmum.Ok, UserMessage = "Se guardó correctamente el archivo.", FileName = fileName });
        }

        [HttpPost]
        public ActionResult SaveForm(UpdateFormModel form)
        {
            //var path = Path.Combine(Server.MapPath("~/Content/FormTemplates"), form.TemplatePath);
            var newEntity = new FormLogic().Insert(new Form()
            {
                CustomerId = form.CustomerId,
                Active = form.Active,
                Description = form.Description,
                Name = form.Name,
                TemplateHeight = form.PdfHeight,
                TemplatePath = string.Format("{0}.pdf", Guid.NewGuid()),
                RequiredCaptures = 2
            });
            var filesFolder = ConfigurationManager.AppSettings[Constants.PROCESSES_LOCAL_PATH];
            string fileName = newEntity.TemplatePath;
            var filePath = Path.Combine(filesFolder, newEntity.FormId.ToString(), fileName);
            var info = new DirectoryInfo(Path.Combine(filesFolder, newEntity.FormId.ToString()));
            if (!info.Exists)
                info.Create();
            var data = (byte[])Session["TEMP_FILE"];
            System.IO.File.WriteAllBytes(filePath, data);
            return Json(new { Status = StatusEmum.Ok, UserMessage = "Se guardó correctamente el producto.", FormId = newEntity.FormId });
        }

        public ActionResult AddSectionFields(ClientModel client)
        {
            return View();
        }

        public ActionResult EditForm(int id)
        {
            var form = new FormLogic().FindById(id);
            ViewBag.SelectDataSources = new FieldDataSourceLogic().GetAllSources();
            return View(form);
        }

        public ActionResult SaveAllForm(Form form, List<SectionModel> sections)
        {
            var sectionLogic = new SectionLogic();
            var oldForm = new FormLogic().FindById(form.FormId);
            form.CustomerId = oldForm.CustomerId;
            form.TemplatePath = oldForm.TemplatePath;
            form.ProductStatus = oldForm.ProductStatus;
            var formLogic = new FormLogic();
            form = formLogic.Update(form);
            if (sections != null)
            {
                var sectionsBiz = sections.Select(x => AutoMapper.Mapper.Map<Section>(x)).ToList();
                formLogic.DeleteExtraSections(sectionsBiz);
                foreach (var section in sectionsBiz)
                {
                    Section newSection;
                    section.FormId = form.FormId;
                    if (section.SectionId == default(int))
                        newSection = sectionLogic.Insert(section);
                    else
                        newSection = sectionLogic.Update(section);
                }
            }
            return Json(new
            {
                Status = StatusEmum.Ok,
                UserMessage = "Se guardó correctamente el producto.",
                CustomerId = form.CustomerId
            });
        }
        public ActionResult SaveSection(SectionModel model)
        {
            var sectionLogic = new SectionLogic();
            var section = AutoMapper.Mapper.Map<Section>(model);
            Section newSection;
            section.FormId = model.FormId;
            if (section.SectionId == default(int))
                newSection = sectionLogic.Insert(section);
            else
                newSection = sectionLogic.Update(section);
            return Json(new
            {
                Status = StatusEmum.Ok,
                UserMessage = "Se guardó correctamente el producto.",
            });
        }
        public ActionResult GetSections(int formId)
        {
            var sections = new SectionLogic().FindByForm(formId);
            return Json(new
            {
                Status = StatusEmum.Ok,
                UserMessage = "Se guardó correctamente el producto.",
                sections = sections.Select(x => AutoMapper.Mapper.Map<SectionModel>(x)),
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

        public ActionResult FormsToAssign()
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

        public ActionResult GetActiveForms(SearchModel<Form> searchModel)
        {
            var logic = new FormLogic();
            var pagining = new Pagining()
            {
                Page = searchModel.Page,
                ItemsByPage = searchModel.ItemsByPage,
                SortBy = searchModel.SortOrder,
                IsDescendentOrder = searchModel.Descendant
            };
            var forms = logic.FindWithParameters(pagining, searchModel.EntityToFind, true);
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

        public ActionResult AssignUsersToProduct(int id)
        {
            var form = new FormLogic().FindById(id);
            var searchModel = new AssignUsersModel()
            {
                ItemsByPage = 10,
                Page = 1,
                EntityToFind = new AssignUser(),
                Form = form
            };
            return View(searchModel);
        }

        [HttpPost]
        public ActionResult GetUsersView(AssignUsersModel searchModel)
        {
            var validUserNames = Roles.GetUsersInRole(Constants.TYPIST_ROLE);
            validUserNames = validUserNames.Union(Roles.GetUsersInRole(Constants.VALIDATOR_ROLE)).ToArray();

            var users = validUserNames.Distinct().Select(Membership.GetUser).ToList();
            List<AssignUser> listResult = new List<AssignUser>();
            foreach (MembershipUser user in users)
            {
                listResult.Add(new AssignUser { UserName = user.UserName, Email = user.Email, FormId = searchModel.EntityToFind.FormId });
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
            var userInFormLogic = new UserInFormLogic();
            listResult.ForEach(x =>
            {
                x.IsTypist = userInFormLogic.Any(x.UserName, x.FormId, UserFunctions.Typist);
                x.IsValidator = userInFormLogic.Any(x.UserName, x.FormId, UserFunctions.Validator);
            });
            searchModel.ListData = listResult;
            return Json(new
            {
                Status = StatusEmum.Ok,
                partialView = RenderRazorViewToString("UsersList", searchModel),
                totalPages = searchModel.TotalPages
            });
        }

        [HttpPost]
        public ActionResult ChangeTypistAssignation(int formId, string userName)
        {
            var userInFormLogic = new UserInFormLogic();
            var userInForm = new UserInForm() { FormId = formId, UserName = userName, UserFunction = (int)UserFunctions.Typist };
            if (userInFormLogic.Any(userName, formId, UserFunctions.Typist))
                userInFormLogic.Delete(userInForm);
            else
                new UserInFormLogic().Insert(userInForm);
            return Json(new
            {
                Status = StatusEmum.Ok,
                UserMessage = "Se guardó correctamente el cambio de estado.",
            });
        }

        [HttpPost]
        public ActionResult ChangeValidatorAssignation(int formId, string userName)
        {
            var userInFormLogic = new UserInFormLogic();
            var userInForm = new UserInForm() { FormId = formId, UserName = userName, UserFunction = (int)UserFunctions.Validator };
            if (userInFormLogic.Any(userName, formId, UserFunctions.Validator))
                userInFormLogic.Delete(userInForm);
            else
                new UserInFormLogic().Insert(userInForm);
            return Json(new
            {
                Status = StatusEmum.Ok,
                UserMessage = "Se guardó correctamente el cambio de estado.",
            });
        }

        public ActionResult ManageFieldSources()
        {
            return View(new SearchModel<FieldDataSource>());
        }

        public ActionResult GetFieldDataSourcesView(SearchModel<FieldDataSource> searchModel)
        {
            var logic = new FieldDataSourceLogic();
            var pagining = new Pagining()
            {
                Page = searchModel.Page,
                ItemsByPage = searchModel.ItemsByPage,
                SortBy = searchModel.SortOrder,
                IsDescendentOrder = searchModel.Descendant
            };
            var forms = logic.Find(pagining);
            searchModel.ListData = forms;
            if (forms.Any())
                searchModel.TotalPages = (int)Math.Ceiling((double)pagining.TotalItems / (double)searchModel.ItemsByPage);
            return Json(new
            {
                Status = StatusEmum.Ok,
                partialView = RenderRazorViewToString("FieldDataSourceList", searchModel),
                totalPages = searchModel.TotalPages
            });
        }


        public ActionResult AddFieldDataSource()
        {
            return View(new FieldDataSource());
        }

        public ActionResult EditFieldDataSource(int id)
        {
            var fieldDataSource = new FieldDataSourceLogic().FindById(id);
            if (fieldDataSource == null)
                return RedirectToAction("ManageFieldSources");
            return View("AddFieldDataSource", fieldDataSource);
        }


        [HttpPost]
        public ActionResult LoadFieldDataSource(FieldDataSource entity)
        {
            try
            {

                if (entity.FieldDataSourceId == default(int) && Request.Files.Count < 1)
                {
                    throw new LogicException("Cargue el archivo por favor");
                }
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    BinaryReader b = new BinaryReader(file.InputStream);
                    byte[] binData = b.ReadBytes((int)file.InputStream.Length);
                    string result = System.Text.Encoding.Default.GetString(binData);
                    var lines = result.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    entity.FieldDataSourceDetails = new List<FieldDataSourceDetail>();
                    foreach (var line in lines)
                    {
                        var values = line.Split(';');
                        if (values.Length != 2)
                        {
                            throw new LogicException("Solo se permiten archivos CVS, delimitado con ';' y con dos valores por fila (VALOR;TEXTO)");
                        }
                        entity.FieldDataSourceDetails.Add(new FieldDataSourceDetail()
                        {
                            Value = values[0],
                            Label = values[1]
                        });
                    }
                    if ((entity.FieldDataSourceId == default(int) && entity.FieldDataSourceDetails.Count < 20)
                         || (entity.FieldDataSourceId > 0 && entity.FieldDataSourceDetails.Count > 0 && entity.FieldDataSourceDetails.Count < 20)
                        )
                    {
                        throw new LogicException("Esta opción solo permite cargar datos con más de 20 registros.");
                    }
                }
                var logic = new FieldDataSourceLogic();
                if (entity.FieldDataSourceId == default(int))
                {
                    entity.CreatedBy = User.Identity.Name;
                    entity.CreatedOn = DateTime.Now;
                    logic.Insert(entity);
                }
                else
                {
                    entity.ModifiedBy = User.Identity.Name;
                    entity.ModifiedOn = DateTime.Now;
                    logic.Update(entity);
                    if (entity.FieldDataSourceDetails != null && entity.FieldDataSourceDetails.Count > 0)
                    {
                        logic.ReplaceSourceDetail(entity, entity.FieldDataSourceDetails);
                    }
                }
                return RedirectToAction("ManageFieldSources");
            }
            catch (Exception ex)
            {
                ViewBag.Error = (ex is LogicException) ? ex.Message : "No se pudieron cargar los datos";
                return View("AddFieldDataSource", entity);
            }
        }
    }
}