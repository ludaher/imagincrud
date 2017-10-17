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
    public class SectionManager
    {
        #region Select

        public Section FindById(int id)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    var section = db.Sections.Find(id);
                    if (section == null) return null;
                    section.Fields = db.Fields.Where(x => x.SectionId.Equals(section.SectionId))
                            .OrderBy(x => x.OrderInForm).ToList();
                    return section;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Find: {0}", ex.ToString());
                throw;
            }
        }
        public List<Section> Find(Pagining pagining)
        {
            try
            {
                List<Section> sections = new List<Section>();
                using (var db = new ImaginCrudDataContext())
                {
                    if (pagining == null)
                    {
                        sections = db.Sections.ToList();
                    }
                    else
                    {
                        sections = db.Sections.Skip((pagining.Page - 1) * pagining.ItemsByPage)
                        .Take(pagining.ItemsByPage).ToList();
                    }
                }
                return sections;
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Find: {0}", ex.ToString());
                throw;
            }
        }
        public List<Section> FindByForm(int formId)
        {
            try
            {
                List<Section> sections = new List<Section>();
                using (var db = new ImaginCrudDataContext())
                {
                    sections = db.Sections.Where(x => x.FormId.Equals(formId)).ToList();
                    foreach (var section in sections)
                    {
                        section.Fields = db.Fields.Where(x => x.SectionId.Equals(section.SectionId))
                            .OrderBy(x => x.OrderInForm).ToList();
                    }
                }
                return sections;
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in FindByForm: {0}", ex.ToString());
                throw;
            }
        }
        #endregion

        #region Change data

        public Section Insert(Section entity)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    int i = 0;
                    entity.Fields.ForEach(x =>
                    {
                        x.FieldName = x.Title.CustomNormalize();
                        x.OrderInForm = i++;
                        x.CreatedBy = entity.CreatedBy;
                        x.CreatedOn = entity.CreatedOn;
                    });
                    db.Sections.Add(entity);
                    db.SaveChanges();
                    _CreateColumns(db, entity);
                }
                return entity;
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

        private void _CreateColumns(ImaginCrudDataContext db, Section entity)
        {
            string query;
            if (entity.IsTable)
            {
                db.Database.ExecuteSqlCommand(entity.GetQueryToCreateTable());
                query = string.Join(" ", entity.Fields.Select(x => x.GetQueryToCreateColumnInSection(entity.FormId)));
            }
            else
                query = string.Join(" ", entity.Fields.Select(x => x.GetQueryToCreateColumnInForm(entity.FormId)));
            db.Database.ExecuteSqlCommand(query);
            db.SaveChanges();
        }

        public Section Update(Section entity)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    int i = 0;
                    entity.Fields.ForEach(x => x.OrderInForm = i++);
                    var fieldsToAdd = entity.Fields.Where(x => x.FieldId == default(int)).ToList();
                    fieldsToAdd.ForEach(x => { x.CreatedBy = entity.ModifiedBy; x.CreatedOn = entity.ModifiedOn.Value; x.FieldName = x.FieldName ?? "default"; });
                    var oldEntity = db.Sections.Find(entity.SectionId);
                    oldEntity.FormId = entity.FormId;
                    oldEntity.Position = entity.Position;
                    oldEntity.SectionName = entity.SectionName;
                    oldEntity.IsTable = entity.IsTable;
                    oldEntity.NumberOfRows = entity.NumberOfRows;
                    oldEntity.Fields = fieldsToAdd;
                    oldEntity.ModifiedBy = entity.ModifiedBy;
                    oldEntity.ModifiedOn = entity.ModifiedOn;
                    db.SaveChanges();
                    var fieldsToUpdate = entity.Fields.Where(x => x.FieldId != default(int)).ToList();
                    foreach (var ftu in fieldsToUpdate)
                    {
                        var currentName = db.Fields.Where(x => x.FieldId.Equals(ftu.FieldId)).Select(x => x.FieldName).FirstOrDefault();
                        ftu.FieldName = ftu.Title.CustomNormalize();
                        if (currentName.Equals(ftu.FieldName) == false)
                        {
                            if (entity.IsTable)
                                db.Database.ExecuteSqlCommand(ftu.GetQueryToUpdateColumnNameInSection(entity.FormId, currentName));
                            else
                                db.Database.ExecuteSqlCommand(ftu.GetQueryToUpdateColumnNameInForm(entity.FormId, currentName));

                        }
                        ftu.ModifiedBy = entity.ModifiedBy;
                        ftu.ModifiedOn = entity.ModifiedOn;
                        db.Fields.Attach(ftu);
                        //((IObjectContextAdapter)db).ObjectContext.ObjectStateManager.ChangeObjectState(ftu, System.Data.Entity.EntityState.Modified);
                        db.Entry(ftu).Property(x => x.CreatedBy).IsModified = false;
                        db.Entry(ftu).Property(x => x.CreatedOn).IsModified = false;
                        db.Entry(ftu).State = System.Data.Entity.EntityState.Modified;
                    }
                    db.SaveChanges();
                    var sectionFields = db.Fields.Where(x => x.SectionId.Equals(entity.SectionId))
                        .ToList();
                    var fieldsToDelete = sectionFields
                        .Where(x => entity.Fields.Select(y => y.FieldId).Contains(x.FieldId) == false);
                    db.Fields.RemoveRange(fieldsToDelete);
                    db.SaveChanges();
                    _CreateColumns(db, entity);
                    return oldEntity;
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

        #endregion
    }

}
