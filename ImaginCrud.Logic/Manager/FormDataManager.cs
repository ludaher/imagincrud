using ImaginCrud.Entities;
using ImaginCrud.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using ImaginCrud.Util;
namespace ImaginCrud.DataAccess
{
    public class FormDataManager
    {
        #region Select

        public FormData FindById(long id)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    var data = db.FormData.Find(id);
                    if (data != null)
                        data.FormDetails = db.FormDataDetails
                            .Where(x => x.FormDataId.Equals(data.FormDataId))
                            .OrderBy(x => x.FieldId).ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in FindById: {0}", ex.ToString());
                throw;
            }
        }
        public List<FormData> Find(Pagining pagining)
        {
            try
            {
                List<FormData> formData = new List<FormData>();
                using (var db = new ImaginCrudDataContext())
                {
                    if (pagining == null)
                    {
                        formData = db.FormData.ToList();
                    }
                    else
                    {
                        formData = db.FormData.Skip((pagining.Page - 1) * pagining.ItemsByPage)
                        .Take(pagining.ItemsByPage).ToList();
                    }
                    foreach (var data in formData)
                    {
                        data.FormDetails = db.FormDataDetails
                            .Where(x => x.FormDataId.Equals(data.FormDataId))
                            .OrderBy(x => x.FieldId).ToList();
                    }
                }
                return formData;
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Find: {0}", ex.ToString());
                throw;
            }
        }

        public List<FormData> FindWithParameters(Pagining pagining, FormData searchEntity)
        {
            try
            {
                IQueryable<FormData> formData;
                using (var db = new ImaginCrudDataContext())
                {
                    formData = db.FormData.OrderBy(x => x.CompletedSections);
                    if (formData.Any() && pagining != null)
                        pagining.TotalItems = formData.Count();
                    if (searchEntity != null)
                    {
                        if (string.IsNullOrWhiteSpace(searchEntity.TypingProcessId) == false)
                        {
                            formData = formData.Where(x => x.TypingProcessId.Equals(searchEntity.TypingProcessId));
                        }
                        if (searchEntity.FormId != default(int))
                        {
                            formData = formData.Where(x => x.FormId == searchEntity.FormId);
                        }
                        if (string.IsNullOrEmpty(searchEntity.ModifiedBy) == false)
                        {
                            formData = formData.Where(x => x.ModifiedBy == searchEntity.ModifiedBy);
                        }
                        if (searchEntity.RegisterType != default(int))
                        {
                            formData = formData.Where(x => x.RegisterType == searchEntity.RegisterType);
                        }
                    }
                    if (pagining != null)
                    {
                        formData = _ApplyOrder(formData, pagining);
                        formData = formData.Skip((pagining.Page - 1) * pagining.ItemsByPage)
                        .Take(pagining.ItemsByPage);
                    }
                    var returnList = formData.ToList();
                    foreach (var data in returnList)
                    {
                        data.FormDetails = db.FormDataDetails
                            .Where(x => x.FormDataId.Equals(data.FormDataId))
                            .OrderBy(x => x.FieldId).ToList();
                    }
                    return returnList;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in FindWithParameters: {0}", ex.ToString());
                throw;
            }
        }


        private IQueryable<FormData> _ApplyOrder(IQueryable<FormData> query, Pagining pagining)
        {
            if (pagining.SortBy == "FormId")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.FormId) : query.OrderBy(x => x.FormId);
            else if (pagining.SortBy == "CompletedSections")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.CompletedSections) : query.OrderBy(x => x.CompletedSections);
            else if (pagining.SortBy == "ModifiedOn")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.ModifiedOn) : query.OrderBy(x => x.ModifiedOn);
            else if (pagining.SortBy == "CreatedOn")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn);
            else if (pagining.SortBy == "ModifiedBy")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.ModifiedBy) : query.OrderBy(x => x.ModifiedBy);
            else if (pagining.SortBy == "RegisterType")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.RegisterType) : query.OrderBy(x => x.RegisterType);
            else
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.CompletedSections) : query.OrderBy(x => x.CompletedSections);
            return query;
        }

        #endregion

        #region Change data

        public FormData Insert(FormData entity)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    db.FormData.Add(entity);
                    db.SaveChanges();
                    _UpdateProcess(db, entity);
                    return entity;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Insert: {0}", ex.ToString());
                throw;
            }
        }
        public FormData Update(FormData entity)
        {
            try
            {
                FormData formData;
                using (var db = new ImaginCrudDataContext())
                {
                    formData = db.FormData.Find(entity.FormDataId);
                    formData.RegisterType = entity.RegisterType;
                    formData.CompletedSections = entity.CompletedSections;
                    formData.Completed = entity.Completed;
                    formData.ModifiedBy = entity.ModifiedBy;
                    formData.ModifiedOn = entity.ModifiedOn;
                    db.SaveChanges();
                    foreach (var detail in entity.FormDetails)
                    {
                        var detailToUpdate = db.FormDataDetails.Where(x => x.FieldId.Equals(detail.FieldId))
                            .Where(x => x.FormDataId.Equals(detail.FormDataId))
                            .FirstOrDefault();
                        if (detailToUpdate == null)
                        {
                            db.FormDataDetails.Add(detail);
                            continue;
                        }
                        detailToUpdate.Value = detail.Value;
                    }
                db.SaveChanges();
                    _UpdateProcess(db, formData);
                }
                return formData;
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Update: {0}", ex.ToString());
                throw;
            }
        }

        private TypingProcess _UpdateProcess(ImaginCrudDataContext db, FormData formData)
        {
            ProcessStatus newProcessStatus = default(ProcessStatus);
            var process = db.TypingProcesss.Find(formData.TypingProcessId, formData.FormId);
            process.ModifiedOn = DateTime.Now;
            process.ModifiedBy = process.ModifiedBy;
            var form = db.Forms.Find(formData.FormId);
            var copyToHistory = false;
            if (formData.RegisterType == (int)RegisterTypes.CaptureComplete)
            {
                var count = db.FormData.Where(x => x.FormId.Equals(formData.FormId))
                    .Where(x => x.TypingProcessId.Equals(formData.TypingProcessId))
                    .Where(x => x.Completed == true)
                    .Where(x => x.RegisterType == (int)RegisterTypes.CaptureComplete)
                    .ToList().Count;
                if (count >= form.RequiredCaptures)
                    newProcessStatus = ProcessStatus.Captured;
                else
                    newProcessStatus = ProcessStatus.Pending;
            }
            else if (formData.RegisterType == (int)RegisterTypes.ValidationComplete)
            {
                newProcessStatus = ProcessStatus.Validated;
                _FillFormTable(db, formData);
                copyToHistory = true;
            }
            db.SaveChanges();
            if (newProcessStatus != default(ProcessStatus) && process.TypingStatus != (int)newProcessStatus)
                new TypingProcessManager().ChangeState(process
                    , newProcessStatus
                    , string.Format("Se almacenó el proceso {0}, cambió de estado {1} a {2}.", process.TypingProcessId, ((ProcessStatus)process.TypingStatus).Description(), newProcessStatus.Description()));
            db.SaveChanges();
            if (copyToHistory)
            {
                _SendToHistory(db, formData);
                db.SaveChanges();
            }
            return process;
        }

        private void _SendToHistory(ImaginCrudDataContext db, FormData formData)
        {
            try
            {
                db.FormDataHistories.RemoveRange(db.FormDataHistories.Where(x => x.TypingProcessId.Equals(formData.TypingProcessId)));
                db.SaveChanges();
                var formDataList = db.FormData.Include("FormDetails").Where(x => x.TypingProcessId.Equals(formData.TypingProcessId));
                foreach (var item in formDataList)
                {
                    var history = AutoMapper.Mapper.Map<FormDataHistory>(item);
                    db.FormDataHistories.Add(history);
                    db.FormData.Remove(item);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error sending form data to history: {0}", ex.ToString());
            }
        }

        private void _FillFormTable(ImaginCrudDataContext db, FormData formData)
        {
            var sections = db.Sections.Include("Fields").Where(x => x.FormId.Equals(formData.FormId));
            var columns = new List<string>();
            var values = new List<string>();
            var noTableSections = sections.Where(x => x.IsTable == false);
            var tableSections = sections.Where(x => x.IsTable == true);
            foreach (var section in noTableSections)
            {
                foreach (var field in section.Fields)
                {
                    columns.Add(field.FieldName);
                    values.Add(_GetFieldValue(field, formData.FormDetails.Where(x => x.FieldId.Equals(field.FieldId)).FirstOrDefault().Value));
                }
            }
            var query = formData.GetQueryToAddaRowInForm(string.Join(", ", columns), string.Join(", ", values));
            var tableRowValues = new List<List<string>>();
            string value;
            foreach (var section in tableSections)
            {
                columns.Clear();
                tableRowValues.Clear();
                query += formData.GetQueryToDeleteProcessInSection(section.SectionId);
                foreach (var field in section.Fields)
                {
                    columns.Add(field.FieldName);
                    value = _GetFieldValue(field, formData.FormDetails.Where(x => x.FieldId.Equals(field.FieldId)).FirstOrDefault().Value);
                    tableRowValues.Add(value.Replace("'", "").Split('|').ToList());
                }
                int rows = tableRowValues.Max(x => x.Count);
                for (int i = 0; i < rows; i++)
                {
                    values.Clear();
                    if (tableRowValues.Any(x => x.Count <= i))
                    {
                        continue;
                    }
                    var rowValues = tableRowValues.Select(x => x.ElementAt(i)).ToList();
                    if (rowValues.Any(x => string.IsNullOrWhiteSpace(x) == false) == false)
                        continue;
                    for (int j = 0; j < rowValues.Count; j++)
                    {
                        values.Add(_GetFieldValue(section.Fields.ElementAt(j), rowValues.ElementAt(j)));
                    }
                    query += formData.GetQueryToAddaRowInSection(section.SectionId, string.Join(", ", columns), string.Join(", ", values));
                }
            }
            using (var ctx = new ImaginCrudDataContext())
                ctx.Database.ExecuteSqlCommand(query);
        }

        private string _GetFieldValue(Field field, string value)
        {
            if (field.FieldTypeId == (int)FieldTypes.DateTime && string.IsNullOrWhiteSpace(value) == false)
            {
                if ("39-19-9999" == field.Validation)
                {
                    value = DateTime.ParseExact(value, "dd-MM-yyyy", null).ToString("yyyy-MM-dd");
                }
                else if ("19-39-9999" == field.Validation)
                {
                    value = DateTime.ParseExact(value, "MM-dd-yyyy", null).ToString("yyyy-MM-dd");
                }
                return $"'{value}'";
            }
            if (field.FieldTypeId == (int)FieldTypes.Number)
            {
                if (!field.Required && string.IsNullOrWhiteSpace(value))
                    value = "null";
                return $"{value}";
            }
            else
            {
                return $"'{value}'";
            }
        }
        #endregion

        public void SaveFormDetails(List<FormDataDetail> formDetails)
        {
            using (var db = new ImaginCrudDataContext())
            {
                foreach (var detail in formDetails)
                {
                    var detailToUpdate = db.FormDataDetails.Where(x => x.FieldId.Equals(detail.FieldId))
                        .Where(x => x.FormDataId.Equals(detail.FormDataId))
                        .FirstOrDefault();
                    if (detailToUpdate == null)
                    {
                        db.FormDataDetails.Add(detail);
                        continue;
                    }
                    detailToUpdate.Value = detail.Value;
                }
                db.SaveChanges();
            }
        }

        public List<UserCaptures> GetFormdatasByUser(string username, DateTime from, DateTime to, int formId)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    var items = db.Database.SqlQuery<UserCaptures>(
                        "[dbo].Get_Total_User_Captures @p0, @p1, @p2, @p3",
                        username, from, to, formId);
                    return items.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in FindById: {0}", ex.ToString());
                throw;
            }
        }
    }
}
