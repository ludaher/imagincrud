using ImaginCrud.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.DataAccess
{
    public class UserInFormManager
    {
        #region Select
        public List<UserInForm> Find(Pagining pagining)
        {
            List<UserInForm> usersInForms = new List<UserInForm>();
            using (var db = new ImaginCrudDataContext())
            {
                if (pagining == null)
                {
                    usersInForms = db.UsersInForms.ToList();
                }
                else
                {
                    usersInForms = db.UsersInForms.Skip((pagining.Page - 1) * pagining.ItemsByPage)
                    .Take(pagining.ItemsByPage).ToList();
                }
            }
            return usersInForms;
        }
        public List<UserInForm> FindByParameters(Pagining pagining, UserInForm searchEntity)
        {
            using (var db = new ImaginCrudDataContext())
            {
                var usersInForms = db.UsersInForms.Where(x => x.Form.Active == true);
                if (searchEntity != null)
                {
                    if (string.IsNullOrWhiteSpace(searchEntity.UserName) == false)
                        usersInForms = usersInForms.Where(x => x.UserName.Equals(searchEntity.UserName));
                    if (searchEntity.FormId != default(int))
                        usersInForms = usersInForms.Where(x => x.FormId == searchEntity.FormId);
                    if (searchEntity.UserFunction != default(int))
                        usersInForms = usersInForms.Where(x => x.UserFunction == searchEntity.UserFunction);
                }
                if (pagining != null)
                {
                    usersInForms = db.UsersInForms.Skip((pagining.Page - 1) * pagining.ItemsByPage)
                    .Take(pagining.ItemsByPage);
                }
                return usersInForms.ToList();
            }
        }

        public bool Any(string userName, int formId, UserFunctions userFunction)
        {
            List<UserInForm> customers = new List<UserInForm>();
            using (var db = new ImaginCrudDataContext())
            {
                return db.UsersInForms.Where(x => x.FormId == formId)
                    .Where(x => x.UserName.Equals(userName))
                    .Any(x => x.UserFunction.Equals((int)userFunction));
            }
        }

        #endregion

        #region Change data

        public void Insert(UserInForm entity)
        {
            using (var db = new ImaginCrudDataContext())
            {
                db.UsersInForms.Add(entity);
                db.SaveChanges();
            }
        }

        public void Delete(UserInForm entity)
        {
            using (var db = new ImaginCrudDataContext())
            {
                db.UsersInForms.Attach(entity);
                db.UsersInForms.Remove(entity);
                db.SaveChanges();
            }
        }

        #endregion
    }
}
