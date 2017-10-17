using ImaginCrud.DataAccess;
using ImaginCrud.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.Logic
{
    public class FieldDataSourceLogic
    {
        private FieldDataSourceManager _Manager;
        private FieldDataSourceManager Manager
        {
            get
            {
                return _Manager ?? (_Manager = new FieldDataSourceManager());
            }
        }

        public FieldDataSource FindById(int id)
        {
            return Manager.FindById(id);
        }

        public List<FieldDataSource> Find(Pagining pagining)
        {
            return Manager.Find(pagining);
        }

        public List<FieldDataSource> GetAllSources()
        {
            return Manager.GetAllSources();
        }
        public FieldDataSource Insert(FieldDataSource entity)
        {
            return Manager.Insert(entity);
        }
        public FieldDataSource Update(FieldDataSource entity)
        {
            return Manager.Update(entity);
        }
        public void ReplaceSourceDetail(FieldDataSource entity, List<FieldDataSourceDetail> details)
        {
            Manager.ReplaceSourceDetail(entity, details);
        }
    }
}
