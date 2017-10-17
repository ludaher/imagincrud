using ImaginCrud.Entities;
using ImaginCrud.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.DataAccess
{
    public class FormManager
    {
       
        #region Select

        public Form FindById(int id)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    var data = db.Forms.Find(id);
                    if (data == null)
                        return null;
                    var processes = db.TypingProcesss.Where(x => x.FormId.Equals(data.FormId));
                    data.TotalProcesses = processes.Count();
                    data.ProcessesPending = processes.Where(x => x.TypingStatus == (int)ProcessStatus.Pending).Count();
                    data.ProcessesInCapture = processes.Where(x => x.TypingStatus == (int)ProcessStatus.InCapture).Count();
                    data.ProcessesCaptured = processes.Where(x => x.TypingStatus == (int)ProcessStatus.Captured).Count();
                    data.ProcessesInValidate = processes.Where(x => x.TypingStatus == (int)ProcessStatus.Validating).Count();
                    data.ProcessesValidated = processes.Where(x => x.TypingStatus == (int)ProcessStatus.Validated).Count();
                    return data;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in FindById: {0}", ex.ToString());
                throw;
            }
        }
        public List<Form> Find(Pagining pagining)
        {
            try
            {
                List<Form> forms = new List<Form>();
                using (var db = new ImaginCrudDataContext())
                {
                    if (pagining == null)
                    {
                        forms = db.Forms.ToList();
                    }
                    else
                    {
                        forms = db.Forms.Skip((pagining.Page - 1) * pagining.ItemsByPage)
                        .Take(pagining.ItemsByPage).ToList();
                        forms.ForEach(data =>
                        {
                            var processes = db.TypingProcesss.Where(x => x.FormId.Equals(data.FormId));
                            data.TotalProcesses = processes.Count();
                            data.ProcessesPending = processes.Where(x => x.TypingStatus == (int)ProcessStatus.Pending).Count();
                            data.ProcessesInCapture = processes.Where(x => x.TypingStatus == (int)ProcessStatus.InCapture).Count();
                            data.ProcessesCaptured = processes.Where(x => x.TypingStatus == (int)ProcessStatus.Captured).Count();
                            data.ProcessesInValidate = processes.Where(x => x.TypingStatus == (int)ProcessStatus.Validating).Count();
                            data.ProcessesValidated = processes.Where(x => x.TypingStatus == (int)ProcessStatus.Validated).Count();
                        });
                    }
                }
                return forms;
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Find: {0}", ex.ToString());
                throw;
            }
        }

        public List<Form> FindWithParameters(Pagining pagining, Form searchEntity, bool? active = null)
        {
            try
            {
                IQueryable<Form> forms;
                using (var db = new ImaginCrudDataContext())
                {
                    forms = db.Forms.OrderBy(x => x.Name);
                    if (forms.Any() && pagining != null)
                        pagining.TotalItems = forms.Count();
                    if (searchEntity != null)
                    {
                        if (active.HasValue)
                        {
                            forms = forms.Where(x => x.Active == active.Value);
                        }
                        if (string.IsNullOrWhiteSpace(searchEntity.Name) == false)
                        {
                            forms = forms.Where(x => x.Name.Contains(searchEntity.Name));
                        }
                        if (searchEntity.FormId != default(int))
                        {
                            forms = forms.Where(x => x.FormId == searchEntity.FormId);
                        }
                        if (searchEntity.CustomerId != default(int))
                        {
                            forms = forms.Where(x => x.CustomerId == searchEntity.CustomerId);
                        }
                        if (searchEntity.ProductStatus != default(int))
                        {
                            forms = forms.Where(x => x.ProductStatus == searchEntity.ProductStatus);
                        }
                    }
                    if (pagining != null)
                    {
                        forms = _ApplyOrder(forms, pagining);
                        forms = forms.Skip((pagining.Page - 1) * pagining.ItemsByPage)
                        .Take(pagining.ItemsByPage);
                    }
                    var returnList = forms.ToList();
                    returnList.ForEach(data =>
                    {
                        var processes = db.TypingProcesss.Where(x => x.FormId.Equals(data.FormId));
                        data.TotalProcesses = processes.Count();
                        data.ProcessesPending = processes.Where(x => x.TypingStatus == (int)ProcessStatus.Pending).Count();
                        data.ProcessesInCapture = processes.Where(x => x.TypingStatus == (int)ProcessStatus.InCapture).Count();
                        data.ProcessesCaptured = processes.Where(x => x.TypingStatus == (int)ProcessStatus.Captured).Count();
                        data.ProcessesInValidate = processes.Where(x => x.TypingStatus == (int)ProcessStatus.Validating).Count();
                        data.ProcessesValidated = processes.Where(x => x.TypingStatus == (int)ProcessStatus.Validated).Count();
                    });
                    return returnList;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in FindWithParameters: {0}", ex.ToString());
                throw;
            }
        }


        private IQueryable<Form> _ApplyOrder(IQueryable<Form> query, Pagining pagining)
        {
            if (pagining.SortBy == "Name")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
            else if (pagining.SortBy == "ProductStatus")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.ProductStatus) : query.OrderBy(x => x.ProductStatus);
            else if (pagining.SortBy == "Active")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.Active) : query.OrderBy(x => x.Active);
            else
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.FormId) : query.OrderBy(x => x.FormId);
            return query;
        }

        #endregion

        #region Change data

        public Form Insert(Form entity)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    entity.ProductStatus = (int)ProductStatus.Registered;
                    db.Forms.Add(entity);
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand(entity.GetQueryToCreateTable());
                    db.SaveChanges();
                    return entity;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Insert: {0}", ex.ToString());
                throw;
            }
        }

        public Form Update(Form entity)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    var form = db.Forms.Find(entity.FormId);
                    form.Active = entity.Active;
                    form.FormId = entity.FormId;
                    form.Description = entity.Description;
                    form.Name = entity.Name;
                    form.RequiredCaptures = entity.RequiredCaptures;
                    form.TemplateHeight = entity.TemplateHeight;
                    form.TemplatePath = entity.TemplatePath;
                    form.ModifiedBy = entity.ModifiedBy;
                    form.ModifiedOn = entity.ModifiedOn;
                    form.ProductStatus = entity.ProductStatus;
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand(entity.GetQueryToCreateTable());
                    db.SaveChanges();
                    return form;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Update: {0}", ex.ToString());
                throw;
            }
        }

        public void DeleteExtraSections(List<Section> sections)
        {
            using (var db = new ImaginCrudDataContext())
            {
                var firstSection = sections.FirstOrDefault();
                if (firstSection == null) return;
                var sectionsIds = sections.Select(x => x.SectionId).ToList();
                var sectionsToDelete = db.Sections.Where(x => x.FormId == firstSection.FormId).Where(x => sectionsIds.Contains(x.SectionId) == false).ToList();
                foreach (var section in sectionsToDelete)
                {
                    var fieldsInSection = db.Fields.Where(x => x.SectionId == section.SectionId).ToList();
                    foreach (var field in fieldsInSection)
                    {
                        db.FormDataDetails.RemoveRange(db.FormDataDetails.Where(x => x.FieldId == section.SectionId).ToList());
                    }
                    db.Fields.RemoveRange(fieldsInSection);
                }
                db.Sections.RemoveRange(sectionsToDelete);

                db.SaveChanges();
            }
        }

        #endregion
    }

}
