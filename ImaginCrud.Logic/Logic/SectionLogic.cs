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
    public class SectionLogic
    {
        private SectionManager _Manager;
        private SectionManager Manager
        {
            get
            {
                return _Manager ?? (_Manager = new SectionManager());
            }
        }
        public Section FindById(int id)
        {
            return Manager.FindById(id);
        }
        public List<Section> Find(Pagining pagining)
        {
            return Manager.Find(pagining);
        }

        public List<Section> FindByForm(int id)
        {
            return Manager.FindByForm(id);
        }

        public Section Insert(Section entity)
        {
            entity.CreatedBy = System.Web.HttpContext.Current.User.Identity.Name;
            entity.CreatedOn = DateTime.Now;
            return Manager.Insert(entity);
        }

        public Section Update(Section entity)
        {

            entity.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            entity.ModifiedOn = DateTime.Now;
            return Manager.Update(entity);
        }
    }
}
