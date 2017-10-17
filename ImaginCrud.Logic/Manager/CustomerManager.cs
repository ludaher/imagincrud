using ImaginCrud.Entities;
using ImaginCrud.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.DataAccess
{
    public class CustomerManager
    {
        #region Select

        public Customer FindById(int id)
        {
            try
            {
                using (var db = new ImaginCrudDataContext() )
                {
                    return db.Customers.Find(id);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in FindById: {0}", ex.ToString());
                throw;
            }
        }

        public List<Customer> Find(Pagining pagining)
        {
            try
            {
                List<Customer> customers = new List<Customer>();
                using (var db = new ImaginCrudDataContext())
                {
                    if (pagining == null)
                    {
                        customers = db.Customers.AsNoTracking().ToList();
                    }
                    else
                    {
                        customers = db.Customers.AsNoTracking().Skip((pagining.Page - 1) * pagining.ItemsByPage)
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
        public List<Customer> FindWithParameters(Pagining pagining, Customer searchEntity)
        {
            try
            {
                IQueryable<Customer> customers;
                using (var db = new ImaginCrudDataContext())
                {
                    customers = db.Customers.OrderBy(x => x.Name);
                    if (customers.Any())
                        pagining.TotalItems = customers.Count();
                    if (searchEntity != null)
                    {
                        if (string.IsNullOrWhiteSpace(searchEntity.Name) == false)
                            customers = customers.Where(x => x.Name.Contains(searchEntity.Name));
                        if (searchEntity.CustomerId != default(int))
                            customers = customers.Where(x => x.CustomerId == searchEntity.CustomerId);
                    }
                    if (pagining != null)
                    {
                        customers = _ApplyOrder(customers, pagining);
                        customers = customers.Skip((pagining.Page - 1) * pagining.ItemsByPage)
                        .Take(pagining.ItemsByPage);
                    }
                    return customers.ToList();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in FindWithParameters: {0}", ex.ToString());
                throw;
            }
        }

        private IQueryable<Customer> _ApplyOrder(IQueryable<Customer> query, Pagining pagining)
        {
            if (pagining.SortBy == "CustomerId")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.CustomerId) : query.OrderBy(x => x.CustomerId);
            else if (pagining.SortBy == "Phone")
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.Phone) : query.OrderBy(x => x.Phone);
            else
                query = pagining.IsDescendentOrder ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
            return query;
        }

        #endregion

        #region Change data

        public void Insert(Customer entity)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    if (db.Customers.Any(x => x.CustomerId.Equals(entity.CustomerId)))
                        throw new LogicException(string.Format("Ya se encuentra un cliente registrado con la identificación {0}", entity.CustomerId));
                    db.Customers.Add(entity);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.ErrorFormat("Error in Insert: {0}", ex.ToString());
                throw;
            }
        }

        public void Update(Customer entity)
        {
            try
            {
                using (var db = new ImaginCrudDataContext())
                {
                    var customer = db.Customers.Find(entity.CustomerId);
                    if (customer == null)
                        throw new LogicException("No se encontró el elemento que se desea actualizar.");
                    customer.Address = entity.Address;
                    customer.Description = entity.Description;
                    customer.Name = entity.Name;
                    customer.Phone = entity.Phone;
                    customer.ModifiedBy = entity.ModifiedBy;
                    customer.ModifiedOn = entity.ModifiedOn;
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
