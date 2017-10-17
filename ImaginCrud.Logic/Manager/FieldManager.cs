using ImaginCrud.Entities;
using ImaginCrud.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.DataAccess
{
    public class FieldManager
    {
        #region Select

        public List<Field> Find(Pagining pagining)
        {
            try
            {
                List<Field> customers = new List<Field>();
                using (var db = new ImaginCrudDataContext())
                {
                    if (pagining == null)
                    {
                        customers = db.Fields.ToList();
                    }
                    else
                    {
                        customers = db.Fields.Skip((pagining.Page - 1) * pagining.ItemsByPage)
                        .Take(pagining.ItemsByPage).ToList();
                    }
                }
                return customers;
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Find: {0}", ex.ToString());
                throw;
            }
        }

        #endregion

        #region Change data

        public void Insert(Field entity)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    db.Fields.Add(entity);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Insert: {0}", ex.ToString());
                throw;
            }
        }

        public void Update(Field entity)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    var oldEntity = db.Fields.Find(entity.FieldId);
                    oldEntity.DefaultValue = entity.DefaultValue;
                    oldEntity.FieldName = entity.FieldName;
                    oldEntity.FieldTypeId = entity.FieldTypeId;
                    oldEntity.Options = entity.Options;
                    oldEntity.ParentFieldId = entity.ParentFieldId;
                    oldEntity.Required = entity.Required;
                    oldEntity.SectionId = entity.SectionId;
                    oldEntity.Title = entity.Title;
                    oldEntity.Validation = entity.Validation;
                    db.SaveChanges();
                }
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
