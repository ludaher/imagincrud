using ImaginCrud.DataAccess;
using ImaginCrud.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.Logic
{
    public class UserInFormLogic
    {
        private UserInFormManager _Manager;
        private UserInFormManager Manager
        {
            get
            {
                return _Manager ?? (_Manager = new UserInFormManager());
            }
        }
        #region Select
        public List<UserInForm> Find(Pagining pagining)
        {
            return Manager.Find(pagining);
        }

        public List<UserInForm> FindByParameters(Pagining pagining, UserInForm searchEntity)
        {
            return Manager.FindByParameters(pagining, searchEntity);
        }
        

        public bool Any(string userName, int formId, UserFunctions userFunction)
        {
            return Manager.Any(userName, formId, userFunction);
        }

        #endregion

        #region Change data

        public void Insert(UserInForm entity)
        {
            Manager.Insert(entity);

        }

        public void Delete(UserInForm entity)
        {
            Manager.Delete(entity);
        }

        #endregion
    }
}
