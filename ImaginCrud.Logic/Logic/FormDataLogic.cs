using ImaginCrud.DataAccess;
using ImaginCrud.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.Logic
{
    public class FormDataLogic
    {
        private FormDataManager _Manager;
        private FormDataManager Manager
        {
            get
            {
                return _Manager ?? (_Manager = new FormDataManager());
            }
        }

        #region Select

        public FormData FindById(long id)
        {
            return Manager.FindById(id);
        }
        public List<FormData> Find(Pagining pagining)
        {
            return Manager.Find(pagining);
        }

        public List<FormData> FindWithParameters(Pagining pagining, FormData searchEntity)
        {
            return Manager.FindWithParameters(pagining, searchEntity);

        }
        #endregion

        #region Change data

        public FormData Insert(FormData entity)
        {
            return Manager.Insert(entity);

        }

        public FormData Update(FormData entity)
        {
            entity.ModifiedOn = DateTime.Now;
            if (entity.FormDataId == default(long))
                return Insert(entity);
            return Manager.Update(entity);

        }

        #endregion

        public List<UserCaptures> GetFormdatasByUser(string username, DateTime from, DateTime to, int formId)
        {
            return Manager.GetFormdatasByUser(username,from,to, formId);
        }
    }
}
