using ImaginCrud.Entities;
using ImaginCrud.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImaginCrud.DataAccess
{
    public class FieldDataSourceManager
    {
        #region Select

        public FieldDataSource FindById(int id)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    var data = db.FieldDataSources.Find(id);
                    if (data != null)
                        data.FieldDataSourceDetails = db.FieldDataSourceDetails
                            .AsNoTracking()
                            .Where(x => x.FieldDataSourceId.Equals(data.FieldDataSourceId))
                            .OrderBy(x => x.Value).ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in FindById: {0}", ex.ToString());
                throw;
            }
        }
        public List<FieldDataSource> Find(Pagining pagining)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    var data = db.FieldDataSources.AsNoTracking().AsQueryable();
                    if (pagining != null)
                    {
                        data = _ApplyOrder(data, pagining);
                        data = data.Skip((pagining.Page - 1) * pagining.ItemsByPage)
                        .Take(pagining.ItemsByPage);
                    }
                    return data.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Find: {0}", ex.ToString());
                throw;
            }
        }

        private IQueryable<FieldDataSource> _ApplyOrder(IQueryable<FieldDataSource> query, Pagining pagining)
        {
            if (pagining.SortBy == "ModifiedOn")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.ModifiedOn) : query.OrderBy(x => x.ModifiedOn);
            else if (pagining.SortBy == "CreatedOn")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn);
            else if (pagining.SortBy == "ModifiedBy")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.ModifiedBy) : query.OrderBy(x => x.ModifiedBy);
            else
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.Description) : query.OrderBy(x => x.Description);
            return query;
        }

        public List<FieldDataSource> GetAllSources()
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    var data = db.FieldDataSources;
                    return data.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in GetAllSources: {0}", ex.ToString());
                throw;
            }
        }

        public List<FieldDataSourceDetail> FindDetailBySourceId(Pagining pagining, FieldDataSourceDetail entityToFind)
        {
            try
            {
                if(entityToFind == null)
                {
                    throw new LogicException("entityToFind is requered to search detail of the source");
                }
                using (var db = new ImaginCrudDataContext())
                {

                        var details= db.FieldDataSourceDetails.AsNoTracking()
                            .Where(x => x.FieldDataSourceId.Equals(entityToFind.FieldDataSourceId));
                    if (string.IsNullOrWhiteSpace(entityToFind.Value))
                        details = details.Where(x => x.Value.Contains(entityToFind.Value) || x.Value.Contains(entityToFind.Value));
                    if (pagining != null)
                    {
                        details = details.Skip((pagining.Page - 1) * pagining.ItemsByPage)
                        .Take(pagining.ItemsByPage);
                    }



                    return details.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in FindById: {0}", ex.ToString());
                throw;
            }
        }
        #endregion

        #region Change data

        public FieldDataSource Insert(FieldDataSource entity)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    db.FieldDataSources.Add(entity);
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

        public FieldDataSource Update(FieldDataSource entity)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    var source = db.FieldDataSources.Find(entity.FieldDataSourceId);
                    source.ModifiedBy = entity.ModifiedBy;
                    source.ModifiedOn = entity.ModifiedOn;
                    source.Description = entity.Description;
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

        public void ReplaceSourceDetail(FieldDataSource entity, List<FieldDataSourceDetail> details)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    var source = db.FieldDataSources.Find(entity.FieldDataSourceId);
                    source.ModifiedBy = entity.ModifiedBy;
                    source.ModifiedOn = entity.ModifiedOn;
                    details.ForEach(x => x.FieldDataSourceId = entity.FieldDataSourceId);
                    db.FieldDataSourceDetails.RemoveRange(db.FieldDataSourceDetails.Where(x => x.FieldDataSourceId == entity.FieldDataSourceId));
                    db.FieldDataSourceDetails.AddRange(details);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in ReplaceSourceDetail: {0}", ex.ToString());
                throw;
            }
        }

        #endregion
    }
}
