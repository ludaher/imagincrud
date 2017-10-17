using ImaginCrud.Entities;
using ImaginCrud.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.DataAccess
{
    public class TypingProcessManager
    {
        #region Select

        public TypingProcess FindById(string id, int formId)
        {
            try
            {
                List<TypingProcess> processes = new List<TypingProcess>();
                using (var db = new ImaginCrudDataContext())
                {
                    return db.TypingProcesss.Find(id, formId);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in FindById: {0}", ex.ToString());
                throw;
            }
        }
        public List<TypingProcess> Find(Pagining pagining)
        {
            try
            {
                List<TypingProcess> processes = new List<TypingProcess>();
                using (var db = new ImaginCrudDataContext())
                {
                    if (pagining == null)
                    {
                        processes = db.TypingProcesss.ToList();
                    }
                    else
                    {
                        processes = db.TypingProcesss.Skip((pagining.Page - 1) * pagining.ItemsByPage)
                        .Take(pagining.ItemsByPage).ToList();
                    }
                }
                return processes;
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Find: {0}", ex.ToString());
                throw;
            }
        }

        public List<TypingProcess> FindWithParameters(Pagining pagining, TypingProcess searchEntity)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    IQueryable<TypingProcess> processes;
                    if (searchEntity != null)
                    {
                        if (searchEntity.FieldsToSearch != null)
                        {
                            var details = db.FormDataDetails.AsQueryable();
                            if (searchEntity.FormId != default(int))
                                details = details.Where(x => x.FormData.FormId == searchEntity.FormId);
                            foreach (var data in searchEntity.FieldsToSearch)
                            {
                                if (data == null || string.IsNullOrWhiteSpace(data.Value))
                                    continue;
                                var field = db.Fields.Find(data.FieldId);
                                if (field.FieldTypeId == (int)FieldTypes.MultiSelect)
                                {
                                    var valueStart = string.Format("{0};", data.Value);
                                    var valueEnd = string.Format("{0};", data.Value);
                                    var valueContains = string.Format("{0};", data.Value);
                                    details = details.Where(x => x.FieldId.Equals(data.FieldId))
                                    .Where(x => x.Value.Equals(data.Value)
                                    || x.Value.StartsWith(valueStart)
                                    || x.Value.EndsWith(valueEnd)
                                    || x.Value.Contains(valueContains)
                                    );
                                }
                                else if (field.FieldTypeId == (int)FieldTypes.Select)
                                {
                                    details = details.Where(x => x.FieldId.Equals(data.FieldId))
                                    .Where(x => x.Value.Equals(data.Value));
                                }
                                else
                                {
                                    details = details.Where(x => x.FieldId.Equals(data.FieldId))
                                    .Where(x => x.Value.Trim().ToUpper().Contains(data.Value.Trim().ToUpper()));
                                }
                            }
                            processes = details.Select(x => x.FormData.TypingProcess)
                                .Distinct();
                        }
                        else
                        {
                            processes = db.TypingProcesss.AsQueryable();
                        }
                        if (searchEntity.FormId != default(int))
                            processes = processes.Where(x => x.FormId == searchEntity.FormId);
                        if (searchEntity.TypingStatus != default(int))
                            processes = processes.Where(x => x.TypingStatus == searchEntity.TypingStatus);
                        if (string.IsNullOrWhiteSpace(searchEntity.ModifiedBy) == false)
                            processes = processes.Where(x => x.ModifiedBy.Equals(searchEntity.ModifiedBy));
                    }
                    else
                    {
                        processes = db.TypingProcesss.AsQueryable();
                    }
                    if (pagining != null)
                    {
                        processes = _ApplyOrder(processes, pagining);
                        pagining.TotalItems = processes.Count();
                        processes = processes.Skip((pagining.Page - 1) * pagining.ItemsByPage)
                        .Take(pagining.ItemsByPage);
                    }
                    return processes.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in FindWithParameters: {0}", ex.ToString());
                throw;
            }
        }

        private IQueryable<TypingProcess> _ApplyOrder(IQueryable<TypingProcess> query, Pagining pagining)
        {
            if (pagining.SortBy == "TypingStatus")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.TypingStatus) : query.OrderBy(x => x.TypingStatus);
            else if (pagining.SortBy == "ModifiedOn")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.ModifiedOn) : query.OrderBy(x => x.ModifiedOn);
            else if (pagining.SortBy == "CreatedOn")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn);
            else if (pagining.SortBy == "ModifiedBy")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.ModifiedBy) : query.OrderBy(x => x.ModifiedBy);
            else
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.FormId) : query.OrderBy(x => x.FormId);
            return query;
        }

        public List<ProductStatusHistory> GetHistory(int formId, string processId)
        {
            using (var db = new ImaginCrudDataContext())
            {
                return db.ProductStatusHistory
                    .Where(x => x.FormId == formId)
                    .Where(x => x.TypingProcessId.Equals(processId))
                    .ToList();
            }
        }
        #endregion

        #region Change data

        public void Insert(TypingProcess entity)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    db.TypingProcesss.Add(entity);
                    db.SaveChanges();
                    var history = new ProductStatusHistory()
                    {
                        FormId = entity.FormId,
                        ModifiedOn = DateTime.Now,
                        ModifiedBy = entity.ModifiedBy ?? "System",
                        Observations = entity.Observations,
                        TypingProcessId = entity.TypingProcessId,
                        TypingStatus = (int)entity.TypingStatus
                    };
                    db.ProductStatusHistory.Add(history);
                    db.SaveChanges();
                }
            }

            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    AppLogger.Logger.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine();
                    foreach (var ve in eve.ValidationErrors)
                    {
                        AppLogger.Logger.ErrorFormat("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Insert: {0}", ex.ToString());
                throw;
            }
        }

        private static object lockObject = new object();
        public string AssignProcessToUserInCapture(int formId, string userName, ProcessStatus fromStatus, ProcessStatus toStatus)
        {
            try
            {
                lock (lockObject)
                {
                    using (var db = new ImaginCrudDataContext())
                    {
                        var typingProcessIds = db.FormData.Where(x => x.ModifiedBy.Equals(userName))
                            .Where(x => x.FormId == formId)
                            .Select(x => x.TypingProcessId)
                            .ToList();
                        var processes = db.TypingProcesss
                            .Where(x => x.FormId == formId)
                            .Where(x => x.TypingStatus == (int)fromStatus)
                            .Where(x => typingProcessIds.Contains(x.TypingProcessId) == false)
                            .OrderBy(x => x.CreatedOn)
                            .ThenBy(x=>x.Priority);
                        var process = processes.FirstOrDefault();
                        if (process == null)
                            return null;
                        var form = db.Forms.Find(formId);
                        form.ProductStatus = (int)ProductStatus.InCapture;
                        process.TypingStatus = (int)toStatus;
                        process.ModifiedBy = userName;
                        process.ModifiedOn = DateTime.Now;
                        var history = new ProductStatusHistory()
                        {
                            FormId = formId,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = userName,
                            Observations = string.Format("Asignado al usuario {0} el {1}", userName, DateTime.Now),
                            TypingProcessId = process.TypingProcessId,
                            TypingStatus = (int)toStatus
                        };
                        db.ProductStatusHistory.Add(history);
                        db.SaveChanges();
                        return process.TypingProcessId;
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    AppLogger.Logger.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine();
                    foreach (var ve in eve.ValidationErrors)
                    {
                        AppLogger.Logger.ErrorFormat("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in AssignProcessToUserInCapture: {0}", ex.ToString());
                throw;
            }
        }

        public void Update(TypingProcess entity)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    db.TypingProcesss.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    AppLogger.Logger.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine();
                    foreach (var ve in eve.ValidationErrors)
                    {
                        AppLogger.Logger.ErrorFormat("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Update: {0}", ex.ToString());
                throw;
            }
        }

        public void ChangeState(TypingProcess process, ProcessStatus status, string observations)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    process.TypingStatus = (int)status;
                    var history = new ProductStatusHistory()
                    {
                        FormId = process.FormId,
                        ModifiedOn = DateTime.Now,
                        ModifiedBy = process.ModifiedBy,
                        Observations = observations,
                        TypingProcessId = process.TypingProcessId,
                        TypingStatus = (int)status
                    };
                    db.ProductStatusHistory.Add(history);
                    db.TypingProcesss.Attach(process);
                    db.Entry(process).State = System.Data.Entity.EntityState.Modified;
                    process.TypingStatus = (int)status;
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    AppLogger.Logger.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine();
                    foreach (var ve in eve.ValidationErrors)
                    {
                        AppLogger.Logger.ErrorFormat("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Insert: {0}", ex.ToString());
                throw;
            }
        }
        #endregion
    }
}
