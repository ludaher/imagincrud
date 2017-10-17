using ImaginCrud.DataAccess;
using ImaginCrud.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace ImaginCrud.Logic
{
    public class CustomerLogic
    {
        private CustomerManager _Manager;
        private CustomerManager Manager
        {
            get
            {
                return _Manager ?? (_Manager = new CustomerManager());
            }
        }
        public List<Customer> Find(Pagining pagining)
        {
            return Manager.Find(pagining);
        }

        public Customer FindById(int id)
        {
            return Manager.FindById(id);
        }
        public List<Customer> FindWithParameters(Pagining pagining, Customer searchEntity)
        {
            return Manager.FindWithParameters(pagining, searchEntity);
        }

        public void Insert(Customer entity)
        {
            entity.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            entity.CreatedOn = DateTime.Now;
            Manager.Insert(entity);
        }

        public void Update(Customer entity)
        {
            entity.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            entity.ModifiedOn = DateTime.Now;
            Manager.Update(entity);
        }
    }

}
