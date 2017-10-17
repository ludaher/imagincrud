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
    public class TypingProcessLogic
    {
        private TypingProcessManager _Manager;
        private TypingProcessManager Manager
        {
            get
            {
                return _Manager ?? (_Manager = new TypingProcessManager());
            }
        }
        public List<TypingProcess> Find(Pagining pagining)
        {
            return Manager.Find(pagining);
        }

        public TypingProcess FindById(string id, int formId)
        {
            return Manager.FindById(id, formId);
        }
            public List<TypingProcess> FindWithParameters(Pagining pagining, TypingProcess searchEntity)
        {
            return Manager.FindWithParameters(pagining, searchEntity);
        }

        public List<ProductStatusHistory> GetHistory(int formId, string processId)
        {
            return Manager.GetHistory(formId, processId);
        }

        public void Insert(TypingProcess entity)
        {
            entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            entity.CreatedOn = DateTime.Now;
            Manager.Insert(entity);
        }

        public void Update(TypingProcess entity)
        {
            //entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //entity.ModifiedBy = System.Web.HttpContext.Current.User.Identity.Name;
            entity.ModifiedOn = DateTime.Now;
            Manager.Update(entity);
        }

        public string AssignProcessToUserInCapture(int formId, string userName, ProcessStatus fromStatus, ProcessStatus toStatus)
        {
            return Manager.AssignProcessToUserInCapture(formId, userName, fromStatus, toStatus);
        }

        public void ChangeState(TypingProcess process, ProcessStatus status, string observations)
        {
            process.ModifiedOn = DateTime.Now;
            Manager.ChangeState(process, status, observations);
        }
    }

}
