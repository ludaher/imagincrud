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
    public class FormLogic
    {
        private FormManager _Manager;
        private FormManager Manager
        {
            get
            {
                return _Manager ?? (_Manager = new FormManager());
            }
        }
        public List<Form> Find(Pagining pagining)
        {
            return Manager.Find(pagining);
        }

        public Form FindById(int id)
        {
            return Manager.FindById(id);
        }
        public List<Form> FindWithParameters(Pagining pagining, Form searchEntity, bool? active = null)
        {
            return Manager.FindWithParameters(pagining, searchEntity, active);
        }

        public Form Insert(Form entity)
        {
            entity.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            entity.CreatedOn = DateTime.Now;
            return Manager.Insert(entity);

        }

        public Form Update(Form entity)
        {
            entity.ModifiedBy = System.Web.HttpContext.Current == null ? System.Environment.UserName : System.Web.HttpContext.Current.User.Identity.Name;
            entity.ModifiedOn = DateTime.Now;
            return Manager.Update(entity);
        }

        public void DeleteExtraSections(List<Section> sections)
        {
            Manager.DeleteExtraSections(sections);
        }
    }
}
