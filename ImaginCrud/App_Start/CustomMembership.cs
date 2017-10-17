using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace ImaginCrud.App_Start
{
    public class CustomMembershipUser : MembershipUser
    {
        public CustomMembershipUser(
            string providerName,
            string name,
            object providerUserKey,
            string email,
            string passwordQuestion,
            string comment,
            bool isApproved,
            bool isLockedOut,
            DateTime creationDate,
            DateTime lastLoginDate,
            DateTime lastActivityDate,
            DateTime lastPasswordChangedDate,
            DateTime lastLockoutDate
            )
            : base(providerName, name, providerUserKey, email, passwordQuestion,
            comment, isApproved, isLockedOut, creationDate, lastLoginDate,
            lastActivityDate, lastPasswordChangedDate, lastLockoutDate)
        {
        }

        // Add additional properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Comments { get; set; }

    }
    
}